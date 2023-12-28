using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACAT.Lib.Core
{
    public partial class ExtendedTableLayout : TableLayoutPanel
    {
        private const int WS_EX_TRANSPARENT = 0x20;
        public ExtendedTableLayout()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.Opaque, false);
            SetStyle(ControlStyles.ResizeRedraw, true);
            this.BackColor = Color.Transparent;
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
