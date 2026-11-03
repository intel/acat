////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ConfigurationBuilder.cs
//
// Fluent builder for constructing ACAT configuration objects used in tests.
// Supports AbbreviationsJson, ActuatorSettingsJson, and PanelConfigJson.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Configuration;
using System.Collections.Generic;

namespace ACATCore.Tests.Builders
{
    /// <summary>
    /// Fluent builder for <see cref="AbbreviationsJson"/> test data.
    /// </summary>
    public sealed class AbbreviationsConfigurationBuilder
    {
        private readonly List<AbbreviationJson> _abbreviations = new List<AbbreviationJson>();

        /// <summary>
        /// Adds an abbreviation entry.
        /// </summary>
        public AbbreviationsConfigurationBuilder WithAbbreviation(string word, string replaceWith, string mode = "Write")
        {
            _abbreviations.Add(new AbbreviationJson
            {
                Word = word,
                ReplaceWith = replaceWith,
                Mode = mode
            });
            return this;
        }

        /// <summary>
        /// Builds the <see cref="AbbreviationsJson"/> instance.
        /// </summary>
        public AbbreviationsJson Build()
        {
            var config = new AbbreviationsJson();
            config.Abbreviations.AddRange(_abbreviations);
            return config;
        }

        /// <summary>Returns a builder pre-populated with common test abbreviations.</summary>
        public static AbbreviationsConfigurationBuilder WithDefaults()
        {
            return new AbbreviationsConfigurationBuilder()
                .WithAbbreviation("btw", "by the way", "Write")
                .WithAbbreviation("omg", "oh my goodness", "Speak");
        }
    }

    /// <summary>
    /// Fluent builder for <see cref="ActuatorSettingsJson"/> test data.
    /// </summary>
    public sealed class ActuatorSettingsConfigurationBuilder
    {
        private readonly List<ActuatorSettingJson> _actuators = new List<ActuatorSettingJson>();

        /// <summary>
        /// Adds a keyboard actuator with optional overrides.
        /// </summary>
        public ActuatorSettingsConfigurationBuilder WithKeyboardActuator(string name = "Keyboard")
        {
            var actuator = ActuatorSettingJson.CreateKeyboardActuator();
            actuator.Name = name;
            _actuators.Add(actuator);
            return this;
        }

        /// <summary>
        /// Adds a custom actuator entry.
        /// </summary>
        public ActuatorSettingsConfigurationBuilder WithActuator(ActuatorSettingJson actuator)
        {
            _actuators.Add(actuator);
            return this;
        }

        /// <summary>
        /// Builds the <see cref="ActuatorSettingsJson"/> instance.
        /// </summary>
        public ActuatorSettingsJson Build()
        {
            return new ActuatorSettingsJson
            {
                ActuatorSettings = new List<ActuatorSettingJson>(_actuators)
            };
        }

        /// <summary>Returns a builder pre-populated with default keyboard settings.</summary>
        public static ActuatorSettingsConfigurationBuilder WithDefaults()
        {
            return new ActuatorSettingsConfigurationBuilder()
                .WithKeyboardActuator();
        }
    }

    /// <summary>
    /// Fluent builder for <see cref="PanelConfigJson"/> test data.
    /// </summary>
    public sealed class PanelConfigurationBuilder
    {
        private string _colorScheme = "Default";
        private readonly List<WidgetAttributeJson> _widgetAttributes = new List<WidgetAttributeJson>();
        private readonly List<AnimationJson> _animations = new List<AnimationJson>();

        /// <summary>Sets the color scheme for the panel layout.</summary>
        public PanelConfigurationBuilder WithColorScheme(string colorScheme)
        {
            _colorScheme = colorScheme;
            return this;
        }

        /// <summary>Adds a widget attribute definition.</summary>
        public PanelConfigurationBuilder WithWidgetAttribute(string name, string label, string fontName = "Arial", string fontSize = "12")
        {
            _widgetAttributes.Add(new WidgetAttributeJson
            {
                Name = name,
                Label = label,
                FontName = fontName,
                FontSize = fontSize
            });
            return this;
        }

        /// <summary>Adds an animation sequence.</summary>
        public PanelConfigurationBuilder WithAnimation(AnimationJson animation)
        {
            _animations.Add(animation);
            return this;
        }

        /// <summary>Builds the <see cref="PanelConfigJson"/> instance.</summary>
        public PanelConfigJson Build()
        {
            return new PanelConfigJson
            {
                WidgetAttributes = new List<WidgetAttributeJson>(_widgetAttributes),
                Layout = new LayoutJson { ColorScheme = _colorScheme },
                Animations = new List<AnimationJson>(_animations)
            };
        }

        /// <summary>Returns a builder pre-configured as a simple menu panel.</summary>
        public static PanelConfigurationBuilder AsSimpleMenu()
        {
            return new PanelConfigurationBuilder()
                .WithColorScheme("Dialog")
                .WithWidgetAttribute("MenuTitle", "Main Menu", "Montserrat SemiBold", "22");
        }
    }
}
