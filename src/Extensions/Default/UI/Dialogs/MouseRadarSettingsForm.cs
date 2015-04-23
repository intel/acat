////////////////////////////////////////////////////////////////////////////
// <copyright file="MouseRadarSettingsForm.cs" company="Intel Corporation">
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
    /// Dialog to set the mouse radar settings.  This
    /// includes the speed of the radar, and the speed
    /// of mouse movement and the number of sweeps etc
    /// </summary>

    [DescriptorAttribute("FC7F284B-2C89-4F21-A096-DCFE1714BC80", "MouseRadarSettingsForm",
                            "Mouse Radar Settings Dialog")]
    public partial class MouseRadarSettingsForm : Form, IDialogPanel
    {
        /// <summary>
        /// The DialogCommon object
        /// </summary>
        private readonly DialogCommon _dialogCommon;

        /// <summary>
        /// Did any of the settings change
        /// </summary>
        private bool isDirty;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public MouseRadarSettingsForm()
        {
            InitializeComponent();

            _dialogCommon = new DialogCommon(this);

            if (!_dialogCommon.Initialize())
            {
                Log.Debug("Initialization error");
            }

            initWidgetSettings(Common.AppPreferences);

            Load += MouseRadarSettingsForm_Load;
            FormClosing += MouseRadarSettingsForm_FormClosing;
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
        /// Set the style of this form
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
        /// Set the state of all the controls based on the
        /// settings
        /// </summary>
        /// <param name="prefs">ACAT settings</param>
        private void initWidgetSettings(ACATPreferences prefs)
        {
            Widget rootWidget = _dialogCommon.GetRootWidget();

            WidgetUtils.SetSliderState(rootWidget, tbRotatingSpeed.Name, prefs.MouseRadarRotatingSpeed, WidgetUtils.SliderUnitsOnes);
            WidgetUtils.SetSliderState(rootWidget, tbRotatingSweeps.Name, prefs.MouseRadarRotatingSweeps, WidgetUtils.SliderUnitsOnes);
            WidgetUtils.SetSliderState(rootWidget, tbRadialSpeed.Name, prefs.MouseRadarRadialSpeed, WidgetUtils.SliderUnitsOnes);
            WidgetUtils.SetSliderState(rootWidget, tbRadialSweeps.Name, prefs.MouseRadarRadialSweeps, WidgetUtils.SliderUnitsOnes);
            WidgetUtils.SetSliderState(rootWidget, tbLineWidth.Name, prefs.MouseRadarLineWidth, WidgetUtils.SliderUnitsOnes);

            WidgetUtils.SetCheckBoxWidgetState(rootWidget, pbStartFromLastCursorPos.Name, prefs.MouseRadarStartFromLastCursorPos);
        }

        /// <summary>
        /// Restore default settings from the preferences file
        /// </summary>
        private void loadDefaultSettings()
        {
            if (DialogUtils.Confirm(this, "Restore default settings?"))
            {
                initWidgetSettings(ACATPreferences.LoadDefaultSettings());
                isDirty = true;
            }
        }

        /// <summary>
        /// Form is closing. Release resources
        /// </summary>
        private void MouseRadarSettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _dialogCommon.OnClosing();
        }

        /// <summary>
        /// Form has been loaded
        /// </summary>
        private void MouseRadarSettingsForm_Load(object sender, EventArgs e)
        {
            _dialogCommon.OnLoad();

            subscribeToEvents();

            _dialogCommon.GetAnimationManager().Start(_dialogCommon.GetRootWidget());
        }

        /// <summary>
        /// If dirty, confirm with user and then quit
        /// </summary>
        private void quit()
        {
            bool quit = true;

            if (isDirty)
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
        /// Save settings and close the dialog. Confirm with the user first
        /// </summary>
        private void saveSettingsAndQuit()
        {
            if (isDirty && DialogUtils.Confirm(this, "Save settings?"))
            {
                updateSettingsFromUI().Save();

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
        /// Update settings based on the values set by the
        /// user in the form
        /// </summary>
        private ACATPreferences updateSettingsFromUI()
        {
            var prefs = ACATPreferences.Load();

            Widget rootWidget = _dialogCommon.GetRootWidget();

            prefs.MouseRadarRotatingSpeed = Common.AppPreferences.MouseRadarRotatingSpeed = WidgetUtils.GetSliderState(rootWidget, tbRotatingSpeed.Name, WidgetUtils.SliderUnitsOnes);
            prefs.MouseRadarRotatingSweeps = Common.AppPreferences.MouseRadarRotatingSweeps = WidgetUtils.GetSliderState(rootWidget, tbRotatingSweeps.Name, WidgetUtils.SliderUnitsOnes);
            prefs.MouseRadarRadialSpeed = Common.AppPreferences.MouseRadarRadialSpeed = WidgetUtils.GetSliderState(rootWidget, tbRadialSpeed.Name, WidgetUtils.SliderUnitsOnes);
            prefs.MouseRadarRadialSweeps = Common.AppPreferences.MouseRadarRadialSweeps = WidgetUtils.GetSliderState(rootWidget, tbRadialSweeps.Name, WidgetUtils.SliderUnitsOnes);
            prefs.MouseRadarLineWidth = Common.AppPreferences.MouseRadarLineWidth = WidgetUtils.GetSliderState(rootWidget, tbLineWidth.Name, WidgetUtils.SliderUnitsOnes);

            prefs.MouseRadarStartFromLastCursorPos = Common.AppPreferences.MouseRadarStartFromLastCursorPos = WidgetUtils.GetCheckBoxWidgetState(rootWidget, pbStartFromLastCursorPos.Name);

            return prefs;
        }

        /// <summary>
        /// User changed something on the form. Set the dirty flag
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void widget_EvtValueChanged(object sender, WidgetEventArgs e)
        {
            isDirty = true;
        }
    }
}