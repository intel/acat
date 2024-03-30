////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// TimedGraphDisplayerForm1x2.cs
//
// Form to display 2 charts, automatically closing it after certain time elapses
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows.Forms;

namespace ACAT.Extensions.BCI.Actuators.EEG.EEGProcessing
{
    public partial class TimedGraphDisplayerForm1x2 : Form
    {
        private readonly Timer formCloser = new Timer();

        public TimedGraphDisplayerForm1x2()
        {
            //InitializeComponent();
        }

        /// <summary>
        /// Load event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimedGraphDisplayerForm1x2_Load(object sender, EventArgs e)
        {
            // Raise Event to close the form after 3 seconds
            formCloser.Interval = 3000; //3 seconds
            formCloser.Enabled = true;
            formCloser.Tick += new EventHandler(FormClose_Tick);
        }

        /// <summary>
        /// Close the form when the event is called
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormClose_Tick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}