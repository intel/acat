////////////////////////////////////////////////////////////////////////////
// <copyright file="CalculatorScanner.cs" company="Intel Corporation">
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
using ACAT.ACATResources;

namespace ACAT.Lib.Core.Extensions.Base.AppAgents.ApplicationFrameHostAgent
{
    /// <summary>
    /// This is the companion scanner for the Windows 10 calculator
    /// </summary>
    [DescriptorAttribute("3E4969B0-DEBC-4327-95D0-C3471774409A",
                        "CalculatorScanner",
                        "Calculator companion scanner")]
    public partial class CalculatorScanner : Form, IScannerPanel, ISupportsStatusBar
    {
        /// <summary>
        /// Dispatcher to handle execution of commands
        /// </summary>
        private readonly DefaultCommandDispatcher _defaultCommandDispatcher;

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
        public CalculatorScanner()
        {
            _scannerCommon = new ScannerCommon(this);

            InitializeComponent();

            Load += OnLoad;
            FormClosing += CalculatorScanner_FormClosing;
            PanelClass = "CalculatorScanner";
            _defaultCommandDispatcher = new DefaultCommandDispatcher(this);
        }

        /// <summary>
        /// Gets the command dispatcher to run commands
        /// </summary>
        public RunCommandDispatcher CommandDispatcher
        {
            get { return _defaultCommandDispatcher; }
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
        /// Gets the panel class for this scanner
        /// </summary>
        public String PanelClass { get; private set; }

        /// <summary>
        /// Gets the PanelCommon object
        /// </summary>
        public IPanelCommon PanelCommon { get { return _scannerCommon; } }

        /// <summary>
        /// Gets the scanner common object
        /// </summary>
        public ScannerCommon ScannerCommon
        {
            get { return _scannerCommon; }
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
        /// Gets the status bar for this scanner
        /// </summary>
        public ScannerStatusBar ScannerStatusBar
        {
            get { return ScannerCommon.StatusBar; }
        }

        /// <summary>
        /// Gets the synch object for this scanner
        /// </summary>
        public SyncLock SyncObj
        {
            get { return _scannerCommon.SyncObj; }
        }

        /// <summary>
        /// Returns the text controller object for the scanner
        /// </summary>
        public ITextController TextController
        {
            get { return _scannerCommon.TextController; }
        }

        /// <summary>
        /// Sets from styles
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
        public bool CheckCommandEnabled(CommandEnabledArg arg)
        {
            return _scannerHelper.CheckCommandEnabled(arg);
        }

        /// <summary>
        /// Intitializes the class
        /// </summary>
        /// <param name="startupArg">startup param</param>
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
        /// Invoked when the focus changes either in the active window or when the
        /// active window itself changes.
        /// </summary>
        /// <param name="monitorInfo">Info about focused element</param>
        public void OnFocusChanged(WindowActivityMonitorInfo monitorInfo)
        {
            _scannerCommon.OnFocusChanged(monitorInfo);
        }

        /// <summary>
        /// Pauses animation and hides scanner
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
        /// Resumes animation and shows scanner
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
        /// Form is closing. Release resources
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _scannerCommon.OnFormClosing(e);
            _scannerHelper.OnFormClosing(e);
            base.OnFormClosing(e);
        }

        /// <summary>
        /// Window procedure
        /// </summary>
        /// <param name="m"></param>
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
        /// Form is closing. Release resources
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void CalculatorScanner_FormClosing(object sender, FormClosingEventArgs e)
        {
            _scannerCommon.OnClosing();
            _scannerCommon.Dispose();
        }

        /// <summary>
        /// Scanner is loading. Initializes the form
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void OnLoad(object sender, EventArgs e)
        {
            Text = R.GetString("Calculator");

            _scannerCommon.OnLoad();

            PanelCommon.AnimationManager.Start(PanelCommon.RootWidget);
        }
    }
}