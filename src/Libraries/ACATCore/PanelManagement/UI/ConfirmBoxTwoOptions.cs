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
    /// A Yes/No message box
    /// </summary>
    public partial class ConfirmBoxTwoOptions : Form
    {
        public Options OptionsResult;

        public DialogResult Result;

        /// <summary>
        /// Confirm Box with multiple results
        /// Results: Yes - No - Abort
        /// </summary>
        public ConfirmBoxTwoOptions()
        {
            InitializeComponent();
            Load += ConfirmBox_Load;
        }

        public enum Options
        {
            Option1,
            Option2
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
        public String Prompt { get; set; }

        public static Options ShowDialog(String prompt, Form parent = null, bool setTopMost = false)
        {
            var confirmBox = new ConfirmBoxTwoOptions
            {
                Prompt = prompt
            };

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

        private void ConfirmBox_Load(object sender, EventArgs e)
        {
            CenterToScreen();
            labelPrompt.Text = Prompt;
            if (Op1Prompt != null && Op2Prompt != null)
            {
                buttonOp1.Text = Op1Prompt;
                buttonOp2.Text = Op2Prompt;
            }
            if (LabelFont != 0)
            {
                labelPrompt.Font = new Font("Montserrat Medium", LabelFont);
            }
            if (Op1LabelFont != 0)
            {
                buttonOp1.Font = new Font("Montserrat Medium", Op1LabelFont);
            }
            if (Op2LabelFont != 0)
            {
                buttonOp2.Font = new Font("Montserrat Medium", Op2LabelFont);
            }

            if (Parent != null)
            {
                CenterToParent();
            }
        }
    }
}