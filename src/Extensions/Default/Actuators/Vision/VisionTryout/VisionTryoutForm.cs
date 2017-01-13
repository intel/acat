////////////////////////////////////////////////////////////////////////////
// <copyright file="VisionTryoutForm.cs" company="Intel Corporation">
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

using ACAT.Extensions.Default.Actuators.VisionActuator.VisionUtils;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.Actuators.Vision.VisionTryout
{
    /// <summary>
    /// Form to let the user tryout ACAT vision, practice gestures
    /// </summary>
    public partial class VisionTryoutForm : Form
    {
        /// <summary>
        /// State machine to track button status
        /// </summary>
        private ButtonState _appState = ButtonState.NONE;

        /// <summary>
        /// To quit the thread
        /// </summary>
        private bool _hasExitAppSent;

        /// <summary>
        /// Title of the form
        /// </summary>
        private String _title = "ACAT Vision Tryout";

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public VisionTryoutForm()
        {
            InitializeComponent();
            Load += Form1_Load;
        }

        /// <summary>
        /// State machine
        /// </summary>
        private enum ButtonState { NONE, START_VISION, STOP_VISION };

        /// <summary>
        /// Starts ACAT vision
        /// </summary>
        private static void startvision()
        {
            VisionSensor.acatVision();
        }

        /// <summary>
        /// User clicked on button.  Depending on state machine
        /// state, handle it
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (_appState == ButtonState.NONE)
            {
                var mainThread = new Thread(startvision);
                mainThread.Start();

                _appState = ButtonState.START_VISION;
                buttonVision.Text = "Stop";
            }
            else if (_appState == ButtonState.START_VISION)
            {
                if (!_hasExitAppSent)
                {
                    _hasExitAppSent = true;
                    VisionSensor.visionCommand("action=EXITAPP");
                }

                _appState = ButtonState.STOP_VISION;
                buttonVision.Text = "Exit";
            }
            else if (_appState == ButtonState.STOP_VISION)
            {
                if (!_hasExitAppSent)
                {
                    VisionSensor.visionCommand("action=EXITAPP");
                }

                Close();
            }
        }

        /// <summary>
        /// Callback from vision.  Unused for this app
        /// </summary>
        /// <param name="text">info about what's happening</param>
        private void CallbackFromVision(string text)
        {
            Trace.WriteLine("Callback from vision " + (text ?? ""));
        }

        /// <summary>
        /// Form is closing. Quit vision
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_hasExitAppSent)
            {
                VisionSensor.visionCommand("action=EXITAPP");
            }
        }

        /// <summary>
        /// Form load event handler.  Quit if there are no
        /// cameras.  If multiple cameras, let the user select
        /// the preferred one to use
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void Form1_Load(object sender, EventArgs e)
        {
            Text = _title;

            var _installedCameras = Cameras.GetCameraNames();

            if (!_installedCameras.Any())
            {
                MessageBox.Show("No cameras detected. Exiting",
                                _title,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);

                Close();
                return;
            }

            var preferredCamera = String.Empty;

            if (_installedCameras.Count() > 1)
            {
                var form = new CameraSelectForm
                {
                    CameraNames = _installedCameras,
                    Name = _title,
                    Prompt = "Select Camera",
                    OKButtonText = "OK",
                    CancelButtonText = "Cancel"
                };

                var result = form.ShowDialog();

                if (result == DialogResult.Cancel)
                {
                    MessageBox.Show("No camera selected.  Exiting", _title);
                    Close();
                    return;
                }

                preferredCamera = form.SelectedCamera;
            }
            else
            {
                preferredCamera = _installedCameras.ElementAt(0);
            }

            VisionSensor.selectCamera(preferredCamera);

            Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - Width,
                                            Screen.PrimaryScreen.WorkingArea.Height - Height);

            TopMost = false;
            TopMost = true;

            VisionSensor.SetVisionEventHandler(CallbackFromVision);
        }
    }
}