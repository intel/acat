////////////////////////////////////////////////////////////////////////////
// <copyright file="Scripts.cs" company="Intel Corporation">
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
using System.Collections;
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

namespace ACAT.Lib.Core.Interpreter
{
    /// <summary>
    /// A hashtable that contains a list of scripts loaded from
    /// the config file.  The list is indexed by the script name and
    /// the values are the interpreted pCode's of the scripts
    /// </summary>
    internal class Scripts : Hashtable
    {
        /// <summary>
        /// Interpreter to parse the script
        /// </summary>
        private readonly Parser _parser;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Scripts()
        {
            _parser = new Parser();
        }

        /// <summary>
        /// Load scripts from the config file (node ACAT/Scripts/Script)
        /// </summary>
        /// <param name="configFile">Name of the config file</param>
        /// <returns>true onf success</returns>
        public bool Load(String configFile)
        {
            bool retVal = true;

            XmlDocument doc = new XmlDocument();

            if (File.Exists(configFile))
            {
                doc.Load(configFile);

                XmlNodeList scriptNodes = doc.SelectNodes("/ACAT/Scripts/Script");

                // enumerate all the script nodes, extract attributes
                foreach (XmlNode node in scriptNodes)
                {
                    var name = XmlUtils.GetXMLAttrString(node, "name");
                    if (!String.IsNullOrEmpty(name))
                    {
                        if (!ContainsKey(name))
                        {
                            var code = XmlUtils.GetXMLAttrString(node, "code");

                            // now, let's parse the script and add the pcode to the
                            // hashtable
                            var pCode = new PCode();
                            bool ret = _parser.Parse(code, ref pCode);
                            if (ret)
                            {
                                Add(name, pCode);
                            }
                        }
                    }
                }
            }
            else
            {
                retVal = false;
            }

            return retVal;
        }
    }
}