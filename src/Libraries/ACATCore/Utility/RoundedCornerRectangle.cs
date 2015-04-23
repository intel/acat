////////////////////////////////////////////////////////////////////////////
// <copyright file="RouncedCornerRectangle.cs" company="Intel Corporation">
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
using System.Drawing.Drawing2D;
using System.Windows.Forms;

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

namespace ACAT.Lib.Core.Utility
{
    /// <summary>
    /// Creates a rounded corner control.  Can be applied to
    /// a window, a control such as a label, text box, button etc.
    /// </summary>
    public class RoundedCornerControl
    {
        public enum Corners
        {
            None = 0,
            UpperLeft = 1,
            UpperRight = 2,
            LowerLeft = 4,
            LowerRight = 8,
            RoundAll = UpperLeft | UpperRight | LowerLeft | LowerRight
        }

        /// <summary>
        /// Creates a graphics path around the border with the
        /// specified origin, width and height.  Corners specifies
        /// rounded corners
        /// </summary>
        /// <param name="x">x origin</param>
        /// <param name="y">y origin</param>
        /// <param name="width">width</param>
        /// <param name="height">height</param>
        /// <param name="radius">radius of corners</param>
        /// <param name="corners">which corners to round</param>
        /// <returns></returns>
        public static GraphicsPath Create(int x, int y, int width, int height, int radius, Corners corners)
        {
            int xw = x + width;
            int yh = y + height;
            int xwr = xw - radius;
            int yhr = yh - radius;
            int xr = x + radius;
            int yr = y + radius;
            int r2 = radius * 2;
            int xwr2 = xw - r2;
            int yhr2 = yh - r2;

            GraphicsPath _graphicsPath = new GraphicsPath();

            _graphicsPath.StartFigure();

            //Upper left
            if ((Corners.UpperLeft & corners) == Corners.UpperLeft)
            {
                _graphicsPath.AddArc(x, y, r2, r2, 180, 90);
            }
            else
            {
                _graphicsPath.AddLine(x, yr, x, y);
                _graphicsPath.AddLine(x, y, xr, y);
            }

            //Upper Border
            _graphicsPath.AddLine(xr, y, xwr, y);

            //Upper Right Corner
            if ((Corners.UpperRight & corners) == Corners.UpperRight)
            {
                _graphicsPath.AddArc(xwr2, y, r2, r2, 270, 90);
            }
            else
            {
                _graphicsPath.AddLine(xwr, y, xw, y);
                _graphicsPath.AddLine(xw, y, xw, yr);
            }

            //Right border
            _graphicsPath.AddLine(xw, yr, xw, yhr);

            //Lower Right Corner
            if ((Corners.LowerRight & corners) == Corners.LowerRight)
            {
                _graphicsPath.AddArc(xwr2, yhr2, r2, r2, 0, 90);
            }
            else
            {
                _graphicsPath.AddLine(xw, yhr, xw, yh);
                _graphicsPath.AddLine(xw, yh, xwr, yh);
            }

            //Lower Border
            _graphicsPath.AddLine(xwr, yh, xr, yh);

            //Lower Left Corner
            if ((Corners.LowerLeft & corners) == Corners.LowerLeft)
            {
                _graphicsPath.AddArc(x, yhr2, r2, r2, 90, 90);
            }
            else
            {
                _graphicsPath.AddLine(xr, yh, x, yh);
                _graphicsPath.AddLine(x, yh, x, yhr);
            }

            //Left border
            _graphicsPath.AddLine(x, yhr, x, yr);

            _graphicsPath.CloseFigure();
            return _graphicsPath;
        }

        /// <summary>
        /// Creates all four rounded corners
        /// </summary>
        /// <param name="x">x origin</param>
        /// <param name="y">y origin</param>
        /// <param name="width">width</param>
        /// <param name="height">height</param>
        /// <param name="radius">rardus of corners</param>
        /// <returns>graphics path</returns>
        public static GraphicsPath Create(int x, int y, int width, int height, int radius = 5)
        {
            return Create(x, y, width, height, radius, Corners.RoundAll);
        }

        /// <summary>
        /// Creates rounded corners for the specified rectangle
        /// </summary>
        /// <param name="rectangle">the rectangle</param>
        /// <param name="radius">rardus of corners</param>
        /// <param name="corners">which corners?</param>
        /// <returns>graphics path</returns>
        public static GraphicsPath Create(Rectangle rectangle, int radius, Corners corners)
        {
            return Create(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, radius, corners);
        }

        /// <summary>
        /// Creates four rounded corners for the specified rectangle
        /// </summary>
        /// <param name="rectangle">the rectangle</param>
        /// <param name="radius">rardus of corners</param>
        /// <returns>graphics path</returns>
        public static GraphicsPath Create(Rectangle rect, int radius = 5)
        {
            return Create(rect.X, rect.Y, rect.Width, rect.Height, radius);
        }

        /// <summary>
        /// Creates a graphics path for the specified rectangle
        /// </summary>
        /// <param name="rect">rectangle</param>
        /// <returns>graphics path</returns>
        public static GraphicsPath Create(Rectangle rect)
        {
            return Create(rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Creates a rounded corner control
        /// </summary>
        /// <param name="control">the control</param>
        /// <param name="radius">radius of roundness</param>
        public static GraphicsPath CreateRoundedControl(Control control, int radius = 8)
        {
            var graphicsPath = Create(-1, -1, control.Width, control.Height, radius);
            var reg = new Region(graphicsPath);
            control.Region = reg;
            return graphicsPath;
        }

        /// <summary>
        /// Creates a rounded corner window
        /// </summary>
        /// <param name="control">the window</param>
        /// <param name="radius">radius of the roundness</param>
        /// <returns>graphics path</returns>
        public static GraphicsPath CreateRoundedForm(Control control, int radius = 15)
        {
            var graphicsPath = new GraphicsPath();
            var rect = new Rectangle(0, 0, control.ClientRectangle.Width, control.ClientRectangle.Height);
            graphicsPath.AddArc(rect.Left, rect.Top, radius, radius, 180, 90);
            graphicsPath.AddArc(rect.Right - radius, rect.Top, radius, radius, 270, 90);
            graphicsPath.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            graphicsPath.AddArc(rect.Left, rect.Bottom - radius, radius, radius, 90, 90);
            graphicsPath.CloseFigure();

            control.Region = new Region(graphicsPath);
            return graphicsPath;
        }

        /// <summary>
        /// Draws a border along the graphics path in the specified color. To be
        /// called in the onpaint routine
        /// </summary>
        /// <param name="graphicsPath">graphics path</param>
        /// <param name="e">onpaint event args </param>
        /// <param name="color">color of the vorder</param>
        public static void DrawBorder(GraphicsPath graphicsPath, PaintEventArgs e, Color color)
        {
            try
            {
                if (graphicsPath != null)
                {
                    var pen = new Pen(new SolidBrush(color)) { Width = 3 };
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
                    e.Graphics.DrawPath(pen, graphicsPath);
                }
            }
            catch
            {
            }
        }
    }
}