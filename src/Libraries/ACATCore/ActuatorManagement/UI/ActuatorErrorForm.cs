////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ActuatorErrorForm.cs
//
// This form displays an actuator error
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows.Forms;

namespace ACAT.Lib.Core.ActuatorManagement
{
    /// <summary>
    /// This form displays an actuator error
    /// </summary>
    public partial class ActuatorErrorForm : Form
    {
        /// <summary>
        /// Initializes a new instance of teh class
        /// </summary>
        public ActuatorErrorForm()
        {
            InitializeComponent();

            ShowInTaskbar = false;
            Load += OnLoad;
        }

        /// <summary>
        /// Get or sets the caption to use in the error form
        /// </summary>
        public String Caption { get; set; }

        /// <summary>
        /// Should the "Configure" button be enabled?
        /// </summary>
        public bool EnableConfigure { get; set; }

        /// <summary>
        /// Gets or sets the error message
        /// </summary>
        public String Prompt { get; set; }

        /// <summary>
        /// Gets or sets the actuator that initiated the error
        /// </summary>
        public IActuator SourceActuator { get; set; }

        /// <summary>
        /// Dismisses the dialog
        /// </summary>
        public void Dismiss()
        {
            DialogResult = DialogResult.Yes;
        }

        /// <summary>
        /// Updates the form owth the caption and the prompt
        /// </summary>
        /// <param name="caption">caption to set</param>
        /// <param name="prompt">prompt to display</param>
        public void Update(String caption, String prompt)
        {
            Invoke(new MethodInvoker(delegate
            {
                labelPrompt.Text = prompt;
                labelCaption.Text = caption;
            }));
        }

        /// <summary>
        /// User pressed the Configure" button. Hides the form and
        /// display the configuration dialog
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonConfigure_Click(object sender, EventArgs e)
        {
            Close();

            ActuatorManager.Instance.OnCalibrationAction(SourceActuator);
        }

        /// <summary>
        /// User pressed the OK button. Closes the form.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Handler for when the form loads
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="eventArgs">event args</param>
        private void OnLoad(object sender, EventArgs eventArgs)
        {
            CenterToScreen();

            TopMost = false;
            TopMost = true;
            labelCaption.Text = Caption;
            labelPrompt.Text = Prompt;

            buttonConfigure.Enabled = EnableConfigure;
        }
    }
}