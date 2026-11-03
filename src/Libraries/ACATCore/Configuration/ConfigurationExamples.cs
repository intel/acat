////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ConfigurationExamples.cs
//
// Example implementations showing how to use the enhanced configuration
// system features
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Configuration;
using ACAT.Core.Utility;
using ACAT.Core.Validation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace ACAT.Core.Examples
{
    /// <summary>
    /// Example configuration class
    /// </summary>
    public class AppConfiguration
    {
        public string ApplicationName { get; set; }
        public int Port { get; set; }
        public bool EnableLogging { get; set; }
        public string Version { get; set; }

        public static AppConfiguration CreateDefault()
        {
            return new AppConfiguration
            {
                ApplicationName = "ACAT",
                Port = 8080,
                EnableLogging = true,
                Version = "1.0.0"
            };
        }
    }

    /// <summary>
    /// Example 1: Basic configuration loading with schema validation
    /// </summary>
    public class Example1_SchemaValidation
    {
        private readonly ILogger _logger;
        private readonly JsonSchemaValidator _schemaValidator;

        public Example1_SchemaValidation(ILogger logger)
        {
            _logger = logger;
            _schemaValidator = new JsonSchemaValidator(_logger);
            
            // Load schema
            _schemaValidator.LoadSchema("app-config", "schemas/json/app-config.schema.json");
        }

        public AppConfiguration LoadConfiguration(string configPath)
        {
            // Validate against schema first
            if (!_schemaValidator.Validate("app-config", configPath, out List<string> errors))
            {
                _logger.LogError("Configuration validation failed:");
                foreach (var error in errors)
                {
                    _logger.LogError("  - {Error}", error);
                }
                throw new Exception("Invalid configuration");
            }

            // Load configuration
            var loader = new JsonConfigurationLoader<AppConfiguration>(logger: _logger);
            return loader.Load(configPath);
        }
    }

    /// <summary>
    /// Example 2: Configuration with hot-reload support
    /// </summary>
    public class Example2_HotReload
    {
        private readonly ILogger _logger;
        private readonly JsonConfigurationLoader<AppConfiguration> _loader;
        private AppConfiguration _currentConfig;

        public Example2_HotReload(ILogger logger)
        {
            _logger = logger;
            
            // Create loader with hot-reload enabled
            _loader = new JsonConfigurationLoader<AppConfiguration>(
                validator: null,
                logger: _logger,
                enableHotReload: true
            );

            // Subscribe to reload events
            _loader.ConfigurationReloaded += OnConfigurationReloaded;
        }

        public AppConfiguration LoadConfiguration(string configPath)
        {
            // Load initial configuration
            _currentConfig = _loader.Load(configPath);
            
            // Enable hot-reload monitoring
            _loader.EnableHotReload(configPath);
            
            _logger.LogInformation("Configuration loaded with hot-reload enabled");
            return _currentConfig;
        }

        private void OnConfigurationReloaded(object sender, ConfigurationReloadEventArgs e)
        {
            if (e.Success)
            {
                _logger.LogInformation("Configuration reloaded: {FilePath}", e.FilePath);
                
                // Reload and apply new configuration
                _currentConfig = _loader.Load(e.FilePath);
                ApplyConfiguration(_currentConfig);
            }
            else
            {
                _logger.LogError("Configuration reload failed: {Error}", e.ErrorMessage);
            }
        }

        private void ApplyConfiguration(AppConfiguration config)
        {
            // Apply the new configuration to your application
            _logger.LogInformation("Applying new configuration: {AppName}, Port: {Port}", 
                config.ApplicationName, config.Port);
            
            // Notify other components of configuration change
            ConfigurationChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler ConfigurationChanged;
    }

    /// <summary>
    /// Example 3: Environment-specific configuration
    /// </summary>
    public class Example3_EnvironmentConfig
    {
        private readonly ILogger _logger;
        private readonly JsonConfigurationLoader<AppConfiguration> _loader;
        private readonly EnvironmentConfiguration _envConfig;

        public Example3_EnvironmentConfig(ILogger logger)
        {
            _logger = logger;
            
            // Create loader with environment support
            _loader = new JsonConfigurationLoader<AppConfiguration>(
                validator: null,
                logger: _logger,
                enableHotReload: false,
                useEnvironmentConfig: true
            );

            _envConfig = new EnvironmentConfiguration(_logger);
        }

        public AppConfiguration LoadConfiguration(string baseConfigPath)
        {
            // Check current environment
            var environment = _envConfig.CurrentEnvironment;
            _logger.LogInformation("Loading configuration for environment: {Environment}", environment);

            // Load configuration with environment overrides
            var config = _loader.LoadWithEnvironment(baseConfigPath);
            
            _logger.LogInformation("Loaded configuration from: {Path}", 
                _envConfig.GetEnvironmentFilePath(baseConfigPath));

            return config;
        }

        public void SetEnvironmentOverride(string key, string value)
        {
            _envConfig.SetOverride(key, value);
            _logger.LogInformation("Set environment override: {Key} = {Value}", key, value);
        }
    }

    /// <summary>
    /// Example 4: Configuration versioning and migration
    /// </summary>
    public class Example4_Versioning
    {
        private readonly ILogger _logger;
        private readonly ConfigurationVersionManager _versionManager;

        public Example4_Versioning(ILogger logger)
        {
            _logger = logger;
            _versionManager = new ConfigurationVersionManager(_logger);
            
            // Set current version
            _versionManager.SetCurrentVersion("app-config", 
                new ConfigurationVersion(2, 0, 0));
            
            // Register migrations
            // _versionManager.RegisterMigration("app-config", new MyMigration());
        }

        public AppConfiguration LoadConfiguration(string configPath)
        {
            // Check if migration is needed
            if (_versionManager.NeedsMigration("app-config", configPath))
            {
                _logger.LogInformation("Configuration migration required");
                
                // Get current version
                var currentVersion = _versionManager.GetConfigurationVersion(configPath);
                _logger.LogInformation("Current configuration version: {Version}", currentVersion);
                
                // Perform migration (creates backup automatically)
                bool migrationSuccess = _versionManager.MigrateConfiguration(
                    "app-config", 
                    configPath, 
                    createBackup: true
                );

                if (!migrationSuccess)
                {
                    throw new Exception("Configuration migration failed");
                }
                
                _logger.LogInformation("Configuration migrated successfully");
            }

            // Load configuration
            var loader = new JsonConfigurationLoader<AppConfiguration>(logger: _logger);
            return loader.Load(configPath);
        }
    }

    /// <summary>
    /// Example 5: Complete configuration management with all features
    /// </summary>
    public class Example5_CompleteConfigurationManager
    {
        private readonly ILogger _logger;
        private readonly JsonConfigurationLoader<AppConfiguration> _loader;
        private readonly JsonSchemaValidator _schemaValidator;
        private readonly ConfigurationVersionManager _versionManager;
        private readonly EnvironmentConfiguration _envConfig;
        private AppConfiguration _currentConfig;

        public Example5_CompleteConfigurationManager(ILogger logger)
        {
            _logger = logger;
            
            // Initialize schema validator
            _schemaValidator = new JsonSchemaValidator(_logger);
            _schemaValidator.LoadSchema("app-config", "schemas/json/app-config.schema.json");
            
            // Initialize version manager
            _versionManager = new ConfigurationVersionManager(_logger);
            _versionManager.SetCurrentVersion("app-config", new ConfigurationVersion(2, 0, 0));
            
            // Initialize environment config
            _envConfig = new EnvironmentConfiguration(_logger);
            
            // Initialize loader with all features
            _loader = new JsonConfigurationLoader<AppConfiguration>(
                validator: null, // Could add FluentValidation validator here
                logger: _logger,
                enableHotReload: true,
                useEnvironmentConfig: true
            );
            
            // Subscribe to reload events
            _loader.ConfigurationReloaded += OnConfigurationReloaded;
        }

        public AppConfiguration Initialize(string baseConfigPath)
        {
            _logger.LogInformation("Initializing configuration system...");
            _logger.LogInformation("Environment: {Environment}", _envConfig.CurrentEnvironment);

            // Get environment-specific config path
            string configPath = _envConfig.GetEnvironmentFilePath(baseConfigPath);
            _logger.LogInformation("Config path: {Path}", configPath);

            // Check and perform migration if needed
            if (_versionManager.NeedsMigration("app-config", configPath))
            {
                _logger.LogInformation("Migrating configuration...");
                if (!_versionManager.MigrateConfiguration("app-config", configPath, createBackup: true))
                {
                    throw new Exception("Configuration migration failed");
                }
            }

            // Validate schema
            if (!_schemaValidator.Validate("app-config", configPath, out List<string> errors))
            {
                _logger.LogError("Schema validation failed:");
                foreach (var error in errors)
                {
                    _logger.LogError("  - {Error}", error);
                }
                throw new Exception("Invalid configuration schema");
            }

            // Load configuration with environment overrides
            _currentConfig = _loader.LoadWithEnvironment(baseConfigPath);
            
            // Enable hot-reload
            _loader.EnableHotReload(configPath);
            
            _logger.LogInformation("Configuration system initialized successfully");
            return _currentConfig;
        }

        private void OnConfigurationReloaded(object sender, ConfigurationReloadEventArgs e)
        {
            if (e.Success)
            {
                _logger.LogInformation("Configuration reloaded: {FilePath}", e.FilePath);
                
                // Validate schema on reload
                if (_schemaValidator.Validate("app-config", e.FilePath, out List<string> errors))
                {
                    // Reload configuration
                    _currentConfig = _loader.Load(e.FilePath);
                    
                    // Notify application
                    _logger.LogInformation("New configuration applied");
                    ConfigurationChanged?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    _logger.LogError("Reloaded configuration failed schema validation, keeping current configuration");
                }
            }
            else
            {
                _logger.LogError("Configuration reload failed: {Error}", e.ErrorMessage);
            }
        }

        public AppConfiguration GetCurrentConfiguration()
        {
            return _currentConfig;
        }

        public event EventHandler ConfigurationChanged;
    }
}
