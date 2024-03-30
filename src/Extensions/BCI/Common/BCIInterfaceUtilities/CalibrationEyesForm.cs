
////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// CalibrationEyesForm.cs
//
// Application window used as a calibration UI for eyes open or closed
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Extensions.BCI.Common.BCIControl;
using ACAT.Lib.Core.ActuatorManagement;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using Newtonsoft.Json;
using static ACAT.Extensions.BCI.Common.BCIControl.BCICalibrationEyesClosedIterationEnd;
using System.Drawing;
using System.Windows.Forms;
using System;

namespace ACAT.Extensions.BCI.Common.BCIInterfaceUtilities
{
    /// <summary>
    /// Scanner form for a Talk-only interface.  Displays a text box with a
    /// reduced alphabet scanner below it enabling the user to type text (with
    /// word prediction) and have the text converted to speech.  The keyboard
    /// layout is ABC.
    /// </summary>
    [DescriptorAttribute("36F021B7-615F-48FD-BA88-01679D9B4B60",
                        "CalibrationEyesForm",
                        "Application window used as a calibration UI for eyes open or closed")]
    public partial class CalibrationEyesForm : Form
    {
        /// <summary>
        /// Main object of the actuator
        /// </summary>
        private IActuator _bciActuator = null;

        /// <summary>
        /// Counter for the user to know when the data collection will start (seconds)
        /// </summary>
        int _CountdownCounter = 3;

        /// <summary>
        /// Main Timer for data collection
        /// </summary>
        private Timer _Countertimer;

        /// <summary>
        /// Interval of the timer
        /// </summary>
        private int _Interval = 5000;

        /// <summary>
        /// Maximum amount of repetitions for the data collection
        /// </summary>
        private int _MaxRepetitions = 5;

        /// <summary>
        /// Current repetition of data collection
        /// </summary>
        private int _Repetitions = 0;

        /// <summary>
        /// Main Timer for data collection
        /// </summary>
        private Timer _timer;

        /// <summary>
        /// Current mode for BCI
        /// </summary>
        BCIMode bCIMode = new BCIMode();

        /// <summary>
        /// Flag to set UI 
        /// </summary>
        private bool eyeOpen = false;

        public CalibrationEyesForm()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.TopMost = true;
            Screen primaryScreen = Screen.PrimaryScreen;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = primaryScreen.WorkingArea.Location;
            _bciActuator = Context.AppActuatorManager.GetActuator(new Guid("77809D19-F450-4D36-A633-D818400B3D9A"));
            Load += Form1_Load;
        }

        /// <summary>
        /// Button event (Start)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonStart_Click(object sender, EventArgs e)
        {
            StartCalibrationEyesSession();
        }
        private void StartCalibrationEyesSession()
        {
            this.Invoke(new MethodInvoker(delegate
            {
                labelInstruction.Text = string.Empty;
            }));
            if (_timer.Enabled)
            {
                _Countertimer.Stop();
                _timer.Stop();
                _bciActuator?.IoctlRequest((int)OpCodes.CalibrationEyesClosedEnd, string.Empty);
            }
            else
                SetEnableStateButtons(false);
            CreateTimers();
            eyeOpen = false;
            _bciActuator?.IoctlRequest((int)OpCodes.StartSession, JsonConvert.SerializeObject(bCIMode));
            _Countertimer.Start();
            _timer.Start();
        }
        private void ButtonCancel_Click_1(object sender, EventArgs e)
        {
            _bciActuator?.IoctlRequest((int)OpCodes.CalibrationEyesClosedEnd, string.Empty);
            ResetValues();
        }

