////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.PanelManagement.Utils;
using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement.Layout;
using Microsoft.Extensions.Logging;
using System.Drawing;
using System.Windows.Forms;

namespace ACAT.Core.Widgets
{
    /// <summary>
    /// A wrapper widget class a TextBox .NET control.  Scales
    /// the font size of the control depending on the scalefactor.
    /// </summary>
    public class TextBoxWidget : ButtonWidgetBase
    {
        private readonly ILogger<TextBoxWidget> _logger;

        /// <summary>
        /// Has this been disposed?
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Font to use
        /// </summary>
        private Font _font;

        /// <summary>
        /// the Font family
        /// </summary>
        private FontFamily _fontFamily;

        /// <summary>
        /// Font size before the widget is scaled up/down
        /// </summary>
        private float _originalFontSize;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="uiControl">the inner .NET Control for the widget</param>
        /// <param name="logger">Logger instance</param>
        public TextBoxWidget(Control uiControl, ILogger<TextBoxWidget> logger)
            : base(uiControl, logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Sets the scale factor to scale the widget up/down.
        /// Resizes the font proportionately
        /// </summary>
        /// <param name="newScaleFactor">the scalefactor</param>
        public override void SetScaleFactor(float newScaleFactor)
        {
            base.SetScaleFactor(newScaleFactor);

            _font = new Font(_fontFamily, _originalFontSize * newScaleFactor, _font.Style);
            UIControl.Font = _font;
        }

        /// <summary>
        /// Sets the button attribute.  Extracts font information from
        /// the button attribute and sets the font on the Uicontrol.
        /// </summary>
        /// <param name="attribute"></param>
        public override void SetWidgetAttribute(WidgetAttribute attribute)
        {
            base.SetWidgetAttribute(attribute);
            _fontFamily = Fonts.Instance.GetFontFamily(new[]
                                                        { widgetAttribute.FontName,
                                                            CoreGlobals.AppPreferences.FontName });

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
                _font = new Font(_fontFamily,
                                    widgetAttribute.FontSize, fontStyle);
                UIControl.Font = _font;
            }

            _originalFontSize = _font.Size;
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
                    _logger.LogTrace("");

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
        /// Releases resources
        /// </summary>
        private void unInit()
        {
            _font?.Dispose();
        }
    }
}