////////////////////////////////////////////////////////////////////////////
// <copyright file="AlphabetScanner.cs" company="Intel Corporation">
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
using System.Diagnostics.CodeAnalysis;
using System.Security.Permissions;
using System.Windows.Forms;
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
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

namespace ACAT.Extensions.Default.UI.Scanners
{
    /// <summary>
    /// This form is the main alphabet scanner for ACAT.  It
    /// has all the letters, word prediction list and also has
    /// entries into the main menu, talk window, and other scanners
    /// such as the cursor scanner, punctuations scanner and the
    /// mouse scanner.  User can access contextual menus for apps
    /// from this scanner as well.
    /// </summary>
    [DescriptorAttribute("D1C82679-8E84-4F1B-BB93-ED55EFC744CC", "AlphabetScanner", "Alphabet Scanner")]
    public partial class AlphabetScanner : Form, IScannerPanel, IScannerPreview, ISupportsStatusBar
    {
        /// <summary>
        /// The AlphabetScannerCommon object. Has a number of
        /// helper functions
        /// </summary>
        private AlphabetScannerCommon _alphabetScannerCommon;

        /// <summary>
        /// The status bar object of the scanner. It shows the status
        /// of the modifier keys such as Shift, Ctrl, Alt.
        /// </summary>
        private ScannerStatusBar _statusBar;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public AlphabetScanner()
        {
            InitializeComponent();
            createStatusBar();
            FormClosing += AlphabetScanner_FormClosing;
        }

        /// <summary>
        /// Gets the command dispatcher object
        /// </summary>
        public RunCommandDispatcher CommandDispatcher
        {
            get { return _alphabetScannerCommon.Dispatcher; }
        }

        /// <summary>
        /// Gets the descriptor for this class
        /// </summary>
        public IDescriptor Descriptor
        {
            get { return DescriptorAttribute.GetDescriptor(GetType()); }
        }

        /// <summary>
        /// Gets this form object
        /// </summary>
        public Form Form
        {
            get { return this; }
        }

        /// <summary>
        /// Gets the panel class for the scanner
        /// </summary>
        public String PanelClass
        {
            get { return _alphabetScannerCommon.PanelClass; }
        }

        /// <summary>
        /// Gets or sets the preview mode. This is used in
        /// the design mode where the user can zoom in/out
        /// the scanner
        /// </summary>
        public bool PreviewMode
        {
            get
            {
                return _alphabetScannerCommon.PreviewMode;
            }

            set
            {
                _alphabetScannerCommon.PreviewMode = value;
            }
        }

        /// <summary>
        /// Gets the scanner common object
        /// </summary>
        public ScannerCommon ScannerCommon
        {
            get { return _alphabetScannerCommon.ScannerCommon; }
        }

        /// <summary>
        /// Gets the status bar control for this scanner
        /// </summary>
        public ScannerStatusBar ScannerStatusBar
        {
            get { return _statusBar; }
        }

        /// <summary>
        /// Gets the synch object
        /// </summary>
        public SyncLock SyncObj
        {
            get { return _alphabetScannerCommon.SyncObj; }
        }

        /// <summary>
        /// Gets the text controller object for this scanner
        /// </summary>
        public ITextController TextController
        {
            get { return _alphabetScannerCommon.TextController; }
        }

