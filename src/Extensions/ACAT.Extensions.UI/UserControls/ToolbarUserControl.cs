using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using ACAT.UserControls;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ACAT.Extensions.UI.UserControls
{
    [ClassDescriptor("4E1A5ED3-ED21-449B-B462-A8AE9F7BDC1F",
        Name = "ToolbarUserControl",
        Description = "User control for the toolbar in the ACAT Dashboard")]
    public class ToolbarUserControl : KeyboardUserControl
    {
        private TableLayoutPanel ButtonBoxRow;

        private System.ComponentModel.IContainer container = null;

        private TableLayoutPanel ToolbarBox;

        public ToolbarUserControl()
        {
            InitializeComponent();
        }

        protected virtual void CreateToolbarButtons()
        {
            var controlbuttons = new List<(string Name, string Text)>
            {
                ("Settings", "i"),
                ("Help", "F"),
                ("About", "!"),
                ("Home", "C")
            };
            // Create buttons with specific properties
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
                    Size = new Size(100, 100),
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
            // ToolbarUserControl
            // 
            this.Name = "ToolbarUserControl";
            this.AccessibleName = "ToolbarUserControl";
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.Padding = new Padding(10);
            this.Dock = DockStyle.Top;

            Label appName = new Label
            {
                Name = "ACAT",
                Text = "ACAT Dashboard",
                AutoSize = true,
                Padding = new Padding(10),
                TextAlign = ContentAlignment.MiddleCenter,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                ForeColor = Color.White,
                Font = new Font("Montserrat", 28, FontStyle.Regular, GraphicsUnit.Point, 0)
            };

            ToolbarBox = new TableLayoutPanel
            {
                Name = "ToolbarBox",
                AccessibleName = "ToolbarBox",
                Dock = DockStyle.Top,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(2),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
            };
            ToolbarBox.SuspendLayout();
            ToolbarBox.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            ToolbarBox.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            ToolbarBox.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            ButtonBoxRow = new TableLayoutPanel
            {
                Name = "ButtonBoxRow",
                AccessibleName = "ButtonBoxRow",
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Top | DockStyle.Right,
                ColumnCount = 1, // Start with no columns, will add dynamically
                RowCount = 1,
                GrowStyle = TableLayoutPanelGrowStyle.AddColumns,
            };

            ButtonBoxRow.SuspendLayout();
            ButtonBoxRow.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            ButtonBoxRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            CreateToolbarButtons();

            ToolbarBox.Controls.Add(appName, 0, 0);
            ToolbarBox.Controls.Add(ButtonBoxRow, 1, 0);

            this.Controls.Add(ToolbarBox);

            ButtonBoxRow.ResumeLayout();
            ToolbarBox.ResumeLayout();
            this.ResumeLayout();
        }
    }
}
