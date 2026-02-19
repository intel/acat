////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ExtensionLoadingIntegrationTests.cs
//
// Integration tests for extension loading with dependency injection
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Extensions;
using ACAT.Core.PanelManagement;
using ACAT.Core.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ACATCore.Tests.Configuration
{
    [TestClass]
    public class ExtensionLoadingIntegrationTests
    {
        private IServiceProvider _serviceProvider;

        [TestInitialize]
        public void Setup()
        {
            // Set up a service provider with ACAT services
            var services = new ServiceCollection();
            services.AddACATInfrastructure();
            _serviceProvider = services.BuildServiceProvider();
            Context.ServiceProvider = _serviceProvider;
        }

        [TestCleanup]
        public void Cleanup()
        {
            Context.ServiceProvider = null;
        }

        [TestMethod]
        public void ExtensionInstantiator_WithServiceProvider_CanCreateExtensions()
        {
            // Arrange
            var logger = _serviceProvider.GetService<ILogger<ExtensionLoadingIntegrationTests>>();
            
            // Create a test extension type list (empty for this test)
            var extensionTypes = new List<Type>();

            // Act
            var extensions = ExtensionInstantiator.CreateExtensionInstances(
                _serviceProvider, 
                extensionTypes, 
                logger);

            // Assert
            Assert.IsNotNull(extensions);
            Assert.IsFalse(extensions.Any()); // Empty list since no types provided
        }

        [TestMethod]
        public void ExtensionInstantiator_WithNullServiceProvider_ThrowsArgumentNullException()
        {
            // Arrange
            var extensionTypes = new List<Type>();

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                ExtensionInstantiator.CreateExtensionInstances(null, extensionTypes));
        }

        [TestMethod]
        public void ExtensionInstantiator_WithNullExtensionTypes_ReturnsEmptyList()
        {
            // Arrange
            var logger = _serviceProvider.GetService<ILogger<ExtensionLoadingIntegrationTests>>();

            // Act
            var extensions = ExtensionInstantiator.CreateExtensionInstances(
                _serviceProvider,
                null,
                logger);

            // Assert
            Assert.IsNotNull(extensions);
            Assert.IsFalse(extensions.Any());
        }

        [TestMethod]
        public void ExtensionInstantiator_CreateSingleExtension_WithNullType_ReturnsNull()
        {
            // Arrange
            var logger = _serviceProvider.GetService<ILogger<ExtensionLoadingIntegrationTests>>();

            // Act
            var extension = ExtensionInstantiator.CreateExtensionInstance(
                _serviceProvider,
                null,
                logger);

            // Assert
            Assert.IsNull(extension);
        }

        [TestMethod]
        public void ServiceProvider_IsAvailableInContext_AfterSetup()
        {
            // Assert
            Assert.IsNotNull(Context.ServiceProvider);
            Assert.AreSame(_serviceProvider, Context.ServiceProvider);
        }

        [TestMethod]
        public void ServiceProvider_CanResolveLoggerFactory()
        {
            // Act
            var loggerFactory = _serviceProvider.GetService<ILoggerFactory>();

            // Assert
            Assert.IsNotNull(loggerFactory);
        }

        [TestMethod]
        public void ServiceProvider_CanCreateLoggers()
        {
            // Arrange
            var loggerFactory = _serviceProvider.GetService<ILoggerFactory>();

            // Act
            var logger = loggerFactory.CreateLogger<ExtensionLoadingIntegrationTests>();

            // Assert
            Assert.IsNotNull(logger);
        }
    }
}
