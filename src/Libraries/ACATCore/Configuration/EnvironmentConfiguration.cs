////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// EnvironmentConfiguration.cs
//
// Support for environment-specific configuration loading and overrides
//
////////////////////////////////////////////////////////////////////////////

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Collections.ObjectModel;

namespace ACAT.Core.Configuration
{
    /// <summary>
    /// Supported configuration environments
    /// </summary>
    public enum ConfigurationEnvironment
    {
        Development,
        Testing,
        Staging,
        Production
    }

    /// <summary>
    /// Service for loading environment-specific configurations
    /// </summary>
    public class EnvironmentConfiguration : IConfigurationManager
    {
        private readonly ILogger _logger;
        private ConfigurationEnvironment _currentEnvironment;
        private readonly Dictionary<string, string> _environmentOverrides;

        /// <summary>
        /// Get the current configuration environment
        /// </summary>
        public ConfigurationEnvironment CurrentEnvironment => _currentEnvironment;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">Logger instance (optional)</param>
        public EnvironmentConfiguration(ILogger logger = null)
        {
            _logger = logger ?? Utility.LogManager.GetLogger<EnvironmentConfiguration>();
            _environmentOverrides = new Dictionary<string, string>();
            _currentEnvironment = DetectEnvironment();
        }

        /// <summary>
        /// Detect the current environment from environment variables
        /// </summary>
        private ConfigurationEnvironment DetectEnvironment()
        {
            try
            {
                // Check ACAT_ENVIRONMENT variable
                string envVar = Environment.GetEnvironmentVariable("ACAT_ENVIRONMENT");
                
                if (!string.IsNullOrEmpty(envVar))
                {
                    if (Enum.TryParse<ConfigurationEnvironment>(envVar, true, out ConfigurationEnvironment env))
                    {
                        _logger?.LogInformation("Environment detected from ACAT_ENVIRONMENT: {Environment}", env);
                        return env;
                    }
                }

                // Check DOTNET_ENVIRONMENT variable (standard .NET convention)
                envVar = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
                
                if (!string.IsNullOrEmpty(envVar))
                {
                    if (Enum.TryParse<ConfigurationEnvironment>(envVar, true, out ConfigurationEnvironment env))
                    {
                        _logger?.LogInformation("Environment detected from DOTNET_ENVIRONMENT: {Environment}", env);
                        return env;
                    }
                }

                // Check ASPNETCORE_ENVIRONMENT variable
                envVar = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                
                if (!string.IsNullOrEmpty(envVar))
                {
                    if (Enum.TryParse<ConfigurationEnvironment>(envVar, true, out ConfigurationEnvironment env))
                    {
                        _logger?.LogInformation("Environment detected from ASPNETCORE_ENVIRONMENT: {Environment}", env);
                        return env;
                    }
                }

                // Default to Production for safety
                _logger?.LogInformation("No environment variable set, defaulting to Production");
                return ConfigurationEnvironment.Production;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error detecting environment, defaulting to Production");
                return ConfigurationEnvironment.Production;
            }
        }

        /// <summary>
        /// Set the current environment explicitly
        /// </summary>
        /// <param name="environment">Environment to set</param>
        public void SetEnvironment(ConfigurationEnvironment environment)
        {
            _currentEnvironment = environment;
            _logger?.LogInformation("Environment explicitly set to: {Environment}", environment);
        }

        /// <summary>
        /// Get environment-specific configuration file path
        /// </summary>
        /// <param name="baseFilePath">Base configuration file path (e.g., "config.json")</param>
        /// <returns>Environment-specific file path if it exists, otherwise base path</returns>
        public string GetEnvironmentFilePath(string baseFilePath)
        {
            try
            {
                if (string.IsNullOrEmpty(baseFilePath))
                {
                    return baseFilePath;
                }

                string directory = Path.GetDirectoryName(baseFilePath);
                string fileName = Path.GetFileNameWithoutExtension(baseFilePath);
                string extension = Path.GetExtension(baseFilePath);

                // Try environment-specific file (e.g., "config.Development.json")
                string envFileName = $"{fileName}.{_currentEnvironment}{extension}";
                string envFilePath = string.IsNullOrEmpty(directory) 
                    ? envFileName 
                    : Path.Combine(directory, envFileName);

                if (File.Exists(envFilePath))
                {
                    _logger?.LogInformation("Using environment-specific configuration: {FilePath}", envFilePath);
                    return envFilePath;
                }

                // Fall back to base file
                _logger?.LogDebug("Environment-specific configuration not found, using base: {FilePath}", baseFilePath);
                return baseFilePath;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error determining environment file path, using base: {FilePath}", baseFilePath);
                return baseFilePath;
            }
        }

        /// <summary>
        /// Load configuration with environment-specific overrides
        /// </summary>
        /// <typeparam name="T">Configuration type</typeparam>
        /// <param name="baseFilePath">Base configuration file path</param>
        /// <param name="applyEnvironmentOverrides">Whether to apply environment variable overrides</param>
        /// <returns>Configuration object with environment overrides applied</returns>
        public T LoadWithEnvironmentOverrides<T>(string baseFilePath, bool applyEnvironmentOverrides = true) where T : class
        {
            try
            {
                // Get environment-specific file path
                string filePath = GetEnvironmentFilePath(baseFilePath);

                if (!File.Exists(filePath))
                {
                    _logger?.LogWarning("Configuration file not found: {FilePath}", filePath);
                    return null;
                }

                // Load JSON
                string jsonContent = File.ReadAllText(filePath);
                T config = System.Text.Json.JsonSerializer.Deserialize<T>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    AllowTrailingCommas = true
                });

                if (config == null)
                {
                    _logger?.LogError("Failed to deserialize configuration from: {FilePath}", filePath);
                    return null;
                }

