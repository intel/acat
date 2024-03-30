////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// GraphDisplayerForm1x2.cs
//
// Form to display 2 charts next to each other
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;
using System;

using System.Windows.Forms;

namespace ACAT.Extensions.BCI.Actuators.EEG.EEGProcessing
{
    public partial class GraphDisplayerForm1x2 : Form
    {
        public GraphDisplayerForm1x2()
        {
            InitializeComponent();
            Load += GraphDisplayerForm1x2_Load;
            Shown += GraphDisplayerForm1x2_Shown;
        }

        private void GraphDisplayerForm1x2_Shown(object sender, EventArgs e)
        {
            ScannerFocus.SetFocus(this);
        }

        private void GraphDisplayerForm1x2_Load(object sender, EventArgs e)
        {
            CenterToScreen();
            TopMost = true;
        }
    }
}