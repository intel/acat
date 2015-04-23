////////////////////////////////////////////////////////////////////////////
// <copyright file="VoiceSettingsForm.cs" company="Intel Corporation">
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
using ACAT.Lib.Core.TTSManagement;
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
    /// Dialog box to set the parameters for text to speech.
    /// This includes volume, rate of speech and pitch
    /// </summary>
    [DescriptorAttribute("0E2822DB-938C-4A45-AC0E-B2744E179944", "VoiceSettingsForm", "Speech Engine Settings Dialog")]
    public partial class VoiceSettingsForm : Form, IDialogPanel
    {
        /// <summary>
        /// The DialogCommon object
        /// </summary>
        private readonly DialogCommon _dialogCommon;

        /// <summary>
        /// Initial value of the pitch setting
        /// </summary>
        private TTSValue _initialPitch;

        /// <summary>
        /// Initial value of the rate setting
        /// </summary>
        private TTSValue _initialRate;

        /// <summary>
        /// Initial value of the volume setting
        /// </summary>
        private TTSValue _initialVolume;

        /// <summary>
        /// Did the user change anything?
        /// </summary>
        private bool _isDirty;

        /// <summary>
        /// Ensures the window stays focused
        /// </summary>
        private WindowActiveWatchdog _windowActiveWatchdog;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public VoiceSettingsForm()
        {
            InitializeComponent();

            saveInitalValues();

            _dialogCommon = new DialogCommon(this);

            if (!_dialogCommon.Initialize())
            {
                Log.Debug("Initialization error");
            }

            populateUI();

            tbPitch.TextChanged += tbPitch_TextChanged;
            tbRate.TextChanged += tbRate_TextChanged;
            tbVolume.TextChanged += tbVolume_TextChanged;
            Load += VoiceSettingsForm_Load;
            FormClosing += VoiceSettingsForm_FormClosing;
        }

        /// <summary>
        /// Gets the descriptor for this class
        /// </summary>
        public IDescriptor Descriptor
        {
            get { return DescriptorAttribute.GetDescriptor(GetType()); }
        }

        /// <summary>
        /// Gets the synchronization object
        /// </summary>
        public SyncLock SyncObj
        {
            get { return _dialogCommon.SyncObj; }
        }

        /// <summary>
        /// Sets the form style
        /// </summary>
        protected override CreateParams CreateParams
        {
            get { return DialogCommon.SetFormStyles(base.CreateParams); }
        }

        /// <summary>
        /// Triggered when a widget is actuated
        /// </summary>
        /// <param name="widget">Which one triggered?</param>
        public void OnButtonActuated(Widget widget)
        {
            Log.Debug("**Actuate** " + widget.Name + " Value: " + widget.Value);

            var value = widget.Value;

            if (String.IsNullOrEmpty(value))
            {
                return;
            }

            Invoke(new MethodInvoker(delegate()
            {
                switch (value)
                {
                    case "valButtonBack":
                        quit();
                        break;

                    case "valButtonSave":
                        saveSettingsAndQuit();
                        break;

                    case "valButtonRestoreDefaults":
                        loadDefaultSettings();
                        break;

                    case "valButtonTest":
                        testVoice();
                        break;
                }
            }));
        }

        /// <summary>
        /// Pauses the animation
        /// </summary>
        public void OnPause()
        {
            _dialogCommon.OnPause();
        }

        /// <summary>
        /// Resumes paused dialog
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
            handled = false;
        }

        /// <summary>
        /// Form is closing . Release resources
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _dialogCommon.OnFormClosing(e);
            base.OnFormClosing(e);
        }

        /// <summary>
        /// Window procedure
        /// </summary>
        /// <param name="m"></param>
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        protected override void WndProc(ref Message m)
        {
            _dialogCommon.HandleWndProc(m);
            base.WndProc(ref m);
        }

        /// <summary>
        /// Ensures that all the settings are valid, within
        ///  the range
        /// </summary>
        /// <returns>true if they are</returns>
        private Boolean HasValidPreferenceValues()
        {
            int value;
            if (!int.TryParse(Windows.GetText(tbVolume), out value) ||
                !_initialVolume.IsValid(value))
            {
                showError("Invalid Volume setting");
                return false;
            }

            if (!int.TryParse(Windows.GetText(tbRate), out value) ||
                !_initialRate.IsValid(value))
            {
                showError("Invalid Rate setting");
                return false;
            }

            if (!int.TryParse(Windows.GetText(tbPitch), out value) ||
                !_initialPitch.IsValid(value))
            {
                showError("Invalid Rate setting");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Restores default settings
        /// </summary>
        private void loadDefaultSettings()
        {
            if (DialogUtils.Confirm(this, "Restore default settings?"))
            {
                // get entire default file and just set those settings that belong to this preferences screen
                Context.AppTTSManager.ActiveEngine.RestoreDefaults();
                populateUI();
                _isDirty = true;
            }
        }

        /// <summary>
        /// Populate the controls with settings from
        /// the TTS engine
        /// </summary>
        private void populateUI()
        {
            tbVolume.Text = Convert.ToString(Context.AppTTSManager.ActiveEngine.GetVolume().Value);
            tbRate.Text = Convert.ToString(Context.AppTTSManager.ActiveEngine.GetRate().Value);
            tbPitch.Text = Convert.ToString(Context.AppTTSManager.ActiveEngine.GetPitch().Value);
        }

        /// <summary>
        /// Confirm with the user and quit the dialog
        /// </summary>
        private void quit()
        {
            bool quit = true;

            if (_isDirty)
            {
                if (!DialogUtils.Confirm(this, "Changes not saved. Quit?"))
                {
                    quit = false;
                }
            }

            if (quit)
            {
                restoreInitialValues();
                Windows.CloseForm(this);
            }
        }

        /// <summary>
        /// Restore initial values
        /// </summary>
        private void restoreInitialValues()
        {
            Context.AppTTSManager.ActiveEngine.SetVolume(_initialVolume.Value);
            Context.AppTTSManager.ActiveEngine.SetRate(_initialRate.Value);
            Context.AppTTSManager.ActiveEngine.SetPitch(_initialPitch.Value);
        }

        /// <summary>
        /// Get the starting values of all the settings
        /// </summary>
        private void saveInitalValues()
        {
            _initialVolume = Context.AppTTSManager.ActiveEngine.GetVolume();
            _initialRate = Context.AppTTSManager.ActiveEngine.GetRate();
            _initialPitch = Context.AppTTSManager.ActiveEngine.GetPitch();
        }

        /// <summary>
        /// Save settings and quit the dialog. Confirm
        /// with the user first if necessary
        /// </summary>
        private void saveSettingsAndQuit()
        {
            if (!HasValidPreferenceValues())
            {
                return;
            }

            if (DialogUtils.Confirm(this, "Save settings?"))
            {
                updateActiveEngineSettingsFromUI();
                Context.AppTTSManager.ActiveEngine.Save();
                _isDirty = false;
            }
            else
            {
                restoreInitialValues();
            }

            Windows.CloseForm(this);
        }

        /// <summary>
        /// Displays error in a message box
        /// </summary>
        /// <param name="error">error string</param>
        private void showError(String error)
        {
            DialogUtils.ShowTimedDialog(this, "Error", error);
        }

        /// <summary>
        /// Subscribes to all the events triggered by the
        /// widgets and the interpreter
        /// </summary>
        private void subscribeToEvents()
        {
            List<Widget> widgetList = new List<Widget>();
            _dialogCommon.GetRootWidget().Finder.FindAllButtons(widgetList);

            foreach (Widget widget in widgetList)
            {
                widget.EvtValueChanged += new WidgetEventDelegate(widget_EvtValueChanged);
            }

            widgetList.Clear();
            _dialogCommon.GetRootWidget().Finder.FindAllChildren(typeof(SliderWidget), widgetList);

            foreach (Widget widget in widgetList)
            {
                widget.EvtValueChanged += new WidgetEventDelegate(widget_EvtValueChanged);
            }
        }

        /// <summary>
        /// The 'pitch' changed in the dialog box
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void tbPitch_TextChanged(object sender, EventArgs e)
        {
            _isDirty = true;
        }

        /// <summary>
        /// The 'rate' changed in the dialog box
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void tbRate_TextChanged(object sender, EventArgs e)
        {
            _isDirty = true;
        }

        /// <summary>
        /// The 'volume' changed in the dialog box
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void tbVolume_TextChanged(object sender, EventArgs e)
        {
            _isDirty = true;
        }

        /// <summary>
        /// Test the current settings by sending a string
        /// to the speech engine
        /// </summary>
        private void testVoice()
        {
            if (HasValidPreferenceValues())
            {
                int volume = Convert.ToInt16(tbVolume.Text);
                int rate = Convert.ToInt16(tbRate.Text);
                int pitch = Convert.ToInt16(tbPitch.Text);

                Context.AppTTSManager.ActiveEngine.SetVolume(volume);
                Context.AppTTSManager.ActiveEngine.SetRate(rate);
                Context.AppTTSManager.ActiveEngine.SetPitch(pitch);

                Context.AppTTSManager.ActiveEngine.Speak(Common.AppPreferences.UserVoiceTestString);
            }
        }

        /// <summary>
        /// Update settings in the TTS engine with values
        /// from the dialog box
        /// </summary>
        private void updateActiveEngineSettingsFromUI()
        {
            Context.AppTTSManager.ActiveEngine.SetVolume(Convert.ToInt16(tbVolume.Text));
            Context.AppTTSManager.ActiveEngine.SetPitch(Convert.ToInt16(tbPitch.Text));
            Context.AppTTSManager.ActiveEngine.SetRate(Convert.ToInt16(tbRate.Text));
        }

        /// <summary>
        /// Form is closing. Release resources
        /// </summary>
        private void VoiceSettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _windowActiveWatchdog.Dispose();
            _dialogCommon.OnClosing();
        }

        /// <summary>
        /// Form has been loaded
        /// </summary>
        private void VoiceSettingsForm_Load(object sender, EventArgs e)
        {
            lblVolumeText.Text = _initialVolume.RangeMin + " to " + _initialVolume.RangeMax;
            lblRateText.Text = _initialRate.RangeMin + " to " + _initialRate.RangeMax;
            lblPitchText.Text = _initialPitch.RangeMin + " to " + _initialPitch.RangeMax;
            lblTTSEngineName.Text = Context.AppTTSManager.ActiveEngine.Descriptor.Name;

            _windowActiveWatchdog = new WindowActiveWatchdog(this);
            _dialogCommon.OnLoad();
            subscribeToEvents();
            _dialogCommon.GetAnimationManager().Start(_dialogCommon.GetRootWidget());
        }

        /// <summary>
        /// Something changed in the dialog box.  Set the dirty
        /// flag to indicate this.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void widget_EvtValueChanged(object sender, WidgetEventArgs e)
        {
            _isDirty = true;
        }
    }
}