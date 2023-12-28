////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System.Drawing;
using System.Windows.Forms;

namespace ACAT.Lib.Core.WidgetManagement
{
    /// <summary>
    /// Usercontrol that represents a button widget.
    /// </summary>
    public partial class ScannerButtonControl : Button
    {
        private Bitmap color;
        private Bitmap grayscale = null;

        public ScannerButtonControl()
        {
            InitializeComponent();
            SetStyle(ControlStyles.Selectable, false);
            UseMnemonic = false;

            this.EnabledChanged += ScannerButtonControl_EnabledChanged;
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
    }
}