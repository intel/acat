////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.ACATResources;
using ACAT.Extensions.BCI.Common.BCIControl;
using ACAT.Lib.Core.ActuatorManagement;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System;

namespace ACAT.Extensions.BCI.Common.BCIInterfaceUtilities
{
    /// <summary>
    /// Form that handles different calibraitons options to configure and initialize calibration sesion
    /// </summary>
    ///
    [DescriptorAttribute("4C48F7B3-FB3F-4857-9C36-E4148BA8FE5D",
                        "ConfirmBoxCalibrationModes",
                        "Application window used as a calibration UI for different modes")]
    public partial class ConfirmBoxCalibrationModes : Form
    {
        /// <summary>
        /// Main object of the actuator
        /// </summary>
        private IActuator _bciActuator = null;

        /// <summary>
        /// Object holding all the parameters of calibrations
        /// </summary>
        private BCIParameters _bCIParameters;

        /// <summary>
        /// Buttons states
        /// </summary>
        private Dictionary<ScannerRoundedButtonControl, CalibrationModeControls> _ButtonsState = new Dictionary<ScannerRoundedButtonControl, CalibrationModeControls>();

        /// <summary>
        /// Mode selected when paramaters show
        /// </summary>
        private BCIScanSections _scanSectionsSelected;

        /// <summary>
        /// String of the actuator response
        /// </summary>
        private BCICalibrationStatus ActuatorResponse;

        /// <summary>
        /// Custom Tooltip object
        /// </summary>
        private CustomToolTip customToolTip = new CustomToolTip();

        /// <summary>
        /// IF typing is enalbed
        /// </summary>
        private bool IsTypingEnabled;

        /// <summary>
        /// Return value when the Form is closed
        /// </summary>
        private Tuple<BCIMenuOptions.Options, BCISimpleParameters> OptionResult;

        private Screen primaryScreen = Screen.PrimaryScreen;

        /// <summary>
        /// Confirm Box with multiple results
        /// </summary>
        public ConfirmBoxCalibrationModes()
        {
            InitializeComponent();
            Load += ConfirmBox_Load;

            webBrowser.DocumentCompleted += WebBrowser_DocumentCompleted;
            var html = R.GetString("BCICalibrationHtmlText").Replace(CoreGlobals.MacroACATUserGuide, HtmlUtils.EncodeString(CoreGlobals.ACATUserGuideFileName));
            webBrowser.DocumentText = html;
        }

