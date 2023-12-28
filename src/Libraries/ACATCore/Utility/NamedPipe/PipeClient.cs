////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.IO.Pipes;
using System.Text;

namespace ACAT.Lib.Core.Utility.NamedPipe
{
    public sealed class PipeClient : IDisposable
    {
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="pipeName">
        /// The name of the pipe.
        /// </param>
        /// <param name="pipeDirection">
        /// Determines the direction of the pipe.
        /// </param>
        public PipeClient(string pipeName, PipeDirection pipeDirection)
        {
            ClientStream = new NamedPipeClientStream(".", pipeName, pipeDirection, PipeOptions.Asynchronous);
        }

        public event EventHandler EvtServerDisconnected;

        /// <summary>
        /// Occurs when a message is received from the pipe server.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        /// <summary>
        /// Gets or sets the client stream.
        /// </summary>
        private NamedPipeClientStream ClientStream { get; set; }

        /// <summary>
        /// Connects to the pipe server.
        /// </summary>
        /// <param name="timeout">
        /// The time to wait before timing out.
        /// </param>
        /// <exception cref="ObjectDisposedException">
        /// The object is disposed.
        /// </exception>
        /// <exception cref="TimeoutException">
        /// Could not connect to the server within the specified timeout period.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Timeout is less than 0 and not set to Infinite.
        /// </exception>
        public void Connect(int timeout = 1000)
        {
            if (!disposed)
            {
                ClientStream.Connect(timeout);
                ClientStream.ReadMode = PipeTransmissionMode.Message;

                var clientState = new PipeClientState(ClientStream);
                ClientStream.BeginRead(
                    clientState.Buffer,
                    0,
                    clientState.Buffer.Length,
                    ReadCallback,
                    clientState);
            }
        }

        /// <summary>
        /// Dispose the pipe client.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Sends a string to the server.
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
                ClientStream.BeginWrite(buffer, 0, buffer.Length, SendCallback, ClientStream);
            }
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    ClientStream.Dispose();
                }

                disposed = true;
            }
        }

        /// <summary>
        /// Trigger the message received event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
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
        /// The asynchronous result.
        /// </param>
        private void ReadCallback(IAsyncResult ar)
        {
            var pipeState = (PipeClientState)ar.AsyncState;

            int received = pipeState.PipeClient.EndRead(ar);
            string stringData = Encoding.UTF8.GetString(pipeState.Buffer, 0, received);
            pipeState.Message.Append(stringData);
            if (pipeState.PipeClient.IsMessageComplete)
            {
                var str = pipeState.Message.ToString();
                if (String.IsNullOrEmpty(str))
                {
                    if (EvtServerDisconnected != null)
                    {
                        EvtServerDisconnected(this, new EventArgs());
                    }
                }
                else
                {
                    OnMessageReceived(new MessageReceivedEventArgs(str));
                }
                pipeState.Message.Clear();
            }

            if (pipeState.PipeClient.IsConnected)
            {
                pipeState.PipeClient.BeginRead(pipeState.Buffer, 0, 255, ReadCallback, pipeState);
            }
        }

        /// <summary>
        /// The send callback.
        /// </summary>
        /// <param name="iar">
        /// The asynchronous result.
        /// </param>
        private void SendCallback(IAsyncResult iar)
        {
            if (!disposed)
            {
                var pipeStream = (NamedPipeClientStream)iar.AsyncState;
                pipeStream.EndWrite(iar);
            }
        }
    }
}