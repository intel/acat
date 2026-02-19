////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Core.WordPredictorManagement
{
    /// <summary>
    /// Factory interface for creating WordPredictionManager instances
    /// Provides abstraction for manager creation to support testing and dependency injection
    /// </summary>
    public interface IWordPredictionManagerFactory
    {
        /// <summary>
        /// Creates or retrieves the WordPredictionManager instance
        /// </summary>
        /// <returns>The WordPredictionManager instance</returns>
        IWordPredictionManager Create();
    }

    /// <summary>
    /// Default factory implementation for WordPredictionManager
    /// Uses the singleton pattern to return the existing instance
    /// </summary>
    public class WordPredictionManagerFactory : IWordPredictionManagerFactory
    {
        /// <summary>
        /// Creates or retrieves the WordPredictionManager singleton instance
        /// </summary>
        /// <returns>The WordPredictionManager singleton instance</returns>
        public IWordPredictionManager Create()
        {
            return WordPredictionManager.Instance;
        }
    }
}
