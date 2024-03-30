////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// UserControlBCIErrorUsbDongle.cs
//
// User control which displays information on errors related to connecting
// to the BCI board usb dongle which streams data from the BCI board
// through bluetooth
//
////////////////////////////////////////////////////////////////////////////

using ACAT.ACATResources;
using ACAT.Lib.Core.Utility;
using System;
using System.Windows.Forms;

namespace ACAT.Extensions.BCI.Actuators.SensorUI
{
    /// <summary>
    /// User control which displays information on errors related to connecting to the BCI board
    /// usb dongle which streams data from the BCI board through bluetooth
    /// </summary>
    public partial class UserControlBCIErrorUsbDongle : UserControl
    {
        private string _htmlText = "<!DOCTYPE html>\r\n<html>\r\n  <head>\r\n  <style>\r\n" +
                                    "a:link{color: rgb(255, 170, 0);}\r\n  </style>\r\n  </head>\r\n " +
                                    "<body style=\"background-color:#232433;\">\r\n    " +
                                    "<p style=\"font-family:'Montserrat Medium'; font-size:24px; color:white; text-align: center;\">\r\n    " +
                                    "Review the <a href=$ACAT_USER_GUIDE#USBDongleError>checklist</a> for Cyton board setup, take corrective action, and then click Retry\r\n" +
                                    "</p>\r\n  </body>\r\n</html>\r\n";

        public UserControlBCIErrorUsbDongle(String stepId)
        {
            InitializeComponent();

            webBrowserTop.DocumentCompleted += WebBrowserDesc_DocumentCompleted;
            var html = _htmlText.Replace(CoreGlobals.MacroACATUserGuide, HtmlUtils.EncodeString(CoreGlobals.ACATUserGuideFileName));
            webBrowserTop.DocumentText = html;

            webBrowserBottom.DocumentCompleted += WebBrowserDesc_DocumentCompleted;
            var htmlContent = R.GetString("BCIOnboardingBottomHtmlText");
            html = htmlContent.Replace(CoreGlobals.MacroACATUserGuide, HtmlUtils.EncodeString(CoreGlobals.ACATUserGuideFileName));
            webBrowserBottom.DocumentText = html;
        }

        private void WebBrowserDesc_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowserBottom.Navigating -= WebBrowserDesc_Navigating;
            webBrowserBottom.Navigating += WebBrowserDesc_Navigating;
            webBrowserTop.Navigating -= WebBrowserDesc_Navigating;
            webBrowserTop.Navigating += WebBrowserDesc_Navigating;
        }

        private void WebBrowserDesc_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            Utils.HandleHelpNavigaion(e);
        }
    }
}