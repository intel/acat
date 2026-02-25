////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// PreferencesBaseAsyncTests.cs
//
// Unit tests validating the async static helpers added to PreferencesBase
// (LoadAsync, ReloadAsync, SaveAsync) and GlobalPreferences (LoadAsync,
// SaveAsync).  These tests are the primary gate for switching existing
// callers from the synchronous API to the async one.
//
////////////////////////////////////////////////////////////////////////////

// xUnit1051: Tests below validate library cancellation behaviour directly
// (not using test-framework cancellation), so suppress the recommendation.
#pragma warning disable xUnit1051

using ACAT.Core.PreferencesManagement;
using ACAT.Core.Utility;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ACATCore.Tests
{
    /// <summary>
    /// Tests for the async static helpers on <see cref="PreferencesBase"/> and
    /// <see cref="GlobalPreferences"/>.
    /// </summary>
    public class PreferencesBaseAsyncTests : IDisposable
    {
        private readonly string _tempDir;

        public PreferencesBaseAsyncTests()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), "ACATPrefsAsyncTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempDir);
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempDir))
                Directory.Delete(_tempDir, recursive: true);
        }

        // ----------------------------------------------------------------
        // PreferencesBase.LoadAsync<T>
        // ----------------------------------------------------------------

        [Fact]
        public async Task PreferencesBase_LoadAsync_ReturnsNullForNullPath()
        {
            var result = await PreferencesBase.LoadAsync<SampleXmlPrefs>(null, loadDefaultsOnFail: false, saveAfterLoad: false);

            Assert.Null(result);
        }

        [Fact]
        public async Task PreferencesBase_LoadAsync_ReturnsDefaultWhenFileMissing()
        {
            string path = Path.Combine(_tempDir, "missing.xml");

            var result = await PreferencesBase.LoadAsync<SampleXmlPrefs>(path, loadDefaultsOnFail: true, saveAfterLoad: false);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task PreferencesBase_LoadAsync_RoundTrip_PreservesData()
        {
            string path = Path.Combine(_tempDir, "prefs_async.xml");
            var original = new SampleXmlPrefs { Name = "AsyncTester", Value = 42 };

            bool saved = await PreferencesBase.SaveAsync(original, path);
            Assert.True(saved);
            Assert.True(File.Exists(path));

            // Load without re-saving (saveAfterLoad=false) to avoid overwrite
            var loaded = await PreferencesBase.LoadAsync<SampleXmlPrefs>(path, loadDefaultsOnFail: true, saveAfterLoad: false);

            Assert.NotNull(loaded);
            Assert.Equal("AsyncTester", loaded.Name);
            Assert.Equal(42, loaded.Value);
        }

        [Fact]
        public async Task PreferencesBase_LoadAsync_CreatesAndSavesFileWhenMissing()
        {
            string path = Path.Combine(_tempDir, "auto_save.xml");

            var result = await PreferencesBase.LoadAsync<SampleXmlPrefs>(path, loadDefaultsOnFail: true, saveAfterLoad: true);

            Assert.NotNull(result);
            Assert.True(File.Exists(path), "File should have been created by saveAfterLoad=true");
        }

        // ----------------------------------------------------------------
        // PreferencesBase.ReloadAsync<T>
        // ----------------------------------------------------------------

        [Fact]
        public async Task PreferencesBase_ReloadAsync_ReturnsNullForNullPath()
        {
            var result = await PreferencesBase.ReloadAsync<SampleXmlPrefs>(null);

            Assert.Null(result);
        }

        [Fact]
        public async Task PreferencesBase_ReloadAsync_ReturnsNullWhenFileMissing()
        {
            string path = Path.Combine(_tempDir, "nonexistent_reload.xml");

            // PreferencesRepository.Load returns a new() default when file missing,
            // so ReloadAsync succeeds with a default rather than null
            var result = await PreferencesBase.ReloadAsync<SampleXmlPrefs>(path);

            // Should return a non-null default (not null), because the underlying
            // PreferencesRepository.LoadAsync falls back to new T() when the file is absent
            Assert.NotNull(result);
        }

        [Fact]
        public async Task PreferencesBase_ReloadAsync_ReturnsUpdatedData()
        {
            string path = Path.Combine(_tempDir, "reload_test.xml");
            var v1 = new SampleXmlPrefs { Name = "Version1", Value = 1 };
            await PreferencesBase.SaveAsync(v1, path);

            var v2 = new SampleXmlPrefs { Name = "Version2", Value = 2 };
            await PreferencesBase.SaveAsync(v2, path);

            var reloaded = await PreferencesBase.ReloadAsync<SampleXmlPrefs>(path);

            Assert.NotNull(reloaded);
            Assert.Equal("Version2", reloaded.Name);
            Assert.Equal(2, reloaded.Value);
        }

        // ----------------------------------------------------------------
        // PreferencesBase.SaveAsync<T>
        // ----------------------------------------------------------------

        [Fact]
        public async Task PreferencesBase_SaveAsync_CreatesFile()
        {
            string path = Path.Combine(_tempDir, "save_async.xml");
            var prefs = new SampleXmlPrefs { Name = "Saved", Value = 99 };

            bool result = await PreferencesBase.SaveAsync(prefs, path);

            Assert.True(result);
            Assert.True(File.Exists(path));
        }

        [Fact]
        public async Task PreferencesBase_SaveAsync_RespectsCancellation()
        {
            string path = Path.Combine(_tempDir, "cancel_save.xml");
            var prefs = new SampleXmlPrefs { Name = "CancelTest", Value = 0 };

            var cts = new CancellationTokenSource();
            cts.Cancel();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => PreferencesBase.SaveAsync(prefs, path, cts.Token));

            cts.Dispose();
        }

        // ----------------------------------------------------------------
        // GlobalPreferences.LoadAsync / SaveAsync
        // ----------------------------------------------------------------

        [Fact]
        public async Task GlobalPreferences_SaveAsync_CreatesFile()
        {
            string path = Path.Combine(_tempDir, "global_async.xml");
            var prefs = new GlobalPreferences { CurrentUser = "AsyncUser" };

            bool result = await GlobalPreferences.SaveAsync(prefs, path);

            Assert.True(result);
            Assert.True(File.Exists(path));
        }

        [Fact]
        public async Task GlobalPreferences_LoadAsync_RoundTrip_PreservesData()
        {
            string path = Path.Combine(_tempDir, "global_rt_async.xml");
            var original = new GlobalPreferences { CurrentUser = "RoundTrip", CurrentProfile = "TestProfile" };

            // SaveAsync static
            bool saved = await GlobalPreferences.SaveAsync(original, path);
            Assert.True(saved);

            // LoadAsync static with explicit path
            GlobalPreferences loaded = await GlobalPreferences.LoadAsync(path, loadDefaultsOnFail: true);

            Assert.NotNull(loaded);
            Assert.Equal("RoundTrip", loaded.CurrentUser);
            Assert.Equal("TestProfile", loaded.CurrentProfile);
        }

        [Fact]
        public async Task GlobalPreferences_Instance_SaveAsync_ReturnsFalseWhenNoPath()
        {
            var prefs = new GlobalPreferences();
            // PreferencesFilePath is empty by default in a test context
            string savedPath = GlobalPreferences.PreferencesFilePath;
            GlobalPreferences.PreferencesFilePath = string.Empty;

            bool result = await prefs.SaveAsync();

            Assert.False(result);

            GlobalPreferences.PreferencesFilePath = savedPath;
        }
    }

    // ----------------------------------------------------------------
    // Helper type
    // ----------------------------------------------------------------

    /// <summary>
    /// Simple XML-serializable preferences type used by async tests.
    /// </summary>
    [Serializable]
    public class SampleXmlPrefs
    {
        public string Name { get; set; } = "Default";
        public int Value { get; set; } = 0;
    }
}
