////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Extensions.BCI.Common.BCIControl;
using ACAT.Lib.Core.ActuatorManagement;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.WidgetManagement;
using System.Text.Json;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System;
using ACAT.Lib.Core.Utility;

namespace ACAT.Extensions.BCI.Common.BCIInterfaceUtilities
{
    /// <summary>
    /// Form that handles different calibraitons options to configure and initialize calibration sesion
    /// </summary>
    /// 
    [DescriptorAttribute("0E41996F-85E7-4809-9F6F-599119853651",
                        "ConfirmBoxTriggerBoxSettings",
                        "Application window used as a configuration UI for trigger test")]
    public partial class ConfirmBoxTriggerBoxSettings : Form
    {
        /// <summary>
        /// Main object of the actuator
        /// </summary>
        private IActuator _bciActuator = null;
        /// <summary>
        /// Interval of the timer
        /// </summary>
        private int _ScanningTime = 200;

        /// <summary>
        /// Maximum amount of repetitions for the data collection
        /// </summary>
        private int _NumberRepetitions = 10;
        /// <summary>
        /// Return value when the Form is closed
        /// </summary>
        private Tuple<BCIMenuOptions.Options, BCISimpleParameters> OptionResult;

        /// <summary>
        /// Custom Tooltip object
        /// </summary>
        private CustomToolTip customToolTip = new CustomToolTip();

        private Screen primaryScreen = Screen.PrimaryScreen;
        /// <summary>
        /// Confirm Box with multiple results
        /// </summary>
        public ConfirmBoxTriggerBoxSettings()
        {
            InitializeComponent();
            Load += ConfirmBox_Load;
        }
        
        public static Tuple<BCIMenuOptions.Options, BCISimpleParameters> ShowDialogForm(Form parent = null, bool setTopMost = false)
        {
            var confirmBox = new ConfirmBoxTriggerBoxSettings();
            if (parent != null && setTopMost)
            {
                parent.TopMost = false;
                confirmBox.TopMost = true;
            }
            //To always display the form in the main screen
            confirmBox.StartPosition = FormStartPosition.Manual;
            confirmBox.Location = confirmBox.primaryScreen.WorkingArea.Location;
            confirmBox.ShowDialog(parent);
            Tuple<BCIMenuOptions.Options, BCISimpleParameters> retVal = confirmBox.OptionResult;
            if (parent != null && setTopMost)
            {
                parent.TopMost = true;
                confirmBox.TopMost = false;
            }
            confirmBox.Dispose();
            return retVal;
        }
        /// <summary>
        /// Sets the values of the sliders based on the calibration mode parameters
        /// </summary>
        /// <param name="calibrationParametersForSection"></param>
        public void SetValuesSliders(int ScanningTime, int NumberRepetitions)
        {
            customSliderScanningTime.Value = ScanningTime;
            labelScanningTime.Text = ScanningTime.ToString();
            customSliderNumberTargets.Value = NumberRepetitions;
            labelNumberTargets.Text = NumberRepetitions.ToString();
        }

        private void ButtonStartTriggerTest_Click(object sender, EventArgs e)
        {
            try
            {
                ScannerRoundedButtonControl scannerRoundedButtonControl = (ScannerRoundedButtonControl)sender;
                switch (scannerRoundedButtonControl.Name)
                {
                    case var _ when scannerRoundedButtonControl.Name.Contains("TriggerTest"):
                        OptionResult = new Tuple<BCIMenuOptions.Options, BCISimpleParameters>(BCIMenuOptions.Options.TriggerTest, GetTriggerTestParameters());
                        BCITriggerTestParameters bciTriggerTestParameters = new BCITriggerTestParameters((int)customSliderNumberTargets.Value, (int)customSliderScanningTime.Value);
                        var str = JsonSerializer.Serialize(bciTriggerTestParameters);
                        _bciActuator?.IoctlRequest((int)OpCodes.TriggerTestSaveParameters, str);
                        break;
                }
                Close();
            }
            catch (Exception ex) 
            {
                Log.Debug("Error ButtonStartTriggerTest_Click: " + ex.Message);
                OptionResult = new Tuple<BCIMenuOptions.Options, BCISimpleParameters>(BCIMenuOptions.Options.TriggerTest, GetTriggerTestParameters());
                Close();
            }

        }

