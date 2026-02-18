////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// AbbreviationsTests.cs
//
// Unit tests for Abbreviations JSON configuration
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.AbbreviationsManagement;
using ACAT.Core.Configuration;
using ACAT.Core.Utility;
using ACAT.Core.Validation;
using FluentValidation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace ACATCore.Tests.Configuration
{
    [TestClass]
    public class AbbreviationsJsonTests
    {
        private string _testDirectory;

        [TestInitialize]
        public void Setup()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), "ACATTests_Abbr_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }

        [TestMethod]
        public void CanCreateDefaultAbbreviations()
        {
            // Act
            var config = AbbreviationsJson.CreateDefault();

            // Assert
            Assert.IsNotNull(config);
            Assert.IsNotNull(config.Abbreviations);
            Assert.AreEqual(0, config.Abbreviations.Count);
        }

        [TestMethod]
        public void CanSerializeToJson()
        {
            // Arrange
            var config = new AbbreviationsJson();
            config.Abbreviations.Add(new AbbreviationJson
            {
                Word = "btw",
                ReplaceWith = "by the way",
                Mode = "Write"
            });

            // Act
            var json = JsonSerializer.Serialize(config);

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(json));
            Assert.IsTrue(json.Contains("btw"));
            Assert.IsTrue(json.Contains("by the way"));
            Assert.IsTrue(json.Contains("Write"));
        }

        [TestMethod]
        public void CanDeserializeFromJson()
        {
            // Arrange
            var json = @"{
                ""abbreviations"": [
                    {
                        ""word"": ""btw"",
                        ""replaceWith"": ""by the way"",
                        ""mode"": ""Write""
                    },
                    {
                        ""word"": ""omg"",
                        ""replaceWith"": ""oh my goodness"",
                        ""mode"": ""Speak""
                    }
                ]
            }";

            // Act
            AbbreviationsJson config = JsonSerializer.Deserialize<AbbreviationsJson>(json);

            // Assert
            Assert.IsNotNull(config);
            Assert.AreEqual(2, config.Abbreviations.Count);
            Assert.AreEqual("btw", config.Abbreviations[0].Word);
            Assert.AreEqual("by the way", config.Abbreviations[0].ReplaceWith);
            Assert.AreEqual("Write", config.Abbreviations[0].Mode);
            Assert.AreEqual("omg", config.Abbreviations[1].Word);
            Assert.AreEqual("Speak", config.Abbreviations[1].Mode);
        }

        [TestMethod]
        public void ValidatorAcceptsValidConfiguration()
        {
            // Arrange
            var validator = new AbbreviationsValidator();
            var config = new AbbreviationsJson();
            config.Abbreviations.Add(new AbbreviationJson
            {
                Word = "btw",
                ReplaceWith = "by the way",
                Mode = "Write"
            });

            // Act
            ValidationResult result = validator.Validate(config);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void ValidatorRejectsEmptyWord()
        {
            // Arrange
            var validator = new AbbreviationsValidator();
            var config = new AbbreviationsJson();
            config.Abbreviations.Add(new AbbreviationJson
            {
                Word = "",
                ReplaceWith = "by the way",
                Mode = "Write"
            });

            // Act
            ValidationResult result = validator.Validate(config);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Count > 0);
        }

        [TestMethod]
        public void ValidatorRejectsInvalidMode()
        {
            // Arrange
            var validator = new AbbreviationsValidator();
            var config = new AbbreviationsJson();
            config.Abbreviations.Add(new AbbreviationJson
            {
                Word = "btw",
                ReplaceWith = "by the way",
                Mode = "InvalidMode"
            });

            // Act
            ValidationResult result = validator.Validate(config);

            // Assert
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void CanLoadAndSaveJsonConfiguration()
        {
            // Arrange
            var loader = new JsonConfigurationLoader<AbbreviationsJson>(new AbbreviationsValidator());
            var testFile = Path.Combine(_testDirectory, "test-abbreviations.json");
            
            var config = new AbbreviationsJson();
            config.Abbreviations.Add(new AbbreviationJson
            {
                Word = "brb",
                ReplaceWith = "be right back",
                Mode = "Write"
            });

            // Act - Save
            bool saveSuccess = loader.Save(config, testFile);

            // Act - Load
            AbbreviationsJson loadedConfig = loader.Load(testFile, createDefaultOnError: false);

            // Assert
            Assert.IsTrue(saveSuccess);
            Assert.IsNotNull(loadedConfig);
            Assert.AreEqual(1, loadedConfig.Abbreviations.Count);
            Assert.AreEqual("brb", loadedConfig.Abbreviations[0].Word);
            Assert.AreEqual("be right back", loadedConfig.Abbreviations[0].ReplaceWith);
        }

        [TestMethod]
        public void AbbreviationsClassCanLoadFromJson()
        {
            // Arrange
            var testFile = Path.Combine(_testDirectory, "test-abbreviations.json");
            var json = @"{
                ""abbreviations"": [
                    {
                        ""word"": ""lol"",
                        ""replaceWith"": ""laughing out loud"",
                        ""mode"": ""Write""
                    }
                ]
            }";
            File.WriteAllText(testFile, json);

            var abbreviations = new Abbreviations();

            // Act
            bool loaded = abbreviations.Load(testFile);

            // Assert
            Assert.IsTrue(loaded);
            Assert.IsNotNull(abbreviations.Lookup("LOL"));
            Assert.AreEqual("laughing out loud", abbreviations.Lookup("LOL").Expansion);
        }

        [TestMethod]
        public void AbbreviationsClassCanSaveToJson()
        {
            // Arrange
            var testFile = Path.Combine(_testDirectory, "test-abbreviations.json");
            var abbreviations = new Abbreviations();
            abbreviations.Add(new Abbreviation("tbd", "to be determined", Abbreviation.AbbreviationMode.Write));

            // Act
            bool saved = abbreviations.Save(testFile);

            // Assert
            Assert.IsTrue(saved);
            Assert.IsTrue(File.Exists(testFile));

            // Verify content
            // Note: Abbreviation class converts mnemonics to uppercase internally
            var content = File.ReadAllText(testFile);
            Assert.IsTrue(content.Contains("TBD"), "Mnemonic should be stored as uppercase");
            Assert.IsTrue(content.Contains("to be determined"));
        }
    }
}
