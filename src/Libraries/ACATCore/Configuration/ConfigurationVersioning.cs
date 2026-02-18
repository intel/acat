////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ConfigurationVersioning.cs
//
// Support for configuration file versioning and migration
//
////////////////////////////////////////////////////////////////////////////

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ACAT.Core.Configuration
{
    /// <summary>
    /// Configuration version information
    /// </summary>
    public class ConfigurationVersion
    {
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Patch { get; set; }

        public ConfigurationVersion()
        {
            Major = 1;
            Minor = 0;
            Patch = 0;
        }

        public ConfigurationVersion(int major, int minor, int patch)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
        }

        public override string ToString()
        {
            return $"{Major}.{Minor}.{Patch}";
        }

        public static ConfigurationVersion Parse(string version)
        {
            if (string.IsNullOrEmpty(version))
            {
                return new ConfigurationVersion();
            }

            string[] parts = version.Split('.');
            if (parts.Length >= 1 && int.TryParse(parts[0], out int major))
            {
                int minor = parts.Length >= 2 && int.TryParse(parts[1], out int m) ? m : 0;
                int patch = parts.Length >= 3 && int.TryParse(parts[2], out int p) ? p : 0;
                return new ConfigurationVersion(major, minor, patch);
            }

            return new ConfigurationVersion();
        }

        public bool IsNewerThan(ConfigurationVersion other)
        {
            if (Major > other.Major) return true;
            if (Major < other.Major) return false;
            if (Minor > other.Minor) return true;
            if (Minor < other.Minor) return false;
            return Patch > other.Patch;
        }

        public bool IsCompatibleWith(ConfigurationVersion other)
        {
            // Same major version is considered compatible
            return Major == other.Major;
        }
    }

    /// <summary>
    /// Interface for configuration migration handlers
    /// </summary>
    public interface IConfigurationMigration
    {
        ConfigurationVersion FromVersion { get; }
        ConfigurationVersion ToVersion { get; }
        bool Migrate(JsonElement source, out JsonElement result, out string error);
    }

    /// <summary>
    /// Service for managing configuration versioning and migrations
    /// </summary>
    public class ConfigurationVersionManager
    {
        private readonly ILogger _logger;
        private readonly Dictionary<string, List<IConfigurationMigration>> _migrations;
        private readonly Dictionary<string, ConfigurationVersion> _currentVersions;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">Logger instance (optional)</param>
        public ConfigurationVersionManager(ILogger logger = null)
        {
            _logger = logger ?? Utility.LoggingConfiguration.CreateLogger<ConfigurationVersionManager>();
            _migrations = new Dictionary<string, List<IConfigurationMigration>>();
            _currentVersions = new Dictionary<string, ConfigurationVersion>();
        }

        /// <summary>
        /// Register a migration handler for a configuration type
        /// </summary>
        /// <param name="configType">Configuration type identifier</param>
        /// <param name="migration">Migration handler</param>
        public void RegisterMigration(string configType, IConfigurationMigration migration)
        {
            if (string.IsNullOrEmpty(configType))
            {
                _logger?.LogWarning("Cannot register migration with null or empty config type");
                return;
            }

            if (migration == null)
            {
                _logger?.LogWarning("Cannot register null migration");
                return;
            }

            if (!_migrations.ContainsKey(configType))
            {
                _migrations[configType] = new List<IConfigurationMigration>();
            }

            _migrations[configType].Add(migration);
            _logger?.LogInformation("Registered migration for {ConfigType}: {FromVersion} -> {ToVersion}",
                configType, migration.FromVersion, migration.ToVersion);
        }

        /// <summary>
        /// Set the current version for a configuration type
        /// </summary>
        /// <param name="configType">Configuration type identifier</param>
        /// <param name="version">Current version</param>
        public void SetCurrentVersion(string configType, ConfigurationVersion version)
        {
            if (string.IsNullOrEmpty(configType))
            {
                _logger?.LogWarning("Cannot set version with null or empty config type");
                return;
            }

            _currentVersions[configType] = version;
            _logger?.LogInformation("Set current version for {ConfigType}: {Version}", configType, version);
        }

        /// <summary>
        /// Get the version from a configuration file
        /// </summary>
        /// <param name="filePath">Path to configuration file</param>
        /// <returns>Configuration version or default (1.0.0) if not specified</returns>
        public ConfigurationVersion GetConfigurationVersion(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    _logger?.LogWarning("Configuration file not found: {FilePath}", filePath);
                    return new ConfigurationVersion();
                }

                string jsonContent = File.ReadAllText(filePath);
                using (JsonDocument doc = JsonDocument.Parse(jsonContent))
                {
                    JsonElement root = doc.RootElement;
                    
                    if (root.TryGetProperty("version", out JsonElement versionElement))
                    {
                        string versionString = versionElement.GetString();
                        return ConfigurationVersion.Parse(versionString);
                    }
                    
                    if (root.TryGetProperty("configVersion", out versionElement))
                    {
                        string versionString = versionElement.GetString();
                        return ConfigurationVersion.Parse(versionString);
                    }
                }

                _logger?.LogDebug("No version found in configuration file, using default: {FilePath}", filePath);
                return new ConfigurationVersion();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error reading configuration version from: {FilePath}", filePath);
                return new ConfigurationVersion();
            }
        }

        /// <summary>
        /// Check if a configuration file needs migration
        /// </summary>
        /// <param name="configType">Configuration type identifier</param>
        /// <param name="filePath">Path to configuration file</param>
        /// <returns>True if migration is needed</returns>
        public bool NeedsMigration(string configType, string filePath)
        {
            if (!_currentVersions.ContainsKey(configType))
            {
                _logger?.LogDebug("No current version registered for {ConfigType}", configType);
                return false;
            }

            ConfigurationVersion fileVersion = GetConfigurationVersion(filePath);
            ConfigurationVersion currentVersion = _currentVersions[configType];

            bool needsMigration = currentVersion.IsNewerThan(fileVersion);
            
            if (needsMigration)
            {
                _logger?.LogInformation("Configuration {FilePath} needs migration from {FileVersion} to {CurrentVersion}",
                    filePath, fileVersion, currentVersion);
            }

            return needsMigration;
        }

        /// <summary>
        /// Migrate a configuration file to the current version
        /// </summary>
        /// <param name="configType">Configuration type identifier</param>
        /// <param name="filePath">Path to configuration file</param>
        /// <param name="createBackup">Whether to create a backup before migration</param>
        /// <returns>True if migration was successful</returns>
        public bool MigrateConfiguration(string configType, string filePath, bool createBackup = true)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    _logger?.LogError("Configuration file not found: {FilePath}", filePath);
                    return false;
                }

                if (!_currentVersions.ContainsKey(configType))
                {
                    _logger?.LogError("No current version registered for {ConfigType}", configType);
                    return false;
                }

                if (!_migrations.ContainsKey(configType) || _migrations[configType].Count == 0)
                {
                    _logger?.LogWarning("No migrations registered for {ConfigType}", configType);
                    return false;
                }

                // Create backup if requested
                if (createBackup)
                {
                    string backupPath = $"{filePath}.backup.{DateTime.Now:yyyyMMddHHmmss}";
                    File.Copy(filePath, backupPath, true);
                    _logger?.LogInformation("Created backup: {BackupPath}", backupPath);
                }

                // Load current configuration
                string jsonContent = File.ReadAllText(filePath);
                using (JsonDocument doc = JsonDocument.Parse(jsonContent))
                {
                    JsonElement current = doc.RootElement.Clone();
                    ConfigurationVersion fromVersion = GetConfigurationVersion(filePath);
                    ConfigurationVersion toVersion = _currentVersions[configType];

                    // Find and apply migrations in sequence
                    List<IConfigurationMigration> applicableMigrations = FindMigrationPath(configType, fromVersion, toVersion);
                    
                    if (applicableMigrations.Count == 0)
                    {
                        _logger?.LogInformation("No migration path found from {FromVersion} to {ToVersion}",
                            fromVersion, toVersion);
                        return false;
                    }

                    foreach (var migration in applicableMigrations)
                    {
                        if (!migration.Migrate(current, out JsonElement result, out string error))
                        {
                            _logger?.LogError("Migration failed from {FromVersion} to {ToVersion}: {Error}",
                                migration.FromVersion, migration.ToVersion, error);
                            return false;
                        }
                        
                        current = result;
                        _logger?.LogInformation("Applied migration: {FromVersion} -> {ToVersion}",
                            migration.FromVersion, migration.ToVersion);
                    }

                    // Save migrated configuration
                    string migratedJson = JsonSerializer.Serialize(current, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(filePath, migratedJson);
                    
                    _logger?.LogInformation("Successfully migrated configuration: {FilePath} to version {ToVersion}",
                        filePath, toVersion);
                    
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error migrating configuration: {FilePath}", filePath);
                return false;
            }
        }

        /// <summary>
        /// Find migration path from one version to another
        /// </summary>
        private List<IConfigurationMigration> FindMigrationPath(string configType, ConfigurationVersion from, ConfigurationVersion to)
        {
            List<IConfigurationMigration> path = new List<IConfigurationMigration>();

            if (!_migrations.ContainsKey(configType))
            {
                return path;
            }

            // Simple sequential migration path
            ConfigurationVersion current = from;
            while (current.IsNewerThan(to) == false && !current.ToString().Equals(to.ToString()))
            {
                IConfigurationMigration nextMigration = null;
                
                foreach (var migration in _migrations[configType])
                {
                    if (migration.FromVersion.ToString().Equals(current.ToString()))
                    {
                        nextMigration = migration;
                        break;
                    }
                }

                if (nextMigration == null)
                {
                    // No migration found, stop
                    break;
                }

                path.Add(nextMigration);
                current = nextMigration.ToVersion;
            }

            return path;
        }
    }
}
