using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class CompositeGradientControl : UserControl
{
    private int _signalX = 50; // Default position in pixels

    public int SignalX
    {
        get => _signalX;
        set
        {
            _signalX = value;
            Invalidate(); // Redraw when value changes
        }
    }

    public CompositeGradientControl()
    {
        this.DoubleBuffered = true;
        this.ResizeRedraw = true;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;


        // Composite Gradient Layer 1: Top to Bottom (Red to Transparent)
        using (var redBrush = new LinearGradientBrush(this.ClientRectangle,
                    Color.Red, Color.Transparent, LinearGradientMode.Horizontal))
        {
            g.FillRectangle(redBrush, this.ClientRectangle);
        }

        // Composite Gradient Layer 2: Left to Right (Transparent to Green)
        using (var blueBrush = new LinearGradientBrush(this.ClientRectangle,
                    Color.Transparent, Color.Green, LinearGradientMode.Horizontal))
        {
            g.FillRectangle(blueBrush, this.ClientRectangle);
        }

        using (var yellowBrush = new LinearGradientBrush(this.ClientRectangle,
                    Color.Transparent, Color.Yellow, LinearGradientMode.Horizontal))
        {
            var blend = new ColorBlend
            {
                Colors = new Color[] { Color.Transparent, Color.Yellow, Color.Transparent },
                Positions = new float[] { 0.0f, 0.5f, 1.0f }
            };

            yellowBrush.InterpolationColors = blend;
            g.FillRectangle(yellowBrush, this.ClientRectangle);
        }


        // White Signal Bar
        int barWidth = 8;
        var barRect = new Rectangle(_signalX - barWidth / 2, 0, barWidth, this.Height);
        using (var whitePen = new SolidBrush(Color.White))
        {
            g.FillRectangle(whitePen, barRect);
        }
    }
}
