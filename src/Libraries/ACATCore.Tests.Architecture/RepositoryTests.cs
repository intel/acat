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

using ACAT.Core.DataAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace ACATCore.Tests.Architecture
{
    /// <summary>
    /// Unit tests for <see cref="RepositoryBase{TEntity,TKey}"/>,
    /// <see cref="ConfigurationRepository"/>, and <see cref="PreferencesRepository"/>.
    /// </summary>
    [TestClass]
    public class RepositoryTests
    {
        // -----------------------------------------------------------------------
        // ConfigurationRepository – basic CRUD
        // -----------------------------------------------------------------------

        [TestMethod]
        public void ConfigurationRepository_Add_EntryIsRetrievable()
        {
            var repo = new ConfigurationRepository("dummy.cfg");
            repo.Add(new ConfigurationEntry { Key = "theme", Value = "dark" });
            var entry = repo.GetById("theme");
            Assert.IsNotNull(entry);
            Assert.AreEqual("dark", entry.Value);
        }

        [TestMethod]
        public void ConfigurationRepository_Update_ChangesValue()
        {
            var repo = new ConfigurationRepository("dummy.cfg");
            repo.Add(new ConfigurationEntry { Key = "lang", Value = "en" });
            repo.Update(new ConfigurationEntry { Key = "lang", Value = "fr" });
            Assert.AreEqual("fr", repo.GetById("lang").Value);
        }

        [TestMethod]
        public void ConfigurationRepository_Remove_EntryNoLongerExists()
        {
            var repo = new ConfigurationRepository("dummy.cfg");
            repo.Add(new ConfigurationEntry { Key = "key1", Value = "v1" });
            repo.Remove("key1");
            Assert.IsNull(repo.GetById("key1"));
        }

        [TestMethod]
        public void ConfigurationRepository_GetAll_ReturnsAllEntries()
        {
            var repo = new ConfigurationRepository("dummy.cfg");
            repo.Add(new ConfigurationEntry { Key = "a", Value = "1" });
            repo.Add(new ConfigurationEntry { Key = "b", Value = "2" });
            Assert.AreEqual(2, repo.GetAll().Count);
        }

        [TestMethod]
        public void ConfigurationRepository_GetById_ReturnsNull_WhenNotFound()
        {
            var repo = new ConfigurationRepository("dummy.cfg");
            Assert.IsNull(repo.GetById("missing"));
        }

        [TestMethod]
        public void ConfigurationRepository_GetValue_ReturnsDefault_WhenKeyMissing()
        {
            var repo = new ConfigurationRepository("dummy.cfg");
            var result = repo.GetValue("no-such-key", "fallback");
            Assert.AreEqual("fallback", result);
        }

        [TestMethod]
        public void ConfigurationRepository_SetValue_AddsNewEntry()
        {
            var repo = new ConfigurationRepository("dummy.cfg");
            repo.SetValue("color", "blue");
            Assert.AreEqual("blue", repo.GetValue("color"));
        }

        [TestMethod]
        public void ConfigurationRepository_SetValue_UpdatesExistingEntry()
        {
            var repo = new ConfigurationRepository("dummy.cfg");
            repo.SetValue("color", "blue");
            repo.SetValue("color", "red");
            Assert.AreEqual("red", repo.GetValue("color"));
        }

        [TestMethod]
        public void ConfigurationRepository_FilePath_IsPreserved()
        {
            var repo = new ConfigurationRepository("/path/to/config.xml");
            Assert.AreEqual("/path/to/config.xml", repo.FilePath);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConfigurationRepository_EmptyPath_ThrowsArgumentException()
        {
            new ConfigurationRepository("");
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void ConfigurationRepository_Update_NonExistentKey_ThrowsKeyNotFoundException()
        {
            var repo = new ConfigurationRepository("dummy.cfg");
            repo.Update(new ConfigurationEntry { Key = "ghost", Value = "v" });
        }

        // -----------------------------------------------------------------------
        // PreferencesRepository
        // -----------------------------------------------------------------------

        [TestMethod]
        public void PreferencesRepository_FilePath_IsPreserved()
        {
            var repo = new PreferencesRepository("/prefs/user.xml");
            Assert.AreEqual("/prefs/user.xml", repo.FilePath);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PreferencesRepository_EmptyPath_ThrowsArgumentException()
        {
            new PreferencesRepository("   ");
        }

        [TestMethod]
        public void PreferencesRepository_GetAll_ReturnsEmptyByDefault()
        {
            var repo = new PreferencesRepository("/prefs/user.xml");
            Assert.AreEqual(0, repo.GetAll().Count);
        }
    }
}
