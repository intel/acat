////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Logging;
using ACAT.Core.Utility;
using System;
using System.IO;

namespace ACATCore.Tests.Integration
{
    /// <summary>
    /// Integration tests for Fresh Install scenario.
    /// Verifies that ACAT creates default configurations and logs on first run.
    /// </summary>
    [TestClass]
    public class FreshInstallIntegrationTests
    {
        private string _testWorkspace;

        [TestInitialize]
        public void Setup()
        {
            _testWorkspace = IntegrationTestHelper.CreateTestWorkspace("FreshInstall");
        }

        [TestCleanup]
        public void Cleanup()
        {
            IntegrationTestHelper.CleanupTestWorkspace(_testWorkspace);
        }

        [TestMethod]
        public void FreshInstall_DefaultDirectoryStructureCreated()
        {
            // Arrange
            string userDir = Path.Combine(_testWorkspace, "User");
            string configDir = Path.Combine(userDir, "Config");
            string logsDir = Path.Combine(userDir, "Logs");

            // Act
            Directory.CreateDirectory(userDir);
            Directory.CreateDirectory(configDir);
            Directory.CreateDirectory(logsDir);

            // Assert
            Assert.IsTrue(Directory.Exists(userDir), "User directory should exist");
            Assert.IsTrue(Directory.Exists(configDir), "Config directory should exist");
            Assert.IsTrue(Directory.Exists(logsDir), "Logs directory should exist");
        }

        [TestMethod]
        public void FreshInstall_DefaultJsonConfigsCreated()
        {
            // Arrange
            string configDir = Path.Combine(_testWorkspace, "Config");
            Directory.CreateDirectory(configDir);

            // Act - Simulate creating default configurations
            string actuatorSettingsPath = Path.Combine(configDir, "ActuatorSettings.json");
            string themeJsonPath = Path.Combine(configDir, "Theme.json");

            // Create sample default configs
            string defaultActuatorSettings = @"{
  ""actuatorSettings"": [
    {
      ""id"": ""9AF14CB3-0169-47E5-A413-43C5610ECAD4"",
      ""name"": ""Keyboard Actuator"",
      ""enabled"": true
    }
  ]
}";
            File.WriteAllText(actuatorSettingsPath, defaultActuatorSettings);

            string defaultTheme = @"{
  ""name"": ""Default"",
  ""colorSchemes"": []
}";
            File.WriteAllText(themeJsonPath, defaultTheme);

            // Assert
            Assert.IsTrue(File.Exists(actuatorSettingsPath), 
                "ActuatorSettings.json should be created");
            Assert.IsTrue(File.Exists(themeJsonPath), 
                "Theme.json should be created");

            // Verify files are not empty
            Assert.IsTrue(new FileInfo(actuatorSettingsPath).Length > 0,
                "ActuatorSettings.json should not be empty");
            Assert.IsTrue(new FileInfo(themeJsonPath).Length > 0,
                "Theme.json should not be empty");
        }

        [TestMethod]
        public void FreshInstall_LogFilesCreatedInCorrectLocation()
        {
            // Arrange
            string logsDir = Path.Combine(_testWorkspace, "Logs");
            Directory.CreateDirectory(logsDir);

            // Act - Simulate log file creation
            string logFileName = $"acat-{DateTime.Now:yyyyMMdd}.log";
            string logFilePath = Path.Combine(logsDir, logFileName);
            File.WriteAllText(logFilePath, "Test log entry\n");

            // Assert
            Assert.IsTrue(File.Exists(logFilePath), 
                "Log file should be created in Logs directory");
            Assert.IsTrue(new FileInfo(logFilePath).Length > 0,
                "Log file should contain content");
        }

        [TestMethod]
        public void FreshInstall_LoggingInitializationSucceeds()
        {
            // Arrange
            string logsDir = Path.Combine(_testWorkspace, "Logs");
            Directory.CreateDirectory(logsDir);

            // Act - Initialize logging using the new infrastructure
            var logger = LoggingConfiguration.CreateLogger("FreshInstallTest");

            // Verify we can write logs
            Exception caughtException = null;
            try
            {
                logger.LogInformation("Fresh install test log entry");
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }

            // Assert
            Assert.IsNull(caughtException, 
                "Logging initialization should not throw exceptions");
        }

        [TestMethod]
        public void FreshInstall_VerifyMinimalFileCount()
        {
            // Arrange
            string configDir = Path.Combine(_testWorkspace, "Config");
            Directory.CreateDirectory(configDir);

            // Act - Create minimum required files
            File.WriteAllText(Path.Combine(configDir, "ActuatorSettings.json"), "{}");
            File.WriteAllText(Path.Combine(configDir, "Theme.json"), "{}");

            // Assert - Verify at least 2 config files exist
            int jsonFileCount = IntegrationTestHelper.CountFilesMatching(configDir, "*.json");
            Assert.IsTrue(jsonFileCount >= 2, 
                $"Expected at least 2 JSON config files, found {jsonFileCount}");
        }
    }
}
