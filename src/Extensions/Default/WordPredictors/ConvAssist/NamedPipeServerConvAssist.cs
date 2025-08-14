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

using ACAT.Core.UserManagement;
using ACAT.Core.Utility;
using ACAT.Core.Utility.NamedPipe;
using ACAT.Core.WordPredictorManagement.Interfaces;
using ACAT.Extension;
using ACAT.Extensions.WordPredictors.ConvAssist.MessageTypes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ACAT.Extensions.WordPredictors.ConvAssist
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
        private readonly object _syncObj = new();

        /// <summary>
        /// Path to the INI files
        /// </summary>
        private readonly string _pathToFiles = string.Empty;

        /// <summary>
        /// Cancelation object to skip task
        /// </summary>
        private CancellationToken cancellationToken;

        private CancellationTokenSource cancellationTokenSource;
        private bool disposed;

        private static readonly object _writeSyncObj = new();

        /// <summary>
        /// Direction of comunication
        /// </summary>
        private readonly PipeDirection PipeDirection;

        /// <summary>
        /// Given Pipe name to be conected
        /// </summary>
        private readonly string PipeName;

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
        public NamedPipeServerStream NamedPipeServer { get; private set; }

        public bool TaskFinished { get; private set; }

        /// <summary>
        /// Closes the pipe server
        /// </summary>
        /// <returns></returns>
        public bool ClosePipeServer()
        {
            try
            {
                if (NamedPipeServer != null && cancellationTokenSource != null)
                {
                    Stop();
                    Dispose();
                }
            }
            catch (Exception ex)
            {
                Log.Exception("ConvAssist ClosePipe Error:" + ex.Message);
                return false;
            }
            clientConected = false;
            clientAnswerParameters = false;
            return true;
        }

        public bool CreatePipeServer(bool send_params = false)
        {
            disposed = false;
            bool success = false;
            cancellationTokenSource = new CancellationTokenSource();
            NamedPipeServer = new NamedPipeServerStream(PipeName, PipeDirection,
                                            1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
            try
            {
                success = StartNamedPipeServer(cancellationToken, send_params).Result;
            }
            catch (Exception ex)
            {
                Log.Exception("Exception in createPipeServer: " + ex);
            }

            return success;
        }

        /// <summary>
        /// Disposes the pipe server.
        /// </summary>
        public void Dispose()
        {
            if (!disposed)
            {
                cancellationTokenSource.Dispose();
                NamedPipeServer.Dispose();
                NamedPipeServer = null;
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
                Log.Exception("ConvAssist AppEvent: " + ex.Message);
            }
            return true;
        }

        public async Task SendParams()
        {
            //Static Path
            string staticPath = Path.Combine(FileUtils.GetResourcesDir(), "WordPredictors", "ConvAssist");

            //Personalize Path
            string personalizedPath = Path.Combine(UserManager.CurrentUserDir, CultureInfo.DefaultThreadCurrentUICulture.TwoLetterISOLanguageName, "WordPredictors", "ConvAssist", "Database");


            List<string> parameters = new()
            {
                JsonSerializer.Serialize(new ConvAssistSetParam(ConvAssistSetParam.ConvAssistParameterType.EnableLog, Common.AppPreferences.EnableLogs.ToString())),
                JsonSerializer.Serialize(new ConvAssistSetParam(ConvAssistSetParam.ConvAssistParameterType.PathLog, FileUtils.GetLogsDir())),
                JsonSerializer.Serialize(new ConvAssistSetParam(ConvAssistSetParam.ConvAssistParameterType.Suggestions, Common.AppPreferences.WordsSuggestions.ToString())),
                JsonSerializer.Serialize(new ConvAssistSetParam(ConvAssistSetParam.ConvAssistParameterType.TestGeneralSentencePrediction, ConvAssistWordPredictor.settings.Test_GeneralSentencePrediction.ToString())),
                JsonSerializer.Serialize(new ConvAssistSetParam(ConvAssistSetParam.ConvAssistParameterType.RetrieveACC, ConvAssistWordPredictor.settings.EnableSmallVocabularySentencePrediction.ToString())),
                JsonSerializer.Serialize(new ConvAssistSetParam(ConvAssistSetParam.ConvAssistParameterType.PathStatic, staticPath)),
                JsonSerializer.Serialize(new ConvAssistSetParam(ConvAssistSetParam.ConvAssistParameterType.PathPersonilized, personalizedPath)),
                JsonSerializer.Serialize(new ConvAssistSetParam(ConvAssistSetParam.ConvAssistParameterType.Path, _pathToFiles))
            };


            foreach (var param in parameters)
            {
                try
                {
                    var message = JsonSerializer.Serialize(new ConvAssistMessage(WordPredictorMessageTypes.SetParam, WordPredictionModes.None, param));
                    
                    //TODO: Check result and handle appropriately.
                    _ = WriteAsync(message, 150).ConfigureAwait(false).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    Log.Exception("Error in param enable log: " + ex);
                }
            }


            // ConvAssist needs some time to get ready.  Send a message to check if it is ready
            string msg = JsonSerializer.Serialize(new ConvAssistMessage(WordPredictorMessageTypes.NextSentencePredictionRequest, WordPredictionModes.None, string.Empty));

            bool clientReady = false;
            var tcs = new TaskCompletionSource<bool>();

            await Task.Run(async () =>
            {
                while (!clientReady)
                {
                    var result = WriteAsync(msg, 150).ConfigureAwait(false).GetAwaiter().GetResult();
                    var resultObject = JsonSerializer.Deserialize<WordAndCharacterPredictionResponse>(result);
                    if (resultObject != null && resultObject.MessageType != WordAndCharacterPredictionResponse.ConvAssistMessageTypes.NotReady)
                    {
                        clientReady = true;
                        tcs.SetResult(true);
                        break;
                    }
                    await Task.Delay(200);
                }
            });

            bool timeout = await ConvAssistUtils.WithTimeout(tcs.Task, TimeSpan.FromSeconds(30));
        }

        /// <summary>
        /// Starts the named pipe server.
        /// </summary>
        /// <param name="token"></param>
        public async Task<bool> StartNamedPipeServer(CancellationToken token, bool send_params = true, int timeout_sec = 30)
        {
            bool success = false;

            if (!disposed)
            {
                var tcs = new TaskCompletionSource<bool>();

                using (token.Register(() =>
                {
                    try { NamedPipeServer.Close(); } catch { }
                }))
                {
                    NamedPipeServer.BeginWaitForConnection(ar =>
                    {
                        try
                        {
                            OnConnection(ar);
                            tcs.TrySetResult(true);
                        }
                        catch (Exception ex)
                        {
                            tcs.TrySetException(ex);
                        }
                    }, new PipeServerStateConvAssist(NamedPipeServer, token));

                    try
                    {
                        success = ConvAssistUtils.WithTimeout(tcs.Task, TimeSpan.FromSeconds(timeout_sec)).ConfigureAwait(false).GetAwaiter().GetResult();
                    }
                    catch (TimeoutException)
                    {
                        Log.Exception("ConvAssist StartNamedPiperServer Timeout");

                    }
                }

                if (success && send_params)
                {
                    try
                    {
                        var sendTask = SendParams();
                        if(await Task.WhenAny(sendTask, Task.Delay(5000)) != sendTask)
                        {
                            throw new TimeoutException("SendParam() took too long.");
                        }
                        await sendTask;
                        success = true;
                    }
                    catch (TimeoutException ex)
                    {
                        success = false;
                        Log.Exception(ex);
                    }
                    catch (Exception ex)
                    {
                        success = false;
                        Log.Exception(ex);
                    }
                }

                PipeServer_EvtClientConnected();

            }

            return success;

            //bool success = false;
            //if (!disposed)
            //{
            //    var tcs = new TaskCompletionSource<bool>();
            //    NamedPipeServer.BeginWaitForConnection(ar =>
            //    {
            //        try
            //        {
            //            OnConnection(ar);
            //            tcs.SetResult(true);
            //        }
            //        catch (Exception ex)
            //        {
            //            tcs.SetException(ex);
            //        }
            //    }, new PipeServerStateConvAssist(NamedPipeServer, token));

            //    try
            //    {
            //        success = await ConvAssistUtils.WithTimeout(tcs.Task, TimeSpan.FromSeconds(timeout_sec));
            //    }
            //    catch (TimeoutException)
            //    {
            //        Log.Exception("ConvAssist StartNamedPipeServer Timeout");
            //    }

            //    if (success && send_params)
            //    {
            //        try
            //        {
            //            var sendTask = SendParams();
            //            if (await Task.WhenAny(sendTask, Task.Delay(5000)) != sendTask)
            //            {
            //                throw new TimeoutException("SendParams() took too long.");
            //            }

            //            await sendTask;
            //            success = true;
            //        }
            //        catch (TimeoutException ex)
            //        {
            //            success = false;
            //            Log.Debug(ex.Message);
            //        }
            //        catch (Exception ex)
            //        {
            //            success = false;
            //            Log.Debug("Error in sendParams: " + ex);
            //        }
            //    }

            //    PipeServer_EvtClientConnected();
            //}

            //return success;
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
                this.NamedPipeServer.BeginWrite(buffer, 0, buffer.Length, this.WriteCallback, this.NamedPipeServer);
            }
        }

