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
    /// Interface for commands that will be executed
    /// </summary>
    public interface IRunCommandHandler
    {
        /// <summary>
        /// Gets or sets the command verb to execute
        /// </summary>
        String Command { get; set; }

        /// <summary>
        /// Command to run after the command executing the "Command"
        /// </summary>
        PostExitCommand Status { get; set; }

        /// <summary>
        /// Executes the command. Handled is set to
        /// true if the command was handled. False otherwise.
        /// If false, the command is passed on to next handler in the
        /// command chain
        /// </summary>
        /// <param name="handled">true if handled</param>
        /// <returns>true on success</returns>
        bool Execute(ref bool handled);

        bool Execute2(object source, ref bool handled);
    }
}