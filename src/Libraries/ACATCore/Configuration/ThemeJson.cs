////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ThemeJson.cs
//
// JSON-serializable POCO classes for theme configuration with
// System.Text.Json attributes
//
////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ACAT.Core.Configuration
{
    /// <summary>
    /// Root configuration for ACAT theme
    /// Supports JSON serialization with System.Text.Json
    /// </summary>
    public class ThemeJson
    {
        /// <summary>
        /// Configuration file version for migration support
        /// </summary>
        [JsonPropertyName("version")]
        public string Version { get; set; } = "1.0.0";

        /// <summary>
        /// Description of the theme
        /// </summary>
        [JsonPropertyName("description")]
        [Required(ErrorMessage = "Theme description is required")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// List of color schemes for different UI elements
        /// </summary>
        [JsonPropertyName("colorSchemes")]
        [Required]
        public List<ColorSchemeJson> ColorSchemes { get; set; } = new();

        /// <summary>
        /// Factory method to create default high contrast theme
        /// </summary>
        public static ThemeJson CreateDefaultHighContrast()
        {
            return new ThemeJson
            {
                Description = "The default theme with high contrast",
                ColorSchemes = new List<ColorSchemeJson>
                {
                    ColorSchemeJson.CreateScanner(),
                    ColorSchemeJson.CreateScannerButton(),
                    ColorSchemeJson.CreateDisabledScannerButton(),
                    ColorSchemeJson.CreateDialog(),
                    ColorSchemeJson.CreateMenu(),
                    ColorSchemeJson.CreateHighContrast()
                }
            };
        }

        /// <summary>
        /// Factory method to create a light theme
        /// </summary>
        public static ThemeJson CreateLightTheme()
        {
            return new ThemeJson
            {
                Description = "Light theme with softer colors",
                ColorSchemes = new List<ColorSchemeJson>
                {
                    new ColorSchemeJson
                    {
                        Name = "Scanner",
                        Background = "#F0F0F0",
                        Foreground = "#333333",
                        HighlightBackground = "#4A90E2",
                        HighlightForeground = "White"
                    }
                }
            };
        }
    }

    /// <summary>
    /// Color scheme configuration for a specific UI element type
    /// </summary>
    public class ColorSchemeJson
    {
        /// <summary>
        /// Name of the color scheme (refers to widget type)
        /// </summary>
        [JsonPropertyName("name")]
        [Required(ErrorMessage = "Color scheme name is required")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Background color in normal state
        /// </summary>
        [JsonPropertyName("background")]
        public string Background { get; set; } = string.Empty;

        /// <summary>
        /// Foreground (text) color in normal state
        /// </summary>
        [JsonPropertyName("foreground")]
        public string Foreground { get; set; } = string.Empty;

        /// <summary>
        /// Background color when highlighted
        /// </summary>
        [JsonPropertyName("highlightBackground")]
        public string HighlightBackground { get; set; } = string.Empty;

        /// <summary>
        /// Foreground color when highlighted
        /// </summary>
        [JsonPropertyName("highlightForeground")]
        public string HighlightForeground { get; set; } = string.Empty;

        /// <summary>
        /// Background color when highlighted and selected
        /// </summary>
        [JsonPropertyName("highlightSelectedBackground")]
        public string HighlightSelectedBackground { get; set; } = string.Empty;

        /// <summary>
        /// Foreground color when highlighted and selected
        /// </summary>
        [JsonPropertyName("highlightSelectedForeground")]
        public string HighlightSelectedForeground { get; set; } = string.Empty;

        /// <summary>
        /// Background image file (if specified, background color is ignored)
        /// </summary>
        [JsonPropertyName("backgroundImage")]
        public string BackgroundImage { get; set; } = string.Empty;

        /// <summary>
        /// Background image when highlighted
        /// </summary>
        [JsonPropertyName("highlightBackgroundImage")]
        public string HighlightBackgroundImage { get; set; } = string.Empty;

        /// <summary>
        /// Factory method to create Scanner color scheme
        /// </summary>
        public static ColorSchemeJson CreateScanner()
        {
            return new ColorSchemeJson
            {
                Name = "Scanner",
                Background = "#232433",
                Foreground = "White",
                HighlightSelectedBackground = "Blue",
                HighlightSelectedForeground = "White",
                HighlightBackground = "#ffaa00",
                HighlightForeground = "#232433"
            };
        }

        /// <summary>
        /// Factory method to create ScannerButton color scheme
        /// </summary>
        public static ColorSchemeJson CreateScannerButton()
        {
            return new ColorSchemeJson
            {
                Name = "ScannerButton",
                Background = "#232433",
                Foreground = "White",
                HighlightSelectedBackground = "Blue",
                HighlightSelectedForeground = "White",
                HighlightBackground = "#ffaa00",
                HighlightForeground = "#232433"
            };
        }

        /// <summary>
        /// Factory method to create DisabledScannerButton color scheme
        /// </summary>
        public static ColorSchemeJson CreateDisabledScannerButton()
        {
            return new ColorSchemeJson
            {
                Name = "DisabledScannerButton",
                Background = "#585453",
                Foreground = "Gray",
                HighlightSelectedBackground = "Black",
                HighlightSelectedForeground = "White",
                HighlightBackground = "#FFB100",
                HighlightForeground = "Gray"
            };
        }

        /// <summary>
        /// Factory method to create Dialog color scheme
        /// </summary>
        public static ColorSchemeJson CreateDialog()
        {
            return new ColorSchemeJson
            {
                Name = "Dialog",
                Background = "#232433",
                Foreground = "White",
                HighlightSelectedBackground = "Gray",
                HighlightSelectedForeground = "White",
                HighlightBackground = "#ffaa00",
                HighlightForeground = "#232433"
            };
        }

        /// <summary>
        /// Factory method to create Menu color scheme
        /// </summary>
        public static ColorSchemeJson CreateMenu()
        {
            return new ColorSchemeJson
            {
                Name = "Menu",
                Background = "#232433",
                Foreground = "White",
                HighlightSelectedBackground = "Gray",
                HighlightSelectedForeground = "White",
                HighlightBackground = "#ffaa00",
                HighlightForeground = "#232433"
            };
        }

        /// <summary>
        /// Factory method to create HighContrast color scheme
        /// </summary>
        public static ColorSchemeJson CreateHighContrast()
        {
            return new ColorSchemeJson
            {
                Name = "HighContrast",
                Background = "#484848",
                Foreground = "White"
            };
        }
    }
}
