////////////////////////////////////////////////////////////////////////////
// <copyright file="TabStopScannerButton.cs" company="Intel Corporation">
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
using System.Windows.Forms;
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

namespace ACAT.Lib.Core.Widgets
{
    /// <summary>
    /// A ScannerButton that sets tab stops for the text. This is useful
    /// if you want to display fields in the button separated by tabs.
    /// </summary>
    public class TabStopScannerButton : ScannerButtonBase
    {
        /// <summary>
        /// First tab stop before the widget has been scaled
        /// </summary>
        private float _originalFirstTab;

        /// <summary>
        /// Tab stops before the widget has been scaled
        /// </summary>
        private float[] _originalTabStops;

        /// <summary>
        /// Scaled first tab stop
        /// </summary>
        private float _scaledFirstTab;

        /// <summary>
        /// Scaled remaining tab stops
        /// </summary>
        private float[] _scaledTabStops;

        /// <summary>
        /// The scale factor
        /// </summary>
        private float _scaleFactor;

        /// <summary>
        /// Text to display in the control
        /// </summary>
        private String _text;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="uiControl">the inner .NET Control for the widget</param>
        public TabStopScannerButton(Control uiControl)
            : base(uiControl)
        {
            _text = String.Empty;
        }

        /// <summary>
        /// Returns the text
        /// </summary>
        /// <returns>text</returns>
        public override string GetText()
        {
            return _text;
        }

        /// <summary>
        /// Scales the widget up/down by scalefactor
        /// </summary>
        /// <param name="newScaleFactor">the scalefactor</param>
        public override void SetScaleFactor(float newScaleFactor)
        {
            base.SetScaleFactor(newScaleFactor);
            _scaleFactor = newScaleFactor;

            UIControl.Invalidate();
        }

        /// <summary>
        /// Sets the tab stop settings
        /// </summary>
        /// <param name="firstTab">first tab stop</param>
        /// <param name="tabStops">remaining tab stops</param>
        public void SetTabStops(float firstTab, float[] tabStops)
        {
            _originalFirstTab = firstTab;
            _originalTabStops = tabStops;
            _scaledFirstTab = firstTab;
            _scaledTabStops = new float[tabStops.Length];
            for (int ii = 0; ii < _scaledTabStops.Length; ii++)
            {
                _scaledTabStops[ii] = _originalTabStops[ii];
            }

            UIControl.Invalidate();
        }

        /// <summary>
        /// Sets the text for the control
        /// </summary>
        /// <param name="text">text to set</param>
        public override void SetText(String text)
        {
            _text = text;
            UIControl.Invalidate();
        }

        /// <summary>
        /// Paint handler. Calculate the scaled tabstops based
        /// on the scale factor. Displays text. The base class
        /// scales the size of the font
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        protected override void UIControl_Paint(object sender, PaintEventArgs e)
        {
            if (isDisposing)
            {
                return;
            }

            try
            {
                if (_originalTabStops == null || _scaledTabStops == null)
                {
                    return;
                }

                _scaledFirstTab = _originalFirstTab * _scaleFactor;
                _scaledTabStops = new float[_originalTabStops.Length];

                for (int ii = 0; ii < _originalTabStops.Length; ii++)
                {
                    _scaledTabStops[ii] = _originalTabStops[ii] * _scaleFactor;
                }

                Size s = measureDisplayStringSize(e.Graphics, GetText(), UIControl.Font);

                // vertically center the text
                var rect = new Rectangle(0, (Height - s.Height) / 2, Width, Height);
                var stringFormat = new StringFormat();
                var solidBrush = new SolidBrush(UIControl.ForeColor);

                if (_scaledTabStops != null)
                {
                    stringFormat.SetTabStops(_scaledFirstTab, _scaledTabStops);
                }

                e.Graphics.DrawString(GetText(), UIControl.Font, solidBrush, rect, stringFormat);

                var pen = Pens.Transparent;
                e.Graphics.DrawRectangle(pen, rect);
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }
        }

        /// <summary>
        /// Measures the graphical dimensions of the string
        /// </summary>
        /// <param name="graphics">the graphics object</param>
        /// <param name="text">text to measure</param>
        /// <param name="font">font to use</param>
        /// <returns>graphical size</returns>
        private Size measureDisplayStringSize(Graphics graphics, string text, Font font)
        {
            SizeF s = graphics.MeasureString(text, font);
            Size size = s.ToSize();
            return size;
        }
    }
}