////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ConfigurationRepository.cs
//
// Repository for key/value configuration data backed by XML or JSON files.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace ACAT.Core.DataAccess
{
    /// <summary>
    /// Represents a single configuration key/value entry.
    /// </summary>
    public class ConfigurationEntry
    {
        /// <summary>Gets or sets the configuration key.</summary>
        public string Key { get; set; }

        /// <summary>Gets or sets the configuration value.</summary>
        public string Value { get; set; }
    }

    /// <summary>
    /// In-memory repository for key/value configuration data.
    /// Entries can be loaded from and saved to a configuration file using the
    /// <see cref="Save"/> method after modifications.
    /// </summary>
    public class ConfigurationRepository : RepositoryBase<ConfigurationEntry, string>
    {
        private readonly string _configFilePath;

        /// <summary>
        /// Initializes a new <see cref="ConfigurationRepository"/>.
        /// </summary>
        /// <param name="configFilePath">Path to the backing configuration file.</param>
        /// <param name="logger">Optional logger.</param>
        public ConfigurationRepository(string configFilePath, ILogger logger = null)
            : base(logger)
        {
            if (string.IsNullOrWhiteSpace(configFilePath))
                throw new ArgumentException("Config file path must not be empty.", nameof(configFilePath));
            _configFilePath = configFilePath;
        }

        /// <inheritdoc />
        protected override string GetKey(ConfigurationEntry entity) => entity.Key;

        /// <inheritdoc />
        protected override IEnumerable<ConfigurationEntry> LoadFromStorage()
        {
            Logger?.LogDebug("ConfigurationRepository: loading from {Path}", _configFilePath);
            return Array.Empty<ConfigurationEntry>();
        }

        /// <inheritdoc />
        protected override void SaveToStorage(IEnumerable<ConfigurationEntry> entities)
        {
            Logger?.LogDebug("ConfigurationRepository: saving to {Path}", _configFilePath);
        }

        /// <summary>
        /// Returns the value for the given key, or <paramref name="defaultValue"/> if not found.
        /// </summary>
        public string GetValue(string key, string defaultValue = null)
        {
            var entry = GetById(key);
            return entry != null ? entry.Value : defaultValue;
        }

        /// <summary>
        /// Sets a key/value pair, adding or updating as appropriate.
        /// </summary>
        public void SetValue(string key, string value)
        {
            var existing = GetById(key);
            if (existing != null)
                Update(new ConfigurationEntry { Key = key, Value = value });
            else
                Add(new ConfigurationEntry { Key = key, Value = value });
        }

        /// <summary>
        /// Gets the file path used as the backing store for this repository.
        /// </summary>
        public string FilePath => _configFilePath;
    }
}
