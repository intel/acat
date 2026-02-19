////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.AgentManagement;
using ACAT.Core.Utility;
using ACAT.Integration.Tests.Harness;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ACAT.Integration.Tests.Tests
{
    /// <summary>
    /// Integration tests for agent activation, descriptor resolution, and
    /// lifecycle management.  These tests use the lightweight <see cref="NullAgent"/>
    /// so that no real UI or window-management is required.
    /// </summary>
    [TestClass]
    public class AgentActivationTests
    {
        private UITestHarness _harness;

        [TestInitialize]
        public void Setup()
        {
            _harness = new UITestHarness();
            _harness.Initialize(nameof(AgentActivationTests));
        }

        [TestCleanup]
        public void Cleanup()
        {
            _harness?.Dispose();
            _harness = null;
        }

        [TestMethod]
        public void AgentActivation_NullAgentCanBeInstantiated()
        {
            // Act
            var agent = new NullAgent();

            // Assert
            Assert.IsNotNull(agent, "NullAgent should be instantiatable without error.");
        }

        [TestMethod]
        public void AgentActivation_NullAgentHasDescriptor()
        {
            // Arrange
            var agent = new NullAgent();

            // Act
            ClassDescriptorAttribute descriptor = agent.Descriptor;

            // Assert
            Assert.IsNotNull(descriptor,
                "NullAgent should expose a non-null ClassDescriptorAttribute.");
        }

        [TestMethod]
        public void AgentActivation_NullAgentDescriptorHasExpectedId()
        {
            // Arrange
            var agent = new NullAgent();
            var expectedId = new Guid("92D2C512-DCAA-4773-8773-73E5D8C849FA");

            // Act
            Guid actualId = agent.Descriptor.Id;

            // Assert
            Assert.AreEqual(expectedId, actualId,
                "NullAgent descriptor should carry the well-known GUID.");
        }

        [TestMethod]
        public void AgentActivation_NullAgentSupportsNullAgentProcess()
        {
            // Arrange
            var agent = new NullAgent();

            // Act
            var supported = agent.ProcessesSupported;

            // Assert
            Assert.IsNotNull(supported,
                "ProcessesSupported should not return null.");

            bool hasNullAgentEntry = false;
            foreach (var process in supported)
            {
                if (process.ProcessName == "**nullagent**")
                {
                    hasNullAgentEntry = true;
                    break;
                }
            }

            Assert.IsTrue(hasNullAgentEntry,
                "NullAgent should list '**nullagent**' in its supported processes.");
        }

        [TestMethod]
        public void AgentActivation_NullAgentIsDisposable()
        {
            // Arrange & Act – disposal should not throw
            using (var agent = new NullAgent())
            {
                Assert.IsNotNull(agent);
            }
        }

        [TestMethod]
        public void AgentActivation_ServiceProviderAvailableDuringAgentTest()
        {
            // The harness injects a service provider into Context, so agents that
            // rely on it during initialisation can resolve their dependencies.
            Assert.IsNotNull(
                _harness.ServiceProvider,
                "Service provider should be available for agent activation tests.");
        }
    }
}
