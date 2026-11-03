////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// AnimationSession.cs
//
// Core implementation of IAnimationSession.
// Manages lifecycle of one scan sequence using IScanTimer + IScanModeStrategy.
// Replaces the monolithic AnimationPlayer state machine for new callers.
//
// Thread-safety model:
//   - _sessionLock protects all state fields.
//   - IEventBus.Publish() and IHighlightRenderer calls are made WITHOUT holding
//     _sessionLock to prevent deadlocks with UI subscribers.
//   - IHighlightRenderer.Render() must be called on the UI thread;
//     the renderer implementation is responsible for marshalling.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.AnimationManagement.Configuration;
using ACAT.Core.AnimationManagement.Interfaces;
using ACAT.Core.AnimationManagement.Rendering;
using ACAT.Core.EventManagement;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Timers;

namespace ACAT.Core.AnimationManagement
{
    /// <summary>
    /// Per-panel scan session. Drives widget highlighting via IScanTimer + IScanModeStrategy.
    /// Publishes <see cref="AnimationStateChangedEvent"/>, <see cref="AnimationTransitionEvent"/>,
    /// and <see cref="AnimationHighlightEvent"/> to <see cref="IEventBus"/> on each state
    /// change and widget advance.
    ///
    /// Lifecycle: Created → Start() → [Running ↔ Paused] → Stop() → Disposed
    /// </summary>
    public class AnimationSession : IAnimationSession
    {
        private readonly AnimationConfig _config;
        private readonly IScanTimer _timer;
        private readonly IScanModeStrategy _strategy;
        private readonly IEventBus _eventBus;
        private readonly IHighlightRenderer _renderer;
        private readonly ILogger<AnimationSession> _logger;

        private readonly object _sessionLock = new object();
        private PlayerState _state = PlayerState.Unknown;
        private AnimationSequenceConfig _currentSequence;
        private int _currentWidgetIndex = -1;
        private int _iterationCount;
        private bool _disposed;

        /// <summary>
        /// Initializes a new AnimationSession.
        /// </summary>
        /// <param name="config">Animation configuration for this panel.</param>
        /// <param name="timer">Scan timer (SystemScanTimer for production, TestScanTimer for tests).</param>
        /// <param name="strategy">Scan mode strategy (AutoScanStrategy, ManualScanStrategy, etc.).</param>
        /// <param name="eventBus">Event bus for publishing state change events.</param>
        /// <param name="renderer">Highlight renderer (calls back to UI layer).</param>
        /// <param name="logger">Optional logger; NullLogger used if not provided.</param>
        public AnimationSession(
            AnimationConfig config,
            IScanTimer timer,
            IScanModeStrategy strategy,
            IEventBus eventBus,
            IHighlightRenderer renderer,
            ILogger<AnimationSession> logger = null)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _timer = timer ?? throw new ArgumentNullException(nameof(timer));
            _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
            _logger = logger ?? NullLogger<AnimationSession>.Instance;

            _timer.Elapsed += OnTimerElapsed;
        }

        /// <inheritdoc/>
        public string PanelName => _config.PanelName;

        /// <inheritdoc/>
        public PlayerState State
        {
            get { lock (_sessionLock) return _state; }
        }

        /// <inheritdoc/>
        public string CurrentAnimationName
        {
            get { lock (_sessionLock) return _currentSequence?.Name; }
        }

        /// <inheritdoc/>
        public void Start(string animationName = null)
        {
            AnimationSequenceConfig sequence;
            PlayerState oldState;

            lock (_sessionLock)
            {
                if (_disposed) throw new ObjectDisposedException(nameof(AnimationSession));
                if (_state == PlayerState.Running) return;

                sequence = ResolveSequence(animationName);
                if (sequence == null)
                {
                    _logger.LogWarning("AnimationSession.Start: no sequence found for panel={PanelName}, animationName={AnimationName}",
                        _config.PanelName, animationName ?? "(first)");
                    return;
                }

                oldState = _state;
                _currentSequence = sequence;
                _currentWidgetIndex = -1;
                _iterationCount = 0;

                double interval = ResolveDouble(_currentSequence.ScanTime, 600.0);
                _timer.Interval = interval;
                _timer.AutoReset = true;

                _strategy.OnSequenceStart(_currentSequence.Widgets, GetContext());
                _state = PlayerState.Running;
            }

            // Publish state change outside lock
            PublishStateChange(oldState, PlayerState.Running);
            _logger.LogDebug("AnimationSession started: panel={PanelName}, sequence={Sequence}, interval={Interval}ms",
                _config.PanelName, sequence.Name, _timer.Interval);

            _timer.Start();
        }

        /// <inheritdoc/>
        public void Stop()
        {
            PlayerState oldState;

            lock (_sessionLock)
            {
                if (_disposed || _state == PlayerState.Stopped) return;
                oldState = _state;
                _strategy.OnSequenceEnd(GetContext());
                _state = PlayerState.Stopped;
            }

            _timer.Stop();
            _renderer.ClearAll();
            PublishStateChange(oldState, PlayerState.Stopped);
            _logger.LogDebug("AnimationSession stopped: panel={PanelName}", _config.PanelName);
        }

