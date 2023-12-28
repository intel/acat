////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PanelManagement.CommandDispatcher;

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Represents a command that should be executed AFTER a
    /// functional agent has exited.  For instance if a "File
    /// Browser" functional agent is active and the user wants to
    /// activate the Talk window.  The browser functional agent
    /// must exit first, and then request ACAT to activate the
    /// Talk window after the browser agent has exited
    /// This command facitilates this.
    /// </summary>
    public class PostExitCommand
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public PostExitCommand()
        {
            Command = null;
            ContextSwitch = false;
        }

        /// <summary>
        /// Gets or sets the command to run
        /// </summary>
        public RunCommandHandler Command { get; set; }

        /// <summary>
        /// Gets or sets whether the command results in a context
        /// switch to a new window (eg activate the talk window)
        /// </summary>
        public bool ContextSwitch { get; set; }
    }
}