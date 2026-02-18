////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ServiceConfigurationTests.cs
//
// Unit tests for ServiceConfiguration
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.AbbreviationsManagement;
using ACAT.Core.ActuatorManagement;
using ACAT.Core.AgentManagement;
using ACAT.Core.CommandManagement;
using ACAT.Core.PanelManagement;
using ACAT.Core.SpellCheckManagement;
using ACAT.Core.ThemeManagement;
using ACAT.Core.TTSManagement;
using ACAT.Core.Utility;
using ACAT.Core.WordPredictorManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ACATCore.Tests.Configuration
{
    [TestClass]
    public class ServiceConfigurationTests
    {
        [TestMethod]
        public void AddACATServices_WithNullServices_ThrowsArgumentNullException()
        {
            // Arrange
            IServiceCollection services = null;

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => services.AddACATServices());
        }

        [TestMethod]
        public void AddACATServices_RegistersAllManagers()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();

            // Act
            services.AddACATServices();
            var serviceProvider = services.BuildServiceProvider();

            // Assert - All managers should be resolvable
            Assert.IsNotNull(serviceProvider.GetService<ActuatorManager>());
            Assert.IsNotNull(serviceProvider.GetService<AgentManager>());
            Assert.IsNotNull(serviceProvider.GetService<TTSManager>());
            Assert.IsNotNull(serviceProvider.GetService<PanelManager>());
            Assert.IsNotNull(serviceProvider.GetService<ThemeManager>());
            Assert.IsNotNull(serviceProvider.GetService<WordPredictionManager>());
            Assert.IsNotNull(serviceProvider.GetService<SpellCheckManager>());
            Assert.IsNotNull(serviceProvider.GetService<AbbreviationsManager>());
            Assert.IsNotNull(serviceProvider.GetService<CommandManager>());
            Assert.IsNotNull(serviceProvider.GetService<AutomationEventManager>());
        }

        [TestMethod]
        public void AddACATServices_RegistersManagersAsSingletons()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();
            var serviceProvider = services.BuildServiceProvider();

            // Act - Get same manager instance twice
            var actuatorManager1 = serviceProvider.GetService<ActuatorManager>();
            var actuatorManager2 = serviceProvider.GetService<ActuatorManager>();

            // Assert - Should be same instance
            Assert.AreSame(actuatorManager1, actuatorManager2);
        }

        [TestMethod]
        public void AddACATInfrastructure_WithNullServices_ThrowsArgumentNullException()
        {
            // Arrange
            IServiceCollection services = null;

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => services.AddACATInfrastructure());
        }

        [TestMethod]
        public void AddACATInfrastructure_ConfiguresLoggingAndServices()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddACATInfrastructure();
            var serviceProvider = services.BuildServiceProvider();

            // Assert - Logging should be configured
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            Assert.IsNotNull(loggerFactory);

            // Assert - Services should be configured
            Assert.IsNotNull(serviceProvider.GetService<ActuatorManager>());
        }

        [TestMethod]
        public void CreateServiceProvider_ReturnsConfiguredProvider()
        {
            // Act
            var serviceProvider = ServiceConfiguration.CreateServiceProvider();

            // Assert
            Assert.IsNotNull(serviceProvider);
            Assert.IsNotNull(serviceProvider.GetService<ILoggerFactory>());
            Assert.IsNotNull(serviceProvider.GetService<ActuatorManager>());
        }

        [TestMethod]
        public void CreateServiceProvider_WithLoggerFactory_UsesProvidedFactory()
        {
            // Arrange
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

            // Act
            var serviceProvider = ServiceConfiguration.CreateServiceProvider(loggerFactory);

            // Assert
            Assert.IsNotNull(serviceProvider);
            var resolvedFactory = serviceProvider.GetService<ILoggerFactory>();
            Assert.AreSame(loggerFactory, resolvedFactory);
        }

        [TestMethod]
        public void CreateServiceProvider_WithNullLoggerFactory_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => 
                ServiceConfiguration.CreateServiceProvider(null));
        }

        [TestMethod]
        public void AddACATServices_AllowsChaining()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            var result = services.AddLogging()
                                .AddACATServices();

            // Assert
            Assert.AreSame(services, result);
        }

        [TestMethod]
        public void AddACATInfrastructure_AllowsChaining()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            var result = services.AddACATInfrastructure();

            // Assert
            Assert.AreSame(services, result);
        }
    }
}
