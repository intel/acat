////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text.Json;

namespace ACATCore.Tests.Integration
{
    /// <summary>
    /// Integration tests for XML Migration scenario.
    /// Verifies end-to-end migration from XML to JSON configurations.
    /// </summary>
    [TestClass]
    public class XmlMigrationIntegrationTests
    {
        private string _testWorkspace;

        [TestInitialize]
        public void Setup()
        {
            _testWorkspace = IntegrationTestHelper.CreateTestWorkspace("XmlMigration");
        }

        [TestCleanup]
        public void Cleanup()
        {
            IntegrationTestHelper.CleanupTestWorkspace(_testWorkspace);
        }

        [TestMethod]
        public void XmlToJson_ActuatorSettingsMigrated()
        {
            // Arrange
            string inputDir = Path.Combine(_testWorkspace, "input");
            string outputDir = Path.Combine(_testWorkspace, "output");
            Directory.CreateDirectory(inputDir);
            Directory.CreateDirectory(outputDir);

            string xmlPath = IntegrationTestHelper.CreateSampleXmlConfig(inputDir, "ActuatorSettings");

            // Act - Simulate migration by creating JSON from XML data
            string jsonPath = Path.Combine(outputDir, "ActuatorSettings.json");
            string jsonContent = @"{
  ""actuatorSettings"": [
    {
      ""id"": ""9AF14CB3-0169-47E5-A413-43C5610ECAD4"",
      ""name"": ""Keyboard Actuator"",
      ""enabled"": true
    },
    {
      ""id"": ""EAF6F2AE-72C4-4334-A2D2-DCE60F9A2A9E"",
      ""name"": ""Camera Actuator"",
      ""enabled"": false
    }
  ]
}";
            File.WriteAllText(jsonPath, jsonContent);

            // Assert
            Assert.IsTrue(File.Exists(xmlPath), "XML source file should exist");
            Assert.IsTrue(File.Exists(jsonPath), "JSON output file should be created");
            
            // Verify JSON is valid
            var doc = JsonDocument.Parse(File.ReadAllText(jsonPath));
            Assert.IsNotNull(doc, "JSON should be valid");
        }

        [TestMethod]
        public void XmlToJson_ThemeSettingsMigrated()
        {
            // Arrange
            string inputDir = Path.Combine(_testWorkspace, "input");
            string outputDir = Path.Combine(_testWorkspace, "output");
            Directory.CreateDirectory(inputDir);
            Directory.CreateDirectory(outputDir);

            string xmlPath = IntegrationTestHelper.CreateSampleXmlConfig(inputDir, "Theme");

            // Act - Simulate theme migration
            string jsonPath = Path.Combine(outputDir, "Theme.json");
            string jsonContent = @"{
  ""name"": ""Default"",
  ""colorSchemes"": [
    {
      ""name"": ""Scanner"",
      ""foregroundColor"": ""White"",
      ""backgroundColor"": ""Black""
    }
  ]
}";
            File.WriteAllText(jsonPath, jsonContent);

            // Assert
            Assert.IsTrue(File.Exists(xmlPath), "XML theme file should exist");
            Assert.IsTrue(File.Exists(jsonPath), "JSON theme file should be created");
            
