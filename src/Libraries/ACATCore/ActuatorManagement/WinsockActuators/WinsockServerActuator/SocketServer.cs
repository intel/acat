////////////////////////////////////////////////////////////////////////////
// <copyright file="SocketServer.cs" company="Intel Corporation">
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
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
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
    /// Represents the TCP socket listener to wait for incoming
    /// connections.  Receives data from the client, initiates
    /// parsing and processing of the conversion.
    /// </summary>
    public class SocketServer
    {
        public delegate void OnClientConnectedDelegate(object sender, WinsockClientConnectEventArgs e);

        public delegate void OnClientDisconnectedDelegate(object sender, WinsockClientConnectEventArgs e);

        public delegate void OnPacketReceivedDelegate(byte[] packet);

        public delegate void OnSendMsgHandler(object sender, byte[] byteData, int count);

#pragma warning disable

        public delegate void OnServerShutdownHandler();

        private delegate void OnClientListUpdatedHandler(ClientConnHandler clientHandler);

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

        private event OnClientListUpdatedHandler OnClientListUpdated;

        public ListDictionary clientList = new ListDictionary();
        private String ipToBind;
        private Thread listenThread;
        private Thread parentThread;
        private int portToBind;
        private TcpListener tcpListener;
        private Thread workThread;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="listenPort">TCP port to listen on</param>
        public SocketServer(int listenPort)
            : this(String.Empty, listenPort)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SocketServer(String listenAddress, int listenPort)
        {
            ipToBind = listenAddress;
            portToBind = listenPort;
            parentThread = Thread.CurrentThread;
        }

        public static IPAddress GetIPToBind()
        {
            return GetIPToBind(String.Empty);
        }

        /// <summary>
        /// Gets the IP Address to try to bind to.  First tries to bind to address from AppConfig.xml in the key
        /// BlueBendServer_IPAddress.  If key is missing/null/blank or if the machine doesn't have the specified IP address
        /// bound to the network card, the default (localhost) IP Address is selected.
        /// </summary>
        /// <returns>IP Address to bind to.</returns>
        public static IPAddress GetIPToBind(String configAddr)
        {
            string host = Dns.GetHostName();
            IPHostEntry he = Dns.GetHostEntry(host);
            IPAddress[] hostIPs = he.AddressList;

            //  If address in configuration file is null, == localhost or == 127.0.0.1
            //  return the Loopback address as the IP Address to bind to.
            if (String.IsNullOrEmpty(configAddr) ||
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

        public void Send(byte[] byteData, int count)
        {
            /*
            if (SpeechTextService.OnSendToClient != null)
            {
                Debug.WriteLine(String.Format("Send {0} bytes to client.", count));
                OnSendToClient.Invoke(null, byteData, count);
            }*/
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
            workThread = new Thread(serverThreadMethod) {IsBackground = true};
            try
            {
                workThread.Start();
            }
            catch (Exception exc)
            {
                Log.Error("Couldn't start the SocketServer main working thread." + exc.StackTrace);
                workThread = null;
            }

            return parentThread;
        }

        /// <summary>
        /// Stops the main service thread, hopefully stopping the process.
        /// </summary>
        public void Stop()
        {
            Log.Debug("SERVER: Stopping listeners until service is started again");
            try
            {
                tcpListener.Stop();
            }
            catch (SocketException se)
            {
                Log.Debug(se.ToString());
            }
            catch (Exception e)
            {
                Log.Debug(e.ToString());
            }
            if (listenThread != null)
            {
                listenThread.Abort();
                listenThread = null;
            }
            if (workThread != null)
            {
                workThread.Abort();
                workThread = null;
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
                this.OnClientDisconnected(this, new WinsockClientConnectEventArgs(clientConn.ID, clientConn.ClientIPAddress));
                clientConn.OnClientConnStatusChanged -= connHandler_OnClientConnStatusChanged;
                clientList.Remove(clientConn.ID);
            }
            else if (status == ClientConnHandler.ConnectionStatus.Connected)
            {
                OnClientConnected(this, new WinsockClientConnectEventArgs(clientConn.ID, clientConn.ClientIPAddress));
                clientList.Add(clientConn.ID, clientConn);
            }

            if (OnClientListUpdated != null)
            {
                OnClientListUpdated.Invoke(clientConn);
            }
        }

        private void connHandler_OnPacketReceived(byte[] packet)
        {
            OnPacketReceived(packet);
        }

        /// <summary>
        /// Accept all incoming requests and create a handler thread to deal with the communication.
        /// </summary>
        private void ListenForClients()
        {
            Log.Debug("SocketServer: Listener Thread");
            try
            {
                this.tcpListener.Start();
            }
            catch (System.Net.Sockets.SocketException se)
            {
                //Log.Error(se.StackTrace);
                Log.Error(se.StackTrace);
                // se.ErrorCode == 10048, this condition means that more than one process is attempting to bind to same port, disallowed.
                //                Log.Write(String.Format("SocketException: NativeError:{0} ErrorCode:{1}, Msg:{2}", se.NativeErrorCode, se.ErrorCode, se.Message));
                //                Log.Debug("Socket Exception Listening for clients", se);
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
                    String strAddr = addr.ToString();

                    Log.Debug("Client " + strAddr + " has connected");

                    // Create a thread to handle communication with a connected client.
                    // The ClientConnHandler is an object that allows a way to pass data to a thread.  The TcpClient is passed to the
                    // object constructor and a thread is started with the objects worker method... allowing the thread access to the client.
                    var connHandler = new ClientConnHandler(client);
                    connHandler.OnPacketReceived += connHandler_OnPacketReceived;
                    if (!String.IsNullOrEmpty(connHandler.ID))
                    {
                        connHandler.OnClientConnStatusChanged += connHandler_OnClientConnStatusChanged;
                        connHandler.WorkerThread = new Thread(connHandler.WorkerThreadMethod) {IsBackground = true};
                        connHandler.WorkerThread.Start();
                    }
                }
                catch (SocketException se)
                {
                    Log.Error(se.StackTrace);
                }
            }
        }

        /// <summary>
        /// Main worker thread for this process.
        /// </summary>
        private void serverThreadMethod()
        {
            IPAddress ipaddr = GetIPToBind(ipToBind);
            int port = GetPortToBind();

            // TODO: Support selecting ports
            tcpListener = new TcpListener(IPAddress.Any, port);
            listenThread = new Thread(ListenForClients) {IsBackground = true};
            try
            {
                listenThread.Start();
            }
            catch (SocketException se)
            {
                Log.Error("SERVER: Couldn't start TCP Listener, exiting the app. " + se.StackTrace);
                return;
            }
            catch (Exception exc)
            {
                // Need to stop the service somehow.  A caught exception will stop
                // the service.  So, just throw the exception
                throw exc;
            }
            string startMessage = String.Format("SERVER: Listener Started on {0}:{1}",
                ((IPEndPoint)tcpListener.LocalEndpoint).Address,
                ((IPEndPoint)tcpListener.LocalEndpoint).Port);
            Log.Debug(startMessage);

            parentThread.Join(); //  A way to get current thread to wait until a thread is complete before continuing.
        }
    }
}