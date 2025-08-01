////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.AgentManagement;
using ACAT.Core.PanelManagement;
using ACAT.Core.PanelManagement.CommandDispatcher;
using ACAT.Core.Utility;
using ACAT.Extension.CommandHandlers;
using ACAT.Scanners;
using System;
using System.Security.Permissions;
using System.Windows.Forms;

namespace ACAT.Extension
{
    /// <summary>
    /// A generic scanner form that acts a a container for a user control
    /// </summary>
    [ClassDescriptorAttribute("6889D5CA-2D64-4123-AB0E-179D3C41560C",
                    "UserControlContainerForm",
                    "Generic container form for a usercontrol")]
    public partial class UserControlContainerForm : GenericScannerForm
    {
        public String EmbeddedUserControlName { get; set; }

        public override DefaultCommandDispatcher _dispatcher { get; }

        public override RunCommandDispatcher CommandDispatcher => _dispatcher;

        public override ITextController TextController => ScannerCommon.TextController;

        public UserControlContainerForm() : base()
        {
            _dispatcher = new ContainerFormDispatcher(this);
        }

        public override bool CheckCommandEnabled(CommandEnabledArg arg)
        {
            return true;
        }

        public override bool HandleInitialize(StartupArg startupArg)
        {
            EmbeddedUserControlName = startupArg.PanelClass;
            return _scannerCommon.UserControlManager.AddUserControlByKeyOrName(panelContainer, "embedUserControl", EmbeddedUserControlName);

        }

        /// <summary>
        /// Pauses animations
        /// </summary>
        //public override void OnPause()
        //{
        //    if (_pauseWatchdog)
        //    {
        //        removeWatchdogs();
        //    }

        //    _scannerCommon.UserControlManager.OnPause();

        //    _scannerCommon.OnPause(true ?
        //                        ScannerCommon.PauseDisplayMode.FadeScanner :
        //                        ScannerCommon.PauseDisplayMode.None);
        //}

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="eventArg"></param>
        /// <returns></returns>
        //public bool OnQueryPanelChange(PanelRequestEventArgs eventArg)
        //{
        //    return true;
        //}

        /// <summary>
        /// Resumes animation
        /// </summary>
        //public override void OnResume()
        //{
        //    enableWatchdogs();

        //    _scannerCommon.UserControlManager.OnResume();

        //    _scannerCommon.OnResume();

        //    _scannerCommon.ResizeToFitDesktop(this);
        //}

        ///// <summary>
        ///// Triggered when the user actuates a widget
        ///// </summary>
        ///// <param name="widget">widget actuated</param>
        ///// <param name="handled">was this handled?</param>
        //public void OnWidgetActuated(WidgetActuatedEventArgs e, ref bool handled)
        //{
        //    //_alphabetScannerCommon.OnWidgetActuated(e, ref handled);
        //}

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="widget"></param>
        //public void SetTargetControl(Form parent, Widget widget)
        //{
        //}

        /// <summary>
        /// Size of the client changed
        /// </summary>
        /// <param name="e">event args</param>
        //protected override void OnClientSizeChanged(EventArgs e)
        //{
        //    base.OnClientSizeChanged(e);
        //    _scannerCommon.OnClientSizeChanged();
        //}

        /// <summary>
        /// Form is closing. Release resources
        /// </summary>
        /// <param name="e">closing param</param>
        //protected override void OnFormClosing(FormClosingEventArgs e)
        //{
        //    _scannerCommon.OnFormClosing(e);
        //    base.OnFormClosing(e);
        //}

        protected override void ScannerFormLoaded(object sender, EventArgs e)
        {
        }

        protected override void ScannerShown(object sender, EventArgs e)
        {
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

        //private void buttonCancel_Click(object sender, EventArgs e)
        //{
        //    FormClosing -= UserControlContainerForm_FormClosing;
        //    removeUserControl();
        //    Close();
        //}

        /// <summary>
        /// Makes sure the scanner stays focused
        /// </summary>
        //private void enableWatchdogs()
        //{
        //    //return;

        //    if (_windowActiveWatchdog == null)
        //    {
        //        _windowActiveWatchdog = new WindowActiveWatchdog(this);
        //    }

        //    _pauseWatchdog = false;
        //}

        //private void removeUserControl()
        //{
        //    this.panelContainer.Controls.Clear();
        //}

        //private void removeWatchdogs()
        //{
        //    if (_windowActiveWatchdog != null)
        //    {
        //        _windowActiveWatchdog.Dispose();
        //        _windowActiveWatchdog = null;
        //    }
        //}

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

        private class ContainerFormDispatcher : DefaultCommandDispatcher
        {
            /// <summary>
            /// Initializes a new instance of the class.
            /// </summary>
            /// <param name="panel">the scanner object</param>
            public ContainerFormDispatcher(IScannerPanel panel)
                : base(panel)
            {
            }
        }
    }
}