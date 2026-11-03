////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// IScanModeStrategy.cs
//
// Strategy interface for scan algorithms (auto, manual, BCI).
// Encapsulates: widget selection, input handling, and timer policy.
// Designed per the Issue #207 design specification §4.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Experimental.AnimationPOC.Config;
using System.Collections.Generic;

namespace ACAT.Experimental.AnimationPOC.Interfaces
{
    /// <summary>
    /// Actions a strategy can return from HandleInput.
    /// </summary>
    public enum ScanInputAction
    {
        /// <summary>Ignore the input.</summary>
        None,

        /// <summary>Move to next widget.</summary>
        Advance,

        /// <summary>Move to previous widget.</summary>
        Reverse,

        /// <summary>Select the currently highlighted widget (triggers OnSelected).</summary>
        Select,

        /// <summary>Pause the scan.</summary>
        Pause,

        /// <summary>Resume a paused scan.</summary>
        Resume,

        /// <summary>Stop the scan entirely.</summary>
        Cancel
    }

    /// <summary>
    /// Types of actuator/input events that can be sent to a strategy.
    /// </summary>
    public enum ScanInputType
    {
        /// <summary>Primary switch activated (auto-scan selection).</summary>
        Switch1Activated,

        /// <summary>Secondary switch activated (directional scan advance).</summary>
        Switch2Activated,

        /// <summary>Directional: move right / advance.</summary>
        ScanRight,

        /// <summary>Directional: move left / reverse.</summary>
        ScanLeft,

        /// <summary>Directional: move up.</summary>
        ScanUp,

        /// <summary>Directional: move down.</summary>
        ScanDown,

        /// <summary>Select action (manual scan confirm).</summary>
        Select,

        /// <summary>Cancel/escape action.</summary>
        Cancel
    }

    /// <summary>
    /// Carries a raw actuator input event to the strategy.
    /// </summary>
    public class ScanInputEvent
    {
        /// <summary>Initializes a new ScanInputEvent.</summary>
        public ScanInputEvent(ScanInputType type)
        {
            Type = type;
        }

        /// <summary>Gets the type of input event.</summary>
        public ScanInputType Type { get; }
    }

    /// <summary>
    /// Read-only session context provided to strategy methods.
    /// Carries timing and state information without exposing mutable session internals.
    /// </summary>
    public interface IScanContext
    {
        /// <summary>Name of the panel being scanned.</summary>
        string PanelName { get; }

        /// <summary>Name of the currently active animation sequence.</summary>
        string CurrentAnimationName { get; }

        /// <summary>Number of iterations completed in the current sequence.</summary>
        int IterationCount { get; }

        /// <summary>Maximum iterations (0 = infinite).</summary>
        int MaxIterations { get; }

        /// <summary>Configured scan step interval in milliseconds.</summary>
        double ScanIntervalMs { get; }

        /// <summary>Configured hesitate (first-widget dwell) time in milliseconds.</summary>
        double HesitateTimeMs { get; }

        /// <summary>Current state of the animation session.</summary>
        PlayerState SessionState { get; }
    }

    /// <summary>
    /// Encapsulates one scanning algorithm (auto, manual, BCI).
    /// Implementations must be stateless with respect to per-session state
    /// unless carrying explicit context through <see cref="IScanContext"/>.
    ///
    /// Thread-safety: all methods may be called from the timer thread or the UI thread.
    /// The AnimationSession guarantees a lock is held before calling any strategy method.
    /// </summary>
    public interface IScanModeStrategy
    {
        /// <summary>Display name registered in DI (e.g. "auto", "manual", "bci").</summary>
        string Name { get; }

        /// <summary>
        /// Returns the index of the next widget to highlight.
        /// Called by AnimationSession on each timer tick (auto) or explicit advance (manual/BCI).
        /// </summary>
        /// <param name="widgets">Ordered list of widgets in the current animation sequence.</param>
        /// <param name="currentIndex">Zero-based index of currently highlighted widget. -1 = start.</param>
        /// <param name="context">Session context.</param>
        /// <returns>Zero-based index of the next widget, or -1 to end the sequence.</returns>
        int SelectNext(IReadOnlyList<AnimationWidgetConfig> widgets, int currentIndex, IScanContext context);

        /// <summary>
        /// Returns the index of the previous widget (for manual reverse-scan).
        /// Auto-scan strategies return currentIndex - 1 (or 0). BCI may return -1.
        /// </summary>
        int SelectPrevious(IReadOnlyList<AnimationWidgetConfig> widgets, int currentIndex, IScanContext context);

        /// <summary>
        /// Handles a raw actuator input event and returns the action the session should take.
        /// </summary>
        ScanInputAction HandleInput(ScanInputEvent inputEvent, IScanContext context);

        /// <summary>
        /// Called when the session starts a new animation sequence.
        /// Strategies may use this to reset internal per-sequence state.
        /// </summary>
        void OnSequenceStart(IReadOnlyList<AnimationWidgetConfig> widgets, IScanContext context);

        /// <summary>
        /// Called when the session ends an animation sequence (all iterations done or stopped).
        /// </summary>
        void OnSequenceEnd(IScanContext context);
    }
}
