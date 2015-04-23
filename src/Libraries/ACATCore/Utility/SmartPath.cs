////////////////////////////////////////////////////////////////////////////
// <copyright file="SmartPath.cs" company="Intel Corporation">
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
using System.Reflection;

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
    /// Handles path's and path conversions
    /// </summary>
    public class SmartPath
    {
        /// <summary>
        /// These are macros that can be specified in the path and
        /// this class has functions to replace them with the actual
        /// paths.
        /// </summary>
        private const string MyDocuments = "@mydocuments";

        private const string MyMusic = "@mymusic";
        private const string MyPictures = "@mypictures";
        private const string MyVideos = "@myvideos";

        /// <summary>
        /// Returns the full path to the application directory
        /// </summary>
        public static String ApplicationPath
        {
            get
            {
                return URItoPath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase));
            }
        }

        /// <summary>
        /// Normalizes the input path and returns the normalized path. By
        /// normalize, it means that macros such as @mydocuments or @mypictures
        /// are expanded into My Documents and My Pictures path.
        /// </summary>
        /// <param name="inputPath">path to normalize</param>
        /// <returns>normalized path</returns>
        public static String ACATNormalizePath(String inputPath)
        {
            var lowerCasePath = inputPath.ToLower().Trim();
            if (lowerCasePath.Contains(MyDocuments))
            {
                inputPath = inputPath.Replace(MyDocuments, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            }
            else if (lowerCasePath.Contains(MyMusic))
            {
                inputPath = inputPath.Replace(MyMusic, Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));
            }
            else if (lowerCasePath.Contains(MyPictures))
            {
                inputPath = inputPath.Replace(MyPictures, Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
            }
            else if (lowerCasePath.Contains(MyVideos))
            {
                inputPath = inputPath.Replace(MyVideos, Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));
            }
            return inputPath;
        }

        /// <summary>
        /// Normalizes a semi-colon delimited set of paths and returns
        /// an array of the normalized paths.
        /// By normalize, it means that macros such as @mydocuments or @mypictures
        /// are expanded into My Documents and My Pictures path.
        /// </summary>
        /// <param name="inputPaths">semi-colon delimited paths</param>
        /// <returns>array of normalized paths</returns>
        public static String[] ACATParseAndNormalizePaths(String inputPaths)
        {
            string[] folders = inputPaths.Split(';');
            for (int ii = 0; ii < folders.Length; ii++)
            {
                folders[ii] = ACATNormalizePath(folders[ii]);
            }
            return folders;
        }

        /// <summary>
        /// Converts a URI to a path
        /// </summary>
        /// <param name="uri">Input URI</param>
        /// <returns>Path</returns>
        public static String URItoPath(String uri)
        {
            // pattern looks like this as example file:\\D:
            if (System.Text.RegularExpressions.Regex.IsMatch(uri, "^file:\\\\[a-z,A-Z]:"))
            {
                // remove up to C:
                return uri.Substring(6);
            }

            if (uri.StartsWith(@"file:"))
            {
                // typical for Linux remove file:
                return uri.Substring(5);
            }

            // presumably this is a path, not a URI
            return uri;
        }
    }
}