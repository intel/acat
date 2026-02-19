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
using MockFactory = ACATCore.Tests.Mocks.MockFactory;
using Xunit;
using Moq;
using System.Collections.Generic;
using System;

namespace ACATCore.Tests
{
    /// <summary>
    /// Demonstrates patterns for using <see cref="ManagerMocks"/> and
    /// <see cref="MockFactory"/> in xUnit unit tests.
    /// </summary>
    public class MockUsageTests
    {
        // ----------------------------------------------------------------
        // ManagerMocks – individual mock creation
        // ----------------------------------------------------------------

        [Fact]
        public void ManagerMocks_CreatePanelManager_ReturnsConfiguredMock()
        {
            var mock = ManagerMocks.CreatePanelManager();

            Assert.NotNull(mock);
            Assert.IsAssignableFrom<IPanelManager>(mock.Object);
        }

        [Fact]
        public void ManagerMocks_CreateAgentManager_ReturnsConfiguredMock()
        {
            var mock = ManagerMocks.CreateAgentManager();

            Assert.NotNull(mock);
            Assert.IsAssignableFrom<IAgentManager>(mock.Object);
        }

        [Fact]
        public void ManagerMocks_CreateActuatorManager_ReturnsConfiguredMock()
        {
            var mock = ManagerMocks.CreateActuatorManager();

            Assert.NotNull(mock);
            Assert.IsAssignableFrom<IActuatorManager>(mock.Object);
        }

        [Fact]
        public void ManagerMocks_CreateWordPredictionManager_ReturnsConfiguredMock()
        {
            var mock = ManagerMocks.CreateWordPredictionManager();

            Assert.NotNull(mock);
            Assert.IsAssignableFrom<IWordPredictionManager>(mock.Object);
        }

        [Fact]
        public void ManagerMocks_CreateTTSManager_ReturnsConfiguredMock()
        {
            var mock = ManagerMocks.CreateTTSManager();

            Assert.NotNull(mock);
            Assert.IsAssignableFrom<ITTSManager>(mock.Object);
        }

        [Fact]
        public void ManagerMocks_CreateLogger_ReturnsConfiguredMock()
        {
            var mock = ManagerMocks.CreateLogger();

            Assert.NotNull(mock);
        }

        // ----------------------------------------------------------------
        // MockFactory – bundle creation
        // ----------------------------------------------------------------

        [Fact]
        public void MockFactory_CreateManagerBundle_AllMocksPopulated()
        {
            var bundle = MockFactory.CreateManagerBundle();

            Assert.NotNull(bundle.PanelManager);
            Assert.NotNull(bundle.AgentManager);
            Assert.NotNull(bundle.ActuatorManager);
            Assert.NotNull(bundle.WordPredictionManager);
            Assert.NotNull(bundle.TTSManager);
            Assert.NotNull(bundle.Logger);
        }

        [Fact]
        public void MockFactory_CreateManagerBundle_EachCallReturnsDistinctInstances()
        {
            var bundle1 = MockFactory.CreateManagerBundle();
            var bundle2 = MockFactory.CreateManagerBundle();

            Assert.NotSame(bundle1.PanelManager, bundle2.PanelManager);
            Assert.NotSame(bundle1.AgentManager, bundle2.AgentManager);
            Assert.NotSame(bundle1.ActuatorManager, bundle2.ActuatorManager);
            Assert.NotSame(bundle1.WordPredictionManager, bundle2.WordPredictionManager);
            Assert.NotSame(bundle1.TTSManager, bundle2.TTSManager);
        }

        // ----------------------------------------------------------------
        // Default return values
        // ----------------------------------------------------------------

        [Fact]
        public void PanelManagerMock_Init_ReturnsTrueByDefault()
        {
            var mock = ManagerMocks.CreatePanelManager();

            bool result = mock.Object.Init(new List<string>());

            Assert.True(result);
        }

        [Fact]
        public void AgentManagerMock_Init_ReturnsTrueByDefault()
        {
            var mock = ManagerMocks.CreateAgentManager();

            bool result = mock.Object.Init(new List<string>());

            Assert.True(result);
        }

        [Fact]
        public void ActuatorManagerMock_Init_ReturnsTrueByDefault()
        {
            var mock = ManagerMocks.CreateActuatorManager();

            bool result = mock.Object.Init(new List<string>());

            Assert.True(result);
        }

        [Fact]
        public void WordPredictionManagerMock_Init_ReturnsTrueByDefault()
        {
            var mock = ManagerMocks.CreateWordPredictionManager();

            bool result = mock.Object.Init(new List<string>());

            Assert.True(result);
        }

        [Fact]
        public void TTSManagerMock_Init_ReturnsTrueByDefault()
        {
            var mock = ManagerMocks.CreateTTSManager();

            bool result = mock.Object.Init(new List<string>());

            Assert.True(result);
        }

        [Fact]
        public void ActuatorManagerMock_IsSwitchActive_ReturnsFalseByDefault()
        {
            var mock = ManagerMocks.CreateActuatorManager();

            Assert.False(mock.Object.IsSwitchActive());
        }

