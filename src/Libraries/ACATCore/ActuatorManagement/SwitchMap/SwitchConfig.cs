////////////////////////////////////////////////////////////////////////////
// <copyright file="SwitchConfig.cs" company="Intel Corporation">
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
using ACAT.Lib.Core.Interpreter;
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

namespace ACAT.Lib.Core.ActuatorManagement
{
    /// <summary>
    /// Stores a mapping between scanners and their SwitchMap elements.
    /// The switch mapping is a lookup table that maps
    /// a switch and an action (a command).  See SwitchMap class
    /// for details on this.  Each scanner can have its own customized
    /// switch map.
    /// </summary>
    public class SwitchConfig
    {
        /// <summary>
        /// Default value string
        /// </summary>
        private const String DefaultAttr = "default";

        /// <summary>
        /// Mapping between the name of a scanner (UI scanner or dialog) and the
        /// list of switches that are customized for that scanner.
        /// </summary>
        private readonly Dictionary<String, SwitchMap> _switchMapTable;

        /// <summary>
        /// Initializes the SwitchConfig object
        /// </summary>
        public SwitchConfig()
        {
            _switchMapTable = new Dictionary<String, SwitchMap>();
        }

        /// <summary>
        /// Gets the default action associated with the switch.  Default
        /// action applies to all screens for which a custom switch action
        /// has not been specified.
        /// </summary>
        /// <param name="switchObj">the switch object</param>
        /// <returns>PCode representing the action, null if none</returns>
        public PCode GetOnTrigger(IActuatorSwitch switchObj)
        {
            return GetOnTrigger(DefaultAttr, switchObj);
        }

        /// <summary>
        /// Returns onTrigger Pcode for the specified switch for the
        /// specified screen
        /// </summary>
        /// <param name="screen">Name of the screen</param>
        /// <param name="actuatorSwitch">Input switch</param>
        /// <returns>null if there is no onTrigger</returns>
        ///
        public PCode GetOnTrigger(String screen, IActuatorSwitch actuatorSwitch)
        {
            try
            {
                Log.Debug("Scanner: " + screen);
                var actuatorName = actuatorSwitch.Actuator.Name;
                Log.Debug("actuatorname: " + actuatorName);

                SwitchMap switchMap;

                if (_switchMapTable.TryGetValue(screen.ToLower(), out switchMap))
                {
                    Log.Debug("Getting swithcmapActuator for " + actuatorName);
                    var switchMapActuator = switchMap[actuatorName];
                    Log.IsNull("switchMapActuator ", switchMapActuator);

                    if (switchMapActuator != null)
                    {
                        Log.Debug("Geting switchElement " + actuatorSwitch.Name);
                        var switchElement = switchMapActuator[actuatorSwitch.Name];
                        Log.IsNull("switchElement ", switchElement);
                        if (switchElement != null)
                        {
                            Log.Debug("Found switchelement");
                            return switchElement.OnTrigger;
                        }
                    }
                }
                else
                {
                    Log.Debug("switchmap is null");
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }

            Log.Debug("Returning null");
            return null;
        }

        /// <summary>
        /// Loads the switch mappings from the config file
        /// </summary>
        /// <param name="configFile">Full path to the config xml file</param>
        /// <returns>true on success</returns>
        public bool Load(String configFile)
        {
            if (!File.Exists(configFile))
            {
                Log.Debug("File " + configFile + " does not exist");
                return false;
            }

            var doc = new XmlDocument();

            doc.Load(configFile);

            var switchMapNodes = doc.SelectNodes("/ACAT/SwitchMap");

            if (switchMapNodes == null)
            {
                return false;
            }

            // load all the elements
            foreach (XmlNode node in switchMapNodes)
            {
                // get the name of the scanner for which switch
                // mappings have been customzied.  Use "default"
                // as the name of the scanner if the customization
                // should apply to all scanners

                var panel = XmlUtils.GetXMLAttrString(node, "screen");

                if (String.IsNullOrEmpty(panel))
                {
                    panel = XmlUtils.GetXMLAttrString(node, "panel");
                }

                if (String.IsNullOrEmpty(panel))
                {
                    panel = DefaultAttr;
                }

                Log.Debug("panel: " + panel);

                // add it to our list
                panel = panel.ToLower();
                var switchMap = new SwitchMap();
                if (!_switchMapTable.ContainsKey(panel) && switchMap.Load(node))
                {
                    Log.Debug("Adding switchmap entry for panel " + panel);
                    _switchMapTable.Add(panel, switchMap);
                }
            }

            return true;
        }
    }
}