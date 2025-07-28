using ACAT.Core.PreferencesManagement;
using ACAT.Core.Utility;
using MahApps.Metro.Controls;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;


namespace ACAT.Lib.Core.PreferencesManagement.UI
{ 
 
    public class SettingsPanelBuilder
    {
        public FrameworkElement CreateScrollViewer(IPreferences prefs)
        {

            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical
            };

            var props = prefs.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                var labeledPanel = CreateLabeledPanel(prop, prefs);
                labeledPanel.Margin = new Thickness(10);
                stackPanel.Children.Add(labeledPanel);
            }

            var scrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                Content = stackPanel,
            };
            return scrollViewer;
        }

        private static StackPanel CreateLabeledSlider(double min, double max, double initialValue = 0, double tickFrequency = 0)
        {
            var panel = new StackPanel
            {
                Orientation = Orientation.Vertical
            };

            // Create the Slider
            var slider = new Slider
            {
                Width = 300,
                Minimum = min,
                Maximum = max,
                Value = initialValue,
                TickFrequency = tickFrequency > 0 ? tickFrequency : (max - min) / 10,
                IsSnapToTickEnabled = true,
                TickPlacement = System.Windows.Controls.Primitives.TickPlacement.BottomRight,
                Margin = new Thickness(0, 0, 0, 4),
                ToolTip = $"Value: {initialValue}",
            };

            // Add the Slider to the panel
            panel.Children.Add(slider);

            // Create a label row below the slider
            var labelPanel = new DockPanel();

            var minLabel = new TextBlock
            {
                Text = min.ToString(),
                HorizontalAlignment = HorizontalAlignment.Left,
                Foreground = System.Windows.Media.Brushes.White
            };
            DockPanel.SetDock(minLabel, Dock.Left);

            var maxLabel = new TextBlock
            {
                Text = max.ToString(),
                HorizontalAlignment = HorizontalAlignment.Right,
                Foreground = System.Windows.Media.Brushes.White
            };
            DockPanel.SetDock(maxLabel, Dock.Right);

            labelPanel.Children.Add(minLabel);
            labelPanel.Children.Add(maxLabel);

            // Add label panel to main container
            panel.Children.Add(labelPanel);

            return panel;
        }

    

        public FrameworkElement CreateLabeledPanel(PropertyInfo prop, object settingsInstance)
        {
            var value = prop.GetValue(settingsInstance);

            var grid = new Grid();

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(7, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
            //grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var descriptionAttr = prop.GetCustomAttribute<DescriptorAttribute>();
            var labelText = descriptionAttr?.Description ?? "MISSING DESCRIPTION";

            var label = new TextBlock
            {
                Text = labelText.TrimEnd(),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                FontFamily = new System.Windows.Media.FontFamily("Montserrat"),
                FontSize = 14,
                FontStyle = FontStyles.Normal,
                FontWeight = FontWeights.Regular,
                Foreground = System.Windows.Media.Brushes.White,
                TextWrapping = TextWrapping.WrapWithOverflow,
            };

            Grid.SetColumn(label, 0);

            FrameworkElement inputControl;

            if (prop.PropertyType == typeof(bool))
            {
                inputControl = new ToggleSwitch
                {
                    IsOn = value is bool b && b,
                    OnContent = "Yes",
                    OffContent = "No"
                };
            }
            else if (prop.PropertyType == typeof(int))
            {
                RangeAttribute range = prop.GetCustomAttribute<RangeAttribute>()
                    ?? new RangeAttribute(0, 100);

                var sliderStack = CreateLabeledSlider((int)range.Minimum, (int)range.Maximum, value is int i ? i : 0, 1);
                inputControl = sliderStack;
            }
            else if (prop.PropertyType == typeof(string))
            {
                inputControl = new TextBox
                {
                    Text = value?.ToString() ?? "",
                };
            }
            else
            {
                // fallback label for unsupported types
                inputControl = new TextBlock
                {
                    Text = $"Unsupported: {prop.PropertyType.Name}",
                    Foreground = System.Windows.Media.Brushes.Gray,
                    VerticalAlignment = VerticalAlignment.Center
                };
            }
            inputControl.VerticalAlignment = VerticalAlignment.Center;
            inputControl.HorizontalAlignment = HorizontalAlignment.Right;

            inputControl.Tag = prop;
            Grid.SetColumn(inputControl, 1);

            grid.Children.Add(label);
            grid.Children.Add(inputControl);
            return grid;
        }
    }
}