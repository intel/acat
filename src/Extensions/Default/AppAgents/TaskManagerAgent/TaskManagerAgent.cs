#region Author Information
// Author: Sai Prasad
// Group : XTL, IXR
#endregion

#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Diagnostics;
using System.Windows.Automation;
using System.Runtime.InteropServices;
using ACATCore.Utility;
using ACATCore.AgentManagement.TextInterface;
using ACATCore.AgentManagement;
#endregion

namespace ACATCore.Extensions.Base.AppAgents.TaskManager
{
    /// <summary>
    /// Interop class to control the output window to which the
    /// text from Aster is routed. 
    /// For now, we support one of two applications:
    /// Notepad or EZKeys
    /// </summary>
    class TaskManagerAgent : GenericAppAgentBase
    {
        String contextMenu = "TaskManagerContextMenu";
        String title = "Task Manager";

        public TaskManagerAgent()
        {
            Name = "TaskManager Agent";
        }

        public override IEnumerable<AgentProcessInfo> ProcessesSupported
        {
            get
            {
                return new AgentProcessInfo[] { new AgentProcessInfo("taskmgr") };
            }
        }

        public override void OnFocusChanged(WindowActivityMonitorInfo monitorInfo, ref bool handled)
        {
            Log.Debug();
            base.OnFocusChanged(monitorInfo, ref handled);
            showPanelOnFocusChanged(this, new PanelRequestEventArgs(contextMenu, title, monitorInfo));
            handled = true;
        }

        public override void OnContextMenuRequest(WindowActivityMonitorInfo monitorInfo)
        {
            showPanel(this, new PanelRequestEventArgs(contextMenu, title, monitorInfo));
        }

#if !AUTO_SWITCH_SCANNER
        public override void CheckWidgetEnabled(CheckEnabledArgs arg)
        {

        }
#endif
    }
}
