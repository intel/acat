////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// AnimationConfigJson.cs
//
// JSON-serializable POCO classes for standalone animation configuration.
//
// These classes mirror the AnimationConfig / AnimationSequenceConfig /
// AnimationWidgetConfig model used by ACATCore so that
// AnimationConfigProvider can load the produced .animation.json files
// with PropertyNameCaseInsensitive = true.
//
////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ACAT.ConfigMigrationTool.Configuration
{
    /// <summary>
    /// Root object for a panel's animation configuration.
    /// Serialised to <c>{panelName}.animation.json</c>.
    /// </summary>
    public class AnimationConfigJson
    {
        /// <summary>Panel name matching the PanelConfigMap key.</summary>
        [JsonPropertyName("panelName")]
        public string PanelName { get; set; } = string.Empty;

        /// <summary>
        /// Scan strategy ("auto", "manual", "step"). Defaults to "auto".
        /// </summary>
        [JsonPropertyName("scanStrategy")]
        public string ScanStrategy { get; set; } = "auto";

        /// <summary>Ordered list of animation sequences for this panel.</summary>
        [JsonPropertyName("sequences")]
        public List<AnimationSequenceConfigJson> Sequences { get; set; } = new();
    }

    /// <summary>
    /// A single named scan sequence (one <c>&lt;Animation&gt;</c> element).
    /// </summary>
    public class AnimationSequenceConfigJson
    {
        /// <summary>Unique name within the panel.</summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>True if this is the first sequence activated when the panel opens.</summary>
        [JsonPropertyName("isFirst")]
        public bool IsFirst { get; set; }

        /// <summary>True if scanning starts automatically.</summary>
        [JsonPropertyName("autoStart")]
        public bool AutoStart { get; set; }

        /// <summary>
        /// Repeat count. String to preserve variable references (e.g. "@GridScanIterations").
        /// </summary>
        [JsonPropertyName("iterations")]
        public string Iterations { get; set; } = "1";

        /// <summary>
        /// Scan step interval in ms. String to preserve variable references.
        /// </summary>
        [JsonPropertyName("scanTime")]
        public string? ScanTime { get; set; }

        /// <summary>
        /// First-widget dwell time in ms. String to preserve variable references.
        /// </summary>
        [JsonPropertyName("firstPauseTime")]
        public string? FirstPauseTime { get; set; }

        /// <summary>PCode executed when this sequence begins.</summary>
        [JsonPropertyName("onEnter")]
        public string? OnEnter { get; set; }

        /// <summary>PCode executed when all iterations complete.</summary>
        [JsonPropertyName("onEnd")]
        public string? OnEnd { get; set; }

        /// <summary>Ordered list of widgets to highlight.</summary>
        [JsonPropertyName("widgets")]
        public List<AnimationWidgetConfigJson> Widgets { get; set; } = new();
    }

    /// <summary>
    /// A single widget step in an animation sequence (one <c>&lt;Widget&gt;</c> element).
    /// </summary>
    public class AnimationWidgetConfigJson
    {
        /// <summary>Widget name as it appears in the panel layout.</summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>True if a beep plays when this widget is highlighted.</summary>
        [JsonPropertyName("playBeep")]
        public bool PlayBeep { get; set; }

        /// <summary>PCode executed when the user selects this widget.</summary>
        [JsonPropertyName("onSelected")]
        public string? OnSelected { get; set; }
    }
}
