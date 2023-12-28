////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.ActuatorManagement;
using ACAT.Lib.Core.AnimationManagement;
using ACAT.Lib.Core.InputActuators;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.UserControlManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ACAT.Lib.Extension
{
    /// <summary>
    /// Displays a code that the user must type to unlock the screen
    /// </summary>
    [DescriptorAttribute("4D767749-D9C6-450E-A1D6-169074F2F66A",
                    "UserControlScreenLock",
                    "User Control to lock the screen")]
    public partial class UserControlScreenLock : UserControl, IUserControl
    {
        private int _index = 0;
        private KeyboardActuator _keyboardActuator;
        private UserControlKeyboardCommon _keyboardCommon;

        private String _pin = Common.AppPreferences.ScreenLockPin;

        public UserControlScreenLock()
        {
            InitializeComponent();
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

            var actuator = ActuatorManager.Instance.GetActuator(typeof(KeyboardActuator));
            if (actuator is KeyboardActuator)
            {
                _keyboardActuator = actuator as KeyboardActuator;
                _keyboardActuator.EvtKeyPress += _keyboardActuator_EvtKeyPress;
            }

            if (!validatePin(_pin))
            {
                _pin = randomPin();
            }

            richTextBox.AppendText(_pin, Color.DimGray);

            scanner.Form.FormClosing += Form_FormClosing;

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

        private void AnimationManager_EvtPlayerStateChanged(object sender, PlayerStateChangedEventArgs e)
        {
            EvtPlayerStateChanged?.Invoke(this, e);
        }

        private void buttonDone_Click(object sender, EventArgs e)
        {
            _keyboardCommon.ScannerForm.Close();
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            _keyboardActuator.EvtKeyPress -= _keyboardActuator_EvtKeyPress;
            if (_keyboardCommon.AnimationManager != null)
            {
                _keyboardCommon.AnimationManager.EvtPlayerStateChanged -= AnimationManager_EvtPlayerStateChanged;
            }
        }

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
                            _keyboardCommon.ScannerForm.Close();
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