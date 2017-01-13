////////////////////////////////////////////////////////////////////////////
// <copyright file="ShowLicenseForm.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2017 Intel Corporation 
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
////////////////////////////////////////////////////////////////////////////

using ACAT.ACATResources;
using ACAT.Lib.Core.Utility;
using System;
using System.IO;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.UI.Dialogs
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
        private void buttonClose_Click(object sender, EventArgs e)
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

            buttonClose.Text = R.GetString(buttonClose.Text);

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