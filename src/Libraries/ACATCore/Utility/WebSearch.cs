////////////////////////////////////////////////////////////////////////////
// <copyright file="WebSearch.cs" company="Intel Corporation">
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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Security.Permissions;
using System.Text;

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
    /// Performs a google or a wiki search on a search
    /// string.  Launchesthe preferred browser and shows the
    /// result of the search
    /// </summary>
    public class WebSearch
    {
        /// <summary>
        /// URL to do a google search with "I'm feeling lucky"
        /// </summary>
        private const String QuickSearchUrl = "http://www.google.com/search?btnI=I&ie=UTF-8&oe=UTF-8&btnI&q=";

        /// <summary>
        /// URL to do a google search
        /// </summary>
        private const String SearchUrl = "http://google.com/search?q=";

        /// <summary>
        /// URL to do a wiki search
        /// </summary>
        private const String WikiSearchUrl = "http://www.google.com/search?btnI=I%27m+Feeling+Lucky&ie=UTF-8&oe=UTF-8&q=";

        /// <summary>
        /// Which brwoser to use.  Path to the
        /// browser (eg IExlpore.exe)
        /// </summary>
        private readonly String _preferredBrowser;

        //String quickSearchUrl = "http://www.google.com/search?btnI=I%27m+Feeling+Lucky&ie=UTF-8&oe=UTF-8&q=";
        //String wikiSearchUrl = "http://en.wikipedia.org/wiki/Special:Search?search=";

        /// <summary>
        /// Initializes a new instance of the class..  Set to null
        /// or empty string to launch internet explorer
        /// </summary>
        /// <param name="preferredBrowser">which browser to use (eg IExplore.exe)</param>
        public WebSearch(String preferredBrowser)
        {
            _preferredBrowser = String.IsNullOrEmpty(preferredBrowser.Trim()) ? "IExplore.exe" : preferredBrowser;
        }

        /// <summary>
        /// Does a google search on the text and launches preferred browser
        /// </summary>
        /// <param name="searchTerm">search term</param>
        /// <returns>Process object for the browser</returns>
        public Process GoogleSearch(String searchTerm)
        {
            Process retVal = null;

            var query = normalizeQuery(searchTerm);

            if (!String.IsNullOrEmpty(query))
            {
                var url = SearchUrl + query;
                retVal = Process.Start(_preferredBrowser, url);
            }

            return retVal;
        }

        /// <summary>
        /// Does a "I'm feeling lucky" search in Wikipedia site on the search term
        /// </summary>
        /// <param name="searchTerm">search term</param>
        /// <returns>Process launched</returns>
        public Process QuickSearch(String searchTerm)
        {
            Process retVal = null;

            var query = normalizeQuery(searchTerm);

            if (!String.IsNullOrEmpty(query))
            {
                var url = QuickSearchUrl + query + " -site:wikipedia.org";
                retVal = Process.Start(_preferredBrowser, url);
            }

            return retVal;
        }

        /// <summary>
        /// Does a wiki search on the search term
        /// </summary>
        /// <param name="searchTerm">search text</param>
        /// <returns>process launched</returns>
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        public Process WikiSearch(String searchTerm)
        {
            Process retVal = null;

            if (!String.IsNullOrEmpty(searchTerm))
            {
                String url = WikiSearchUrl + searchTerm + " site:wikipedia.org";
                retVal = Process.Start(_preferredBrowser, url);
            }

            return retVal;
        }

        /// <summary>
        /// Normalizes the search query by removing tabs and
        /// spaces and replacing them with +
        /// </summary>
        /// <param name="queryTerm">input search term</param>
        /// <returns>normalized search term</returns>
        private String normalizeQuery(String queryTerm)
        {
            var words = queryTerm.Split(' ', '\t');

            var sb = new StringBuilder();
            if (words.Length > 0)
            {
                sb.Append(words[0]);
                for (int ii = 1; ii < words.Length; ii++)
                {
                    if (words[ii].Length > 0)
                    {
                        sb.Append("+");
                        sb.Append(words[ii]);
                    }
                }

                return sb.ToString();
            }

            return String.Empty;
        }
    }
}