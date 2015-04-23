////////////////////////////////////////////////////////////////////////////
// <copyright file="ScannerSettingsForm.cs" company="Intel Corporation">
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
    /// Sets the scanner settings such as the various timings,
    /// number of iterations for scanning etc.
    /// </summary>
    [DescriptorAttribute("3BC26865-9D90-4DFD-BFAB-D7E69DDFA789", "ScannerSettingsForm",
                            "Mute Settings Dialog")]
    public partial class ScannerSettingsForm : Form, IDialogPanel
    {
        /// <summary>
        /// The DialogCommon object
        /// </summary>
        private readonly DialogCommon _dialogCommon;

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

            _dialogCommon = new DialogCommon(this);

            if (!_dialogCommon.Initialize())
            {
                Log.Debug("Initialization error");
            }

            initWidgetSettings(Common.AppPreferences);

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
        /// Get the values from the form and update the settings
        /// </summary>
        private ACATPreferences getSettingsFromUI()
        {
            var rootWidget = _dialogCommon.GetRootWidget();
            var prefs = ACATPreferences.Load();

            prefs.SelectClick = Common.AppPreferences.SelectClick = WidgetUtils.GetCheckBoxWidgetState(rootWidget, pbSelectingClick.Name);

            prefs.HalfScanIterations = Common.AppPreferences.HalfScanIterations = WidgetUtils.GetSliderState(rootWidget, tbEveryHalf.Name, WidgetUtils.SliderUnitsOnes);
            prefs.RowScanIterations = Common.AppPreferences.RowScanIterations = WidgetUtils.GetSliderState(rootWidget, tbEveryRow.Name, WidgetUtils.SliderUnitsOnes);
            prefs.ColumnScanIterations = Common.AppPreferences.ColumnScanIterations = WidgetUtils.GetSliderState(rootWidget, tbEveryColumn.Name, WidgetUtils.SliderUnitsOnes);
            prefs.WordPredictionScanIterations = Common.AppPreferences.WordPredictionScanIterations = WidgetUtils.GetSliderState(rootWidget, tbWordPrediction.Name, WidgetUtils.SliderUnitsOnes);

            prefs.AcceptTime = Common.AppPreferences.AcceptTime = WidgetUtils.GetSliderState(rootWidget, tbAcceptTime.Name, WidgetUtils.SliderUnitsThousandths);
            prefs.SteppingTime = Common.AppPreferences.SteppingTime = WidgetUtils.GetSliderState(rootWidget, tbSteppingTime.Name, WidgetUtils.SliderUnitsThousandths);
            prefs.HesitateTime = Common.AppPreferences.HesitateTime = WidgetUtils.GetSliderState(rootWidget, tbHesitateTime.Name, WidgetUtils.SliderUnitsThousandths);
            prefs.WordPredictionHesitateTime = Common.AppPreferences.WordPredictionHesitateTime = WidgetUtils.GetSliderState(rootWidget, tbWordListHesitateTime.Name, WidgetUtils.SliderUnitsThousandths);
            prefs.TabScanTime = Common.AppPreferences.TabScanTime = WidgetUtils.GetSliderState(rootWidget, tbTabScanTime.Name, WidgetUtils.SliderUnitsThousandths);
            prefs.FirstRepeatTime = Common.AppPreferences.FirstRepeatTime = WidgetUtils.GetSliderState(rootWidget, tbFirstRepeatTime.Name, WidgetUtils.SliderUnitsThousandths);

            return prefs;
        }

        /// <summary>
        /// Initialize the controls on the form based on
        /// the corresponding values in the preferences
        /// </summary>
        /// <param name="prefs">ACAT preferences</param>
        private void initWidgetSettings(ACATPreferences prefs)
        {
            // TOGGLE IMAGE BUTTON KEYS USED FOR BOTTOM-LEFT PANEL
            var rootWidget = _dialogCommon.GetRootWidget();

            WidgetUtils.SetCheckBoxWidgetState(rootWidget, pbSelectingClick.Name, prefs.SelectClick);

            WidgetUtils.SetSliderState(rootWidget, tbEveryHalf.Name, prefs.HalfScanIterations, WidgetUtils.SliderUnitsOnes);
            WidgetUtils.SetSliderState(rootWidget, tbEveryRow.Name, prefs.RowScanIterations, WidgetUtils.SliderUnitsOnes);
            WidgetUtils.SetSliderState(rootWidget, tbEveryColumn.Name, prefs.ColumnScanIterations, WidgetUtils.SliderUnitsOnes);
            WidgetUtils.SetSliderState(rootWidget, tbWordPrediction.Name, prefs.WordPredictionScanIterations, WidgetUtils.SliderUnitsOnes);

            WidgetUtils.SetSliderState(rootWidget, tbAcceptTime.Name, prefs.AcceptTime, WidgetUtils.SliderUnitsThousandths);
            WidgetUtils.SetSliderState(rootWidget, tbSteppingTime.Name, prefs.SteppingTime, WidgetUtils.SliderUnitsThousandths);
            WidgetUtils.SetSliderState(rootWidget, tbHesitateTime.Name, prefs.HesitateTime, WidgetUtils.SliderUnitsThousandths);
            WidgetUtils.SetSliderState(rootWidget, tbWordListHesitateTime.Name, prefs.WordPredictionHesitateTime, WidgetUtils.SliderUnitsThousandths);
            WidgetUtils.SetSliderState(rootWidget, tbTabScanTime.Name, prefs.TabScanTime, WidgetUtils.SliderUnitsThousandths);
            WidgetUtils.SetSliderState(rootWidget, tbFirstRepeatTime.Name, prefs.FirstRepeatTime, WidgetUtils.SliderUnitsThousandths);
        }

        /// <summary>
        /// Loads default settings from the preferences file
        /// </summary>
        private void loadDefaultSettings()
        {
            if (DialogUtils.Confirm(this, "Restore default settings?"))
            {
                // get entire default file and just set those settings that belong to this preferences screen
                initWidgetSettings(ACATPreferences.LoadDefaultSettings());
                _isDirty = true;
            }
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
                Windows.CloseForm(this);
            }
        }

        /// <summary>
        /// Saves the settings and quits the dialog
        /// </summary>
        private void saveSettingsAndQuit()
        {
            if (_isDirty && DialogUtils.Confirm(this, "Save settings?"))
            {
                getSettingsFromUI().Save();

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
        /// Form has been loaded. Initialize
        /// </summary>
        private void ScannerSettingsForm_Load(object sender, EventArgs e)
        {
            _dialogCommon.OnLoad();

            subscribeToEvents();

            _dialogCommon.GetAnimationManager().Start(_dialogCommon.GetRootWidget());
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