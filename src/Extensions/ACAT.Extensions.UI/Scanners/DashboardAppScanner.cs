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
using System.Drawing;
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

            //this.ScannerBorder.Size = new Size(800, 800);
            this.ScannerBorder.BackColor = Color.Transparent;
            this.ScannerBorder.Name = "DashboardScannerBorder";
            this.ScannerBorder.AccessibleName = "DashboardScannerBorder";
            this.ScannerBorder.Margin = new Padding(10);
            this.ScannerBorder.ColumnCount = 1;
            this.ScannerBorder.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            this.ScannerBorder.RowCount = 2;
            this.ScannerBorder.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            this.ScannerBorder.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            this.ScannerBorder.Controls.Add(this.panelTopToolbar, 0, 0);
            this.ScannerBorder.Controls.Add(this.panelDashboardControls, 0, 1);
            this.ScannerBorder.Dock = DockStyle.Fill;
            this.ScannerBorder.AutoSize = true;
            this.ScannerBorder.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            this.Text = "ACAT Dashboard";
            this.Name = "DashboardAppScanner";
            this.BackColor = Color.FromArgb(35, 36, 51);
            this.ForeColor = Color.White;
            this.Controls.Add(ScannerBorder);
            this.ShowInTaskbar = true;
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.MinimumSize = new Size(1500, 254);

            this.panelTopToolbar.ResumeLayout();
            this.panelDashboardControls.ResumeLayout();
            this.ScannerBorder.ResumeLayout();
            this.ResumeLayout();
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

            _scannerCommon.ResizeToFitDesktop(this);

            _windowActiveWatchdog = new WindowActiveWatchdog(this);
        }

        protected override void ScannerShown(object sender, EventArgs e)
        {
            ScannerFocus.SetFocus(this);
        }

        protected override void SubscribeToEvents()
        {
            Load += ScannerFormLoaded;
            Shown += ScannerShown;
            FormClosing += ScannerFormClosing;
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
        private class CommandHandler : RunCommandHandler
        {
            public CommandHandler(String cmd) : base(cmd) { }

public override bool Execute(ref bool handled)
            {
                handled = true;

                var form = Dispatcher.Scanner.Form as DashboardAppScanner;

                switch (Command)
                {
                    case "CmdACATTalk":
                    case "CmdQuick":
                    case "CmdPointer":
                    case "CmdKeyboard":
                    case "CmdSystem":
                    case "CmdLocation":
                        MessageBox.Show("This command is not implemented yet.", "Not Implemented", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;


                    default:
                        handled = false;
                        break;
                }

                return true;
            }

            public override bool Execute(ref bool handled, object source = null)
            {

                var form = Dispatcher.Scanner.Form as DashboardAppScanner;

                switch (Command)
                {
                    default:
                    handled = false;
                        break;
                }

                return true;
            }
        }

        private class DashboardAppDispatcher : DefaultCommandDispatcher
        {
            public DashboardAppDispatcher(IScannerPanel panel) : base(panel)
            {
                //TODO: Add Command Handlers.
            }
        }
    }
}
