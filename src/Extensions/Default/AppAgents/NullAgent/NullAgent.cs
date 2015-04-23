using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using ACAT.Lib.Core.Utility;
using System.Runtime.InteropServices;
using ACAT.Lib.Core.AgentManagement.TextInterface;
using ACAT.Lib.Core.AgentManagement;

namespace ACAT.Lib.Core.Extensions.Base.AppAgents.Null
{
    [DescriptorAttribute("92D2C512-DCAA-4773-8773-73E5D8C849FA", "Null Agent", "No-op agent")]
    class NullAgent : AgentBase
    {
        TextControlAgentBase _textInterface = new TextControlAgentBase();

        public NullAgent()
        {
            Name = "Null Agent";
        }

        public override IEnumerable<AgentProcessInfo> ProcessesSupported
        {
            get
            {
                return new AgentProcessInfo[] { new AgentProcessInfo("**nullagent**") };
            }
        }

        public override void OnFocusChanged(WindowActivityMonitorInfo monitorInfo, ref bool handled)
        {
            Log.Debug();

            setTextInterface(_textInterface);

            handled = true;
        }


        public override void OnContextMenuRequest(WindowActivityMonitorInfo monitorInfo)
        {
            showPanel(this, new PanelRequestEventArgs(PanelClasses.UnsupportedAppContextMenu, monitorInfo));
        }

    }
}
