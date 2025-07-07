using ACAT.Core.PanelManagement;
using ACAT.Core.Utility;
using ACAT.Extension;
using ACAT.Extensions.UI.UserControls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACAT.Extensions.UI.Scanners
{
    [ClassDescriptor("D3F4A1B2-3C4D-5E6F-7A8B-9A0B1C2D3E4F",
                Name = "DashboardAppScanner",
                Description = "Scanner for Dashboard Applications")]
    public partial class DashboardAppScanner : UserControlContainerForm
    {
        //private TableLayoutPanel mainPanel = new TableLayoutPanel
        //{
        //    AccessibleName = "MainPanel",
        //    Name = "MainPanel",
        //    Dock = DockStyle.Fill,
        //    AutoSize = true,
        //    GrowStyle = TableLayoutPanelGrowStyle.AddRows
        //};

        //private Panel panelToolbar = new Panel
        //{
        //    AccessibleName="ToolbarPanel",
        //    BackColor = Color.Transparent,
        //    Dock = DockStyle.Top,
        //    Padding = new Padding(5),
        //    Margin = new Padding(10),
        //    Size = new Size(1600, 100)
        //    //AutoSize = true,
        //    //AutoSizeMode = AutoSizeMode.GrowOnly
        //};

        //private Panel panelMainMenu = new Panel
        //{
        //    AccessibleName = "MainMenuPanel",
        //    BackColor = Color.Blue,
        //    Dock = DockStyle.Top,
        //    Padding = new Padding(5),
        //    Margin = new Padding(10),
        //    AutoSize = true,
        //    AutoSizeMode = AutoSizeMode.GrowOnly
        //};

        protected override void InitializeComponent()
        {
            base.InitializeComponent();

            Text = "ACAT Dashboard";
            Size = new Size(1600, 1200);
            BackColor = Color.FromArgb(35, 36, 51);
            ForeColor = Color.White;

            mainPanel.Controls.Add(new ToolbarUserControl());

            //mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            //mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            //mainPanel.Controls.Add(panelToolbar, 0, 0);
            //mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            //mainPanel.Controls.Add(panelMainMenu, 0, 1);

            //Controls.Add(mainPanel);

        }

        public override bool Initialize(StartupArg startup)
        {
            return base.Initialize(startup);
            //bool success = false;
            //if (base.Initialize(startup)) 
            //{
            //    //success = ScannerCommon.UserControlManager.AddUserControlByKeyOrName(panelToolbar, "ToolbarUserControl", "ToolbarUserControl");
            //    //success = success && ScannerCommon.UserControlManager.AddUserControlByKeyOrName(panelMainMenu, "", "");
            //}
        }
    }
}
