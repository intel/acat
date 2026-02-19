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
    public class EnvironmentConfiguration
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
                T config = JsonSerializer.Deserialize<T>(jsonContent, new JsonSerializerOptions
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
    }
}
