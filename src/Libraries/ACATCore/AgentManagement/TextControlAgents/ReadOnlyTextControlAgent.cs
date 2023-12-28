////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;

namespace ACAT.Lib.Core.AgentManagement.TextInterface
{
    /// <summary>
    /// Interfaces a text control that disallows editing.  Only
    /// text navigation is allowed.  Any feature that results in
    /// text editing are not supported.
    /// </summary>
    public class ReadOnlyTextControlAgent : TextControlAgentBase
    {
        /// <summary>
        /// Support only navigation features
        /// </summary>
        private readonly String[] _supportedCommands =
        {
            "CmdCopy",
            "CmdPrevChar",
            "CmdNextChar",
            "CmdPrevLine",
            "CmdNextLine",
            "CmdPrevWord",
            "CmdNextWord",
            "CmdPrevPage",
            "CmdNextPage",
            "CmdTopOfDoc",
            "CmdEndOfDoc",
            "CmdSelectAll",
            "CmdSelectModeToggle",
        };

        /// <summary>
        /// Returns whether the widget associated with a feature
        /// should be enabled or not.
        /// </summary>
        /// <param name="arg">widget info</param>
        public override void CheckCommandEnabled(CommandEnabledArg arg)
        {
            // support only if this is a navigation feature
            if (_supportedCommands.Contains(arg.Command))
            {
                arg.Handled = true;
                arg.Enabled = true;
            }
        }
    }
}