////////////////////////////////////////////////////////////////////////////
// <copyright file="SpellChecker.cs" company="Intel Corporation">
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

using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.SpellCheckManagement;
using ACAT.Lib.Core.UserManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace ACAT.Extensions.Default.SpellCheckers
{
    /// <summary>
    /// A rudimentary spell checker class. Has a small spell dictionary.
    /// Checks spelling and replaces the misspelt word
    /// with the right word.
    /// Also does auto-correction.  Capitalizes words that appear
    /// after a sentence terminator (such as '.', '!' etc).  Inserts
    /// spaces after a terminator.
    /// </summary>
    [DescriptorAttribute("9DB43B3D-A407-4FC5-8025-89497E5B9767",
                        "ACAT SpellChecker",
                        "ACAT's Default Spell Checker.")]
    public class SpellChecker : ISpellChecker, IExtension
    {
        /// <summary>
        /// Name of the spellcheck file that contains a mapping between
        /// the misspelt word and its correct spelling
        /// </summary>
        private const string SpellCheckFileName = "SpellCheck.xml";

        /// <summary>
        /// Spell check dictionary.  Maps a misspelt word with the correct spelling
        /// </summary>
        private readonly Dictionary<String, String> _wordList = new Dictionary<string, string>();

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Gets the descriptor for this class
        /// </summary>
        public IDescriptor Descriptor
        {
            get { return DescriptorAttribute.GetDescriptor(GetType()); }
        }

        /// <summary>
        /// Disposes resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // Prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Returns the extension invoker object
        /// </summary>
        /// <returns>invoker object</returns>
        public ExtensionInvoker GetInvoker()
        {
            return null;
        }

        /// <summary>
        /// Initializes the spell checker.  Reads the spell check file and
        /// loads the spellings into the list
        /// </summary>
        /// <param name="ci">Culture</param>
        /// <returns>true on success</returns>
        public bool Init(CultureInfo ci)
        {
            bool retVal = true;

            var spellingFile = getSpellCheckFilePath(ci);

            var doc = new XmlDocument();

            try
            {
                if (!String.IsNullOrEmpty(spellingFile) && File.Exists(spellingFile))
                {
                    doc.Load(spellingFile);

                    var spellingNodes = doc.SelectNodes("/ACAT/Spellings/Spelling");

                    if (spellingNodes != null)
                    {
                        // load all the spellings
                        foreach (XmlNode node in spellingNodes)
                        {
                            createAndAdd(node);
                        }
                    }
                }
                else
                {
                    Log.Debug("Spelling file " + spellingFile + " does not exist");
                    retVal = false;
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Error processing spelling file " + spellingFile + ". Exception: " + ex);
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Loads factory default settings.
        /// </summary>
        /// <returns>true always</returns>
        public bool LoadDefaultSettings()
        {
            return true;
        }

        /// <summary>
        /// Loads settings from the specified directory
        /// </summary>
        /// <param name="configFileDirectory">directory name</param>
        /// <returns>true always</returns>
        public bool LoadSettings(String configFileDirectory)
        {
            return true;
        }

        /// <summary>
        /// Looks up the spelling list for the indicated word
        /// and returns the correct spelling if found. Returns
        /// empty string if it didn't find the word.
        /// </summary>
        /// <param name="word">word to lookup</param>
        /// <returns>the correctly spelt word</returns>
        public String Lookup(String word)
        {
            var replacement = String.Empty;
            if (_wordList != null)
            {
                _wordList.TryGetValue(word, out replacement);
            }

            return replacement;
        }

        /// <summary>
        ///  Saves settings into the specified directory
        /// </summary>
        /// <param name="configFileDirectory">name of the directory</param>
        /// <returns>true always</returns>
        public bool SaveSettings(String configFileDirectory)
        {
            return true;
        }

        /// <summary>
        /// Uninitializes
        /// </summary>
        public void Uninit()
        {
        }

        /// <summary>
        /// Disposer. Release resources and cleanup.
        /// </summary>
        /// <param name="disposing">true to dispose managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!_disposed)
            {
                Log.Debug();

                if (disposing)
                {
                    // dispose all managed resources.
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        /// Extracts word and replacement from the xml node and
        /// adds the word and its replacement to the word list.
        /// </summary>
        /// <param name="node"></param>
        private void createAndAdd(XmlNode node)
        {
            var word = XmlUtils.GetXMLAttrString(node, "word").Trim();
            var replaceWith = XmlUtils.GetXMLAttrString(node, "replaceWith").Trim();

            if (!String.IsNullOrEmpty(word) &&
                !_wordList.ContainsKey(word) &&
                !String.IsNullOrEmpty(replaceWith))
            {
                _wordList.Add(word, replaceWith);
            }
        }

        /// <summary>
        /// Returns the full path to the spell check file.  Checks the
        /// culture specific folder under the user folder.
        /// </summary>
        /// <returns>full path to the spellcheck file, empty if doesn't exist</returns>
        private string getSpellCheckFilePath(CultureInfo ci)
        {
            var spellCheckFile = Path.Combine(UserManager.GetResourcesDir(ci), SpellCheckFileName);

            return File.Exists(spellCheckFile) ? spellCheckFile : String.Empty;
        }
    }
}