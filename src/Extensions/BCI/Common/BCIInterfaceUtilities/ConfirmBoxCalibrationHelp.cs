////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Extensions.BCI.Common.BCIControl;
using ACAT.Core.Utility;
using ACATResources;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ACAT.Extensions.BCI.Common.BCIInterfaceUtilities
{
    /// <summary>
    /// A message box with three button options
    /// </summary>

    [Descriptor("E4B7E06F-B1F3-48EC-AF5A-557AB2A809C8",
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
                confirmBox.Location = new Point(x, y);
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
                "<li>"+
                StringResources.Startbyselectinga+
                " <span style=\"font-family: 'Montserrat Black'; font-size:25px\">mode</span>.</li><li>"+
                StringResources.ClickOn+
                "<span style=\"font-family: 'Montserrat Black'; font-size:25px\">calibrate</span>"+
                StringResources.thatcorrespondstothe+
                "<span style=\"font-family: 'Montserrat Black'; font-size:25px\">mode</span>"+
                StringResources.thatyouhaveselected+
                " <span style=\"font-family: 'Montserrat Black'; font-size:25px\">score</span> "+
                StringResources.thatyouarehappywith+
                "</li><li>"+
                StringResources.Optionallyyoucanadjustthe+
                " <span style=\"font-family: 'Montserrat Black'; font-size:25px\">parameters</span> " +
                StringResources.tohelpyouincreaseyourscore+
                "</li><li>"+
                StringResources.Completeatleastthreemodes
                +"</li></ol></body></html>";
            webBrowserCalibrationHelp.DocumentText = htmlContent;
        }
    }
}