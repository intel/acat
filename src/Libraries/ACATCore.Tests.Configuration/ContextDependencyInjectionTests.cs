////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ContextDependencyInjectionTests.cs
//
// Unit tests for Context class dependency injection functionality
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.ActuatorManagement;
using ACAT.Core.AgentManagement;
using ACAT.Core.PanelManagement;
using ACAT.Core.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ACATCore.Tests.Configuration
{
    [TestClass]
    public class ContextDependencyInjectionTests
    {
        [TestCleanup]
        public void Cleanup()
        {
            // Reset ServiceProvider after each test
            Context.ServiceProvider = null;
        }

        [TestMethod]
        public void GetManager_WithNullServiceProvider_ThrowsInvalidOperationException()
        {
            // Arrange
            Context.ServiceProvider = null;

            // Act & Assert
            try
            {
                Context.GetManager<IActuatorManager>();
                Assert.Fail("Expected InvalidOperationException was not thrown");
            }
            catch (InvalidOperationException)
            {
                // Expected exception
            }
        }

        [TestMethod]
        public void GetManager_WithConfiguredServiceProvider_ReturnsManager()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();
            Context.ServiceProvider = services.BuildServiceProvider();

            // Act
            var manager = Context.GetManager<IActuatorManager>();

            // Assert
            Assert.IsNotNull(manager);
            Assert.IsInstanceOfType(manager, typeof(IActuatorManager));
        }

        [TestMethod]
        public void GetManager_WithConfiguredServiceProvider_ReturnsSameInstanceAsDirect()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();
            var serviceProvider = services.BuildServiceProvider();
            Context.ServiceProvider = serviceProvider;

            // Act
            var managerFromContext = Context.GetManager<IActuatorManager>();
            var managerFromProvider = serviceProvider.GetService<IActuatorManager>();

            // Assert
            Assert.AreSame(managerFromContext, managerFromProvider);
        }

        [TestMethod]
        public void GetManager_MultipleManagers_AllResolvable()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();
            Context.ServiceProvider = services.BuildServiceProvider();

            // Act
            var actuatorManager = Context.GetManager<IActuatorManager>();
            var agentManager = Context.GetManager<IAgentManager>();
            var panelManager = Context.GetManager<IPanelManager>();

            // Assert
            Assert.IsNotNull(actuatorManager);
            Assert.IsNotNull(agentManager);
            Assert.IsNotNull(panelManager);
        }

        [TestMethod]
        public void GetManager_UnregisteredService_ReturnsNull()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            // Don't register ACAT services
            Context.ServiceProvider = services.BuildServiceProvider();

            // Act
            var manager = Context.GetManager<IActuatorManager>();

            // Assert
            Assert.IsNull(manager);
        }

        [TestMethod]
        public void ServiceProvider_CanBeSet_AndRetrieved()
        {
            // Arrange
            var services = new ServiceCollection();
            var serviceProvider = services.BuildServiceProvider();

            // Act
            Context.ServiceProvider = serviceProvider;

            // Assert
            Assert.AreSame(serviceProvider, Context.ServiceProvider);
        }

        [TestMethod]
        public void ServiceProvider_CanBeSetToNull()
        {
            // Arrange
            var services = new ServiceCollection();
            Context.ServiceProvider = services.BuildServiceProvider();

            // Act
            Context.ServiceProvider = null;

            // Assert
            Assert.IsNull(Context.ServiceProvider);
        }

        [TestMethod]
        public void IContext_ResolvedFromServiceProvider_IsNotNull()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();
            var serviceProvider = services.BuildServiceProvider();

            // Act
            var context = serviceProvider.GetService<IContext>();

            // Assert
            Assert.IsNotNull(context);
            Assert.IsInstanceOfType(context, typeof(IContext));
        }

        [TestMethod]
        public void IContext_ResolvingSetsStaticServiceProvider()
        {
            // Arrange
            Context.ServiceProvider = null;
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();
            var serviceProvider = services.BuildServiceProvider();

            // Act – resolving IContext should configure Context.ServiceProvider
            serviceProvider.GetRequiredService<IContext>();

            // Assert
            Assert.IsNotNull(Context.ServiceProvider);
            Assert.AreSame(serviceProvider, Context.ServiceProvider);
        }

        [TestMethod]
        public void Context_AndIContext_ResolveSameSingletonInstance()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();
            var serviceProvider = services.BuildServiceProvider();

            // Act
            var iContext = serviceProvider.GetRequiredService<IContext>();
            var context  = serviceProvider.GetRequiredService<Context>();

            // Assert – both registrations return the same singleton
            Assert.AreSame(iContext, context);
        }
    }
}
