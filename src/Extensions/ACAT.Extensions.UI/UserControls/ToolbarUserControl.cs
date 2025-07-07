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
        private Font defaultFont = new Font("Montserrat", 28);
        private Font acatFont1Font = new Font("ACATFont1", 28);

        public ToolbarUserControl()
        {
            InitializeComponent();
        }

        public TableLayoutPanel tableLayout { get; private set; }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ToolbarUserControl
            // 
            this.Name = "ToolbarUserControl";
            this.AccessibleName = "ToolbarUserControl";
            this.Size = new System.Drawing.Size(800, 50);
            this.ResumeLayout(false);
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowOnly;

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

            var ToolbarButtons = new List<Control>
                {
                    new ScannerButtonControl { Name = "Settings", Text = "i", Font = acatFont1Font, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink},
                    new ScannerButtonControl { Name = "Help", Text = "F", Font = acatFont1Font,AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink },
                    new ScannerButtonControl { Name = "About", Text = "!", Font = defaultFont,AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink },
                    new ScannerButtonControl { Name = "Home", Text = "_", Font = defaultFont,AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink }
                    //new ScannerButtonControl { Name = "Minimize", Text = "_", Font = defaultFont,AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink },
                    //new ScannerButtonControl { Name = "CloseButton", Text = "X", Font = defaultFont, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink }
                };

            tableLayout = new TableLayoutPanel
            {
                Name = "ToolbarTableLayoutPanel",
                AccessibleName = "ToolbarTableLayoutPanel",
                Dock = DockStyle.Top,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(2)
            };
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var buttonPanel = new FlowLayoutPanel
            {
                Name = "ButtonPanel",
                AccessibleName = "ButtonPanel",
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Dock = DockStyle.Fill,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                WrapContents = false
            };


            // Add the buttons to the tableLayout
            foreach (var button in ToolbarButtons)
            {
                //button.Font = defaultFont;
                button.BackColor = Color.Transparent;
                button.ForeColor = Color.White;

                buttonPanel.Controls.Add(button);
            }

            tableLayout.Controls.Add(appName, 0, 0);
            tableLayout.Controls.Add(buttonPanel, 1, 0);

            this.Controls.Add(tableLayout);

            this.ResumeLayout();
        }
    }
}
