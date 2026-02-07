////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.AgentManagement;
using ACAT.Core.PanelManagement;
using ACAT.Core.PanelManagement.Common;
using ACAT.Core.PanelManagement.Interfaces;
using ACAT.Core.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace ACAT.Extension.UI
{
    /// <summary>
    /// Helper functions for scanners
    /// </summary>
    public class ScannerHelper
    {
        private readonly ILogger<ScannerHelper> _logger;

        /// <summary>
        /// Initializes an instances of the class
        /// </summary>
        /// <param name="panel">the scanner object</param>
        /// <param name="startupArg">initialization arguments</param>
        /// <param name="logger">Logger instance</param>
        public ScannerHelper(IScannerPanel panel, StartupArg startupArg, ILogger<ScannerHelper> logger)
        {            _logger = logger;            DialogMode = startupArg.DialogMode;

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
                arg.Enabled = Context.AppAgentMgr.CurrentEditingMode == EditingMode.TextEntry && TextController.LastEditChange != TextController.LastAction.Unknown;
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
            _logger.LogDebug("ScannerHelper.  Assembly resolve raised");
            return FileUtils.AssemblyResolve(Assembly.GetExecutingAssembly(), args);
        }
    }
}