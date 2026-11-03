////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// RepositoryTests.cs
//
// Unit tests for the DataAccess repository classes.
//
////////////////////////////////////////////////////////////////////////////

// xUnit1051: In test methods the async methods below are testing library cancellation behaviour
// directly (not using test-framework cancellation), so suppress the recommendation.
#pragma warning disable xUnit1051

using ACAT.Core.Configuration;
using ACAT.Core.DataAccess;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ACATCore.Tests
{
    /// <summary>
    /// Unit tests for <see cref="IRepository{T}"/>, <see cref="IAsyncRepository{T}"/>,
    /// <see cref="PreferencesRepository{T}"/>, <see cref="ConfigurationRepository{T}"/>,
    /// and <see cref="ThemeRepository"/>.
    /// </summary>
    public class RepositoryTests : IDisposable
    {
        private readonly string _tempDir;

        public RepositoryTests()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), "ACATRepositoryTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempDir);
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempDir))
            {
                Directory.Delete(_tempDir, recursive: true);
            }
        }

        // ----------------------------------------------------------------
        // PreferencesRepository tests
        // ----------------------------------------------------------------

        [Fact]
        public void PreferencesRepository_ImplementsIRepository()
        {
            var repo = new PreferencesRepository<SamplePreferences>();

            Assert.IsAssignableFrom<IRepository<SamplePreferences>>(repo);
        }

        [Fact]
        public void PreferencesRepository_GetDefault_ReturnsNonNull()
        {
            var repo = new PreferencesRepository<SamplePreferences>();

            var result = repo.GetDefault();

            Assert.NotNull(result);
        }

        [Fact]
        public void PreferencesRepository_Save_ReturnsFalseForNullEntity()
        {
            var repo = new PreferencesRepository<SamplePreferences>();

            bool success = repo.Save(null, Path.Combine(_tempDir, "prefs.xml"));

            Assert.False(success);
        }

        [Fact]
        public void PreferencesRepository_Save_ReturnsFalseForNullPath()
        {
            var repo = new PreferencesRepository<SamplePreferences>();

            bool success = repo.Save(new SamplePreferences(), null);

            Assert.False(success);
        }

        [Fact]
        public void PreferencesRepository_Load_ReturnsDefaultForNullPath()
        {
            var repo = new PreferencesRepository<SamplePreferences>();

            var result = repo.Load(null);

            Assert.Null(result);
        }

        [Fact]
        public void PreferencesRepository_Load_ReturnsDefaultWhenFileMissing()
        {
            var repo = new PreferencesRepository<SamplePreferences>();
            string path = Path.Combine(_tempDir, "nonexistent.xml");

            var result = repo.Load(path);

            // Should return a new default instance rather than null
            Assert.NotNull(result);
        }

        // ----------------------------------------------------------------
        // ConfigurationRepository tests
        // ----------------------------------------------------------------

        [Fact]
        public void ConfigurationRepository_ImplementsIRepository()
        {
            var repo = new ConfigurationRepository<AbbreviationsJson>();

            Assert.IsAssignableFrom<IRepository<AbbreviationsJson>>(repo);
        }

        [Fact]
        public void ConfigurationRepository_GetDefault_ReturnsNonNull()
        {
            var repo = new ConfigurationRepository<AbbreviationsJson>();

            var result = repo.GetDefault();

            Assert.NotNull(result);
        }

        [Fact]
        public void ConfigurationRepository_Save_CreatesFile()
        {
            var repo = new ConfigurationRepository<AbbreviationsJson>();
            string path = Path.Combine(_tempDir, "abbreviations.json");
            var config = AbbreviationsJson.CreateDefault();

            bool success = repo.Save(config, path);

            Assert.True(success);
            Assert.True(File.Exists(path));
        }

        [Fact]
        public void ConfigurationRepository_RoundTrip_PreservesData()
        {
            var repo = new ConfigurationRepository<AbbreviationsJson>();
            string path = Path.Combine(_tempDir, "abbreviations_rt.json");
            var original = new AbbreviationsJson();
            original.Abbreviations.Add(new AbbreviationJson { Word = "brb", ReplaceWith = "be right back" });

            repo.Save(original, path);
            var loaded = repo.Load(path);

            Assert.NotNull(loaded);
            Assert.Single(loaded.Abbreviations);
            Assert.Equal("brb", loaded.Abbreviations[0].Word);
            Assert.Equal("be right back", loaded.Abbreviations[0].ReplaceWith);
        }

        [Fact]
        public void ConfigurationRepository_Load_ReturnsDefaultWhenFileMissing()
        {
            var repo = new ConfigurationRepository<AbbreviationsJson>();
            string path = Path.Combine(_tempDir, "missing.json");

            var result = repo.Load(path);

            Assert.NotNull(result);
        }

        [Fact]
        public void ConfigurationRepository_Save_ReturnsFalseForNullEntity()
        {
            var repo = new ConfigurationRepository<AbbreviationsJson>();

            bool success = repo.Save(null, Path.Combine(_tempDir, "config.json"));

            Assert.False(success);
        }

        [Fact]
        public void ConfigurationRepository_Save_ReturnsFalseForNullPath()
        {
            var repo = new ConfigurationRepository<AbbreviationsJson>();

            bool success = repo.Save(new AbbreviationsJson(), null);

            Assert.False(success);
        }

        [Fact]
        public void ConfigurationRepository_Load_ReturnsNullForNullPath()
        {
            var repo = new ConfigurationRepository<AbbreviationsJson>();

            var result = repo.Load(null);

            Assert.Null(result);
        }

        // ----------------------------------------------------------------
        // ThemeRepository tests
        // ----------------------------------------------------------------

        [Fact]
        public void ThemeRepository_ImplementsIRepository()
        {
            var repo = new ThemeRepository();

            Assert.IsAssignableFrom<IRepository<ACAT.Core.ThemeManagement.Theme>>(repo);
        }

        [Fact]
        public void ThemeRepository_GetDefault_ReturnsNonNull()
        {
            var repo = new ThemeRepository();

            var result = repo.GetDefault();

            Assert.NotNull(result);
        }

        [Fact]
        public void ThemeRepository_Load_ReturnsNullForNullPath()
        {
            var repo = new ThemeRepository();

            var result = repo.Load(null);

            Assert.Null(result);
        }

        [Fact]
        public void ThemeRepository_Load_ReturnsNullForMissingFile()
        {
            var repo = new ThemeRepository();
            string path = Path.Combine(_tempDir, "nonexistent", "Theme.json");

            var result = repo.Load(path);

            Assert.Null(result);
        }

        [Fact]
        public void ThemeRepository_Save_ReturnsFalse()
        {
            var repo = new ThemeRepository();
            var theme = repo.GetDefault();

            bool success = repo.Save(theme, "anykey");

            Assert.False(success);
        }

        // ----------------------------------------------------------------
        // IAsyncRepository<T> — ConfigurationRepository async tests
        // ----------------------------------------------------------------

        [Fact]
        public void ConfigurationRepository_ImplementsIAsyncRepository()
        {
            var repo = new ConfigurationRepository<AbbreviationsJson>();

            Assert.IsAssignableFrom<IAsyncRepository<AbbreviationsJson>>(repo);
        }

        [Fact]
        public async Task ConfigurationRepository_LoadAsync_ReturnsNullForNullPath()
        {
            var repo = new ConfigurationRepository<AbbreviationsJson>();

            var result = await repo.LoadAsync(null);

            Assert.Null(result);
        }

        [Fact]
        public async Task ConfigurationRepository_LoadAsync_ReturnsDefaultWhenFileMissing()
        {
            var repo = new ConfigurationRepository<AbbreviationsJson>();
            string path = Path.Combine(_tempDir, "missing_async.json");

            var result = await repo.LoadAsync(path);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task ConfigurationRepository_SaveAsync_ReturnsFalseForNullEntity()
        {
            var repo = new ConfigurationRepository<AbbreviationsJson>();

            bool success = await repo.SaveAsync(null, Path.Combine(_tempDir, "config_async.json"));

            Assert.False(success);
        }

        [Fact]
        public async Task ConfigurationRepository_SaveAsync_ReturnsFalseForNullPath()
        {
            var repo = new ConfigurationRepository<AbbreviationsJson>();

            bool success = await repo.SaveAsync(new AbbreviationsJson(), null);

            Assert.False(success);
        }

        [Fact]
        public async Task ConfigurationRepository_AsyncRoundTrip_PreservesData()
        {
            var repo = new ConfigurationRepository<AbbreviationsJson>();
            string path = Path.Combine(_tempDir, "abbreviations_async_rt.json");
            var original = new AbbreviationsJson();
            original.Abbreviations.Add(new AbbreviationJson { Word = "lol", ReplaceWith = "laughing out loud" });

            bool saved = await repo.SaveAsync(original, path);
            Assert.True(saved);

            var loaded = await repo.LoadAsync(path);

            Assert.NotNull(loaded);
            Assert.Single(loaded.Abbreviations);
            Assert.Equal("lol", loaded.Abbreviations[0].Word);
            Assert.Equal("laughing out loud", loaded.Abbreviations[0].ReplaceWith);
        }

        [Fact]
        public async Task ConfigurationRepository_GetDefaultAsync_ReturnsNonNull()
        {
            var repo = new ConfigurationRepository<AbbreviationsJson>();

            var result = await repo.GetDefaultAsync();

            Assert.NotNull(result);
        }

        [Fact]
        public async Task ConfigurationRepository_LoadAsync_RespectsCancellation()
        {
            var repo = new ConfigurationRepository<AbbreviationsJson>();
            string path = Path.Combine(_tempDir, "cancel_test.json");

            // Write a file so Load proceeds past the existence check to the async read stage
            File.WriteAllText(path, "{}");

            var cts = new CancellationTokenSource();
            cts.Cancel();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => repo.LoadAsync(path, cts.Token));

            cts.Dispose();
        }

        // ----------------------------------------------------------------
        // IAsyncRepository<T> — PreferencesRepository async tests
        // ----------------------------------------------------------------

        [Fact]
        public void PreferencesRepository_ImplementsIAsyncRepository()
        {
            var repo = new PreferencesRepository<SamplePreferences>();

            Assert.IsAssignableFrom<IAsyncRepository<SamplePreferences>>(repo);
        }

        [Fact]
        public async Task PreferencesRepository_AsyncRoundTrip_PreservesData()
        {
            var repo = new PreferencesRepository<SamplePreferences>();
            string path = Path.Combine(_tempDir, "prefs_async_rt.xml");
            var original = new SamplePreferences { UserName = "AsyncUser", Volume = 90 };

            bool saved = await repo.SaveAsync(original, path);
            Assert.True(saved);
            Assert.True(File.Exists(path));

            var loaded = await repo.LoadAsync(path);

            Assert.NotNull(loaded);
            Assert.Equal("AsyncUser", loaded.UserName);
            Assert.Equal(90, loaded.Volume);
        }

        [Fact]
        public async Task PreferencesRepository_GetDefaultAsync_ReturnsNonNull()
        {
            var repo = new PreferencesRepository<SamplePreferences>();

            var result = await repo.GetDefaultAsync();

            Assert.NotNull(result);
        }

        // ----------------------------------------------------------------
        // IAsyncRepository<T> — ThemeRepository async tests
        // ----------------------------------------------------------------

        [Fact]
        public void ThemeRepository_ImplementsIAsyncRepository()
        {
            var repo = new ThemeRepository();

            Assert.IsAssignableFrom<IAsyncRepository<ACAT.Core.ThemeManagement.Theme>>(repo);
        }

        [Fact]
        public async Task ThemeRepository_LoadAsync_ReturnsNullForNullPath()
        {
            var repo = new ThemeRepository();

            var result = await repo.LoadAsync(null);

            Assert.Null(result);
        }

        [Fact]
        public async Task ThemeRepository_SaveAsync_ReturnsFalse()
        {
            var repo = new ThemeRepository();
            var theme = repo.GetDefault();

            bool success = await repo.SaveAsync(theme, "anykey");

            Assert.False(success);
        }

        [Fact]
        public async Task ThemeRepository_GetDefaultAsync_ReturnsNonNull()
        {
            var repo = new ThemeRepository();

            var result = await repo.GetDefaultAsync();

            Assert.NotNull(result);
        }
    }

// ----------------------------------------------------------------
// Helper types used by tests
// ----------------------------------------------------------------

/// <summary>
/// Simple serializable class for PreferencesRepository round-trip tests.
/// </summary>
[Serializable]
public class SamplePreferences
{
    public string UserName { get; set; } = "Default";
    public int Volume { get; set; } = 50;
}
}
