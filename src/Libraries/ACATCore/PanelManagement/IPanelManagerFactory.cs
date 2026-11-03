////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Core.PanelManagement
{
    /// <summary>
    /// Factory interface for creating PanelManager instances
    /// Provides abstraction for manager creation to support testing and dependency injection
    /// </summary>
    public interface IPanelManagerFactory
    {
        /// <summary>
        /// Creates or retrieves the PanelManager instance
        /// </summary>
        /// <returns>The PanelManager instance</returns>
        IPanelManager Create();
    }

    /// <summary>
    /// Default factory implementation for PanelManager
    /// Uses the singleton pattern to return the existing instance
    /// </summary>
    public class PanelManagerFactory : IPanelManagerFactory
    {
        /// <summary>
        /// Creates or retrieves the PanelManager singleton instance
        /// </summary>
        /// <returns>The PanelManager singleton instance</returns>
        public IPanelManager Create()
        {
            return PanelManager.Instance;
        }
    }
}
