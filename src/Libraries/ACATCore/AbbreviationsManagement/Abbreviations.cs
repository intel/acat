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

using ACAT.Core.UserManagement;
using ACAT.Core.Utility;
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
        /// Name of the abbreviations file
        /// </summary>
        public const string AbbreviationFile = "Abbreviations.xml";

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
        /// folder under the culture specific folder first.
        /// </summary>
        /// <returns>full path to the abbreviations file</returns>
        public string GetAbbreviationsFilePath()
        {
            var abbreviationsFile = Path.Combine(UserManager.GetResourcesDir(), AbbreviationFile);

            return File.Exists(abbreviationsFile) ? abbreviationsFile : UserManager.GetFullPath(AbbreviationFile);
        }

        /// <summary>
        /// Loads abbreviations from the specified file.  If filename
        /// is null, loads from the default file.  Parses the XML file
        /// and populates the sorted list
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

            var doc = new XmlDocument();

            try
            {
                _abbreviationList.Clear();

                if (File.Exists(abbreviationsFile))
                {
                    doc.Load(abbreviationsFile);

                    var abbrNodes = doc.SelectNodes("/ACAT/Abbreviations/Abbreviation");

                    if (abbrNodes != null)
                    {
                        // load all the abbreviations
                        foreach (XmlNode node in abbrNodes)
                        {
                            createAndAddAbbreviation(node);
                        }
                    }
                }
                else
                {
                    _logger?.LogDebug("Abbreviation file {AbbreviationsFile} does not exist", abbreviationsFile);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error processing abbreviations file {AbbreviationsFile}", abbreviationsFile);
                retVal = false;
            }

            return retVal;
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
        /// Saves abbreviations to the specified file
        /// </summary>
        /// <param name="abbreviationsFile">name of the file</param>
        /// <returns>true on success</returns>
        public bool Save(String abbreviationsFile)
        {
            bool retVal = true;

            try
            {
                XmlTextWriter xmlTextWriter = createAbbreviationsFile(abbreviationsFile);
                if (xmlTextWriter != null)
                {
                    foreach (Abbreviation abbr in _abbreviationList.Values)
                    {
                        xmlTextWriter.WriteStartElement("Abbreviation");
                        xmlTextWriter.WriteAttributeString(WordAttr, abbr.Mnemonic);
                        xmlTextWriter.WriteAttributeString(ReplaceWithAttr, abbr.Expansion);
                        xmlTextWriter.WriteAttributeString(ModeAttr, abbr.Mode.ToString());

                        xmlTextWriter.WriteEndElement();
                    }

                    closeAbbreviationFile(xmlTextWriter);
                }
            }
            catch (IOException ex)
            {
                _logger?.LogError(ex, ex.Message);
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