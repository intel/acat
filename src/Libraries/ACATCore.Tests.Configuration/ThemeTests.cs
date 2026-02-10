////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ThemeTests.cs
//
// Unit tests for Theme JSON configuration
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Configuration;
using ACAT.Core.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;

namespace ACATCore.Tests.Configuration
{
    [TestClass]
    public class ThemeJsonTests
    {
        [TestMethod]
        public void CanCreateDefaultTheme()
        {
            // Act
            var theme = ThemeJson.CreateDefaultHighContrast();

            // Assert
            Assert.IsNotNull(theme);
            Assert.IsNotNull(theme.ColorSchemes);
            Assert.IsTrue(theme.ColorSchemes.Count >= 6);
            Assert.AreEqual("The default theme with high contrast", theme.Description);
        }

        [TestMethod]
        public void CanSerializeToJson()
        {
            // Arrange
            var theme = ThemeJson.CreateDefaultHighContrast();

            // Act
            var json = JsonSerializer.Serialize(theme);

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(json));
            Assert.IsTrue(json.Contains("colorSchemes"));
            Assert.IsTrue(json.Contains("Scanner"));
        }

        [TestMethod]
        public void CanDeserializeFromJson()
        {
            // Arrange
            var json = @"{
                ""description"": ""Test Theme"",
                ""colorSchemes"": [
                    {
                        ""name"": ""Scanner"",
                        ""background"": ""#232433"",
                        ""foreground"": ""White""
                    }
                ]
            }";

            // Act
            var theme = JsonSerializer.Deserialize<ThemeJson>(json);

            // Assert
            Assert.IsNotNull(theme);
            Assert.AreEqual("Test Theme", theme.Description);
            Assert.AreEqual(1, theme.ColorSchemes.Count);
            Assert.AreEqual("Scanner", theme.ColorSchemes[0].Name);
        }

        [TestMethod]
        public void FactoryMethodCreatesScanner()
        {
            // Act
            var scheme = ColorSchemeJson.CreateScanner();

            // Assert
            Assert.IsNotNull(scheme);
            Assert.AreEqual("Scanner", scheme.Name);
            Assert.AreEqual("#232433", scheme.Background);
            Assert.AreEqual("White", scheme.Foreground);
        }

        [TestMethod]
        public void FactoryMethodCreatesScannerButton()
        {
            // Act
            var scheme = ColorSchemeJson.CreateScannerButton();

            // Assert
            Assert.IsNotNull(scheme);
            Assert.AreEqual("ScannerButton", scheme.Name);
        }
    }

    [TestClass]
    public class ThemeValidatorTests
    {
        private ThemeValidator _validator;

        [TestInitialize]
        public void Setup()
        {
            _validator = new ThemeValidator();
        }

        [TestMethod]
        public void ValidThemePass()
        {
            // Arrange
            var theme = ThemeJson.CreateDefaultHighContrast();

            // Act
            var result = _validator.Validate(theme);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void EmptyDescriptionFails()
        {
            // Arrange
            var theme = new ThemeJson
            {
                Description = "",
                ColorSchemes = new System.Collections.Generic.List<ColorSchemeJson>
                {
                    ColorSchemeJson.CreateScanner()
                }
            };

            // Act
            var result = _validator.Validate(theme);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Exists(e => e.ErrorMessage.Contains("description is required")));
        }

        [TestMethod]
        public void NullColorSchemesFails()
        {
            // Arrange
            var theme = new ThemeJson
            {
                Description = "Test",
                ColorSchemes = null
            };

            // Act
            var result = _validator.Validate(theme);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Exists(e => e.ErrorMessage.Contains("cannot be null")));
        }

        [TestMethod]
        public void EmptyColorSchemesFails()
        {
            // Arrange
            var theme = new ThemeJson
            {
                Description = "Test",
                ColorSchemes = new System.Collections.Generic.List<ColorSchemeJson>()
            };

            // Act
            var result = _validator.Validate(theme);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Exists(e => e.ErrorMessage.Contains("At least one color scheme")));
        }

        [TestMethod]
        public void DuplicateColorSchemeNamesFails()
        {
            // Arrange
            var theme = new ThemeJson
            {
                Description = "Test",
                ColorSchemes = new System.Collections.Generic.List<ColorSchemeJson>
                {
                    new ColorSchemeJson { Name = "Scanner", Background = "#232433", Foreground = "White" },
                    new ColorSchemeJson { Name = "Scanner", Background = "#111111", Foreground = "Black" }
                }
            };

            // Act
            var result = _validator.Validate(theme);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Exists(e => e.ErrorMessage.Contains("must be unique")));
        }

        [TestMethod]
        public void InvalidColorFormatFails()
        {
            // Arrange
            var theme = new ThemeJson
            {
                Description = "Test",
                ColorSchemes = new System.Collections.Generic.List<ColorSchemeJson>
                {
                    new ColorSchemeJson { Name = "Scanner", Background = "not-a-color", Foreground = "White" }
                }
            };

            // Act
            var result = _validator.Validate(theme);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Exists(e => e.ErrorMessage.Contains("valid color")));
        }

        [TestMethod]
        public void MissingBackgroundAndImageFails()
        {
            // Arrange
            var theme = new ThemeJson
            {
                Description = "Test",
                ColorSchemes = new System.Collections.Generic.List<ColorSchemeJson>
                {
                    new ColorSchemeJson { Name = "Scanner", Background = "", BackgroundImage = "", Foreground = "White" }
                }
            };

            // Act
            var result = _validator.Validate(theme);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Exists(e => e.ErrorMessage.Contains("background color or background image")));
        }

        [TestMethod]
        public void ValidHexColorPasses()
        {
            // Arrange
            var theme = new ThemeJson
            {
                Description = "Test",
                ColorSchemes = new System.Collections.Generic.List<ColorSchemeJson>
                {
                    new ColorSchemeJson { Name = "Scanner", Background = "#232433", Foreground = "#FFF" }
                }
            };

            // Act
            var result = _validator.Validate(theme);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void ValidColorNamePasses()
        {
            // Arrange
            var theme = new ThemeJson
            {
                Description = "Test",
                ColorSchemes = new System.Collections.Generic.List<ColorSchemeJson>
                {
                    new ColorSchemeJson { Name = "Scanner", Background = "White", Foreground = "Black" }
                }
            };

            // Act
            var result = _validator.Validate(theme);

            // Assert
            Assert.IsTrue(result.IsValid);
        }
    }
}
