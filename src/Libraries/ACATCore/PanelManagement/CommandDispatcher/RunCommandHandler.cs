////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Lib.Core.PanelManagement.CommandDispatcher
{
    /// <summary>
    /// Base class for handling a command.  A command (an action verb)
    /// has a handler that runs the command.
    /// </summary>
    public class RunCommandHandler : IRunCommandHandler
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="command"></param>
        public RunCommandHandler(String command)
        {
            Command = command;
            Status = new PostExitCommand();
        }

        /// <summary>
        /// Gets or sets the command (action verb)
        /// </summary>
        public String Command { get; set; }

        /// <summary>
        /// Gets or sets the command to execute after the command handler
        /// returns
        /// </summary>
        public virtual PostExitCommand Status { get; set; }

        /// <summary>
        /// Gets or sets the command dispatcher
        /// </summary>
        protected internal IRunCommandDispatcher Dispatcher { get; set; }

        /// <summary>
        /// Handler to run the command
        /// </summary>
        /// <param name="handled">was the command handler?</param>
        /// <returns>true on success</returns>
        public virtual bool Execute(ref bool handled)
        {
            return true;
        }

        public virtual bool Execute2(object source, ref bool handled)
        {
            return true;
        }
    }
}