using ACAT.Core.PreferencesManagement;
using ACAT.Core.PreferencesManagement.UI;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace ACATConfigNext.UserControls
{
    public class SettingsPanel : UserControl
    {
        protected Panel basePanel;

        public SettingsPanel(Action<UserControl, string> showPanel, IPreferences prefs)
        {
            basePanel = new Panel()
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Margin = new Padding(10)
            };

            var builder = new SettingsPanelBuilder();

            var scrollViewer = builder.CreateScrollViewer(prefs);

            var host = new ElementHost
            {
                Dock = DockStyle.Fill,
                Child = scrollViewer,
            };

            basePanel.Controls.Add(host);
            Controls.Add(basePanel);
        }
    }
}
