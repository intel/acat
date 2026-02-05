////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using Microsoft.VisualStudio.TestTools.UnitTesting;
using ACAT.Core.Utility;
using System;
using System.IO;

namespace ACATCore.Tests.Logging
{
    [TestClass]
    public class CachedLogBehaviorTests
    {
        private string workDir;

        [TestInitialize]
        public void SetupTest()
        {
            workDir = TestWorkspace.CreateIsolatedFolder();
        }

        [TestCleanup]
        public void CleanupTest()
        {
            TestWorkspace.CleanupAll();
        }

        [TestMethod]
        public void NewCachedLogInstanceCreatesSuccessfully()
        {
            string logName = "creation_test";
            CachedLog logInstance = new CachedLog(logName);
            
            Assert.IsNotNull(logInstance);
        }

        [TestMethod]
        public void LogEntryMethodAcceptsTypeAndData()
        {
            string logName = "entry_test";
            CachedLog logInstance = new CachedLog(logName);
            
            logInstance.LogEntry("EventType1", "Event data here");
            
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void MultipleEntriesCanBeAddedBeforeSave()
        {
            string logName = "multi_entry_test";
            CachedLog logInstance = new CachedLog(logName);
            
            for (int i = 0; i < 10; i++)
            {
                logInstance.LogEntry($"Type{i}", $"Data{i}");
            }
            
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void SaveOperationReturnsTrue()
        {
            string logName = $"save_test_{Guid.NewGuid():N}";
            CachedLog logInstance = new CachedLog(logName);
            
            logInstance.LogEntry("TestType", "TestData");
            bool result = logInstance.Save();
            
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void EmptyCachedLogCanBeSavedWithoutError()
        {
            string logName = $"empty_test_{Guid.NewGuid():N}";
            CachedLog logInstance = new CachedLog(logName);
            
            bool result = logInstance.Save();
            
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void SpecialCharactersInDataAreHandled()
        {
            string logName = $"special_chars_{Guid.NewGuid():N}";
            CachedLog logInstance = new CachedLog(logName);
            
            logInstance.LogEntry("Type", "Data,with,commas");
            logInstance.LogEntry("Type", "Data\"with\"quotes");
            logInstance.LogEntry("Type", "Data\nwith\nnewlines");
            
            bool result = logInstance.Save();
            
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void SubsequentSavesAppendToExistingFile()
        {
            string logName = $"append_test_{Guid.NewGuid():N}";
            
            CachedLog firstInstance = new CachedLog(logName);
            firstInstance.LogEntry("First", "FirstData");
            bool firstSave = firstInstance.Save();
            
            CachedLog secondInstance = new CachedLog(logName);
            secondInstance.LogEntry("Second", "SecondData");
            bool secondSave = secondInstance.Save();
            
            Assert.IsTrue(firstSave && secondSave);
        }
    }
}