                // Apply environment variable overrides if requested
                if (applyEnvironmentOverrides)
                {
                    ApplyEnvironmentVariableOverrides(config);
                }

                _logger?.LogInformation("Loaded configuration with environment overrides from: {FilePath}", filePath);
                return config;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error loading configuration with environment overrides: {FilePath}", baseFilePath);
                return null;
            }
        }

        /// <summary>
        /// Apply environment variable overrides to configuration object
        /// </summary>
        /// <typeparam name="T">Configuration type</typeparam>
        /// <param name="config">Configuration object to apply overrides to</param>
        private void ApplyEnvironmentVariableOverrides<T>(T config) where T : class
        {
            try
            {
                // Get all properties of the configuration object
                var properties = typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

                foreach (var property in properties)
                {
                    if (!property.CanWrite)
                    {
                        continue;
                    }

                    // Check for environment variable in format ACAT_<PropertyName>
                    string envVarName = $"ACAT_{property.Name.ToUpperInvariant()}";
                    string envVarValue = Environment.GetEnvironmentVariable(envVarName);

                    if (!string.IsNullOrEmpty(envVarValue))
                    {
                        try
                        {
                            // Convert and set the value
                            object convertedValue = Convert.ChangeType(envVarValue, property.PropertyType);
                            property.SetValue(config, convertedValue);
                            
                            _logger?.LogInformation("Applied environment override: {PropertyName} = {Value}", 
                                property.Name, envVarValue);
                        }
                        catch (Exception ex)
                        {
                            _logger?.LogWarning(ex, "Failed to apply environment override for {PropertyName}", property.Name);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error applying environment variable overrides");
            }
        }

        /// <summary>
        /// Set an environment-specific override value
        /// </summary>
        /// <param name="key">Configuration key</param>
        /// <param name="value">Override value</param>
        public void SetOverride(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                _logger?.LogWarning("Cannot set override with null or empty key");
                return;
            }

            _environmentOverrides[key] = value;
            _logger?.LogInformation("Set environment override: {Key} = {Value}", key, value);
        }

        /// <summary>
        /// Get an environment-specific override value
        /// </summary>
        /// <param name="key">Configuration key</param>
        /// <returns>Override value or null if not set</returns>
        public string GetOverride(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            return _environmentOverrides.TryGetValue(key, out string value) ? value : null;
        }

        /// <summary>
        /// Clear all environment overrides
        /// </summary>
        public void ClearOverrides()
        {
            _environmentOverrides.Clear();
            _logger?.LogInformation("Cleared all environment overrides");
        }

        /// <summary>
        /// Get all environment overrides
        /// </summary>
        public Dictionary<string, string> GetAllOverrides()
        {
            return new Dictionary<string, string>(_environmentOverrides);
        }

        /// <summary>
        /// Get the local override configuration file path (e.g., config.local.json).
        /// Local override files are intended for developer-specific settings and should
        /// be excluded from source control via .gitignore.
        /// </summary>
        /// <param name="baseFilePath">Base configuration file path (e.g., "config.json")</param>
        /// <returns>Local override file path, regardless of whether the file exists</returns>
        public string GetLocalOverrideFilePath(string baseFilePath)
        {
            if (string.IsNullOrEmpty(baseFilePath))
            {
                return baseFilePath;
            }

            try
            {
                string directory = Path.GetDirectoryName(baseFilePath);
                string fileName = Path.GetFileNameWithoutExtension(baseFilePath);
                string extension = Path.GetExtension(baseFilePath);

                string localFileName = $"{fileName}.local{extension}";
                return string.IsNullOrEmpty(directory)
                    ? localFileName
                    : Path.Combine(directory, localFileName);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error determining local override file path for: {FilePath}", baseFilePath);
                return baseFilePath;
            }
        }

        /// <summary>
        /// Returns the ordered list of configuration files that should be loaded
        /// and merged for the current environment. Files appear in priority order
        /// (lowest to highest): base, environment-specific, local override.
        /// Only files that actually exist on disk are included.
        /// </summary>
        /// <param name="baseFilePath">Base configuration file path (e.g., "config.json")</param>
        /// <returns>Ordered list of existing file paths starting from the base file</returns>
        public IReadOnlyList<string> GetConfigurationFiles(string baseFilePath)
        {
            var files = new List<string>();

            if (string.IsNullOrEmpty(baseFilePath))
            {
                return files.AsReadOnly();
            }

            try
            {
                // 1. Base configuration
                if (File.Exists(baseFilePath))
                {
                    files.Add(baseFilePath);
                }

                // 2. Environment-specific configuration (e.g., config.Development.json)
                string directory = Path.GetDirectoryName(baseFilePath);
                string fileName = Path.GetFileNameWithoutExtension(baseFilePath);
                string extension = Path.GetExtension(baseFilePath);

                string envFileName = $"{fileName}.{_currentEnvironment}{extension}";
                string envFilePath = string.IsNullOrEmpty(directory)
                    ? envFileName
                    : Path.Combine(directory, envFileName);

                if (File.Exists(envFilePath) && 
                    !string.Equals(Path.GetFullPath(envFilePath), Path.GetFullPath(baseFilePath), StringComparison.OrdinalIgnoreCase))
                {
                    files.Add(envFilePath);
                }

                // 3. Local overrides (e.g., config.local.json)
                string localFilePath = GetLocalOverrideFilePath(baseFilePath);
                if (File.Exists(localFilePath))
                {
                    files.Add(localFilePath);
                }

                _logger?.LogDebug("Configuration file hierarchy for {BaseFilePath}: [{Files}]",
                    baseFilePath, string.Join(", ", files));
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error building configuration file hierarchy for: {FilePath}", baseFilePath);
            }

            return files.AsReadOnly();
        }
    }
}
