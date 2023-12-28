////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.AnimationManagement;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.UserControlManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ACAT.Lib.Extension
{
    /// <summary>
    /// The tryout app that allows the user to practice switch scanning and
    /// also set a suitable scanning speed
    /// </summary>
    [DescriptorAttribute("61E8A29A-5076-4047-A9F5-89E7E4903407",
                        "UserControlDefaultTryout",
                    "User Control to adjust the scan timing")]
    public partial class UserControlDefaultTryout : UserControl, IUserControl
    {
        private int _currentWordIndex = 0;
        private int _index = 0;
        private UserControlKeyboardCommon _keyboardCommon;

        private readonly int _prevScanTime = CoreGlobals.AppPreferences.ScanTime;

        private readonly List<String> _words = new List<string>();

        public UserControlDefaultTryout()
        {
            InitializeComponent();

            _words.Add("tab");
            _words.Add("eat");
            _words.Add("tea");
            _words.Add("ate");
            _words.Add("bet");
            _words.Add("bat");

            // beta
        }

        public event AnimationPlayerStateChanged EvtPlayerStateChanged;

        /// <summary>
        /// Gets the descriptor for this class
        /// </summary>
        public IDescriptor Descriptor
        {
            get { return DescriptorAttribute.GetDescriptor(GetType()); }
        }

        /// <summary>
        /// Gets the snchronization object
        /// </summary>
        public SyncLock SyncObj
        {
            get { return _keyboardCommon.SyncObj; }
        }

        public IUserControlCommon UserControlCommon
        {
            get
            {
                return _keyboardCommon;
            }
        }

        public bool Initialize(UserControlConfigMapEntry mapEntry, TextController textController, IScannerPanel scanner)
        {
            _keyboardCommon = new UserControlKeyboardCommon(this, mapEntry, textController, scanner);

            bool retVal = _keyboardCommon.Initialize();

            _keyboardCommon.AnimationManager.EvtPlayerStateChanged += AnimationManager_EvtPlayerStateChanged;

            richTextBox.AppendText(_words[_currentWordIndex], Color.DimGray);

            customSliderScanningSpeed.ValueChanged += CustomSliderScanningSpeed_ValueChanged;

            customSliderScanningSpeed.MouseUp += CustomSliderScanningSpeed_MouseUp;
            customSliderScanningSpeed.Minimum = 1;
            customSliderScanningSpeed.Maximum = 100;
            customSliderScanningSpeed.Value = (int)(Common.AppPreferences.ScanTime / 100);

            checkBoxDontShowThisOnStartup.Checked = false;

            checkBoxDontShowThisOnStartup.CheckStateChanged += CheckBoxDontShowThisOnStartup_CheckStateChanged;

            return retVal;
        }

        public void OnLoad()
        {
            _keyboardCommon.OnLoad();
            _keyboardCommon.AnimationManager.OnLoad(_keyboardCommon.RootWidget);
        }

        public void OnPause()
        {
            _keyboardCommon.OnPause();
        }

        public void OnResume()
        {
            _keyboardCommon.OnResume();
        }

        public void OnWidgetActuated(WidgetActuatedEventArgs e, ref bool handled)
        {
            var wordToType = _words[_currentWordIndex];

            if (_index < wordToType.Length)
            {
                if (Char.ToLower(e.SourceWidget.Value[0]) == Char.ToLower(wordToType[_index]))
                {
                    Invoke(new MethodInvoker(delegate
                    {
                        _index++;
                        updateText();

                        if (_index == wordToType.Length)
                        {
                            (_keyboardCommon.ScannerForm as IScannerPanel).OnPause();

                            var toastForm = new ToastForm2("GREAT JOB!", "Continue practicing \nor hit \"Next\" to continue");
                            Windows.SetWindowPosition(toastForm, Windows.WindowPosition.CenterScreen);
                            toastForm.ShowDialog(this);

                            toastForm.Dispose();

                            _currentWordIndex++;
                            if (_currentWordIndex >= _words.Count)
                            {
                                _currentWordIndex = 0;
                            }
                            _index = 0;
                            richTextBox.Text = String.Empty;
                            richTextBox.AppendText(_words[_currentWordIndex], Color.DimGray);
                            (_keyboardCommon.ScannerForm as IScannerPanel).OnResume();
                            return;
                        }
                    }));
                }
            }

            handled = true;
        }

        private void AnimationManager_EvtPlayerStateChanged(object sender, PlayerStateChangedEventArgs e)
        {
            EvtPlayerStateChanged?.Invoke(this, e);
        }

        private void buttonDone_Click(object sender, EventArgs e)
        {
            if (CoreGlobals.AppPreferences.ScanTime != _prevScanTime)
            {
                
                if (!DialogUtils.ConfirmScanner(null, "Save scan speed?"))
                {
                    CoreGlobals.AppPreferences.ScanTime = _prevScanTime;
                }
            }

            if (checkBoxDontShowThisOnStartup.Checked)
            {
                ConfirmBoxSingleOption.ShowDialog("Refer to the ACAT User Guide on how to re-enable this screen", "OK", _keyboardCommon.ScannerForm);
            }

            CoreGlobals.AppPreferences.Save();

            _keyboardCommon.ScannerForm.Close();
        }

        private void changeScanningSpeed()
        {
            Invoke(new MethodInvoker(delegate
            {
                (_keyboardCommon.ScannerForm as IScannerPanel).OnPause();

                decimal sliderVal = customSliderScanningSpeed.Value;
                int sliderValRounded = (int)(sliderVal * 100);

                Common.AppPreferences.ScanTime = sliderValRounded;

                Common.AppPreferences.NotifyPreferencesChanged();

                (_keyboardCommon.ScannerForm as IScannerPanel).OnResume();
            }));
        }

        private void CheckBoxDontShowThisOnStartup_CheckStateChanged(object sender, EventArgs e)
        {
            Common.AppPreferences.ShowSwitchTryoutOnStartup = !checkBoxDontShowThisOnStartup.Checked;
        }

        private void confirmSaveAndClose()
        {
            Invoke(new MethodInvoker(delegate
            {
                if (!DialogUtils.ConfirmScanner(null, "Save scan speed?"))
                {
                    CoreGlobals.AppPreferences.ScanTime = _prevScanTime;
                }

                CoreGlobals.AppPreferences.Save();

                _keyboardCommon.ScannerForm.Close();
            }));
        }

        private void customSlider2PositiveLabel_Click(object sender, EventArgs e)
        {
            decimal currentVal = customSliderScanningSpeed.Value;
            decimal newVal = (decimal)((float)(customSliderScanningSpeed.Value * 100) + 50) / 100;
            customSliderScanningSpeed.Value = (newVal) <= customSliderScanningSpeed.Maximum ? newVal : customSliderScanningSpeed.Maximum;

            if (currentVal != customSliderScanningSpeed.Value)
            {
                changeScanningSpeed();
            }
        }

        private void customSliderNegativeLabel_Click(object sender, EventArgs e)
        {
            decimal currentVal = customSliderScanningSpeed.Value;
            decimal newVal = (decimal)((float)(customSliderScanningSpeed.Value * 100) - 50) / 100;
            customSliderScanningSpeed.Value = (newVal) >= customSliderScanningSpeed.Minimum ? newVal : customSliderScanningSpeed.Minimum;
            if (currentVal != customSliderScanningSpeed.Value)
            {
                changeScanningSpeed();
            }
        }

        private void CustomSliderScanningSpeed_MouseUp(object sender, MouseEventArgs e)
        {
            changeScanningSpeed();
        }

        private void CustomSliderScanningSpeed_ValueChanged(object sender, EventArgs e)
        {
            decimal sliderVal = customSliderScanningSpeed.Value;
            int sliderValRounded = (int)(sliderVal * 100);

            //Log.Debug("UserControlCustomSlider_EvtValueChanged | sliderVal: " + sliderVal.ToString());

            labelCustomSliderValue.Text = sliderValRounded.ToString() + " ms";
        }

        private void updateText()
        {
            var wordToType = _words[_currentWordIndex];

            richTextBox.Text = String.Empty;

            for (int j = 0; j < _index; j++)
            {
                richTextBox.AppendText(wordToType[j].ToString(), Color.Black);
            }

            for (int j = _index; j < wordToType.Length; j++)
            {
                richTextBox.AppendText(wordToType[j].ToString(), Color.Gray);
            }
        }
    }

    public static class RichTextBoxExtensions
    {
        public static void AppendText(this RichTextBox box, string text, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }
    }
}