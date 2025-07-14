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
        private TableLayoutPanel ButtonBoxRow;

        private System.ComponentModel.IContainer container = null;

        private TableLayoutPanel ToolbarBox;

        public DashboardUserControl()
        {
            InitializeComponent();
        }

        private void CreateToolbarButtons()
        {
            var controlbuttons = new List<(string Name, string Text)>
            {
                ("ACATTalk", "H"),
                ("QuickTalk", "I"),
                ("PointerControl", "Q"),
                ("Keyboard", "e"),
                ("System", "H"),
                ("Location", "G")
            };

            foreach (var (button, index) in controlbuttons.Select((p, i) => (p, i)))
            {
                var scannerButton = new ScannerRoundedButtonControl
                {
                    BorderColor = System.Drawing.Color.DimGray,
                    BorderRadiusBottomLeft = 12,
                    BorderRadiusBottomRight = 12,
                    BorderRadiusTopLeft = 12,
                    BorderRadiusTopRight = 12,
                    BorderWidth = 3F,
                    Dock = DockStyle.Fill,
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = Color.White,
                    Name = button.Name,
                    Margin = new Padding(10),
                    TabIndex = index,
                    Text = button.Text,
                    UseMnemonic = false,
                    //UseVisualStyleBackColor = true,
                    Anchor = AnchorStyles.Top,
                    Size = new Size(220, 220),
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                };


                ButtonBoxRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                ButtonBoxRow.Controls.Add(scannerButton, index + 1, 0);
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

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // DashboardUserControl
            // 
            this.Name = "DashboardUserControl";
            this.AccessibleName = "DashboardUserControl";
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.Padding = new Padding(10);
            this.Dock = DockStyle.Top;

            ToolbarBox = new TableLayoutPanel
            {
                Name = "ToolbarBox",
                AccessibleName = "ToolbarBox",
                Dock = DockStyle.Top,
                ColumnCount = 1,
                RowCount = 1,
                Padding = new Padding(2),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
            };
            ToolbarBox.SuspendLayout();
            ToolbarBox.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            ToolbarBox.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            ButtonBoxRow = new TableLayoutPanel
            {
                Name = "ButtonBoxRow",
                AccessibleName = "ButtonBoxRow",
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Top,
                ColumnCount = 1, // Start with no columns, will add dynamically
                RowCount = 1,
                GrowStyle = TableLayoutPanelGrowStyle.AddColumns,
            };

            ButtonBoxRow.SuspendLayout();
            ButtonBoxRow.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            ButtonBoxRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 0F));
            CreateToolbarButtons();

            ToolbarBox.Controls.Add(ButtonBoxRow, 0, 0);

            Controls.Add(ToolbarBox);

            ButtonBoxRow.ResumeLayout();
            ToolbarBox.ResumeLayout();
            this.ResumeLayout();
        }
    }
}
