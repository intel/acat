////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ShowDialogHistoryForm.cs
//
// Displays a transcript of the dialog
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows.Forms;

namespace ConvAssistTest
{
    public partial class ShowDialogHistoryForm : Form
    {
        public String History;

        public ShowDialogHistoryForm()
        {
            InitializeComponent();

            Load += ShowDialogHistoryForm_Load;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ShowDialogHistoryForm_Load(object sender, EventArgs e)
        {
            CenterToScreen();

            textBox1.Text = History;

            Shown += ShowDialogHistoryForm_Shown;
        }

        private void ShowDialogHistoryForm_Shown(object sender, EventArgs e)
        {
            buttonClose.Focus();
        }
    }
}