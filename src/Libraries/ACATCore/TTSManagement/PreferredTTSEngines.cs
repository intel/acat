////////////////////////////////////////////////////////////////////////////
// <copyright file="PreferredTTSEngines.cs" company="Intel Corporation">
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

namespace ACAT.Lib.Core.TTSManagement
{
    /// <summary>
    /// Maintains a list of preferred TTS Engines
    /// specific for each culture (language)
    /// The class is serialized to a file and loaded
    /// from a file as well
    /// </summary>
    [Serializable]
    public class PreferredTTSEngines : PreferencesBase
    {
        /// <summary>
        /// Path to the file to serialize to
        /// </summary>
        [NonSerialized, XmlIgnore]
        public static String FilePath;

        /// <summary>
        /// List of preferred TTS Engines
        /// </summary>
        public List<PreferredTTSEngine> TTSEngines;

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public PreferredTTSEngines()
        {
            TTSEngines = new List<PreferredTTSEngine>();
        }

        /// <summary>
        /// Returns the list of the preferred TTS Engines
        /// </summary>
        public IEnumerable<PreferredTTSEngine> List
        {
            get { return TTSEngines; }
        }

        /// <summary>
        /// Deserializes list of TTS Engines from the file and
        /// returns an instance of this class
        /// </summary>
        /// <returns>an object of this class</returns>
        public static PreferredTTSEngines Load()
        {
            return Load<PreferredTTSEngines>(FilePath);
        }

        /// <summary>
        /// Returns the ID of the preferred TTS Engine
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

            if (Equals(guid, Guid.Empty))
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
        /// Sets the specified id of the TTS Engine as the
        /// default for the language
        /// </summary>
        /// <param name="language">Language (culture)</param>
        /// <param name="guid">ID of the TTS Engine</param>
        /// <returns></returns>
        public bool SetAsDefault(String language, Guid guid)
        {
            var preferredEngine = TTSEngines.FirstOrDefault(engine => String.Compare(language, engine.Language, true) == 0);
            if (preferredEngine != null)
            {
                preferredEngine.ID = guid;
            }
            else
            {
                TTSEngines.Add(new PreferredTTSEngine(guid, language));
            }

            return true;
        }

        /// <summary>
        /// Gets the preferred TTS Engine for the specified
        /// language
        /// </summary>
        /// <param name="language">Language (culture)</param>
        /// <returns>ID, Guid.empty if none found</returns>
        private Guid getByLanguage(String language)
        {
            foreach (var preferredEngine in TTSEngines)
            {
                if (String.Compare(preferredEngine.Language, language, true) == 0)
                {
                    return preferredEngine.ID;
                }
            }

            return Guid.Empty;
        }
    }
}