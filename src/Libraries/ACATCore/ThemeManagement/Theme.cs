////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Configuration;
using ACAT.Core.DataAccess;
using ACAT.Core.Utility;
using ACAT.Core.Validation;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace ACAT.Core.ThemeManagement
{
    /// <summary>
    /// Contains all the attribtues for a Theme. This includes
    /// the color schemes for all the various UI elements such
    /// as Scanners, Dialogs, Menus, buttons in scanners etc.
    /// The theme is loaded from a JSON file
    /// </summary>
    public class Theme : IDisposable
    {
        /// <summary>
        /// Name of the preview screnshot image file
        /// </summary>
        public const String PreviewScannerImageName = "Preview.png";

        private readonly ILogger<Theme> _logger;

        /// <summary>
        /// Has this object been disposed?
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="name">Name of the color scheme</param>
        private Theme(String name, ILogger<Theme> logger = null)
        {
            _logger = logger;
            Name = name;
            Colors = new ColorSchemes();
        }

        /// <summary>
        /// Collection of color schemes for this theme
        /// </summary>
        public ColorSchemes Colors { get; private set; }

        /// <summary>
        /// Gets the name of the theme
        /// </summary>
        public String Name { get; private set; }

        /// <summary>
        /// Class factory to create a theme object with the specified name.
        /// </summary>
        /// <param name="name">Name of theme</param>
        /// <returns>Theme object</returns>
        public static Theme Create(String name)
        {
            return new Theme(name);
        }

        /// <summary>
        /// Class factory to create a Theme object with the specified name. themeDir
        /// directory contains all the assets for the Theme. themeFile is the json
        /// file that contains references to all the theme assets.
        /// </summary>
        /// <param name="themeName">Name of the theme</param>
        /// <param name="themeDir">directory where theme assets are located</param>
        /// <param name="themeFile">name of the theme config file</param>
        /// <returns></returns>
        public static Theme Create(String themeName, String themeDir, String themeFile, ILogger<Theme> logger = null)
        {
            Theme theme = null;

            if (!File.Exists(themeFile))
            {
                logger?.LogWarning("Theme file not found: {ThemeFile}", themeFile);
                return null;
            }

            try
            {
                // Try loading as JSON first
                var validator = new ThemeValidator();
                var loader = new JsonConfigurationLoader<ThemeJson>(validator, logger);

                ThemeJson themeJson = loader.Load(themeFile, createDefaultOnError: false);
                
                if (themeJson != null)
                {
                    theme = new Theme(themeName, logger) 
                    { 
                        Colors = ColorSchemes.CreateFromJson(themeJson.ColorSchemes, themeDir) 
                    };
                    
                    logger?.LogInformation("Successfully loaded theme from JSON: {ThemeFile}", themeFile);
                    return theme;
                }

                // Fallback to XML if JSON fails (for backward compatibility during transition)
                logger?.LogWarning("JSON load failed, attempting XML fallback for: {ThemeFile}", themeFile);
                theme = LoadFromXml(themeName, themeDir, themeFile, logger);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Failed to create theme from file: {ThemeFile}", themeFile);
            }

            return theme;
        }

        /// <summary>
        /// Asynchronously creates a <see cref="Theme"/> object from the specified file.
        /// Uses <see cref="JsonConfigurationLoader{T}.LoadAsync"/> so the calling thread
        /// is not blocked during file I/O. Falls back to synchronous XML loading when the
        /// file has a <c>.xml</c> extension (XML parsing has no async variant).
        /// </summary>
        /// <param name="themeName">Name of the theme.</param>
        /// <param name="themeDir">Directory containing theme assets.</param>
        /// <param name="themeFile">Path to the theme configuration file.</param>
        /// <param name="logger">Optional logger.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>The created <see cref="Theme"/>, or <c>null</c> on failure.</returns>
        public static async Task<Theme> CreateAsync(String themeName, String themeDir, String themeFile,
            ILogger<Theme> logger = null, CancellationToken cancellationToken = default)
        {
            Theme theme = null;

            if (!File.Exists(themeFile))
            {
                logger?.LogWarning("Theme file not found: {ThemeFile}", themeFile);
                return null;
            }

            try
            {
                var validator = new ThemeValidator();
                var loader = new JsonConfigurationLoader<ThemeJson>(validator, logger);
                ThemeJson themeJson = await loader.LoadAsync(themeFile, createDefaultOnError: false, cancellationToken).ConfigureAwait(false);

                if (themeJson != null)
                {
                    theme = new Theme(themeName, logger)
                    {
                        Colors = ColorSchemes.CreateFromJson(themeJson.ColorSchemes, themeDir)
                    };
                    logger?.LogInformation("Successfully loaded theme from JSON: {ThemeFile}", themeFile);
                    return theme;
                }

                // Fallback to XML for backward compatibility; XML has no async variant so run on thread pool
                logger?.LogWarning("JSON load failed, attempting XML fallback for: {ThemeFile}", themeFile);
                theme = await Task.Run(() => LoadFromXml(themeName, themeDir, themeFile, logger), cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Failed to create theme from file: {ThemeFile}", themeFile);
            }

            return theme;
        }

        /// <summary>
        /// Legacy XML loader for backward compatibility
        /// </summary>
        private static Theme LoadFromXml(String themeName, String themeDir, String themeFile, ILogger<Theme> logger)
        {
            Theme theme = null;

            try
            {
                var doc = new XmlDocument();
                doc.Load(themeFile);

                // create the colorschemes object by parsing the colorschemes nodes
                XmlNode colorSchemesNode = doc.SelectSingleNode("/ACAT/Theme/ColorSchemes");
                if (colorSchemesNode != null)
                {
                    theme = new Theme(themeName, logger) { Colors = ColorSchemes.Create(colorSchemesNode, themeDir) };
                    logger?.LogInformation("Loaded theme from XML (legacy): {ThemeFile}", themeFile);
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Failed to load theme from XML: {ThemeFile}", themeFile);
            }

            return theme;
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
        /// Disposer. Release resources and cleanup.
        /// </summary>
        /// <param name="disposing">true to dispose managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!_disposed)
            {
                _logger?.LogTrace("Disposing Theme");

                if (disposing)
                {
                    Colors.Dispose();
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }
    }
}