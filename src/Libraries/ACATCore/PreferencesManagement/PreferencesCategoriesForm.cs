////////////////////////////////////////////////////////////////////////////
// <copyright file="PreferencesCategoriesForm.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2017 Intel Corporation 
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.ThemeManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace ACAT.Lib.Core.PreferencesManagement
{
    /// <summary>
    /// Form that displays a list of ACAT modules (or categories) such as Agents,
    /// Word Prediction, Text To Sepeech.  User can select
    /// a category to set preferences for that module
    /// </summary>
    public partial class PreferencesCategoriesForm : Form
    {
        /// <summary>
        /// Aspect ratio of form at design time
        /// </summary>
        private float _designTimeAspectRatio = 0.0f;

        /// <summary>
        /// Has first call to OnClientSizeChanged been made?
        /// </summary>
        private bool _firstClientChangedCall = true;

        /// <summary>
        /// Title of message box
        /// </summary>
        private String _title = "ACAT Config";

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public PreferencesCategoriesForm()
        {
            InitializeComponent();
            Load += PreferencesCategoriesForm_Load;
        }

        /// <summary>
        /// Delegate for the event triggered when the user changes
        /// the default language
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="arg">event args</param>
        public delegate void NotifyPreferencesLanguageChanged(object sender, PreferencesLanguageChanged arg);

        /// Delegate for the event triggered when the user changes
        /// the default theme
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="arg">event args</param>
        public delegate void NotifyThemeChanged(object sender, String selectedTheme);

        /// <summary>
        /// Event raised when the default language changes
        /// </summary>
        public event NotifyPreferencesLanguageChanged EvtLanguageChanged;

        /// <summary>
        /// Event raised when the default theme is changed
        /// </summary>
        public event NotifyThemeChanged EvtThemeChanged;

        /// <summary>
        /// Client size changed
        /// </summary>
        /// <param name="e">event args</param>
        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);
            if (_firstClientChangedCall)
            {
                _designTimeAspectRatio = (float)ClientSize.Height / ClientSize.Width;
                _firstClientChangedCall = false;
            }
        }

        /// <summary>
        /// Handler for the Actuators category
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonActuators_Click(object sender, EventArgs e)
        {
            Hide();

            if (Context.AppActuatorManager.LoadExtensions(Context.ExtensionDirs, true))
            {
                Context.AppActuatorManager.ShowPreferences();
            }

            Show();
        }

        /// <summary>
        /// Handler for the Application Agents category
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonApplications_Click(object sender, EventArgs e)
        {
            Hide();

            if (Context.AppAgentMgr.LoadExtensions(Context.ExtensionDirs))
            {
                Context.AppAgentMgr.ShowPreferencesAppAgents();
            }
            else
            {
                showMessage("Error loading extensions for ACAT Agents");
            }

            Show();
        }

        /// <summary>
        /// Closes the dialog
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Event handler to change the theme (color scheme)
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonColorScheme_Click(object sender, EventArgs e)
        {
            Hide();

            var theme = ThemeSelectForm.SelectTheme();
            if (!String.IsNullOrEmpty(theme))
            {
                if (EvtThemeChanged != null)
                {
                    EvtThemeChanged(this, theme);
                }
            }

            Show();
        }

        /// <summary>
        /// Displays general preferences
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonGeneral_Click(object sender, EventArgs e)
        {
            var generalSettingsCategory = new GeneralSettingsCategory();

            Hide();
            var form = new PreferencesEditForm { SupportsPreferencesObj = generalSettingsCategory, Title = "General Settings" };
            form.ShowDialog();
            Show();
        }

        /// <summary>
        /// Handler for the SpellChecker category
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonLanguage_Click(object sender, EventArgs e)
        {
            bool isDefault = false;

            var installedLanguages = ResourceUtils.EnumerateInstalledLanguages();
            if (installedLanguages.Count() > 1)
            {
                Hide();
                var cultureInfo = LanguageSelectForm.SelectLanguage();
                Show();
                if (cultureInfo == null)
                {
                    ResourceUtils.SetEnglishCulture();
                    cultureInfo = CultureInfo.DefaultThreadCurrentUICulture;
                }
                else
                {
                    ResourceUtils.SetCulture(cultureInfo.Name);
                    isDefault = LanguageSelectForm.IsDefault;
                }

                if (EvtLanguageChanged != null)
                {
                    EvtLanguageChanged(this, new PreferencesLanguageChanged(cultureInfo, isDefault));
                }
            }
        }

        /// <summary>
        /// Handler for the spell checker category
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonSpellCheckers_Click(object sender, EventArgs e)
        {
            Hide();

            if (Context.AppSpellCheckManager.LoadExtensions(Context.ExtensionDirs))
            {
                Context.AppSpellCheckManager.ShowPreferences();
            }
            else
            {
                showMessage("Error loading SpellChecker extensions");
            }

            Show();
        }

        private void buttonTools_Click(object sender, EventArgs e)
        {
            Hide();

            if (Context.AppAgentMgr.LoadExtensions(Context.ExtensionDirs))
            {
                Context.AppAgentMgr.ShowPreferencesFunctionalAgents();
            }
            else
            {
                showMessage("Error loading extensions for ACAT tools");
            }

            Show();

        }

        /// <summary>
        /// Handler for the Text-to-speech category
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonTTS_Click(object sender, EventArgs e)
        {
            Hide();

            if (Context.AppTTSManager.LoadExtensions(Context.ExtensionDirs))
            {
                Context.AppTTSManager.ShowPreferences();
            }
            else
            {
                showMessage("Error loading Text-to-speech extensions");
            }

            Show();
        }

        /// <summary>
        /// Handler for the Word Predictors category
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonWordPredictors_Click(object sender, EventArgs e)
        {
            Hide();

            if (Context.AppWordPredictionManager.LoadExtensions(Context.ExtensionDirs))
            {
                Context.AppWordPredictionManager.ShowPreferences();
            }
            else
            {
                showMessage("Error loading Word Prediction extensions");
            }

            Show();
        }

        /// <summary>
        /// Form loader handler
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void PreferencesCategoriesForm_Load(object sender, EventArgs e)
        {
            float currentAspectRatio = (float)ClientSize.Height / ClientSize.Width;

            if (_designTimeAspectRatio != 0.0f && currentAspectRatio != _designTimeAspectRatio)
            {
                ClientSize = new System.Drawing.Size(ClientSize.Width, (int)(_designTimeAspectRatio * ClientSize.Width));
            }

            var installedLanguages = ResourceUtils.EnumerateInstalledLanguages();
            buttonLanguage.Enabled = (installedLanguages.Count() > 1);

            TopMost = false;
            TopMost = true;

            Activate();

            CenterToScreen();
        }

        /// <summary>
        /// Displays the message in a MessageBox
        /// </summary>
        /// <param name="message">message to display</param>
        private void showMessage(String message)
        {
            MessageBox.Show(message, _title, MessageBoxButtons.OK);
        }

        /// <summary>
        /// Event args for the event raised to indicate that
        /// the language changed
        /// </summary>
        public class PreferencesLanguageChanged
        {
            public CultureInfo CI;
            public bool IsDefault;

            public PreferencesLanguageChanged(CultureInfo ci, bool isDefault)
            {
                CI = ci;
                IsDefault = isDefault;
            }
        }

        /// <summary>
        /// Class to handle General settings of ACAT
        /// </summary>
        private class GeneralSettingsCategory : ISupportsPreferences
        {
            /// <summary>
            /// Gets whether this supports a custom settings dialog
            /// </summary>
            public bool SupportsPreferencesDialog
            {
                get { return false; }
            }

            /// <summary>
            /// Returns the default preferences (factory settings)
            /// </summary>
            /// <returns></returns>
            public IPreferences GetDefaultPreferences()
            {
                return CoreGlobals.AppDefaultPreferences;
            }

            /// <summary>
            /// Returns the current values of general preferences
            /// </summary>
            /// <returns></returns>
            public IPreferences GetPreferences()
            {
                return CoreGlobals.AppPreferences;
            }

            /// <summary>
            /// Shows the preferences dialog
            /// </summary>
            /// <returns>true on success</returns>
            public bool ShowPreferencesDialog()
            {
                return true;
            }
        }
    }
}