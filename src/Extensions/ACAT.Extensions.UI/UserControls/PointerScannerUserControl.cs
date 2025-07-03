using ACAT.Core.AnimationManagement;
using ACAT.Core.PanelManagement;
using ACAT.Core.PanelManagement.CommandDispatcher;
using ACAT.Core.UserControlManagement;
using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using ACAT.Extension;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static ACAT.Core.PanelManagement.ScannerPositionSizeController;

namespace ACAT.Extensions.UI.Scanners.UserControls
{
    [ClassDescriptor("802B03F0-1294-4D06-A601-2CEBFBFA5D9C",
                    "PointerScanner",
                    "User Control for Pointer Scanner")]
    public partial class PointerScannerUserControl : UserControl, IUserControl
    {
        //private Widget _rootWidget;
        //private ScannerCommon _scannerCommon;
        private UserControlKeyboardCommon _keyboardCommon;

        private readonly GridMouseMover _gridMouseMover = new GridMouseMover
        {
            GridRectangleSpeed = GetOptionalProperty(Common.AppPreferences, "MouseGridRectangleSpeed", 40),
            GridRectangleCycles = GetOptionalProperty(Common.AppPreferences, "MouseGridRectangleCycles", 2),
            GridLineSpeed = GetOptionalProperty(Common.AppPreferences, "MouseGridLineSpeed", 20),
            GridLineCycles = GetOptionalProperty(Common.AppPreferences, "MouseGridRectangleCycles", 1),
            GridLineThickness = GetOptionalProperty(Common.AppPreferences, "MouseGridLineThickness", 2),
            EnableVerticalGridRectangle = true
        };

        private static int GetOptionalProperty(object obj, string propertyName, int fallback = 0)
        {
            var prop = obj.GetType().GetProperty(propertyName);
            return prop != null ? (int)prop.GetValue(obj) : fallback;
        }

        //public override bool CheckCommandEnabled(CommandEnabledArg arg)
        //{
        //    switch (arg.Command)
        //    {
        //        case "ScanDown":
        //        case "ScanUp":
        //        case "CmdLeftClick":
        //        case "CmdLeftDoubleClick":
        //        case "CmdLeftClickAndHold":
        //        case "CmdRightClick":
        //        case "CmdGoBack":
        //            {
        //                arg.Enabled = true;
        //                arg.Handled = true;
        //                break;
        //            }
        //    }
        //    return true;
        //}

        private void startGridSweep(GridMouseMover.Direction direction)
        {
            OnPause(); // Stop any current animation
            _gridMouseMover.GridRectangleDirection = direction;
            _gridMouseMover.Start();
            OnResume(); // Resume animation
        }

        public PointerScannerUserControl()
            : base()
        {
            InitializeComponent();

            //commandDispatcher.Commands.Add(new CommandHandler("ScanDown"));
            //commandDispatcher.Commands.Add(new CommandHandler("ScanUp"));
            //commandDispatcher.Commands.Add(new CommandHandler("CmdLeftClick"));
            //commandDispatcher.Commands.Add(new CommandHandler("CmdLeftDoubleClick"));
            //commandDispatcher.Commands.Add(new CommandHandler("CmdLeftClickAndHold"));
            //commandDispatcher.Commands.Add(new CommandHandler("CmdRightClick"));
            //commandDispatcher.Commands.Add(new CommandHandler("CmdGoBack"));
        }

        public ClassDescriptorAttribute Descriptor => ClassDescriptorAttribute.GetDescriptor(GetType());

        public SyncLock SyncObj => _keyboardCommon.SyncObj;

