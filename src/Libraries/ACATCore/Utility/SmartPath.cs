////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Globalization;
using System.IO;
using System.Reflection;

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
                return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
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

        public static string DocsPath
        {
            get
            {
                return Path.Combine(ApplicationPath, "Docs", CultureInfo.DefaultThreadCurrentUICulture.TwoLetterISOLanguageName);
            }
        }
    }
}