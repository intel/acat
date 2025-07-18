using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Forms;
using ACAT.Lib.Core.Utility;
using MahApps.Metro.Controls;
using FontStyle = System.Drawing.FontStyle;
using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Forms.Integration;
using static ACAT.Lib.Core.Interpreter.Interpret;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Media.Media3D;
using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.PreferencesManagement;
using static ACATConfigNext.Program;
using ACAT.Lib.Core.ActuatorManagement;


namespace ACATConfigNext
{
    public static class CustomControls
    {
        public static System.Windows.Forms.Label CreateLabel(string text, FontStyle style = FontStyle.Bold)
        {
            return new System.Windows.Forms.Label
            {
                Text = text,
                Dock = DockStyle.Bottom,
                //AutoSize = true,
                Font = new Font("Montserrat", 10, style),
                ForeColor = Color.White,
              //  Margin = new Padding(0, 0, 0, 5)
            };
        }

        public static System.Windows.Forms.Label CreateDescriptionLabel(string description)
        {
            return new System.Windows.Forms.Label
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

        public static System.Windows.Forms.TableLayoutPanel CreateCategoryTableLayoutPanel()
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

        public static System.Windows.Forms.CheckBox CreateCheckBox(string text)
        {
            return new System.Windows.Forms.CheckBox
            {
                Text = text,
                AutoSize = true,
                Font = new Font("Montserrat", 8),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Margin = new Padding(5)
            };
        }

        public static System.Windows.Forms.Button CreateFlatButton(string text, object tag = null, int? width = null, int? top = null, int? left = null, int? height = null)
        {
            var button = new System.Windows.Forms.Button
            {
                Text = text,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                // Font = new Font("Montserrat", 11),
                //ForeColor = Color.White,
                //BackColor = Color.FromArgb(60, 63, 80),
                //Margin = new Padding(5),
                //Padding = new Padding(6),
                //AutoSize = width == null,
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

        public static System.Windows.Forms.Button CreateSetupButton(string text, EventHandler onClick = null, object tag = null, int? width = null, int? top = null, int? left = null, int? height = null)
        {
            var button = new System.Windows.Forms.Button
            {
                Text = text,
               FlatStyle = FlatStyle.Flat,
               FlatAppearance = { BorderSize = 0 },
                 Font = new Font("Montserrat", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(48, 49, 64),
                Margin = new Padding(5),
                Padding = new Padding(6),
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

            if (onClick != null)
            {
                button.Click += onClick;
            }

            return button;
        }


        public static System.Windows.Forms.Panel CreatePanel(DockStyle dock, int width, Color? backColor = null)
        {
            return new System.Windows.Forms.Panel
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
                var label = new System.Windows.Forms.Label
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

        public static FrameworkElement CreateLabeledPanel(PropertyInfo prop, object settingsInstance)
        {
            var value = prop.GetValue(settingsInstance);

            //// Horizontal layout for label + input
            //var panel = new StackPanel
            //{
            //    Orientation = Orientation.Horizontal,
            //    Margin = new Thickness(4)
            //};

            var grid = new Grid
            {
                Margin = new Thickness(4)
            };
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var descriptionAttr = prop.GetCustomAttribute<DescriptorAttribute>();
            var labelText = descriptionAttr?.Description ?? "MISSING DESCRIPTION";

            var label = new TextBlock
            {
                Text = labelText,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 500,
                FontFamily = new System.Windows.Media.FontFamily("Montserrat"),
                FontSize = 6,
                FontStyle = FontStyles.Normal,
                FontWeight = FontWeights.Regular,
                Foreground = System.Windows.Media.Brushes.White,
                TextWrapping = TextWrapping.WrapWithOverflow,
            };

            Grid.SetColumn(label, 0);

            FrameworkElement inputControl;

            if (prop.PropertyType == typeof(bool))
            {
                //inputControl = new CheckBox
                //{
                //    IsChecked = value is bool b && b,
                //    VerticalAlignment = VerticalAlignment.Center
                //};
                inputControl = new ToggleSwitch
                {
                    IsOn = value is bool b && b,
                    VerticalAlignment = VerticalAlignment.Center,
                    Width = 100,
                    FontFamily = new System.Windows.Media.FontFamily("Montserrat"),
                    FontSize = 8,
                    OnContent = "Yes",
                    OffContent = "No"
                };
            }
            else if (prop.PropertyType == typeof(int))
            {
                var slider = new Slider
                {
                    Minimum = 0,
                    Maximum = 100,
                    Width = 150,
                    Value = value is int i ? i : 0,
                    FontFamily = new System.Windows.Media.FontFamily("Montserrat"),
                    FontSize = 8,
                    VerticalAlignment = VerticalAlignment.Center
                };

                // Optional: apply [Range] attribute
                var rangeAttr = prop.GetCustomAttribute<RangeAttribute>();
                if (rangeAttr != null)
                {
                    slider.Minimum = (int)rangeAttr.Minimum;
                    slider.Maximum = (int)rangeAttr.Maximum;
                }

                inputControl = slider;
            }
            else if (prop.PropertyType == typeof(string))
            {
                inputControl = new System.Windows.Controls.TextBox
                {
                    Text = value?.ToString() ?? "",
                    FontFamily = new System.Windows.Media.FontFamily("Montserrat"),
                    FontSize = 8,
                    Width = 100,
                    VerticalAlignment = VerticalAlignment.Center
                };
            }
            else
            {
                // fallback label for unsupported types
                inputControl = new TextBlock
                {
                    Text = $"Unsupported: {prop.PropertyType.Name}",
                    FontFamily = new System.Windows.Media.FontFamily("Montserrat"),
                    FontSize = 6,
                    Foreground = System.Windows.Media.Brushes.Gray,
                    VerticalAlignment = VerticalAlignment.Center
                };
            }

            inputControl.Tag = prop;
            Grid.SetColumn(inputControl, 1);

            grid.Children.Add(label);
            grid.Children.Add(inputControl);
            return grid;
        }

        public static ElementHost ElementHost(FrameworkElement CreateLabeledPanel) 
        {
            var button = new ElementHost
            {
                Child = CreateLabeledPanel,
                AutoSize = true,
                Margin = new Padding(1),
                Dock = DockStyle.Top
            };

            return button;
        }

  

    }
}
