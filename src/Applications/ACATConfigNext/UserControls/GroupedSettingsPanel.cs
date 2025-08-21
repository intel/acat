using ACAT.Core.Utility;
using ACAT.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using ACAT.Core.PreferencesManagement;
using System.ComponentModel;

using MMC = MahApps.Metro.Controls;
using SW = System.Windows;

using System.Windows.Forms.Integration;

namespace ACATConfigNext.UserControls
{
    internal class GroupedSettingsPanel : UserControl
    {
        private TableLayoutPanel basePanel;
        public GroupedSettingsPanel(Action<UserControl, string> showPanel, IEnumerable<IExtension> acat_extensions, PropertyChangedEventHandler settingsChangedHandler)
        {
            basePanel = new TableLayoutPanel()
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Margin = new Padding(10),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                GrowStyle = TableLayoutPanelGrowStyle.AddRows,
                RowCount = acat_extensions.Count(),
                RowStyles = { new RowStyle(SizeType.AutoSize) },
                ColumnCount = 1
            };

            SuspendLayout();
            basePanel.SuspendLayout();


            foreach (var extension in acat_extensions)
            {
                var panel = new TableLayoutPanel
                {
                    BackColor = Color.FromArgb(74, 75, 93),//Gray
                    Name = extension.Descriptor.Name,
                    Dock = DockStyle.Top,
                    Padding = new Padding(10),
                    Margin = new Padding(10),
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    RowCount = 1,
                    ColumnCount = 3,
                    GrowStyle = TableLayoutPanelGrowStyle.AddRows,
                    Tag = extension
                   
                };
                panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); 
                panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                AddPanelClickEvent(panel, showPanel, settingsChangedHandler);

                /*var binding = new Binding(prop.Name)
                {
                    Source = settingsInstance,
                    Mode = BindingMode.TwoWay
                };*/

                // var value = prop.Property.GetValue(settingsInstance);

                // bool initialState = value is bool b && b;
                bool initialState = true;
                var toggleContainer = new SW.Controls.StackPanel
                {
                    Orientation = SW.Controls.Orientation.Horizontal,
                    VerticalAlignment = SW.VerticalAlignment.Center,
                    HorizontalAlignment = SW.HorizontalAlignment.Right
                };

                var label1 = new Label
                {
                    Text = extension.Descriptor.Name,
                    AutoSize = true,
                    Font = new Font("Montserrat", 18),
                    ForeColor = Color.White,
                    Anchor = AnchorStyles.Left,
                };

                var labeltoogle = new SW.Controls.TextBlock
                {
                    Text = initialState ? "Enable" : "Enable", // initial text
                    FontFamily = new System.Windows.Media.FontFamily("Montserrat"),
                    Foreground = System.Windows.Media.Brushes.White,
                    FontSize = 14,
                    VerticalAlignment = SW.VerticalAlignment.Center,
                    Margin = new SW.Thickness(0, 0, 8, 0) // spacing before toggle
                };

                var toggle = new MMC.ToggleSwitch
                {
                    // OnContent = "Yes",
                    // OffContent = "No",
                    IsOn = initialState,
                    OnContent = null,
                    OffContent = null,
                    VerticalAlignment = SW.VerticalAlignment.Center,
                };
               // toggle.SetBinding(MMC.ToggleSwitch.IsOnProperty, binding);

                toggle.Toggled += (s, e) =>
                {
                    labeltoogle.Text = toggle.IsOn ? "Enable" : "Enable";
                };

                toggleContainer.Children.Add(labeltoogle);
                toggleContainer.Children.Add(toggle);
                //inputControl = toggleContainer;

                var host = new ElementHost
                {
                    Dock = DockStyle.Fill,
                    Child = toggleContainer
                };
                AddPanelClickEvent(label1, showPanel, settingsChangedHandler);

                var label2 = new Label
                {
                    Text = ">",
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Montserrat", 11, FontStyle.Bold),
                    ForeColor = Color.White,
                    Margin = new Padding(5),
                    Padding = new Padding(6),
                    AutoSize = true,
                    Anchor = AnchorStyles.Right,
                    BackColor = Color.Transparent
                };
                AddPanelClickEvent(label2, showPanel, settingsChangedHandler);

                panel.Controls.Add(label1, 0, 0); // Column 0

                if (acat_extensions.Count() >= 4)
                {
                    panel.Controls.Add(host, 1, 0); // Column 1
                }
                panel.Controls.Add(label2, 2, 0); // Column 2

                basePanel.Controls.Add(panel);
            }
            Controls.Add(basePanel);
            basePanel.ResumeLayout();
            ResumeLayout(true);
        }

        private static void AddPanelClickEvent(Control control, Action<UserControl, string> showPanel, PropertyChangedEventHandler settingsChangedHandler)
        {
            control.Click += (s, e) =>
            {
                Log.Debug("Clicked on control: " + control.Name);
                Control clickedControl = s as Control;

                while (clickedControl != null && clickedControl is not TableLayoutPanel)
                {
                    clickedControl = clickedControl.Parent;
                }

                if (clickedControl is TableLayoutPanel panel)
                {
                    var extension = panel.Tag as IExtension;
                    if (extension != null)
                    {
                        Log.Debug("Showing acat_extensions for: " + extension.Descriptor.Name);
                        MethodInfo method = extension.GetType().GetMethod("GetPreferences");

                        var prefs = method?.Invoke(extension, null) as PreferencesBase;
                        if (prefs != null)
                        {
                            showPanel?.Invoke(new SettingsPanel(showPanel, prefs, settingsChangedHandler), extension.Descriptor.Name);
                        }
                        else
                        {
                            method = extension.GetType().GetMethod("ShowPreferencesDialog");
                            if (method != null)
                            {
                                Log.Debug("Showing preferences dialog for: " + extension.Descriptor.Name);
                                method.Invoke(extension, null);
                            }
                            else
                            {
                                Log.Error("No preferences method found for extension: " + extension.Descriptor.Name);
                            }
                        }
                    }
                }
            };
        }
    }
}
