using System;
using System.Collections.Generic;
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Extension.AppAgents.Generic;

namespace ACAT.Lib.Core.Extensions.Base.AppAgents.GenericApp
{
    [DescriptorAttribute("8CFBC12A-6CC4-4751-ABB5-3A7172500569", "Unsupported App Agent", "App Agent unsupported applications")]
    class UnsupportedAppAgent : GenericAppAgent
    {
        public UnsupportedAppAgent()
        {
        }

        protected override void OnDispose()
        {
            base.OnDispose();
        }

        public override IEnumerable<AgentProcessInfo> ProcessesSupported
        {
            get
            {
                return base.ProcessesSupported;
            }
        }

        public override void OnPanelClosed(String panelClass, WindowActivityMonitorInfo monitorInfo)
        {
            base.OnPanelClosed(panelClass, monitorInfo);
        }

        public override void OnContextMenuRequest(WindowActivityMonitorInfo monitorInfo)
        {
            base.OnContextMenuRequest(monitorInfo);
        }

        public override void OnRunCommand(String command, object commandArg, ref bool handled)
        {
            base.OnRunCommand(command, commandArg, ref handled);

        }

        public override void CheckWidgetEnabled(CheckEnabledArgs arg)
        {
            base.CheckWidgetEnabled(arg);
        }
    }
}
