////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ACAT.Extensions.BCI.Common.BCIInterfaceUtilities
{
    /// <summary>
    /// A message box with three button options
    /// </summary>

    [DescriptorAttribute("EC50871E-9FD7-4DA1-A516-5E762CD19880",
                    "ConfirmBoxSingleOption",
                    "Application window used to display a single option")]
    public partial class ConfirmBoxSingleOption : Form
    {

        public bool Result;

        private Screen primaryScreen = Screen.PrimaryScreen;
        /// <summary>
        /// Confirm Box with multiple results
        /// Results: Yes - No - Abort
        /// </summary>
        public ConfirmBoxSingleOption()
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
        public String Prompt { get; set; }
        public String PromptTitle { get; set; }

        public static bool ShowDialog(String promptTitle, String prompt,
            string opc1Prompt,
            Form parent = null, bool setTopMost = false)
        {
            var confirmBox = new ConfirmBoxSingleOption();
            confirmBox.PromptTitle = promptTitle;
            confirmBox.Prompt = prompt;
            confirmBox.Op1Prompt = opc1Prompt;
            if (parent != null && setTopMost)
            {
                parent.TopMost = false;
                confirmBox.TopMost = true;
            }
            //To always display the form in the main screen
            confirmBox.StartPosition = FormStartPosition.Manual;
            confirmBox.Location = confirmBox.primaryScreen.WorkingArea.Location;
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
        private void buttonOp3_Click(object sender, EventArgs e)
        {
            Result = true;
            Close();
        }

        private void ConfirmBox_Load(object sender, EventArgs e)
        {
            CenterToScreen();
            labelPrompt.Text = Prompt;
            labelTitle.Text = PromptTitle;
            if (Op1Prompt != null)
            {
                buttonOp1.Text = Op1Prompt;
            }
            if (LabelFont != 0)
            {
                labelPrompt.Font = new Font("Montserrat", LabelFont);
            }
            if (Op1LabelFont != 0)
            {
                buttonOp1.Font = new Font("Montserrat", Op1LabelFont);
            }
        }
    }
}