////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ManagerMocks.cs
//
// Pre-configured Moq mock instances for ACAT manager interfaces.
// Use these mocks in unit tests to isolate the system under test from
// real manager implementations.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.ActuatorManagement;
using ACAT.Core.ActuatorManagement.Interfaces;
using ACAT.Core.AgentManagement;
using ACAT.Core.AgentManagement.Interfaces;
using ACAT.Core.Extensions;
using ACAT.Core.PanelManagement;
using ACAT.Core.PanelManagement.Interfaces;
using ACAT.Core.TTSManagement;
using ACAT.Core.TTSManagement.Interfaces;
using ACAT.Core.WordPredictorManagement;
using ACAT.Core.WordPredictorManagement.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;

namespace ACATCore.Tests.Mocks
{
    /// <summary>
    /// Pre-configured Moq mock instances for ACAT manager interfaces.
    /// Each property returns a new <see cref="Mock{T}"/> with sensible defaults
    /// so tests can be written with minimal setup boilerplate.
    /// </summary>
    public static class ManagerMocks
    {
        /// <summary>
        /// Creates a new <see cref="Mock{IPanelManager}"/> with commonly used
        /// members configured to return safe default values.
        /// </summary>
        public static Mock<IPanelManager> CreatePanelManager()
        {
            var mock = new Mock<IPanelManager>();

            mock.Setup(m => m.Init(It.IsAny<IEnumerable<string>>())).Returns(true);
            mock.Setup(m => m.Show(It.IsAny<IPanel>())).Returns(true);
            mock.Setup(m => m.Show(It.IsAny<IPanel>(), It.IsAny<IPanel>())).Returns(true);
            mock.Setup(m => m.ShowDialog(It.IsAny<IPanel>())).Returns(true);
            mock.Setup(m => m.ShowDialog(It.IsAny<IPanel>(), It.IsAny<IPanel>())).Returns(true);
            mock.Setup(m => m.ShowPopup(It.IsAny<IPanel>())).Returns(true);
            mock.Setup(m => m.ShowPopup(It.IsAny<IPanel>(), It.IsAny<IPanel>())).Returns(true);
            mock.Setup(m => m.IsCurrentPanelClass(It.IsAny<string>())).Returns(false);
            mock.Setup(m => m.GetCurrentPanelName()).Returns(string.Empty);
            mock.Setup(m => m.GetCurrentPanel()).Returns((IPanel)null);
            mock.Setup(m => m.GetCurrentForm()).Returns((IPanel)null);
            mock.Setup(m => m.CreatePanel(It.IsAny<string>())).Returns((System.Windows.Forms.Form)null);
            mock.Setup(m => m.CurrentForm).Returns((System.Windows.Forms.Form)null);

            return mock;
        }

        /// <summary>
        /// Creates a new <see cref="Mock{IAgentManager}"/> with commonly used
        /// members configured to return safe default values.
        /// </summary>
        public static Mock<IAgentManager> CreateAgentManager()
        {
            var mock = new Mock<IAgentManager>();

            mock.Setup(m => m.Init(It.IsAny<IEnumerable<string>>())).Returns(true);
            mock.Setup(m => m.PostInit()).Returns(true);
            mock.Setup(m => m.LoadExtensions(It.IsAny<IEnumerable<string>>())).Returns(true);
            mock.Setup(m => m.CanActivateFunctionalAgent()).Returns(false);
            mock.Setup(m => m.IsCurrentAgent(It.IsAny<string>())).Returns(false);
            mock.Setup(m => m.GetCurrentAgentName()).Returns(string.Empty);
            mock.Setup(m => m.GetAgentByName(It.IsAny<string>())).Returns((IApplicationAgent)null);
            mock.Setup(m => m.GetAgentByCategory(It.IsAny<string>())).Returns((IApplicationAgent)null);
            mock.Setup(m => m.GetFunctionalAgentByName(It.IsAny<string>())).Returns((IFunctionalAgent)null);
            mock.Setup(m => m.GetExtensions()).Returns(new List<object>());
            mock.Setup(m => m.ActiveAgent).Returns((IApplicationAgent)null);
            mock.Setup(m => m.EnableAppAgentContextSwitch).Returns(true);
            mock.Setup(m => m.EnableContextualMenusForDialogs).Returns(false);
            mock.Setup(m => m.EnableContextualMenusForMenus).Returns(false);

            return mock;
        }

