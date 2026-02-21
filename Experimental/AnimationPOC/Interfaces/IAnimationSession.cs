////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// IAnimationSession.cs
//
// Per-panel scan session interface. Replaces the AnimationPlayer +
// AnimationManager per-panel pattern defined in the Issue #207 design spec §5.2.
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Experimental.AnimationPOC.Interfaces
{
    /// <summary>
    /// Represents a single active scan session bound to one panel or user control.
    ///
    /// Lifecycle:
    ///   Created by IAnimationService.CreateSession()
    ///   Started by caller: session.Start()
    ///   Stopped by caller: session.Stop() — or panel deactivation
    ///   Disposed by caller: session.Dispose()
    ///
    /// Events:
    ///   State changes are published to IEventBus as AnimationStateChangedEvent.
    ///   Transitions are published as AnimationTransitionEvent.
    ///   (No direct C# events on this interface — consume via IEventBus.)
    ///
    /// Thread-safety:
    ///   All public methods are safe to call from any thread.
    ///   Internal state is protected by a single lock (_sessionLock).
    ///   IEventBus.Publish() is called without holding _sessionLock to avoid deadlocks.
    /// </summary>
    public interface IAnimationSession : IDisposable
    {
        /// <summary>Gets the name of the panel this session is scanning.</summary>
        string PanelName { get; }

        /// <summary>Gets the current session state.</summary>
        PlayerState State { get; }

        /// <summary>Gets the name of the currently active animation sequence.</summary>
        string CurrentAnimationName { get; }

        /// <summary>
        /// Starts the session with the first animation (isFirst=true) in the config,
        /// or with the named animation if <paramref name="animationName"/> is provided.
        /// Transitions to Running state.
        /// No-op if already Running.
        /// </summary>
        void Start(string animationName = null);

        /// <summary>Stops the session. Transitions to Stopped. Timer is disabled.</summary>
        void Stop();

        /// <summary>Pauses scanning. Timer is disabled. Widget remains highlighted.</summary>
        void Pause();

        /// <summary>Resumes scanning from the current widget position.</summary>
        void Resume();

        /// <summary>
        /// Signals actuator input (switch press) during scanning.
        /// Delegates to IScanModeStrategy.HandleInput().
        /// Executes OnSelected logic for the current widget if the strategy returns Select.
        /// Transitions to Interrupted state, then back to Running (or next animation).
        /// </summary>
        void Interrupt();

        /// <summary>
        /// Transitions to the named animation sequence, or to the next sequence
        /// if <paramref name="targetAnimationName"/> is null.
        /// Used for row-to-column drill-down in hierarchical scan.
        /// </summary>
        void Transition(string targetAnimationName = null);

        /// <summary>
        /// Sets the selected/active widget by name. Used by manual and BCI scan modes
        /// to position the highlight without timer-driven advancement.
        /// </summary>
        void SetSelectedWidget(string widgetName);

        /// <summary>Highlights the default home widget (first widget of first animation).</summary>
        void HighlightDefaultHome();
    }
}
