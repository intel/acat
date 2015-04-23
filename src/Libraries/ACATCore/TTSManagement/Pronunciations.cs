////////////////////////////////////////////////////////////////////////////
// <copyright file="Pronunciations.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2015 Intel Corporation 
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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Xml;
using ACAT.Lib.Core.Utility;

#region SupressStyleCopWarnings

[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1126:PrefixCallsCorrectly",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1101:PrefixLocalCallsWithThis",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1121:UseBuiltInTypeAlias",
        Scope = "namespace",
        Justification = "Since they are just aliases, it doesn't really matter")]
[module: SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1200:UsingDirectivesMustBePlacedWithinNamespace",
        Scope = "namespace",
        Justification = "ACAT guidelines")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1309:FieldNamesMustNotBeginWithUnderscore",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private fields begin with an underscore")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1300:ElementMustBeginWithUpperCaseLetter",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private/Protected methods begin with lowercase")]

#endregion SupressStyleCopWarnings

namespace ACAT.Lib.Core.TTSManagement
{
    /// <summary>
    /// Holds a sorted list of pronunciation objects.  Raises events when
    /// it detects a pronunciation has been entered in the text stream
    /// so the application can handle the expansion suitably.  The list of
    /// pronunciation is created by parsing the xml file that has a list
    /// of all the pronunciations.
    /// </summary>
    public class Pronunciations : IDisposable
    {
        /// <summary>
        /// xml attribute to get the alternate pronunciation
        /// </summary>
        private const String PronunciationAttr = "pronunciation";

        /// <summary>
        /// Xml attribute to get the original word
        /// </summary>
        private const String WordAttr = "word";

        /// <summary>
        /// Holds a sorted list of pronunciations
        /// </summary>
        private readonly SortedDictionary<String, Pronunciation> _pronunciationList = new SortedDictionary<string, Pronunciation>();

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Holds a mapping between words and their pronunciations
        /// </summary>
        public SortedDictionary<String, Pronunciation> PronunciationList
        {
            get { return _pronunciationList; }
        }

        /// <summary>
        /// Adds the pronunciation to the list.  If it already exists,
        /// it is replaced.
        /// </summary>
        /// <param name="pronunciation">the pronunciation object</param>
        /// <returns>true on success</returns>
        public bool Add(Pronunciation pronunciation)
        {
            if (String.IsNullOrEmpty(pronunciation.Word) ||
                String.IsNullOrWhiteSpace(pronunciation.Word) ||
                String.IsNullOrWhiteSpace(pronunciation.AltPronunciation) ||
                String.IsNullOrEmpty(pronunciation.AltPronunciation))
            {
                return false;
            }

            _pronunciationList[pronunciation.Word] = pronunciation;
            return true;
        }

