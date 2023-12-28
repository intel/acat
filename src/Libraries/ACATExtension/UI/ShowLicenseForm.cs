////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.ACATResources;
using ACAT.Lib.Core.Utility;
using System;
using System.IO;
using System.Windows.Forms;

namespace ACAT.Lib.Extension
{
    /// <summary>
    /// A form to display text from license.txt which contains a list
    /// of terms and conditions and all the licenses.
    /// </summary>
    public partial class ShowLicenseForm : Form
    {
        /// <summary>
        /// Name of the license file (default name)
        /// </summary>
        public String DefaultLicenseFileName = "License.txt";

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public ShowLicenseForm()
        {
            InitializeComponent();
            ShowInTaskbar = false;
            Load += ShowLicenseForm_Load;
        }

        /// <summary>
        /// Gets or set s the name of the license file
        /// </summary>
        public String LicenseFileName { get; set; }

        /// <summary>
        /// Close button handler.  Close the form
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Form loader.  Read license text and populate the box
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void ShowLicenseForm_Load(object sender, EventArgs e)
        {
            TopMost = true;

            CenterToScreen();

            if (String.IsNullOrEmpty(LicenseFileName))
            {
                LicenseFileName = DefaultLicenseFileName;
            }

            var path = Path.Combine(SmartPath.ApplicationPath, LicenseFileName);
            if (!File.Exists(path))
            {
                MessageBox.Show("License file " + path + " does not exist");
                Close();
            }

            buttonOK.Text = R.GetString(buttonOK.Text);

            try
            {
                var text = File.ReadAllText(path);
                textBoxLicense.Text = text;
            }
            catch
            {
                Close();
            }
        }
    }
}