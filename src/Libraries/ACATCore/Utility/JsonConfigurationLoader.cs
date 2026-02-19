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

using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using System;
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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="validator">FluentValidation validator (optional)</param>
        /// <param name="logger">Logger instance (optional)</param>
        public JsonConfigurationLoader(IValidator<T> validator = null, ILogger logger = null)
        {
            _validator = validator;
            _logger = logger ?? LogManager.GetLogger<JsonConfigurationLoader<T>>();
            
            // Configure JSON serialization options
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
                WriteIndented = true
            };
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

                T config = JsonSerializer.Deserialize<T>(json);

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
                string json = JsonSerializer.Serialize(config);
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
    }
}
