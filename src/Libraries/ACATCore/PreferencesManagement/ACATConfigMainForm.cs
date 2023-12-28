////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ACATConfigMainForm.cs
//
// Main form for ACAT Config application
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;

namespace ACAT.Lib.Core.PreferencesManagement
{
    /// <summary>
    /// Main form for ACAT Config application
    /// </summary>
    public partial class ACATConfigMainForm : Form
    {
        /// <summary>
        /// Holds information on high level preferenes category in list of checkboxes on left side of the screen
        /// </summary>
        public List<Tuple<String, CheckBox>> _configCategoryList;

        /// <summary>
        /// Keeps track of which preferences forms are currently displayed
        /// </summary>
        private static Stack<Form> _shownPreferenceForms;

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
        /// Form that displays a list of ACAT modules (or categories) such as Agents,
        /// Word Prediction, Text To Sepeech.  User can select
        /// a category to set preferences for that module
        /// </summary>
        public ACATConfigMainForm()
        {
            InitializeComponent();
            Load += ACATConfigMainForm_Load;
            Shown += ACATConfigMainForm_Shown;
            _configCategoryList = new List<Tuple<String, CheckBox>>();
            _configCategoryList.Add(new Tuple<String, CheckBox>(checkBoxCategoryGeneral.Text.ToString(), checkBoxCategoryGeneral));
            _configCategoryList.Add(new Tuple<String, CheckBox>(checkBoxCategoryActuators.Text.ToString(), checkBoxCategoryActuators));
            _configCategoryList.Add(new Tuple<String, CheckBox>(checkBoxCategoryTextToSpeech.Text.ToString(), checkBoxCategoryTextToSpeech));
            _configCategoryList.Add(new Tuple<String, CheckBox>(checkBoxCategoryWordPrediction.Text.ToString(), checkBoxCategoryWordPrediction));

            SetNewFormButtonHandlers(); // Reset buttons to default states and clear all event handlers

            _shownPreferenceForms = new Stack<Form>();

            Paint += (s, args) =>
            {
                // On first start, have first category (General) selected
                handleConfigCategorySelected(checkBoxCategoryGeneral, null);
            };
        }

        /// <summary>
        /// Delegate for the event triggered when the user changes
        /// the default language
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="arg">event args</param>
        public delegate void NotifyPreferencesLanguageChanged(object sender, PreferencesLanguageChanged arg);

        /// <summary>
        /// Delegate for the event triggered when the user clicks Defaults button
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="arg">event args</param>
        public delegate void NotifyResetToDefaultButtonClicked(object sender, EventArgs e);

        /// Delegate for the event triggered when the user changes
        /// the default theme
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="arg">event args</param>
        public delegate void NotifyThemeChanged(object sender, String selectedTheme);

        /// <summary>
        /// Delegate for the event triggered when the user clicks Wrap Text check box button
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="arg">event args</param>
        public delegate void NotifyWrapTextCheckBoxClicked(object sender, EventArgs e);

        /// <summary>
        /// Event raised when the default language changes
        /// </summary>
        public event NotifyPreferencesLanguageChanged EvtLanguageChanged;

