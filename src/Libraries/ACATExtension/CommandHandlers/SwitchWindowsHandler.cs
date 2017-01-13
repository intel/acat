////////////////////////////////////////////////////////////////////////////
// <copyright file="SwitchWindowsHandler.cs" company="Intel Corporation">
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
using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
using ACAT.Lib.Core.Utility;
using System;
using System.Windows.Forms;

namespace ACAT.Lib.Extension.CommandHandlers
{
    /// <summary>
    /// Activates the agent that lets the user switch
    /// between windows of the active process. For instance
    /// if there are multiple notepad windows, only displays all
    /// the active notepad windows
    /// </summary>
    public class SwitchWindowsHandler : RunCommandHandler
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="cmd">The command to be executed</param>
        public SwitchWindowsHandler(String cmd)
            : base(cmd)
        {
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="handled">set to true if the command was handled</param>
        /// <returns>true on success</returns>
        public override bool Execute(ref bool handled)
        {
            bool retVal = true;

            handled = true;

            if (!Context.AppAgentMgr.CanActivateFunctionalAgent())
            {
                return false;
            }

            Form form = Dispatcher.Scanner.Form;

            form.Invoke(new MethodInvoker(delegate
            {
                IApplicationAgent switchWindowsAgent = Context.AppAgentMgr.GetAgentByCategory("SwitchWindowsAgent");
                if (switchWindowsAgent == null)
                {
                    retVal = false;
                }
                else
                {
                    IntPtr foregroundWindow = Windows.GetForegroundWindow();
                    var process = WindowActivityMonitor.GetProcessForWindow(foregroundWindow);
                    IExtension extension = switchWindowsAgent;

                    extension.GetInvoker().SetValue("FilterByProcessName", process.ProcessName);

                    Context.AppAgentMgr.ActivateAgent(switchWindowsAgent as IFunctionalAgent);

                    WindowActivityMonitor.GetActiveWindow();
                }
            }));
            return retVal;
        }
    }
}