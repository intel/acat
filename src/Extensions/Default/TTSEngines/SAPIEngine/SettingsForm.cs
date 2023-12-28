////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// SettingsForm.cs
//
// Configure TTS Engine settings form
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PreferencesManagement;
using ACAT.Lib.Core.UserManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Speech.Synthesis;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.TTSEngines.SAPIEngine
{
    /// <summary>
    /// Displays the settings for the SAPI Text to speech engine
    /// </summary>
    public partial class SettingsForm : Form
    {
        /// <summary>
        /// Aspect ratio of form at design time
        /// </summary>
        private float _designTimeAspectRatio = 0.0f;

        /// <summary>
        /// Did the user change anything?
        /// </summary>
        private bool _dirty;

        /// <summary>
        /// Has first call to OnClientSizeChanged been made?
        /// </summary>
        private bool _firstClientChangedCall = true;

        /// <summary>
        /// The settings object
        /// </summary>
        private SAPISettings _settings;

        /// <summary>
        /// The Microsoft speech synthesizer object
        /// </summary>
        private SpeechSynthesizer _speechSynthesizer;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        public SettingsForm()
        {
            InitializeComponent();

            Load += SettingsForm_Load;
            FormClosing += SettingsForm_FormClosing;
            comboBoxSelectVoice.SelectedIndexChanged += ComboBoxSelectVoice_SelectedIndexChanged;
        }

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
        /// User canceled out.  Confirm and quit
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (_dirty)
            {
                if (!confirm("Changes not saved. Quit anyway?"))
                {
                    return;
                }
            }

            Close();
        }

        /// <summary>
        /// User clicked the Defaults button.  Restore defaults and
        /// update the UI
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonDefaults_Click(object sender, EventArgs e)
        {
            if (!confirm("Restore default settings?"))
            {
                return;
            }

            var settings = new SAPISettings();

            updateUI(settings);
        }

        /// <summary>
        /// User clicked OK. Confirm if settings needs to be saved,
        /// and then save them and close window
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (!confirm("Save settings?"))
            {
                return;
            }

            if (comboBoxSelectVoice.Items.Count > 0 &&
                    comboBoxSelectVoice.SelectedItem != null)
            {
                var voice = comboBoxSelectVoice.SelectedItem as String;
                if (!String.IsNullOrWhiteSpace(voice))
                {
                    _settings.Voice = voice;
                }
                else
                {
                    bool result = ConfirmBox.ShowDialog("Must select voice", null, false);
                    return;
                }
            }

            _settings.Save();

            if (Windows.GetOSVersion() == Windows.WindowsVersion.Win7)
            {
                bool result = ConfirmBox.ShowDialog("You are running Windows 7. Text to speech voice selection may not work", null, false);
            }

            Close();
        }

        /// <summary>
        /// User clicked on the Settings button.  Show other
        /// settings for the TTS engine
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonSettings_Click(object sender, EventArgs e)
        {
            this.TopMost = false;
            Hide();

            PreferencesEditForm2 form = new PreferencesEditForm2
            {
                Title = Text,
                SupportsPreferencesObj = new SAPIEngine()
            };

            form.TopMost = true;
            form.ShowDialog();

            Show();

            this.TopMost = true;

            _settings = SAPISettings.Load();
        }

        /// <summary>
        /// Voice checkbox toggled. Set ui state
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void checkBoxSelectVoice_CheckedChanged(object sender, EventArgs e)
        {
            _dirty = true;
            setComboBoxStates();
        }

        /// <summary>
        /// User selected something in the gender combo box
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void ComboBoxGender_SelectedIndexChanged(object sender, EventArgs e)
        {
            _dirty = true;
        }

        /// <summary>
        /// User selected something in the voice combo box
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void ComboBoxSelectVoice_SelectedIndexChanged(object sender, EventArgs e)
        {
            _dirty = true;
        }

        /// <summary>
        /// Displays a yes/no message box with a prompt
        /// </summary>
        /// <param name="prompt">prmopt to display</param>
        /// <returns>true if yes</returns>
        private bool confirm(String prompt)
        {
            this.TopMost = false;
            Hide();

            bool result = ConfirmBox.ShowDialog(prompt.ToString(), null, false);

            Show();
            this.TopMost = true;

            return result;
        }

        private void loadVoices(bool currentCulture = true)
        {
            comboBoxSelectVoice.Items.Clear();

            IEnumerable<InstalledVoice> ins = currentCulture
                ? _speechSynthesizer.GetInstalledVoices(CultureInfo.DefaultThreadCurrentUICulture)
                : _speechSynthesizer.GetInstalledVoices();

            foreach (InstalledVoice iv in ins)
            {
                String voiceName = iv.VoiceInfo.Name.ToString();
                String gender = " ";
                if (voiceName.ToUpper().Contains("DAVID"))
                {
                    gender += "(Male)";
                }
                else if (voiceName.ToUpper().Contains("ZIRA"))
                {
                    gender += "(Female)";
                }
                voiceName += gender;
                comboBoxSelectVoice.Items.Add(voiceName);
            }
        }

        /// <summary>
        /// Update the enabled states of the combo boxes
        /// </summary>
        private void setComboBoxStates()
        {
        }

        /// <summary>
        /// Dispose resources
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _speechSynthesizer.Dispose();
        }

        /// <summary>
        /// Form loader.  Init settings and update the ui
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void SettingsForm_Load(object sender, EventArgs e)
        {
            float currentAspectRatio = (float)ClientSize.Height / ClientSize.Width;

            if (_designTimeAspectRatio != 0.0f && currentAspectRatio != _designTimeAspectRatio)
            {
                ClientSize = new System.Drawing.Size(ClientSize.Width, (int)(_designTimeAspectRatio * ClientSize.Width));
            }

            TopMost = false;
            TopMost = true;

            CenterToScreen();

            _settings = SAPIEngine.SAPISettings;
            if (String.IsNullOrEmpty(SAPISettings.PreferencesFilePath))
            {
                SAPISettings.PreferencesFilePath = UserManager.GetFullPath(SAPIEngine.SettingsFileName);
            }

            if (_settings == null)
            {
                _settings = SAPISettings.Load();
            }

            _speechSynthesizer = new SpeechSynthesizer();

            loadVoices(false);

            comboBoxSelectVoice.DropDownStyle = ComboBoxStyle.DropDownList;

            updateUI(_settings);

            _dirty = false;
        }

        /// <summary>
        /// Converts a string representation of the gender to
        /// the enum. Returns NotSet if string is invalid
        /// </summary>
        /// <param name="gender">string to convert</param>
        /// <returns>enum value</returns>
        private VoiceGender stringToVoiceGender(string gender)
        {
            VoiceGender voiceGender;

            if (Enum.TryParse(gender, out voiceGender))
            {
                return voiceGender;
            }

            return VoiceGender.NotSet;
        }

        /// <summary>
        /// Updates the UI with values from the settings object
        /// </summary>
        /// <param name="settings">the TTS settings object</param>
        private void updateUI(SAPISettings settings)
        {
            int selectedIndex = -1;

            if (!String.IsNullOrEmpty(settings.Voice))
            {
                for (int index = 0; index < comboBoxSelectVoice.Items.Count; index++)
                {
                    var voice = comboBoxSelectVoice.Items[index] as String;
                    if (String.Compare(settings.Voice, voice, true) == 0)
                    {
                        selectedIndex = index;
                        break;
                    }
                }
            }

            if (comboBoxSelectVoice.Items.Count > 0)
            {
                comboBoxSelectVoice.SelectedIndex = (selectedIndex >= 0) ? selectedIndex : 0;
            }

            setComboBoxStates();
        }
    }
}