////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// AutoScanStrategy.cs
//
// Auto-scan strategy: timer-driven row-column sequential highlighting.
// This is the only strategy implemented in the Phase A POC.
// Implements the pseudo-code from Issue #207 design spec §4.2.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Experimental.AnimationPOC.Config;
using ACAT.Experimental.AnimationPOC.Interfaces;
using System.Collections.Generic;

namespace ACAT.Experimental.AnimationPOC.Strategies
{
    /// <summary>
    /// Auto-scan strategy: advances through widgets sequentially on each timer tick.
    ///
    /// Widget selection:
    ///   - Widgets are highlighted in the order defined in AnimationSequenceConfig.Widgets.
    ///   - When currentIndex == -1 (start of sequence), returns 0 (first widget).
    ///   - After the last widget, returns -1 to signal end-of-sequence.
    ///   - Iteration management (looping) is handled by AnimationSession, not this strategy.
    ///
    /// Input handling:
    ///   - Switch1Activated while Running → Select (select current widget)
    ///   - Switch1Activated while Paused  → Resume
    ///   - All other inputs               → None
    ///
    /// This strategy is stateless: no instance fields that survive between method calls.
    /// </summary>
    public class AutoScanStrategy : IScanModeStrategy
    {
        /// <inheritdoc/>
        public string Name => "auto";

        /// <inheritdoc/>
        public int SelectNext(IReadOnlyList<AnimationWidgetConfig> widgets, int currentIndex, IScanContext context)
        {
            if (widgets == null || widgets.Count == 0) return -1;

            if (currentIndex < 0) return 0;           // start of sequence → first widget

            int next = currentIndex + 1;
            return next >= widgets.Count ? -1 : next; // -1 = end of sequence
        }

        /// <inheritdoc/>
        public int SelectPrevious(IReadOnlyList<AnimationWidgetConfig> widgets, int currentIndex, IScanContext context)
        {
            if (widgets == null || widgets.Count == 0) return -1;

            if (currentIndex <= 0) return 0;          // wrap to start
            return currentIndex - 1;
        }

        /// <inheritdoc/>
        public ScanInputAction HandleInput(ScanInputEvent inputEvent, IScanContext context)
        {
            if (inputEvent == null) return ScanInputAction.None;

            if (inputEvent.Type == ScanInputType.Switch1Activated)
            {
                if (context.SessionState == PlayerState.Paused)
                    return ScanInputAction.Resume;

                return ScanInputAction.Select;
            }

            return ScanInputAction.None;
        }

        /// <inheritdoc/>
        public void OnSequenceStart(IReadOnlyList<AnimationWidgetConfig> widgets, IScanContext context)
        {
            // Auto scan is stateless — no per-sequence initialization needed.
        }

        /// <inheritdoc/>
        public void OnSequenceEnd(IScanContext context)
        {
            // Auto scan is stateless — no per-sequence cleanup needed.
        }
    }
}
