////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// PanelConfigTests.cs
//
// Unit tests for PanelConfig JSON configuration
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Configuration;
using ACAT.Core.Validation;
using ACAT.Core.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentValidation.Results;
//using System.Text.Json;

namespace ACATCore.Tests.Configuration
{
    [TestClass]
    public class PanelConfigJsonTests
    {
        [TestMethod]
        public void CanCreateSimpleMenu()
        {
            // Act
            var panel = PanelConfigJson.CreateSimpleMenu();

            // Assert
            Assert.IsNotNull(panel);
            Assert.IsNotNull(panel.Layout);
            Assert.IsNotNull(panel.WidgetAttributes);
            Assert.IsNotNull(panel.Animations);
        }

        [TestMethod]
        public void CanSerializeToJson()
        {
            // Arrange
            var panel = PanelConfigJson.CreateSimpleMenu();

            // Act
            var json = JsonSerializer.Serialize(panel);

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(json));
            Assert.IsTrue(json.Contains("widgetAttributes"));
            Assert.IsTrue(json.Contains("layout"));
        }

        [TestMethod]
        public void CanDeserializeFromJson()
        {
            // Arrange
            var json = @"{
                ""widgetAttributes"": [],
                ""layout"": {
                    ""colorScheme"": ""Dialog"",
                    ""widgets"": []
                },
                ""animations"": []
            }";

            // Act
            PanelConfigJson panel = JsonSerializer.Deserialize<PanelConfigJson>(json);

