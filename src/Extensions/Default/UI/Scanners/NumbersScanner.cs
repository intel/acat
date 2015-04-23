////////////////////////////////////////////////////////////////////////////
// <copyright file="NumbersScanner.cs" company="Intel Corporation">
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
using ACAT.Lib.Extension.CommandHandlers;

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
    /// Scanner that allows entry of numbers
    /// </summary>
    [DescriptorAttribute("EABAEEA7-EABF-4499-B571-2D8F29ABFF09", "NumbersScanner", "Numbers Scanner")]
    public partial class NumbersScanner : Form, IScannerPanel, ISupportsStatusBar
    {
        /// <summary>
        /// The Command dispatcher object
        /// </summary>
        private readonly DefaultCommandDispatcher _dispatcher;

        /// <summary>
        /// The ScannerCommon object
        /// </summary>
        private ScannerCommon _scannerCommon;

        /// <summary>
        /// The ScannerHelper object
        /// </summary>
        private ScannerHelper _scannerHelper;

        /// <summary>
        /// The StatusBar for this scanner
        /// </summary>
        private ScannerStatusBar _statusBar;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public NumbersScanner()
        {
            InitializeComponent();
            createStatusBar();
            Load += NumbersScanner_Load;
            FormClosing += NumbersScanner_FormClosing;
            PanelClass = PanelClasses.Number;
            _dispatcher = new DefaultCommandDispatcher(this);
        }

        /// <summary>
        /// Gets the command dispatcher
        /// </summary>
        public RunCommandDispatcher CommandDispatcher
        {
            get { return _dispatcher; }
        }

        /// <summary>
        /// Gets the descriptor for this class
        /// </summary>
        public IDescriptor Descriptor
        {
            get { return DescriptorAttribute.GetDescriptor(GetType()); }
        }

        /// <summary>
        /// Gets the scanner form object
        /// </summary>
        public Form Form
        {
            get { return this; }
        }

        /// <summary>
        /// Gets the scanner name
        /// </summary>
        public String PanelClass { get; private set; }

        /// <summary>
        /// Gets the ScannerCommon object
        /// </summary>
        public ScannerCommon ScannerCommon
        {
            get { return _scannerCommon; }
        }

        /// <summary>
        /// Gets the statusbar object for the scanner
        /// </summary>
        public ScannerStatusBar ScannerStatusBar
        {
            get { return _statusBar; }
        }

        /// <summary>
        /// Gets the snchronization object
        /// </summary>
        public SyncLock SyncObj
        {
            get { return _scannerCommon.SyncObj; }
        }

        /// <summary>
        /// Gets the TextController object
        /// </summary>
        public ITextController TextController
        {
            get { return _scannerCommon.TextController; }
        }

        /// <summary>
        /// Sets the window styhle
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
        /// Checks which of the widgets should be enabled depending
        /// on the context
        /// </summary>
        /// <param name="arg">widget info</param>
        /// <returns>true on success</returns>
        public bool CheckWidgetEnabled(CheckEnabledArgs arg)
        {
            return _scannerHelper.CheckWidgetEnabled(arg);
        }

        /// <summary>
        /// Initialzes the scanner
        /// </summary>
        /// <param name="startupArg">Starting arguments</param>
        /// <returns>true on success</returns>
        public bool Initialize(StartupArg startupArg)
        {
            _scannerCommon = new ScannerCommon(this);
            _scannerHelper = new ScannerHelper(this, startupArg);

            if (!_scannerCommon.Initialize(startupArg))
            {
                Log.Debug("Could not initialize form " + Name);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Invoked whenever the focus changes in Windows
        /// </summary>
        /// <param name="monitorInfo">Info about focused window</param>
        public void OnFocusChanged(WindowActivityMonitorInfo monitorInfo)
        {
            _scannerCommon.OnFocusChanged(monitorInfo);
        }

        /// <summary>
        /// Pauses the scanner
        /// </summary>
        public void OnPause()
        {
            Log.Debug();

            _scannerCommon.GetAnimationManager().Pause();

            _scannerCommon.HideScanner();

            _scannerCommon.OnPause();
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
        /// Resumes the scanner
        /// </summary>
        public void OnResume()
        {
            Log.Debug();

            _scannerCommon.GetAnimationManager().Resume();

            _scannerCommon.ShowScanner();

            _scannerCommon.OnResume();
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="widget"></param>
        /// <param name="handled"></param>
        public void OnWidgetActuated(Widget widget, ref bool handled)
        {
            handled = false;
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
        /// Invoked when the form is clsoing. Release resources
        /// </summary>
        /// <param name="e">event args</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _scannerHelper.OnFormClosing(e);
            _scannerCommon.OnFormClosing(e);
            base.OnFormClosing(e);
        }

        /// <summary>
        /// Win proc function
        /// </summary>
        /// <param name="m">Windows message</param>
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        protected override void WndProc(ref Message m)
        {
            _scannerCommon.HandleWndProc(m);
            base.WndProc(ref m);
        }

        /// <summary>
        /// Creates the statusbar object for this scanner
        /// </summary>
        private void createStatusBar()
        {
            if (_statusBar == null)
            {
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

        /// <summary>
        /// Form is closing. Dispose resources
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arg</param>
        private void NumbersScanner_FormClosing(object sender, FormClosingEventArgs e)
        {
            KeyStateTracker.FuncOff();
            _scannerCommon.OnClosing();
            _scannerCommon.Dispose();
        }

        /// <summary>
        /// Form loader.  Initialize, subscribe to events.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arg</param>
        private void NumbersScanner_Load(object sender, EventArgs e)
        {
            _scannerCommon.OnLoad();

            _scannerCommon.GetAnimationManager().Start(_scannerCommon.GetRootWidget());
        }
    }
}