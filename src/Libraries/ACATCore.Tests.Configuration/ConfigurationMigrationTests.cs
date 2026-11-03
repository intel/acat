////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ConfigurationMigrationTests.cs
//
// Unit tests for ConfigurationMigrationService, MigrationBase, and
// version field presence on config JSON classes.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Configuration;
using ACAT.Core.Utility;
using ACAT.Core.Utility.Migrations;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text.Json;

namespace ACATCore.Tests.Configuration
{
    // -----------------------------------------------------------------------
    // Concrete migration used only in tests: 1.0.0 -> 2.0.0
    // Adds a "migrated" boolean flag to the JSON document.
    // -----------------------------------------------------------------------
    internal class TestMigration_V1_to_V2 : MigrationBase
    {
        public TestMigration_V1_to_V2(ILogger logger = null) : base(logger) { }

        public override ConfigurationVersion FromVersion => new ConfigurationVersion(1, 0, 0);
        public override ConfigurationVersion ToVersion   => new ConfigurationVersion(2, 0, 0);

        protected override bool ApplyMigration(JsonElement source, out JsonElement result, out string error)
        {
            error  = null;
            result = BuildMigratedElement(source, new System.Collections.Generic.Dictionary<string, object>
            {
                { "migrated", true }
            });
            return true;
        }
    }

    // Migration that always fails (for rollback testing)
    internal class FailingMigration : MigrationBase
    {
        public FailingMigration(ILogger logger = null) : base(logger) { }

        public override ConfigurationVersion FromVersion => new ConfigurationVersion(1, 0, 0);
        public override ConfigurationVersion ToVersion   => new ConfigurationVersion(2, 0, 0);

        protected override bool ApplyMigration(JsonElement source, out JsonElement result, out string error)
        {
            error  = "Intentional failure for testing";
            result = source;
            return false;
        }
    }

    [TestClass]
    public class ConfigurationMigrationTests
    {
        private string _testDir;
        private ILogger _logger;

        [TestInitialize]
        public void Setup()
        {
            _testDir = Path.Combine(Path.GetTempPath(), $"acat_migration_test_{Guid.NewGuid()}");
            Directory.CreateDirectory(_testDir);
            _logger = LoggingConfiguration.CreateLogger<ConfigurationMigrationTests>();
        }

        [TestCleanup]
        public void Cleanup()
        {
            try { Directory.Delete(_testDir, true); } catch { /* ignore */ }
        }

        // -----------------------------------------------------------------------
        // Version field presence in config classes
        // -----------------------------------------------------------------------

        [TestMethod]
        public void AbbreviationsJson_HasVersionField_DefaultsTo_1_0_0()
        {
            var config = new AbbreviationsJson();
            Assert.AreEqual("1.0.0", config.Version, "AbbreviationsJson should default to version 1.0.0");
        }

        [TestMethod]
        public void ActuatorSettingsJson_HasVersionField_DefaultsTo_1_0_0()
        {
            var config = new ActuatorSettingsJson();
            Assert.AreEqual("1.0.0", config.Version, "ActuatorSettingsJson should default to version 1.0.0");
        }

        [TestMethod]
        public void ThemeJson_HasVersionField_DefaultsTo_1_0_0()
        {
            var config = new ThemeJson();
            Assert.AreEqual("1.0.0", config.Version, "ThemeJson should default to version 1.0.0");
        }

        [TestMethod]
        public void PronunciationsJson_HasVersionField_DefaultsTo_1_0_0()
        {
            var config = new PronunciationsJson();
            Assert.AreEqual("1.0.0", config.Version, "PronunciationsJson should default to version 1.0.0");
        }

        [TestMethod]
        public void PanelConfigJson_HasVersionField_DefaultsTo_1_0_0()
        {
            var config = new PanelConfigJson();
            Assert.AreEqual("1.0.0", config.Version, "PanelConfigJson should default to version 1.0.0");
        }

