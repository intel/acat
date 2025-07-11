using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACATConfigNext
{
    public static class CustomControls
    {
        public static Label CreateLabel(string text, FontStyle style = FontStyle.Bold)
        {
            return new Label
            {
                Text = text,
                Dock = DockStyle.Bottom,
                //AutoSize = true,
                Font = new Font("Montserrat", 10, style),
                ForeColor = Color.White,
              //  Margin = new Padding(0, 0, 0, 5)
            };
        }

        public static Label CreateDescriptionLabel(string description)
        {
            return new Label
            {
                Text = InsertLineBreaks(description, 60),
                Dock = DockStyle.Bottom,
                AutoSize = true,
                Font = new Font("Segoe UI", 8, FontStyle.Italic),
                ForeColor = Color.White,
              //  Margin = new Padding(0, 0, 0, 5)
            };
        }

        private static string InsertLineBreaks(string text, int maxLineLength)
        {
            if (string.IsNullOrWhiteSpace(text) || text.Length <= maxLineLength)
                return text;

            var words = text.Split(' ');
            var result = new List<string>();
            var line = new StringBuilder();

            foreach (var word in words)
            {
                if ((line.Length + word.Length + 1) > maxLineLength)
                {
                    result.Add(line.ToString().TrimEnd());
                    line.Clear();
                }

                line.Append(word + " ");
            }

            if (line.Length > 0)
                result.Add(line.ToString().TrimEnd());

            return string.Join("\n", result);
        }

        public static TableLayoutPanel CreateCategoryTableLayoutPanel()
        {
            var panel = new TableLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowOnly,
              //  Margin = new Padding(10),
                Padding = new Padding(10),
                BackColor = Color.FromArgb(48, 49, 64),
                Dock = DockStyle.Top,
            };

            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));

            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            return panel;
        }

        public static CheckBox CreateCheckBox(string text)
        {
            return new CheckBox
            {
                Text = text,
                AutoSize = true,
                Font = new Font("Montserrat", 12),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Margin = new Padding(5)
            };
        }

        public static Button CreateFlatButton(string text,object tag = null,int? width = null,int? top = null, int? left = null, int? height = null)
        {
            var button = new Button
            {
                Text = text,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                // Font = new Font("Montserrat", 11),
                //ForeColor = Color.White,
                //BackColor = Color.FromArgb(60, 63, 80),
                //Margin = new Padding(5),
                //Padding = new Padding(6),
                AutoSize = width == null,
                Tag = tag
            };

            if (width.HasValue)
                button.Width = width.Value;

            if (top.HasValue)
                button.Top = top.Value;

            if (left.HasValue)
                button.Left = left.Value;

            if (height.HasValue)
                button.Height = height.Value;

            return button;
        }

        public static Panel CreatePanel(DockStyle dock, int width, Color? backColor = null)
        {
            return new Panel
            {
                Dock = dock,
                Width = width,
                BackColor = backColor ?? Color.Transparent,
                Margin = new Padding(0)
            };
        }

        public static FlowLayoutPanel CreateFlowPanel(DockStyle dock,int height,string text = null,Padding? padding = null,bool autoScroll = true,Color? backColor = null)
        {
            var panel = new FlowLayoutPanel
            {
                Dock = dock,
                Height = height,
                AutoScroll = autoScroll,
                Padding = padding ?? new Padding(5, 5, 0, 0),
                BackColor = backColor ?? Color.Transparent,
                WrapContents = false,
                AutoSize = false,
                Margin = new Padding(0)
            };

            if (!string.IsNullOrEmpty(text))
            {
                var label = new Label
                {
                    Text = text,
                    AutoSize = true,
                    Font = new Font("Montserrat", 12, FontStyle.Bold),
                    ForeColor = Color.White,
                    Margin = new Padding(0, 5, 10, 0)
                };

                panel.Controls.Add(label);
            }

            return panel;
        }



    }
}
