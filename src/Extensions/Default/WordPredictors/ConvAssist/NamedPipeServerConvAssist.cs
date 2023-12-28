////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// NamedPipeServerConvAssist.cs.
//
// Manages and creates a Named Pipe service to connect with the
// word predictor (ConvAssist)
// Handles states and messages comming from the other side connected
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.UserManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.Utility.NamedPipe;
using ACAT.Lib.Core.WordPredictionManagement;
using ACAT.Lib.Extension;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.WordPredictors.ConvAssist
{
    /// <summary>
    /// Type os the events being tracked
    /// </summary>
    public enum EventTypes
    {
        NONE,
        BEGIN,
        END,
        EVENT,
        CRG
    }

    public class NamedPipeServerConvAssist : IDisposable
    {
        /// <summary>
        /// Result from the parameters request
        /// </summary>
        public volatile bool clientAnswerParameters;

        /// <summary>
        /// State of the current client conection
        /// </summary>
        public volatile bool clientConected;

        /// <summary>
        /// How many clients are currently connected?
        /// </summary>
        public int NumClientsConnected;

        /// <summary>
        /// Used the synchronization for animation transition
        /// </summary>
        private readonly object _syncObj = new object();

        /// <summary>
        /// Path to the INI files
        /// </summary>
        private string _pathToFiles = string.Empty;

        /// <summary>
        /// Cancelation object to skip task
        /// </summary>
        private CancellationToken cancellationToken;

        private CancellationTokenSource cancellationTokenSource;
        private bool disposed;

        /// <summary>
        /// Direction of comunication
        /// </summary>
        private PipeDirection PipeDirection;

        /// <summary>
        /// Given Pipe name to be conected
        /// </summary>
        private string PipeName;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pipeName">Name of the pipe</param>
        /// <param name="pipeDirection">What direction?</param>
        public NamedPipeServerConvAssist(string pipeName, PipeDirection pipeDirection, string path)
        {
            this.PipeName = pipeName;
            this.PipeDirection = pipeDirection;
            this._pathToFiles = path;
        }

        /// <summary>
        /// Event when a connection was stablished
        /// </summary>
        public event EventHandler EvtClientConnected;

        public event EventHandler EvtClientDisconnected;

        /// <summary>
        /// Triggered when a message is received
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public string messageReceived { get; set; }

        /// <summary>
        /// Gets the named-pipe server stream.
        /// </summary>
        public NamedPipeServerStream NamedPipeServerStream { get; private set; }

        public bool TaskFinished { get; private set; }

        /// <summary>
        /// Closes the pipe server
        /// </summary>
        /// <returns></returns>
        public bool ClosePipeServer()
        {
            try
            {
                if (NamedPipeServerStream != null && cancellationTokenSource != null)
                {
                    Stop();
                    Dispose();
                }
            }
            catch (Exception ex)
            {
                Log.Debug("ConvAssist ClosePipe Error:" + ex.Message);
                return false;
            }
            clientConected = false;
            clientAnswerParameters = false;
            return true;
        }

        public bool CreatePipeServer()
        {
            disposed = false;
            cancellationTokenSource = new CancellationTokenSource();
            NamedPipeServerStream = new NamedPipeServerStream(PipeName, PipeDirection,
                                            1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
            try
            {
                Start();
            }
            catch (Exception ex)
            {
                Log.Debug("Exception in createPipeServer: " + ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Disposes the pipe server.
        /// </summary>
        public void Dispose()
        {
            if (!disposed)
            {
                cancellationTokenSource.Dispose();
                NamedPipeServerStream.Dispose();
                NamedPipeServerStream = null;
                cancellationTokenSource = null;
                disposed = true;
            }
        }

        /// <summary>
        /// Notify the pipe client that the app is starting/shutting down
        /// </summary>
        /// <param name="eventType">Type of the event </param>
        /// <returns>true on success</returns>
        public bool NotifyAppEvent(EventTypes eventType)
        {
            if (NumClientsConnected <= 0)
            {
                return false;
            }
            String data = String.Format("{{\"Target\" : \"APP\", \"Event\": \"{0}\"}}", eventType.ToString());
            try
            {
                int len = data.Length;
                byte[] intBytes = BitConverter.GetBytes(len);
                Write(data);
            }
            catch (Exception ex)
            {
                Log.Debug("ConvAssist AppEvent: " + ex.Message);
            }
            return true;
        }

        public async Task SendParams()
        {
            ///NOT IMPLEMENTED YET NEED EXE RELEASE OF CONVASSIST
            await Task.Delay(25);
            try
            {
                ConvAssistSetParam paramEnableLog = new ConvAssistSetParam(ConvAssistSetParam.ConvAssistParameterType.EnableLog, Common.AppPreferences.EnableLogs.ToString());
                string paramEnableLogstring = JsonConvert.SerializeObject(paramEnableLog);
                ConvAssistMessage messageparamEnableLog = new ConvAssistMessage(WordPredictorMessageTypes.SetParam, WordPredictionModes.None, paramEnableLogstring);
                string jsonMessageparamEnableLog = JsonConvert.SerializeObject(messageparamEnableLog);
                var resultEnableLog = await WriteAsync(jsonMessageparamEnableLog, 150);
            }
            catch (Exception ex)
            {
                Log.Debug("Error in param enable log: " + ex);
            }

            await Task.Delay(25);
            try
            {
                ConvAssistSetParam parampathLog = new ConvAssistSetParam(ConvAssistSetParam.ConvAssistParameterType.PathLog, FileUtils.GetLogsDir());
                string parampathLogstring = JsonConvert.SerializeObject(parampathLog);
                ConvAssistMessage messageparampathLog = new ConvAssistMessage(WordPredictorMessageTypes.SetParam, WordPredictionModes.None, parampathLogstring);
                string jsonMessageparampathLog = JsonConvert.SerializeObject(messageparampathLog);
                var resultpathLog = await WriteAsync(jsonMessageparampathLog, 150);
            }
            catch (Exception ex)
            {
                Log.Debug("Error in param Log path: " + ex);
            }

            await Task.Delay(25);
            try
            {
                //SUGGESTIONS FOR WORD PREDCITONS PARAMETER
                ConvAssistSetParam paramSuggestions = new ConvAssistSetParam(ConvAssistSetParam.ConvAssistParameterType.Suggestions, Common.AppPreferences.WordsSuggestions.ToString());
                string paramSuggestionsstring = JsonConvert.SerializeObject(paramSuggestions);
                ConvAssistMessage messageparamSuggestions = new ConvAssistMessage(WordPredictorMessageTypes.SetParam, WordPredictionModes.None, paramSuggestionsstring);
                string jsonMessageparamSuggestions = JsonConvert.SerializeObject(messageparamSuggestions);
                var resultSuggestions = await WriteAsync(jsonMessageparamSuggestions, 250);
            }
            catch (Exception ex)
            {
                Log.Debug("Error in paramSuggestions: " + ex);
            }
            await Task.Delay(25);
            try
            {
                //TEST GENERAL SENTENCE PREDICTION PARAMETER
                ConvAssistSetParam paramTestPred = new ConvAssistSetParam(ConvAssistSetParam.ConvAssistParameterType.TestGeneralSentencePrediction, ConvAssistWordPredictor.settings.Test_GeneralSentencePrediction.ToString());
                string paramTestPredstring = JsonConvert.SerializeObject(paramTestPred);
                ConvAssistMessage messageparamTestPred = new ConvAssistMessage(WordPredictorMessageTypes.SetParam, WordPredictionModes.None, paramTestPredstring);
                string jsonMessageparamTestPred = JsonConvert.SerializeObject(messageparamTestPred);
                var resultTestPred = await WriteAsync(jsonMessageparamTestPred, 250);
            }
            catch (Exception ex)
            {
                Log.Debug("Error in paramTestPred: " + ex);
            }
            await Task.Delay(25);
            try
            {
                //RETRIEVE FROM ACC PARAMETER
                ConvAssistSetParam paramRetrieveACC = new ConvAssistSetParam(ConvAssistSetParam.ConvAssistParameterType.RetrieveACC, ConvAssistWordPredictor.settings.EnableSmallVocabularySentencePrediction.ToString());
                string paramRetrieveACCstring = JsonConvert.SerializeObject(paramRetrieveACC);
                ConvAssistMessage messageparamRetrieveACC = new ConvAssistMessage(WordPredictorMessageTypes.SetParam, WordPredictionModes.None, paramRetrieveACCstring);
                string jsonMessageparamRetrieveACC = JsonConvert.SerializeObject(messageparamRetrieveACC);
                var resultRetrieveACC = await WriteAsync(jsonMessageparamRetrieveACC, 250);
            }
            catch (Exception ex)
            {
                Log.Debug("Error in paramRetrieveACC: " + ex);
            }

            await Task.Delay(25);
            try
            {
                //PATH STATIC
                string staticPath = Path.Combine(FileUtils.GetResourcesDir(), "WordPredictors", "ConvAssist");
                ConvAssistSetParam paramRetrievePathStatic = new ConvAssistSetParam(ConvAssistSetParam.ConvAssistParameterType.PathStatic, staticPath);
                string paramRetrievePathStaticstring = JsonConvert.SerializeObject(paramRetrievePathStatic);
                ConvAssistMessage messageparamRetrievePathStatic = new ConvAssistMessage(WordPredictorMessageTypes.SetParam, WordPredictionModes.None, paramRetrievePathStaticstring);
                string jsonMessageparamRetrievePathStatic = JsonConvert.SerializeObject(messageparamRetrievePathStatic);
                var resultRetrievePathStatic = await WriteAsync(jsonMessageparamRetrievePathStatic, 150);
            }
            catch (Exception ex)
            {
                Log.Debug("Error in paramPathStatic: " + ex);
            }

            await Task.Delay(25);
            try
            {
                //PATH PERSONALIZED
                string personalizedPath = Path.Combine(UserManager.CurrentUserDir, CultureInfo.DefaultThreadCurrentUICulture.Name, "WordPredictors", "ConvAssist", "Database");
                ConvAssistSetParam paramRetrievePathPersonalized = new ConvAssistSetParam(ConvAssistSetParam.ConvAssistParameterType.PathPersonilized, personalizedPath);
                string paramRetrievePathPersonalizedstring = JsonConvert.SerializeObject(paramRetrievePathPersonalized);
                ConvAssistMessage messageparamRetrievePathPersonalized = new ConvAssistMessage(WordPredictorMessageTypes.SetParam, WordPredictionModes.None, paramRetrievePathPersonalizedstring);
                string jsonMessageparamRetrievePathPersonalized = JsonConvert.SerializeObject(messageparamRetrievePathPersonalized);
                var resultRetrievePathPersonalized = await WriteAsync(jsonMessageparamRetrievePathPersonalized, 150);
            }
            catch (Exception ex)
            {
                Log.Debug("Error in paramPathPersonalized: " + ex);
            }

            await Task.Delay(25);
            try
            {
                //PATH FOR THE INI FILES FOR EACH MODE
                ConvAssistSetParam paramPath = new ConvAssistSetParam(ConvAssistSetParam.ConvAssistParameterType.Path, _pathToFiles);
                string paramPathstring = JsonConvert.SerializeObject(paramPath);
                ConvAssistMessage messageparamPath = new ConvAssistMessage(WordPredictorMessageTypes.SetParam, WordPredictionModes.None, paramPathstring);
                string jsonMessageparamPath = JsonConvert.SerializeObject(messageparamPath);
                var resultPath = await WriteAsync(jsonMessageparamPath, 60000);
                var resultObject = JsonConvert.DeserializeObject<WordAndCharacterPredictionResponse>(resultPath);
                if (resultObject != null && resultObject.MessageType == WordAndCharacterPredictionResponse.ConvAssistMessageTypes.SetParam)
                    clientAnswerParameters = true;
            }
            catch (Exception ex)
            {
                Log.Debug("Error in paramPath: " + ex);
            }
        }

        /// <summary>
        /// Starts the named pipe server
        /// </summary>
        public void Start()
        {
            Start(cancellationTokenSource.Token);
        }

        /// <summary>
        /// Starts the named pipe server.
        /// </summary>
        /// <param name="token"></param>
        public void Start(CancellationToken token)
        {
            if (!disposed)
            {
                this.NamedPipeServerStream.BeginWaitForConnection(OnConnection, new PipeServerStateConvAssist(this.NamedPipeServerStream, token));
            }
        }

        /// <summary>
        /// Stops the pipe server.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The object is disposed.</exception>
        public void Stop()
        {
            if (!this.disposed)
            {
                this.cancellationTokenSource.Cancel();
            }
        }

        public void Write(byte[] buffer)
        {
            if (!this.disposed)
            {
                this.NamedPipeServerStream.BeginWrite(buffer, 0, buffer.Length, this.WriteCallback, this.NamedPipeServerStream);
            }
        }

        /// <summary>
        /// Sends a string to the client. callback method
        /// </summary>
        /// <param name="value">
        /// The string to send to the server.
        /// </param>
        /// <exception cref="ObjectDisposedException">
        /// The object is disposed.
        /// </exception>
        public void Write(string value)
        {
            if (!this.disposed)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(value);
                this.NamedPipeServerStream.BeginWrite(buffer, 0, buffer.Length, this.WriteCallback, this.NamedPipeServerStream);
            }
        }

        /// <summary>
        /// Sends a string to the client. Async method
        /// </summary>
        /// <param name="value">
        /// The string to send to the server.
        /// </param>
        /// <exception cref="ObjectDisposedException">
        /// The object is disposed.
        /// </exception>
        public async Task<string> WriteAsync(string value, int msDelay)
        {
            TaskFinished = false;
            string message;
            //Variable set when the event from receiving data triggers
            messageReceived = string.Empty;
            if (!this.disposed)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(value);
                this.NamedPipeServerStream.BeginWrite(buffer, 0, buffer.Length, this.WriteCallback, this.NamedPipeServerStream);
            }

            CancellationTokenSource source = new CancellationTokenSource(msDelay);

            try
            {
                while (!source.IsCancellationRequested)
                {
                    if (messageReceived != string.Empty)
                    {
                        source.Cancel(true);
                    }
                    await Task.Delay(10);
                }
            }
            catch
            {

            }
            finally
            {
                source.Dispose();
            }
            
            message = messageReceived;
            messageReceived = string.Empty;
            TaskFinished = true;
            return message;
        }

        /// <summary>
        /// Sends a string to the client. Sync method
        /// </summary>
        /// <param name="value">
        /// The string to send to the server.
        /// </param>
        /// <exception cref="ObjectDisposedException">
        /// The object is disposed.
        /// </exception>
        public string WriteSync(string value, int msDelay)
        {
            string message = string.Empty;
            try
            {
                Log.Debug("Pressaio WriteSync Lock on");
                EnterCriticalSection(_syncObj);
                TaskFinished = false;
                //Variable set when the event from receiving data triggers
                messageReceived = string.Empty;
                if (!this.disposed)
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(value);
                    this.NamedPipeServerStream.BeginWrite(buffer, 0, buffer.Length, this.WriteCallback, this.NamedPipeServerStream);
                }

                CancellationTokenSource source = new CancellationTokenSource(msDelay);
                try
                {
                    while (!source.IsCancellationRequested)
                    {
                        if (messageReceived != string.Empty)
                        {
                            source.Cancel(true);
                        }
                        Thread.Sleep(10);
                    }
                }
                catch
                {
                }

                source.Dispose();
                message = messageReceived;
                messageReceived = string.Empty;
                TaskFinished = true;
            }
            catch (Exception ex)
            {
                Log.Debug("Pressaio WriteSync " + ex);
            }
            finally
            {
                ExitCriticalSection(_syncObj);
            }
            Log.Debug("Pressaio WriteSync Lock off");
            return message;
        }

        public void WriteToPipe(string data)
        {
            Write(data);
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
        /// Callback when a client connects
        /// </summary>
        /// <param name="ar"></param>
        private void OnConnection(IAsyncResult ar)
        {
            Log.Debug("Pressaio Establish connection");
            NumClientsConnected++;
            clientConected = true;
            PipeServer_EvtClientConnected();
            var pipeServerState = (PipeServerStateConvAssist)ar.AsyncState;
            if (pipeServerState.ExternalCancellationToken.IsCancellationRequested)
            {
                return;
            }
            pipeServerState.PipeServer.EndWaitForConnection(ar);
            pipeServerState.PipeServer.BeginRead(pipeServerState.Buffer, 0, PipeServerStateConvAssist.BufferSize, ReadCallback, pipeServerState);
        }

        /// <summary>
        /// The on message received.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        ///
        private void OnMessageReceived(MessageReceivedEventArgs e)
        {
            this.MessageReceived?.Invoke(this, e);
            messageReceived = e.Message;
        }

        private void PipeServer_EvtClientConnected()
        {
            EvtClientConnected?.Invoke(null, new EventArgs());
            clientConected = true;
            _ = SendParams().ConfigureAwait(false);
        }

        private void PipeServer_EvtClientDisconnected()
        {
            Log.Debug("Pressaio PipeServer_EvtClientDisconnected");
            EvtClientDisconnected?.Invoke(null, new EventArgs());
            clientConected = false;
            clientAnswerParameters = false;
            ClosePipeServer();
            CreatePipeServer();
        }

        /// <summary>
        /// The read callback.
        /// </summary>
        /// <param name="ar">
        /// The ar.
        /// </param>
        private void ReadCallback(IAsyncResult ar)
        {
            var pipeState = (PipeServerStateConvAssist)ar.AsyncState;
            int received = pipeState.PipeServer.EndRead(ar);
            if (received <= 0)
            {
                NumClientsConnected--;
                PipeServer_EvtClientDisconnected();
                return;
            }
            if (received < 4)
            {
                return;
            }
            string stringData = Encoding.UTF8.GetString(pipeState.Buffer, 0, received);
            pipeState.Message.Append(stringData);
            if (pipeState.PipeServer.IsMessageComplete)
            {
                OnMessageReceived(new MessageReceivedEventArgs(stringData));
                pipeState.Message.Clear();
            }

            if (!(cancellationToken.IsCancellationRequested || pipeState.ExternalCancellationToken.IsCancellationRequested))
            {
                if (pipeState.PipeServer.IsConnected)
                {
                    pipeState.PipeServer.BeginRead(pipeState.Buffer, 0, PipeServerStateConvAssist.BufferSize, ReadCallback, pipeState);
                }
                else
                {
                    pipeState.PipeServer.BeginWaitForConnection(OnConnection, pipeState);
                }
            }
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

        /// <summary>
        /// The send callback.
        /// </summary>
        /// <param name="iar">
        /// The iar.
        /// </param>
        private void WriteCallback(IAsyncResult iar)
        {
            try
            {
                var pipeStream = (NamedPipeServerStream)iar.AsyncState;
                pipeStream.EndWrite(iar);
            }
            catch (Exception ex)
            {
                Log.Debug("Pressaio Error in writeCallback: " + ex.Message);
            }
        }
    }

    /// <summary>
    ///     Message received event arguments.
    /// </summary>
    /*public class MessageReceivedEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="message">
        /// The message received.
        /// </param>
        public MessageReceivedEventArgs(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                this.Message = message;
            }
        }

        #endregion Constructors and Destructors

        #region Public Properties

        /// <summary>
        ///     Gets the message received from the named-pipe.
        /// </summary>
        public string Message { get; private set; }

        #endregion Public Properties
    }*/
    /// <summary>
    /// Contains global variables
    /// </summary>

    /*public static class Globals
    {
        /// <summary>
        /// Title of the application
        /// </summary>
        public static String AppTitle = "Named Pipe Server";
        /// <summary>
        /// Folder where files are stored
        /// </summary>
        public static String LabelsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Study Data";
        /// <summary>
        /// Name of the pipe server
        /// </summary>
        public static String PipeServerName = "TestPipe";
        public static String Version = String.Empty;
    }*/
}