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
    public partial class ConfirmBoxLargeSingleOption : Form
    {
        public ConfirmBoxLargeSingleOption()
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
            var confirmBox = new ConfirmBoxLargeSingleOption();
            confirmBox.Prompt = prompt;
            confirmBox.DecisionPrompt = decisionPrompt;
            confirmBox.DisplayCheckBox = displayCheckBox;

            confirmBox.ShowDialog(parent);

            confirmBox.CheckBoxChecked = confirmBox.checkBoxDontShowThisMessage.Checked;

            bool retVal = confirmBox.CheckBoxChecked;

            confirmBox.Dispose();

            return retVal;
        }

        private void buttonYes_Decision(object sender, EventArgs e)
        {
            Close();
        }

        private void ConfirmBox_Load(object sender, EventArgs e)
        {
            CenterToScreen();

            TopMost = true;

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