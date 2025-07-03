using ACAT.Core.PanelManagement;
using ACAT.Core.Utility;
using ACAT.Extension;
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
        private TableLayoutPanel mainPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            GrowStyle = TableLayoutPanelGrowStyle.AddRows
        };

        private Panel panelToolbar = new Panel
        {
            BackColor = Color.Red,
            Size = new Size(800, 40),
            Dock = DockStyle.Top,
            Padding = new Padding(5),
            Margin = new Padding(10)
        };
        private Panel panelMainMenu = new Panel
        {
            BackColor = Color.Blue,
            Size = new Size(800, 80),
            Dock = DockStyle.Top,
            Padding = new Padding(5),
            Margin = new Padding(10)
        };

        protected override void InitializeComponent()
        {
            // Initialize components specific to the DashboardAppScanner
            // This could include setting up controls, event handlers, etc.
            // For example:
            Text = "ACAT Dashboard";
            Size = new System.Drawing.Size(800, 120);
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;


            mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainPanel.Controls.Add(panelToolbar, 0, 0);
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainPanel.Controls.Add(panelMainMenu, 0, 1);
        }

        public override bool Initialize(StartupArg startup)
        {
            if (base.Initialize(startup)) {
                Controls.Add(mainPanel);
                Size = mainPanel.Size;
            }

            return true;
        }
    }
}
