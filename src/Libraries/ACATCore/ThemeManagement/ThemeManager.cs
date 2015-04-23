////////////////////////////////////////////////////////////////////////////
// <copyright file="ThemeManagement.cs" company="Intel Corporation">
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
using ACAT.Lib.Core.Utility;

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
        ///  Theme config file name
        /// </summary>
        private const String ThemeConfigFileName = "Skin.xml";

        /// <summary>
        /// Returns the singleton instance
        /// </summary>
        private static readonly ThemeManager _instance = new ThemeManager();

        /// <summary>
        /// Mapping between the Theme name and the directory
        /// </summary>
        private readonly Dictionary<String, String> _themesLookupTable = new Dictionary<String, String>();

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
            ActiveThemeName = "DefaultSkin";
            DefaultTheme = Theme.Create(ActiveThemeName);
            _activeTheme = Theme.Create(ActiveThemeName);
        }

        /// <summary>
        /// The default Theme
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
        /// Gets the name of the currently active  Theme
        /// </summary>
        public String ActiveThemeName
        {
            get;
            private set;
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

        public bool Init()
        {
            String userSkinsDir = FileUtils.GetUserSkinsDir();

            DirectoryWalker walker;
            if (Directory.Exists(userSkinsDir))
            {
                walker = new DirectoryWalker(userSkinsDir);
                walker.Walk(new OnDirectoryFoundDelegate(onDirFound));
            }

            walker = new DirectoryWalker(FileUtils.GetSkinsDir());
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
            var themeName = name.ToLower();
            Log.Debug("Set active Theme to " + themeName);

            if (!_themesLookupTable.ContainsKey(themeName))
            {
                Log.Debug("Could not find Theme " + themeName + " in the table");
                return false;
            }
            Log.Debug("Found Theme " + themeName + " in the table");

            var themeDir = _themesLookupTable[themeName];
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
            var themeName = components[components.Length - 1].ToLower();
            if (!_themesLookupTable.ContainsKey(themeName))
            {
                Log.Debug("Adding Theme: " + themeName + ", skinDir: " + dirName);
                _themesLookupTable.Add(themeName.ToLower(), dirName);
            }
        }
    }
}