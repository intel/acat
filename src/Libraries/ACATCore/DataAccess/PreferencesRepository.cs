////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// PreferencesRepository.cs
//
// Repository for loading and saving ACAT user preferences.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.PreferencesManagement;
using ACAT.Core.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace ACAT.Core.DataAccess
{
    /// <summary>
    /// Represents a keyed preferences entry managed by the repository.
    /// </summary>
    public class PreferencesEntry
    {
        /// <summary>Gets the unique key identifying this preferences entry.</summary>
        public string Key { get; set; }

        /// <summary>Gets the preferences object for this entry.</summary>
        public PreferencesBase Value { get; set; }
    }

    /// <summary>
    /// Repository that loads and persists <see cref="PreferencesBase"/> objects from XML files.
    /// Each preferences entry is identified by a string key (typically the file path).
    /// </summary>
    public class PreferencesRepository : RepositoryBase<PreferencesEntry, string>
    {
        private readonly string _preferencesFilePath;

        /// <summary>
        /// Initializes a new <see cref="PreferencesRepository"/>.
        /// </summary>
        /// <param name="preferencesFilePath">Path to the XML preferences file.</param>
        /// <param name="logger">Optional logger.</param>
        public PreferencesRepository(string preferencesFilePath, ILogger logger = null)
            : base(logger)
        {
            if (string.IsNullOrWhiteSpace(preferencesFilePath))
                throw new ArgumentException("Preferences file path must not be empty.", nameof(preferencesFilePath));
            _preferencesFilePath = preferencesFilePath;
        }

        /// <inheritdoc />
        protected override string GetKey(PreferencesEntry entity) => entity.Key;

        /// <inheritdoc />
        protected override IEnumerable<PreferencesEntry> LoadFromStorage()
        {
            // The preferences file path serves as the repository's single entry key.
            // Concrete preference types are loaded by callers via PreferencesBase.Load<T>().
            // This base implementation simply signals that the storage location is available.
            Logger?.LogDebug("PreferencesRepository: storage path is {Path}", _preferencesFilePath);
            return Array.Empty<PreferencesEntry>();
        }

        /// <inheritdoc />
        protected override void SaveToStorage(IEnumerable<PreferencesEntry> entities)
        {
            foreach (var entry in entities)
            {
                if (entry?.Value == null) continue;
                var saved = entry.Value.Save();
                if (!saved)
                    Logger?.LogWarning("PreferencesRepository: failed to save entry '{Key}'", entry.Key);
            }
        }

        /// <summary>
        /// Gets the file path used as the backing store for this repository.
        /// </summary>
        public string FilePath => _preferencesFilePath;
    }
}
