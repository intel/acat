////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Lib.Core.CommandManagement
{
    /// <summary>
    /// Holds a command and its user-friendly description
    /// </summary>
    [Serializable]
    public class CmdDescriptor
    {
        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public CmdDescriptor()
        {
            Command = String.Empty;
            Description = String.Empty;
        }

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="description">Its description</param>
        public CmdDescriptor(String command, string description = "", bool enableSwitchMap = false)
        {
            Command = command;
            Description = description;
            EnableSwitchMap = enableSwitchMap;
        }

        /// <summary>
        /// Gets or sets the command
        /// </summary>
        public String Command { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// Gets or sets whether this command can be mapped
        /// to a switch or not
        /// </summary>
        public bool EnableSwitchMap { get; set; }
    }
}