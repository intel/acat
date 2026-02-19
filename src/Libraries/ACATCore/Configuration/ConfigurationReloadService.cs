////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ConfigurationReloadService.cs
//
// Service for monitoring configuration files and reloading them when changed
//
////////////////////////////////////////////////////////////////////////////

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace ACAT.Core.Configuration
{
    /// <summary>
    /// Event args for configuration reload events
    /// </summary>
    public class ConfigurationReloadEventArgs : EventArgs
    {
        public string FilePath { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime ReloadTime { get; set; }
    }

    /// <summary>
    /// Service for monitoring configuration files and automatically reloading them when changed
    /// </summary>
    public class ConfigurationReloadService : IDisposable
    {
        private readonly ILogger _logger;
        private readonly Dictionary<string, FileSystemWatcher> _watchers;
        private readonly Dictionary<string, Timer> _debounceTimers;
        private readonly object _lock = new object();
        private const int DebounceDelayMs = 500; // Wait 500ms after file change before reloading

        /// <summary>
        /// Event raised when a configuration file is successfully reloaded
        /// </summary>
        public event EventHandler<ConfigurationReloadEventArgs> ConfigurationReloaded;

        /// <summary>
        /// Event raised when a configuration file reload fails
        /// </summary>
        public event EventHandler<ConfigurationReloadEventArgs> ConfigurationReloadFailed;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">Logger instance (optional)</param>
        public ConfigurationReloadService(ILogger logger = null)
        {
            _logger = logger ?? Utility.LogManager.GetLogger<ConfigurationReloadService>();
            _watchers = new Dictionary<string, FileSystemWatcher>();
            _debounceTimers = new Dictionary<string, Timer>();
        }

        /// <summary>
        /// Start monitoring a configuration file for changes
        /// </summary>
        /// <param name="filePath">Path to configuration file to monitor</param>
        /// <returns>True if monitoring started successfully</returns>
        public bool StartMonitoring(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    _logger?.LogError("Cannot monitor null or empty file path");
                    return false;
                }

                if (!File.Exists(filePath))
                {
                    _logger?.LogWarning("Configuration file does not exist (yet): {FilePath}. Will monitor directory.", filePath);
                }

                string directory = Path.GetDirectoryName(filePath);
                string fileName = Path.GetFileName(filePath);

                if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
                {
                    _logger?.LogError("Directory does not exist: {Directory}", directory);
                    return false;
                }

                lock (_lock)
                {
                    if (_watchers.ContainsKey(filePath))
                    {
                        _logger?.LogWarning("Already monitoring file: {FilePath}", filePath);
                        return true;
                    }

                    FileSystemWatcher watcher = new FileSystemWatcher
                    {
                        Path = directory,
                        Filter = fileName,
                        NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime,
                        EnableRaisingEvents = true
                    };

                    watcher.Changed += (sender, e) => OnFileChanged(e.FullPath);
                    watcher.Created += (sender, e) => OnFileChanged(e.FullPath);
                    watcher.Renamed += (sender, e) => OnFileChanged(e.FullPath);

                    _watchers[filePath] = watcher;
                    _logger?.LogInformation("Started monitoring configuration file: {FilePath}", filePath);

                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error starting file monitoring for: {FilePath}", filePath);
                return false;
            }
        }

        /// <summary>
        /// Stop monitoring a configuration file
        /// </summary>
        /// <param name="filePath">Path to configuration file</param>
        /// <returns>True if monitoring stopped successfully</returns>
        public bool StopMonitoring(string filePath)
        {
            try
            {
                lock (_lock)
                {
                    if (!_watchers.ContainsKey(filePath))
                    {
                        _logger?.LogWarning("File is not being monitored: {FilePath}", filePath);
                        return false;
                    }

                    // Stop and dispose watcher
                    FileSystemWatcher watcher = _watchers[filePath];
                    watcher.EnableRaisingEvents = false;
                    watcher.Dispose();
                    _watchers.Remove(filePath);

                    // Stop and dispose debounce timer if exists
                    if (_debounceTimers.ContainsKey(filePath))
                    {
                        _debounceTimers[filePath]?.Dispose();
                        _debounceTimers.Remove(filePath);
                    }

                    _logger?.LogInformation("Stopped monitoring configuration file: {FilePath}", filePath);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error stopping file monitoring for: {FilePath}", filePath);
                return false;
            }
        }

        /// <summary>
        /// Stop monitoring all configuration files
        /// </summary>
        public void StopAll()
        {
            lock (_lock)
            {
                foreach (var filePath in new List<string>(_watchers.Keys))
                {
                    StopMonitoring(filePath);
                }
            }
        }

        /// <summary>
        /// Get list of files currently being monitored
        /// </summary>
        public List<string> GetMonitoredFiles()
        {
            lock (_lock)
            {
                return new List<string>(_watchers.Keys);
            }
        }

        /// <summary>
        /// Handle file change event with debouncing
        /// </summary>
        private void OnFileChanged(string filePath)
        {
            lock (_lock)
            {
                // Dispose existing timer if present
                if (_debounceTimers.ContainsKey(filePath))
                {
                    _debounceTimers[filePath]?.Dispose();
                }

                // Create new debounce timer
                _debounceTimers[filePath] = new Timer(
                    callback: (state) => ProcessFileChange((string)state),
                    state: filePath,
                    dueTime: DebounceDelayMs,
                    period: Timeout.Infinite
                );
            }
        }

        /// <summary>
        /// Process the file change after debounce delay
        /// </summary>
        private void ProcessFileChange(string filePath)
        {
            try
            {
                _logger?.LogInformation("Configuration file changed: {FilePath}", filePath);

                // Clean up debounce timer
                lock (_lock)
                {
                    if (_debounceTimers.ContainsKey(filePath))
                    {
                        _debounceTimers[filePath]?.Dispose();
                        _debounceTimers.Remove(filePath);
                    }
                }

                // Verify file exists and is accessible
                if (!File.Exists(filePath))
                {
                    string error = $"Configuration file no longer exists: {filePath}";
                    _logger?.LogWarning(error);
                    OnConfigurationReloadFailed(new ConfigurationReloadEventArgs
                    {
                        FilePath = filePath,
                        Success = false,
                        ErrorMessage = error,
                        ReloadTime = DateTime.Now
                    });
                    return;
                }

                // Wait a moment to ensure file is not locked
                Thread.Sleep(100);

                // Raise successful reload event
                OnConfigurationReloaded(new ConfigurationReloadEventArgs
                {
                    FilePath = filePath,
                    Success = true,
                    ErrorMessage = null,
                    ReloadTime = DateTime.Now
                });

                _logger?.LogInformation("Configuration reload notification sent for: {FilePath}", filePath);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error processing configuration file change: {FilePath}", filePath);
                OnConfigurationReloadFailed(new ConfigurationReloadEventArgs
                {
                    FilePath = filePath,
                    Success = false,
                    ErrorMessage = ex.Message,
                    ReloadTime = DateTime.Now
                });
            }
        }

        /// <summary>
        /// Raise the ConfigurationReloaded event
        /// </summary>
        protected virtual void OnConfigurationReloaded(ConfigurationReloadEventArgs e)
        {
            ConfigurationReloaded?.Invoke(this, e);
        }

        /// <summary>
        /// Raise the ConfigurationReloadFailed event
        /// </summary>
        protected virtual void OnConfigurationReloadFailed(ConfigurationReloadEventArgs e)
        {
            ConfigurationReloadFailed?.Invoke(this, e);
        }

        /// <summary>
        /// Dispose of all resources
        /// </summary>
        public void Dispose()
        {
            StopAll();
        }
    }
}
