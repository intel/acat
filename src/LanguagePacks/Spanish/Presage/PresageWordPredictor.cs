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

namespace ACAT.Extensions.Default.WordPredictors.es.Presage
{
    /// <summary>
    /// Spanish language word prediction extension.
    /// Uses the Presage word predictor  for next word prediction.
    /// Supports both ARPA-based database and NGRAM database
    /// selectable from the Settings for this extension
    /// Presage is an intelligent predictive
    /// text engine created by Matteo Vescovi.
    /// http://presage.sourceforge.net/
    /// </summary>
    [DescriptorAttribute("1DFDB82B-EA60-4C03-B33E-45A18E9D9074",
                            "Presage Word Predictor (Spanish)",
                            "Spanish word predictor based on Presage intelligent predictive text engine")]
    public class PresageWordPredictor : PresageWordPredictorBase
    {
        /// <summary>
        /// Name of the preferences file
        /// </summary>
        private const String SettingsFileName = "PresageWordPredictorSettings.xml";

        /// <summary>
        /// List of chars to be removed
        /// </summary>
        private string _removeChars = ",.\"!?'¿¡";

        /// <summary>
        /// The preferences object
        /// </summary>
        private Settings _settings;

        /// <summary>
        /// Initializes and instance of the class
        /// </summary>
        public PresageWordPredictor()
        {
            Settings.PreferencesFilePath = getUserRelativePath("es", SettingsFileName, true);

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
        /// Returns the Presage database file name (full path)
        /// for the specified language and for the specified file.
        /// Returns empty string if file does not exist.
        /// </summary>
        /// <returns>full path to the file</returns>
        protected override String getDatabaseFilePath(CultureInfo ci, String dbFileName)
        {
            String path = getAppRelativeDBPath(ci.Name, dbFileName);
            if (!File.Exists(path))
            {
                path = getAppRelativeDBPath(ci.TwoLetterISOLanguageName, dbFileName);
                if (!File.Exists(path))
                {
                    return String.Empty;
                }
            }

            return path;
        }

        /// <summary>
        /// Initializes the presage config file with the location of
        /// the database files. Supports both ARPA predictor and
        /// NGRAM predictor
        /// </summary>
        /// <param name="ci">Culture info (must be English)</param>
        /// <returns>true on success</returns>
        protected override bool initDatabase(CultureInfo ci)
        {
            bool retVal = true;

            try
            {
                if (ci.TwoLetterISOLanguageName.ToLower() != "es")
                {
                    return false;
                }

                if (_settings.UseARPAPredictor)
                {
                    var dbFileName = getDatabaseFilePath(ci, _settings.ARPAPredictorDatabaseFileName);
                    if (String.IsNullOrEmpty(dbFileName))
                    {
                        return false;
                    }

                    Log.Debug("Presage (Spanish). ARPA filename: " + dbFileName);

                    presage.set_config("Presage.Predictors.DefaultARPAPredictor.ARPAFILENAME", dbFileName);

                    dbFileName = getDatabaseFilePath(ci, _settings.ARPAPredictorVocabFileName);
                    if (String.IsNullOrEmpty(dbFileName))
                    {
                        return false;
                    }

                    presage.set_config("Presage.Predictors.DefaultARPAPredictor.VOCABFILENAME", dbFileName);

                    presage.set_config("Presage.PredictorRegistry.PREDICTORS",
                        "DefaultARPAPredictor UserSmoothedNgramPredictor DefaultRecencyPredictor");
                }
                else
                {
                    var dbFileName = getDatabaseFilePath(ci, _settings.NGramPredictorDatabaseFileName);
                    if (String.IsNullOrEmpty(dbFileName))
                    {
                        return false;
                    }

                    Log.Debug("Presage (Spanish). dbFileName: " + dbFileName);

                    presage.set_config("Presage.Predictors.DefaultSmoothedNgramPredictor.DBFILENAME", dbFileName);

                    presage.set_config("Presage.PredictorRegistry.PREDICTORS",
                        "DefaultSmoothedNgramPredictor UserSmoothedNgramPredictor DefaultRecencyPredictor");
                }

                var learningdbFileName = getLearningDatabaseFilePath(ci);
                presage.set_config("Presage.Predictors.UserSmoothedNgramPredictor.DBFILENAME", learningdbFileName);

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

                    prediction[ii] = filterChars(prediction[ii]);
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
        /// Remove unnecessary chars
        /// </summary>
        /// <param name="input">input string</param>
        /// <returns>filtered string</returns>
        private String filterChars(String input)
        {
            var removedChars = input.Select(ch => _removeChars.Contains(ch) ? (char?)null : ch);

            return string.Concat(removedChars.ToArray());
        }

        /// <summary>
        /// Returns the full path to the learn.db file for the specificed
        /// culture
        /// </summary>
        /// <param name="ci">culture info</param>
        /// <returns>full path to the file</returns>
        private String getLearningDatabaseFilePath(CultureInfo ci)
        {
            return getLearningDBFilePath(ci.TwoLetterISOLanguageName, _settings.LearningDatabaseFileName);
        }
    }
}