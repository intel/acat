////////////////////////////////////////////////////////////////////////////
// <copyright file="SwitchMapActuator.cs" company="Intel Corporation">
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
    /// Encapsulates a list of SwitchElement objects in the
    /// Switch map configuration.
    /// </summary>
    public class SwitchMapActuator
    {
        // contains a table of "Switch" elements
        private readonly Dictionary<String, SwitchElement> _switchTable;

        /// <summary>
        /// Initializes a SwitchMapActuator class
        /// </summary>
        public SwitchMapActuator()
        {
            _switchTable = new Dictionary<String, SwitchElement>();
        }

        /// <summary>
        /// Gets a collection of switch elements for this actuator
        /// </summary>
        public ICollection<SwitchElement> SwitchElements
        {
            get
            {
                return _switchTable.Values;
            }
        }

        /// <summary>
        /// Returns the switch element object for the specified
        /// switch name
        /// </summary>
        /// <param name="name">Name of the switch</param>
        /// <returns>Switch element object</returns>
        public SwitchElement this[String name]
        {
            get
            {
                SwitchElement retVal;
                _switchTable.TryGetValue(name.ToLower(), out retVal);
                return retVal;
            }
        }

        /// <summary>
        /// Loads attributes from the xml node
        /// </summary>
        /// <param name="node">Node containing attributes</param>
        /// <returns>true on success</returns>
        public bool Load(XmlNode node)
        {
            XmlNodeList switchNodes = node.SelectNodes("Switch");
            if (switchNodes == null)
            {
                return false;
            }
            // load all the elements
            foreach (XmlNode switchNode in switchNodes)
            {
                var switchName = XmlUtils.GetXMLAttrString(switchNode, "name");
                Log.Debug("SwitchName: " + switchName);
                if (String.IsNullOrEmpty(switchName))
                {
                    continue;
                }

                switchName = switchName.ToLower();
                var switchElement = new SwitchElement();
                if (!_switchTable.ContainsKey(switchName) && switchElement.Load(switchNode))
                {
                    Log.Debug("Adding switchElement " + switchName);
                    _switchTable.Add(switchName, switchElement);
                }
            }

            Log.Debug("Done walking through switchnodes");
            return true;
        }
    }
}