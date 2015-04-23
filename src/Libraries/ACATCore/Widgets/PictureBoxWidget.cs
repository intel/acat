////////////////////////////////////////////////////////////////////////////
// <copyright file="PictureBoxWidget.cs" company="Intel Corporation">
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
    /// Represents a Control that displays a bitmap.  If the
    /// widget needs to be highlighted, draws a rectangle around it.
    /// </summary>
    public class PictureBoxWidget : PictureBoxWidgetBase
    {
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="uiControl">the inner .NET Control for the widget</param>
        public PictureBoxWidget(Control uiControl)
            : base(uiControl)
        {
            HighlightBorderWidth = 3;
            Log.Debug(uiControl.Name);
        }

        public int HighlightBorderWidth { get; set; }

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
        /// Draw the text associated with this button using the font
        /// settings. Also set the fg and bg colors depending on the
        /// state of the button - whether it's normal, highlighted,
        /// selected etc
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
                base.UIControl_Paint(sender, e);

                Pen pen = null;
                Brush brush = null;
                if (IsSelectedHighlightOn)
                {
                    brush = new SolidBrush(Color.FromArgb(255, 0, 0, 0));
                    pen = new Pen(brush, HighlightBorderWidth);
                }
                else if (IsHighlightOn)
                {
                    brush = new SolidBrush(Color.FromArgb(255, 255, 0, 0));
                    pen = new Pen(brush, HighlightBorderWidth);
                }

                if (pen != null)
                {
                    var rect = new Rectangle(1, 1, UIControl.Width - HighlightBorderWidth, UIControl.Height - HighlightBorderWidth);
                    e.Graphics.DrawRectangle(pen, rect);
                }

                if (pen != null)
                {
                    pen.Dispose();
                }

                if (brush != null)
                {
                    brush.Dispose();
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }
        }

        /// <summary>
        /// Release resources
        /// </summary>
        private void unInit()
        {
        }
    }
}