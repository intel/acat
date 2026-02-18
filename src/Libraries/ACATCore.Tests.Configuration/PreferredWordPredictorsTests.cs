////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// PreferredWordPredictorsTests.cs
//
// Unit tests for PreferredWordPredictors JSON configuration
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Configuration;
using ACAT.Core.Utility;
using ACAT.Core.Validation;
using ACAT.Core.WordPredictorManagement;
using FluentValidation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace ACATCore.Tests.Configuration
{
    [TestClass]
    public class PreferredWordPredictorsJsonTests
    {
        private string _testDirectory;

        [TestInitialize]
        public void Setup()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), "ACATTests_WordPred_" + Guid.NewGuid().ToString());
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
        public void CanCreateDefaultPreferredWordPredictors()
        {
            // Act
            var config = PreferredWordPredictorsJson.CreateDefault();

            // Assert
            Assert.IsNotNull(config);
            Assert.IsNotNull(config.WordPredictors);
            Assert.AreEqual(0, config.WordPredictors.Count);
        }

        [TestMethod]
        public void CanSerializeToJson()
        {
            // Arrange
            var config = new PreferredWordPredictorsJson();
            var testGuid = Guid.NewGuid();
            config.WordPredictors.Add(new PreferredWordPredictorJson
            {
                Language = "en",
                Id = testGuid.ToString()
            });

            // Act
            var json = JsonSerializer.Serialize(config);

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(json));
            Assert.IsTrue(json.Contains("en"));
            Assert.IsTrue(json.Contains(testGuid.ToString()));
        }

        [TestMethod]
        public void CanDeserializeFromJson()
        {
            // Arrange
            var testGuid1 = Guid.NewGuid();
            var testGuid2 = Guid.NewGuid();
            var json = $@"{{
                ""wordPredictors"": [
                    {{
                        ""language"": ""en"",
                        ""id"": ""{testGuid1}""
                    }},
                    {{
                        ""language"": ""fr"",
                        ""id"": ""{testGuid2}""
                    }}
                ]
            }}";

            // Act
            PreferredWordPredictorsJson config = JsonSerializer.Deserialize<PreferredWordPredictorsJson>(json);

            // Assert
            Assert.IsNotNull(config);
            Assert.AreEqual(2, config.WordPredictors.Count);
            Assert.AreEqual("en", config.WordPredictors[0].Language);
            Assert.AreEqual(testGuid1.ToString(), config.WordPredictors[0].Id);
            Assert.AreEqual("fr", config.WordPredictors[1].Language);
            Assert.AreEqual(testGuid2.ToString(), config.WordPredictors[1].Id);
        }

        [TestMethod]
        public void ValidatorAcceptsValidConfiguration()
        {
            // Arrange
            var validator = new PreferredWordPredictorsValidator();
            var config = new PreferredWordPredictorsJson();
            config.WordPredictors.Add(new PreferredWordPredictorJson
            {
                Language = "en",
                Id = Guid.NewGuid().ToString()
            });

            // Act
            ValidationResult result = validator.Validate(config);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void ValidatorRejectsEmptyLanguage()
        {
            // Arrange
            var validator = new PreferredWordPredictorsValidator();
            var config = new PreferredWordPredictorsJson();
            config.WordPredictors.Add(new PreferredWordPredictorJson
            {
                Language = "",
                Id = Guid.NewGuid().ToString()
            });

            // Act
            ValidationResult result = validator.Validate(config);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Count > 0);
        }

        [TestMethod]
        public void ValidatorRejectsInvalidGuid()
        {
            // Arrange
            var validator = new PreferredWordPredictorsValidator();
            var config = new PreferredWordPredictorsJson();
            config.WordPredictors.Add(new PreferredWordPredictorJson
            {
                Language = "en",
                Id = "not-a-valid-guid"
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
            var loader = new JsonConfigurationLoader<PreferredWordPredictorsJson>(new PreferredWordPredictorsValidator());
            var testFile = Path.Combine(_testDirectory, "test-wordpredictors.json");
            
            var config = new PreferredWordPredictorsJson();
            var testGuid = Guid.NewGuid();
            config.WordPredictors.Add(new PreferredWordPredictorJson
            {
                Language = "es",
                Id = testGuid.ToString()
            });

            // Act - Save
            bool saveSuccess = loader.Save(config, testFile);

            // Act - Load
            PreferredWordPredictorsJson loadedConfig = loader.Load(testFile, createDefaultOnError: false);

            // Assert
            Assert.IsTrue(saveSuccess);
            Assert.IsNotNull(loadedConfig);
            Assert.AreEqual(1, loadedConfig.WordPredictors.Count);
            Assert.AreEqual("es", loadedConfig.WordPredictors[0].Language);
            Assert.AreEqual(testGuid.ToString(), loadedConfig.WordPredictors[0].Id);
        }

        [TestMethod]
        public void PreferredWordPredictorsCanLoadFromJson()
        {
            // Arrange
            var testFile = Path.Combine(_testDirectory, "test-wordpredictors.json");
            var testGuid = Guid.NewGuid();
            var json = $@"{{
                ""wordPredictors"": [
                    {{
                        ""language"": ""de"",
                        ""id"": ""{testGuid}""
                    }}
                ]
            }}";
            File.WriteAllText(testFile, json);

            PreferredWordPredictors.FilePath = testFile;

            // Act
            var config = PreferredWordPredictors.Load();

            // Assert
            Assert.IsNotNull(config);
            Assert.AreEqual(1, config.WordPredictors.Count);
            Assert.AreEqual("de", config.WordPredictors[0].Language);
            Assert.AreEqual(testGuid, config.WordPredictors[0].ID);
        }

        [TestMethod]
        public void PreferredWordPredictorsCanSaveToJson()
        {
            // Arrange
            var testFile = Path.Combine(_testDirectory, "test-wordpredictors.json");
            var testGuid = Guid.NewGuid();
            
            PreferredWordPredictors.FilePath = testFile;
            var config = new PreferredWordPredictors();
            config.WordPredictors.Add(new PreferredWordPredictor(testGuid, "it"));

            // Act
            bool saved = config.Save();

            // Assert
            Assert.IsTrue(saved);
            Assert.IsTrue(File.Exists(testFile));

            // Verify content
            var content = File.ReadAllText(testFile);
            Assert.IsTrue(content.Contains("it"));
            Assert.IsTrue(content.Contains(testGuid.ToString()));
        }

        [TestMethod]
        public void ConverterHandlesEmptyList()
        {
            // Act
            PreferredWordPredictorsJson jsonConfig = PreferredWordPredictorsConverter.ToJson(new System.Collections.Generic.List<PreferredWordPredictor>());

            // Assert
            Assert.IsNotNull(jsonConfig);
            Assert.AreEqual(0, jsonConfig.WordPredictors.Count);
        }

        [TestMethod]
        public void ConverterHandlesNullList()
        {
            // Act
            PreferredWordPredictorsJson jsonConfig = PreferredWordPredictorsConverter.ToJson(null);

            // Assert
            Assert.IsNotNull(jsonConfig);
            Assert.AreEqual(0, jsonConfig.WordPredictors.Count);
        }
    }
}
