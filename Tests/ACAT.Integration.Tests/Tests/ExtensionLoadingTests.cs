////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Extensions;
using ACAT.Core.PanelManagement;
using ACAT.Core.Utility;
using ACAT.Integration.Tests.Harness;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ACAT.Integration.Tests.Tests
{
    /// <summary>
    /// Integration tests for extension loading via the ACAT
    /// <see cref="ExtensionInstantiator"/> and the DI service provider.
    /// </summary>
    [TestClass]
    public class ExtensionLoadingTests
    {
        private UITestHarness _harness;

        [TestInitialize]
        public void Setup()
        {
            _harness = new UITestHarness();
            _harness.Initialize(nameof(ExtensionLoadingTests));
        }

        [TestCleanup]
        public void Cleanup()
        {
            _harness?.Dispose();
            _harness = null;
        }

        [TestMethod]
        public void ExtensionLoading_ServiceProviderIsConfigured()
        {
            Assert.IsNotNull(
                _harness.ServiceProvider,
                "Service provider must be available to load extensions.");
        }

        [TestMethod]
        public void ExtensionLoading_LoggerFactoryIsResolvable()
        {
            // Act
            var loggerFactory = _harness.ServiceProvider.GetService<ILoggerFactory>();

            // Assert
            Assert.IsNotNull(loggerFactory,
                "ILoggerFactory should be registered in the ACAT service provider.");
        }

        [TestMethod]
        public void ExtensionLoading_EmptyTypeListReturnsEmptyCollection()
        {
            // Arrange
            var emptyTypes = new List<Type>();

            // Act
            var extensions = ExtensionInstantiator.CreateExtensionInstances(
                _harness.ServiceProvider,
                emptyTypes);

            // Assert
            Assert.IsNotNull(extensions);
            Assert.IsFalse(extensions.Any(),
                "An empty type list should produce an empty extension collection.");
        }

        [TestMethod]
        public void ExtensionLoading_NullTypeListReturnsEmptyCollection()
        {
            // Act
            var extensions = ExtensionInstantiator.CreateExtensionInstances(
                _harness.ServiceProvider,
                null);

            // Assert
            Assert.IsNotNull(extensions);
            Assert.IsFalse(extensions.Any(),
                "A null type list should produce an empty extension collection.");
        }

        [TestMethod]
        public void ExtensionLoading_NullServiceProviderThrows()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                ExtensionInstantiator.CreateExtensionInstances(null, new List<Type>()));
        }

        [TestMethod]
        public void ExtensionLoading_SingleNullTypeReturnsNull()
        {
            // Act
            var extension = ExtensionInstantiator.CreateExtensionInstance(
                _harness.ServiceProvider,
                null);

            // Assert
            Assert.IsNull(extension,
                "Requesting an extension for a null type should return null.");
        }

        [TestMethod]
        public void ExtensionLoading_ContextServiceProviderMatchesHarness()
        {
            // The harness sets Context.ServiceProvider; verify the round-trip.
            Assert.AreSame(
                _harness.ServiceProvider,
                Context.ServiceProvider,
                "Context.ServiceProvider should reference the harness service provider.");
        }
    }
}
