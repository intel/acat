////////////////////////////////////////////////////////////////////////////
// <copyright file="XmlUtils.cs" company="Intel Corporation">
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
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

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

namespace ACAT.Lib.Core.Utility
{
    /// <summary>
    /// Helper functions to parse an xml file, extracts attributes, serialize
    /// and deserialize classes to xml files.
    /// </summary>
    public class XmlUtils
    {
        /// <summary>
        /// Returns value for an xml attribute. If the attr was
        /// not found in the xml node, returns the default value.
        /// Value is returned as a boolean.  The original value
        /// in the xml should be either "true" or "false".
        /// </summary>
        /// <param name="node">node containing the attribute</param>
        /// <param name="attrName">name of the attribute</param>
        /// <param name="defaultValue">default value</param>
        /// <returns>the boolean value</returns>
        public static bool GetXMLAttrBool(XmlNode node, string attrName, bool defaultValue)
        {
            bool retVal = defaultValue;

            try
            {
                XmlAttribute attr = node.Attributes[attrName];
                if (attr == null)
                {
                    return defaultValue;
                }

                if (String.Compare(attr.Value, "true", true) == 0)
                {
                    retVal = true;
                }
                else if (String.Compare(attr.Value, "false", true) == 0)
                {
                    retVal = false;
                }
            }
            catch
            {
                retVal = defaultValue;
            }

            return retVal;
        }

        /// <summary>
        /// Returns value for an xml attribute. If the attr was
        /// not found in the xml node, returns the default value.
        /// Value is returned as an int.  Default val is returned
        /// if the value in the xml could not be converted to an
        /// integer
        /// </summary>
        /// <param name="node">node containing the attribute</param>
        /// <param name="attrName">name of the attribute</param>
        /// <param name="defaultValue">default value</param>
        /// <returns>the integer value</returns>
        public static int GetXMLAttrInt(XmlNode node, string attrName, int defaultValue)
        {
            int retVal = defaultValue;

            try
            {
                XmlAttribute attr = node.Attributes[attrName];
                return (attr != null) ? Convert.ToInt32(attr.Value) : defaultValue;
            }
            catch
            {
                retVal = defaultValue;
            }

            return retVal;
        }

        /// <summary>
        /// Returns value for an xml attribute in the xml node
        /// </summary>
        /// <param name="node">The XML node</param>
        /// <param name="attrName">The name of the attribute</param>
        /// <returns>Value if attribute was specified, empty string otherwise</returns>
        public static String GetXMLAttrString(XmlNode node, string attrName)
        {
            try
            {
                XmlAttribute attr = node.Attributes[attrName];
                return (attr != null) ? attr.Value : String.Empty;
            }
            catch
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Returns value for an xml attribute. If the attr was
        /// not found in the xml node, returns the default value.
        /// Value is returned as a string
        /// </summary>
        /// <param name="node">node containing the attribute</param>
        /// <param name="attrName">name of the attribute</param>
        /// <param name="defaultValue">default value</param>
        /// <returns>the string value</returns>
        public static String GetXMLAttrString(XmlNode node, string attrName, string defaultValue)
        {
            try
            {
                XmlAttribute attr = node.Attributes[attrName];
                return (attr != null) ? attr.Value : defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Deserializes an object from a string. Returns the created object
        /// </summary>
        /// <typeparam name="T">class to deserialize</typeparam>
        /// <param name="objectData">xml data</param>
        /// <param name="obj">created object</param>
        /// <returns>true on success</returns>
        public static bool XmlDeserializeFromString<T>(string objectData, out T obj)
        {
            bool retVal = true;

            try
            {
                var serializer = new XmlSerializer(typeof(T));

                using (TextReader reader = new StringReader(objectData))
                {
                    obj = (T)serializer.Deserialize(reader);
                }
            }
            catch (Exception e)
            {
                Log.Exception(e);
                retVal = false;
                obj = default(T);
            }

            return retVal;
        }

        /// <summary>
        /// De-serializes an object from the file.  Returns the object
        /// </summary>
        /// <typeparam name="T">class to deserialize</typeparam>
        /// <param name="filename">file to serialize from</param>
        /// <returns>object</returns>
        public static T XmlFileLoad<T>(string filename)
        {
            T retVal = default(T);

            try
            {
                using (TextReader outputStream = new StreamReader(filename))
                {
                    var xml = new XmlSerializer(typeof(T));
                    retVal = (T)xml.Deserialize(outputStream);
                }
            }
            catch (Exception e)
            {
                retVal = default(T);
                Log.Info("Error.  FileName: " + filename + ". Error: " + e.ToString());
            }

            return retVal;
        }

        /// <summary>
        /// Serialized an object to the specified file.  The class
        /// must be flagged as Serializable
        /// </summary>
        /// <typeparam name="T">class to serialize</typeparam>
        /// <param name="o">object to serialize</param>
        /// <param name="filename">file to serialize to</param>
        /// <returns>true on success</returns>
        public static bool XmlFileSave<T>(T o, string filename)
        {
            bool retVal = true;

            try
            {
                using (TextWriter outputStream = new StreamWriter(filename))
                {
                    var xml = new XmlSerializer(typeof(T));
                    xml.Serialize(outputStream, o);
                }
            }
            catch (Exception e)
            {
                retVal = false;
                Log.Error("XmlFileSave error.  FileName: " + filename + ". Error: " + e.ToString());
            }

            return retVal;
        }

        /// <summary>
        /// Serializes an object to a string representation
        /// </summary>
        /// <typeparam name="T">class to serialize</typeparam>
        /// <param name="obj">object to serialize</param>
        /// <returns>serialized xml string</returns>
        public static string XmlSerializeToString<T>(T obj)
        {
            var serializer = new XmlSerializer(typeof(T));
            var sb = new StringBuilder();

            using (TextWriter writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, obj);
            }

            return sb.ToString();
        }
    }
}