////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// AnimationEvents.cs
//
// Event types published by AnimationSession to IEventBus.
// Designed per Issue #207 design spec §13.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Experimental.AnimationPOC.Interfaces;
using System;

namespace ACAT.Experimental.AnimationPOC.Events
{
    /// <summary>Published by AnimationSession on every state transition.</summary>
    public class AnimationStateChangedEvent : IEvent
    {
        /// <summary>Initializes a new AnimationStateChangedEvent.</summary>
        public AnimationStateChangedEvent(string panelName, PlayerState oldState, PlayerState newState,
            string currentAnimationName)
        {
            PanelName = panelName;
            OldState = oldState;
            NewState = newState;
            CurrentAnimationName = currentAnimationName;
            Timestamp = DateTime.UtcNow;
        }

        /// <summary>Name of the panel whose session changed state.</summary>
        public string PanelName { get; }

        /// <summary>State before the transition.</summary>
        public PlayerState OldState { get; }

        /// <summary>State after the transition.</summary>
        public PlayerState NewState { get; }

        /// <summary>Name of the current animation sequence at the time of the state change.</summary>
        public string CurrentAnimationName { get; }

        /// <summary>UTC timestamp of the event creation.</summary>
        public DateTime Timestamp { get; }
    }

    /// <summary>Published by AnimationSession when transitioning between animation sequences.</summary>
    public class AnimationTransitionEvent : IEvent
    {
        /// <summary>Initializes a new AnimationTransitionEvent.</summary>
        public AnimationTransitionEvent(string panelName, string fromAnimation, string toAnimation,
            bool isTopLevel = false)
        {
            PanelName = panelName;
            FromAnimation = fromAnimation;
            ToAnimation = toAnimation;
            IsTopLevel = isTopLevel;
            Timestamp = DateTime.UtcNow;
        }

        /// <summary>Name of the panel whose animation transitioned.</summary>
        public string PanelName { get; }

        /// <summary>Name of the animation sequence being left.</summary>
        public string FromAnimation { get; }

        /// <summary>Name of the animation sequence being entered.</summary>
        public string ToAnimation { get; }

        /// <summary>True if this transition is to a top-level (first) sequence.</summary>
        public bool IsTopLevel { get; }

        /// <summary>UTC timestamp of the event creation.</summary>
        public DateTime Timestamp { get; }
    }

    /// <summary>
    /// Published by AnimationSession when advancing to a new widget highlight.
    /// In Phase A, AnimationSession calls IHighlightRenderer directly.
    /// This event is also published for subscribers that need highlight notifications
    /// (e.g. audio feedback, accessibility tools).
    /// </summary>
    public class AnimationHighlightEvent : IEvent
    {
        /// <summary>Initializes a new AnimationHighlightEvent.</summary>
        public AnimationHighlightEvent(string panelName, string widgetName, bool playBeep,
            string previousWidgetName = null)
        {
            PanelName = panelName;
            WidgetName = widgetName;
            PlayBeep = playBeep;
            PreviousWidgetName = previousWidgetName;
            Timestamp = DateTime.UtcNow;
        }

        /// <summary>Name of the panel containing the highlighted widget.</summary>
        public string PanelName { get; }

        /// <summary>Name of the widget being highlighted.</summary>
        public string WidgetName { get; }

        /// <summary>Whether to play a beep sound for this highlight step.</summary>
        public bool PlayBeep { get; }

        /// <summary>Name of the widget that was previously highlighted (may be null).</summary>
        public string PreviousWidgetName { get; }

        /// <summary>UTC timestamp of the event creation.</summary>
        public DateTime Timestamp { get; }
    }
}
