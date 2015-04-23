////////////////////////////////////////////////////////////////////////////
// <copyright file="SwitchMap.cs" company="Intel Corporation">
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

namespace ACAT.Lib.Core.ActuatorManagement
{
    /// <summary>
    /// Stores a list of actuators for a specific scanner, or a
    /// class of scanners.  Each actuator has list of switches,
    /// and each switch has an  action associated with it.
    /// </summary>
    public class SwitchMap
    {
        /// <summary>
        /// Maps an actuator name to the the Actuator object
        /// </summary>
        private readonly Dictionary<String, SwitchMapActuator> _actuatorsTable;

        /// <summary>
        /// Initializes the SwitchMap class
        /// </summary>
        public SwitchMap()
        {
            Enabled = true;
            _actuatorsTable = new Dictionary<String, SwitchMapActuator>();
        }

        /// <summary>
        /// Gets or sets the enabled property
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the scanner name to which this switch map belongs
        /// </summary>
        public String Screen { get; set; }

        /// <summary>
        /// Gets a collection of SwtichMapActuator objects
        /// </summary>
        public ICollection<SwitchMapActuator> SwitchMapActuators
        {
            get { return _actuatorsTable.Values; }
        }

        /// <summary>
        /// Get the actuator object for the specified acutator name
        /// </summary>
        /// <param name="actuatorName">name of the actuator</param>
        /// <returns>the object</returns>
        public SwitchMapActuator this[String actuatorName]
        {
            get
            {
                SwitchMapActuator retVal;
                _actuatorsTable.TryGetValue(actuatorName.ToLower(), out retVal);
                return retVal;
            }
        }

        /// <summary>
        /// Loads values from the config file
        /// </summary>
        /// <param name="configFile">Full path to the config xml file</param>
        /// <returns>true on success</returns>
        public bool Load(XmlNode node)
        {
            Screen = XmlUtils.GetXMLAttrString(node, "screen");
            Enabled = XmlUtils.GetXMLAttrBool(node, "enabled", true);

            XmlNodeList actuatorNodes = node.SelectNodes("Actuators/Actuator");

            // load all the elements
            foreach (XmlNode actuatorNode in actuatorNodes)
            {
                var actuatorName = XmlUtils.GetXMLAttrString(actuatorNode, "name");
                Log.Debug("actuatorName:  " + actuatorName);
                if (!String.IsNullOrEmpty(actuatorName))
                {
                    actuatorName = actuatorName.ToLower();
                    var switchMapActuator = new SwitchMapActuator();
                    if (!_actuatorsTable.ContainsKey(actuatorName) && switchMapActuator.Load(actuatorNode))
                    {
                        Log.Debug("Adding actuatorNode " + actuatorName);
                        _actuatorsTable.Add(actuatorName, switchMapActuator);
                    }
                }
            }

            return true;
        }
    }
}