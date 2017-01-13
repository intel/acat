////////////////////////////////////////////////////////////////////////////
// <copyright file="NullAgent.cs" company="Intel Corporation">
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

using ACAT.Lib.Core.AgentManagement.TextInterface;
using ACAT.Lib.Core.Utility;
using System.Collections.Generic;

namespace ACAT.Lib.Core.AgentManagement
{
    /// <summary>
    /// This is a no-op agent.  This only serves one purpose.  Instead
    /// of the agent manager having to return a null if no agent is found
    /// to support the foreground app, it returns an instance of the null agent
    /// instead of returning NULL. Saves headaches for the caller
    /// </summary>
    [DescriptorAttribute("92D2C512-DCAA-4773-8773-73E5D8C849FA",
                        "Null Agent",
                        "No-op agent")]
    public class NullAgent : AgentBase
    {
        /// <summary>
        /// The text interface
        /// </summary>
        private readonly TextControlAgentBase _textInterface = new TextControlAgentBase();

        /// <summary>
        /// Gets which processes this agent supported. Use the
        /// 'magic string' for the null agents that AgentManager requires.
        /// </summary>
        public override IEnumerable<AgentProcessInfo> ProcessesSupported
        {
            get { return new[] { new AgentProcessInfo("**nullagent**") }; }
        }

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

            setTextInterface(_textInterface);

            handled = true;
        }
    }
}