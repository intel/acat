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
using ACATResources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;

namespace ACAT.Lib.Core.PreferencesManagement
{
    public partial class ACATConfigMainForm : Form
    {
     
        public List<Tuple<String, CheckBox>> _configCategoryList;    // Holds information on high level preferenes category in list of checkboxes on left side of the screen

        private static Stack<Form> _shownPreferenceForms;           // Keeps track of which preferences forms are currently displayed

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
        private readonly String _title = "ACAT Config";

        /// <summary>
        /// Form that displays a list of ACAT modules (or categories) such as Agents,
        /// Word Prediction, Text To Sepeech.  User can select
        /// a category to set preferences for that module
        /// </summary>
        public ACATConfigMainForm()
        {

            InitializeComponent();
            Load += ACATConfigMainForm_Load;
          //  Shown += ACATConfigMainForm_Shown;
            _configCategoryList = new List<Tuple<String, CheckBox>>
            {
                new Tuple<String, CheckBox>(checkBoxCategoryGeneral.Text.ToString(), checkBoxCategoryGeneral),
                new Tuple<String, CheckBox>(checkBoxCategoryActuators.Text.ToString(), checkBoxCategoryActuators),
                new Tuple<String, CheckBox>(checkBoxCategoryTextToSpeech.Text.ToString(), checkBoxCategoryTextToSpeech),
                new Tuple<String, CheckBox>(checkBoxCategoryWordPrediction.Text.ToString(), checkBoxCategoryWordPrediction),
            };

            SetNewFormButtonHandlers(); // Reset buttons to default states and clear all event handlers

            _shownPreferenceForms = new Stack<Form>();

            Paint += (s, args) => { handleConfigCategorySelected(checkBoxCategoryGeneral, null); };
        }

        // public delegate void NotifyPreferencesLanguageChanged(object sender, PreferencesLanguageChanged arg);

        public delegate void NotifyResetToDefaultButtonClicked(object sender, EventArgs e);

        public delegate void NotifyThemeChanged(object sender, String selectedTheme);

        public delegate void NotifyWrapTextCheckBoxClicked(object sender, EventArgs e);

        //  public event NotifyPreferencesLanguageChanged EvtLanguageChanged;

        public event NotifyThemeChanged EvtThemeChanged;

        private event NotifyResetToDefaultButtonClicked EvtResetToDefaultButtonClicked;

        private event NotifyWrapTextCheckBoxClicked EvtWrapTextCheckBoxClicked;

