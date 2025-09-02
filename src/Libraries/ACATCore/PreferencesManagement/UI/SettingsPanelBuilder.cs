using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

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
            var scrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                Padding = new Thickness(0, 0, 40, 0),
                CanContentScroll = true,
                SnapsToDevicePixels = true,
                UseLayoutRounding = true,
            };

            var props = GetObservableProperties(prefs.GetType());

            var itemsControl = new ItemsControl
            {
                ItemsSource = props,
                SnapsToDevicePixels = true
            };

            itemsControl.ItemTemplate = new DataTemplate(typeof(ObservablePropertyInfo))
            {
                VisualTree = new FrameworkElementFactory(typeof(Border), "container")
            };

            itemsControl.ItemTemplate = BuildItemTemplate(prefs);

            scrollViewer.DataContext = prefs;
            scrollViewer.Content = itemsControl;

            return scrollViewer;
        }

        private DataTemplate BuildItemTemplate(PreferencesBase prefs)
        {
            var template = new DataTemplate(typeof(ObservablePropertyInfo));

            var factory = new FrameworkElementFactory(typeof(ContentPresenter));

            factory.SetBinding(ContentPresenter.ContentProperty, new Binding("."));
            factory.SetValue(ContentPresenter.MarginProperty, new Thickness(0, 0, 0, 16));
            factory.AddHandler(ContentPresenter.LoadedEvent,
                new RoutedEventHandler((s, e) =>
                {
                    if (s is ContentPresenter cp && cp.Content is ObservablePropertyInfo prop)
                    {
                        cp.Content = CreateLabeledPanel(prop, prefs);
                    }
                }));

            template.VisualTree = factory;

            return template;
        }

        private static StackPanel CreateLabeledSlider(double min, double max, double initialValue = 0, double tickFrequency = 0)
        {
            var panel = new StackPanel
            {
                Orientation = System.Windows.Controls.Orientation.Vertical,
                Margin = new Thickness(8)
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

            panel.Children.Add(slider);

            panel.Children.Add(CreateMinMaxLabel(min, max));

            return panel;
        }

        private static DockPanel CreateMinMaxLabel(double min, double max)
        {
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
            return labelPanel;
        }

        protected FrameworkElement CreateLabeledPanel(ObservablePropertyInfo prop, object settingsInstance)
        {
            var value = prop.Property.GetValue(settingsInstance);

            var container = new Border
            {
                Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(200, 74, 75, 93)),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Padding = new Thickness(16),
            };

            var grid = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                ShowGridLines = true,
            };

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // labels
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) });                   // spacer
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });                      // input

            var labelStack = BuildLabelStack(prop);
            Grid.SetColumn(labelStack, 0);
            grid.Children.Add(labelStack);

            var spacer = new Border { HorizontalAlignment = HorizontalAlignment.Stretch };
            Grid.SetColumn(spacer, 1);
            grid.Children.Add(spacer);

            var inputControl = BuildInputControl(prop, value, settingsInstance);
            Grid.SetColumn(inputControl, 2);
            grid.Children.Add(inputControl);

            container.Child = grid;

            return container;

            //// LEFT: labels
            //var stackPanel = new StackPanel
            //{
            //    Orientation = System.Windows.Controls.Orientation.Vertical,
            //    HorizontalAlignment = HorizontalAlignment.Stretch
            //};

            //float labelFontSize = 14f / scaleFactor;

            //var displayAttribute = prop.GetAttribute<DisplayAttribute>();
            //var label = new TextBlock
            //{
            //    Text = displayAttribute?.ResourceType != null && !string.IsNullOrEmpty(displayAttribute.Name)
            //        ? (displayAttribute.ResourceType.GetProperty(displayAttribute.Name, BindingFlags.Static | BindingFlags.Public)?
            //            .GetValue(null, null) as string ?? displayAttribute.Name)
            //        : displayAttribute?.Name ?? "MISSING DESCRIPTION",
            //    HorizontalAlignment = HorizontalAlignment.Stretch,
            //    FontFamily = new System.Windows.Media.FontFamily("Montserrat"),
            //    FontSize = 14,
            //    FontWeight = FontWeights.DemiBold,
            //    Foreground = System.Windows.Media.Brushes.White,
            //    TextWrapping = TextWrapping.WrapWithOverflow,
            //    Padding = new Thickness(left: 10, top: 0, right: 0, bottom: 0),
            //};

            //stackPanel.Children.Add(label);

            //if (!string.IsNullOrEmpty(displayAttribute?.Description))
            //{
            //    var description = new TextBlock
            //    {
            //        Text = displayAttribute?.ResourceType?.GetProperty(displayAttribute.Description, BindingFlags.Static | BindingFlags.Public)?
            //            .GetValue(null, null) as string ?? displayAttribute?.Description,
            //        FontStyle = FontStyles.Normal,
            //        FontSize = label.FontSize,
            //        FontFamily = label.FontFamily,
            //        Foreground = System.Windows.Media.Brushes.White,
            //        TextWrapping = TextWrapping.WrapWithOverflow,
            //        Padding = new Thickness(left: 14, top: 0, right: 0, bottom: 0),
            //    };
            //    stackPanel.Children.Add(description);
            //}

            //Grid.SetColumn(stackPanel, 0);
            //grid.Children.Add(stackPanel);

            //// MIDDLE: spacer
            //var spacer = new Border
            //{
            //    HorizontalAlignment = HorizontalAlignment.Stretch
            //};
            //Grid.SetColumn(spacer, 1);
            //grid.Children.Add(spacer);

            //// RIGHT: input control
            //var binding = new System.Windows.Data.Binding(prop.Name)
            //{
            //    Source = settingsInstance,
            //    Mode = BindingMode.TwoWay
            //};

            //FrameworkElement inputControl;

            //if (prop.Property.PropertyType == typeof(string) &&
            //    prop.GetAttribute<UIHintAttribute>()?.UIHint == "PinEntry")
            //{
            //    inputControl = new PasswordBox
            //    {
            //        MaxLength = 5,
            //        VerticalAlignment = VerticalAlignment.Center,
            //        HorizontalAlignment = HorizontalAlignment.Stretch,
            //        MinWidth = scaledWidthColumn1/2,
            //        Padding = new Thickness(200, 0, 0, 0)
            //    };
            //}
            //else if (prop.Property.PropertyType == typeof(bool))
            //{
            //    bool initialState = value is bool b && b;

            //    var toggleContainer = new StackPanel
            //    {
            //        Orientation = System.Windows.Controls.Orientation.Horizontal,
            //        VerticalAlignment = VerticalAlignment.Center,
            //        HorizontalAlignment = HorizontalAlignment.Right
            //    };

            //    var labelToggle = new TextBlock
            //    {
            //        Text = initialState ? "On" : "Off",
            //        FontFamily = new System.Windows.Media.FontFamily("Montserrat"),
            //        Foreground = System.Windows.Media.Brushes.White,
            //        FontSize = labelFontSize,
            //        VerticalAlignment = VerticalAlignment.Center,
            //        Margin = new Thickness(0, 0, 8, 0)
            //    };

            //    var toggle = new ToggleSwitch
            //    {
            //        IsOn = initialState,
            //        OnContent = null,
            //        OffContent = null,
            //        VerticalAlignment = VerticalAlignment.Center
            //    };
            //    toggle.SetBinding(ToggleSwitch.IsOnProperty, binding);
            //    toggle.Toggled += (s, e) => { labelToggle.Text = toggle.IsOn ? "On" : "Off"; };

            //    toggleContainer.Children.Add(labelToggle);
            //    toggleContainer.Children.Add(toggle);
            //    inputControl = toggleContainer;
            //}
            //else if (prop.GetAttribute<UIHintAttribute>()?.UIHint == "TextBox" ||
            //         prop.Property.PropertyType == typeof(string))
            //{
            //    var tb = new System.Windows.Controls.TextBox
            //    {
            //        VerticalAlignment = VerticalAlignment.Center,
            //        HorizontalAlignment = HorizontalAlignment.Right
            //    };
            //    tb.SetBinding(System.Windows.Controls.TextBox.TextProperty, binding);
            //    inputControl = tb;
            //}
            //else if (prop.Property.PropertyType == typeof(int))
            //{
            //    RangeAttribute range = prop.GetAttribute<RangeAttribute>() ?? new RangeAttribute(0, 25);

            //    var sliderStack = CreateLabeledSlider((int)range.Minimum, (int)range.Maximum, value is int i ? i : 0, 1);
            //    sliderStack.VerticalAlignment = VerticalAlignment.Center;
            //    sliderStack.HorizontalAlignment =  HorizontalAlignment.Right;
            //    var sliderStackscaledWidth = (int)(300 / scaleFactor);
            //    sliderStack.Width = sliderStackscaledWidth;

            //    var slider = sliderStack?.Children?.OfType<Slider>().FirstOrDefault();
            //    slider?.SetBinding(Slider.ValueProperty, binding);
            //    slider?.SetBinding(Slider.ToolTipProperty, binding);

            //    inputControl = sliderStack;
            //}
            //else
            //{
            //    inputControl = new TextBlock
            //    {
            //        Text = $"Unsupported: {prop.Property.PropertyType.Name}",
            //        Foreground = System.Windows.Media.Brushes.Gray,
            //        VerticalAlignment = VerticalAlignment.Center,
            //        HorizontalAlignment = HorizontalAlignment.Right
            //    };
            //}

            //if (inputControl is StackPanel sp && sp.Children.OfType<ToggleSwitch>().Any())
            //{
            //    middleColumn.Width = new GridLength(scaledWidthColumn2 * 4.7); // toggle switch
            //}

            //Grid.SetColumn(inputControl, 2);
            //grid.Children.Add(inputControl);
            //container.Child = grid;

            //return container;
        }

        private UIElement BuildInputControl(ObservablePropertyInfo prop, object value, object settingsInstance)
        {
            FrameworkElement inputControl;

            if (prop.Property.PropertyType == typeof(string) &&
                prop.GetAttribute<UIHintAttribute>()?.UIHint == "PinEntry")
            {
                inputControl = new PasswordBox
                {
                    MaxLength = 5,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    //Padding = new Thickness(200, 0, 0, 0)
                };
            }
            else if (prop.Property.PropertyType == typeof(bool))
            {
                bool initialState = value is bool b && b;

                var toggleContainer = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Right
                };

                var labelToggle = new TextBlock
                {
                    Text = initialState ? "On" : "Off",
                    FontFamily = new System.Windows.Media.FontFamily("Montserrat"),
                    Foreground = System.Windows.Media.Brushes.White,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, 8, 0)
                };

                var toggle = new ToggleSwitch
                {
                    IsOn = initialState,
                    OnContent = null,
                    OffContent = null,
                    VerticalAlignment = VerticalAlignment.Center
                };
                toggle.Toggled += (s, e) => { labelToggle.Text = toggle.IsOn ? "On" : "Off"; };

                toggleContainer.Children.Add(labelToggle);
                toggleContainer.Children.Add(toggle);
                inputControl = toggleContainer;
            }
            else if (prop.GetAttribute<UIHintAttribute>()?.UIHint == "TextBox" ||
                     prop.Property.PropertyType == typeof(string))
            {
                var tb = new System.Windows.Controls.TextBox
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Right
                };
                inputControl = tb;
            }
            else if (prop.Property.PropertyType == typeof(int))
            {
                RangeAttribute range = prop.GetAttribute<RangeAttribute>() ?? new RangeAttribute(0, 25);

                var sliderStack = CreateLabeledSlider((int)range.Minimum, (int)range.Maximum, value is int i ? i : 0, 1);
                sliderStack.VerticalAlignment = VerticalAlignment.Center;
                sliderStack.HorizontalAlignment = HorizontalAlignment.Right;

                var slider = sliderStack?.Children?.OfType<Slider>().FirstOrDefault();

                inputControl = sliderStack;
            }
            else
            {
                inputControl = new TextBlock
                {
                    Text = $"Unsupported: {prop.Property.PropertyType.Name}",
                    Foreground = System.Windows.Media.Brushes.Gray,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Right
                };
            }

            return inputControl;
        }

        private UIElement BuildLabelStack(ObservablePropertyInfo prop)
        {
            var stackPanel = new StackPanel
            {
                Orientation = System.Windows.Controls.Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            var displayAttribute = prop.GetAttribute<DisplayAttribute>();
            var label = new TextBlock
            {
                Text = displayAttribute?.ResourceType != null && !string.IsNullOrEmpty(displayAttribute.Name)
                    ? (displayAttribute.ResourceType.GetProperty(displayAttribute.Name, BindingFlags.Static | BindingFlags.Public)?
                        .GetValue(null, null) as string ?? displayAttribute.Name)
                    : displayAttribute?.Name ?? "MISSING DESCRIPTION",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                FontFamily = new System.Windows.Media.FontFamily("Montserrat"),
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = System.Windows.Media.Brushes.White,
                TextWrapping = TextWrapping.WrapWithOverflow,
            };

            stackPanel.Children.Add(label);

            if (!string.IsNullOrEmpty(displayAttribute?.Description))
            {
                var description = new TextBlock
                {
                    Text = displayAttribute?.ResourceType?.GetProperty(displayAttribute.Description, BindingFlags.Static | BindingFlags.Public)?
                        .GetValue(null, null) as string ?? displayAttribute?.Description,
                    FontStyle = FontStyles.Normal,
                    FontSize = label.FontSize,
                    FontFamily = label.FontFamily,
                    Foreground = System.Windows.Media.Brushes.White,
                    TextWrapping = TextWrapping.WrapWithOverflow,
                    Padding = new Thickness(left: 0, top: 0, right: 0, bottom: 0),
                };
                stackPanel.Children.Add(description);
            }

            return stackPanel;
        }

        private StackPanel AddSettingRow(UIElement label, UIElement control, double verticalMargin = 8)
        {
            var rowPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, verticalMargin / 2, 0, verticalMargin / 2),
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            rowPanel.Children.Add(label);
            rowPanel.Children.Add(control);

            // Control will stretch to fill remaining space
            //control.HorizontalAlignment = HorizontalAlignment.Stretch;

            return rowPanel;
        }
    }
}