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
    /// Contains style attributes for a scanner panel
    /// </summary>
    public partial class ScannerPanel : Panel
    {
        public ScannerPanel()
        {
            InitializeComponent();
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.Opaque, false);
            SetStyle(ControlStyles.ResizeRedraw, true);
            BackColor = Color.Transparent;
            Dock = DockStyle.Fill;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_EX_TRANSPARENT = 0x20;
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WS_EX_TRANSPARENT;
                return cp;
            }
        }
    }
}
