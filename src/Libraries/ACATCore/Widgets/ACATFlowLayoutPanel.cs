using System;
using System.Drawing;
using System.Windows.Forms;
using ACAT.Lib.Core.WidgetManagement;

namespace ACAT.Lib.Core.Widgets
{
    public class ACATFlowLayoutPanel : FlowLayoutPanel
    {
        public ACATFlowLayoutPanel()
        {
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BackColor = Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            Dock = DockStyle.Fill;
            FlowDirection = FlowDirection.LeftToRight;
            WrapContents = false;
        }
        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
        }
    }
}
