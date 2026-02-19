////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// TestFixtureBase.cs
//
// Base class for all ACAT test fixtures, providing common setup/teardown
// and utility helpers for managing temporary test state.
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace ACATCore.Tests.Fixtures
{
    /// <summary>
    /// Base class for ACAT test fixtures. Provides isolated temporary
    /// directory management and deterministic cleanup after each test.
    /// Uses xUnit's IDisposable pattern for test lifecycle management.
    /// </summary>
    public abstract class TestFixtureBase : IDisposable
    {
        private readonly List<string> _tempDirectories = new List<string>();
        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        /// <summary>
        /// Gets the primary temporary directory created for this test.
        /// </summary>
        protected string TestDirectory { get; private set; }

        /// <summary>
        /// Constructor performs test setup, creating the per-test temp directory
        /// and calling <see cref="OnSetUp"/>.
        /// </summary>
        protected TestFixtureBase()
        {
            TestDirectory = CreateTempDirectory();
            OnSetUp();
        }

        /// <summary>
        /// Override to perform test-specific setup after the base
        /// initialisation has completed.
        /// </summary>
        protected virtual void OnSetUp() { }

        /// <summary>
        /// Override to perform test-specific teardown before the base
        /// cleanup runs.
        /// </summary>
        protected virtual void OnTearDown() { }

        /// <summary>
        /// Creates a new isolated temporary directory and registers it for
        /// automatic cleanup at the end of the test.
        /// </summary>
        protected string CreateTempDirectory()
        {
            string path = Path.Combine(
                Path.GetTempPath(),
                "ACATTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(path);
            _tempDirectories.Add(path);
            return path;
        }

        /// <summary>
        /// Writes <paramref name="content"/> to <paramref name="fileName"/>
        /// inside <see cref="TestDirectory"/> and returns the full path.
        /// </summary>
        protected string WriteTestFile(string fileName, string content)
        {
            string path = Path.Combine(TestDirectory, fileName);
            File.WriteAllText(path, content);
            return path;
        }

        /// <summary>
        /// Registers a disposable resource so it is disposed during cleanup.
        /// </summary>
        protected T RegisterDisposable<T>(T resource) where T : IDisposable
        {
            _disposables.Add(resource);
            return resource;
        }

        /// <summary>
        /// IDisposable implementation. Calls <see cref="OnTearDown"/> then releases
        /// all disposables and temp directories registered during the test.
        /// </summary>
        public void Dispose()
        {
            OnTearDown();
            DisposeAll();
            DeleteTempDirectories();
        }

        // ----------------------------------------------------------------
        // Private helpers
        // ----------------------------------------------------------------

        private void DisposeAll()
        {
            foreach (IDisposable d in _disposables)
            {
                try { d.Dispose(); }
                catch { /* best-effort */ }
            }
            _disposables.Clear();
        }

        private void DeleteTempDirectories()
        {
            foreach (string dir in _tempDirectories)
            {
                DeleteDirectorySafely(dir);
            }
            _tempDirectories.Clear();
        }

        private static void DeleteDirectorySafely(string path)
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
