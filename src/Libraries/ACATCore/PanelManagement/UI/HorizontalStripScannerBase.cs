////////////////////////////////////////////////////////////////////////////
// <copyright file="HorizontalStripScannerBase.cs" company="Intel Corporation">
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
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Windows.Forms;

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Base class for all horizontal strip scanners.  This
    /// is a scanner with a single row of buttons.
    /// The width of the scanner is dynamically
    /// computed depending on how many menu items are there
    /// </summary>
    public partial class HorizontalStripScannerBase : Form, IScannerPanel
    {
        /// <summary>
        /// The command dispatcher to execute commands
        /// </summary>
        private readonly RunCommandDispatcher _dispatcher;

        /// <summary>
        /// The root widget representing this scanner form
        /// </summary>
        private Widget _rootWidget;

        /// <summary>
        /// ScannerCommon object for all the heavy lifting
        /// </summary>
        private ScannerCommon _scannerCommon;

        /// <summary>
        /// Actual width, adjusted to the number of actual
        /// buttons.
        /// </summary>
        private int _width;

        public HorizontalStripScannerBase()
        {
            InitializeComponent();
            _dispatcher = new RunCommandDispatcher(this);
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="panelClass">Scanner class</param>
        /// <param name="title">Title of the scanner</param>
        public HorizontalStripScannerBase(String panelClass, String title)
        {
            this.MinimumSize = new System.Drawing.Size(30, 30);
            InitializeComponent();
            this.MinimumSize = new System.Drawing.Size(30, 30);

            Load += HorizontalStripScanner_Load;
            FormClosing += HorizontalStripScanner_FormClosing;
            _dispatcher = new RunCommandDispatcher(this);
            //Text = title;
            Text = String.Empty;
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
        public IPanelCommon PanelCommon { get { return _scannerCommon; } }

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

            Width = _width;
        }

        /// <summary>
        /// Invoked when the user actuates a button in
        /// the scanner form
        /// </summary>
        /// <param name="widget">widget actuated</param>
        /// <param name="handled">was this handled here?</param>
        public virtual void OnWidgetActuated(Widget widget, ref bool handled)
        {
            handled = false;
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
        private void HorizontalStripScanner_FormClosing(object sender, FormClosingEventArgs e)
        {
            _scannerCommon.OnClosing();
            _scannerCommon.Dispose();
        }

        /// <summary>
        /// Load handler. Perform initialization
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event argument</param>
        private void HorizontalStripScanner_Load(object sender, EventArgs e)
        {
            setFormWidth();

            _scannerCommon.OnLoad(false);

            PanelCommon.AnimationManager.Start(_rootWidget);
        }

        /// <summary>
        /// Depending on how many buttons there actually are,
        /// remove unused buttons and set the width of the scanner
        /// accordingly
        /// </summary>
        private void setFormWidth()
        {
            List<Widget> children = new List<Widget>();
            _rootWidget.Finder.FindAllButtons(children);
            int count = children.Count();
            int width = B1.Width + B1.Margin.Left + B1.Margin.Right;
            if (count > 0 && count < tableLayoutPanel1.ColumnCount)
            {
                int colsRemoved = 0;
                for (int col = tableLayoutPanel1.ColumnCount - 1; col >= count; col--)
                {
                    tableLayoutPanel1.ColumnStyles.RemoveAt(col);
                    tableLayoutPanel1.ColumnCount--;
                    colsRemoved++;
                }

                Width -= colsRemoved * width;
            }

            _width = Width;
        }
    }
}