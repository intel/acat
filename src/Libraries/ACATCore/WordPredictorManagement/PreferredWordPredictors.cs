////////////////////////////////////////////////////////////////////////////
// <copyright file="PreferredWordPredictors.cs" company="Intel Corporation">
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

using ACAT.Lib.Core.PreferencesManagement;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;

namespace ACAT.Lib.Core.WordPredictorManagement
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
        /// Deserializes list of word predictors from the file and
        /// returns an instance of this class
        /// </summary>
        /// <returns>an object of this class</returns>
        public static PreferredWordPredictors Load()
        {
            return Load<PreferredWordPredictors>(FilePath);
        }

        /// <summary>
        /// Returns the ID of the preferred word predictor
        /// for the specified culture
        /// </summary>
        /// <param name="ci">culture</param>
        /// <returns>id, guid.empty if none found</returns>
        public Guid GetByCulture(CultureInfo ci)
        {
            if (ci == null)
            {
                return getByLanguage(String.Empty);
            }

            var guid = getByLanguage(ci.Name);

            if (Guid.Equals(guid, Guid.Empty))
            {
                guid = getByLanguage(ci.TwoLetterISOLanguageName);
            }

            return guid;
        }

        /// <summary>
        /// Persists this object to a file
        /// </summary>
        /// <returns>true on success</returns>
        public override bool Save()
        {
            if (!String.IsNullOrEmpty(FilePath))
            {
                return Save(this, FilePath);
            }

            return false;
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