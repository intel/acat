////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// SocketServer.cs
//
// Represents the TCP socket listener to wait for incoming
// connections.  Receives data from the client, initiates
// parsing and processing of the conversion.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ACAT.Core.ActuatorManagement.WinsockActuators.WinsockServerActuator
{
    /// <summary>
    /// Represents the TCP socket listener to wait for incoming
    /// connections.  Receives data from the client, initiates
    /// parsing and processing of the conversion.
    /// </summary>
    public class SocketServer
    {
        private readonly ILogger<SocketServer> _logger;

        /// <summary>
        /// List of clients connected to this socket server
        /// </summary>
        public ListDictionary clientList = new();

        /// <summary>
        /// IP address to bind to
        /// </summary>
        private readonly string ipToBind;

        /// <summary>
        /// The socket listener thread
        /// </summary>
        private Thread listenThread;

        /// <summary>
        /// The parent thread
        /// </summary>
        private readonly Thread parentThread;

        /// <summary>
        /// Port to listen on
        /// </summary>
        private readonly int portToBind;

        /// <summary>
        /// The tcp listnener object
        /// </summary>
        private TcpListener tcpListener;

        /// <summary>
        /// Thread that listens for incoming connections
        /// </summary>
        private Thread workerThread;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="listenPort">TCP port to listen on</param>
        public SocketServer(int listenPort)
            : this(string.Empty, listenPort)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SocketServer(string listenAddress, int listenPort, ILogger<SocketServer> logger = null)
        {
            _logger = logger;
            ipToBind = listenAddress;
            portToBind = listenPort;
            parentThread = Thread.CurrentThread;
        }

        public delegate void OnClientConnectedDelegate(object sender, WinsockClientConnectEventArgs e);

        public delegate void OnClientDisconnectedDelegate(object sender, WinsockClientConnectEventArgs e);

        public delegate void OnPacketReceivedDelegate(byte[] packet);

        public delegate void OnSendMsgHandler(object sender, byte[] byteData, int count);

#pragma warning disable

        public delegate void OnServerShutdownHandler();

        public static event OnSendMsgHandler OnSendToClient;

#pragma warning restore
        /// <summary>
        /// Fires when server is stopping. Child threads should listen for this and stop.
        /// </summary>

#pragma warning disable

        public static event OnServerShutdownHandler OnServerShutdown;

#pragma warning enable

        public event OnClientConnectedDelegate OnClientConnected;

        public event OnClientDisconnectedDelegate OnClientDisconnected;

        public event OnPacketReceivedDelegate OnPacketReceived;

        public static IPAddress GetIPToBind()
        {
            return GetIPToBind(string.Empty);
        }

        /// <summary>
        /// Gets the IP Address to try to bind to.  First tries to bind to address from AppConfig.xml in the key
        /// BlueBendServer_IPAddress.  If key is missing/null/blank or if the machine doesn't have the specified IP address
        /// bound to the network card, the default (localhost) IP Address is selected.
        /// </summary>
        /// <returns>IP Address to bind to.</returns>
        public static IPAddress GetIPToBind(string configAddr)
        {
            string host = Dns.GetHostName();
            IPHostEntry he = Dns.GetHostEntry(host);
            IPAddress[] hostIPs = he.AddressList;

            //  If address in configuration file is null, == localhost or == 127.0.0.1
            //  return the Loopback address as the IP Address to bind to.
            if (string.IsNullOrEmpty(configAddr) ||
                configAddr.Equals("localhost", StringComparison.InvariantCultureIgnoreCase) ||
                configAddr.Equals("127.0.0.1", StringComparison.InvariantCultureIgnoreCase))
            {
                return IPAddress.Loopback;
            }

            // check to see if the device has bound the IP address the config entry wants to bind to
            // If so, return that address as the address to bind, otherwise, return the loopback address.
            foreach (IPAddress addr in hostIPs)
            {
                //Log.Write("HostIP = " + addr.ToString());
                if (addr.ToString().Equals(configAddr, StringComparison.InvariantCultureIgnoreCase))
                {
                    return addr;
                }
            }
            return IPAddress.Loopback;
        }

        /// <summary>
        /// Gets the IP port to bind to for the instance of this service to listen on.
        /// </summary>
        /// <returns>Port for the service to bind to.</returns>
        public int GetPortToBind()
        {
            return portToBind;
        }

        /// <summary>
        /// Sends data to connected client
        /// </summary>
        /// <param name="clientid">client id</param>
        /// <param name="byteData">data to send</param>
        /// <param name="count">how many bytes</param>
        public void Send(string clientid, byte[] byteData, int count)
        {
            if (clientList.Contains(clientid))
            {
                var client = (ClientConnHandler)clientList[clientid];
                client.SendToClient(byteData, count);
            }
        }

        /// <summary>
        /// Starts the main server thread, returns a reference to the main service thread that is in a Join
        /// state.  Message pumps still process and child threads running.  Hopefully, the main thread can
        /// be aborted by a control application.  :)
        /// </summary>
        /// <returns>Reference to main service thread in Join state.</returns>
        public Thread Start()
        {
            // Set up the service thread for most of the real work.  :)
            workerThread = new Thread(serverThreadMethod) { IsBackground = true };
            try
            {
                workerThread.Start();
            }
            catch (Exception exc)
            {
                _logger?.LogError(exc, "Couldn't start the SocketServer main working thread");
                workerThread = null;
            }

            return parentThread;
        }

        /// <summary>
        /// Stops the main service thread, hopefully stopping the process.
        /// </summary>
        public void Stop()
        {
            _logger?.LogDebug("SERVER: Stopping listeners until service is started again");
            try
            {
                tcpListener.Stop();
            }
            catch (SocketException se)
            {
                _logger?.LogError(se, "Socket exception while stopping TCP listener");
            }
            catch (Exception e)
            {
                _logger?.LogError(e, "Exception while stopping TCP listener");
            }
            if (listenThread != null)
            {
                listenThread.Abort();
                listenThread = null;
            }
            if (workerThread != null)
            {
                workerThread.Abort();
                workerThread = null;
            }
        }

        /// <summary>
        /// Client connection status changed
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="status">status of the connection</param>
        private void connHandler_OnClientConnStatusChanged(object sender, ClientConnHandler.ConnectionStatus status)
        {
            ClientConnHandler clientConn = (ClientConnHandler)sender;
            if (status == ClientConnHandler.ConnectionStatus.Disconnected)
            {
                OnClientDisconnected(this, new WinsockClientConnectEventArgs(clientConn.ID, clientConn.ClientIPAddress));
                clientConn.OnClientConnStatusChanged -= connHandler_OnClientConnStatusChanged;
                clientList.Remove(clientConn.ID);
            }
            else if (status == ClientConnHandler.ConnectionStatus.Connected)
            {
                OnClientConnected(this, new WinsockClientConnectEventArgs(clientConn.ID, clientConn.ClientIPAddress));
                clientList.Add(clientConn.ID, clientConn);
            }
        }

        /// <summary>
        /// Invoked when data is received
        /// </summary>
        /// <param name="packet">the data received</param>
        private void connHandler_OnPacketReceived(byte[] packet)
        {
            OnPacketReceived(packet);
        }

        /// <summary>
        /// Accepts all incoming requests and creates a handler thread
        /// to deal with the communication.
        /// </summary>
        private void ListenForClients()
        {
            _logger?.LogDebug("SocketServer: Listener Thread started");
            try
            {
                tcpListener.Start();
            }
            catch (SocketException se)
            {
                _logger?.LogError(se, "Socket exception while starting TCP listener (ErrorCode: {ErrorCode})", se.ErrorCode);
                listenThread.Abort();
                return;
            }

            while (true)
            {
                //blocks until a client has connected to the server
                try
                {
                    TcpClient client = tcpListener.AcceptTcpClient();

                    IPEndPoint ipe = (IPEndPoint)client.Client.LocalEndPoint;
                    IPAddress addr = IPAddress.Parse(ipe.Address.ToString());
                    string strAddr = addr.ToString();

                    _logger?.LogDebug("Client {ClientAddress} has connected", strAddr);

                    // Create a thread to handle communication with a connected client.
                    // The ClientConnHandler is an object that allows a way to pass data to a thread.  The TcpClient is passed to the
                    // object constructor and a thread is started with the objects worker method... allowing the thread access to the client.
                    var connHandler = new ClientConnHandler(client);
                    connHandler.OnPacketReceived += connHandler_OnPacketReceived;
                    if (!string.IsNullOrEmpty(connHandler.ID))
                    {
                        connHandler.OnClientConnStatusChanged += connHandler_OnClientConnStatusChanged;
                        connHandler.WorkerThread = new Thread(connHandler.WorkerThreadMethod) { IsBackground = true };
                        connHandler.WorkerThread.Start();
                    }
                }
                catch (SocketException se)
                {
                    _logger?.LogError(se, "Socket exception while accepting client connection");
                }
            }
        }

        /// <summary>
        /// Main worker thread for this process.
        /// </summary>
        private void serverThreadMethod()
        {
            int port = GetPortToBind();

            tcpListener = new TcpListener(IPAddress.Any, port);
            listenThread = new Thread(ListenForClients) { IsBackground = true };
            try
            {
                listenThread.Start();
            }
            catch (SocketException se)
            {
                _logger?.LogError(se, "SERVER: Couldn't start TCP Listener, exiting the app");
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            string startMessage = string.Format("SERVER: Listener Started on {0}:{1}",
                ((IPEndPoint)tcpListener.LocalEndpoint).Address,
                ((IPEndPoint)tcpListener.LocalEndpoint).Port);
            _logger?.LogDebug("{StartMessage}", startMessage);

            parentThread.Join(); //  A way to get current thread to wait until a thread is complete before continuing.
        }
    }
}