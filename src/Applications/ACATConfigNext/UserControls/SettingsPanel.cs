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

            //basePanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            //basePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            var builder = new SettingsPanelBuilder();

            var scrollViewer = builder.CreateScrollViewer(prefs);

            var host = new ElementHost
            {
                Dock = DockStyle.Fill,
                Child = scrollViewer,
            };

            //foreach (var prop in props)
            //{
            //    var propPanel = builder.CreateLabeledPanel(prop, prefs);

            //    var host = new ElementHost
            //    {
            //        BackColor = Color.Blue,
            //        Child = propPanel,
            //        AutoSize = true,
            //        Margin = new Padding(10),
            //        Padding = new Padding(10),
            //        Dock = DockStyle.Top,
            //    };

            //    basePanel.Controls.Add(host);
            //}
            basePanel.Controls.Add(host);
            Controls.Add(basePanel);
        }
    }
}
