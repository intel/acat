////////////////////////////////////////////////////////////////////////////
// <copyright file="MenuControlAgentBase.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2015 Intel Corporation 
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
using System.Diagnostics;
using System.Windows.Forms;
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;

namespace ACAT.Lib.Extension.AppAgents.MenuControlAgent
{
    /// <summary>
    /// Base class for the Agent that handles menus.  Provides
    /// functionality to navigate menus and make selections
    /// </summary>
    public class MenuControlAgentBase : GenericAppAgentBase
    {
        /// <summary>
        /// Has the scanner for handling menus been shown already?
        /// </summary>
        protected bool scannerShown;

        /// <summary>
        /// Assembly name of this application
        /// </summary>
        private String _currentProcessName;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public MenuControlAgentBase()
        {
            _currentProcessName = Process.GetCurrentProcess().ProcessName.ToLower();
        }

        /// <summary>
        /// Gets which processes this agent supported. Use the
        /// 'magic string' for menu agents that AgentManager requires
        /// </summary>
        public override IEnumerable<AgentProcessInfo> ProcessesSupported
        {
            get { return new[] { new AgentProcessInfo("**menuagent**") }; }
        }

        /// <summary>
        /// Displays the contextual menu
        /// </summary>
        /// <param name="monitorInfo">Foreground window info</param>
        public override void OnContextMenuRequest(WindowActivityMonitorInfo monitorInfo)
        {
            showMenuContextMenu(monitorInfo);
        }

        /// <summary>
        /// Invoked when the foreground window focus changes.  Display the
        /// contextual menus for dialogs
        /// </summary>
        /// <param name="monitorInfo">Foreground window info</param>
        /// <param name="handled">set to true if handled</param>
        public override void OnFocusChanged(WindowActivityMonitorInfo monitorInfo, ref bool handled)
        {
            Log.Debug();

            if (ignoreApp(monitorInfo.FgProcess.ProcessName))
            {
                handled = false;
                return;
            }

            if (!scannerShown)
            {
                base.OnFocusChanged(monitorInfo, ref handled);
                showMenuContextMenu(monitorInfo);
            }
            handled = true;
        }

        /// <summary>
        /// Focus shifted to a non-menu.  This agent is
        /// getting deactivated.
        /// </summary>
        public override void OnFocusLost()
        {
            scannerShown = false;
        }

        /// <summary>
        /// Invoked to run a command
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <param name="commandArg">Optional arguments for the command</param>
        /// <param name="handled">set this to true if handled</param>
        public override void OnRunCommand(String command, object commandArg, ref bool handled)
        {
            // send escapes to close the menu
            if (String.Compare(command, "CmdCloseMenu", true) == 0)
            {
                AgentManager.Instance.Keyboard.Send(Keys.Escape);
                AgentManager.Instance.Keyboard.Send(Keys.Escape);
                handled = true;
            }
        }

        /// <summary>
        /// Don't support these apps
        /// </summary>
        /// <param name="appName">name of app</param>
        /// <returns>true if no support</returns>
        private bool ignoreApp(String appName)
        {
            return String.Compare(appName, _currentProcessName, true) == 0;
        }

        /// <summary>
        /// Show the contexual menus for menus
        /// </summary>
        /// <param name="monitorInfo">Foreground menu window information</param>
        private void showMenuContextMenu(WindowActivityMonitorInfo monitorInfo)
        {
            var args = new PanelRequestEventArgs(PanelClasses.MenuContextMenu, monitorInfo)
            {
                UseCurrentScreenAsParent = true,
                Title = "Menu"
            };

            showPanel(this, args);
            scannerShown = true;
        }
    }
}