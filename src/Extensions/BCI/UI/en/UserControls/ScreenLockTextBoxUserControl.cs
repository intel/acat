////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ScreenLockTextBoxUserControl.cs
//
// Texbox User control to display the numeric key to unlock the UI.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.ActuatorManagement;
using ACAT.Lib.Core.InputActuators;
using ACAT.Lib.Core.ThemeManagement;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACAT.Extensions.BCI.UI.UserControls
{
    public partial class ScreenLockTextBoxUserControl : UserControl
    {
        int _index = 0;

        private KeyboardActuator _keyboardActuator;

        String _pin;

        // TODO - Localize Me
        private String _prompt = "\r\nPlease enter this pin to unlock: $$$\r\nYou entered: ###";

        public ScreenLockTextBoxUserControl(Form parent, Control container)
        {
            InitializeComponent();
            container.ControlAdded += Container_ControlAdded;
        }

        public event EventHandler EvtScreenUnlocked;
        public void OnPause()
        {
        }

        public void OnResume()
        {
        }

        private void _keyboardActuator_EvtKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar >= '1' && e.KeyChar <= '5')
            {
                updateText();
                handleKeyPress(e.KeyChar);
            }
        }

        private void Container_ControlAdded(object sender, ControlEventArgs e)
        {
            if (e.Control != this)
            {
                return;
            }

            var colorScheme = ThemeManager.Instance.ActiveTheme.Colors.GetColorScheme(ColorSchemes.TalkWindowSchemeName);
            labelPin.BackColor = colorScheme.Background;
            labelPin.ForeColor = colorScheme.Foreground;

            var actuator = ActuatorManager.Instance.GetActuator(typeof(KeyboardActuator));
            if (actuator is KeyboardActuator)
            {
                _keyboardActuator = actuator as KeyboardActuator;
                _keyboardActuator.EvtKeyPress += _keyboardActuator_EvtKeyPress;
            }

            _index = 0;

            _pin = Lib.Extension.Common.AppPreferences.ScreenLockPin;

            if (!validatePin(_pin))
            {
                _pin = randomPin();
            }

            updateText();
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
                            _ = updateUI().ConfigureAwait(false);
                            //_keyboardActuator.EvtKeyPress -= _keyboardActuator_EvtKeyPress;
                            //EvtScreenUnlocked?.Invoke(this, new EventArgs());
                        }

                    }
                    else
                    {
                        _index = 0;
                        updateText();
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
            var wordTyped = String.Empty;

            for (int j = 0; j < _index; j++)
            {
                wordTyped += _pin[j];
            }

            var prompt = _prompt.Replace("$$$", _pin);
            prompt = prompt.Replace("###", wordTyped);
            labelPin.Text = prompt;
        }

        private async Task updateUI()
        {
            await Task.Delay(10);
            updateText();
            await Task.Delay(200);
            _keyboardActuator.EvtKeyPress -= _keyboardActuator_EvtKeyPress;
            EvtScreenUnlocked?.Invoke(this, new EventArgs());
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
