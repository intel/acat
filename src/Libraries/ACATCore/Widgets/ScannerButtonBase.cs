////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using System.Drawing;
using System.Windows.Forms;

namespace ACAT.Lib.Core.Widgets
{
    /// <summary>
    /// The base class for all widgets that use ScannerButton as
    /// the underlying .NET Control.  Examples are buttons on the
    /// scanner, contextual menu icon widgets, contextual menu text
    /// widgets etc.
    /// </summary>
    public class ScannerButtonBase : ButtonWidgetBase
    {
        /// <summary>
        /// The Button UI control
        /// </summary>
        protected Button button;

        /// <summary>
        /// Has this been disposed off yet?
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Font to use to display the text
        /// </summary>
        private Font _font;

        /// <summary>
        /// The font family
        /// </summary>
        private FontFamily _fontFamily;

        /// <summary>
        /// Size of the font before the widget was scaled
        /// </summary>
        private float _originalFontSize;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="uiControl">the inner .NET Control for the widget</param>
        public ScannerButtonBase(Control uiControl)
            : base(uiControl)
        {
            button = uiControl as Button;
        }

        /// <summary>
        /// Scales the font size up/down depending on the scalefactor
        /// </summary>
        /// <param name="newScaleFactor">the scale factor</param>
        public override void SetScaleFactor(float newScaleFactor)
        {
            base.SetScaleFactor(newScaleFactor);

            _font = new Font(_fontFamily, _originalFontSize * newScaleFactor, _font.Style);
            UIControl.Font = _font;
        }

        /// <summary>
        ///  Sets the font and alignment of text for the button
        /// </summary>
        /// <param name="attribute">The WidgetAttribute object</param>
        public override void SetWidgetAttribute(WidgetAttribute attribute)
        {
            base.SetWidgetAttribute(attribute);

            _fontFamily = Fonts.Instance.GetFontFamily(new[]
                                                    {   widgetAttribute.FontName,
                                                        CoreGlobals.AppPreferences.FontName,
                                                        "Arial" });
            if (_fontFamily != null)
            {
                FontStyle fontStyle = FontStyle.Regular;
                if (widgetAttribute.FontBold)
                {
                    fontStyle |= FontStyle.Bold;
                }
                if (widgetAttribute.FontItalic)
                {
                    fontStyle |= FontStyle.Italic;
                }
                _font = new Font(_fontFamily, widgetAttribute.FontSize, fontStyle);
                UIControl.Font = _font;
            }

            _originalFontSize = _font.Size;

            SetText(widgetAttribute.Label);

            if (attribute.Alignment != null)
            {
                if (UIControl is Button)
                {
                    var button = UIControl as Button;
                    button.TextAlign = widgetAttribute.Alignment.Value;
                }
            }
        }

        /// <summary>
        /// Disposes resources
        /// </summary>
        /// <param name="disposing">true to dispose managed resources</param>
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                try
                {
                    Log.Debug();

                    if (disposing)
                    {
                        // release managed resources
                        unInit();
                    }

                    // Release the native unmanaged resources

                    _disposed = true;
                }
                finally
                {
                    // Call Dispose on your base class.
                    base.Dispose(disposing);
                }
            }
        }

        /// <summary>
        /// Unhighlights the widget
        /// </summary>
        /// <returns>true</returns>
        protected override bool highlightOff()
        {
            bool retVal = true;

            if (button != null)
            {
                if (BackgroundImage != null)
                {
                    button.BackgroundImage = BackgroundImage;
                    button.ForeColor = ForegroundColor;
                }
                else
                {
                    button.BackColor = Color.Transparent;
                    button.BackgroundImage = null;
                    retVal = base.highlightOff();
                }
            }
            else
            {
                retVal = base.highlightOff();
            }

            return retVal;
        }

        /// <summary>
        /// Highlights the widget.
        /// </summary>
        /// <returns>true</returns>
        protected override bool highlightOn()
        {
            bool retVal = true;
            if (button != null)
            {
                if (HighlightBackgroundImage != null)
                {
                    button.BackgroundImage = HighlightBackgroundImage;
                    button.ForeColor = HighlightForegroundColor;
                }
                else
                {
                    button.BackgroundImage = null;
                    retVal = base.highlightOn();
                }
            }
            else
            {
                retVal = base.highlightOn();
            }

            return retVal;
        }

        /// <summary>
        /// Turns selected highlight on
        /// </summary>
        /// <returns>true</returns>
        protected override bool selectedHighlightOn()
        {
            bool retVal = true;

            if (button != null)
            {
                if (HighlightSelectedBackgroundImage != null)
                {
                    button.BackgroundImage = HighlightSelectedBackgroundImage;
                    button.ForeColor = HighlightSelectedForegroundColor;
                }
                else
                {
                    button.BackgroundImage = null;
                    retVal = base.selectedHighlightOn();
                }
            }
            else
            {
                retVal = base.highlightOn();
            }

            return retVal;
        }

        /// <summary>
        /// Releases resources
        /// </summary>
        private void unInit()
        {
            if (_font != null)
            {
                _font.Dispose();
            }

            _fontFamily = null;
        }
    }
}