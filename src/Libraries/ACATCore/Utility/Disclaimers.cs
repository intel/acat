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
    /// Manages disclaimers that could be displayed in an
    /// About box for example
    /// </summary>
    public static class Disclaimers
    {
        /// <summary>
        /// Stores mapping between the attribution source and the
        /// attribution string
        /// </summary>
        private static readonly Dictionary<String, String> _disclaimers = new Dictionary<string, string>();

        /// <summary>
        /// Adds a disclaimer. Token is a string to ensure if the
        /// same 3rd party adds disclaimers multiple times, they
        /// are not duplicated. Use the same token value to prevent this.
        /// </summary>
        /// <param name="source">source of the disclaimer</param>
        /// <param name="disclaimer">the disclaimer string</param>
        /// <returns></returns>
        public static bool Add(String source, String disclaimer)
        {
            bool retVal = !_disclaimers.ContainsKey(source);

            if (retVal)
            {
                _disclaimers.Add(source, disclaimer);
            }

            return retVal;
        }

        /// <summary>
        /// Gets the disclaimer  for the specified source
        /// </summary>
        /// <param name="source">source</param>
        /// <returns>disclaimer  value, empty string if not found</returns>
        public static String Get(String source)
        {
            return (_disclaimers.ContainsKey(source) ? _disclaimers[source] : String.Empty);
        }

        /// <summary>
        /// Gets all the disclaimers
        /// </summary>
        /// <returns>string array of all the disclaimers</returns>
        public static IEnumerable<String> GetAll()
        {
            return _disclaimers.Values;
        }
    }
}