////////////////////////////////////////////////////////////////////////////
// <copyright file="MouseScanner2.cs" company="Intel Corporation">
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
using ACAT.Lib.Core.Audit;
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
#endregion

namespace ACAT.Extensions.Default.UI.Scanners
{
    /// <summary>
    /// This scanner enables the user to move the mouse around
    /// the screen. It uses two modes of scanning - grid and
    /// radar.  User can also click, double click, drag, 
    /// right click etc.
    /// </summary>
    [DescriptorAttribute("0190D5B7-FA40-48A9-B888-267C60072C57", "MouseScanner", "Mouse Scanner (Smaller version)")]
    public partial class MouseScanner2 : Form, IScannerPanel, ISupportsStatusBar
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
        /// The current mode - grid or radar
        /// </summary>
        private MouseMode _mode = MouseMode.None;

        /// <summary>
        /// Moves the mouse in the radar mode
        /// </summary>
        private RadarMouseMover _radarMouseMover;

        /// <summary>
        /// The ScannerCommon object
        /// </summary>
        private ScannerCommon _scannerCommon;

        /// <summary>
        /// The ScannerHelper object
        /// </summary>
        private ScannerHelper _scannerHelper;
        /// <summary>
        /// Status bar object for this scanner
        /// </summary>
        private ScannerStatusBar _statusBar;
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public MouseScanner2()
        {
            InitializeComponent();
            createStatusBar();
            Load += MouseScannerScreen_Load;
            FormClosing += MouseScannerScreen_FormClosing;
            PanelClass = PanelClasses.Mouse;
            _dispatcher = new Dispatcher(this);
        }

        /// <summary>
        /// Scanning mode
        /// </summary>
        private enum MouseMode
        {
            None,
            Radar,
            Grid
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
            get { return _statusBar; }
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
        public bool CheckWidgetEnabled(CheckEnabledArgs arg)
        {
            return _scannerHelper.CheckWidgetEnabled(arg);
        }

        /// <summary>
        /// Initialzes the scanner
        /// </summary>
        /// <param name="startupArg">Starting arguments</param>
        /// <returns>true on success</returns>
        public bool Initialize(StartupArg startupArg)
        {
            _scannerCommon = new ScannerCommon(this);
            _scannerHelper = new ScannerHelper(this, startupArg);

            if (!_scannerCommon.Initialize(startupArg))
            {
                Log.Debug("Could not initialize form " + Name);
                return false;
            }

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
            _scannerCommon.HandleWndProc(m);
            base.WndProc(ref m);
        }
        /// <summary>
        /// Event handler for state changes in the grid mouse mover
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arg</param>
        private void _gridMouseMover_EvtMouseMoverStateChanged(object sender, MouseMoverStateChangedEventArgs e)
        {
            handleStateChanged(e);
        }

        /// <summary>
        /// Event handler for state changes in the radar mouse mover
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arg</param>
        private void _radarMouseMover_EvtMouseMoverStateChanged(object sender, MouseMoverStateChangedEventArgs e)
        {
            handleStateChanged(e);
        }

        /// <summary>
        /// Invoked when a switch is activated.  Inform the radar/grid
        /// mover objects that a switch activation was detected
        /// </summary>
        /// <param name="switchObj">Switch that was activated</param>
        /// <param name="handled">true if it was handled</param>
        private void AppActuatorManager_EvtSwitchHook(IActuatorSwitch switchObj, ref bool handled)
        {
            handled = false;

            switch (_mode)
            {
                case MouseMode.Radar:
                    handled = _radarMouseMover.Actuate();
                    break;

                case MouseMode.Grid:
                    handled = _gridMouseMover.Actuate();
                    break;
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
                Cycles = Common.AppPreferences.MouseGridVerticalSweeps,
                Sweeps = Common.AppPreferences.MouseGridHorizontalSweeps,
                MouseSpeed = Common.AppPreferences.MouseGridHorizontalSpeed,
                ScanSpeed = Common.AppPreferences.MouseGridVerticalSpeed,
                LineWidth = Common.AppPreferences.MouseGridLineWidth,
                StartFromLastCursorPos = Common.AppPreferences.MouseGridStartFromLastCursorPos,
                GridScanSpeedMultiplier = Common.AppPreferences.MouseGridScanSpeedMultiplier,
                MouseMoveSpeedMultiplier = Common.AppPreferences.MouseGridMouseMoveSpeedMultiplier
            };
            gridMouseMover.Init();
            gridMouseMover.EvtMouseMoverStateChanged += _gridMouseMover_EvtMouseMoverStateChanged;

            return gridMouseMover;
        }

