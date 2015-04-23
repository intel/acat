////////////////////////////////////////////////////////////////////////////
// <copyright file="OutlineWindow.cs" company="Intel Corporation">
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

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Draws an outline (a red border) around a window to
    /// highlight it.  Creates an invisible form and draws on
    /// it.  Exposes a Draw function that draws the rectangle
    /// </summary>
    public class OutlineWindow : IDisposable
    {
        /// <summary>
        /// Width of the pen to draw the border
        /// </summary>
        private const int PenWidth = 3;

        /// <summary>
        /// The invisible form
        /// </summary>
        private readonly Form _form;

        /// <summary>
        /// Disposed yet?
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="parent">Parent form</param>
        public OutlineWindow(Form parent)
        {
            _form = new Form();
            initForm(_form);
            _form.Show(parent);
        }

        /// <summary>
        /// Disposes resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // Prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Draws a rectangle on the invisible form.  rectangle
        /// should be the rectangle that bounds the window of
        /// interest
        /// </summary>
        /// <param name="rectangle">Rectangle to draw</param>
        /// <param name="penWidth">width of the pen</param>
        public void Draw(System.Windows.Rect rectangle, int penWidth = 0)
        {
            Log.Debug(rectangle.ToString());

            _form.Invalidate();
            _form.Refresh();

            if (rectangle.Width == 0.0f && rectangle.Height == 0.0f)
            {
                return;
            }

            if (penWidth == 0)
            {
                penWidth = PenWidth;
            }

            var pen = new Pen(Color.Red, penWidth);
            var formGraphics = _form.CreateGraphics();

            var x = (float)rectangle.X;
            if (x < penWidth)
            {
                x = penWidth;
            }

            var y = (float)rectangle.Y;
            if (y < penWidth)
            {
                y = penWidth;
            }

            if (rectangle.Right > (Screen.PrimaryScreen.WorkingArea.Width - penWidth))
            {
                rectangle.Width = rectangle.Width - (rectangle.Right - Screen.PrimaryScreen.WorkingArea.Width) - 3 * penWidth;
            }

            if (rectangle.Bottom > Screen.PrimaryScreen.WorkingArea.Height - penWidth)
            {
                rectangle.Height = rectangle.Height - (rectangle.Bottom - Screen.PrimaryScreen.WorkingArea.Height) - penWidth;
            }

            var width = (rectangle.X > 0) ? (float)rectangle.Width : (float)rectangle.Width + (float)rectangle.X;
            var height = (rectangle.Y > 0) ? (float)rectangle.Height : (float)rectangle.Height + (float)rectangle.Y;

            Log.Debug("Draw rectangle " + x + " " + y + " " + width + " " + height);

            formGraphics.DrawRectangle(pen, x, y, width, height);

            pen.Dispose();
            formGraphics.Dispose();
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
                    // dispose all managed resources.
                    _form.Close();
                    _form.Dispose();
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        /// Initialize the invisible form
        /// </summary>
        /// <param name="form">form object</param>
        private void initForm(Form form)
        {
            form.BackColor = Color.Magenta; ;
            form.TransparencyKey = Color.Magenta;

            form.Opacity = 0.9;
            form.ShowInTaskbar = false;
            form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            form.WindowState = FormWindowState.Maximized;

            Log.Debug("Form width: " + form.Width + "Form height: " + form.Height);
            form.TopMost = true;
            int boundWidth = Screen.PrimaryScreen.Bounds.Width;
            int boundHeight = Screen.PrimaryScreen.Bounds.Height;
            Log.Debug("boundWidth=" + boundWidth + " boundHeight=" + boundHeight);

            form.Location = new Point(0, 0);
        }
    }
}