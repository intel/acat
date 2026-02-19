////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// MockUsageTests.cs
//
// Sample unit tests demonstrating how to use the ManagerMocks and
// MockFactory helper classes with Moq.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.ActuatorManagement;
using ACAT.Core.AgentManagement;
using ACAT.Core.PanelManagement;
using ACAT.Core.TTSManagement;
using ACAT.Core.WordPredictorManagement;
using ACATCore.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;

namespace ACATCore.Tests
{
    /// <summary>
    /// Demonstrates patterns for using <see cref="ManagerMocks"/> and
    /// <see cref="MockFactory"/> in MSTest unit tests.
    /// </summary>
    [TestClass]
    public class MockUsageTests
    {
        // ----------------------------------------------------------------
        // ManagerMocks – individual mock creation
        // ----------------------------------------------------------------

        [TestMethod]
        public void ManagerMocks_CreatePanelManager_ReturnsConfiguredMock()
        {
            var mock = ManagerMocks.CreatePanelManager();

            Assert.IsNotNull(mock);
            Assert.IsInstanceOfType(mock.Object, typeof(IPanelManager));
        }

        [TestMethod]
        public void ManagerMocks_CreateAgentManager_ReturnsConfiguredMock()
        {
            var mock = ManagerMocks.CreateAgentManager();

            Assert.IsNotNull(mock);
            Assert.IsInstanceOfType(mock.Object, typeof(IAgentManager));
        }

        [TestMethod]
        public void ManagerMocks_CreateActuatorManager_ReturnsConfiguredMock()
        {
            var mock = ManagerMocks.CreateActuatorManager();

            Assert.IsNotNull(mock);
            Assert.IsInstanceOfType(mock.Object, typeof(IActuatorManager));
        }

        [TestMethod]
        public void ManagerMocks_CreateWordPredictionManager_ReturnsConfiguredMock()
        {
            var mock = ManagerMocks.CreateWordPredictionManager();

            Assert.IsNotNull(mock);
            Assert.IsInstanceOfType(mock.Object, typeof(IWordPredictionManager));
        }

        [TestMethod]
        public void ManagerMocks_CreateTTSManager_ReturnsConfiguredMock()
        {
            var mock = ManagerMocks.CreateTTSManager();

            Assert.IsNotNull(mock);
            Assert.IsInstanceOfType(mock.Object, typeof(ITTSManager));
        }

        [TestMethod]
        public void ManagerMocks_CreateLogger_ReturnsConfiguredMock()
        {
            var mock = ManagerMocks.CreateLogger();

            Assert.IsNotNull(mock);
        }

        // ----------------------------------------------------------------
        // MockFactory – bundle creation
        // ----------------------------------------------------------------

        [TestMethod]
        public void MockFactory_CreateManagerBundle_AllMocksPopulated()
        {
            var bundle = MockFactory.CreateManagerBundle();

            Assert.IsNotNull(bundle.PanelManager);
            Assert.IsNotNull(bundle.AgentManager);
            Assert.IsNotNull(bundle.ActuatorManager);
            Assert.IsNotNull(bundle.WordPredictionManager);
            Assert.IsNotNull(bundle.TTSManager);
            Assert.IsNotNull(bundle.Logger);
        }

        [TestMethod]
        public void MockFactory_CreateManagerBundle_EachCallReturnsDistinctInstances()
        {
            var bundle1 = MockFactory.CreateManagerBundle();
            var bundle2 = MockFactory.CreateManagerBundle();

            Assert.AreNotSame(bundle1.PanelManager, bundle2.PanelManager);
            Assert.AreNotSame(bundle1.AgentManager, bundle2.AgentManager);
            Assert.AreNotSame(bundle1.ActuatorManager, bundle2.ActuatorManager);
            Assert.AreNotSame(bundle1.WordPredictionManager, bundle2.WordPredictionManager);
            Assert.AreNotSame(bundle1.TTSManager, bundle2.TTSManager);
        }

        // ----------------------------------------------------------------
        // Default return values
        // ----------------------------------------------------------------

