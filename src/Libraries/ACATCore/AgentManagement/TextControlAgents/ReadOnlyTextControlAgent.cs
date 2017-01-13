////////////////////////////////////////////////////////////////////////////
// <copyright file="ReadOnlyTextControlAgent.cs" company="Intel Corporation">
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