////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ConfigurationMigration.cs
//
// High-level utility for configuration file version detection and automatic
// migration. Wraps ConfigurationVersionManager to provide:
//   - Automatic version detection on load
//   - Backup creation before migration
//   - Sequential migration application
//   - Rollback capability (restore from backup)
//   - Comprehensive migration logging
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ACAT.Core.Utility
{
    /// <summary>
    /// Result of a configuration migration operation
    /// </summary>
    public class MigrationResult
    {
        /// <summary>
        /// Whether the migration succeeded
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Version migrated from
        /// </summary>
        public ConfigurationVersion FromVersion { get; set; }

        /// <summary>
        /// Version migrated to
        /// </summary>
        public ConfigurationVersion ToVersion { get; set; }

        /// <summary>
        /// Path of the backup file created before migration, or null if no backup was created
        /// </summary>
        public string BackupPath { get; set; }

        /// <summary>
        /// Error message if migration failed
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// List of migration steps applied
        /// </summary>
        public List<string> MigrationSteps { get; set; } = new List<string>();
    }

    /// <summary>
    /// Service for automatic detection and migration of configuration files.
    /// Detects the version in a config file on load, compares it with the
    /// current expected version, creates a backup, applies registered migrations
    /// in sequence, and supports rollback to the backed-up file on failure.
    /// </summary>
    public class ConfigurationMigrationService
    {
        private readonly ILogger _logger;
        private readonly ConfigurationVersionManager _versionManager;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">Logger instance (optional)</param>
        public ConfigurationMigrationService(ILogger logger = null)
        {
            _logger = logger ?? LogManager.GetLogger<ConfigurationMigrationService>();
            _versionManager = new ConfigurationVersionManager(_logger);
        }

        /// <summary>
        /// Register the current (target) version for a configuration type
        /// </summary>
        /// <param name="configType">Configuration type identifier</param>
        /// <param name="version">Current version to target</param>
        public void RegisterCurrentVersion(string configType, ConfigurationVersion version)
        {
            _versionManager.SetCurrentVersion(configType, version);
        }

        /// <summary>
        /// Register a migration handler for a configuration type
        /// </summary>
        /// <param name="configType">Configuration type identifier</param>
        /// <param name="migration">Migration handler implementing IConfigurationMigration</param>
        public void RegisterMigration(string configType, IConfigurationMigration migration)
        {
            _versionManager.RegisterMigration(configType, migration);
        }

        /// <summary>
        /// Get the version stored in a configuration file
        /// </summary>
        /// <param name="filePath">Path to the configuration file</param>
        /// <returns>Parsed ConfigurationVersion, or 1.0.0 if no version field found</returns>
        public ConfigurationVersion GetVersion(string filePath)
        {
            return _versionManager.GetConfigurationVersion(filePath);
        }

        /// <summary>
        /// Determine whether a configuration file needs migration
        /// </summary>
        /// <param name="configType">Configuration type identifier</param>
        /// <param name="filePath">Path to the configuration file</param>
        /// <returns>True if the file version is older than the registered current version</returns>
        public bool NeedsMigration(string configType, string filePath)
        {
            return _versionManager.NeedsMigration(configType, filePath);
        }

        /// <summary>
        /// Check whether a migration is needed and, if so, create a backup and apply all
        /// registered migrations in sequence.  If migration fails, the backup is restored
        /// (rollback) and an error result is returned.
        /// </summary>
        /// <param name="configType">Configuration type identifier</param>
        /// <param name="filePath">Path to the configuration file</param>
        /// <param name="createBackup">Whether to create a timestamped backup before migrating (default: true)</param>
        /// <returns>MigrationResult describing the outcome</returns>
        public MigrationResult MigrateIfNeeded(string configType, string filePath, bool createBackup = true)
        {
            var result = new MigrationResult();

            if (!File.Exists(filePath))
            {
                result.Success = false;
                result.ErrorMessage = $"Configuration file not found: {filePath}";
                _logger?.LogError("Configuration file not found: {FilePath}", filePath);
                return result;
            }

            ConfigurationVersion fileVersion = _versionManager.GetConfigurationVersion(filePath);
            result.FromVersion = fileVersion;

            if (!NeedsMigration(configType, filePath))
            {
                result.Success = true;
                _logger?.LogDebug(
                    "No migration needed for {ConfigType} at {FilePath} (version {Version})",
                    configType, filePath, fileVersion);
                return result;
            }

            _logger?.LogInformation(
                "Migration required for {ConfigType} at {FilePath} (current version: {Version})",
                configType, filePath, fileVersion);

            // Create backup
            string backupPath = null;
            if (createBackup)
            {
                backupPath = CreateBackup(filePath);
                if (backupPath == null)
                {
                    result.Success = false;
                    result.ErrorMessage = $"Failed to create backup for: {filePath}";
                    return result;
                }
                result.BackupPath = backupPath;
            }

            // Apply migrations
            bool migrationSuccess = ApplyMigrations(configType, filePath, result);

            if (!migrationSuccess && backupPath != null)
            {
                // Rollback to backup
                _logger?.LogWarning(
                    "Migration failed for {ConfigType}. Rolling back to backup: {BackupPath}",
                    configType, backupPath);
                RollbackFromBackup(filePath, backupPath, result);
            }

            return result;
        }

        /// <summary>
        /// Restore a configuration file from its backup.
        /// This method can be called manually to roll back to a previously created backup.
        /// </summary>
        /// <param name="filePath">Path to the configuration file to restore</param>
        /// <param name="backupPath">Path to the backup file</param>
        /// <returns>True if rollback succeeded, false otherwise</returns>
        public bool Rollback(string filePath, string backupPath)
        {
            if (!File.Exists(backupPath))
            {
                _logger?.LogError("Backup file not found, cannot roll back: {BackupPath}", backupPath);
                return false;
            }

            try
            {
                File.Copy(backupPath, filePath, overwrite: true);
                _logger?.LogInformation(
                    "Rolled back configuration {FilePath} from backup {BackupPath}",
                    filePath, backupPath);
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to roll back {FilePath} from backup {BackupPath}",
                    filePath, backupPath);
                return false;
            }
        }

        /// <summary>
        /// Return all backup files associated with a configuration file, sorted oldest-first.
        /// </summary>
        /// <param name="filePath">Path to the configuration file</param>
        /// <returns>Ordered list of backup file paths</returns>
        public IReadOnlyList<string> GetBackups(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return new List<string>();
            }

            string directory = Path.GetDirectoryName(filePath) ?? string.Empty;
            string fileName = Path.GetFileName(filePath);

            if (!Directory.Exists(directory))
            {
                return new List<string>();
            }

            string pattern = fileName + ".backup.*";
            string[] backups = Directory.GetFiles(directory, pattern);
            Array.Sort(backups);
            return backups.ToList();
        }

        // ----------------------------------------------------------------
        // Private helpers
        // ----------------------------------------------------------------

        /// <summary>
        /// Create a timestamped backup of a file.
        /// </summary>
        private string CreateBackup(string filePath)
        {
            try
            {
                string backupPath = $"{filePath}.backup.{DateTime.UtcNow:yyyyMMddHHmmssffff}";
                File.Copy(filePath, backupPath, overwrite: true);
                _logger?.LogInformation("Created backup: {BackupPath}", backupPath);
                return backupPath;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to create backup for: {FilePath}", filePath);
                return null;
            }
        }

        /// <summary>
        /// Apply registered migrations sequentially and write the result back to disk.
        /// </summary>
        private bool ApplyMigrations(string configType, string filePath, MigrationResult result)
        {
            try
            {
                bool success = _versionManager.MigrateConfiguration(configType, filePath, createBackup: false);
                if (success)
                {
                    result.ToVersion = _versionManager.GetConfigurationVersion(filePath);
                    result.Success = true;
                    _logger?.LogInformation(
                        "Migration complete for {ConfigType}: {FromVersion} -> {ToVersion}",
                        configType, result.FromVersion, result.ToVersion);
                }
                else
                {
                    result.Success = false;
                    result.ErrorMessage = $"Migration failed for config type '{configType}'";
                    _logger?.LogError(
                        "Migration failed for {ConfigType} at {FilePath}",
                        configType, filePath);
                }
                return success;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                _logger?.LogError(ex, "Unexpected error during migration for {ConfigType} at {FilePath}",
                    configType, filePath);
                return false;
            }
        }

        /// <summary>
        /// Restore original file from backup and record the rollback in the result.
        /// </summary>
        private void RollbackFromBackup(string filePath, string backupPath, MigrationResult result)
        {
            bool rolledBack = Rollback(filePath, backupPath);
            if (rolledBack)
            {
                result.MigrationSteps.Add($"Rolled back to backup: {backupPath}");
            }
            else
            {
                result.MigrationSteps.Add($"Rollback FAILED from backup: {backupPath}");
            }
        }
    }
}
