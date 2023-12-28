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
using System;
using System.Security.Permissions;
using System.Windows.Forms;
using static ACAT.Lib.Core.PanelManagement.ScannerCommon;

namespace ACAT.Lib.Extension
{
    /// <summary>
    /// Base class for all horizontal strip scanners.  This
    /// is a scanner with a single row of buttons.
    /// The width of the scanner is dynamically
    /// computed depending on how many menu items are there
    /// </summary>
    [DescriptorAttribute("D6AE907B-CB4B-417E-9FCC-E587D976FFD7",
                "ScanTimeAdjustScanner",
                "Adjust scan time")]
    public partial class ScanTimeAdjustForm : Form, IScannerPanel
    {
        /// <summary>
        /// The command dispatcher to execute commands
        /// </summary>
        private readonly RunCommandDispatcher _dispatcher;

        private readonly int _prevScanTime = CoreGlobals.AppPreferences.ScanTime;

        /// <summary>
        /// The root widget representing this scanner form
        /// </summary>
        private Widget _rootWidget;

        /// <summary>
        /// ScannerCommon object for all the heavy lifting
        /// </summary>
        private ScannerCommon _scannerCommon;

        public ScanTimeAdjustForm()
        {
            InitializeComponent();
            _dispatcher = new RunCommandDispatcher(this);
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="panelClass">Scanner class</param>
        /// <param name="title">Title of the scanner</param>
        public ScanTimeAdjustForm(String panelClass, String title)
        {
            this.MinimumSize = new System.Drawing.Size(30, 30);
            InitializeComponent();
            this.MinimumSize = new System.Drawing.Size(30, 30);

            Load += ScanTimeAdjustForm_Load;
            FormClosing += ScanTimeAdjustForm_FormClosing;
            _dispatcher = new RunCommandDispatcher(this);
            Text = title;
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
            return false;
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
            _scannerCommon.OnPause(PauseDisplayMode.None);
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
        public virtual void OnWidgetActuated(WidgetActuatedEventArgs e, ref bool handled)
        {
            if (e.SourceWidget.Value == "@CmdGoBack")
            {
                restoreDefaultScanSpeed();
                Windows.CloseForm(this);
            }
            else if (e.SourceWidget.Value == "@CmdSaveScanSpeed")
            {
                if (CoreGlobals.AppPreferences.ScanTime != _prevScanTime)
                {
                    confirmSaveAndClose();
                }
                else
                {
                    Windows.CloseForm(this);
                }
            }
            else if (e.SourceWidget.Value == "@CmdScanSpeedUp")
            {
                changedScanningSpeedBy(-50);
            }
            else if (e.SourceWidget.Value == "@CmdScanSpeedDown")
            {
                changedScanningSpeedBy(50);
            }

            handled = true;
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

        private void changedScanningSpeedBy(int delta)
        {
            Invoke(new MethodInvoker(delegate
            {
                OnPause();

                var scanTime = Common.AppPreferences.ScanTime + delta;

                if (scanTime < 100 || scanTime > 10000)
                {
                    OnResume();
                    return;
                }

                Common.AppPreferences.ScanTime = scanTime;

                Common.AppPreferences.NotifyPreferencesChanged();

                updateLabelScanSpeed();

                OnResume();
            }));
        }

        private void confirmSaveAndClose()
        {
            Invoke(new MethodInvoker(delegate
            {
                if (DialogUtils.ConfirmScanner(null, "Save setting?"))
                {
                    CoreGlobals.AppPreferences.Save();
                }
                else
                {
                    restoreDefaultScanSpeed();
                }

                Close();
            }));
        }

        private void restoreDefaultScanSpeed()
        {
            CoreGlobals.AppPreferences.ScanTime = _prevScanTime;
            Common.AppPreferences.NotifyPreferencesChanged();
        }

        /// <summary>
        /// Form has closed.  Uninitialize
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event argument</param>
        private void ScanTimeAdjustForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _scannerCommon.OnClosing();
            _scannerCommon.Dispose();
        }

        /// <summary>
        /// Load handler. Perform initialization
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event argument</param>
        private void ScanTimeAdjustForm_Load(object sender, EventArgs e)
        {
            _scannerCommon.OnLoad();

            updateLabelScanSpeed();

            PanelCommon.AnimationManager.Start(_rootWidget);
        }

        private void updateLabelScanSpeed()
        {
            Invoke(new MethodInvoker(delegate
            {
                labelScanSpeed.Text = CoreGlobals.AppPreferences.ScanTime + " msecs";
            }));
        }
    }
}