////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Integration.Tests.Harness;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace ACAT.Integration.Tests.Tests
{
    /// <summary>
    /// Integration tests that verify scanner-related lifecycle behaviour.
    /// These tests focus on configuration loading and directory structures
    /// that scanners depend on, ensuring they can be created in isolation.
    /// </summary>
    [TestClass]
    public class ScannerLifecycleTests
    {
        private UITestHarness _harness;

        [TestInitialize]
        public void Setup()
        {
            _harness = new UITestHarness();
            _harness.Initialize(nameof(ScannerLifecycleTests));
        }

        [TestCleanup]
        public void Cleanup()
        {
            _harness?.Dispose();
            _harness = null;
        }

        [TestMethod]
        public void ScannerLifecycle_WorkspaceIsCreated()
        {
            Assert.IsTrue(
                Directory.Exists(_harness.WorkspaceDirectory),
                "Harness workspace directory should exist after initialization.");
        }

        [TestMethod]
        public void ScannerLifecycle_ServiceProviderIsAvailable()
        {
            Assert.IsNotNull(
                _harness.ServiceProvider,
                "Service provider should be available after harness initialization.");
        }

        [TestMethod]
        public void ScannerLifecycle_PanelConfigDirectoryCanBeCreated()
        {
            // Arrange
            string panelConfigDir = _harness.CreateWorkspaceSubDirectory("PanelConfig");

            // Assert
            Assert.IsTrue(
                Directory.Exists(panelConfigDir),
                "PanelConfig sub-directory should be creatable inside the workspace.");
        }

        [TestMethod]
        public void ScannerLifecycle_DefaultConfigFileCanBeWritten()
        {
            // Arrange
            const string configContent = @"{
  ""panelClass"": ""AlphabetScanner"",
  ""enabled"": true
}";

            // Act
            string configPath = _harness.WriteWorkspaceFile(
                Path.Combine("PanelConfig", "AlphabetScanner.json"),
                configContent);

            // Assert
            Assert.IsTrue(File.Exists(configPath),
                "Scanner config file should be written to the workspace.");
            Assert.IsTrue(new FileInfo(configPath).Length > 0,
                "Written config file should not be empty.");
        }

        [TestMethod]
        public void ScannerLifecycle_WorkspaceIsIsolatedBetweenTests()
        {
            // Each test gets a unique workspace path derived from a fresh GUID,
            // so two different harness instances will never share the same directory.
            using (var otherHarness = new UITestHarness())
            {
                otherHarness.Initialize(nameof(ScannerLifecycle_WorkspaceIsIsolatedBetweenTests));

                Assert.AreNotEqual(
                    _harness.WorkspaceDirectory,
                    otherHarness.WorkspaceDirectory,
                    "Each harness instance should have a unique workspace directory.");
            }
        }

        [TestMethod]
        public void ScannerLifecycle_WorkspaceIsRemovedAfterDispose()
        {
            // Arrange
            string workspace;
            using (var tempHarness = new UITestHarness())
            {
                tempHarness.Initialize("TempScanner");
                workspace = tempHarness.WorkspaceDirectory;
                Assert.IsTrue(Directory.Exists(workspace));
            } // Dispose is called here

            // Assert
            Assert.IsFalse(
                Directory.Exists(workspace),
                "Workspace directory should be deleted after harness disposal.");
        }
    }
}