        #region UserControl Members
        private void InitializeComponent()
        {
            PointerControlsBox = new TableLayoutPanel
            {
                AccessibleName = "PointerControlsBox",
                AccessibleRole = AccessibleRole.Grouping,
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 1,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                GrowStyle = TableLayoutPanelGrowStyle.AddColumns,
                Size = new Size(550, 100)
            };
            PointerControlsBox.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            PointerControlsBox.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            //TODO: Load these from the PanelClass.XML file
            var buttons = new List<ScannerRoundedButtonControl>
                {
                    new ScannerRoundedButtonControl { Name = "GoBack", Text = "Go Back" },
                    new ScannerRoundedButtonControl { Name = "ScanDown", Text = "Scan Down" },
                    new ScannerRoundedButtonControl { Name = "ScanUp", Text = "Scan Up" },
                    new ScannerRoundedButtonControl { Name = "AutoPosition", Text = "AutoPosition" },
                    new ScannerRoundedButtonControl { Name = "LeftClick", Text = "LEft Click" },
                    new ScannerRoundedButtonControl { Name = "RightClick", Text = "Right Click" },
                    new ScannerRoundedButtonControl { Name = "LeftDoubleClick", Text = "Left Double Click" },
                    new ScannerRoundedButtonControl { Name = "LeftClickAndHold", Text = "Left Click and Hold" }
                };

            foreach (var button in buttons)
            {
                button.AccessibleName = button.Name;
                button.AccessibleRole = AccessibleRole.PushButton;
                //button.BackColor = Color.Transparent;
                button.ImageAlign = ContentAlignment.MiddleCenter;
                button.Size = new Size(80, 80);
                button.BorderColor = System.Drawing.Color.DimGray;
                button.BorderRadiusBottomLeft = 12;
                button.BorderRadiusBottomRight = 12;
                button.BorderRadiusTopLeft = 12;
                button.BorderRadiusTopRight = 12;
                button.BorderWidth = 3F;
                button.ForeColor = Color.White;
                var col = PointerControlsBox.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

                button.Click += ButtonClicked;
                PointerControlsBox.Controls.Add(button, col, 0);
            }

            this.AccessibleName = "PointerScanner";
            this.AccessibleRole = AccessibleRole.Grouping;
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.Name = "PointerScanner";
            this.Margin = new Padding(10);
            //this.BackColor = Color.Transparent;
            this.Controls.Add(PointerControlsBox);
        }

        private void ButtonClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;

            if (button != null)
            {
                switch (button.Name)
                {
                    case "ScanUp":
                        startGridSweep(GridMouseMover.Direction.Up);
                        break;
                    case "ScanDown":
                        startGridSweep(GridMouseMover.Direction.Down);
                        break;
                }
            }
        }

        private TableLayoutPanel PointerControlsBox;

        //public SyncLock SyncObj => _scannerCommon.SyncObj;

        public IUserControlCommon UserControlCommon => throw new NotImplementedException();

        public event AnimationPlayerStateChanged EvtPlayerStateChanged;
        #endregion


        public bool Initialize(UserControlConfigMapEntry mapEntry, TextController textController, IScannerPanel scanner)
        {
            return true;
        }

        private void AnimationManager_EvtPlayerStateChanged(object sender, PlayerStateChangedEventArgs e)
        {
            EvtPlayerStateChanged?.Invoke(this, e);
        }

        public void OnLoad()
        {
        }

        public void OnPause()
        {
        }

        public void OnResume()
        {
        }

        public void OnWidgetActuated(WidgetActuatedEventArgs e, ref bool handled)
        {
            handled = false;
        }

        private class CommandHandler : RunCommandHandler
        {
            private PointerScannerUserControl _scannerControl { get; set; }
            public CommandHandler(string command) 
                : base(command)
            {
            }

            public override bool Execute(ref bool handled)
            {

                switch (Command)
                {
                    case "ScanDown":
                        _scannerControl.startGridSweep(GridMouseMover.Direction.Down);
                        handled = true;
                        break;
                    case "ScanUp":
                        _scannerControl.startGridSweep(GridMouseMover.Direction.Up);
                        handled = true;
                        break;
                }
                if (!handled)
                {
                    return base.Execute(ref handled);
                }
                return handled;
            }
        }
    }
}
