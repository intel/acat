////////////////////////////////////////////////////////////////////////////
// <copyright file="WordPredictionSettingsForm.cs" company="Intel Corporation">
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
    /// Dialog to change settings related to word prediction
    /// </summary>
    [DescriptorAttribute("DF4867B5-C812-44A9-8DFB-1D6D9DC4A81A",
                        "WordPredictionSettingsForm",
                        "Word Prediction Settings Dialog")]
    public partial class WordPredictionSettingsForm : Form, IDialogPanel
    {
        /// <summary>
        /// The DialogCommon object
        /// </summary>
        private DialogCommon _dialogCommon;

        /// <summary>
        /// Did the user change anything?
        /// </summary>
        private bool _isDirty;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public WordPredictionSettingsForm()
        {
            InitializeComponent();

            panelTitle.Text = R.GetString("WordPredictionSettings");
            Load += ScannerSettingsForm_Load;
            FormClosing += ScannerSettingsForm_FormClosing;
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
        /// Gets the synch object
        /// </summary>
        public SyncLock SyncObj
        {
            get { return _dialogCommon.SyncObj; }
        }

        /// <summary>
        /// Sets form styles
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

            initWidgetSettings(Common.AppPreferences);

            return true;
        }

        /// <summary>
        /// Triggered when a widget is actuated
        /// </summary>
        /// <param name="widget">Which one triggered?</param>
        ///
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
        /// <param name="e">closing arg</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _dialogCommon.OnFormClosing(e);
            base.OnFormClosing(e);
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
        /// Inits the controls in the dialog box based on
        /// settings
        /// </summary>
        /// <param name="prefs">ACAT settings</param>
        private void initWidgetSettings(ACATPreferences prefs)
        {
            var rootWidget = PanelCommon.RootWidget;

            (rootWidget.Finder.FindChild(pbDynamicLearning.Name) as CheckBoxWidget).SetState(prefs.EnableWordPredictionDynamicModel);
            (rootWidget.Finder.FindChild(tbWordCount.Name) as SliderWidget).SetState(prefs.WordPredictionCount, SliderWidget.SliderUnitsOnes);
        }

        /// <summary>
        /// Loads default values for all the settings in the
        /// dialog
        /// </summary>
        private void loadDefaultSettings()
        {
            if (DialogUtils.Confirm(this, R.GetString("RestoreDefaultSettings")))
            {
                Context.AppWordPredictionManager.LoadDefaultSettings();

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
            lblDynamicLearning.Text = R.GetString(lblDynamicLearning.Text);
            lblWordCount.Text = R.GetString(lblWordCount.Text);
        }

        /// <summary>
        /// Confirms with the user and quits the dialog
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
        /// Saves settings and closes the dialog
        /// </summary>
        private void saveSettingsAndQuit()
        {
            if (_isDirty && DialogUtils.Confirm(this, R.GetString("SaveSettings")))
            {
                updateSettingsFromUI().Save();

                Context.AppWordPredictionManager.SaveSettings();

                _isDirty = false;
                Common.AppPreferences.NotifyPreferencesChanged();
            }

            Windows.CloseForm(this);
        }

        /// <summary>
        /// Form is closing. Release resources
        /// </summary>
        private void ScannerSettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _dialogCommon.OnClosing();
        }

        /// <summary>
        /// Form has been loaded
        /// </summary>
        private void ScannerSettingsForm_Load(object sender, EventArgs e)
        {
            populateFormText();

            _dialogCommon.OnLoad();

            subscribeToEvents();

            PanelCommon.AnimationManager.Start(PanelCommon.RootWidget);
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
        /// Update settings based on what the user has
        /// set in the dialog.  Returns the preferences object
        /// with the settings
        /// </summary>
        /// <returns>Preferences object</returns>
        private ACATPreferences updateSettingsFromUI()
        {
            var rootWidget = PanelCommon.RootWidget;
            var prefs = ACATPreferences.Load();

            prefs.EnableWordPredictionDynamicModel = Common.AppPreferences.EnableWordPredictionDynamicModel = (rootWidget.Finder.FindChild(pbDynamicLearning.Name) as CheckBoxWidget).GetState();
            prefs.WordPredictionCount = Common.AppPreferences.WordPredictionCount = (rootWidget.Finder.FindChild(tbWordCount.Name) as SliderWidget).GetState(SliderWidget.SliderUnitsOnes);

            return prefs;
        }

        /// <summary>
        /// Some setting changed. Set the dirty flag to indicate this
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void widget_EvtValueChanged(object sender, WidgetEventArgs e)
        {
            _isDirty = true;
        }

        private void lblOK_Click(object sender, EventArgs e)
        {

        }

        private void lblBack_Click(object sender, EventArgs e)
        {

        }
    }
}