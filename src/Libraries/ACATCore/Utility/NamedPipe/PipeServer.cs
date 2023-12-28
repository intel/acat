////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace ACAT.Lib.Core.Utility.NamedPipe
{
    public sealed class PipeServer : IDisposable
    {
        private CancellationToken cancellationToken;
        private CancellationTokenSource cancellationTokenSource;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipeServer"/> class.
        /// </summary>
        /// <param name="pipeName">
        /// The name of the pipe.
        /// </param>
        /// <param name="pipeDirection">
        /// Determines the direction of the pipe.
        /// </param>
        /// <param name="allowAllReadWrite">
        /// Allow the pipe to be visible by all the users
        /// Current user and Administrator user
        /// </param>
        public PipeServer(string pipeName, PipeDirection pipeDirection, bool allowAllReadWrite = false)
        {
            if (!allowAllReadWrite)
            {
                ServerStream = new NamedPipeServerStream(
                pipeName,
                pipeDirection,
                1,
                PipeTransmissionMode.Message,
                PipeOptions.Asynchronous);
                cancellationTokenSource = new CancellationTokenSource();
            }
            else
            {
                PipeSecurity pipeSecurity = new PipeSecurity();
                var id = new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null);
                //Allow Everyone read and write access to the pipe. 
                pipeSecurity.SetAccessRule(new PipeAccessRule(id, PipeAccessRights.ReadWrite, AccessControlType.Allow));

                ServerStream = new NamedPipeServerStream(
                    pipeName,
                    pipeDirection,
                    1,
                    PipeTransmissionMode.Message,
                    PipeOptions.Asynchronous,
                    0x4000,
                    0x400,
                    pipeSecurity,
                    HandleInheritability.Inheritable);
                cancellationTokenSource = new CancellationTokenSource();
            }
        }

        /// <summary>
        /// Occurs when a message is received from the named pipe.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        /// <summary>
        /// Gets the named-pipe server stream.
        /// </summary>
        public NamedPipeServerStream ServerStream { get; private set; }

        /// <summary>
        /// Disposes the pipe server.
        /// </summary>
        public void Dispose()
        {
            if (!disposed)
            {
                cancellationTokenSource.Dispose();
                ServerStream.Dispose();

                disposed = true;
            }
        }

        /// <summary>
        /// Sends a string to the client.
        /// </summary>
        /// <param name="value">
        /// The string to send to the server.
        /// </param>
        /// <exception cref="ObjectDisposedException">
        /// The object is disposed.
        /// </exception>
        public void Send(string value)
        {
            if (!disposed)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(value);
                ServerStream.BeginWrite(buffer, 0, buffer.Length, SendCallback, ServerStream);
            }
        }

        /// <summary>
        /// Start the pipe server.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The object is disposed.</exception>
        public void Start()
        {
            Start(cancellationTokenSource.Token);
        }

        /// <summary>
        /// Start the pipe server.
        /// </summary>
        /// <param name="token"></param>
        public void Start(CancellationToken token)
        {
            if (!disposed)
            {
                var state = new PipeServerState(ServerStream, token);
                ServerStream.BeginWaitForConnection(ConnectionCallback, state);
            }
        }

        /// <summary>
        /// Stops the pipe server.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The object is disposed.</exception>
        public void Stop()
        {
            if (!disposed)
            {
                cancellationTokenSource.Cancel();
            }
        }

        /// <summary>
        /// The connection callback.
        /// </summary>
        /// <param name="ar">
        /// The ar.
        /// </param>
        private void ConnectionCallback(IAsyncResult ar)
        {
            var pipeServer = (PipeServerState)ar.AsyncState;
            pipeServer.PipeServer.EndWaitForConnection(ar);

            pipeServer.PipeServer.BeginRead(pipeServer.Buffer, 0, 255, ReadCallback, pipeServer);
        }

        /// <summary>
        /// The on message received.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnMessageReceived(MessageReceivedEventArgs e)
        {
            EventHandler<MessageReceivedEventArgs> handler = MessageReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// The read callback.
        /// </summary>
        /// <param name="ar">
        /// The ar.
        /// </param>
        private void ReadCallback(IAsyncResult ar)
        {
            try
            {
                var pipeState = (PipeServerState)ar.AsyncState;

                int received = pipeState.PipeServer.EndRead(ar);
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
                        pipeState.PipeServer.BeginRead(pipeState.Buffer, 0, 255, ReadCallback, pipeState);
                    }
                    else
                    {
                        pipeState.PipeServer.BeginWaitForConnection(ConnectionCallback, pipeState);
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Debug("Error in ReadCallback: " + ex.Message);
            }

        }

        /// <summary>
        /// The send callback.
        /// </summary>
        /// <param name="iar">
        /// The iar.
        /// </param>
        private void SendCallback(IAsyncResult iar)
        {
            var pipeStream = (NamedPipeServerStream)iar.AsyncState;
            pipeStream.EndWrite(iar);
        }
    }
}