////////////////////////////////////////////////////////////////////////////
// <copyright file="MouseScanner.cs" company="Intel Corporation">
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

using ACAT.Lib.Core.ActuatorManagement;
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.Audit;
using ACAT.Lib.Core.InputActuators;
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
    /// This scanner enables the user to move the mouse around
    /// the display. It uses grid scanning technique to scan the display
    /// User can also click, double click, drag, right click etc.
    /// </summary>
    [DescriptorAttribute("802B03F0-1294-4D06-A601-2CEBFBFA5D9C",
                        "MouseScanner",
                        "Enables mouse placement and mouse action on the display")]
    public partial class MouseScanner : Form, IScannerPanel, ISupportsStatusBar
    {
        /// <summary>
        /// Command dispatcher object
        /// </summary>
        private readonly Dispatcher _dispatcher;

        /// <summary>
        /// Moves the mouse in the grid mode
        /// </summary>
        private GridMouseMover _gridMouseMover;

        /// <summary>
        /// The keyboard actuator object
        /// </summary>
        private KeyboardActuator _keyboardActuator;

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
        public MouseScanner()
        {
            _scannerCommon = new ScannerCommon(this);

            InitializeComponent();

            Load += MouseScanner_Load;
            FormClosing += MouseScanner_FormClosing;
            PanelClass = PanelClasses.Mouse;
            _dispatcher = new Dispatcher(this);
        }

        /// <summary>
        /// Gets the CommandDispatcher to handle commands
        /// </summary>
        public RunCommandDispatcher CommandDispatcher
        {
            get
            {
                return _dispatcher;
            }
        }

        /// <summary>
        /// Gets the descriptor for this class
        /// </summary>
        public IDescriptor Descriptor
        {
            get
            {
                return DescriptorAttribute.GetDescriptor(GetType());
            }
        }

        /// <summary>
        /// Gets the form object
        /// </summary>
        public Form Form
        {
            get { return this; }
        }

        /// <summary>
        /// Gets the panel class
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
        /// Gets the status bar object
        /// </summary>
        public ScannerStatusBar ScannerStatusBar
        {
            get { return ScannerCommon.StatusBar; }
        }

        /// <summary>
        /// Gets the object used for synchronization
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

            Context.AppPanelManager.PausePanelChangeRequests();

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
            pause();
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
            resume();
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
            Context.AppActuatorManager.EvtSwitchHook -= AppActuatorManager_EvtSwitchHook;
            if (_keyboardActuator != null)
            {
                _keyboardActuator.EvtMouseDown -= MouseScannerScreen_EvtMouseDown;
            }

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
        /// Invoked when a switch is activated.  Inform the mouse
        /// mover objects that a switch activation was detected
        /// </summary>
        /// <param name="switchObj">Switch that was activated</param>
        /// <param name="handled">true if it was handled</param>
        private void AppActuatorManager_EvtSwitchHook(IActuatorSwitch switchObj, ref bool handled)
        {
            handled = false;

            if (_gridMouseMover != null)
            {
                _gridMouseMover.Actuate();
                handled = true;
            }
        }

        /// <summary>
        /// Creates and initializes the object that will handle grid mouse
        /// movement.
        /// </summary>
        /// <returns>The created object</returns>
        private GridMouseMover createGridMouseMover()
        {
            var gridMouseMover = new GridMouseMover
            {
                GridRectangleSpeed = Common.AppPreferences.MouseGridRectangleSpeed,
                GridRectangleCycles = Common.AppPreferences.MouseGridRectangleCycles,
                GridLineSpeed = Common.AppPreferences.MouseGridLineSpeed,
                GridLineCycles = Common.AppPreferences.MouseGridRectangleCycles,
                GridLineThickness = Common.AppPreferences.MouseGridLineThickness,
                EnableVerticalGridRectangle = Common.AppPreferences.MouseGridEnableVerticalRectangleScan
            };

            return gridMouseMover;
        }

        /// <summary>
        /// Form is closing. Dispose resources
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arg</param>
        private void MouseScanner_FormClosing(object sender, FormClosingEventArgs e)
        {
            _scannerCommon.OnClosing();
            _scannerCommon.Dispose();
        }

        /// <summary>
        /// Form loader.  Initialize, subscribe to events.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arg</param>
        private void MouseScanner_Load(object sender, EventArgs e)
        {
            ScannerCommon.HideTalkWindow();

            _scannerCommon.OnLoad();

            Context.AppActuatorManager.EvtSwitchHook += AppActuatorManager_EvtSwitchHook;
            var actuator = Context.AppActuatorManager.GetActuator(typeof(KeyboardActuator));
            if (actuator is KeyboardActuator)
            {
                _keyboardActuator = actuator as KeyboardActuator;
                _keyboardActuator.EvtMouseDown += MouseScannerScreen_EvtMouseDown;
            }

            PanelCommon.AnimationManager.Start(PanelCommon.RootWidget);
        }

        /// <summary>
        /// Event handler for mouse down.  Treat this as a switch
        /// activation.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="mouseEventArgs">event args</param>
        private void MouseScannerScreen_EvtMouseDown(object sender, MouseEventArgs mouseEventArgs)
        {
            if (_gridMouseMover != null)
            {
                _gridMouseMover.Actuate();
            }
        }

        /// <summary>
        /// Pause the scanner
        /// </summary>
        private void pause()
        {
            Log.Debug();

            _scannerCommon.OnPause();
        }

        /// <summary>
        /// Resumes the scanner
        /// </summary>
        private void resume()
        {
            Log.Debug();

            _scannerCommon.OnResume();

            // takes care of partial transparency with grid mouse where
            // status bar is transparent after grid mouse stops.
            Refresh();
        }

        /// <summary>
        /// Starts moving the mouse in the grid mode, in the specified
        /// direction
        /// </summary>
        /// <param name="direction">down or up</param>
        private void startGridSweep(GridMouseMover.Direction direction)
        {
            pause();

            _gridMouseMover = createGridMouseMover();

            AuditLog.Audit(new AuditEventMouseMover(direction.ToString()));

            _gridMouseMover.GridRectangleDirection = direction;

            _gridMouseMover.Start();

            _gridMouseMover = null;

            resume();
        }

        /// <summary>
        /// Handles all  the commands for the mouse scanner
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

                var form = Dispatcher.Scanner.Form as MouseScanner;

                switch (Command)
                {
                    case "CmdScanVerticalDown":
                        form.startGridSweep(GridMouseMover.Direction.Down);
                        break;

                    case "CmdScanVerticalUp":
                        form.startGridSweep(GridMouseMover.Direction.Up);
                        break;

                    case "CmdLeftClick":
                        MouseUtils.SimulateLeftMouseClick();
                        break;

                    case "CmdLeftDoubleClick":
                        MouseUtils.SimulateLeftMouseDoubleClick();
                        break;

                    case "CmdLeftClickAndHold":
                        MouseUtils.SimulateLeftMouseDrag();
                        break;

                    case "CmdRightClick":
                        MouseUtils.SimulateRightMouseClick();
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
                Commands.Add(new GoBackCommandHandler("CmdGoBack"));
                Commands.Add(new CommandHandler("CmdScanVerticalDown"));
                Commands.Add(new CommandHandler("CmdScanVerticalUp"));
                Commands.Add(new CommandHandler("CmdLeftClick"));
                Commands.Add(new CommandHandler("CmdLeftDoubleClick"));
                Commands.Add(new CommandHandler("CmdLeftClickAndHold"));
                Commands.Add(new CommandHandler("CmdRightClick"));
            }
        }

        /// <summary>
        /// Handles exiting from the form. We had disabled panel
        /// change requests when this scanner was initialized. Enable
        /// it back.
        /// </summary>
        private class GoBackCommandHandler : GoBackHandler
        {
            /// <summary>
            /// Initializes a new instance of the class.
            /// </summary>
            /// <param name="cmd"></param>
            public GoBackCommandHandler(String cmd)
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
                Context.AppPanelManager.ResumePanelChangeRequests();
                return base.Execute(ref handled);
            }
        }
    }
}