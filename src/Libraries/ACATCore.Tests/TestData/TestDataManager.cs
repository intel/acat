////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// TestDataManager.cs
//
// Manages test state: provides access to embedded sample configuration
// files and helpers for creating transient per-test directories.
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

namespace ACATCore.Tests.TestData
{
    /// <summary>
    /// Central helper for managing test data and per-test directory state.
    /// <para>
    /// Provides:
    /// <list type="bullet">
    /// <item>Access to embedded sample configuration JSON files bundled with the test assembly.</item>
    /// <item>Creation of isolated temporary directories that are cleaned up on demand.</item>
    /// <item>A simple in-memory key/value state store scoped to a single test run.</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class TestDataManager : IDisposable
    {
        private readonly object _lock = new object();
        private readonly List<string> _tempDirectories = new List<string>();
        private readonly Dictionary<string, object> _state = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        private bool _disposed;

        /// <summary>
        /// Base path to the <c>TestData</c> directory deployed alongside the
        /// test assembly.
        /// </summary>
        public static string TestDataDirectory { get; } = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty,
            "TestData");

        // ----------------------------------------------------------------
        // Temporary directory management
        // ----------------------------------------------------------------

        /// <summary>
        /// Creates a new isolated temporary directory and registers it for
        /// cleanup when <see cref="Dispose"/> is called.
        /// </summary>
        public string CreateTempDirectory()
        {
            string path = Path.Combine(
                Path.GetTempPath(),
                "ACATTestData_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(path);
            lock (_lock)
            {
                _tempDirectories.Add(path);
            }
            return path;
        }

        /// <summary>
        /// Removes all temporary directories created by this instance.
        /// </summary>
        public void CleanupTempDirectories()
        {
            lock (_lock)
            {
                foreach (string dir in _tempDirectories)
                {
                    DeleteSafely(dir);
                }
                _tempDirectories.Clear();
            }
        }

        // ----------------------------------------------------------------
        // Sample configuration file helpers
        // ----------------------------------------------------------------

        /// <summary>
        /// Returns the full path to a sample configuration file located in
        /// the <c>TestData</c> directory, or throws if the file does not exist.
        /// </summary>
        public static string GetSampleFilePath(string fileName)
        {
            string path = Path.Combine(TestDataDirectory, fileName);
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(
                    $"Sample test data file not found: {path}", path);
            }
            return path;
        }

        /// <summary>
        /// Reads and returns the text content of a sample configuration file.
        /// </summary>
        public static string ReadSampleFile(string fileName)
        {
            return File.ReadAllText(GetSampleFilePath(fileName));
        }

        /// <summary>
        /// Copies a sample configuration file into a temporary directory and
        /// returns the new path.  The destination directory must already exist.
        /// </summary>
        public static string CopySampleFileTo(string fileName, string destinationDirectory)
        {
            string source = GetSampleFilePath(fileName);
            string dest = Path.Combine(destinationDirectory, fileName);
            File.Copy(source, dest, overwrite: true);
            return dest;
        }

        // ----------------------------------------------------------------
        // In-memory state store
        // ----------------------------------------------------------------

        /// <summary>
        /// Stores a value under the given key for the lifetime of this instance.
        /// </summary>
        public void SetState(string key, object value)
        {
            lock (_lock)
            {
                _state[key] = value;
            }
        }

        /// <summary>
        /// Retrieves a value previously stored by <see cref="SetState"/>.
        /// Returns <c>null</c> when the key is not present.
        /// </summary>
        public object GetState(string key)
        {
            lock (_lock)
            {
                _state.TryGetValue(key, out object value);
                return value;
            }
        }

        /// <summary>
        /// Removes all keys from the in-memory state store.
        /// </summary>
        public void ClearState()
        {
            lock (_lock)
            {
                _state.Clear();
            }
        }

        // ----------------------------------------------------------------
        // IDisposable
        // ----------------------------------------------------------------

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            CleanupTempDirectories();
        }

        // ----------------------------------------------------------------
        // Private helpers
        // ----------------------------------------------------------------

        private static void DeleteSafely(string path)
        {
            for (int attempt = 0; attempt < 3; attempt++)
            {
                try
                {
                    if (Directory.Exists(path))
                    {
                        Directory.Delete(path, true);
                    }
                    return;
                }
                catch
                {
                    Thread.Sleep(150);
                }
            }
        }
    }
}
