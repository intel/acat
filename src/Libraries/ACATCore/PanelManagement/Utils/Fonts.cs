////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;
using System;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Collection of fonts used by ACAT. ACAT uses its own font
    /// files that contain some of the icons displayed in the scanners.
    /// This class loads the fonts dynamically to obviate the
    /// need to install them.  The fonts are unloaded when the app quits.
    /// When ACAT requests for a font, this class checks both the installed
    /// fonts as well as the dynamically loaded fonts to find a match.
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
        /// Adds font specified by the file to the collection
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

        /// <summary>
        /// Walks the specified directory and locates any fonts files in there
        /// and loads them
        /// </summary>
        /// <param name="directory">Root diretory to start from</param>
        /// <param name="wildCard">matching wild cards to look for</param>
        private static void loadFontsFromDir(String directory, String wildCard)
        {
            var walker = new DirectoryWalker(directory, wildCard);
            Log.Debug("Walking dir " + directory);
            walker.Walk(new OnFileFoundDelegate(onFileFound));
        }

        /// <summary>
        /// Directory walker delegate when it finds a file.
        /// </summary>
        /// <param name="file"></param>
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
            var done = false;

            foreach (var name in fontNames)
            {
                foreach (var family in _installedFamilies)
                {
                    if (String.Compare(family.Name, name, true) == 0)
                    {
                        font = family;
                        done = true;
                        break;
                    }
                }

                if (done)
                {
                    break;
                }
            }

            return font;
        }
    }
}