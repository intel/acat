using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using ACAT.Extension.UI.UserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ACAT.Extensions.UI.UserControls.Toolbars
{
    [ClassDescriptor("4E1A5ED3-ED21-449B-B462-A8AE9F7BDC1F",
        Name = "ToolbarUserControl",
        Description = "User control for the toolbar in the ACAT Dashboard")]
    [DesignerCategory("code")]
    public class ToolbarUserControl : KeyboardUserControl
    {
        private readonly TableLayoutPanel ToolbarBox = new()
        {
            Name = "ToolbarBox",
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            BackColor = Color.Transparent,
            ColumnCount = 2,
            RowCount = 1,
            GrowStyle = TableLayoutPanelGrowStyle.AddColumns,
        };

        private readonly TableLayoutPanel DefaultButtonsBox = new()
        {
            Name = "DefaultButtonsBox",
            AccessibleName = "DefaultButtonsBox",
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            BackColor = Color.Transparent,
            Dock = DockStyle.Top,
            Anchor = AnchorStyles.Right | AnchorStyles.Top,
            GrowStyle = TableLayoutPanelGrowStyle.AddColumns,
            ColumnCount = 5,
            RowCount = 1,
            Visible = true,
        };

        readonly Label appName = new()
        {
            Name = "ACAT Dashboard",
            Text = "ACAT Dashboard",
            AutoSize = true,
            TextAlign = ContentAlignment.MiddleLeft,
            Anchor = AnchorStyles.Top | AnchorStyles.Left,
            Dock = DockStyle.Left,
            ForeColor = Color.White,
        };

        private readonly IContainer container = null;

        protected Dictionary<string, string> DefaultButtons { get; private set; }
        //protected Dictionary<string, string> PanelSettingsButtons { get; private set; }

        public ToolbarUserControl()
        {
            InitializeButtonsList();
            InitializeComponent();
        }

        protected virtual void InitializeButtonsList()
        {
            DefaultButtons = new Dictionary<string, string>()
            {
                //{ "Settings", BootstrapFontUtility.GetBootstrapFontCharacter("gear-wide-connected") },
                //{ "Help", BootstrapFontUtility.GetBootstrapFontCharacter("life-preserver") },
                //{ "About", BootstrapFontUtility.GetBootstrapFontCharacter("question-circle") },
                //{ "Home", BootstrapFontUtility.GetBootstrapFontCharacter("house-door") },
                { "Menu", BootstrapFontUtility.GetBootstrapFontCharacter("list") }
            };
        //PanelSettingsButtons = new Dictionary<string, string>
        // {
        //     { "Shrink", BootstrapFontUtility.GetBootstrapFontCharacter("arrows-angle-contract") },
        //     { "Grow", BootstrapFontUtility.GetBootstrapFontCharacter("arrows-angle-expand") },
        //     { "Fade", BootstrapFontUtility.GetBootstrapFontCharacter("circle") },
        //     { "Unfade", BootstrapFontUtility.GetBootstrapFontCharacter("circle-half") },
        //     { "Home2", BootstrapFontUtility.GetBootstrapFontCharacter("house-door") }
        // };
        }

        protected virtual void CreateToolbarButtons(TableLayoutPanel parent, Dictionary<string, string> buttons)
        {
            float scaleFactor = 1; // this.DeviceDpi / 96f;
            var defaultSize = new Size((int)(40 * scaleFactor), (int)(40 * scaleFactor));

            // Create buttons with specific properties
            foreach ((KeyValuePair<string, string> button, int index) in buttons.Select((p, i) => (p, i)))
            {
                var scannerButton = new ScannerRoundedButtonControl
                {
                    BorderColor = Color.DimGray,
                    BorderRadiusBottomLeft = 4,
                    BorderRadiusBottomRight = 4,
                    BorderRadiusTopLeft = 4,
                    BorderRadiusTopRight = 4,
                    BorderWidth = 3F,
                    Dock = DockStyle.Fill,
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = Color.White,
                    //Font = new Font("bootstrap-icons", 18, FontStyle.Regular, GraphicsUnit.Point, 0),
                    TabIndex = index,
                    Name = button.Key,
                    Text = button.Value,
                    UseMnemonic = false,
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    Size = defaultSize,
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                };

                parent.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                parent.Controls.Add(scannerButton, index, 1);
                scannerButton.Font = FontUtil.ScaleFontToHeight(scannerButton.Height, "bootstrap-icons", scale: 0.5f);
                scannerButton.Resize += (s, e) =>
                {
                    Button button = (Button)s;
                    button.Font= FontUtil.ScaleFontToHeight(button.Height, "bootstrap-icons", scale: 0.5f);
                };
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && container != null)
            {
                container.Dispose();
            }
            base.Dispose(disposing);
        }

        protected void InitializeComponent()
        {
            SuspendLayout();
            Name = "ToolbarUserControl";
            AccessibleName = "ToolbarUserControl";
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Dock = DockStyle.Top;

            ToolbarBox.SuspendLayout();

            CreateToolbarButtons(DefaultButtonsBox, DefaultButtons);

            ToolbarBox.Controls.Add(DefaultButtonsBox, 1, 0);
            appName.Font = FontUtil.ScaleFontToHeight(DefaultButtonsBox.Height, "Montserrat Thin");
            appName.Resize += (s, e) =>
            {
                appName.Font = FontUtil.ScaleFontToHeight(DefaultButtonsBox.Height, "Montserrat Thin");
            };
            ToolbarBox.Controls.Add(appName, 0, 0);

            Controls.Add(ToolbarBox);

            DefaultButtonsBox.ResumeLayout(true);
            ToolbarBox.ResumeLayout(true);
            ResumeLayout(true);
        }
    }
}
