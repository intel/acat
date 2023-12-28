////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ACAT.Lib.Core.WidgetManagement
{
    /// <summary>
    /// A button with rounded corners whose radius can be configured
    /// </summary>
    public partial class ScannerRoundedButtonControl : Button
    {
        private Color b_color = Color.WhiteSmoke;
        private int b_radiusBottomLeft = 25;
        private int b_radiusBottomRight = 25;
        private int b_radiusTopLeft = 25;
        private int b_radiusTopRight = 25;
        private float b_width = 3f;
        private Bitmap color;
        private Bitmap grayscale = null;

        //private Color bc_color = Color.Transparent;
        public ScannerRoundedButtonControl()
        {
            InitializeComponent();
            SetStyle(ControlStyles.Selectable, false);
            UseMnemonic = false;
            this.FlatStyle = FlatStyle.Flat;
            //this.Dock = DockStyle.Fill;
            this.EnabledChanged += ScannerButtonControl_EnabledChanged;
        }

        /// <summary>
        /// Button property category
        /// </summary>
        [Category("Border"), DisplayName("Border Color")]
        public Color BorderColor
        {
            get { return b_color; }
            set
            {
                if (b_color == value) return;
                b_color = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Button property category
        /// </summary>
        [Category("Border"), DisplayName("Border Radius Bottom Left")]
        public int BorderRadiusBottomLeft
        {
            get
            {
                //b_radius = Math.Min(Math.Min(Height, Width), b_radius);
                return b_radiusBottomLeft;
            }
            set
            {
                if (b_radiusBottomLeft == value) return;
                //b_radius = Math.Min(Math.Min(Height, Width), value);
                b_radiusBottomLeft = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Button property category
        /// </summary>
        [Category("Border"), DisplayName("Border Radius Bottom Right")]
        public int BorderRadiusBottomRight
        {
            get
            {
                //b_radius = Math.Min(Math.Min(Height, Width), b_radius);
                return b_radiusBottomRight;
            }
            set
            {
                if (b_radiusBottomRight == value) return;
                //b_radius = Math.Min(Math.Min(Height, Width), value);
                b_radiusBottomRight = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Button property category
        /// </summary>
        [Category("Border"), DisplayName("Border Radius Top Left")]
        public int BorderRadiusTopLeft
        {
            get
            {
                //b_radius = Math.Min(Math.Min(Height, Width), b_radius);
                return b_radiusTopLeft;
            }
            set
            {
                if (b_radiusTopLeft == value) return;
                //b_radius = Math.Min(Math.Min(Height, Width), value);
                b_radiusTopLeft = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Button property category
        /// </summary>
        [Category("Border"), DisplayName("Border Radius Top Right")]
        public int BorderRadiusTopRight
        {
            get
            {
                //b_radius = Math.Min(Math.Min(Height, Width), b_radius);
                return b_radiusTopRight;
            }
            set
            {
                if (b_radiusTopRight == value) return;
                //b_radius = Math.Min(Math.Min(Height, Width), value);
                b_radiusTopRight = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Button property category
        /// </summary>
        [Category("Border"), DisplayName("Border Width")]
        public float BorderWidth
        {
            get
            {
                return b_width;
            }
            set
            {
                if (b_width == value) return;
                b_width = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Method that draws the button design
        /// </summary>
        /// <param name="e">Press event</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            base.OnPaint(e);
            RectangleF Rect = new RectangleF(0, 0, this.Width, this.Height);
            using (GraphicsPath GraphPath = GetRoundPath(Rect, BorderRadiusTopLeft, BorderRadiusBottomLeft, BorderRadiusTopRight, BorderRadiusBottomRight, BorderWidth))
            {
                //GraphicsPath GraphInnerPath = GetRoundPath(Rect, BorderRadiusLeft, BorderRadiusRight, BorderWidth);
                //Pen pen = new Pen(BorderColor, BorderWidth);
                this.Region = new Region(GraphPath);
                using (Pen pen = new Pen(BorderColor, BorderWidth))
                {
                    pen.Alignment = PenAlignment.Inset;
                    e.Graphics.DrawPath(pen, GraphPath);
                }
            }
        }

        /// <summary>
        /// Method that creates the button shape design
        /// </summary>
        /// <param name="Rect">Instance of the System.Drawing.RectangleF class with the specified location and size</param>
        /// <param name="radiusTopLeft">Radius value for corner</param>
        /// <param name="radiusBottomLeft">Radius value for corner</param>
        /// <param name="radiusTopRight">Radius value for corner</param>
        /// <param name="radiusBottomRight">Radius value for corner</param>
        /// <param name="width">Width of the border around the button</param>
        /// <returns>Design of the frame to be drawn</returns>
        private GraphicsPath GetRoundPath(RectangleF Rect, int radiusTopLeft, int radiusBottomLeft, int radiusTopRight, int radiusBottomRight, float width = 0)
        {
            //Fix radius to rect left size
            radiusTopLeft = (int)Math.Max((Math.Min(radiusTopLeft, Math.Min(Rect.Width, Rect.Height)) - width), 1);
            float rlt = radiusTopLeft / 2f;

            radiusBottomLeft = (int)Math.Max((Math.Min(radiusBottomLeft, Math.Min(Rect.Width, Rect.Height)) - width), 1);
            float rlb = radiusBottomLeft / 2f;

            radiusTopRight = (int)Math.Max((Math.Min(radiusTopRight, Math.Min(Rect.Width, Rect.Height)) - width), 1);
            float rrt = radiusTopRight / 2f;

            radiusBottomRight = (int)Math.Max((Math.Min(radiusBottomRight, Math.Min(Rect.Width, Rect.Height)) - width), 1);
            float rrb = radiusBottomRight / 2f;

            float w2 = width / 2f;
            GraphicsPath GraphPath = new GraphicsPath();
            //Top-Left Arc
            GraphPath.AddArc(Rect.X + w2, Rect.Y + w2, radiusTopLeft, radiusTopLeft, 180, 90);

            //Top-Right Arc
            GraphPath.AddArc(Rect.X + Rect.Width - radiusTopRight - w2, Rect.Y + w2, radiusTopRight, radiusTopRight, 270, 90);

            //Bottom-Right Arc
            GraphPath.AddArc(Rect.X + Rect.Width - w2 - radiusBottomRight, Rect.Y + Rect.Height - w2 - radiusBottomRight, radiusBottomRight, radiusBottomRight, 0, 90);

            //Bottom-Left Arc
            GraphPath.AddArc(Rect.X + w2, Rect.Y - w2 + Rect.Height - radiusBottomLeft, radiusBottomLeft, radiusBottomLeft, 90, 90);

            //Close line ( Left)
            GraphPath.AddLine(Rect.X + w2, Rect.Y + Rect.Height - rlt - w2, Rect.X + w2, Rect.Y + rlt + w2);
            //GraphPath.CloseFigure();
            return GraphPath;
        }

        private void ScannerButtonControl_EnabledChanged(object sender, System.EventArgs e)
        {
            if (Image != null && grayscale != null)
            {
                color = new Bitmap(Image);
                Bitmap c = new Bitmap(Image);
                int x, y;
                // Loop through the images pixels to reset color.
                for (x = 0; x < c.Width; x++)
                {
                    for (y = 0; y < c.Height; y++)
                    {
                        Color pixelColor = c.GetPixel(x, y);
                        Color newColor = Color.FromArgb(pixelColor.R, 0, 0);
                        c.SetPixel(x, y, newColor); // Now greyscale
                    }
                }

                grayscale = c;   // d is grayscale version of c
            }

            if (!Enabled)
            {
                if (grayscale != null)
                {
                    this.Image = grayscale;
                }
            }
            else if (color != null)
            {
                this.Image = color;
            }
        }

        /// <summary>
        /// Button property category
        /// </summary>
        /*[Category("BackColor"), DisplayName("Back Color")]
        public override Color BackColor
        {
            get { return bc_color; }
            set
            {
                if (bc_color == value) return;
                bc_color = value;
                Invalidate();
            }
        }*/
    }
}