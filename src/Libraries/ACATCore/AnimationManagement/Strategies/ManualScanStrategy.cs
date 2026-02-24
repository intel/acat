////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// ManualScanStrategy.cs
//
// Manual scan strategy: user-triggered widget advancement.
// The timer is not used for advancement; the user explicitly advances
// via input events. Only advances on ScanRight/Switch2Activated input.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.AnimationManagement.Configuration;
using ACAT.Core.AnimationManagement.Interfaces;
using System.Collections.Generic;

namespace ACAT.Core.AnimationManagement.Strategies
{
    /// <summary>
    /// Manual scan strategy: advances to the next widget only when the user
    /// explicitly triggers a forward (ScanRight / Switch2Activated) input.
    /// Switch1Activated selects the currently highlighted widget.
    ///
    /// Input handling:
    ///   - Switch2Activated / ScanRight  → Advance (move to next widget)
    ///   - ScanLeft                       → Reverse (move to previous widget)
    ///   - Switch1Activated / Select      → Select (select current widget)
    ///   - All other inputs               → None
    ///
    /// This strategy is stateless.
    /// </summary>
    public class ManualScanStrategy : IScanModeStrategy
    {
        /// <inheritdoc/>
        public string Name => "manual";

        /// <inheritdoc/>
        public int SelectNext(IReadOnlyList<AnimationWidgetConfig> widgets, int currentIndex, IScanContext context)
        {
            if (widgets == null || widgets.Count == 0) return -1;

            if (currentIndex < 0) return 0;

            int next = currentIndex + 1;
            return next >= widgets.Count ? 0 : next; // wrap around in manual mode
        }

        /// <inheritdoc/>
        public int SelectPrevious(IReadOnlyList<AnimationWidgetConfig> widgets, int currentIndex, IScanContext context)
        {
            if (widgets == null || widgets.Count == 0) return -1;

            if (currentIndex <= 0) return widgets.Count - 1; // wrap to end
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
            // Manual scan is stateless — no per-sequence initialization needed.
        }

        /// <inheritdoc/>
        public void OnSequenceEnd(IScanContext context)
        {
            // Manual scan is stateless — no per-sequence cleanup needed.
        }
    }
}
