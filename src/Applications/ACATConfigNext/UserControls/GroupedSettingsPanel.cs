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
    internal class GroupedSettingsPanel : SettingsPanel
    {
        public GroupedSettingsPanel(Action<UserControl, string> showPanel, IPreferences prefs) : 
            base(showPanel, prefs)
        {
            List<String> settings = new List<String>()
            {
                "Camera Actuator",
                "Keyboard Actuator",
                "Custom Actuator",
                "BCI Actuator"
            };

            SuspendLayout();
            basePanel.SuspendLayout();

            foreach (var setting in settings)
            {

                var panel = new TableLayoutPanel
                {
                    BackColor = Color.DarkGray,
                    Name = setting,
                    Dock = DockStyle.Top,
                    Padding = new Padding(10),
                    Margin = new Padding(10),
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    RowCount = settings.Count,
                    ColumnCount = 1,
                    GrowStyle = TableLayoutPanelGrowStyle.AddRows
                };

                AddPanelClickEvent(panel, showPanel, prefs, setting);

                var label = new Label
                {
                    Text = setting,
                    AutoSize = true,
                    Font = new Font("Montserrat", 18),
                    ForeColor = Color.White,
                };
                AddPanelClickEvent(label, showPanel, prefs, setting);

                panel.Controls.Add(label);
                //basePanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                basePanel.Controls.Add(panel);
            }

            //basePanel.Controls.Add(groupedItems);
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
