////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace ACAT.Lib.Core.Widgets
{
    /// <summary>
    /// A widget that uses a Label as the Control.  Can be used
    /// to display any text
    /// </summary>
    public class LabelWidget : ButtonWidgetBase
    {
        /// <summary>
        /// Should the corners of the control be rounded?
        /// </summary>
        protected bool enableRoundCorners;

        /// <summary>
        /// Default radius of the corner
        /// </summary>
        private int _cornerRadius = 8;

        /// <summary>
        /// Has this been disposed?
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Font to use to display the text
        /// </summary>
        private Font _font;

        /// <summary>
        /// Font family of the text
        /// </summary>
        private FontFamily _fontFamily;

        /// <summary>
        /// The original (non-scaled) font size of the text
        /// </summary>
        private float _originalFontSize;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="uiControl">the inner .NET Control for the widget</param>
        public LabelWidget(Control uiControl)
            : base(uiControl)
        {
            Label label = (Label)uiControl;
            label.Paint += label_Paint;

            if (!enableRoundCorners)
            {
                _cornerRadius = 1;
            }

            uiControl.MouseDown += uiControl_MouseDown;
            uiControl.MouseUp += uiControl_MouseUp;
        }

        /// <summary>
        /// Checks if roundedcorners attribute has been specified.
        /// If so, rounds the corners of the control
        /// </summary>
        /// <param name="node">the XML node</param>
        public override void Load(XmlNode node)
        {
            base.Load(node);

            enableRoundCorners = XmlUtils.GetXMLAttrBool(node, "roundedCorners", false);
            if (!enableRoundCorners)
            {
                _cornerRadius = 0;
            }
            else
            {
                createRoundedControl(DrawBorder, _cornerRadius);
            }
        }

        /// <summary>
        /// Scales the font size up/down depending on the scale factor of the scanner
        /// </summary>
        /// <param name="newScaleFactor">the scale factor</param>
        public override void SetScaleFactor(float newScaleFactor)
        {
            base.SetScaleFactor(newScaleFactor);

            _font = new Font(_fontFamily, _originalFontSize * newScaleFactor, _font.Style);
            UIControl.Font = _font;
            setCornerRadius(newScaleFactor);
        }

        /// <summary>
        ///  Sets the WidgetAttribute for this widget.  Gets the font
        /// attribute and sets the font for the displayed text
        /// </summary>
        /// <param name="attribute">The WidgetAttribute object</param>
        public override void SetWidgetAttribute(WidgetAttribute attribute)
        {
            base.SetWidgetAttribute(attribute);
            _fontFamily = Fonts.Instance.GetFontFamily(new[]
                                                            { widgetAttribute.FontName,
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
        /// Invoked when the widget is resized
        /// </summary>
        protected override void onResize()
        {
            if (enableRoundCorners)
            {
                createRoundedControl(DrawBorder, _cornerRadius);
            }
        }

        /// <summary>
        /// Paint handler. Round the corners if indicated so.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void label_Paint(object sender, PaintEventArgs e)
        {
            if (isDisposing)
            {
                return;
            }

            try
            {
                if (DrawBorder && graphicsPath != null)
                {
                    RoundedCornerControl.DrawBorder(graphicsPath, e, Color.Black);
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }
        }

        /// <summary>
        /// Determines the radius of rounded corners depending on
        /// how big or small the widget is.
        /// </summary>
        /// <param name="newScaleFactor">scale factor</param>
        private void setCornerRadius(float newScaleFactor)
        {
            if (enableRoundCorners)
            {
                _cornerRadius = newScaleFactor < (float)0.9 ? 3 : 8;
            }
            else
            {
                _cornerRadius = 0;
            }
        }

        /// <summary>
        /// Change the color if a mouse down was detected
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arg</param>
        private void uiControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (IsMouseClickActuateOn)
            {
                setBackgroundColor(HighlightSelectedBackgroundColor);
                setForegroundColor(HighlightSelectedForegroundColor);
            }
        }

        /// <summary>
        /// Restore the color if a mouse up was detected
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arg</param>
        private void uiControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (IsMouseClickActuateOn)
            {
                setBackgroundColor(BackgroundColor);
                setForegroundColor(ForegroundColor);
            }
        }

        /// <summary>
        /// Release resources
        /// </summary>
        private void unInit()
        {
            if (UIControl != null)
            {
                UIControl.MouseDown -= uiControl_MouseDown;
                UIControl.MouseUp -= uiControl_MouseUp;
            }

            if (_font != null)
            {
                _font.Dispose();
            }

            _fontFamily = null;
        }
    }
}