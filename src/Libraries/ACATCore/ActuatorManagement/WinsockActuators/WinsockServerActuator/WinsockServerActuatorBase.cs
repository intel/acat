////////////////////////////////////////////////////////////////////////////
// <copyright file="WinsockServerActuatorBase.cs" company="Intel Corporation">
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
    /// Represents the base class for an actuator receives switch triggers from the a
    /// TCP client. Parses the received data and then raises switch trigger events.
    /// Data is a string and the format is listed in the documentation for the
    /// WinsockCommon class.
    /// </summary>
    public class WinsockServerActuatorBase : ActuatorBase
    {
        /// <summary>
        /// Has this object been disposed?
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// TCP socket server that listens for connections
        /// </summary>
        ///
        private SocketServer _socketServer;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public WinsockServerActuatorBase()
        {
            _socketServer = null;
        }

        /// <summary>
        /// Gets or sets the port number to listen on
        /// </summary>
        public int ServerListenPort { get; set; }

        /// <summary>
        /// Class factory to create an actuator switch object
        /// </summary>
        /// <returns>The winsock switch object</returns>
        public override IActuatorSwitch CreateSwitch()
        {
            return new WinsockSwitch();
        }

        /// <summary>
        /// Class factory to create the winsock client actuator switch which is a
        /// clone of the switch specified
        /// </summary>
        /// <param name="sourceSwitch">source switch to clone</param>
        /// <returns>Winsock switch object</returns>
        public virtual IActuatorSwitch CreateSwitch(IActuatorSwitch sourceSwitch)
        {
            return new WinsockSwitch(sourceSwitch);
        }

        /// <summary>
        /// Perform initialization - allocate socket server, subscribe
        /// to events
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
        /// Dispose resources
        /// </summary>
        /// <param name="disposing">true to dispose managed resources</param>
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                try
                {
                    Log.Debug();

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
        /// Invoked when a client connects
        /// </summary>
        /// <param name="e">event args</param>
        protected virtual void onClientConnected(WinsockClientConnectEventArgs e)
        {
        }

        /// <summary>
        /// Invoked when a client disconnects
        /// </summary>
        /// <param name="e">event args</param>
        protected virtual void onClientDisconnected(WinsockClientConnectEventArgs e)
        {
        }

        /// <summary>
        /// Invoked when data is received.  Converted to string
        /// and parsed.  After parsing, switch trigger event is raised
        /// </summary>
        /// <param name="packet"></param>
        protected virtual void onDataReceived(byte[] packet)
        {
            String strData = ASCIIEncoding.ASCII.GetString(packet, 0, packet.Length);
            Log.Debug("Received data: " + strData);

            // parse the string, find the switch that causes the trigger
            IActuatorSwitch switchObj = WinsockCommon.parseAndGetSwitch(strData, Switches, CreateSwitch);
            if (switchObj != null)
            {
                triggerEvent(switchObj);
            }
        }

        /// <summary>
        /// Depending on the action, invokes events
        /// </summary>
        /// <param name="switchObj">The object that raised the event</param>
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
        /// Event handler for client connect
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arg</param>
        private void _socketServer_OnClientConnected(object sender, WinsockClientConnectEventArgs e)
        {
            Log.Debug("ImageActuator:  Client disconnected " + e.IPAddress);
            onClientConnected(e);
        }

        /// <summary>
        /// Event handler for client disconnect
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event argument</param>
        private void _socketServer_OnClientDisconnected(object sender, WinsockClientConnectEventArgs e)
        {
            Log.Debug("ImageActuator:  Client connected " + e.IPAddress);
            onClientDisconnected(e);
        }

        /// <summary>
        /// Event handler for handling a incoming packet from
        /// the tcp client
        /// </summary>
        /// <param name="packet"></param>
        private void _socketServer_OnPacketReceived(byte[] packet)
        {
            onDataReceived(packet);
        }

        /// <summary>
        /// Initializes the socket server object and subscribes to events
        /// </summary>
        private void initSocketInterface()
        {
            if (ServerListenPort > 0)
            {
                _socketServer = new SocketServer(ServerListenPort);

                _socketServer.OnPacketReceived += _socketServer_OnPacketReceived;
                _socketServer.OnClientConnected += _socketServer_OnClientConnected;
                _socketServer.OnClientDisconnected += _socketServer_OnClientDisconnected;
                _socketServer.Start();
            }
            else
            {
                Log.Debug("Listen error.  Listen port not set");
            }
        }

        /// <summary>
        /// Uninitialize.  Stop socket server
        /// </summary>
        /// <returns>true</returns>
        private void unInit()
        {
            if (_socketServer != null)
            {
                _socketServer.Stop();
                _socketServer = null;
            }
        }
    }
}