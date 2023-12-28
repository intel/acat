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
    /// A simple Yes/No message box 
    /// </summary>
    public partial class ConfirmBox : Form
    {
        public bool Result;

        public ConfirmBox()
        {
            InitializeComponent();
            Load += ConfirmBox_Load;
        }

        public int LabelFont { get; set; }
        public int NoLabelFont { get; set; }
        public String NoPrompt { get; set; }
        public String Prompt { get; set; }
        public int YesLabelFont { get; set; }

        /// <summary>
        /// If Custom labels will be used is necessary to define each element
        /// If not then the regular Yes/no window will Show
        /// </summary>
        public String YesPrompt { get; set; }

        public static bool ShowDialog(String prompt, Form parent = null, bool setTopMost = false)
        {
            return ShowDialog(prompt, "Yes", parent, setTopMost);
        }

        public static bool ShowDialog(String prompt, String yesButtonText = "Yes", Form parent = null, bool setTopMost = false)
        {
            var confirmBox = new ConfirmBox();
            confirmBox.Prompt = prompt;
            if (parent != null && setTopMost)
            {
                parent.TopMost = false;
                confirmBox.TopMost = true;
            }

            if (!String.IsNullOrEmpty(yesButtonText))
            {
                confirmBox.buttonYes.Text = yesButtonText;
            }

            confirmBox.ShowDialog(parent);
            bool retVal = confirmBox.Result;
            if (parent != null && setTopMost)
            {
                parent.TopMost = true;
                confirmBox.TopMost = false;
            }

            confirmBox.Dispose();

            return retVal;
        }

        private void buttonNo_Click(object sender, EventArgs e)
        {
            Result = false;
            Close();
        }

        private void buttonYes_Click(object sender, EventArgs e)
        {
            Result = true;
            Close();
        }

        private void ConfirmBox_Load(object sender, EventArgs e)
        {
            CenterToScreen();
            labelPrompt.Text = Prompt;
            if (YesPrompt != null && NoPrompt != null)
            {
                buttonYes.Text = YesPrompt;
                buttonNo.Text = NoPrompt;
            }
            if (LabelFont != 0)
            {
                labelPrompt.Font = new Font("Montserrat", LabelFont);
            }
            if (YesLabelFont != 0)
            {
                buttonYes.Font = new Font("Montserrat", YesLabelFont);
            }
            if (NoLabelFont != 0)
            {
                buttonNo.Font = new Font("Montserrat", NoLabelFont);
            }
        }
    }
}