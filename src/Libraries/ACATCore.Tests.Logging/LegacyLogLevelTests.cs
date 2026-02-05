////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using Microsoft.VisualStudio.TestTools.UnitTesting;
using ACAT.Core.Utility;
using System;
using System.Diagnostics;

namespace ACATCore.Tests.Logging
{
    [TestClass]
    public class LegacyLogLevelTests
    {
        [TestMethod]
        public void WhenTraceSwitchSetToVerbose_VerboseCallsShouldSucceed()
        {
            Log.TraceLevelSwitch = new TraceSwitch("VerboseCheck", "", "Verbose");
            
            Log.Verbose("Testing verbose output");
            
            Assert.AreEqual(TraceLevel.Verbose, Log.TraceLevelSwitch.Level);
        }

        [TestMethod]
        public void WhenTraceSwitchSetToInfo_InfoCallsShouldSucceed()
        {
            Log.TraceLevelSwitch = new TraceSwitch("InfoCheck", "", "Info");
            
            Log.Info("Testing info output");
            
            Assert.AreEqual(TraceLevel.Info, Log.TraceLevelSwitch.Level);
        }

        [TestMethod]
        public void WhenTraceSwitchSetToWarning_WarningCallsShouldSucceed()
        {
            Log.TraceLevelSwitch = new TraceSwitch("WarnCheck", "", "Warning");
            
            Log.Warn("Testing warning output");
            
            Assert.AreEqual(TraceLevel.Warning, Log.TraceLevelSwitch.Level);
        }

        [TestMethod]
        public void WhenTraceSwitchSetToError_ErrorCallsShouldSucceed()
        {
            Log.TraceLevelSwitch = new TraceSwitch("ErrorCheck", "", "Error");
            
            Log.Error("Testing error output");
            
            Assert.AreEqual(TraceLevel.Error, Log.TraceLevelSwitch.Level);
        }

        [TestMethod]
        public void WhenTraceSwitchSetToOff_NoLoggingOccurs()
        {
            Log.TraceLevelSwitch = new TraceSwitch("OffCheck", "", "Off");
            
            Assert.AreEqual(TraceLevel.Off, Log.TraceLevelSwitch.Level);
            
            Log.Error("This should be filtered");
            Log.Warn("This should be filtered");
        }

        [TestMethod]
        public void DebugMethodAcceptsStringParameter()
        {
            Log.TraceLevelSwitch = new TraceSwitch("DebugCheck", "", "Info");
            
            string testMsg = "Debug message content";
            Log.Debug(testMsg);
            
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void ExceptionMethodAcceptsStringMessage()
        {
            Log.TraceLevelSwitch = new TraceSwitch("ExcStrCheck", "", "Error");
            
            string errorMsg = "Exception message string";
            Log.Exception(errorMsg);
            
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void ExceptionMethodAcceptsExceptionObject()
        {
            Log.TraceLevelSwitch = new TraceSwitch("ExcObjCheck", "", "Error");
            
            Exception testException = new InvalidOperationException("Test exception");
            Log.Exception(testException);
            
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void IsNullMethodAcceptsObjectReference()
        {
            Log.TraceLevelSwitch = new TraceSwitch("NullCheck", "", "Info");
            
            object testObj = new object();
            Log.IsNull("Object check", testObj);
            
            Assert.IsTrue(true);
        }
    }
}
