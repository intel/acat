////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;
using System;
using System.IO;
using System.Xml;

namespace ACAT.Lib.Core.ThemeManagement
{
    /// <summary>
    /// Contains all the attribtues for a Theme. This includes
    /// the color schemes for all the various UI elements such
    /// as Scanners, Dialogs, Menus, buttons in scanners etc.
    /// The theme is loaded from an XML file
    /// </summary>
    public class Theme : IDisposable
    {
        /// <summary>
        /// Name of the preview screnshot image file
        /// </summary>
        public const String PreviewScannerImageName = "Preview.png";

        /// <summary>
        /// Has this object been disposed?
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="name">Name of the color scheme</param>
        private Theme(String name)
        {
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
        /// directory contains all the assets for the Theme. themeFile is the xml
        /// file that contains references to all the theme assets.
        /// </summary>
        /// <param name="themeName">Name of the theme</param>
        /// <param name="themeDir">directory where theme assets are located</param>
        /// <param name="themeFile">name of the theme config file</param>
        /// <returns></returns>
        public static Theme Create(String themeName, String themeDir, String themeFile)
        {
            Theme theme = null;

            if (!File.Exists(themeFile))
            {
                return null;
            }

            try
            {
                var doc = new XmlDocument();

                doc.Load(themeFile);

                // create the colorschemes object by parsing the colorschemes nodes
                var colorSchemesNode = doc.SelectSingleNode("/ACAT/Theme/ColorSchemes");
                if (colorSchemesNode != null)
                {
                    theme = new Theme(themeName) { Colors = ColorSchemes.Create(colorSchemesNode, themeDir) };
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
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
                Log.Debug();

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