////////////////////////////////////////////////////////////////////////////
// <copyright file="SwitchElement.cs" company="Intel Corporation">
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
using System.Diagnostics.CodeAnalysis;
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
    /// Represents a switch map element that maps a switch
    /// to an action.  For instance, for a keyboard actuator,
    /// a switch can be configured for the key combination Ctrl-T
    /// and can be mapped to a function to toggle the talk window
    /// </summary>
    public class SwitchElement
    {
        /// <summary>
        /// Action to execute when switch is triggered.
        /// </summary>
        public PCode OnTrigger;

        /// <summary>
        /// To parse the onTrigger code
        /// </summary>
        private readonly Parser _parser;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public SwitchElement()
        {
            OnTrigger = null;
            SwitchName = String.Empty;
            _parser = new Parser();
        }

        /// <summary>
        /// Get or sets the enabled state
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets the name of the swtich
        /// </summary>
        public String SwitchName { get; private set; }

        /// <summary>
        /// Load attribute from the XML node
        /// </summary>
        /// <param name="xmlNode">Xml node</param>
        /// <returns>true on success</returns>
        public bool Load(XmlNode xmlNode)
        {
            SwitchName = XmlUtils.GetXMLAttrString(xmlNode, "name");
            var script = XmlUtils.GetXMLAttrString(xmlNode, "onTrigger");

            // a switch action of default() means this is used as a selector.
            if (!String.IsNullOrEmpty(script))
            {
                OnTrigger = new PCode();
                if (String.Compare(script, "default()", true) != 0)
                {
                    Log.Debug("Name: " + SwitchName + " onTrigger: " + script);
                    _parser.Parse(script, ref OnTrigger);
                }
                else
                {
                    Log.Debug("Name: " + SwitchName + " onTrigger: default()");
                }
            }
            else
            {
                Log.Debug("Name: " + SwitchName + " script is empty. setting onTrigger to NULL");
                OnTrigger = null;
            }

            return true;
        }
    }
}