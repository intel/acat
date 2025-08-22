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

        // private TableLayoutPanel PanelSettingsBox = new TableLayoutPanel
        // {
        //     Name = "PanelSettingsBox",
        //     AccessibleName = "PanelSettingsBox",
        //     AutoSize = true,
        //     AutoSizeMode = AutoSizeMode.GrowAndShrink,
        //     Dock = DockStyle.Top | DockStyle.Right,
        //     GrowStyle = TableLayoutPanelGrowStyle.AddColumns,
        //     CellBorderStyle = TableLayoutPanelCellBorderStyle.Single
        // };

        readonly Label appName = new()
        {
            Name = "ACAT",
            Text = "ACAT Dashboard",
            AutoSize = true,
            TextAlign = ContentAlignment.MiddleCenter,
            Anchor = AnchorStyles.Top | AnchorStyles.Left,
            Dock = DockStyle.Left,
            ForeColor = Color.White,
            //Font = new Font("Montserrat", 28, FontStyle.Regular, GraphicsUnit.Point, 0)
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
                { "Settings", BootstrapFontUtility.GetBootstrapFontCharacter("gear-wide-connected") },
                { "Help", BootstrapFontUtility.GetBootstrapFontCharacter("life-preserver") },
                { "About", BootstrapFontUtility.GetBootstrapFontCharacter("question-circle") },
                { "Home", BootstrapFontUtility.GetBootstrapFontCharacter("house-door") },
                { "Exit", BootstrapFontUtility.GetBootstrapFontCharacter("door-closed") }
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
            float scaleFactor = this.DeviceDpi / 96f;
            var defaultSize = new Size((int)(80 * scaleFactor), (int)(80 * scaleFactor));

            // Create buttons with specific properties
            foreach (var (button, index) in buttons.Select((p, i) => (p, i)))
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

            ToolbarBox.Controls.Add(appName, 0, 0);
            appName.Font = FontUtil.ScaleFontToHeight(DefaultButtonsBox.Height, "Montserrat Thin");
            ToolbarBox.Controls.Add(DefaultButtonsBox, 1, 0);

            Controls.Add(ToolbarBox);

            DefaultButtonsBox.ResumeLayout(true);
            ToolbarBox.ResumeLayout(true);
            ResumeLayout(true);
        }

        // public void HandlePanelSettingsClicked()
        // {
        //     DefaultButtonsBox.Visible = false;
        //     PanelSettingsBox.Visible = true;
        // }

        // internal void HandleCommand(string cmd)
        // {
        //     switch (cmd)
        //     {
        //         case "Shrink":
        //             Shrink();
        //             break;
        //         case "Grow":
        //             Grow();
        //             break;
        //         case "Fade":
        //             Fade();
        //             break;
        //         case "Unfade":
        //             UnFade();
        //             break;
        //         case "Home2":
        //             HandlePanelSettingsClicked();
        //             break;
        //         default:
        //             // Handle other commands or do nothing
        //             break;
        //     }
        // }

        // private bool _transparencyIncreasing = false;

        // private void ToggleTransparency()
        // {
        //     const double MinOpacity = 0.5;
        //     const double MaxOpacity = 1.0;
        //     const double Step = 0.1;

        //     using var parent = this.TopLevelControl as Form;

        //     if (_transparencyIncreasing)
        //     {
        //         parent.Opacity = Math.Min(MaxOpacity, parent.Opacity + Step);
        //         if (parent.Opacity >= MaxOpacity)
        //             _transparencyIncreasing = false;
        //     }
        //     else
        //     {
        //         parent.Opacity = Math.Max(MinOpacity, parent.Opacity - Step);
        //         if (parent.Opacity <= MinOpacity)
        //             _transparencyIncreasing = true;
        //     }
        // }

        // private void Fade()
        // {
        //     ToggleTransparency();
        // }

        // private void UnFade()
        // {
        //     ToggleTransparency();
        // }

        // private void Grow()
        // {

        // }

        // private void Shrink()
        // {

        // }
    }
}