        [TestMethod]
        public void PreferredWordPredictorsJson_HasVersionField_DefaultsTo_1_0_0()
        {
            var config = new PreferredWordPredictorsJson();
            Assert.AreEqual("1.0.0", config.Version, "PreferredWordPredictorsJson should default to version 1.0.0");
        }

        // -----------------------------------------------------------------------
        // Migration detection (NeedsMigration)
        // -----------------------------------------------------------------------

        [TestMethod]
        public void NeedsMigration_FileVersionMatchesCurrent_ReturnsFalse()
        {
            var service = new ConfigurationMigrationService(_logger);
            service.RegisterCurrentVersion("test", new ConfigurationVersion(1, 0, 0));

            string path = WriteConfig(@"{""version"":""1.0.0"",""name"":""test""}");

            Assert.IsFalse(service.NeedsMigration("test", path),
                "No migration needed when file version equals current version");
        }

        [TestMethod]
        public void NeedsMigration_FileVersionOlderThanCurrent_ReturnsTrue()
        {
            var service = new ConfigurationMigrationService(_logger);
            service.RegisterCurrentVersion("test", new ConfigurationVersion(2, 0, 0));

            string path = WriteConfig(@"{""version"":""1.0.0"",""name"":""test""}");

            Assert.IsTrue(service.NeedsMigration("test", path),
                "Migration needed when file version is older than current version");
        }

        [TestMethod]
        public void NeedsMigration_NoVersionField_DefaultsTo_1_0_0_AndDetectsMigration()
        {
            var service = new ConfigurationMigrationService(_logger);
            service.RegisterCurrentVersion("test", new ConfigurationVersion(2, 0, 0));

            string path = WriteConfig(@"{""name"":""test""}");

            Assert.IsTrue(service.NeedsMigration("test", path),
                "File with no version field defaults to 1.0.0 and should need migration to 2.0.0");
        }

        // -----------------------------------------------------------------------
        // Automatic backup
        // -----------------------------------------------------------------------

        [TestMethod]
        public void MigrateIfNeeded_WithBackup_CreatesBackupFile()
        {
            var service = new ConfigurationMigrationService(_logger);
            service.RegisterCurrentVersion("test", new ConfigurationVersion(2, 0, 0));
            service.RegisterMigration("test", new TestMigration_V1_to_V2(_logger));

            string path = WriteConfig(@"{""version"":""1.0.0"",""name"":""original""}");

            var result = service.MigrateIfNeeded("test", path, createBackup: true);

            Assert.IsTrue(result.Success, "Migration should succeed");
            Assert.IsNotNull(result.BackupPath, "BackupPath should be set");
            Assert.IsTrue(File.Exists(result.BackupPath), "Backup file should exist on disk");
        }

        [TestMethod]
        public void MigrateIfNeeded_WithBackup_BackupContainsOriginalContent()
        {
            const string originalJson = @"{""version"":""1.0.0"",""name"":""original""}";
            var service = new ConfigurationMigrationService(_logger);
            service.RegisterCurrentVersion("test", new ConfigurationVersion(2, 0, 0));
            service.RegisterMigration("test", new TestMigration_V1_to_V2(_logger));

            string path = WriteConfig(originalJson);
            var result = service.MigrateIfNeeded("test", path, createBackup: true);

            string backupContent = File.ReadAllText(result.BackupPath);
            Assert.AreEqual(originalJson, backupContent,
                "Backup file should contain the original pre-migration content");
        }

        [TestMethod]
        public void MigrateIfNeeded_WithoutBackup_NoBackupFileCreated()
        {
            var service = new ConfigurationMigrationService(_logger);
            service.RegisterCurrentVersion("test", new ConfigurationVersion(2, 0, 0));
            service.RegisterMigration("test", new TestMigration_V1_to_V2(_logger));

            string path = WriteConfig(@"{""version"":""1.0.0"",""name"":""test""}");
            var result = service.MigrateIfNeeded("test", path, createBackup: false);

            Assert.IsTrue(result.Success);
            Assert.IsNull(result.BackupPath, "No backup should be created when createBackup=false");
        }

