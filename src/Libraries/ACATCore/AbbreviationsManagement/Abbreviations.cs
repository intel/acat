////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// Abbreviations.cs
//
// Represents a sorted list of abbreviation objects.  The list of
// abbreviations is created by parsing the xml file that has a list
// of all the abbreviations.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Configuration;
using ACAT.Core.UserManagement;
using ACAT.Core.Utility;
using ACAT.Core.Validation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace ACAT.Core.AbbreviationsManagement
{
    public class Abbreviations : IDisposable
    {
        private readonly ILogger<Abbreviations> _logger;

        /// <summary>
        /// Name of the abbreviations file (JSON format)
        /// </summary>
        public const string AbbreviationFile = "Abbreviations.json";

        /// <summary>
        /// Legacy XML file name for backward compatibility
        /// </summary>
        private const string LegacyAbbreviationFile = "Abbreviations.xml";

        /// <summary>
        ///  xml attribute for the abbreviation mode
        /// </summary>
        private const string ModeAttr = "mode";

        /// <summary>
        /// xml attribute for the expansion element
        /// </summary>
        private const string ReplaceWithAttr = "replaceWith";

        /// <summary>
        /// xml attribute for the abbreviation mnemonic element
        /// </summary>
        private const string WordAttr = "word";

        /// <summary>
        /// Holds a sorted list of abbreviations
        /// </summary>
        private readonly SortedDictionary<String, Abbreviation> _abbreviationList = new();

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Constructor
        /// </summary>
        public Abbreviations(ILogger<Abbreviations> logger = null)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets the sorted list of abbreviations
        /// </summary>
        public IEnumerable<Abbreviation> AbbreviationList
        {
            get { return _abbreviationList.Values.ToList(); }
        }

        /// <summary>
        /// Adds the abbreviation object to the list.  If it already exists,
        /// it is replaced.
        /// </summary>
        /// <param name="abbreviation">Abbreviation to add</param>
        /// <returns>true on success</returns>
        public bool Add(Abbreviation abbreviation)
        {
            if (String.IsNullOrEmpty(abbreviation.Mnemonic) ||
                String.IsNullOrWhiteSpace(abbreviation.Mnemonic) ||
                String.IsNullOrWhiteSpace(abbreviation.Expansion) ||
                String.IsNullOrEmpty(abbreviation.Expansion))
            {
                return false;
            }

            _abbreviationList[abbreviation.Mnemonic] = abbreviation;
            return true;
        }

        /// <summary>
        /// Clears all the abbreviations in the list
        /// </summary>
        public void Clear()
        {
            _abbreviationList.Clear();
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
        /// Checks if an abbreviation already exists in the list
        /// </summary>
        /// <param name="abbreviation">the mnemonic</param>
        /// <returns>true if it exists, false otherwise</returns>
        public bool Exists(String abbreviation)
        {
            return _abbreviationList.ContainsKey(abbreviation.ToUpper());
        }

        /// <summary>
        /// Returns the full path to the abbreviations file.  Checks the user
        /// folder under the culture specific folder first. Tries JSON first,
        /// then falls back to legacy XML file.
        /// </summary>
        /// <returns>full path to the abbreviations file</returns>
        public string GetAbbreviationsFilePath()
        {
            // Try JSON file first
            var abbreviationsFile = Path.Combine(UserManager.GetResourcesDir(), AbbreviationFile);
            if (File.Exists(abbreviationsFile))
            {
                return abbreviationsFile;
            }

            abbreviationsFile = UserManager.GetFullPath(AbbreviationFile);
            if (File.Exists(abbreviationsFile))
            {
                return abbreviationsFile;
            }

            // Fall back to legacy XML file
            var legacyFile = Path.Combine(UserManager.GetResourcesDir(), LegacyAbbreviationFile);
            if (File.Exists(legacyFile))
            {
                return legacyFile;
            }

            legacyFile = UserManager.GetFullPath(LegacyAbbreviationFile);
            if (File.Exists(legacyFile))
            {
                return legacyFile;
            }

            // Return JSON path for new file creation
            return UserManager.GetFullPath(AbbreviationFile);
        }

        /// <summary>
        /// Loads abbreviations from the specified file.  If filename
        /// is null, loads from the default file.  Supports both JSON and XML formats.
        /// JSON is preferred; XML is used for backward compatibility.
        /// </summary>
        /// <param name="abbreviationsFile">name of the abbreviations file</param>
        /// <returns>true on success</returns>
        public bool Load(String abbreviationsFile = null)
        {
            bool retVal = true;

            if (String.IsNullOrEmpty(abbreviationsFile))
            {
                abbreviationsFile = GetAbbreviationsFilePath();
            }

            _abbreviationList.Clear();

            if (!File.Exists(abbreviationsFile))
            {
                _logger?.LogDebug("Abbreviation file {AbbreviationsFile} does not exist", abbreviationsFile);
                return true; // Return true for non-existent file (empty list is valid)
            }

            try
            {
                // Determine format based on file extension
                var extension = Path.GetExtension(abbreviationsFile)?.ToLowerInvariant();
                
                if (extension == ".json")
                {
                    retVal = LoadFromJson(abbreviationsFile);
                }
                else if (extension == ".xml")
                {
                    retVal = LoadFromXml(abbreviationsFile);
                }
                else
                {
                    _logger?.LogWarning("Unknown abbreviation file format: {Extension}", extension);
                    retVal = false;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error loading abbreviations file {AbbreviationsFile}", abbreviationsFile);
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Loads abbreviations from a JSON file
        /// </summary>
        /// <param name="filePath">Path to JSON file</param>
        /// <returns>true on success</returns>
        private bool LoadFromJson(string filePath)
        {
            try
            {
                var loader = new JsonConfigurationLoader<AbbreviationsJson>(
                    new AbbreviationsValidator(), 
                    _logger);

                AbbreviationsJson config = loader.Load(filePath, createDefaultOnError: false);
                
                if (config == null)
                {
                    _logger?.LogError("Failed to load abbreviations from JSON: {FilePath}", filePath);
                    return false;
                }

                // Convert JSON entries to Abbreviation objects
                foreach (AbbreviationJson entry in config.Abbreviations)
                {
                    if (!string.IsNullOrWhiteSpace(entry.Word) && 
                        !string.IsNullOrWhiteSpace(entry.ReplaceWith))
                    {
                        Add(new Abbreviation(entry.Word, entry.ReplaceWith, entry.Mode));
                    }
                }

                _logger?.LogInformation("Loaded {Count} abbreviations from JSON", _abbreviationList.Count);
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error loading abbreviations from JSON: {FilePath}", filePath);
                return false;
            }
        }

        /// <summary>
        /// Loads abbreviations from a legacy XML file
        /// </summary>
        /// <param name="filePath">Path to XML file</param>
        /// <returns>true on success</returns>
        private bool LoadFromXml(string filePath)
        {
            var doc = new XmlDocument();

            try
            {
                _logger?.LogDebug("Loading abbreviations from legacy XML file: {FilePath}", filePath);
                
                doc.Load(filePath);

                XmlNodeList abbrNodes = doc.SelectNodes("/ACAT/Abbreviations/Abbreviation");

                if (abbrNodes != null)
                {
                    // load all the abbreviations
                    foreach (XmlNode node in abbrNodes)
                    {
                        createAndAddAbbreviation(node);
                    }
                }

                _logger?.LogInformation("Loaded {Count} abbreviations from XML", _abbreviationList.Count);
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error processing abbreviations XML file {FilePath}", filePath);
                return false;
            }
        }

        /// <summary>
        /// Returns the abbreviation object that corresponds to the
        /// mnemonic
        /// </summary>
        /// <param name="mnemonic">Mnemonic to look for</param>
        /// <returns>Abbreviation object if found null otherwise</returns>
        public Abbreviation Lookup(String mnemonic)
        {
            var lookupString = mnemonic.ToUpper();

            // do we detect something?
            if (_abbreviationList.ContainsKey(lookupString))
            {
                _logger?.LogDebug("Yes. Abbreviation list contains : {LookupString}", lookupString);
                return _abbreviationList[lookupString];
            }

            return null;
        }

        /// <summary>
        /// Removes an abbreviation from the list
        /// </summary>
        /// <param name="abbreviation">mnemonic of abbr to remove</param>
        /// <returns>true on success</returns>
        public bool Remove(String abbreviation)
        {
            bool retVal = true;
            try
            {
                if (Exists(abbreviation))
                {
                    _abbreviationList.Remove(abbreviation);
                }
            }
            catch
            {
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Saves abbreviations to the specified file in JSON format
        /// </summary>
        /// <param name="abbreviationsFile">name of the file</param>
        /// <returns>true on success</returns>
        public bool Save(String abbreviationsFile)
        {
            bool retVal = true;

            try
            {
                // Create JSON configuration object
                var config = new AbbreviationsJson();
                
                foreach (Abbreviation abbr in _abbreviationList.Values)
                {
                    config.Abbreviations.Add(new AbbreviationJson
                    {
                        Word = abbr.Mnemonic,
                        ReplaceWith = abbr.Expansion,
                        Mode = abbr.Mode.ToString()
                    });
                }

                // Save using JsonConfigurationLoader
                var loader = new JsonConfigurationLoader<AbbreviationsJson>(
                    new AbbreviationsValidator(), 
                    _logger);
                
                retVal = loader.Save(config, abbreviationsFile);
                
                if (retVal)
                {
                    _logger?.LogInformation("Saved {Count} abbreviations to JSON: {FilePath}", 
                        _abbreviationList.Count, abbreviationsFile);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error saving abbreviations to: {AbbreviationsFile}", abbreviationsFile);
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Saves all the abbreviations from the sorted list
        /// to the abbreviations file
        /// </summary>
        /// <returns>true on success</returns>
        public bool Save()
        {
            return Save(GetAbbreviationsFilePath());
        }

        /// <summary>
        /// Updates an existing abbreviation object.
        /// </summary>
        /// <param name="abbreviation">Abbreviation to update</param>
        /// <returns>true if updated successfully</returns>
        public bool Update(Abbreviation abbreviation)
        {
            if (Exists(abbreviation.Mnemonic))
            {
                Remove(abbreviation.Mnemonic);
                return Add(abbreviation);
            }

            return false;
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
                _logger?.LogTrace("");

                if (disposing)
                {
                    foreach (Abbreviation abbr in _abbreviationList.Values)
                    {
                        abbr.Dispose();
                    }

                    _abbreviationList.Clear();
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        /// Writes the closing xml element and closes the xmltextwriter object
        /// </summary>
        /// <param name="xmlTextWriter">opened xml writer object</param>
        private void closeAbbreviationFile(XmlWriter xmlTextWriter)
        {
            try
            {
                xmlTextWriter.WriteEndDocument();
                xmlTextWriter.Flush();
                xmlTextWriter.Close();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, ex.Message);
            }
        }

        /// <summary>
        /// Creates an empty abbreviations file
        /// </summary>
        /// <param name="fileName">Name of the file</param>
        /// <returns>XmlTextWriter object</returns>
        private XmlTextWriter createAbbreviationsFile(String fileName)
        {
            XmlTextWriter xmlTextWriter;

            // overwrite even if it already exists
            try
            {
                xmlTextWriter = new XmlTextWriter(fileName, null) { Formatting = Formatting.Indented };
                xmlTextWriter.WriteStartDocument();
                xmlTextWriter.WriteStartElement("ACAT");
                xmlTextWriter.WriteStartElement("Abbreviations");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, ex.Message);
                xmlTextWriter = null;
            }

            return xmlTextWriter;
        }

        /// <summary>
        /// Parses the xml node attributes from the xml nodeand
        /// creates an abbreviation object adds it to the sorted list
        /// </summary>
        /// <param name="node">Source xml node</param>
        private void createAndAddAbbreviation(XmlNode node)
        {
            var word = XmlUtils.GetXMLAttrString(node, WordAttr).Trim();
            var replaceWith = XmlUtils.GetXMLAttrString(node, ReplaceWithAttr);
            var mode = XmlUtils.GetXMLAttrString(node, ModeAttr).Trim();
            Add(new Abbreviation(word, replaceWith, mode));
        }
    }
}