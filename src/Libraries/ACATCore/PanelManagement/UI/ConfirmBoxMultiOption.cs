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
    /// A message box with three button options
    /// </summary>
    public partial class ConfirmBoxMultiOption : Form
    {
        public Options OptionsResult;

        public DialogResult Result;

        /// <summary>
        /// Confirm Box with multiple results
        /// Results: Yes - No - Abort
        /// </summary>
        public ConfirmBoxMultiOption()
        {
            InitializeComponent();
            Load += ConfirmBox_Load;
        }

        public enum Options
        {
            Option1,
            Option2,
            Option3,
        }

        public int LabelFont { get; set; }
        public int Op1LabelFont { get; set; }

        /// <summary>
        /// If Custom labels will be used is necessary to define each element
        /// If not then the regular Yes/no window will Show
        /// </summary>
        public String Op1Prompt { get; set; }

        public int Op2LabelFont { get; set; }
        public String Op2Prompt { get; set; }
        public int Op3LabelFont { get; set; }
        public String Op3Prompt { get; set; }
        public String Prompt { get; set; }
        public String PromptTitle { get; set; }

        public static Options ShowDialog(String prompt, String promptTitle, Form parent = null, bool setTopMost = false)
        {
            var confirmBox = new ConfirmBoxMultiOption();
            confirmBox.Prompt = prompt;
            confirmBox.PromptTitle = promptTitle;
            if (parent != null && setTopMost)
            {
                parent.TopMost = false;
                confirmBox.TopMost = true;
            }

            confirmBox.ShowDialog(parent);
            Options retVal = confirmBox.OptionsResult;
            if (parent != null && setTopMost)
            {
                parent.TopMost = true;
                confirmBox.TopMost = false;
            }

            confirmBox.Dispose();

            return retVal;
        }

        private void buttonOp1_Click(object sender, EventArgs e)
        {
            OptionsResult = Options.Option1;
            Close();
        }

        private void buttonOp2_Click(object sender, EventArgs e)
        {
            OptionsResult = Options.Option2;
            Close();
        }

        private void buttonOp3_Click(object sender, EventArgs e)
        {
            OptionsResult = Options.Option3;
            Close();
        }

        private void ConfirmBox_Load(object sender, EventArgs e)
        {
            CenterToScreen();
            labelPrompt.Text = Prompt;
            labelTitle.Text = PromptTitle;
            if (Op1Prompt != null && Op2Prompt != null && Op3Prompt != null)
            {
                buttonOp1.Text = Op1Prompt;
                buttonOp2.Text = Op2Prompt;
                buttonOp3.Text = Op3Prompt;
            }
            if (LabelFont != 0)
            {
                labelPrompt.Font = new Font("Montserrat", LabelFont);
            }
            if (Op1LabelFont != 0)
            {
                buttonOp1.Font = new Font("Montserrat", Op1LabelFont);
            }
            if (Op2LabelFont != 0)
            {
                buttonOp2.Font = new Font("Montserrat", Op2LabelFont);
            }
            if (Op3LabelFont != 0)
            {
                buttonOp3.Font = new Font("Montserrat", Op3LabelFont);
            }
        }
    }
}