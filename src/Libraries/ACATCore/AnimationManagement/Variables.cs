////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;

namespace ACAT.Lib.Core.AnimationManagement
{
    /// <summary>
    /// Maintains a system-wide name value pairs for variables that
    /// are used in the scripts
    /// </summary>
    internal class Variables
    {
        public static string CurrentPanel = "CurrentScreen";
        public static string HesitateTime = "FirstPauseTime";
        public static string SelectedBox = "SelectedBox";
        public static string SelectedButton = "SelectedButton";
        public static string SelectedRow = "SelectedRow";
        public static string SelectedWidget = "SelectedWidget";

        /// <summary>
        /// The name-value pair list
        /// </summary>
        private readonly Hashtable _list;

        /// <summary>
        /// Constructor
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
            var str = GetString(variableName);
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
            //Log.Debug("Set variable " + variableName);
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