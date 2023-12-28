////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
using System;

namespace ACAT.Lib.Extension.CommandHandlers
{
    /// <summary>
    /// Manages the Talk window - closes it, clears it or
    /// toggles its visibility
    /// </summary>
    public class TalkWindowHandler : RunCommandHandler
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="cmd">The command to be executed</param>
        public TalkWindowHandler(String cmd)
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
            handled = true;

            switch (Command)
            {
                case "CmdTalkApp":
                    var form = PanelManager.Instance.CreatePanel("TalkApplicationScanner");
                    if (form != null)
                    {
                        // Add ad-hoc agent that will handle the form
                        var agent = Context.AppAgentMgr.GetAgentByName("Talk Application Agent");
                        if (agent != null)
                        {
                            Context.AppAgentMgr.AddAgent(form.Handle, agent);
                            Context.AppPanelManager.ShowDialog(form as IPanel);
                        }
                    }

                    break;

                default:
                    handled = false;
                    break;
            }

            return true;
        }
    }
}