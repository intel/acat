////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Logging;
using ACAT.Core.Utility;
using System;

namespace ACATCore.Tests.Logging
{
    [TestClass]
    public class ModernLoggingConfigTests
    {
        [TestMethod]
        public void GenericLoggerCreationSucceeds()
        {
            ILogger<ModernLoggingConfigTests> logger = LoggingConfiguration.CreateLogger<ModernLoggingConfigTests>();
            
            Assert.IsNotNull(logger);
        }

        [TestMethod]
        public void CategoryNameLoggerCreationSucceeds()
        {
            string category = "CustomCategory";
            ILogger logger = LoggingConfiguration.CreateLogger(category);
            
            Assert.IsNotNull(logger);
        }

        [TestMethod]
        public void LoggerFactoryCreationSucceeds()
        {
            ILoggerFactory factory = LoggingConfiguration.CreateLoggerFactory();
            
            Assert.IsNotNull(factory);
        }

        [TestMethod]
        public void DebugLevelLoggingAccepted()
        {
            ILogger logger = LoggingConfiguration.CreateLogger("DebugTest");
            
            logger.LogDebug("Debug level message");
            
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void InformationLevelLoggingAccepted()
        {
            ILogger logger = LoggingConfiguration.CreateLogger("InfoTest");
            
            logger.LogInformation("Information level message");
            
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void WarningLevelLoggingAccepted()
        {
            ILogger logger = LoggingConfiguration.CreateLogger("WarnTest");
            
            logger.LogWarning("Warning level message");
            
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void ErrorLevelLoggingAccepted()
        {
            ILogger logger = LoggingConfiguration.CreateLogger("ErrorTest");
            
            logger.LogError("Error level message");
            
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void StructuredLoggingWithParametersWorks()
        {
            ILogger logger = LoggingConfiguration.CreateLogger("StructuredTest");
            
            string userId = "User123";
            int actionId = 456;
            DateTime timestamp = DateTime.Now;
            
            logger.LogInformation("User {UserId} performed action {ActionId} at {Timestamp}",
                userId, actionId, timestamp);
            
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void ExceptionLoggingWithMessageWorks()
        {
            ILogger logger = LoggingConfiguration.CreateLogger("ExceptionTest");
            
            Exception ex = new InvalidOperationException("Test exception");
            logger.LogError(ex, "Exception occurred during test");
            
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void MultipleLoggerInstancesCoexist()
        {
            ILogger logger1 = LoggingConfiguration.CreateLogger("Logger1");
            ILogger logger2 = LoggingConfiguration.CreateLogger("Logger2");
            ILogger<ModernLoggingConfigTests> logger3 = LoggingConfiguration.CreateLogger<ModernLoggingConfigTests>();
            
            Assert.IsNotNull(logger1);
            Assert.IsNotNull(logger2);
            Assert.IsNotNull(logger3);
        }

        [TestMethod]
        public void FactoryCanCreateMultipleLoggers()
        {
            ILoggerFactory factory = LoggingConfiguration.CreateLoggerFactory();
            
            ILogger logger1 = factory.CreateLogger("Factory1");
            ILogger logger2 = factory.CreateLogger("Factory2");
            
            Assert.IsNotNull(logger1);
            Assert.IsNotNull(logger2);
        }
    }
}