        /// <summary>
        /// Gets the parameters values from the sliders from the active calibration selected
        /// </summary>
        /// <returns></returns>
        private BCISimpleParameters GetTriggerTestParameters()
        {
            return new BCISimpleParameters { ScannTime = (int)customSliderScanningTime.Value, Targets = (int)customSliderNumberTargets.Value, IterationsPertarget = 0, MinScore = 0 };
        }

        private void ButtonDown_Click(object sender, EventArgs e)
        {
            ScannerRoundedButtonControl scannerRoundedButtonControl = (ScannerRoundedButtonControl)sender;
            switch (scannerRoundedButtonControl.Name)
            {
                case var _ when scannerRoundedButtonControl.Name.Contains("ScanningTime"):
                    var newvalueScanningTime = customSliderScanningTime.Value - 50;
                    if (newvalueScanningTime >= customSliderScanningTime.Minimum)
                    {
                        customSliderScanningTime.Value -= 50;
                        labelScanningTime.Text = customSliderScanningTime.Value.ToString();
                    }
                    break;
                case var _ when scannerRoundedButtonControl.Name.Contains("NumberTargets"):
                    var newvalueNumberTargets = customSliderNumberTargets.Value - 1;
                    if (newvalueNumberTargets >= customSliderNumberTargets.Minimum)
                    {
                        customSliderNumberTargets.Value -= 1;
                        labelNumberTargets.Text = customSliderNumberTargets.Value.ToString();
                    }
                    break;
            }
        }

        private void ButtonExit_Click_1(object sender, EventArgs e)
        {
            OptionResult = new Tuple<BCIMenuOptions.Options, BCISimpleParameters>(BCIMenuOptions.Options.Exit, new BCISimpleParameters());
            Close();
        }

        private void ButtonInfoModes_MouseEnter(object sender, EventArgs e)
        {
            ScannerRoundedButtonControl scannerRoundedButtonControl = (ScannerRoundedButtonControl)sender;
            switch (scannerRoundedButtonControl.Name)
            {
                case var _ when scannerRoundedButtonControl.Name.Contains("Box"):
                    customToolTip?.ShowToolTip("Trigger test it is", (ScannerRoundedButtonControl)sender, 5, 5);
                    break;
            }
        }

        private void ButtonInfoModes_MouseLeave(object sender, EventArgs e)
        {
            try{ customToolTip?.HideToolTip(); } catch (Exception ex) { Log.Debug("Error ButtonInfoModes_MouseLeave: " + ex.Message); }
        }

        private void ButtonInfoParameters_MouseEnter(object sender, EventArgs e)
        {
            ScannerRoundedButtonControl scannerRoundedButtonControl = (ScannerRoundedButtonControl)sender;
            switch (scannerRoundedButtonControl.Name)
            {
                case var _ when scannerRoundedButtonControl.Name.Contains("ScanningTime"):
                    customToolTip?.ShowToolTip("Scanning time hint", (ScannerRoundedButtonControl)sender, 5, 5);
                    break;
                case var _ when scannerRoundedButtonControl.Name.Contains("NumberTargets"):
                    customToolTip?.ShowToolTip("Number of targets hint", (ScannerRoundedButtonControl)sender, 5, 5);
                    break;
            }
        }

