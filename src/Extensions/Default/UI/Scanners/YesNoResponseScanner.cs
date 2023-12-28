////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// YesNoResponseScanner.cs
//
// Displays a scanner with just Yes and No, which the user can select and
// the response is converted to speech.
// This is a useful form of communications for quick yes/no type responses
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
using ACAT.Lib.Core.TTSManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using ACAT.Lib.Extension;
using ACAT.Lib.Extension.CommandHandlers;
using System;
using System.Security.Permissions;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.UI.Menus
{
    [DescriptorAttribute("BF9D82F4-B43F-4188-B8AD-D0EE2F5D7E2B",
                        "YesNoResponseScanner ",
                        "Yes No Response Scanner")]
    public partial class YesNoResponseScanner : Form, IScannerPanel, IExtension
    {
        /// <summary>
        /// Represents the widget of this form
        /// </summary>
        protected Widget rootWidget;

        /// <summary>
        /// The scannerCommon object
        /// </summary>
        protected ScannerCommon scannerCommon;

        /// <summary>
        /// Startup args
        /// </summary>
        protected StartupArg startupArg;

        /// <summary>
        /// Startup command arguments
        /// </summary>
        protected object startupCommandArg;

        private const int TimerInterval = 3000;

        /// <summary>
        /// The command dispatcher object
        /// </summary>
        private readonly Dispatcher _dispatcher;

        /// <summary>
        /// Provdes access to methods/properties in this class
        /// </summary>
        private readonly ExtensionInvoker _invoker;

        private readonly String _no = "No";

        /// <summary>
        /// The scanner helper object
        /// </summary>
        private ScannerHelper _scannerHelper;

        private Timer _timer;

        /// <summary>
        /// Title of the scanner
        /// </summary>
        private readonly String _title = String.Empty;

        private readonly String _yes = "Yes";

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="panelClass">Panel class of the scanner</param>
        /// <param name="panelTitle">Title of the scanner</param>
        public YesNoResponseScanner(String panelClass, String panelTitle)
        {
            scannerCommon = new ScannerCommon(this);

            InitializeComponent();

            Load += YesNoResponseScanner_Load;
            FormClosing += YesNoResponseScanner_FormClosing;

            _title = panelTitle;
            PanelClass = panelClass;

            _dispatcher = new Dispatcher(this);
            _invoker = new ExtensionInvoker(this);
        }

        /// Gets the command dispatcher for Runcommand
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
        /// Gets or sets the panel class
        /// </summary>
        public String PanelClass { get; protected set; }

        /// <summary>
        /// Gets the PanelCommon object
        /// </summary>
        public IPanelCommon PanelCommon
        { get { return scannerCommon; } }

        /// <summary>
        /// Gets the scanner common object
        /// </summary>
        public ScannerCommon ScannerCommon
        {
            get { return scannerCommon; }
        }

        /// <summary>
        /// Returns sync object
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
        /// Set form style
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
        /// Don't want the form to steal focus
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
        public bool CheckCommandEnabled(CommandEnabledArg arg)
        {
            return _scannerHelper.CheckCommandEnabled(arg);
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
        /// Initialzes the class
        /// </summary>
        /// <param name="startupArg">startup parameters</param>
        /// <returns>true on success</returns>
        public bool Initialize(StartupArg startupArg)
        {
            Log.Debug();
            PanelClass = startupArg.PanelClass;
            startupCommandArg = startupArg.Arg;
            this.startupArg = startupArg;

            _scannerHelper = new ScannerHelper(this, startupArg);

            if (!scannerCommon.Initialize(startupArg))
            {
                Log.Debug("Could not initialize form " + Name);
                return false;
            }

            rootWidget = PanelCommon.RootWidget;
            return true;
        }

        /// <summary>
        /// Invoked when the focus changes either in the active window or when the
        /// active window itself changes.
        /// </summary>
        /// <param name="monitorInfo">Info about focused element</param>
        public void OnFocusChanged(WindowActivityMonitorInfo monitorInfo)
        {
            scannerCommon.OnFocusChanged(monitorInfo);
        }

        /// <summary>
        /// Pauses animation and hides the scanner
        /// </summary>
        public virtual void OnPause()
        {
            Log.Debug();

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
        /// Resumes animation and shows the scanner
        /// </summary>
        public virtual void OnResume()
        {
            Log.Debug();

            scannerCommon.OnResume();
        }

        /// <summary>
        /// Invoked when a widget is actuated
        /// </summary>
        /// <param name="widget">widget actuated</param>
        /// <param name="handled">was this handled</param>
        public virtual void OnWidgetActuated(WidgetActuatedEventArgs e, ref bool handled)
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
        /// Size of the client changed
        /// </summary>
        /// <param name="e">event args</param>
        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);
            scannerCommon.OnClientSizeChanged();
        }

        /// <summary>
        /// Release resources
        /// </summary>
        /// <param name="e">event args</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _scannerHelper.OnFormClosing(e);
            scannerCommon.OnFormClosing(e);

            base.OnFormClosing(e);
        }

        /// <summary>
        /// Window proc
        /// </summary>
        /// <param name="m"></param>
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        protected override void WndProc(ref Message m)
        {
            if (scannerCommon != null)
            {
                if (scannerCommon.HandleWndProc(m))
                {
                    return;
                }
            }

            base.WndProc(ref m);
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop();
            Windows.SetText(Prompt, String.Empty);
        }

        private void scannerRoundedButtonControl1_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Close and dispose scanner
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void YesNoResponseScanner_FormClosing(object sender, FormClosingEventArgs e)
        {
            _timer.Stop();
            _timer.Dispose();

            scannerCommon.OnClosing();
            scannerCommon.Dispose();
        }

        /// <summary>
        /// Form loader. Initialize variables
        /// </summary>
        private void YesNoResponseScanner_Load(object sender, EventArgs e)
        {
            Log.Debug();

            scannerCommon.OnLoad();

            if (!String.IsNullOrEmpty(_title))
            {
                Text = _title;
            }

            _timer = new Timer
            {
                Interval = TimerInterval
            };
            _timer.Tick += _timer_Tick;

            PanelCommon.AnimationManager.Start(rootWidget);
        }

        /// <summary>
        /// Handles commands. We have yes and no.  Sets
        /// the choice and closes the scanner
        /// </summary>
        private class CommandHandler : RunCommandHandler
        {
            /// <summary>
            /// Initializes a new instance of the class.
            /// </summary>
            /// <param name="cmd">command to execute</param>
            public CommandHandler(String cmd)
                : base(cmd)
            {
            }

            /// <summary>
            /// Executes the command
            /// </summary>
            /// <param name="handled">was this handled</param>
            /// <returns>true</returns>
            public override bool Execute(ref bool handled)
            {
                handled = true;

                var form = Dispatcher.Scanner.Form as YesNoResponseScanner;

                switch (Command)
                {
                    case "CmdGoBack":
                        form._timer.Stop();
                        Windows.CloseForm(form);
                        break;

                    case "CmdYes":
                        Windows.SetText(form.Prompt, form._yes);
                        TTSManager.Instance.ActiveEngine.Speak(form._yes);
                        form._timer.Stop();
                        form._timer.Start();
                        break;

                    case "CmdNo":
                        Windows.SetText(form.Prompt, form._no);
                        TTSManager.Instance.ActiveEngine.Speak(form._no);
                        form._timer.Stop();
                        form._timer.Start();
                        break;
                }

                return true;
            }
        }

        /// <summary>
        /// Command dispatcher class
        /// </summary>
        private class Dispatcher : DefaultCommandDispatcher
        {
            /// <summary>
            /// Initializes a new instance of the class
            /// </summary>
            /// <param name="panel">the scanner object</param>
            public Dispatcher(IScannerPanel panel)
                : base(panel)
            {
                Commands.Add(new CommandHandler("CmdGoBack"));
                Commands.Add(new CommandHandler("CmdYes"));
                Commands.Add(new CommandHandler("CmdNo"));
            }
        }
    }
}