////////////////////////////////////////////////////////////////////////////
// <copyright file="ACATAgentBase.cs" company="Intel Corporation">
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

using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ACAT.Lib.Extension.AppAgents.ACATApp
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
        public ACATAgentBase()
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
            Log.Debug();

            handled = true;
        }
    }
}