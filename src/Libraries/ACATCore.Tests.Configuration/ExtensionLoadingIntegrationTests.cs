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
using ACAT.Core.DependencyInjection;
using ACAT.Core.Utility;
using ACAT.Core.Utility.TypeLoader;
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

        // ---------------------------------------------------------------
        // Minimal fake extension used only in ExtensionLoader tests
        // ---------------------------------------------------------------

        private interface IFakeExtension : IPluginExtension { }

        // ---------------------------------------------------------------
        // Existing ExtensionInstantiator tests (unchanged)
        // ---------------------------------------------------------------

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
            try
            {
                ExtensionInstantiator.CreateExtensionInstances(null, extensionTypes);
                Assert.Fail("Expected ArgumentNullException was not thrown");
            }
            catch (ArgumentNullException)
            {
                // Expected exception
            }
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

        // ---------------------------------------------------------------
        // ExtensionLoader tests
        // ---------------------------------------------------------------

        [TestMethod]
        public void ExtensionLoader_Constructor_WithNullServiceProvider_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(
                () => new ExtensionLoader<IFakeExtension>(null));
        }

        [TestMethod]
        public void ExtensionLoader_Constructor_WithServiceProvider_Succeeds()
        {
            // Act
            var loader = new ExtensionLoader<IFakeExtension>(_serviceProvider);

            // Assert
            Assert.IsNotNull(loader);
        }

        [TestMethod]
        public void ExtensionLoader_LoadedTypes_IsEmptyBeforeLoadingAssemblies()
        {
            // Arrange
            var loader = new ExtensionLoader<IFakeExtension>(_serviceProvider);

            // Assert
            Assert.IsNotNull(loader.LoadedTypes);
            Assert.AreEqual(0, loader.LoadedTypes.Count);
        }

        [TestMethod]
        public void ExtensionLoader_CreateInstance_WithUnknownGuid_ReturnsNull()
        {
            // Arrange
            var loader = new ExtensionLoader<IFakeExtension>(_serviceProvider);
            var unknownId = Guid.NewGuid();

            // Act
            var instance = loader.CreateInstance(unknownId);

            // Assert
            Assert.IsNull(instance);
        }

        [TestMethod]
        public void ExtensionLoader_CreateAllInstances_WithNoTypesLoaded_ReturnsEmptyCollection()
        {
            // Arrange
            var loader = new ExtensionLoader<IFakeExtension>(_serviceProvider);

            // Act
            var instances = loader.CreateAllInstances();

            // Assert
            Assert.IsNotNull(instances);
            Assert.IsFalse(instances.Any());
        }

        [TestMethod]
        public void ExtensionLoader_RegisterExtensions_WithNullServices_ThrowsArgumentNullException()
        {
            // Arrange
            var loader = new ExtensionLoader<IFakeExtension>(_serviceProvider);

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(
                () => loader.RegisterExtensions(null));
        }

        [TestMethod]
        public void ExtensionLoader_RegisterExtensions_WithNoLoadedTypes_LeavesServicesUnchanged()
        {
            // Arrange
            var loader = new ExtensionLoader<IFakeExtension>(_serviceProvider);
            var services = new ServiceCollection();
            var initialCount = services.Count;

            // Act
            loader.RegisterExtensions(services);

            // Assert – no new registrations because no types were loaded
            Assert.AreEqual(initialCount, services.Count);
        }

        [TestMethod]
        public void ExtensionLoader_AddExtensionLoader_RegistersIExtensionLoaderAsSingleton()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddExtensionLoader<IFakeExtension>();
            var provider = services.BuildServiceProvider();

            // Act
            var loader1 = provider.GetService<IExtensionLoader<IFakeExtension>>();
            var loader2 = provider.GetService<IExtensionLoader<IFakeExtension>>();

            // Assert – resolved as singleton (same reference both times)
            Assert.IsNotNull(loader1);
            Assert.AreSame(loader1, loader2);
        }

        [TestMethod]
        public void ExtensionLoader_ImplementsIExtensionLoader()
        {
            // Act
            var loader = new ExtensionLoader<IFakeExtension>(_serviceProvider);

            // Assert
            Assert.IsInstanceOfType(loader, typeof(IExtensionLoader<IFakeExtension>));
        }
    }
}
