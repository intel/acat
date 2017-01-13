////////////////////////////////////////////////////////////////////////////
// <copyright file="CmdDescriptor.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2017 Intel Corporation 
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