////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// AnimationPlayerAdapter.cs
//
// Adapter that bridges PanelAnimationManager / UserControlAnimationManager
// to the new IAnimationService / IAnimationSession engine.
//
// Design:
//   - Created per-panel when IAnimationService is available (injected via
//     PanelAnimationManager.AnimationService property).
//   - Falls back to legacy AnimationPlayer if session creation fails.
//   - Wraps IAnimationSession lifecycle in Start / Stop / Pause / Resume
//     calls that map to AnimationPlayer's public surface.
//   - Routes XmlAnimationConfigAdapter-converted configs to IAnimationService.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.AnimationManagement.Configuration;
using ACAT.Core.AnimationManagement.Interfaces;
using ACAT.Core.AnimationManagement.Rendering;
using ACAT.Core.EventManagement;
using ACAT.Core.WidgetManagement;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Windows.Forms;
using System.Xml;

namespace ACAT.Core.AnimationManagement
{
    /// <summary>
    /// Adapts the new <see cref="IAnimationService"/> / <see cref="IAnimationSession"/>
    /// engine for use by <see cref="PanelAnimationManager"/> and
    /// <see cref="UserControlAnimationManager"/>.
    ///
    /// Callers create one adapter per panel activation via
    /// <see cref="AnimationPlayerAdapter.TryCreate"/>. If the new engine is
    /// unavailable or session creation fails, <c>TryCreate</c> returns null and
    /// the caller falls back to the legacy <see cref="AnimationPlayer"/>.
    ///
    /// Thread-safety: methods are safe to call from any thread; they delegate to
    /// IAnimationSession which is itself thread-safe.
    /// </summary>
    public class AnimationPlayerAdapter : IDisposable
    {
        private readonly IAnimationSession _session;
        private readonly ILogger<AnimationPlayerAdapter> _logger;
        private bool _disposed;

        private AnimationPlayerAdapter(IAnimationSession session, ILogger<AnimationPlayerAdapter> logger)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _logger = logger ?? NullLogger<AnimationPlayerAdapter>.Instance;
        }

        /// <summary>
        /// Gets the name of the panel this adapter is managing.
        /// </summary>
        public string PanelName => _session.PanelName;

        /// <summary>
        /// Gets the current state of the underlying animation session.
        /// </summary>
        public PlayerState State => _session.State;

        /// <summary>
        /// Gets the name of the currently active animation sequence.
        /// </summary>
        public string CurrentAnimationName => _session.CurrentAnimationName;

        /// <summary>
        /// Attempts to create an <see cref="AnimationPlayerAdapter"/> using the new engine.
        /// Returns <c>null</c> if <paramref name="animationService"/> is null, if config
        /// conversion fails, or if session creation throws.
        /// </summary>
        /// <param name="panelName">The registered panel name.</param>
        /// <param name="animationsNode">
        ///   The <c>&lt;Animations&gt;</c> XML node from the panel config file.
        ///   May be null; if so, an empty config is used (no animations).
        /// </param>
        /// <param name="animationService">The IAnimationService from the DI container.</param>
        /// <param name="eventBus">The application event bus (used to publish state events).</param>
        /// <param name="rootWidget">The root widget object for the panel (passed to renderer).</param>
        /// <param name="scanStrategy">
        ///   Scan strategy name ("auto", "manual", "step"). Defaults to "auto".
        /// </param>
        /// <param name="logger">Optional logger.</param>
        /// <returns>A new adapter, or null if the new engine cannot be used.</returns>
        public static AnimationPlayerAdapter TryCreate(
            string panelName,
            XmlNode animationsNode,
            IAnimationService animationService,
            IEventBus eventBus,
            object rootWidget,
            string scanStrategy = "auto",
            ILogger<AnimationPlayerAdapter> logger = null)
        {
            var log = logger ?? NullLogger<AnimationPlayerAdapter>.Instance;

            if (animationService == null)
            {
                log.LogDebug("AnimationPlayerAdapter.TryCreate: IAnimationService not available for panel {PanelName}", panelName);
                return null;
            }

            try
            {
                AnimationConfig config;
                if (animationsNode != null)
                {
                    var xmlAdapter = new XmlAnimationConfigAdapter();
                    config = xmlAdapter.Convert(panelName, animationsNode);
                }
                else
                {
                    config = new AnimationConfig
                    {
                        PanelName = panelName,
                        ScanStrategy = scanStrategy ?? "auto"
                    };
                }

                var session = animationService.CreateSession(rootWidget, config, scanStrategy ?? "auto",
                    BuildRenderer(rootWidget, log));
                log.LogDebug("AnimationPlayerAdapter.TryCreate: created session for panel {PanelName}", panelName);
                return new AnimationPlayerAdapter(session, log);
            }
            catch (Exception ex)
            {
                log.LogWarning(ex,
                    "AnimationPlayerAdapter.TryCreate: failed to create session for panel {PanelName}; falling back to legacy AnimationPlayer",
                    panelName);
                return null;
            }
        }

