////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// XmlDeserializers.cs
//
// XML deserializers for ACAT configuration files
//
////////////////////////////////////////////////////////////////////////////

using ACAT.ConfigMigrationTool.Configuration;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ACAT.ConfigMigrationTool
{
    /// <summary>
    /// Provides methods to deserialize XML configuration files to POCOs
    /// </summary>
    public static class XmlDeserializers
    {
        /// <summary>
        /// Deserializes ActuatorSettings XML to POCO
        /// </summary>
        public static ActuatorSettingsJson DeserializeActuatorSettings(string xmlPath)
        {
            XDocument doc = XDocument.Load(xmlPath);
            var root = doc.Root;
            
            if (root == null || root.Name.LocalName != "ActuatorConfig")
            {
                throw new InvalidOperationException("Invalid ActuatorSettings XML format");
            }

            ActuatorSettingsJson result = new ActuatorSettingsJson();
            var settingsElement = root.Element("ActuatorSettings");
            
            if (settingsElement != null)
            {
                foreach (var actuatorElement in settingsElement.Elements("ActuatorSetting"))
                {
                    ActuatorSettingJson actuator = new ActuatorSettingJson
                    {
                        Name = GetElementValue(actuatorElement, "Name"),
                        Id = GetElementValue(actuatorElement, "Id"),
                        Description = GetElementValue(actuatorElement, "Description"),
                        Enabled = GetBoolValue(actuatorElement, "Enabled"),
                        ImageFileName = GetElementValue(actuatorElement, "ImageFileName")
                    };

                    var switchSettingsElement = actuatorElement.Element("SwitchSettings");
                    if (switchSettingsElement != null)
                    {
                        foreach (var switchElement in switchSettingsElement.Elements("SwitchSetting"))
                        {
                            SwitchSettingJson switchSetting = new SwitchSettingJson
                            {
                                Name = GetElementValue(switchElement, "Name"),
                                Source = GetElementValue(switchElement, "Source"),
                                Description = GetElementValue(switchElement, "Description"),
                                Enabled = GetBoolValue(switchElement, "Enabled"),
                                Actuate = GetBoolValue(switchElement, "Actuate"),
                                Command = GetElementValue(switchElement, "Command"),
                                MinHoldTime = GetElementValue(switchElement, "MinHoldTime"),
                                BeepFile = GetElementValue(switchElement, "BeepFile")
                            };
                            actuator.SwitchSettings.Add(switchSetting);
                        }
                    }

                    result.ActuatorSettings.Add(actuator);
                }
            }

            return result;
        }

        /// <summary>
        /// Deserializes Theme XML to POCO
        /// </summary>
        public static ThemeJson DeserializeTheme(string xmlPath)
        {
            XDocument doc = XDocument.Load(xmlPath);
            var root = doc.Root;
            
            if (root == null || root.Name.LocalName != "ACAT")
            {
                throw new InvalidOperationException("Invalid Theme XML format");
            }

            ThemeJson result = new ThemeJson();
            var themeElement = root.Element("Theme");
            
            if (themeElement != null)
            {
                result.Description = themeElement.Attribute("description")?.Value ?? "";
                
                var colorSchemesElement = themeElement.Element("ColorSchemes");
                if (colorSchemesElement != null)
                {
                    foreach (var schemeElement in colorSchemesElement.Elements("ColorScheme"))
                    {
                        ColorSchemeJson scheme = new ColorSchemeJson
                        {
                            Name = schemeElement.Attribute("name")?.Value ?? "",
                            Background = schemeElement.Attribute("background")?.Value ?? "",
                            Foreground = schemeElement.Attribute("foreground")?.Value ?? "",
                            HighlightBackground = schemeElement.Attribute("highlightBackground")?.Value ?? "",
                            HighlightForeground = schemeElement.Attribute("highlightForeground")?.Value ?? "",
                            HighlightSelectedBackground = schemeElement.Attribute("highlightSelectedBackground")?.Value ?? "",
                            HighlightSelectedForeground = schemeElement.Attribute("highlightSelectedForeground")?.Value ?? "",
                            BackgroundImage = schemeElement.Attribute("backgroundImage")?.Value ?? "",
                            HighlightBackgroundImage = schemeElement.Attribute("highlightBackgroundImage")?.Value ?? ""
                        };
                        result.ColorSchemes.Add(scheme);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Deserializes PanelConfig XML to POCO
        /// </summary>
        public static PanelConfigJson DeserializePanelConfig(string xmlPath)
        {
            XDocument doc = XDocument.Load(xmlPath);
            var root = doc.Root;
            
            if (root == null || root.Name.LocalName != "ACAT")
            {
                throw new InvalidOperationException("Invalid PanelConfig XML format");
            }

            PanelConfigJson result = new PanelConfigJson();

            // Parse WidgetAttributes
            var widgetAttributesElement = root.Element("WidgetAttributes");
            if (widgetAttributesElement != null)
            {
                foreach (var attrElement in widgetAttributesElement.Elements("WidgetAttribute"))
                {
                    WidgetAttributeJson attr = new WidgetAttributeJson
                    {
                        Name = attrElement.Attribute("name")?.Value ?? "",
                        Label = attrElement.Attribute("label")?.Value ?? "",
                        Value = attrElement.Attribute("value")?.Value ?? "",
                        FontName = attrElement.Attribute("fontname")?.Value ?? "",
                        FontSize = ParseFontSize(attrElement.Attribute("fontsize")?.Value ?? ""),
                        Bold = ParseBoolOrString(attrElement.Attribute("bold")?.Value ?? "false"),
                        Italic = ParseBoolOrString(attrElement.Attribute("italic")?.Value ?? "false")
                    };
                    result.WidgetAttributes.Add(attr);
                }
            }

            // Parse Layout
            var layoutElement = root.Element("Layout");
            if (layoutElement != null)
            {
                result.Layout = new LayoutJson
                {
                    ColorScheme = layoutElement.Attribute("colorScheme")?.Value ?? "Dialog",
                    Widgets = ParseWidgets(layoutElement)
                };
            }

            // Parse Animations
            var animationsElement = root.Element("Animations");
            if (animationsElement != null)
            {
                foreach (var animElement in animationsElement.Elements("Animation"))
                {
                    AnimationJson animation = new AnimationJson
                    {
                        Name = animElement.Attribute("name")?.Value ?? "",
                        Start = ParseBoolOrString(animElement.Attribute("start")?.Value ?? "false"),
                        AutoStart = ParseBoolOrString(animElement.Attribute("autoStart")?.Value ?? "false"),
                        FirstPauseTime = animElement.Attribute("firstPauseTime")?.Value ?? "",
                        OnEnter = animElement.Attribute("onEnter")?.Value ?? "",
                        ScanTime = animElement.Attribute("scanTime")?.Value ?? "",
                        Iterations = animElement.Attribute("iterations")?.Value ?? ""
                    };

                    var stepsElement = animElement.Element("AnimationSteps");
                    if (stepsElement != null)
                    {
                        foreach (var stepElement in stepsElement.Elements("AnimationStep"))
                        {
                            AnimationStepJson step = new AnimationStepJson
                            {
                                WidgetName = stepElement.Attribute("widgetName")?.Value ?? "",
                                OnSelect = stepElement.Attribute("onSelect")?.Value ?? ""
                            };
                            animation.Steps.Add(step);
                        }
                    }

                    result.Animations.Add(animation);
                }
            }

            return result;
        }

        private static List<WidgetJson> ParseWidgets(XElement parentElement)
        {
            List<WidgetJson> widgets = new List<WidgetJson>();
            
            foreach (var widgetElement in parentElement.Elements("Widget"))
            {
                WidgetJson widget = new WidgetJson
                {
                    Class = widgetElement.Attribute("class")?.Value ?? "",
                    Name = widgetElement.Attribute("name")?.Value ?? "",
                    ColorScheme = widgetElement.Attribute("colorScheme")?.Value ?? "",
                    Enabled = widgetElement.Attribute("enabled")?.Value ?? "",
                    DefaultEnabled = ParseDefaultEnabled(widgetElement.Attribute("defaultEnabled")?.Value)
                };

                // Recursively parse children
                widget.Children = ParseWidgets(widgetElement);
                widgets.Add(widget);
            }

            return widgets;
        }

        private static string GetElementValue(XElement parent, string elementName)
        {
            return parent.Element(elementName)?.Value ?? "";
        }

        private static bool GetBoolValue(XElement parent, string elementName)
        {
            var value = parent.Element(elementName)?.Value;
            if (string.IsNullOrEmpty(value))
                return false;
            
            return bool.TryParse(value, out var result) && result;
        }

        private static object ParseFontSize(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";
            
            // If it's a number, return as string (will be serialized as string)
            if (int.TryParse(value, out _))
                return value;
            
            // Return as-is for references like "@FontSize"
            return value;
        }

        private static object ParseBoolOrString(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;
            
            // Check if it's a reference (starts with & or @)
            if (value.StartsWith("&") || value.StartsWith("@"))
                return value;
            
            // Try to parse as boolean
            if (bool.TryParse(value, out var boolResult))
                return boolResult;
            
            // Return as string
            return value;
        }

        private static object? ParseDefaultEnabled(string? value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            
            if (bool.TryParse(value, out var boolResult))
                return boolResult;
            
            return value;
        }
    }
}
