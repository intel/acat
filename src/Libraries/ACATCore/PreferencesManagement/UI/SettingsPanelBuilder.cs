using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;


namespace ACAT.Core.PreferencesManagement.UI
{ 
 
    public class SettingsPanelBuilder
    {
        protected class ObservablePropertyInfo
        {
            public PropertyInfo Property { get; set; }
            public FieldInfo BackingField { get; set; }
            public string Name => Property.Name;

#nullable enable
            public T? GetAttribute<T>() where T : Attribute
            {
                return BackingField.GetCustomAttribute<T>();
            }
#nullable disable
        }

        protected static List<ObservablePropertyInfo> GetObservableProperties(Type type)
        {
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
//                .Where(f => f.GetCustomAttribute(typeof(ObservablePropertyAttribute)) != null);

            var list = new List<ObservablePropertyInfo>();

            foreach (var field in fields)
            {
                var propertyName = Char.ToUpper(field.Name[0]) + field.Name.Substring(1);
                var prop = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);

                if (prop != null)
                {
                    list.Add(new ObservablePropertyInfo
                    {
                        Property = prop,
                        BackingField = field
                    });
                }
            }

            return list;
        }

        public FrameworkElement CreateScrollViewer(PreferencesBase prefs)
        {
            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical
            };

            var scrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
            };


            var props = GetObservableProperties(prefs.GetType());

            foreach (var prop in props)
            {
                var labeledPanel = CreateLabeledPanel(prop, prefs);
                labeledPanel.Margin = new Thickness(10);
                stackPanel.Children.Add(labeledPanel);
            }
            scrollViewer.DataContext = prefs; // Set the DataContext to the preferences object for binding

            scrollViewer.Content = stackPanel;

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

    

        protected FrameworkElement CreateLabeledPanel(ObservablePropertyInfo prop, object settingsInstance)
        {
            var value = prop.Property.GetValue(settingsInstance);

            var grid = new Grid();

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(7, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });

            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            var displayAttribute = prop.GetAttribute<DisplayAttribute>();
            var label = new TextBlock
            {
                Text = displayAttribute?.ResourceType != null && !string.IsNullOrEmpty(displayAttribute.Name) ? (displayAttribute.ResourceType.GetProperty(displayAttribute.Name, BindingFlags.Static | BindingFlags.Public)?
                .GetValue(null, null) as string ?? displayAttribute.Name): displayAttribute?.Name ?? "MISSING DESCRIPTION",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                FontFamily = new System.Windows.Media.FontFamily("Montserrat"),
                FontSize = 14,
                FontStyle = FontStyles.Normal,
                FontWeight = FontWeights.DemiBold,
                Foreground = System.Windows.Media.Brushes.White,
                TextWrapping = TextWrapping.WrapWithOverflow,
            };
            stackPanel.Children.Add(label);

                if (!string.IsNullOrEmpty(displayAttribute?.Description))
                {
                var description = new TextBlock
                {
                    Text = displayAttribute?.ResourceType?.GetProperty(displayAttribute.Description, BindingFlags.Static | BindingFlags.Public)? .GetValue(null, null) as string ?? displayAttribute?.Description,
                    FontStyle = FontStyles.Italic,
                    FontSize = 12,
                    FontFamily = label.FontFamily,
                    Foreground = System.Windows.Media.Brushes.White,
                    TextWrapping = TextWrapping.WrapWithOverflow,
                };
                stackPanel.Children.Add(description);
            }
            Grid.SetColumn(stackPanel, 0);

            /// Create the actual user control now and bind it to the preference property.
            FrameworkElement inputControl;

            var binding = new Binding(prop.Name)
            {
                Source = settingsInstance,
                Mode = BindingMode.TwoWay
            };

            if (prop.Property.PropertyType == typeof(string) && prop.GetAttribute<UIHintAttribute>()?.UIHint == "PinEntry")
            {
                inputControl = new PasswordBox
                {
                    //Password = value?.ToString() ?? "",
                    MaxLength = 5, // Assuming PIN is 5 digits
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Right
                };
            }
            else if (prop.Property.PropertyType == typeof(bool))
            {
                inputControl = new ToggleSwitch
                {
                    //IsOn = value is bool b && b,
                    OnContent = "Yes",
                    OffContent = "No"
                };
                inputControl.SetBinding(ToggleSwitch.IsOnProperty, binding);
            }

            else if (prop.GetAttribute<UIHintAttribute>()?.UIHint == "TextBox" || prop.Property.PropertyType == typeof(string))
            {
                inputControl = new TextBox
                {
                    //Text = value?.ToString() ?? "",
                };
                inputControl.SetBinding(TextBox.TextProperty, binding);
            }

            else if (prop.Property.PropertyType == typeof(int))
            {
                RangeAttribute range = prop.GetAttribute<RangeAttribute>()
                    ?? new RangeAttribute(0, 25);

                StackPanel sliderStack = CreateLabeledSlider((int)range.Minimum, (int)range.Maximum, value is int i ? i : 0, 1);
                // Bind the slider value to the property
                binding.StringFormat = "Value {0:f0}";
                var slider = sliderStack?.Children?.OfType<Slider>().FirstOrDefault();

                slider?.SetBinding(Slider.ValueProperty, binding);
                slider?.SetBinding(Slider.ToolTipProperty, binding);

                inputControl = sliderStack;
            }
            else
            {
                // fallback label for unsupported types
                inputControl = new TextBlock
                {
                    Text = $"Unsupported: {prop.Property.PropertyType.Name}",
                    Foreground = System.Windows.Media.Brushes.Gray,
                    VerticalAlignment = VerticalAlignment.Center
                };
            }
            inputControl.VerticalAlignment = VerticalAlignment.Center;
            inputControl.HorizontalAlignment = HorizontalAlignment.Right;

            inputControl.Tag = prop;
            Grid.SetColumn(inputControl, 1);

            grid.Children.Add(stackPanel);
            grid.Children.Add(inputControl);
            return grid;
        }
    }
}