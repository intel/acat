////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Core.AbbreviationsManagement
{
    /// <summary>
    /// Factory interface for creating AbbreviationsManager instances
    /// Provides abstraction for manager creation to support testing and dependency injection
    /// </summary>
    public interface IAbbreviationsManagerFactory
    {
        /// <summary>
        /// Creates or retrieves the AbbreviationsManager instance
        /// </summary>
        /// <returns>The AbbreviationsManager instance</returns>
        IAbbreviationsManager Create();
    }

    /// <summary>
    /// Default factory implementation for AbbreviationsManager
    /// Uses the singleton pattern to return the existing instance
    /// </summary>
    public class AbbreviationsManagerFactory : IAbbreviationsManagerFactory
    {
        /// <summary>
        /// Creates or retrieves the AbbreviationsManager singleton instance
        /// </summary>
        /// <returns>The AbbreviationsManager singleton instance</returns>
        public IAbbreviationsManager Create()
        {
            return AbbreviationsManager.Instance;
        }
    }
}
