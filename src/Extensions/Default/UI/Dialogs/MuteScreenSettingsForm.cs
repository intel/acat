////////////////////////////////////////////////////////////////////////////
// <copyright file="MuteScreenSettings.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2015 Intel Corporation 
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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Permissions;
using System.Windows.Forms;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using ACAT.Lib.Core.Widgets;
using ACAT.Lib.Extension;

#region SupressStyleCopWarnings

[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1126:PrefixCallsCorrectly",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1101:PrefixLocalCallsWithThis",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1121:UseBuiltInTypeAlias",
        Scope = "namespace",
        Justification = "Since they are just aliases, it doesn't really matter")]
[module: SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1200:UsingDirectivesMustBePlacedWithinNamespace",
        Scope = "namespace",
        Justification = "ACAT guidelines")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1309:FieldNamesMustNotBeginWithUnderscore",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private fields begin with an underscore")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1300:ElementMustBeginWithUpperCaseLetter",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private/Protected methods begin with lowercase")]

#endregion SupressStyleCopWarnings

namespace ACAT.Extensions.Default.UI.Dialogs
{
    /// <summary>
    /// Dialog that enables the user to set the pin code
    /// for the ACAT mute scanner. User can also set
    /// the number of digits in the pin and the max digit
    /// </summary>
    [DescriptorAttribute("F3774E1D-E15A-4E5F-90B2-1851FAE93D3C", "MuteScreenSettingsForm", "Mute Screen Settings")]
    public partial class MuteScreenSettingsForm : Form, IDialogPanel
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
        private readonly DialogCommon _dialogCommon;

        /// <summary>
        /// Ensures that the window stays focused
        /// </summary>
        private WindowActiveWatchdog _windowActiveWatchdog;

        /// <summary>
        /// Have the settings changed?
        /// </summary>
        private bool isDirty;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public MuteScreenSettingsForm()
        {
            InitializeComponent();

            _windowActiveWatchdog = new WindowActiveWatchdog(this);

            _dialogCommon = new DialogCommon(this);

            if (!_dialogCommon.Initialize())
            {
                Log.Debug("Initialization error");
            }

            initWidgetSettings();

            Load += MuteScreenSettingsForm_Load;
            FormClosing += MuteScreenSettingsForm_FormClosing;
        }

