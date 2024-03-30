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

    [DescriptorAttribute("E4B7E06F-B1F3-48EC-AF5A-557AB2A809C8",
                "ConfirmBoxCalibrationHelp",
                "Application window used to display a help for calibration")]
    public partial class ConfirmBoxCalibrationHelp : Form
    {
        /// <summary>
        /// Result is the state of the check box
        /// </summary>
        public bool Result;

        /// <summary>
        /// Confirm Box with multiple results
        /// Results: Yes - No - Abort
        /// </summary>
        public ConfirmBoxCalibrationHelp()
        {
            InitializeComponent(); 
            Load += ConfirmBox_Load;
        }

        public static bool ShowDialogHelp(Form parent = null, bool setTopMost = false)
        {
            var confirmBox = new ConfirmBoxCalibrationHelp();
            if (parent != null && setTopMost)
            {
                parent.TopMost = false;
                confirmBox.TopMost = true;
            }
            if(parent != null)
            {
                confirmBox.StartPosition = FormStartPosition.Manual;
                // Calculate the position relative to the main form
                int x = parent.Left + (parent.Width - confirmBox.Width) / 2; // Center horizontally
                int y = parent.Bottom - confirmBox.Height - 50; // Align to the bottom of the main form
                // Set the location of the smaller form
                confirmBox.Location = new System.Drawing.Point(x, y);
            }else
                confirmBox.CenterToScreen();
            confirmBox.BringToFront();
            confirmBox.ShowDialog(parent);
            bool retVal = confirmBox.Result;
            if (parent != null && setTopMost)
            {
                //parent.TopMost = true;
                confirmBox.TopMost = false;
            }
            confirmBox.Dispose();
            return retVal;
        }

        private void buttonOp3_Click(object sender, EventArgs e)
        {
            Result = checkBoxDontShowAgain.Checked;
            try { webBrowserCalibrationHelp.Dispose();} catch (Exception) { }
            
            Close();
        }
        private void ConfirmBox_Load(object sender, EventArgs e)
        {
            WebBrowserControlInit();
        }
        /// <summary>
        /// Initialize the UI elements
        /// </summary>
        private void WebBrowserControlInit()
        {
            string htmlContent = "<!DOCTYPE html><html><head></head><body style=\"background-color:#232433;\">" +
                "<ol style=\"font-family:'Montserrat Medium'; font-size:18px; color:white; text-align: left;\">" +
                "<li>Start by selecting a <span style=\"font-family: 'Montserrat Black'; font-size:25px\">mode</span>.</li>" +
                "<li>Click on <span style=\"font-family: 'Montserrat Black'; font-size:25px\">calibrate</span> that corresponds to the <span style=\"font-family: 'Montserrat Black'; font-size:25px\">mode</span> that you have selected. if you already have a <span style=\"font-family: 'Montserrat Black'; font-size:25px\">score</span> that you are happy with it's not necessary to recalibrate.</li>" +
                "<li>Optionally, you can adjust the <span style=\"font-family: 'Montserrat Black'; font-size:25px\">parameters</span> to help you increase your score.</li>" +
                "<li>Complete at least three modes.</li>" +
                "</ol></body></html>";
            webBrowserCalibrationHelp.DocumentText = htmlContent;
        }
    }
}