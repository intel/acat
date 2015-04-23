////////////////////////////////////////////////////////////////////////////
// <copyright file="Variables.cs" company="Intel Corporation">
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
    /// Maintains a system-wide name value pairs for variables that
    /// are used in the scripts
    /// </summary>
    internal class Variables
    {
        public static string CurrentScreen = "CurrentScreen";

        public static string HesitateTime = "HesitateTime";

        public static string SelectedBox = "SelectedBox";

        public static string SelectedButton = "SelectedButton";

        public static string SelectedRow = "SelectedRow";

        // set of supported variable names
        public static string SelectedWidget = "SelectedWidget";

        /// <summary>
        /// The name-value pair list
        /// </summary>
        private readonly Hashtable _list;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Variables()
        {
            _list = new Hashtable();
        }

        /// <summary>
        /// Clears the name-value list
        /// </summary>
        public void Clear()
        {
            _list.Clear();
        }

        /// <summary>
        /// Removes the variable from the list
        /// </summary>
        /// <param name="variableName">Name of the variable</param>
        public void Clear(String variableName)
        {
            if (_list.ContainsKey(variableName))
            {
                _list.Remove(variableName);
            }
        }

        /// <summary>
        /// Returns the value
        /// </summary>
        /// <param name="variableName">Name of the variable</param>
        /// <returns>Value if found, null otherwise</returns>
        public object Get(String variableName)
        {
            return _list.ContainsKey(variableName) ? _list[variableName] : null;
        }

        /// <summary>
        /// Returns a boolean representation of the value. The value
        /// should be either "true" or "false".
        /// </summary>
        /// <param name="variableName">Name of the variable</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>The boolean value</returns>
        public bool GetBool(String variableName, bool defaultValue)
        {
            bool retVal = defaultValue;
            const string strTrue = "true";
            const string strFalse = "false";

            if (!_list.ContainsKey(variableName))
            {
                return retVal;
            }

            var strValue = GetString(variableName);
            if (String.Compare(strValue, strTrue, true) == 0)
            {
                retVal = true;
            }
            else if (String.Compare(strValue, strFalse, true) == 0)
            {
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Returns an integer representation of the value
        /// </summary>
        /// <param name="variableName">Name of the variable</param>
        /// <param name="defaultValue">The default value</param>
        /// <returns>A integer representation.  Null if var not found</returns>
        public int GetInt(String variableName, int defaultValue)
        {
            int retVal;
            String str = GetString(variableName);
            if (String.IsNullOrEmpty(str))
            {
                return defaultValue;
            }

            try
            {
                retVal = Convert.ToInt32(str);
            }
            catch
            {
                retVal = defaultValue;
            }

            return retVal;
        }

        /// <summary>
        /// Returns a string representation of the value
        /// </summary>
        /// <param name="variableName">Name of the variable</param>
        /// <returns>A string representation.  Null if var not found</returns>
        public String GetString(String variableName)
        {
            object value = Get(variableName);
            return (value != null) ? value.ToString() : String.Empty;
        }

        /// <summary>
        /// Sets the variable to the value.  If the variable
        /// is not already in the list, adds it. Value can be
        /// a string, an int, a boolean etc
        /// </summary>
        /// <param name="variableName">Name of the variable</param>
        /// <param name="value">It's value</param>
        public void Set(String variableName, object value)
        {
            if (_list.ContainsKey(variableName))
            {
                _list[variableName] = value;
            }
            else
            {
                _list.Add(variableName, value);
            }
        }
    }
}