////////////////////////////////////////////////////////////////////////////
// <copyright file="CameraActuator.cs" company="Intel Corporation">
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
using ACAT.Lib.Core.InputActuators;
using ACAT.Lib.Core.UserManagement;
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

namespace ACAT.Extensions.Hawking.Actuators.Camera
{
    /// <summary>
    /// Controls the ACAT vision software.  Hides the vision software
    /// window on connect and shuts it down on exit. All the heavy
    /// lifting is done by the base class.
    /// </summary>
    [DescriptorAttribute("A5B9BFF9-0A35-41AC-8989-69A3D60F4435", "Camera Actuator - Winsock Client", "Activate switches over TCP/IP. ACAT is the TCP client")]
    public class CameraActuator : WinsockClientActuatorBase
    {
        /// <summary>
        /// Settings for this actuator
        /// </summary>
        public static Settings CameraActuatorSettings;

        /// <summary>
        /// Name of the settings file
        /// </summary>
        private const String SettingsFileName = "CameraActuatorSettings.xml";

        /// <summary>
        /// Has this object been disposed?
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="CameraActuator" /> class.
        /// </summary>
        public CameraActuator()
        {
            CameraActuatorSettings = new Settings();
        }

        /// <summary>
        /// Perform initialization. Hides vision software windows
        /// </summary>
        /// <returns>true on success</returns>
        public override bool Init()
        {
            Settings.SettingsFilePath = UserManager.GetFullPath(SettingsFileName);
            CameraActuatorSettings = Settings.Load();

            ServerAddress = CameraActuatorSettings.SocketClientConnectToAddress;
            ServerPort = CameraActuatorSettings.SocketClientConnectToPort;

            IntPtr handle = User32Interop.FindWindow(null, CameraActuatorSettings.ACATVisionWindowName);
            if (handle != IntPtr.Zero)
            {
                Windows.MinimizeWindow(handle);
            }

            return base.Init();
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

                    if (IsConnected)
                    {
                        Send("action=EXITAPP");
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
        /// Invoked when successfully connected to the vision software. Hide
        /// the window
        /// </summary>
        /// <param name="address">Address of the server</param>
        protected override void onConnected(System.Net.IPAddress address)
        {
            Send("action=HIDEGUI");
        }
    }
}