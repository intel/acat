////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
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