        // ----------------------------------------------------------------
        // Lifecycle delegation — mirror AnimationPlayer's public surface
        // ----------------------------------------------------------------

        /// <summary>
        /// Starts the animation session, beginning with the named animation or the
        /// first animation (IsFirst = true) if <paramref name="animationName"/> is null.
        /// </summary>
        public void Start(string animationName = null)
        {
            ThrowIfDisposed();
            _logger.LogDebug("AnimationPlayerAdapter.Start panel={PanelName} animation={AnimationName}",
                PanelName, animationName ?? "(first)");
            _session.Start(animationName);
        }

        /// <summary>Stops the session. Clears all highlights.</summary>
        public void Stop()
        {
            ThrowIfDisposed();
            _session.Stop();
        }

        /// <summary>Pauses scanning. Widget remains highlighted.</summary>
        public void Pause()
        {
            ThrowIfDisposed();
            _session.Pause();
        }

        /// <summary>Resumes scanning from the current widget position.</summary>
        public void Resume()
        {
            ThrowIfDisposed();
            _session.Resume();
        }

        /// <summary>
        /// Signals actuator input (switch press). Delegates to
        /// IScanModeStrategy.HandleInput() via IAnimationSession.Interrupt().
        /// </summary>
        public void Interrupt()
        {
            ThrowIfDisposed();
            _session.Interrupt();
        }

        /// <summary>
        /// Transitions to the named animation sequence, or the next sequence
        /// if <paramref name="animationName"/> is null.
        /// </summary>
        public void Transition(string animationName = null)
        {
            ThrowIfDisposed();
            _session.Transition(animationName);
        }

        /// <summary>Sets the selected widget by name.</summary>
        public void SetSelectedWidget(string widgetName)
        {
            ThrowIfDisposed();
            _session.SetSelectedWidget(widgetName);
        }

        /// <summary>Highlights the default home widget.</summary>
        public void HighlightDefaultHome()
        {
            ThrowIfDisposed();
            _session.HighlightDefaultHome();
        }

        // ----------------------------------------------------------------
        // IDisposable
        // ----------------------------------------------------------------

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _session.Dispose();
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(AnimationPlayerAdapter));
        }

        // ----------------------------------------------------------------
        // Renderer factory
        // ----------------------------------------------------------------

        /// <summary>
        /// Builds a <see cref="WinFormsHighlightRenderer"/> whose callbacks delegate to
        /// the <see cref="Widget"/> tree rooted at <paramref name="rootWidget"/>.
        /// Returns <c>null</c> when <paramref name="rootWidget"/> is not a
        /// <see cref="Widget"/> (the caller should then rely on the DI singleton renderer).
        /// All render/clear calls are marshalled to the UI thread via
        /// <see cref="Control.BeginInvoke"/> when required.
        /// </summary>
        private static IHighlightRenderer BuildRenderer(object rootWidget, ILogger log)
        {
            if (rootWidget is not Widget root) return null;

            var ctrl = root.UIControl as Control;
            return new WinFormsHighlightRenderer(
                renderCallback: (name, style) =>
                    InvokeOnUIThread(ctrl, () => root.Finder.FindChild(name)?.HighlightOn()),
                clearCallback: (name) =>
                    InvokeOnUIThread(ctrl, () => root.Finder.FindChild(name)?.HighlightOff()),
                clearAllCallback: () =>
                    InvokeOnUIThread(ctrl, () => root.HighlightOff()));
        }

        /// <summary>
        /// Executes <paramref name="action"/> on the UI thread. If <paramref name="ctrl"/>
        /// requires a cross-thread invoke, uses <see cref="Control.BeginInvoke"/>;
        /// otherwise executes directly on the current thread.
        /// </summary>
        private static void InvokeOnUIThread(Control ctrl, Action action)
        {
            if (ctrl != null && ctrl.InvokeRequired)
                ctrl.BeginInvoke(action);
            else
                action();
        }
    }
}
