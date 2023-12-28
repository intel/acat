////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ACAT.Lib.Extension
{
    /// <summary>
    /// Displays the about box with information about the application,
    /// version, copyright and 3rd Party attributions.
    /// </summary>
    public partial class AboutBoxForm : Form
    {
        /// <summary>
        /// The name of the application
        /// </summary>
        private String _appName;

        /// <summary>
        /// 3rd party attributions
        /// </summary>
        private List<String> _attributions;

        /// <summary>
        /// Company information
        /// </summary>
        private String _companyInfo;

        /// <summary>
        /// Copyright info
        /// </summary>
        private String _copyRightInfo;

        /// Version number information
        /// </summary>
        private String _versionInfo;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public AboutBoxForm(string titleText)
        {
            InitializeComponent();

            _attributions = new List<string>();

            Text = titleText;

            Load += Form_Load;
        }

        /// <summary>
        /// Gets or sets the title of the form
        /// </summary>
        public String AppName
        {
            get
            {
                return _appName;
            }

            set
            {
                _appName = value;
                Windows.SetText(labelAppTitle, value);
            }
        }

        /// <summary>
        /// Gets or sets 3rd party attributions
        /// </summary>
        public IEnumerable<String> Attributions
        {
            get
            {
                return _attributions;
            }

            set
            {
                _attributions = value.ToList();
            }
        }

        /// <summary>
        /// Gets or sets copyright information
        /// </summary>
        public String CopyrightInfo
        {
            get
            {
                return _copyRightInfo;
            }

            set
            {
                _copyRightInfo = value;
                Windows.SetText(labelCopyrightInfo, value);
            }
        }

        /// <summary>
        /// Gets or sets the company information
        /// </summary>
        public String UrlInfo
        {
            get
            {
                return _companyInfo;
            }
            set
            {
                _companyInfo = value;
                Windows.SetText(labelURL, value);
            }
        }

        /// <summary>
        /// Gets or sets the prompt to be displayed
        /// </summary>
        public String VersionInfo
        {
            get
            {
                return _versionInfo;
            }

            set
            {
                _versionInfo = value;
                Windows.SetText(labelVersionInfo, value);
            }
        }

        /// Event handler for displaying license file
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonLicenses_Click(object sender, EventArgs e)
        {
            var showLicenseForm = new ShowLicenseForm();
            showLicenseForm.ShowDialog(this);
            showLicenseForm.Dispose();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Form loader handler
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void Form_Load(object sender, EventArgs e)
        {
            buttonOK.Focus();
            CenterToScreen();
        }

        private void buttonDisclaimer_Click(object sender, EventArgs e)
        {
            var showDisclaimerForm = new ShowDisclaimersForm();
            showDisclaimerForm.ShowDialog(this);
            showDisclaimerForm.Dispose();
        }
    }
}