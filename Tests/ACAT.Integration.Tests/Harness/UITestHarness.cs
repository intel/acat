////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.PanelManagement;
using ACAT.Core.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;

namespace ACAT.Integration.Tests.Harness
{
    /// <summary>
    /// Provides a test harness for UI component integration tests.
    /// Manages service provider setup, workspace isolation, and cleanup
    /// so each test runs in a controlled, reproducible environment.
    /// </summary>
    public sealed class UITestHarness : IDisposable
    {
        private static readonly string TempBasePath =
            Path.Combine(Path.GetTempPath(), "ACAT.Integration.Tests");

        private bool _disposed;
        private ServiceProvider _serviceProvider;

        /// <summary>
        /// Gets the isolated workspace directory for this test instance.
        /// </summary>
        public string WorkspaceDirectory { get; private set; }

        /// <summary>
        /// Gets the configured <see cref="IServiceProvider"/> for this test instance.
        /// </summary>
        public IServiceProvider ServiceProvider => _serviceProvider;

        /// <summary>
        /// Initialises the harness: creates an isolated workspace directory and
        /// configures the ACAT service provider.
        /// </summary>
        /// <param name="testName">A short name used to label the workspace folder.</param>
        public void Initialize(string testName)
        {
            WorkspaceDirectory = Path.Combine(
                TempBasePath,
                testName,
                Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(WorkspaceDirectory);

            var services = new ServiceCollection();
            services.AddACATInfrastructure();
            _serviceProvider = services.BuildServiceProvider();

            Context.ServiceProvider = _serviceProvider;
        }

        /// <summary>
        /// Returns a logger resolved from the test service provider.
        /// </summary>
        public ILogger<T> GetLogger<T>() =>
            _serviceProvider?.GetService<ILogger<T>>();

        /// <summary>
        /// Creates a sub-directory inside the workspace and returns its full path.
        /// </summary>
        public string CreateWorkspaceSubDirectory(string name)
        {
            string path = Path.Combine(WorkspaceDirectory, name);
            Directory.CreateDirectory(path);
            return path;
        }

        /// <summary>
        /// Writes a text file to the workspace and returns its full path.
        /// </summary>
        public string WriteWorkspaceFile(string relativePath, string content)
        {
            string fullPath = Path.Combine(WorkspaceDirectory, relativePath);
            string directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }
            File.WriteAllText(fullPath, content);
            return fullPath;
        }

        /// <summary>
        /// Tears down the service provider and removes the workspace directory.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            Context.ServiceProvider = null;

            _serviceProvider?.Dispose();
            _serviceProvider = null;

            CleanupWorkspace();

            _disposed = true;
        }

        private void CleanupWorkspace()
        {
            if (!Directory.Exists(WorkspaceDirectory))
            {
                return;
            }

            try
            {
                Directory.Delete(WorkspaceDirectory, recursive: true);
            }
            catch (UnauthorizedAccessException)
            {
                // Retry once after a brief pause in case files are still locked.
                Thread.Sleep(100);
                try
                {
                    Directory.Delete(WorkspaceDirectory, recursive: true);
                }
                catch
                {
                    // Best-effort cleanup; do not fail the test.
                }
            }
        }
    }
}
