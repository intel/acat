////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
using ACAT.Lib.Core.Utility;
using System;
using System.Windows.Forms;

namespace ACAT.Lib.Extension.CommandHandlers
{
    /// <summary>
    /// Sends special keys to the keyboard buffer
    /// </summary>
    public class SendKeyHandler : RunCommandHandler
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="cmd">The command to be executed</param>
        public SendKeyHandler(String cmd)
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
                case "CmdCapsLock":
                    Context.AppAgentMgr.Keyboard.Send(Keys.CapsLock);
                    break;

                case "CmdNumLock":
                    Context.AppAgentMgr.Keyboard.Send(Keys.NumLock);
                    break;

                case "CmdScrollLock":
                    Context.AppAgentMgr.Keyboard.Send(Keys.Scroll);
                    break;

                case "CmdEnterKey":
                    Context.AppAgentMgr.Keyboard.Send(KeyStateTracker.GetExtendedKeys(), '\n');
                    KeyStateTracker.KeyTriggered('\n');
                    break;

                case "CmdPeriodKey":
                    Context.AppAgentMgr.Keyboard.Send(KeyStateTracker.GetExtendedKeys(), '.');
                    break;

                case "CmdCommaKey":
                    Context.AppAgentMgr.Keyboard.Send(KeyStateTracker.GetExtendedKeys(), ',');
                    break;
            }

            return true;
        }
    }
}