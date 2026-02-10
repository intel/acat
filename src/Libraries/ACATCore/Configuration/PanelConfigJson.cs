////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// PanelConfigJson.cs
//
// JSON-serializable POCO classes for panel configuration with
// System.Text.Json attributes
//
////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ACAT.Core.Configuration
{
    /// <summary>
    /// Root configuration for ACAT panel (scanner, keyboard, menu, or dialog)
    /// Supports JSON serialization with System.Text.Json
    /// </summary>
    public class PanelConfigJson
    {
        /// <summary>
        /// List of widget attribute definitions
        /// </summary>
        [JsonPropertyName("widgetAttributes")]
        [Required]
        public List<WidgetAttributeJson> WidgetAttributes { get; set; } = new();

        /// <summary>
        /// Layout configuration for the panel
        /// </summary>
        [JsonPropertyName("layout")]
        [Required]
        public LayoutJson Layout { get; set; } = new();

        /// <summary>
        /// Animation sequences for scanning behavior
        /// </summary>
        [JsonPropertyName("animations")]
        public List<AnimationJson> Animations { get; set; } = new();

        /// <summary>
        /// Factory method to create a simple menu panel
        /// </summary>
        public static PanelConfigJson CreateSimpleMenu()
        {
            return new PanelConfigJson
            {
                WidgetAttributes = new List<WidgetAttributeJson>
                {
                    new WidgetAttributeJson
                    {
                        Name = "MenuTitle",
                        Label = "Main Menu",
                        FontName = "Montserrat SemiBold",
                        FontSize = "22"
                    }
                },
                Layout = new LayoutJson
                {
                    ColorScheme = "Dialog",
                    Widgets = new List<WidgetJson>
                    {
                        new WidgetJson
                        {
                            Class = "RowWidget",
                            Name = "TitleRow",
                            Children = new List<WidgetJson>
                            {
                                new WidgetJson
                                {
                                    Class = "ScannerButton",
                                    Name = "MenuTitle",
                                    ColorScheme = "MenuTitle"
                                }
                            }
                        }
                    }
                }
            };
        }
    }

    /// <summary>
    /// Attributes for a widget (label, value, font settings)
    /// </summary>
    public class WidgetAttributeJson
    {
        /// <summary>
        /// Name of the widget attribute
        /// </summary>
        [JsonPropertyName("name")]
        [Required(ErrorMessage = "Widget attribute name is required")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Display label for the widget
        /// </summary>
        [JsonPropertyName("label")]
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// Value or command associated with the widget
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// Font family name
        /// </summary>
        [JsonPropertyName("fontName")]
        public string FontName { get; set; } = string.Empty;

        /// <summary>
        /// Font size in points
        /// </summary>
        [JsonPropertyName("fontSize")]
        public string FontSize { get; set; } = string.Empty;

        /// <summary>
        /// Whether text is bold
        /// </summary>
        [JsonPropertyName("bold")]
        public object Bold { get; set; } = false;

        /// <summary>
        /// Whether text is italic
        /// </summary>
        [JsonPropertyName("italic")]
        public object Italic { get; set; } = false;
    }

    /// <summary>
    /// Layout configuration for the panel
    /// </summary>
    public class LayoutJson
    {
        /// <summary>
        /// Default color scheme for the layout
        /// </summary>
        [JsonPropertyName("colorScheme")]
        [Required(ErrorMessage = "Color scheme is required")]
        public string ColorScheme { get; set; } = "Dialog";

        /// <summary>
        /// Hierarchical widget tree
        /// </summary>
        [JsonPropertyName("widgets")]
        public List<WidgetJson> Widgets { get; set; } = new();
    }

    /// <summary>
    /// UI widget element (button, container, etc.)
    /// </summary>
    public class WidgetJson
    {
        /// <summary>
        /// Widget class type
        /// </summary>
        [JsonPropertyName("class")]
        [Required(ErrorMessage = "Widget class is required")]
        public string Class { get; set; } = string.Empty;

        /// <summary>
        /// Unique name of the widget
        /// </summary>
        [JsonPropertyName("name")]
        [Required(ErrorMessage = "Widget name is required")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Color scheme to apply to this widget
        /// </summary>
        [JsonPropertyName("colorScheme")]
        public string ColorScheme { get; set; } = string.Empty;

        /// <summary>
        /// Whether widget is enabled
        /// </summary>
        [JsonPropertyName("enabled")]
        public string Enabled { get; set; } = string.Empty;

        /// <summary>
        /// Default enabled state
        /// </summary>
        [JsonPropertyName("defaultEnabled")]
        public object DefaultEnabled { get; set; } = null;

        /// <summary>
        /// Child widgets for container widgets
        /// </summary>
        [JsonPropertyName("children")]
        public List<WidgetJson> Children { get; set; } = new();
    }

    /// <summary>
    /// Animation sequence configuration for scanning
    /// </summary>
    public class AnimationJson
    {
        /// <summary>
        /// Name of the animation
        /// </summary>
        [JsonPropertyName("name")]
        [Required(ErrorMessage = "Animation name is required")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Whether this is the starting animation
        /// </summary>
        [JsonPropertyName("start")]
        public object Start { get; set; } = false;

        /// <summary>
        /// Whether animation starts automatically
        /// </summary>
        [JsonPropertyName("autoStart")]
        public object AutoStart { get; set; } = false;

        /// <summary>
        /// Initial pause time before animation starts
        /// </summary>
        [JsonPropertyName("firstPauseTime")]
        public string FirstPauseTime { get; set; } = string.Empty;

        /// <summary>
        /// Action when entering animation
        /// </summary>
        [JsonPropertyName("onEnter")]
        public string OnEnter { get; set; } = string.Empty;

        /// <summary>
        /// Time for each scan step
        /// </summary>
        [JsonPropertyName("scanTime")]
        public string ScanTime { get; set; } = string.Empty;

        /// <summary>
        /// Number of scan iterations
        /// </summary>
        [JsonPropertyName("iterations")]
        public string Iterations { get; set; } = string.Empty;

        /// <summary>
        /// Animation steps
        /// </summary>
        [JsonPropertyName("steps")]
        public List<AnimationStepJson> Steps { get; set; } = new();
    }

    /// <summary>
    /// A single step in an animation sequence
    /// </summary>
    public class AnimationStepJson
    {
        /// <summary>
        /// Name of the widget to animate
        /// </summary>
        [JsonPropertyName("widgetName")]
        [Required(ErrorMessage = "Widget name is required")]
        public string WidgetName { get; set; } = string.Empty;

        /// <summary>
        /// Action to perform when widget is selected
        /// </summary>
        [JsonPropertyName("onSelect")]
        public string OnSelect { get; set; } = string.Empty;
    }
}
