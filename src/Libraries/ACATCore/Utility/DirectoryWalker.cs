////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;

namespace ACAT.Lib.Core.Utility
{
    public delegate void OnDirectoryFoundDelegate(String dirName);

    public delegate void OnFileFoundDelegate(String fileName);

    /// <summary>
    /// Walks a specified directory recursively, looks for files that
    /// match a specified wildcard and invokes a callback
    /// function for every matching file it finds.
    /// This class can be used to discover DLL's, font files, image files
    /// etc.
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