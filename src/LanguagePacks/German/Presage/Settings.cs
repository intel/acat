////////////////////////////////////////////////////////////////////////////
// <copyright file="Settings.cs" company="Intel Corporation">
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
using System;

namespace ACAT.Extensions.Default.WordPredictors.de.Presage
{
    /// <summary>
    /// Preference settings for the Presage word predictor (German)
    /// </summary>
    [Serializable]
    public class Settings : PresageSettingsBase
    {
        /// <summary>
        /// Name of the NGRAM database file
        /// </summary>
        public String DatabaseFileName = "database.db";

        /// <summary>
        /// Name of the file for learnt phrases
        /// </summary>
        public String LearningDatabaseFileName = "learn.db";

        /// <summary>
        /// Loads the settings from the settings file
        /// </summary>
        /// <returns>Settings object</returns>
        public static Settings Load()
        {
            Settings retVal = Load<Settings>(PreferencesFilePath);
            Save(retVal, PreferencesFilePath);
            return retVal;
        }

        /// <summary>
        /// Saves the settings to the settings file
        /// </summary>
        /// <returns>true on success</returns>
        public override bool Save()
        {
            return Save<Settings>(this, PreferencesFilePath);
        }
    }
}