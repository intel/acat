////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// WebcamGestureSelectUserControl.cs
//
// UserControl that enables the user to set gesture sensitivity and hold
// time parameters
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PanelManagement;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.Actuators.CameraActuator
{
    internal partial class WebcamGestureSettingsUserControl : UserControl
    {
        private const String ctHoldTimeHelp = "Set the value to the approximate time you tend to hold the gesture";
        private const String ctSensitivityHelp = "Lower values will increase camera sensitivity to your cheek twitch​";
        private const String erHoldTimeHelp = "Set the value to the approximate time you tend to hold the gesture";
        private const String erSensitivityHelp = "Lower values will increase camera sensitivity to your eyebrow raise​";
        private const String head = "Head";
        private const String headSensitivityHelp = "Lower values increase camera sensitivity to head movements causing jitter in face regions.";
        private CameraActuator _cameraActuator;
        private int _ctIndex;
        private int _erIndex;
        private int _headIndex;
        private volatile bool _holdTimeChanged = false;
        private int _holdTimeInitialValue;
        private int _prevDropdownGestureSelectIndex;
        private volatile bool _sensitivityChanged = false;
        private int _sensitivityInitialValue;
        private List<String> _switches;
        private Color buttonApplyBackColor;
        private Tuple<int, int> ctHoldTimeRange = Tuple.Create<int, int>(0, 1000);
        private Tuple<int, int> ctSensitivityRange = Tuple.Create<int, int>(5, 50);
        private Tuple<int, int> erHoldTimeRange = Tuple.Create<int, int>(0, 2000);
        private Tuple<int, int> erSensitivityRange = Tuple.Create<int, int>(5, 50);
        private Tuple<int, int> headSensitivityRange = Tuple.Create<int, int>(20, 100);

        internal WebcamGestureSettingsUserControl(CameraActuator cameraActuator)
        {
            InitializeComponent();
            buttonApplyBackColor = buttonApplyChanges.BackColor;
            _cameraActuator = cameraActuator;
            dropdownGestureSelect.SelectedIndexChanged += DropdownGestureSelect_SelectedIndexChanged;
            numericUpDownSensitivity.ValueChanged += NumericUpDownSensitivity_ValueChanged;
            numericUpDownHoldTime.ValueChanged += NumericUpDownHoldTime_ValueChanged;
            numericUpDownHoldTime.Increment = 25;
        }

        public delegate void PauseDelegate();

        public delegate void ResumeDelegate();

        public event PauseDelegate EvtPause;

        public event ResumeDelegate EvtResume;

        public bool PendingChanges
        {
            get
            {
                return _sensitivityChanged || _holdTimeChanged;
            }
        }

        public bool Initialize(List<String> switches)
        {
            UnInit();

            _switches = switches;
            buttonApplyChanges.EnabledChanged += ButtonApplyChanges_EnabledChanged;
            buttonApplyChanges.Enabled = false;

            return true;
        }

        public void OnClose()
        {
            if (PendingChanges)
            {
                if (showSaveMessageBox())
                {
                    saveAndSendParameters(dropdownGestureSelect.SelectedIndex);
                }
            }
        }

        public void Shown()
        {
            initializeGestureList();
        }

        public void UnInit()
        {
            buttonApplyChanges.EnabledChanged -= ButtonApplyChanges_EnabledChanged;
        }

        private void buttonApplyChanges_Click(object sender, EventArgs e)
        {
            saveAndSendParameters(dropdownGestureSelect.SelectedIndex);

            buttonApplyChanges.Enabled = false;

            _holdTimeChanged = false;
            _sensitivityChanged = false;

            if (dropdownGestureSelect.SelectedIndex == _headIndex)
            {
                _sensitivityInitialValue = CameraActuator.CameraActuatorSettings.HeadMovementSensitivity;
            }
            else if (dropdownGestureSelect.SelectedIndex == _ctIndex)
            {
                _sensitivityInitialValue = CameraActuator.CameraActuatorSettings.CheekTwitchSensitivity;
                _holdTimeInitialValue = CameraActuator.CameraActuatorSettings.CheekTwitchHoldTime;
            }
            else if (dropdownGestureSelect.SelectedIndex == _erIndex)
            {
                _sensitivityInitialValue = CameraActuator.CameraActuatorSettings.EyebrowRaiseSensitivity;
                _holdTimeInitialValue = CameraActuator.CameraActuatorSettings.EyebrowRaiseHoldTime;
            }
        }

        private void ButtonApplyChanges_EnabledChanged(object sender, EventArgs e)
        {
            buttonApplyChanges.BackColor = buttonApplyChanges.Enabled ? buttonApplyBackColor : Color.Gray;
        }

        private void buttonDefaults_Click(object sender, EventArgs e)
        {
            setNumericUpDownValues(dropdownGestureSelect.SelectedIndex, new Settings());
            _holdTimeChanged = true;
            _sensitivityChanged = true;
            setApplyButtonState();
        }

        private void DropdownGestureSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (PendingChanges)
            {
                if (showSaveMessageBox())
                {
                    saveAndSendParameters(_prevDropdownGestureSelectIndex);
                }
            }

            _prevDropdownGestureSelectIndex = dropdownGestureSelect.SelectedIndex;
            setNumericUpDownValues(dropdownGestureSelect.SelectedIndex, CameraActuator.CameraActuatorSettings);
        }

        private String formatRange(Tuple<int, int> range)
        {
            return String.Format("Range: " + range.Item1 + " to " + range.Item2);
        }

        private String getSwitchName(String sw)
        {
            foreach (var actuatorSwitch in _cameraActuator.Switches)
            {
                if (String.Compare(actuatorSwitch.Source, sw, true) == 0)
                {
                    return actuatorSwitch.Name;
                }
            }
            return String.Empty;
        }

        private void initializeGestureList()
        {
            dropdownGestureSelect.Items.Clear();

            _ctIndex = -1;
            _erIndex = -1;
            _headIndex = -1;

            foreach (var sw in _switches)
            {
                if (String.Compare(sw, "CT", true) == 0)
                {
                    _ctIndex = dropdownGestureSelect.Items.Add(getSwitchName(sw));
                }
                else if (String.Compare(sw, "ER", true) == 0)
                {
                    _erIndex = dropdownGestureSelect.Items.Add(getSwitchName(sw));
                }
            }

            _headIndex = dropdownGestureSelect.Items.Add(head);

            dropdownGestureSelect.SelectedIndex = 0;
            _prevDropdownGestureSelectIndex = dropdownGestureSelect.SelectedIndex;
        }

        private void NumericUpDownHoldTime_ValueChanged(object sender, EventArgs e)
        {
            _holdTimeChanged = (int)numericUpDownHoldTime.Value != _holdTimeInitialValue;

            setApplyButtonState();
        }

        private void NumericUpDownSensitivity_ValueChanged(object sender, EventArgs e)
        {
            _sensitivityChanged = (int)numericUpDownSensitivity.Value != _sensitivityInitialValue;

            setApplyButtonState();
        }

        private void saveAndSendParameters(int dropdownIndex)
        {
            if (dropdownIndex == _headIndex)
            {
                CameraActuator.CameraActuatorSettings.HeadMovementSensitivity = (int)numericUpDownSensitivity.Value;
            }
            else if (dropdownIndex == _ctIndex)
            {
                CameraActuator.CameraActuatorSettings.CheekTwitchSensitivity = (int)numericUpDownSensitivity.Value;
                CameraActuator.CameraActuatorSettings.CheekTwitchHoldTime = (int)numericUpDownHoldTime.Value;
            }
            else if (dropdownIndex == _erIndex)
            {
                CameraActuator.CameraActuatorSettings.EyebrowRaiseSensitivity = (int)numericUpDownSensitivity.Value;
                CameraActuator.CameraActuatorSettings.EyebrowRaiseHoldTime = (int)numericUpDownHoldTime.Value;
            }

            _cameraActuator.setVisionSettings();
            _sensitivityChanged = false;
            _holdTimeChanged = false;
        }

        private void setApplyButtonState()
        {
            buttonApplyChanges.Enabled = _sensitivityChanged || _holdTimeChanged;
        }

        private void setCheekTwitchValues(Settings settings)
        {
            setHoldTimeVisibility(true);

            labelSensitivityHelpText.Text = ctSensitivityHelp;
            labelSensitivityRange.Text = formatRange(ctSensitivityRange);
            _sensitivityInitialValue = settings.CheekTwitchSensitivity;
            _holdTimeInitialValue = settings.CheekTwitchHoldTime;
            numericUpDownSensitivity.Minimum = ctSensitivityRange.Item1;
            numericUpDownSensitivity.Maximum = ctSensitivityRange.Item2;
            numericUpDownSensitivity.Value = _sensitivityInitialValue;

            labelHoldTimeHelpText.Text = ctHoldTimeHelp;
            labelHoldTimeRange.Text = formatRange(ctHoldTimeRange);
            _holdTimeInitialValue = settings.CheekTwitchHoldTime;
            numericUpDownHoldTime.Minimum = ctHoldTimeRange.Item1;
            numericUpDownHoldTime.Maximum = ctHoldTimeRange.Item2;
            numericUpDownHoldTime.Value = _holdTimeInitialValue;
        }

        private void setEyebrowRaiseValues(Settings settings)
        {
            setHoldTimeVisibility(true);

            labelSensitivityHelpText.Text = erSensitivityHelp;
            labelSensitivityRange.Text = formatRange(erSensitivityRange);
            _sensitivityInitialValue = settings.EyebrowRaiseSensitivity;
            _holdTimeInitialValue = settings.EyebrowRaiseHoldTime;
            numericUpDownSensitivity.Minimum = erSensitivityRange.Item1;
            numericUpDownSensitivity.Maximum = erSensitivityRange.Item2;
            numericUpDownSensitivity.Value = _sensitivityInitialValue;

            labelHoldTimeHelpText.Text = erHoldTimeHelp;
            labelHoldTimeRange.Text = formatRange(erHoldTimeRange);
            _holdTimeInitialValue = settings.EyebrowRaiseHoldTime;
            numericUpDownHoldTime.Minimum = erHoldTimeRange.Item1;
            numericUpDownHoldTime.Maximum = erHoldTimeRange.Item2;
            numericUpDownHoldTime.Value = _holdTimeInitialValue;
        }

        private void setHeadValues(Settings settings)
        {
            setHoldTimeVisibility(false);

            labelSensitivityHelpText.Text = headSensitivityHelp;
            _sensitivityInitialValue = settings.HeadMovementSensitivity;
            numericUpDownSensitivity.Minimum = headSensitivityRange.Item1;
            numericUpDownSensitivity.Maximum = headSensitivityRange.Item2;
            numericUpDownSensitivity.Value = _sensitivityInitialValue;
            labelSensitivityRange.Text = formatRange(headSensitivityRange);
        }

        private void setHoldTimeVisibility(bool visible)
        {
            labelHoldTimeCaption.Visible = visible;
            labelHoldTimeHelpText.Visible = visible;
            numericUpDownHoldTime.Visible = visible;
            labelHoldTimeRange.Visible = visible;
        }

        private void setNumericUpDownValues(int index, Settings settings)
        {
            if (index == _headIndex)
            {
                setHeadValues(settings);
            }
            else if (index == _ctIndex)
            {
                setCheekTwitchValues(settings);
            }
            else if (index == _erIndex)
            {
                setEyebrowRaiseValues(settings);
            }

            _holdTimeChanged = false;
            _sensitivityChanged = false;
        }

        private bool showSaveMessageBox()
        {
            EvtPause?.Invoke();
            var msgBox = new ConfirmBoxTwoOptions
            {
                Prompt = "Save gesture parameters?",
                Op1Prompt = "Yes",
                Op2Prompt = "No"
            };

            msgBox.ShowDialog(this);

            var result = msgBox.OptionsResult;

            msgBox.Dispose();

            EvtResume?.Invoke();

            return result == ConfirmBoxTwoOptions.Options.Option1;
        }
    }
}