////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
using ACAT.Lib.Core.UserControlManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using ACAT.Lib.Extension.CommandHandlers;
using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Windows.Forms;

namespace ACAT.Lib.Extension
{
    /// <summary>
    /// A generic scanner form that acts a a container for a user control
    /// </summary>
    [DescriptorAttribute("6889D5CA-2D64-4123-AB0E-179D3C41560C",
                    "UserControlContainerForm",
                    "Generic container form for a usercontrol")]
    public partial class UserControlContainerForm : Form, IScannerPanel
    {
        private readonly Dispatcher _dispatcher;

        /// <summary>
        /// The AlphabetScannerCommon object. Has a number of
        /// helper functions
        /// </summary>
        private readonly ScannerCommon2 _scannerCommon;

        private String _panelClass;
        private bool _pauseWatchdog;
        private WindowActiveWatchdog _windowActiveWatchdog;

        public UserControlContainerForm()
        {
            _scannerCommon = new ScannerCommon2(this);

            InitializeComponent();

            subscribeToEvents();

            Load += UserControlContainerForm_Load;

            _dispatcher = new Dispatcher(this);

            FormClosing += UserControlContainerForm_FormClosing;
        }

        /// <summary>
        /// Gets the command dispatcher object
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

        public String EmbeddedUserControlName
        {
            get; set;
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
            get { return _panelClass; }
        }

        /// <summary>
        /// Gets the PanelCommon object
        /// </summary>
        public IPanelCommon PanelCommon
        { get { return _scannerCommon; } }

        public ScannerCommon ScannerCommon
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the scanner common object
        /// </summary>
        public ScannerCommon2 ScannerCommon2
        {
            get { return _scannerCommon; }
        }

        /// <summary>
        /// Gets the synch object
        /// </summary>
        public SyncLock SyncObj
        {
            get { return _scannerCommon.SyncObj; }
        }

        /// <summary>
        /// Gets the text controller object for this scanner
        /// </summary>
        public ITextController TextController
        {
            get { return _scannerCommon.TextController; }
        }

        /// <summary>
        /// Invoked to check if a scanner button should be enabled.  Uses context
        /// to determine the 'enabled' state.
        /// </summary>
        /// <param name="arg">info about the scanner button</param>
        public bool CheckCommandEnabled(CommandEnabledArg arg)
        {
            return true;
        }

        /// <summary>
        /// Intitialize the class
        /// </summary>
        /// <param name="startupArg">startup params</param>
        /// <returns>true on cussess</returns>
        public bool Initialize(StartupArg startupArg)
        {
            _panelClass = startupArg.PanelClass;

            bool retVal = _scannerCommon.Initialize(startupArg);

            ControlBox = true;

            _scannerCommon.UserControlManager.GridScanIterations = Common.AppPreferences.GridScanIterations;

            _scannerCommon.UserControlManager.AddUserControlByKeyOrName(panelContainer, "embedUserControl", EmbeddedUserControlName);

            List<IUserControl> list = new List<IUserControl>();

            UserControlManager.FindAllUserControls(this, list);

            return retVal;
        }

        /// <summary>
        /// Invoked when the focus changes either in the active window or when the
        /// active window itself changes.
        /// </summary>
        /// <param name="monitorInfo">Info about focused element</param>
        public void OnFocusChanged(WindowActivityMonitorInfo monitorInfo)
        {
            _scannerCommon.OnFocusChanged(monitorInfo);
        }

        /// <summary>
        /// Pauses animations
        /// </summary>
        public void OnPause()
        {
            if (_pauseWatchdog)
            {
                removeWatchdogs();
            }

            _scannerCommon.UserControlManager.OnPause();

            _scannerCommon.OnPause(true ?
                                ScannerCommon2.PauseDisplayMode.FadeScanner :
                                ScannerCommon2.PauseDisplayMode.None);
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
            enableWatchdogs();

            _scannerCommon.UserControlManager.OnResume();

            _scannerCommon.OnResume();

            _scannerCommon.ResizeToFitDesktop(this);
        }

        /// <summary>
        /// Triggered when the user actuates a widget
        /// </summary>
        /// <param name="widget">widget actuated</param>
        /// <param name="handled">was this handled?</param>
        public void OnWidgetActuated(WidgetActuatedEventArgs e, ref bool handled)
        {
            //_alphabetScannerCommon.OnWidgetActuated(e, ref handled);
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
            _scannerCommon.OnClientSizeChanged();
        }

        /// <summary>
        /// Form is closing. Release resources
        /// </summary>
        /// <param name="e">closing param</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _scannerCommon.OnFormClosing(e);
            base.OnFormClosing(e);
        }

        /// <summary>
        /// Window procedure
        /// </summary>
        /// <param name="m">windows message</param>
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        protected override void WndProc(ref Message m)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MOVE = 0xF010;

            if (m.Msg == WM_SYSCOMMAND)
            {
                int command = m.WParam.ToInt32() & 0xfff0;
                if (command == SC_MOVE)
                {
                    base.WndProc(ref m);
                    return;
                }
            }

            if (!_scannerCommon.HandleWndProc(m))
            {
                base.WndProc(ref m);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            FormClosing -= UserControlContainerForm_FormClosing;
            removeUserControl();
            Close();
        }

        /// <summary>
        /// Makes sure the scanner stays focused
        /// </summary>
        private void enableWatchdogs()
        {
            //return;

            if (_windowActiveWatchdog == null)
            {
                _windowActiveWatchdog = new WindowActiveWatchdog(this);
            }

            _pauseWatchdog = false;
        }

        private void removeUserControl()
        {
            this.panelContainer.Controls.Clear();
        }

        private void removeWatchdogs()
        {
            if (_windowActiveWatchdog != null)
            {
                _windowActiveWatchdog.Dispose();
                _windowActiveWatchdog = null;
            }
        }

        /// <summary>
        /// Subscribes to the various events
        /// </summary>
        private void subscribeToEvents()
        {
            Load += UserControlContainerForm_Load;
            Shown += UserControlContainerForm_Shown;
            FormClosing += UserControlContainerForm_FormClosing; ;
        }

        private void UserControlContainerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _scannerCommon.OnClosing();
        }

        private void UserControlContainerForm_Load(object sender, EventArgs e)
        {
            _scannerCommon.OnLoad();

            _scannerCommon.ResizeToFitDesktop(this);
        }

        /// <summary>
        /// Event handler for when form is shown
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void UserControlContainerForm_Shown(object sender, EventArgs e)
        {
            ScannerFocus.SetFocus(this);
        }

        private class Dispatcher : DefaultCommandDispatcher
        {
            /// <summary>
            /// Initializes a new instance of the class.
            /// </summary>
            /// <param name="panel">the scanner object</param>
            public Dispatcher(IScannerPanel panel)
                : base(panel)
            {
            }
        }
    }
}