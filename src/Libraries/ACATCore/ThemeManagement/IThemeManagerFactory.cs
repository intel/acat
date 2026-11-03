////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Core.ThemeManagement
{
    /// <summary>
    /// Factory interface for creating ThemeManager instances
    /// Provides abstraction for manager creation to support testing and dependency injection
    /// </summary>
    public interface IThemeManagerFactory
    {
        /// <summary>
        /// Creates or retrieves the ThemeManager instance
        /// </summary>
        /// <returns>The ThemeManager instance</returns>
        IThemeManager Create();
    }

    /// <summary>
    /// Default factory implementation for ThemeManager
    /// Uses the singleton pattern to return the existing instance
    /// </summary>
    public class ThemeManagerFactory : IThemeManagerFactory
    {
        /// <summary>
        /// Creates or retrieves the ThemeManager singleton instance
        /// </summary>
        /// <returns>The ThemeManager singleton instance</returns>
        public IThemeManager Create()
        {
            return ThemeManager.Instance;
        }
    }
}
