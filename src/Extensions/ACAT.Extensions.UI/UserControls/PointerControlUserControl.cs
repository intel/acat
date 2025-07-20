using ACAT.Core.AgentManagement;
using ACAT.Core.PanelManagement;
using ACAT.Core.PanelManagement.CommandDispatcher;
using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using ACAT.Extensions.UI;
using ACAT.UserControls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ACAT.Extensions.UI.UserControls
{
    [ClassDescriptor("72061314-CE5E-4DB8-92C8-C0F81E5CB3EE",
        Name = "PointerControlUserControl",
        Description = "Pointer control User control for the ACAT Dashboard")]
    public class PointerControlUserControl : DashboardUserControl
    {
        private TableLayoutPanel ToolbarBox = new TableLayoutPanel
        {
            Name = "ToolbarBox",
            Dock = DockStyle.Fill,
            CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            BackColor = Color.Transparent,
            Padding = new Padding(10),
            Margin = new Padding(10),
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
            Margin = new Padding(10),
            Padding = new Padding(20),
            BackColor = Color.Transparent,
            Dock = DockStyle.Top,
            Anchor = AnchorStyles.Right | AnchorStyles.Top,
            GrowStyle = TableLayoutPanelGrowStyle.AddColumns,
            ColumnCount = 7,
            RowCount = 1,
            Visible = true
        };

        private System.ComponentModel.IContainer container = null;

        protected override List<ButtonSpec> DefaultButtons { get; set; }

        protected override void InitializeButtonsList()
        {
            DefaultButtons = new List<ButtonSpec>
             {
                new() { Name = "MoveAndClick", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("reply-all"), Visible = true},
                new() { Name = "Move", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("reply"), Visible = true},
                new() { Name = "LeftClick", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("mouse2"), Visible = true},
                new() { Name = "RightClick", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("menu-app-fill"), Visible = true},
                new() { Name = "ClickHold", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("mouse2-fill"), Visible = true},
                new() { Name = "ScrollUp", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("arrow-up-square"), Visible = true},
                new() { Name = "ScrollDown", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("arrow-down-square"), Visible = true},
            };
        }

        protected override void InitializeComponent()
        {
            base.InitializeComponent();
            this.Name = "PointerControlUserControl";
            this.AccessibleName = "PointerControlUserControl";
        }
    }
}