        // -----------------------------------------------------------------------
        // Migration execution
        // -----------------------------------------------------------------------

        [TestMethod]
        public void MigrateIfNeeded_AppliesMigration_UpdatesVersionInFile()
        {
            var service = new ConfigurationMigrationService(_logger);
            service.RegisterCurrentVersion("test", new ConfigurationVersion(2, 0, 0));
            service.RegisterMigration("test", new TestMigration_V1_to_V2(_logger));

            string path = WriteConfig(@"{""version"":""1.0.0"",""name"":""test""}");
            var result = service.MigrateIfNeeded("test", path);

            Assert.IsTrue(result.Success);
            var afterVersion = service.GetVersion(path);
            Assert.AreEqual(2, afterVersion.Major, "Version in file should be updated to 2.x.x after migration");
        }

        [TestMethod]
        public void MigrateIfNeeded_ReturnsCorrectFromAndToVersions()
        {
            var service = new ConfigurationMigrationService(_logger);
            service.RegisterCurrentVersion("test", new ConfigurationVersion(2, 0, 0));
            service.RegisterMigration("test", new TestMigration_V1_to_V2(_logger));

            string path = WriteConfig(@"{""version"":""1.0.0"",""name"":""test""}");
            var result = service.MigrateIfNeeded("test", path);

            Assert.AreEqual(1, result.FromVersion.Major);
            Assert.AreEqual(2, result.ToVersion.Major);
        }

        [TestMethod]
        public void MigrateIfNeeded_NoMigrationNeeded_ReturnsSuccessWithoutModifying()
        {
            const string json = @"{""version"":""2.0.0"",""name"":""test""}";
            var service = new ConfigurationMigrationService(_logger);
            service.RegisterCurrentVersion("test", new ConfigurationVersion(2, 0, 0));

            string path = WriteConfig(json);
            var result = service.MigrateIfNeeded("test", path);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(json, File.ReadAllText(path), "File should not be modified when no migration is needed");
        }

        // -----------------------------------------------------------------------
        // Rollback
        // -----------------------------------------------------------------------

        [TestMethod]
        public void MigrateIfNeeded_FailingMigration_RollsBackToOriginal()
        {
            const string originalJson = @"{""version"":""1.0.0"",""name"":""original""}";
            var service = new ConfigurationMigrationService(_logger);
            service.RegisterCurrentVersion("test", new ConfigurationVersion(2, 0, 0));
            service.RegisterMigration("test", new FailingMigration(_logger));

            string path = WriteConfig(originalJson);
            var result = service.MigrateIfNeeded("test", path, createBackup: true);

            Assert.IsFalse(result.Success, "Migration should report failure");
            string afterContent = File.ReadAllText(path);
            Assert.AreEqual(originalJson, afterContent,
                "File should be restored to original content after failed migration");
        }

        [TestMethod]
        public void Rollback_ValidBackup_RestoresFile()
        {
            const string originalJson = @"{""version"":""1.0.0"",""name"":""original""}";
            var service = new ConfigurationMigrationService(_logger);
            service.RegisterCurrentVersion("test", new ConfigurationVersion(2, 0, 0));
            service.RegisterMigration("test", new TestMigration_V1_to_V2(_logger));

            string path = WriteConfig(originalJson);
            var result = service.MigrateIfNeeded("test", path, createBackup: true);

            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.BackupPath);

            // Manually roll back
            bool rolledBack = service.Rollback(path, result.BackupPath);
            Assert.IsTrue(rolledBack, "Rollback should succeed");
            Assert.AreEqual(originalJson, File.ReadAllText(path), "File should match original after rollback");
        }

        [TestMethod]
        public void Rollback_MissingBackupFile_ReturnsFalse()
        {
            var service = new ConfigurationMigrationService(_logger);
            string path = WriteConfig(@"{""version"":""2.0.0""}");

            bool result = service.Rollback(path, Path.Combine(_testDir, "nonexistent.backup"));
            Assert.IsFalse(result, "Rollback with missing backup should return false");
        }

