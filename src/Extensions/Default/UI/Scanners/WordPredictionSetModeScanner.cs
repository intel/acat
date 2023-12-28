////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using ACAT.Lib.Core.WordPredictionManagement;
using ACAT.Lib.Extension.CommandHandlers;
using System;
using System.Security.Permissions;
using System.Windows.Forms;

namespace ACAT.Lib.Extension
{
    /// <summary>
    /// Base class for all horizontal strip scanners.  This
    /// is a scanner with a single row of buttons.
    /// The width of the scanner is dynamically
    /// computed depending on how many menu items are there
    /// </summary>
    [Descriptor("954E418F-B206-457C-B7A5-7C32B4D85E76",
                    "WordPredictionSetModeScanner",
                    "Set the word prediction mode")]
    public partial class WordPredictionSetModeScanner : Form, IScannerPanel
    {
        /// <summary>
        /// The command dispatcher.  If the derived class as additional
        /// commands, just call Commands.Add on this object
        /// </summary>
        private readonly Dispatcher _dispatcher;

        /// <summary>
        /// Current word prediction mode
        /// </summary>
        private readonly WordPredictionModes _mode;

        /// <summary>
        /// The root widget representing this scanner form
        /// </summary>
        private Widget _rootWidget;

        /// <summary>
        /// ScannerCommon object for all the heavy lifting
        /// </summary>
        private ScannerCommon _scannerCommon;

