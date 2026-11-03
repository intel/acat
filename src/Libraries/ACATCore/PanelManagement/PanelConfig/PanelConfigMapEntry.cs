////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Xml;

namespace ACAT.Core.PanelManagement.PanelConfig
{
    /// <summary>
    /// Holds config information about a scanner - its type,
    /// its GUID, the associated animation xml file name
    /// </summary>
    public class PanelConfigMapEntry
    {
        private static readonly ILogger _logger = LogManager.GetLogger(typeof(PanelConfigMapEntry).Name);

        private readonly Dictionary<string, string> _userControlsDict = new();

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public PanelConfigMapEntry()
        {
            PanelClass = string.Empty;
            ConfigName = string.Empty;
            ConfigFileName = string.Empty;
            FormId = Guid.Empty;
            Description = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="configId">config id</param>
        /// <param name="panelClass">class of the scanner</param>
        /// <param name="configName">animation config name </param>
        /// <param name="configFileName">animation config file name for the scanner</param>
        /// <param name="formId">guid of the scanner</param>
        /// <param name="formType">.NET class Type of the scanner</param>
        public PanelConfigMapEntry(Guid configId, string panelClass, string configName, string configFileName, Guid formId, Type formType, string description = null)
        {
            PanelClass = panelClass;
            ConfigName = configName;
            ConfigFileName = configFileName;
            FormId = formId;
            FormType = formType;
            ConfigId = configId;
            Description = description ?? string.Empty;
        }

        /// <summary>
        /// Gets the animation config file name for the scanner
        /// </summary>
        public string ConfigFileName { get; internal set; }

        /// <summary>
        /// Unique id for this entry
        /// </summary>
        public Guid ConfigId { get; internal set; }

        /// <summary>
        /// Gets the name of the configuration for the scanner
        /// </summary>
        public string ConfigName { get; internal set; }

        /// <summary>
        /// User friendly description of this entry
        /// </summary>
        public string Description { get; internal set; }

        /// <summary>
        /// Gets the ACAT descriptor guid for the scanner
        /// </summary>
        public Guid FormId { get; internal set; }

        /// <summary>
        /// Returns the .NET class type for the scanner
        /// </summary>
        public Type FormType { get; internal set; }

        /// <summary>
        /// Gets the scanner class
        /// </summary>
        public string PanelClass { get; internal set; }

        public string GetUserControlName(string key)
        {
            var str = key.ToLower();
            return _userControlsDict.ContainsKey(str) ? _userControlsDict[str] : string.Empty;
        }

        /// <summary>
        /// Checks if the specified map entry is the
        /// same as this object
        /// </summary>
        /// <param name="mapEntry">entry to compare</param>
        /// <returns>true if it is</returns>
        public bool IsEqual(PanelConfigMapEntry mapEntry)
        {
            bool retVal;

            if (mapEntry.FormId != Guid.Empty && FormId != Guid.Empty)
            {
                retVal = FormId.Equals(mapEntry.FormId);
            }
            else
            {
                retVal = string.Compare(ConfigName, mapEntry.ConfigName, true) == 0;
            }

            return retVal;
        }

        /// <summary>
        /// Parses the xml node, extracts the attributes from
        /// the file
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool Load(XmlNode node)
        {
            bool retVal;

            ConfigFileName = XmlUtils.GetXMLAttrString(node, "configFile").ToLower();
            ConfigName = XmlUtils.GetXMLAttrString(node, "configName");
            PanelClass = XmlUtils.GetXMLAttrString(node, "panelClass");
            Description = XmlUtils.GetXMLAttrString(node, "description");
            var userControls = XmlUtils.GetXMLAttrString(node, "userControls");

            parseUserControlsList(userControls);

            var configIdString = XmlUtils.GetXMLAttrString(node, "configId");

            if (string.IsNullOrEmpty(PanelClass))
            {
                PanelClass = ConfigName;
            }

            string guidString = XmlUtils.GetXMLAttrString(node, "formId");

            if (string.IsNullOrEmpty(ConfigFileName) ||
                string.IsNullOrEmpty(ConfigName) ||
                string.IsNullOrEmpty(configIdString) ||
                string.IsNullOrEmpty(guidString))
            {
                retVal = false;
            }
            else
            {
                retVal = Guid.TryParse(guidString, out Guid guid);
                if (retVal)
                {
                    FormId = guid;
                }

                retVal = Guid.TryParse(configIdString, out guid);
                if (retVal)
                {
                    ConfigId = guid;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Converts to a string representation
        /// </summary>
        /// <returns>string representation</returns>
        public override string ToString()
        {
            if (FormType == null)
            {
                _logger.LogDebug("FORM TYPE IS NULL");
            }

            return "PanelConfigMapEntry. configName: " + ConfigName +
                    ", formId: " + FormId +
                    ", configFile: " + ConfigFileName +
                    " FormType: " + (FormType != null ? FormType.Name : "<not set>");
        }

        private void parseUserControlsList(string userControlsList)
        {
            if (string.IsNullOrEmpty(userControlsList))
            {
                return;
            }

            var nameValuePairs = userControlsList.Split(';');
            if (nameValuePairs.Length == 0)
            {
                return;
            }

            foreach (var nameValuePair in nameValuePairs)
            {
                var tokens = nameValuePair.Split('=');
                if (tokens.Length == 2)
                {
                    var str = tokens[0].ToLower().Trim();
                    if (_userControlsDict.ContainsKey(str))
                    {
                        _userControlsDict[str] = tokens[1].Trim();
                    }
                    else
                    {
                        _userControlsDict.Add(str, tokens[1].Trim());
                    }
                }
            }
        }
    }
}