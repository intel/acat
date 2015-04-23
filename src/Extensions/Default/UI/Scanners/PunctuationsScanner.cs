////////////////////////////////////////////////////////////////////////////
// <copyright file="PunctuationsScanner.cs" company="Intel Corporation">
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
    /// Scanner that has all the punctuations, numbers and function
    /// keys (F1 through F10)
    /// </summary>
    [DescriptorAttribute("D7F899FF-73AD-4B32-A0CA-6E0FDA83CC0A", "PunctuationsScanner", "Numbers and Punctuations")]
    public partial class PunctuationsScanner : Form, IScannerPanel, ISupportsStatusBar
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
        public PunctuationsScanner()
        {
            InitializeComponent();
            createStatusBar();
            Load += PunctuationsScanner_Load;
            FormClosing += PunctuationsScanner_FormClosing;
            PanelClass = PanelClasses.Punctuation;
            _dispatcher = new Dispatcher(this);
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
        /// Show this scanner without setting focus to it
        /// </summary>
        protected override bool ShowWithoutActivation
        {
            get { return true; }
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
        /// Creates the scanner status bar object
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
        private void PunctuationsScanner_FormClosing(object sender, FormClosingEventArgs e)
        {
            KeyStateTracker.ClearFunc();
            _scannerCommon.OnClosing();
            _scannerCommon.Dispose();
        }

        /// <summary>
        /// Form loader. Initialize and start animation
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arg</param>
        private void PunctuationsScanner_Load(object sender, EventArgs e)
        {
            _scannerCommon.OnLoad();

            _scannerCommon.GetAnimationManager().Start(_scannerCommon.GetRootWidget());
        }

        /// <summary>
        /// Handles all  the commands for the scanner
        /// </summary>
        private class CommandHandler : RunCommandHandler
        {
            /// <summary>
            /// Initializes an instance of the handler
            /// </summary>
            /// <param name="cmd">the command</param>
            public CommandHandler(String cmd)
                : base(cmd)
            {
            }

            /// <summary>
            /// Executes the command
            /// </summary>
            /// <param name="handled">set to true if handled</param>
            /// <returns>true</returns>
            public override bool Execute(ref bool handled)
            {
                handled = true;

                switch (Command)
                {
                    case "CmdFunctionKey":
                        if (KeyStateTracker.IsFuncOn())
                        {
                            KeyStateTracker.ClearFunc();
                        }
                        else
                        {
                            KeyStateTracker.FuncTriggered();
                        }

                        break;

                    case "CmdNumberPeriod":
                        Context.AppAgentMgr.Keyboard.Send(KeyStateTracker.GetExtendedKeys(), '.');
                        break;

                    case "CmdNumberComma":
                        Context.AppAgentMgr.Keyboard.Send(KeyStateTracker.GetExtendedKeys(), ',');
                        break;

                    default:
                        handled = false;
                        break;
                }

                return true;
            }
        }

        /// <summary>
        /// Command dispatcher class that takes care of all the
        /// commands associated with this scanner
        /// </summary>
        private class Dispatcher : DefaultCommandDispatcher
        {
            /// <summary>
            /// Initializes a new instance of the class.
            /// </summary>
            /// <param name="panel">the scanner</param>
            public Dispatcher(IScannerPanel panel)
                : base(panel)
            {
                Commands.Add(new CommandHandler("CmdFunctionKey"));
                Commands.Add(new CommandHandler("CmdNumberPeriod"));
                Commands.Add(new CommandHandler("CmdNumberComma"));
                Commands.Add(new FKeyHandler("1"));
                Commands.Add(new FKeyHandler("2"));
                Commands.Add(new FKeyHandler("3"));
                Commands.Add(new FKeyHandler("4"));
                Commands.Add(new FKeyHandler("5"));
                Commands.Add(new FKeyHandler("6"));
                Commands.Add(new FKeyHandler("7"));
                Commands.Add(new FKeyHandler("8"));
                Commands.Add(new FKeyHandler("9"));
                Commands.Add(new FKeyHandler("0"));
            }
        }

        /// <summary>
        /// Simulates function key presses F1 through F10
        /// </summary>
        private class FKeyHandler : FunctionKeyHandler
        {
            /// <summary>
            /// Initializes a new instance of the class.
            /// </summary>
            /// <param name="cmd">The command</param>
            public FKeyHandler(String cmd)
                : base(cmd)
            {
            }

            /// <summary>
            /// Executes the command
            /// </summary>
            /// <param name="handled">true if handled</param>
            /// <returns>true on success</returns>
            public override bool Execute(ref bool handled)
            {
                var form = Dispatcher.Scanner.Form as PunctuationsScanner;

                switch (Command)
                {
                    case "1":
                    case "2":
                    case "3":
                    case "4":
                    case "5":
                    case "6":
                    case "7":
                    case "8":
                    case "9":
                    case "0":
                        if (KeyStateTracker.IsFuncOn())
                        {
                            sendFunctionKey("F" + Command);

                            KeyStateTracker.ClearFunc();
                            KeyStateTracker.ClearShift();
                            KeyStateTracker.ClearAlt();
                            KeyStateTracker.ClearCtrl();
                        }
                        else
                        {
                            if (form._scannerCommon.ActuatedWidget != null)
                            {
                                form._scannerCommon.ActuateButton(form._scannerCommon.ActuatedWidget, Command[0]);
                            }
                        }

                        handled = true;
                        break;

                    default:
                        handled = false;
                        break;
                }

                return true;
            }
        }
    }
}