        public static Tuple<BCIMenuOptions.Options, BCISimpleParameters> ShowDialog(BCICalibrationStatus actuatorResponse, bool enableBeginBtn, Form parent = null, bool setTopMost = false)
        {
            var confirmBox = new ConfirmBoxCalibrationModes();
            if (parent != null && setTopMost)
            {
                parent.TopMost = false;
                confirmBox.TopMost = true;
            }
            //To always display the form in the main screen
            confirmBox.StartPosition = FormStartPosition.Manual;
            confirmBox.Location = confirmBox.primaryScreen.WorkingArea.Location;
            confirmBox.ActuatorResponse = actuatorResponse;
            confirmBox.IsTypingEnabled = enableBeginBtn;
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
        public void SetValuesSliders(CalibrationParametersForSection calibrationParametersForSection)
        {
            ValidateSliderValues(customSliderScanningTime, labelScanningTime, calibrationParametersForSection.ScanTime);
            ValidateSliderValues(customSliderNumberTargets, labelNumberTargets, calibrationParametersForSection.TargetCount);
            ValidateSliderValues(customSliderIterationstarget, labelIterationstarget, calibrationParametersForSection.IterationsPerTarget);
            ValidateSliderValues(customSliderMinimumScore, labelMinimumScore, calibrationParametersForSection.MinimumScoreRequired);
            if (!tableLayoutConfigurations.Visible)
            {
                ShowAllOptionsParameters(false);
                tableLayoutConfigurations.Visible = true;
                tableLayoutConfigurations.Enabled = true;
            }
        }

        /// <summary>
        /// Actuator event handler
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="response"></param>
        private void BciActuator_EvtIoctlResponse(int opcode, string response)
        {
            switch (opcode)
            {
                case (int)OpCodes.SendParameters:
                    _bCIParameters = JsonConvert.DeserializeObject<BCIParameters>(response);
                    TriggeFirstOptioFromMenu();
                    break;
            }
        }

        private void ButtonBeginACAT_Click(object sender, EventArgs e)
        {
            if (IsTypingEnabled)
            {
                OptionResult = new Tuple<BCIMenuOptions.Options, BCISimpleParameters>(BCIMenuOptions.Options.Typing, new BCISimpleParameters());
                Close();
            }
        }

        private void ButtonCalibrate_Click(object sender, EventArgs e)
        {
            Log.Debug("BCI LOG | Mode Selected: " + _scanSectionsSelected);
            BCISimpleParameters parameters = new BCISimpleParameters();
            try
            {
                ScannerRoundedButtonControl scannerRoundedButtonControl = (ScannerRoundedButtonControl)sender;
                switch (scannerRoundedButtonControl.Name)
                {
                    case var _ when scannerRoundedButtonControl.Name.Contains("Box"):
                        Log.Debug("BCI LOG | Calibrate Button Selected: " + BCIScanSections.Box);
                        if (_scanSectionsSelected == BCIScanSections.Box)
                            parameters = GetCalibrationParameters();
                        else
                            parameters = GetCalibrationParametersDefault(BCIScanSections.Box);
                        OptionResult = new Tuple<BCIMenuOptions.Options, BCISimpleParameters>(BCIMenuOptions.Options.Box, parameters);
                        break;

                    case var _ when scannerRoundedButtonControl.Name.Contains("Sentence"):
                        Log.Debug("BCI LOG | Calibrate Button Selected " + BCIScanSections.Sentence);
                        if (_scanSectionsSelected == BCIScanSections.Sentence)
                            parameters = GetCalibrationParameters();
                        else
                            parameters = GetCalibrationParametersDefault(BCIScanSections.Sentence);
                        OptionResult = new Tuple<BCIMenuOptions.Options, BCISimpleParameters>(BCIMenuOptions.Options.Sentence, parameters);
                        break;

                    case var _ when scannerRoundedButtonControl.Name.Contains("KeyboardL"):
                        Log.Debug("BCI LOG | Calibrate Button Selected " + BCIScanSections.KeyboardL);
                        if (_scanSectionsSelected == BCIScanSections.KeyboardL)
                            parameters = GetCalibrationParameters();
                        else
                            parameters = GetCalibrationParametersDefault(BCIScanSections.KeyboardL);
                        OptionResult = new Tuple<BCIMenuOptions.Options, BCISimpleParameters>(BCIMenuOptions.Options.KeyboardL, parameters);
                        break;

                    case var _ when scannerRoundedButtonControl.Name.Contains("Word"):
                        Log.Debug("BCI LOG | Calibrate Button Selected " + BCIScanSections.Word);
                        if (_scanSectionsSelected == BCIScanSections.Word)
                            parameters = GetCalibrationParameters();
                        else
                            parameters = GetCalibrationParametersDefault(BCIScanSections.Word);
                        OptionResult = new Tuple<BCIMenuOptions.Options, BCISimpleParameters>(BCIMenuOptions.Options.Word, parameters);
                        break;

                    case var _ when scannerRoundedButtonControl.Name.Contains("KeyboardR"):
                        Log.Debug("BCI LOG | Calibrate Button Selected " + BCIScanSections.KeyboardR);
                        if (_scanSectionsSelected == BCIScanSections.KeyboardR)
                            parameters = GetCalibrationParameters();
                        else
                            parameters = GetCalibrationParametersDefault(BCIScanSections.KeyboardR);
                        OptionResult = new Tuple<BCIMenuOptions.Options, BCISimpleParameters>(BCIMenuOptions.Options.KeyboardR, parameters);
                        break;
                }
                Close();
            }
            catch (Exception ex)
            {
                Log.Debug("BCI LOG | Error ButtonCalibrate_Click " + ex.Message);
            }
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

                case var _ when scannerRoundedButtonControl.Name.Contains("Iterationstarget"):
                    var newvalueIterationstarget = customSliderIterationstarget.Value - 1;
                    if (newvalueIterationstarget >= customSliderIterationstarget.Minimum)
                    {
                        customSliderIterationstarget.Value -= 1;
                        labelIterationstarget.Text = customSliderIterationstarget.Value.ToString();
                    }
                    break;

                case var _ when scannerRoundedButtonControl.Name.Contains("MinimumScore"):
                    var newvalueMinimumScore = customSliderMinimumScore.Value - 1;
                    if (newvalueMinimumScore >= customSliderMinimumScore.Minimum)
                    {
                        customSliderMinimumScore.Value -= 1;
                        labelMinimumScore.Text = customSliderMinimumScore.Value.ToString();
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
                    customToolTip?.ShowToolTip(BCIR.GetString("HintBoxCalibrationScreen"), (ScannerRoundedButtonControl)sender, 5, 5);
                    break;

                case var _ when scannerRoundedButtonControl.Name.Contains("Sentence"):
                    customToolTip?.ShowToolTip(BCIR.GetString("HintSentenceCalibrationScreen"), (ScannerRoundedButtonControl)sender, 5, 5);
                    break;

                case var _ when scannerRoundedButtonControl.Name.Contains("KeyboardL"):
                    customToolTip?.ShowToolTip(BCIR.GetString("HintKeyboardLeftCalibrationScreen"), (ScannerRoundedButtonControl)sender, 5, 5);
                    break;

                case var _ when scannerRoundedButtonControl.Name.Contains("Word"):
                    customToolTip?.ShowToolTip(BCIR.GetString("HintWordCalibrationScreen"), (ScannerRoundedButtonControl)sender, 5, 5);
                    break;

                case var _ when scannerRoundedButtonControl.Name.Contains("KeyboardR"):
                    customToolTip?.ShowToolTip(BCIR.GetString("HintKeyboardRightCalibrationScreen"), (ScannerRoundedButtonControl)sender, 5, 5);
                    break;

                case var _ when scannerRoundedButtonControl.Name.Contains("ButtonBeginACAT"):
                    if (!IsTypingEnabled)
                        customToolTip?.ShowToolTip(BCIR.GetString("HintCalibrationBeginACATNotEnalbed"), (ScannerRoundedButtonControl)sender, -120, -260);
                    break;
            }
        }

        private void ButtonInfoModes_MouseLeave(object sender, EventArgs e)
        {
            try { customToolTip?.HideToolTip(); } catch (Exception ex) { Log.Debug("Error ButtonInfoModes_MouseLeave: " + ex.Message); }
        }

        private void ButtonInfoParameters_MouseEnter(object sender, EventArgs e)
        {
            ScannerRoundedButtonControl scannerRoundedButtonControl = (ScannerRoundedButtonControl)sender;
            switch (scannerRoundedButtonControl.Name)
            {
                case var _ when scannerRoundedButtonControl.Name.Contains("ScanningTime"):
                    customToolTip?.ShowToolTip(BCIR.GetString("HintScanningTime"), (ScannerRoundedButtonControl)sender, 5, 5);
                    break;

                case var _ when scannerRoundedButtonControl.Name.Contains("NumberTargets"):
                    customToolTip?.ShowToolTip(BCIR.GetString("HintNumberTargets"), (ScannerRoundedButtonControl)sender, 5, 5);
                    break;

                case var _ when scannerRoundedButtonControl.Name.Contains("IterationsTarget"):
                    customToolTip?.ShowToolTip(BCIR.GetString("HintIterationstarget"), (ScannerRoundedButtonControl)sender, 5, 5);
                    break;

                case var _ when scannerRoundedButtonControl.Name.Contains("MinimumScore"):
                    customToolTip?.ShowToolTip(BCIR.GetString("HintMinimumScore"), (ScannerRoundedButtonControl)sender, 5, 5);
                    break;
            }
        }

        private void ButtonOpc_Click(object sender, EventArgs e)
        {
            ScannerRoundedButtonControl scannerRoundedButtonControl = (ScannerRoundedButtonControl)sender;
            try
            {
                switch (scannerRoundedButtonControl.Name)
                {
                    case var _ when scannerRoundedButtonControl.Name.Contains("Box"):
                        SetButtonColorState((ScannerRoundedButtonControl)sender, LineBoxSelection);
                        SetValuesSliders(_bCIParameters.CalibrationParameters[BCIScanSections.Box]);
                        _scanSectionsSelected = BCIScanSections.Box;
                        break;

                    case var _ when scannerRoundedButtonControl.Name.Contains("Sentence"):
                        SetButtonColorState((ScannerRoundedButtonControl)sender, LineSentenceSelection);
                        SetValuesSliders(_bCIParameters.CalibrationParameters[BCIScanSections.Sentence]);
                        _scanSectionsSelected = BCIScanSections.Sentence;
                        break;

                    case var _ when scannerRoundedButtonControl.Name.Contains("KeyboardL"):
                        SetButtonColorState((ScannerRoundedButtonControl)sender, LineKeyboardLSelection);
                        SetValuesSliders(_bCIParameters.CalibrationParameters[BCIScanSections.KeyboardL]);
                        _scanSectionsSelected = BCIScanSections.KeyboardL;
                        break;

                    case var _ when scannerRoundedButtonControl.Name.Contains("Word"):
                        SetButtonColorState((ScannerRoundedButtonControl)sender, LineWordSelection);
                        SetValuesSliders(_bCIParameters.CalibrationParameters[BCIScanSections.Word]);
                        _scanSectionsSelected = BCIScanSections.Word;
                        break;

                    case var _ when scannerRoundedButtonControl.Name.Contains("KeyboardR"):
                        SetButtonColorState((ScannerRoundedButtonControl)sender, LineKeyboardRSelection);
                        SetValuesSliders(_bCIParameters.CalibrationParameters[BCIScanSections.KeyboardR]);
                        _scanSectionsSelected = BCIScanSections.KeyboardR;
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Debug("BCI LOG | Error ButtonOpc_Click " + ex.Message);
            }
        }

        private void ButtonOtherTest_Click(object sender, EventArgs e)
        {
            var result = BCIInterfaceUtils.ShowMoreTestForm(this);
            switch (result)
            {
                case BCIMenuOptions.Options.RemapCalibrations:
                    var retValue = BCIInterfaceUtils.ShowRemapCalibrationsForm(this);
                    if (retValue)
                    {
                        //Need to close so it can request again calibration ststus in case that mapping change
                        OptionResult = new Tuple<BCIMenuOptions.Options, BCISimpleParameters>(BCIMenuOptions.Options.RemapCalibrations, new BCISimpleParameters());
                        Close();
                    }
                    break;

                case BCIMenuOptions.Options.TriggerTest:
                    OptionResult = new Tuple<BCIMenuOptions.Options, BCISimpleParameters>(BCIMenuOptions.Options.TriggerTest, new BCISimpleParameters());
                    Close();
                    break;

                case BCIMenuOptions.Options.SignalCheck:
                    OptionResult = new Tuple<BCIMenuOptions.Options, BCISimpleParameters>(BCIMenuOptions.Options.SignalCheck, new BCISimpleParameters());
                    Close();
                    break;

                case BCIMenuOptions.Options.EyesCalibration:
                    break;
            }
        }

        private void ButtonRestoreDefaults_Click(object sender, EventArgs e)
        {
            SetValuesSliders(_bCIParameters.CalibrationParameters[_scanSectionsSelected]);
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

                case var _ when scannerRoundedButtonControl.Name.Contains("Iterationstarget"):
                    var newvalueIterationstarget = customSliderIterationstarget.Value + 1;
                    if (newvalueIterationstarget <= customSliderIterationstarget.Maximum)
                    {
                        customSliderIterationstarget.Value += 1;
                        labelIterationstarget.Text = customSliderIterationstarget.Value.ToString();
                    }
                    break;

                case var _ when scannerRoundedButtonControl.Name.Contains("MinimumScore"):
                    var newvalueMinimumScore = customSliderMinimumScore.Value + 1;
                    if (newvalueMinimumScore <= customSliderMinimumScore.Maximum)
                    {
                        customSliderMinimumScore.Value += 1;
                        labelMinimumScore.Text = customSliderMinimumScore.Value.ToString();
                    }
                    break;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (!_ButtonsState[ButtonOpcWord].IsSelected && !_ButtonsState[ButtonOpcKeyboardR].IsSelected)
                ShowAllOptions(checkBoxAdditionalCalibrations.Checked);
            else
                ((CheckBox)sender).Checked = true;
        }

        private void checkBoxAdvancesParameters_CheckedChanged(object sender, EventArgs e)
        {
            ShowAllOptionsParameters(checkBoxAdvancesParameters.Checked);
        }

        private void ConfirmBox_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            InitializeButtonsState();
            ShowAllOptions(false);
            InitializeCustomSliders();
            SetUIElements();
            SetTextUIElements();
            ProcessCalibrationStatusResult();
            ShowAllCalibrationModes();
            _bciActuator = Context.AppActuatorManager.GetActuator(new Guid("77809D19-F450-4D36-A633-D818400B3D9A"));
            if (_bciActuator != null)
            {
                _bciActuator.EvtIoctlResponse += BciActuator_EvtIoctlResponse;
            }
            var strBciModeParams = JsonConvert.SerializeObject(new BCIUserInputParameters());
            _bciActuator?.IoctlRequest((int)OpCodes.RequestParameters, strBciModeParams);
            DisplayCalibrationHelp();
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

        private void customSliderIterationstarget_ValueChanged(object sender, EventArgs e)
        {
            labelIterationstarget.Text = customSliderIterationstarget.Value.ToString();
        }

        private void customSliderMinimumScore_ValueChanged(object sender, EventArgs e)
        {
            labelMinimumScore.Text = customSliderMinimumScore.Value.ToString();
        }

        private void customSliderNumberTargets_ValueChanged(object sender, EventArgs e)
        {
            labelNumberTargets.Text = customSliderNumberTargets.Value.ToString();
        }

        private void customSliderScanningTime_ValueChanged(object sender, EventArgs e)
        {
            labelScanningTime.Text = customSliderScanningTime.Value.ToString();
        }

        /// <summary>
        /// Shows the calibration help
        /// </summary>
        private void DisplayCalibrationHelp()
        {
            try
            {
                if (CoreGlobals.AppPreferences.ShowCalibrationHelp)
                {
                    bool saveConfig = BCIInterfaceUtils.ShowCalibrationHelpWindow(this);
                    if (saveConfig)
                    {
                        CoreGlobals.AppPreferences.ShowCalibrationHelp = false;
                        CoreGlobals.AppPreferences.Save();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug("BCI LOG | Exception DisplayCalibrationHelp " + ex.Message);
            }
        }

        /// <summary>
        /// Gets the parameters values from the sliders from the active calibration selected
        /// </summary>
        /// <returns></returns>
        private BCISimpleParameters GetCalibrationParameters()
        {
            return new BCISimpleParameters { ScannTime = (int)customSliderScanningTime.Value, Targets = (int)customSliderNumberTargets.Value, IterationsPertarget = (int)customSliderIterationstarget.Value, MinScore = (int)customSliderMinimumScore.Value };
        }

        /// <summary>
        /// Gets the parameters values from the actuator instead from the sliders
        /// </summary>
        /// <returns></returns>
        private BCISimpleParameters GetCalibrationParametersDefault(BCIScanSections bCIScanSections)
        {
            BCISimpleParameters bciSimpleParameters = new BCISimpleParameters();
            try
            {
                bciSimpleParameters = new BCISimpleParameters
                {
                    ScannTime = _bCIParameters.CalibrationParameters[bCIScanSections].ScanTime,
                    Targets = _bCIParameters.CalibrationParameters[bCIScanSections].TargetCount,
                    IterationsPertarget = _bCIParameters.CalibrationParameters[bCIScanSections].IterationsPerTarget,
                    MinScore = _bCIParameters.CalibrationParameters[bCIScanSections].MinimumScoreRequired
                };
            }
            catch (Exception ex)
            {
                Log.Debug("BCI LOG | Error GetCalibrationParametersDefault | " + ex.Message.ToString());
            }
            return bciSimpleParameters;
        }

        /// <summary>
        /// Sets the initial state of controls for the UI
        /// </summary>
        private void InitializeButtonsState()
        {
            //Button - is button selected, Panel line user as division
            _ButtonsState.Add(ButtonOpcBox, new CalibrationModeControls { IsSelected = false, DivisionLineMode = LineBoxSelection });
            _ButtonsState.Add(ButtonOpcSentence, new CalibrationModeControls { IsSelected = false, DivisionLineMode = LineSentenceSelection });
            _ButtonsState.Add(ButtonOpcKeyboardL, new CalibrationModeControls { IsSelected = false, DivisionLineMode = LineKeyboardLSelection });
            _ButtonsState.Add(ButtonOpcWord, new CalibrationModeControls { IsSelected = false, DivisionLineMode = LineWordSelection });
            _ButtonsState.Add(ButtonOpcKeyboardR, new CalibrationModeControls { IsSelected = false, DivisionLineMode = LineKeyboardRSelection });
        }

        /// <summary>
        /// Sets the initial values for the sliders
        /// </summary>
        private void InitializeCustomSliders()
        {
            customSliderScanningTime.Minimum = 100;
            customSliderScanningTime.Maximum = 400;
            customSliderNumberTargets.Minimum = 10;
            customSliderNumberTargets.Maximum = 100;
            customSliderIterationstarget.Minimum = 1;
            customSliderIterationstarget.Maximum = 20;
            customSliderMinimumScore.Minimum = 10;
            customSliderMinimumScore.Maximum = 100;
        }

        /// <summary>
        /// Process the result from the actuator to display the scores of calibrations if applicable
        /// </summary>
        private void ProcessCalibrationStatusResult()
        {
            try
            {
                foreach (var calibrationData in ActuatorResponse.DictClassifierInfo)
                {
                    switch (calibrationData.Key)
                    {
                        case BCIScanSections.Box:
                            SetLabelScoreText(labelScoreBox, ButtonCalibrateBox, calibrationData.Value);
                            break;

                        case BCIScanSections.Sentence:
                            SetLabelScoreText(labelScoreSentence, ButtonCalibrateSentence, calibrationData.Value);
                            break;

                        case BCIScanSections.KeyboardL:
                            SetLabelScoreText(labelScoreKeyboardL, ButtonCalibrateKeyboardL, calibrationData.Value);
                            break;

                        case BCIScanSections.Word:
                            SetLabelScoreText(labelScoreWord, ButtonCalibrateWord, calibrationData.Value);
                            break;

                        case BCIScanSections.KeyboardR:
                            SetLabelScoreText(labelScoreKeyboardR, ButtonCalibrateKeyboardR, calibrationData.Value);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug("BCI LOG | Error ProcessCalibrationStatusResult | " + ex.Message.ToString());
            }
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
                foreach (KeyValuePair<ScannerRoundedButtonControl, CalibrationModeControls> button in _ButtonsState)
                {
                    if (button.Value.IsSelected)
                    {
                        button.Key.BackColor = Color.FromArgb(35, 36, 51);
                        button.Key.ForeColor = Color.White;
                        button.Value.DivisionLineMode.BackColor = Color.FromArgb(35, 36, 51);
                        _ButtonsState[button.Key].IsSelected = false;
                        break;
                    }
                }
                scannerRoundedButtonControl.BackColor = Color.FromArgb(255, 170, 0);
                scannerRoundedButtonControl.ForeColor = Color.Black;
                linedivision.BackColor = Color.FromArgb(255, 170, 0);
                _ButtonsState[scannerRoundedButtonControl].IsSelected = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Sets the text for the buttons and labels used to display the scores
        /// </summary>
        /// <param name="label"></param>
        /// <param name="scannerRoundedButtonControl"></param>
        /// <param name="score"></param>
        private void SetLabelScoreText(Label label, ScannerRoundedButtonControl scannerRoundedButtonControl, BCIClassifierInfo bCIClassifierInfo)
        {
            if (bCIClassifierInfo.Auc > 0)
                label.Text = (bCIClassifierInfo.Auc * 100).ToString();
            else
                label.Text = "-";
            switch (bCIClassifierInfo.ClassifierStatus)
            {
                case BCIClassifierStatus.Expired:
                case BCIClassifierStatus.NotFound:
                    if (bCIClassifierInfo.IsRequired)
                        scannerRoundedButtonControl.Text = BCIR.GetString("CalibrateNow");
                    else
                        scannerRoundedButtonControl.Text = BCIR.GetString("CalibrateOptional");
                    break;

                case BCIClassifierStatus.Ok:
                    if (bCIClassifierInfo.IsRequired)
                        scannerRoundedButtonControl.Text = BCIR.GetString("Recalibrate");
                    else
                        scannerRoundedButtonControl.Text = BCIR.GetString("CalibrateOptional");
                    break;
            }
        }

        /// <summary>
        /// Sets the text for the controls in the UI
        /// </summary>
        private void SetTextUIElements()
        {
            ButtonBeginACAT.Text = BCIR.GetString("StartTyping");
        }

        /// <summary>
        /// Sets the initial state of some UI elements
        /// </summary>
        private void SetUIElements()
        {
            tableLayoutConfigurations.Visible = false;
            tableLayoutConfigurations.Enabled = false;
            tableLayoutPanelAdditionalOptions.Visible = false;
            tableLayoutPanelAdditionalOptions.Enabled = false;
            labelCalibrationMessage.Visible = true;
            labelCalibrationMessage.Enabled = true;
            if (!IsTypingEnabled)
            {
                ButtonBeginACAT.BackColor = Color.FromArgb(129, 129, 129);
                ButtonBeginACAT.ForeColor = Color.White;
            }
        }

        /// <summary>
        /// Triggers the checkbox if is necessary to show all options
        /// </summary>
        /// <param name="show"></param>
        private void ShowAllCalibrationModes()
        {
            if (!ActuatorResponse.ShowOnlyDefaults)
            {
                checkBoxAdditionalCalibrations.Checked = true;
                checkBox1_CheckedChanged(checkBoxAdditionalCalibrations, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Display in the UI all the options
        /// </summary>
        /// <param name="show"></param>
        private void ShowAllOptions(bool show)
        {
            tableLayoutPanelAdditionalOptions.Visible = show;
            tableLayoutPanelAdditionalOptions.Enabled = show;
            labelCalibrationMessage.Visible = !show;
            labelCalibrationMessage.Enabled = !show;
        }

        /// <summary>
        /// Shows all the options of parameters for calibration
        /// </summary>
        /// <param name="show"></param>
        private void ShowAllOptionsParameters(bool show)
        {
            tableLayoutNumberTargets.Visible = show;
            tableLayoutNumberTargets.Enabled = show;
            tableLayoutIterationsTarget.Visible = show;
            tableLayoutIterationsTarget.Enabled = show;
            tableLayoutMinimumScore.Visible = show;
            tableLayoutMinimumScore.Enabled = show;
        }

        /// <summary>
        /// Triggers the button being pressed
        /// </summary>
        private void TriggeFirstOptioFromMenu()
        {
            ButtonOpc_Click(ButtonOpcBox, EventArgs.Empty);
        }

        /// <summary>
        /// Validates the value from the parameters to see if are within range of the sliders
        /// </summary>
        /// <param name="colorSlider"></param>
        /// <param name="label"></param>
        /// <param name="value"></param>
        private void ValidateSliderValues(ColorSlider.ColorSlider colorSlider, Label label, int value)
        {
            try
            {
                if (value >= colorSlider.Minimum && value <= colorSlider.Maximum)
                {
                    colorSlider.Value = value;
                    label.Text = value.ToString();
                }
                else
                {
                    colorSlider.Value = colorSlider.Minimum;
                    label.Text = colorSlider.Minimum.ToString();
                }
            }
            catch (Exception ex)
            {
                colorSlider.Value = colorSlider.Minimum;
                label.Text = colorSlider.Minimum.ToString();
                Log.Debug("BCI LOG | ValidateSliderValues | " + ex.Message.ToString());
            }
        }

        private void WebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowser.Navigating -= WebBrowser_Navigating;
            webBrowser.Navigating += WebBrowser_Navigating;
        }

        private void WebBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            var str = e.Url.ToString();

            Log.Debug("Url is [" + str + "]");

            if (str.ToLower().Contains("blank"))
            {
                return;
            }

            e.Cancel = true;

            String param1 = String.Empty;
            String param2 = String.Empty;

            if (str.Contains("about:"))
            {
                var index = str.IndexOf(':');

                str = str.Substring(index + 1);

                index = str.IndexOf('#');

                if (index > 0)
                {
                    param1 = str.Substring(0, index);
                    param2 = str.Substring(index + 1, str.Length - index - 1);
                }
                else
                {
                    param1 = str;
                }
            }

            List<String> list = new List<String>();

            if (param2.ToLower().EndsWith(".mp4"))
            {
                list.Add("Video");
                list.Add(String.Empty);
                list.Add(String.Empty);
                list.Add((param2));
                list.Add(String.Empty);
            }
            else if (param1.ToLower().EndsWith(".pdf"))
            {
                list.Add("PDF");
                list.Add("true");
                list.Add(R.GetString("PDFLoaderHtml"));
                list.Add(param1);
                list.Add(param2);
            }

            try
            {
                this.TopMost = false;
                HtmlUtils.LoadHtml(SmartPath.ApplicationPath, list.ToArray());
            }
            catch
            {
            }
            finally
            {
            }
        }

        private class CalibrationModeControls
        {
            public Panel DivisionLineMode;
            public bool IsSelected;
        }
    }
}