////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// RepositoryTests.cs
//
// Unit tests for the repository pattern base class and concrete repositories.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Configuration;
using ACAT.Core.DataAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace ACATCore.Tests.Architecture
{
    /// <summary>
    /// Unit tests for <see cref="IRepository{T}"/>, <see cref="PreferencesRepository{T}"/>,
    /// and <see cref="ConfigurationRepository{T}"/>.
    /// </summary>
    [TestClass]
    public class RepositoryTests
    {
        private string _tempDir;

        [TestInitialize]
        public void SetUp()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), "ACATArchTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempDir);
        }

        [TestCleanup]
        public void TearDown()
        {
            if (Directory.Exists(_tempDir))
                Directory.Delete(_tempDir, recursive: true);
        }

        // -----------------------------------------------------------------------
        // PreferencesRepository<T>
        // -----------------------------------------------------------------------

        [TestMethod]
        public void PreferencesRepository_ImplementsIRepository()
        {
            var repo = new PreferencesRepository<SamplePrefs>();
            Assert.IsInstanceOfType(repo, typeof(IRepository<SamplePrefs>));
        }

        [TestMethod]
        public void PreferencesRepository_GetDefault_ReturnsNonNull()
        {
            var repo = new PreferencesRepository<SamplePrefs>();
            var result = repo.GetDefault();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void PreferencesRepository_Load_NullPath_ReturnsNull()
        {
            var repo = new PreferencesRepository<SamplePrefs>();
            var result = repo.Load(null);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void PreferencesRepository_Save_RoundTrip()
        {
            var repo = new PreferencesRepository<SamplePrefs>();
            string path = Path.Combine(_tempDir, "prefs.xml");
            var prefs = new SamplePrefs { UserName = "TestUser", Volume = 75 };

            bool saved = repo.Save(prefs, path);
            Assert.IsTrue(saved, "Save should return true");
            Assert.IsTrue(File.Exists(path), "Preferences file should exist after save");

            var loaded = repo.Load(path);
            Assert.IsNotNull(loaded);
            Assert.AreEqual("TestUser", loaded.UserName);
            Assert.AreEqual(75, loaded.Volume);
        }

        // -----------------------------------------------------------------------
        // ConfigurationRepository<T>
        // -----------------------------------------------------------------------

        [TestMethod]
        public void ConfigurationRepository_ImplementsIRepository()
        {
            var repo = new ConfigurationRepository<AbbreviationsJson>();
            Assert.IsInstanceOfType(repo, typeof(IRepository<AbbreviationsJson>));
        }

        [TestMethod]
        public void ConfigurationRepository_GetDefault_ReturnsNonNull()
        {
            var repo = new ConfigurationRepository<AbbreviationsJson>();
            var result = repo.GetDefault();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ConfigurationRepository_Load_NullPath_ReturnsNull()
        {
            var repo = new ConfigurationRepository<AbbreviationsJson>();
            var result = repo.Load(null);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ConfigurationRepository_Load_MissingFile_ReturnsDefault()
        {
            var repo = new ConfigurationRepository<AbbreviationsJson>();
            string path = Path.Combine(_tempDir, "nonexistent.json");
            var result = repo.Load(path);
            Assert.IsNotNull(result);
        }

        // -----------------------------------------------------------------------
        // Helper type
        // -----------------------------------------------------------------------

        [Serializable]
        public class SamplePrefs
        {
            public string UserName { get; set; } = "Default";
            public int Volume { get; set; } = 50;
        }
    }
}