        /// <inheritdoc/>
        public void Pause()
        {
            PlayerState oldState;

            lock (_sessionLock)
            {
                if (_disposed || _state != PlayerState.Running) return;
                oldState = _state;
                _state = PlayerState.Paused;
            }

            _timer.Stop();
            PublishStateChange(oldState, PlayerState.Paused);
            _logger.LogDebug("AnimationSession paused: panel={PanelName}", _config.PanelName);
        }

        /// <inheritdoc/>
        public void Resume()
        {
            PlayerState oldState;

            lock (_sessionLock)
            {
                if (_disposed || _state != PlayerState.Paused) return;
                oldState = _state;
                _state = PlayerState.Running;
            }

            PublishStateChange(oldState, PlayerState.Running);
            _timer.Start();
            _logger.LogDebug("AnimationSession resumed: panel={PanelName}", _config.PanelName);
        }

        /// <inheritdoc/>
        public void Interrupt()
        {
            ScanInputAction action;
            AnimationWidgetConfig currentWidget = null;
            PlayerState oldState;

            lock (_sessionLock)
            {
                if (_disposed || (_state != PlayerState.Running && _state != PlayerState.Paused)) return;

                oldState = _state;
                action = _strategy.HandleInput(
                    new ScanInputEvent(ScanInputType.Switch1Activated), GetContext());

                if (action == ScanInputAction.Select)
                {
                    if (_currentWidgetIndex >= 0 && _currentSequence != null &&
                        _currentWidgetIndex < _currentSequence.Widgets.Count)
                    {
                        currentWidget = _currentSequence.Widgets[_currentWidgetIndex];
                    }
                    _state = PlayerState.Interrupted;
                }
                else if (action == ScanInputAction.Resume)
                {
                    _state = PlayerState.Running;
                }
            }

            if (action == ScanInputAction.Select)
            {
                _timer.Stop();
                PublishStateChange(oldState, PlayerState.Interrupted);

                if (currentWidget != null)
                {
                    _logger.LogDebug("AnimationSession interrupted, widget selected: {WidgetName}", currentWidget.Name);
                    // OnSelected PCode execution would be wired here for full implementation.
                }

                // Transition back to Running after interrupt
                lock (_sessionLock)
                {
                    if (!_disposed) _state = PlayerState.Running;
                }
                PublishStateChange(PlayerState.Interrupted, PlayerState.Running);
                _timer.Start();
            }
            else if (action == ScanInputAction.Resume)
            {
                PublishStateChange(PlayerState.Paused, PlayerState.Running);
                _timer.Start();
            }
        }

        /// <inheritdoc/>
        public void Transition(string targetAnimationName = null)
        {
            AnimationSequenceConfig fromSeq;
            AnimationSequenceConfig toSeq;

            lock (_sessionLock)
            {
                if (_disposed) return;
                fromSeq = _currentSequence;
                toSeq = ResolveSequence(targetAnimationName);
                if (toSeq == null) return;
                _currentSequence = toSeq;
                _currentWidgetIndex = -1;
                _iterationCount = 0;
                _strategy.OnSequenceStart(toSeq.Widgets, GetContext());
            }

            var transitionEvent = new AnimationTransitionEvent(
                _config.PanelName,
                fromSeq?.Name,
                toSeq.Name,
                toSeq.IsFirst);
            _eventBus.Publish(transitionEvent);
            _logger.LogDebug("AnimationSession transition: {From} → {To}", fromSeq?.Name, toSeq.Name);
        }

