////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Core.CommandManagement
{
    /// <summary>
    /// Factory interface for creating CommandManager instances
    /// Provides abstraction for manager creation to support testing and dependency injection
    /// </summary>
    public interface ICommandManagerFactory
    {
        /// <summary>
        /// Creates or retrieves the CommandManager instance
        /// </summary>
        /// <returns>The CommandManager instance</returns>
        ICommandManager Create();
    }

    /// <summary>
    /// Default factory implementation for CommandManager
    /// Uses the singleton pattern to return the existing instance
    /// </summary>
    public class CommandManagerFactory : ICommandManagerFactory
    {
        /// <summary>
        /// Creates or retrieves the CommandManager singleton instance
        /// </summary>
        /// <returns>The CommandManager singleton instance</returns>
        public ICommandManager Create()
        {
            return CommandManager.Instance;
        }
    }
}
