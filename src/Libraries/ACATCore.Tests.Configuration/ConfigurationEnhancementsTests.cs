////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ConfigurationEnhancementsTests.cs
//
// Tests for configuration system enhancements: schema validation,
// hot-reload, environment-specific configuration, and versioning
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Configuration;
using ACAT.Core.Utility;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading;

namespace ACATCore.Tests.Configuration
{
    /// <summary>
    /// Test configuration class
    /// </summary>
    public class TestConfiguration
    {
        public string Name { get; set; }
        public int Port { get; set; }
        public bool Enabled { get; set; }
        public string Version { get; set; }

        public static TestConfiguration CreateDefault()
        {
            return new TestConfiguration
            {
                Name = "Test",
                Port = 8080,
                Enabled = true,
                Version = "1.0.0"
            };
        }
    }

    [TestClass]
    public class ConfigurationEnhancementsTests
    {
        private string _testDirectory;
        private ILogger _logger;

        [TestInitialize]
        public void Setup()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), $"acat_test_{Guid.NewGuid()}");
            Directory.CreateDirectory(_testDirectory);

            // Use the shared logger factory instead of creating one with AddConsole
            _logger = LoggingConfiguration.CreateLogger<ConfigurationEnhancementsTests>();
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(_testDirectory))
            {
                try
                {
                    Directory.Delete(_testDirectory, true);
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
        }

        #region JSON Schema Validation Tests

        [TestMethod]
        public void JsonSchemaValidator_LoadSchema_ValidSchema_Success()
        {
            // Arrange
            var validator = new JsonSchemaValidator(_logger);
            string schemaPath = Path.Combine(_testDirectory, "test.schema.json");
            
            string schema = @"{
  ""$schema"": ""http://json-schema.org/draft-07/schema#"",
  ""type"": ""object"",
  ""required"": [""name""],
  ""properties"": {
    ""name"": { ""type"": ""string"" }
  }
}";
            File.WriteAllText(schemaPath, schema);

            // Act
            bool result = validator.LoadSchema("test", schemaPath);

            // Assert
            Assert.IsTrue(result, "Schema should load successfully");
        }

        [TestMethod]
        public void JsonSchemaValidator_Validate_ValidJson_Success()
        {
            // Arrange
            var validator = new JsonSchemaValidator(_logger);
            string schemaPath = Path.Combine(_testDirectory, "test.schema.json");
            string configPath = Path.Combine(_testDirectory, "test.json");
            
            string schema = @"{
  ""type"": ""object"",
  ""required"": [""name""],
  ""properties"": {
    ""name"": { ""type"": ""string"" }
  }
}";
            File.WriteAllText(schemaPath, schema);
            
            string config = @"{
  ""name"": ""TestConfig""
}";
            File.WriteAllText(configPath, config);
            
            validator.LoadSchema("test", schemaPath);

            // Act
            bool result = validator.Validate("test", configPath, out List<string> errors);

            // Assert
            Assert.IsTrue(result, "Validation should pass");
            Assert.AreEqual(0, errors.Count, "Should have no errors");
        }

        [TestMethod]
        public void JsonSchemaValidator_Validate_MissingRequired_Fails()
        {
            // Arrange
            var validator = new JsonSchemaValidator(_logger);
            string schemaPath = Path.Combine(_testDirectory, "test.schema.json");
            string configPath = Path.Combine(_testDirectory, "test.json");
            
            string schema = @"{
  ""type"": ""object"",
  ""required"": [""name""],
  ""properties"": {
    ""name"": { ""type"": ""string"" }
  }
}";
            File.WriteAllText(schemaPath, schema);
            
            string config = @"{
  ""value"": ""test""
}";
            File.WriteAllText(configPath, config);
            
            validator.LoadSchema("test", schemaPath);

            // Act
            bool result = validator.Validate("test", configPath, out List<string> errors);

            // Assert
            Assert.IsFalse(result, "Validation should fail");
            Assert.IsTrue(errors.Count > 0, "Should have validation errors");
        }

        [TestMethod]
        public void JsonSchemaValidator_Validate_Performance_CompletesWithin100ms()
        {
            const int MaxValidationTimeMs = 100;

            // Arrange
            var validator = new JsonSchemaValidator(_logger);
            string schemaPath = Path.Combine(_testDirectory, "perf.schema.json");
            string configPath = Path.Combine(_testDirectory, "perf.json");

            string schema = @"{
  ""type"": ""object"",
  ""required"": [""name"", ""port"", ""enabled""],
  ""properties"": {
    ""name"":    { ""type"": ""string""  },
    ""port"":    { ""type"": ""number""  },
    ""enabled"": { ""type"": ""boolean"" }
  }
}";
            File.WriteAllText(schemaPath, schema);

            string config = @"{
  ""name"": ""TestConfig"",
  ""port"": 8080,
  ""enabled"": true
}";
            File.WriteAllText(configPath, config);

            validator.LoadSchema("perf", schemaPath);

            // Warm up (excludes JIT from measurement)
            validator.Validate("perf", configPath, out _);

            // Act
            var stopwatch = Stopwatch.StartNew();
            bool result = validator.Validate("perf", configPath, out List<string> errors);
            stopwatch.Stop();

            // Assert
            Assert.IsTrue(result, "Validation should pass");
            Assert.AreEqual(0, errors.Count, "Should have no validation errors");
            Assert.IsTrue(stopwatch.ElapsedMilliseconds < MaxValidationTimeMs,
                $"Schema validation must complete in under {MaxValidationTimeMs}ms; took {stopwatch.ElapsedMilliseconds}ms");
        }

        #endregion

        #region Configuration Hot-Reload Tests

        [TestMethod]
        public void ConfigurationReloadService_StartMonitoring_ValidFile_Success()
        {
            // Arrange
            var reloadService = new ConfigurationReloadService(_logger);
            string configPath = Path.Combine(_testDirectory, "config.json");
            File.WriteAllText(configPath, "{}");

            // Act
            bool result = reloadService.StartMonitoring(configPath);

            // Assert
            Assert.IsTrue(result, "Monitoring should start successfully");
            
            // Cleanup
            reloadService.Dispose();
        }

        [TestMethod]
        public void ConfigurationReloadService_FileChanged_RaisesEvent()
        {
            // Arrange
            var reloadService = new ConfigurationReloadService(_logger);
            string configPath = Path.Combine(_testDirectory, "config.json");
            File.WriteAllText(configPath, @"{""name"": ""original""}");
            
            bool eventRaised = false;
            string eventFilePath = null;
            
            reloadService.ConfigurationReloaded += (sender, e) =>
            {
                eventRaised = true;
                eventFilePath = e.FilePath;
            };

            reloadService.StartMonitoring(configPath);

            // Act
            Thread.Sleep(100); // Let monitoring start
            File.WriteAllText(configPath, @"{""name"": ""modified""}");
            Thread.Sleep(1000); // Wait for debounce and event

            // Assert
            Assert.IsTrue(eventRaised, "ConfigurationReloaded event should be raised");
            Assert.IsNotNull(eventFilePath, "Event file path should be set");
            
            // Cleanup
            reloadService.Dispose();
        }

        [TestMethod]
        public void ConfigurationReloadService_StopMonitoring_StopsEvents()
        {
            // Arrange
            var reloadService = new ConfigurationReloadService(_logger);
            string configPath = Path.Combine(_testDirectory, "config.json");
            File.WriteAllText(configPath, @"{""name"": ""original""}");
            
            int eventCount = 0;
            
            reloadService.ConfigurationReloaded += (sender, e) =>
            {
                eventCount++;
            };

            reloadService.StartMonitoring(configPath);
            Thread.Sleep(100);
            
            // Act
            reloadService.StopMonitoring(configPath);
            File.WriteAllText(configPath, @"{""name"": ""modified""}");
            Thread.Sleep(1000);

            // Assert
            Assert.AreEqual(0, eventCount, "No events should be raised after stopping");
            
            // Cleanup
            reloadService.Dispose();
        }

        #endregion

        #region Environment-Specific Configuration Tests

        [TestMethod]
        public void EnvironmentConfiguration_DetectEnvironment_DefaultsToProduction()
        {
            // Arrange
            var envConfig = new EnvironmentConfiguration(_logger);

            // Act
            var environment = envConfig.CurrentEnvironment;

            // Assert
            Assert.AreEqual(ConfigurationEnvironment.Production, environment,
                "Should default to Production when no environment variable is set");
        }

        [TestMethod]
        public void EnvironmentConfiguration_GetEnvironmentFilePath_DevelopmentFile_ReturnsDevPath()
        {
            // Arrange
            var envConfig = new EnvironmentConfiguration(_logger);
            envConfig.SetEnvironment(ConfigurationEnvironment.Development);
            
            string basePath = Path.Combine(_testDirectory, "config.json");
            string devPath = Path.Combine(_testDirectory, "config.Development.json");
            
            File.WriteAllText(basePath, @"{""name"": ""base""}");
            File.WriteAllText(devPath, @"{""name"": ""dev""}");

            // Act
            string result = envConfig.GetEnvironmentFilePath(basePath);

            // Assert
            Assert.AreEqual(devPath, result, "Should return development-specific file path");
        }

        [TestMethod]
        public void EnvironmentConfiguration_GetEnvironmentFilePath_NoEnvFile_ReturnsBasePath()
        {
            // Arrange
            var envConfig = new EnvironmentConfiguration(_logger);
            envConfig.SetEnvironment(ConfigurationEnvironment.Development);
            
            string basePath = Path.Combine(_testDirectory, "config.json");
            File.WriteAllText(basePath, @"{""name"": ""base""}");

            // Act
            string result = envConfig.GetEnvironmentFilePath(basePath);

            // Assert
            Assert.AreEqual(basePath, result, "Should fall back to base file path");
        }

        [TestMethod]
        public void EnvironmentConfiguration_SetGetOverride_StoresValue()
        {
            // Arrange
            var envConfig = new EnvironmentConfiguration(_logger);

            // Act
            envConfig.SetOverride("TestKey", "TestValue");
            string result = envConfig.GetOverride("TestKey");

            // Assert
            Assert.AreEqual("TestValue", result, "Should retrieve stored override value");
        }

        #endregion

        #region Configuration Versioning Tests

        [TestMethod]
        public void ConfigurationVersion_Parse_ValidVersion_Success()
        {
            // Act
            var version = ConfigurationVersion.Parse("1.2.3");

            // Assert
            Assert.AreEqual(1, version.Major);
            Assert.AreEqual(2, version.Minor);
            Assert.AreEqual(3, version.Patch);
        }

        [TestMethod]
        public void ConfigurationVersion_IsNewerThan_NewerVersion_ReturnsTrue()
        {
            // Arrange
            var v1 = new ConfigurationVersion(1, 0, 0);
            var v2 = new ConfigurationVersion(2, 0, 0);

            // Act
            bool result = v2.IsNewerThan(v1);

            // Assert
            Assert.IsTrue(result, "Version 2.0.0 should be newer than 1.0.0");
        }

        [TestMethod]
        public void ConfigurationVersion_IsCompatibleWith_SameMajor_ReturnsTrue()
        {
            // Arrange
            var v1 = new ConfigurationVersion(1, 0, 0);
            var v2 = new ConfigurationVersion(1, 2, 3);

            // Act
            bool result = v1.IsCompatibleWith(v2);

            // Assert
            Assert.IsTrue(result, "Versions with same major version should be compatible");
        }

        [TestMethod]
        public void ConfigurationVersionManager_GetVersion_FileWithVersion_ReturnsVersion()
        {
            // Arrange
            var versionManager = new ConfigurationVersionManager(_logger);
            string configPath = Path.Combine(_testDirectory, "config.json");
            
            string config = @"{
  ""version"": ""1.2.3"",
  ""name"": ""test""
}";
            File.WriteAllText(configPath, config);

            // Act
            var version = versionManager.GetConfigurationVersion(configPath);

            // Assert
            Assert.AreEqual(1, version.Major);
            Assert.AreEqual(2, version.Minor);
            Assert.AreEqual(3, version.Patch);
        }

        [TestMethod]
        public void ConfigurationVersionManager_NeedsMigration_OlderVersion_ReturnsTrue()
        {
            // Arrange
            var versionManager = new ConfigurationVersionManager(_logger);
            versionManager.SetCurrentVersion("test", new ConfigurationVersion(2, 0, 0));
            
            string configPath = Path.Combine(_testDirectory, "config.json");
            string config = @"{
  ""version"": ""1.0.0"",
  ""name"": ""test""
}";
            File.WriteAllText(configPath, config);

            // Act
            bool result = versionManager.NeedsMigration("test", configPath);

            // Assert
            Assert.IsTrue(result, "Should need migration from 1.0.0 to 2.0.0");
        }

        #endregion

        #region Integration Tests

        [TestMethod]
        public void JsonConfigurationLoader_EnableHotReload_LoadsAndReloads()
        {
            // Arrange
            string configPath = Path.Combine(_testDirectory, "config.json");
            var config = new TestConfiguration { Name = "Original", Port = 8080 };
            string json = System.Text.Json.JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(configPath, json);

            var loader = new JsonConfigurationLoader<TestConfiguration>(
                validator: null,
                logger: _logger,
                enableHotReload: true,
                useEnvironmentConfig: false
            );

            bool reloadEventFired = false;
            loader.ConfigurationReloaded += (sender, e) => reloadEventFired = true;

            // Act
            var loadedConfig = loader.Load(configPath);
            loader.EnableHotReload(configPath);
            
            Thread.Sleep(100); // Let monitoring start
            
            // Modify file
            config.Name = "Modified";
            json = System.Text.Json.JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(configPath, json);
            
            Thread.Sleep(1000); // Wait for debounce and event

            // Assert
            Assert.IsNotNull(loadedConfig, "Configuration should load successfully");
            Assert.AreEqual("Original", loadedConfig.Name);
            Assert.IsTrue(reloadEventFired, "Reload event should fire");
            
            // Cleanup
            loader.Dispose();
        }

        [TestMethod]
        public void JsonConfigurationLoader_LoadWithEnvironment_LoadsDevConfig()
        {
            // Arrange
            string baseConfigPath = Path.Combine(_testDirectory, "config.json");
            string devConfigPath = Path.Combine(_testDirectory, "config.Development.json");
            
            var baseConfig = new TestConfiguration { Name = "Base", Port = 8080 };
            var devConfig = new TestConfiguration { Name = "Dev", Port = 9090 };
            
            File.WriteAllText(baseConfigPath, System.Text.Json.JsonSerializer.Serialize(baseConfig));
            File.WriteAllText(devConfigPath, System.Text.Json.JsonSerializer.Serialize(devConfig));

            var loader = new JsonConfigurationLoader<TestConfiguration>(
                validator: null,
                logger: _logger,
                enableHotReload: false,
                useEnvironmentConfig: true
            );

            // Manually set environment through loader's internal config
            Environment.SetEnvironmentVariable("ACAT_ENVIRONMENT", "Development");
            var envConfigLoader = new JsonConfigurationLoader<TestConfiguration>(
                validator: null,
                logger: _logger,
                enableHotReload: false,
                useEnvironmentConfig: true
            );

            // Act
            var config = envConfigLoader.LoadWithEnvironment(baseConfigPath);

            // Assert
            Assert.IsNotNull(config);
            Assert.AreEqual("Dev", config.Name, "Should load development configuration");
            
            // Cleanup
            Environment.SetEnvironmentVariable("ACAT_ENVIRONMENT", null);
        }

        #endregion
    }
}
