// <copyright file="FunctionKeyScanner.cs" company="Intel Corporation">
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

using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using ACAT.Lib.Extension;
using ACAT.Lib.Extension.CommandHandlers;
using System;
using System.Security.Permissions;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.UI.Scanners
{
    /// <summary>
    /// Scanner for typing function keys F1 through F12
    /// </summary>
    [DescriptorAttribute("889D0562-78F4-47BC-A879-DB6572227D01",
                        "FunctionKeyScanner",
                        "Function Key Scanner")]
    public partial class FunctionKeyScanner : Form, IScannerPanel, ISupportsStatusBar
    {
        /// <summary>
        /// The Command dispatcher object
        /// </summary>
        private readonly DefaultCommandDispatcher _dispatcher;

        /// <summary>
        /// The ScannerCommon object
        /// </summary>
        private readonly ScannerCommon _scannerCommon;

        /// <summary>
        /// The ScannerHelper object
        /// </summary>
        private ScannerHelper _scannerHelper;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public FunctionKeyScanner()
        {
            _scannerCommon = new ScannerCommon(this);

            InitializeComponent();

            Load += FunctionKeyScanner_Load;
            FormClosing += FunctionKeyScanner_FormClosing;
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
        /// Gets the PanelCommon object
        /// </summary>
        public IPanelCommon PanelCommon { get { return _scannerCommon; } }

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
            get { return ScannerCommon.StatusBar; }
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
        /// Sets the window style
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
        public bool CheckCommandEnabled(CommandEnabledArg arg)
        {
            return _scannerHelper.CheckCommandEnabled(arg);
        }

        /// <summary>
        /// Initialzes the scanner
        /// </summary>
        /// <param name="startupArg">Starting arguments</param>
        /// <returns>true on success</returns>
        public bool Initialize(StartupArg startupArg)
        {
            _scannerHelper = new ScannerHelper(this, startupArg);

            if (!_scannerCommon.Initialize(startupArg))
            {
                Log.Debug("Could not initialize form " + Name);
                return false;
            }

            _scannerCommon.SetStatusBar(statusStrip);

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
        /// Size of the client changed
        /// </summary>
        /// <param name="e">event args</param>
        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);
            _scannerCommon.OnClientSizeChanged();
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
            if (_scannerCommon != null)
            {
                if (_scannerCommon.HandleWndProc(m))
                {
                    return;
                }
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// Form is closing. Disposes resources
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arg</param>
        private void FunctionKeyScanner_FormClosing(object sender, FormClosingEventArgs e)
        {
            _scannerCommon.OnClosing();
            _scannerCommon.Dispose();
        }

        /// <summary>
        /// Form loader.  Initializes, subscribes to events.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arg</param>
        private void FunctionKeyScanner_Load(object sender, EventArgs e)
        {
            _scannerCommon.OnLoad();

            PanelCommon.AnimationManager.Start(PanelCommon.RootWidget);
        }
    }
}