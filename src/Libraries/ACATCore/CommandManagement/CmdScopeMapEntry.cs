////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
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