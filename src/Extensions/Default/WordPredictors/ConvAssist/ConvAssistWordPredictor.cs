////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ConvAssistWordPredictor.cs
//
/// Creates and handles the channel of comunication between ACAT and ConvAssist
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PreferencesManagement;
using ACAT.Lib.Core.UserManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WordPredictionManagement;
using ACAT.Lib.Extension;
using ACAT.ACATResources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.WordPredictors.ConvAssist
{
    /// <summary>
    /// English language word prediction extension.
    /// Uses the ConvAssist word predictor for next word prediction.
    /// </summary>
    [DescriptorAttribute("1505D4A3-26AD-451F-9FD3-44EC92271AF3",
                            "ConvAssist Word Predictor (English)",
                            "The ConvAssist predictive text engine with enhanced language modeling capabilities fine-tuned for AAC uses")]
    public class ConvAssistWordPredictor : ConvAssistWordPredictorBase
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
        private readonly object _syncObj = new object();

        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        private readonly ManualResetEvent mevent = new ManualResetEvent(false);
        private readonly Stack<object> sentenceStack;
        private readonly Stack<object> wpStack;
        private readonly Stack<object> keyWordStack;
        private readonly Stack<object> crgResponsesStack;
        private readonly SentencePredictionsRequestHandler _sentencePredictionsRequestHandler;
        private readonly CRGKeywordPredictionRequestHandler _crgKeywordPredictionRequestHandler;
        private readonly CRGResponsePredictionRequestHandler _crgResponsePredictionRequestHandler;

        /// <summary>
        /// The preferences object
        /// </summary>
        internal static Settings settings;

        private WordPredictionsRequestHandler _wordPredictionsRequestHandler;

        /// <summary>
        /// Named Pipe object
        /// </summary>
        private NamedPipeServerConvAssist namedPipe;

        private bool pipeCreated;
        private Task wordPredictionTask;

        /// <summary>
        /// Initializes and instance of the class
        /// </summary>
        public ConvAssistWordPredictor()
        {
            Settings.PreferencesFilePath = getUserRelativePath("en", SettingsFileName, true);

            settings = Settings.Load();

            convAssistSettings = settings;

            _wordPredictionsRequestHandler = new WordPredictionsRequestHandler(this);
            _sentencePredictionsRequestHandler = new SentencePredictionsRequestHandler(this);
            _crgKeywordPredictionRequestHandler = new CRGKeywordPredictionRequestHandler(this);
            _crgResponsePredictionRequestHandler = new CRGResponsePredictionRequestHandler(this); 

            wpStack = new Stack<object>();
            sentenceStack = new Stack<object>();
            keyWordStack = new Stack<object>();
            crgResponsesStack = new Stack<object>();

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
        public override bool Init(CultureInfo ci)
        {
            Attributions.Add("CONVASSIST",
                                "The ConvAssist predictive text functionality is derived from Pressagio, the " +
                                "intelligent predictive text and characters. ");

            Disclaimers.Add("ConvAssist", R.GetString("DisclaimerConvAssist"));

            string filesPath = Path.Combine(UserManager.CurrentUserDir, CultureInfo.DefaultThreadCurrentUICulture.Name, "WordPredictors", "ConvAssist", "Settings");
            namedPipe = new NamedPipeServerConvAssist(PipeName, PipeDirection.InOut, filesPath);
            pipeCreated = namedPipe.CreatePipeServer();
            if (!pipeCreated)
                pipeCreated = namedPipe.CreatePipeServer();

            // Initial validation, the client.exe running
            Process[] ConvAssistExeProcess = Process.GetProcessesByName(ConvAssistName);
            // Number  of processes
            // 2 processes of ConvAssist.exe indicates that ConvAssist is running correctly.
            // 0 ConvAssist is not running
            switch (ConvAssistExeProcess.Length)
            {
                case 0:
                    string filePath = Path.Combine(FileUtils.ACATPath, ConvAssistAppFolder, ConvAssistAppName);
                    if (File.Exists(filePath))
                    {
                        //Process.Start(filePath);
                        var processStartInfo = new ProcessStartInfo
                        {
                            FileName = filePath,
                            WorkingDirectory = Path.GetDirectoryName(filePath)
                        };

                        Process.Start(processStartInfo);
                        int waitcounter = 0;
                        //Wait for the .exe to fully load
                        while (!namedPipe.clientConected)
                        {
                            if (waitcounter > settings.ConvAssistExeLoadWaitTime)
                                break;
                            Thread.Sleep(1000);
                            waitcounter += 1;
                        }
                    }
                    break;
            }
            int counterToQuit = 0;
            while (!namedPipe.clientAnswerParameters)
            {
                counterToQuit += 1;
                Thread.Sleep(1000);
                if (counterToQuit > settings.ConvAssistModuleLoadWaitTime)
                    break;
            }

            wordPredictionTask = Task.Factory.StartNew(WordPredictionTaskProcess, TaskCreationOptions.LongRunning);

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
            if (req.PredictionType == PredictionTypes.Words)
            {
                wpStack.Push(req);
                mevent.Set();
            }
            else if (req.PredictionType == PredictionTypes.Sentences)
            {
                sentenceStack.Push(req);
                mevent.Set();
            }
            else if (req.PredictionType == PredictionTypes.Keywords)
            {
                keyWordStack.Push(req);
                mevent.Set();
            }
            else if (req.PredictionType == PredictionTypes.CRGResponses)
            {
                crgResponsesStack.Push(req);
                mevent.Set();
            }
            else
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Send a request message Syncronously
        /// </summary>
        /// <param name="text">Text to send to the client</param>
        /// <returns>Sentences predictions</returns>
        public string ConvAssistLearn(string text, WordPredictorMessageTypes requestType)
        {
            ConvAssistMessage message = new ConvAssistMessage(requestType, WordPredictionModes.None, text);
            string jsonMessage = JsonConvert.SerializeObject(message);
            //var answer = namedPipe.WriteSync(text, 150);
            return namedPipe.WriteSync(jsonMessage, 10000);
        }

        /// <summary>
        /// Send a request message Syncronously
        /// </summary>
        /// <param name="text">Text to send to the client</param>
        /// <returns>Sentences predictions</returns>
        public string SendMessageConvAssistSentencePrediction(string text, WordPredictionModes mode)
        {
            ConvAssistMessage message = new ConvAssistMessage(WordPredictorMessageTypes.NextSentencePredictionRequest, mode, text);
            string jsonMessage = JsonConvert.SerializeObject(message);

            return namedPipe.WriteSync(jsonMessage, 10000);
        }

        /// <summary>
        /// Send a request message Syncronously
        /// </summary>
        /// <param name="text">Text to send to the client</param>
        /// <returns>Words and letters predictions</returns>
        public string SendMessageConvAssistWordPrediction(string text, WordPredictionModes mode)
        {
            ConvAssistMessage message = new ConvAssistMessage(WordPredictorMessageTypes.NextWordPredictionRequest, mode, text);
            string jsonMessage = JsonConvert.SerializeObject(message);

            return namedPipe.WriteSync(jsonMessage, 10000);
        }

        /// <summary>
        /// Send a request message syncronously for keyword prediction
        /// </summary>
        /// <param name="text">Text to send to the client</param>
        /// <returns>CRG Keywords</returns>
        public string SendMessageConvAssistCRGKeywordPrediction(string text, WordPredictionModes mode)
        {
            ConvAssistMessage message = new ConvAssistMessage(WordPredictorMessageTypes.CRGKeywordPredictionRequest, mode, text);
            string jsonMessage = JsonConvert.SerializeObject(message);
            
            return namedPipe.WriteSync(jsonMessage, 10000);
        }

        /// <summary>
        /// Send a request message syncronously for CRG response prediction
        /// </summary>
        /// <param name="text">Text to send to the client</param>
        /// <returns>CRG Keywords</returns>
        public string SendMessageConvAssistCRGResponsePrediction(string text, String keyword, WordPredictionModes mode)
        {
            if (keyword == null)
            {
                keyword = string.Empty;
            }

            ConvAssistMessage message = new ConvAssistMessage(WordPredictorMessageTypes.CRGSentencePredictionRequest, mode, text, true, keyword);
            string jsonMessage = JsonConvert.SerializeObject(message);

            return namedPipe.WriteSync(jsonMessage, 10000);
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
                Log.Exception(ex);
            }

            return result;
        }

        private void Close(bool waitOnCompletion = true)
        {
            wpStack.Push(null);
            sentenceStack.Push(null);
            mevent.Set();

            if (waitOnCompletion)
            {
                Task.WaitAll(wordPredictionTask);
            }
        }

        private void EnterCriticalSection(object syncObj)
        {
            while (!TryEnter(_syncObj))
            {
                if (Application.MessageLoop)
                {
                    Application.DoEvents();
                }
            }
        }

        private void ExitCriticalSection(object syncObj)
        {
            Monitor.Exit(syncObj);
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
        private WordPredictionResponse ProcessPredictionRequest(WordPredictionRequest request)
        {
            Log.Debug("Predict for: " + request.Context);
            WordPredictionResponse response = null;

            try
            {
                EnterCriticalSection(_syncObj);

                if (!namedPipe.clientConected)
                {
                    return new WordPredictionResponse(request, new List<String>(), false);
                }

                switch(request.PredictionType)
                {
                    case PredictionTypes.Words:
                        response = _wordPredictionsRequestHandler.ProcessPredictionRequest(request);
                        break;

                    case PredictionTypes.Sentences:
                        response = _sentencePredictionsRequestHandler.ProcessPredictionRequest(request);
                        break;

                    case PredictionTypes.Keywords:
                        response = _crgKeywordPredictionRequestHandler.ProcessPredictionRequest(request);
                        break;

                    case PredictionTypes.CRGResponses:
                        response = _crgResponsePredictionRequestHandler.ProcessPredictionRequest(request);
                        break;  
                }
                
                
            }
            catch (Exception ex)
            {
                Log.Debug("ConvAssist Exception " + ex);
                response = new WordPredictionResponse(request, new List<String>(), false);
            }
            finally
            {
                ExitCriticalSection(_syncObj);
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

        private void WordPredictionTaskProcess()
        {
            while (true)
            {
                WordPredictionRequest item = null;
                WordPredictionResponse response;

                if (wpStack.Count == 0 && 
                    sentenceStack.Count == 0 &&
                    keyWordStack.Count == 0 && 
                    crgResponsesStack.Count == 0)
                {
                    mevent.WaitOne();
                }

                while (wpStack.Count > 0)
                {
                    lock (_syncObj)
                    {
                        if (wpStack.Count > 0)
                        {
                            item = wpStack.Pop() as WordPredictionRequest;
                            if (item == null)
                            {
                                mevent.Reset();
                                return;
                            }

                            wpStack.Clear();
                        }
                    }

                    response = ProcessPredictionRequest(item);

                    notifyPredictionResults(response);
                }

                lock (_syncObj)
                {
                    if (sentenceStack.Count > 0)
                    {
                        item = sentenceStack.Pop() as WordPredictionRequest;
                        if (item == null)
                        {
                            mevent.Reset();
                            return;
                        }
                        sentenceStack.Clear();
                    }
                }

                if (item != null)
                {
                    response = ProcessPredictionRequest(item);

                    notifyPredictionResults(response);
                }

                lock (_syncObj)
                {
                    if (keyWordStack.Count > 0)
                    {
                        item = keyWordStack.Pop() as WordPredictionRequest;
                        if (item == null)
                        {
                            mevent.Reset();
                            return;
                        }
                        keyWordStack.Clear();
                    }
                }

                if (item != null)
                {
                    response = ProcessPredictionRequest(item);

                    notifyPredictionResults(response);
                }

                lock (_syncObj)
                {
                    if (crgResponsesStack.Count > 0)
                    {
                        item = crgResponsesStack.Pop() as WordPredictionRequest;
                        if (item == null)
                        {
                            mevent.Reset();
                            return;
                        }
                        crgResponsesStack.Clear();
                    }
                }

                if (item != null)
                {
                    response = ProcessPredictionRequest(item);

                    notifyPredictionResults(response);
                }

                if (item == null)
                {
                    mevent.Reset();
                    return;
                }

                mevent.Reset();
            }
        }

        private void showDisclaimer()
        {
            if (!settings.ShowDisclaimerOnStartup)
            {
                return;
            }

            if (DisclaimerDialog.ShowDialog(R.GetString("DisclaimerConvAssist"), null, true))
            {
                settings.ShowDisclaimerOnStartup = false;
                settings.Save();
            }
        }
    }
}