            // Assert
            Assert.IsNotNull(panel);
            Assert.IsNotNull(panel.WidgetAttributes);
            Assert.IsNotNull(panel.Layout);
            Assert.AreEqual("Dialog", panel.Layout.ColorScheme);
        }

        [TestMethod]
        public void CanDeserializeComplexPanel()
        {
            // Arrange
            var json = @"{
                ""widgetAttributes"": [
                    {
                        ""name"": ""Button1"",
                        ""label"": ""OK"",
                        ""value"": ""@CmdOK"",
                        ""fontName"": ""Arial"",
                        ""fontSize"": 12,
                        ""bold"": true
                    }
                ],
                ""layout"": {
                    ""colorScheme"": ""Dialog"",
                    ""widgets"": [
                        {
                            ""class"": ""RowWidget"",
                            ""name"": ""Row1"",
                            ""children"": [
                                {
                                    ""class"": ""ScannerButton"",
                                    ""name"": ""Button1"",
                                    ""colorScheme"": ""ScannerButton""
                                }
                            ]
                        }
                    ]
                },
                ""animations"": []
            }";

            // Act
            PanelConfigJson panel = JsonSerializer.Deserialize<PanelConfigJson>(json);

            // Assert
            Assert.IsNotNull(panel);
            Assert.AreEqual(1, panel.WidgetAttributes.Count);
            Assert.AreEqual("Button1", panel.WidgetAttributes[0].Name);
            Assert.AreEqual(1, panel.Layout.Widgets.Count);
            Assert.AreEqual("RowWidget", panel.Layout.Widgets[0].Class);
            Assert.AreEqual(1, panel.Layout.Widgets[0].Children.Count);
        }

        [TestMethod]
        public void CanDeserializeFontSizeAsString()
        {
            // Arrange
            var json = @"{
                ""widgetAttributes"": [
                    {
                        ""name"": ""Button1"",
                        ""fontSize"": ""16pt""
                    }
                ],
                ""layout"": {
                    ""colorScheme"": ""Dialog"",
                    ""widgets"": []
                },
                ""animations"": []
            }";

            // Act
            PanelConfigJson panel = JsonSerializer.Deserialize<PanelConfigJson>(json);

            // Assert
            Assert.IsNotNull(panel);
            Assert.IsNotNull(panel.WidgetAttributes[0].FontSize);
            // When deserialized as object, strings become JsonElement
            var fontSize = panel.WidgetAttributes[0].FontSize.ToString();
            Assert.AreEqual("16pt", fontSize);
        }

        [TestMethod]
        public void CanDeserializeFontSizeAsNumber()
        {
            // Arrange
            var json = @"{
                ""widgetAttributes"": [
                    {
                        ""name"": ""Button1"",
                        ""fontSize"": 16
                    }
                ],
                ""layout"": {
                    ""colorScheme"": ""Dialog"",
                    ""widgets"": []
                },
                ""animations"": []
            }";

            // Act
            PanelConfigJson panel = JsonSerializer.Deserialize<PanelConfigJson>(json);

            // Assert
            Assert.IsNotNull(panel);
            Assert.IsNotNull(panel.WidgetAttributes[0].FontSize);
            // When deserialized as object, numbers become JsonElement
            var fontSizeElement = (System.Text.Json.JsonElement)panel.WidgetAttributes[0].FontSize;
            Assert.AreEqual(16, fontSizeElement.GetInt32());
        }
    }

    [TestClass]
    public class PanelConfigValidatorTests
    {
        private PanelConfigValidator _validator;

        [TestInitialize]
        public void Setup()
        {
            _validator = new PanelConfigValidator();
        }

        [TestMethod]
        public void ValidPanelConfigPasses()
        {
            // Arrange
            var panel = PanelConfigJson.CreateSimpleMenu();

            // Act
            ValidationResult result = _validator.Validate(panel);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void NullWidgetAttributesFails()
        {
            // Arrange
            var panel = new PanelConfigJson
            {
                WidgetAttributes = null,
                Layout = new LayoutJson { ColorScheme = "Dialog" }
            };

            // Act
            ValidationResult result = _validator.Validate(panel);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Exists(e => e.ErrorMessage.Contains("cannot be null")));
        }

        [TestMethod]
        public void NullLayoutFails()
        {
            // Arrange
            var panel = new PanelConfigJson
            {
                Layout = null
            };

            // Act
            ValidationResult result = _validator.Validate(panel);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Exists(e => e.ErrorMessage.Contains("Layout is required")));
        }

        [TestMethod]
        public void EmptyColorSchemeFails()
        {
            // Arrange
            var panel = new PanelConfigJson
            {
                Layout = new LayoutJson { ColorScheme = "" }
            };

            // Act
            ValidationResult result = _validator.Validate(panel);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Exists(e => e.ErrorMessage.Contains("Color scheme is required")));
        }

        [TestMethod]
        public void EmptyWidgetClassFails()
        {
            // Arrange
            var panel = new PanelConfigJson
            {
                Layout = new LayoutJson
                {
                    ColorScheme = "Dialog",
                    Widgets = new System.Collections.Generic.List<WidgetJson>
                    {
                        new WidgetJson { Class = "", Name = "Widget1" }
                    }
                }
            };

            // Act
            ValidationResult result = _validator.Validate(panel);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Exists(e => e.ErrorMessage.Contains("Widget class is required")));
        }

        [TestMethod]
        public void EmptyWidgetNameFails()
        {
            // Arrange
            var panel = new PanelConfigJson
            {
                Layout = new LayoutJson
                {
                    ColorScheme = "Dialog",
                    Widgets = new System.Collections.Generic.List<WidgetJson>
                    {
                        new WidgetJson { Class = "RowWidget", Name = "" }
                    }
                }
            };

            // Act
            ValidationResult result = _validator.Validate(panel);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Exists(e => e.ErrorMessage.Contains("Widget name is required")));
        }

        [TestMethod]
        public void DuplicateWidgetAttributeNamesFails()
        {
            // Arrange
            var panel = new PanelConfigJson
            {
                WidgetAttributes = new System.Collections.Generic.List<WidgetAttributeJson>
                {
                    new WidgetAttributeJson { Name = "Button1", Label = "OK" },
                    new WidgetAttributeJson { Name = "Button1", Label = "Cancel" }
                },
                Layout = new LayoutJson { ColorScheme = "Dialog" }
            };

            // Act
            ValidationResult result = _validator.Validate(panel);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Exists(e => e.ErrorMessage.Contains("Widget attribute names must be unique")));
        }

        [TestMethod]
        public void DuplicateAnimationNamesFails()
        {
            // Arrange
            var panel = new PanelConfigJson
            {
                Layout = new LayoutJson { ColorScheme = "Dialog" },
                Animations = new System.Collections.Generic.List<AnimationJson>
                {
                    new AnimationJson { Name = "Anim1" },
                    new AnimationJson { Name = "Anim1" }
                }
            };

            // Act
            ValidationResult result = _validator.Validate(panel);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Exists(e => e.ErrorMessage.Contains("Animation names must be unique")));
        }

        [TestMethod]
        public void EmptyAnimationStepsFails()
        {
            // Arrange
            var panel = new PanelConfigJson
            {
                Layout = new LayoutJson { ColorScheme = "Dialog" },
                Animations = new System.Collections.Generic.List<AnimationJson>
                {
                    new AnimationJson
                    {
                        Name = "Anim1",
                        Steps = new System.Collections.Generic.List<AnimationStepJson>()
                    }
                }
            };

            // Act
            ValidationResult result = _validator.Validate(panel);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Exists(e => e.ErrorMessage.Contains("at least one step")));
        }

        [TestMethod]
        public void EmptyAnimationStepWidgetNameFails()
        {
            // Arrange
            var panel = new PanelConfigJson
            {
                Layout = new LayoutJson { ColorScheme = "Dialog" },
                Animations = new System.Collections.Generic.List<AnimationJson>
                {
                    new AnimationJson
                    {
                        Name = "Anim1",
                        Steps = new System.Collections.Generic.List<AnimationStepJson>
                        {
                            new AnimationStepJson { WidgetName = "" }
                        }
                    }
                }
            };

            // Act
            ValidationResult result = _validator.Validate(panel);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Exists(e => e.ErrorMessage.Contains("Widget name is required")));
        }
    }
}
