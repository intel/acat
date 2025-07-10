////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ACAT.Core.Utility
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
    /// <remarks>
    /// Initialzes an instance of the class. Finds all files that
    /// match the wildcard
    /// </remarks>
    /// <param name="rootDir">directory to walk</param>
    /// <param name="fileWildCard">files to find</param>
    public class DirectoryWalker
    {
        /// <summary>
        /// Invoked when a directory is found
        /// </summary>
        private OnDirectoryFoundDelegate _dirFoundDelegate = null;

        /// <summary>
        /// Invoked when a matching file is found
        /// </summary>
        private OnFileFoundDelegate _fileFoundDelegate = null;

        /// <summary>
        /// The directory to walk
        /// </summary>
        private readonly String _rootDir;

        /// <summary>
        /// Files to look for
        /// </summary>
        private String _wildCard;

        /// <summary>
        /// Initialzies an instance of the class.  Finds all
        /// files in the specified directory
        /// </summary>
        /// <param name="rootDir">Directory to walk</param>
        public DirectoryWalker(String rootDir) : this(rootDir, string.Empty)
        {
        }
        
        public DirectoryWalker(String rootDir, String fileWildCard)
        {
            if (String.IsNullOrEmpty(rootDir) || !Directory.Exists(rootDir))
            {
                throw new DirectoryNotFoundException($"Directory not found: {rootDir}");
            }
            _rootDir = rootDir;
            _wildCard = fileWildCard;
        }

        private bool IsSkippableDirectory(String dirPath)
        {
            String[] skipdirs = { "external", "ConvAssistApp", "Install" };

            string dirName = Path.GetFileName(dirPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

            return skipdirs.Any(skip =>
               dirName.IndexOf(skip, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        public void Walk(OnDirectoryFoundDelegate dirFoundDelegate, bool recursive = false )
        {
            Walk(dirFoundDelegate, null, recursive);   
        }

        public void Walk(OnFileFoundDelegate fileFoundDelegate, bool recursive = false )
        {
            Walk(null, fileFoundDelegate, recursive);
        }
        
        /// <summary>
        /// Walks the directory.  If reecursive is true, goes
        /// into all the subfolders as well. Finds all the sub
        /// folders and matching files in the directory
        /// </summary>
        /// <param name="dirFoundDelegate">Invoked when a subfolder is found</param>
        /// <param name="fileFoundDelegate">Invoked when a file is found</param>
        /// <param name="recursive">Set to true for recursive. Default false</param>
        public void Walk(OnDirectoryFoundDelegate dirFoundDelegate,
                        OnFileFoundDelegate fileFoundDelegate,
                        bool recursive = false)
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
                    ListFiles(_rootDir);
                }

                if (recursive)
                {
                    ListDirs(_rootDir, recursive);
                }
            }
        }

        /// <summary>
        /// Lists all the subfolders in the specified root folder. Invokes
        /// callback if directory is found
        /// </summary>
        /// <param name="dirPath">root folder</param>
        /// <param name="recursive">set to true for recursive</param>
        private void ListDirs(string dirPath, bool recursive)
        {
            try
            {
                var dirs = new List<string>(Directory.EnumerateDirectories(dirPath));

                foreach (var dir in dirs)
                {
                    if (IsSkippableDirectory(dir))
                    {
                        continue;
                    }

                    _dirFoundDelegate?.Invoke(dir);

                    if (_fileFoundDelegate != null)
                    {
                        ListFiles(dir);
                    }

                    if (recursive)
                    {
                        ListDirs(dir, recursive);
                    }
                }
            }
            catch (UnauthorizedAccessException uaex)
            {
                Log.Exception(uaex.Message);
            }
            catch (PathTooLongException pathex)
            {
                Log.Exception(pathex.Message);
            }
            catch (Exception ex)
            {
                Log.Exception(ex.Message);
            }
        }

        /// <summary>
        /// Lists all the files for the specified folder and
        /// invokes the callback when matching files are found
        /// </summary>
        /// <param name="dirPath">the folder</param>
        private void ListFiles(string dirPath)
        {
            if (IsSkippableDirectory(dirPath))
            {
                return;
            }

            string[] filePaths = Directory.GetFiles(dirPath, _wildCard);
            if (filePaths.Length > 0)
            {
                foreach (string str in filePaths)
                {
                    _fileFoundDelegate?.Invoke(str);
                }
            }
        }
    }
}