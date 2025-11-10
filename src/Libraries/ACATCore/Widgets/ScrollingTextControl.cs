using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACAT.Core.Widgets
{
    public partial class ScrollingTextControl : Control
    {
        private Timer scrollTimer;
        private int textX;
        private string displayText = "All your bases are belong to us.";
        private int scrollSpeed = 2;

        public string ScrollText
        {
            get => displayText;
            set
            {
                displayText = value;
                textX = this.Width;
                Invalidate();
            }
        }
        public int ScrollSpeed
        {
            get => scrollSpeed;
            set => Math.Max(1, value);
        }

        public ScrollingTextControl()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.UserPaint, true);

            scrollTimer = new Timer();
            scrollTimer.Interval = 15;
            scrollTimer.Tick += ScrollTimer_Tick;
            scrollTimer.Start();

            this.Font = new Font("Montserrat", 36, FontStyle.Regular);
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            this.ForeColor = Color.White;
        }

        private void ScrollTimer_Tick(object sender, EventArgs e)
        {
            textX -= scrollSpeed;
            if (textX < -TextRenderer.MeasureText(displayText, this.Font).Width)
            {
                textX = this.Width;
            }
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.Clear(this.BackColor);
            TextRenderer.DrawText(e.Graphics, ScrollText, this.Font, new Point(textX,0), this.ForeColor);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                {
                    scrollTimer?.Dispose();

                }
            }
            base.Dispose(disposing);
        }
    }
}
