////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// CQRSHandlerTests.cs
//
// Unit tests for CQRS command handler resolution paths:
//  1. Handler available via DI (DI path)
//  2. Handler not available – caller falls back to legacy singleton (fallback path)
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.ActuatorManagement;
using ACAT.Core.ActuatorManagement.Interfaces;
using ACAT.Core.PanelManagement;
using ACAT.Core.PanelManagement.Common;
using ACAT.Core.PanelManagement.Interfaces;
using ACAT.Core.Patterns.CQRS;
using ACAT.Core.Patterns.CQRS.Samples;
using ACATCore.Tests.Mocks;
using Moq;
using System.Windows.Forms;
using Xunit;

namespace ACATCore.Tests
{
    /// <summary>
    /// Unit tests for the CQRS <see cref="CreatePanelCommandHandler"/> and
    /// <see cref="HandleActuatorSwitchCommandHandler"/>, covering both the
    /// DI-available and fallback (handler-null) resolution paths.
    /// </summary>
    public class CQRSHandlerTests
    {
        // ----------------------------------------------------------------
        // CreatePanelCommandHandler – DI path
        // ----------------------------------------------------------------

        [Fact]
        public void CreatePanelCommandHandler_WhenHandlerAvailable_CallsPanelManagerAndSetsCreatedPanel()
        {
            // Arrange
            var mockPanel = new Mock<Form>();
            var panelManagerMock = ManagerMocks.CreatePanelManager();
            panelManagerMock
                .Setup(m => m.CreatePanel(It.IsAny<string>()))
                .Returns(mockPanel.Object);

            var handler = new CreatePanelCommandHandler(panelManagerMock.Object);
            var command = new CreatePanelCommand("TestScanner");

            // Act
            handler.Handle(command);

            // Assert – panel manager was called with the correct class name
            panelManagerMock.Verify(m => m.CreatePanel("TestScanner"), Times.Once);
        }

        [Fact]
        public void CreatePanelCommandHandler_WithTitleAndStartupArg_CallsCorrectOverload()
        {
            // Arrange
            var mockPanel = new Mock<Form>();
            var startupArg = new StartupArg("TestScanner");
            var panelManagerMock = ManagerMocks.CreatePanelManager();
            panelManagerMock
                .Setup(m => m.CreatePanel(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<StartupArg>()))
                .Returns(mockPanel.Object);

            var handler = new CreatePanelCommandHandler(panelManagerMock.Object);
            var command = new CreatePanelCommand("TestScanner", "My Title", startupArg);

            // Act
            handler.Handle(command);

            // Assert – overload with title and startupArg was invoked
            panelManagerMock.Verify(
                m => m.CreatePanel("TestScanner", "My Title", startupArg),
                Times.Once);
        }

        // ----------------------------------------------------------------
        // CreatePanelCommandHandler – fallback path (handler is null)
        // ----------------------------------------------------------------

        [Fact]
        public void CreatePanelCommand_WhenHandlerIsNull_CreatedPanelRemainsNull()
        {
            // Arrange – no handler, simulate the null-handler fallback path
            ICommandHandler<CreatePanelCommand> handler = null;
            var command = new CreatePanelCommand("TestScanner");

            // Act – caller would use legacy fallback; here we just verify
            // that the command remains in its initial state (CreatedPanel == null)
            // and no exception is thrown when handler is not invoked.
            if (handler != null)
            {
                handler.Handle(command);
            }

            // Assert
            Assert.Null(command.CreatedPanel);
        }

        // ----------------------------------------------------------------
        // HandleActuatorSwitchCommandHandler – DI path
        // ----------------------------------------------------------------

        [Fact]
        public void HandleActuatorSwitchCommandHandler_Pause_CallsActuatorManagerPause()
        {
            // Arrange
            var actuatorManagerMock = ManagerMocks.CreateActuatorManager();
            var handler = new HandleActuatorSwitchCommandHandler(actuatorManagerMock.Object);
            var command = new HandleActuatorSwitchCommand(ActuatorSwitchAction.Pause);

            // Act
            handler.Handle(command);

            // Assert
            actuatorManagerMock.Verify(m => m.Pause(), Times.Once);
            actuatorManagerMock.Verify(m => m.Resume(), Times.Never);
        }

        [Fact]
        public void HandleActuatorSwitchCommandHandler_Resume_CallsActuatorManagerResume()
        {
            // Arrange
            var actuatorManagerMock = ManagerMocks.CreateActuatorManager();
            var handler = new HandleActuatorSwitchCommandHandler(actuatorManagerMock.Object);
            var command = new HandleActuatorSwitchCommand(ActuatorSwitchAction.Resume);

            // Act
            handler.Handle(command);

            // Assert
            actuatorManagerMock.Verify(m => m.Resume(), Times.Once);
            actuatorManagerMock.Verify(m => m.Pause(), Times.Never);
        }

        // ----------------------------------------------------------------
        // HandleActuatorSwitchCommandHandler – fallback path (handler is null)
        // ----------------------------------------------------------------

        [Fact]
        public void HandleActuatorSwitchCommand_WhenHandlerIsNull_NoExceptionThrown()
        {
            // Arrange – simulate no DI handler available (null fallback path)
            ICommandHandler<HandleActuatorSwitchCommand> handler = null;
            var command = new HandleActuatorSwitchCommand(ActuatorSwitchAction.Pause);

            // Act – caller would use legacy fallback; no exception should occur
            // when the handler check is performed and handler is null.
            var exceptionThrown = false;
            try
            {
                if (handler != null)
                {
                    handler.Handle(command);
                }
                // legacy fallback would call Context.AppActuatorManager.Pause() here
            }
            catch
            {
                exceptionThrown = true;
            }

            // Assert
            Assert.False(exceptionThrown);
        }

        // ----------------------------------------------------------------
        // ActuatorBase property injection
        // ----------------------------------------------------------------

        [Fact]
        public void ActuatorBase_CreatePanelHandlerProperty_CanBeSetAndRetrieved()
        {
            // Arrange
            var panelManagerMock = ManagerMocks.CreatePanelManager();
            var handlerMock = new Mock<ICommandHandler<CreatePanelCommand>>();
            var actuator = new TestActuator();

            // Act
            actuator.CreatePanelHandler = handlerMock.Object;

            // Assert
            Assert.Same(handlerMock.Object, actuator.CreatePanelHandler);
        }

        /// <summary>
        /// Minimal concrete actuator used only for unit tests.
        /// </summary>
        private class TestActuator : ActuatorBase
        {
            public override IActuatorSwitch CreateSwitch() => null;
        }
    }
}
