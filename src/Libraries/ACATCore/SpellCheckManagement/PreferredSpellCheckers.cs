////////////////////////////////////////////////////////////////////////////
// <copyright file="PreferredSpellCheckers.cs" company="Intel Corporation">
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

namespace ACAT.Lib.Core.SpellCheckManagement
{
    /// <summary>
    /// Maintains a list of preferred spell checkers
    /// specific for each culture (language)
    /// The class is serialized to a file and loaded
    /// from a file as well
    /// </summary>
    [Serializable]
    public class PreferredSpellCheckers : PreferencesBase
    {
        /// <summary>
        /// Path to the file to serialize to
        /// </summary>
        [NonSerialized, XmlIgnore]
        public static String FilePath;

        /// <summary>
        /// List of preferred word predictors
        /// </summary>
        public List<PreferredSpellChecker> SpellCheckers;

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public PreferredSpellCheckers()
        {
            SpellCheckers = new List<PreferredSpellChecker>();
        }

        /// <summary>
        /// Returns the list of the preferred spellchekers
        /// </summary>
        public IEnumerable<PreferredSpellChecker> List
        {
            get { return SpellCheckers; }
        }

        /// <summary>
        /// Deserializes list of spellcheckers from the file and
        /// returns an instance of this class
        /// </summary>
        /// <returns>an object of this class</returns>
        public static PreferredSpellCheckers Load()
        {
            return Load<PreferredSpellCheckers>(FilePath);
        }

        /// <summary>
        /// Returns the ID of the preferred spellchecker
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
        /// Sets the specified id of the spellchecker as the
        /// default for the language
        /// </summary>
        /// <param name="language">Language (culture)</param>
        /// <param name="guid">ID of the spellchecker</param>
        /// <returns></returns>
        public bool SetAsDefault(String language, Guid guid)
        {
            var preferredSpellChecker = SpellCheckers.FirstOrDefault(spellChecker => String.Compare(language, spellChecker.Language, true) == 0);
            if (preferredSpellChecker != null)
            {
                preferredSpellChecker.ID = guid;
            }
            else
            {
                SpellCheckers.Add(new PreferredSpellChecker(guid, language));
            }

            return true;
        }

        /// <summary>
        /// Gets the preferred spellchecker for the specified language
        /// </summary>
        /// <param name="language">Language (culture)</param>
        /// <returns>ID, Guid.empty if none found</returns>
        private Guid getByLanguage(String language)
        {
            foreach (var preferredSpellChecker in SpellCheckers)
            {
                if (String.IsNullOrEmpty(language))
                {
                    if (String.IsNullOrEmpty(preferredSpellChecker.Language))
                    {
                        return preferredSpellChecker.ID;
                    }
                }
                else if (String.Compare(preferredSpellChecker.Language, language, true) == 0)
                {
                    return preferredSpellChecker.ID;
                }
            }

            return Guid.Empty;
        }
    }
}