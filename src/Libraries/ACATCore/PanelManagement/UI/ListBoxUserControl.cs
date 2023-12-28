////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System.Drawing;
using System.Windows.Forms;

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Custom list box with custom colors and custom
    /// item height and custom font
    /// </summary>
    public partial class ListBoxUserControl : ListBox
    {
        /// <summary>
        /// CTOR
        /// </summary>
        public ListBoxUserControl()
        {
            InitializeComponent();
            DrawItem += listBox_DrawItem;
            MeasureItem += listBox_MeasureItem;
            DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
        }

        /// <summary>
        /// Draws each list box item with the spceficed colors and fonts
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (Items.Count == 0 || e.Index < 0)
            {
                return;
            }

            Brush brush;

            //if the item state is selected them change the back color
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                e = new DrawItemEventArgs(e.Graphics, e.Font, e.Bounds, e.Index,
                                          e.State ^ DrawItemState.Selected,
                                           Color.FromArgb(0x23, 0x24, 0x33), Color.FromArgb(0xff, 0xaa, 0x00));
                brush = new SolidBrush(Color.FromArgb(0x23, 0x24, 0x33));
            }
            else
            {
                brush = (Enabled) ? Brushes.White : Brushes.DimGray;// new SolidBrush(Color.FromArgb(0xff, 0xaa, 0x00)): Brushes.DimGray;
            }

            // Draw the background of the ListBox control for each item.
            e.DrawBackground();

            SizeF size = e.Graphics.MeasureString(Items[e.Index].ToString(), e.Font);
            e.Graphics.DrawString(Items[e.Index].ToString(), e.Font, brush, e.Bounds.Left, e.Bounds.Top + (e.Bounds.Height / 2 - size.Height / 2));

            // If the ListBox has focus, draw a focus rectangle around the selected item.
            e.DrawFocusRectangle();
        }

        /// <summary>
        /// Sets the height of each list box item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            e.ItemHeight = listBox.Font.Height + 18;
        }
    }
}