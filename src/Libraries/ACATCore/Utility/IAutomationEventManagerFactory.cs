////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Core.Utility
{
    /// <summary>
    /// Factory interface for creating AutomationEventManager instances
    /// Provides abstraction for manager creation to support testing and dependency injection
    /// </summary>
    public interface IAutomationEventManagerFactory
    {
        /// <summary>
        /// Creates or retrieves the AutomationEventManager instance
        /// </summary>
        /// <returns>The AutomationEventManager instance</returns>
        IAutomationEventManager Create();
    }

    /// <summary>
    /// Default factory implementation for AutomationEventManager
    /// Uses the singleton pattern to return the existing instance
    /// </summary>
    public class AutomationEventManagerFactory : IAutomationEventManagerFactory
    {
        /// <summary>
        /// Creates or retrieves the AutomationEventManager singleton instance
        /// </summary>
        /// <returns>The AutomationEventManager singleton instance</returns>
        public IAutomationEventManager Create()
        {
            return AutomationEventManager.Instance;
        }
    }
}
