////////////////////////////////////////////////////////////////////////////
// <copyright file="ScannerButtonBase.cs" company="Intel Corporation">
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

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;

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

namespace ACAT.Lib.Core.Widgets
{
    /// <summary>
    /// The base class for all widgets that use ScannerButton as
    /// the underlying .NET Control.
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
            if (button != null)
            {
                button.FlatAppearance.BorderSize = 0;
            }
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
                _font = new Font(_fontFamily, widgetAttribute.FontSize, widgetAttribute.FontBold ?
                                        FontStyle.Bold :
                                        FontStyle.Regular);
                UIControl.Font = _font;
            }

            _originalFontSize = _font.Size;

            SetText(widgetAttribute.Label);

            if (attribute.Alignment != null)
            {
                if (UIControl is Button)
                {
                    var button = (Button)UIControl;
                    button.TextAlign = widgetAttribute.Alignment.Value;
                }
            }
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
        /// Turn highlight off
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
        /// Turn highlight on
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
        /// Turn selected highlight on
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
        /// Release resources
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