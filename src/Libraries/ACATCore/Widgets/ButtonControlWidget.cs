////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.ThemeManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using System.Drawing;
using System.Windows.Forms;

namespace ACAT.Lib.Core.Widgets
{
    /// <summary>
    /// A widget that uses a Button as the UI control.  Can be used
    /// to display any text
    /// </summary>
    public class ButtonControlWidget : ButtonWidgetBase
    {
        /// <summary>
        /// Has this object been disposed off yet?
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Font to use for this widget
        /// </summary>
        private Font _font;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="uiControl">the inner .NET Control for the widget</param>
        public ButtonControlWidget(Control uiControl)
            : base(uiControl)
        {
            if (uiControl is Button)
            {
                Colors = ThemeManager.Instance.ActiveTheme.Colors.GetColorScheme(ColorSchemes.ButtonSchemeName);
                Button button = UIControl as Button;
                if (button != null)
                {
                    button.KeyPress += button_KeyPress;
                }
            }
        }

        /// <summary>
        /// Set the font specified in the attribute object and also set
        /// the text for the Button
        /// </summary>
        /// <param name="attribute">The button attribute object</param>
        public override void SetWidgetAttribute(WidgetAttribute attribute)
        {
            base.SetWidgetAttribute(attribute);

            var fontFamily = Fonts.Instance.GetFontFamily(new[]
                                            {   widgetAttribute.FontName,
                                                CoreGlobals.AppPreferences.FontName,
                                                "Arial" });

            if (fontFamily != null)
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

                _font = new Font(fontFamily,
                                widgetAttribute.FontSize,
                                fontStyle);
                UIControl.Font = _font;
            }

            SetText(widgetAttribute.Label);
        }

        /// <summary>
        /// Dispose resources
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

                    _disposed = true;

                    // Release the native unmanaged resources
                }
                finally
                {
                    // Call Dispose on your base class.
                    base.Dispose(disposing);
                }
            }
        }

        /// <summary>
        /// If a space was pressed, actuate the widget
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void button_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ')
            {
                Actuate();
            }
        }

        /// <summary>
        /// Release resources
        /// </summary>
        private void unInit()
        {
            if (_font != null)
            {
                _font.Dispose();
            }
        }
    }
}