////////////////////////////////////////////////////////////////////////////
// <copyright file="YesNoScanner.cs" company="Intel Corporation">
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
using ACAT.Lib.Core.ActuatorManagement;
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.InputActuators;
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

namespace ACAT.Extensions.Default.UI.ContextMenus
{
    /// <summary>
    /// Scanner that displays a Yes/No dialog with a prompt.
    /// Displays the yes/no text in the scanner. Returns the
    /// choice selected in the "Choice" property
    /// </summary>
    [DescriptorAttribute("B760945E-AA11-4C99-AB62-F03F47F9C76A", "YesNoScanner ", "Yes No Scanner")]
    public partial class YesNoScanner : Form, IScannerPanel, IExtension
    {
        /// <summary>
        /// The widget that represents this form
        /// </summary>
        protected Widget rootWidget;

        /// <summary>
        /// The scannercommon object
        /// </summary>
        protected ScannerCommon scannerCommon;

        /// <summary>
        /// Startup arguments
        /// </summary>
        protected StartupArg startupArg;

        /// <summary>
        /// Contains startup command arguments
        /// </summary>
        protected object startupCommandArg;

        /// <summary>
        /// The command dispatcher object
        /// </summary>
        private readonly Dispatcher _dispatcher;

        /// <summary>
        /// Provides access to properties and methods in this class
        /// </summary>
        private readonly ExtensionInvoker _invoker;

        /// <summary>
        /// The keyboard actuator object
        /// </summary>
        private readonly KeyboardActuator _keyboardActuator;

        /// <summary>
        /// The scanner helper object
        /// </summary>
        private ScannerHelper _scannerHelper;

        /// <summary>
        /// Title of this form
        /// </summary>
        private String _title = String.Empty;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="panelClass">Panel class of the scanner</param>
        /// <param name="panelTitle">Title of the scanner</param>
        public YesNoScanner(String panelClass, String panelTitle)
        {
            InitializeComponent();

            Load += ContextMenu_Load;
            FormClosing += ContextMenu_FormClosing;

            Choice = false;
            _title = panelTitle;
            PanelClass = panelClass;
            Caption = String.Empty;

            _invoker = new ExtensionInvoker(this);
            _dispatcher = new Dispatcher(this);

            var actuator = ActuatorManager.Instance.GetActuator(typeof(KeyboardActuator));
            if (actuator is KeyboardActuator)
            {
                _keyboardActuator = actuator as KeyboardActuator;
                _keyboardActuator.EvtKeyPress += _keyboardActuator_EvtKeyPress;
            }
        }

        /// <summary>
        /// Gets or sets the prompt string
        /// </summary>
        public String Caption { get; set; }

        /// <summary>
        /// Gets or sets the choice. True if yes
        /// </summary>
        public bool Choice { get; set; }

        /// <summary>
        /// Gets the command dispatcher object that handles
        /// commands
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
        /// Gets the form
        /// </summary>
        public Form Form
        {
            get { return this; }
        }

        /// <summary>
        /// Gets the panel class
        /// </summary>
        public String PanelClass { get; protected set; }

        /// <summary>
        /// Gets the scannercommon object
        /// </summary>
        public ScannerCommon ScannerCommon
        {
            get { return scannerCommon; }
        }

        /// <summary>
        /// Gets the synch object
        /// </summary>
        public SyncLock SyncObj
        {
            get { return scannerCommon.SyncObj; }
        }

        /// <summary>
        /// Gets the text controller object
        /// </summary>
        public ITextController TextController
        {
            get { return scannerCommon.TextController; }
        }

