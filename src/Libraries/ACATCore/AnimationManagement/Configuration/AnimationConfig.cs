////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// AnimationConfig.cs
//
// JSON-first data model for animation configuration.
// Designed per Issue #207 design spec §6.
//
////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

namespace ACAT.Core.AnimationManagement.Configuration
{
    /// <summary>
    /// Root config object for one panel's animations. Loaded from
    /// {panelName}.animation.json (preferred) or converted from legacy XML.
    /// </summary>
    public class AnimationConfig
    {
        /// <summary>
        /// Matches the panel's registered name (PanelConfigMap key).
        /// </summary>
        public string PanelName { get; set; }

        /// <summary>
        /// Scan strategy to use (overrides session default). Null = use "auto".
        /// Supported values: "auto", "manual", "step".
        /// </summary>
        public string ScanStrategy { get; set; }

        /// <summary>The ordered list of animation sequences for this panel.</summary>
        public List<AnimationSequenceConfig> Sequences { get; set; } = new List<AnimationSequenceConfig>();
    }

    /// <summary>
    /// A single named scan sequence: an ordered list of widgets highlighted one at a time.
    /// Corresponds to one &lt;Animation&gt; element in the legacy XML format.
    /// </summary>
    public class AnimationSequenceConfig
    {
        /// <summary>
        /// Unique name within the panel. Used by Transition() calls.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// If true, this is the first sequence activated when the panel opens.
        /// Exactly one sequence per panel should have IsFirst = true.
        /// </summary>
        public bool IsFirst { get; set; }

        /// <summary>
        /// If true, scanning starts automatically without waiting for input.
        /// </summary>
        public bool AutoStart { get; set; }

        /// <summary>
        /// Number of times to repeat this sequence before moving to the next or stopping.
        /// String to support preference variable references (e.g. "@GridScanIterations").
        /// "0" or absent = loop indefinitely.
        /// </summary>
        public string Iterations { get; set; } = "1";

        /// <summary>
        /// Scan step interval in milliseconds.
        /// String to support preference variable references (e.g. "@MenuDialogScanTime").
        /// </summary>
        public string ScanTime { get; set; }

        /// <summary>
        /// Extra dwell time on the first widget (hesitate time) in milliseconds.
        /// String to support preference variable references (e.g. "@FirstPauseTime").
        /// </summary>
        public string FirstPauseTime { get; set; }

        /// <summary>PCode script executed when this animation sequence begins.</summary>
        public string OnEnter { get; set; }

        /// <summary>PCode script executed when all iterations complete.</summary>
        public string OnEnd { get; set; }

        /// <summary>Ordered list of widgets to highlight in this sequence.</summary>
        public List<AnimationWidgetConfig> Widgets { get; set; } = new List<AnimationWidgetConfig>();
    }

    /// <summary>
    /// A single widget step in an animation sequence.
    /// Defines which widget to highlight and what happens when the user selects it.
    /// </summary>
    public class AnimationWidgetConfig
    {
        /// <summary>
        /// Widget name as it appears in the panel layout.
        /// Supports wildcards:
        ///   "Box1/*"            — all direct children of Box1
        ///   "@SelectedWidget"   — the widget currently selected by the user
        ///   "@SelectedWidget/*" — children of the selected widget
        /// Wildcard expansion is performed at Start() time, not at config load time.
        /// </summary>
        public string Name { get; set; }

        /// <summary>If true, a beep sound plays when this widget is highlighted.</summary>
        public bool PlayBeep { get; set; }

        /// <summary>
        /// PCode script executed when the user selects (actuates) this widget.
        /// </summary>
        public string OnSelected { get; set; }
    }
}