        /// <summary>
        /// Creates and initializes the object that will handle radar mouse 
        /// movement.
        /// </summary>
        /// <returns>The created object</returns>
        private RadarMouseMover createRadarMouseMover()
        {
            var radarMouseMover = new RadarMouseMover
            {
                RotatingSweeps = Common.AppPreferences.MouseRadarRotatingSweeps,
                RadialSweeps = Common.AppPreferences.MouseRadarRadialSweeps,
                RadialSpeed = Common.AppPreferences.MouseRadarRadialSpeed,
                RotatingSpeed = Common.AppPreferences.MouseRadarRotatingSpeed,
                LineWidth = Common.AppPreferences.MouseRadarLineWidth,
                StartFromLastCursorPos = Common.AppPreferences.MouseRadarStartFromLastCursorPos,
                RotatingSpeedMultiplier = Common.AppPreferences.MouseRadarRotatingSpeedMultiplier,
                RadialSpeedMultiplier = Common.AppPreferences.MouseRadarRadialSpeedMultipler
            };

            radarMouseMover.Init();
            radarMouseMover.EvtMouseMoverStateChanged += _radarMouseMover_EvtMouseMoverStateChanged;
            return radarMouseMover;
        }

        /// <summary>
        /// Creates the statusbar object for this scanner
        /// </summary>
        private void createStatusBar()
        {
            if (_statusBar == null)
            {
                _statusBar = new ScannerStatusBar
                {
                    AltStatus = BAltStatus,
                    CtrlStatus = BCtrlStatus,
                    FuncStatus = BFuncStatus,
                    ShiftStatus = BShiftStatus,
                    LockStatus = BLockStatus
                };
            }
        }

        /// <summary>
        /// Disposes the grid mouse move object
        /// </summary>
        private void disposeGridMouseMover()
        {
            if (_gridMouseMover != null)
            {
                _gridMouseMover.EvtMouseMoverStateChanged -= _gridMouseMover_EvtMouseMoverStateChanged;
                _gridMouseMover.Dispose();
            }
        }

        /// <summary>
        /// Disposes the radar mouse move object
        /// </summary>
        private void disposeRadarMouseMover()
        {
            if (_radarMouseMover != null)
            {
                _radarMouseMover.EvtMouseMoverStateChanged -= _radarMouseMover_EvtMouseMoverStateChanged;
                _radarMouseMover.Dispose();
            }
        }

        /// <summary>
        /// Handles state changes in the grid/radar mouse movers.  If the 
        /// scanner is paused, resumes animation, and resets the mode
        /// </summary>
        /// <param name="e">event args</param>
        private void handleStateChanged(MouseMoverStateChangedEventArgs e)
        {
            if (e.State == MouseMoverStates.Idle)
            {
                if (_scannerCommon.IsPaused)
                {
                    resume();
                }

                _mode = MouseMode.None;
            }
        }

        /// <summary>
        /// Checks if either the radar or grid mouse movers are
        /// currently idle
        /// </summary>
        /// <returns>true if they are </returns>
        private bool mouseMoversIdle()
        {
            return _radarMouseMover.IsIdle() && _gridMouseMover.IsIdle();
        }

        /// <summary>
        /// Event handler for mouse down.  Treat this as a switch
        /// activation.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="mouseEventArgs">event args</param>
        private void MouseScannerScreen_EvtMouseDown(object sender, MouseEventArgs mouseEventArgs)
        {
            switch (_mode)
            {
                case MouseMode.Radar:
                    _radarMouseMover.Actuate();
                    break;

                case MouseMode.Grid:
                    _gridMouseMover.Actuate();
                    break;
            }
        }

        /// <summary>
        /// Form is closing. Dispose resources
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arg</param>
        private void MouseScannerScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            disposeRadarMouseMover();

            disposeGridMouseMover();

            _scannerCommon.OnClosing();
            _scannerCommon.Dispose();
        }