        /// <summary>
        /// Creates a new <see cref="Mock{IActuatorManager}"/> with commonly used
        /// members configured to return safe default values.
        /// </summary>
        public static Mock<IActuatorManager> CreateActuatorManager()
        {
            var mock = new Mock<IActuatorManager>();

            mock.Setup(m => m.Init(It.IsAny<IEnumerable<string>>())).Returns(true);
            mock.Setup(m => m.PostInit()).Returns(true);
            mock.Setup(m => m.LoadExtensions(It.IsAny<IEnumerable<string>>(), It.IsAny<bool>())).Returns(true);
            mock.Setup(m => m.IsSwitchActive()).Returns(false);
            mock.Setup(m => m.RegisterSwitch(It.IsAny<IActuator>(), It.IsAny<ACAT.Core.ActuatorManagement.Settings.SwitchSetting>())).Returns(true);
            mock.Setup(m => m.UnregisterSwitch(It.IsAny<IActuator>(), It.IsAny<string>())).Returns(true);
            mock.Setup(m => m.GetActuator(It.IsAny<System.Type>())).Returns((IActuator)null);
            mock.Setup(m => m.GetActuator(It.IsAny<System.Guid>())).Returns((IActuator)null);
            mock.Setup(m => m.GetCalibrationSupportedActuator()).Returns((IActuator)null);
            mock.Setup(m => m.GetKeyboardActuator()).Returns((IActuator)null);
            mock.Setup(m => m.GetSwitchInterfaceActuator()).Returns((IActuator)null);
            mock.Setup(m => m.CheckScanTimingConfigureEnable()).Returns(false);
            mock.Setup(m => m.ActuatorsList).Returns(new List<IActuator>());

            return mock;
        }

        /// <summary>
        /// Creates a new <see cref="Mock{IWordPredictionManager}"/> with commonly used
        /// members configured to return safe default values.
        /// </summary>
        public static Mock<IWordPredictionManager> CreateWordPredictionManager()
        {
            var mock = new Mock<IWordPredictionManager>();

            mock.Setup(m => m.Init(It.IsAny<IEnumerable<string>>())).Returns(true);
            mock.Setup(m => m.PostInit()).Returns(true);
            mock.Setup(m => m.LoadExtensions(It.IsAny<IEnumerable<string>>())).Returns(true);
            mock.Setup(m => m.SetActiveWordPredictor(It.IsAny<System.Globalization.CultureInfo>())).Returns(true);
            mock.Setup(m => m.SwitchLanguage(It.IsAny<System.Globalization.CultureInfo>())).Returns(true);
            mock.Setup(m => m.ActiveWordPredictor).Returns((IWordPredictor)null);
            mock.Setup(m => m.WordPredictorsList).Returns(new List<IWordPredictor>());
            mock.Setup(m => m.WordPredictorExtensions).Returns(new List<System.Type>());
            mock.Setup(m => m.WordPredictorRootDirRelativeToProfile).Returns(string.Empty);
            mock.Setup(m => m.WordPredictorRootDirRelativeToUser).Returns(string.Empty);

            return mock;
        }

        /// <summary>
        /// Creates a new <see cref="Mock{ITTSManager}"/> with commonly used
        /// members configured to return safe default values.
        /// </summary>
        public static Mock<ITTSManager> CreateTTSManager()
        {
            var mock = new Mock<ITTSManager>();

            mock.Setup(m => m.Init(It.IsAny<IEnumerable<string>>())).Returns(true);
            mock.Setup(m => m.LoadExtensions(It.IsAny<IEnumerable<string>>())).Returns(true);
            mock.Setup(m => m.SetActiveEngine(It.IsAny<System.Globalization.CultureInfo>())).Returns(true);
            mock.Setup(m => m.SwitchLanguage(It.IsAny<System.Globalization.CultureInfo>())).Returns(true);
            mock.Setup(m => m.GetNormalizedVolume()).Returns(new TTSValue());
            mock.Setup(m => m.ActiveEngine).Returns((ITTSEngine)null);
            mock.Setup(m => m.TTSEnginesList).Returns(new List<ACAT.Core.Extensions.IExtension>());
            mock.Setup(m => m.GetExtensions()).Returns(new List<System.Type>());

            return mock;
        }

        /// <summary>
        /// Creates a new <see cref="Mock{ILogger}"/> suitable for verifying
        /// that log messages are written in tests.
        /// </summary>
        public static Mock<ILogger> CreateLogger()
        {
            return new Mock<ILogger>();
        }

        /// <summary>
        /// Creates a new <see cref="Mock{ILogger{T}}"/> for a specific category.
        /// </summary>
        public static Mock<ILogger<T>> CreateLogger<T>()
        {
            return new Mock<ILogger<T>>();
        }
    }
}
