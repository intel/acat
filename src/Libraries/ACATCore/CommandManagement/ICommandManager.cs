////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Core.CommandManagement
{
    /// <summary>
    /// Interface for CommandManager to support dependency injection.
    /// Manages application commands.
    /// </summary>
    public interface ICommandManager
    {
        /// <summary>
        /// The table containing a list of commands with their descriptions
        /// </summary>
        CmdDescriptorTable AppCommandTable { get; set; }

        /// <summary>
        /// Initializes the command manager
        /// </summary>
        /// <returns>true on success</returns>
        bool Init();
    }
}
