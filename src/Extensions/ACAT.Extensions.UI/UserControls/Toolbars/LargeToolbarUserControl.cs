using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using ACAT.UserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ACAT.Extensions.UI.UserControls
{
    [DesignerCategory("code")]
    public abstract class LargeToolbarUserControl : KeyboardUserControl
    {
        private TableLayoutPanel ToolbarBox = new TableLayoutPanel
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

        private TableLayoutPanel DefaultButtonsBox = new TableLayoutPanel
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
            RowCount = 1,
            Visible = true
        };

        private readonly System.ComponentModel.IContainer container = null;

        protected List<ButtonSpec> Buttons { get; set; }

        protected class ButtonSpec
        {
            public string Name { get; set; }
            public string Icon { get; set; }
            public bool Visible { get; set; }
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
            var defaultSize = new Size(100, 100);

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
                    Font = new Font("bootstrap-icons", 44, FontStyle.Regular, GraphicsUnit.Point, 0),
                    TabIndex = index,
                    Name = button.Name,
                    Text = button.Icon,
                    Visible = button.Visible,
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

        protected virtual void InitializeComponent()
        {
            this.SuspendLayout();
            this.AccessibleName = this.Name;
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.Dock = DockStyle.Top;

            ToolbarBox.SuspendLayout();

            CreateToolbarButtons(DefaultButtonsBox);

            ToolbarBox.Controls.Add(DefaultButtonsBox, 0, 0);

            Controls.Add(ToolbarBox);

            DefaultButtonsBox.ResumeLayout(true);
            ToolbarBox.ResumeLayout(true);
            this.ResumeLayout(true);
        }

        // This method should be overridden in derived classes to handle button clicks
        // For example, it can be used to trigger specific actions based on the button clicked
        // Example: if (buttonSpec.Name == "MoveAndClick") { /* Handle MoveAndClick action */ }
        public abstract void OnButtonClicked(object s, EventArgs e);
    }
}