        /// <summary>
        /// Set the form styles
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                Log.Debug();
                return Windows.SetFormStyles(base.CreateParams);
            }
        }

        /// <summary>
        /// Don't let the fom take focus
        /// </summary>
        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        /// <summary>
        /// Invoked to check if a scanner button should be enabled.  Uses context
        /// to determine the 'enabled' state.
        /// </summary>
        /// <param name="arg">info about the scanner button</param>
        /// <returns>true on success</returns>
        public bool CheckWidgetEnabled(CheckEnabledArgs arg)
        {
            return _scannerHelper.CheckWidgetEnabled(arg);
        }

        /// <summary>
        /// Returns the extension invoker object
        /// </summary>
        /// <returns>the invoker object</returns>
        public ExtensionInvoker GetInvoker()
        {
            return _invoker;
        }

        /// <summary>
        /// Initialize the class
        /// </summary>
        /// <param name="arg">startup arg</param>
        /// <returns>true on success</returns>
        public bool Initialize(StartupArg arg)
        {
            Log.Debug();

            PanelClass = arg.PanelClass;
            startupCommandArg = arg.Arg;
            startupArg = arg;

            _scannerHelper = new ScannerHelper(this, startupArg);
            scannerCommon = new ScannerCommon(this);

            if (!scannerCommon.Initialize(startupArg))
            {
                Log.Debug("Could not initialize form " + Name);
                return false;
            }

            rootWidget = scannerCommon.GetRootWidget();
            return true;
        }

        /// <summary>
        /// Invoked when the focus changes either in the active window or when the
        /// active window itself changes.
        /// </summary>
        /// <param name="monitorInfo">foreground window info</param>
        public void OnFocusChanged(WindowActivityMonitorInfo monitorInfo)
        {
            scannerCommon.OnFocusChanged(monitorInfo);
        }

        /// <summary>
        /// Pause the animation and hide the scanner
        /// </summary>
        public virtual void OnPause()
        {
            Log.Debug();

            scannerCommon.GetAnimationManager().Pause();

            scannerCommon.HideScanner();

            scannerCommon.OnPause();
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public bool OnQueryPanelChange(PanelRequestEventArgs arg)
        {
            return true;
        }

        /// <summary>
        /// Resumes the animation and shows the scanner
        /// </summary>
        public virtual void OnResume()
        {
            Log.Debug();

            scannerCommon.GetAnimationManager().Resume();

            scannerCommon.ShowScanner();

            scannerCommon.OnResume();
        }

        /// <summary>
        /// Invoked when a widget is actuated
        /// </summary>
        /// <param name="widget">widget</param>
        /// <param name="handled">was this handled</param>
        public virtual void OnWidgetActuated(Widget widget, ref bool handled)
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
        /// Release resources
        /// </summary>
        /// <param name="e">arg</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _scannerHelper.OnFormClosing(e);
            scannerCommon.OnFormClosing(e);

            _keyboardActuator.EvtKeyPress -= _keyboardActuator_EvtKeyPress;
            base.OnFormClosing(e);
        }

        /// <summary>
        /// Win pro
        /// </summary>
        /// <param name="m">windows message</param>
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        protected override void WndProc(ref Message m)
        {
            if (scannerCommon != null)
            {
                scannerCommon.HandleWndProc(m);
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// Key press handler
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void _keyboardActuator_EvtKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 27 || e.KeyChar == 'n' || e.KeyChar == 'N')
            {
                e.Handled = true;
                Choice = false;
                Close();
            }
            else if (e.KeyChar == 'y' || e.KeyChar == 'Y')
            {
                e.Handled = true;
                Choice = true;
                Close();
            }
        }

        /// <summary>
        /// Close the form and dispose off resources
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void ContextMenu_FormClosing(object sender, FormClosingEventArgs e)
        {
            scannerCommon.OnClosing();
            scannerCommon.Dispose();
        }

        /// <summary>
        /// Form loaded. Initialize
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void ContextMenu_Load(object sender, EventArgs e)
        {
            Log.Debug();

            scannerCommon.OnLoad(false);

            var widget = scannerCommon.GetRootWidget().Finder.FindChild("ContextMenuTitle");
            if (widget != null)
            {
                widget.SetText(_title);
            }

            widget = scannerCommon.GetRootWidget().Finder.FindChild("Prompt");
            if (widget != null && !String.IsNullOrEmpty(Caption))
            {
                widget.SetText(Caption);
            }

            scannerCommon.GetAnimationManager().Start(rootWidget);
        }

        /// <summary>
        /// Handles yes/no command, sets the choice and then
        /// closes the scanner
        /// </summary>
        private class CommandHandler : RunCommandHandler
        {
            /// <summary>
            /// Initializes a new instance of the class.
            /// </summary>
            /// <param name="cmd">the command to execute</param>
            public CommandHandler(String cmd)
                : base(cmd)
            {
            }

            /// <summary>
            /// Executes the command
            /// </summary>
            /// <param name="handled">true if it was handled</param>
            /// <returns>true on success</returns>
            public override bool Execute(ref bool handled)
            {
                handled = true;

                var form = Dispatcher.Scanner.Form as YesNoScanner;

                switch (Command)
                {
                    case "CmdYes":
                        form.Choice = true;
                        break;

                    case "CmdNo":
                        form.Choice = false;
                        break;
                }

                if (handled)
                {
                    Windows.CloseForm(form);
                }

                return true;
            }
        }

        /// <summary>
        /// Command dispatcher
        /// </summary>
        private class Dispatcher : DefaultCommandDispatcher
        {
            /// <summary>
            /// Initializes a new instance of the class.
            /// </summary>
            /// <param name="panel">the scanner object</param>
            public Dispatcher(IScannerPanel panel)
                : base(panel)
            {
                Commands.Add(new CommandHandler("CmdYes"));
                Commands.Add(new CommandHandler("CmdNo"));
            }
        }
    }
}