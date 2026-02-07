////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.ActuatorManagement.Interfaces;
using ACAT.Core.PanelManagement;
using ACAT.Core.Utility;
using ACAT.Extensions.BCI.Common.BCIControl;
using ACATResources;
using Microsoft.Extensions.Logging;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace ACAT.Extensions.BCI.Common.BCIInterfaceUtilities
{
    /// <summary>
    /// Scanner form for a Talk-only interface.  Displays a text box with a
    /// reduced alphabet scanner below it enabling the user to type text (with
    /// word prediction) and have the text converted to speech.  The keyboard
    /// layout is ABC.
    /// </summary>
    [ClassDescriptor("36F021B7-615F-48FD-BA88-01679D9B4B61",
                        "CalibrationEyesSettingsForm",
                        "Application window used as a calibration UI for eyes open or closed settings")]
    public partial class CalibrationEyesSettingsForm : Form
    {
        #region Properties

        public ResultParams ResultParameters = new();

        /// <summary>
        /// Logger instance
        /// </summary>
        private readonly ILogger<CalibrationEyesSettingsForm> _logger;

        /// <summary>
        /// Main object of the actuator
        /// </summary>
        private readonly IActuator _bciActuator = null;

        /// <summary>
        /// Interval of the timer
        /// </summary>
        private int _Interval = 5000;

        /// <summary>
        /// Maximum amount of repetitions for the data collection
        /// </summary>
        private int _MaxRepetitions = 10;

        /// <summary>
        /// Interval of the timer
        /// </summary>
        private int _TempInterval = 5000;

        /// <summary>
        /// Maximum amount of repetitions for the data collection
        /// </summary>
        private int _TempMaxRepetitions = 10;

        #endregion Properties

        public CalibrationEyesSettingsForm(ILogger<CalibrationEyesSettingsForm> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            Screen primaryScreen = Screen.PrimaryScreen;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = primaryScreen.WorkingArea.Location;
            _bciActuator = Context.AppActuatorManager.GetActuator(new Guid("77809D19-F450-4D36-A633-D818400B3D9A"));
            Load += Form1_Load;
        }

        public static ResultParams ShowDialog(string label, Form parent = null, bool setTopMost = false)
        {
            var confirmBox = new CalibrationEyesSettingsForm();
            confirmBox.ShowDialog(parent);
            ResultParams retVal = confirmBox.ResultParameters;
            confirmBox.Dispose();
            return retVal;
        }

        #region Control Events

        private void BtnDownInterval_Click(object sender, EventArgs e)
        {
            if (_TempInterval > 100)
            {
                _TempInterval -= 100;
                textBoxInterval.Text = _TempInterval.ToString();
            }
            ValidateParameters();
        }

        private void BtnDownRepetitions_Click(object sender, EventArgs e)
        {
            if (_TempMaxRepetitions > 0)
            {
                _TempMaxRepetitions -= 1;
                textBoxReps.Text = _TempMaxRepetitions.ToString();
            }
            ValidateParameters();
        }

        private void BtnUpInterval_Click(object sender, EventArgs e)
        {
            _TempInterval += 100;
            textBoxInterval.Text = _TempInterval.ToString();
            ValidateParameters();
        }

        private void BtnUpRepetitions_Click(object sender, EventArgs e)
        {
            _TempMaxRepetitions += 1;
            textBoxReps.Text = _TempMaxRepetitions.ToString();
            ValidateParameters();
        }

        private void ButtonCancel_Close(object sender, EventArgs e)
        {
            OnFormClosing();
        }

        private void ButtonExit_Click(object sender, EventArgs e)
        {
            OnFormClosing();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            _Interval = _TempInterval;
            _MaxRepetitions = _TempMaxRepetitions;
            BCICalibrationEyesClosedParameters bCICalibrationEyesClosedParameters = new(_MaxRepetitions, _Interval);
            _bciActuator?.IoctlRequest((int)OpCodes.CalibrationEyesClosedSaveParameters, bCICalibrationEyesClosedParameters);
            ValidateParameters();
        }

        private void CalibrationEyesForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RequestParameters();
            Thread.Sleep(100);
            InitUI();
        }

        /// <summary>
        /// Handler for the actuator response from BCI
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="response"></param>
        private void BciActuator_EvtIoctlResponse(int opcode, object response)
        {
            switch (opcode)
            {
                case (int)OpCodes.CalibrationEyesClosedSendParameters:
                    var bciParams = response as BCICalibrationEyesClosedParameters;
                    _MaxRepetitions = bciParams.NumRepetitions;
                    _Interval = bciParams.IntervalDuration;
                    _TempInterval = bciParams.IntervalDuration;
                    _TempMaxRepetitions = bciParams.NumRepetitions;
                    break;
            }
        }

        private void textBoxInterval_TextChanged(object sender, EventArgs e)
        {
            bool inputReplace;
            inputReplace = TextChangedInput(textBoxInterval);
            try
            {
                if (textBoxInterval.Text.Length > 0)
                    _TempInterval = Int32.Parse(textBoxInterval.Text);
                if (textBoxInterval.Text.Length == 0 && inputReplace)
                {
                    _TempInterval = 0;
                    textBoxInterval.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing interval value");
                _TempInterval = 5000;
                textBoxInterval.Text = _TempInterval.ToString();
            }
            ValidateParameters();
        }

        private void textBoxReps_TextChanged(object sender, EventArgs e)
        {
            bool inputReplace;
            inputReplace = TextChangedInput(textBoxReps);
            try
            {
                if (textBoxReps.Text.Length > 0)
                    _TempMaxRepetitions = Int32.Parse(textBoxReps.Text);
                if (textBoxReps.Text.Length == 0 && inputReplace)
                {
                    _TempMaxRepetitions = 0;
                    textBoxReps.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing repetitions value");
                _TempMaxRepetitions = 10;
                textBoxReps.Text = _TempMaxRepetitions.ToString();
            }
            ValidateParameters();
        }

        #endregion Control Events

        #region Methods

        /// <summary>
        /// Initialize the graphic elements of the UI
        /// </summary>
        private void InitUI()
        {
            this.Invoke(new MethodInvoker(delegate
            {
                textBoxInterval.Text = _TempInterval.ToString();
                textBoxReps.Text = _TempMaxRepetitions.ToString();
            }));
            ValidateParameters();
        }

        private void OnFormClosing()
        {
            try
            {
                bool quitApp = true;
                if (_TempInterval != _Interval || _TempMaxRepetitions != _MaxRepetitions)
                {
                    ConfirmBoxTwoOption confirmBox = new()
                    {
                        Prompt = StringResources.exitwithoutsaving,
                        Op1Prompt = StringResources.OK,
                        Op3Prompt = StringResources.Cancel
                    };
                    confirmBox.ShowDialog(this);
                    quitApp = confirmBox.Result;
                    confirmBox.Dispose();
                }
                if (!quitApp)
                    return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in EyesSettingsForm");
            }
            if (_bciActuator != null)
            {
                _bciActuator.EvtIoctlResponse -= BciActuator_EvtIoctlResponse;
            }
            ResultParameters = new ResultParams { Interval = _Interval, MaxRepetitions = _MaxRepetitions };
            this.Close();
        }

        /// <summary>
        /// Call to request parameters
        /// </summary>
        public void RequestParameters()
        {
            if (_bciActuator != null)
            {
                _bciActuator.EvtIoctlResponse += BciActuator_EvtIoctlResponse;
            }
            BCIMode bCIMode = new() { BciMode = BCIModes.CALIBRATION_EYESOPENCLOSE, BciCalibrationMode = BCIScanSections.None, };
            _bciActuator?.IoctlRequest((int)OpCodes.CalibrationEyesClosedRequestParameters, string.Empty);
        }

        /// <summary>
        /// Handles the input of the text box to filter letters and numbers
        /// </summary>
        /// <param name="textBox"></param>
        /// <returns></returns>
        private bool TextChangedInput(TextBox textBox)
        {
            bool inputReplace = false;
            try
            {
                string input = textBox.Text;
                string pattern = @"^\d+$"; // Only allows one or more digits

                if (!Regex.IsMatch(input, pattern))
                {
                    textBox.Text = Regex.Replace(input, "[^0-9]", ""); // Remove any non-numeric characters
                    textBox.SelectionStart = textBox.Text.Length; // Move the cursor to the end
                    inputReplace = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in TextChangedInput");
            }
            return inputReplace;
        }

        private void EnableSaveButton(bool enable)
        {
            this.Invoke(new MethodInvoker(delegate
            {
                ButtonSave.Enabled = enable;
                ButtonSave.Visible = enable;
            }));
        }

        private void ValidateParameters()
        {
            if (_TempInterval != _Interval || _TempMaxRepetitions != _MaxRepetitions)
                EnableSaveButton(true);
            else
                EnableSaveButton(false);
        }

        public struct ResultParams
        {
            public int Interval;
            public int MaxRepetitions;
        }

        #endregion Methods
    }
}