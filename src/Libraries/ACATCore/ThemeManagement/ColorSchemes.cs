////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Xml;

namespace ACAT.Lib.Core.ThemeManagement
{
    /// <summary>
    /// Encapsulates a list of color scheme objects for the various ui elements
    /// and for the various panel types such as dialogs, scanners, buttons
    /// Each color scheme object contains the background/foreground color or image
    /// to be used in the various states of the UI element such as highlighted,
    /// normal, selected etc.
    /// </summary>
    public class ColorSchemes : IDisposable
    {
        /// <summary>
        /// Name of the color scheme for all the buttons
        /// </summary>
        public static String ButtonSchemeName = "Button";

        /// <summary>
        /// Name of the color scheme for the icons in contextual menus
        /// </summary>
        public static String ContextMenuIconSchemeName = "ContextMenuIconButton";

        /// <summary>
        /// Name of the color scheme for the text in the contextual menus
        /// </summary>
        public static string ContextMenuTextSchemeName = "ContextMenuTextButton";

        /// <summary>
        /// Name of the color scheme for the title in the contextual menus
        /// </summary>
        public static string ContextMenuTitleSchemeName = "ContextMenuTitle";

        /// <summary>
        /// Name of the default color scheme.  If a ui element does not have a
        /// color scheme defined, this is used as the default
        /// </summary>
        public static ColorScheme DefaultColorScheme = new ColorScheme("default");

        /// <summary>
        /// Name of the color scheme for all the dialogs
        /// </summary>
        public static String DialogSchemeName = "Dialog";

        /// <summary>
        ///  Name of the color  scheme for a disabled contextual menu icon UI element
        /// </summary>
        public static String DisabledContextMenuIconSchemeName = "DisabledContextMenuIconButton";

        /// <summary>
        /// Name of the color  scheme for a disabled contextual menu text UI element
        /// </summary>
        public static string DisabledContextMenuTextSchemeName = "DisabledContextMenuTextButton";

        /// <summary>
        /// Name of the color scheme for all the disabled buttons in the scanners
        /// </summary>
        public static String DisabledScannerButtonSchemeName = "DisabledScannerButton";

        /// <summary>
        /// Name of color scheme for all the scanners
        /// </summary>
        public static String ScannerSchemeName = "Scanner";

        /// <summary>
        /// Name of the color scheme for the edit window in the talk window
        /// </summary>
        public static string TalkWindowSchemeName = "TalkWindow";

        /// <summary>
        /// Name of the color scheme for the Predicted word list UI element
        /// </summary>
        public static String WordListItemSchemeName = "WordListItemButton";

        /// <summary>
        /// Maps the name of a color scheme to the color scheme object
        /// </summary>
        private readonly Dictionary<String, ColorScheme> _colorsTable;

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Initializes the ColorSchemes class
        /// </summary>
        public ColorSchemes()
        {
            _colorsTable = new Dictionary<String, ColorScheme>();
        }

        /// <summary>
        /// Parses the xml node and creates a color scheme object form it. The
        /// theme dir path contains the assets for the color scheme
        /// </summary>
        /// <param name="node">the xml node</param>
        /// <param name="themeDir">path to the assets</param>
        /// <returns>Color scheme collection</returns>
        public static ColorSchemes Create(XmlNode node, String themeDir)
        {
            var colorSchemes = new ColorSchemes();
            colorSchemes.loadAndAddColorScheme(node, themeDir);

            return colorSchemes;
        }

        /// <summary>
        /// Disposes resoureces
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // Prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Does the specified scheme exist?
        /// </summary>
        /// <param name="scheme">Name of the scheme</param>
        /// <returns>true if it does</returns>
        public bool Exists(String scheme)
        {
            return _colorsTable.ContainsKey(scheme.ToLower().Trim());
        }

        /// <summary>
        /// Returns the color scheme object the maps to the specified name
        /// </summary>
        /// <param name="scheme">name of the color scheme</param>
        /// <returns>the color scheme object</returns>
        public ColorScheme GetColorScheme(String scheme)
        {
            var retVal = DefaultColorScheme;

            var schemeName = scheme.ToLower();
            if (String.IsNullOrEmpty(schemeName))
            {
                return retVal;
            }

            _colorsTable.TryGetValue(schemeName, out retVal);
            return retVal ?? DefaultColorScheme;
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
                if (disposing)
                {
                    foreach (ColorScheme colorScheme in _colorsTable.Values)
                    {
                        colorScheme.Dispose();
                    }

                    _colorsTable.Clear();
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        /// Parses the xml node, creates a color scheme object and
        /// adds it to the collection.
        /// </summary>
        /// <param name="node">xml node to parse</param>
        /// <param name="themeDir">directory where assets are located</param>
        private void loadAndAddColorScheme(XmlNode node, String themeDir)
        {
            var colorSchemeNodes = node.SelectNodes("ColorScheme");

            if (colorSchemeNodes == null)
            {
                return;
            }

            // load each scheme from the config file
            foreach (XmlNode colorSchemeNode in colorSchemeNodes)
            {
                var colorScheme = ColorScheme.Create(colorSchemeNode, themeDir);
                var name = colorScheme.Name.ToLower();
                if (!_colorsTable.ContainsKey(name))
                {
                    _colorsTable.Add(name, colorScheme);
                }
            }
        }
    }
}