// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// MouseScanner.cs
// This form is for the ACAT Mouse Scanner applications.  This scanner
// enables the user to move the mouse around the display using grid scanning
// technique.  The user can also click, double click, drag, right click etc.

using ACAT.Lib.Core.Audit;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
using ACAT.Lib.Core.Utility;
using System;
using System.Windows.Forms;
using ACAT.Lib.Core.ActuatorManagement;
using ACAT.Lib.Core.WidgetManagement;
using ACAT.Lib.Extension.CommandHandlers;
using ACAT.Lib.Core.AgentManagement;
using System.Security.Permissions;

namespace ACAT.Lib.Extension
{
    [DescriptorAttribute("802B03F0-1294-4D06-A601-2CEBFBFA5D9C",
                        "MouseScanner",
                        "Enables mouse placement and mouse action on the display")]
    public partial class MouseScanner : Form, IScannerPanel
    {
        private readonly Dispatcher _dispatcher;

        private Widget _rootWidget;

        private ScannerCommon _scannerCommon;

        public MouseScanner()
        {
            InitializeComponent();
            _dispatcher = new Dispatcher(this);
        }

        public MouseScanner(String panelClass, String title) : this()
        {
            Load += MouseScanner_Load;
            FormClosing += MouseScanner_FormClosing;
            Text = title;
        }

        public virtual RunCommandDispatcher CommandDispatcher => _dispatcher;

        public IDescriptor Descriptor => DescriptorAttribute.GetDescriptor(GetType());

        public Form Form => this;

        public String PanelClass
        {
            get;
            protected set;
        }

        public IPanelCommon PanelCommon => _scannerCommon;

        public ScannerCommon ScannerCommon => _scannerCommon;


        public SyncLock SyncObj => _scannerCommon.SyncObj;

        public ITextController TextController => _scannerCommon.TextController;

        protected override CreateParams CreateParams => Windows.SetFormStyles(base.CreateParams);

        protected override bool ShowWithoutActivation => true;

        public virtual bool CheckCommandEnabled(CommandEnabledArg arg)
        {
            throw new NotImplementedException();
        }

        public virtual bool Initialize(StartupArg startupArg)
        {
            PanelClass = startupArg.PanelClass;

            _scannerCommon = new ScannerCommon(this)
            {
                PositionSizeController = { AutoPosition = false }
            };

            if (!_scannerCommon.Initialize(startupArg))
            {
                return false;
            }

            _rootWidget = PanelCommon.RootWidget;
            return true;
        }

        public void OnFocusChanged(WindowActivityMonitorInfo monitorInfo)
        {
            _scannerCommon.OnFocusChanged(monitorInfo);
        }

        public virtual void OnPause()
        {
            _scannerCommon.OnPause();
        }

        public virtual bool OnQueryPanelChange(PanelRequestEventArgs eventArg)
        {
            return true;
        }

        public virtual void OnResume()
        {
            _scannerCommon.OnResume();
        }

        public void OnWidgetActuated(WidgetActuatedEventArgs e, ref bool handled)
        {
            throw new NotImplementedException();
        }

