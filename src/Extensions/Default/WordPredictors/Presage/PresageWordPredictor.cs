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
using ACAT.Lib.Core.UserManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ACAT.Extensions.Default.WordPredictors.Neutral.Presage
{
    /// <summary>
    /// The Word Predictor that uses the Presage word predictor
    /// for next word prediction. Presage is an intelligent predictive
    /// text engine created by Matteo Vescovi.
    /// This is the culture independent version and looks for the
    /// presage database file (database.db) in the lanugage folder.
    /// http://presage.sourceforge.net/
    /// </summary>
    [DescriptorAttribute("3F12555B-175C-45D8-93B8-60EAAE6705C8",
                            "Presage Word Predictor (Generic)",
                            "Generic Word predictor based on Presage intelligent predictive text engine")]
    public class PresageWordPredictor : PresageWordPredictorBase
    {
        /// <summary>
        /// Name of the preferences file
        /// </summary>
        private const String SettingsFileName = "PresageWordPredictorSettings.xml";

        /// <summary>
        /// Settings for this extension
        /// </summary>
        private Settings _settings;

        /// <summary>
        /// Initializes and instance of the class
        /// </summary>
        public PresageWordPredictor()
        {
            var ci = CultureInfo.DefaultThreadCurrentUICulture;
            var settingsFilePath = (ci == null) ?
                                    UserManager.GetFullPath(SettingsFileName) :
                                    getUserRelativePath(ci.TwoLetterISOLanguageName, SettingsFileName, true);

            Settings.PreferencesFilePath = settingsFilePath;

            _settings = Settings.Load();

            DatabaseFileName = _settings.DatabaseFileName;

            LearningDBFileName = _settings.LearningDatabaseFileName;

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
                presage.learn(_settings.UseDefaultEncoding ? UTF8EncodingToDefault(text) : text);
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
                if (_settings.UseDefaultEncoding)
                {
                    prevWords = UTF8EncodingToDefault(prevWords);
                    currentWord = UTF8EncodingToDefault(currentWord);
                }

                string[] prediction = presage.predict(prevWords, currentWord);

                for (int ii = 0; ii < prediction.Length; ii++)
                {
                    if (_settings.UseDefaultEncoding)
                    {
                        prediction[ii] = defaultEncodingToUTF8(prediction[ii]);
                    }

                    if (!String.IsNullOrEmpty(_settings.FilterChars))
                    {
                        prediction[ii] = filterChars(prediction[ii]);
                    }
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
            var removedChars = input.Select(ch => _settings.FilterChars.Contains(ch) ? (char?)null : ch);

            return string.Concat(removedChars.ToArray());
        }
    }
}