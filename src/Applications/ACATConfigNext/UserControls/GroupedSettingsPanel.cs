using ACAT.Core.Utility;
using ACAT.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using ACAT.Core.PreferencesManagement;

namespace ACATConfigNext.UserControls
{
    internal class GroupedSettingsPanel : UserControl
    {
        private TableLayoutPanel basePanel;
        public GroupedSettingsPanel(Action<UserControl, string> showPanel, IEnumerable<IExtension> acat_extensions)
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
                    BackColor = Color.FromArgb(74, 75, 93),
                    Name = extension.Descriptor.Name,
                    Dock = DockStyle.Top,
                    Padding = new Padding(10),
                    Margin = new Padding(10),
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    RowCount = 1,
                    ColumnCount = 1,
                    GrowStyle = TableLayoutPanelGrowStyle.AddRows,
                    Tag = extension
                   
                };
                AddPanelClickEvent(panel, showPanel);

                var label = new Label
                {
                    Text = extension.Descriptor.Name,
                    AutoSize = true,
                    Font = new Font("Montserrat", 18),
                    ForeColor = Color.White,
                };
                AddPanelClickEvent(label, showPanel);

                panel.Controls.Add(label);
                basePanel.Controls.Add(panel);
            }
            Controls.Add(basePanel);
            basePanel.ResumeLayout();
            ResumeLayout(true);
        }

        private static void AddPanelClickEvent(Control control, Action<UserControl, string> showPanel)
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

                        var prefs = method?.Invoke(extension, null) as IPreferences;
                        if (prefs != null)
                        {
                            showPanel?.Invoke(new SettingsPanel(showPanel, prefs), extension.Descriptor.Name);
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