        /// <summary>
        /// Event raised when the default theme is changed
        /// </summary>
        public event NotifyThemeChanged EvtThemeChanged;
        /// <summary>
        /// Event raised when the user clicks Defaults button
        /// </summary>
        private event NotifyResetToDefaultButtonClicked EvtResetToDefaultButtonClicked;
        /// <summary>
        /// Event raised when the user clicks Wrap Text check box button
        /// </summary>
        private event NotifyWrapTextCheckBoxClicked EvtWrapTextCheckBoxClicked;
        /// <summary>
        /// Sets handlers for button press events on this form (Done, Defaults, Wrap Text)
        /// </summary>
        /// <param name="handlerDoneButtonClicked"></param>
        /// <param name="handlerResetToDefaultButtonClicked"></param>
        /// <param name="handlerWrapTextBoxClicked"></param>
        /// <param name="wrapTextDefault"></param>
        public void SetNewFormButtonHandlers(NotifyResetToDefaultButtonClicked handlerResetToDefaultButtonClicked = null,
            NotifyWrapTextCheckBoxClicked handlerWrapTextBoxClicked = null, bool wrapTextDefault = false)
        {
            // Log.Debug("SetNewFormButtonHandlers");

            // Clear all existing event handlers
            EvtResetToDefaultButtonClicked = null;
            EvtWrapTextCheckBoxClicked = null;

            // Automatically disable "Defaults" and "Wrap Text" buttons - Set to visible later if provided button clcik handlers
            buttonResetToDefault.Visible = false;
            checkBoxWrapText.Visible = false;
            buttonSave.Visible = false;

            // Set handler for Reset to Default button
            if (handlerResetToDefaultButtonClicked != null)
            {
                buttonResetToDefault.Visible = true;
                EvtResetToDefaultButtonClicked += handlerResetToDefaultButtonClicked;
            }
            else
            {
                buttonResetToDefault.Visible = false;
            }

            // Set handler for Wrap Text button
            if (handlerWrapTextBoxClicked != null)
            {
                checkBoxWrapText.Visible = true;
                EvtWrapTextCheckBoxClicked += handlerWrapTextBoxClicked;
                checkBoxWrapText.Checked = wrapTextDefault;
            }
            else
            {
                checkBoxWrapText.Visible = false;
            }
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
        /// Form load handler
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void ACATConfigMainForm_Load(object sender, EventArgs e)
        {
            float currentAspectRatio = (float)ClientSize.Height / ClientSize.Width;

            if (_designTimeAspectRatio != 0.0f && currentAspectRatio != _designTimeAspectRatio)
            {
                ClientSize = new System.Drawing.Size(ClientSize.Width, (int)(_designTimeAspectRatio * ClientSize.Width));
            }

            var installedLanguages = ResourceUtils.EnumerateInstalledLanguages();

            TopMost = false;
            TopMost = true;

            Activate();

            CenterToScreen();
        }

        private void ACATConfigMainForm_Shown(object sender, EventArgs e)
        {
            ConfirmBoxSingleOption.ShowDialog("Please exercise caution when changing ACAT settings. Refer to the ACAT User Guide for help", "OK", this);
        }
        /// <summary>
        /// Handle Exit button press - save any changes if made, Close form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonExit_Click(object sender, EventArgs e)
        {
            // Prompt user if they want to save preferences changes made and close all forms up until this point
            savePrefCloseAllShownForms();

            // Exit ACATConfig
            Close();
        }

        /// <summary>
        /// Handle Reset to Defaults button press - call corresponding event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonResetToDefault_Click(object sender, EventArgs e)
        {
            if (EvtResetToDefaultButtonClicked != null)
            {
                EvtResetToDefaultButtonClicked(sender, e);
            }
        }

        /// <summary>
        /// Handle Save button press - prompt user to save edits if any made
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSave_Click(object sender, EventArgs e)
        {
            // Prompt user if they want to save preferences changes made
            // Do NOT close any forms if Save button pressed
            savePrefCloseAllShownForms(true);

            // Make Save button invisible again
            buttonSave.Visible = false;
        }

        /// <summary>
        /// Handle Wrap Text button press - call corresponding event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxWrapText_Click(object sender, EventArgs e)
        {
            if (EvtWrapTextCheckBoxClicked != null)
            {
                EvtWrapTextCheckBoxClicked(sender, e);
            }
        }

        /// <summary>
        /// User selected new config category (high level choices in left most list of check boxes - Ex: General, Actuators, Word Prediction. etc.)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handleConfigCategorySelected(object sender, EventArgs e)
        {
            String newConfigCategory = ((CheckBox)sender).Text.ToString();
            Log.Debug("handleConfigCategorySelected | Selected high level category: " + newConfigCategory);

            // Unselect all other checkboxes
            foreach (Tuple<String, CheckBox> tupleObj in _configCategoryList)
            {
                if (tupleObj.Item1.ToString() != newConfigCategory)
                {
                    tupleObj.Item2.Checked = false;
                    tupleObj.Item2.ForeColor = System.Drawing.Color.White;
                }
                else
                {
                    tupleObj.Item2.Checked = true;
                    tupleObj.Item2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
                }
            }

            // Prompt user if they want to save preferences changes made and close all forms up until this point
            savePrefCloseAllShownForms();

            // Get handle of control you will set to parent of preferences form
            IntPtr parentControlHandle = tableLayoutPanelConfigSettings.Handle;

            // Create new PreferencesCategorySelectForm or PreferencesEditForm depending on which high level category selected
            Form newPreferencesSelectForm = null;
            IntPtr formHandle = this.Handle;

            // Create PreferencesEditForm to show General settings and add it as child of table layout panel in this form
            if (newConfigCategory == "General")
            {
                var generalSettingsCategory = new GeneralSettingsCategory();
                newPreferencesSelectForm = new PreferencesEditForm { SupportsPreferencesObj = generalSettingsCategory, Title = "General Settings" };
                formHandle = ((PreferencesEditForm)newPreferencesSelectForm).Handle;

                NotifyWrapTextCheckBoxClicked HandlerWrapTextButtonClicked = ((PreferencesEditForm)newPreferencesSelectForm).checkBoxWrapText_CheckedChanged;
                bool wrapTextBoxChecked = ((PreferencesEditForm)newPreferencesSelectForm)._wrapText;

                // Set button config / handlers for General settings - Enable and set handlers for "Wrap Text", "Set Defaults" buttons
                NotifyResetToDefaultButtonClicked HandlerResetToDefaultsButtonClicked = ((PreferencesEditForm)newPreferencesSelectForm).buttonDefaults_Click; // General Settings category has Reset to Defaults functionality
                buttonResetToDefault.Visible = true;
                SetNewFormButtonHandlers(HandlerResetToDefaultsButtonClicked, HandlerWrapTextButtonClicked, wrapTextBoxChecked); // Set handlers / enable "Wrap Text", "Set Defaults" buttons
                ((PreferencesEditForm)newPreferencesSelectForm).EvtPreferencesChangeMade += handlePreferenceChangeMade; // Set handler for when preferences change mande
            }

            // Get PreferencesCategorySelectForm from ActuatorManager, or WordPredictionManager, or TTSManager and add it as child of table layout panel in this form
            else if (newConfigCategory == "Actuators")
            {
                if (Context.AppActuatorManager.LoadExtensions(Context.ExtensionDirs, true))
                {
                    newPreferencesSelectForm = (PreferencesCategorySelectForm)Context.AppActuatorManager.GetPreferencesSelectionForm(parentControlHandle);
                    formHandle = ((PreferencesCategorySelectForm)newPreferencesSelectForm).Handle;
                    SetNewFormButtonHandlers(); // Reset buttons to default states and clear all event handlers
                    ((PreferencesCategorySelectForm)newPreferencesSelectForm).EvtPreferencesChangeMade += handlePreferenceChangeMade; // Set handler for when preferences change mande
                }
                else
                {
                    showMessage("Error loading Actuator extensions");
                    return;
                }
            }
            else if (newConfigCategory == "Word Prediction")
            {
                if (Context.AppWordPredictionManager.LoadExtensions(Context.ExtensionDirs))
                {
                    newPreferencesSelectForm = (PreferencesCategorySelectForm)Context.AppWordPredictionManager.GetPreferencesSelectionForm(parentControlHandle);
                    formHandle = ((PreferencesCategorySelectForm)newPreferencesSelectForm).Handle;
                    SetNewFormButtonHandlers(); // Reset buttons to default states and clear all event handlers
                    ((PreferencesCategorySelectForm)newPreferencesSelectForm).EvtPreferencesChangeMade += handlePreferenceChangeMade; // Set handler for when preferences change mande
                }
                else
                {
                    showMessage("Error loading Word Prediction extensions");
                    return;
                }
            }
            else if (newConfigCategory == "Text to Speech")
            {
                if (Context.AppTTSManager.LoadExtensions(Context.ExtensionDirs))
                {
                    newPreferencesSelectForm = (PreferencesCategorySelectForm)Context.AppTTSManager.GetPreferencesSelectionForm(parentControlHandle);
                    formHandle = ((PreferencesCategorySelectForm)newPreferencesSelectForm).Handle;
                    SetNewFormButtonHandlers(); // Reset buttons to default states and clear all event handlers
                    ((PreferencesCategorySelectForm)newPreferencesSelectForm).EvtPreferencesChangeMade += handlePreferenceChangeMade; // Set handler for when preferences change mande
                }
                else
                {
                    showMessage("Error loading Text-to-speech extensions");
                    return;
                }
            }

            if (newPreferencesSelectForm == null)
            {
                showMessage("Error creating form for preferences cofiguration");
                return;
            }

            newPreferencesSelectForm.Dock = DockStyle.Fill;

            //// Change window style according to SetParent documentation
            //// https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setparent
            IntPtr style = (IntPtr)User32Interop.GetWindowLong(formHandle, -16);
            uint currentStyle = (uint)style.ToInt32();
            currentStyle &= ~User32Interop.WS_POPUP;
            currentStyle |= User32Interop.WS_CHILD;
            IntPtr newStyle = new IntPtr((int)currentStyle);
            User32Interop.SetWindowLong(formHandle, -16, newStyle);

            // Use lower level User32Interop function to set parent of PreferencesCategorySelectForm to table layout panel in this form
            User32Interop.SetParent(newPreferencesSelectForm.Handle, parentControlHandle);

            // Tie preferences save button press event to save preferences function of ActuatorManager, or WordPredictionManager, or TTSManager
            if (newConfigCategory == "Actuators")
            {
                ((PreferencesCategorySelectForm)newPreferencesSelectForm).EvtSavePreferences += Context.AppActuatorManager.SavePreferences;
            }
            else if (newConfigCategory == "Word Prediction")
            {
                ((PreferencesCategorySelectForm)newPreferencesSelectForm).EvtSavePreferences += Context.AppWordPredictionManager.SavePreferences;
            }
            else if (newConfigCategory == "Text to Speech")
            {
                ((PreferencesCategorySelectForm)newPreferencesSelectForm).EvtSavePreferences += Context.AppTTSManager.SavePreferences;
            }

            // Set handler for when new form is completed / exited
            newPreferencesSelectForm.FormClosed += handleDoneSettingPreferences;

            // Set handler for when "Setup" button pressed for preferences category
            if (newPreferencesSelectForm.GetType() == typeof(PreferencesCategorySelectForm))
            {
                ((PreferencesCategorySelectForm)newPreferencesSelectForm).EvtPreferencesCategorySelected += handlePreferencesCategorySelected;
            }

            // Push new preferences select form to stack
            _shownPreferenceForms.Push(newPreferencesSelectForm);

            // Show as regular form / control
            newPreferencesSelectForm.Show();
        }

        /// <summary>
        /// Handler for when completed setting preferences for a category (form closed)
        ///  Reset to default state - no buttons highlighted, no handlers attached, etc.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handleDoneSettingPreferences(object sender, FormClosedEventArgs e)
        {
            SetNewFormButtonHandlers(); // Reset buttons and clear all event handlers
        }

        /// <summary>
        /// Handler for when preferences change made - show Save button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handlePreferenceChangeMade()
        {
            buttonSave.Visible = true;
        }

        /// <summary>
        /// Handler for when preferences category is selected ("Setup" button in "Configure" column)
        /// Load custom preferences dialog called by ShowPreferencesDialog, or load default PreferencesEditForm2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="supportsPreferences"></param>
        private void handlePreferencesCategorySelected(object sender, ISupportsPreferences supportsPreferences)
        {
            try
            {
                // Log.Debug("\n\nhandlePreferencesCategorySelected | sender: " + sender.ToString()+ " | supportsPreferences.SupportsPreferencesDialog: " + supportsPreferences.SupportsPreferencesDialog.ToString());

                if (sender == null || ((Form)sender).IsDisposed)
                {
                    return;
                }

                Form senderForm = (Form)sender;

                // Show custom preferences dialog if available
                if (supportsPreferences.SupportsPreferencesDialog)
                {
                    senderForm.Hide();
                    supportsPreferences.ShowPreferencesDialog();
                    senderForm.Show();
                }

                // Otherwise show generic PreferencesEditForm
                else
                {
                    var prefs = supportsPreferences.GetPreferences();
                    if (prefs != null)
                    {
                        senderForm.Hide();

                        // Title passed to PreferencesEditForm is used to set SettingColumn.HeaderText
                        var title = (supportsPreferences is IExtension)
                            ? (supportsPreferences as IExtension).Descriptor.Name + " Settings"
                            : "Settings";

                        PreferencesEditForm newPreferencesEditForm = new PreferencesEditForm
                        {
                            Title = title,
                            SupportsPreferencesObj = supportsPreferences
                        };

                        // Get handle of control you will make parent of new form
                        IntPtr parentControlHandle = tableLayoutPanelConfigSettings.Handle;

                        newPreferencesEditForm.Dock = DockStyle.Fill;

                        //// Change window style according to SetParent documentation
                        //// https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setparent
                        IntPtr handle = newPreferencesEditForm.Handle;
                        IntPtr style = (IntPtr)User32Interop.GetWindowLong(handle, -16);
                        uint currentStyle = (uint)style.ToInt32();
                        currentStyle &= ~User32Interop.WS_POPUP;
                        currentStyle |= User32Interop.WS_CHILD;
                        IntPtr newStyle = new IntPtr((int)currentStyle);
                        User32Interop.SetWindowLong(handle, -16, newStyle);

                        // Use lower level User32Interop function to set parent of PreferencesEditForm to target control
                        User32Interop.SetParent(newPreferencesEditForm.Handle, parentControlHandle);

                        // Set handlers for button press events of main form
                        NotifyWrapTextCheckBoxClicked HandlerWrapTextButtonClicked = ((PreferencesEditForm)newPreferencesEditForm).checkBoxWrapText_CheckedChanged;
                        bool wrapTextBoxChecked = ((PreferencesEditForm)newPreferencesEditForm)._wrapText;

                        // Get whether preferences supports reset to default function
                        NotifyResetToDefaultButtonClicked HandlerResetToDefaultsButtonClicked = null;
                        IPreferences DefaultPreferences = supportsPreferences.GetDefaultPreferences();
                        if (DefaultPreferences == null)
                        {
                            // Log.Debug("handlePreferencesCategorySelected | DefaultPreferences == null");
                            buttonResetToDefault.Visible = false;
                        }
                        else
                        {
                            // Log.Debug("handlePreferencesCategorySelected | DefaultPreferences != null");
                            HandlerResetToDefaultsButtonClicked = ((PreferencesEditForm)newPreferencesEditForm).buttonDefaults_Click;
                            buttonResetToDefault.Visible = true;
                        }

                        SetNewFormButtonHandlers(HandlerResetToDefaultsButtonClicked, HandlerWrapTextButtonClicked, wrapTextBoxChecked);

                        // Set handler for when preferences setting change made
                        newPreferencesEditForm.EvtPreferencesChangeMade += handlePreferenceChangeMade;

                        // Set handler for when child form is closing
                        newPreferencesEditForm.FormClosing += handlePreferencesEditFormClosing;

                        // Push new preferences edit form to stack
                        _shownPreferenceForms.Push(newPreferencesEditForm);

                        // Show new form
                        newPreferencesEditForm.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        /// <summary>
        /// Handler for when preferences edit form is is closing
        /// Pops latest form from stack and shows the new form on top of the stack
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handlePreferencesEditFormClosing(object sender, FormClosingEventArgs e)
        {
            // Log.Debug("handlePreferencesEditFormClosing | _shownPreferenceForms: " + _shownPreferenceForms.ToString());
            if (_shownPreferenceForms != null && _shownPreferenceForms.Count > 0)
            {
                Form formPopped = _shownPreferenceForms.Pop();
                // Log.Debug("formPopped: " + formPopped.ToString());
                if (_shownPreferenceForms.Count > 0)
                {
                    Form formNewTop = _shownPreferenceForms.Peek();
                    // Log.Debug("formNewTop: " + formNewTop.ToString());
                    formNewTop.Show();
                }
            }
        }

        /// <summary>
        /// Prompt user asking if they want to save any changes made
        /// If yes - save preferences for all forms saved in the stack struct
        /// Close all open preference category or preference edit forms
        /// </summary>
        /// <param name="saveButtonPressed">If this function called from Save button</param>
        private void savePrefCloseAllShownForms(bool saveButtonPressed = false)
        {
            bool shownFormDirty = false;
            bool shownFormValidated = true;
            bool userSaveConfirmation = false;

            if (_shownPreferenceForms != null && _shownPreferenceForms.Count >= 1)
            {
                int numFormsOpen = _shownPreferenceForms.Count;
                if (saveButtonPressed)
                {
                    numFormsOpen = 1;
                }

                for (int i = 0; i < numFormsOpen; i++)
                {
                    if (_shownPreferenceForms.Count == 0)
                    {
                        break;
                    }

                    var shownForm = _shownPreferenceForms.Peek();

                    if (shownForm.GetType() == typeof(PreferencesEditForm))
                    {
                        shownFormValidated = ((PreferencesEditForm)shownForm).Validate();
                    }

                    /*
                    if (!shownFormValidated)
                    {
                        Log.Debug("savePrefCloseAllShownForms | !shownFormValidated | break");
                        break;
                    }
                    */

                    if (!saveButtonPressed)
                    {
                        _shownPreferenceForms.Pop();
                    }

                    if (shownForm.GetType() == typeof(PreferencesCategorySelectForm))
                    {
                        if (((PreferencesCategorySelectForm)shownForm)._isDirty)
                        {
                            shownFormDirty = true;
                        }
                    }
                    else if (shownForm.GetType() == typeof(PreferencesEditForm))
                    {
                        if (((PreferencesEditForm)shownForm)._isDirty)
                        {
                            shownFormDirty = true;
                        }
                    }

                    if (shownFormDirty == true && userSaveConfirmation == false)
                    {
                        userSaveConfirmation = ConfirmBox.ShowDialog("Save changes? ", this, true);
                    }

                    if (shownForm.GetType() == typeof(PreferencesCategorySelectForm))
                    {
                        if (userSaveConfirmation == true)
                        {
                            ((PreferencesCategorySelectForm)shownForm).validateAndSave();
                        }
                        ((PreferencesCategorySelectForm)shownForm)._isDirty = false;
                    }
                    else if (shownForm.GetType() == typeof(PreferencesEditForm))
                    {
                        if (userSaveConfirmation == true)
                        {
                            ((PreferencesEditForm)shownForm).validateAndSave();
                        }
                        ((PreferencesEditForm)shownForm)._isDirty = false;
                    }

                    if (!saveButtonPressed)
                    {
                        ((Form)shownForm).Close();
                    }
                }
            }
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