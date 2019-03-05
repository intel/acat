////////////////////////////////////////////////////////////////////////////
// <copyright file="ScannerHelper.cs" company="Intel Corporation">
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
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace ACAT.Lib.Extension
{
    /// <summary>
    /// Helper functions for scanners
    /// </summary>
    public class ScannerHelper
    {
        /// <summary>
        /// Initializes an instances of the class
        /// </summary>
        /// <param name="panel">the scanner object</param>
        /// <param name="startupArg">initialization arguments</param>
        public ScannerHelper(IScannerPanel panel, StartupArg startupArg)
        {
            DialogMode = startupArg.DialogMode;

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += currentDomain_AssemblyResolve;
        }

        /// <summary>
        /// Gets the DialogMode.  If this is true, then
        /// the scanner is being used as a companion scanner
        /// for an ACAT dialog.
        /// </summary>
        public bool DialogMode { get; private set; }

        public bool CheckCommandEnabled(CommandEnabledArg arg)
        {
            arg.Handled = false;

            if (arg.Command == "CmdUndoLastEditChange")
            {
                arg.Enabled = (Context.AppAgentMgr.CurrentEditingMode == EditingMode.TextEntry && TextController.LastEditChange != TextController.LastAction.Unknown);
                arg.Handled = true;
                return true;
            }

            if (DialogMode)
            {
                switch (arg.Command)
                {
                    case "CmdTalkWindowToggle":
                    case "CmdMainMenu":
                    case "CmdMouseScanner":
                    case "CmdContextMenu":
                    case "CmdToolsMenu":
                    case "CmdWindowPosSizeMenu":
                        arg.Enabled = false;
                        arg.Handled = true;
                        break;

                    case "CmdDualMonitorMenu":
                        arg.Enabled = DualMonitor.MultipleMonitors;
                        arg.Handled = true;
                        break;
                }
            }

            return true;
        }

        /// <summary>
        /// Call this function in the OnFormClosing function
        /// of the scanner form
        /// </summary>
        /// <param name="e"></param>
        public void OnFormClosing(FormClosingEventArgs e)
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve -= currentDomain_AssemblyResolve;
        }

        /// <summary>
        /// Resolve assembly handler
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="args">event arg</param>
        /// <returns></returns>
        private Assembly currentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Log.Debug("ScannerHelper.  Assembly resolve raised");
            return FileUtils.AssemblyResolve(Assembly.GetExecutingAssembly(), args);
        }
    }
}