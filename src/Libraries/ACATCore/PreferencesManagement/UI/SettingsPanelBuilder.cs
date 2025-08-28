using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Interop;
using System.Xml.Linq;
using SW = System.Windows;

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
          /*  var stackPanel = new StackPanel
            {
                Orientation = SW.Controls.Orientation.Vertical,
              //  HorizontalAlignment = HorizontalAlignment.Left
            };*/

            var scrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                Padding = new Thickness(0,0,40,0)
            };


            var props = GetObservableProperties(prefs.GetType());

            var tableLayout = new TableLayoutPanel
            {
                BackColor = Color.FromArgb(74, 75, 93),//Gray
                Dock = DockStyle.Fill,
                //Padding = new Padding(10),
                //Margin = new Padding(0,0,0,2),
            //    AutoSize = true,
             //   AutoSizeMode = AutoSizeMode.GrowAndShrink,
                //RowCount = 1,
                ColumnCount = 1,
                GrowStyle = TableLayoutPanelGrowStyle.AddRows,
               //  RowStyles = { new RowStyle(SizeType.AutoSize) },

            }; 
            tableLayout.RowCount = 0;
          //  tableLayout.GrowStyle = TableLayoutPanelGrowStyle.AddRows;

            foreach (var prop in props)
            {
                var stackPanel = new SW.Controls.StackPanel
                {
                    Orientation = SW.Controls.Orientation.Horizontal,
                  //  Margin = new SW.Thickness(8)

                };


                var labeledPanel = CreateLabeledPanel(prop, prefs);
                stackPanel.Children.Add(labeledPanel);

                float scaleFactor;
                using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
                {
                    scaleFactor = g.DpiX / 96f; // 96 is default DPI
                }
                int heightPanel = (int)(85 * scaleFactor);

                var elementHost = new ElementHost
                {
                    Dock = DockStyle.Fill,
                    Child = stackPanel,
                    Margin = new Padding(0,0,0,16),
                    Height = heightPanel,
                    Padding = new Padding(0, 8, 0, 8),
                };

                // add to new row
                int rowIndex = tableLayout.RowCount++;
                tableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                tableLayout.Controls.Add(elementHost, 0, rowIndex);


            }

            // === Wrap TableLayoutPanel inside WindowsFormsHost ===
            var wfHost = new WindowsFormsHost
            {
                Child = tableLayout
            };


            scrollViewer.DataContext = prefs; // Set the DataContext to the preferences object for binding

            scrollViewer.Content = wfHost;

            return scrollViewer;
        }

        private static StackPanel CreateLabeledSlider(double min, double max, double initialValue = 0, double tickFrequency = 0)
        {
            var panel = new StackPanel
            {
                Orientation = SW.Controls.Orientation.Vertical
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
                HorizontalAlignment = SW.HorizontalAlignment.Left,
                Foreground = System.Windows.Media.Brushes.White
            };
            DockPanel.SetDock(minLabel, Dock.Left);

            var maxLabel = new TextBlock
            {
                Text = max.ToString(),
                HorizontalAlignment = SW.HorizontalAlignment.Right,
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

            var container = new Border
            {
                HorizontalAlignment = SW.HorizontalAlignment.Stretch,
                Margin = new Thickness(0, 4, 0, 4)
            };

            var grid = new Grid
            {
                HorizontalAlignment = SW.HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center
            };


            float scaleFactor;
            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
            {
                scaleFactor = g.DpiX / 96f; // 96 is default DPI
            }

            var scaledWidthColumn1 = (int)(600 / scaleFactor);
            var scaledWidthColumn2 = (int)(60 / scaleFactor);


            // COLUMN 0: labels
            var leftColumn = new ColumnDefinition { Width = new GridLength(scaledWidthColumn1) };
            grid.ColumnDefinitions.Add(leftColumn);

            // COLUMN 1: middle spacer (width will be set dynamically)
            var middleColumn = new ColumnDefinition { Width = new GridLength(scaledWidthColumn2) };
            grid.ColumnDefinitions.Add(middleColumn);

            // COLUMN 2: input control
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // LEFT: labels
            var stackPanel = new StackPanel
            {
                Orientation = System.Windows.Controls.Orientation.Vertical,
                HorizontalAlignment = SW.HorizontalAlignment.Stretch
            };

            float labelFontSize = 14f / scaleFactor;

            var displayAttribute = prop.GetAttribute<DisplayAttribute>();
            var label = new TextBlock
            {
                Text = displayAttribute?.ResourceType != null && !string.IsNullOrEmpty(displayAttribute.Name)
                    ? (displayAttribute.ResourceType.GetProperty(displayAttribute.Name, BindingFlags.Static | BindingFlags.Public)?
                        .GetValue(null, null) as string ?? displayAttribute.Name)
                    : displayAttribute?.Name ?? "MISSING DESCRIPTION",
                HorizontalAlignment = SW.HorizontalAlignment.Stretch,
                FontFamily = new System.Windows.Media.FontFamily("Montserrat"),
                FontSize = 14,
                FontWeight = FontWeights.DemiBold,
                Foreground = System.Windows.Media.Brushes.White,
                TextWrapping = TextWrapping.WrapWithOverflow,
                Padding = new Thickness(left: 10, top: 0, right: 0, bottom: 0),
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
                    Padding = new Thickness(left: 14, top: 0, right: 0, bottom: 0),
                };
                stackPanel.Children.Add(description);
            }

            Grid.SetColumn(stackPanel, 0);
            grid.Children.Add(stackPanel);

            // MIDDLE: spacer
            var spacer = new Border
            {
                HorizontalAlignment = SW.HorizontalAlignment.Stretch
            };
            Grid.SetColumn(spacer, 1);
            grid.Children.Add(spacer);

            // RIGHT: input control
            var binding = new System.Windows.Data.Binding(prop.Name)
            {
                Source = settingsInstance,
                Mode = BindingMode.TwoWay
            };

            FrameworkElement inputControl;

            if (prop.Property.PropertyType == typeof(string) &&
                prop.GetAttribute<UIHintAttribute>()?.UIHint == "PinEntry")
            {
                inputControl = new PasswordBox
                {
                    MaxLength = 5,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = SW.HorizontalAlignment.Stretch,
                    MinWidth = scaledWidthColumn1/2,
                    Padding = new Thickness(200, 0, 0, 0)
                };
            }
            else if (prop.Property.PropertyType == typeof(bool))
            {
                bool initialState = value is bool b && b;

                var toggleContainer = new StackPanel
                {
                    Orientation = System.Windows.Controls.Orientation.Horizontal,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = SW.HorizontalAlignment.Right
                };

                var labelToggle = new TextBlock
                {
                    Text = initialState ? "On" : "Off",
                    FontFamily = new System.Windows.Media.FontFamily("Montserrat"),
                    Foreground = System.Windows.Media.Brushes.White,
                    FontSize = labelFontSize,
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
                toggle.SetBinding(ToggleSwitch.IsOnProperty, binding);
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
                    HorizontalAlignment = SW.HorizontalAlignment.Right
                };
                tb.SetBinding(System.Windows.Controls.TextBox.TextProperty, binding);
                inputControl = tb;
            }
            else if (prop.Property.PropertyType == typeof(int))
            {
                RangeAttribute range = prop.GetAttribute<RangeAttribute>() ?? new RangeAttribute(0, 25);

                var sliderStack = CreateLabeledSlider((int)range.Minimum, (int)range.Maximum, value is int i ? i : 0, 1);
                sliderStack.VerticalAlignment = VerticalAlignment.Center;
                sliderStack.HorizontalAlignment =  SW.HorizontalAlignment.Right;
                var sliderStackscaledWidth = (int)(300 / scaleFactor);
                sliderStack.Width = sliderStackscaledWidth;

                var slider = sliderStack?.Children?.OfType<Slider>().FirstOrDefault();
                slider?.SetBinding(Slider.ValueProperty, binding);
                slider?.SetBinding(Slider.ToolTipProperty, binding);

                inputControl = sliderStack;
            }
            else
            {
                inputControl = new TextBlock
                {
                    Text = $"Unsupported: {prop.Property.PropertyType.Name}",
                    Foreground = System.Windows.Media.Brushes.Gray,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment =  SW.HorizontalAlignment.Right
                };
            }

            if (inputControl is StackPanel sp && sp.Children.OfType<ToggleSwitch>().Any())
            {
                middleColumn.Width = new GridLength(scaledWidthColumn2 * 4.7); // toggle switch
            }

            Grid.SetColumn(inputControl, 2);
            grid.Children.Add(inputControl);
            container.Child = grid;


            //  var wpfWindow = Window.GetWindow(container);
            // if (wpfWindow != null)
            // {
            //  var interopHelper = new WindowInteropHelper(wpfWindow);
            //  var hwnd = interopHelper.Handle;

            //  var winFormsControl = SW.Forms.Control.FromHandle(hwnd);
            //   var parentForm = winFormsControl?.FindForm();
            // }


            // inputControl.VerticalAlignment = VerticalAlignment.Center;
            //   inputControl.HorizontalAlignment = SW.HorizontalAlignment.Right;



            /*
            container.SizeChanged += (s, e) =>
            {
                double availableWidth = container.ActualWidth;
                double col0Width = stackPanel.ActualWidth;
                double col2Width = inputControl.ActualWidth;

                double spacerWidth = Math.Max(availableWidth - col0Width - col2Width - 1440, 0);

             



                spacer.Width = spacerWidth;
            };*/


            /*void AdjustColumns()
            {
                var mainForm = SW.Forms.Application.OpenForms.Cast<Form>().FirstOrDefault();
                if (mainForm == null) return;

                bool isToggle = inputControl is StackPanel sp && sp.Children.OfType<ToggleSwitch>().Any();

                if (mainForm.WindowState == FormWindowState.Maximized)
                {
                    // Shrink a little to leave some margin
                    middleColumn.Width = new GridLength(isToggle ? 280 : 60);
                    leftColumn.Width = new GridLength(1060); // original 1100 minus 40
                }
                else
                {
                    // Normal sizing
                    middleColumn.Width = new GridLength(isToggle ? 280 : 60);
                    leftColumn.Width = new GridLength(1100);
                }
            }

            // Call once in case form is already shown
            AdjustColumns();

            /*
            // Hook resizing so it updates dynamically
            var formRef = SW.Forms.Application.OpenForms.Cast<Form>().FirstOrDefault();
            if (formRef != null)
            {
                formRef.SizeChanged += (s, e) => AdjustColumns();
                formRef.ResizeEnd += (s, e) => AdjustColumns();
            }
            */
            return container;
        }
    }
}