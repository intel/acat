////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ConfigurationMigratorTests.cs
//
// Unit tests for configuration migrator
//
////////////////////////////////////////////////////////////////////////////

using ACAT.ConfigMigrationTool;
using System.Text.Json;

namespace ACAT.ConfigMigrationTool.Tests
{
    [TestClass]
    public class ConfigurationMigratorTests
    {
        private string _testInputDir = "";
        private string _testOutputDir = "";
        private ConfigurationMigrator _migrator = null!;

        [TestInitialize]
        public void Setup()
        {
            _testInputDir = Path.Combine(Path.GetTempPath(), "acat-input-" + Guid.NewGuid().ToString());
            _testOutputDir = Path.Combine(Path.GetTempPath(), "acat-output-" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testInputDir);
            Directory.CreateDirectory(_testOutputDir);
            _migrator = new ConfigurationMigrator();
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(_testInputDir))
            {
                Directory.Delete(_testInputDir, true);
            }
            if (Directory.Exists(_testOutputDir))
            {
                Directory.Delete(_testOutputDir, true);
            }
        }

        [TestMethod]
        [Ignore("Spectre.Console doesn't support concurrent interactive displays in test environments")]
        public async Task MigrateAsync_WithValidActuatorSettings_Success()
        {
            // Arrange
            var xmlContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<ActuatorConfig>
  <ActuatorSettings>
    <ActuatorSetting>
      <Name>Keyboard</Name>
      <Id>d91a1877-c92b-4d7e-9ab6-f01f30b12df9</Id>
      <Description>Test</Description>
      <Enabled>true</Enabled>
      <ImageFileName>test.jpg</ImageFileName>
      <SwitchSettings>
        <SwitchSetting>
          <Name>Trigger</Name>
          <Source>F12</Source>
          <Enabled>true</Enabled>
          <Actuate>true</Actuate>
          <Command>@Trigger</Command>
          <MinHoldTime>@MinActuationHoldTime</MinHoldTime>
          <BeepFile>beep.wav</BeepFile>
        </SwitchSetting>
      </SwitchSettings>
    </ActuatorSetting>
  </ActuatorSettings>
</ActuatorConfig>";

            var xmlPath = Path.Combine(_testInputDir, "ActuatorSettings.xml");
            File.WriteAllText(xmlPath, xmlContent);

            // Act
            var result = await _migrator.MigrateAsync(_testInputDir, _testOutputDir, false, false);

            // Assert
            Assert.AreEqual(1, result.TotalFiles);
            Assert.AreEqual(1, result.SuccessCount);
            Assert.AreEqual(0, result.FailureCount);

            var jsonPath = Path.Combine(_testOutputDir, "ActuatorSettings.json");
            Assert.IsTrue(File.Exists(jsonPath));

            var jsonContent = File.ReadAllText(jsonPath);
            Assert.IsTrue(jsonContent.Contains("\"name\": \"Keyboard\""));
        }

        [TestMethod]
        [Ignore("Spectre.Console doesn't support concurrent interactive displays in test environments")]
        public async Task MigrateAsync_WithDryRun_NoFilesCreated()
        {
            // Arrange
            var xmlContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<ACAT>
  <Theme description=""Test"">
    <ColorSchemes>
      <ColorScheme name=""Scanner"" background=""#232433"" foreground=""White"" />
    </ColorSchemes>
  </Theme>
</ACAT>";

            var xmlPath = Path.Combine(_testInputDir, "Theme.xml");
            File.WriteAllText(xmlPath, xmlContent);

            // Act
            var result = await _migrator.MigrateAsync(_testInputDir, _testOutputDir, true, false);

            // Assert
            Assert.AreEqual(1, result.TotalFiles);
            Assert.AreEqual(1, result.SuccessCount);
            Assert.IsTrue(result.DryRun);

            var jsonPath = Path.Combine(_testOutputDir, "Theme.json");
            Assert.IsFalse(File.Exists(jsonPath), "Dry run should not create output files");
        }

        [TestMethod]
        public async Task MigrateAsync_WithBackup_CreatesBackupFiles()
        {
            // Arrange
            var xmlContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<ACAT>
  <Theme description=""Test"">
    <ColorSchemes>
      <ColorScheme name=""Scanner"" background=""#232433"" foreground=""White"" />
    </ColorSchemes>
  </Theme>
</ACAT>";

            var xmlPath = Path.Combine(_testInputDir, "Theme.xml");
            File.WriteAllText(xmlPath, xmlContent);

            // Act
            var result = await _migrator.MigrateAsync(_testInputDir, _testOutputDir, false, true);

            // Assert
            Assert.AreEqual(1, result.BackedUpFiles.Count);
            var backupPath = xmlPath + ".backup";
            Assert.IsTrue(File.Exists(backupPath), "Backup file should be created");
        }

        [TestMethod]
        public async Task MigrateAsync_EmptyDirectory_ReturnsZeroFiles()
        {
            // Act
            var result = await _migrator.MigrateAsync(_testInputDir, _testOutputDir, false, false);

            // Assert
            Assert.AreEqual(0, result.TotalFiles);
            Assert.AreEqual(0, result.SuccessCount);
            Assert.AreEqual(0, result.FailureCount);
        }

        [TestMethod]
        public async Task ValidateAsync_ValidJson_Success()
        {
            // Arrange
            var json = @"{
  ""description"": ""Test Theme"",
  ""colorSchemes"": [
    {
      ""name"": ""Scanner"",
      ""background"": ""#232433"",
      ""foreground"": ""White""
    }
  ]
}";

            var jsonPath = Path.Combine(_testInputDir, "Theme.json");
            File.WriteAllText(jsonPath, json);

            // Act
            var result = await _migrator.ValidateAsync(_testInputDir);

            // Assert
            Assert.AreEqual(1, result.TotalFiles);
            Assert.AreEqual(1, result.SuccessCount);
            Assert.AreEqual(0, result.FailureCount);
        }

        [TestMethod]
        public async Task ValidateAsync_InvalidJson_ReportsError()
        {
            // Arrange
            var json = @"{
  ""actuatorSettings"": [
    {
      ""name"": """",
      ""id"": ""invalid-guid"",
      ""enabled"": true
    }
  ]
}";

            var jsonPath = Path.Combine(_testInputDir, "ActuatorSettings.json");
            File.WriteAllText(jsonPath, json);

            // Act
            var result = await _migrator.ValidateAsync(_testInputDir);

            // Assert
            Assert.AreEqual(1, result.TotalFiles);
            Assert.AreEqual(0, result.SuccessCount);
            Assert.AreEqual(1, result.FailureCount);
        }

        [TestMethod]
        public async Task RollbackAsync_WithBackupFiles_RestoresOriginals()
        {
            // Arrange
            var originalContent = "Original content";
            var modifiedContent = "Modified content";

            var xmlPath = Path.Combine(_testInputDir, "test.xml");
            var backupPath = xmlPath + ".backup";

            File.WriteAllText(backupPath, originalContent);
            File.WriteAllText(xmlPath, modifiedContent);

            // Act
            var result = await _migrator.RollbackAsync(_testInputDir);

            // Assert
            Assert.AreEqual(1, result.TotalFiles);
            Assert.AreEqual(1, result.SuccessCount);
            Assert.AreEqual(0, result.FailureCount);

            var restoredContent = File.ReadAllText(xmlPath);
            Assert.AreEqual(originalContent, restoredContent);
            Assert.IsFalse(File.Exists(backupPath), "Backup file should be deleted after rollback");
        }
    }
}
