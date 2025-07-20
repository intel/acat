using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using ACAT.UserControls;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ACAT.Extensions.UI.UserControls
{
    [ClassDescriptor("3933BEAF-FAB3-4C6B-AB16-D0B07B0F2C6D",
        Name = "DashboardUserControl",
        Description = "User control for the ACAT Dashboard")]
    public class DashboardUserControl : KeyboardUserControl
    {
        private TableLayoutPanel ToolbarBox = new TableLayoutPanel
        {
            Name = "ToolbarBox",
            Dock = DockStyle.Fill,
            CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            BackColor = Color.Transparent,
            Padding = new Padding(10),
            Margin = new Padding(10),
            ColumnCount = 1,
            RowCount = 1,
            GrowStyle = TableLayoutPanelGrowStyle.AddColumns,
        };

        private TableLayoutPanel DefaultButtonsBox = new TableLayoutPanel
        {
            Name = "DefaultButtonsBox",
            AccessibleName = "DefaultButtonsBox",
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Margin = new Padding(10),
            Padding = new Padding(20),
            BackColor = Color.Transparent,
            Dock = DockStyle.Top,
            Anchor = AnchorStyles.Right | AnchorStyles.Top,
            GrowStyle = TableLayoutPanelGrowStyle.AddColumns,
            ColumnCount = 7,
            RowCount = 1,
            Visible = true
        };

        private System.ComponentModel.IContainer container = null;

        protected class ButtonSpec
        {
            public string Name { get; set; }
            public string Icon { get; set; }
            public bool Visible { get; set; }
        }

        protected virtual List<ButtonSpec> DefaultButtons { get; set; }

        public DashboardUserControl()
        {
            InitializeButtonsList();
            InitializeComponent();
        }

        protected virtual void InitializeButtonsList()
        {
            DefaultButtons = new List<ButtonSpec>
             {
                new() { Name = "ACATTalk", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("chat-fill"), Visible = true },
                new() { Name = "QuickTalk", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("chat"), Visible = true },
                new() { Name = "PointerControl", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("mouse2"), Visible = true },
                new() { Name = "Keyboard", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("keyboard"), Visible = true },
                new() { Name = "Windows", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("windows"), Visible = true },
                new() { Name = "Location", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("three-dots"), Visible = true },
                new() { Name = "MainMenu", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("arrows-move"), Visible = true },
             };
        }

        protected virtual void CreateToolbarButtons(TableLayoutPanel parent, List<ButtonSpec> buttons)
        {
            var defaultSize = new Size(200, 200);
            var padding = new Padding(10);

            // Create buttons with specific properties
            foreach (var (button, index) in buttons.Select((p, i) => (p, i)))
            {
                var scannerButton = new ScannerRoundedButtonControl
                {
                    BorderColor = Color.DimGray,
                    BorderRadiusBottomLeft = 12,
                    BorderRadiusBottomRight = 12,
                    BorderRadiusTopLeft = 12,
                    BorderRadiusTopRight = 12,
                    BorderWidth = 3F,
                    Dock = DockStyle.Fill,
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = Color.White,
                    Font = new Font("bootstrap-icons", 44, FontStyle.Regular, GraphicsUnit.Point, 0),
                    Margin = new Padding(10),
                    TabIndex = index,
                    Name = button.Name,
                    Text = button.Icon,
                    Visible = button.Visible,
                    UseMnemonic = false,
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    Size = defaultSize,
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                };

                parent.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                parent.Controls.Add(scannerButton, index, 1);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (container != null))
            {
                container.Dispose();
            }
            base.Dispose(disposing);
        }

        protected virtual void InitializeComponent()
        {
            this.SuspendLayout();
            this.Name = "DashboardUserControl";
            this.AccessibleName = "DashboardUserControl";
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.Padding = new Padding(10);
            this.Dock = DockStyle.Top;

            ToolbarBox.SuspendLayout();

            CreateToolbarButtons(DefaultButtonsBox, DefaultButtons);

            ToolbarBox.Controls.Add(DefaultButtonsBox, 0, 0);

            Controls.Add(ToolbarBox);

            DefaultButtonsBox.ResumeLayout(true);
            ToolbarBox.ResumeLayout(true);
            this.ResumeLayout(true);
        }
    }
}