#nullable enable
        public async Task<string> WriteAsync(string value, int msDelay)
        {
            if (disposed)
                return string.Empty;

            TaskFinished = false;
            messageReceived = string.Empty;

            var writeTcs = new TaskCompletionSource<object?>();

            byte[] buffer = Encoding.UTF8.GetBytes(value);
            this.NamedPipeServer.BeginWrite(buffer, 0, buffer.Length, ar =>
            {
                try
                {
                    this.NamedPipeServer.EndWrite(ar);
                    writeTcs.TrySetResult(null);
                }
                catch (Exception ex)
                {
                    writeTcs.TrySetException(ex);
                }
            }, null);

            using var source = new CancellationTokenSource(msDelay);

            try
            {
                await writeTcs.Task.ConfigureAwait(false);

                while (!source.IsCancellationRequested && !disposed)
                {
                    if (!string.IsNullOrEmpty(messageReceived))
                        break;

                    await Task.Delay(10, source.Token).ConfigureAwait(false);
                }
            }

            catch (TaskCanceledException)
            {
                // Expected if msDelay elapsed
            }
            finally
            {
                TaskFinished = true;
            }

            var message = messageReceived;
            messageReceived = string.Empty;
            return message;
        }
#nullable disable
        ///// <summary>
        ///// Sends a string to the client. Async method
        ///// </summary>
        ///// <param name="value">
        ///// The string to send to the server.
        ///// </param>
        ///// <exception cref="ObjectDisposedException">
        ///// The object is disposed.
        ///// </exception>
        //public async Task<string> WriteAsync(string value, int msDelay)
        //{
        //    TaskFinished = false;
        //    string message;
        //    //Variable set when the event from receiving data triggers
        //    messageReceived = string.Empty;
        //    if (!this.disposed)
        //    {
        //        byte[] buffer = Encoding.UTF8.GetBytes(value);
        //        this.NamedPipeServer.BeginWrite(buffer, 0, buffer.Length, this.WriteCallback, this.NamedPipeServer);
        //    }

        //    CancellationTokenSource source = new CancellationTokenSource(msDelay);

        //    try
        //    {
        //        while (!source.IsCancellationRequested)
        //        {
        //            if (messageReceived != string.Empty)
        //            {
        //                source.Cancel(true);
        //            }
        //            await Task.Delay(10);
        //        }
        //    }
        //    catch
        //    {
        //    }
        //    finally
        //    {
        //        source.Dispose();
        //    }

        //    message = messageReceived;
        //    messageReceived = string.Empty;
        //    TaskFinished = true;
        //    return message;
        //}

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
                Log.Debug("ConvAssist WriteSync Lock on");
                lock (_writeSyncObj)
                {
                    TaskFinished = false;
                    //Variable set when the event from receiving data triggers
                    messageReceived = string.Empty;
                    if (!this.disposed)
                    {
                        byte[] buffer = Encoding.UTF8.GetBytes(value);
                        this.NamedPipeServer.BeginWrite(buffer, 0, buffer.Length, this.WriteCallback, this.NamedPipeServer);
                    }

                    CancellationTokenSource source = new(msDelay);
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
            }
            catch (Exception ex)
            {
                Log.Exception("ConvAssist WriteSync " + ex);
            }
            Log.Debug("ConvAssist WriteSync Lock off");
            return message;
        }

        public void WriteToPipe(string data)
        {
            Write(data);
        }

        /// <summary>
        /// Callback when a client connects
        /// </summary>
        /// <param name="ar"></param>
        private void OnConnection(IAsyncResult ar)
        {
            Log.Debug("ConvAssist Establish connection");
            NumClientsConnected++;
            clientConected = true;
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
        }

        private void PipeServer_EvtClientDisconnected()
        {
            Log.Debug("ConvAssist PipeServer_EvtClientDisconnected");
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
                Log.Exception("ConvAssist Error in writeCallback: " + ex.Message);
            }
        }
    }
}