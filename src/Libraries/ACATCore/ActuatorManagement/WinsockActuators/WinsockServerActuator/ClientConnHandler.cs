////////////////////////////////////////////////////////////////////////////
// <copyright file="ClientConnHandler.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2015 Intel Corporation 
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ACAT.Lib.Core.Utility;

#region SupressStyleCopWarnings

[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1126:PrefixCallsCorrectly",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1101:PrefixLocalCallsWithThis",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1121:UseBuiltInTypeAlias",
        Scope = "namespace",
        Justification = "Since they are just aliases, it doesn't really matter")]
[module: SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1200:UsingDirectivesMustBePlacedWithinNamespace",
        Scope = "namespace",
        Justification = "ACAT guidelines")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1309:FieldNamesMustNotBeginWithUnderscore",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private fields begin with an underscore")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1300:ElementMustBeginWithUpperCaseLetter",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private/Protected methods begin with lowercase")]

#endregion SupressStyleCopWarnings

namespace ACAT.Lib.Core.InputActuators
{
    /// <summary>
    /// Manages the send/receive of data between a connected client.
    /// The socket server will instantiate one of these objects for
    /// each connected client.
    /// </summary>
    internal class ClientConnHandler : IDisposable
    {
        public ConnectionStatus ClientConnectionStatus = ConnectionStatus.Disconnected;
        public String ID = String.Empty;
        public Thread WorkerThread;
        protected const int ReadBufferSize = 8192;
        protected int bytesRead = 0;
        protected NetworkStream clientStream;
        protected IPEndPoint remoteEp;
        protected bool runThread = true;
        protected TcpClient tcpClient;
        private readonly object _lockObj = new object();
        private MemoryStream _memoryStream;
        private int _packetSize = 512;

        /// <summary>
        /// Initializes a instance of the class
        /// </summary>
        /// <param name="client">The TcpClient connection that is connected to a client.</param>
        public ClientConnHandler(TcpClient client)
        {
            tcpClient = client;
            clientStream = tcpClient.GetStream();
            ID = Guid.NewGuid().ToString();
            _memoryStream = new MemoryStream();

            try
            {
                remoteEp = (IPEndPoint)tcpClient.Client.RemoteEndPoint;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }

            OnClientMsgReceived += ClientConnHandler_OnClientMsgReceived;
            SocketServer.OnServerShutdown += SocketServer_OnServerShutdown;
            SocketServer.OnSendToClient += SocketServer_OnSendToClient;
        }

        /// <summary>
        /// Event delegate for connection status changed
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="status">the connection status</param>
        public delegate void OnClientConnStatusChangedDelegate(object sender, ConnectionStatus status);

        /// <summary>
        /// Event delegated when data is received
        /// </summary>
        /// <param name="packet">data packet</param>
        public delegate void OnPacketReceivedDelegate(byte[] packet);

        /// <summary>
        /// Event delegated when data is received
        /// </summary>
        /// <param name="message">message received</param>
        /// <param name="count">length</param>
        private delegate void OnClientMsgReceivedDelegate(byte[] message, int count);

        /// <summary>
        /// Raised when the connection status of the client changes.
        /// </summary>
        public event OnClientConnStatusChangedDelegate OnClientConnStatusChanged;

        /// <summary>
        /// Raised when packet is received
        /// </summary>
        public event OnPacketReceivedDelegate OnPacketReceived;

        /// <summary>
        /// Raised when the connection receives a message from a client.
        /// </summary>
        private event OnClientMsgReceivedDelegate OnClientMsgReceived;

        /// <summary>
        /// Status of the connection
        /// </summary>
        public enum ConnectionStatus
        {
            Connected,
            Disconnected
        }

