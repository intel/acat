
using System.Drawing;
using System.Windows.Forms;

namespace ACAT.Lib.Core.WidgetManagement
{
    public partial class ScannerTableLayout : TableLayoutPanel
    {
        public ScannerTableLayout()
        {
            InitializeComponent();
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.Opaque, false);
            SetStyle(ControlStyles.ResizeRedraw, true);
            this.BackColor = Color.Transparent;
            //this.EnabledChanged += ScannerTableLayout_EnabledChanged;
        }

        private void ScannerTableLayout_EnabledChanged(object sender, System.EventArgs e)
        {
            
        }


        protected override CreateParams CreateParams
        {

            get
            {
                const int WS_EX_TRANSPARENT = 0x20;
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WS_EX_TRANSPARENT;
                return cp;
            }

        }

    }
}
