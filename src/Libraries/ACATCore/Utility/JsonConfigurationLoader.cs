////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// JsonConfigurationLoader.cs
//
// Generic JSON configuration loader with validation support.
// Loads configuration files with FluentValidation integration,
// error handling, and fallback to defaults.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Configuration;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace ACAT.Core.Utility
{
    /// <summary>
    /// Generic loader for JSON configuration files with validation
    /// </summary>
    /// <typeparam name="T">Configuration type to load</typeparam>
    public class JsonConfigurationLoader<T> where T : class, new()
    {
        private readonly ILogger _logger;
        private readonly IValidator<T> _validator;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly EnvironmentConfiguration _environmentConfig;
        private readonly ConfigurationReloadService _reloadService;
        private readonly JsonSchemaValidator _schemaValidator;
        private readonly string _schemaName;
        private readonly bool _strictMode;

        /// <summary>
        /// Event raised when configuration is reloaded
        /// </summary>
        public event EventHandler<ConfigurationReloadEventArgs> ConfigurationReloaded;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="validator">FluentValidation validator (optional)</param>
        /// <param name="logger">Logger instance (optional)</param>
        /// <param name="enableHotReload">Enable automatic configuration hot-reload (default: false)</param>
        /// <param name="useEnvironmentConfig">Enable environment-specific configuration (default: false)</param>
        /// <param name="schemaValidator">JSON schema validator for pre-deserialization validation (optional)</param>
        /// <param name="schemaName">Name of the schema to validate against (required when schemaValidator is provided)</param>
        /// <param name="strictMode">If true, schema validation failures cause load to fail; if false, failures are logged as warnings (default: false)</param>
        public JsonConfigurationLoader(IValidator<T> validator = null, ILogger logger = null, 
            bool enableHotReload = false, bool useEnvironmentConfig = false,
            JsonSchemaValidator schemaValidator = null, string schemaName = null, bool strictMode = false)
        {
            _validator = validator;
            _logger = logger ?? LogManager.GetLogger<JsonConfigurationLoader<T>>();
            _schemaValidator = schemaValidator;
            _schemaName = schemaName;
            _strictMode = strictMode;
            
            // Configure JSON serialization options
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
                WriteIndented = true
            };

            // Initialize environment configuration if enabled
            if (useEnvironmentConfig)
            {
                _environmentConfig = new EnvironmentConfiguration(_logger);
            }

            // Initialize reload service if enabled
            if (enableHotReload)
            {
                _reloadService = new ConfigurationReloadService(_logger);
                _reloadService.ConfigurationReloaded += OnConfigurationFileChanged;
            }
        }

        /// <summary>
        /// Load configuration from JSON file with validation and error handling
        /// </summary>
        /// <param name="filePath">Path to JSON configuration file</param>
        /// <param name="createDefaultOnError">If true, creates default config if file is missing or invalid</param>
        /// <returns>Configuration object or null on error</returns>
        public T Load(string filePath, bool createDefaultOnError = true)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                _logger?.LogError("Configuration file path is null or empty");
                return createDefaultOnError ? CreateDefault() : null;
            }

            try
            {
                // Check if file exists
                if (!File.Exists(filePath))
                {
                    _logger?.LogWarning("Configuration file not found: {FilePath}", filePath);
                    
                    if (createDefaultOnError)
                    {
                        _logger?.LogInformation("Creating default configuration at: {FilePath}", filePath);
                        T defaultConfig = CreateDefault();
                        Save(defaultConfig, filePath);
                        return defaultConfig;
                    }
                    
                    return null;
                }

                // Read and deserialize JSON
                string json = File.ReadAllText(filePath);
                
                if (string.IsNullOrWhiteSpace(json))
                {
                    _logger?.LogWarning("Configuration file is empty: {FilePath}", filePath);
                    return createDefaultOnError ? CreateDefault() : null;
                }

                // Validate JSON against schema before deserialization
                if (_schemaValidator != null && !string.IsNullOrEmpty(_schemaName))
                {
                    bool schemaValid = _schemaValidator.ValidateContent(_schemaName, json, out List<string> schemaErrors);

                    if (!schemaValid)
                    {
                        foreach (string error in schemaErrors ?? new List<string>())
                        {
                            if (_strictMode)
                                _logger?.LogError("Schema validation error in {FilePath}: {Error}", filePath, error);
                            else
                                _logger?.LogWarning("Schema validation warning in {FilePath}: {Error}", filePath, error);
                        }

                        if (_strictMode)
                        {
                            _logger?.LogError("Schema validation failed (strict mode) for: {FilePath}", filePath);
                            return createDefaultOnError ? CreateDefault() : null;
                        }

                        _logger?.LogWarning("Schema validation failed (non-strict mode), continuing with deserialization: {FilePath}", filePath);
                    }
                }
                else if (_schemaValidator != null && string.IsNullOrEmpty(_schemaName))
                {
                    _logger?.LogWarning("JsonSchemaValidator provided but schemaName is null or empty; schema validation will be skipped for: {FilePath}", filePath);
                }

                T config = System.Text.Json.JsonSerializer.Deserialize<T>(json, _jsonOptions);
                if (config == null)
                {
                    _logger?.LogError("Failed to deserialize configuration from: {FilePath}", filePath);
                    return createDefaultOnError ? CreateDefault() : null;
                }

                // Validate if validator is provided
                if (_validator != null)
                {
                    ValidationResult validationResult = _validator.Validate(config);
                    
                    if (!validationResult.IsValid)
                    {
                        _logger?.LogError("Configuration validation failed for: {FilePath}", filePath);
                        
                        foreach (ValidationFailure error in validationResult.Errors)
                        {
                            _logger?.LogError("  - {PropertyName}: {ErrorMessage}", 
                                error.PropertyName, error.ErrorMessage);
                        }

                        if (createDefaultOnError)
                        {
                            _logger?.LogWarning("Using default configuration due to validation errors");
                            return CreateDefault();
                        }
                        
                        return null;
                    }
                }

                _logger?.LogInformation("Successfully loaded configuration from: {FilePath}", filePath);
                return config;
            }
            catch (JsonException ex)
            {
                _logger?.LogError(ex, "JSON parsing error in configuration file: {FilePath}", filePath);
                
                if (createDefaultOnError)
                {
                    _logger?.LogWarning("Using default configuration due to JSON parsing error");
                    return CreateDefault();
                }
                
                return null;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Unexpected error loading configuration from: {FilePath}", filePath);
                
                if (createDefaultOnError)
                {
                    _logger?.LogWarning("Using default configuration due to unexpected error");
                    return CreateDefault();
                }
                
                return null;
            }
        }

        /// <summary>
        /// Save configuration to JSON file
        /// </summary>
        /// <param name="config">Configuration object to save</param>
        /// <param name="filePath">Path to save JSON file</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool Save(T config, string filePath)
        {
            if (config == null)
            {
                _logger?.LogError("Cannot save null configuration");
                return false;
            }

            if (string.IsNullOrEmpty(filePath))
            {
                _logger?.LogError("Configuration file path is null or empty");
                return false;
            }

            try
            {
                // Validate before saving if validator is provided
                if (_validator != null)
                {
                    ValidationResult validationResult = _validator.Validate(config);
                    
                    if (!validationResult.IsValid)
                    {
                        _logger?.LogError("Cannot save invalid configuration to: {FilePath}", filePath);
                        
                        foreach (ValidationFailure error in validationResult.Errors)
                        {
                            _logger?.LogError("  - {PropertyName}: {ErrorMessage}", 
                                error.PropertyName, error.ErrorMessage);
                        }
                        
                        return false;
                    }
                }

                // Ensure directory exists
                string directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Serialize and save
                string json = System.Text.Json.JsonSerializer.Serialize(config);
                File.WriteAllText(filePath, json);
                
                _logger?.LogInformation("Successfully saved configuration to: {FilePath}", filePath);
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error saving configuration to: {FilePath}", filePath);
                return false;
            }
        }

        /// <summary>
        /// Create a default configuration instance
        /// </summary>
        /// <returns>New default configuration object</returns>
        public T CreateDefault()
        {
            try
            {
                // Try to call a static CreateDefault() method if it exists
                MethodInfo createDefaultMethod = typeof(T).GetMethod("CreateDefault", 
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                
                if (createDefaultMethod != null && createDefaultMethod.ReturnType == typeof(T))
                {
                    return (T)createDefaultMethod.Invoke(null, null);
                }
                
                // Otherwise, use default constructor
                return new T();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error creating default configuration");
                return new T();
            }
        }

        /// <summary>
        /// Get user-friendly error message for validation failures
        /// </summary>
        /// <param name="config">Configuration to validate</param>
        /// <returns>Error message or empty string if valid</returns>
        public string GetValidationErrorMessage(T config)
        {
            if (_validator == null || config == null)
            {
                return string.Empty;
            }

            ValidationResult validationResult = _validator.Validate(config);
            
            if (validationResult.IsValid)
            {
                return string.Empty;
            }

            var errorMessage = "Configuration validation failed:\n";
            foreach (ValidationFailure error in validationResult.Errors)
            {
                errorMessage += $"- {error.PropertyName}: {error.ErrorMessage}\n";
            }

            return errorMessage;
        }

        /// <summary>
        /// Enable hot-reload for a configuration file
        /// </summary>
        /// <param name="filePath">Path to configuration file to monitor</param>
        /// <returns>True if monitoring started successfully</returns>
        public bool EnableHotReload(string filePath)
        {
            if (_reloadService == null)
            {
                _logger?.LogWarning("Hot-reload service not initialized. Create loader with enableHotReload=true");
                return false;
            }

            return _reloadService.StartMonitoring(filePath);
        }

        /// <summary>
        /// Disable hot-reload for a configuration file
        /// </summary>
        /// <param name="filePath">Path to configuration file</param>
        /// <returns>True if monitoring stopped successfully</returns>
        public bool DisableHotReload(string filePath)
        {
            if (_reloadService == null)
            {
                return false;
            }

            return _reloadService.StopMonitoring(filePath);
        }

        /// <summary>
        /// Handle configuration file changed event
        /// </summary>
        private void OnConfigurationFileChanged(object sender, ConfigurationReloadEventArgs e)
        {
            if (e.Success)
            {
                _logger?.LogInformation("Configuration file reloaded: {FilePath}", e.FilePath);
                ConfigurationReloaded?.Invoke(this, e);
            }
            else
            {
                _logger?.LogError("Configuration file reload failed: {FilePath}. Error: {Error}", 
                    e.FilePath, e.ErrorMessage);
            }
        }

        /// <summary>
        /// Load configuration with environment-specific overrides
        /// </summary>
        /// <param name="baseFilePath">Base configuration file path</param>
        /// <param name="createDefaultOnError">If true, creates default config if file is missing or invalid</param>
        /// <returns>Configuration object with environment overrides applied</returns>
        public T LoadWithEnvironment(string baseFilePath, bool createDefaultOnError = true)
        {
            if (_environmentConfig == null)
            {
                _logger?.LogWarning("Environment configuration not initialized. Create loader with useEnvironmentConfig=true");
                return Load(baseFilePath, createDefaultOnError);
            }

            string filePath = _environmentConfig.GetEnvironmentFilePath(baseFilePath);
            T config = Load(filePath, createDefaultOnError);

            if (config != null)
            {
                // Apply environment variable overrides
                var properties = typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                
                foreach (var property in properties)
                {
                    if (!property.CanWrite)
                    {
                        continue;
                    }

                    string envVarName = $"ACAT_{property.Name.ToUpperInvariant()}";
                    string envVarValue = Environment.GetEnvironmentVariable(envVarName);

                    if (!string.IsNullOrEmpty(envVarValue))
                    {
                        try
                        {
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

            return config;
        }

        /// <summary>
        /// Get the current environment
        /// </summary>
        /// <returns>Current configuration environment or null if not enabled</returns>
        public ConfigurationEnvironment? GetCurrentEnvironment()
        {
            return _environmentConfig?.CurrentEnvironment;
        }

        /// <summary>
        /// Dispose of resources
        /// </summary>
        public void Dispose()
        {
            _reloadService?.Dispose();
        }
    }
}
