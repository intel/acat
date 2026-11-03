////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// AnimationService.cs
//
// Root service for the animation engine (IAnimationService implementation).
// Thin factory + session registry. Singleton lifetime.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.AnimationManagement.Configuration;
using ACAT.Core.AnimationManagement.Interfaces;
using ACAT.Core.AnimationManagement.Strategies;
using ACAT.Core.EventManagement;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;

namespace ACAT.Core.AnimationManagement
{
    /// <summary>
    /// Root service for the animation engine.
    /// Creates and tracks AnimationSession instances.
    /// Thread-safe: all methods are synchronized.
    /// </summary>
    public class AnimationService : IAnimationService
    {
        private readonly IEventBus _eventBus;
        private readonly IHighlightRenderer _renderer;
        private readonly ILogger<AnimationService> _logger;
        private readonly IScanStrategyFactory _strategyFactory;
        private readonly object _lock = new object();
        private readonly List<IAnimationSession> _activeSessions = new List<IAnimationSession>();
        private bool _disposed;

        /// <summary>
        /// Initializes a new AnimationService.
        /// </summary>
        public AnimationService(
            IEventBus eventBus,
            IHighlightRenderer renderer,
            IScanStrategyFactory strategyFactory = null,
            ILogger<AnimationService> logger = null)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _renderer = renderer; // may be null; CreateSession validates per-call
            _strategyFactory = strategyFactory ?? new DefaultScanStrategyFactory();
            _logger = logger ?? NullLogger<AnimationService>.Instance;
        }

        /// <inheritdoc/>
        public IAnimationSession CreateSession(object rootWidget, AnimationConfig config, string strategyName, IHighlightRenderer renderer = null)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            var effectiveRenderer = renderer ?? _renderer;
            if (effectiveRenderer == null)
                throw new InvalidOperationException(
                    "IHighlightRenderer has not been registered. " +
                    "Either register an IHighlightRenderer in DI or pass one to CreateSession().");

            var strategy = _strategyFactory.Create(strategyName ?? config.ScanStrategy ?? "auto");
            var timer = new SystemScanTimer();

            var session = new AnimationSession(config, timer, strategy, _eventBus, effectiveRenderer, null);

            lock (_lock)
            {
                if (_disposed) throw new ObjectDisposedException(nameof(AnimationService));
                _activeSessions.Add(session);
            }

            _logger.LogDebug("AnimationService: created session for panel={PanelName}, strategy={Strategy}",
                config.PanelName, strategy.Name);

            return session;
        }

        /// <inheritdoc/>
        public void Shutdown()
        {
            List<IAnimationSession> sessions;
            lock (_lock)
            {
                _disposed = true;
                sessions = new List<IAnimationSession>(_activeSessions);
                _activeSessions.Clear();
            }

            foreach (var session in sessions)
            {
                try
                {
                    session.Stop();
                    session.Dispose();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "AnimationService.Shutdown: error disposing session");
                }
            }

            _logger.LogInformation("AnimationService shutdown complete. {Count} session(s) closed.", sessions.Count);
        }
    }

    /// <summary>
    /// Factory interface for scan strategy resolution.
    /// Allows named-factory pattern as an alternative to keyed DI services.
    /// </summary>
    public interface IScanStrategyFactory
    {
        /// <summary>Returns the strategy for the given name.</summary>
        IScanModeStrategy Create(string strategyName);
    }

    /// <summary>
    /// Default factory: returns the appropriate strategy by name.
    /// Supports "auto", "manual", and "step" strategies.
    /// </summary>
    public class DefaultScanStrategyFactory : IScanStrategyFactory
    {
        /// <inheritdoc/>
        public IScanModeStrategy Create(string strategyName)
        {
            switch (strategyName?.ToLowerInvariant())
            {
                case "manual":
                    return new ManualScanStrategy();
                case "step":
                    return new StepScanStrategy();
                case "auto":
                case null:
                default:
                    return new AutoScanStrategy();
            }
        }
    }
}