        public void SetTargetControl(Form parent, Widget widget)
        {
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _scannerCommon.OnFormClosing(e);
            base.OnFormClosing(e);
        }
        [EnvironmentPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        [EnvironmentPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        protected override void WndProc(ref Message m)
        {
            _scannerCommon?.HandleWndProc(m);
            base.WndProc(ref m);
        }

        private void MouseScanner_FormClosing(object sender, FormClosingEventArgs e)
        {
            _scannerCommon.OnClosing();
            _scannerCommon.Dispose();
        }

        private void MouseScanner_Load(object sender, EventArgs e)
        {
            _scannerCommon.OnLoad();
            PanelCommon.AnimationManager.Start(_rootWidget);
        }

        private readonly IActuator _calibrationSupportedActuator;
        private readonly bool _enableScanTimingConfigure = true;
        private GridMouseMover _gridMouseMover;

        ///// <summary>
        ///// Initializes a new instance of the <see cref="ACATForm"/> class.
        ///// </summary>
        //public MouseScanner(String panelClass, String panelTitle) 
        //    : base(panelClass, StringResources.MouseScanner)
        //{
        //    commandDispatcher.Commands.Add(new CommandHandler("CmdScanVerticalDown"));
        //    commandDispatcher.Commands.Add(new CommandHandler("CmdScanVerticalUp"));
        //    commandDispatcher.Commands.Add(new CommandHandler("CmdLeftClick"));
        //    commandDispatcher.Commands.Add(new CommandHandler("CmdLeftDoubleClick"));
        //    commandDispatcher.Commands.Add(new CommandHandler("CmdLeftClickAndHold"));
        //    commandDispatcher.Commands.Add(new CommandHandler("CmdRightClick"));
        //    commandDispatcher.Commands.Add(new CommandHandler("CmdGoBack"));

        //    _calibrationSupportedActuator = Context.AppActuatorManager.GetCalibrationSupportedActuator();

        //    Load += MouseScanner_Load;

        //    InitializeComponent();

        //    // Add the mouse control buttons to the flow layout
        //    // panel.  These are the buttons that will be used to control
        //    // the mouse movement and actions.

        //    var buttons = new List<ScannerButtonControl>
        //    {
        //        new ScannerButtonControl { Name = "GoBack", AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Dock = DockStyle.Fill },
        //        new ScannerButtonControl { Name = "ScanDown", AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Dock = DockStyle.Fill },
        //        new ScannerButtonControl { Name = "ScanUp", AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Dock = DockStyle.Fill },
        //        new ScannerButtonControl { Name = "AutoPosition", AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Dock = DockStyle.Fill },
        //        new ScannerButtonControl { Name = "LeftClick", AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Dock = DockStyle.Fill },
        //        new ScannerButtonControl { Name = "RightClick", AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Dock = DockStyle.Fill },
        //        new ScannerButtonControl { Name = "LeftDoubleClick", AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Dock = DockStyle.Fill },
        //        new ScannerButtonControl { Name = "LeftClickAndHold", AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Dock = DockStyle.Fill }
        //    };

        //    MouseScannerButtons.Controls.AddRange(buttons.ToArray());
        //}


        private GridMouseMover createGridMouseMover()
        {
            var gridMouseMover = new GridMouseMover
            {
                GridRectangleSpeed = GetOptionalProperty(Common.AppPreferences, "MouseGridRectangleSpeed", 40),
                GridRectangleCycles = GetOptionalProperty(Common.AppPreferences, "MouseGridRectangleCycles", 2),
                GridLineSpeed = GetOptionalProperty(Common.AppPreferences, "MouseGridLineSpeed", 20),
                GridLineCycles = GetOptionalProperty(Common.AppPreferences, "MouseGridRectangleCycles", 1),
                GridLineThickness = GetOptionalProperty(Common.AppPreferences, "MouseGridLineThickness", 2),
                EnableVerticalGridRectangle = true
            };

            return gridMouseMover;
        }

        private int GetOptionalProperty(object obj, string propertyName, int fallback = 0)
        {
            var prop = obj.GetType().GetProperty(propertyName);
            return prop != null ? (int)prop.GetValue(obj) : fallback;
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
        /// Starts moving the mouse in the grid mode, in the specified
        /// direction
        /// </summary>
        /// <param name="direction">down or up</param>
        private void startGridSweep(GridMouseMover.Direction direction)
        {

            _gridMouseMover = createGridMouseMover();

            AuditLog.Audit(new AuditEventMouseMover(direction.ToString()));

            _gridMouseMover.GridRectangleDirection = direction;

            _gridMouseMover.Start();

            _gridMouseMover = null;
        }

        ///// <summary>
        ///// Handles all  the commands for the mouse scanner
        ///// </summary>
        //private class CommandHandler : RunCommandHandler
        //{
        //    /// <summary>
        //    /// Initializes an instance of the handler
        //    /// </summary>
        //    /// <param name="cmd">the command</param>
        //    public CommandHandler(String cmd)
        //        : base(cmd)
        //    {
        //    }

        //    /// <summary>
        //    /// Executes the command
        //    /// </summary>
        //    /// <param name="handled">set to true if handled</param>
        //    /// <returns>true</returns>
        //    public override bool Execute(ref bool handled)
        //    {
        //        handled = true;

        //        var form = Dispatcher.Scanner.Form as MouseScanner;

        //        switch (Command)
        //        {
        //            case "CmdScanVerticalDown":
        //                form.startGridSweep(GridMouseMover.Direction.Down);
        //                break;

        //            case "CmdScanVerticalUp":
        //                form.startGridSweep(GridMouseMover.Direction.Up);
        //                break;

        //            case "CmdLeftClick":
        //                MouseUtils.SimulateLeftMouseClick();
        //                break;

        //            case "CmdLeftDoubleClick":
        //                MouseUtils.SimulateLeftMouseDoubleClick();
        //                break;

        //            case "CmdLeftClickAndHold":
        //                MouseUtils.SimulateLeftMouseDrag();
        //                break;

        //            case "CmdRightClick":
        //                MouseUtils.SimulateRightMouseClick();
        //                break;

        //            default:
        //                handled = false;
        //                break;
        //        }

        //        return true;
        //    }
        //}

        public class Dispatcher : DefaultCommandDispatcher
        { 
            public Dispatcher(IScannerPanel panel)
                : base(panel)
            {
            }
        }
    }
}