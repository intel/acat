////////////////////////////////////////////////////////////////////////////
// <copyright file="EditAppInfoForm.cs" company="Intel Corporation">
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
using System.Drawing;
using System.Windows.Forms;

namespace LaunchAppAgent
{
    /// <summary>
    /// Form to add/edit info for an application.  Info includes
    /// app name, path/folder to the application and optional args
    /// </summary>
    public partial class EditAppInfoForm : Form
    {
        /// <summary>
        /// Aspect ratio of form at design time
        /// </summary>
        private float _designTimeAspectRatio = 0.0f;

        /// <summary>
        /// Has first call to OnClientSizeChanged been made?
        /// </summary>
        private bool _firstClientChangedCall = true;

        /// <summary>
        /// Initializes a new instance of the form
        /// </summary>
        public EditAppInfoForm()
        {
            InitializeComponent();

            AppName = String.Empty;
            Path = String.Empty;
            Arguments = String.Empty;

            Load += AddNewAppForm_Load;
        }

        /// <summary>
        /// Gets or sets the application name
        /// </summary>
        public String AppName { get; set; }

        /// <summary>
        /// Gets or sets optional arguments
        /// </summary>
        public String Arguments { get; set; }

        /// <summary>
        /// Gets or sets the path to the application.
        /// </summary>
        public String Path { get; set; }

        /// <summary>
        /// Client size changed
        /// </summary>
        /// <param name="e">event args</param>
        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);
            if (_firstClientChangedCall)
            {
                _designTimeAspectRatio = (float)ClientSize.Height / ClientSize.Width;
                _firstClientChangedCall = false;
            }
        }

        /// <summary>
        /// Initializes the form
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void AddNewAppForm_Load(object sender, EventArgs e)
        {
            float currentAspectRatio = (float)ClientSize.Height / ClientSize.Width;

            if (_designTimeAspectRatio != 0.0f && currentAspectRatio != _designTimeAspectRatio)
            {
                ClientSize = new Size(ClientSize.Width, (int)(_designTimeAspectRatio * ClientSize.Width));
            }

            TopMost = true;
            CenterToScreen();

            textBoxApplicationName.Text = AppName;
            textBoxPath.Text = Path;
            textBoxArguments.Text = Arguments;
        }

        /// <summary>
        /// User wants to select a file.  Display the open file dialog
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonBrowseFile_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Exe Files (.exe)|*.exe|All Files (*.*)|*.*",
                FilterIndex = 1,
                Multiselect = false
            };

            var dialogResult = openFileDialog.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                textBoxPath.Text = openFileDialog.FileName;
            }
        }

        /// <summary>
        /// User wants to canel out of the form
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// User pressed OK.  Update properties and close the dialog
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            AppName = textBoxApplicationName.Text.Trim();
            Path = textBoxPath.Text.Trim();
            Arguments = textBoxArguments.Text.Trim();

            if (String.IsNullOrEmpty(AppName))
            {
                MessageBox.Show("Application name cannot be empty", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (String.IsNullOrEmpty(Path))
            {
                MessageBox.Show("Path cannot be empty", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// User wants to select a folder. Display the browse folder dialog
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonSelectFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            DialogResult result = fbd.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                textBoxPath.Text = fbd.SelectedPath;
            }
        }
    }
}