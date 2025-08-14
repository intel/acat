using System.Drawing;

namespace ACATApp.Utilities
{
    public static class ButtonIconGenerator
    {

        public static Bitmap WindowsStartButton(int size)
        {
            // Create a square bitmap
            Bitmap bmp = new(size, size);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Define a simple 4-pane white Windows flag
                using Brush flagBrush = new SolidBrush(Color.White);
                int margin = size / 6;
                int half = size / 2 - margin / 2;

                // Top-left pane
                g.FillRectangle(flagBrush, margin, margin, half - margin / 2, half - margin / 2);

                // Top-right pane
                g.FillRectangle(flagBrush, size / 2 + margin / 2, margin, half - margin / 2, half - margin / 2);

                // Bottom-left pane
                g.FillRectangle(flagBrush, margin, size / 2 + margin / 2, half - margin / 2, half - margin / 2);

                // Bottom-right pane
                g.FillRectangle(flagBrush, size / 2 + margin / 2, size / 2 + margin / 2, half - margin / 2, half - margin / 2);
            }

            return bmp;
        }
    }
}
