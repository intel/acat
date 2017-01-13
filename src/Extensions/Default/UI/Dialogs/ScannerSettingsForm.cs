////////////////////////////////////////////////////////////////////////////
// <copyright file="ScannerSettingsForm.cs" company="Intel Corporation">
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
    /// Sets the scanner settings such as the various timings,
    /// number of iterations for scanning etc.
    /// </summary>
    [DescriptorAttribute("3BC26865-9D90-4DFD-BFAB-D7E69DDFA789",
                        "ScannerSettingsForm",
                        "Mute Settings Dialog")]
    public partial class ScannerSettingsForm : Form, IDialogPanel
    {
        /// <summary>
        /// The DialogCommon object
        /// </summary>
        private DialogCommon _dialogCommon;

        /// <summary>
        ///  Did the user change anything?
        /// </summary>
        private bool _isDirty;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ScannerSettingsForm()
        {
            InitializeComponent();

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
        /// Set the form style
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

            if (!_dialogCommon.Initialize(startupArg))
            {
                return false;
            }

            initWidgetSettings(Common.AppPreferences);

            return true;
        }

        /// <summary>
        /// Triggered when a widget is actuated.
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
        /// Pause the scanner
        /// </summary>
        public void OnPause()
        {
            _dialogCommon.OnPause();
        }

        /// <summary>
        /// Resume paused scanner
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
        /// Form is closing release resources
        /// </summary>
        /// <param name="e">event arg</param>
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
        /// Gets the values from the form and updates the settings. Returns
        /// the preferences object with the new settings
        /// </summary>
        /// <returns>The </returns>
        private ACATPreferences getSettingsFromUI()
        {
            var rootWidget = PanelCommon.RootWidget;
            var prefs = ACATPreferences.Load();

            prefs.SelectClick = Common.AppPreferences.SelectClick = (rootWidget.Finder.FindChild(pbSelectingClick.Name) as CheckBoxWidget).GetState();

            prefs.GridScanIterations = Common.AppPreferences.GridScanIterations = (rootWidget.Finder.FindChild(tbEveryHalf.Name) as SliderWidget).GetState(SliderWidget.SliderUnitsOnes);
            prefs.RowScanIterations = Common.AppPreferences.RowScanIterations = (rootWidget.Finder.FindChild(tbEveryRow.Name) as SliderWidget).GetState(SliderWidget.SliderUnitsOnes);
            prefs.ColumnScanIterations = Common.AppPreferences.ColumnScanIterations = (rootWidget.Finder.FindChild(tbEveryColumn.Name) as SliderWidget).GetState(SliderWidget.SliderUnitsOnes);
            prefs.WordPredictionScanIterations = Common.AppPreferences.WordPredictionScanIterations = (rootWidget.Finder.FindChild(tbWordPrediction.Name) as SliderWidget).GetState(SliderWidget.SliderUnitsOnes);
            prefs.StripScannerColumnIterations = Common.AppPreferences.StripScannerColumnIterations = (rootWidget.Finder.FindChild(tbStripScanner.Name) as SliderWidget).GetState(SliderWidget.SliderUnitsOnes);

            prefs.MinActuationHoldTime = Common.AppPreferences.MinActuationHoldTime = (rootWidget.Finder.FindChild(tbAcceptTime.Name) as SliderWidget).GetState(SliderWidget.SliderUnitsThousandths);
            prefs.ScanTime = Common.AppPreferences.ScanTime = (rootWidget.Finder.FindChild(tbSteppingTime.Name) as SliderWidget).GetState(SliderWidget.SliderUnitsThousandths);
            prefs.FirstPauseTime = Common.AppPreferences.FirstPauseTime = (rootWidget.Finder.FindChild(tbHesitateTime.Name) as SliderWidget).GetState(SliderWidget.SliderUnitsThousandths);
            prefs.WordPredictionFirstPauseTime = Common.AppPreferences.WordPredictionFirstPauseTime = (rootWidget.Finder.FindChild(tbWordListHesitateTime.Name) as SliderWidget).GetState(SliderWidget.SliderUnitsThousandths);
            prefs.MenuDialogScanTime = Common.AppPreferences.MenuDialogScanTime = (rootWidget.Finder.FindChild(tbTabScanTime.Name) as SliderWidget).GetState(SliderWidget.SliderUnitsThousandths);
            prefs.FirstRepeatTime = Common.AppPreferences.FirstRepeatTime = (rootWidget.Finder.FindChild(tbFirstRepeatTime.Name) as SliderWidget).GetState(SliderWidget.SliderUnitsThousandths);

            return prefs;
        }

        /// <summary>
        /// Initialize the controls on the form based on
        /// the corresponding values in the preferences
        /// </summary>
        /// <param name="prefs">ACAT preferences</param>
        private void initWidgetSettings(ACATPreferences prefs)
        {
            var rootWidget = PanelCommon.RootWidget;

            (rootWidget.Finder.FindChild(pbSelectingClick.Name) as CheckBoxWidget).SetState(prefs.SelectClick);

            (rootWidget.Finder.FindChild(tbEveryHalf.Name) as SliderWidget).SetState(prefs.GridScanIterations, SliderWidget.SliderUnitsOnes);
            (rootWidget.Finder.FindChild(tbEveryRow.Name) as SliderWidget).SetState(prefs.RowScanIterations, SliderWidget.SliderUnitsOnes);
            (rootWidget.Finder.FindChild(tbEveryColumn.Name) as SliderWidget).SetState(prefs.ColumnScanIterations, SliderWidget.SliderUnitsOnes);
            (rootWidget.Finder.FindChild(tbWordPrediction.Name) as SliderWidget).SetState(prefs.WordPredictionScanIterations, SliderWidget.SliderUnitsOnes);
            (rootWidget.Finder.FindChild(tbStripScanner.Name) as SliderWidget).SetState(prefs.StripScannerColumnIterations, SliderWidget.SliderUnitsOnes);

            (rootWidget.Finder.FindChild(tbAcceptTime.Name) as SliderWidget).SetState(prefs.MinActuationHoldTime, SliderWidget.SliderUnitsThousandths);
            (rootWidget.Finder.FindChild(tbSteppingTime.Name) as SliderWidget).SetState(prefs.ScanTime, SliderWidget.SliderUnitsThousandths);
            (rootWidget.Finder.FindChild(tbHesitateTime.Name) as SliderWidget).SetState(prefs.FirstPauseTime, SliderWidget.SliderUnitsThousandths);
            (rootWidget.Finder.FindChild(tbWordListHesitateTime.Name) as SliderWidget).SetState(prefs.WordPredictionFirstPauseTime, SliderWidget.SliderUnitsThousandths);
            (rootWidget.Finder.FindChild(tbTabScanTime.Name) as SliderWidget).SetState(prefs.MenuDialogScanTime, SliderWidget.SliderUnitsThousandths);
            (rootWidget.Finder.FindChild(tbFirstRepeatTime.Name) as SliderWidget).SetState(prefs.FirstRepeatTime, SliderWidget.SliderUnitsThousandths);
        }

        /// <summary>
        /// Loads default settings from the preferences file
        /// </summary>
        private void loadDefaultSettings()
        {
            if (DialogUtils.Confirm(this, R.GetString("RestoreDefaultSettings")))
            {
                // get entire default file and just set those settings that belong to this preferences screen
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
            lblNumberofTimes.Text = R.GetString(lblNumberofTimes.Text);
            lblScanTimes.Text = R.GetString(lblScanTimes.Text);
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
        /// Saves the settings and quits the dialog
        /// </summary>
        private void saveSettingsAndQuit()
        {
            if (_isDirty && DialogUtils.Confirm(this, R.GetString("SaveSettings")))
            {
                getSettingsFromUI().Save();

                _isDirty = false;
                Common.AppPreferences.NotifyPreferencesChanged();
            }

            Windows.CloseForm(this);
        }

        /// <summary>
        /// Form is closing. Releases resources
        /// </summary>
        private void ScannerSettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _dialogCommon.OnClosing();
        }

        /// <summary>
        /// Form has been loaded. Initialize
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
        /// User changed some setting on the screen. Set
        ///  the dirty flag to indicate this
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void widget_EvtValueChanged(object sender, WidgetEventArgs e)
        {
            _isDirty = true;
        }
    }
}