        // -----------------------------------------------------------------------
        // GetBackups
        // -----------------------------------------------------------------------

        [TestMethod]
        public void GetBackups_AfterMigration_ReturnsAtLeastOneBackup()
        {
            var service = new ConfigurationMigrationService(_logger);
            service.RegisterCurrentVersion("test", new ConfigurationVersion(2, 0, 0));
            service.RegisterMigration("test", new TestMigration_V1_to_V2(_logger));

            string path = WriteConfig(@"{""version"":""1.0.0"",""name"":""test""}");
            service.MigrateIfNeeded("test", path, createBackup: true);

            var backups = service.GetBackups(path);
            Assert.IsTrue(backups.Count >= 1, "At least one backup should exist after migration");
        }

        [TestMethod]
        public void GetBackups_NoMigrationDone_ReturnsEmpty()
        {
            var service = new ConfigurationMigrationService(_logger);
            string path = WriteConfig(@"{""version"":""1.0.0""}");

            var backups = service.GetBackups(path);
            Assert.AreEqual(0, backups.Count, "No backups should exist when migration was never run");
        }

        // -----------------------------------------------------------------------
        // GetVersion
        // -----------------------------------------------------------------------

        [TestMethod]
        public void GetVersion_FileWithVersionField_ReturnsParsedVersion()
        {
            var service = new ConfigurationMigrationService(_logger);
            string path = WriteConfig(@"{""version"":""3.5.2"",""name"":""test""}");

            var version = service.GetVersion(path);
            Assert.AreEqual(3, version.Major);
            Assert.AreEqual(5, version.Minor);
            Assert.AreEqual(2, version.Patch);
        }

        [TestMethod]
        public void GetVersion_FileWithNoVersionField_Returns_1_0_0()
        {
            var service = new ConfigurationMigrationService(_logger);
            string path = WriteConfig(@"{""name"":""test""}");

            var version = service.GetVersion(path);
            Assert.AreEqual(new ConfigurationVersion(1, 0, 0), version,
                "Missing version field should default to 1.0.0");
        }

        // -----------------------------------------------------------------------
        // MigrationBase helpers
        // -----------------------------------------------------------------------

        [TestMethod]
        public void MigrationBase_BuildMigratedElement_SetsVersionToToVersion()
        {
            var migration = new TestMigration_V1_to_V2();
            var source = JsonDocument.Parse(@"{""version"":""1.0.0"",""name"":""test""}").RootElement;

            bool ok = migration.Migrate(source, out JsonElement result, out string error);

            Assert.IsTrue(ok);
            Assert.IsNull(error);
            Assert.IsTrue(result.TryGetProperty("version", out var v));
            Assert.AreEqual("2.0.0", v.GetString(), "Version in result should be updated to ToVersion");
        }

        [TestMethod]
        public void MigrationBase_BuildMigratedElement_PreservesExistingProperties()
        {
            var migration = new TestMigration_V1_to_V2();
            var source = JsonDocument.Parse(@"{""version"":""1.0.0"",""name"":""original""}").RootElement;

            migration.Migrate(source, out JsonElement result, out _);

            Assert.IsTrue(result.TryGetProperty("name", out var name));
            Assert.AreEqual("original", name.GetString(), "Existing properties should be preserved");
        }

        // -----------------------------------------------------------------------
        // Edge cases
        // -----------------------------------------------------------------------

        [TestMethod]
        public void MigrateIfNeeded_FileNotFound_ReturnsFailure()
        {
            var service = new ConfigurationMigrationService(_logger);
            service.RegisterCurrentVersion("test", new ConfigurationVersion(2, 0, 0));

            var result = service.MigrateIfNeeded("test", Path.Combine(_testDir, "nonexistent.json"));

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.ErrorMessage);
        }

        // -----------------------------------------------------------------------
        // Helpers
        // -----------------------------------------------------------------------

        private string WriteConfig(string json)
        {
            string path = Path.Combine(_testDir, $"config_{Guid.NewGuid():N}.json");
            File.WriteAllText(path, json);
            return path;
        }
    }
}
