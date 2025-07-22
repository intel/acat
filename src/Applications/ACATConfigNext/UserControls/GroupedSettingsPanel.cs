using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.PreferencesManagement;
using ACATConfigNext.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACATConfigNext.UserControls
{
    internal class GroupedSettingsPanel : UserControl
    {
        private TableLayoutPanel basePanel;
        public GroupedSettingsPanel(Action<UserControl, string> showPanel, IEnumerable<IExtension> settings)
        {
            basePanel = new TableLayoutPanel()
            {
                BackColor = Color.Purple,
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Margin = new Padding(10),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                GrowStyle = TableLayoutPanelGrowStyle.AddRows,
                RowCount = settings.Count(),
                ColumnCount = 1
            };

            SuspendLayout();
            basePanel.SuspendLayout();

            foreach (var setting in settings)
            {
                var panel = new TableLayoutPanel
                {
                    BackColor = Color.DarkGray,
                    Name = setting.Descriptor.Name,
                    Dock = DockStyle.Top,
                    Padding = new Padding(10),
                    Margin = new Padding(10),
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    RowCount = 1,
                    ColumnCount = 1,
                    GrowStyle = TableLayoutPanelGrowStyle.AddRows
                };

                //AddPanelClickEvent(panel, showPanel, prefs, setting);

                var label = new Label
                {
                    Text = setting.Descriptor.Name,
                    AutoSize = true,
                    Font = new Font("Montserrat", 18),
                    ForeColor = Color.White,
                };
                //AddPanelClickEvent(label, showPanel, prefs, setting);

                panel.Controls.Add(label);
                basePanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                basePanel.Controls.Add(panel);
            }
            Controls.Add(basePanel);
            basePanel.ResumeLayout();
            ResumeLayout(true);
        }

        private static void AddPanelClickEvent(Control control, Action<UserControl, string> showPanel, IPreferences prefs, string setting)
        {
            control.Click += (s, e) =>
            {
                showPanel(new SettingsPanel(showPanel, prefs), setting);
            };
        }
    }
}
