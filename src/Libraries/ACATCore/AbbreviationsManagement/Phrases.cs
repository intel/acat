////////////////////////////////////////////////////////////////////////////
// <copyright file="Phrases.cs" company="Intel Corporation">
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
using ACAT.Lib.Core.UserManagement;
using System;
using System.Collections.Generic;
using System.IO;

namespace ACAT.Lib.Core.AbbreviationsManagement
{
    /// <summary>
    /// Represents a list of phrases
    /// </summary>

    [Serializable]
    public class Phrases : PreferencesBase
    {
        /// <summary>
        /// Name of the abbreviations file
        /// </summary>
        public const string PhrasesFile = "Phrases.xml";

        /// <summary>
        /// List of phrases
        /// </summary>
        public List<Phrase> PhraseList = new List<Phrase>();

        /// <summary>
        /// Load phrases
        /// </summary>
        /// <returns>Phrases object</returns>
        public static Phrases Load()
        {
            //create();
            return PreferencesBase.Load<Phrases>(getPhrasesFilePath());
        }

        /// <summary>
        /// Loads phrases from the specified file (full path)
        /// </summary>
        /// <param name="phrasesFile">full path to the file</param>
        /// <returns></returns>
        public static Phrases Load(String phrasesFile)
        {
            return PreferencesBase.Load<Phrases>(phrasesFile, false);
        }

        /// <summary>
        /// Adds the specified phrase to the list
        /// </summary>
        /// <param name="newPhrase">phrase to add</param>
        public void Add(Phrase newPhrase)
        {
            foreach (var phrase in PhraseList)
            {
                if (String.Compare(phrase.Text, newPhrase.Text, true) == 0)
                {
                    phrase.Favorite = newPhrase.Favorite;
                    return;
                }
            }

            PhraseList.Add(newPhrase);
        }

        /// <summary>
        /// Saves phrases to the specified file
        /// </summary>
        /// <param name="phrasesFilePath">file to save to</param>
        /// <returns>true on success</returns>
        public bool Save(String phrasesFilePath)
        {
            return Save(this, phrasesFilePath);
        }

        /// <summary>
        /// Saves phrases to the phrases file
        /// </summary>
        /// <returns>true on success</returns>
        public override bool Save()
        {
            return Save(this, getPhrasesFilePath());
        }

        /// <summary>
        /// Returns the full path to the abbreviations file.  Checks the user
        /// folder under the culture specific folder first.
        /// </summary>
        /// <returns>full path to the abbreviations file</returns>
        private static string getPhrasesFilePath()
        {
            return Path.Combine(UserManager.GetResourcesDir(), PhrasesFile);
        }
    }
}