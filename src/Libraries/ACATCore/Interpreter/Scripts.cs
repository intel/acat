////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;
using System;
using System.Collections;
using System.IO;
using System.Xml;

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
        /// Loads scripts from the config file (node ACAT/Scripts/Script)
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