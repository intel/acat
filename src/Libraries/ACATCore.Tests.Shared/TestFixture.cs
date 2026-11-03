////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// TestFixture.cs
//
// Utilities for managing test fixtures and shared test resources
//
////////////////////////////////////////////////////////////////////////////

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace ACATCore.Tests.Shared
{
    /// <summary>
    /// Base class for test fixtures that are shared across multiple tests
    /// Uses AssemblyInitialize and AssemblyCleanup for setup/teardown
    /// </summary>
    public abstract class TestFixture
    {
        /// <summary>
        /// Initialize fixture before any tests run
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Cleanup fixture after all tests complete
        /// </summary>
        public abstract void Cleanup();
    }

    /// <summary>
    /// Manages test data files and resources
    /// </summary>
    public class TestDataFixture : TestFixture
    {
        private readonly Dictionary<string, string> _testFiles = new Dictionary<string, string>();
        private string _fixtureDirectory;

        public string FixtureDirectory => _fixtureDirectory;

        public override void Initialize()
        {
            _fixtureDirectory = Path.Combine(
                Path.GetTempPath(),
                "ACATTestFixtures",
                Guid.NewGuid().ToString());
            Directory.CreateDirectory(_fixtureDirectory);
        }

        public override void Cleanup()
        {
            if (Directory.Exists(_fixtureDirectory))
            {
                try
                {
                    Directory.Delete(_fixtureDirectory, true);
                }
                catch
                {
                    // Best effort cleanup
                }
            }
        }

        /// <summary>
        /// Creates a test file and tracks it for cleanup
        /// </summary>
        public string CreateTestFile(string fileName, string content)
        {
            string filePath = Path.Combine(_fixtureDirectory, fileName);
            File.WriteAllText(filePath, content);
            _testFiles[fileName] = filePath;
            return filePath;
        }

        /// <summary>
        /// Gets the path to a test file
        /// </summary>
        public string GetTestFilePath(string fileName)
        {
            return _testFiles.TryGetValue(fileName, out string path) ? path : null;
        }

        /// <summary>
        /// Checks if a test file exists
        /// </summary>
        public bool TestFileExists(string fileName)
        {
            string path = GetTestFilePath(fileName);
            return path != null && File.Exists(path);
        }
    }

    /// <summary>
    /// Provides utilities for managing test workspace isolation
    /// </summary>
    public class TestWorkspace : IDisposable
    {
        private readonly string _workspaceRoot;
        private readonly List<string> _createdDirectories = new List<string>();
        private readonly List<string> _createdFiles = new List<string>();

        public string WorkspaceRoot => _workspaceRoot;

        public TestWorkspace(string testName = "TestWorkspace")
        {
            _workspaceRoot = Path.Combine(
                Path.GetTempPath(),
                "ACATWorkspaces",
                testName,
                Guid.NewGuid().ToString());
            Directory.CreateDirectory(_workspaceRoot);
            _createdDirectories.Add(_workspaceRoot);
        }

        /// <summary>
        /// Creates a directory in the workspace
        /// </summary>
        public string CreateDirectory(string relativePath)
        {
            string fullPath = Path.Combine(_workspaceRoot, relativePath);
            Directory.CreateDirectory(fullPath);
            _createdDirectories.Add(fullPath);
            return fullPath;
        }

        /// <summary>
        /// Creates a file in the workspace
        /// </summary>
        public string CreateFile(string relativePath, string content = "")
        {
            string fullPath = Path.Combine(_workspaceRoot, relativePath);
            string directory = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            File.WriteAllText(fullPath, content);
            _createdFiles.Add(fullPath);
            return fullPath;
        }

        /// <summary>
        /// Gets the full path for a relative path in the workspace
        /// </summary>
        public string GetPath(string relativePath)
        {
            return Path.Combine(_workspaceRoot, relativePath);
        }

        /// <summary>
        /// Checks if a file exists in the workspace
        /// </summary>
        public bool FileExists(string relativePath)
        {
            return File.Exists(GetPath(relativePath));
        }

        /// <summary>
        /// Checks if a directory exists in the workspace
        /// </summary>
        public bool DirectoryExists(string relativePath)
        {
            return Directory.Exists(GetPath(relativePath));
        }

        /// <summary>
        /// Reads content from a file in the workspace
        /// </summary>
        public string ReadFile(string relativePath)
        {
            return File.ReadAllText(GetPath(relativePath));
        }

        /// <summary>
        /// Lists all files in the workspace matching a pattern
        /// </summary>
        public string[] GetFiles(string searchPattern = "*.*", SearchOption searchOption = SearchOption.AllDirectories)
        {
            return Directory.GetFiles(_workspaceRoot, searchPattern, searchOption);
        }

        /// <summary>
        /// Cleans up the workspace
        /// </summary>
        public void Dispose()
        {
            if (Directory.Exists(_workspaceRoot))
            {
                try
                {
                    Directory.Delete(_workspaceRoot, true);
                }
                catch (UnauthorizedAccessException)
                {
                    // Retry after a brief delay
                    System.Threading.Thread.Sleep(100);
                    try
                    {
                        Directory.Delete(_workspaceRoot, true);
                    }
                    catch
                    {
                        // Best effort cleanup
                    }
                }
                catch
                {
                    // Best effort cleanup
                }
            }
        }
    }
}
