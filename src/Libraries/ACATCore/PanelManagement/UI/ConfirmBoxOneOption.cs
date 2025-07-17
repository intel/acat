////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Utility;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ACAT.Core.PanelManagement
{
    /// <summary>
    /// A message box with one button option
    /// </summary>

    [ClassDescriptor("592656B9-0534-480A-A27E-4BFF4D0C6742",
                "ConfirmBoxOneOption",
                "Application window used to display a three options")]
    public partial class ConfirmBoxOneOption : Form
    {
        public DialogResult Result;

        /// <summary>
        /// Confirm Box with multiple results
        /// Results: Yes - No - Abort
        /// </summary>
        public ConfirmBoxOneOption()
        {
            InitializeComponent();
            Load += ConfirmBox_Load;
        }

        public int LabelFont { get; set; }

        /// <summary>
        /// If Custom labels will be used is necessary to define each element
        /// If not then the regular Yes/no window will Show
        /// </summary>
        public int DecisionPromptFont { get; set; }

        public String DecisionPrompt { get; set; }
        public String Prompt { get; set; }
        public String PromptTitle { get; set; }

        public static bool ShowDialog(String promptTitle, String prompt,
            string decisionPrompt, Form parent = null, bool setTopMost = false)
        {
            var confirmBox = new ConfirmBoxOneOption
            {
                PromptTitle = promptTitle,
                Prompt = prompt
            };
            if (parent != null && setTopMost)
            {
                parent.TopMost = false;
                confirmBox.TopMost = true;
            }
            confirmBox.ShowDialog(parent);

            if (parent != null && setTopMost)
            {
                parent.TopMost = true;
                confirmBox.TopMost = false;
            }
            confirmBox.Dispose();
            return true;
        }

        private void buttonDecision_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ConfirmBox_Load(object sender, EventArgs e)
        {
            CenterToScreen();
            labelPrompt.Text = Prompt;
            labelTitle.Text = PromptTitle;
            if (LabelFont != 0)
            {
                labelPrompt.Font = new Font("Montserrat", LabelFont);
            }
            if (DecisionPromptFont != 0)
            {
                buttonDecision.Font = new Font("Montserrat", DecisionPromptFont);
            }
        }
    }
}