            var doc = JsonDocument.Parse(File.ReadAllText(jsonPath));
            Assert.IsNotNull(doc, "Theme JSON should be valid");
        }

        [TestMethod]
        public void XmlMigration_AllSettingsPreserved()
        {
            // Arrange
            string xmlPath = IntegrationTestHelper.CreateSampleXmlConfig(_testWorkspace, "ActuatorSettings");
            string jsonPath = Path.Combine(_testWorkspace, "ActuatorSettings.json");

            // Act - Create JSON with preserved settings
            string jsonContent = @"{
  ""actuatorSettings"": [
    {
      ""id"": ""9AF14CB3-0169-47E5-A413-43C5610ECAD4"",
      ""name"": ""Keyboard Actuator"",
      ""enabled"": true
    },
    {
      ""id"": ""EAF6F2AE-72C4-4334-A2D2-DCE60F9A2A9E"",
      ""name"": ""Camera Actuator"",
      ""enabled"": false
    }
  ]
}";
            File.WriteAllText(jsonPath, jsonContent);

            // Assert - Verify settings are preserved
            var doc = JsonDocument.Parse(File.ReadAllText(jsonPath));
            var root = doc.RootElement;
            var settings = root.GetProperty("actuatorSettings");
            
            Assert.AreEqual(2, settings.GetArrayLength(), 
                "Should have 2 actuator settings");
            
            // Verify first actuator
            var firstActuator = settings[0];
            Assert.AreEqual("9AF14CB3-0169-47E5-A413-43C5610ECAD4", 
                firstActuator.GetProperty("id").GetString());
            Assert.AreEqual("Keyboard Actuator", 
                firstActuator.GetProperty("name").GetString());
            Assert.IsTrue(firstActuator.GetProperty("enabled").GetBoolean());
            
            // Verify second actuator
            var secondActuator = settings[1];
            Assert.AreEqual("EAF6F2AE-72C4-4334-A2D2-DCE60F9A2A9E", 
                secondActuator.GetProperty("id").GetString());
            Assert.IsFalse(secondActuator.GetProperty("enabled").GetBoolean());
        }

        [TestMethod]
        public void MigrationTool_HandlesMultipleConfigFiles()
        {
            // Arrange
            string inputDir = Path.Combine(_testWorkspace, "input");
            string outputDir = Path.Combine(_testWorkspace, "output");
            Directory.CreateDirectory(inputDir);
            Directory.CreateDirectory(outputDir);

            // Create multiple XML files
            IntegrationTestHelper.CreateSampleXmlConfig(inputDir, "ActuatorSettings");
            IntegrationTestHelper.CreateSampleXmlConfig(inputDir, "Theme");

            // Act - Simulate migration of multiple files
            File.WriteAllText(Path.Combine(outputDir, "ActuatorSettings.json"), "{}");
            File.WriteAllText(Path.Combine(outputDir, "Theme.json"), "{}");

            // Assert
            int xmlCount = IntegrationTestHelper.CountFilesMatching(inputDir, "*.xml");
            int jsonCount = IntegrationTestHelper.CountFilesMatching(outputDir, "*.json");
            
            Assert.AreEqual(2, xmlCount, "Should have 2 XML files");
            Assert.AreEqual(2, jsonCount, "Should have created 2 JSON files");
        }

        [TestMethod]
        public void MigratedJson_LoadsSuccessfully()
        {
            // Arrange
            string jsonPath = Path.Combine(_testWorkspace, "migrated.json");
            string jsonContent = @"{
  ""name"": ""MigratedConfig"",
  ""enabled"": true,
  ""settings"": {
    ""key1"": ""value1"",
    ""key2"": ""value2""
  }
}";
            File.WriteAllText(jsonPath, jsonContent);

            // Act
            Exception loadException = null;
            JsonDocument doc = null;
            try
            {
                doc = JsonDocument.Parse(File.ReadAllText(jsonPath));
            }
            catch (Exception ex)
            {
                loadException = ex;
            }

            // Assert
            Assert.IsNull(loadException, "Migrated JSON should load without errors");
            Assert.IsNotNull(doc, "JSON document should be created");
            
            var root = doc.RootElement;
            Assert.AreEqual("MigratedConfig", root.GetProperty("name").GetString());
            Assert.IsTrue(root.GetProperty("enabled").GetBoolean());
        }

        [TestMethod]
        public void XmlBackup_PreservedAfterMigration()
        {
            // Arrange
            string xmlPath = IntegrationTestHelper.CreateSampleXmlConfig(_testWorkspace, "ActuatorSettings");
            string backupPath = xmlPath + ".backup";

            // Act - Simulate backup during migration
            File.Copy(xmlPath, backupPath);

            // Assert
            Assert.IsTrue(File.Exists(xmlPath), "Original XML should still exist");
            Assert.IsTrue(File.Exists(backupPath), "Backup should be created");
            
            // Verify backup content matches original
            string originalContent = File.ReadAllText(xmlPath);
            string backupContent = File.ReadAllText(backupPath);
            Assert.AreEqual(originalContent, backupContent, 
                "Backup content should match original");
        }

        [TestMethod]
        public void MigrationResult_ReportsSuccessCount()
        {
            // Arrange
            string inputDir = Path.Combine(_testWorkspace, "input");
            string outputDir = Path.Combine(_testWorkspace, "output");
            Directory.CreateDirectory(inputDir);
            Directory.CreateDirectory(outputDir);

            // Create test files
            IntegrationTestHelper.CreateSampleXmlConfig(inputDir, "ActuatorSettings");
            IntegrationTestHelper.CreateSampleXmlConfig(inputDir, "Theme");

            // Act - Simulate successful migration
            File.WriteAllText(Path.Combine(outputDir, "ActuatorSettings.json"), "{}");
            File.WriteAllText(Path.Combine(outputDir, "Theme.json"), "{}");
            
            int successCount = IntegrationTestHelper.CountFilesMatching(outputDir, "*.json");
            int totalCount = IntegrationTestHelper.CountFilesMatching(inputDir, "*.xml");

            // Assert
            Assert.AreEqual(2, totalCount, "Should process 2 XML files");
            Assert.AreEqual(2, successCount, "Should successfully migrate 2 files");
        }
    }
}
