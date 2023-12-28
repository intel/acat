////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Linq;

namespace ACAT.Lib.Core.Utility
{
    /// <summary>
    ///  A simple lookup table with a CRUD interface
    /// </summary>
    public static class DataDictionary
    {
        private static Dictionary<object, object> _dictionary = new Dictionary<object, object>();

        public static bool Add(object name, object value, bool replaceIfExists = false)
        {
            if (name == null)
            {
                return false;
            }

            if (_dictionary.Keys.Contains(name))
            {
                if (replaceIfExists)
                {
                    _dictionary[name] = value;
                    return true;
                }

                return false;
            }

            _dictionary[name] = value;

            return true;
        }

        public static bool Contains(object name)
        {
            if (name == null)
            {
                return false;
            }

            return _dictionary.Keys.Contains(name);
        }

        public static object Get(object name)
        {
            if (name == null)
            {
                return null;
            }

            return (_dictionary.Keys.Contains(name)) ? _dictionary[name] : null;
        }

        public static ICollection<object> GetNames()
        {
            return _dictionary.Keys;
        }

        public static object Remove(object name)
        {
            if (name != null && _dictionary.Keys.Contains(name))
            {
                var retVal = _dictionary[name];
                _dictionary.Remove(name);
                return retVal;
            }

            return null;
        }

        public static bool Update(object name, object value, bool addIfDoesntExist = true)
        {
            if (name == null)
            {
                return false;
            }

            if (_dictionary.Keys.Contains(name))
            {
                _dictionary[name] = value;
                return true;
            }
            else if (addIfDoesntExist)
            {
                Add(name, value);
                return true;
            }

            return false;
        }
    }
}