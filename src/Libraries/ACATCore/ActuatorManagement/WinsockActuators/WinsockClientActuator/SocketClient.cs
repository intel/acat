////////////////////////////////////////////////////////////////////////////
// <copyright file="SocketClient.cs" company="Intel Corporation">
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
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

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
    /// A generic TCP socket client to send data to a TCP server.  Supports
    /// asynchronous and synchronous writes
    /// </summary>
    public class SocketClient : IDisposable
    {
        /// <summary>
        /// Port to connect to on the server
        /// </summary>
        private readonly int _port = -1;

        /// <summary>
        /// Sync event to indicate that DNS host resolved has completed
        /// </summary>
        private readonly WaitHandle _resolveHostEvent;

        /// <summary>
        /// .NET TCPClient
        /// </summary>
        private readonly TcpClient _tcpClient;

        /// <summary>
        /// List of addresses to connect to.
        /// </summary>
        private IPAddress[] _addressList;

        /// <summary>
        /// How many addresses have we tried to connect so far?
        /// </summary>
        private int _connectionAttempts;

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        private int _fixedPacketSize = 512;

        /// <summary>
        /// Minimum transmission unit.
        /// </summary>
        /// <summary>
        /// Initialize an instance of the class
        /// </summary>
        /// <param name="address">Server address to connect to</param>
        /// <param name="port">Server port</param>
        public SocketClient(IPAddress address, int port)
            : this(new[] { address }, port)
        {
        }

        /// <summary>
        /// Initialize an instance of the class. The addresses parameter contains a list
        /// of addresses that the client will attempt to connect to.
        /// </summary>
        /// <param name="addresses">List of server addresses to try connecting to</param>
        /// <param name="port">Server port</param>
        public SocketClient(IPAddress[] addresses, int port)
            : this(port)
        {
            _addressList = addresses;
        }

        /// <summary>
        /// Constructor.  Allocates resources.
        /// </summary>
        /// <param name="hostNameOrAddress">Host name or address. Will try to resolve hostname</param>
        /// <param name="port">server port</param>
        public SocketClient(string hostNameOrAddress, int port)
            : this(port)
        {
            // asynchronously try and resolve the host name
            _resolveHostEvent = new AutoResetEvent(false);
            Dns.BeginGetHostAddresses(hostNameOrAddress, getHostAddressesCallback, null);
        }

        /// <summary>
        /// Constructor.  Allocates resources
        /// </summary>
        /// <param name="port">Server port</param>
        private SocketClient(int port)
        {
            _port = port;
            _tcpClient = new TcpClient();
            Encoding = Encoding.Default;
        }

        /// <summary>
        /// Invoked by ConnectAsync to indicate that an async connection went through
        /// </summary>
        /// <param name="address">Server address</param>
        public delegate void ClientConnectedDelegate(IPAddress address);

        /// <summary>
        /// Invoked by ConnectAsync to indicate there was a connection error
        /// </summary>
        /// <param name="error">Error message</param>
        public delegate void ClientConnectErrorDelegate(String error);

        /// <summary>
        /// Invoked when the client or the server closed the connection
        /// </summary>
        /// <param name="addr">server address</param>
        public delegate void ClientConnectionClosedDelegate(IPAddress addr);

        /// <summary>
        /// Invoked when the client receives data from the server
        /// </summary>
        /// <param name="address">server address</param>
        /// <param name="data">data received</param>
        public delegate void ClientDataReceivedDelegate(IPAddress address, byte[] data);

        /// <summary>
        /// Invoked when there is a read error
        /// </summary>
        /// <param name="address">server address</param>
        /// <param name="error">error message</param>
        public delegate void ClientReadErrorDelegate(IPAddress address, String error);

        /// <summary>
        /// Invoked by WriteAsync to indicate that a async write completed successfully
        /// </summary>
        /// <param name="address">server address</param>
        public delegate void ClientWriteCompleteDelegate(IPAddress address);

        /// <summary>
        /// Invoked when an async write fails
        /// </summary>
        /// <param name="address">Server address</param>
        /// <param name="error">Error message</param>
        public delegate void ClientWriteErrorDelegate(IPAddress address, String error);

        /// <summary>
        /// Event raised by ConnectAsync to indicate that an async connection went through
        /// </summary>
        public event ClientConnectedDelegate OnClientConnected;

        /// <summary>
        /// Raised by ConnectAsync to indicate there was a connection error
        /// </summary>
        public event ClientConnectErrorDelegate OnClientConnectError;

        /// <summary>
        /// Raised when the client or the server closed the connection
        /// </summary>
        public event ClientConnectionClosedDelegate OnClientConnectionClosed;

        /// <summary>
        /// Raised when the client receives data from the server
        /// </summary>
        public event ClientDataReceivedDelegate OnClientDataReceived;

        //public event ClientReadErrorDelegate OnClientReadError;

        /// <summary>
        /// Raised by WriteAsync to indicate that a async write completed successfully
        /// </summary>
        public event ClientWriteCompleteDelegate OnClientWriteComplete;

        /// <summary>
        /// Raised when an async write fails
        /// </summary>
        public event ClientWriteErrorDelegate OnClientWriteError;

        /// <summary>
        /// Raised when there is a read error
        /// </summary>
        ///
#pragma warning disable
#pragma warning enable

        /// <summary>
        /// Set encoding format for data
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// Get/Set the fixed packet size of packets.  If set,
        /// uses the fixed packet protocol where the data being
        /// sent MUST be less than or equal to the packet size and
        /// all packets sent are of this size.
        /// </summary>
        public int FixedPacketSize
        {
            get
            {
                return _fixedPacketSize;
            }
            set
            {
                _fixedPacketSize = value;
            }
        }

        /// <summary>
        /// Gets whether a connection to a server exists
        /// </summary>
        public bool IsConnected
        {
            get { return _tcpClient.Connected; }
        }

        /// <summary>
        /// Synchronously connect to the server
        /// </summary>
        /// <returns>true on success</returns>
        public bool Connect()
        {
            try
            {
                // wait for the host name to resolve
                if (_resolveHostEvent != null)
                {
                    _resolveHostEvent.WaitOne();
                }
            }
            catch (Exception e)
            {
                Utility.Log.Warn("an exception occured!  e=" + e.ToString());
                return false;
            }

            Interlocked.Exchange(ref _connectionAttempts, 0);

            // try to connect to the addresses in the list
            foreach (IPAddress addr in _addressList)
            {
                try
                {
                    _tcpClient.Connect(addr, _port);

                    // success!
                    NetworkStream networkStream = _tcpClient.GetStream();

                    var buffer = new byte[_tcpClient.ReceiveBufferSize];

                    networkStream.BeginRead(buffer, 0, buffer.Length, ReadCallback, buffer);

                    if (OnClientConnected != null)
                    {
                        OnClientConnected(getIPAddress(_tcpClient));
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Utility.Log.Warn("Connect error.  an exception occured!  e=" + ex);
                }
            }
            return false;
        }

        /// <summary>
        /// Asynchronously try and connect to the server.  Events
        /// are raised to indicate success/failure of the connection
        /// </summary>
        public void ConnectAsync()
        {
            try
            {
                // wait for host name resolution
                if (_resolveHostEvent != null)
                {
                    _resolveHostEvent.WaitOne();
                }

                Interlocked.Exchange(ref _connectionAttempts, 0);
                _tcpClient.BeginConnect(_addressList, _port, ConnectCallback, null);
            }
            catch (Exception e)
            {
                if (OnClientConnectError != null)
                {
                    OnClientConnectError(e.ToString());
                }
            }
        }

        /// <summary>
        /// Disposes resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // Prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Synchronously send string data to the server.  Converts the string
        /// into byte sequence and sends it out.
        /// If the FixedPacketSize property is set to > 0, sends a packet
        /// of FixedPacketSize.  In this case, the bytes array of the string
        /// should be less than or equal to FixedPacketSize.
        /// </summary>
        /// <param name="msg">String data to send.</param>
        /// <returns>number of bytes written, -1 on error</returns>
        public int Write(String msg)
        {
            return Write(Encoding.ASCII.GetBytes(msg));
        }

        /// <summary>
        /// Synchronously send byte sequence to the server.  If the
        /// FixedPacketSize property is set to > 0, sends a packet
        /// of FixedPacketSize.  In this case, the bytes array length
        /// should be less than or equal to FixedPacketSize
        /// </summary>
        /// <param name="bytes">data to send</param>
        /// <returns>number of bytes written, -1 on error</returns>
        public int Write(byte[] bytes)
        {
            try
            {
                byte[] buffer = getTransmitBuffer(bytes);
                if (buffer == null)
                {
                    return -1;
                }
                NetworkStream networkStream = _tcpClient.GetStream();
                networkStream.Write(buffer, 0, buffer.Length);
                return bytes.Length;
            }
            catch (Exception e)
            {
                Utility.Log.Warn("exception caught! e=" + e.ToString());
                return -1;
            }
        }

        /// <summary>
        /// Asynchronously send string data to the server.  Converts the string
        /// into byte sequence and sends it out.
        /// If the FixedPacketSize property is set to > 0, sends a packet
        /// of FixedPacketSize.  In this case, the bytes array of the string
        /// should be less than or equal to FixedPacketSize.
        /// Status of the write are notifed through events
        /// </summary>
        /// <param name="msg">String data to send.</param>
        public void WriteAsync(String msg)
        {
            WriteAsync(Encoding.ASCII.GetBytes(msg));
        }

        /// <summary>
        /// Asynchronously send byte sequence to the server.  If the
        /// FixedPacketSize property is set to > 0, sends a packet
        /// of FixedPacketSize.  In this case, the bytes array length
        /// should be less than or equal to FixedPacketSize
        /// Status of the write are notifed through events
        /// </summary>
        /// <param name="bytes">data to send</param>
        /// <returns>number of bytes written, -1 on error</returns>
        public void WriteAsync(byte[] bytes)
        {
            try
            {
                byte[] buffer = getTransmitBuffer(bytes);
                NetworkStream networkStream = _tcpClient.GetStream();
                networkStream.BeginWrite(buffer, 0, buffer.Length, WriteCallback, null);
            }
            catch (Exception e)
            {
                if (OnClientWriteError != null)
                {
                    OnClientWriteError(getIPAddress(_tcpClient), e.ToString());
                }
            }
        }

        /// <summary>
        /// Disposer. Release resources and cleanup.
        /// </summary>
        /// <param name="disposing">true to dispose managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!_disposed)
            {
                if (disposing)
                {
                    // dispose all managed resources.
                    if (_tcpClient != null)
                    {
                        _tcpClient.Close();
                    }
                }

                // Release unmanaged resources.
            }
            _disposed = true;
        }

        /// <summary>
        /// Invoked when async connect completes
        /// </summary>
        /// <param name="result"></param>
        private void ConnectCallback(IAsyncResult result)
        {
            try
            {
                _tcpClient.EndConnect(result);
            }
            catch (Exception e)
            {
                Interlocked.Increment(ref _connectionAttempts);

                if (_connectionAttempts >= _addressList.Length)
                {
                    if (OnClientConnectError != null)
                    {
                        this.OnClientConnectError(e.ToString());
                    }
                }
                return;
            }

            // notify app
            if (OnClientConnected != null)
            {
                OnClientConnected(getIPAddress(_tcpClient));
            }

            NetworkStream networkStream = _tcpClient.GetStream();

            var buffer = new byte[_tcpClient.ReceiveBufferSize];

            networkStream.BeginRead(buffer, 0, buffer.Length, ReadCallback, buffer);
        }

        /// <summary>
        /// Called when DHS host name resolution completes
        /// </summary>
        /// <param name="result"></param>
        private void getHostAddressesCallback(IAsyncResult result)
        {
            _addressList = Dns.EndGetHostAddresses(result);
            ((AutoResetEvent)_resolveHostEvent).Set();
        }

        /// <summary>
        /// Returns the IP address of the server the client
        /// is connected to
        /// </summary>
        /// <param name="tcpClient">TCP client object</param>
        /// <returns>IPAddress of server</returns>
        private IPAddress getIPAddress(TcpClient tcpClient)
        {
            try
            {
                var ipe = (IPEndPoint)tcpClient.Client.LocalEndPoint;
                return IPAddress.Parse(ipe.Address.ToString());
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets fixed size transmit buffer
        /// </summary>
        /// <param name="bytes">input byte seq</param>
        /// <returns>fixed buffer size data</returns>
        private byte[] getTransmitBuffer(byte[] bytes)
        {
            byte[] buffer;
            if (FixedPacketSize > 0)
            {
                if (bytes.Length > FixedPacketSize)
                {
                    return null;
                }

                buffer = new byte[FixedPacketSize];
                Buffer.BlockCopy(bytes, 0, buffer, 0, bytes.Length);
            }
            else
            {
                buffer = bytes;
            }
            return buffer;
        }

        /// <summary>
        /// Invoked when data is received
        /// </summary>
        /// <param name="result"></param>
        private void ReadCallback(IAsyncResult result)
        {
            int read;
            NetworkStream networkStream;

            try
            {
                networkStream = _tcpClient.GetStream();
                read = networkStream.EndRead(result);
            }
            catch (Exception e)
            {
                Utility.Log.Warn("exception caught! e=" + e);

                if (OnClientConnectionClosed != null)
                {
                    OnClientConnectionClosed(getIPAddress(_tcpClient));
                }
                //OnClientReadError(getIPAddress(tcpClient), e.ToString());
                return;
            }

            // connection was closed
            if (read == 0)
            {
                if (OnClientConnectionClosed != null)
                {
                    OnClientConnectionClosed(getIPAddress(_tcpClient));
                }
                return;
            }

            var buffer = result.AsyncState as byte[];

            if (OnClientDataReceived != null)
            {
                OnClientDataReceived(getIPAddress(_tcpClient), buffer);
            }

            // start next read
            networkStream.BeginRead(buffer, 0, buffer.Length, ReadCallback, buffer);
        }

        /// <summary>
        /// Invoked when an async write operation completes
        /// </summary>
        /// <param name="result"></param>
        private void WriteCallback(IAsyncResult result)
        {
            try
            {
                NetworkStream networkStream = _tcpClient.GetStream();
                networkStream.EndWrite(result);
            }
            catch (Exception e)
            {
                if (OnClientWriteError != null)
                {
                    OnClientWriteError(getIPAddress(_tcpClient), e.ToString());
                }
                return;
            }
            if (OnClientWriteComplete != null)
            {
                OnClientWriteComplete(getIPAddress(_tcpClient));
            }
        }
    }
}