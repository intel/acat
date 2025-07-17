using ACAT.Core.AgentManagement;
using ACAT.Core.UserControlManagement;
using ACAT.Core.PanelManagement;
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
        private const string defaultButtonsName = "DefaultButtons";
        protected Dictionary<string, string> _defaultButtons = new Dictionary<string, string>
        {
            { "Settings", BootstrapFontUtility.GetBootstrapFontCharacter("gear-wide-connected") },
            { "Help", BootstrapFontUtility.GetBootstrapFontCharacter("life-preserver") },
            { "About", BootstrapFontUtility.GetBootstrapFontCharacter("question-circle") },
            { "PanelSettings", BootstrapFontUtility.GetBootstrapFontCharacter("three-dots") },
            { "Home", BootstrapFontUtility.GetBootstrapFontCharacter("house-door") }
        };

        private const string _panelSettingsButtonsName = "panelSettingsButtons";
        protected Dictionary<string, string> _panelSettingsButtons = new Dictionary<string, string>
        {
            { "Shrink", BootstrapFontUtility.GetBootstrapFontCharacter("arrows-angle-contract") },
            { "Grow", BootstrapFontUtility.GetBootstrapFontCharacter("arrows-angle-expand") },
            { "Fade", BootstrapFontUtility.GetBootstrapFontCharacter("circle") },
            { "Unfade", BootstrapFontUtility.GetBootstrapFontCharacter("circle-half") },
            { "Home", BootstrapFontUtility.GetBootstrapFontCharacter("house-door") }
        };

        private TableLayoutPanel ToolbarBox = new TableLayoutPanel
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

        private TableLayoutPanel DefaultButtonsBox = new TableLayoutPanel
        {
            Name = "DefaultButtonsBox",
            AccessibleName = "DefaultButtonsBox",
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Dock = DockStyle.Top | DockStyle.Right,
            ColumnCount = 1, // Start with no columns, will add dynamically
            RowCount = 1,
            GrowStyle = TableLayoutPanelGrowStyle.AddColumns,
        };
        
        private TableLayoutPanel PanelSettingsBox = new TableLayoutPanel
        {
            Name = "PanelSettingsBox",
            AccessibleName = "PanelSettingsBox",
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Dock = DockStyle.Top | DockStyle.Right,
            ColumnCount = 1, // Start with no columns, will add dynamically
            RowCount = 1,
            GrowStyle = TableLayoutPanelGrowStyle.AddColumns,
        };

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

        private System.ComponentModel.IContainer container = null;


        public ToolbarUserControl()
        {
            InitializeComponent();
        }

        protected virtual void CreateToolbarButtons(TableLayoutPanel parent, Dictionary<string, string> buttons)
        {
            // Create buttons with specific properties
            foreach (var (button, index) in buttons.Select((p, i) => (p, i)))
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
                    Font = new Font("bootstrap-icons", 24, FontStyle.Regular, GraphicsUnit.Point, 0),
                    Name = button.Key,
                    Margin = new Padding(10),
                    TabIndex = index,
                    Text = button.Value,
                    UseMnemonic = false,
                    //UseVisualStyleBackColor = true,
                    Anchor = AnchorStyles.Top,
                    Size = new Size(100, 100),
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                };


                parent.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                parent.Controls.Add(scannerButton, index + 1, 0);
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

        protected void InitializeComponent()
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

            ToolbarBox.SuspendLayout();
            ToolbarBox.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            ToolbarBox.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            ToolbarBox.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            DefaultButtonsBox.SuspendLayout();
            PanelSettingsBox.SuspendLayout();
            CreateToolbarButtons(DefaultButtonsBox, _defaultButtons);
            CreateToolbarButtons(PanelSettingsBox, _panelSettingsButtons);

            PanelSettingsBox.Visible = false;
            DefaultButtonsBox.Visible = true;

            ToolbarBox.Controls.Add(appName, 0, 0);
            ToolbarBox.Controls.Add(PanelSettingsBox, 1, 0);
            ToolbarBox.Controls.Add(DefaultButtonsBox, 1, 0);

            this.Controls.Add(ToolbarBox);

            DefaultButtonsBox.ResumeLayout();
            PanelSettingsBox.ResumeLayout();
            ToolbarBox.ResumeLayout();
            this.ResumeLayout();
        }
    }
}
