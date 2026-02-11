////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Configuration;
using ACAT.Core.UserManagement;
using ACAT.Core.Utility;
using ACAT.Core.Validation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace ACAT.Core.TTSManagement
{
    /// <summary>
    /// Holds a sorted list of pronunciation objects.  Raises events when
    /// it detects a pronunciation has been entered in the text stream
    /// so the application can handle the expansion suitably.  The list of
    /// pronunciation is created by parsing the xml file that has a list
    /// of all the pronunciations.
    /// Pronunciations are useful where the TTS engine may not pronounce
    /// words correctly (eg proper nouns). This object maps the actual
    /// spelling with the phonetic spelling. The phonetically spelt word
    /// is the one sent to the TTS engine to convert to speech.
    /// </summary>
    public class Pronunciations : IDisposable
    {
        private static readonly ILogger<Pronunciations> _staticLogger = LoggingConfiguration.CreateLogger<Pronunciations>();
        private readonly ILogger<Pronunciations> _logger;

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
        private readonly SortedDictionary<String, Pronunciation> _pronunciationList = new();

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        public Pronunciations(ILogger<Pronunciations> logger = null)
        {
            _logger = logger;
        }

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
        /// Loads pronunciation from the specified file. Supports both JSON and XML formats.
        /// JSON is preferred; XML is used for backward compatibility.
        /// </summary>
        /// <param name="filePath">fullpath to the file</param>
        /// <returns>true on success</returns>
        public bool Load(String filePath)
        {
            bool retVal = true;

            if (String.IsNullOrEmpty(filePath))
            {
                return false;
            }

            _pronunciationList.Clear();

            if (!File.Exists(filePath))
            {
                _logger?.LogDebug("Pronunciation file not found: {FilePath}", filePath);
                return true; // Return true for non-existent file (empty list is valid)
            }

            try
            {
                // Determine format based on file extension
                var extension = Path.GetExtension(filePath)?.ToLowerInvariant();
                
                if (extension == ".json")
                {
                    retVal = LoadFromJson(filePath);
                }
                else if (extension == ".xml")
                {
                    retVal = LoadFromXml(filePath);
                }
                else
                {
                    _logger?.LogWarning("Unknown pronunciation file format: {Extension}", extension);
                    retVal = false;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error loading pronunciation file {FilePath}", filePath);
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Loads pronunciations from a JSON file
        /// </summary>
        /// <param name="filePath">Path to JSON file</param>
        /// <returns>true on success</returns>
        private bool LoadFromJson(string filePath)
        {
            try
            {
                var loader = new JsonConfigurationLoader<PronunciationsJson>(
                    new PronunciationsValidator(), 
                    _logger);
                
                var config = loader.Load(filePath, createDefaultOnError: false);
                
                if (config == null)
                {
                    _logger?.LogError("Failed to load pronunciations from JSON: {FilePath}", filePath);
                    return false;
                }

                // Convert JSON entries to Pronunciation objects
                foreach (var entry in config.Pronunciations)
                {
                    if (!string.IsNullOrWhiteSpace(entry.Word) && 
                        !string.IsNullOrWhiteSpace(entry.Pronunciation))
                    {
                        Add(new Pronunciation(entry.Word, entry.Pronunciation));
                    }
                }

                _logger?.LogInformation("Loaded {Count} pronunciations from JSON", _pronunciationList.Count);
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error loading pronunciations from JSON: {FilePath}", filePath);
                return false;
            }
        }

        /// <summary>
        /// Loads pronunciations from a legacy XML file
        /// </summary>
        /// <param name="filePath">Path to XML file</param>
        /// <returns>true on success</returns>
        private bool LoadFromXml(string filePath)
        {
            var doc = new XmlDocument();

            try
            {
                _logger?.LogDebug("Loading pronunciations from legacy XML file: {FilePath}", filePath);

                doc.Load(filePath);

                var xmlNodes = doc.SelectNodes("/ACAT/Pronunciations/Pronunciation");

                // load all the pronunciations
                foreach (XmlNode node in xmlNodes)
                {
                    createAndAddPronunciation(node);
                }

                _logger?.LogInformation("Loaded {Count} pronunciations from XML", _pronunciationList.Count);
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error processing pronunciation XML file {FilePath}", filePath);
                return false;
            }
        }

        /// <summary>
        /// Loads pronunciation from the specified file. Parses the file
        /// and populates the sorted list. Tries JSON first, then XML.
        /// </summary>
        /// <param name="ci">Culture for which to load the file</param>
        /// <param name="pronunciationsFileName">Name of the file (e.g., "Pronunciations.json")</param>
        /// <returns>true on success</returns>
        public bool Load(CultureInfo ci, String pronunciationsFileName)
        {
            _logger?.LogDebug("Loading pronunciations for culture {Culture}, file {FileName}", ci.Name, pronunciationsFileName);

            String filePath = getPronunciationsFilePath(ci, pronunciationsFileName);
            
            // If specified file doesn't exist, try alternate formats
            if (string.IsNullOrEmpty(filePath))
            {
                // Validate pronunciationsFileName is not null or empty
                if (string.IsNullOrWhiteSpace(pronunciationsFileName))
                {
                    _logger?.LogWarning("Pronunciation file name is null or empty for culture {Culture}", ci.Name);
                    return false;
                }

                var baseName = Path.GetFileNameWithoutExtension(pronunciationsFileName);
                
                // Validate baseName is not empty after extraction
                if (string.IsNullOrWhiteSpace(baseName))
                {
                    _logger?.LogWarning("Invalid pronunciation file name for culture {Culture}: {FileName}", 
                        ci.Name, pronunciationsFileName);
                    return false;
                }
                
                // Try JSON format first
                var jsonFileName = baseName + ".json";
                filePath = getPronunciationsFilePath(ci, jsonFileName);
                
                // If JSON not found, try XML format
                if (string.IsNullOrEmpty(filePath))
                {
                    var xmlFileName = baseName + ".xml";
                    filePath = getPronunciationsFilePath(ci, xmlFileName);
                    
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        _logger?.LogDebug("Found XML pronunciation file: {FilePath}", filePath);
                    }
                }
                else
                {
                    _logger?.LogDebug("Found JSON pronunciation file: {FilePath}", filePath);
                }

                // Log with baseName variable (already validated)
                if (string.IsNullOrEmpty(filePath))
                {
                    _logger?.LogWarning("Pronunciation file not found for culture {Culture}. Tried: {BaseName}.json and {BaseName}.xml", 
                        ci.Name, baseName);
                    return false;
                }
            }

            return Load(filePath);
        }

        /// <summary>
        /// Looks up the word and returns its pronunciation object.
        /// </summary>
        /// <param name="word">word to lookup</param>
        /// <returns>pronunciation object, null if not found</returns>
        public Pronunciation Lookup(String word)
        {
            var w = word.ToLower().Trim();
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
        /// alternate pronunciation.  Returns the converted sentence with the
        /// phonetically spelt words.
        /// </summary>
        /// <param name="inputString">input text</param>
        /// <returns>converted text</returns>
        public String ReplaceWithAlternatePronunciations(String inputString)
        {
            String word;
            var strOutput = new StringBuilder();
            var strWord = new StringBuilder();
            Pronunciation pronunciation;

            _logger?.LogDebug("Processing input string: {InputString}", inputString);

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

            _logger?.LogDebug("Replaced string: {ReplacedString}", retVal);

            return retVal;
        }

        /// <summary>
        /// Saves all the pronunciation from the lookup table to the pronunciation file in JSON format
        /// </summary>
        /// <returns>true on success</returns>
        public bool Save(String pronunciationsFile)
        {
            bool retVal = true;
            try
            {
                // Create JSON configuration object
                var config = new PronunciationsJson();
                
                foreach (var pronunciationObj in _pronunciationList.Values)
                {
                    config.Pronunciations.Add(new PronunciationJson
                    {
                        Word = pronunciationObj.Word,
                        Pronunciation = pronunciationObj.AltPronunciation
                    });
                }

                // Save using JsonConfigurationLoader
                var loader = new JsonConfigurationLoader<PronunciationsJson>(
                    new PronunciationsValidator(), 
                    _logger);
                
                retVal = loader.Save(config, pronunciationsFile);
                
                if (retVal)
                {
                    _logger?.LogInformation("Saved {Count} pronunciations to JSON: {FilePath}", 
                        _pronunciationList.Count, pronunciationsFile);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error saving pronunciations to: {PronunciationsFile}", pronunciationsFile);
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
                _logger?.LogTrace("Disposing Pronunciations");

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
                _staticLogger?.LogError(ex, "Exception closing pronunciation file");
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
                _staticLogger?.LogError(ex, "Exception creating pronunciations file");
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
            _logger?.LogDebug("Adding pronunciation - word: {Word}, pronunciation: {Pronunciation}", word, pronunciation);

            Add(new Pronunciation(word, pronunciation));
        }

        /// <summary>
        /// Returns the full path to the pronunciations file.  Checks the
        /// culture specific folder under the user folder.
        /// </summary>
        /// <returns>full path to the pronunciartions file, empty if doesn't exist</returns>
        private string getPronunciationsFilePath(CultureInfo ci, String pronunciationsFileName)
        {
            var file = Path.Combine(UserManager.GetResourcesDir(ci), pronunciationsFileName);

            return File.Exists(file) ? file : String.Empty;
        }
    }
}