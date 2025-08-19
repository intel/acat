using ACAT.Core.AgentManagement;
using ACAT.Core.AgentManagement.Interfaces;
using ACAT.Core.PanelManagement;
using ACAT.Core.PanelManagement.CommandDispatcher;
using ACAT.Core.PanelManagement.Common;
using ACAT.Core.PanelManagement.Interfaces;
using ACAT.Core.UserControlManagement.Interfaces;
using ACAT.Core.Utility;
using ACAT.Extension;
using ACAT.Extension.CommandHandlers;
using ACAT.Extension.UI.ScannerForms;
using ACAT.Extensions.UI.UserControls.Toolbars;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ACAT.Extensions.UI.Scanners
{
    [ClassDescriptor("D3F4A1B2-3C4D-5E6F-7A8B-9A0B1C2D3E4F",
                Name = "DashboardAppScanner",
                Description = "Scanner for Dashboard Applications")]
    [DesignerCategory("code")]
    public partial class DashboardAppScanner : GenericScannerForm
    {
        private TableLayoutPanel panelDashboardControls;
        private TableLayoutPanel panelTopToolbar;
        private TableLayoutPanel ScannerBorder;

        private UserControl currentPanel = null;

        public DashboardAppScanner() : base()
        {
            _dispatcher = new DashboardAppDispatcher(this);
            _launchAppAgent = Context.AppAgentMgr.GetFunctionalAgentByName("LaunchAppAgent") as IFunctionalAgent;
            if (_launchAppAgent == null)
            {
                Log.Error("LaunchAppAgent not found. Ensure it is registered in the ACAT configuration.");
            }
        }

        private IFunctionalAgent _launchAppAgent { get; set; }

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
            ScannerCommon.UserControlManager.GridScanIterations = Common.AppPreferences.GridScanIterations;
            bool success = ScannerCommon.UserControlManager.AddUserControlByKeyOrName(panelTopToolbar, "toolbar", "ToolbarUserControl");
            success = success && ScannerCommon.UserControlManager.AddUserControlByKeyOrName(panelDashboardControls, "dashboard", "DashboardUserControl");

            return success;
        }

        //private bool _transparencyIncreasing = false;

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

            // Subscribe to the change event on the panelDashboardControls to
            // handle setting what the current panel is.
            this.panelDashboardControls.ControlAdded += (s, e) =>
            {
                if (e.Control is UserControl userControl)
                {
                    currentPanel = userControl;
                    Log.Debug($"Current panel set to: {currentPanel.GetType().Name}");
                }
            };
        }

        protected override void ScannerFormLoaded(object sender, EventArgs e)
        {
            var icon = ImageUtils.GetEntryAssemblyIcon();
            if (icon != null)
            {
                Icon = icon;
            }

            panelTopToolbar.Focus();
            ScannerCommon.OnLoad();

            SetColorScheme();

        }


        protected override void SubscribeToEvents()
        {
            Load += ScannerFormLoaded;
            //Shown += ScannerShown;
            FormClosing += ScannerFormClosing;
        }

        public bool HandleCmdPointerControl()
        {
            bool success = ScannerCommon.UserControlManager.PushUserControlByKeyOrName(panelDashboardControls, "cursorcontroller", "CursorControlUserControl");
            ScannerCommon.UserControlManager.StartTopLevelAnimation();
            return success;
        }

        public bool HandleCmdShowSystem()
        {
            ScannerCommon.UserControlManager.PushUserControlByKeyOrName(panelDashboardControls, "supportedapps", "LaunchAppUserControl");

            if (_launchAppAgent != null) 
            {
                var uc = panelDashboardControls.Controls.Find("LaunchAppUserControl", true);
                _launchAppAgent?.Activate(uc.Length > 0 ? uc[0] as IUserControl : null);
            }

            ScannerCommon.UserControlManager.StartTopLevelAnimation();
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
            private class CommandHandlerArgs :EventArgs
            {
                public CommandHandlerArgs(String command)
                {
                    Command = command;
                }
                public String Command { get; }

                public override string ToString()
                {
                    return Command;
                }
            }

            public DashboardAppCommandHandler(String cmd) : base(cmd) { }

            public override bool Execute(ref bool handled)
            {
                Log.Info($"Executing command: {Command}");
                var form = Dispatcher.Scanner.Form as DashboardAppScanner;

                handled = Command switch
                {
                    "CmdShowKeyboard" => form.HandleCmdShowKeyboard(),
                    "CmdGoBack" => form.HandleCmdGoBack(),
                    "CmdShowPointerControl" => form.HandleCmdPointerControl(),
                    "CmdShowSystem" => form.HandleCmdShowSystem(),
                    _ => false,
                };

                return true;
            }

            public override bool Execute(ref bool handled, object source = null)
            {
                var form = Dispatcher.Scanner.Form as DashboardAppScanner;

                form.ScannerCommon.UserControlManager.StopTopLevelAnimation();
                form.Visible = false;

                Log.Info($"Executing command: {Command} from source: {source?.GetType().Name}");

                LargeToolbarUserControl largeToolbarUserControl = source as LargeToolbarUserControl;                
                largeToolbarUserControl?.OnButtonClicked(source, new CommandHandlerArgs(Command));
                handled = true;

                form.Visible = true;
                form.ScannerCommon.UserControlManager.StartTopLevelAnimation();
                return true;
            }
        }

        private bool HandleCmdShowKeyboard()
        {
            ScannerCommon.UserControlManager.PushUserControlByKeyOrName(panelDashboardControls, "keyboard", "KeyboardUserControl");
            ScannerCommon.UserControlManager.StartTopLevelAnimation();
            return true;
        }

        private bool HandleCmdGoBack()
        {
            ScannerCommon.UserControlManager.PopUserControl(panelDashboardControls);
            ScannerCommon.UserControlManager.StartTopLevelAnimation();
            return true;
        }

        private class DashboardAppDispatcher : DefaultCommandDispatcher
        {
            public DashboardAppDispatcher(IScannerPanel panel) : base(panel)
            {
                /* Main Menu Commands */
                Commands.Add(new DashboardAppCommandHandler("CmdShowACATTalk"));
                Commands.Add(new DashboardAppCommandHandler("CmdShowQuickTalk"));
                Commands.Add(new DashboardAppCommandHandler("CmdShowPointerControl"));
                Commands.Add(new DashboardAppCommandHandler("CmdShowKeyboard"));
                Commands.Add(new DashboardAppCommandHandler("CmdShowSystem"));
                Commands.Add(new DashboardAppCommandHandler("CmdShowLocation"));
                
                /* Top Toolbar Commands */
                Commands.Add(new DashboardAppCommandHandler("CmdACATMenu"));
                Commands.Add(new DashboardAppCommandHandler("CmdShowSettings"));
                Commands.Add(new DashboardAppCommandHandler("CmdShowHelp"));
                Commands.Add(new DashboardAppCommandHandler("CmdShowAbout"));
                Commands.Add(new DashboardAppCommandHandler("CmdPanelSettings"));
                Commands.Add(new DashboardAppCommandHandler("CmdGoBack"));
                Commands.Add(new DashboardAppCommandHandler("CmdShrink"));
                Commands.Add(new DashboardAppCommandHandler("CmdGrow"));
                Commands.Add(new DashboardAppCommandHandler("CmdFade"));
                Commands.Add(new DashboardAppCommandHandler("CmdUnfade"));

                /* Cursor Control Commands */
                Commands.Add(new DashboardAppCommandHandler("MoveAndClick"));
                Commands.Add(new DashboardAppCommandHandler("Move"));
                Commands.Add(new DashboardAppCommandHandler("LeftClick"));
                Commands.Add(new DashboardAppCommandHandler("RightClick"));
                Commands.Add(new DashboardAppCommandHandler("ClickHold"));
                Commands.Add(new DashboardAppCommandHandler("ScrollUp"));
                Commands.Add(new DashboardAppCommandHandler("ScrollDown"));

                /* Launch App Commands */
                Commands.Add(new DashboardAppCommandHandler("Chrome"));
                Commands.Add(new DashboardAppCommandHandler("Edge"));
            }
        }
    }
}
