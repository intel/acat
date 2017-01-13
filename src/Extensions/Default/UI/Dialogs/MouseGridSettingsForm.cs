////////////////////////////////////////////////////////////////////////////
// <copyright file="MouseGridSettingsForm.cs" company="Intel Corporation">
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
    /// Dialog to set the mouse grid settings.  This
    /// includes the speed of the grid, the speed
    /// of mouse movement, the number of cycles
    /// </summary>
    [DescriptorAttribute("71049A94-0435-4739-AE2C-77E2BD3CB0F0",
                        "MouseGridSettingsForm",
                        "Mouse Grid Settings Dialog")]
    public partial class MouseGridSettingsForm : Form, IDialogPanel
    {
        /// <summary>
        /// The DialogCommon object
        /// </summary>
        private DialogCommon _dialogCommon;

        /// <summary>
        /// Were any of the settings changed?
        /// </summary>
        private bool _isDirty;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public MouseGridSettingsForm()
        {
            InitializeComponent();

            Load += MouseGridSettingsForm_Load;
            FormClosing += MouseGridSettingsForm_FormClosing;
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
            bool retVal = _dialogCommon.Initialize(startupArg);

            if (retVal)
            {
                initWidgetSettings(Common.AppPreferences);
            }

            return retVal;
        }

        /// <summary>
        /// Invoked when a widget is actuated
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
        /// Form closing. Release resources
        /// </summary>
        /// <param name="e">eent arg</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _dialogCommon.OnFormClosing(e);
            base.OnFormClosing(e);
        }

        /// <summary>
        /// Window proc
        /// </summary>
        /// <param name="m">window message</param>
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        protected override void WndProc(ref Message m)
        {
            _dialogCommon.HandleWndProc(m);
            base.WndProc(ref m);
        }

        /// <summary>
        /// Updates settings based on the values set by the
        /// user in the form.  Returns a ACATPreferences object
        /// with the new values.
        /// </summary>
        /// <returns>ACATPreferences object</returns>
        private ACATPreferences getSettingsFromUI()
        {
            var rootWidget = PanelCommon.RootWidget;
            var prefs = ACATPreferences.Load();

            prefs.MouseGridRectangleSpeed = Common.AppPreferences.MouseGridRectangleSpeed = (rootWidget.Finder.FindChild(tbRectangleSpeed.Name) as SliderWidget).GetState(SliderWidget.SliderUnitsOnes);
            prefs.MouseGridRectangleCycles = Common.AppPreferences.MouseGridRectangleCycles = (rootWidget.Finder.FindChild(tbRectangleCycles.Name) as SliderWidget).GetState(SliderWidget.SliderUnitsOnes);
            prefs.MouseGridLineSpeed = Common.AppPreferences.MouseGridLineSpeed = (rootWidget.Finder.FindChild(tbLineSpeed.Name) as SliderWidget).GetState(SliderWidget.SliderUnitsOnes);
            prefs.MouseGridLineCycles = Common.AppPreferences.MouseGridLineCycles = (rootWidget.Finder.FindChild(tbLineCycles.Name) as SliderWidget).GetState(SliderWidget.SliderUnitsOnes);
            prefs.MouseGridLineThickness = Common.AppPreferences.MouseGridLineThickness = (rootWidget.Finder.FindChild(tbLineThickness.Name) as SliderWidget).GetState(SliderWidget.SliderUnitsOnes);

            prefs.MouseGridEnableVerticalRectangleScan = Common.AppPreferences.MouseGridEnableVerticalRectangleScan = (rootWidget.Finder.FindChild(pbEnableVerticalRectScan.Name) as CheckBoxWidget).GetState();

            return prefs;
        }

        /// <summary>
        /// Sets the state of all the controls based on the
        /// settings in the prefs parameter
        /// </summary>
        /// <param name="prefs">ACAT settings object</param>
        private void initWidgetSettings(ACATPreferences prefs)
        {
            var rootWidget = PanelCommon.RootWidget;

            (rootWidget.Finder.FindChild(tbRectangleSpeed.Name) as SliderWidget).SetState(prefs.MouseGridRectangleSpeed, SliderWidget.SliderUnitsOnes);
            (rootWidget.Finder.FindChild(tbRectangleCycles.Name) as SliderWidget).SetState(prefs.MouseGridRectangleCycles, SliderWidget.SliderUnitsOnes);
            (rootWidget.Finder.FindChild(tbLineSpeed.Name) as SliderWidget).SetState(prefs.MouseGridLineSpeed, SliderWidget.SliderUnitsOnes);
            (rootWidget.Finder.FindChild(tbLineCycles.Name) as SliderWidget).SetState(prefs.MouseGridLineCycles, SliderWidget.SliderUnitsOnes);
            (rootWidget.Finder.FindChild(tbLineThickness.Name) as SliderWidget).SetState(prefs.MouseGridLineThickness, SliderWidget.SliderUnitsOnes);

            (rootWidget.Finder.FindChild(pbEnableVerticalRectScan.Name) as CheckBoxWidget).SetState(prefs.MouseGridEnableVerticalRectangleScan);
        }

        /// <summary>
        /// Restores default settings from the preferences file
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
        /// Form is closing. Release resources
        /// </summary>
        private void MouseGridSettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _dialogCommon.OnClosing();
        }

        /// <summary>
        /// Form has been loaded. Initialize
        /// </summary>
        private void MouseGridSettingsForm_Load(object sender, EventArgs e)
        {
            populateFormText();

            _dialogCommon.OnLoad();

            subscribeToEvents();

            PanelCommon.AnimationManager.Start(PanelCommon.RootWidget);
        }

        /// <summary>
        /// Populate form controls text from Language resource
        /// </summary>
        private void populateFormText()
        {
            panelTitle.Text = R.GetString(panelTitle.Text);
            lblEnableVerticalRectScan.Text = R.GetString(lblEnableVerticalRectScan.Text);
        }

        /// <summary>
        /// If dirty, confirm with user and then quit
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
        /// Saves settings and close the dialog. Confirms with the user first
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
        /// User changed something on the form. Sets the dirty flag
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void widget_EvtValueChanged(object sender, WidgetEventArgs e)
        {
            _isDirty = true;
        }
    }
}