        /// <summary>
        /// Button event (Close)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCancel_Close(object sender, EventArgs e)
        {
            OnFormClosing();
        }
        private void ButtonExit_Click(object sender, EventArgs e)
        {
            OnFormClosing();
        }
        /// <summary>
        /// Event when the main form closes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CalibrationEyesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_timer != null && _timer.Enabled)
            {
                _timer.Stop();
                _bciActuator?.IoctlRequest((int)OpCodes.CalibrationEyesClosedEnd, string.Empty);
                _timer = null;
            }
            if (_Countertimer != null && _Countertimer.Enabled)
            {
                _Countertimer.Stop();
                _Countertimer = null;
            }
        }

        /// <summary>
        /// Initialize Timers 
        /// </summary>
        private void CreateTimers()
        {
            if (_timer == null)
            {
                _timer = new Timer();
                _timer.Interval = 3100;
                _timer.Tick += Timer_Tick;
            }
            else
                _timer.Interval = 3100;

            if (_Countertimer == null)
            {
                _Countertimer = new Timer();
                _Countertimer.Interval = 10;
                _Countertimer.Tick += Timer_Tick_Countdown;
            }
            else
                _Countertimer.Interval = 10;
        }

        /// <summary>
        /// Event when loading main form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            SetEnableStateButtons(false);
            InitUI();
            ShowCalibrationEyesSettingsForm();
            StartCalibrationEyesSession();
        }

        /// <summary>
        /// Initialize the graphic elements of the UI
        /// </summary>
        private void InitUI()
        {
            this.Invoke(new MethodInvoker(delegate
            {
                pictureBoxEyesClosed.Visible = false;
                pictureBoxEyesOpen.Visible = false;
                labelInstruction.Text = string.Empty;
                labelCountdown.Text = "Press \"Start\" to begin \nEye calibration";
                ButtonCancel.Visible = false;
                ButtonCancel.Enabled = false;
            }));
        }

        private void OnFormClosing()
        {
            if (_timer.Enabled)
            {
                _timer.Stop();
                _timer = null;
            }
            if (_Countertimer.Enabled)
            {
                _Countertimer.Stop();
                _Countertimer = null;
            }
            _bciActuator?.IoctlRequest((int)OpCodes.CalibrationEyesClosedEnd, string.Empty);
            this.Close();
        }
        /// <summary>
        /// Resets the values to start a new eyes calibration session
        /// </summary>
        private void ResetValues()
        {
            SetEnableStateButtons(true);
            _Countertimer.Stop();
            _timer.Stop();
            _CountdownCounter = 3;
            eyeOpen = false;
            this.Invoke(new MethodInvoker(delegate
            {
                pictureBoxEyesClosed.Visible = false;
                pictureBoxEyesOpen.Visible = false;
                labelInstruction.Text = "Calibration finished";
                labelCountdown.Text = "Press \"Start\" to begin \nEye calibration";
                labelCountdown.Visible = true;
            }));
            _Repetitions = 0;
        }

        /// <summary>
        /// Enable or disable the start Button if parameters are received 
        /// </summary>
        /// <param name="enable"></param>
        private void SetEnableStateButtons(bool enable)
        {
            this.Invoke(new MethodInvoker(delegate
            {
                ButtonStart.Visible = enable;
                ButtonStart.Enabled = enable;
                ButtonCancel.Visible = !enable;
                ButtonCancel.Enabled = !enable;
            }));
        }

        /// <summary>
        /// Shows the CalibrationEyes Settigs Form
        /// </summary>
        private void ShowCalibrationEyesSettingsForm()
        {
            CalibrationEyesSettingsForm calibrationEyesSettingsForm = new CalibrationEyesSettingsForm();
            calibrationEyesSettingsForm.ShowDialog();
            var parameters = calibrationEyesSettingsForm.ResultParameters;
            calibrationEyesSettingsForm.Dispose();
            _MaxRepetitions = parameters.MaxRepetitions;
            _Interval = parameters.Interval;
            CreateTimers();
            SetEnableStateButtons(true);
        }
        /// <summary>
        /// Main timer 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            labelCountdown.Visible = false;
            Timer timer = (Timer)sender;
            timer.Interval = _Interval;
            if (_Repetitions < _MaxRepetitions)
            {
                _Repetitions += 1;
                eyeOpen = !eyeOpen;
                SoundManager.playSound(eyeOpen ? SoundManager.SoundType.OpenEyesCalibration : SoundManager.SoundType.CloseEyesCalibration);

                this.Invoke(new MethodInvoker(delegate
                {
                    pictureBoxEyesClosed.Visible = !eyeOpen;
                    pictureBoxEyesOpen.Visible = eyeOpen;
                    labelInstruction.Text = (eyeOpen) ? "Open your eyes" : "Close your eyes";
                    panelTriggerBox.BackColor = eyeOpen ? Color.White : Color.Black;
                }));

                BCICalibrationEyesClosedIterationEnd bCICalibrationEyesClosedIterationEnd = new BCICalibrationEyesClosedIterationEnd { BciEyesClosedMode = eyeOpen ? BCIEyesClosedModes.EyesOpened : BCIEyesClosedModes.EyesClosed,};
                _bciActuator?.IoctlRequest((int)OpCodes.CalibrationEyesClosedIterationEnd, JsonConvert.SerializeObject(bCICalibrationEyesClosedIterationEnd));
            }
            else
            {
                ResetValues();
                SoundManager.playSound(SoundManager.SoundType.OpenEyesCalibration);
                _bciActuator?.IoctlRequest((int)OpCodes.CalibrationEyesClosedEnd, string.Empty);
            }

        }
        /// <summary>
        /// Countdown Timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick_Countdown(object sender, EventArgs e)
        {
            labelCountdown.Text = _CountdownCounter.ToString();
            Timer timer = (Timer)sender;
            timer.Interval = 1000;
            _CountdownCounter -= 1;
        }
    }
}