        /// <inheritdoc/>
        public void SetSelectedWidget(string widgetName)
        {
            if (string.IsNullOrEmpty(widgetName)) return;

            lock (_sessionLock)
            {
                if (_disposed || _currentSequence == null) return;
                for (int i = 0; i < _currentSequence.Widgets.Count; i++)
                {
                    if (_currentSequence.Widgets[i].Name == widgetName)
                    {
                        _currentWidgetIndex = i;
                        break;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void HighlightDefaultHome()
        {
            AnimationWidgetConfig firstWidget = null;

            lock (_sessionLock)
            {
                if (_disposed || _currentSequence == null || _currentSequence.Widgets.Count == 0) return;
                firstWidget = _currentSequence.Widgets[0];
            }

            if (firstWidget != null)
            {
                _renderer.Render(firstWidget.Name, new HighlightStyle { PlayBeep = firstWidget.PlayBeep });
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            lock (_sessionLock)
            {
                if (_disposed) return;
                _disposed = true;
            }

            _timer.Stop();
            _timer.Elapsed -= OnTimerElapsed;
            _renderer.ClearAll();
            _logger.LogDebug("AnimationSession disposed: panel={PanelName}", _config.PanelName);
        }

        // ---------------------------------------------------------------
        // Timer callback
        // ---------------------------------------------------------------

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            AnimationWidgetConfig previousWidget = null;
            AnimationWidgetConfig nextWidget = null;
            bool sequenceEnded = false;

            lock (_sessionLock)
            {
                if (_disposed || _state != PlayerState.Running || _currentSequence == null) return;

                // Get previous widget for the highlight event
                if (_currentWidgetIndex >= 0 && _currentWidgetIndex < _currentSequence.Widgets.Count)
                    previousWidget = _currentSequence.Widgets[_currentWidgetIndex];

                int nextIndex = _strategy.SelectNext(_currentSequence.Widgets, _currentWidgetIndex, GetContext());

                if (nextIndex < 0)
                {
                    // End of one pass through the sequence
                    _iterationCount++;
                    int maxIter = ResolveInt(_currentSequence.Iterations, 1);

                    if (maxIter > 0 && _iterationCount >= maxIter)
                    {
                        // All iterations complete
                        _strategy.OnSequenceEnd(GetContext());
                        _state = PlayerState.Timeout;
                        sequenceEnded = true;
                    }
                    else
                    {
                        // Loop: restart the sequence
                        _currentWidgetIndex = -1;
                        nextIndex = _strategy.SelectNext(_currentSequence.Widgets, -1, GetContext());
                        _currentWidgetIndex = nextIndex;
                        nextWidget = nextIndex >= 0 ? _currentSequence.Widgets[nextIndex] : null;
                    }
                }
                else
                {
                    _currentWidgetIndex = nextIndex;
                    nextWidget = _currentSequence.Widgets[nextIndex];
                }
            }

            if (sequenceEnded)
            {
                _timer.Stop();
                PublishStateChange(PlayerState.Running, PlayerState.Timeout);
                lock (_sessionLock)
                {
                    if (!_disposed) _state = PlayerState.Stopped;
                }
                PublishStateChange(PlayerState.Timeout, PlayerState.Stopped);
                _renderer.ClearAll();
                return;
            }

            if (nextWidget != null)
            {
                // Clear previous highlight
                if (previousWidget != null)
                    _renderer.ClearHighlight(previousWidget.Name);

                // Apply new highlight
                _renderer.Render(nextWidget.Name, new HighlightStyle { PlayBeep = nextWidget.PlayBeep });

                // Publish highlight event
                _eventBus.Publish(new AnimationHighlightEvent(
                    _config.PanelName,
                    nextWidget.Name,
                    nextWidget.PlayBeep,
                    previousWidget?.Name));
            }
        }

        // ---------------------------------------------------------------
        // Helpers
        // ---------------------------------------------------------------

        private AnimationSequenceConfig ResolveSequence(string animationName)
        {
            if (_config.Sequences == null || _config.Sequences.Count == 0) return null;

            if (animationName != null)
                return _config.Sequences.Find(s => s.Name == animationName);

            var first = _config.Sequences.Find(s => s.IsFirst);
            return first ?? _config.Sequences[0];
        }

        private IScanContext GetContext()
        {
            return new ScanContextSnapshot(
                panelName: _config.PanelName,
                currentAnimationName: _currentSequence?.Name,
                iterationCount: _iterationCount,
                maxIterations: ResolveInt(_currentSequence?.Iterations, 1),
                scanIntervalMs: ResolveDouble(_currentSequence?.ScanTime, 600.0),
                hesitateTimeMs: ResolveDouble(_currentSequence?.FirstPauseTime, 0.0),
                sessionState: _state);
        }

        private void PublishStateChange(PlayerState oldState, PlayerState newState)
        {
            _eventBus.Publish(new AnimationStateChangedEvent(
                _config.PanelName, oldState, newState, _currentSequence?.Name));
        }

        private static double ResolveDouble(string expression, double defaultValue)
        {
            if (string.IsNullOrWhiteSpace(expression)) return defaultValue;
            // Preference variable references (e.g. "@GridScanTime") return defaultValue in Phase A.
            if (expression.StartsWith("@")) return defaultValue;
            return double.TryParse(expression, out double result) ? result : defaultValue;
        }

        private static int ResolveInt(string expression, int defaultValue)
        {
            if (string.IsNullOrWhiteSpace(expression)) return defaultValue;
            if (expression.StartsWith("@")) return defaultValue;
            return int.TryParse(expression, out int result) ? result : defaultValue;
        }

        // ---------------------------------------------------------------
        // Inner: ScanContext snapshot (immutable, safe to pass without lock)
        // ---------------------------------------------------------------

        private sealed class ScanContextSnapshot : IScanContext
        {
            public ScanContextSnapshot(string panelName, string currentAnimationName,
                int iterationCount, int maxIterations, double scanIntervalMs,
                double hesitateTimeMs, PlayerState sessionState)
            {
                PanelName = panelName;
                CurrentAnimationName = currentAnimationName;
                IterationCount = iterationCount;
                MaxIterations = maxIterations;
                ScanIntervalMs = scanIntervalMs;
                HesitateTimeMs = hesitateTimeMs;
                SessionState = sessionState;
            }

            public string PanelName { get; }
            public string CurrentAnimationName { get; }
            public int IterationCount { get; }
            public int MaxIterations { get; }
            public double ScanIntervalMs { get; }
            public double HesitateTimeMs { get; }
            public PlayerState SessionState { get; }
        }
    }
}