        [Fact]
        public void AgentManagerMock_CanActivateFunctionalAgent_ReturnsFalseByDefault()
        {
            var mock = ManagerMocks.CreateAgentManager();

            Assert.False(mock.Object.CanActivateFunctionalAgent());
        }

        // ----------------------------------------------------------------
        // Customising mocks in tests
        // ----------------------------------------------------------------

        [Fact]
        public void PanelManagerMock_GetCurrentPanelName_CanBeOverridden()
        {
            var mock = ManagerMocks.CreatePanelManager();
            mock.Setup(m => m.GetCurrentPanelName()).Returns("MyTestPanel");

            string name = mock.Object.GetCurrentPanelName();

            Assert.Equal("MyTestPanel", name);
        }

        [Fact]
        public void AgentManagerMock_IsCurrentAgent_CanBeOverridden()
        {
            var mock = ManagerMocks.CreateAgentManager();
            mock.Setup(m => m.IsCurrentAgent("NotepadAgent")).Returns(true);

            Assert.True(mock.Object.IsCurrentAgent("NotepadAgent"));
            Assert.False(mock.Object.IsCurrentAgent("OtherAgent"));
        }

        [Fact]
        public void TTSManagerMock_SetActiveEngine_CanBeOverridden()
        {
            var mock = ManagerMocks.CreateTTSManager();
            mock.Setup(m => m.SetActiveEngine(null)).Returns(false);

            bool result = mock.Object.SetActiveEngine(null);

            Assert.False(result);
        }

        // ----------------------------------------------------------------
        // WithInitResult extension methods
        // ----------------------------------------------------------------

        [Fact]
        public void MockFactory_WithInitResult_PanelManager_ConfiguresReturn()
        {
            var mock = MockFactory.CreatePanelManager().WithInitResult(false);

            bool result = mock.Object.Init(new List<string>());

            Assert.False(result);
        }

        [Fact]
        public void MockFactory_WithInitResult_AgentManager_ConfiguresReturn()
        {
            var mock = MockFactory.CreateAgentManager().WithInitResult(false);

            bool result = mock.Object.Init(new List<string>());

            Assert.False(result);
        }

        [Fact]
        public void MockFactory_WithInitResult_ActuatorManager_ConfiguresReturn()
        {
            var mock = MockFactory.CreateActuatorManager().WithInitResult(false);

            bool result = mock.Object.Init(new List<string>());

            Assert.False(result);
        }

        [Fact]
        public void MockFactory_WithInitResult_WordPredictionManager_ConfiguresReturn()
        {
            var mock = MockFactory.CreateWordPredictionManager().WithInitResult(false);

            bool result = mock.Object.Init(new List<string>());

            Assert.False(result);
        }

        [Fact]
        public void MockFactory_WithInitResult_TTSManager_ConfiguresReturn()
        {
            var mock = MockFactory.CreateTTSManager().WithInitResult(false);

            bool result = mock.Object.Init(new List<string>());

            Assert.False(result);
        }

        // ----------------------------------------------------------------
        // Verification – asserting that methods were called
        // ----------------------------------------------------------------

        [Fact]
        public void PanelManagerMock_CloseCurrentPanel_CanBeVerified()
        {
            var mock = ManagerMocks.CreatePanelManager();

            mock.Object.CloseCurrentPanel();

            mock.Verify(m => m.CloseCurrentPanel(), Times.Once);
        }

        [Fact]
        public void ActuatorManagerMock_Pause_Resume_CanBeVerified()
        {
            var mock = ManagerMocks.CreateActuatorManager();

            mock.Object.Pause();
            mock.Object.Resume();

            mock.Verify(m => m.Pause(), Times.Once);
            mock.Verify(m => m.Resume(), Times.Once);
        }

        [Fact]
        public void AgentManagerMock_RunCommand_CanBeVerified()
        {
            var mock = ManagerMocks.CreateAgentManager();
            bool handled = false;

            mock.Object.RunCommand("SomeCommand", ref handled);

            mock.Verify(m => m.RunCommand(It.IsAny<string>(), ref It.Ref<bool>.IsAny), Times.Once);
        }

        [Fact]
        public void WordPredictionManagerMock_SaveSettings_CanBeVerified()
        {
            var mock = ManagerMocks.CreateWordPredictionManager();

            mock.Object.SaveSettings();

            mock.Verify(m => m.SaveSettings(), Times.Once);
        }

        // ----------------------------------------------------------------
        // Strict mocks
        // ----------------------------------------------------------------

        [Fact]
        public void MockFactory_CreateStrictPanelManager_ThrowsOnUnsetupCall()
        {
            var mock = MockFactory.CreateStrictPanelManager();
            mock.Setup(m => m.GetCurrentPanelName()).Returns("StrictPanel");

            // Calling a set-up method should succeed
            string name = mock.Object.GetCurrentPanelName();
            Assert.Equal("StrictPanel", name);
        }

        [Fact]
        public void MockFactory_CreateStrictAgentManager_ThrowsOnUnsetupCall()
        {
            var mock = MockFactory.CreateStrictAgentManager();

            // No setup for Init – should throw MockException
            Assert.Throws<MockException>(() => mock.Object.Init(new List<string>()));
        }
    }
}
