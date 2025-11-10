// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0

using ACAT.Core.PanelManagement;
using ACAT.Core.PanelManagement.Interfaces;
using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using ACAT.Extension.UI;
using ACAT.Extension.UI.UserControls;
using ACATResources;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ACAT.Extension
{
    [ClassDescriptor("61E8A29A-5076-4047-A9F5-89E7E4903407",
                        "UserControlDefaultTryout",
                    "User Control to adjust the scan timing")]
    public partial class UserControlDefaultTryout : KeyboardUserControl
    {
        private int _currentWordIndex = 0;
        private int _index = 0;

        private readonly int _prevScanTime = CoreGlobals.AppPreferences.ScanTime;

        private readonly List<String> _words = new();

        public UserControlDefaultTryout() : base()
        {
            InitializeComponent();

            _words.Add(StringResources.TryoutWord1);
            _words.Add(StringResources.TryoutWord2);
            _words.Add(StringResources.TryoutWord3);
            _words.Add(StringResources.TryoutWord4);
            _words.Add(StringResources.TryoutWord5);
            _words.Add(StringResources.TryoutWord6);
        }

        protected override bool HandleInitialize()
        {
            richTextBox.AppendText(_words[_currentWordIndex], Color.DimGray);

            customSliderScanningSpeed.ValueChanged += CustomSliderScanningSpeed_ValueChanged;

            customSliderScanningSpeed.MouseUp += CustomSliderScanningSpeed_MouseUp;
            customSliderScanningSpeed.Minimum = 1;
            customSliderScanningSpeed.Maximum = 100;
            customSliderScanningSpeed.Value = Common.AppPreferences.ScanTime / 100;

            checkBoxDontShowThisOnStartup.Checked = false;

            checkBoxDontShowThisOnStartup.CheckStateChanged += CheckBoxDontShowThisOnStartup_CheckStateChanged;

            return true;
        }

        public override void OnWidgetActuated(WidgetActuatedEventArgs e, ref bool handled)
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
                            (_userControlCommon.ScannerForm as IScannerPanel).OnPause();

                            var toastForm = new ToastForm2(StringResources.TryoutAlert, StringResources.TryoutSuccess);
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
                            (_userControlCommon.ScannerForm as IScannerPanel).OnResume();
                            return;
                        }
                    }));
                }
            }

            handled = true;
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
                ConfirmBoxOneOption.ShowDialog("Refer to the ACAT User Guide on how to re-enable this screen",
                    "", "OK", _userControlCommon.ScannerForm);
            }

            CoreGlobals.AppPreferences.Save();

            _userControlCommon.ScannerForm.Close();
        }

        private void changeScanningSpeed()
        {
            Invoke(new MethodInvoker(delegate
            {
                (_userControlCommon.ScannerForm as IScannerPanel).OnPause();

                decimal sliderVal = customSliderScanningSpeed.Value;
                int sliderValRounded = (int)(sliderVal * 100);

                Common.AppPreferences.ScanTime = sliderValRounded;

                Common.AppPreferences.NotifyPreferencesChanged();

                (_userControlCommon.ScannerForm as IScannerPanel).OnResume();
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

                _userControlCommon.ScannerForm.Close();
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