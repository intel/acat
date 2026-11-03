////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// FactoryRegistrationTests.cs
//
// Unit tests for factory registration and resolution
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ACATCore.Tests.Configuration
{
    [TestClass]
    public class FactoryRegistrationTests
    {
        [TestMethod]
        public void AddACATServices_RegistersAllFactories()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();

            // Act
            services.AddACATServices();
            var serviceProvider = services.BuildServiceProvider();

            // Assert - All factory interfaces should be resolvable
            Assert.IsNotNull(serviceProvider.GetService<IActuatorManagerFactory>());
            Assert.IsNotNull(serviceProvider.GetService<IAgentManagerFactory>());
            Assert.IsNotNull(serviceProvider.GetService<ITTSManagerFactory>());
            Assert.IsNotNull(serviceProvider.GetService<IPanelManagerFactory>());
            Assert.IsNotNull(serviceProvider.GetService<IThemeManagerFactory>());
            Assert.IsNotNull(serviceProvider.GetService<IWordPredictionManagerFactory>());
            Assert.IsNotNull(serviceProvider.GetService<ISpellCheckManagerFactory>());
            Assert.IsNotNull(serviceProvider.GetService<IAbbreviationsManagerFactory>());
            Assert.IsNotNull(serviceProvider.GetService<ICommandManagerFactory>());
            Assert.IsNotNull(serviceProvider.GetService<IAutomationEventManagerFactory>());
        }

        [TestMethod]
        public void ActuatorManagerFactory_FromDI_CreatesManager()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();
            var serviceProvider = services.BuildServiceProvider();

            // Act
            var factory = serviceProvider.GetService<IActuatorManagerFactory>();
            var manager = factory.Create();

            // Assert
            Assert.IsNotNull(manager);
            Assert.IsInstanceOfType(manager, typeof(IActuatorManager));
        }

        [TestMethod]
        public void AgentManagerFactory_FromDI_CreatesManager()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();
            var serviceProvider = services.BuildServiceProvider();

            // Act
            var factory = serviceProvider.GetService<IAgentManagerFactory>();
            var manager = factory.Create();

            // Assert
            Assert.IsNotNull(manager);
            Assert.IsInstanceOfType(manager, typeof(IAgentManager));
        }

        [TestMethod]
        public void TTSManagerFactory_FromDI_CreatesManager()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();
            var serviceProvider = services.BuildServiceProvider();

            // Act
            var factory = serviceProvider.GetService<ITTSManagerFactory>();
            var manager = factory.Create();

            // Assert
            Assert.IsNotNull(manager);
            Assert.IsInstanceOfType(manager, typeof(ITTSManager));
        }

        [TestMethod]
        public void PanelManagerFactory_FromDI_CreatesManager()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();
            var serviceProvider = services.BuildServiceProvider();

            // Act
            var factory = serviceProvider.GetService<IPanelManagerFactory>();
            var manager = factory.Create();

            // Assert
            Assert.IsNotNull(manager);
            Assert.IsInstanceOfType(manager, typeof(IPanelManager));
        }

        [TestMethod]
        public void ThemeManagerFactory_FromDI_CreatesManager()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();
            var serviceProvider = services.BuildServiceProvider();

            // Act
            var factory = serviceProvider.GetService<IThemeManagerFactory>();
            var manager = factory.Create();

            // Assert
            Assert.IsNotNull(manager);
            Assert.IsInstanceOfType(manager, typeof(IThemeManager));
        }

        [TestMethod]
        public void WordPredictionManagerFactory_FromDI_CreatesManager()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();
            var serviceProvider = services.BuildServiceProvider();

            // Act
            var factory = serviceProvider.GetService<IWordPredictionManagerFactory>();
            var manager = factory.Create();

            // Assert
            Assert.IsNotNull(manager);
            Assert.IsInstanceOfType(manager, typeof(IWordPredictionManager));
        }

        [TestMethod]
        public void SpellCheckManagerFactory_FromDI_CreatesManager()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();
            var serviceProvider = services.BuildServiceProvider();

            // Act
            var factory = serviceProvider.GetService<ISpellCheckManagerFactory>();
            var manager = factory.Create();

            // Assert
            Assert.IsNotNull(manager);
            Assert.IsInstanceOfType(manager, typeof(ISpellCheckManager));
        }

        [TestMethod]
        public void AbbreviationsManagerFactory_FromDI_CreatesManager()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();
            var serviceProvider = services.BuildServiceProvider();

            // Act
            var factory = serviceProvider.GetService<IAbbreviationsManagerFactory>();
            var manager = factory.Create();

            // Assert
            Assert.IsNotNull(manager);
            Assert.IsInstanceOfType(manager, typeof(IAbbreviationsManager));
        }

        [TestMethod]
        public void CommandManagerFactory_FromDI_CreatesManager()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();
            var serviceProvider = services.BuildServiceProvider();

            // Act
            var factory = serviceProvider.GetService<ICommandManagerFactory>();
            var manager = factory.Create();

            // Assert
            Assert.IsNotNull(manager);
            Assert.IsInstanceOfType(manager, typeof(ICommandManager));
        }

        [TestMethod]
        public void AutomationEventManagerFactory_FromDI_CreatesManager()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();
            var serviceProvider = services.BuildServiceProvider();

            // Act
            var factory = serviceProvider.GetService<IAutomationEventManagerFactory>();
            var manager = factory.Create();

            // Assert
            Assert.IsNotNull(manager);
            Assert.IsInstanceOfType(manager, typeof(IAutomationEventManager));
        }

        [TestMethod]
        public void Factory_CreatedManager_MatchesDIResolvedManager()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();
            var serviceProvider = services.BuildServiceProvider();

            // Act
            var factory = serviceProvider.GetService<IActuatorManagerFactory>();
            var managerFromFactory = factory.Create();
            var managerFromDI = serviceProvider.GetService<IActuatorManager>();

            // Assert - Should be same instance since both resolve to singleton
            Assert.AreSame(managerFromFactory, managerFromDI);
        }

        [TestMethod]
        public void Factory_MultipleCalls_ReturnsSameInstance()
        {
            // Arrange
            var factory = new ActuatorManagerFactory();

            // Act
            var manager1 = factory.Create();
            var manager2 = factory.Create();

            // Assert - Factory returns singleton instance each time
            Assert.AreSame(manager1, manager2);
        }
    }
}
