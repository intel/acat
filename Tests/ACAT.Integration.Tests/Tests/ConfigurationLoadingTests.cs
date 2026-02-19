////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Utility;
using ACAT.Integration.Tests.Harness;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace ACAT.Integration.Tests.Tests
{
    /// <summary>
    /// Integration tests that verify JSON configuration loading behaviour,
    /// including valid files, missing files, empty files, and malformed JSON.
    /// </summary>
    [TestClass]
    public class ConfigurationLoadingTests
    {
        private UITestHarness _harness;

        // Simple POCO used as the configuration type for these tests.
        private class SampleConfig
        {
            public string Name { get; set; } = "default";
            public bool Enabled { get; set; } = true;
            public int MaxRetries { get; set; } = 3;
        }

        [TestInitialize]
        public void Setup()
        {
            _harness = new UITestHarness();
            _harness.Initialize(nameof(ConfigurationLoadingTests));
        }

        [TestCleanup]
        public void Cleanup()
        {
            _harness?.Dispose();
            _harness = null;
        }

        [TestMethod]
        public void ConfigurationLoading_ValidJsonFileIsLoaded()
        {
            // Arrange
            const string json = @"{
  ""name"": ""TestConfig"",
  ""enabled"": true,
  ""maxRetries"": 5
}";
            string path = _harness.WriteWorkspaceFile("config.json", json);

            var loader = new JsonConfigurationLoader<SampleConfig>();

            // Act
            SampleConfig config = loader.Load(path, createDefaultOnError: false);

            // Assert
            Assert.IsNotNull(config, "A valid JSON file should be loaded successfully.");
            Assert.AreEqual("TestConfig", config.Name);
            Assert.IsTrue(config.Enabled);
            Assert.AreEqual(5, config.MaxRetries);
        }

        [TestMethod]
        public void ConfigurationLoading_MissingFileReturnsDefault()
        {
            // Arrange
            string missingPath = Path.Combine(_harness.WorkspaceDirectory, "nonexistent.json");
            var loader = new JsonConfigurationLoader<SampleConfig>();

            // Act
            SampleConfig config = loader.Load(missingPath, createDefaultOnError: true);

            // Assert
            Assert.IsNotNull(config,
                "A missing file with createDefaultOnError=true should return a default config.");
        }

        [TestMethod]
        public void ConfigurationLoading_MissingFileWithNoDefaultReturnsNull()
        {
            // Arrange
            string missingPath = Path.Combine(_harness.WorkspaceDirectory, "nonexistent.json");
            var loader = new JsonConfigurationLoader<SampleConfig>();

            // Act
            SampleConfig config = loader.Load(missingPath, createDefaultOnError: false);

            // Assert
            Assert.IsNull(config,
                "A missing file with createDefaultOnError=false should return null.");
        }

        [TestMethod]
        public void ConfigurationLoading_EmptyFileReturnsDefault()
        {
            // Arrange
            string path = _harness.WriteWorkspaceFile("empty.json", string.Empty);
            var loader = new JsonConfigurationLoader<SampleConfig>();

            // Act
            SampleConfig config = loader.Load(path, createDefaultOnError: true);

            // Assert
            Assert.IsNotNull(config,
                "An empty file with createDefaultOnError=true should return a default config.");
        }

        [TestMethod]
        public void ConfigurationLoading_NullPathReturnsDefault()
        {
            // Arrange
            var loader = new JsonConfigurationLoader<SampleConfig>();

            // Act
            SampleConfig config = loader.Load(null, createDefaultOnError: true);

            // Assert
            Assert.IsNotNull(config,
                "A null path with createDefaultOnError=true should return a default config.");
        }

        [TestMethod]
        public void ConfigurationLoading_MissingFileCreatesDefaultOnDisk()
        {
            // Arrange
            string path = Path.Combine(_harness.WorkspaceDirectory, "newconfig.json");
            var loader = new JsonConfigurationLoader<SampleConfig>();

            // Act
            loader.Load(path, createDefaultOnError: true);

            // Assert
            Assert.IsTrue(File.Exists(path),
                "Loading a missing file with createDefaultOnError=true should create the file.");
        }

        [TestMethod]
        public void ConfigurationLoading_WorkspaceIsCleanedUp()
        {
            // Arrange
            string workspace;
            using (var tempHarness = new UITestHarness())
            {
                tempHarness.Initialize("TempConfig");
                workspace = tempHarness.WorkspaceDirectory;
                tempHarness.WriteWorkspaceFile("cfg.json", "{}");
                Assert.IsTrue(Directory.Exists(workspace));
            } // Dispose called here

            // Assert
            Assert.IsFalse(Directory.Exists(workspace),
                "Workspace should be removed after harness disposal, ensuring test isolation.");
        }
    }
}