        /// <summary>
        /// Gets the descriptor for this class
        /// </summary>
        public IDescriptor Descriptor
        {
            get { return DescriptorAttribute.GetDescriptor(GetType()); }
        }

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
        /// Triggered when a widget is triggered
        /// </summary>
        /// <param name="widget">Which one triggered?</param>
        public void OnButtonActuated(Widget widget)
        {
            Log.Debug("**Actuate** " + widget.UIControl.Name + " Value: " + widget.Value);

            var value = widget.Value;

            if (String.IsNullOrEmpty(value))
            {
                Log.Debug("received actuation from empty widget!");
                return;
            }

            Invoke(new MethodInvoker(delegate()
            {
                switch (value)
                {
                    case "valButtonOK":
                        onOK();
                        break;

                    case "valButtonCancel":
                        onCancel();
                        break;

                    default:
                        Log.Warn("unhandled widget actuation! value=" + value);
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
        /// <param name="strPIN">The Pin</param>
        /// <param name="charMaxDigit">max length of the pin</param>
        /// <param name="errorMessage">error message if not valid</param>
        /// <returns>true if it is a valid pin</returns>
        private Boolean CheckPINCode(String strPIN, char charMaxDigit, ref String errorMessage)
        {
            int testPin;

            Log.Debug("strPIN=" + strPIN + "  strMaxDigit=" + charMaxDigit);

            if (strPIN.Length == 0)
            {
                errorMessage = "PIN cannot be empty";
                return false;
            }

            if (!Int32.TryParse(strPIN, out testPin))
            {
                errorMessage = "PIN contains non-numeric characters";
                return false;
            }

            // wrong length
            if ((strPIN.Length < MinPinLength) || (strPIN.Length > MaxPinLength))
            {
                errorMessage = "PIN length has to be between " + MinPinLength + " and " + MaxPinLength + " digits";
                return false;
            }

            // pin is out of fange
            for (byte i = 0; i < strPIN.Length; i++)
            {
                Log.Debug("strPIN[i]=" + (strPIN[i] - '0') + "  charMaxDigit=" + (charMaxDigit - '0'));
                if ((strPIN[i] - '0') > (charMaxDigit - '0'))
                {
                    errorMessage = "PIN has digits outside of the valid range";
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
            var rootWidget = _dialogCommon.GetRootWidget();

            WidgetUtils.SetSliderState(rootWidget, tbMaxDigit.Name, Common.AppPreferences.MutePinDigitMax, WidgetUtils.SliderUnitsOnes);

            Windows.SetText(tbPINCode, Common.AppPreferences.MutePin);
        }

        /// <summary>
        /// Form is closing. Release resources
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void MuteScreenSettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _windowActiveWatchdog.Dispose();

            _dialogCommon.OnClosing();
        }

        /// <summary>
        /// Form has been loaded. Initialize
        /// </summary>
        private void MuteScreenSettingsForm_Load(object sender, EventArgs e)
        {
            tbPINCode.TextChanged += tbPINCode_TextChanged;
            tbPINCode.KeyPress += tbPINCode_KeyPress;

            _dialogCommon.OnLoad();

            Invoke(new MethodInvoker(delegate()
            {
                _windowActiveWatchdog = new WindowActiveWatchdog(this);
            }));

            subscribeToEvents();

            _dialogCommon.GetAnimationManager().Start(_dialogCommon.GetRootWidget());
        }

        /// <summary>
        /// Use wants to cancel out
        /// </summary>
        private void onCancel()
        {
            bool cancel = true;

            if (isDirty)
            {
                if (!DialogUtils.Confirm(this, "Changes not saved.\nQuit anyway?"))
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
            var hasValidPin = CheckPINCode(pin, Windows.GetText(svalMaxDigit)[0], ref errorMessage);

            if (hasValidPin == false)
            {
                DialogUtils.ShowTimedDialog(this, errorMessage);
                return;
            }

            if (DialogUtils.Confirm(this, "Your new PIN is " + pin + "\nSave changes?"))
            {
                var prefs = updateSettingsFromUI();
                if (prefs != null)
                {
                    prefs.Save();
                }

                isDirty = false;
                Common.AppPreferences.NotifyPreferencesChanged();
            }

            Windows.CloseForm(this);
        }

        /// <summary>
        /// Subscribes to all the events triggered by the
        /// widgets and the interpreter
        /// </summary>
        private void subscribeToEvents()
        {
            var widgetList = new List<Widget>();
            _dialogCommon.GetRootWidget().Finder.FindAllButtons(widgetList);

            foreach (var widget in widgetList)
            {
                widget.EvtValueChanged += widget_EvtValueChanged;
            }

            widgetList.Clear();
            _dialogCommon.GetRootWidget().Finder.FindAllChildren(typeof(SliderWidget), widgetList);
            foreach (var widget in widgetList)
            {
                widget.EvtValueChanged += widget_EvtValueChanged;
            }
        }

        /// <summary>
        /// Key press event handler
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void tbPINCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// User entered something. Set the dirty flag
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void tbPINCode_TextChanged(object sender, EventArgs e)
        {
            isDirty = true;
        }

        /// <summary>
        /// Update the preferences based on values from
        /// the form (the pincode in this case)
        /// </summary>
        private ACATPreferences updateSettingsFromUI()
        {
            var strPinCode = Windows.GetText(tbPINCode);
            Int32 intPinCode;

            Log.Debug("strPINCode=" + strPinCode);

            ACATPreferences prefs = null;

            if (Int32.TryParse(strPinCode, out intPinCode) == false)
            {
                // fail because it is not a number
                Log.Debug("Invalid pincode provided!");
            }
            else
            {
                prefs = ACATPreferences.Load();
                prefs.MutePin = Common.AppPreferences.MutePin = strPinCode;
                prefs.MutePinDigitMax = Common.AppPreferences.MutePinDigitMax = (byte)WidgetUtils.GetSliderState(_dialogCommon.GetRootWidget(), tbMaxDigit.Name, WidgetUtils.SliderUnitsOnes);
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
            isDirty = true;
        }
    }
}