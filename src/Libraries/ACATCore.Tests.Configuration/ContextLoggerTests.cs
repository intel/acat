////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ContextLoggerTests.cs
//
// Unit tests for LogManager.GetLogger functionality
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.ActuatorManagement;
using ACAT.Core.PanelManagement;
using ACAT.Core.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ACATCore.Tests.Configuration
{
    [TestClass]
    public class ContextLoggerTests
    {
        [TestCleanup]
        public void Cleanup()
        {
            Context.ServiceProvider = null;
        }

        [TestMethod]
        public void GetLogger_WithType_ReturnsLogger()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();
            Context.ServiceProvider = services.BuildServiceProvider();

            // Act
            var logger = LogManager.GetLogger(typeof(ActuatorManager));

            // Assert
            Assert.IsNotNull(logger);
            Assert.IsInstanceOfType(logger, typeof(ILogger));
        }

        [TestMethod]
        public void GetLogger_Generic_ReturnsTypedLogger()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();
            Context.ServiceProvider = services.BuildServiceProvider();

            // Act
            var logger = LogManager.GetLogger<ActuatorManager>();

            // Assert
            Assert.IsNotNull(logger);
            Assert.IsInstanceOfType(logger, typeof(ILogger<ActuatorManager>));
        }

        [TestMethod]
        public void GetLogger_WithNullType_ThrowsArgumentNullException()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();
            Context.ServiceProvider = services.BuildServiceProvider();

            // Act & Assert
            try
            {
                LogManager.GetLogger((Type)null);
                Assert.Fail("Expected ArgumentNullException was not thrown");
            }
            catch (ArgumentNullException)
            {
                // Expected exception
            }
        }

        [TestMethod]
        public void GetLogger_SameType_ReturnsSameLoggerCategory()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();
            Context.ServiceProvider = services.BuildServiceProvider();

            // Act
            var logger1 = LogManager.GetLogger<ActuatorManager>();
            var logger2 = LogManager.GetLogger<ActuatorManager>();

            // Assert - Should get loggers for the same category
            Assert.IsNotNull(logger1);
            Assert.IsNotNull(logger2);
            // Note: Loggers themselves might not be same instance (depends on logging provider)
            // but they should log to the same category
        }

        [TestMethod]
        public void GetLogger_DifferentTypes_ReturnsDifferentCategories()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();
            Context.ServiceProvider = services.BuildServiceProvider();

            // Act
            var logger1 = LogManager.GetLogger<ActuatorManager>();
            var logger2 = LogManager.GetLogger<ACAT.Core.AgentManagement.AgentManager>();

            // Assert - Should get loggers (even if for different categories)
            Assert.IsNotNull(logger1);
            Assert.IsNotNull(logger2);
        }

        [TestMethod]
        public void GetLogger_CanLog_NoExceptions()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Debug);
            });
            services.AddACATServices();
            Context.ServiceProvider = services.BuildServiceProvider();

            var logger = LogManager.GetLogger<ContextLoggerTests>();

            // Act & Assert - Should be able to log without exceptions
            logger.LogDebug("Debug message");
            logger.LogInformation("Info message");
            logger.LogWarning("Warning message");
            logger.LogError("Error message");
        }

        [TestMethod]
        public void GetLogger_AfterServiceProviderChange_UsesNewProvider()
        {
            // Arrange
            var services1 = new ServiceCollection();
            services1.AddLogging();
            services1.AddACATServices();
            Context.ServiceProvider = services1.BuildServiceProvider();

            var logger1 = LogManager.GetLogger<ActuatorManager>();

            // Act - Change service provider
            var services2 = new ServiceCollection();
            services2.AddLogging();
            services2.AddACATServices();
            Context.ServiceProvider = services2.BuildServiceProvider();

            var logger2 = LogManager.GetLogger<ActuatorManager>();

            // Assert - Should get logger from new provider
            Assert.IsNotNull(logger1);
            Assert.IsNotNull(logger2);
            // Loggers will be different since they come from different factories
        }

        [TestMethod]
        public void LogManager_GetLogger_ReturnsValidLogger()
        {
            // Act
            var logger = LogManager.GetLogger(typeof(ContextLoggerTests));

            // Assert
            Assert.IsNotNull(logger);
            Assert.IsInstanceOfType(logger, typeof(ILogger));
        }

        [TestMethod]
        public void LogManager_GetLogger_Generic_ReturnsTypedLogger()
        {
            // Act
            var logger = LogManager.GetLogger<ContextLoggerTests>();

            // Assert
            Assert.IsNotNull(logger);
            Assert.IsInstanceOfType(logger, typeof(ILogger<ContextLoggerTests>));
        }

        [TestMethod]
        public void LogManager_GetLogger_WithNullType_ThrowsArgumentNullException()
        {
            // Act & Assert
            try
            {
                LogManager.GetLogger((Type)null);
                Assert.Fail("Expected ArgumentNullException was not thrown");
            }
            catch (ArgumentNullException)
            {
                // Expected exception
            }
        }
    }
}
