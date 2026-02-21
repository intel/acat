////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// JsonConfigurationLoaderTests.cs
//
// Unit tests for JsonConfigurationLoader
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Configuration;
using ACAT.Core.Utility;
using ACAT.Core.Validation;
using FluentValidation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace ACATCore.Tests.Configuration
{
    [TestClass]
    public class JsonConfigurationLoaderTests
    {
        private string _testDirectory;
        private string _testFilePath;

        [TestInitialize]
        public void Setup()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), "ACATTests_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);
            _testFilePath = Path.Combine(_testDirectory, "test-config.json");
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }

        [TestMethod]
        public void LoadNonExistentFileCreatesDefault()
        {
            // Arrange
            var loader = new JsonConfigurationLoader<ActuatorSettingsJson>();
            var nonExistentPath = Path.Combine(_testDirectory, "does-not-exist.json");

            // Act
            ActuatorSettingsJson config = loader.Load(nonExistentPath, createDefaultOnError: true);

            // Assert
            Assert.IsNotNull(config);
            Assert.IsNotNull(config.ActuatorSettings);
            Assert.IsTrue(File.Exists(nonExistentPath), "Default config should be created");
        }

        [TestMethod]
        public void LoadNonExistentFileReturnsNullWhenCreateDefaultFalse()
        {
            // Arrange
            var loader = new JsonConfigurationLoader<ActuatorSettingsJson>();
            var nonExistentPath = Path.Combine(_testDirectory, "does-not-exist.json");

            // Act
            ActuatorSettingsJson config = loader.Load(nonExistentPath, createDefaultOnError: false);

            // Assert
            Assert.IsNull(config);
            Assert.IsFalse(File.Exists(nonExistentPath));
        }

        [TestMethod]
        public void SaveAndLoadRoundTrip()
        {
            // Arrange
            var loader = new JsonConfigurationLoader<ActuatorSettingsJson>();
            var originalConfig = ActuatorSettingsJson.CreateDefault();
            originalConfig.ActuatorSettings[0].Description = "Test description";

            // Act - Save
            bool saveSuccess = loader.Save(originalConfig, _testFilePath);

            // Act - Load
            ActuatorSettingsJson loadedConfig = loader.Load(_testFilePath, createDefaultOnError: false);

            // Assert
            Assert.IsTrue(saveSuccess, "Save should succeed");
            Assert.IsNotNull(loadedConfig);
            Assert.AreEqual(originalConfig.ActuatorSettings.Count, loadedConfig.ActuatorSettings.Count);
            Assert.AreEqual(originalConfig.ActuatorSettings[0].Name, loadedConfig.ActuatorSettings[0].Name);
            Assert.AreEqual(originalConfig.ActuatorSettings[0].Description, loadedConfig.ActuatorSettings[0].Description);
        }

        [TestMethod]
        public void LoadValidatesWithValidator()
        {
            // Arrange
            var validator = new ActuatorSettingsValidator();
            var loader = new JsonConfigurationLoader<ActuatorSettingsJson>(validator);
            
            // Create invalid config (no enabled actuators)
            var invalidConfig = new ActuatorSettingsJson
            {
                ActuatorSettings = new System.Collections.Generic.List<ActuatorSettingJson>
                {
                    new ActuatorSettingJson
                    {
                        Name = "Test",
                        Id = Guid.NewGuid().ToString(),
                        Enabled = false
                    }
                }
            };
            
            // Save invalid config to file (bypass validation by using System.Text.Json directly)
            var json = System.Text.Json.JsonSerializer.Serialize(invalidConfig);
            File.WriteAllText(_testFilePath, json);

            // Act - Load with validation
            ActuatorSettingsJson loadedConfig = loader.Load(_testFilePath, createDefaultOnError: true);

            // Assert - Should return default due to validation failure
            Assert.IsNotNull(loadedConfig);
            // The loaded config should be default (which is valid)
            ValidationResult validationResult = validator.Validate(loadedConfig);
            Assert.IsTrue(validationResult.IsValid);
        }

        [TestMethod]
        public void SaveValidatesBeforeSaving()
        {
            // Arrange
            var validator = new ActuatorSettingsValidator();
            var loader = new JsonConfigurationLoader<ActuatorSettingsJson>(validator);
            
            // Create invalid config (no enabled actuators)
            var invalidConfig = new ActuatorSettingsJson
            {
                ActuatorSettings = new System.Collections.Generic.List<ActuatorSettingJson>
                {
                    new ActuatorSettingJson
                    {
                        Name = "Test",
                        Id = Guid.NewGuid().ToString(),
                        Enabled = false
                    }
                }
            };

            // Act
            bool saveSuccess = loader.Save(invalidConfig, _testFilePath);

            // Assert
            Assert.IsFalse(saveSuccess, "Save should fail for invalid config");
            Assert.IsFalse(File.Exists(_testFilePath), "File should not be created for invalid config");
        }

        [TestMethod]
        public void LoadEmptyFileCreatesDefault()
        {
            // Arrange
            var loader = new JsonConfigurationLoader<ActuatorSettingsJson>();
            File.WriteAllText(_testFilePath, "");

            // Act
            ActuatorSettingsJson config = loader.Load(_testFilePath, createDefaultOnError: true);

            // Assert
            Assert.IsNotNull(config);
            Assert.IsNotNull(config.ActuatorSettings);
        }

        [TestMethod]
        public void LoadInvalidJsonCreatesDefault()
        {
            // Arrange
            var loader = new JsonConfigurationLoader<ActuatorSettingsJson>();
            File.WriteAllText(_testFilePath, "{ invalid json }");

            // Act
            ActuatorSettingsJson config = loader.Load(_testFilePath, createDefaultOnError: true);

            // Assert
            Assert.IsNotNull(config);
            Assert.IsNotNull(config.ActuatorSettings);
        }

        [TestMethod]
        public void CreateDefaultCallsStaticFactoryMethod()
        {
            // Arrange
            var loader = new JsonConfigurationLoader<ActuatorSettingsJson>();

            // Act
            ActuatorSettingsJson config = loader.CreateDefault();

            // Assert
            Assert.IsNotNull(config);
            Assert.IsNotNull(config.ActuatorSettings);
            Assert.AreEqual(1, config.ActuatorSettings.Count);
            Assert.AreEqual("Keyboard", config.ActuatorSettings[0].Name);
        }

        [TestMethod]
        public void GetValidationErrorMessageReturnsEmptyForValidConfig()
        {
            // Arrange
            var validator = new ActuatorSettingsValidator();
            var loader = new JsonConfigurationLoader<ActuatorSettingsJson>(validator);
            var validConfig = ActuatorSettingsJson.CreateDefault();

            // Act
            var errorMessage = loader.GetValidationErrorMessage(validConfig);

            // Assert
            Assert.AreEqual(string.Empty, errorMessage);
        }

        [TestMethod]
        public void GetValidationErrorMessageReturnsMessageForInvalidConfig()
        {
            // Arrange
            var validator = new ActuatorSettingsValidator();
            var loader = new JsonConfigurationLoader<ActuatorSettingsJson>(validator);
            var invalidConfig = new ActuatorSettingsJson
            {
                ActuatorSettings = new System.Collections.Generic.List<ActuatorSettingJson>()
            };

            // Act
            var errorMessage = loader.GetValidationErrorMessage(invalidConfig);

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(errorMessage));
            Assert.IsTrue(errorMessage.Contains("At least one actuator"));
        }

        [TestMethod]
        public void SaveCreatesDirectoryIfNotExists()
        {
            // Arrange
            var loader = new JsonConfigurationLoader<ActuatorSettingsJson>();
            var nestedPath = Path.Combine(_testDirectory, "nested", "folder", "config.json");
            var config = ActuatorSettingsJson.CreateDefault();

            // Act
            bool success = loader.Save(config, nestedPath);

            // Assert
            Assert.IsTrue(success);
            Assert.IsTrue(File.Exists(nestedPath));
        }

        [TestMethod]
        public void LoadHandlesJsonCommentsAndTrailingCommas()
        {
            // Arrange
            var loader = new JsonConfigurationLoader<ActuatorSettingsJson>();
            var jsonWithComments = @"{
                // This is a comment
                ""actuatorSettings"": [
                    {
                        ""name"": ""Test"",
                        ""id"": ""d91a1877-c92b-4d7e-9ab6-f01f30b12df9"",
                        ""enabled"": true,
                        ""switchSettings"": [],
                    }
                ]
            }";
            File.WriteAllText(_testFilePath, jsonWithComments);

            // Act
            ActuatorSettingsJson config = loader.Load(_testFilePath, createDefaultOnError: false);

            // Assert
            Assert.IsNotNull(config);
            Assert.AreEqual(1, config.ActuatorSettings.Count);
            Assert.AreEqual("Test", config.ActuatorSettings[0].Name);
        }

        [TestMethod]
        public void LoadWithSchemaValidator_ValidJson_LoadsSuccessfully()
        {
            // Arrange
            string schemaPath = Path.Combine(_testDirectory, "test.schema.json");
            string schema = @"{
  ""type"": ""object"",
  ""required"": [""actuatorSettings""],
  ""properties"": {
    ""actuatorSettings"": { ""type"": ""array"" }
  }
}";
            File.WriteAllText(schemaPath, schema);

            var schemaValidator = new JsonSchemaValidator();
            schemaValidator.LoadSchema("actuator", schemaPath);

            var loader = new JsonConfigurationLoader<ActuatorSettingsJson>(
                schemaValidator: schemaValidator, schemaName: "actuator");

            var originalConfig = ActuatorSettingsJson.CreateDefault();
            var json = System.Text.Json.JsonSerializer.Serialize(originalConfig);
            File.WriteAllText(_testFilePath, json);

            // Act
            ActuatorSettingsJson config = loader.Load(_testFilePath, createDefaultOnError: false);

            // Assert
            Assert.IsNotNull(config);
            Assert.AreEqual(1, config.ActuatorSettings.Count);
        }

        [TestMethod]
        public void LoadWithSchemaValidator_StrictMode_InvalidJson_ReturnsDefault()
        {
            // Arrange
            string schemaPath = Path.Combine(_testDirectory, "test.schema.json");
            string schema = @"{
  ""type"": ""object"",
  ""required"": [""actuatorSettings""],
  ""properties"": {
    ""actuatorSettings"": { ""type"": ""array"" }
  }
}";
            File.WriteAllText(schemaPath, schema);

            var schemaValidator = new JsonSchemaValidator();
            schemaValidator.LoadSchema("actuator", schemaPath);

            // Create a JSON file missing the required 'actuatorSettings' property
            var invalidJson = @"{ ""otherProperty"": ""value"" }";
            File.WriteAllText(_testFilePath, invalidJson);

            var loader = new JsonConfigurationLoader<ActuatorSettingsJson>(
                schemaValidator: schemaValidator, schemaName: "actuator", strictMode: true);

            // Act
            ActuatorSettingsJson config = loader.Load(_testFilePath, createDefaultOnError: true);

            // Assert - strict mode should return default due to schema failure
            Assert.IsNotNull(config);
            Assert.IsNotNull(config.ActuatorSettings);
        }

        [TestMethod]
        public void LoadWithSchemaValidator_NonStrictMode_InvalidJson_StillDeserializes()
        {
            // Arrange
            string schemaPath = Path.Combine(_testDirectory, "test.schema.json");
            string schema = @"{
  ""type"": ""object"",
  ""required"": [""actuatorSettings""],
  ""properties"": {
    ""actuatorSettings"": { ""type"": ""array"" }
  }
}";
            File.WriteAllText(schemaPath, schema);

            var schemaValidator = new JsonSchemaValidator();
            schemaValidator.LoadSchema("actuator", schemaPath);

            // Create JSON missing required 'actuatorSettings' property, but still deserializable
            var missingRequiredJson = @"{ ""otherProperty"": ""value"" }";
            File.WriteAllText(_testFilePath, missingRequiredJson);

            var loader = new JsonConfigurationLoader<ActuatorSettingsJson>(
                schemaValidator: schemaValidator, schemaName: "actuator", strictMode: false);

            // Act - non-strict mode: schema failure should be a warning, not a failure
            ActuatorSettingsJson config = loader.Load(_testFilePath, createDefaultOnError: false);

            // Assert - non-strict mode still deserializes despite schema warning
            Assert.IsNotNull(config);
        }
    }
}
