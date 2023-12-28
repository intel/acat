////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ACAT.Lib.Extension
{
    /// <summary>
    /// A single button message box with an optional checkbox for
    /// "Don't show this again"
    /// </summary>
    public partial class DisclaimerDialog : Form
    {
        public DisclaimerDialog()
        {
            InitializeComponent();
            Load += DisclaimerDialog_Load;
        }

        public int FontSize { get; set; }

        public bool CheckBoxChecked { get; set; }

        public bool DisplayCheckBox { get; set; }

        public String DisclaimerText { get; set; }

        public static bool ShowDialog(String disclaimer, Form parent = null, bool displayCheckBox = false)
        {
            var disclaimerDialog = new DisclaimerDialog
            {
                DisclaimerText = disclaimer,
                DisplayCheckBox = displayCheckBox
            };

            disclaimerDialog.ShowDialog(parent);

            disclaimerDialog.CheckBoxChecked = disclaimerDialog.checkBoxDontShowThisMessage.Checked;

            bool retVal = disclaimerDialog.CheckBoxChecked;

            disclaimerDialog.Dispose();

            return retVal;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            CheckBoxChecked = checkBoxDontShowThisMessage.Checked;
            Close();
        }

        private void DisclaimerDialog_Load(object sender, EventArgs e)
        {
            CenterToScreen();
            richTextBoxDisclaimer.Text = DisclaimerText;
            if (FontSize != 0)
            {
                richTextBoxDisclaimer.Font = new Font(richTextBoxDisclaimer.Font.Name, FontSize);
            }

            checkBoxDontShowThisMessage.Visible = DisplayCheckBox;
        }
    }
}