////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;

namespace ACAT.Lib.Extension.AppAgents.DialogControlAgent
{
    /// <summary>
    /// Base class for agent to handle applicaoion dialogs.  For eg,
    /// the Find dialog in Windows Notepad. Provides functionality to
    /// tab through the controls in the dialog, navigate
    /// list boxes, activate buttons etc.
    /// </summary>
    public class DialogControlAgentBase : GenericAppAgentBase
    {
        /// <summary>
        /// If set to true, the agent will autoswitch the
        /// scanners depending on which element has focus.
        /// Eg: Alphabet scanner if an edit text window has focus,
        /// the contextual menu if the main document has focus
        /// </summary>
        protected bool autoSwitchScanners = true;

        /// <summary>
        /// Handle to the window that was previously active
        /// </summary>
        private IntPtr _prevHwnd = IntPtr.Zero;

        /// <summary>
        /// Gets which processes this agent supported. Use the
        /// 'magic string' for dialog agents that AgentManager requires
        /// </summary>
        public override IEnumerable<AgentProcessInfo> ProcessesSupported
        {
            get
            {
                return new[] { new AgentProcessInfo("**dialogagent**") };
            }
        }

        /// <summary>
        /// Displays the contextual menu
        /// </summary>
        /// <param name="monitorInfo">Foreground window info</param>
        public override void OnContextMenuRequest(WindowActivityMonitorInfo monitorInfo)
        {
            showPanel(this, new PanelRequestEventArgs(PanelClasses.DialogContextMenu, monitorInfo.Title, monitorInfo));
        }

        /// <summary>
        /// Invoked when the foreground window focus changes.  Display the
        /// contextual menus for dialogs which will allow interaction with the dialog
        /// </summary>
        /// <param name="monitorInfo">Foreground window info</param>
        /// <param name="handled">set to true if handled</param>
        public override void OnFocusChanged(WindowActivityMonitorInfo monitorInfo, ref bool handled)
        {
            Log.Debug("prevHwnd: " + _prevHwnd + ", fgHwnd: " + monitorInfo.FgHwnd);

            base.OnFocusChanged(monitorInfo, ref handled);

            if (autoSwitchScanners && _prevHwnd != monitorInfo.FgHwnd)
            {
                Log.Debug("They are not equal. Show dialog panel");

                showPanel(this, new PanelRequestEventArgs(PanelClasses.DialogContextMenu,
                                                            monitorInfo.Title,
                                                            monitorInfo));

                _prevHwnd = monitorInfo.FgHwnd;
            }

            handled = true;
        }

        /// <summary>
        /// Focus shifted to a non-dialog.  This agent is getting deactivated.
        /// </summary>
        public override void OnFocusLost()
        {
            _prevHwnd = IntPtr.Zero;
        }
    }
}