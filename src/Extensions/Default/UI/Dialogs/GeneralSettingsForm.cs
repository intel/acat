////////////////////////////////////////////////////////////////////////////
// <copyright file="GeneralSettingsForm.cs" company="Intel Corporation">
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
    /// Dialog form that allows user to change common
    /// settings for ACAT.  All settings in this dialog
    /// box are checkboxes.
    /// </summary>
    [DescriptorAttribute("216A2E8D-15A7-4995-B2D3-C0D3B4D42374",
                        "GeneralSettingsForm",
                        "General Settings Dialog")]
    public partial class GeneralSettingsForm : Form, IDialogPanel
    {
        /// <summary>
        /// The dialogCommon object
        /// </summary>
        private DialogCommon _dialogCommon;

        /// <summary>
        /// Did the user change any setting?
        /// </summary>
        private bool _isDirty;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public GeneralSettingsForm()
        {
            InitializeComponent();

            Load += GeneralSettingsForm_Load;
            FormClosing += GeneralSettingsForm_FormClosing;
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
        /// Gets the synch object for the scanner
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
            get
            {
                return DialogCommon.SetFormStyles(base.CreateParams);
            }
        }

        /// <summary>
        /// Intitializes the class
        /// </summary>
        /// <param name="startupArg">startup param</param>
        /// <returns>true on success</returns>
        public bool Initialize(StartupArg startupArg)
        {
            _dialogCommon = new DialogCommon(this);

            bool retVal = _dialogCommon.Initialize(startupArg);

            if (retVal)
            {
                initWidgetSettings(Common.AppPreferences);
            }

            return retVal;
        }

        /// <summary>
        /// Triggered when a widget is triggered. This handles all
        /// the buttons on the form - OK, cancel and restore defaults
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

            Invoke(new MethodInvoker(delegate
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
                }
            }));
        }

        /// <summary>
        /// Pauses animation
        /// </summary>
        public void OnPause()
        {
            _dialogCommon.OnPause();
        }

        /// <summary>
        /// Resumes animation
        /// </summary>
        public void OnResume()
        {
            _dialogCommon.OnResume();
        }

        /// <summary>
        /// Invoked when there is a need to run a command. The
        /// command comes from the animation file
        /// </summary>
        /// <param name="command">the command to run</param>
        /// <param name="handled">wast it handled?</param>
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
        /// Form is closing. Release resources
        /// </summary>
        /// <param name="e">event args</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _dialogCommon.OnFormClosing(e);
            base.OnFormClosing(e);
        }

        /// <summary>
        /// Win proc
        /// </summary>
        /// <param name="m"></param>
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        protected override void WndProc(ref Message m)
        {
            _dialogCommon.HandleWndProc(m);
            base.WndProc(ref m);
        }

        /// <summary>
        /// Form is closing. Release resources
        /// </summary>
        private void GeneralSettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _dialogCommon.OnClosing();
        }

        /// <summary>
        /// Form has been loaded. Initialize the form
        /// </summary>
        private void GeneralSettingsForm_Load(object sender, EventArgs e)
        {
            populateFormText();

            _dialogCommon.OnLoad();

            subscribeToEvents();

            PanelCommon.AnimationManager.Start(PanelCommon.RootWidget);
        }

        /// <summary>
        /// Gets the settings with the current state of
        /// widgets in the form and returns a ACATPreferences
        /// object that can be presisted.
        /// </summary>
        /// <returns>Preferences with settings from the form</returns>
        private ACATPreferences getAppPreferencesFromUI()
        {
            var prefs = ACATPreferences.Load();

            var rootWidget = PanelCommon.RootWidget;

            prefs.HideScannerOnIdle = Common.AppPreferences.HideScannerOnIdle = (rootWidget.Finder.FindChild(pbHideScannersOnIdle.Name) as CheckBoxWidget).GetState();
            prefs.ExpandAbbreviationsOnSeparator = Common.AppPreferences.ExpandAbbreviationsOnSeparator = (rootWidget.Finder.FindChild(pbExpandAbbreviationsOnSeparator.Name) as CheckBoxWidget).GetState();
            prefs.ShowTalkWindowOnStartup = Common.AppPreferences.ShowTalkWindowOnStartup = (rootWidget.Finder.FindChild(pbShowTalkWindowOnStartup.Name) as CheckBoxWidget).GetState();
            prefs.RetainTalkWindowContentsOnHide = Common.AppPreferences.RetainTalkWindowContentsOnHide = (rootWidget.Finder.FindChild(pbRetainTalkWindowText.Name) as CheckBoxWidget).GetState();
            prefs.AutoSaveScannerLastPosition = Common.AppPreferences.AutoSaveScannerLastPosition = (rootWidget.Finder.FindChild(pbScannerAutoSaveLastPosition.Name) as CheckBoxWidget).GetState();

            if (Common.AppPreferences.AutoSaveScannerLastPosition)
            {
                prefs.ScannerPosition = Common.AppPreferences.ScannerPosition = Context.AppWindowPosition;
            }

            return prefs;
        }

        /// <summary>
        /// Initializes the state of all the widgets in the form
        /// with values from the settings.
        /// </summary>
        /// <param name="prefs">ACAT Preferences</param>
        private void initWidgetSettings(ACATPreferences prefs)
        {
            var rootWidget = PanelCommon.RootWidget;

            ((CheckBoxWidget)(rootWidget.Finder.FindChild(pbScannerAutoSaveLastPosition.Name))).SetState(prefs.AutoSaveScannerLastPosition);
            ((CheckBoxWidget)(rootWidget.Finder.FindChild(pbHideScannersOnIdle.Name))).SetState(prefs.HideScannerOnIdle);
            ((CheckBoxWidget)(rootWidget.Finder.FindChild(pbShowTalkWindowOnStartup.Name))).SetState(prefs.ShowTalkWindowOnStartup);
            ((CheckBoxWidget)(rootWidget.Finder.FindChild(pbRetainTalkWindowText.Name))).SetState(prefs.RetainTalkWindowContentsOnHide);
            ((CheckBoxWidget)(rootWidget.Finder.FindChild(pbExpandAbbreviationsOnSeparator.Name))).SetState(prefs.ExpandAbbreviationsOnSeparator);
        }

        /// <summary>
        /// Restores default settings from the settings file for ACAT
        /// </summary>
        private void loadDefaultSettings()
        {
            if (DialogUtils.Confirm(this, R.GetString("RestoreDefaultSettings")))
            {
                initWidgetSettings(ACATPreferences.LoadDefaultSettings());
                _isDirty = true;
            }
        }

        /// <summary>
        /// Populate form controls text from Language resource
        /// </summary>
        private void populateFormText()
        {
            panelTitle.Text = R.GetString(panelTitle.Text);
            lblScannerAutoSaveLastPosition.Text = R.GetString(lblScannerAutoSaveLastPosition.Text);
            lblHideScannersOnIdle.Text = R.GetString(lblHideScannersOnIdle.Text);
            lblShowTalkWindowOnStartup.Text = R.GetString(lblShowTalkWindowOnStartup.Text);
            lblRetainTalkWindowText.Text = R.GetString(lblRetainTalkWindowText.Text);
            lblExpandAbbreviationsOnSeparator.Text = R.GetString(lblExpandAbbreviationsOnSeparator.Text);
        }

        /// <summary>
        /// Confirms whether to quit and close the form
        /// </summary>
        private void quit()
        {
            bool quit = true;

            if (_isDirty)
            {
                if (!DialogUtils.Confirm(this, R.GetString("ChangesNotSavedQuit")))
                {
                    quit = false;
                }
            }

            if (quit)
            {
                Windows.CloseForm(this);
            }
        }

        /// <summary>
        /// Saves changes and closes the dialog
        /// </summary>
        private void saveSettingsAndQuit()
        {
            if (DialogUtils.Confirm(this, R.GetString("SaveSettings")))
            {
                getAppPreferencesFromUI().Save();

                _isDirty = false;

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
            PanelCommon.RootWidget.Finder.FindAllButtons(widgetList);

            foreach (var widget in widgetList)
            {
                widget.EvtValueChanged += widget_EvtValueChanged;
            }
        }

        /// <summary>
        /// User changed some setting on the form. Sets the dirty flag
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void widget_EvtValueChanged(object sender, WidgetEventArgs e)
        {
            _isDirty = true;
        }
    }
}