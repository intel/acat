////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// UserControlHardwareSwitchSetup.cs
//
// User control used in onboarding that allows the user to configure
// the hotkey for an external off-the-shelf switch
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.ActuatorManagement;
using ACAT.Lib.Core.Onboarding;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using ACAT.ACATResources;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ACAT.Lib.Core.InputActuators
{
    /// <summary>
    /// User control used in onboarding that allows the user to configure
    /// the hotkey for an external off-the-shelf switch
    /// </summary>
    public partial class UserControlHardwareSwitchSetup : UserControl, IOnboardingUserControl
    {
        // TODO - Localize Me
        private const String bodyStyle = " background-color:#232433;";
        // TODO - Localize Me
        private const String headStyle = "a:link{color: rgb(255, 170, 0);}";
        // TODO - Localize Me
        private const String textStyle = "font-family:'Montserrat Medium'; font-size:24px; color:white; text-align:center";
        private Color _buttonDefaultBackColor;
        private Color _buttonDefaultForeColor;
        private Dictionary<String, Button> _buttonMap = new Dictionary<string, Button>();
        private List<String> _functionKeys = new List<String> { "f1", "f2", "f3", "f4", "f5", "f6", "f7", "f8", "f9", "f10", "f11", "f12" };
        // TODO - Localize Me
        private String _htmlTemplate = "<!DOCTYPE html><html><head><style>{0}</style></head><body style=\"{1}\"><p style=\"{2}\">{3}<font></body></html>";
        private IOnboardingExtension _onboardingExtension;
        private String _stepId;
        private String _strTriggerHotkey;
        private OnboardingHardwareSwitchSetup.SwitchType _switchType;
        private bool canceled = false;
        public UserControlHardwareSwitchSetup(IOnboardingWizard wizard, IOnboardingExtension onboardingExtension, String stepId, OnboardingHardwareSwitchSetup.SwitchType switchType)
        {
            InitializeComponent();

            _onboardingExtension = onboardingExtension;
            _stepId = stepId;
            _switchType = switchType;

            initButtonList();

            _buttonDefaultBackColor = buttonF1.BackColor;
            _buttonDefaultForeColor = buttonF1.ForeColor;
        }

        public IOnboardingExtension OnboardingExtension
        {
            get
            {
                return _onboardingExtension;
            }
        }

        public String StepId
        {
            get
            {
                return _stepId;
            }
        }

        public UserControl GetUserControl()
        {
            return this;
        }

        public bool Initialize()
        {
            _strTriggerHotkey = String.Empty;

            var config = Context.AppActuatorManager.GetActuatorConfig();

            IActuator actuator;

            actuator = (_switchType == OnboardingHardwareSwitchSetup.SwitchType.Keyboard) ?
                                        Context.AppActuatorManager.GetKeyboardActuator() :
                                        Context.AppActuatorManager.GetSwitchInterfaceActuator();

            String bookmark = String.Empty;
            if (_switchType == OnboardingHardwareSwitchSetup.SwitchType.Keyboard)
            {
                labelTitle.Text = "Configure Keyboard Hotkey";
                labelPrompt.Text = "ACAT is configured to trigger on the hotkey displayed in green below. " +
                                    "You can change the hotkey by clicking on the buttons to select a different key combination.";
                bookmark = "KeyboardSwitch";
            }
            else
            {
                labelTitle.Text = "Configure Switch";
                labelPrompt.Text = "ACAT is configured to trigger on the hotkey displayed in green below. Your switch should be pre-configured to send this hotkey when it is activated. " +
                                    "Refer to the manual for your switch on how to do this. " +
                                    "Alternatively, you can click on the buttons below to change ACAT's hotkey to " +
                                    "match the hotkey configuration for your switch.";
                bookmark = "SwitchConfigure";
            }

            


            String html = String.Format(_htmlTemplate, headStyle, bodyStyle, textStyle, "Click <a href=" + HtmlUtils.EncodeString(CoreGlobals.ACATUserGuideFileName) + "#" + bookmark +">here</a> for help");
            Log.Debug(html);
                
            webBrowser.DocumentCompleted += WebBrowser_DocumentCompleted;
            webBrowser.DocumentText = html;

            if (actuator != null)
            {
                foreach (var setting in config.ActuatorSettings)
                {
                    if (setting.Id.Equals(actuator.Descriptor.Id))
                    {
                        foreach (var switchSetting in setting.SwitchSettings)
                        {
                            if (String.Compare(switchSetting.Command, "@Trigger", true) == 0)
                            {
                                _strTriggerHotkey = switchSetting.Source;
                            }
                        }
                    }
                }
            }

            Log.Debug("triggerHotKey: " + _strTriggerHotkey);

            _strTriggerHotkey = Regex.Replace(_strTriggerHotkey, "alt", "", RegexOptions.IgnoreCase);
            _strTriggerHotkey = Regex.Replace(_strTriggerHotkey, "shift", "", RegexOptions.IgnoreCase);
            _strTriggerHotkey = Regex.Replace(_strTriggerHotkey, "ctrl", "", RegexOptions.IgnoreCase);
            _strTriggerHotkey = _strTriggerHotkey.Replace("+", "");

            if (!isFunctionKey(_strTriggerHotkey))
            {
                Log.Debug("Invalid triggerHotKey. Reseting to F12");
                _strTriggerHotkey = "F12";
            }

            Log.Debug("After parsing triggerhotkey: " + _strTriggerHotkey);

            initButtons();

            return true;
        }

        public void OnAdded()
        {
        }

        public bool OnPreAdd()
        {
            return true;
        }

        public void OnRemoved()
        {
            if (canceled)
            {
                return;
            }

            var config = Context.AppActuatorManager.GetActuatorConfig();
            IActuator actuator;
            if (_switchType == OnboardingHardwareSwitchSetup.SwitchType.Keyboard)
            {
                actuator = Context.AppActuatorManager.GetKeyboardActuator();
            }
            else
            {
                actuator = Context.AppActuatorManager.GetSwitchInterfaceActuator();
            }

            foreach (var setting in config.ActuatorSettings)
            {
                if (actuator != null && setting.Id.Equals(actuator.Descriptor.Id))
                {
                    foreach (var switchSetting in setting.SwitchSettings)
                    {
                        if (String.Compare(switchSetting.Command, "@Trigger", true) == 0)
                        {
                            switchSetting.Source = _strTriggerHotkey;
                        }
                    }
                }
            }

            config.Save();
        }

        public bool QueryCancelOnboarding()
        {
            canceled = true;
            return true;
        }

        public bool QueryGoToNextStep()
        {
            var isValid = isHotkeyValid();

            Log.Debug("isValidKeyCombo: " + isValid);

            bool retVal;

            if (!isValid)
            {
                ConfirmBoxSingleOption.ShowDialog("Invalid key combination", "OK");
                retVal = false;
            }
            else
            {
                retVal = true;
            }

            return retVal; ;
        }

        public bool QueryGoToPrevStep()
        {
            return true;
        }

        private void button_FunctionKeyClick(object sender, EventArgs e)
        {
            handleFunctionKeyButtonClick(sender);
        }

        private void buttonAlt_Click(object sender, EventArgs e)
        {
            flipButtonTag(sender as Button);
            setKeyComboAndUpdateButtonColors();
        }

        private void buttonCtrl_Click(object sender, EventArgs e)
        {
            flipButtonTag(sender as Button);
            setKeyComboAndUpdateButtonColors();
        }

        private void buttonShift_Click(object sender, EventArgs e)
        {
            flipButtonTag(sender as Button);
            setKeyComboAndUpdateButtonColors();
        }

        private String capitalize(String str)
        {
            if (str.Length == 0)
            {
                return str;
            }

            return str.Length == 1 ? char.ToUpper(str[0]).ToString() : char.ToUpper(str[0]) + str.Substring(1);
        }

        private void clearButtonTags()
        {
            foreach (var button in _buttonMap.Values)
            {
                button.Tag = false;
            }
        }

        private void flipButtonTag(Button button)
        {
            bool? flag = button.Tag as bool?;

            if (flag != null)
            {
                button.Tag = !flag;
            }
        }

        private void handleFunctionKeyButtonClick(object sender)
        {
            resetFunctionKeysState();
            flipButtonTag(sender as Button);
            setKeyComboAndUpdateButtonColors();
        }

        private void initButtonList()
        {
            //_buttonMap.Add("ctrl", buttonCtrl);
            //_buttonMap.Add("alt", buttonAlt);
            //_buttonMap.Add("shift", buttonShift);
            _buttonMap.Add("f1", buttonF1);
            _buttonMap.Add("f2", buttonF2);
            _buttonMap.Add("f3", buttonF3);
            _buttonMap.Add("f4", buttonF4);
            _buttonMap.Add("f5", buttonF5);
            _buttonMap.Add("f6", buttonF6);
            _buttonMap.Add("f7", buttonF7);
            _buttonMap.Add("f8", buttonF8);
            _buttonMap.Add("f9", buttonF9);
            _buttonMap.Add("f10", buttonF10);
            _buttonMap.Add("f11", buttonF11);
            _buttonMap.Add("f12", buttonF12);

            clearButtonTags();
        }

        private void initButtons()
        {
            clearButtonTags();

            var str = _strTriggerHotkey.ToLower();

            var keys = str.Split('+');

            foreach (var key in keys)
            {
                if (_buttonMap.ContainsKey(key))
                {
                    _buttonMap[key].Tag = true;
                }
            }

            setButtonColors();
        }

        private bool isFunctionKey(String key)
        {
            return _functionKeys.Any(key.ToLower().Contains);
        }

        private bool isHotkeyValid()
        {
            if (String.IsNullOrEmpty(_strTriggerHotkey))
            {
                return false;
            }

            var str = _strTriggerHotkey.ToLower();

            foreach (var fkey in _functionKeys)
            {
                if (str.EndsWith(fkey))
                {
                    return true;
                }
            }

            return false;
        }
        private void resetFunctionKeysState()
        {
            foreach (var key in _buttonMap.Keys)
            {
                if (key.StartsWith("f"))
                {
                    _buttonMap[key].Tag = false;
                }
            }
        }

        private void setButtonColors()
        {
            foreach (var button in _buttonMap.Values)
            {
                bool? flag = button.Tag as bool?;
                button.BackColor = (flag == true) ? Color.Green : _buttonDefaultBackColor;
                button.ForeColor = (flag == true) ? Color.White : _buttonDefaultForeColor;
            }
        }

        private void setKeyComboAndUpdateButtonColors()
        {
            _strTriggerHotkey = String.Empty;

            foreach (var key in _buttonMap.Keys)
            {
                var button = _buttonMap[key];

                if ((button.Tag as bool?) == true)
                {
                    _strTriggerHotkey += capitalize(key) + "+";
                }
            }

            if (!String.IsNullOrEmpty(_strTriggerHotkey))
            {
                if (_strTriggerHotkey[_strTriggerHotkey.Length - 1] == '+')
                {
                    _strTriggerHotkey = _strTriggerHotkey.Remove(_strTriggerHotkey.Length - 1, 1);
                }
            }

            setButtonColors();
        }

        private void WebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowser.Navigating -= webBrowser_Navigating;
            webBrowser.Navigating += webBrowser_Navigating;
        }

        private void webBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            var str = e.Url.ToString();

            Log.Debug("Url is [" + str + "]");

            if (str.ToLower().Contains("blank"))
            {
                return;
            }

            e.Cancel = true;

            String bookmark = String.Empty;

            if (str.Contains("about:"))
            {
                var index = str.IndexOf(':');

                str = str.Substring(index + 1);

                index = str.IndexOf('#');

                if (index > 0)
                {
                    bookmark = str.Substring(index + 1, str.Length - index - 1);
                }
            }

            List<String> list = new List<String>
            {
                "PDF",
                "true",
                R.GetString("PDFLoaderHtml"),
                CoreGlobals.ACATUserGuideFileName,
                bookmark
            };

            try
            {
                HtmlUtils.LoadHtml(SmartPath.ApplicationPath, list.ToArray());
            }
            catch
            {

            }
            finally
            {

            }
        }

    }
}