////////////////////////////////////////////////////////////////////////////
// <copyright file="PreferredPanelConfig.cs" company="Intel Corporation">
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

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Stores a mapping between preferred config names and the scanners
    /// If a scanner has multiple animation config files, this allows for
    /// selecting a preferred config file from the list.
    /// </summary>
    internal class PreferredPanelConfig
    {
        /// <summary>
        /// Name of the preferred config file
        /// </summary>
        private const String PreferredPanelConfigFileName = "PreferredPanelConfig.xml";

        /// <summary>
        ///
        /// </summary>
        private readonly Dictionary<String, Dictionary<String, String>> _mapping;

        /// <summary>
        /// List of names of preferered config names
        /// </summary>
        private String[] _preferredConfigNames;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public PreferredPanelConfig()
        {
            _mapping = new Dictionary<String, Dictionary<String, String>>();
        }

        public String[] PreferredPanelConfigNames
        {
            get { return _preferredConfigNames; }
            set
            {
                var configNames = new string[value.Length];
                for (int ii = 0; ii < value.Length; ii++)
                {
                    configNames[ii] = value[ii].ToLower().Trim();
                }
                _preferredConfigNames = configNames;
            }
        }

        /// <summary>
        /// Looks up the map table and returns the name of the
        /// configuration for the specified scanner
        /// </summary>
        /// <param name="panelClass">scanner</param>
        /// <returns>name of the configuration</returns>
        public String GetConfigName(String panelClass)
        {
            foreach (var configName in _preferredConfigNames)
            {
                if (_mapping.ContainsKey(configName))
                {
                    var panelMapping = _mapping[configName];
                    String str = panelClass.ToLower().Trim();
                    if (panelMapping.ContainsKey(str))
                    {
                        return panelMapping[str];
                    }
                }
            }
            return String.Empty;
        }

        /// <summary>
        /// Parses the preferred panel config file name, populates
        /// the map table
        /// </summary>
        /// <returns>true on success</returns>
        public bool Load()
        {
            bool retVal = true;

            var file = UserManagement.ProfileManager.GetFullPath(PreferredPanelConfigFileName);

            loadPreferredPanelConfig(file);

            file = UserManagement.UserManager.GetFullPath(PreferredPanelConfigFileName);

            loadPreferredPanelConfig(file);

            return retVal;
        }

        private Dictionary<String, String> loadConfig(XmlNode node)
        {
            var mapping = new Dictionary<String, String>();

            var name = XmlUtils.GetXMLAttrString(node, "name");
            if (String.IsNullOrEmpty(name))
            {
                return mapping;
            }

            var configNodes = node.SelectNodes("PanelConfig");
            if (configNodes == null)
            {
                return mapping;
            }

            foreach (XmlNode configNode in configNodes)
            {
                var configName = XmlUtils.GetXMLAttrString(configNode, "configName").Trim().ToLower();
                var panelClass = XmlUtils.GetXMLAttrString(configNode, "panelClass").Trim().ToLower();

                if (!String.IsNullOrEmpty(configName) && !String.IsNullOrEmpty(panelClass))
                {
                    if (!mapping.ContainsKey(panelClass))
                    {
                        mapping.Add(panelClass, configName);
                    }
                }
            }

            return mapping;
        }

        /// <summary>
        /// Parses the preferred panel config file name, populates
        /// the map table
        /// </summary>
        /// <returns>true on success</returns>
        private bool loadPreferredPanelConfig(String file)
        {
            bool retVal = true;

            if (!File.Exists(file))
            {
                return true;
            }

            try
            {
                var doc = new XmlDocument();

                doc.Load(file);

                var configNodes = doc.SelectNodes("/ACAT/PanelConfigs");
                if (configNodes == null)
                {
                    return false;
                }

                // load each scheme from the config file
                foreach (XmlNode node in configNodes)
                {
                    String name = XmlUtils.GetXMLAttrString(node, "name").ToLower().Trim();
                    if (String.IsNullOrEmpty(name) || _mapping.ContainsKey(name))
                    {
                        Log.Debug("PreferredPanelconfig will not be added. Name either already exists or is empty " +
                                  name);
                        continue;
                    }

                    var mapping = loadConfig(node);
                    if (mapping.Values.Count > 0)
                    {
                        _mapping.Add(name, mapping);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
                retVal = false;
            }

            return retVal;
        }
    }
}