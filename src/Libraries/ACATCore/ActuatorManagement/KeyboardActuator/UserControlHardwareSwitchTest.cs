////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// UserControlHardwareSwitchTest.cs
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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACAT.Lib.Core.InputActuators
{
    /// <summary>
    /// User control used in onboarding that allows the user to configure
    /// the hotkey for an external off-the-shelf switch
    /// </summary>
    public partial class UserControlHardwareSwitchTest : UserControl, IOnboardingUserControl
    {
        private const String bodyStyle = " background-color:#232433;";
        private const String headStyle = "a:link{color: rgb(255, 170, 0);}";
        private const int switchActivationDelay = 300;
        private const String textStyle = "font-family:'Montserrat Medium'; font-size:24px; color:white; text-align:center";
        private int _acceptTime = 0;
        private Stopwatch _acceptTimer;
        private Image _buttonActuatedImage;
        private Image _buttonDefaultImage;
        private bool _hotkeyActive = false;
        private String _htmlTemplate = "<!DOCTYPE html><html><head><style>{0}</style></head><body style=\"{1}\"><p style=\"{2}\">{3}<font></body></html>";
        private KeyboardHook _keyboardHook;
        private IOnboardingExtension _onboardingExtension;
        private String _stepId;
        private String _strTriggerHotkey;
        private bool _switchTested = false;
        private OnboardingHardwareSwitchSetup.SwitchType _switchType;

        const int minHoldTimeMaxValue = 2000;

        public UserControlHardwareSwitchTest(IOnboardingWizard wizard, IOnboardingExtension onboardingExtension, String stepId, OnboardingHardwareSwitchSetup.SwitchType switchType)
        {
            InitializeComponent();

            _onboardingExtension = onboardingExtension;
            _stepId = stepId;
            _switchType = switchType;
            _acceptTimer = new Stopwatch();

            _buttonDefaultImage = ImageUtils.LoadImage(Path.Combine(FileUtils.GetImagePath("SwitchTestDefault.png")));
            _buttonActuatedImage = ImageUtils.LoadImage(Path.Combine(FileUtils.GetImagePath("SwitchTestActivated.png")));

            buttonSwitchTest.BackgroundImage = _buttonDefaultImage;

            _keyboardHook = new KeyboardHook();
            _keyboardHook.EvtKeyDown += UserControlHardwareSwitchSetup_EvtKeyDown;
            _keyboardHook.EvtKeyUp += UserControlHardwareSwitchSetup_EvtKeyUp;


            ((Control)numericUpDownHoldTime).TextChanged += numericUpDownHoldTime_textChanged;
            numericUpDownHoldTime.ValueChanged += NumericUpDownHoldTime_ValueChanged;
            numericUpDownHoldTime.Maximum = minHoldTimeMaxValue;
            numericUpDownHoldTime.Minimum = 0;
            numericUpDownHoldTime.Increment = 50;
            
            numericUpDownHoldTime.Value = CoreGlobals.AppPreferences.MinActuationHoldTime;

            _keyboardHook.SetHook();
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

        public void handleKeyDownEvent(object sender, KeyEventArgs e)
        {
            KeyStateTracker.KeyDown(e.KeyCode);

            string hotKey = KeyStateTracker.GetKeyPressedStatus();
            if (String.IsNullOrEmpty(hotKey))
            {
                hotKey = e.KeyCode.ToString();
            }
            else
            {
                hotKey += "+" + e.KeyCode;
            }

            Log.Debug("Keyudown Hotkey: " + hotKey);

            if (String.Compare(hotKey, _strTriggerHotkey, true) == 0)
            {
                if (!_hotkeyActive)
                {
                    _hotkeyActive = true;
                    _acceptTimer.Restart();
                }
            }
        }

        public void handleKeyUpEvent(object sender, KeyEventArgs e)
        {
            KeyStateTracker.KeyUp(e.KeyCode);

            string hotKey = KeyStateTracker.GetKeyPressedStatus();
            if (String.IsNullOrEmpty(hotKey))
            {
                hotKey = e.KeyCode.ToString();
            }
            else
            {
                hotKey += "+" + e.KeyCode;
            }

            Log.Debug("Keyup hotkey: " + hotKey);


            if (String.Compare(e.KeyCode.ToString(), _strTriggerHotkey, true) == 0)
            {
                _hotkeyActive = false;

                Log.Debug("elapsedmilliseconds " + _acceptTimer.ElapsedMilliseconds);
                if (_acceptTimer.IsRunning && _acceptTimer.ElapsedMilliseconds >= _acceptTime)
                {
                    Log.Debug("Setting _switchTested to true");
                    _switchTested = true;
                    setButtonBackgroundAndText(true, "Switch\nActivated");

                    Task task = Task.Delay(switchActivationDelay).ContinueWith(t => setButtonBackgroundAndText(false));
                }
                else
                {
                    Log.Debug("Key not accepted as elapsed time is < MinActuationHoldTime");
                }

                _acceptTimer.Stop();
            }
        }

        public bool Initialize()
        {
            _strTriggerHotkey = String.Empty;

            var config = Context.AppActuatorManager.GetActuatorConfig();

            IActuator actuator;

            actuator = (_switchType == OnboardingHardwareSwitchSetup.SwitchType.Keyboard) ?
                                        Context.AppActuatorManager.GetKeyboardActuator() :
                                        Context.AppActuatorManager.GetSwitchInterfaceActuator();

            String html = String.Format(_htmlTemplate, headStyle, bodyStyle, textStyle, "Click <a href=" + HtmlUtils.EncodeString(CoreGlobals.ACATUserGuideFileName) + "#SwitchTest>here</a> for help");

            

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
                                break;
                            }
                        }
                    }
                }
            }

            Log.Debug("keyboardActuationCombo: " + _strTriggerHotkey);

            _acceptTime = CoreGlobals.AppPreferences.MinActuationHoldTime;

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
            if (_keyboardHook != null)
            {
                _keyboardHook.RemoveHook();
                _keyboardHook = null;
            }
        }

        public bool QueryCancelOnboarding()
        {
            return true;
        }

        public bool QueryGoToNextStep()
        {
            Log.Debug("querynextstep: swichtested: " + _switchTested);

            bool retVal;

            if (_switchType == OnboardingHardwareSwitchSetup.SwitchType.SwitchInterface && !_switchTested)
            {
                var confirmBox = new ConfirmBox
                {
                    Prompt = "You have not tested the switch by activating it. Proceed to the next step anyway?"
                };
                confirmBox.ShowDialog(this);
                retVal = confirmBox.Result;

                confirmBox.Dispose();
            }
            else
            {
                retVal = true;
            }

            if (retVal)
            {
                CoreGlobals.AppPreferences.MinActuationHoldTime = (int)numericUpDownHoldTime.Value;
                CoreGlobals.AppPreferences.Save();

            }

            return retVal; ;
        }

        public bool QueryGoToPrevStep()
        {
            return true;
        }

        public void UserControlHardwareSwitchSetup_EvtKeyDown(object sender, KeyEventArgs e)
        {
            handleKeyDownEvent(sender, e);
        }

        public void UserControlHardwareSwitchSetup_EvtKeyUp(object sender, KeyEventArgs e)
        {
            handleKeyUpEvent(sender, e);
        }

        private void NumericUpDownHoldTime_ValueChanged(object sender, EventArgs e)
        {
            _acceptTime = (int)numericUpDownHoldTime.Value;
        }

        private void setButtonBackgroundAndText(bool activated, String text = "")
        {
            Invoke(new MethodInvoker(delegate
            {
                buttonSwitchTest.BackgroundImage = (activated) ? _buttonActuatedImage : _buttonDefaultImage;
                buttonSwitchTest.Text = text;
            }));
        }

        private void numericUpDownHoldTime_textChanged(object sender, EventArgs e)
        {
            
            var str = ((Control)numericUpDownHoldTime).Text;
            
            try
            {
                if (String.IsNullOrEmpty(str))
                {
                    _acceptTime = 0;
                }

                _acceptTime = (int)double.Parse(str);

                if (_acceptTime > minHoldTimeMaxValue)
                {
                    _acceptTime = minHoldTimeMaxValue;
                }
            }
            catch
            {

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
