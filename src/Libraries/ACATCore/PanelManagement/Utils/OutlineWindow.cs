﻿////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Draws an outline (a red border) around a window to
    /// highlight it.  Creates an invisible form and draws on
    /// it.  Exposes a Draw function that draws the rectangle
    /// </summary>
    internal class OutlineWindow : IDisposable
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
        /// Color of the outline
        /// </summary>
        private readonly Color _lineColor = Color.Red;

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

            Pen pen = null;
            Graphics formGraphics = null;
            try
            {
                pen = new Pen(_lineColor, penWidth);
                formGraphics = _form.CreateGraphics();

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
            }
            catch
            {

            }
            finally
            {
                pen?.Dispose();
                formGraphics?.Dispose();
            }
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