////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Extensions.BCI.Common.BCIControl;
using ACAT.Lib.Core.Utility;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ACAT.Extensions.BCI.Common.BCIInterfaceUtilities
{
    /// <summary>
    /// A message box with three button options
    /// </summary>

    [DescriptorAttribute("592656B9-0534-480A-A27E-4BFF4D0C6742",
                "ConfirmBoxThreeOption",
                "Application window used to display a three options")]
    public partial class ConfirmBoxThreeOption : Form
    {
        public BCIMenuOptions.MainMenuOptions OptionsResult;

        public DialogResult Result;

        private bool _EnableOption3 = true;
        /// <summary>
        /// Confirm Box with multiple results
        /// Results: Yes - No - Abort
        /// </summary>
        public ConfirmBoxThreeOption()
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
        public int Op2LabelFont { get; set; }
        public String Op2Prompt { get; set; }
        public int Op3LabelFont { get; set; }
        public String Op3Prompt { get; set; }
        public String Prompt { get; set; }
        public String PromptTitle { get; set; }

        public static BCIMenuOptions.MainMenuOptions ShowDialog(String promptTitle, String prompt,
            string opc1Prompt, string opc2Prompt, string opc3Prompt, bool enableOption3 = true,
            Form parent = null, bool setTopMost = false)
        {
            var confirmBox = new ConfirmBoxThreeOption();
            confirmBox.PromptTitle = promptTitle;
            confirmBox.Prompt = prompt;
            confirmBox.Op1Prompt = opc1Prompt;
            confirmBox.Op2Prompt = opc2Prompt;
            confirmBox.Op3Prompt = opc3Prompt;
            confirmBox._EnableOption3 = enableOption3;
            if (parent != null && setTopMost)
            {
                parent.TopMost = false;
                confirmBox.TopMost = true;
            }
            confirmBox.ShowDialog(parent);
            BCIMenuOptions.MainMenuOptions retVal = confirmBox.OptionsResult;
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
            OptionsResult = BCIMenuOptions.MainMenuOptions.ExitApplication;
            Close();
        }

        private void buttonOp2_Click(object sender, EventArgs e)
        {
            OptionsResult = BCIMenuOptions.MainMenuOptions.CalibrateOrShowCalibrationModes;
            Close();
        }

        private void buttonOp3_Click(object sender, EventArgs e)
        {
            if (_EnableOption3)
            {
                OptionsResult = BCIMenuOptions.MainMenuOptions.TypingOrRecalibrate;
                Close();
            }
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
            UIInit();
        }

        /// <summary>
        /// Initialize the UI elements
        /// </summary>
        private void UIInit()
        {
            if (!_EnableOption3)
            {
                buttonOp3.ForeColor = Color.White;
                buttonOp3.BackColor = Color.FromArgb(129, 129, 129);
            }
        }
    }
}