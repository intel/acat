////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// UserControlBCIErrorOpticalSensorDetect.cs
//
// User control which displays information on errors related to connecting
// to the optical sensor
//
////////////////////////////////////////////////////////////////////////////

using ACAT.ACATResources;
using ACAT.Lib.Core.Utility;
using System;
using System.IO;
using System.Windows.Forms;

namespace ACAT.Extensions.BCI.Actuators.SensorUI
{
    /// <summary>
    ///  User control which displays information on errors related to connecting to the optical sensor
    /// </summary>
    public partial class UserControlBCIErrorOpticalSensorDetect : UserControl
    {
        private String _htmlText = "<!DOCTYPE html>\r\n<html>\r\n  <head>\r\n  " +
                                    "<style>\r\n    a:link{color: rgb(255, 170, 0);}\r\n  </style>\r\n  " +
                                    "</head>\r\n  <body style=\"background-color:#232433;\">\r\n    " +
                                    "<p style=\"font-family:'Montserrat Medium'; font-size:24px; color:white; text-align: center;\">\r\n    " +
                                    "Make sure the optical sensor is plugged in. If it is already plugged in, unplug it and plug it back. " +
                                    "Then click on Retry. To fix this error, please review <a href=$ACAT_USER_GUIDE#OpticalSensorNotDetected>instructions</a>\r\n </p>\r\n  </body>\r\n</html>\r\n\r\n\r\n\r\n";

        public UserControlBCIErrorOpticalSensorDetect(String stepId)
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

        private String getHtmlText(String file)
        {
            var docsPath = SmartPath.ApplicationPath + "\\Docs";

            var htmlFile = docsPath + "\\" + file;

            if (File.Exists(htmlFile))
            {
                return File.ReadAllText(htmlFile);
            }

            return String.Empty;
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