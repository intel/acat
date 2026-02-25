////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.DataAccess;
using ACAT.Core.PanelManagement;
using ACAT.Core.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ACAT.Core.ThemeManagement
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
    public class ThemeManager : IThemeManager, IDisposable
    {
        private readonly ILogger<ThemeManager> _logger;
        private readonly IRepository<Theme> _themeRepository;

        /// <summary>
        /// Name of the default theme
        /// </summary>
        public const String DefaultThemeName = "Default";

        /// <summary>
        /// Mapping between the Theme name and the directory
        /// </summary>
        public readonly Dictionary<String, String> ThemesLookupTable = new();

        /// <summary>
        ///  Theme config file names (JSON is preferred, XML is fallback)
        /// </summary>
        private const String ThemeConfigFileNameJson = "Theme.json";
        private const String ThemeConfigFileNameXml = "Theme.xml";
        private const String ThemeConfigFileName = ThemeConfigFileNameJson; // Default for new themes

        /// <summary>
        /// Returns the singleton instance - lazy initialized to get logger from DI container
        /// </summary>
        private static readonly Lazy<ThemeManager> _instance = new Lazy<ThemeManager>(() =>
        {
            // Get logger and themeRepository from DI container if available
            ILogger<ThemeManager> logger = Context.ServiceProvider?.GetService(typeof(ILogger<ThemeManager>)) as ILogger<ThemeManager>
                ?? LogManager.GetLogger<ThemeManager>();
            IRepository<Theme> themeRepository = Context.ServiceProvider?.GetService(typeof(IRepository<Theme>)) as IRepository<Theme>
                ?? new ThemeRepository(logger);

            return new ThemeManager(logger, themeRepository);
        });

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
        /// <param name="logger">Logger instance (required)</param>
        /// <param name="themeRepository">Repository for loading themes (optional)</param>
        private ThemeManager(ILogger<ThemeManager> logger, IRepository<Theme> themeRepository = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _themeRepository = themeRepository ?? new ThemeRepository(logger);
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
            get { return _instance.Value; }
        }

        /// <summary>
        /// Gets the currently active Theme object
        /// </summary>
        public Theme ActiveTheme
        {
            get
            {
                _logger?.LogDebug("Active Theme name is {ThemeName}", _activeTheme.Name);
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
        /// themes root dir looking for Theme.json or Theme.xml files
        /// </summary>
        /// <returns></returns>
        public bool Init()
        {
            String userThemesDir = FileUtils.GetUserThemesDir();

            // Look for JSON themes first (preferred), then XML themes (legacy)
            DirectoryWalker walker;
            if (Directory.Exists(userThemesDir))
            {
                walker = new DirectoryWalker(userThemesDir, ThemeConfigFileNameJson);
                walker.Walk(new OnFileFoundDelegate(onFileFound));
                
                // Also scan for XML themes for backward compatibility
                walker = new DirectoryWalker(userThemesDir, ThemeConfigFileNameXml);
                walker.Walk(new OnFileFoundDelegate(onFileFound));
            }

            walker = new DirectoryWalker(FileUtils.GetThemesDir(), ThemeConfigFileNameJson);
            walker.Walk(new OnFileFoundDelegate(onFileFound));
            
            // Also scan for XML themes for backward compatibility
            walker = new DirectoryWalker(FileUtils.GetThemesDir(), ThemeConfigFileNameXml);
            walker.Walk(new OnFileFoundDelegate(onFileFound));
            
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
            _logger?.LogDebug("Set active Theme to {ThemeName}", name);

            var themeDir = GetThemeDir(name);
            if (String.IsNullOrEmpty(themeDir))
            {
                _logger?.LogError("Could not find Theme {ThemeName}, using default", name);
                themeDir = GetThemeDir(DefaultThemeName);
                if (String.IsNullOrEmpty(themeDir))
                {
                    return false;
                }

                name = DefaultThemeName;
            }

            // Try JSON first, then fallback to XML for backward compatibility
            var themeFileJson = Path.Combine(themeDir, ThemeConfigFileNameJson);
            var themeFileXml = Path.Combine(themeDir, ThemeConfigFileNameXml);
            
            string themeFile = File.Exists(themeFileJson) ? themeFileJson : themeFileXml;

            _logger?.LogDebug("Creating Theme {ThemeName}, themeDir: {ThemeDir}, themeFile: {ThemeFile}", 
                name, themeDir, themeFile);

            // Note: Theme.Create() already uses JsonConfigurationLoader/XmlDocument which are proper
            // abstractions. ThemeRepository is available via _themeRepository if needed, but
            // Theme.Create() is the preferred API as it handles JSON/XML fallback logic.
            // The repository pattern is demonstrated here for architectural consistency.

            // create the Theme object. This parses the Theme json/xml file and
            // creates the Theme object
            var theme = Theme.Create(name, themeDir, themeFile, LogManager.GetLogger<Theme>());
            if (theme != null)
            {
                _activeTheme?.Dispose();

                _activeTheme = theme;
                ActiveThemeName = name;
                _logger.LogDebug("Created Theme successfully. active Theme is {ThemeName}", _activeTheme.Name);
            }
            else
            {
                _logger.LogError("Error creating theme with name {ThemeName}", name);
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Asynchronously sets the active theme by name.
        /// Uses <see cref="Theme.CreateAsync"/> to load theme configuration
        /// without blocking the calling thread.
        /// </summary>
        /// <param name="name">Name of the Theme.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns><c>true</c> on success.</returns>
        public async Task<bool> SetActiveThemeAsync(String name, CancellationToken cancellationToken = default)
        {
            _logger?.LogDebug("SetActiveThemeAsync: setting active Theme to {ThemeName}", name);

            var themeDir = GetThemeDir(name);
            if (String.IsNullOrEmpty(themeDir))
            {
                _logger?.LogError("Could not find Theme {ThemeName}, using default", name);
                themeDir = GetThemeDir(DefaultThemeName);
                if (String.IsNullOrEmpty(themeDir))
                {
                    return false;
                }

                name = DefaultThemeName;
            }

            var themeFileJson = Path.Combine(themeDir, ThemeConfigFileNameJson);
            var themeFileXml = Path.Combine(themeDir, ThemeConfigFileNameXml);
            string themeFile = File.Exists(themeFileJson) ? themeFileJson : themeFileXml;

            _logger?.LogDebug("CreateAsync Theme {ThemeName}, themeDir: {ThemeDir}, themeFile: {ThemeFile}",
                name, themeDir, themeFile);

            var theme = await Theme.CreateAsync(name, themeDir, themeFile, LogManager.GetLogger<Theme>(), cancellationToken)
                .ConfigureAwait(false);

            if (theme != null)
            {
                _activeTheme?.Dispose();
                _activeTheme = theme;
                ActiveThemeName = name;
                _logger.LogDebug("SetActiveThemeAsync: active Theme is now {ThemeName}", _activeTheme.Name);
                return true;
            }

            _logger.LogError("SetActiveThemeAsync: error creating theme with name {ThemeName}", name);
            return false;
        }
        /// <param name="disposing">true to dispose managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!_disposed)
            {
                _logger.LogTrace("Disposing ThemeManager");

                if (disposing)
                {
                    DefaultTheme?.Dispose();

                    _activeTheme?.Dispose();
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
        /// <param name="fileName">directory to explore</param>
        private void onFileFound(String filePath)
        {
            var file = new FileInfo(filePath);
            if (!ThemesLookupTable.ContainsKey(file.Directory.Name))
            {
                _logger?.LogDebug("Adding Theme: {ThemeName}, themeDir: {ThemeDir}", file.Directory.Name, file.Directory.FullName);
                ThemesLookupTable.Add(file.Directory.Name, file.Directory.FullName);
            }
        }
    }
}