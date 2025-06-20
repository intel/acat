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
    /// A message box with three button options
    /// </summary>

    [Descriptor("592656B9-0534-480A-A27E-4BFF4D0C6742",
                "ConfirmBoxTwoOption",
                "Application window used to display a three options")]
    public partial class ConfirmBoxTwoOption : Form
    {
        //public BCIMenuOptions.MainMenuOptions OptionsResult;

        public bool Result { get; }

        /// <summary>
        /// Confirm Box with multiple results
        /// Results: Yes - No - Abort
        /// </summary>
        public ConfirmBoxTwoOption()
        {
            InitializeComponent();
            Load += ConfirmBox_Load;
        }

        public int LabelFont { get; set; }

        /// <summary>
        /// If Custom labels will be used is necessary to define each element
        /// If not then the regular Yes/no window will Show
        /// </summary>
        public int Op1LabelFont { get; set; }

        public String Op1Prompt { get; set; }
        public int Op3LabelFont { get; set; }
        public String Op3Prompt { get; set; }
        public String Prompt { get; set; }
        public String PromptTitle { get; set; }

        public static bool ShowDialog(String promptTitle, String prompt,
            string opc1Prompt, string opc3Prompt,
            Form parent = null, bool setTopMost = false)
        {
            var confirmBox = new ConfirmBoxTwoOption
            {
                PromptTitle = promptTitle,
                Prompt = prompt,
                Op1Prompt = opc1Prompt,
                Op3Prompt = opc3Prompt
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
            //TODO:  FIXME !!!
            return false;
        }

        private void buttonOp1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonOp3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ConfirmBox_Load(object sender, EventArgs e)
        {
            CenterToScreen();
            labelPrompt.Text = Prompt;
            labelTitle.Text = PromptTitle;
            if (Op1Prompt != null && Op3Prompt != null)
            {
                buttonOp1.Text = Op1Prompt;
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
            if (Op3LabelFont != 0)
            {
                buttonOp3.Font = new Font("Montserrat", Op3LabelFont);
            }
        }
    }
}