////////////////////////////////////////////////////////////////////////////
// <copyright file="Theme.cs" company="Intel Corporation">
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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml;
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
    /// Contains all the attribtues for a skin. This includes
    /// the color schemes for all the various screen elements.
    /// </summary>
    public class Theme : IDisposable
    {
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
        /// Class factory to create a Theme object with the specified name. skinDir
        /// directory contains all the assets for the Theme. Skinfile is the xml
        /// file that contains references to all the theme assets.
        /// </summary>
        /// <param name="themeName">Name of the theme</param>
        /// <param name="skinDir">directory where theme assets are located</param>
        /// <param name="themeFile">name of the theme config file</param>
        /// <returns></returns>
        public static Theme Create(String themeName, String skinDir, String themeFile)
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
                var colorSchemesNode = doc.SelectSingleNode("/ACAT/Skin/ColorSchemes");
                if (colorSchemesNode != null)
                {
                    theme = new Theme(themeName) { Colors = ColorSchemes.Create(colorSchemesNode, skinDir) };
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