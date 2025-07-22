using ACAT.Lib.Core.PreferencesManagement;
using ACAT.Lib.Core.PreferencesManagement.UI;
using ACAT.Lib.Core.Utility;
using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace ACATConfigNext.UserControls
{
    public class SettingsPanelDescriptor : Attribute, IDescriptor
    {
        public string Category { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }

        public Guid Id { get; set; }

        public bool HasSettings { get; set; }

        public SettingsPanelDescriptor(string category, string description, string name, bool hasSettings)
        {
            Category = category;
            Description = description;
            Name = name;
            Id = Guid.Empty;
            HasSettings = hasSettings;
        }
    }

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
