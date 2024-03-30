
////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Extensions.BCI.Common.BCIControl;
using ACAT.Lib.Core.ActuatorManagement;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System;
using System.Threading;

namespace ACAT.Extensions.BCI.Common.BCIInterfaceUtilities
{
    /// <summary>
    /// Scanner form for a Talk-only interface.  Displays a text box with a
    /// reduced alphabet scanner below it enabling the user to type text (with
    /// word prediction) and have the text converted to speech.  The keyboard
    /// layout is ABC.
    /// </summary>
    [DescriptorAttribute("36F021B7-615F-48FD-BA88-01679D9B4B61",
                        "CalibrationEyesSettingsForm",
                        "Application window used as a calibration UI for eyes open or closed settings")]
    public partial class CalibrationEyesSettingsForm : Form
    {
        public ResultParams ResultParameters = new ResultParams();

        /// <summary>
        /// Main object of the actuator
        /// </summary>
        private IActuator _bciActuator = null;
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
        public CalibrationEyesSettingsForm()
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

        public static ResultParams ShowDialog(string label, Form parent = null, bool setTopMost = false)
        {
            var confirmBox = new CalibrationEyesSettingsForm();
            if (parent != null && setTopMost)
            {
                parent.TopMost = false;
                confirmBox.TopMost = true;
            }
            confirmBox.ShowDialog(parent);
            ResultParams retVal = confirmBox.ResultParameters;
            confirmBox.Dispose();
            return retVal;
        }

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
        /// Button event (Start)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSave_Click(object sender, EventArgs e)
        {
            _Interval = _TempInterval;
            _MaxRepetitions = _TempMaxRepetitions;
            BCICalibrationEyesClosedParameters bCICalibrationEyesClosedParameters = new BCICalibrationEyesClosedParameters(_MaxRepetitions, _Interval);
            var str = JsonConvert.SerializeObject(bCICalibrationEyesClosedParameters);
            _bciActuator?.IoctlRequest((int)OpCodes.CalibrationEyesClosedSaveParameters, str);
            ValidateParameters();
        }

        /// <summary>
        /// Event when the main form closes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CalibrationEyesForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void EnableSaveButton(bool enable)
        {
            this.Invoke(new MethodInvoker(delegate
            {
                ButtonSave.Enabled = enable;
                ButtonSave.Visible = enable;
            }));
        }

        /// <summary>
        /// Event when loading main form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            RequestParameters();
            Thread.Sleep(100);
            InitUI();
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
            BCIMode bCIMode = new BCIMode { BciMode = BCIModes.CALIBRATION_EYESOPENCLOSE, BciCalibrationMode = BCIScanSections.None, };
            _bciActuator?.IoctlRequest((int)OpCodes.CalibrationEyesClosedRequestParameters, string.Empty);
        }
        /// <summary>
        /// Handler for the actuator response from BCI
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="response"></param>
        private void BciActuator_EvtIoctlResponse(int opcode, string response)
        {
            switch (opcode)
            {
                case (int)OpCodes.CalibrationEyesClosedSendParameters:
                    var bciParams = JsonConvert.DeserializeObject<BCICalibrationEyesClosedParameters>(response);
                    _MaxRepetitions = bciParams.NumRepetitions;
                    _Interval = bciParams.IntervalDuration;
                    _TempInterval = bciParams.IntervalDuration;
                    _TempMaxRepetitions = bciParams.NumRepetitions;
                    break;
            }
        }
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
                    ConfirmBox confirmBox = new ConfirmBox
                    {
                        Prompt = "Are you sure to exit without saving?"
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
                Log.Debug("Error in EyesSettingsForm: " + ex.Message);
            }
            if (_bciActuator != null)
            {
                _bciActuator.EvtIoctlResponse -= BciActuator_EvtIoctlResponse;
            }
            ResultParameters = new ResultParams { Interval = _Interval, MaxRepetitions = _MaxRepetitions };
            this.Close();
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
                Log.Debug(ex.ToString());
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
                Log.Debug(ex.ToString());
                _TempMaxRepetitions = 10;
                textBoxReps.Text = _TempMaxRepetitions.ToString();
            }
            ValidateParameters();
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
                Log.Debug(ex.ToString());
            }
            return inputReplace;
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
    }
}
