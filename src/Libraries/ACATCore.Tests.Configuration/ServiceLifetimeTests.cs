////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ServiceLifetimeTests.cs
//
// Unit tests for service lifetime management (Singleton, Scoped, Transient)
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
using System.Linq;

namespace ACATCore.Tests.Configuration
{
    [TestClass]
    public class ServiceLifetimeTests
    {
        [TestMethod]
        public void AllManagers_AreRegisteredAsSingletons()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();

            // Act - Get service descriptors for all manager interfaces
            var descriptors = services.Where(sd => 
                sd.ServiceType == typeof(IActuatorManager) ||
                sd.ServiceType == typeof(IAgentManager) ||
                sd.ServiceType == typeof(ITTSManager) ||
                sd.ServiceType == typeof(IPanelManager) ||
                sd.ServiceType == typeof(IThemeManager) ||
                sd.ServiceType == typeof(IWordPredictionManager) ||
                sd.ServiceType == typeof(ISpellCheckManager) ||
                sd.ServiceType == typeof(IAbbreviationsManager) ||
                sd.ServiceType == typeof(ICommandManager) ||
                sd.ServiceType == typeof(IAutomationEventManager)
            ).ToList();

            // Assert - All should be Singleton
            Assert.AreEqual(10, descriptors.Count, "Expected 10 manager interface registrations");
            foreach (var descriptor in descriptors)
            {
                Assert.AreEqual(ServiceLifetime.Singleton, descriptor.Lifetime,
                    $"{descriptor.ServiceType.Name} should be registered as Singleton");
            }
        }

        [TestMethod]
        public void ActuatorManager_MultipleCalls_ReturnsSameInstance()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();
            var serviceProvider = services.BuildServiceProvider();

            // Act
            var instance1 = serviceProvider.GetService<IActuatorManager>();
            var instance2 = serviceProvider.GetService<IActuatorManager>();
            var instance3 = serviceProvider.GetService<ActuatorManager>();

