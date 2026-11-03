////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Core.SpellCheckManagement
{
    /// <summary>
    /// Factory interface for creating SpellCheckManager instances
    /// Provides abstraction for manager creation to support testing and dependency injection
    /// </summary>
    public interface ISpellCheckManagerFactory
    {
        /// <summary>
        /// Creates or retrieves the SpellCheckManager instance
        /// </summary>
        /// <returns>The SpellCheckManager instance</returns>
        ISpellCheckManager Create();
    }

    /// <summary>
    /// Default factory implementation for SpellCheckManager
    /// Uses the singleton pattern to return the existing instance
    /// </summary>
    public class SpellCheckManagerFactory : ISpellCheckManagerFactory
    {
        /// <summary>
        /// Creates or retrieves the SpellCheckManager singleton instance
        /// </summary>
        /// <returns>The SpellCheckManager singleton instance</returns>
        public ISpellCheckManager Create()
        {
            return SpellCheckManager.Instance;
        }
    }
}
