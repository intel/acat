////////////////////////////////////////////////////////////////////////////
// <copyright file="PresageWordPredictor.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2017 Intel Corporation 
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
////////////////////////////////////////////////////////////////////////////

using ACAT.Extensions.Default.WordPredictors.PresageBase;
using ACAT.Lib.Core.PreferencesManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ACAT.Extensions.Default.WordPredictors.de.Presage
{
    /// <summary>
    /// German language word prediction extension.
    /// Uses the Presage word predictor  for next word prediction.
    /// Supports NGRAM predictor only.
    /// Presage is an intelligent predictive
    /// text engine created by Matteo Vescovi.
    /// http://presage.sourceforge.net/
    /// </summary>
    [DescriptorAttribute("F5FCDDB0-4D62-49D7-A9DC-46EEF0B4FFB9",
                            "Presage Word Predictor (Portuguese)",
                            "German word predictor based on Presage intelligent predictive text engine")]
    public class PresageWordPredictor : PresageWordPredictorBase
    {
        /// <summary>
        /// Name of the preferences file
        /// </summary>
        private const String SettingsFileName = "PresageWordPredictorSettings.xml";

        /// <summary>
        /// Holds preferences for this extension
        /// </summary>
        private Settings _settings;

        /// <summary>
        /// Initializes and instance of the class
        /// </summary>
        public PresageWordPredictor()
        {
            Settings.PreferencesFilePath = getUserRelativePath("de", SettingsFileName, true);

            _settings = Settings.Load();

            presageSettings = _settings;
        }

        /// <summary>
        /// Returns the default preferences object for the word predictor
        /// </summary>
        /// <returns>default preferences object</returns>
        public override IPreferences GetDefaultPreferences()
        {
            return PreferencesBase.LoadDefaults<Settings>();
        }

        /// <summary>
        /// Returns the preferences object for the word predictor
        /// </summary>
        /// <returns>preferences object</returns>
        public override IPreferences GetPreferences()
        {
            return _settings;
        }

        /// <summary>
        /// Initializes the presage config file with the location of
        /// the database files.
        /// </summary>
        /// <param name="ci">Culture info (must be English)</param>
        /// <returns>true on success</returns>
        protected override bool initDatabase(CultureInfo ci)
        {
            bool retVal = true;

            try
            {
                if (ci.TwoLetterISOLanguageName.ToLower() != "de")
                {
                    return false;
                }

                // check if presage database file for the specified language exists.
                var dbFileName = getDatabaseFilePath(ci);
                if (String.IsNullOrEmpty(dbFileName))
                {
                    return false;
                }

                presage.set_config("Presage.Predictors.DefaultSmoothedNgramPredictor.DBFILENAME", dbFileName);

                var learningdbFileName = getLearningDBFilePath(ci.TwoLetterISOLanguageName, _settings.LearningDatabaseFileName);
                presage.set_config("Presage.Predictors.UserSmoothedNgramPredictor.DBFILENAME", learningdbFileName);

                presage.set_config("Presage.PredictorRegistry.PREDICTORS",
                        "DefaultSmoothedNgramPredictor UserSmoothedNgramPredictor DefaultRecencyPredictor");

                presage.set_config("Presage.Selector.REPEAT_SUGGESTIONS", "yes");
                presage.set_config("Presage.ContextTracker.ONLINE_LEARNING", "no");
            }
            catch (Exception ex)
            {
                Log.Debug("Error initializing Presage. " + ex);
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Adds the specified text to the user's personal
        /// word prediction model to learn the user's writing
        /// style.  This makes word prediciton more relevant.
        /// </summary>
        /// <param name="text">Text to add</param>
        /// <returns>true on success</returns>
        protected override bool learn(String text)
        {
            bool result = false;

            try
            {
                presage.learn(UTF8EncodingToDefault(text));
                result = true;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }

            return result;
        }

        /// <summary>
        /// Returns a list of next word predictions based on the context
        /// from the previous words in the sentence.  The number of words
        /// returned is set by the PredictionWordCount property
        /// </summary>
        /// <param name="prevWords">Previous words in the sentence</param>
        /// <param name="currentWord">current word (may be partially spelt out</param>
        /// <param name="success">true if the function was successsful</param>
        /// <returns>A list of predicted words</returns>
        protected override IEnumerable<String> predict(String prevWords, String currentWord, ref bool success)
        {
            Log.Debug("Predict for: " + prevWords + " " + currentWord);

            var retVal = new List<string>();

            success = true;

            try
            {
                prevWords = UTF8EncodingToDefault(prevWords);
                currentWord = UTF8EncodingToDefault(currentWord);

                string[] prediction = presage.predict(prevWords, currentWord);

                for (int ii = 0; ii < prediction.Length; ii++)
                {
                    prediction[ii] = defaultEncodingToUTF8(prediction[ii]);
                }

                var predictionList = prediction.ToList();

                for (int count = 0, ii = 0; count < PredictionWordCount && ii < predictionList.Count(); ii++)
                {
                    if (matchPrefix(currentWord, predictionList[ii]))
                    {
                        //Log.Debug(String.Format("Prediction["+ ii + "] = " + predictions[ii].Term));
                        retVal.Add(predictionList[ii]);
                        count++;
                    }
                }
            }
            catch (Exception ex)
            {
                success = false;
                Log.Debug("Presage Predict Exception " + ex);
                retVal = new List<string>();
            }

            return retVal;
        }

        /// <summary>
        /// Returns the Presage database file name (full path)
        /// for the specified language. Returns empty string if
        /// file does not exist
        /// </summary>
        /// <returns>full path to the file</returns>
        private String getDatabaseFilePath(CultureInfo ci)
        {
            String path = getAppRelativeDBPath(ci.Name, _settings.DatabaseFileName);
            if (!File.Exists(path))
            {
                path = getAppRelativeDBPath(ci.TwoLetterISOLanguageName, _settings.DatabaseFileName);
                if (!File.Exists(path))
                {
                    return String.Empty;
                }
            }

            return path;
        }
    }
}