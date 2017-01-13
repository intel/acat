////////////////////////////////////////////////////////////////////////////
// <copyright file="CmdScopeMapEntry.cs" company="Intel Corporation">
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
using System.Collections.Generic;
using System.Linq;

namespace ACAT.Lib.Core.CommandManagement
{
    /// <summary>
    /// Represents a mapping between a Command and the scopes
    /// for which it is valid
    /// </summary>
    [Serializable]
    public class CmdScopeMapEntry
    {
        /// <summary>
        /// List of command scopes
        /// </summary>
        public List<CmdScope> CmdScopes;

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public CmdScopeMapEntry()
        {
            Command = String.Empty;
            Enabled = false;
            CmdScopes = new List<CmdScope>();
        }

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="enabled">Enabled or not?</param>
        public CmdScopeMapEntry(String command, bool enabled = true)
        {
            Command = command;
            Enabled = enabled;
            CmdScopes = new List<CmdScope>();
        }

        public CmdScopeMapEntry(String command, bool enabled = true, IEnumerable<CmdScope> scopes = null)
        {
            Command = command;
            Enabled = enabled;

            CmdScopes = (scopes != null) ? scopes.ToList() : new List<CmdScope>();
        }

        /// <summary>
        /// Gets or sets the command
        /// </summary>
        public String Command { get; set; }

        /// <summary>
        /// Gets or sets whether this is enabled or not
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Adds the specified scope to the list
        /// </summary>
        /// <param name="cmdScope">cmdscope object to add</param>
        public void Add(CmdScope cmdScope)
        {
            CmdScopes.Add(cmdScope);
        }
    }
}