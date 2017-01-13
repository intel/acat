////////////////////////////////////////////////////////////////////////////
// <copyright file="CameraSelectForm.cs" company="Intel Corporation">
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
using System.Collections.Generic;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.Actuators.VisionActuator.VisionUtils
{
    /// <summary>
    /// Displays a list of installed cameras in a combo box and stores
    /// the camera selected by the user
    /// </summary>
    public partial class CameraSelectForm : Form
    {
        /// <summary>
        /// List of discovered cameras
        /// </summary>
        public IEnumerable<String> CameraNames;

        /// <summary>
        /// Aspect ratio of form at design time
        /// </summary>
        private float _designTimeAspectRatio = 0.0f;

        /// <summary>
        /// Has first call to OnClientSizeChanged been made?
        /// </summary>
        private bool _firstClientChangedCall = true;

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public CameraSelectForm()
        {
            InitializeComponent();

            Load += CameraSelectForm_Load;
        }

        /// <summary>
        /// Text to display on the Cancel button
        /// </summary>
        public String CancelButtonText { get; set; }

        /// <summary>
        /// Text to display on the OK button
        /// </summary>
        public String OKButtonText { get; set; }

        /// <summary>
        /// Prompt to display on the form
        /// </summary>
        public String Prompt { get; set; }

        /// <summary>
        /// Gets the user selected camera
        /// </summary>
        public String SelectedCamera { get; private set; }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            SelectedCamera = String.Empty;

            DialogResult = DialogResult.Cancel;

            Close();
        }

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
        /// Event handler for the OK button.  Store user choice and
        /// close the dialog
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            int selectedIndex = comboBoxCameraList.SelectedIndex;

            SelectedCamera = comboBoxCameraList.Items[selectedIndex] as String;

            DialogResult = DialogResult.OK;

            Close();
        }

        /// <summary>
        /// Called when the form loads. Get a list of cameras and
        /// populate the combo box
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void CameraSelectForm_Load(object sender, EventArgs e)
        {
            float currentAspectRatio = (float)ClientSize.Height / ClientSize.Width;

            if (_designTimeAspectRatio != 0.0f && currentAspectRatio != _designTimeAspectRatio)
            {
                ClientSize = new System.Drawing.Size(ClientSize.Width, (int)(_designTimeAspectRatio * ClientSize.Width));
            }

            CenterToScreen();

            TopMost = false;
            TopMost = true;

            if (!String.IsNullOrEmpty(Prompt))
            {
                label1.Text = Prompt;
            }

            if (!String.IsNullOrEmpty(OKButtonText))
            {
                buttonOK.Text = OKButtonText;
            }

            if (!String.IsNullOrEmpty(CancelButtonText))
            {
                buttonCancel.Text = CancelButtonText;
            }

            foreach (var im in CameraNames)
            {
                comboBoxCameraList.Items.Add(im);
            }

            if (comboBoxCameraList.Items.Count > 0)
            {
                comboBoxCameraList.SelectedIndex = 0;
            }
        }
    }
}