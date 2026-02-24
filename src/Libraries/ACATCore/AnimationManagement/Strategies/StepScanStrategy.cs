////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// StepScanStrategy.cs
//
// Step scan strategy: one widget highlighted at a time per input event.
// Similar to manual scan but advances exactly one step per input,
// wrapping at end. Used for directional navigation without timer.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.AnimationManagement.Configuration;
using ACAT.Core.AnimationManagement.Interfaces;
using System.Collections.Generic;

namespace ACAT.Core.AnimationManagement.Strategies
{
    /// <summary>
    /// Step scan strategy: advances exactly one widget per actuator input.
    /// No automatic timer-driven advancement — each step requires explicit input.
    ///
    /// Input handling:
    ///   - ScanRight / Switch2Activated → Advance one step forward
    ///   - ScanLeft                      → Reverse one step backward
    ///   - ScanUp / ScanDown             → None (reserved for row/column navigation)
    ///   - Switch1Activated / Select     → Select current widget
    ///   - Cancel                        → Cancel scan
    ///
    /// This strategy is stateless.
    /// </summary>
    public class StepScanStrategy : IScanModeStrategy
    {
        /// <inheritdoc/>
        public string Name => "step";

        /// <inheritdoc/>
        public int SelectNext(IReadOnlyList<AnimationWidgetConfig> widgets, int currentIndex, IScanContext context)
        {
            if (widgets == null || widgets.Count == 0) return -1;

            if (currentIndex < 0) return 0;

            int next = currentIndex + 1;
            return next >= widgets.Count ? 0 : next; // wrap around
        }

        /// <inheritdoc/>
        public int SelectPrevious(IReadOnlyList<AnimationWidgetConfig> widgets, int currentIndex, IScanContext context)
        {
            if (widgets == null || widgets.Count == 0) return -1;

            if (currentIndex <= 0) return widgets.Count - 1; // wrap to last
            return currentIndex - 1;
        }

        /// <inheritdoc/>
        public ScanInputAction HandleInput(ScanInputEvent inputEvent, IScanContext context)
        {
            if (inputEvent == null) return ScanInputAction.None;

            switch (inputEvent.Type)
            {
                case ScanInputType.Switch2Activated:
                case ScanInputType.ScanRight:
                    return ScanInputAction.Advance;

                case ScanInputType.ScanLeft:
                    return ScanInputAction.Reverse;

                case ScanInputType.Switch1Activated:
                case ScanInputType.Select:
                    return ScanInputAction.Select;

                case ScanInputType.Cancel:
                    return ScanInputAction.Cancel;

                default:
                    return ScanInputAction.None;
            }
        }

        /// <inheritdoc/>
        public void OnSequenceStart(IReadOnlyList<AnimationWidgetConfig> widgets, IScanContext context)
        {
            // Step scan is stateless — no per-sequence initialization needed.
        }

        /// <inheritdoc/>
        public void OnSequenceEnd(IScanContext context)
        {
            // Step scan is stateless — no per-sequence cleanup needed.
        }
    }
}
