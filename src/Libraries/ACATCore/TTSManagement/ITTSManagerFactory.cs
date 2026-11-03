////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Core.TTSManagement
{
    /// <summary>
    /// Factory interface for creating TTSManager instances
    /// Provides abstraction for manager creation to support testing and dependency injection
    /// </summary>
    public interface ITTSManagerFactory
    {
        /// <summary>
        /// Creates or retrieves the TTSManager instance
        /// </summary>
        /// <returns>The TTSManager instance</returns>
        ITTSManager Create();
    }

    /// <summary>
    /// Default factory implementation for TTSManager
    /// Uses the singleton pattern to return the existing instance
    /// </summary>
    public class TTSManagerFactory : ITTSManagerFactory
    {
        /// <summary>
        /// Creates or retrieves the TTSManager singleton instance
        /// </summary>
        /// <returns>The TTSManager singleton instance</returns>
        public ITTSManager Create()
        {
            return TTSManager.Instance;
        }
    }
}
