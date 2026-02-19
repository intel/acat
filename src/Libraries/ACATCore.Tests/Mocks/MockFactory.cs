////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// MockFactory.cs
//
// Factory class for creating complex mock setups used in ACAT unit tests.
// Provides pre-configured mock bundles and fluent configuration helpers.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.ActuatorManagement;
using ACAT.Core.AgentManagement;
using ACAT.Core.PanelManagement;
using ACAT.Core.TTSManagement;
using ACAT.Core.WordPredictorManagement;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;

namespace ACATCore.Tests.Mocks
{
    /// <summary>
    /// A bundle of all manager mocks, created together so that tests can
    /// access every mock from a single object returned by
    /// <see cref="MockFactory.CreateManagerBundle"/>.
    /// </summary>
    public class ManagerMockBundle
    {
        /// <summary>Gets the mock for <see cref="IPanelManager"/>.</summary>
        public Mock<IPanelManager> PanelManager { get; }

        /// <summary>Gets the mock for <see cref="IAgentManager"/>.</summary>
        public Mock<IAgentManager> AgentManager { get; }

        /// <summary>Gets the mock for <see cref="IActuatorManager"/>.</summary>
        public Mock<IActuatorManager> ActuatorManager { get; }

        /// <summary>Gets the mock for <see cref="IWordPredictionManager"/>.</summary>
        public Mock<IWordPredictionManager> WordPredictionManager { get; }

        /// <summary>Gets the mock for <see cref="ITTSManager"/>.</summary>
        public Mock<ITTSManager> TTSManager { get; }

        /// <summary>Gets the mock for <see cref="ILogger"/>.</summary>
        public Mock<ILogger> Logger { get; }

        internal ManagerMockBundle(
            Mock<IPanelManager> panelManager,
            Mock<IAgentManager> agentManager,
            Mock<IActuatorManager> actuatorManager,
            Mock<IWordPredictionManager> wordPredictionManager,
            Mock<ITTSManager> ttsManager,
            Mock<ILogger> logger)
        {
            PanelManager = panelManager;
            AgentManager = agentManager;
            ActuatorManager = actuatorManager;
            WordPredictionManager = wordPredictionManager;
            TTSManager = ttsManager;
            Logger = logger;
        }
    }

    /// <summary>
    /// Factory that produces Moq mock objects and pre-configured bundles for
    /// ACAT manager interfaces.  Use <see cref="CreateManagerBundle"/> to get
    /// all mocks at once, or the individual <c>Create*</c> methods for a
    /// single mock with custom additional setup.
    /// </summary>
    /// <example>
    /// <code>
    /// // Get all mocks in one call
    /// var bundle = MockFactory.CreateManagerBundle();
    /// bundle.PanelManager.Setup(m => m.Init(It.IsAny&lt;IEnumerable&lt;string&gt;&gt;())).Returns(true);
    ///
    /// // Or create individual mocks
    /// var panelManagerMock = MockFactory.CreatePanelManager();
    /// panelManagerMock.Setup(m => m.GetCurrentPanelName()).Returns("TestPanel");
    /// </code>
    /// </example>
    public static class MockFactory
    {
        /// <summary>
        /// Creates a <see cref="ManagerMockBundle"/> containing pre-configured
        /// mocks for every ACAT manager interface.
        /// </summary>
        public static ManagerMockBundle CreateManagerBundle()
        {
            return new ManagerMockBundle(
                ManagerMocks.CreatePanelManager(),
                ManagerMocks.CreateAgentManager(),
                ManagerMocks.CreateActuatorManager(),
                ManagerMocks.CreateWordPredictionManager(),
                ManagerMocks.CreateTTSManager(),
                ManagerMocks.CreateLogger());
        }

        /// <summary>
        /// Creates a <see cref="Mock{IPanelManager}"/> with default setups.
        /// </summary>
        public static Mock<IPanelManager> CreatePanelManager() =>
            ManagerMocks.CreatePanelManager();

        /// <summary>
        /// Creates a <see cref="Mock{IAgentManager}"/> with default setups.
        /// </summary>
        public static Mock<IAgentManager> CreateAgentManager() =>
            ManagerMocks.CreateAgentManager();

        /// <summary>
        /// Creates a <see cref="Mock{IActuatorManager}"/> with default setups.
        /// </summary>
        public static Mock<IActuatorManager> CreateActuatorManager() =>
            ManagerMocks.CreateActuatorManager();

