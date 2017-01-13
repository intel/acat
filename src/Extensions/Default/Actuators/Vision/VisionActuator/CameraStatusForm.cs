////////////////////////////////////////////////////////////////////////////
// <copyright file="CameraStatusForm.cs" company="Intel Corporation">
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

using ACAT.Lib.Core.Utility;
using System;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.Actuators.Vision.VisionActuator
{
    /// <summary>
    /// Displays a prompt string with the current status of the
    /// camera (say, "Camera Initializing..."
    /// </summary>
    public partial class CameraStatusForm : Form
    {
        /// <summary>
        /// Delegate for the event raised when the user closes
        /// the form by clicking on the X button
        /// </summary>
        public CancelEventDelegate EvtCancel;

        /// <summary>
        /// Set to true if the event needs to be raised
        /// </summary>
        private bool _raiseEvtCloseEvent = true;

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public CameraStatusForm()
        {
            InitializeComponent();
            Load += CameraStatusForm_Load;
        }

        /// <summary>
        /// Deleate for the event raised when the user
        /// closes the form by pressing the X button
        /// </summary>
        public delegate void CancelEventDelegate();

        /// <summary>
        /// Gets or sets the prompt string to be displayed
        /// </summary>
        public String Prompt { get; set; }

        /// <summary>
        /// Closes the form programatically.
        /// </summary>
        public void Cancel()
        {
            _raiseEvtCloseEvent = false;
            Windows.CloseForm(this);
        }

        /// <summary>
        /// Form closing override.  Raise event if necessary
        /// </summary>
        /// <param name="e">args</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (_raiseEvtCloseEvent && EvtCancel != null)
            {
                EvtCancel();
            }
        }

        /// <summary>
        /// Event handler when the form is loaded
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void CameraStatusForm_Load(object sender, EventArgs e)
        {
            TopMost = false;
            TopMost = true;
            CenterToScreen();
            textBoxPrompt.Text = Prompt;
        }
    }
}
