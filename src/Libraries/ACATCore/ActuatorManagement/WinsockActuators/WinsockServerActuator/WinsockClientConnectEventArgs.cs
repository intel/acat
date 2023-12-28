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

namespace ACAT.Lib.Core.InputActuators
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
        public WinsockClientConnectEventArgs(String id, String ipAddress)
        {
            Id = id;
            IPAddress = ipAddress;
        }

        /// <summary>
        /// Gets or sets the client id
        /// </summary>
        public String Id { get; private set; }

        /// <summary>
        /// Gets or sets the ip address
        /// </summary>
        public String IPAddress { get; private set; }
    }
}