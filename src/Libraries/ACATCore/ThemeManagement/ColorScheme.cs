////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;
using System;
using System.Drawing;
using System.IO;
using System.Xml;

namespace ACAT.Lib.Core.ThemeManagement
{
    /// <summary>
    /// Represents a color scheme.  Includes properties
    /// such as bg color, fg color, highlight color, images to use
    /// for background and foreground
    /// </summary>
    public class ColorScheme : IDisposable
    {
        /// <summary>
        /// Default bg color to use if not defined in the config file
        /// </summary>
        private readonly Color _defaultBackgroundColor = Color.White;

        /// <summary>
        /// Default fg color to use if not defined in the config file
        /// </summary>
        private readonly Color _defaultForegroundColor = Color.SkyBlue;

        /// <summary>
        /// Default highlight bg color to use if not defined in the config file
        /// </summary>
        private readonly Color _defaultHighlightBackground = Color.SkyBlue;

        /// <summary>
        /// Default highlight fg color to use if not defined in the config file
        /// </summary>
        private readonly Color _defaultHighlightForeground = Color.White;

        /// <summary>
        /// Default bg color to use for selected element if not defined in the config file
        /// </summary>
        private readonly Color _defaultSelectedBackground = Color.Black;

        /// <summary>
        /// Default bg color to use for selected element if not defined in the config file
        /// </summary>
        private readonly Color _defaultSelectedBackground2 = Color.Red;

        /// <summary>
        /// Default fg color to use for selected element if not defined in the config file
        /// </summary>
        private readonly Color _defaultSelectedForeground = Color.White;

        /// <summary>
        /// Default fg color to use for selected element if not defined in the config file
        /// </summary>
        private readonly Color _defaultSelectedForeground2 = Color.White;

        /// <summary>
        /// Background color of an unselected, unhighlighed element
        /// </summary>
        private Color _backgroundColor;

        /// <summary>
        /// Background image to use in the normal state
        /// </summary>
        private Image _backgroundImage;

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Foreground color of an unselected, unhighlighed element
        /// </summary>
        private Color _foregroundColor;

        /// <summary>
        /// Background color of a highlighed element
        /// </summary>
        private Color _highlightBackground;

        /// <summary>
        /// Background image to use in the highlighted state
        /// </summary>
        private Image _highlightBackgroundImage;

        /// <summary>
        /// Background color of a selected element
        /// </summary>
        private Color _highlightSelectedBackground;

        /// <summary>
        /// Background color of a selected element
        /// </summary>
        private Color _highlightSelectedBackground2;

        /// <summary>
        /// Background image to use in the selected state
        /// </summary>
        private Image _highlightSelectedBackgroundImage;

        /// <summary>
        /// Foreground color of a selected element
        /// </summary>
        private Color _highlightSelectedForeground;

        /// <summary>
        /// Foreground color of a selected element
        /// </summary>
        private Color _highlightSelectedForeground2;

        /// <summary>
        /// Foreground color of a highlighed element
        /// </summary>
        private Color _hightlightForeground;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="name">Name of the color scheme</param>
        public ColorScheme(String name)
        {
            Name = name;

            _backgroundColor = _defaultBackgroundColor;
            _foregroundColor = _defaultForegroundColor;
            _highlightBackground = _defaultHighlightBackground;
            _hightlightForeground = _defaultHighlightForeground;
            _highlightSelectedBackground = _defaultSelectedBackground;
            _highlightSelectedForeground = _defaultSelectedForeground;
            _highlightSelectedBackground2 = _defaultSelectedBackground2;
            _highlightSelectedForeground2 = _defaultSelectedForeground2;
            _backgroundImage = null;
            _highlightBackgroundImage = null;
            _highlightSelectedBackgroundImage = null;
        }

