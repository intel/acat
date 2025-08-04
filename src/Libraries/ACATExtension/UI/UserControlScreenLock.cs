////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.ActuatorManagement;
using ACAT.Core.AnimationManagement;
using ACAT.Core.InputActuators;
using ACAT.Core.PanelManagement;
using ACAT.Core.UserControlManagement;
using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using ACAT.UserControls;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ACAT.Extension
{
    /// <summary>
    /// Displays a code that the user must type to unlock the screen
    /// </summary>
    [ClassDescriptor("4D767749-D9C6-450E-A1D6-169074F2F66A",
                    "UserControlScreenLock",
                    "User Control to lock the screen")]
    public partial class UserControlScreenLock : KeyboardUserControl
    {
        private int _index = 0;
        private String _pin = Common.AppPreferences.ScreenLockPin;

        public UserControlScreenLock()
        {
            InitializeComponent();
        }

        protected override bool HandleInitialize()
        {
            if (!validatePin(_pin))
            {
                _pin = randomPin();
            }

            richTextBox.AppendText(_pin, Color.DimGray);
            return true;
        }

        public override void OnWidgetActuated(WidgetActuatedEventArgs e, ref bool handled)
        {
            handleKeyPress(e.SourceWidget.Value[0]);

            handled = true;
        }

        private void _keyboardActuator_EvtKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar >= '1' && e.KeyChar <= '5')
            {
                handleKeyPress(e.KeyChar);
            }
        }

        private void buttonDone_Click(object sender, EventArgs e)
        {
            _userControlCommon.ScannerForm.Close();
        }

        //private void Form_FormClosing(object sender, FormClosingEventArgs e)
        //{
        //    _keyboardActuator.EvtKeyPress -= _keyboardActuator_EvtKeyPress;
        //    if (_keyboardCommon.AnimationManager != null)
        //    {
        //        _keyboardCommon.AnimationManager.EvtPlayerStateChanged -= AnimationManager_EvtPlayerStateChanged;
        //    }
        //}

        private void handleKeyPress(char key)
        {
            if (_index < _pin.Length)
            {
                Invoke(new MethodInvoker(delegate
                {
                    if (key == _pin[_index])
                    {
                        _index++;
                        updateText();

                        if (_index == _pin.Length)
                        {
                            _userControlCommon.ScannerForm.Close();
                        }
                    }
                    else
                    {
                        _index = 0;
                        richTextBox.Text = String.Empty;
                        richTextBox.AppendText(_pin, Color.DimGray);
                    }
                }));
            }
        }

        private String randomPin()
        {
            String pin = String.Empty;
            var rand = new Random();
            for (int i = 0; i < 4; i++)
            {
                while (true)
                {
                    var next = rand.Next(1, 6).ToString();
                    if (!pin.Contains(next))
                    {
                        pin += next;
                        break;
                    }
                }
            }

            return pin;
        }

        private void updateText()
        {
            var wordToType = _pin;

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

        private bool validatePin(String pin)
        {
            if (String.IsNullOrEmpty(pin) || pin.Length < 3)
            {
                return false;
            }

            foreach (char c in pin)
            {
                if (c < '1' || c > '5')
                {
                    return false;
                }
            }

            return true;
        }
    }
}