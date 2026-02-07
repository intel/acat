////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.PanelManagement.Utils;
using ACAT.Core.ThemeManagement;
using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement.Layout;
using Microsoft.Extensions.Logging;
using System.Drawing;
using System.Windows.Forms;

namespace ACAT.Core.Widgets
{
    /// <summary>
    /// A widget that uses a Button as the UI control.  Can be used
    /// to display any text
    /// </summary>
    public class ButtonControlWidget : ButtonWidgetBase
    {
        private readonly ILogger<ButtonControlWidget> _logger;

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
        /// <param name="logger">Logger instance</param>
        public ButtonControlWidget(Control uiControl, ILogger<ButtonControlWidget> logger)
            : base(uiControl, logger)
        {
            _logger = logger;

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

        public ButtonControlWidget(
        Control uiControl,
        string name,
        string label,
        string command,
        string fontname,
        int fontsize,
        bool bold,
        ILogger<ButtonControlWidget> logger) : this(uiControl, logger)
        {
            _logger = logger;

            uiControl.Name = name;
            uiControl.Text = label;
            uiControl.Font = new Font(fontname, fontsize, bold ? FontStyle.Bold : FontStyle.Regular);
            //uiControl.Click += (sender, e) => WidgetManager.Instance.ExecuteCommand(command);
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
                                                CoreGlobals.AppPreferences.FontName });

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
                    _logger.LogTrace("");

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
            _font?.Dispose();
        }
    }
}