        public WordPredictionSetModeScanner()
        {
            InitializeComponent();
            _dispatcher = new Dispatcher(this);
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="panelClass">Scanner class</param>
        /// <param name="title">Title of the scanner</param>
        public WordPredictionSetModeScanner(String panelClass, String title)
        {
            InitializeComponent();

            Load += WordPredictionSetModeScanner_Load;
            FormClosing += WordPredictionSetModeScanner_FormClosing;
            _dispatcher = new Dispatcher(this);
            Text = title;

            _mode = Context.AppWordPredictionManager.ActiveWordPredictor.GetMode();
        }

        /// <summary>
        /// Gets the command dispatcher object
        /// </summary>
        public virtual RunCommandDispatcher CommandDispatcher
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
        /// Gets this form
        /// </summary>
        public Form Form
        {
            get { return this; }
        }

        /// <summary>
        /// Gets the scanner class
        /// </summary>
        public String PanelClass
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the PanelCommon object
        /// </summary>
        public IPanelCommon PanelCommon
        { get { return _scannerCommon; } }

        /// <summary>
        /// Gets the scannerCommon object
        /// </summary>
        public ScannerCommon ScannerCommon
        {
            get { return _scannerCommon; }
        }

        /// <summary>
        /// Gets the synchronization object for this scanner
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
        /// Set the form style
        /// </summary>
        protected override CreateParams CreateParams
        {
            get { return Windows.SetFormStyles(base.CreateParams); }
        }

        /// <summary>
        /// Tell windows not to set focus to this form when
        /// user clicks on it
        /// </summary>
        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        /// <summary>
        /// Called to check if the specified widget in arg should
        /// be enabled or not.  This function is called perfiodically
        /// because application context may change any time. Set
        /// the handled property in arg to true if this is handled.
        /// </summary>
        /// <param name="arg">argument</param>
        /// <returns>true on success</returns>
        public virtual bool CheckCommandEnabled(CommandEnabledArg arg)
        {
            switch (arg.Command)
            {
                case "CmdTypingModeSentence":
                    arg.Enabled = _mode != WordPredictionModes.Sentence;
                    arg.Handled = true;
                    break;

                case "CmdTypingModePhrase":
                    arg.Enabled = _mode != WordPredictionModes.CannedPhrases;
                    arg.Handled = true;
                    break;

                case "CmdTypingModeShortHand":
                    arg.Enabled = _mode != WordPredictionModes.Shorthand;
                    arg.Handled = true;
                    break;

                default:
                    arg.Handled = false;
                    break;
            }

            return true;
        }

        /// <summary>
        /// Performs initialization
        /// </summary>
        /// <param name="startupArg">Startup arguments</param>
        /// <returns>true on success</returns>
        public virtual bool Initialize(StartupArg startupArg)
        {
            PanelClass = startupArg.PanelClass;

            _scannerCommon = new ScannerCommon(this) { PositionSizeController = { AutoPosition = false } };

            if (!_scannerCommon.Initialize(startupArg))
            {
                return false;
            }

            _rootWidget = PanelCommon.RootWidget;

            return true;
        }

        /// <summary>
        /// Notification to indicate there was a focus switch
        /// in the application window
        /// </summary>
        /// <param name="monitorInfo"></param>
        public void OnFocusChanged(WindowActivityMonitorInfo monitorInfo)
        {
            _scannerCommon.OnFocusChanged(monitorInfo);
        }

        /// <summary>
        /// Called to pause the scanner
        /// </summary>
        public virtual void OnPause()
        {
            _scannerCommon.OnPause();
        }

        /// <summary>
        /// Should we allow ACAT to switch this scanner out when
        /// there is a application context switch.
        /// </summary>
        /// <param name="arg">contextual info</param>
        /// <returns>true</returns>
        public virtual bool OnQueryPanelChange(PanelRequestEventArgs eventArg)
        {
            return true;
        }

        /// <summary>
        /// Called to resume the scanner
        /// </summary>
        public virtual void OnResume()
        {
            _scannerCommon.OnResume();
        }

        /// <summary>
        /// Invoked when the user actuates a button in
        /// the scanner form
        /// </summary>
        /// <param name="widget">widget actuated</param>
        /// <param name="handled">was this handled here?</param>
        public void OnWidgetActuated(WidgetActuatedEventArgs e, ref bool handled)
        {
            String prompt = "Switch typing mode to ";

            WordPredictionModes mode;

            if (e.SourceWidget.Value == "@CmdTypingModeSentence")
            {
                mode = WordPredictionModes.Sentence;
                prompt += "SENTENCE?";
            }
            else if (e.SourceWidget.Value == "@CmdTypingModePhrase")
            {
                mode = WordPredictionModes.CannedPhrases;
                prompt += "CANNED PHRASE?";
            }
            else if (e.SourceWidget.Value == "@CmdTypingModeShortHand")
            {
                mode = WordPredictionModes.Shorthand;
                prompt += "SHORTHAND?";
            }
            else
            {
                mode = WordPredictionModes.None;
                prompt = String.Empty;
            }

            if (!String.IsNullOrEmpty(prompt))
            {
                if (DialogUtils.ConfirmScanner(this, prompt))
                {
                    Context.AppWordPredictionManager.ActiveWordPredictor.SetMode(mode);
                    Windows.CloseAsync(this);
                }
            }
        }

        /// <summary>
        /// Unused
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="widget"></param>
        public void SetTargetControl(Form parent, Widget widget)
        {
        }

        /// <summary>
        /// Override this to perform cleanup logic. Make sure
        /// this function is called from the derived class
        /// </summary>
        /// <param name="e">event arg</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _scannerCommon.OnFormClosing(e);
            base.OnFormClosing(e);
        }

        /// <summary>
        /// Window proc
        /// </summary>
        /// <param name="m">Windows message</param>
        [EnvironmentPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        protected override void WndProc(ref Message m)
        {
            if (_scannerCommon != null)
            {
                _scannerCommon.HandleWndProc(m);
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// Form has closed.  Uninitialize
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event argument</param>
        private void WordPredictionSetModeScanner_FormClosing(object sender, FormClosingEventArgs e)
        {
            _scannerCommon.OnClosing();
            _scannerCommon.Dispose();
        }

        /// <summary>
        /// Load handler. Perform initialization
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event argument</param>
        private void WordPredictionSetModeScanner_Load(object sender, EventArgs e)
        {
            _scannerCommon.OnLoad();

            PanelCommon.AnimationManager.Start(_rootWidget);
        }

        /// <summary>
        /// The dispatcher object.  The DefaultCommandDispatcher
        /// will take care of executing the commands
        /// </summary>
        public class Dispatcher : DefaultCommandDispatcher
        {
            public Dispatcher(IScannerPanel panel)
                : base(panel)
            {
            }
        }
    }
}