        private void ButtonOpcTriggerTest_Click(object sender, EventArgs e)
        {
            ScannerRoundedButtonControl scannerRoundedButtonControl = (ScannerRoundedButtonControl)sender;
            try
            {
                switch (scannerRoundedButtonControl.Name)
                {
                    case var _ when scannerRoundedButtonControl.Name.Contains("TriggerTest"):
                        SetButtonColorState((ScannerRoundedButtonControl)sender, LineBoxSelection);
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Error ButtonOpc_Click: " + ex.Message);
            }
        }

        private void ButtonRestoreDefaults_Click(object sender, EventArgs e)
        {
            SetValuesSliders(_ScanningTime, _NumberRepetitions);
        }

        private void ButtonUp_Click(object sender, EventArgs e)
        {
            ScannerRoundedButtonControl scannerRoundedButtonControl = (ScannerRoundedButtonControl)sender;
            switch (scannerRoundedButtonControl.Name)
            {
                case var _ when scannerRoundedButtonControl.Name.Contains("ScanningTime"):
                    var newvalueScanningTime = customSliderScanningTime.Value + 50;
                    if (newvalueScanningTime <= customSliderScanningTime.Maximum)
                    {
                        customSliderScanningTime.Value += 50;
                        labelScanningTime.Text = customSliderScanningTime.Value.ToString();
                    }
                    break;
                case var _ when scannerRoundedButtonControl.Name.Contains("NumberTargets"):
                    var newvalueNumberTargets = customSliderNumberTargets.Value + 1;
                    if (newvalueNumberTargets <= customSliderNumberTargets.Maximum)
                    {
                        customSliderNumberTargets.Value += 1;
                        labelNumberTargets.Text = customSliderNumberTargets.Value.ToString();
                    }
                    break;
            }
        }



        private void ConfirmBox_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            InitializeCustomSliders();
            _bciActuator = Context.AppActuatorManager.GetActuator(new Guid("77809D19-F450-4D36-A633-D818400B3D9A"));
            if (_bciActuator != null)
            {
                _bciActuator.EvtIoctlResponse += BciActuator_EvtIoctlResponse;
            }
            _bciActuator?.IoctlRequest((int)OpCodes.TriggerTestRequestParameters, string.Empty);
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
                case (int)OpCodes.TriggerTestSendParameters:
                    var triggerTestParams = JsonSerializer.Deserialize<BCITriggerTestParameters>(response);
                    _ScanningTime = triggerTestParams.ScanTime;
                    _NumberRepetitions = triggerTestParams.NumRepetitions;
                    SetValuesSliders(_ScanningTime, _NumberRepetitions);
                    TriggeFirstOptioFromMenu();
                    break;
            }
        }
        private void ConfirmBoxCalibrationModes_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_bciActuator != null)
            {
                _bciActuator.EvtIoctlResponse -= BciActuator_EvtIoctlResponse;
            }
            customToolTip?.CloseToolTip();
            customToolTip?.Dispose();
            customToolTip = null;
        }


        private void customSliderNumberTargets_ValueChanged(object sender, EventArgs e)
        {
            labelNumberTargets.Text = customSliderNumberTargets.Value.ToString();
        }

        private void customSliderScanningTime_ValueChanged(object sender, EventArgs e)
        {
            labelScanningTime.Text = customSliderScanningTime.Value.ToString();
        }


        private void InitializeCustomSliders()
        {
            customSliderScanningTime.Minimum = 50;
            customSliderScanningTime.Maximum = 3000;
            customSliderNumberTargets.Minimum = 1;
            customSliderNumberTargets.Maximum = 200;
        }



        /// <summary>
        /// Sets the color for buttons being selected
        /// </summary>
        /// <param name="scannerRoundedButtonControl"></param>
        /// <param name="linedivision"></param>
        private void SetButtonColorState(ScannerRoundedButtonControl scannerRoundedButtonControl, Panel linedivision)
        {
            try
            {
                scannerRoundedButtonControl.BackColor = Color.FromArgb(255, 170, 0);
                scannerRoundedButtonControl.ForeColor = Color.Black;
                linedivision.BackColor = Color.FromArgb(255, 170, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        /// <summary>
        /// Triggers the button being pressed
        /// </summary>
        private void TriggeFirstOptioFromMenu()
        {
            ButtonOpcTriggerTest_Click(ButtonOpcTriggerTest, EventArgs.Empty);
        }

    }
}