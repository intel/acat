////////////////////////////////////////////////////////////////////////////
// <copyright file="YesNoForm.cs" company="Intel Corporation">
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

using System;
using System.Windows.Forms;

namespace ACATDashboard
{
    /// <summary>
    /// Displays a Yes/No dialog box with a prompot
    /// </summary>
    public partial class YesNoForm : Form
    {
        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        /// <param name="prompt">Prompt to display</param>
        public YesNoForm(String prompt)
        {
            InitializeComponent();
            labelPrompt.Text = prompt;
            CenterToScreen();
        }

        /// <summary>
        /// Gets or sets the prompt
        /// </summary>
        public String Prompt { get; set; }

        /// <summary>
        /// User clicked No. Close the dialog
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonNo_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
            Close();
        }

        /// <summary>
        /// User clicked Yes.  Close the dialog
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonYes_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
            Close();
        }
    }
}