////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ManagerFactoryTests.cs
//
// Unit tests for manager factory interfaces and implementations
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

namespace ACATCore.Tests.Configuration
{
    [TestClass]
    public class ManagerFactoryTests
    {
        [TestMethod]
        public void ActuatorManagerFactory_Create_ReturnsInstance()
        {
            // Arrange
            var factory = new ActuatorManagerFactory();

            // Act
            var manager = factory.Create();

            // Assert
            Assert.IsNotNull(manager);
            Assert.IsInstanceOfType(manager, typeof(IActuatorManager));
        }

        [TestMethod]
        public void AgentManagerFactory_Create_ReturnsInstance()
        {
            // Arrange
            var factory = new AgentManagerFactory();

            // Act
            var manager = factory.Create();

            // Assert
            Assert.IsNotNull(manager);
            Assert.IsInstanceOfType(manager, typeof(IAgentManager));
        }

        [TestMethod]
        public void TTSManagerFactory_Create_ReturnsInstance()
        {
            // Arrange
            var factory = new TTSManagerFactory();

            // Act
            var manager = factory.Create();

            // Assert
            Assert.IsNotNull(manager);
            Assert.IsInstanceOfType(manager, typeof(ITTSManager));
        }

        [TestMethod]
        public void PanelManagerFactory_Create_ReturnsInstance()
        {
            // Arrange
            var factory = new PanelManagerFactory();

            // Act
            var manager = factory.Create();

            // Assert
            Assert.IsNotNull(manager);
            Assert.IsInstanceOfType(manager, typeof(IPanelManager));
        }

        [TestMethod]
        public void ThemeManagerFactory_Create_ReturnsInstance()
        {
            // Arrange
            var factory = new ThemeManagerFactory();

            // Act
            var manager = factory.Create();

            // Assert
            Assert.IsNotNull(manager);
            Assert.IsInstanceOfType(manager, typeof(IThemeManager));
        }

        [TestMethod]
        public void WordPredictionManagerFactory_Create_ReturnsInstance()
        {
            // Arrange
            var factory = new WordPredictionManagerFactory();

            // Act
            var manager = factory.Create();

            // Assert
            Assert.IsNotNull(manager);
            Assert.IsInstanceOfType(manager, typeof(IWordPredictionManager));
        }

        [TestMethod]
        public void SpellCheckManagerFactory_Create_ReturnsInstance()
        {
            // Arrange
            var factory = new SpellCheckManagerFactory();

            // Act
            var manager = factory.Create();

            // Assert
            Assert.IsNotNull(manager);
            Assert.IsInstanceOfType(manager, typeof(ISpellCheckManager));
        }

        [TestMethod]
        public void AbbreviationsManagerFactory_Create_ReturnsInstance()
        {
            // Arrange
            var factory = new AbbreviationsManagerFactory();

            // Act
            var manager = factory.Create();

            // Assert
            Assert.IsNotNull(manager);
            Assert.IsInstanceOfType(manager, typeof(IAbbreviationsManager));
        }

        [TestMethod]
        public void CommandManagerFactory_Create_ReturnsInstance()
        {
            // Arrange
            var factory = new CommandManagerFactory();

            // Act
            var manager = factory.Create();

            // Assert
            Assert.IsNotNull(manager);
            Assert.IsInstanceOfType(manager, typeof(ICommandManager));
        }

        [TestMethod]
        public void AutomationEventManagerFactory_Create_ReturnsInstance()
        {
            // Arrange
            var factory = new AutomationEventManagerFactory();

            // Act
            var manager = factory.Create();

            // Assert
            Assert.IsNotNull(manager);
            Assert.IsInstanceOfType(manager, typeof(IAutomationEventManager));
        }

        [TestMethod]
        public void ServiceConfiguration_RegistersAllFactories()
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
        public void Factory_Create_ReturnsSameInstanceAsDirectResolution()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();
            var serviceProvider = services.BuildServiceProvider();

            // Act
            var factory = serviceProvider.GetService<IActuatorManagerFactory>();
            var managerFromFactory = factory.Create();
            var managerDirect = serviceProvider.GetService<IActuatorManager>();

            // Assert - Should be same instance
            Assert.AreSame(managerFromFactory, managerDirect);
        }
    }
}
