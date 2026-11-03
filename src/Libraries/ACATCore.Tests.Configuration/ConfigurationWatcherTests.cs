////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ConfigurationWatcherTests.cs
//
// Tests for ConfigurationWatcher: directory watching, validation,
// rollback on failure, cancellation, and debouncing.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading;

namespace ACATCore.Tests.Configuration
{
    [TestClass]
    public class ConfigurationWatcherTests
    {
        private string _testDirectory;

        [TestInitialize]
        public void Setup()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), $"acat_watcher_{Guid.NewGuid()}");
            Directory.CreateDirectory(_testDirectory);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(_testDirectory))
            {
                try { Directory.Delete(_testDirectory, true); } catch { }
            }
        }

        #region Constructor Tests

        [TestMethod]
        public void Constructor_ValidDirectory_CreatesInstance()
        {
            var watcher = new ConfigurationWatcher(_testDirectory);
            Assert.IsNotNull(watcher);
            Assert.AreEqual(_testDirectory, watcher.WatchDirectory);
            Assert.IsFalse(watcher.IsWatching);
            watcher.Dispose();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_NullDirectory_ThrowsArgumentNullException()
        {
            var _ = new ConfigurationWatcher(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_EmptyDirectory_ThrowsArgumentNullException()
        {
            var _ = new ConfigurationWatcher(string.Empty);
        }

        #endregion

        #region Start / Stop Tests

        [TestMethod]
        public void Start_ExistingDirectory_ReturnsTrue()
        {
            var watcher = new ConfigurationWatcher(_testDirectory);
            bool result = watcher.Start();
            Assert.IsTrue(result, "Start should return true for an existing directory");
            Assert.IsTrue(watcher.IsWatching);
            watcher.Dispose();
        }

        [TestMethod]
        public void Start_NonExistentDirectory_ReturnsFalse()
        {
            string missing = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var watcher = new ConfigurationWatcher(missing);
            bool result = watcher.Start();
            Assert.IsFalse(result, "Start should return false for a non-existent directory");
            Assert.IsFalse(watcher.IsWatching);
            watcher.Dispose();
        }

        [TestMethod]
        public void Start_AlreadyWatching_ReturnsTrue()
        {
            var watcher = new ConfigurationWatcher(_testDirectory);
            watcher.Start();
            bool secondStart = watcher.Start();
            Assert.IsTrue(secondStart, "Second Start call should return true");
            Assert.IsTrue(watcher.IsWatching);
            watcher.Dispose();
        }

        [TestMethod]
        public void Stop_WhileWatching_StopsWatcher()
        {
            var watcher = new ConfigurationWatcher(_testDirectory);
            watcher.Start();
            Assert.IsTrue(watcher.IsWatching);
            watcher.Stop();
            Assert.IsFalse(watcher.IsWatching);
            watcher.Dispose();
        }

        [TestMethod]
        public void Dispose_StopsWatching()
        {
            var watcher = new ConfigurationWatcher(_testDirectory);
            watcher.Start();
            Assert.IsTrue(watcher.IsWatching);
            watcher.Dispose();
            Assert.IsFalse(watcher.IsWatching);
        }

        #endregion

        #region Event Notification Tests

        [TestMethod]
        public void FileChanged_RaisesConfigurationChangedEvent()
        {
            string configFile = Path.Combine(_testDirectory, "settings.json");
            File.WriteAllText(configFile, "{}");

            bool eventRaised = false;
            string eventFilePath = null;

            var watcher = new ConfigurationWatcher(_testDirectory);
            watcher.ConfigurationChanged += (sender, e) =>
            {
                eventRaised = true;
                eventFilePath = e.FilePath;
            };

            watcher.Start();
            Thread.Sleep(100); // Allow watcher to initialise

            File.WriteAllText(configFile, @"{""name"":""updated""}");
            Thread.Sleep(1000); // Wait for debounce + event

            Assert.IsTrue(eventRaised, "ConfigurationChanged event should be raised");
            Assert.IsNotNull(eventFilePath, "Event file path should be set");
            watcher.Dispose();
        }

        [TestMethod]
        public void FileChanged_AfterStop_DoesNotRaiseEvent()
        {
            string configFile = Path.Combine(_testDirectory, "settings.json");
            File.WriteAllText(configFile, "{}");

            int eventCount = 0;

            var watcher = new ConfigurationWatcher(_testDirectory);
            watcher.ConfigurationChanged += (sender, e) => eventCount++;

            watcher.Start();
            Thread.Sleep(100);

            watcher.Stop();
            File.WriteAllText(configFile, @"{""stopped"":true}");
            Thread.Sleep(1000);

            Assert.AreEqual(0, eventCount, "No events should fire after Stop()");
            watcher.Dispose();
        }

        #endregion

        #region Cancellation Tests

        [TestMethod]
        public void FileChanged_CancelledByChangingEvent_RaisesFailedEvent()
        {
            string configFile = Path.Combine(_testDirectory, "settings.json");
            File.WriteAllText(configFile, "{}");

            bool failedEventRaised = false;
            bool changedEventRaised = false;

            var watcher = new ConfigurationWatcher(_testDirectory);

            // Cancel every reload
            watcher.ConfigurationChanging += (sender, e) => e.Cancel = true;
            watcher.ConfigurationChanged += (sender, e) => changedEventRaised = true;
            watcher.ConfigurationChangeFailed += (sender, e) => failedEventRaised = true;

            watcher.Start();
            Thread.Sleep(100);

            File.WriteAllText(configFile, @"{""cancelled"":true}");
            Thread.Sleep(1000);

            Assert.IsTrue(failedEventRaised, "ConfigurationChangeFailed should be raised when reload is cancelled");
            Assert.IsFalse(changedEventRaised, "ConfigurationChanged should NOT be raised when reload is cancelled");
            watcher.Dispose();
        }

        #endregion

        #region Validation / Rollback Tests

        [TestMethod]
        public void FileChanged_ValidationPasses_RaisesChangedEvent()
        {
            string configFile = Path.Combine(_testDirectory, "settings.json");
            File.WriteAllText(configFile, "{}");

            bool changedEventRaised = false;
            bool failedEventRaised = false;

            var watcher = new ConfigurationWatcher(_testDirectory);
            watcher.ValidationCallback = _ => true; // Always valid
            watcher.ConfigurationChanged += (sender, e) => changedEventRaised = true;
            watcher.ConfigurationChangeFailed += (sender, e) => failedEventRaised = true;

            watcher.Start();
            Thread.Sleep(100);

            File.WriteAllText(configFile, @"{""valid"":true}");
            Thread.Sleep(1000);

            Assert.IsTrue(changedEventRaised, "ConfigurationChanged should be raised when validation passes");
            Assert.IsFalse(failedEventRaised, "ConfigurationChangeFailed should NOT be raised when validation passes");
            watcher.Dispose();
        }

        [TestMethod]
        public void FileChanged_ValidationFails_RaisesFailedEvent()
        {
            string configFile = Path.Combine(_testDirectory, "settings.json");
            File.WriteAllText(configFile, "{}");

            bool changedEventRaised = false;
            bool failedEventRaised = false;

            var watcher = new ConfigurationWatcher(_testDirectory);
            watcher.ValidationCallback = _ => false; // Always fails
            watcher.ConfigurationChanged += (sender, e) => changedEventRaised = true;
            watcher.ConfigurationChangeFailed += (sender, e) => failedEventRaised = true;

            watcher.Start();
            Thread.Sleep(100);

            File.WriteAllText(configFile, @"{""invalid"":true}");
            Thread.Sleep(1000);

            Assert.IsTrue(failedEventRaised, "ConfigurationChangeFailed should be raised when validation fails");
            Assert.IsFalse(changedEventRaised, "ConfigurationChanged should NOT be raised when validation fails");
            watcher.Dispose();
        }

        [TestMethod]
        public void FileChanged_ValidationCallbackThrows_RaisesFailedEvent()
        {
            string configFile = Path.Combine(_testDirectory, "settings.json");
            File.WriteAllText(configFile, "{}");

            bool failedEventRaised = false;

            var watcher = new ConfigurationWatcher(_testDirectory);
            watcher.ValidationCallback = _ => throw new InvalidOperationException("Validation error");
            watcher.ConfigurationChangeFailed += (sender, e) => failedEventRaised = true;

            watcher.Start();
            Thread.Sleep(100);

            File.WriteAllText(configFile, @"{""throws"":true}");
            Thread.Sleep(1000);

            Assert.IsTrue(failedEventRaised, "ConfigurationChangeFailed should be raised when validation throws");
            watcher.Dispose();
        }

        #endregion

        #region Debouncing Tests

        [TestMethod]
        public void RapidChanges_AreDebounced_SingleEventRaised()
        {
            string configFile = Path.Combine(_testDirectory, "settings.json");
            File.WriteAllText(configFile, "{}");

            int eventCount = 0;

            var watcher = new ConfigurationWatcher(_testDirectory);
            watcher.ConfigurationChanged += (sender, e) => Interlocked.Increment(ref eventCount);

            watcher.Start();
            Thread.Sleep(100);

            // Write file 5 times rapidly within the debounce window
            for (int i = 0; i < 5; i++)
            {
                File.WriteAllText(configFile, $@"{{""iteration"":{i}}}");
                Thread.Sleep(50);
            }

            Thread.Sleep(1200); // Wait well past the debounce window

            Assert.AreEqual(1, eventCount,
                "Rapid writes within the debounce window should produce exactly one event");
            watcher.Dispose();
        }

        #endregion

        #region ChangedEventArgs Tests

        [TestMethod]
        public void ConfigurationChanged_EventArgs_ContainsExpectedData()
        {
            string configFile = Path.Combine(_testDirectory, "settings.json");
            File.WriteAllText(configFile, "{}");

            ConfigurationWatcherChangedEventArgs capturedArgs = null;

            var watcher = new ConfigurationWatcher(_testDirectory);
            watcher.ConfigurationChanged += (sender, e) => capturedArgs = e;

            watcher.Start();
            Thread.Sleep(100);

            File.WriteAllText(configFile, @"{""name"":""test""}");
            Thread.Sleep(1000);

            Assert.IsNotNull(capturedArgs, "Event args should not be null");
            Assert.IsTrue(capturedArgs.Success, "Success should be true");
            Assert.AreEqual(_testDirectory, capturedArgs.DirectoryPath, "DirectoryPath should match");
            Assert.AreEqual(configFile, capturedArgs.FilePath, "FilePath should match");
            Assert.IsNull(capturedArgs.ErrorMessage, "ErrorMessage should be null on success");
            watcher.Dispose();
        }

        [TestMethod]
        public void ConfigurationChangeFailed_EventArgs_ContainsErrorMessage()
        {
            string configFile = Path.Combine(_testDirectory, "settings.json");
            File.WriteAllText(configFile, "{}");

            ConfigurationWatcherChangedEventArgs capturedArgs = null;

            var watcher = new ConfigurationWatcher(_testDirectory);
            watcher.ValidationCallback = _ => false;
            watcher.ConfigurationChangeFailed += (sender, e) => capturedArgs = e;

            watcher.Start();
            Thread.Sleep(100);

            File.WriteAllText(configFile, @"{""bad"":true}");
            Thread.Sleep(1000);

            Assert.IsNotNull(capturedArgs, "Event args should not be null");
            Assert.IsFalse(capturedArgs.Success, "Success should be false on failure");
            Assert.IsNotNull(capturedArgs.ErrorMessage, "ErrorMessage should be set on failure");
            watcher.Dispose();
        }

        #endregion
    }
}
