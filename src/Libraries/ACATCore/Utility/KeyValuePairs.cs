////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Text;

namespace ACAT.Lib.Core.Utility
{
    /// <summary>
    /// Maintains a keyvalue pair table where the value can be a bool,
    /// double, float, string, int and has get/set functions for each
    /// of these
    /// </summary>
    public class KeyValuePairs
    {
        private Hashtable hash;

        public KeyValuePairs()
        {
            hash = new Hashtable();
        }

        public KeyValuePairs(String dataString)
        {
            Parse(dataString);
        }

        public bool GetValueBool(String key, out bool value, bool defaultValue)
        {
            bool retVal = true;

            if (KeyExists(key))
            {
                retVal = bool.TryParse(hash[key].ToString(), out value);
            }
            else
            {
                value = defaultValue;
                retVal = false;
            }

            return retVal;
        }

        public bool GetValueDouble(String key, out double value, double defaultValue = 0)
        {
            bool retVal = true;

            if (KeyExists(key))
            {
                retVal = Double.TryParse(hash[key].ToString(), out value);
            }
            else
            {
                value = defaultValue;
                retVal = false;
            }

            return retVal;
        }

        public bool GetValueFloat(String key, out float value, float defaultValue)
        {
            bool retVal = true;

            if (KeyExists(key))
            {
                retVal = float.TryParse(hash[key].ToString(), out value);
            }
            else
            {
                value = defaultValue;
                retVal = false;
            }

            return retVal;
        }

        public bool GetValueInt(String key, out int value, int defaultValue = 0)
        {
            bool retVal = true;

            if (KeyExists(key))
            {
                retVal = Int32.TryParse(hash[key].ToString(), out value);
            }
            else
            {
                value = defaultValue;
                retVal = false;
            }

            return retVal;
        }

        public bool GetValueString(String key, out String value, String defaultValue = "")
        {
            bool retVal = true;

            if (KeyExists(key))
            {
                value = hash[key].ToString();
            }
            else
            {
                value = defaultValue;
                retVal = false;
            }

            return retVal;
        }

        public bool KeyExists(String key)
        {
            return hash.Contains(key);
        }

        public void Parse(String dataString)
        {
            if (hash == null)
            {
                hash = new Hashtable();
            }

            String[] array = dataString.Split(';');
            if (array.Length > 0)
            {
                foreach (String str in array)
                {
                    String[] keyValuePair = str.Split('=');
                    if (keyValuePair.Length == 2)
                    {
                        hash.Add(keyValuePair[0].Trim(), keyValuePair[1].Trim());
                    }
                }
            }
        }

        public override String ToString()
        {
            var sb = new StringBuilder();
            foreach (String key in hash.Keys)
            {
                sb.AppendLine(key + "=" + hash[key]);
            }

            return sb.ToString();
        }
    }
}