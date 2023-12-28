////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace ACAT.Lib.Core.Utility
{
    /// <summary>
    /// Manages third party attributions that could be displayed in an
    /// About box for example
    /// </summary>
    public static class Attributions
    {
        /// <summary>
        /// Stores mapping between the attribution source and the
        /// attribution string
        /// </summary>
        private static readonly Dictionary<String, String> _attributions = new Dictionary<string, string>();

        /// <summary>
        /// Adds an attribution. Token is a string to ensure if the
        /// same 3rd party adds attributions multiple times, they
        /// are not duplicated. Use the same token value to prevent this.
        /// </summary>
        /// <param name="source">source of the attribution</param>
        /// <param name="attribution">the attribution string</param>
        /// <returns></returns>
        public static bool Add(String source, String attribution)
        {
            bool retVal = !_attributions.ContainsKey(source);

            if (retVal)
            {
                _attributions.Add(source, attribution);
            }

            return retVal;
        }

        /// <summary>
        /// Gets the attribution for the specified source
        /// </summary>
        /// <param name="source">source</param>
        /// <returns>attribution value, empty string if not found</returns>
        public static String Get(String source)
        {
            return (_attributions.ContainsKey(source) ? _attributions[source] : String.Empty);
        }

        /// <summary>
        /// Gets all the attributions
        /// </summary>
        /// <returns>string array of all the attributions</returns>
        public static IEnumerable<String> GetAll()
        {
            return _attributions.Values;
        }
    }
}