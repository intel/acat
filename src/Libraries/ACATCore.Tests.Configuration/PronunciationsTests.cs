////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// PronunciationsTests.cs
//
// Unit tests for Pronunciations JSON configuration
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Configuration;
using ACAT.Core.TTSManagement;
using ACAT.Core.Utility;
using ACAT.Core.Validation;
using FluentValidation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace ACATCore.Tests.Configuration
{
    [TestClass]
    public class PronunciationsJsonTests
    {
        private string _testDirectory;

        [TestInitialize]
        public void Setup()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), "ACATTests_Pron_" + Guid.NewGuid().ToString());
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
        public void CanCreateDefaultPronunciations()
        {
            // Act
            var config = PronunciationsJson.CreateDefault();

            // Assert
            Assert.IsNotNull(config);
            Assert.IsNotNull(config.Pronunciations);
            Assert.AreEqual(0, config.Pronunciations.Count);
        }

        [TestMethod]
        public void CanSerializeToJson()
        {
            // Arrange
            var config = new PronunciationsJson();
            config.Pronunciations.Add(new PronunciationJson
            {
                Word = "github",
                Pronunciation = "git hub"
            });

            // Act
            var json = JsonSerializer.Serialize(config);

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(json));
            Assert.IsTrue(json.Contains("github"));
            Assert.IsTrue(json.Contains("git hub"));
        }

        [TestMethod]
        public void CanDeserializeFromJson()
        {
            // Arrange
            var json = @"{
                ""pronunciations"": [
                    {
                        ""word"": ""github"",
                        ""pronunciation"": ""git hub""
                    },
                    {
                        ""word"": ""linux"",
                        ""pronunciation"": ""lie nucks""
                    }
                ]
            }";

            // Act
            PronunciationsJson config = JsonSerializer.Deserialize<PronunciationsJson>(json);

            // Assert
            Assert.IsNotNull(config);
            Assert.AreEqual(2, config.Pronunciations.Count);
            Assert.AreEqual("github", config.Pronunciations[0].Word);
            Assert.AreEqual("git hub", config.Pronunciations[0].Pronunciation);
            Assert.AreEqual("linux", config.Pronunciations[1].Word);
            Assert.AreEqual("lie nucks", config.Pronunciations[1].Pronunciation);
        }

        [TestMethod]
        public void ValidatorAcceptsValidConfiguration()
        {
            // Arrange
            var validator = new PronunciationsValidator();
            var config = new PronunciationsJson();
            config.Pronunciations.Add(new PronunciationJson
            {
                Word = "github",
                Pronunciation = "git hub"
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
            var validator = new PronunciationsValidator();
            var config = new PronunciationsJson();
            config.Pronunciations.Add(new PronunciationJson
            {
                Word = "",
                Pronunciation = "test"
            });

            // Act
            ValidationResult result = validator.Validate(config);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Count > 0);
        }

        [TestMethod]
        public void ValidatorRejectsEmptyPronunciation()
        {
            // Arrange
            var validator = new PronunciationsValidator();
            var config = new PronunciationsJson();
            config.Pronunciations.Add(new PronunciationJson
            {
                Word = "test",
                Pronunciation = ""
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
            var loader = new JsonConfigurationLoader<PronunciationsJson>(new PronunciationsValidator());
            var testFile = Path.Combine(_testDirectory, "test-pronunciations.json");
            
            var config = new PronunciationsJson();
            config.Pronunciations.Add(new PronunciationJson
            {
                Word = "ACAT",
                Pronunciation = "A cat"
            });

            // Act - Save
            bool saveSuccess = loader.Save(config, testFile);

            // Act - Load
            PronunciationsJson loadedConfig = loader.Load(testFile, createDefaultOnError: false);

            // Assert
            Assert.IsTrue(saveSuccess);
            Assert.IsNotNull(loadedConfig);
            Assert.AreEqual(1, loadedConfig.Pronunciations.Count);
            Assert.AreEqual("ACAT", loadedConfig.Pronunciations[0].Word);
            Assert.AreEqual("A cat", loadedConfig.Pronunciations[0].Pronunciation);
        }

        [TestMethod]
        public void PronunciationsClassCanLoadFromJson()
        {
            // Arrange
            var testFile = Path.Combine(_testDirectory, "test-pronunciations.json");
            var json = @"{
                ""pronunciations"": [
                    {
                        ""word"": ""intel"",
                        ""pronunciation"": ""in tell""
                    }
                ]
            }";
            File.WriteAllText(testFile, json);

            var pronunciations = new Pronunciations();

            // Act
            bool loaded = pronunciations.Load(testFile);

            // Assert
            Assert.IsTrue(loaded);
            Assert.IsNotNull(pronunciations.Lookup("intel"));
            Assert.AreEqual("in tell", pronunciations.Lookup("intel").AltPronunciation);
        }

        [TestMethod]
        public void PronunciationsClassCanSaveToJson()
        {
            // Arrange
            var testFile = Path.Combine(_testDirectory, "test-pronunciations.json");
            var pronunciations = new Pronunciations();
            pronunciations.Add(new Pronunciation("microsoft", "mike row soft"));

            // Act
            bool saved = pronunciations.Save(testFile);

            // Assert
            Assert.IsTrue(saved);
            Assert.IsTrue(File.Exists(testFile));
            
            // Verify content
            var content = File.ReadAllText(testFile);
            Assert.IsTrue(content.Contains("microsoft"));
            Assert.IsTrue(content.Contains("mike row soft"));
        }

        [TestMethod]
        public void BackwardCompatibilityWithXml()
        {
            // Arrange
            var xmlFile = Path.Combine(_testDirectory, "test-pronunciations.xml");
            var xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<ACAT>
  <Pronunciations>
    <Pronunciation word=""xml"" pronunciation=""ex em el"" />
  </Pronunciations>
</ACAT>";
            File.WriteAllText(xmlFile, xml);

            var pronunciations = new Pronunciations();

            // Act
            bool loaded = pronunciations.Load(xmlFile);

            // Assert
            Assert.IsTrue(loaded);
            Assert.IsNotNull(pronunciations.Lookup("xml"));
            Assert.AreEqual("ex em el", pronunciations.Lookup("xml").AltPronunciation);
        }
    }
}
