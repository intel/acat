using ACAT.Core.PreferencesManagement;
using ACAT.Core.PreferencesManagement.UI;
using ACAT.Core.Utility;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace ACATConfigNext.UserControls
{
    public class SettingsPanel : UserControl
    {
        protected Panel basePanel;

        protected PreferencesBase _prefs;

        public bool Save()
        {
            return _prefs.Save();
        }

        public SettingsPanel(Action<UserControl, string> showPanel, PreferencesBase prefs, PropertyChangedEventHandler settingsChangedHandler)
        {
            basePanel = new Panel()
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                AutoScroll = false,
            };

            _prefs = prefs;
            _prefs.PropertyChanged += settingsChangedHandler;

            this.Disposed += SettingsPanel_Disposed;
            var builder = new SettingsPanelBuilder();

            FrameworkElement scrollViewer = builder.CreateScrollViewer(prefs);

            var host = new ElementHost
            {
                Dock = DockStyle.Fill,
                Child = scrollViewer,
            };
            basePanel.Controls.Add(host);

            Controls.Add(basePanel);
        }

        private void SettingsPanel_Disposed(object sender, EventArgs e)
        {
            if (_prefs != null)
            {
                _prefs.PropertyChanged -= SettingsPanel_Disposed;
                _prefs = null;
            }
            Disposed -= SettingsPanel_Disposed;

            if (basePanel != null)
            {
                basePanel.Dispose();
                basePanel = null;
            }
        }
    }
}