        /// <summary>
        /// Creates a <see cref="Mock{IWordPredictionManager}"/> with default setups.
        /// </summary>
        public static Mock<IWordPredictionManager> CreateWordPredictionManager() =>
            ManagerMocks.CreateWordPredictionManager();

        /// <summary>
        /// Creates a <see cref="Mock{ITTSManager}"/> with default setups.
        /// </summary>
        public static Mock<ITTSManager> CreateTTSManager() =>
            ManagerMocks.CreateTTSManager();

        /// <summary>
        /// Creates a <see cref="Mock{ILogger}"/> for verifying log output.
        /// </summary>
        public static Mock<ILogger> CreateLogger() =>
            ManagerMocks.CreateLogger();

        /// <summary>
        /// Creates a <see cref="Mock{ILogger{T}}"/> for a specific category type.
        /// </summary>
        public static Mock<ILogger<T>> CreateLogger<T>() =>
            ManagerMocks.CreateLogger<T>();

        /// <summary>
        /// Creates a strict <see cref="Mock{IPanelManager}"/> (all un-setup calls throw).
        /// Use when you want to ensure only expected members are called.
        /// </summary>
        public static Mock<IPanelManager> CreateStrictPanelManager() =>
            new Mock<IPanelManager>(MockBehavior.Strict);

        /// <summary>
        /// Creates a strict <see cref="Mock{IAgentManager}"/> (all un-setup calls throw).
        /// </summary>
        public static Mock<IAgentManager> CreateStrictAgentManager() =>
            new Mock<IAgentManager>(MockBehavior.Strict);

        /// <summary>
        /// Creates a strict <see cref="Mock{IActuatorManager}"/> (all un-setup calls throw).
        /// </summary>
        public static Mock<IActuatorManager> CreateStrictActuatorManager() =>
            new Mock<IActuatorManager>(MockBehavior.Strict);

        /// <summary>
        /// Creates a strict <see cref="Mock{IWordPredictionManager}"/> (all un-setup calls throw).
        /// </summary>
        public static Mock<IWordPredictionManager> CreateStrictWordPredictionManager() =>
            new Mock<IWordPredictionManager>(MockBehavior.Strict);

        /// <summary>
        /// Creates a strict <see cref="Mock{ITTSManager}"/> (all un-setup calls throw).
        /// </summary>
        public static Mock<ITTSManager> CreateStrictTTSManager() =>
            new Mock<ITTSManager>(MockBehavior.Strict);

        /// <summary>
        /// Configures <paramref name="mock"/> so that
        /// <see cref="IPanelManager.Init"/> returns <paramref name="result"/>
        /// and the mock is ready for use as an already-initialized panel manager.
        /// </summary>
        public static Mock<IPanelManager> WithInitResult(this Mock<IPanelManager> mock, bool result)
        {
            mock.Setup(m => m.Init(It.IsAny<IEnumerable<string>>())).Returns(result);
            return mock;
        }

        /// <summary>
        /// Configures <paramref name="mock"/> so that
        /// <see cref="IAgentManager.Init"/> returns <paramref name="result"/>.
        /// </summary>
        public static Mock<IAgentManager> WithInitResult(this Mock<IAgentManager> mock, bool result)
        {
            mock.Setup(m => m.Init(It.IsAny<IEnumerable<string>>())).Returns(result);
            return mock;
        }

        /// <summary>
        /// Configures <paramref name="mock"/> so that
        /// <see cref="IActuatorManager.Init"/> returns <paramref name="result"/>.
        /// </summary>
        public static Mock<IActuatorManager> WithInitResult(this Mock<IActuatorManager> mock, bool result)
        {
            mock.Setup(m => m.Init(It.IsAny<IEnumerable<string>>())).Returns(result);
            return mock;
        }

        /// <summary>
        /// Configures <paramref name="mock"/> so that
        /// <see cref="IWordPredictionManager.Init"/> returns <paramref name="result"/>.
        /// </summary>
        public static Mock<IWordPredictionManager> WithInitResult(this Mock<IWordPredictionManager> mock, bool result)
        {
            mock.Setup(m => m.Init(It.IsAny<IEnumerable<string>>())).Returns(result);
            return mock;
        }

        /// <summary>
        /// Configures <paramref name="mock"/> so that
        /// <see cref="ITTSManager.Init"/> returns <paramref name="result"/>.
        /// </summary>
        public static Mock<ITTSManager> WithInitResult(this Mock<ITTSManager> mock, bool result)
        {
            mock.Setup(m => m.Init(It.IsAny<IEnumerable<string>>())).Returns(result);
            return mock;
        }
    }
}
