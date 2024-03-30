////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// UserControlBCIErrorPortConfig.cs
//
// User control which displays information on errors related to configuration
// of the COM port for the BCI board
//
////////////////////////////////////////////////////////////////////////////

using ACAT.ACATResources;
using ACAT.Extensions.BCI.Actuators.EEG.EEGSettings;
using ACAT.Lib.Core.Utility;
using System;
using System.IO;
using System.Windows.Forms;

namespace ACAT.Extensions.BCI.Actuators.SensorUI
{
    /// <summary>
    /// User control which displays information on errors related to configuration 
    /// of the COM port for the BCI board
    /// </summary>
    public partial class UserControlBCIErrorPortConfig : UserControl
    {
        public String _currentBCIComPort = "x";

        private String _htmlText = "<!DOCTYPE html>\r\n<html>\r\n  <head>\r\n  <style>\r\n    a:link{color: rgb(255, 170, 0);}\r\n  </style>\r\n  </head>\r\n" +
                                    "<body style=\"background-color:#232433;\">\r\n    " +
                                    "<p style=\"font-family:'Montserrat Medium'; font-size:24px; color:white; text-align: center;\">\r\n    " +
                                    "Your board is connected to <span style=\"font-family:'Montserrat Black'; color:white;\"><em>@@@</em></span>. " +
                                    "The latency timer setting for <span style=\"font-family:'Montserrat Black';color:white;\"><em>@@@</em></span> is incorrect. " +
                                    "To fix this error, please review <a href=$ACAT_USER_GUIDE#PortConfigError>instructions</a>.<br/> Exit ACAT, " +
                                    "take corrective action, and restart ACAT.\r\n    </p>\r\n  </body>\r\n</html>";

        public UserControlBCIErrorPortConfig(String stepId)
        {
            InitializeComponent();

            // Get current BCI COM port to display in label info
            _currentBCIComPort = BCIActuatorSettings.Settings.DAQ_ComPort;

            webBrowserTop.DocumentCompleted += WebBrowserDesc_DocumentCompleted;
            var html = _htmlText.Replace(CoreGlobals.MacroACATUserGuide, HtmlUtils.EncodeString(CoreGlobals.ACATUserGuideFileName));
            html = html.Replace("@@@", _currentBCIComPort);
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