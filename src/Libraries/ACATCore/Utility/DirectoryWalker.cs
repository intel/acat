////////////////////////////////////////////////////////////////////////////
// <copyright file="DirectoryWalker.cs" company="Intel Corporation">
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

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
    public delegate void OnDirectoryFoundDelegate(String dirName);

    public delegate void OnFileFoundDelegate(String fileName);

    /// <summary>
    /// Walks a specified directory recursively, looks for files that
    /// match a specified wildcard and invokes a callback
    /// function for every matching file it finds.
    /// </summary>
    public class DirectoryWalker
    {
        /// <summary>
        /// Invoked when a directory is found
        /// </summary>
        private OnDirectoryFoundDelegate _dirFoundDelegate;

        /// <summary>
        /// Invoked when a matching file is found
        /// </summary>
        private OnFileFoundDelegate _fileFoundDelegate;

        /// <summary>
        /// The directory to walk
        /// </summary>
        private String _rootDir = String.Empty;

        /// <summary>
        /// Files to look for
        /// </summary>
        private String _wildCard = String.Empty;

        /// <summary>
        /// Initialzies an instance of the class.  Finds all
        /// files in the specified directory
        /// </summary>
        /// <param name="rootDir">Directory to walk</param>
        public DirectoryWalker(String rootDir)
        {
            _rootDir = rootDir;
            _wildCard = String.Empty;
            _fileFoundDelegate = null;
            _dirFoundDelegate = null;
        }

        /// <summary>
        /// Initialzes an instance of the class. Finds all files that
        /// match the wildcard
        /// </summary>
        /// <param name="rootDir">directory to walk</param>
        /// <param name="fileWildCard">files to find</param>
        public DirectoryWalker(String rootDir, String fileWildCard)
        {
            _rootDir = rootDir;
            _wildCard = fileWildCard;
            _fileFoundDelegate = null;
            _dirFoundDelegate = null;
        }

        /// <summary>
        /// Walks the directory.  If reecursive is true, goes
        /// into all the subfolders as well. Finds all the sub
        /// folders in the directory
        /// </summary>
        /// <param name="dirFoundDelegate">invoked when a subfolder is found</param>
        /// <param name="recursive">set to true for recursive</param>
        public void Walk(OnDirectoryFoundDelegate dirFoundDelegate, bool recursive = true)
        {
            Walk(dirFoundDelegate, null);
        }

        /// <summary>
        /// Walks the directory.  If reecursive is true, goes
        /// into all the subfolders as well. Finds all the matching
        /// files in the directory
        /// </summary>
        /// <param name="fileFoundDelegate">invoked when a file is found</param>
        /// <param name="recursive">set to true for recursive</param>
        public void Walk(OnFileFoundDelegate fileFoundDelegate, bool recursive = true)
        {
            Walk(null, fileFoundDelegate);
        }

        /// <summary>
        /// Walks the directory.  If reecursive is true, goes
        /// into all the subfolders as well. Finds all the sub
        /// folders and matching files in the directory
        /// </summary>
        /// <param name="dirFoundDelegate">invoked when a subfolder is found</param>
        /// <param name="fileFoundDelegate">invoked when a file is found</param>
        /// <param name="recursive">set to true for recursive</param>
        public void Walk(OnDirectoryFoundDelegate dirFoundDelegate, OnFileFoundDelegate fileFoundDelegate, bool recursive = true)
        {
            if (Directory.Exists(_rootDir) && (dirFoundDelegate != null || fileFoundDelegate != null))
            {
                _dirFoundDelegate = dirFoundDelegate;
                _fileFoundDelegate = fileFoundDelegate;
                if (_fileFoundDelegate != null && String.IsNullOrEmpty(_wildCard))
                {
                    _wildCard = "*.*";
                }

                if (fileFoundDelegate != null)
                {
                    listFiles(_rootDir);
                }

                listDirs(_rootDir, recursive);
            }
        }

        /// <summary>
        /// Lists all the subfolders in the specified root folder. Invokes
        /// callback if directory is found
        /// </summary>
        /// <param name="dirPath">root folder</param>
        /// <param name="recursive">set to true for recursive</param>
        private void listDirs(string dirPath, bool recursive)
        {
            try
            {
                var dirs = new List<string>(Directory.EnumerateDirectories(dirPath));

                foreach (var dir in dirs)
                {
                    if (_dirFoundDelegate != null)
                    {
                        _dirFoundDelegate(dir);
                    }

                    if (_fileFoundDelegate != null)
                    {
                        listFiles(dir);
                    }

                    if (recursive)
                    {
                        listDirs(dir, recursive);
                    }
                }
            }
            catch (UnauthorizedAccessException uaex)
            {
                Console.WriteLine(uaex.Message);
            }
            catch (PathTooLongException pathex)
            {
                Console.WriteLine(pathex.Message);
            }
        }

        /// <summary>
        /// Lists all the files for the specified folder and
        /// invokes the callback when matching files are found
        /// </summary>
        /// <param name="dirPath">the folder</param>
        private void listFiles(string dirPath)
        {
            string[] filePaths = Directory.GetFiles(dirPath, _wildCard);
            if (filePaths.Length > 0)
            {
                foreach (string str in filePaths)
                {
                    _fileFoundDelegate(str);
                }
            }
        }
    }
}