        /// <summary>
        /// Clears all the pronunciations in the list
        /// </summary>
        public void Clear()
        {
            _pronunciationList.Clear();
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
        /// Checks if a word already exists in the lookup table
        /// </summary>
        /// <param name="word">pronunciat</param>
        /// <returns></returns>
        public bool Exists(String word)
        {
            // TODO see if we need to make pronunciations case sensitive or not
            //return _pronunciationList.ContainsKey(pronunciation.ToUpper());
            return _pronunciationList.ContainsKey(word);
        }

        /// <summary>
        /// Load pronunciation from the specified file.  If filename
        /// is null, loads from the default file.  Parses the XML file
        /// and populates the sorted list
        /// </summary>
        /// <param name="pronunciationsFile">Name of the file</param>
        /// <returns>true on success</returns>
        public bool Load(String pronunciationsFile)
        {
            Log.Debug("Entering...");

            bool retVal = true;

            if (String.IsNullOrEmpty(pronunciationsFile))
            {
                return false;
            }

            Log.Debug("pronunciationsFile=" + pronunciationsFile);

            var doc = new XmlDocument();

            try
            {
                _pronunciationList.Clear();

                if (!File.Exists(pronunciationsFile))
                {
                    Log.Debug("Pronunciation file " + pronunciationsFile + " does not exist");
                    return false;
                }

                Log.Debug("found pronuncation file!");
                doc.Load(pronunciationsFile);

                var xmlNodes = doc.SelectNodes("/ACAT/Pronunciations/Pronunciation");

                Log.Debug("xmlNodes count=" + xmlNodes.Count);

                // load all the pronunciations
                foreach (XmlNode node in xmlNodes)
                {
                    Log.Debug("adding node:" + node);
                    createAndAddPronunciation(node);
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Error processing pronunciation file " + pronunciationsFile + ". Exception: " + ex.ToString());
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Looks up the word and returns its pronunciation
        /// object.
        /// </summary>
        /// <param name="word">word to lookup</param>
        /// <returns>pronunciation object, null if not found</returns>
        public Pronunciation Lookup(String word)
        {
            var w = word.ToLower();
            return Exists(w) ? _pronunciationList[w] : null;
        }

        /// <summary>
        /// Removes the word from the lookup table
        /// </summary>
        /// <param name="word">word to lookup</param>
        /// <returns>true on success</returns>
        public bool Remove(String word)
        {
            bool retVal = true;
            try
            {
                if (Exists(word))
                {
                    _pronunciationList.Remove(word);
                }
            }
            catch
            {
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Takes in a string of text (a sentence for example), parses it into
        /// words, looks up each word in the lookup table to see if there is
        /// an alternate pronunciation and if so, replaces the word with the
        /// alternate pronunciation.  Returns the converted sentence
        /// </summary>
        /// <param name="inputString">input text</param>
        /// <returns>converted text</returns>
        public String ReplaceWithAlternatePronunciations(String inputString)
        {
            String word;
            var strOutput = new StringBuilder();
            var strWord = new StringBuilder();
            Pronunciation pronunciation;

            foreach (char ch in inputString)
            {
                if (Char.IsLetterOrDigit(ch) || ch == '\'' || ch == '’')
                {
                    strWord.Append(ch);
                }
                else
                {
                    word = strWord.ToString();
                    strOutput.Append(((pronunciation = Lookup(word)) != null) ? pronunciation.AltPronunciation : word);

                    strWord = new StringBuilder();

                    strOutput.Append(ch);
                }
            }

            word = strWord.ToString();
            strOutput.Append(((pronunciation = Lookup(word)) != null) ? pronunciation.AltPronunciation : word);

            var retVal = strOutput.ToString();

            Log.Debug("replacedString: " + retVal);

            return retVal;
        }

        /// <summary>
        /// Saves all the pronunciation from the lookup table to the pronunciation file
        /// </summary>
        /// <returns>true on success</returns>
        public bool Save(String pronunciationsFile)
        {
            bool retVal = true;
            try
            {
                var xmlTextWriter = createPronunciationsFile(pronunciationsFile);
                if (xmlTextWriter != null)
                {
                    foreach (var pronunciationObj in _pronunciationList.Values)
                    {
                        xmlTextWriter.WriteStartElement("Pronunciation");
                        xmlTextWriter.WriteAttributeString(WordAttr, pronunciationObj.Word);
                        xmlTextWriter.WriteAttributeString(PronunciationAttr, pronunciationObj.AltPronunciation);

                        xmlTextWriter.WriteEndElement();
                    }

                    closePronunciationFile(xmlTextWriter);
                }
            }
            catch (IOException ex)
            {
                Log.Exception(ex);
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Replaces the old pronunciation with a new one.
        /// </summary>
        /// <param name="word">word to look for</param>
        /// <param name="pronunciation">new pronunciation object</param>
        /// <returns></returns>
        public bool Update(String word, Pronunciation pronunciation)
        {
            Remove(word);
            return Add(pronunciation);
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
                    foreach (var p in _pronunciationList.Values)
                    {
                        p.Dispose();
                    }

                    _pronunciationList.Clear();
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        /// Closes the pronunciation file after writing out the close tag
        /// </summary>
        /// <param name="xmlTextWriter">the xml writer object</param>
        private static void closePronunciationFile(XmlWriter xmlTextWriter)
        {
            try
            {
                xmlTextWriter.WriteEndDocument();
                xmlTextWriter.Flush();
                xmlTextWriter.Close();
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }
        }

        /// <summary>
        /// Creates an empty pronunciation XML file
        /// </summary>
        /// <param name="fileName">name of the file to create</param>
        /// <returns>xml writer</returns>
        private static XmlTextWriter createPronunciationsFile(String fileName)
        {
            XmlTextWriter xmlTextWriter;

            // overwrite even if it already exists
            try
            {
                xmlTextWriter = new XmlTextWriter(fileName, null) { Formatting = Formatting.Indented };
                xmlTextWriter.WriteStartDocument();
                xmlTextWriter.WriteStartElement("ACAT");
                xmlTextWriter.WriteStartElement("Pronunciations");
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
                xmlTextWriter = null;
            }

            return xmlTextWriter;
        }

        /// <summary>
        /// Parses the xml node attributes and creates an pronunciation object
        /// and adds it to the sort list
        /// </summary>
        /// <param name="node">xml node to parse</param>
        private void createAndAddPronunciation(XmlNode node)
        {
            var word = XmlUtils.GetXMLAttrString(node, WordAttr).Trim().ToLower();
            var pronunciation = XmlUtils.GetXMLAttrString(node, PronunciationAttr);
            Log.Debug("word=" + word + " pronunciation=" + pronunciation);

            Add(new Pronunciation(word, pronunciation));
        }
    }
}