        public void SetNewFormButtonHandlers(NotifyResetToDefaultButtonClicked handlerResetToDefaultButtonClicked = null, NotifyWrapTextCheckBoxClicked handlerWrapTextBoxClicked = null, bool wrapTextDefault = false)
        {
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

        private void ACATConfigMainForm_Load(object sender, EventArgs e)
        {
            float currentAspectRatio = (float)ClientSize.Height / ClientSize.Width;

            if (_designTimeAspectRatio != 0.0f && currentAspectRatio != _designTimeAspectRatio)
            {
                ClientSize = new System.Drawing.Size(ClientSize.Width, (int)(_designTimeAspectRatio * ClientSize.Width));
            }

            _ = ResourceUtils.EnumerateInstalledLanguages();

            TopMost = false;
            TopMost = true;

            Activate();

            CenterToScreen();
        }

        private void ACATConfigMainForm_Shown(object sender, EventArgs e)
        {
            ConfirmBoxOneOption.ShowDialog("Please exercise caution when changing ACAT settings.",
                "Refer to the ACAT User Guide for help", StringResources.OK, this);
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            savePrefCloseAllShownForms();
            Close();
        }

        private void buttonResetToDefault_Click(object sender, EventArgs e)
        {
            EvtResetToDefaultButtonClicked?.Invoke(sender, e);
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            savePrefCloseAllShownForms(true);
            buttonSave.Visible = false;
        }

        private void checkBoxWrapText_Click(object sender, EventArgs e)
        {
            EvtWrapTextCheckBoxClicked?.Invoke(sender, e);
        }

        private void handleConfigCategorySelected(object sender, EventArgs e)
        {
            String newConfigCategory = ((CheckBox)sender).Text.ToString();
            Log.Debug("handleConfigCategorySelected | Selected high level category: " + newConfigCategory);

            foreach (var (category, checkBox) in _configCategoryList)
            {
                bool isSelected = category == newConfigCategory;
                checkBox.Checked = isSelected;
                checkBox.ForeColor = isSelected
                    ? System.Drawing.Color.FromArgb(35, 36, 51)
                    : System.Drawing.Color.White;
            }

            // Prompt user if they want to save preferences changes made and close all forms up until this point
            savePrefCloseAllShownForms();

            // Get handle of control you will set to parent of preferences form
            IntPtr parentControlHandle = tableLayoutPanelConfigSettings.Handle;

            // Create new PreferencesCategorySelectForm or PreferencesEditForm depending on which high level category selected
            Form newPreferencesSelectForm = null;
            IntPtr formHandle = this.Handle;
            var existingApp = System.Windows.Application.Current;

            // Handle category-specific initialization
            switch (newConfigCategory)
            {
                case "General":
                    //PreferencesEditForm.EnsureInitialized();
                    newPreferencesSelectForm = CreateGeneralPreferencesForm();
                    break;

                case "Actuators":
                    //PreferencesEditForm.EnsureInitialized();
                    if (!Context.AppActuatorManager.LoadExtensions(Context.ExtensionDirs, true))
                    {
                        ShowError("Actuator");
                        return;
                    }
                    newPreferencesSelectForm = Context.AppActuatorManager.GetPreferencesSelectionForm(parentControlHandle);
                    break;

                case "Word Prediction":
                    //PreferencesEditForm.EnsureInitialized();
                    if (!Context.AppWordPredictionManager.LoadExtensions(Context.ExtensionDirs))
                    {
                        ShowError("Word Prediction");
                        return;
                    }
                    newPreferencesSelectForm = Context.AppWordPredictionManager.GetPreferencesSelectionForm(parentControlHandle);
                    break;

                case "Text to Speech":
                    //PreferencesEditForm.EnsureInitialized();
                    if (!Context.AppTTSManager.LoadExtensions(Context.ExtensionDirs))
                    {
                        ShowError("Text-to-Speech");
                        return;
                    }
                   newPreferencesSelectForm = Context.AppTTSManager.GetPreferencesSelectionForm(parentControlHandle);
                    break;
            }

            // Finalize form setup if a form was created
            if (newPreferencesSelectForm == null)
            {
                MessageBox.Show("Error creating form for preferences configuration", "ACAT Config", MessageBoxButtons.OK);
                return;
            }

            formHandle = newPreferencesSelectForm.Handle;

            // For all non-General categories, just reset handlers and hide buttons
            if (newConfigCategory != "General")
            {
                SetNewFormButtonHandlers();
            }

            // Hook up save button visibility on preference change
            if (newPreferencesSelectForm is PreferencesEditForm editForm)
            {
                editForm.EvtPreferencesChangeMade += () => buttonSave.Visible = true;
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
            newPreferencesSelectForm.FormClosed += (s, args) => SetNewFormButtonHandlers();

            // Set handler for when "Setup" button pressed for preferences category
            if (newPreferencesSelectForm is PreferencesCategorySelectForm categoryForm)
            {
                categoryForm.EvtPreferencesCategorySelected += handlePreferencesCategorySelected;
            }

            // Push new preferences select form to stack
            _shownPreferenceForms.Push(newPreferencesSelectForm);

            // Show as regular form / control
            newPreferencesSelectForm.Show();
        }

        private PreferencesEditForm CreateGeneralPreferencesForm()
        {
            var generalSettings = new GeneralSettingsCategory();

            var generalForm = new PreferencesEditForm
            {
                SupportsPreferencesObj = generalSettings,
                Title = "General Settings"
            };

            // Hook up handlers and defaults
            NotifyWrapTextCheckBoxClicked wrapTextHandler = generalForm.checkBoxWrapText_CheckedChanged;
            NotifyResetToDefaultButtonClicked resetHandler = generalForm.buttonDefaults_Click;
            bool wrapTextDefault = generalForm._wrapText;

         //   buttonResetToDefault.Visible = true;
            SetNewFormButtonHandlers(resetHandler, wrapTextHandler, wrapTextDefault);

            return generalForm;
        }


        private void ShowError(string category)
        {
            MessageBox.Show($"Error loading {category} extensions", "ACAT Config", MessageBoxButtons.OK);
        }

        //Handler for when preferences category is selected ("Setup" button in "Configure" column)
        //Load custom preferences dialog called by ShowPreferencesDialog, or load default PreferencesEditForm2
        private void handlePreferencesCategorySelected(object sender, ISupportsPreferences supportsPreferences)
        {
            try
            {
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

                      //  newPreferencesEditForm.EvtPreferencesChangeMade += handlePreferenceChangeMade;

                        newPreferencesEditForm.EvtPreferencesChangeMade += () => buttonSave.Visible = true;

                       // newPreferencesEditForm.FormClosing += handlePreferencesEditFormClosing;

                        newPreferencesEditForm.FormClosing += (_, e) =>
                        {
                            if (_shownPreferenceForms?.Count > 0)
                            {
                                _shownPreferenceForms.Pop();
                                _shownPreferenceForms.Peek()?.Show();
                            }
                        };

                        // Push new preferences edit form to stack
                        _shownPreferenceForms.Push(newPreferencesEditForm);
                        newPreferencesEditForm.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        private void savePrefCloseAllShownForms(bool saveButtonPressed = false)
        {
            if (_shownPreferenceForms == null || _shownPreferenceForms.Count == 0)
                return;

            int formsToProcess = saveButtonPressed ? 1 : _shownPreferenceForms.Count;
            bool userConfirmedSave = false;

            for (int i = 0; i < formsToProcess; i++)
            {
                if (_shownPreferenceForms.Count == 0)
                    break;

                var shownForm = _shownPreferenceForms.Peek();

                if (shownForm is PreferencesEditForm editForm)
                    _ = editForm.Validate();

                if (!saveButtonPressed)
                    _shownPreferenceForms.Pop();

                bool isDirty = shownForm is PreferencesCategorySelectForm categoryForm ? categoryForm._isDirty : (shownForm is PreferencesEditForm editForm2 ? editForm2._isDirty : false);

                userConfirmedSave = (isDirty && !userConfirmedSave) ? ConfirmBoxTwoOption.ShowDialog("Save changes?", "", StringResources.Yes, StringResources.No, this, true) : userConfirmedSave;

                if (shownForm is PreferencesCategorySelectForm categoryForm3)
                {
                    if (userConfirmedSave)
                        categoryForm3.validateAndSave();
                    categoryForm3._isDirty = false;
                }
                else if (shownForm is PreferencesEditForm editForm3)
                {
                    if (userConfirmedSave)
                        editForm3.validateAndSave();
                    editForm3._isDirty = false;
                }

                if (!saveButtonPressed)
                    ((Form)shownForm).Close();
            }
        }

        /* public class PreferencesLanguageChanged
        {
            public CultureInfo CI;
            public bool IsDefault;

            public PreferencesLanguageChanged(CultureInfo ci, bool isDefault)
            {
                CI = ci;
                IsDefault = isDefault;
            }
        } */

        private class GeneralSettingsCategory : ISupportsPreferences
        {
            public bool SupportsPreferencesDialog
            {
                get { return false; }
            }

            public IPreferences GetDefaultPreferences()
            {
                return CoreGlobals.AppDefaultPreferences;
            }

            public IPreferences GetPreferences()
            {
                return CoreGlobals.AppPreferences;
            }

            public bool ShowPreferencesDialog()
            {
                return true;
            }
        }
    }
}