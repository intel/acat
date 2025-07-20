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
using System;

namespace ACAT.Extensions.UI.UserControls
{
    [ClassDescriptor("4E1A5ED3-ED21-449B-B462-A8AE9F7BDC1F",
        Name = "ToolbarUserControl",
        Description = "User control for the toolbar in the ACAT Dashboard")]
    public class ToolbarUserControl : KeyboardUserControl
    {
        private TableLayoutPanel ToolbarBox = new TableLayoutPanel
        {
            Name = "ToolbarBox",
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            BackColor = Color.Transparent,
            Padding = new Padding(10),
            Margin = new Padding(10),
            ColumnCount = 2,
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

        Label appName = new Label
        {
            Name = "ACAT",
            Text = "ACAT Dashboard",
            AutoSize = true,
            Padding = new Padding(10),
            TextAlign = ContentAlignment.MiddleCenter,
            Anchor = AnchorStyles.Top | AnchorStyles.Left,
            Dock = DockStyle.Left,
            ForeColor = Color.White,
            Font = new Font("Montserrat", 28, FontStyle.Regular, GraphicsUnit.Point, 0)
        };

        private System.ComponentModel.IContainer container = null;

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
                //{ "PanelSettings", BootstrapFontUtility.GetBootstrapFontCharacter("three-dots") },
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
            var defaultSize = new Size(100, 100);
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
                    Font = new Font("bootstrap-icons", 24, FontStyle.Regular, GraphicsUnit.Point, 0),
                    Margin = new Padding(10),
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
            this.Name = "ToolbarUserControl";
            this.AccessibleName = "ToolbarUserControl";
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.Padding = new Padding(10);
            this.Dock = DockStyle.Top;

            ToolbarBox.SuspendLayout();

            CreateToolbarButtons(DefaultButtonsBox, DefaultButtons);

            ToolbarBox.Controls.Add(appName, 0, 0);
            ToolbarBox.Controls.Add(DefaultButtonsBox, 1, 0);

            this.Controls.Add(ToolbarBox);

            DefaultButtonsBox.ResumeLayout(true);
            ToolbarBox.ResumeLayout(true);
            this.ResumeLayout(true);
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
