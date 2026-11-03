////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

namespace ACAT.Core.Configuration
{
    /// <summary>
    /// Interface for configuration management to support dependency injection.
    /// Provides environment-specific configuration loading, hot-reload support,
    /// and key/value override capabilities.
    /// </summary>
    public interface IConfigurationManager
    {
        /// <summary>
        /// Gets the current configuration environment
        /// </summary>
        ConfigurationEnvironment CurrentEnvironment { get; }

        /// <summary>
        /// Sets the current environment explicitly
        /// </summary>
        /// <param name="environment">Environment to set</param>
        void SetEnvironment(ConfigurationEnvironment environment);

        /// <summary>
        /// Gets the environment-specific configuration file path.
        /// Returns the environment-specific override file if it exists,
        /// otherwise returns the base file path.
        /// </summary>
        /// <param name="baseFilePath">Base configuration file path (e.g., "config.json")</param>
        /// <returns>Environment-specific file path if it exists, otherwise the base path</returns>
        string GetEnvironmentFilePath(string baseFilePath);

        /// <summary>
        /// Loads a configuration object from file with optional environment-specific
        /// overrides applied from environment variables.
        /// </summary>
        /// <typeparam name="T">Configuration type to deserialize into</typeparam>
        /// <param name="baseFilePath">Base configuration file path</param>
        /// <param name="applyEnvironmentOverrides">Whether to apply environment variable overrides</param>
        /// <returns>Populated configuration object, or <c>null</c> if the file was not found or
        /// could not be deserialized</returns>
        T LoadWithEnvironmentOverrides<T>(string baseFilePath, bool applyEnvironmentOverrides = true) where T : class;

        /// <summary>
        /// Sets a named override value
        /// </summary>
        /// <param name="key">Configuration key</param>
        /// <param name="value">Override value</param>
        void SetOverride(string key, string value);

        /// <summary>
        /// Gets a named override value
        /// </summary>
        /// <param name="key">Configuration key</param>
        /// <returns>Override value, or <c>null</c> if the key has not been set</returns>
        string GetOverride(string key);

        /// <summary>
        /// Clears all named override values
        /// </summary>
        void ClearOverrides();

        /// <summary>
        /// Returns a snapshot of all currently set override values
        /// </summary>
        /// <returns>Dictionary of key/value override pairs</returns>
        Dictionary<string, string> GetAllOverrides();

        /// <summary>
        /// Gets the local override configuration file path (e.g., config.local.json).
        /// Local override files are intended for developer-specific settings and should
        /// be excluded from source control.
        /// </summary>
        /// <param name="baseFilePath">Base configuration file path (e.g., "config.json")</param>
        /// <returns>Local override file path (e.g., "config.local.json"), regardless of whether it exists</returns>
        string GetLocalOverrideFilePath(string baseFilePath);

        /// <summary>
        /// Returns the ordered list of configuration files that should be loaded
        /// and merged for the current environment.  Files appear in priority order
        /// (lowest to highest): base, environment-specific, local override.
        /// Only files that actually exist on disk are included.
        /// </summary>
        /// <param name="baseFilePath">Base configuration file path (e.g., "config.json")</param>
        /// <returns>
        /// Ordered list of existing file paths from which configuration should be
        /// loaded and merged, starting from the base file.
        /// </returns>
        IReadOnlyList<string> GetConfigurationFiles(string baseFilePath);
    }
}
