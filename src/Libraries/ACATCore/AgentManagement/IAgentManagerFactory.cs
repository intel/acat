////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Core.AgentManagement
{
    /// <summary>
    /// Factory interface for creating AgentManager instances
    /// Provides abstraction for manager creation to support testing and dependency injection
    /// </summary>
    public interface IAgentManagerFactory
    {
        /// <summary>
        /// Creates or retrieves the AgentManager instance
        /// </summary>
        /// <returns>The AgentManager instance</returns>
        IAgentManager Create();
    }

    /// <summary>
    /// Default factory implementation for AgentManager
    /// Uses the singleton pattern to return the existing instance
    /// </summary>
    public class AgentManagerFactory : IAgentManagerFactory
    {
        /// <summary>
        /// Creates or retrieves the AgentManager singleton instance
        /// </summary>
        /// <returns>The AgentManager singleton instance</returns>
        public IAgentManager Create()
        {
            return AgentManager.Instance;
        }
    }
}
