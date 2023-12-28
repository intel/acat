////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;
using System;
using System.Xml;

namespace ACAT.Lib.Core.UserControlManagement
{
    /// <summary>
    /// Holds config information about a usercontrol - its type,
    /// its GUID, the associated animation xml file name
    /// </summary>
    public class UserControlConfigMapEntry
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public UserControlConfigMapEntry()
        {
            Name = String.Empty;
            ConfigName = String.Empty;
            ConfigFileName = String.Empty;
            UserControlId = Guid.Empty;
            Description = String.Empty;
            ConfigId = Guid.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public UserControlConfigMapEntry(Guid configId, String name, String configName, String configFileName, Guid userControlId, Type userControlType, String description = null)
        {
            Name = name;
            ConfigName = configName;
            ConfigFileName = configFileName;
            UserControlId = userControlId;
            UserControlType = userControlType;
            ConfigId = configId;
            Description = description ?? String.Empty;
        }

        /// <summary>
        /// Gets the animation config file name for the usercontrol
        /// </summary>
        public String ConfigFileName { get; internal set; }

        /// <summary>
        /// Unique id for this entry
        /// </summary>
        public Guid ConfigId { get; internal set; }

        /// <summary>
        /// Gets the name of the configuration for the usercontrol
        /// </summary>
        public String ConfigName { get; internal set; }

        /// <summary>
        /// User friendly description of this entry
        /// </summary>
        public String Description { get; internal set; }

        /// <summary>
        /// Gets the scanner class
        /// </summary>
        public String Name { get; internal set; }

        /// <summary>
        /// Gets the ACAT descriptor guid for the usercontrol
        /// </summary>
        public Guid UserControlId { get; internal set; }

        /// <summary>
        /// Returns the .NET class type for the usercontrol
        /// </summary>
        public Type UserControlType { get; internal set; }

        /// <summary>
        /// Checks if the specified map entry is the
        /// same as this object
        /// </summary>
        /// <param name="mapEntry">entry to compare</param>
        /// <returns>true if it is</returns>
        public bool IsEqual(UserControlConfigMapEntry mapEntry)
        {
            bool retVal;

            if (mapEntry.UserControlId != Guid.Empty && UserControlId != Guid.Empty)
            {
                retVal = UserControlId.Equals(mapEntry.UserControlId);
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
            Name = XmlUtils.GetXMLAttrString(node, "name");
            Description = XmlUtils.GetXMLAttrString(node, "description");

            var configIdString = XmlUtils.GetXMLAttrString(node, "configId");

            String guidString = XmlUtils.GetXMLAttrString(node, "userControlId");

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
                    UserControlId = guid;
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
            return "UserControlConfigMapEntry. configName: " + ConfigName +
                    ", userControlId: " + UserControlId +
                    ", configFile: " + ConfigFileName +
                    " UserControlType: " + (UserControlType != null ? UserControlType.Name : "<not set>");
        }
    }
}