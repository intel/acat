////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// SampleImageForm.cs
//
// Displays a sample image during calibration for reference
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.Actuators.CameraActuator
{
    public partial class SampleImageForm : Form
    {
        public SampleImageForm()
        {
            InitializeComponent();
            Load += SampleImageForm_Load;
        }

        private void SampleImageForm_Load(object sender, EventArgs e)
        {
            TopMost = true;
        }
    }
}