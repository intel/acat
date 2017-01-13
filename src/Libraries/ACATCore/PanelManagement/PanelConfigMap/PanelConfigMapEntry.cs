////////////////////////////////////////////////////////////////////////////
// <copyright file="PanelConfigMapEntry.cs" company="Intel Corporation">
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

using ACAT.Lib.Core.Utility;
using System;
using System.Xml;

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Holds config information about a scanner - its type,
    /// its GUID, the associated animation xml file name
    /// </summary>
    public class PanelConfigMapEntry
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public PanelConfigMapEntry()
        {
            PanelClass = String.Empty;
            ConfigName = String.Empty;
            ConfigFileName = String.Empty;
            FormId = Guid.Empty;
            Description = String.Empty;
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
        public PanelConfigMapEntry(Guid configId, String panelClass, String configName, String configFileName, Guid formId, Type formType, String description = null)
        {
            PanelClass = panelClass;
            ConfigName = configName;
            ConfigFileName = configFileName;
            FormId = formId;
            FormType = formType;
            ConfigId = configId;
            Description = description ?? String.Empty;
        }

        /// <summary>
        /// Gets the animation config file name for the scanner
        /// </summary>
        public String ConfigFileName { get; internal set; }

        /// <summary>
        /// Unique id for this entry
        /// </summary>
        public Guid ConfigId { get; internal set; }

        /// <summary>
        /// Gets the name of the configuration for the scanner
        /// </summary>
        public String ConfigName { get; internal set; }

        /// <summary>
        /// User friendly description of this entry
        /// </summary>
        public String Description { get; internal set; }

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
        public String PanelClass { get; internal set; }

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
                retVal = String.Compare(ConfigName, mapEntry.ConfigName, true) == 0;
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

            var configIdString = XmlUtils.GetXMLAttrString(node, "configId");

            if (String.IsNullOrEmpty(PanelClass))
            {
                PanelClass = ConfigName;
            }

            String guidString = XmlUtils.GetXMLAttrString(node, "formId");

            if (String.IsNullOrEmpty(ConfigFileName) ||
                String.IsNullOrEmpty(ConfigName) ||
                String.IsNullOrEmpty(configIdString) ||
                String.IsNullOrEmpty(guidString))
            {
                retVal = false;
            }
            else
            {
                Guid guid;
                retVal = Guid.TryParse(guidString, out guid);
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
            return "PanelConfigMapEntry. configName: " + ConfigName +
                    ", formId: " + FormId +
                    ", configFile: " + ConfigFileName +
                    " FormType: " + (FormType != null ? FormType.Name : "<not set>");
        }
    }
}