        /// <summary>
        /// Form loader.  Initialize, subscribe to events.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arg</param>
        private void MouseScannerScreen_Load(object sender, EventArgs e)
        {
            _scannerCommon.HideTalkWindow();

            _scannerCommon.OnLoad();

            _radarMouseMover = createRadarMouseMover();
            _gridMouseMover = createGridMouseMover();

            Context.AppActuatorManager.EvtSwitchHook += AppActuatorManager_EvtSwitchHook;
            var actuator = Context.AppActuatorManager.GetActuator(typeof(KeyboardActuator));
            if (actuator is KeyboardActuator)
            {
                _keyboardActuator = actuator as KeyboardActuator;
                _keyboardActuator.EvtMouseDown += MouseScannerScreen_EvtMouseDown;
            }

            _scannerCommon.GetAnimationManager().Start(_scannerCommon.GetRootWidget());
        }

        /// <summary>
        /// Pause the scanner
        /// </summary>
        private void pause()
        {
            Log.Debug();

            _scannerCommon.GetAnimationManager().Pause();

            _scannerCommon.HideScanner();

            _scannerCommon.OnPause();
        }

        /// <summary>
        /// Resumes the scanner
        /// </summary>
        private void resume()
        {
            Log.Debug();

            _scannerCommon.ShowScanner();

            // takes care of partial transparency with grid mouse where
            // status bar is transparent after grid mouse stops.
            Refresh();

            _scannerCommon.GetAnimationManager().Resume();

            _scannerCommon.OnResume();
        }

        /// <summary>
        /// Starts moving the mouse in the grid mode, in the specified
        /// direction
        /// </summary>
        /// <param name="direction">down or up</param>
        private void startGridSweep(GridMouseMover.GridSweepDirections direction)
        {
            if (_gridMouseMover.IsIdle())
            {
                _mode = MouseMode.Grid;

                pause();
                _gridMouseMover.SetGridSweepDirection(direction);

                AuditLog.Audit(new AuditEventMouseMover("grid", direction.ToString()));

                _gridMouseMover.Start();
            }
        }

        /// <summary>
        /// Starts moving the mouse in the radar mode, in the specified
        /// direction
        /// </summary>
        /// <param name="direction">clockwise/counter-clockwise</param>
        private void startRadarSweep(RadarMouseMover.RadarSweepDirections direction)
        {
            if (_radarMouseMover.IsIdle())
            {
                _mode = MouseMode.Radar;

                pause();

                _radarMouseMover.SetSweepDirection(direction);

                AuditLog.Audit(new AuditEventMouseMover("radar", direction.ToString()));

                _radarMouseMover.Start();
            }
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

                var form = Dispatcher.Scanner.Form as MouseScanner2;

                switch (Command)
                {
                    case "CmdRadarCounterClockwise":
                        form.startRadarSweep(RadarMouseMover.RadarSweepDirections.CounterClockwise);
                        break;

                    case "CmdRadarClockwise":
                        form.startRadarSweep(RadarMouseMover.RadarSweepDirections.ClockWise);
                        break;

                    case "CmdScanVerticalDown":
                        form.startGridSweep(GridMouseMover.GridSweepDirections.TopDown);
                        break;

                    case "CmdScanVerticalUp":
                        form.startGridSweep(GridMouseMover.GridSweepDirections.BottomUp);
                        break;

                    case "CmdLeftClick":
                        if (form.mouseMoversIdle())
                        {
                            MouseUtils.SimulateLeftMouseClick();
                        }

                        break;

                    case "CmdLeftDoubleClick":
                        if (form.mouseMoversIdle())
                        {
                            MouseUtils.SimulateLeftMouseDoubleClick();
                        }

                        break;

                    case "CmdLeftClickAndHold":
                        if (form.mouseMoversIdle())
                        {
                            MouseUtils.SimulateLeftMouseDrag();
                        }

                        break;

                    case "CmdRightClick":
                        if (form.mouseMoversIdle())
                        {
                            MouseUtils.SimulateRightMouseClick();
                        }

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
                Commands.Add(new CommandHandler("CmdRadarCounterClockwise"));
                Commands.Add(new CommandHandler("CmdRadarClockwise"));
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