        /// <summary>
        /// Gets or sets the normal background color
        /// </summary>
        public Color Background
        {
            get
            {
                return _backgroundColor;
            }

            set
            {
                _backgroundColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the background image for the normal state
        /// </summary>
        public Image BackgroundImage
        {
            get
            {
                return _backgroundImage;
            }

            set
            {
                _backgroundImage = value;
            }
        }

        /// <summary>
        /// Gets or sets the normal foreground color
        /// </summary>
        public Color Foreground
        {
            get
            {
                return _foregroundColor;
            }

            set
            {
                _foregroundColor = value;
            }
        }

        /// <summary>
        /// Gets or sets background color for highlighted state
        /// </summary>
        public Color HighlightBackground
        {
            get
            {
                return _highlightBackground;
            }

            set
            {
                _highlightBackground = value;
            }
        }

        /// <summary>
        /// Gets or sets background image for the highlighted state
        /// </summary>
        public Image HighlightBackgroundImage
        {
            get
            {
                return _highlightBackgroundImage;
            }

            set
            {
                _highlightBackgroundImage = value;
            }
        }

        /// <summary>
        /// Gets or sets fg color for the highlighted state
        /// </summary>
        public Color HighlightForeground
        {
            get
            {
                return _hightlightForeground;
            }

            set
            {
                _hightlightForeground = value;
            }
        }

        /// <summary>
        /// Gets or sets bg color for  the selected state
        /// </summary>
        public Color HighlightSelectedBackground
        {
            get
            {
                return _highlightSelectedBackground;
            }

            set
            {
                _highlightSelectedBackground = value;
            }
        }

        /// <summary>
        /// Gets or sets bg color for  the selected state
        /// </summary>
        public Color HighlightSelectedBackground2
        {
            get
            {
                return _highlightSelectedBackground2;
            }

            set
            {
                _highlightSelectedBackground2 = value;
            }
        }

        /// <summary>
        /// Gets or sets bg image for the selected state
        /// </summary>
        public Image HighlightSelectedBackgroundImage
        {
            get
            {
                return _highlightSelectedBackgroundImage;
            }

            set
            {
                _highlightSelectedBackgroundImage = value;
            }
        }

        /// <summary>
        /// Gets or sets  fg color of selected element
        /// </summary>
        public Color HighlightSelectedForeground
        {
            get
            {
                return _highlightSelectedForeground;
            }

            set
            {
                _highlightSelectedForeground = value;
            }
        }

        /// <summary>
        /// Gets or sets  fg color of selected element
        /// </summary>
        public Color HighlightSelectedForeground2
        {
            get
            {
                return _highlightSelectedForeground2;
            }

            set
            {
                _highlightSelectedForeground2 = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the color scheme
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Class factory to create a ColorScheme object.  Parses the xml node,
        /// reads all the attributes and sets the class attributes
        /// </summary>
        /// <param name="node">The xml node that contains the scheme attributes</param>
        /// <param name="imageDir">Directory where the the image files are located</param>
        /// <returns></returns>
        public static ColorScheme Create(XmlNode node, String imageDir)
        {
            String scheme = XmlUtils.GetXMLAttrString(node, "name");

            var colorScheme = new ColorScheme(scheme);

            String color = XmlUtils.GetXMLAttrString(node, "background");
            colorScheme.SetBackground(color);

            String imageFileName = XmlUtils.GetXMLAttrString(node, "backgroundImage");
            colorScheme.BackgroundImage = loadImage(Path.Combine(imageDir, imageFileName));

            color = XmlUtils.GetXMLAttrString(node, "foreground");
            colorScheme.SetForeground(color);

            color = XmlUtils.GetXMLAttrString(node, "highlightBackground");
            colorScheme.SetHighlightBackground(color);

            imageFileName = XmlUtils.GetXMLAttrString(node, "highlightBackgroundImage");
            colorScheme.HighlightBackgroundImage = loadImage(Path.Combine(imageDir, imageFileName));

            color = XmlUtils.GetXMLAttrString(node, "highlightForeground");
            colorScheme.SetHighlightForeground(color);

            color = XmlUtils.GetXMLAttrString(node, "highlightSelectedBackground");
            colorScheme.SetHighlightSelectedBackground(color);

            imageFileName = XmlUtils.GetXMLAttrString(node, "highlightSelectedBackgroundImage");
            colorScheme.HighlightSelectedBackgroundImage = loadImage(Path.Combine(imageDir, imageFileName));

            color = XmlUtils.GetXMLAttrString(node, "highlightSelectedForeground");
            colorScheme.SetHighlightSelectedForeground(color);

            return colorScheme;
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
        /// Parses the string representation of the background
        /// color and sets it. The string represenation should
        /// be one of the Enum Color types or in the HTML color
        /// format #NNNNNN.    If the string is invalid, sets the
        /// default color
        /// </summary>
        /// <param name="strColor">String representation of color</param>
        /// <returns>true on success, false if invalid color</returns>
        public bool SetBackground(String strColor)
        {
            var color = _defaultBackgroundColor;

            bool retVal = fromString(strColor, ref color);
            if (retVal)
            {
                _backgroundColor = color;
            }

            return retVal;
        }

        /// <summary>
        /// Parses the string representation of the foreground
        /// color and sets it. The string represenation should
        /// be one of the Enum Color types or in the HTML color
        /// format #NNNNNN.  If the string is invalid, sets the default color
        /// </summary>
        /// <param name="strColor">String representation of color</param>
        /// <returns>true on success, false if invalid color</returns>
        public bool SetForeground(String strColor)
        {
            var color = _defaultForegroundColor;

            bool retVal = fromString(strColor, ref color);
            if (retVal)
            {
                _foregroundColor = color;
            }

            return retVal;
        }

        /// <summary>
        /// Parses the string representation of the highlighed background
        /// color and sets it. The string represenation should
        /// be one of the Enum Color types or in the HTML color
        /// format #NNNNNN.  If the string is invalid, sets the default color
        /// </summary>
        /// <param name="strColor">String representation of color</param>
        /// <returns>true on success, false if invalid color</returns>
        public bool SetHighlightBackground(String strColor)
        {
            var color = _defaultHighlightBackground;

            bool retVal = fromString(strColor, ref color);
            if (retVal)
            {
                _highlightBackground = color;
            }

            return retVal;
        }

        /// <summary>
        /// Parses the string representation of the highlighted foreground
        /// color and sets it. The string represenation should
        /// be one of the Enum Color types or in the HTML color
        /// format #NNNNNN.   If the string is invalid, sets the default color
        /// </summary>
        /// <param name="strColor">String representation of color</param>
        /// <returns>true on success, false if invalid color</returns>
        public bool SetHighlightForeground(String strColor)
        {
            var color = _defaultHighlightForeground;

            bool retVal = fromString(strColor, ref color);
            if (retVal)
            {
                _hightlightForeground = color;
            }

            return retVal;
        }

        /// <summary>
        /// Parses the string representation of the selected background
        /// color and sets it. The string represenation should
        /// be one of the Enum Color types or in the HTML color
        /// format #NNNNNN.  If the string
        /// is invalid, sets the default color
        /// </summary>
        /// <param name="strColor">String representation of color</param>
        /// <returns>true on success, false if invalid color</returns>
        public bool SetHighlightSelectedBackground(String strColor)
        {
            var color = _defaultSelectedBackground;

            bool retVal = fromString(strColor, ref color);
            if (retVal)
            {
                _highlightSelectedBackground = color;
            }

            return retVal;
        }

        /// <summary>
        /// Parses the string representation of the selected foreground
        /// color and sets it. The string represenation should
        /// be one of the Enum Color types or in the HTML color
        /// format #NNNNNN.  If the string is invalid, sets the default color
        /// </summary>
        /// <param name="strColor">String representation of color</param>
        /// <returns>true on success, false if invalid color</returns>
        public bool SetHighlightSelectedForeground(String strColor)
        {
            var color = _defaultSelectedBackground;

            bool retVal = fromString(strColor, ref color);
            if (retVal)
            {
                _highlightSelectedForeground = color;
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
                    if (_backgroundImage != null)
                    {
                        _backgroundImage.Dispose();
                        _backgroundImage = null;
                    }

                    if (_highlightBackgroundImage != null)
                    {
                        _highlightBackgroundImage.Dispose();
                        _highlightBackgroundImage = null;
                    }

                    if (_highlightSelectedBackgroundImage != null)
                    {
                        _highlightSelectedBackgroundImage.Dispose();
                        _highlightSelectedBackgroundImage = null;
                    }
                }
                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        /// Loads the image specified by the path
        /// </summary>
        /// <param name="bitmapFile">path to the bitmap</param>
        /// <returns>true on success</returns>
        private static Image loadImage(String bitmapFile)
        {
            Image retVal = null;
            if (String.IsNullOrEmpty(bitmapFile))
            {
                return null;
            }

            Log.Debug("imagePath: " + bitmapFile);

            if (File.Exists(bitmapFile))
            {
                Log.Debug("File exists. Loading image");
                retVal = Image.FromFile(bitmapFile);
            }
            else
            {
                Log.Debug("Could not find bitmap file " + bitmapFile);
            }

            return retVal;
        }

        /// <summary>
        /// Converts string to a Color enum.  The String representation
        /// can either be in the html format #NNNNNN or a Color Enum
        /// </summary>
        /// <param name="color">Input color</param>
        /// <param name="retColor">Output enum color</param>
        /// <returns>true if successful, false otherwise</returns>
        private bool fromString(String color, ref Color retColor)
        {
            bool retVal = true;
            if (!String.IsNullOrEmpty(color))
            {
                try
                {
                    retColor = color[0] == '#' ? ColorTranslator.FromHtml(color) : Color.FromName(color);
                }
                catch
                {
                    retVal = false;
                }
            }
            else
            {
                retVal = false;
            }

            return retVal;
        }
    }
}