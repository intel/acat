////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// CustomToolTip.cs
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ACAT.Extensions.BCI.Common.BCIInterfaceUtilities
{

    [DescriptorAttribute("3BC92A59-0B89-475D-8E2E-636CA9482740",
            "CustomToolTip",
            "Application window used to display a Custom Tooltip")]
    public partial class CustomToolTip : Form
    {
        public CustomToolTip()
        {
            InitializeComponent();
            ShowInTaskbar = false;
            TopMost = true;
            StartPosition = FormStartPosition.Manual;
            //Closing += OnClosing;
        }
        public void CloseToolTip()
        {
            Close();
            this.Dispose();
        }

        public void HideToolTip()
        {
            Hide();
        }

        public void ShowToolTip(string text, Control control, int horizontalDistance, int verticalDistance)
        {
            labelTooltip.Text = text;
            Point screenLocation = control.PointToScreen(new Point(0, 0));
            int x = screenLocation.X + horizontalDistance;
            int y = screenLocation.Y + control.Height + verticalDistance;
            Location = new Point(x, y);
            BringToFront(); 
            Show();
        }
        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            Close();
        }
    }
}