        [TestMethod]
        public void PanelManagerMock_Init_ReturnsTrueByDefault()
        {
            var mock = ManagerMocks.CreatePanelManager();

            bool result = mock.Object.Init(new List<string>());

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AgentManagerMock_Init_ReturnsTrueByDefault()
        {
            var mock = ManagerMocks.CreateAgentManager();

            bool result = mock.Object.Init(new List<string>());

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ActuatorManagerMock_Init_ReturnsTrueByDefault()
        {
            var mock = ManagerMocks.CreateActuatorManager();

            bool result = mock.Object.Init(new List<string>());

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void WordPredictionManagerMock_Init_ReturnsTrueByDefault()
        {
            var mock = ManagerMocks.CreateWordPredictionManager();

            bool result = mock.Object.Init(new List<string>());

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TTSManagerMock_Init_ReturnsTrueByDefault()
        {
            var mock = ManagerMocks.CreateTTSManager();

            bool result = mock.Object.Init(new List<string>());

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ActuatorManagerMock_IsSwitchActive_ReturnsFalseByDefault()
        {
            var mock = ManagerMocks.CreateActuatorManager();

            Assert.IsFalse(mock.Object.IsSwitchActive());
        }

        [TestMethod]
        public void AgentManagerMock_CanActivateFunctionalAgent_ReturnsFalseByDefault()
        {
            var mock = ManagerMocks.CreateAgentManager();

            Assert.IsFalse(mock.Object.CanActivateFunctionalAgent());
        }

        // ----------------------------------------------------------------
        // Customising mocks in tests
        // ----------------------------------------------------------------

        [TestMethod]
        public void PanelManagerMock_GetCurrentPanelName_CanBeOverridden()
        {
            var mock = ManagerMocks.CreatePanelManager();
            mock.Setup(m => m.GetCurrentPanelName()).Returns("MyTestPanel");

            string name = mock.Object.GetCurrentPanelName();

            Assert.AreEqual("MyTestPanel", name);
        }

        [TestMethod]
        public void AgentManagerMock_IsCurrentAgent_CanBeOverridden()
        {
            var mock = ManagerMocks.CreateAgentManager();
            mock.Setup(m => m.IsCurrentAgent("NotepadAgent")).Returns(true);

            Assert.IsTrue(mock.Object.IsCurrentAgent("NotepadAgent"));
            Assert.IsFalse(mock.Object.IsCurrentAgent("OtherAgent"));
        }

        [TestMethod]
        public void TTSManagerMock_SetActiveEngine_CanBeOverridden()
        {
            var mock = ManagerMocks.CreateTTSManager();
            mock.Setup(m => m.SetActiveEngine(null)).Returns(false);

            bool result = mock.Object.SetActiveEngine(null);

            Assert.IsFalse(result);
        }

        // ----------------------------------------------------------------
        // WithInitResult extension methods
        // ----------------------------------------------------------------

        [TestMethod]
        public void MockFactory_WithInitResult_PanelManager_ConfiguresReturn()
        {
            var mock = MockFactory.CreatePanelManager().WithInitResult(false);

            bool result = mock.Object.Init(new List<string>());

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MockFactory_WithInitResult_AgentManager_ConfiguresReturn()
        {
            var mock = MockFactory.CreateAgentManager().WithInitResult(false);

            bool result = mock.Object.Init(new List<string>());

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MockFactory_WithInitResult_ActuatorManager_ConfiguresReturn()
        {
            var mock = MockFactory.CreateActuatorManager().WithInitResult(false);

            bool result = mock.Object.Init(new List<string>());

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MockFactory_WithInitResult_WordPredictionManager_ConfiguresReturn()
        {
            var mock = MockFactory.CreateWordPredictionManager().WithInitResult(false);

            bool result = mock.Object.Init(new List<string>());

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MockFactory_WithInitResult_TTSManager_ConfiguresReturn()
        {
            var mock = MockFactory.CreateTTSManager().WithInitResult(false);

            bool result = mock.Object.Init(new List<string>());

            Assert.IsFalse(result);
        }

        // ----------------------------------------------------------------
        // Verification – asserting that methods were called
        // ----------------------------------------------------------------

        [TestMethod]
        public void PanelManagerMock_CloseCurrentPanel_CanBeVerified()
        {
            var mock = ManagerMocks.CreatePanelManager();

            mock.Object.CloseCurrentPanel();

            mock.Verify(m => m.CloseCurrentPanel(), Times.Once);
        }

        [TestMethod]
        public void ActuatorManagerMock_Pause_Resume_CanBeVerified()
        {
            var mock = ManagerMocks.CreateActuatorManager();

            mock.Object.Pause();
            mock.Object.Resume();

            mock.Verify(m => m.Pause(), Times.Once);
            mock.Verify(m => m.Resume(), Times.Once);
        }

        [TestMethod]
        public void AgentManagerMock_RunCommand_CanBeVerified()
        {
            var mock = ManagerMocks.CreateAgentManager();
            bool handled = false;

            mock.Object.RunCommand("SomeCommand", ref handled);

            mock.Verify(m => m.RunCommand(It.IsAny<string>(), ref It.Ref<bool>.IsAny), Times.Once);
        }

        [TestMethod]
        public void WordPredictionManagerMock_SaveSettings_CanBeVerified()
        {
            var mock = ManagerMocks.CreateWordPredictionManager();

            mock.Object.SaveSettings();

            mock.Verify(m => m.SaveSettings(), Times.Once);
        }

        // ----------------------------------------------------------------
        // Strict mocks
        // ----------------------------------------------------------------

        [TestMethod]
        public void MockFactory_CreateStrictPanelManager_ThrowsOnUnsetupCall()
        {
            var mock = MockFactory.CreateStrictPanelManager();
            mock.Setup(m => m.GetCurrentPanelName()).Returns("StrictPanel");

            // Calling a set-up method should succeed
            string name = mock.Object.GetCurrentPanelName();
            Assert.AreEqual("StrictPanel", name);
        }

        [TestMethod]
        [ExpectedException(typeof(MockException))]
        public void MockFactory_CreateStrictAgentManager_ThrowsOnUnsetupCall()
        {
            var mock = MockFactory.CreateStrictAgentManager();

            // No setup for Init – should throw MockException
            mock.Object.Init(new List<string>());
        }
    }
}
