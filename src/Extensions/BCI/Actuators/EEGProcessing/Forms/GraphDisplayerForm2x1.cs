////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// GraphDisplayErForm2x1.cs
//
// Form to display 2 charts, top and bottom
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;
using System;
using System.Windows.Forms;

namespace ACAT.Extensions.BCI.Actuators.EEG.EEGProcessing
{
    public partial class GraphDisplayerForm2x1 : Form
    {
        public GraphDisplayerForm2x1()
        {
            InitializeComponent();
            Load += GraphDisplayerForm2x1_Load;
            Shown += GraphDisplayerForm2x1_Shown;
        }

        private void GraphDisplayerForm2x1_Shown(object sender, EventArgs e)
        {
            ScannerFocus.SetFocus(this);
        }

        private void GraphDisplayerForm2x1_Load(object sender, EventArgs e)
        {
            CenterToScreen();
            TopMost = true;
        }
    }
}