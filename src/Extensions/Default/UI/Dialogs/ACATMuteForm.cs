////////////////////////////////////////////////////////////////////////////
// <copyright file="ACATMuteForm.cs" company="Intel Corporation">
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
using System.Threading;
using System.Windows.Forms;
using ACAT.Lib.Core.ActuatorManagement;
using ACAT.Lib.Core.AnimationManagement;
using ACAT.Lib.Core.Audit;
using ACAT.Lib.Core.InputActuators;
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
    /// The Mute form is meant to be used to 'eat' all the
    /// actuations from the sensor.  This could be used for
    /// instance when ACAT is driven from a cheek sensor or
    /// with a camera sensor.  If the user doesn't want to
    /// interact with ACAT, e.g. while eating, the sensor may
    /// create noise and false positives if the ACAT UI is active.
    /// The mute screen prevents this by displaying a pin code
    /// that the user has to explicitly enter to unlock the
    /// mute screen
    /// </summary>
    ///
    [DescriptorAttribute("303AF1A4-C097-4C84-B031-96A10935E0A6", "ACATMuteForm", "Mute Dialog")]
    public partial class ACATMuteForm : Form, IPanel
    {
        /// <summary>
        /// Default format of date to display on the form
        /// </summary>
        private const String DefaultDateFormat = "dddd, MMMM d, yyyy";

        /// <summary>
        /// Default format of the time to display on the form
        /// </summary>
        private const String DefaultTimeFormat = "h:mm:ss tt";

        /// <summary>
        /// The keyboard actuator object
        /// </summary>
        private readonly KeyboardActuator _keyboardActuator;

        /// <summary>
        /// Array of buttons used as the numeric digits
        /// </summary>
        private readonly String[] _keypadButtonArray = { "B0", "B1", "B2", "B3", "B4", "B5", "B6", "B7", "B8", "B9" };

        /// <summary>
        /// Pin to unlock the screen
        /// </summary>
        private readonly String _pin = Common.AppPreferences.MutePin;

        /// <summary>
        /// Array of buttons to enter the pin
        /// </summary>
        private readonly String[] _pinButtonArray = { "B11", "B12", "B13", "B14" };

        /// <summary>
        /// Used for synchronization
        /// </summary>
        private readonly SyncLock _syncObj;

        /// <summary>
        /// Ensures this window stays focused
        /// </summary>
        private readonly WindowOverlapWatchdog _windowOverlapWatchdog;

        /// <summary>
        /// The animation manager object
        /// </summary>
        private AnimationManager _animationManager;

        /// <summary>
        /// Pin entered so far
        /// </summary>
        private String _pinEntered = String.Empty;

        /// <summary>
        /// Represents the widget of this form
        /// </summary>
        private Widget _rootWidget;

        /// <summary>
        /// The widgetManager object
        /// </summary>
        private WidgetManager _widgetManager;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ACATMuteForm()
        {
            InitializeComponent();

            _syncObj = new SyncLock();

            ShowInTaskbar = false;
            if (!initialize())
            {
                Log.Debug("Initialization error!");
            }

            TopMost = true;

            RoundedCornerControl.CreateRoundedControl(Title);

            Windows.ShowWindowBorder(this, Common.AppPreferences.ScannerShowBorder);

            Windows.SetWindowPosition(this, Windows.WindowPosition.CenterScreen);

            _windowOverlapWatchdog = new WindowOverlapWatchdog(this);

            updateDateTime();

            // Title isn't used on this form, but we keep it hidden instead of removing it
            // so we don't have to modify the IScreenInterface GetTitle() method
            Title.Hide();

            // timer used for displaying the time
            timer.Start();

            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;

            Load += MuteForm_Load;
            FormClosing += MuteForm_FormClosing;

            var actuator = ActuatorManager.Instance.GetActuator(typeof(KeyboardActuator));
            if (actuator != null)
            {
                _keyboardActuator = actuator as KeyboardActuator;
                if (_keyboardActuator != null)
                {
                    _keyboardActuator.EvtKeyDown += _keyboardActuator_EvtKeyDown;
                    _keyboardActuator.EvtKeyUp += _keyboardActuator_EvtKeyUp;
                }
            }
        }

        /// <summary>
        /// Gets the descriptor for this class
        /// </summary>
        public IDescriptor Descriptor
        {
            get { return DescriptorAttribute.GetDescriptor(GetType()); }
        }

        /// <summary>
        /// Gets the synch object
        /// </summary>
        public SyncLock SyncObj
        {
            get { return _syncObj; }
        }

        /// <summary>
        /// Wnd proc handler. Resume animation if
        /// user clicks the mouse
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        public bool HandleWndProc(Message m)
        {
            const int WM_MOUSEACTIVATE = 0x21;

            if (m.Msg == WM_MOUSEACTIVATE)
            {
                if (_animationManager != null)
                {
                    Invoke(new MethodInvoker(delegate()
                    {
                        if (_animationManager.GetPlayerState() == PlayerState.Timeout)
                        {
                            _animationManager.Resume();
                        }
                    }));
                }
            }

            return false;
        }

        /// <summary>
        /// Not used
        /// </summary>
        public void OnPause()
        {
        }

        /// <summary>
        /// Not used
        /// </summary>
        public void OnResume()
        {
        }

        /// <summary>
        /// Unsubsribe events and close form
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_keyboardActuator != null)
            {
                _keyboardActuator.EvtKeyDown -= _keyboardActuator_EvtKeyDown;
                _keyboardActuator.EvtKeyUp -= _keyboardActuator_EvtKeyUp;
            }

            _animationManager.Stop();

            base.OnFormClosing(e);
        }

        /// <summary>
        /// Window proc
        /// </summary>
        /// <param name="m">Windows message</param>
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        protected override void WndProc(ref Message m)
        {
            HandleWndProc(m);
            base.WndProc(ref m);
        }

        /// <summary>
        /// Key down event handler
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void _keyboardActuator_EvtKeyDown(object sender, KeyEventArgs e)
        {
            int value = e.KeyValue;

            e.Handled = true;
            var ch = (char)value;
            if (char.IsDigit(ch))
            {
                handlePinInput(ch.ToString());
            }
        }

        /// <summary>
        /// Key up handler
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void _keyboardActuator_EvtKeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// The 'value' is the pin the user entered. Verify
        /// if this is a valid pin and 'unlock' the digits
        /// entered so far. If the whole pin has been entered
        /// unlock the mute screen.
        /// After each digit is entered, transitions the animation to
        /// the next one
        /// </summary>
        /// <param name="value">pin value entered so far</param>
        private void handlePinInput(String value)
        {
            int nextIndex = _pinEntered.Length;
            if (nextIndex >= _pin.Length)
            {
                return;
            }

            if (value[0] == _pin[nextIndex])
            {
                _pinEntered = _pinEntered + value;
                Log.Debug("pin: " + _pin + ", _pinEntered: " + _pinEntered);
                switch (_pinEntered.Length)
                {
                    case 1:
                        _animationManager.Transition("FirstKey");
                        break;

                    case 2:
                        _animationManager.Transition("SecondKey");
                        break;

                    case 3:
                        _animationManager.Transition("ThirdKey");
                        break;

                    case 4:
                        _animationManager.Transition("FourthKey");
                        break;
                }

                if (_pinEntered.Length == _pin.Length)
                {
                    unlockMuteScreen();
                }
            }
            else
            {
                AuditLog.Audit(new AuditEventMuteScreen("invalidpin"));

                _pinEntered = String.Empty;
                _animationManager.Transition("TopLevel");
            }
        }

        /// <summary>
        /// Loads all animations
        /// </summary>
        private bool initAnimationManager()
        {
            _animationManager = new AnimationManager();
            var configFile = PanelConfigMap.GetConfigFileForScreen(GetType());

            var retVal = _animationManager.Init(configFile);

            if (!retVal)
            {
                Log.Error("Error initializing animation manager");
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Initialize the form
        /// </summary>
        /// <returns>true on success</returns>
        private bool initialize()
        {
            var retVal = initWidgetManager();

            if (retVal)
            {
                retVal = initAnimationManager();
            }

            return retVal;
        }

        /// <summary>
        /// Initialize the widget manager
        /// </summary>
        private bool initWidgetManager()
        {
            _widgetManager = new WidgetManager(this);

            var configFile = PanelConfigMap.GetConfigFileForScreen(this.GetType());
            var retVal = _widgetManager.Initialize(configFile);

            if (!retVal)
            {
                Log.Error("Unable to initialize widget manager");
            }
            else
            {
                _rootWidget = _widgetManager.RootWidget;
            }

            return retVal;
        }

        /// <summary>
        /// Form is closing. Release resources
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void MuteForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _animationManager.Dispose();

            _animationManager = null;

            _widgetManager.Dispose();

            if (_windowOverlapWatchdog != null)
            {
                _windowOverlapWatchdog.Dispose();
            }
        }

        /// <summary>
        /// Form has been loaded. Initialize the UI
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void MuteForm_Load(object sender, EventArgs e)
        {
            subscribeToEvents();

            // Hide all the buttons that are not reqd.  Pin length is
            // configurable.  Any numeric buttons past the pin length should be hidden
            for (int ii = Common.AppPreferences.MutePinDigitMax + 1; ii < _keypadButtonArray.Length; ii++)
            {
                var buttonWidget = _rootWidget.Finder.FindChild(_keypadButtonArray[ii]);
                if (buttonWidget != null)
                {
                    buttonWidget.Hide();
                }
            }

            // Display the pin
            for (int ii = 0; ii < _pin.Length && ii < _pinButtonArray.Length; ii++)
            {
                Widget widget = _rootWidget.Finder.FindChild(_pinButtonArray[ii]);
                if (widget != null)
                {
                    widget.Value = _pin[ii].ToString();
                    widget.SetText(_pin[ii].ToString());
                }
            }

            // hide remaining buttons
            for (int ii = _pin.Length; ii < _pinButtonArray.Length; ii++)
            {
                Widget widget = _rootWidget.Finder.FindChild(_pinButtonArray[ii]);
                if (widget != null)
                {
                    widget.Hide();
                }
            }

            AuditLog.Audit(new AuditEventMuteScreen("show"));

            _animationManager.Start(_rootWidget);
        }

        /// <summary>
        /// Subscribes to all the events triggered by the
        /// widgets and the interpreter
        /// </summary>
        private void subscribeToEvents()
        {
            var buttonList = new List<Widget>();
            _rootWidget.Finder.FindChild(typeof(LabelWidget), buttonList);

            foreach (var widget in buttonList)
            {
                widget.EvtActuated += widget_EvtActuated;
            }
        }

        /// <summary>
        /// Updates the date / time on the form
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void timer_Tick(object sender, EventArgs e)
        {
            updateDateTime();
        }

        /// <summary>
        /// Thread proc to close the form and quit
        /// </summary>
        private void unlockDoneProc()
        {
            const int UNLOCK_DONE_PROC_SLEEP_TIME = 1000;

            _animationManager.Stop();
            Thread.Sleep(UNLOCK_DONE_PROC_SLEEP_TIME);

            AuditLog.Audit(new AuditEventMuteScreen("close"));

            Windows.CloseForm(this);
        }

        /// <summary>
        /// Unlock the screen and close asynchronously
        /// </summary>
        private void unlockMuteScreen()
        {
            Invoke(new MethodInvoker(WindowActivityMonitor.Resume));
            var unlockDoneThread = new Thread(unlockDoneProc);
            unlockDoneThread.Start();
        }

        /// <summary>
        /// Displayes the current date time on the form
        /// </summary>
        private void updateDateTime()
        {
            try
            {
                lblDate.Text = DateTime.Today.ToString(Common.AppPreferences.MuteScreenDisplayDateFormat);
            }
            catch
            {
                lblDate.Text = DateTime.Today.ToString(DefaultDateFormat);
            }

            try
            {
                lblTime.Text = DateTime.Now.ToString(Common.AppPreferences.MuteScreenDisplayTimeFormat);
            }
            catch
            {
                lblTime.Text = DateTime.Now.ToString(DefaultTimeFormat);
            }
        }

        /// <summary>
        /// Triggered when a widget is triggered
        /// </summary>
        /// <param name="widget">Which one triggered?</param>
        private void widget_EvtActuated(object sender, WidgetEventArgs e)
        {
            var widget = e.SourceWidget;

            Log.Debug("widget actuated! UIControl.Name=" + widget.UIControl.Name + ", value: " + widget.Value);

            handlePinInput(widget.Value);
        }
    }
}