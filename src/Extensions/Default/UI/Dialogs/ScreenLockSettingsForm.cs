////////////////////////////////////////////////////////////////////////////
// <copyright file="ScreenLockSettingsForm.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2017 Intel Corporation 
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
////////////////////////////////////////////////////////////////////////////

using ACAT.ACATResources;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using ACAT.Lib.Core.Widgets;
using ACAT.Lib.Extension;
using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.UI.Dialogs
{
    /// <summary>
    /// Dialog that enables the user to set the pin code
    /// for the ACAT mute scanner. User can also set
    /// the number of digits in the pin and the max digit
    /// </summary>
    [DescriptorAttribute("F3774E1D-E15A-4E5F-90B2-1851FAE93D3C",
                        "ScreenLockSettingsForm",
                        "Screen Lock Settings")]
    public partial class ScreenLockSettingsForm : Form, IDialogPanel
    {
        /// <summary>
        /// Maximum length of pin
        /// </summary>
        private const int MaxPinLength = 4;

        /// <summary>
        /// Minimum length of pin
        /// </summary>
        private const int MinPinLength = 2;

        /// <summary>
        /// Dialog common object
        /// </summary>
        private DialogCommon _dialogCommon;

        /// <summary>
        /// Have the settings changed?
        /// </summary>
        private bool _isDirty;

        /// <summary>
        /// Ensures that the window stays focused
        /// </summary>
        private WindowActiveWatchdog _windowActiveWatchdog;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ScreenLockSettingsForm()
        {
            InitializeComponent();

            _windowActiveWatchdog = new WindowActiveWatchdog(this);

            Load += ScreenLockSettingsForm_Load;
            FormClosing += ScreenLockSettingsForm_FormClosing;
        }

        /// <summary>
        /// Gets the descriptor for this class
        /// </summary>
        public IDescriptor Descriptor
        {
            get { return DescriptorAttribute.GetDescriptor(GetType()); }
        }

        /// <summary>
        /// Gets the PanelCommon object
        /// </summary>
        public IPanelCommon PanelCommon { get { return _dialogCommon; } }

        /// <summary>
        /// Returns the synch object
        /// </summary>
        public SyncLock SyncObj
        {
            get { return _dialogCommon.SyncObj; }
        }

        /// <summary>
        /// Sets the form styles
        /// </summary>
        protected override CreateParams CreateParams
        {
            get { return DialogCommon.SetFormStyles(base.CreateParams); }
        }

        /// <summary>
        /// Intitializes the class
        /// </summary>
        /// <param name="startupArg">startup param</param>
        /// <returns>true on success</returns>
        public bool Initialize(StartupArg startupArg)
        {
            _dialogCommon = new DialogCommon(this);

            if (!_dialogCommon.Initialize(startupArg))
            {
                return false;
            }

            initWidgetSettings();

            return true;
        }

        /// <summary>
        /// Triggered when a widget is triggered
        /// </summary>
        /// <param name="widget">Which one triggered?</param>
        public void OnButtonActuated(Widget widget)
        {
            Log.Debug("**Actuate** " + widget.UIControl.Name + " Value: " + widget.Value);

            var value = widget.Value;

            if (String.IsNullOrEmpty(value))
            {
                return;
            }

            Invoke(new MethodInvoker(delegate
            {
                switch (value)
                {
                    case "valButtonOK":
                        onOK();
                        break;

                    case "valButtonCancel":
                        onCancel();
                        break;
                }
            }));
        }

        /// <summary>
        /// Pauses the scanner
        /// </summary>
        public void OnPause()
        {
            _dialogCommon.OnPause();
        }

        /// <summary>
        /// Resumes paused scanner
        /// </summary>
        public void OnResume()
        {
            _dialogCommon.OnResume();
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="command"></param>
        /// <param name="handled"></param>
        public void OnRunCommand(string command, ref bool handled)
        {
            switch (command)
            {
                default:
                    handled = false;
                    break;
            }
        }

        /// <summary>
        /// Window proc
        /// </summary>
        /// <param name="m"></param>
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        protected override void WndProc(ref Message m)
        {
            _dialogCommon.HandleWndProc(m);
            base.WndProc(ref m);
        }

        /// <summary>
        /// Validates the pin entered so far to make sure it is
        /// all digits, is within the length specified
        /// </summary>
        /// <param name="pin">The Pin</param>
        /// <param name="maxDigit">max length of the pin</param>
        /// <param name="errorMessage">error message if not valid</param>
        /// <returns>true if it is a valid pin</returns>
        private Boolean checkPINCode(String pin, char maxDigit, ref String errorMessage)
        {
            int testPin;

            Log.Debug("strPIN=" + pin + "  strMaxDigit=" + maxDigit);

            if (pin.Length == 0)
            {
                errorMessage = R.GetString("PinCannotBeEmpty");
                return false;
            }

            if (!Int32.TryParse(pin, out testPin))
            {
                errorMessage = R.GetString("PinContainsNonNumeric");
                return false;
            }

            // wrong length
            if ((pin.Length < MinPinLength) || (pin.Length > MaxPinLength))
            {
                errorMessage = String.Format(R.GetString("PinHasToBeWithinLimits"), MinPinLength, MaxPinLength);
                return false;
            }

            // pin is out of fange
            for (byte i = 0; i < pin.Length; i++)
            {
                Log.Debug("PIN[i]=" + (pin[i] - '0') + "  maxDigit=" + (maxDigit - '0'));
                if ((pin[i] - '0') > (maxDigit - '0'))
                {
                    errorMessage = R.GetString("PinOutsideRange");
                    return false;
                }
            }

            Log.Debug("pin is valid!");
            return true;
        }

        /// <summary>
        /// Initialize controls on the form based on the settings
        /// (the pin in this case)
        /// </summary>
        private void initWidgetSettings()
        {
            var rootWidget = PanelCommon.RootWidget;

            (rootWidget.Finder.FindChild(tbMaxDigit.Name) as SliderWidget).SetState(Common.AppPreferences.ScreenLockPinMaxDigitValue, SliderWidget.SliderUnitsOnes);

            Windows.SetText(tbPINCode, Common.AppPreferences.ScreenLockPin);
        }

        /// <summary>
        /// Use wants to cancel out
        /// </summary>
        private void onCancel()
        {
            bool cancel = true;

            if (_isDirty)
            {
                if (!DialogUtils.Confirm(this, R.GetString("ChangesNotSavedQuit")))
                {
                    cancel = false;
                }
            }

            if (cancel)
            {
                Windows.CloseForm(this);
            }
        }

        /// <summary>
        /// User pressed OK.  Check the pin is valid, and quit
        /// </summary>
        private void onOK()
        {
            var pin = Windows.GetText(tbPINCode).Trim();

            var errorMessage = String.Empty;
            var hasValidPin = checkPINCode(pin, Windows.GetText(svalMaxDigit)[0], ref errorMessage);

            if (hasValidPin == false)
            {
                showTimedDialog(errorMessage);
                return;
            }

            var str = R.GetString("NewPinSaveChanges").Replace("\\n", Environment.NewLine);
            var prompt = String.Format(str, pin);

            if (DialogUtils.Confirm(this, prompt))
            {
                var prefs = updateSettingsFromUI();
                if (prefs != null)
                {
                    prefs.Save();
                }

                _isDirty = false;
                Common.AppPreferences.NotifyPreferencesChanged();
            }

            Windows.CloseForm(this);
        }

        /// <summary>
        /// Populate form controls text from Language resource
        /// </summary>
        private void populateFormText()
        {
            panelTitle.Text = R.GetString(panelTitle.Text);
        }

        /// <summary>
        /// Form is closing. Release resources
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void ScreenLockSettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _windowActiveWatchdog.Dispose();

            _dialogCommon.OnClosing();
        }

        /// <summary>
        /// Form has been loaded. Initialize
        /// </summary>
        private void ScreenLockSettingsForm_Load(object sender, EventArgs e)
        {
            populateFormText();

            tbPINCode.TextChanged += tbPINCode_TextChanged;
            tbPINCode.KeyPress += textBoxPinCode_KeyPress;

            _dialogCommon.OnLoad();

            Invoke(new MethodInvoker(delegate
            {
                _windowActiveWatchdog = new WindowActiveWatchdog(this);
            }));

            subscribeToEvents();

            PanelCommon.AnimationManager.Start(PanelCommon.RootWidget);
        }

        /// <summary>
        /// Displays a timed dialog with the title and message
        /// </summary>
        /// <param name="message">message</param>
        private void showTimedDialog(String message)
        {
            _windowActiveWatchdog.Pause();
            DialogUtils.ShowTimedDialog(this, message);
            _windowActiveWatchdog.Resume();
        }

        /// <summary>
        /// Subscribes to all the events triggered by the
        /// widgets and the interpreter
        /// </summary>
        private void subscribeToEvents()
        {
            var widgetList = new List<Widget>();
            PanelCommon.RootWidget.Finder.FindAllButtons(widgetList);

            foreach (var widget in widgetList)
            {
                widget.EvtValueChanged += widget_EvtValueChanged;
            }

            widgetList.Clear();
            PanelCommon.RootWidget.Finder.FindAllChildren(typeof(SliderWidget), widgetList);
            foreach (var widget in widgetList)
            {
                widget.EvtValueChanged += widget_EvtValueChanged;
            }
        }

        /// <summary>
        /// User entered something. Set the dirty flag
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void tbPINCode_TextChanged(object sender, EventArgs e)
        {
            _isDirty = true;
        }

        /// <summary>
        /// Key press event handler
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void textBoxPinCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Updates the preferences based on values from
        /// the form (the pincode in this case). Returns a
        /// new Preferences object
        /// </summary>
        /// <returns>ACATPreferences object</returns>
        private ACATPreferences updateSettingsFromUI()
        {
            var pin = Windows.GetText(tbPINCode);
            Int32 intPinCode;

            Log.Debug("strPINCode=" + pin);

            ACATPreferences prefs = null;

            if (Int32.TryParse(pin, out intPinCode) == false)
            {
                // fail because it is not a number
                Log.Debug("Invalid pincode provided!");
            }
            else
            {
                prefs = ACATPreferences.Load();
                prefs.ScreenLockPin = Common.AppPreferences.ScreenLockPin = pin;
                prefs.ScreenLockPinMaxDigitValue = Common.AppPreferences.ScreenLockPinMaxDigitValue = (byte)(PanelCommon.RootWidget.Finder.FindChild(tbMaxDigit.Name) as SliderWidget).GetState(SliderWidget.SliderUnitsOnes);
            }
            return prefs;
        }

        /// <summary>
        /// Something changed. Set the dirty flag
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void widget_EvtValueChanged(object sender, WidgetEventArgs e)
        {
            _isDirty = true;
        }
    }
}