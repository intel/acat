////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Configuration;
using ACAT.Core.PreferencesManagement;
using ACAT.Core.Utility;
using ACAT.Core.Validation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace ACAT.Core.WordPredictorManagement
{
    /// <summary>
    /// Maintains a list of preferred word predictors
    /// specific for each culture (language)
    /// The class is serialized to a file and loaded
    /// from a file as well
    /// </summary>
    [Serializable]
    public class PreferredWordPredictors : PreferencesBase
    {
        private static readonly ILogger<PreferredWordPredictors> _logger = LoggingConfiguration.CreateLogger<PreferredWordPredictors>();

        /// <summary>
        /// Path to the file to serialize to
        /// </summary>
        [NonSerialized, XmlIgnore]
        public static String FilePath;

        /// <summary>
        /// List of preferred word predictors
        /// </summary>
        public List<PreferredWordPredictor> WordPredictors;

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public PreferredWordPredictors()
        {
            WordPredictors = new List<PreferredWordPredictor>();
        }

        /// <summary>
        /// Returns the list of the preferred word predictors
        /// </summary>
        public IEnumerable<PreferredWordPredictor> List
        {
            get { return WordPredictors; }
        }

        /// <summary>
        /// Deserializes list of word predictors from JSON file and
        /// returns an instance of this class
        /// </summary>
        /// <returns>an object of this class</returns>
        public static PreferredWordPredictors Load()
        {
            return LoadFromJson(FilePath);
        }

        /// <summary>
        /// Loads settings from JSON file with validation
        /// </summary>
        /// <param name="filePath">Path to JSON configuration file</param>
        /// <returns>PreferredWordPredictors object</returns>
        private static PreferredWordPredictors LoadFromJson(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                _logger.LogError("PreferredWordPredictors FilePath is null or empty");
                return LoadDefaults<PreferredWordPredictors>();
            }

            try
            {
                // Use JsonConfigurationLoader with validation
                var validator = new PreferredWordPredictorsValidator();
                var loader = new JsonConfigurationLoader<PreferredWordPredictorsJson>(validator, _logger);

                var jsonSettings = loader.Load(filePath, createDefaultOnError: true);

                if (jsonSettings == null)
                {
                    _logger.LogWarning("Failed to load JSON settings, using defaults");
                    return LoadDefaults<PreferredWordPredictors>();
                }

                // Convert JSON model to legacy model
                var config = new PreferredWordPredictors();
                config.WordPredictors = PreferredWordPredictorsConverter.FromJson(jsonSettings);

                _logger.LogInformation("Successfully loaded {Count} preferred word predictor(s) from JSON", config.WordPredictors.Count);

                return config;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading preferred word predictors from JSON: {FilePath}", filePath);
                return LoadDefaults<PreferredWordPredictors>();
            }
        }

        /// <summary>
        /// Returns the ID of the preferred word predictor
        /// for the specified culture
        /// </summary>
        /// <param name="ci">culture</param>
        /// <returns>id, guid.empty if none found</returns>
        public Guid GetByCulture(System.Globalization.CultureInfo ci)
        {
            if (ci == null)
            {
                return getByLanguage(String.Empty);
            }

            var guid = getByLanguage(ci.TwoLetterISOLanguageName);
            return guid;
        }

        public override bool ResetToDefault()
        {
            var tmp = LoadDefaults<PreferredWordPredictors>();
            var res = SaveToJson(tmp, FilePath);
            Load();

            return res;
        }

        /// <summary>
        /// Persists this object to a JSON file
        /// </summary>
        /// <returns>true on success</returns>
        public override bool Save()
        {
            return !string.IsNullOrEmpty(FilePath) && SaveToJson(this, FilePath);
        }

        /// <summary>
        /// Saves settings to JSON file with validation
        /// </summary>
        /// <param name="config">Configuration to save</param>
        /// <param name="filePath">Path to JSON file</param>
        /// <returns>True if successful</returns>
        private static bool SaveToJson(PreferredWordPredictors config, string filePath)
        {
            if (config == null)
            {
                _logger.LogError("Cannot save null PreferredWordPredictors");
                return false;
            }

            if (string.IsNullOrEmpty(filePath))
            {
                _logger.LogError("PreferredWordPredictors FilePath is null or empty");
                return false;
            }

            try
            {
                // Convert legacy model to JSON model
                var jsonSettings = PreferredWordPredictorsConverter.ToJson(config.WordPredictors);

                // Use JsonConfigurationLoader with validation
                var validator = new PreferredWordPredictorsValidator();
                var loader = new JsonConfigurationLoader<PreferredWordPredictorsJson>(validator, _logger);

                bool success = loader.Save(jsonSettings, filePath);

                if (success)
                {
                    _logger.LogInformation("Successfully saved preferred word predictors to JSON: {FilePath}", filePath);
                }
                else
                {
                    _logger.LogError("Failed to save preferred word predictors to JSON: {FilePath}", filePath);
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving preferred word predictors to JSON: {FilePath}", filePath);
                return false;
            }
        }

        /// <summary>
        /// Sets the specified id of the word predictor as the
        /// default for the language
        /// </summary>
        /// <param name="language">Language (culture)</param>
        /// <param name="guid">ID of the word predictor</param>
        /// <returns></returns>
        public bool SetAsDefault(String language, Guid guid)
        {
            var preferredWP = WordPredictors.FirstOrDefault(wordPredictor => String.Compare(language, wordPredictor.Language, true) == 0);
            if (preferredWP != null)
            {
                preferredWP.ID = guid;
            }
            else
            {
                WordPredictors.Add(new PreferredWordPredictor(guid, language));
            }

            return true;
        }

        /// <summary>
        /// Gets the preferred word predictor for the specified
        /// language
        /// </summary>
        /// <param name="language">Language (culture)</param>
        /// <returns>ID, Guid.empty if none found</returns>
        private Guid getByLanguage(String language)
        {
            foreach (var preferredWordPredictor in WordPredictors)
            {
                if (String.Compare(preferredWordPredictor.Language, language, true) == 0)
                {
                    return preferredWordPredictor.ID;
                }
            }

            return Guid.Empty;
        }
    }
}