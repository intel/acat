using ACAT.Core.SpellCheckManagement;
using CommunityToolkit.Mvvm.ComponentModel;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
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

        protected static IEnumerable<FieldInfo> GetAllInstanceFields(Type type)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly;

            while (type != null)
            {
                foreach (FieldInfo field in type.GetFields(flags))
                {
                    yield return field;
                }
                type = type.BaseType;
            }
        }

        protected static List<ObservablePropertyInfo> GetObservableProperties(Type type)
        {

            IEnumerable<FieldInfo> fields = GetAllInstanceFields(type)
                .Where(f => f.GetCustomAttribute<ObservablePropertyAttribute>() != null);

            var list = new List<ObservablePropertyInfo>();

            foreach (FieldInfo field in fields)
            {
                var propertyName = Char.ToUpper(field.Name[0]) + field.Name.Substring(1);
                PropertyInfo prop = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);

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

            List<ObservablePropertyInfo> props = GetObservableProperties(prefs.GetType());

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
            // This DataTemplate is for ObservablePropertyInfo items
            var template = new DataTemplate(typeof(ObservablePropertyInfo));

            // Use ContentControl instead of ContentPresenter to avoid recursion
            var factory = new FrameworkElementFactory(typeof(ContentControl));

            // Give a little margin like before
            factory.SetValue(ContentControl.MarginProperty, new Thickness(0, 0, 0, 16));

            // Replace the content with your dynamically generated panel
            factory.AddHandler(ContentControl.LoadedEvent,
                new RoutedEventHandler((s, e) =>
                {
                    if (s is ContentControl cc && cc.DataContext is ObservablePropertyInfo prop)
                    {
                        // Generate the actual labeled control for this property
                        cc.Content = CreateLabeledPanel(prop, prefs);
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

            // --- WRAP THE ROW IN A BORDER FOR BACKGROUND AND MARGIN ---
            var container = new Border
            {
                Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(200, 74, 75, 93)), // row-specific background
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Padding = new Thickness(16),
                Margin = new Thickness(0, 4, 0, 4) // vertical spacing between rows
            };

            // --- ROW GRID WITH CONSISTENT COLUMN DEFINITIONS ---
            var rowGrid = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
            };

            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) }); // label
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(8) });                    // spacer
            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });                       // control

            // --- LEFT: label stack ---
            UIElement labelStack = BuildLabelStack(prop);
            Grid.SetColumn(labelStack, 0);
            rowGrid.Children.Add(labelStack);

            // --- MIDDLE: spacer (optional) ---
            var spacer = new Border { HorizontalAlignment = HorizontalAlignment.Stretch };
            Grid.SetColumn(spacer, 1);
            rowGrid.Children.Add(spacer);

            // --- RIGHT: input control ---
            UIElement inputControl = BuildInputControl(prop, value, settingsInstance);
            Grid.SetColumn(inputControl, 2);
            rowGrid.Children.Add(inputControl);

            // --- SET THE ROW GRID AS THE CHILD OF THE BORDER ---
            container.Child = rowGrid;

            return container;
        }

        private UIElement BuildInputControl(ObservablePropertyInfo prop, object value, object settingsInstance)
        {
            FrameworkElement inputControl;

            if (prop.Property.PropertyType == typeof(string) &&
                prop.GetAttribute<UIHintAttribute>()?.UIHint == "PinEntry")
            {
                var passwordBox = new TextBox
                {
                    FontSize = 24,
                    MaxLength = 5,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    //Padding = new Thickness(200, 0, 0, 0)
                };

                passwordBox.SetBinding(TextBox.TextProperty, new Binding(prop.Name)
                {
                    Source = settingsInstance,
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

                inputControl = passwordBox;
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
                    FontSize = 24,
                    Foreground = System.Windows.Media.Brushes.White,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(0, 0, 8, 0)
                };

                var toggle = new ToggleSwitch
                {
                    IsOn = initialState,
                    OnContent = null,
                    OffContent = null,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    FontSize = 24,
                };
                toggle.SetBinding(ToggleSwitch.IsOnProperty, new Binding(prop.Name)
                {
                    Source = settingsInstance,
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });
                toggle.Toggled += (s, e) => { labelToggle.Text = toggle.IsOn ? "On" : "Off"; };

                toggleContainer.Children.Add(labelToggle);
                toggleContainer.Children.Add(toggle);
                inputControl = toggleContainer;
            }
            else if (prop.GetAttribute<UIHintAttribute>()?.UIHint == "TextBox" ||
                     prop.Property.PropertyType == typeof(string))
            {
                var textBox = new System.Windows.Controls.TextBox
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    FontSize = 24,

                };
                textBox.SetBinding(TextBox.TextProperty, new Binding(prop.Name)
                {
                    Source = settingsInstance,
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                });

                inputControl = textBox;
            }
            else if (prop.Property.PropertyType == typeof(int))
            {
                RangeAttribute range = prop.GetAttribute<RangeAttribute>() ?? new RangeAttribute(0, 25);

                StackPanel sliderStack = CreateLabeledSlider((int)range.Minimum, (int)range.Maximum, value is int i ? i : 0, 1);
                sliderStack.VerticalAlignment = VerticalAlignment.Center;
                sliderStack.HorizontalAlignment = HorizontalAlignment.Right;

                Slider slider = sliderStack?.Children?.OfType<Slider>().FirstOrDefault();

                if (slider != null)
                {
                    slider.SetBinding(Slider.ValueProperty, new Binding(prop.Name)
                    {
                        Source = settingsInstance,
                        Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    });
                    slider.ToolTip = $"Value: {slider.Value}";
                    slider.ValueChanged += (s, e) =>
                    {
                        slider.ToolTip = $"Value: {slider.Value}";
                    };
                }

                inputControl = sliderStack;
            }
            else
            {
                inputControl = new TextBlock
                {
                    Text = $"Unsupported: {prop.Property.PropertyType.Name}",
                    Foreground = System.Windows.Media.Brushes.Gray,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    FontSize = 24,

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

            DisplayAttribute displayAttribute = prop.GetAttribute<DisplayAttribute>();
            var label = new TextBlock
            {
                Text = displayAttribute?.ResourceType != null && !string.IsNullOrEmpty(displayAttribute.Name)
                    ? (displayAttribute.ResourceType.GetProperty(displayAttribute.Name, BindingFlags.Static | BindingFlags.Public)?
                        .GetValue(null, null) as string ?? displayAttribute.Name)
                    : displayAttribute?.Name ?? "MISSING DESCRIPTION",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                FontFamily = new System.Windows.Media.FontFamily("Montserrat"),
                FontSize = 24,  // 18 Point in WinForms
                FontWeight = FontWeights.Regular,
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
                    FontSize = 23.3,  // 16 Point in WinForms
                    FontFamily = label.FontFamily,
                    Foreground = System.Windows.Media.Brushes.White,
                    TextWrapping = TextWrapping.WrapWithOverflow,
                    Padding = new Thickness(left: 0, top: 0, right: 0, bottom: 0),
                };
                stackPanel.Children.Add(description);
            }

            return stackPanel;
        }
    }
}