        /// <summary>
        /// Set form styles
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
                return Windows.SetFormStyles(base.CreateParams);
            }
        }

        /// <summary>
        /// Invoked to check if a scanner button should be enabled.  Uses context
        /// to determine the 'enabled' state.
        /// </summary>
        /// <param name="arg">info about the scanner button</param>
        public bool CheckWidgetEnabled(CheckEnabledArgs arg)
        {
            return _alphabetScannerCommon.CheckWidgetEnabled(arg);
        }

        /// <summary>
        /// Intitialize the class
        /// </summary>
        /// <param name="startupArg">startup params</param>
        /// <returns>true on cussess</returns>
        public bool Initialize(StartupArg startupArg)
        {
            _alphabetScannerCommon = new AlphabetScannerCommon(this);
            _alphabetScannerCommon.EvtFormatPreditionWord += _homeScreenCommon_EvtFormatPreditionWord;
            return _alphabetScannerCommon.Initialize(startupArg);
        }

        /// <summary>
        /// Invoked when the focus changes either in the active window or when the
        /// active window itself changes.
        /// </summary>
        /// <param name="monitorInfo">Info about focused element</param>
        public void OnFocusChanged(WindowActivityMonitorInfo monitorInfo)
        {
            _alphabetScannerCommon.OnFocusChanged(monitorInfo);
        }

        /// <summary>
        /// Pauses animations
        /// </summary>
        public void OnPause()
        {
            _alphabetScannerCommon.OnPause();
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="eventArg"></param>
        /// <returns></returns>
        public bool OnQueryPanelChange(PanelRequestEventArgs eventArg)
        {
            return true;
        }

        /// <summary>
        /// Resumes animation
        /// </summary>
        public void OnResume()
        {
            _alphabetScannerCommon.OnResume();
        }

        /// <summary>
        /// Triggered when the user actuates a widget
        /// </summary>
        /// <param name="widget">widget actuated</param>
        /// <param name="handled">was this handled?</param>
        public void OnWidgetActuated(Widget widget, ref bool handled)
        {
            _alphabetScannerCommon.OnWidgetActuated(widget, ref handled);
        }

        /// <summary>
        /// Restores default zoom and position settings
        /// </summary>
        public void RestoreDefaults()
        {
            _alphabetScannerCommon.RestoreDefaults();
        }

        /// <summary>
        /// Saves the current zoom settings
        /// </summary>
        public void SaveSettings()
        {
            _alphabetScannerCommon.SaveSettings();
        }

        /// <summary>
        /// Zooms out the scanner
        /// </summary>
        public void ScaleDown()
        {
            _alphabetScannerCommon.ScaleDown();
        }

        /// <summary>
        /// Zooms in the scanner
        /// </summary>
        public void ScaleUp()
        {
            _alphabetScannerCommon.ScaleUp();
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="widget"></param>
        public void SetTargetControl(Form parent, Widget widget)
        {
        }

        /// <summary>
        /// Form is closing. Release resources
        /// </summary>
        /// <param name="e">closing param</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _alphabetScannerCommon.OnFormClosing(e);
            base.OnFormClosing(e);
        }

        /// <summary>
        /// Window procedure
        /// </summary>
        /// <param name="m"></param>
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        protected override void WndProc(ref Message m)
        {
            _alphabetScannerCommon.WndProc(ref m);
            base.WndProc(ref m);
        }

        /// <summary>
        /// Handler to format the word prediction words.  Add
        /// an index number prefix.
        /// </summary>
        /// <param name="index">index of the word</param>
        /// <param name="word">word</param>
        /// <returns>formatted string</returns>
        private string _homeScreenCommon_EvtFormatPreditionWord(int index, string word)
        {
            return Common.AppPreferences.PrefixNumbersInWordPredictionList ?
                        (((index == 10) ? 0 : index) + ": " + word) :
                        word;
        }

        /// <summary>
        /// Release resources and stop threads/timers
        /// </summary>
        private void AlphabetScanner_FormClosing(object sender, FormClosingEventArgs e)
        {
            _alphabetScannerCommon.OnClosing(sender, e);
        }

        /// <summary>
        /// The form has loaded.  Start the animation sequence
        /// </summary>
        private void AlphabetScanner_Load(object sender, EventArgs e)
        {
            _alphabetScannerCommon.OnLoad(sender, e);
        }

        /// <summary>
        /// Creates the status bar for the form
        /// </summary>
        private void createStatusBar()
        {
            if (_statusBar != null)
            {
                return;
            }

            _statusBar = new ScannerStatusBar
            {
                AltStatus = BAltStatus,
                CtrlStatus = BCtrlStatus,
                FuncStatus = BFuncStatus,
                ShiftStatus = BShiftStatus,
                LockStatus = BLockStatus
            };
        }
    }
}