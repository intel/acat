////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.AgentManagement;
using ACAT.Core.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ACAT.Extension.AppAgents.ACATApp
{
    /// <summary>
    /// The Application agent for the executing assembly.
    /// </summary>
    public class ACATAgentBase : AgentBase
    {
        /// <summary>
        /// Name of the executing assembly
        /// </summary>
        private readonly String _currentProcessName;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ACATAgentBase(ILogger<ACATAgentBase> logger) : base(logger)
        {
            _currentProcessName = Process.GetCurrentProcess().ProcessName.ToLower();
        }

        /// <summary>
        /// Gets the list of process supported by this agent
        /// </summary>
        public override IEnumerable<AgentProcessInfo> ProcessesSupported
        {
            get { return new[] { new AgentProcessInfo(_currentProcessName) }; }
        }

        /// <summary>
        /// Request for a contextual menu came in.
        /// </summary>
        /// <param name="monitorInfo">Foreground window info</param>
        public override void OnContextMenuRequest(WindowActivityMonitorInfo monitorInfo)
        {
        }

        /// <summary>
        /// Invoked when the foreground window focus changes.
        /// </summary>
        /// <param name="monitorInfo">Foreground window info</param>
        /// <param name="handled">set to true if handled</param>
        public override void OnFocusChanged(WindowActivityMonitorInfo monitorInfo, ref bool handled)
        {
            handled = true;
        }
    }
}