            // Assert
            Assert.AreSame(instance1, instance2);
            Assert.AreSame(instance1, instance3);
        }

        [TestMethod]
        public void AgentManager_MultipleCalls_ReturnsSameInstance()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();
            var serviceProvider = services.BuildServiceProvider();

            // Act
            var instance1 = serviceProvider.GetService<IAgentManager>();
            var instance2 = serviceProvider.GetService<IAgentManager>();

            // Assert
            Assert.AreSame(instance1, instance2);
        }

        [TestMethod]
        public void TTSManager_MultipleCalls_ReturnsSameInstance()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();
            var serviceProvider = services.BuildServiceProvider();

            // Act
            var instance1 = serviceProvider.GetService<ITTSManager>();
            var instance2 = serviceProvider.GetService<ITTSManager>();

            // Assert
            Assert.AreSame(instance1, instance2);
        }

        [TestMethod]
        public void PanelManager_MultipleCalls_ReturnsSameInstance()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();
            var serviceProvider = services.BuildServiceProvider();

            // Act
            var instance1 = serviceProvider.GetService<IPanelManager>();
            var instance2 = serviceProvider.GetService<IPanelManager>();

            // Assert
            Assert.AreSame(instance1, instance2);
        }

        [TestMethod]
        public void ThemeManager_MultipleCalls_ReturnsSameInstance()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();
            var serviceProvider = services.BuildServiceProvider();

            // Act
            var instance1 = serviceProvider.GetService<IThemeManager>();
            var instance2 = serviceProvider.GetService<IThemeManager>();

            // Assert
            Assert.AreSame(instance1, instance2);
        }

        [TestMethod]
        public void WordPredictionManager_MultipleCalls_ReturnsSameInstance()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();
            var serviceProvider = services.BuildServiceProvider();

            // Act
            var instance1 = serviceProvider.GetService<IWordPredictionManager>();
            var instance2 = serviceProvider.GetService<IWordPredictionManager>();

            // Assert
            Assert.AreSame(instance1, instance2);
        }

        [TestMethod]
        public void SpellCheckManager_MultipleCalls_ReturnsSameInstance()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();
            var serviceProvider = services.BuildServiceProvider();

            // Act
            var instance1 = serviceProvider.GetService<ISpellCheckManager>();
            var instance2 = serviceProvider.GetService<ISpellCheckManager>();

            // Assert
            Assert.AreSame(instance1, instance2);
        }

        [TestMethod]
        public void AbbreviationsManager_MultipleCalls_ReturnsSameInstance()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();
            var serviceProvider = services.BuildServiceProvider();

            // Act
            var instance1 = serviceProvider.GetService<IAbbreviationsManager>();
            var instance2 = serviceProvider.GetService<IAbbreviationsManager>();

            // Assert
            Assert.AreSame(instance1, instance2);
        }

        [TestMethod]
        public void CommandManager_MultipleCalls_ReturnsSameInstance()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();
            var serviceProvider = services.BuildServiceProvider();

            // Act
            var instance1 = serviceProvider.GetService<ICommandManager>();
            var instance2 = serviceProvider.GetService<ICommandManager>();

            // Assert
            Assert.AreSame(instance1, instance2);
        }

        [TestMethod]
        public void AutomationEventManager_MultipleCalls_ReturnsSameInstance()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();
            var serviceProvider = services.BuildServiceProvider();

            // Act
            var instance1 = serviceProvider.GetService<IAutomationEventManager>();
            var instance2 = serviceProvider.GetService<IAutomationEventManager>();

            // Assert
            Assert.AreSame(instance1, instance2);
        }

        [TestMethod]
        public void Singletons_SharedAcrossNestedScopes()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();
            var rootProvider = services.BuildServiceProvider();

            // Act
            var rootInstance = rootProvider.GetService<IActuatorManager>();
            
            IActuatorManager scopedInstance;
            using (var scope = rootProvider.CreateScope())
            {
                scopedInstance = scope.ServiceProvider.GetService<IActuatorManager>();
            }

            // Assert - Singleton should be same across scopes
            Assert.AreSame(rootInstance, scopedInstance);
        }

        [TestMethod]
        public void MultipleServiceProviders_ShareGlobalSingletonInstances()
        {
            // Arrange
            var services1 = new ServiceCollection();
            services1.AddLogging();
            services1.AddACATServices();
            var provider1 = services1.BuildServiceProvider();

            var services2 = new ServiceCollection();
            services2.AddLogging();
            services2.AddACATServices();
            var provider2 = services2.BuildServiceProvider();

            // Act
            var instance1 = provider1.GetService<IActuatorManager>();
            var instance2 = provider2.GetService<IActuatorManager>();

            // Assert - Managers are global singletons, so they should be the same
            // even across different service providers (backed by Manager.Instance)
            Assert.AreSame(instance1, instance2);
        }

        [TestMethod]
        public void AllFactories_AreRegisteredAsSingletons()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();

            // Act - Get service descriptors for all factory interfaces
            var descriptors = services.Where(sd =>
                sd.ServiceType == typeof(IActuatorManagerFactory) ||
                sd.ServiceType == typeof(IAgentManagerFactory) ||
                sd.ServiceType == typeof(ITTSManagerFactory) ||
                sd.ServiceType == typeof(IPanelManagerFactory) ||
                sd.ServiceType == typeof(IThemeManagerFactory) ||
                sd.ServiceType == typeof(IWordPredictionManagerFactory) ||
                sd.ServiceType == typeof(ISpellCheckManagerFactory) ||
                sd.ServiceType == typeof(IAbbreviationsManagerFactory) ||
                sd.ServiceType == typeof(ICommandManagerFactory) ||
                sd.ServiceType == typeof(IAutomationEventManagerFactory)
            ).ToList();

            // Assert - All factories should be Singleton
            Assert.AreEqual(10, descriptors.Count, "Expected 10 factory interface registrations");
            foreach (var descriptor in descriptors)
            {
                Assert.AreEqual(ServiceLifetime.Singleton, descriptor.Lifetime,
                    $"{descriptor.ServiceType.Name} should be registered as Singleton");
            }
        }
    }
}
