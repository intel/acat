using ACAT.Core.WidgetManagement;
using ACAT.UserControls;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ACAT.Core.Utility;

namespace ACAT.Extensions.UI.UserControls
{
    [ClassDescriptor("4E1A5ED3-ED21-449B-B462-A8AE9F7BDC1F",
        Name = "ToolbarUserControl",
        Description = "User control for the toolbar in the ACAT Dashboard")]
    public class ToolbarUserControl : GenericUserControl
    {
        private Font acatFont1Font = new Font("ACAT Font 1", 28);
        private Font defaultFont = new Font("Montserrat", 28);
        public ToolbarUserControl()
        {
            InitializeComponent();
        }

        public TableLayoutPanel tableLayout { get; private set; }

        protected override bool HandleInitialize()
        {
            return true;
        }


        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ToolbarUserControl
            // 
            this.Name = "ToolbarUserControl";
            this.AccessibleName = "ToolbarUserControl";
            this.Size = new System.Drawing.Size(800, 120);
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.Padding = new Padding(10);

            Label appName = new Label
            {
                Name = "ACAT",
                Text = "ACAT Dashboard",
                Font = defaultFont,
                AutoSize = true,
                Padding = new Padding(10),
                TextAlign = ContentAlignment.MiddleCenter,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                ForeColor = Color.White
            };

            List<Control> ToolbarButtons = CreateToolbarButtons();

            tableLayout = new TableLayoutPanel
            {
                Name = "ToolbarTableLayoutPanel",
                AccessibleName = "ToolbarTableLayoutPanel",
                Dock = DockStyle.Top,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(2),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };
            tableLayout.SuspendLayout();
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var buttonPanel = new TableLayoutPanel
            {
                Name = "ButtonPanel",
                AccessibleName = "ButtonPanel",
                Size = new Size(800, 60),
                //AutoSize = true,
                Dock = DockStyle.Fill,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                ColumnCount = 0, // Start with no columns, will add dynamically
                RowCount = 1,
                GrowStyle = TableLayoutPanelGrowStyle.AddColumns
            };
        
            buttonPanel.SuspendLayout();
            buttonPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            var buttons = CreateToolbarButtons();
            foreach (var button in buttons)
            {
                buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                buttonPanel.Controls.Add(button);
            }

            tableLayout.Controls.Add(appName, 0, 0);
            tableLayout.Controls.Add(buttonPanel, 1, 0);

            this.Controls.Add(tableLayout);

            buttonPanel.ResumeLayout();
            tableLayout.ResumeLayout();
            this.ResumeLayout();
        }

        private List<Control> CreateToolbarButtons()
        {
            // Correcting the array initialization issue by using a List of tuples instead of an invalid array initializer
            var buttons = new List<(string Name, string Text)>
            {
                ("Settings", "i"),
                ("Help", "F"),
                ("About", "!"),
                ("Home", "C")
            };

            var ToolbarButtons = new List<Control>();

            // Create buttons with specific properties
            foreach (var button in buttons)
            {
                var scannerButton = new ScannerRoundedButtonControl
                {
                    BorderColor = System.Drawing.Color.DimGray,
                    BorderRadiusBottomLeft = 12,
                    BorderRadiusBottomRight = 12,
                    BorderRadiusTopLeft = 12,
                    BorderRadiusTopRight = 12,
                    BorderWidth = 3F,
                    Dock = System.Windows.Forms.DockStyle.Fill,
                    FlatStyle = System.Windows.Forms.FlatStyle.Flat,
                    ForeColor = System.Drawing.Color.White,
                    Name = button.Name,
                    Size = new System.Drawing.Size(60, 60),
                    TabIndex = 0,
                    Text = button.Text,
                    UseMnemonic = false,
                    UseVisualStyleBackColor = true,
                };
                ToolbarButtons.Add(scannerButton);
            }

            return ToolbarButtons;
        }
    }
}
