////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using Microsoft.Extensions.Logging;
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
        private readonly ILogger<DirectoryWalker> _logger;

        ///// <summary>
        ///// Invoked when a directory is found
        ///// </summary>
        //private OnDirectoryFoundDelegate _dirFoundDelegate = null;

        ///// <summary>
        ///// Invoked when a matching file is found
        ///// </summary>
        //private OnFileFoundDelegate _fileFoundDelegate = null;

        /// <summary>
        /// The directory to walk
        /// </summary>
        private readonly String _rootDir;

        /// <summary>
        /// Files to look for
        /// </summary>
        private readonly String _wildCard = "*.*";

        /// <summary>
        /// Initializes an instance of the class.  Finds all
        /// files in the specified directory
        /// </summary>
        /// <param name="rootDir">Directory to walk</param>
        public DirectoryWalker(String rootDir) : this(rootDir, string.Empty)
        {
        }
        
        public DirectoryWalker(String rootDir, String fileWildCard, ILogger<DirectoryWalker> logger = null)
        {
            _logger = logger ?? LogManager.GetLogger<DirectoryWalker>();
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

        public void Walk(OnFileFoundDelegate fileFoundDelegate)
        {
            try
            {
                IEnumerable<string> dllFiles = Directory.EnumerateFiles(_rootDir, _wildCard, SearchOption.AllDirectories);

                foreach (var file in dllFiles)
                {
                    _logger.LogTrace("Found file: " + file);
                    fileFoundDelegate?.Invoke(file);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Access denied: " + ex.Message);
            }
            catch (IOException ex)
            {
                _logger.LogTrace("IO error: " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: " + ex.Message);
            }
        }

    }
}