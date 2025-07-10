using ACAT.Core.WidgetManagement;
using ACAT.UserControls;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ACAT.Core.Utility;

namespace ACAT.Extensions.UI.UserControls
{
    [ClassDescriptor("3933BEAF-FAB3-4C6B-AB16-D0B07B0F2C6D",
        Name = "DashboardUserControl",
        Description = "User control for the ACAT Dashboard")]
    public class DashboardUserControl : GenericUserControl
    {
        public DashboardUserControl()
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
            // DashboardUserControl
            // 
            this.Name = "DashboardUserControl";
            this.AccessibleName = "DashboardUserControl";
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.Padding = new Padding(10);


            List<ScannerButtonControl> controls = CreateMenuItems();

            tableLayout = new TableLayoutPanel
            {
                Name = "ToolbarTableLayoutPanel",
                AccessibleName = "ToolbarTableLayoutPanel",
                Dock = DockStyle.Top,
                ColumnCount = 1,
                RowCount = 1,
                Padding = new Padding(2),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            }; 
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayout.SuspendLayout();

            var buttonPanel = new FlowLayoutPanel
            {
                Name = "ButtonPanel",
                AccessibleName = "ButtonPanel",
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Dock = DockStyle.Fill,
                Anchor = AnchorStyles.Top,
                WrapContents = false
            };
            buttonPanel.SuspendLayout();

            foreach (ScannerButtonControl button in controls)
            {
                buttonPanel.Controls.Add(button);
            }

            tableLayout.Controls.Add(buttonPanel, 0, 0);
            this.Controls.Add(tableLayout);

            buttonPanel.ResumeLayout();
            tableLayout.ResumeLayout();
            this.ResumeLayout();
        }

        private List<ScannerButtonControl> CreateMenuItems()
        {
            var iconFont = new Font("ACAT Icon", 44);
            var font1Font = new Font("ACAT FONT 1", 44);

            var MainMenuButtons = new List<ScannerButtonControl>
                    {
                        new ScannerButtonControl { Name = "ACATTalk", Text = "H", Font = iconFont},
                        new ScannerButtonControl { Name = "QuickTalk",Text = "I", Font = iconFont },
                        new ScannerButtonControl { Name = "PointerControl", Text = "Q", Font = iconFont },
                        new ScannerButtonControl { Name = "Keyboard", Text = "e", Font = font1Font},
                        new ScannerButtonControl { Name = "System", Text = "H", Font = font1Font },
                        new ScannerButtonControl { Name = "Location", Text = "G", Font = font1Font },
                    };

            foreach (var button in MainMenuButtons)
            {
                button.BackColor = Color.FromArgb(36, 36, 51);
                button.ForeColor = Color.FromArgb(255, 170, 0);
                button.Size = new Size(200, 200);
                button.Padding = new Padding(4);
            }

            return MainMenuButtons;
        }
    }
}
