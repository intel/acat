////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ConvAssistWordPredictor.cs
//
/// Creates and handles the channel of communication between ACAT and ConvAssist
//
////////////////////////////////////////////////////////////////////////////
//#define DEBUG_CONVASSIST

using ACAT.Core.PanelManagement;
using ACAT.Core.PreferencesManagement;
using ACAT.Core.PreferencesManagement.Interfaces;
using ACAT.Core.UserManagement;
using ACAT.Core.Utility;
using ACAT.Core.WordPredictorManagement;
using ACAT.Core.WordPredictorManagement.Interfaces;
using ACAT.Extensions.WordPredictors.ConvAssist.MessageTypes;
using ACATResources;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace ACAT.Extensions.WordPredictors.ConvAssist
{
    /// <summary>
    /// English language word prediction extension.
    /// Uses the ConvAssist word predictor for next word prediction.
    /// </summary>
    [ClassDescriptor("1505D4A3-26AD-451F-9FD3-44EC92271AF3",
                            "ConvAssist Word Predictor (English)",
                            "The ConvAssist predictive text engine with enhanced language modeling capabilities fine-tuned for AAC uses")]
    public class ConvAssistWordPredictor : ConvAssistWordPredictorBase, IAsyncInitializer<CultureInfo>, IAsyncWordPredictorLearning
    {
        /// <summary>
        /// Name of the named pipe to communicate with ConvAssist
        /// </summary>
        public String PipeName = "ACATConvAssistPipe";

        /// <summary>
        /// Name of the ConvAssist exe
        /// </summary>
        private const String ConvAssistAppName = ConvAssistName + ".exe";

        /// <summary>
        /// Name of ConvAssist App
        /// </summary>
        private const String ConvAssistName = "ConvAssist";

        /// <summary>
        /// Name of the preferences file
        /// </summary>
        private const String SettingsFileName = "ConvAssistWordPredictorSettings.xml";

        private const String ConvAssistAppFolder = "ConvAssistApp";

        /// <summary>
        /// Used the synchronization for multiple calls
        /// </summary>
        private readonly object _syncObj = new();

        private readonly CancellationTokenSource cts = new();
        private readonly SemaphoreSlim predictionSignal = new(0);
        private readonly Stack<object> sentenceStack;
        private readonly Stack<object> wpStack;
        private readonly SentencePredictionsRequestHandler _sentencePredictionsRequestHandler;

        /// <summary>
        /// The preferences object
        /// </summary>
        internal static Settings settings;

        private readonly ILogger<ConvAssistWordPredictor> _logger;

        private readonly WordPredictionsRequestHandler _wordPredictionsRequestHandler;

        /// <summary>
        /// Named Pipe object
        /// </summary>
        private NamedPipeServerConvAssist namedPipe;

        private bool pipeCreated;
        private bool preferencesLoaded;
        private Task wordPredictionTask;

        /// <summary>
        /// Initializes an instance of the class with default logger
        /// </summary>
        public ConvAssistWordPredictor() : this(Context.ServiceProvider?.GetService(typeof(ILogger<ConvAssistWordPredictor>)) as ILogger<ConvAssistWordPredictor> 
            ?? LoggingConfiguration.CreateLogger<ConvAssistWordPredictor>())
        {
        }

        /// <summary>
        /// Initializes and instance of the class
        /// </summary>
        public ConvAssistWordPredictor(ILogger<ConvAssistWordPredictor> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Settings.PreferencesFilePath = getUserRelativePath(CultureInfo.CurrentCulture.TwoLetterISOLanguageName, SettingsFileName, true);

            settings = new Settings();

            convAssistSettings = settings;

            _wordPredictionsRequestHandler = new WordPredictionsRequestHandler(this, null);
            _sentencePredictionsRequestHandler = new SentencePredictionsRequestHandler(this, null);

            wpStack = new Stack<object>();
            sentenceStack = new Stack<object>();

            cts.Token.Register(() => Close(false));
        }

        public override bool SupportsPredictSync => false;

        public override void Dispose()
        {
            try
            {
                cts.Cancel();
                base.Dispose();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Returns the default preferences object for the word predictor
        /// </summary>
        /// <returns>default preferences object</returns>
        public override IPreferences GetDefaultPreferences()
        {
            return PreferencesBase.LoadDefaults<Settings>();
        }

        /// <summary>
        /// Returns the preferences object for the word predictor
        /// </summary>
        /// <returns>preferences object</returns>
        public override IPreferences GetPreferences()
        {
            if (!preferencesLoaded)
            {
                settings = Settings.Load();
                convAssistSettings = settings;
                preferencesLoaded = true;
            }
            return settings;
        }

        /// <summary>
        /// Performs initialization.  Must be called first.
        /// Initializes Named Pipe server.
        /// Open client from .exe
        /// waits for comunication to be established
        /// </summary>
        /// <param name="ci">language for word prediction</param>
        /// <returns>true on success</returns>
        ///

        public override bool Init(CultureInfo ci)
        {
            settings = Settings.Load();
            convAssistSettings = settings;
            preferencesLoaded = true;
            if (settings == null)
            {
                return false;
            }

            Attributions.Add("CONVASSIST",
                    "The ConvAssist predictive text functionality is derived from Pressagio, the " +
                    "intelligent predictive text and characters. ");

            Disclaimers.Add("ConvAssist", StringResources.DisclaimerConvAssist);

            // Start the ConvAssist Process
            bool send_params = true;

            // TODO: Either fix ConvAssist to handle getting params after already setup
            // or add check to see if ConvAssist is already configured and don't
            // send params if it is

#if !DEBUG_CONVASSIST
            //NOTE: When debugging ConvAssist, it will be running as a python process, not as an exe
            //So, we don't need to start it here
            string path = Path.Combine(FileUtils.ACATPath, ConvAssistAppFolder, ConvAssistAppName);

            _logger.LogInformation("ConvAssist path: {Path}", path);

            Process[] runningProcesses = Process.GetProcessesByName(ConvAssistName);
            if (runningProcesses.Length == 0)
            {
                ProcessStartInfo convAssistInfo = new()
                {
                    FileName = path,
                    WorkingDirectory = Path.GetDirectoryName(path),
                    Arguments = "",
                    UseShellExecute = true,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    CreateNoWindow = false
                };

                using Process convAssist = Process.Start(convAssistInfo);
                while (!convAssist.Responding)
                {
                    Thread.Sleep(100);
                }
            }
#endif
            _logger.LogInformation("ConvAssist process started. Starting Named Pipe.");
            // Now start the named pipe server and wait for the client to connect
            string convAssistSettings = Path.Combine(UserManager.CurrentUserDir, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, "WordPredictors", "ConvAssist", "Settings");

            namedPipe = new NamedPipeServerConvAssist(PipeName, PipeDirection.InOut, convAssistSettings, LoggingConfiguration.CreateLogger<NamedPipeServerConvAssist>());
            pipeCreated = namedPipe.CreatePipeServer(send_params);

            if (pipeCreated)
            {
                wordPredictionTask = WordPredictionTaskProcessAsync();
            }

            return pipeCreated;
        }

        Task<bool> IAsyncInitializer<CultureInfo>.InitAsync(CultureInfo ci)
        {
            return InitAsync(ci);
        }

        /// <summary>
        /// Asynchronously initialises the ConvAssist named pipe server.
        /// Uses <see cref="NamedPipeServerConvAssist.CreatePipeServerAsync"/> to avoid
        /// blocking the calling thread with <c>.Result</c>.
        /// </summary>
        /// <param name="ci">Language for word prediction.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns><c>true</c> on success.</returns>
        public async Task<bool> InitAsync(CultureInfo ci, CancellationToken cancellationToken = default)
        {
            settings = await Settings.LoadAsync(cancellationToken).ConfigureAwait(false);
            convAssistSettings = settings;
            preferencesLoaded = true;
            if (settings == null)
            {
                return false;
            }

            Attributions.Add("CONVASSIST",
                    "The ConvAssist predictive text functionality is derived from Pressagio, the " +
                    "intelligent predictive text and characters. ");

            Disclaimers.Add("ConvAssist", StringResources.DisclaimerConvAssist);

            bool send_params = true;

#if !DEBUG_CONVASSIST
            string path = Path.Combine(FileUtils.ACATPath, ConvAssistAppFolder, ConvAssistAppName);
            _logger.LogInformation("ConvAssist path: {Path}", path);

            Process[] runningProcesses = Process.GetProcessesByName(ConvAssistName);
            if (runningProcesses.Length == 0)
            {
                ProcessStartInfo convAssistInfo = new()
                {
                    FileName = path,
                    WorkingDirectory = Path.GetDirectoryName(path),
                    Arguments = "",
                    UseShellExecute = true,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    CreateNoWindow = false
                };

                using Process convAssist = Process.Start(convAssistInfo);
                while (!convAssist.Responding)
                {
                    await Task.Delay(100, cancellationToken).ConfigureAwait(false);
                }
            }
#endif
            _logger.LogInformation("ConvAssist process started. Starting Named Pipe.");
            string convAssistSettings = Path.Combine(UserManager.CurrentUserDir, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, "WordPredictors", "ConvAssist", "Settings");

            namedPipe = new NamedPipeServerConvAssist(PipeName, PipeDirection.InOut, convAssistSettings, LoggingConfiguration.CreateLogger<NamedPipeServerConvAssist>());
            pipeCreated = await namedPipe.CreatePipeServerAsync(send_params, cancellationToken).ConfigureAwait(false);

            if (pipeCreated)
            {
                wordPredictionTask = WordPredictionTaskProcessAsync();
            }

            return pipeCreated;
        }

        /// <summary>
        /// Display disclaimer dialog
        /// </summary>
        /// <returns>true</returns>
        public override bool PostInit()
        {
            showDisclaimer();

            return true;
        }

        public override WordPredictionResponse Predict(WordPredictionRequest req)
        {
            return new WordPredictionResponse(req, null, false);
        }

        public override bool PredictAsync(WordPredictionRequest req)
        {
            _logger.LogInformation(">>> PredictAsync called - Type: {Type}, Mode: {Mode}, PrevWords: '{PrevWords}', CurrentWord: '{CurrentWord}'",
                req.PredictionType, req.WordPredictionMode, req.PrevWords ?? "", req.CurrentWord ?? "");

            lock (_syncObj)
            {
                if (cts.IsCancellationRequested)
                {
                    return false;
                }
                if (req.PredictionType == PredictionTypes.Words)
                {
                    wpStack.Push(req);
                }
                else if (req.PredictionType == PredictionTypes.Sentences)
                {
                    sentenceStack.Push(req);
                }
                else
                {
                    return false;
                }
            }
            predictionSignal.Release();

            return true;
        }

        /// <summary>
        /// Send a request message Synchronously
        /// </summary>
        /// <param name="text">Text to send to the client</param>
        /// <returns>Sentences predictions</returns>
        public string ConvAssistLearn(string text, WordPredictorMessageTypes requestType)
        {
            ConvAssistMessage message = new(requestType, WordPredictionModes.None, text);
            string jsonMessage = JsonSerializer.SerializeForInterop(message);
            //var answer = namedPipe.WriteSync(text, 150);
            return namedPipe.WriteSync(jsonMessage, 10000);
        }

        /// <summary>
        /// Asynchronously sends a learn request using <see cref="NamedPipeServerConvAssist.WriteAsync"/>
        /// instead of the blocking <c>WriteSync</c>.
        /// </summary>
        public Task<string> ConvAssistLearnAsync(string text, WordPredictorMessageTypes requestType)
        {
            ConvAssistMessage message = new(requestType, WordPredictionModes.None, text);
            string jsonMessage = JsonSerializer.SerializeForInterop(message);
            return namedPipe.WriteAsync(jsonMessage, 10000);
        }

        /// <summary>
        /// Send a request message Synchronously
        /// </summary>
        /// <param name="text">Text to send to the client</param>
        /// <returns>Sentences predictions</returns>
        public string SendMessageConvAssistSentencePrediction(string text, WordPredictionModes mode)
        {
            ConvAssistMessage message = new(WordPredictorMessageTypes.NextSentencePredictionRequest, mode, text);
            string jsonMessage = JsonSerializer.SerializeForInterop(message);
            //var answer = namedPipe.WriteSync(text, 150);
            return namedPipe.WriteSync(jsonMessage, 10000);
        }

        /// <summary>
        /// Asynchronously sends a sentence prediction request using <see cref="NamedPipeServerConvAssist.WriteAsync"/>
        /// instead of the blocking <c>WriteSync</c>.
        /// </summary>
        public Task<string> SendMessageConvAssistSentencePredictionAsync(string text, WordPredictionModes mode)
        {
            ConvAssistMessage message = new(WordPredictorMessageTypes.NextSentencePredictionRequest, mode, text);
            string jsonMessage = JsonSerializer.SerializeForInterop(message);
            return namedPipe.WriteAsync(jsonMessage, 10000);
        }

        /// <summary>
        /// Send a request message Syncronously
        /// </summary>
        /// <param name="text">Text to send to the client</param>
        /// <returns>Words and letters predictions</returns>
        public string SendMessageConvAssistWordPrediction(string text, WordPredictionModes mode)
        {
            ConvAssistMessage message = new(WordPredictorMessageTypes.NextWordPredictionRequest, mode, text);
            string jsonMessage = JsonSerializer.SerializeForInterop(message);
            //var answer = namedPipe.WriteSync(text, 150);
            return namedPipe.WriteSync(jsonMessage, 10000);
        }

        /// <summary>
        /// Asynchronously sends a word prediction request using <see cref="NamedPipeServerConvAssist.WriteAsync"/>
        /// instead of the blocking <c>WriteSync</c>.
        /// </summary>
        public Task<string> SendMessageConvAssistWordPredictionAsync(string text, WordPredictionModes mode)
        {
            ConvAssistMessage message = new(WordPredictorMessageTypes.NextWordPredictionRequest, mode, text);
            string jsonMessage = JsonSerializer.SerializeForInterop(message);
            return namedPipe.WriteAsync(jsonMessage, 10000);
        }

        /// <summary>
        /// Adds the specified text to the user's personal
        /// word prediction model to learn the user's writing
        /// style.  This makes word prediciton more relevant.
        /// </summary>
        /// <param name="text">Text to add</param>
        /// <returns>true on success</returns>
        protected override bool learn(String text, WordPredictorMessageTypes requestType)
        {
            bool result = false;

            try
            {
                var res = ConvAssistLearn(text, requestType);
                if (res.Length > 1)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in Init");
            }

            return result;
        }

        public async Task<bool> LearnAsync(string text, WordPredictorMessageTypes requestType)
        {
            if (!ACAT.Extension.Common.AppPreferences.WordPredictionEnableLearn)
            {
                return true;
            }
            try
            {
                var response = await ConvAssistLearnAsync(text, requestType).ConfigureAwait(false);
                return response.Length > 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in LearnAsync");
                return false;
            }
        }

        private void Close(bool waitOnCompletion = true)
        {
            lock (_syncObj)
            {
                wpStack.Push(null);
                sentenceStack.Push(null);
            }
            predictionSignal.Release();

            if (waitOnCompletion)
            {
                if (wordPredictionTask != null)
                {
                    Task.WaitAll(wordPredictionTask);
                }
            }
        }

        /// <summary>
        /// Returns a list of next word predictions based on the context
        /// from the previous words in the sentence.  The number of words
        /// returned is set by the PredictionWordCount propertys
        /// </summary>
        /// <param name="prevWords">Previous words in the sentence</param>
        /// <param name="currentWord">current word (may be partially spelt out</param>
        /// <param name="success">true if the function was successsful</param>
        /// <returns>A list of predicted words</returns>
        private async Task<WordPredictionResponse> ProcessPredictionRequestAsync(WordPredictionRequest request)
        {
            _logger.LogDebug("Predict for: {PrevWords} {CurrentWord}", request.PrevWords, request.CurrentWord);
            WordPredictionResponse response = null;

            try
            {
                if (!namedPipe.clientConected)
                {
                    return new WordPredictionResponse(request, new List<String>(), false);
                }
                if (request.PredictionType == PredictionTypes.Words)
                {
                    response = await _wordPredictionsRequestHandler.ProcessPredictionRequestAsync(request).ConfigureAwait(false);
                }
                else if (request.PredictionType == PredictionTypes.Sentences)
                {
                    response = await _sentencePredictionsRequestHandler.ProcessPredictionRequestAsync(request).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ConvAssist Exception {Exception}", ex);
                response = new WordPredictionResponse(request, new List<String>(), false);
            }

            return response;
        }

        /// <summary>
        /// Tries to enter a critical section
        /// </summary>
        /// <param name="syncObj">Object used to synchronoze</param>
        /// <returns>true if entered successfully</returns>
        private bool TryEnter(Object syncObj)
        {
            bool lockTaken = false;
            Monitor.TryEnter(syncObj, ref lockTaken);
            return lockTaken;
        }

        private async Task WordPredictionTaskProcessAsync()
        {
            while (true)
            {
                await predictionSignal.WaitAsync().ConfigureAwait(false);
                while (true)
                {
                    WordPredictionRequest item;
                    lock (_syncObj)
                    {
                        if (wpStack.Count > 0)
                        {
                            item = wpStack.Pop() as WordPredictionRequest;
                            wpStack.Clear();
                        }
                        else if (sentenceStack.Count > 0)
                        {
                            item = sentenceStack.Pop() as WordPredictionRequest;
                            sentenceStack.Clear();
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (item == null)
                    {
                        return;
                    }
                    var response = await ProcessPredictionRequestAsync(item).ConfigureAwait(false);
                    notifyPredictionResults(response);
                }
            }
        }

        private void showDisclaimer()
        {
            if (!settings.ShowDisclaimerOnStartup)
            {
                return;
            }

            //TODO: Move this out of the word predictor code and into UI
            //if (DisclaimerDialog.ShowDialog(Resources.DisclaimerConvAssist, null, true))
            //{
            //    settings.ShowDisclaimerOnStartup = false;
            //    settings.Save();
            //}
        }
    }
}