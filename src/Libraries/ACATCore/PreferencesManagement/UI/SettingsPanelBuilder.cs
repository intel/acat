using ACAT.Core.Utility;
using MahApps.Metro.Controls;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;


namespace ACAT.Lib.Core.PreferencesManagement.UI
{ 
 
    public class SettingsPanelBuilder
    {
        public SettingsPanelBuilder()
        {
            if (Application.Current != null && Application.Current.Resources.MergedDictionaries.Count == 0)
            {
                var resources = new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml", UriKind.Absolute)
                };
                Application.Current.Resources.MergedDictionaries.Add(resources);
                Application.Current.Resources.MergedDictionaries.Add(
                    new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml") });
                Application.Current.Resources.MergedDictionaries.Add(
                    new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Themes/Dark.Amber.xaml") });
            }
        }

        public FrameworkElement CreateLabeledPanel(PropertyInfo prop, object settingsInstance)
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
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto});

            var descriptionAttr = prop.GetCustomAttribute<DescriptorAttribute>();
            var labelText = descriptionAttr?.Description ?? "MISSING DESCRIPTION";

            var label = new TextBlock
            {
                Text = labelText,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 500,
                FontFamily = new System.Windows.Media.FontFamily("Montserrat"),
                FontSize = 24,
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
                inputControl = new TextBox
                {
                    Text = value?.ToString() ?? "",
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
    }
}