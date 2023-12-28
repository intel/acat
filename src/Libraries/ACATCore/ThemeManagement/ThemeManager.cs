////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.IO;

namespace ACAT.Lib.Core.ThemeManagement
{
    /// <summary>
    /// A singleton class that holds a mapping between a Theme name and
    /// the directory where all the Theme asssets are stored.  This class
    /// also maintains the currently active Theme. On startup, it descends
    /// into the Themes root folder and looks a for all the Theme config files
    /// located in the dir tree and creates the mapping table between the
    /// Theme name and the folder. The name of the Theme is the same as the
    /// name of the folder containing the Theme assets.
    /// </summary>
    ///
    public class ThemeManager : IDisposable
    {
        /// <summary>
        /// Name of the default theme
        /// </summary>
        public const String DefaultThemeName = "Default";

        /// <summary>
        /// Mapping between the Theme name and the directory
        /// </summary>
        public readonly Dictionary<String, String> ThemesLookupTable = new Dictionary<String, String>();

        /// <summary>
        ///  Theme config file name
        /// </summary>
        private const String ThemeConfigFileName = "Theme.xml";

        /// <summary>
        /// Returns the singleton instance
        /// </summary>
        private static readonly ThemeManager _instance = new ThemeManager();

        /// <summary>
        /// The current active ksin
        /// </summary>
        private Theme _activeTheme;

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Initializes the singleton instance of the manager
        /// </summary>
        private ThemeManager()
        {
            ActiveThemeName = DefaultThemeName;
            DefaultTheme = Theme.Create(ActiveThemeName);
            _activeTheme = Theme.Create(ActiveThemeName);
        }

        /// <summary>
        /// Gets or sets the default theme
        /// </summary>
        public static Theme DefaultTheme { get; set; }

        /// <summary>
        /// Gets the singleton instance
        /// </summary>
        public static ThemeManager Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Gets the currently active Theme object
        /// </summary>
        public Theme ActiveTheme
        {
            get
            {
                Log.Debug("Active Theme name is " + _activeTheme.Name);
                return _activeTheme;
            }
        }

        /// <summary>
        /// Gets the name of the currently active theme
        /// </summary>
        public String ActiveThemeName
        {
            get;
            private set;
        }

        /// <summary>
        /// Ges a list of thems discovered
        /// </summary>
        public IEnumerable<String> Themes
        {
            get { return ThemesLookupTable.Keys; }
        }

        /// <summary>
        /// Dispose resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // Prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the directory of the specified theme.
        /// Return empty string if theme invalid.
        /// </summary>
        /// <param name="theme">theme</param>
        /// <returns>theme diretory</returns>
        public String GetThemeDir(String theme)
        {
            foreach (var key in ThemesLookupTable.Keys)
            {
                if (String.Compare(key, theme, true) == 0)
                {
                    return ThemesLookupTable[key];
                }
            }

            return String.Empty;
        }

        /// <summary>
        /// Initializes the theme manager.  Walks the
        /// themes root dir.
        /// </summary>
        /// <returns></returns>
        public bool Init()
        {
            String userThemesDir = FileUtils.GetUserThemesDir();

            DirectoryWalker walker;
            if (Directory.Exists(userThemesDir))
            {
                walker = new DirectoryWalker(userThemesDir);
                walker.Walk(new OnDirectoryFoundDelegate(onDirFound));
            }

            walker = new DirectoryWalker(FileUtils.GetThemesDir());
            walker.Walk(new OnDirectoryFoundDelegate(onDirFound));
            return true;
        }

        /// <summary>
        /// Looks the themes table for the specified name and creates the Theme
        /// object
        /// </summary>
        /// <param name="name">Name of the Theme</param>
        /// <returns>true on success</returns>
        public bool SetActiveTheme(String name)
        {
            bool retVal = true;
            Log.Debug("Set active Theme to " + name);

            var themeDir = GetThemeDir(name);
            if (String.IsNullOrEmpty(themeDir))
            {
                Log.Debug("Could not find Theme " + name + ", using default");
                themeDir = GetThemeDir(DefaultThemeName);
                if (String.IsNullOrEmpty(themeDir))
                {
                    return false;
                }

                name = DefaultThemeName;
            }

            var themeFile = Path.Combine(themeDir, ThemeConfigFileName);

            Log.Debug("Creating Theme " + name + ", themeDir: " + themeDir);

            // create the Theme object. This parses the Theme xml file and
            // creates the Theme object
            var theme = Theme.Create(name, themeDir, themeFile);
            if (theme != null)
            {
                if (_activeTheme != null)
                {
                    _activeTheme.Dispose();
                }

                _activeTheme = theme;
                ActiveThemeName = name;
                Log.Debug("Created Theme successfully. active Theme is " + _activeTheme.Name);
            }
            else
            {
                Log.Debug("Error creating Theme");
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Disposer. Release resources and cleanup.
        /// </summary>
        /// <param name="disposing">true to dispose managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!_disposed)
            {
                Log.Debug();

                if (disposing)
                {
                    if (DefaultTheme != null)
                    {
                        DefaultTheme.Dispose();
                    }

                    if (_activeTheme != null)
                    {
                        _activeTheme.Dispose();
                    }
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        /// Call back function for the directory walker.  Checks if
        /// the directory holds a Theme.xml file and if it does, adds
        /// the directory name to the mapping table.  Name of the directory
        /// is also the name of the theme
        /// </summary>
        /// <param name="dirName">directory to explore</param>
        private void onDirFound(String dirName)
        {
            if (!File.Exists(Path.Combine(dirName, ThemeConfigFileName)))
            {
                return;
            }

            Log.Debug("Found Theme in  " + dirName);

            var components = dirName.Split('\\');
            var themeName = components[components.Length - 1];
            if (!ThemesLookupTable.ContainsKey(themeName))
            {
                Log.Debug("Adding Theme: " + themeName + ", themeDir: " + dirName);
                ThemesLookupTable.Add(themeName, dirName);
            }
        }
    }
}