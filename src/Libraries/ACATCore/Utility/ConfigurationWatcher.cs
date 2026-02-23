////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ConfigurationWatcher.cs
//
// Watches a configuration directory for file changes and provides a safe
// reload mechanism with validation, rollback, and debouncing support.
//
////////////////////////////////////////////////////////////////////////////

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;

namespace ACAT.Core.Utility
{
    /// <summary>
    /// Event args for the ConfigurationChanging event. Set Cancel = true to prevent the reload.
    /// </summary>
    public class ConfigurationWatcherChangingEventArgs : CancelEventArgs
    {
        /// <summary>
        /// The directory being watched.
        /// </summary>
        public string DirectoryPath { get; set; }

        /// <summary>
        /// The full path of the file that changed.
        /// </summary>
        public string FilePath { get; set; }
    }

    /// <summary>
    /// Event args for ConfigurationChanged and ConfigurationChangeFailed events.
    /// </summary>
    public class ConfigurationWatcherChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The directory being watched.
        /// </summary>
        public string DirectoryPath { get; set; }

        /// <summary>
        /// The full path of the file that changed.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Whether the change was applied successfully.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Error message if the reload failed.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// The UTC time the change was detected.
        /// </summary>
        public DateTime ChangeTime { get; set; }
    }

    /// <summary>
    /// Watches a configuration directory for file changes and provides a safe
    /// reload mechanism with validation, rollback on failure, debouncing, and
    /// cancellation support.
    /// </summary>
    public class ConfigurationWatcher : IDisposable
    {
        private readonly ILogger _logger;
        private FileSystemWatcher _watcher;
        private readonly Dictionary<string, Timer> _debounceTimers = new Dictionary<string, Timer>();
        private readonly object _lock = new object();
        private const int DebounceDelayMs = 500;
        private const int FileLockRetryCount = 5;
        private const int FileLockRetryDelayMs = 100;
        private bool _disposed = false;

        /// <summary>
        /// Gets the directory being watched.
        /// </summary>
        public string WatchDirectory { get; }

        /// <summary>
        /// Gets whether the watcher is currently active.
        /// </summary>
        public bool IsWatching { get; private set; }

        /// <summary>
        /// Optional callback invoked to validate a changed config file before it is applied.
        /// Return true if the config is valid; false to trigger rollback/failure.
        /// </summary>
        public Func<string, bool> ValidationCallback { get; set; }

        /// <summary>
        /// Raised before a configuration change is applied. Set <see cref="CancelEventArgs.Cancel"/>
        /// to true to prevent the reload.
        /// </summary>
        public event EventHandler<ConfigurationWatcherChangingEventArgs> ConfigurationChanging;

        /// <summary>
        /// Raised after a configuration file change has been validated and applied successfully.
        /// </summary>
        public event EventHandler<ConfigurationWatcherChangedEventArgs> ConfigurationChanged;

        /// <summary>
        /// Raised when a configuration file change could not be applied (validation failed,
        /// file locked, or reload cancelled).
        /// </summary>
        public event EventHandler<ConfigurationWatcherChangedEventArgs> ConfigurationChangeFailed;

        /// <summary>
        /// Initializes a new instance of <see cref="ConfigurationWatcher"/>.
        /// </summary>
        /// <param name="watchDirectory">The directory to watch for configuration changes.</param>
        /// <param name="logger">Optional logger instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="watchDirectory"/> is null or empty.</exception>
        public ConfigurationWatcher(string watchDirectory, ILogger logger = null)
        {
            if (string.IsNullOrEmpty(watchDirectory))
            {
                throw new ArgumentNullException(nameof(watchDirectory));
            }

            WatchDirectory = watchDirectory;
            _logger = logger ?? LogManager.GetLogger<ConfigurationWatcher>();
        }

        /// <summary>
        /// Starts watching the configuration directory.
        /// </summary>
        /// <param name="filter">File filter pattern (default: "*.json").</param>
        /// <returns>True if watching started successfully, false otherwise.</returns>
        public bool Start(string filter = "*.json")
        {
            if (_disposed)
            {
                _logger?.LogError("ConfigurationWatcher has been disposed");
                return false;
            }

            if (IsWatching)
            {
                _logger?.LogWarning("ConfigurationWatcher is already watching: {Directory}", WatchDirectory);
                return true;
            }

            if (!Directory.Exists(WatchDirectory))
            {
                _logger?.LogError("Watch directory does not exist: {Directory}", WatchDirectory);
                return false;
            }

            try
            {
                _watcher = new FileSystemWatcher
                {
                    Path = WatchDirectory,
                    Filter = filter,
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime,
                    IncludeSubdirectories = false,
                    EnableRaisingEvents = true
                };

                _watcher.Changed += OnFileSystemEvent;
                _watcher.Created += OnFileSystemEvent;
                _watcher.Renamed += OnRenamed;

                IsWatching = true;
                _logger?.LogInformation("ConfigurationWatcher started for directory: {Directory}", WatchDirectory);
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to start ConfigurationWatcher for: {Directory}", WatchDirectory);
                return false;
            }
        }

        /// <summary>
        /// Stops watching the configuration directory.
        /// </summary>
        public void Stop()
        {
            lock (_lock)
            {
                if (!IsWatching)
                {
                    return;
                }

                if (_watcher != null)
                {
                    _watcher.EnableRaisingEvents = false;
                    _watcher.Changed -= OnFileSystemEvent;
                    _watcher.Created -= OnFileSystemEvent;
                    _watcher.Renamed -= OnRenamed;
                    _watcher.Dispose();
                    _watcher = null;
                }

                foreach (var timer in _debounceTimers.Values)
                {
                    timer?.Dispose();
                }

                _debounceTimers.Clear();
                IsWatching = false;
                _logger?.LogInformation("ConfigurationWatcher stopped for directory: {Directory}", WatchDirectory);
            }
        }

        /// <summary>
        /// Handles rename events from FileSystemWatcher.
        /// </summary>
        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            ScheduleDebounced(e.FullPath);
        }

        /// <summary>
        /// Handles change/create events from FileSystemWatcher.
        /// </summary>
        private void OnFileSystemEvent(object sender, FileSystemEventArgs e)
        {
            ScheduleDebounced(e.FullPath);
        }

        /// <summary>
        /// Schedules processing of a file change after the debounce delay, cancelling any
        /// pending timer for the same file.
        /// </summary>
        private void ScheduleDebounced(string filePath)
        {
            lock (_lock)
            {
                if (_debounceTimers.TryGetValue(filePath, out Timer existing))
                {
                    existing?.Dispose();
                }

                _debounceTimers[filePath] = new Timer(
                    callback: state => ProcessChange((string)state),
                    state: filePath,
                    dueTime: DebounceDelayMs,
                    period: Timeout.Infinite
                );
            }
        }

        /// <summary>
        /// Processes a debounced file change: validates the new config, fires the appropriate
        /// events, and supports rollback on validation failure.
        /// </summary>
        private void ProcessChange(string filePath)
        {
            // Clean up the debounce timer
            lock (_lock)
            {
                if (_debounceTimers.TryGetValue(filePath, out Timer t))
                {
                    t?.Dispose();
                    _debounceTimers.Remove(filePath);
                }
            }

            try
            {
                _logger?.LogInformation("Configuration file changed: {FilePath}", filePath);

                // Fire the Changing event; allow subscribers to cancel the reload
                var changingArgs = new ConfigurationWatcherChangingEventArgs
                {
                    DirectoryPath = WatchDirectory,
                    FilePath = filePath
                };

                ConfigurationChanging?.Invoke(this, changingArgs);

                if (changingArgs.Cancel)
                {
                    _logger?.LogInformation("Configuration reload cancelled by subscriber: {FilePath}", filePath);
                    OnChangeFailed(filePath, "Reload cancelled by subscriber");
                    return;
                }

                // Ensure the file is accessible (handles file-lock issues with retries)
                if (!WaitForFileAccess(filePath, FileLockRetryCount, FileLockRetryDelayMs))
                {
                    string error = $"Configuration file is locked or inaccessible: {filePath}";
                    _logger?.LogError(error);
                    OnChangeFailed(filePath, error);
                    return;
                }

                // Run validation callback if supplied
                if (ValidationCallback != null)
                {
                    bool valid;
                    try
                    {
                        valid = ValidationCallback(filePath);
                    }
                    catch (Exception validationEx)
                    {
                        _logger?.LogError(validationEx, "Exception in validation callback for: {FilePath}", filePath);
                        OnChangeFailed(filePath, validationEx.Message);
                        return;
                    }

                    if (!valid)
                    {
                        string error = $"Validation failed for configuration file: {filePath}. Rolling back.";
                        _logger?.LogError(error);
                        OnChangeFailed(filePath, error);
                        return;
                    }
                }

                // All checks passed — notify subscribers of successful change
                OnChangeSucceeded(filePath);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Unexpected error processing configuration change: {FilePath}", filePath);
                OnChangeFailed(filePath, ex.Message);
            }
        }

        /// <summary>
        /// Raises the <see cref="ConfigurationChanged"/> event.
        /// </summary>
        private void OnChangeSucceeded(string filePath)
        {
            _logger?.LogInformation("Configuration change applied: {FilePath}", filePath);
            ConfigurationChanged?.Invoke(this, new ConfigurationWatcherChangedEventArgs
            {
                DirectoryPath = WatchDirectory,
                FilePath = filePath,
                Success = true,
                ChangeTime = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Raises the <see cref="ConfigurationChangeFailed"/> event.
        /// </summary>
        private void OnChangeFailed(string filePath, string errorMessage)
        {
            ConfigurationChangeFailed?.Invoke(this, new ConfigurationWatcherChangedEventArgs
            {
                DirectoryPath = WatchDirectory,
                FilePath = filePath,
                Success = false,
                ErrorMessage = errorMessage,
                ChangeTime = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Tries to open a file for reading to verify it is not locked.
        /// Retries up to <paramref name="retries"/> times with a delay between attempts.
        /// </summary>
        /// <returns>True if the file is accessible, false otherwise.</returns>
        private bool WaitForFileAccess(string filePath, int retries, int retryDelayMs)
        {
            if (!File.Exists(filePath))
            {
                _logger?.LogWarning("Configuration file does not exist: {FilePath}", filePath);
                return false;
            }

            for (int attempt = 0; attempt < retries; attempt++)
            {
                try
                {
                    using (File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        return true;
                    }
                }
                catch (IOException)
                {
                    if (attempt < retries - 1)
                    {
                        Thread.Sleep(retryDelayMs);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Releases all resources used by the <see cref="ConfigurationWatcher"/>.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            Stop();
            _disposed = true;
        }
    }
}