        /// <summary>
        /// Gets the IP address of the connected client
        /// </summary>
        public String ClientIPAddress
        {
            get
            {
                var ipe = (IPEndPoint)this.tcpClient.Client.LocalEndPoint;
                var addr = IPAddress.Parse(ipe.Address.ToString());
                return addr.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the packet size
        /// </summary>
        public int PacketSize
        {
            get
            {
                return _packetSize;
            }

            set
            {
                _packetSize = value;
            }
        }

        /// <summary>
        /// Disposes resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Thread method to read messages from the client and
        /// dispatch them to be parsed, processed and distributed.
        /// </summary>
        public void WorkerThreadMethod()
        {
            ClientConnectionStatus = ConnectionStatus.Connected;
            if (OnClientConnStatusChanged != null)
            {
                OnClientConnStatusChanged.Invoke(this, ConnectionStatus.Connected);
            }

            var message = new byte[ReadBufferSize];
            _memoryStream.SetLength(0);

            while (true)
            {
                bytesRead = 0;
                try
                {
                    // blocks until a client sends a message
                    bytesRead = clientStream.Read(message, 0, ReadBufferSize);
                    Log.Debug("Returned from read. Bytes read: " + bytesRead);
                }
                catch (Exception e)
                {
                    // Probably a socket exception.
                    Log.Error("SERVER: ClientConnHandler, exception reading stream " + e.StackTrace);
                    break;
                }

                if (bytesRead == 0)
                {
                    //the client has disconnected from the server
                    break;
                }

                // Data has been received, let's process it.
                Log.Debug("Calling OnClientMsgReceived");
                if (OnClientMsgReceived != null)
                {
                    OnClientMsgReceived.Invoke(message, bytesRead);
                }

                Log.Debug("Returned from OnClientMsgReceived");
            }

            // Out of the WorkerThreadMethod loop... time to close stuff down.
            ClientConnectionStatus = ConnectionStatus.Disconnected;
            if (OnClientConnStatusChanged != null)
            {
                OnClientConnStatusChanged.Invoke(this, ConnectionStatus.Disconnected);
            }

            tcpClient.Close();
        }

        /// <summary>
        /// Dispose off this object
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free resources
                SocketServer.OnSendToClient -= SocketServer_OnSendToClient;
                SocketServer.OnServerShutdown -= SocketServer_OnServerShutdown;

                tcpClient = null;

                if (WorkerThread != null)
                {
                    WorkerThread.Abort();
                    WorkerThread = null;
                }
            }
            // free some native resources if applicable.
        }

        /// <summary>
        /// Sends a string message to a network stream client.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="count">number of bytes to send</param>
        /// <returns>true on success</returns>
        protected bool SendToClient(byte[] message, int count)
        {
            NetworkStream stream = tcpClient.GetStream();

            try
            {
                stream.Write(message, 0, count);
                stream.Flush();
                return true;
            }
            catch (Exception e)
            {
                Log.Exception(e);
                return false;
            }
        }

        /// <summary>
        /// Callback function for the thread pool queue, called
        ///  when data is received
        /// </summary>
        /// <param name="item">the work item containing the data</param>
        private static void Worker(object item)
        {
            var workItem = (WorkItem)item;
            workItem.ClientConn.OnPacketReceived(workItem.Data);
        }

        /// <summary>
        /// Event handler called when the client receives a message.
        /// </summary>
        /// <param name="message">data received</param>
        /// <param name="count">length of data</param>
        private void ClientConnHandler_OnClientMsgReceived(byte[] message, int count)
        {
            HandleClientMessage(message, count);
        }

        /// <summary>
        /// Handler that parses the message and then queues up the message
        /// to the worker threads
        /// </summary>
        /// <param name="messageFromClient">Data received from the client</param>
        /// <param name="count"> Length of the data</param>
        private void HandleClientMessage(byte[] messageFromClient, int count)
        {
            Log.Debug("Received " + count + " bytes");
            lock (_lockObj)
            {
                int offset = 0;
                int bytesRemaining = count;

                while (bytesRemaining > 0)
                {
                    int bytesLeftInCurrentPacket = _packetSize - (int)_memoryStream.Length;
                    int bytesToRead = (bytesRemaining < bytesLeftInCurrentPacket) ? bytesRemaining : bytesLeftInCurrentPacket;

                    _memoryStream.Write(messageFromClient, offset, bytesToRead);
                    if (_memoryStream.Length == _packetSize)
                    {
                        var item = new WorkItem
                        {
                            ClientConn = this,
                            Data = _memoryStream.ToArray()
                        };

                        Log.Debug(String.Format("Entire packet received.  Calling threadpool for data received"));
                        ThreadPool.QueueUserWorkItem(Worker, item);
                        Log.Debug(String.Format("Returned from QueueUserWorkItem"));
                        _memoryStream = new MemoryStream();
                    }

                    offset += bytesToRead;
                    bytesRemaining -= bytesToRead;
                }
            }
        }

        /// <summary>
        /// Event handler for sending data to the client
        /// </summary>
        /// <param name="sender">Usually a reference to the SocketServer.</param>
        /// <param name="message">Message to send to our connected client.</param>
        private void SocketServer_OnSendToClient(object sender, byte[] message, int count)
        {
            SendToClient(message, count);
        }

        /// <summary>
        /// Event handler for socket erver shutdown.  Stop threads.
        /// </summary>
        private void SocketServer_OnServerShutdown()
        {
            try
            {
                WorkerThread.Abort();
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        /// <summary>
        /// Object passed to the thread that handles dispatching of
        /// the results of the conversion back to the client.
        /// </summary>
        private class WorkItem
        {
            public ClientConnHandler ClientConn { get; set; }

            public byte[] Data { get; set; }
        }
    }
}