////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// WinsockClientConnectEventArgs.cs
//
// Argument for the event raised when a TCP/IP client connects
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Core.ActuatorManagement.WinsockActuators.WinsockServerActuator
{
    /// <summary>
    /// Argument for the event raised when a TCP/IP client connects
    /// </summary>
    public class WinsockClientConnectEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="id">Client ID</param>
        /// <param name="ipAddress">IP address of the client</param>
        public WinsockClientConnectEventArgs(string id, string ipAddress)
        {
            Id = id;
            IPAddress = ipAddress;
        }

        /// <summary>
        /// Gets or sets the client id
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets or sets the ip address
        /// </summary>
        public string IPAddress { get; private set; }
    }
}