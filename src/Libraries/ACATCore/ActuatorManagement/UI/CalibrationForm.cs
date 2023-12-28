////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// CalibrationForm.cs
//
// Form to display the status of calibration. Displays calibration status
// of the actuator including an optional count-down or count-up timer
//
////////////////////////////////////////////////////////////////////////////

using ACAT.ACATResources;
using ACAT.Lib.Core.Utility;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace ACAT.Lib.Core.ActuatorManagement
{
    /// <summary>
    /// Form to display the status of calibration. Displays calibration status
    /// of the actuator including an optional count-down or count-up timer
    /// </summary>
    public partial class CalibrationForm : Form
    {
        /// <summary>
        /// Thickness of border around the form
        /// </summary>
        private const int BorderThickness = 1;

        /// <summary>
        /// How much time has elapsed since calibration started
        /// </summary>
        private readonly String _timeElapsedPrompt = R.GetString("TimeElapsed") + ": ";

        /// <summary>
        /// How much time is remaining until end of calibration
        /// </summary>
        private readonly String _timeRemainingPrompt = R.GetString("TimeRemaining") + ": ";

        /// <summary>
        /// To track calibration time
        /// </summary>
        private Timer _timer;

        /// <summary>
        /// Tracks time
        /// </summary>
        private int _timerCount;

        /// <summary>
        /// Count up? or count down
        /// / </summary>
        private int _timerIncrement = 1;

        /// <summary>
        /// Initializes an instance of hte class
        /// </summary>
        public CalibrationForm()
        {
            InitializeComponent();
            ShowInTaskbar = false;

            Load += OnLoad;
            Closing += OnClosing;
            BorderPanel.Paint += BorderPanelOnPaint;
        }

        /// <summary>
        /// Gets or sets the text to be displayed on the button
        /// in the form
        /// </summary>
        ///
        public String ButtonText { get; set; }

        /// <summary>
        /// Gets or sets Title caption for the form
        /// </summary>
        public String Caption { get; set; }

        /// <summary>
        /// Gets or sets whether to enable the "Configure" button
        /// on the form.
        /// </summary>
        public bool EnableConfigure { get; set; }

        /// <summary>
        /// Gets or sets the prompt to display
        /// </summary>
        public String Prompt { get; set; }

        /// <summary>
        /// Gets or sets the Actuator that is being calibrated
        /// </summary>
        public IActuator SourceActuator { get; set; }

        /// <summary>
        /// Gets or sets the Timeout for the calibration
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Closes the form
        /// </summary>
        public void Dismiss()
        {
            DialogResult = DialogResult.Yes;
            Windows.CloseForm(this);
        }

        /// <summary>
        /// Updates the caption, prompt, timeout
        /// </summary>
        /// <param name="caption">title bar</param>
        /// <param name="prompt">prompt to display</param>
        /// <param name="timeout">calibration timeout</param>
        public void Update(String caption, String prompt, int timeout)
        {
            Update(caption, prompt);
        }

        /// <summary>
        /// Updates the caption, prompt, timeout
        /// </summary>
        /// <param name="caption">title bar</param>
        /// <param name="prompt">prompt to display</param>
        public void Update(String caption, String prompt)
        {
            Invoke(new MethodInvoker(delegate
            {
                labelPrompt.Text = prompt;
                Text = caption;
            }));
        }

        /// <summary>
        /// Paint handler to paint the outer border
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void BorderPanelOnPaint(object sender, PaintEventArgs e)
        {
            if (BorderPanel.BorderStyle == BorderStyle.FixedSingle)
            {
                const int thickness = BorderThickness;
                const int halfThickness = thickness / 2;
                using (var pen = new Pen(Color.Black, thickness))
                {
                    e.Graphics.DrawRectangle(
                        pen,
                        new Rectangle(halfThickness,
                        halfThickness,
                        BorderPanel.ClientSize.Width - thickness,
                        BorderPanel.ClientSize.Height - thickness));
                }
            }
        }

        /// <summary>
        /// User pressed the configure button.  Notify
        /// actuator about this
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonConfigure_Click(object sender, EventArgs e)
        {
            stopTimer();

            Dismiss();

            ActuatorManager.Instance.OnCalibrationAction(SourceActuator);
        }

        /// <summary>
        /// Form is closing
        /// </summary>
        /// <param name="sender">event sender </param>
        /// <param name="cancelEventArgs">event args</param>
        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
            }
        }

        /// <summary>
        /// Form is loading. Initialize and start the calibration timer
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="eventArgs">event args</param>
        private void OnLoad(object sender, EventArgs eventArgs)
        {
            TopMost = false;
            TopMost = true;
            Text = Caption;
            labelPrompt.Text = Prompt;

            buttonConfigure.Visible = EnableConfigure;

            if (!EnableConfigure)
            {
                int oldHeight = Height;
                Height = buttonConfigure.Top + 10;
                int diff = oldHeight - Height;
                BorderPanel.Height -= diff;
            }
            else
            {
                if (!String.IsNullOrEmpty(ButtonText))
                {
                    buttonConfigure.Text = ButtonText;
                }
            }

            bool enableTimer = true;

            if (Timeout > 0)
            {
                _timerCount = Timeout;
                _timerIncrement = -1;
            }
            else if (Timeout == 0)
            {
                labelTimePrompt.Text = string.Empty;
                enableTimer = false;
            }
            else
            {
                _timerCount = 0;
                _timerIncrement = 1;
            }

            if (enableTimer)
            {
                startTimer();
            }
            else
            {
                _timer = null;
            }
        }

        /// <summary>
        /// Starts the timer for calibration
        /// </summary>
        private void startTimer()
        {
            _timer = new Timer { Interval = 1000 };
            _timer.Tick += TimerOnTick;
            TimerOnTick(null, null);
            _timer.Start();
        }

        /// <summary>
        /// Stops the calibration timer
        /// </summary>
        private void stopTimer()
        {
            if (_timer != null)
            {
                _timer.Tick -= TimerOnTick;
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }
        }

        /// <summary>
        /// Timer function.  Update the prompt to display the time
        /// elapsed or time left
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="eventArgs">event args</param>
        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            labelTimePrompt.Text = (_timerIncrement == -1) ? _timeRemainingPrompt : _timeElapsedPrompt;
            labelTimePrompt.Text += _timerCount + " " + R.GetString("Secs");

            _timerCount += _timerIncrement;

            if (_timerIncrement == -1 && _timerCount == 0)
            {
                ActuatorManager.Instance.OnCalibrationPeriodExpired(SourceActuator);
                _timerCount = Timeout;
            }
        }
    }
}