using ACAT.Core.AgentManagement;
using ACAT.Core.PanelManagement;
using ACAT.Core.PanelManagement.CommandDispatcher;
using ACAT.Core.ThemeManagement;
using ACAT.Core.Utility;
using ACAT.Extension.CommandHandlers;
using ACAT.Extensions.UI.UserControls;
using ACAT.Scanners;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ACAT.Extensions.UI.Scanners
{
    [ClassDescriptor("D3F4A1B2-3C4D-5E6F-7A8B-9A0B1C2D3E4F",
                Name = "DashboardAppScanner",
                Description = "Scanner for Dashboard Applications")]
    public partial class DashboardAppScanner : GenericScannerForm
    {
        private TableLayoutPanel ScannerBorder;
        private TableLayoutPanel panelTopToolbar;
        private TableLayoutPanel panelDashboardControls;

        public override DefaultCommandDispatcher _dispatcher => throw new NotImplementedException();

        public override RunCommandDispatcher CommandDispatcher => _dispatcher;

        public override ITextController TextController => ScannerCommon.TextController;

        public DashboardAppScanner() : base()
        {
            Resize += DashboardAppScanner_Resize;
        }

        private void DashboardAppScanner_Resize(object sender, EventArgs e)
        {
            Log.Debug("DashboardAppScanner_Resize called");
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
            this.ScannerBorder.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
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
            //this.Size = new Size(1600, 800);
            //this.MinimumSize = new Size(1600, 800);
            this.BackColor = Color.FromArgb(35, 36, 51);
            this.ForeColor = Color.White;
            this.Controls.Add(ScannerBorder);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.ShowInTaskbar = true;
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            this.panelTopToolbar.ResumeLayout();
            this.panelDashboardControls.ResumeLayout();
            this.ScannerBorder.ResumeLayout();
            this.ResumeLayout();
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

        public override bool HandleInitialize(StartupArg startup)
        {
            //bool success = true;
            bool success = ScannerCommon.UserControlManager.AddUserControlByKeyOrName(panelTopToolbar, "toolbar", "ToolbarUserControl");
            success = success && ScannerCommon.UserControlManager.AddUserControlByKeyOrName(panelDashboardControls, "dashboard", "DashboardUserControl");

            return success;
        }

        public override bool CheckCommandEnabled(CommandEnabledArg arg)
        {
            return false;
            //throw new NotImplementedException();
        }

        protected override void HandlePause()
        {
            //throw new NotImplementedException();
        }

        protected override void HandleResume()
        {
            //throw new NotImplementedException();
        }

        protected override void ScannerFormLoaded(object sender, EventArgs e)
        {
            panelTopToolbar.Focus();
            _scannerCommon.OnLoad();

            setColorScheme();

            _windowActiveWatchdog = new WindowActiveWatchdog(this);

        }

        protected override void ScannerShown(object sender, EventArgs e)
        {
            ScannerFocus.SetFocus(this);
        }
        protected override void subscribeToEvents()
        {
            Load += ScannerFormLoaded;
            Shown += ScannerShown;
            FormClosing += ScannerFormClosing;
        }

        protected override void updateControlsFromTheme(ColorScheme colorScheme)
        {
            //throw new NotImplementedException();
        }

        private class CommandHandler : RunCommandHandler
        {
            public CommandHandler(string cmd) : base(cmd) { }

        }

        private class DashboardAppDispatcher : DefaultCommandDispatcher
        {
            public DashboardAppDispatcher(IScannerPanel scanner) : base(scanner)
            {
                //TODO: Add Command Handlers.
            }
        }
    }
}
