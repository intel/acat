////////////////////////////////////////////////////////////////////////////
// <copyright file="Fonts.cs" company="Intel Corporation">
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
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
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

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Collection of fonts used by ACAT.  This obviates the
    /// need to install special fonts used by ACAT.  The fonts
    /// can be loaded at runtime and unloaded when the app quits
    /// </summary>
    public class Fonts : IDisposable
    {
        /// <summary>
        /// Singleton instance of this class
        /// </summary>
        private static readonly Fonts _instance = new Fonts();

        /// <summary>
        /// Collection of installed fonts
        /// </summary>
        private readonly InstalledFontCollection _ifc = new InstalledFontCollection();

        /// <summary>
        /// Collection of installed font families
        /// </summary>
        private readonly FontFamily[] _installedFamilies;

        /// <summary>
        /// Collection of private fonts
        /// </summary>
        private readonly PrivateFontCollection _privateFonts;

        /// <summary>
        /// Initialzies an instance of the class
        /// </summary>
        private Fonts()
        {
            _privateFonts = new PrivateFontCollection();
            _installedFamilies = _ifc.Families;
        }

        /// <summary>
        /// Gets the singleton instance of the class
        /// </summary>
        public static Fonts Instance
        {
            get { return _instance; }
        }

        public static void LoadFontsFromDir(String directory)
        {
            loadFontsFromDir(directory, "*.ttf");
            loadFontsFromDir(directory, "*.otf");
        }

        /// <summary>
        /// Add font specified by the file to the collection
        /// </summary>
        /// <param name="fontFileName">full path to the font file</param>
        /// <returns>true on success</returns>
        public bool AddFontFile(String fontFileName)
        {
            bool retVal = true;

            try
            {
                _privateFonts.AddFontFile(fontFileName);
            }
            catch (Exception ex)
            {
                Log.Debug("Could not add font file " + fontFileName + ", exception: " + ex);
                retVal = false;
            }
            return retVal;
        }

        /// <summary>
        /// Disposes off the object
        /// </summary>
        public void Dispose()
        {
            _privateFonts.Dispose();
            _ifc.Dispose();
        }

        /// <summary>
        /// Returns the font family of the specified font
        /// </summary>
        /// <param name="name">name of font</param>
        /// <returns>font family</returns>
        public FontFamily GetFontFamily(String name)
        {
            return _privateFonts.Families.FirstOrDefault(fontFamily => String.Compare(name, fontFamily.Name, true) == 0);
        }

        /// <summary>
        /// Returns a fontfamily object for the first valid font name in the array.
        /// First checks the collection. If it can't find it, it checks the
        /// fonts installed on the computer
        /// </summary>
        /// <param name="fontNames">array of font names</param>
        /// <returns>Font family object, null if it can't</returns>
        public FontFamily GetFontFamily(String[] fontNames)
        {
            return tryCollection(fontNames) ?? tryInstalledFonts(fontNames);
        }

        private static void loadFontsFromDir(String directory, String wildCard)
        {
            var walker = new DirectoryWalker(directory, wildCard);
            Log.Debug("Walking dir " + directory);
            walker.Walk(new OnFileFoundDelegate(onFileFound));
        }

        private static void onFileFound(String file)
        {
            Log.Debug("Found font file " + file);
            Instance.AddFontFile(file);
        }

        /// <summary>
        /// Returns a fontfamily object for the first valid font name in the array
        /// that is found in the private collection
        /// </summary>
        /// <param name="fontNames">list of font names</param>
        /// <returns>font family if found, null otherwise</returns>
        private FontFamily tryCollection(String[] fontNames)
        {
            return fontNames.Select(name => GetFontFamily(name)).FirstOrDefault(font => font != null);
        }

        /// <summary>
        /// Returns a font object for the first valid font name in the array
        /// that is installed on the hot computer
        /// </summary>
        /// <param name="fontNames">array of font names</param>
        /// <returns>Font object</returns>
        private FontFamily tryInstalledFonts(String[] fontNames)
        {
            FontFamily font = null;
            //var ifc = new InstalledFontCollection();

            foreach (var name in fontNames)
            {
                foreach (var family in _installedFamilies)
                {
                    if (String.Compare(family.Name, name, true) == 0)
                    {
                        font = family;
                        break;
                    }
                }
            }

            return font;
        }
    }
}