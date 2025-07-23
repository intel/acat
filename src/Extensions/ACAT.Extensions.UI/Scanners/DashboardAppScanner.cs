using ACAT.Core.AgentManagement;
using ACAT.Core.Audit;
using ACAT.Core.PanelManagement;
using ACAT.Core.PanelManagement.CommandDispatcher;
using ACAT.Core.ThemeManagement;
using ACAT.Core.TTSManagement;
using ACAT.Core.Utility;
using ACAT.Core.WordPredictionManagement;
using ACAT.Extension;
using ACAT.Extension.CommandHandlers;
using ACAT.Extensions.UI.UserControls;
using ACAT.Scanners;
using ACATResources;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace ACAT.Extensions.UI.Scanners
{
    [ClassDescriptor("D3F4A1B2-3C4D-5E6F-7A8B-9A0B1C2D3E4F",
                Name = "DashboardAppScanner",
                Description = "Scanner for Dashboard Applications")]
    public partial class DashboardAppScanner : GenericScannerForm, ISupportsStatusBar
    {
        private TableLayoutPanel panelDashboardControls;
        private TableLayoutPanel panelTopToolbar;
        private TableLayoutPanel ScannerBorder;
        public DashboardAppScanner() : base()
        {
            _dispatcher = new DashboardAppDispatcher(this);
        }

        public override DefaultCommandDispatcher _dispatcher { get; }
        public override RunCommandDispatcher CommandDispatcher => _dispatcher;
        public ScannerStatusBar ScannerStatusBar
        {
            get { return ScannerCommon.StatusBar; }
        }


        public override ITextController TextController => ScannerCommon.TextController;
        
        public override bool CheckCommandEnabled(CommandEnabledArg arg)
        {
            switch (arg.Command)
            {
                case "CmdACATMenu":
                    break;
                case "CmdACATTalk":
                case "CmdQuick":
                case "CmdPointer":
                case "CmdKeyboard":
                case "CmdSystem":
                case "CmdLocation":
                    arg.Handled = true;
                    arg.Enabled = true;
                    break;

                default:
                    _scannerHelper.CheckCommandEnabled(arg);
                    break;
            }

            return true;
        }

        public override bool HandleInitialize(StartupArg startup)
        {
            _scannerCommon.UserControlManager.GridScanIterations = Common.AppPreferences.GridScanIterations;
            bool success = ScannerCommon.UserControlManager.AddUserControlByKeyOrName(panelTopToolbar, "toolbar", "ToolbarUserControl");
            success = success && ScannerCommon.UserControlManager.AddUserControlByKeyOrName(panelDashboardControls, "dashboard", "DashboardUserControl");

            return success;
        }

        private bool _transparencyIncreasing = false;

        protected override void InitializeComponent()
        {
            this.ScannerBorder = new TableLayoutPanel();
            this.panelTopToolbar = new TableLayoutPanel();
            this.panelDashboardControls = new TableLayoutPanel();

            this.panelTopToolbar.SuspendLayout();
            this.panelDashboardControls.SuspendLayout();
            this.SuspendLayout();

            InitializeTopToolbar();
            InitializeDashboard();

            this.ScannerBorder.BackColor = Color.Transparent;
            this.ScannerBorder.Name = "DashboardScannerBorder";
            this.ScannerBorder.AccessibleName = "DashboardScannerBorder";
            this.ScannerBorder.Margin = new Padding(10);
            this.ScannerBorder.ColumnCount = 1;
            this.ScannerBorder.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            this.ScannerBorder.RowCount = 2;
            this.ScannerBorder.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            this.ScannerBorder.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            this.ScannerBorder.Dock = DockStyle.Fill;
            this.ScannerBorder.AutoSize = true;
            this.ScannerBorder.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.ScannerBorder.Controls.Add(this.panelTopToolbar, 0, 0);
            this.ScannerBorder.Controls.Add(this.panelDashboardControls, 0, 1);

            this.Text = "ACAT Dashboard";
            this.Name = "DashboardAppScanner";
            this.BackColor = Color.FromArgb(35, 36, 51);
            this.ForeColor = Color.White;
            this.Controls.Add(ScannerBorder);
            this.ShowInTaskbar = true;
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.StartPosition = FormStartPosition.CenterScreen;

            this.panelTopToolbar.ResumeLayout(true);
            this.panelDashboardControls.ResumeLayout(true);
            this.ScannerBorder.ResumeLayout(true);
            this.ResumeLayout(true);
        }

        protected override void ScannerFormLoaded(object sender, EventArgs e)
        {
            var icon = ImageUtils.GetEntryAssemblyIcon();
            if (icon != null)
            {
                Icon = icon;
            }

            panelTopToolbar.Focus();
            _scannerCommon.OnLoad();

            SetColorScheme();

            _windowActiveWatchdog = new WindowActiveWatchdog(this);
        }

        protected override void ScannerShown(object sender, EventArgs e)
        {
            ScannerFocus.SetFocus(this);
        }

        protected override void SubscribeToEvents()
        {
            Load += ScannerFormLoaded;
            //Shown += ScannerShown;
            FormClosing += ScannerFormClosing;
        }

        public bool HandleCmdPointerControl()
        {
            _scannerCommon.UserControlManager.PushUserControlByKeyOrName(panelDashboardControls, "pointercontroller", "PointerControlUserControl");
            return true;
        }

        private void InitializeDashboard()
        {
            this.panelDashboardControls.Name = "Dashboard";
            this.panelDashboardControls.AccessibleName = "Dashboard";
            this.panelDashboardControls.Dock = DockStyle.Top;
            this.panelDashboardControls.AutoSize = true;
            this.panelDashboardControls.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        }

        private void InitializeTopToolbar()
        {
            this.panelTopToolbar.Name = "Toolbar";
            this.panelTopToolbar.AccessibleName = "Toolbar";
            this.panelTopToolbar.Dock = DockStyle.Top;
            this.panelTopToolbar.AutoSize = true;
            this.panelTopToolbar.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        }
        private class DashboardAppCommandHandler : RunCommandHandler
        {
            public DashboardAppCommandHandler(String cmd) : base(cmd) { }

            public override bool Execute(ref bool handled)
            {
                var form = Dispatcher.Scanner.Form as DashboardAppScanner;

                handled = Command switch
                {
                    "CmdShowKeyboard" => form.HandleCmdShowKeyboard(),
                    "CmdGoBack" => form.HandleCmdGoBack(),
                    "CmdShowPointerControl" => form.HandleCmdPointerControl(),
                    _ => false,
                };

                return true;
            }

            public override bool Execute(ref bool handled, object source = null)
            {
                handled = false;

                //var form = Dispatcher.Scanner.Form as DashboardAppScanner;

                //switch (Command)
                //{
                //    case "CmdGoBack":
                //        if (source is UserControl)
                //        {
                //            //var userControl = source as UserControl;
                //            form._scannerCommon.UserControlManager.PopUserControl(userControl.Parent);//form.panelKeyboard);
                //            form._scannerCommon.UserControlManager.StartTopLevelAnimation();
                //        }
                //        break;
                //}

                return true;
            }
        }

        private bool HandleCmdShowKeyboard()
        {
            _scannerCommon.UserControlManager.PushUserControlByKeyOrName(panelDashboardControls, "keyboard", "KeyboardUserControl");
            _scannerCommon.UserControlManager.StartTopLevelAnimation();
            return true;
        }

        private bool HandleCmdGoBack()
        {
            _scannerCommon.UserControlManager.PopUserControl(panelDashboardControls);
            _scannerCommon.UserControlManager.StartTopLevelAnimation();
            return true;
        }

        private class DashboardAppDispatcher : DefaultCommandDispatcher
        {
            public DashboardAppDispatcher(IScannerPanel panel) : base(panel)
            {
                Commands.Add(new DashboardAppCommandHandler("CmdACATMenu"));
                Commands.Add(new DashboardAppCommandHandler("CmdShowACATTalk"));
                Commands.Add(new DashboardAppCommandHandler("CmdShowQuickTalk"));
                Commands.Add(new DashboardAppCommandHandler("CmdShowPointerControl"));
                Commands.Add(new DashboardAppCommandHandler("CmdShowKeyboard"));
                Commands.Add(new DashboardAppCommandHandler("CmdShowSystem"));
                Commands.Add(new DashboardAppCommandHandler("CmdShowLocation"));
                Commands.Add(new DashboardAppCommandHandler("CmdShowSettings"));
                Commands.Add(new DashboardAppCommandHandler("CmdShowHelp"));
                Commands.Add(new DashboardAppCommandHandler("CmdShowAbout"));
                Commands.Add(new DashboardAppCommandHandler("CmdPanelSettings"));
                Commands.Add(new DashboardAppCommandHandler("CmdGoBack"));
                Commands.Add(new DashboardAppCommandHandler("CmdShrink"));
                Commands.Add(new DashboardAppCommandHandler("CmdGrow"));
                Commands.Add(new DashboardAppCommandHandler("CmdFade"));
                Commands.Add(new DashboardAppCommandHandler("CmdUnfade"));
            }
        }
    }
}
