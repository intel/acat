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
    [DesignerCategory("code")]
    public abstract class LargeToolbarUserControl : KeyboardUserControl
    {
        private readonly TableLayoutPanel ToolbarBox = new()
        {
            Name = "ToolbarBox",
            AccessibleName = "ToolbarBox",
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            BackColor = Color.Transparent,
            ColumnCount = 1,
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
            ColumnCount = 7,
            RowCount = 2,
            RowStyles = { new RowStyle(SizeType.AutoSize), new RowStyle(SizeType.AutoSize) },
            Visible = true
        };

        private readonly IContainer container = null;

        protected List<ButtonSpec> Buttons { get; set; }

        protected class ButtonSpec
        {
            public string Name { get; set; }
            public string Icon { get; set; }
            public bool Visible { get; set; }
            public string LabelText { get; set; }
        }
        public LargeToolbarUserControl(string name)
        {
            Name = name;
            InitializeButtonsList();
            InitializeComponent();
        }

        protected abstract void InitializeButtonsList();

        protected virtual void CreateToolbarButtons(TableLayoutPanel parent)
        {
            float scaleFactor = this.DeviceDpi / 96f;
            var defaultSize = new Size((int)(200 * scaleFactor), (int)(200 * scaleFactor));

            // Create buttons with specific properties
            foreach (var (button, index) in Buttons.Select((p, i) => (p, i)))
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
                    TabIndex = index,
                    Name = button.Name,
                    Text = button.Icon,
                    Visible = button.Visible,
                    UseMnemonic = false,
                    Anchor = AnchorStyles.Top,
                    Size = defaultSize,
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Margin = new Padding(4, 4, 4, 4)
                };

                parent.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                parent.Controls.Add(scannerButton, index, 0);
                scannerButton.Font = FontUtil.ScaleFontToHeight(scannerButton.Height, "bootstrap-icons", scale: 0.5f);
                scannerButton.Resize += (s, e) =>
                {
                    Button button = (Button)s;
                    button.Font = FontUtil.ScaleFontToHeight(button.Height, "bootstrap-icons", scale: 0.5f);
                };

                if (button.LabelText != null && button.LabelText.Length > 0)
                {
                    var label = new Label
                    {
                        Text = button.LabelText,
                        ForeColor = Color.White,
                        BackColor = Color.Transparent,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Dock = DockStyle.Fill,
                        AutoSize = true,
                        Anchor = AnchorStyles.Top,
                    };
                    label.Font = FontUtil.ScaleFontToHeight((int)(scannerButton.Width * .5f), "Montserrat Thin", FontStyle.Regular, scale: 0.45f);
                    parent.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    parent.Controls.Add(label, index, 1);
                }

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

        protected virtual void InitializeComponent()
        {
            SuspendLayout();
            AccessibleName = Name;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Dock = DockStyle.Top;

            ToolbarBox.SuspendLayout();

            CreateToolbarButtons(DefaultButtonsBox);

            ToolbarBox.Controls.Add(DefaultButtonsBox, 0, 0);

            Controls.Add(ToolbarBox);

            DefaultButtonsBox.ResumeLayout(true);
            ToolbarBox.ResumeLayout(true);
            ResumeLayout(true);
        }

        // This method should be overridden in derived classes to handle button clicks
        // For example, it can be used to trigger specific actions based on the button clicked
        // Example: if (buttonSpec.Name == "MoveAndClick") { /* Handle MoveAndClick action */ }
        public abstract void OnButtonClicked(object s, EventArgs e);
    }
}
