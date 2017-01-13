////////////////////////////////////////////////////////////////////////////
// <copyright file="ApplicationFrameHostAgent.cs" company="Intel Corporation">
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

using ACAT.ACATResources;
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ACAT.Lib.Extension;

namespace ACAT.Lib.Core.Extensions.Base.AppAgents.CalculatorAgent
{
    /// <summary>
    /// Application agent for Calculator process. Displays a scanner
    /// that mirrors the calculator allowing the user to interact with
    /// the calculator
    /// </summary>
    [DescriptorAttribute("2D98AE5B-0B45-42F7-A947-A410CD447D97",
                            "Calculator Agent",
                            "Manages interactions with the Win7/Win8 Calculator")]
    internal class CalculatorAgent : GenericAppAgentBase
    {
        /// <summary>
        /// Title of the calculator window
        /// </summary>
        private readonly string CalculatorTitle = R.GetString2("WindowsAppCalculatorTitle").ToLower();

        /// <summary>
        /// Which features does this support?
        /// </summary>
        private readonly String[] _supportedCommands = { "CmdContextMenu" };

        /// <summary>
        /// Which processes does this agent support?
        /// </summary>
        public override IEnumerable<AgentProcessInfo> ProcessesSupported
        {
            get { return new[] { new AgentProcessInfo("calc") }; }
        }

        /// <summary>
        /// Invoked to check if a scanner button should be enabled.  Uses context
        /// to determine the 'enabled' state.
        /// </summary>
        /// <param name="arg">info about the scanner button</param>
        public override void CheckCommandEnabled(CommandEnabledArg arg)
        {
            checkCommandEnabled(_supportedCommands, arg);
        }

        /// <summary>
        /// Invoked when required to display a contextual menu
        /// </summary>
        /// <param name="monitorInfo">info about foreground window</param>
        public override void OnContextMenuRequest(WindowActivityMonitorInfo monitorInfo)
        {
            showPanel(this, new PanelRequestEventArgs("CalculatorScanner", monitorInfo));
        }

        /// <summary>
        /// Invoked when the focus changes either in the active window or when the
        /// active window itself changes.
        /// </summary>
        /// <param name="monitorInfo">Info about focused element</param>
        /// <param name="handled">was this handled</param>
        public override void OnFocusChanged(WindowActivityMonitorInfo monitorInfo, ref bool handled)
        {
            base.OnFocusChanged(monitorInfo, ref handled);

            showPanel(this, new PanelRequestEventArgs("CalculatorScanner",
                                                        CalculatorTitle, 
                                                        monitorInfo));
            handled = true;
        }

        /// <summary>
        /// Invoked when the user selects something in the scanner that
        /// corresponds to a mapped functionality.
        /// </summary>
        /// <param name="command">the command</param>
        /// <param name="commandArg">any optional arguments</param>
        /// <param name="handled">was it handled?</param>
        public override void OnRunCommand(String command, object commandArg, ref bool handled)
        {
            handled = true;

            switch (command)
            {
                default:
                    base.OnRunCommand(command, commandArg, ref handled);
                    break;
            }
        }
    }
}