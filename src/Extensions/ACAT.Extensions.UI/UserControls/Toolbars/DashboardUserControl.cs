using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using ACAT.UserControls;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ACAT.Extensions.UI.UserControls
{
    [ClassDescriptor("3933BEAF-FAB3-4C6B-AB16-D0B07B0F2C6D",
        Name = "DashboardUserControl",
        Description = "User control for the ACAT Dashboard")]
    public class DashboardUserControl : LargeToolbarUserControl
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


        public DashboardUserControl()
        {
            InitializeButtonsList();
            InitializeComponent();
        }

        protected override void InitializeButtonsList()
        {
            DefaultButtons = new List<ButtonSpec>
             {
                new() { Name = "ACATTalk", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("chat-fill"), Visible = true },
                new() { Name = "QuickTalk", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("chat"), Visible = true },
                new() { Name = "PointerControl", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("mouse2"), Visible = true },
                new() { Name = "Keyboard", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("keyboard"), Visible = true },
                new() { Name = "Windows", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("windows"), Visible = true },
                new() { Name = "Location", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("three-dots"), Visible = true },
                new() { Name = "MainMenu", Icon = BootstrapFontUtility.GetBootstrapFontCharacter("arrows-move"), Visible = true },
             };
        }


    }
}
