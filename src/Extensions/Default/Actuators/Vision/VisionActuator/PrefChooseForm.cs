////////////////////////////////////////////////////////////////////////////
// <copyright file="PrefChooseForm.cs" company="Intel Corporation">
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
using ACAT.Extensions.Default.Actuators.VisionActuator.VisionUtils;
using ACAT.Lib.Core.ActuatorManagement;
using ACAT.Lib.Core.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.Actuators.Vision.VisionActuator
{
    /// <summary>
    /// Displays a form with choices to either configure switches or
    /// select the default camera
    /// </summary>
    public partial class PrefChooseForm : Form
    {
        /// <summary>
        /// List of cameras installed on the system
        /// </summary>
        private IEnumerable<String> _installedCameras;

        /// <summary>
        /// Title of the form
        /// </summary>
        private String _title = "ACAT Vision";

        /// <summary>
        /// Aspect ratio of form at design time
        /// </summary>
        private float _designTimeAspectRatio = 0.0f;

        /// <summary>
        /// Has first call to OnClientSizeChanged been made?
        /// </summary>
        private bool _firstClientChangedCall = true;

        /// <summary>
        /// Creates an instance of the form
        /// </summary>
        public PrefChooseForm()
        {
            InitializeComponent();

            Load += PrefChooseForm_Load;
        }

        /// <summary>
        /// Gets or sets the actuator for which preferences are being set
        /// </summary>
        public IActuator Actuator { get; set; }

        /// <summary>
        /// Close button handler
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonClose_Click(object sender, EventArgs e)
        {
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
        /// Configure the switches for this actuator.  Invokes the
        /// default dialog for doing this
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonConfigureSwitches_Click(object sender, EventArgs e)
        {
            Hide();

            var configureSwitchesForm = new ConfigureSwitchesForm {Actuator = Actuator};
            configureSwitchesForm.ShowDialog();

            Show();
        }

        /// <summary>
        /// Select the default camera to use as the vision sensor
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonSelectCamera_Click(object sender, EventArgs e)
        {
            Hide();
            showCameraSelectDialog();
            Show();
        }

        /// <summary>
        /// Event handler for form loader
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void PrefChooseForm_Load(object sender, EventArgs e)
        {
            float currentAspectRatio = (float)ClientSize.Height / ClientSize.Width;

            if (_designTimeAspectRatio != 0.0f && currentAspectRatio != _designTimeAspectRatio)
            {
                ClientSize = new System.Drawing.Size(ClientSize.Width, (int)(_designTimeAspectRatio * ClientSize.Width));
            }

            CenterToScreen();

            TopMost = true;

            _installedCameras = Cameras.GetCameraNames();
            if (!_installedCameras.Any())
            {
                buttonSelectCamera.Enabled = false;
            }
        }

        /// <summary>
        /// Displays the dialog that lets the user select the 
        /// default camera.  If the user selected one, saves
        /// the name of the camera in the settings file for the 
        /// vision actuator
        /// </summary>
        /// <returns>true on success</returns>
        private bool showCameraSelectDialog()
        {
            var preferredCamera = String.Empty;

            if (!_installedCameras.Any())
            {
                return false;
            }

            var form = new CameraSelectForm
            {
                CameraNames = _installedCameras,
                Name = _title,
                Prompt = R.GetString("SelectCamera"),
                OKButtonText = R.GetString("OK"),
                CancelButtonText = R.GetString("Cancel")
            };

            var result = form.ShowDialog();

            if (result != DialogResult.Cancel)
            {
                MessageBox.Show(String.Format(R.GetString("SelectedCamera"), form.SelectedCamera),
                    _title,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                preferredCamera = form.SelectedCamera;

                Settings.SettingsFilePath = UserManager.GetFullPath(VisionActuator.SettingsFileName);
                var settings = Settings.Load();

                settings.PreferredCamera = preferredCamera;

                settings.Save();
            }

            return true;
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
