////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
using ACAT.Lib.Core.Utility;
using System;
using System.Windows.Forms;

namespace ACAT.Lib.Extension.CommandHandlers
{
    /// <summary>
    /// The Command string represents which modifier key to
    /// trigger (Shift, Ctrl or Alt key)
    /// </summary>
    public class ModifierKeyTriggerHandler : RunCommandHandler
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="cmd">The command to be executed</param>
        public ModifierKeyTriggerHandler(String cmd)
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
                case "CmdShiftKey":
                    KeyStateTracker.KeyTriggered(Keys.LShiftKey);
                    break;

                case "CmdCtrlKey":
                    KeyStateTracker.KeyTriggered(Keys.LControlKey);
                    break;

                case "CmdAltKey":
                    KeyStateTracker.KeyTriggered(Keys.LMenu);
                    break;

                default:
                    handled = false;
                    break;
            }

            return false;
        }
    }
}