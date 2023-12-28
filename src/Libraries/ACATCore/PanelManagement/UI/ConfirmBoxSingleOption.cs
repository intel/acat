////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// A single button message box with an optional checkbox for
    /// "Don't show this again"
    /// </summary>
    public partial class ConfirmBoxSingleOption : Form
    {
        public ConfirmBoxSingleOption()
        {
            InitializeComponent();
            Load += ConfirmBox_Load;
        }
        public int LabelFont { get; set; }
        public bool CheckBoxChecked { get; set; }

        public String DecisionPrompt { get; set; }

        public bool DisplayCheckBox { get; set; }

        /// <summary>
        /// Custom Message Box showing a confirmation option
        /// </summary>
        public String Prompt { get; set; }

        public static bool ShowDialog(String prompt, String decisionPrompt, Form parent = null, bool displayCheckBox = false)
        {
            var confirmBoxSingleOption = new ConfirmBoxSingleOption();
            confirmBoxSingleOption.Prompt = prompt;
            confirmBoxSingleOption.DecisionPrompt = decisionPrompt;
            confirmBoxSingleOption.DisplayCheckBox = displayCheckBox;

            confirmBoxSingleOption.ShowDialog(parent);

            confirmBoxSingleOption.CheckBoxChecked = confirmBoxSingleOption.checkBoxDontShowThisMessage.Checked;

            bool retVal = confirmBoxSingleOption.CheckBoxChecked;

            confirmBoxSingleOption.Dispose();

            return retVal;
        }

        private void buttonYes_Decision(object sender, EventArgs e)
        {
            Close();
        }

        private void ConfirmBox_Load(object sender, EventArgs e)
        {
            CenterToScreen();
            labelPrompt.Text = Prompt;
            if (LabelFont != 0)
            {
                labelPrompt.Font = new Font("Montserrat", LabelFont);
            }
            buttonDecision.Text = DecisionPrompt;
            checkBoxDontShowThisMessage.Visible = DisplayCheckBox;
        }
    }
}