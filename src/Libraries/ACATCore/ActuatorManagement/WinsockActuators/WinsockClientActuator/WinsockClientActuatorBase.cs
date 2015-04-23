////////////////////////////////////////////////////////////////////////////
// <copyright file="WinsockClientActuatorBase.cs" company="Intel Corporation">
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
using System.Text;
using System.Threading;
using ACAT.Lib.Core.ActuatorManagement;
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
    /// Represents the base class for an actuator that receives
    /// trigger events from a TCP/IP socket client.  ACAT in this case
    /// acts as a client and the source app for the trigger acts as
    /// the TCP server. Does a lot of the heavy lifting such as trying
    /// to connect to the server, receiving and parsing the data packet
    /// and raising events for switch triggers. Data packet is a string.
    /// Refer to WinsockCommon class for the data format.
    /// </summary>
    ///
    public class WinsockClientActuatorBase : ActuatorBase
    {
        /// <summary>
        /// Socket client class that handles the client connection
        /// </summary>
        protected SocketClient socketClient;

        /// <summary>
        /// How long to wait between retries to connect to the socket
        /// server
        /// </summary>
        protected int waitInterval = 3000;

        /// <summary>
        /// Used for connect retries
        /// </summary>
        private readonly ManualResetEvent _evtConnectRetry = new ManualResetEvent(false);

        /// <summary>
        /// Thread used to try to connect to the server
        /// </summary>
        private Thread _connectThread;

        /// <summary>
        /// Has this object been disposed?
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Set to stop trying to connect
        /// </summary>
        private volatile bool _quitConnect;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public WinsockClientActuatorBase()
        {
            socketClient = null;
        }

        /// <summary>
        /// Gets whether the socket client is connected to a server
        /// </summary>
        public bool IsConnected
        {
            get { return socketClient != null && socketClient.IsConnected; }
        }

        /// <summary>
        /// Gets or sets the IP address of the server
        /// </summary>
        public String ServerAddress { get; set; }

        /// <summary>
        /// Gets or sets the TCPIP port of the server
        /// </summary>
        public int ServerPort { get; set; }

        /// <summary>
        /// Class factory to create the winsock client actuator switch
        /// </summary>
        /// <returns>Winsock client switch object</returns>
        public override IActuatorSwitch CreateSwitch()
        {
            return new WinsockSwitch();
        }

        /// <summary>
        /// Class factory to create the winsock client actuator switch which is a
        /// clone of the switch specified
        /// </summary>
        /// <param name="sourceSwitch">source switch to clone</param>
        /// <returns>Winsock  switch object</returns>
        public virtual IActuatorSwitch CreateSwitch(IActuatorSwitch sourceSwitch)
        {
            return new WinsockSwitch(sourceSwitch);
        }

        /// <summary>
        /// Performs initialization
        /// </summary>
        /// <returns>true on success</returns>
        public override bool Init()
        {
            initSocketInterface();

            return true;
        }

        /// <summary>
        /// Pauses the actuator
        /// </summary>
        public override void Pause()
        {
            actuatorState = State.Paused;
        }

        /// <summary>
        /// Resumes paused actuator
        /// </summary>
        public override void Resume()
        {
            actuatorState = State.Running;
        }

        /// <summary>
        /// Disposes resources
        /// </summary>
        /// <param name="disposing">true to dispose managed resources</param>
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                try
                {
                    Log.Debug();

                    _quitConnect = true;
                    _evtConnectRetry.Set();
                    if (_connectThread != null)
                    {
                        _connectThread.Join(2000);
                    }

                    if (disposing)
                    {
                        // release managed resources
                        unInit();
                    }

                    // Release the native unmanaged resources
                    _disposed = true;
                }
                finally
                {
                    // Call Dispose on your base class.
                    base.Dispose(disposing);
                }
            }
        }

        /// <summary>
        /// Invoked when successfully connected to the tcpip server
        /// </summary>
        /// <param name="address">Address of the server</param>
        protected virtual void onConnected(System.Net.IPAddress address)
        {
        }

        /// <summary>
        /// Invoked when data is received over the socket.  The byte
        /// array is converted into a string, parsed and an event raised to
        /// indiate a switch was triggered
        /// </summary>
        /// <param name="address">IP address of the server sending the data</param>
        /// <param name="data">The data itself</param>
        protected virtual void onDataReceived(System.Net.IPAddress address, byte[] data)
        {
            String strData = ASCIIEncoding.ASCII.GetString(data, 0, data.Length);
            Log.Debug("Received data: " + strData);

            // parse the string, find the switch that causes the trigger
            IActuatorSwitch switchObj = WinsockCommon.parseAndGetSwitch(strData, Switches, CreateSwitch);
            if (switchObj != null)
            {
                triggerEvent(switchObj);
            }
        }

        /// <summary>
        /// Invoked when a disconnect occurs.
        /// </summary>
        /// <param name="address">Address of the server</param>
        protected virtual void onDisconnected(System.Net.IPAddress address)
        {
        }

        /// <summary>
        /// Send data over socket
        /// </summary>
        /// <param name="msg">msg to send</param>
        /// <returns>true on sucess</returns>
        protected int Send(String msg)
        {
            try
            {
                if (socketClient.IsConnected)
                {
                    return socketClient.Write(msg);
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return -1;
        }

        /// <summary>
        /// Depending on the switch action, raised events such as
        /// switch engaged, switch disengaged etc
        /// </summary>
        /// <param name="switchObj"></param>
        protected void triggerEvent(IActuatorSwitch switchObj)
        {
            switch (switchObj.Action)
            {
                case SwitchAction.Down:
                    OnSwitchActivated(switchObj);
                    break;

                case SwitchAction.Up:
                    OnSwitchDeactivated(switchObj);
                    break;

                case SwitchAction.Trigger:
                    OnSwitchTriggered(switchObj);
                    break;
            }
        }

        /// <summary>
        /// Event handler invoked when the client connects to the server
        /// </summary>
        /// <param name="address">Tcp/ip server address</param>
        private void _socketClient_OnClientConnected(System.Net.IPAddress address)
        {
            Log.Debug("Connected to" + address);
            onConnected(address);
        }

        /// <summary>
        /// Event handler invoked when socket client disconnects.
        /// </summary>
        /// <param name="addr">Address of the tcp/ip server</param>
        private void _socketClient_OnClientConnectionClosed(System.Net.IPAddress addr)
        {
            Log.Debug("Disconnected from " + addr);
            closeClientSocket(socketClient);
            socketClient = null;
            _evtConnectRetry.Set();

            onDisconnected(addr);
        }

        /// <summary>
        /// Event handler invoked when data is received over the socket
        /// </summary>
        /// <param name="address">Server address</param>
        /// <param name="data">The data received</param>
        private void _socketClient_OnClientDataReceived(System.Net.IPAddress address, byte[] data)
        {
            onDataReceived(address, data);
        }

        /// <summary>
        /// Closes the client socket. Unsunscribes from events
        /// </summary>
        /// <param name="client"></param>
        private void closeClientSocket(SocketClient client)
        {
            if (client == null)
            {
                return;
            }
            client.OnClientDataReceived -= _socketClient_OnClientDataReceived;
            client.OnClientConnected -= _socketClient_OnClientConnected;
            client.OnClientConnectionClosed -= _socketClient_OnClientConnectionClosed;
        }

        /// <summary>
        /// Thread proc which tries to connect to the tcpip server.  If it cannot
        /// connect, it sleeps for sometime and retries
        /// </summary>
        private void connectProc()
        {
            while (!_quitConnect)
            {
                try
                {
                    Log.Debug("Trying to connecting to tcp/ip server");
                    if (socketClient == null)
                    {
                        socketClient = createSocketClient();
                    }

                    if (socketClient != null)
                    {
                        if (!socketClient.Connect())
                        {
                            Log.Debug("Tcp/ip server not detected.  Will retry...");
                            Thread.Sleep(waitInterval);
                        }
                        else
                        {
                            Log.Debug("Successfully connected to tcp/ip server");
                            _evtConnectRetry.WaitOne();
                            _evtConnectRetry.Reset();
                        }
                    }
                    else
                    {
                        Thread.Sleep(waitInterval);
                    }
                }
                catch (Exception ex)
                {
                    Log.Debug("Error connecting to server " + ex);
                }
            }

            Log.Debug("Thread exited");
        }

        /// <summary>
        /// Creates a socket client object and subscribes to events for
        /// connect, disconnet and data receive
        /// </summary>
        /// <returns>Socket client object</returns>
        private SocketClient createSocketClient()
        {
            SocketClient client = null;

            if (ServerAddress != null && ServerPort >= 0)
            {
                System.Net.IPAddress ipaddress = System.Net.IPAddress.Parse(ServerAddress);

                client = new SocketClient(ipaddress, ServerPort);
                client.OnClientDataReceived += _socketClient_OnClientDataReceived;
                client.OnClientConnected += _socketClient_OnClientConnected;
                client.OnClientConnectionClosed += _socketClient_OnClientConnectionClosed;
            }

            return client;
        }

        /// <summary>
        /// Starts a thread which will attempt to connect to the tcpip server
        /// </summary>
        private void initSocketInterface()
        {
            _connectThread = new Thread(connectProc);
            _connectThread.Start();
        }

        /// <summary>
        /// Uninitialize.  Stop socket server
        /// </summary>
        /// <returns>true</returns>
        private void unInit()
        {
            if (socketClient != null)
            {
                socketClient.Dispose();
                socketClient = null;
            }
        }
    }
}