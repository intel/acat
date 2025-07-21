using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using ACAT.Applications;
using ACAT.Lib.Core.ActuatorManagement;
using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.PreferencesManagement;
using ACAT.Lib.Core.TTSManagement;
using ACAT.Lib.Core.UserManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.Utility.WpfUserControlUtilities;
using ACAT.Lib.Core.WordPredictionManagement;
using ACAT.Lib.Extension;

namespace ACATConfigNext
{
    internal static class Program
    {
        public class SettingsForm : Form
        {
            private readonly Panel navPanel;
            private readonly Panel contentPanel;
            private readonly FlowLayoutPanel breadcrumbPanel;
            public static TableLayoutPanel _tableLayoutPanel = CustomControls.CreateCategoryTableLayoutPanel();
            public static string _currentPanelType;

            private Button selectedCategoryButton;

            private List<(UserControl Panel, string Label)> breadcrumbStack = new();
            private string currentPageLabel;

            private static HashSet<IPreferences> _modifiedPreferences = new HashSet<IPreferences>();
            private static List<PreferencesCategory> _currentWordPredictorCategories = new List<PreferencesCategory>();
            private static List<PreferencesCategory> _currentTTSCategories = new List<PreferencesCategory>();
            private static List<PreferencesCategory> _currentActuatorCategories = new List<PreferencesCategory>();



            public SettingsForm()
            {
                Text = "ACAT Settings";
                Size = new System.Drawing.Size(2000, 1400);
                StartPosition = FormStartPosition.CenterScreen;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                MinimizeBox = false;
                BackColor = Color.FromArgb(31, 31, 56);
                ForeColor = Color.White;

                contentPanel = CustomControls.CreatePanel(DockStyle.Fill, 0);
                breadcrumbPanel = CustomControls.CreateFlowPanel(DockStyle.Top, height: 40, text: "Settings", padding: new Padding(10, 5, 0, 0));
                navPanel = CustomControls.CreatePanel(DockStyle.Left, 200);

                Controls.Add(contentPanel);
                Controls.Add(breadcrumbPanel);
                Controls.Add(navPanel);

                LoadNavigation();
            }

            private void LoadNavigation()
            {
                string[] categories = { "General", "Actuators", "Word Predictors", "Text to Speech" };
                int y = 10;

                foreach (var category in categories)
                {
                    var btn = CustomControls.CreateFlatButton(text: category, tag: category, width: navPanel.Width - 20, top: y, left: 10, height: 40);
                    btn.Click += Category_Click;
                    navPanel.Controls.Add(btn);
                    y += 50;
                }

                if (navPanel.Controls.Count > 0)
                {
                    // Show the first category by default
                    var firstButton = (Button)navPanel.Controls[0];
                    Category_Click(firstButton, EventArgs.Empty);
                }
                else
                {
                    // If no categories, show a default message
                    contentPanel.Controls.Add(new Label { Text = "No settings available.", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter });
                }
            }

            private void Category_Click(object sender, EventArgs e)
            {
                if (sender is Button clickedButton)
                {
                    // Check if the clicked button is already selected  
                    if (selectedCategoryButton == clickedButton)
                    {
                        return; // Exit early if clicking on the current category  
                    }


                    if (selectedCategoryButton != null)
                    {
                        selectedCategoryButton.BackColor = Color.Transparent; // Reset previous button color
                    }

                    selectedCategoryButton = clickedButton;
                    selectedCategoryButton.BackColor = Color.FromArgb(255, 170, 0); // Highlight selected button

                    string category = (string)((Button)sender).Tag;
                    _currentPanelType = category; // Track the current panel type  

                    breadcrumbStack.Clear();
                    contentPanel.Controls.Clear();
                    currentPageLabel = category;

                    UserControl panel = category switch
                    {
                        "General" => new GeneralSettingsPanel(ShowPanel),
                        "Actuators" => new ActuatorSettingsPanel(ShowPanel),
                        "Word Predictors" => new WordPredictorsPanel(ShowPanel),
                        "Text to Speech" => new TTSPanel(ShowPanel),
                        _ => throw new ArgumentException("Invalid category"),
                    };

                    ShowPanel(panel, category);
                }
            }

            public void ShowPanel(UserControl panel, string label)
            {
                if (contentPanel.Controls.Count > 0)
                {
                    var lastPanel = (UserControl)contentPanel.Controls[0];
                    var lastLabel = breadcrumbPanel.Controls.OfType<Control>().LastOrDefault(c => c is Label || c is LinkLabel)?.Text;

                    breadcrumbStack.Add((lastPanel, lastLabel));
                    contentPanel.Controls.Clear();
                }

                currentPageLabel = label;
                panel.Dock = DockStyle.Fill;
                contentPanel.Controls.Add(panel);

                UpdateBreadcrumbTrail();
            }

            private void UpdateBreadcrumbTrail()
            {
                breadcrumbPanel.Controls.Clear();

                for (int i = 0; i < breadcrumbStack.Count; i++)
                {
                    // Add separator
                    //if (i > 0)
                    //    breadcrumbPanel.Controls.Add(new Label { Text = " > ", AutoSize = true });

                    var link = new LinkLabel
                    {
                        Text = breadcrumbStack[i].Label,
                        AutoSize = true,
                        Tag = i,
                        LinkColor = Color.White,
                        ActiveLinkColor = Color.White,
                        VisitedLinkColor = Color.White,
                        LinkBehavior = LinkBehavior.HoverUnderline
                    };

                    link.Click += (s, e) =>
                    {
                        int index = (int)((LinkLabel)s).Tag;
                        NavigateToBreadcrumb(index);
                    };

                    breadcrumbPanel.Controls.Add(link);
                }

                // Add separator before the current (last) label if not first
                if (breadcrumbStack.Count > 0)
                    breadcrumbPanel.Controls.Add(new Label { Text = " > ", AutoSize = true });

                // Add current page as plain text
                breadcrumbPanel.Controls.Add(new Label
                {
                    Text = currentPageLabel,
                    AutoSize = true,
                    Font = new Font(DefaultFont, FontStyle.Bold)
                });
            }

            private void NavigateToBreadcrumb(int index)
            {
                // Get the selected breadcrumb entry
                var (targetPanel, label) = breadcrumbStack[index];

                // Trim breadcrumbStack to one step before the target
                breadcrumbStack = breadcrumbStack.Take(index).ToList();

                currentPageLabel = label;

                contentPanel.Controls.Clear();
                targetPanel.Dock = DockStyle.Fill;
                contentPanel.Controls.Add(targetPanel);

                UpdateBreadcrumbTrail();
            }

            public static TableLayoutPanel RefreshGeneralSettingsPanel(Action<Control> onControlCreated = null)
            {
                var tableLayoutPanel = CustomControls.CreateCategoryTableLayoutPanel();

                if (CoreGlobals.AppPreferences == null)
                {
                    if (!AppCommon.LoadUserPreferences())
                    {
                        return tableLayoutPanel; // Return empty panel on error  
                    }
                }

                if (CoreGlobals.AppPreferences != null)
                {
                    var descriptor = CoreGlobals.AppPreferences.GetType().GetCustomAttribute<DescriptorAttribute>();

                    tableLayoutPanel.Controls.Add(CustomControls.CreateLabel(descriptor?.Category ?? "UNKNOWN CATEGORY"));
                    tableLayoutPanel.Controls.Add(CustomControls.CreateLabel(descriptor?.Description ?? "UNKNOWN DESCRIPTION"));

                    var props = CoreGlobals.AppPreferences.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

                    foreach (var prop in props)
                    {
                        var propPanel = CustomControls.CreateLabeledPanel(prop, CoreGlobals.AppPreferences);
                        var host = CustomControls.ElementHost(propPanel);
                        tableLayoutPanel.Controls.Add(host);

                        onControlCreated?.Invoke(host);
                    }
                }

                return tableLayoutPanel;
            }

            public static TableLayoutPanel RefreshExtensionPanel<TManager, TCollection>(Func<bool> loadManagerExtensions, Func<IEnumerable<string>> getExtensionDirs, TCollection context, Func<TCollection, IEnumerable<Type>> getTypeCollection, string panelTitle, EventHandler<PreferencesCategory> onClick = null) where TCollection : class
            {
                if (CoreGlobals.AppPreferences == null)
                {
                    if (!AppCommon.LoadUserPreferences())
                    {
                        return new TableLayoutPanel();
                    }
                }

                if (CoreGlobals.AppPreferences?.Extensions != null)
                {
                    _tableLayoutPanel.Controls.Clear();

                    if (!loadManagerExtensions())
                    {
                        return new TableLayoutPanel();
                    }

                    var extensionDirs = getExtensionDirs();

                    if (context is WordPredictors wp)
                    {
                        wp.Load(extensionDirs);
                    }
                    else if (context is TTSEngines tts)
                    {
                        tts.Load(extensionDirs);
                    }
                    else if (context is Actuators actuators)
                    {
                        actuators.Load(extensionDirs, UserManager.GetFullPath("ActuatorSettings.xml"), true);
                    }

                    var list = new List<PreferencesCategory>();

                    if (context is Actuators actuatorContext)
                    {
                        foreach (var actuator in actuatorContext.ActuatorList)
                        {
                            list.Add(new PreferencesCategory(actuator, true, actuator.Enabled));
                        }
                        _currentActuatorCategories = list;
                    }
                    else
                    {
                        foreach (var type in getTypeCollection(context))
                        {
                            var instance = Activator.CreateInstance(type);
                            list.Add(new PreferencesCategory(instance, true, true));
                        }

                        if (context is WordPredictors)
                        {
                            _currentWordPredictorCategories = list;
                        }
                        else if (context is TTSEngines)
                        {
                            _currentTTSCategories = list;
                        }
                    }

                    IEnumerable<PreferencesCategory> PreferencesCategories = list;

                    foreach (var category in PreferencesCategories)
                    {
                        if (!IsValidExtension(category, out var desc))
                            continue;

                        var categoryItem = CustomControls.CreateCategoryTableLayoutPanel();

                        categoryItem.Controls.Add(CustomControls.CreateLabel(desc.Name), 0, 0);
                        categoryItem.Controls.Add(CustomControls.CreateDescriptionLabel(desc.Description), 0, 2);


                        var checkBox = CustomControls.CreateCheckBox("Enabled");
                        checkBox.Tag = category;
                        categoryItem.Controls.Add(checkBox, 1, 1);
                        categoryItem.SetRowSpan(checkBox, 2);

                        var setupButton = CustomControls.CreateSetupButton(">", onClick: (sender, e) => OnSetupClicked(sender, category), tag: category);
                        categoryItem.Controls.Add(setupButton, 2, 0);
                        categoryItem.SetRowSpan(setupButton, 3);


                        _tableLayoutPanel.Controls.Add(categoryItem);
                    }

                    return _tableLayoutPanel;
                }

                return new TableLayoutPanel();
            }

            public static bool IsValidExtension(PreferencesCategory category, out IDescriptor descriptor)
            {
                descriptor = null;


                var extension = category.PreferenceObj as IExtension;
                if (extension == null)
                    return false;

                descriptor = extension.Descriptor;
                return descriptor != null && descriptor.HasSettings;
            }

            #region Save

            private void SaveButton_Click(object sender, EventArgs e)
            {
                try
                {
                    bool success = SaveAllPreferences();

                    if (success)
                    {
                        MessageBox.Show("Settings saved successfully.", "Save Complete",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        _modifiedPreferences.Clear();
                    }
                    else
                    {
                        MessageBox.Show("Some settings could not be saved. Please check the logs.",
                            "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                    MessageBox.Show("An error occurred while saving settings.", "Save Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            public static bool SaveAllPreferences()
            {
                bool success = true;

                try
                {
                    if (CoreGlobals.AppPreferences != null)
                    {
                        success &= CoreGlobals.AppPreferences.Save();
                    }

                    success &= SaveActuatorPreferences();

                    success &= SaveWordPredictorPreferences();

                    success &= SaveTTSPreferences();

                    return success;
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                    return false;
                }
            }

            private static bool SaveActuatorPreferences()
            {
                try
                {
                    var actuatorConfig = ActuatorManager.Instance.GetActuatorConfig();
                    return actuatorConfig.Save();
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                    return false;
                }
            }

            private static bool SaveWordPredictorPreferences()
            {
                try
                {
                    foreach (var category in _currentWordPredictorCategories)
                    {
                        if (category.PreferenceObj is ISupportsPreferences supportsPrefs)
                        {
                            var preferences = supportsPrefs.GetPreferences();
                            if (preferences != null)
                            {
                                preferences.Save();
                            }
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                    return false;
                }
            }

            private static bool SaveTTSPreferences()
            {
                try
                {
                    foreach (var category in _currentTTSCategories)
                    {
                        if (category.PreferenceObj is ISupportsPreferences supportsPrefs)
                        {
                            var preferences = supportsPrefs.GetPreferences();
                            if (preferences != null)
                            {
                                preferences.Save();
                            }
                        }

                        if (category.PreferenceObj is ITTSEngine ttsEngine)
                        {
                            ttsEngine.Save();
                        }
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                    return false;
                }
            }

            public static void TrackPreferenceModification(IPreferences preferences)
            {
                _modifiedPreferences.Add(preferences);
            }

            #endregion


            #region Reset to Default

            public static bool ResetAllPreferencesToDefaults()
            {
                bool success = true;

                try
                {
                    // Reset general preferences  
                    if (CoreGlobals.AppPreferences != null)
                    {
                        var defaultPrefs = ACATPreferences.LoadDefaultSettings();
                        if (defaultPrefs != null)
                        {
                            // Copy default values to current preferences  
                            CopyPreferencesValues(defaultPrefs, CoreGlobals.AppPreferences);
                            success &= CoreGlobals.AppPreferences.Save();
                        }
                    }

                    // Reset extension preferences  
                    success &= ResetActuatorPreferences();
                    success &= ResetWordPredictorPreferences();
                    success &= ResetTTSPreferences();

                    // Clear modified preferences tracking  
                    _modifiedPreferences.Clear();

                    return success;
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                    return false;
                }
            }

            private static bool ResetActuatorPreferences()
            {
                try
                {
                    foreach (var category in _currentActuatorCategories)
                    {
                        if (category.PreferenceObj is ISupportsPreferences supportsPrefs)
                        {
                            var defaultPrefs = supportsPrefs.GetDefaultPreferences();
                            var currentPrefs = supportsPrefs.GetPreferences();

                            if (defaultPrefs != null && currentPrefs != null)
                            {
                                CopyPreferencesValues(defaultPrefs, currentPrefs);
                                currentPrefs.Save();
                            }
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                    return false;
                }
            }

            private static bool ResetWordPredictorPreferences()
            {
                try
                {
                    foreach (var category in _currentWordPredictorCategories)
                    {
                        if (category.PreferenceObj is ISupportsPreferences supportsPrefs)
                        {
                            var defaultPrefs = supportsPrefs.GetDefaultPreferences();
                            var currentPrefs = supportsPrefs.GetPreferences();

                            if (defaultPrefs != null && currentPrefs != null)
                            {
                                CopyPreferencesValues(defaultPrefs, currentPrefs);
                                currentPrefs.Save();
                            }
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                    return false;
                }
            }

            private static bool ResetTTSPreferences()
            {
                try
                {
                    foreach (var category in _currentTTSCategories)
                    {
                        if (category.PreferenceObj is ISupportsPreferences supportsPrefs)
                        {
                            var defaultPrefs = supportsPrefs.GetDefaultPreferences();
                            var currentPrefs = supportsPrefs.GetPreferences();

                            if (defaultPrefs != null && currentPrefs != null)
                            {
                                CopyPreferencesValues(defaultPrefs, currentPrefs);
                                currentPrefs.Save();
                            }
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                    return false;
                }
            }

            private static void CopyPreferencesValues(IPreferences source, IPreferences target)
            {
                var sourceType = source.GetType();
                var targetType = target.GetType();

                var properties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (var prop in properties)
                {
                    if (prop.CanRead && prop.CanWrite)
                    {
                        try
                        {
                            var value = prop.GetValue(source);
                            var targetProp = targetType.GetProperty(prop.Name);

                            if (targetProp != null && targetProp.CanWrite)
                            {
                                targetProp.SetValue(target, value);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Debug($"Could not copy property {prop.Name}: {ex.Message}");
                        }
                    }
                }
            }

            private void ResetToDefaultButton_Click(object sender, EventArgs e)
            {
                try
                {
                    var result = MessageBox.Show(
                        "Are you sure you want to reset all settings to their default values? This action cannot be undone.",
                        "Reset to Defaults",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        bool success = ResetAllPreferencesToDefaults();

                        if (success)
                        {
                            MessageBox.Show("Settings have been reset to default values.", "Reset Complete",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            RefreshCurrentPanel();
                        }
                        else
                        {
                            MessageBox.Show("Some settings could not be reset. Please check the logs.",
                                "Reset Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                    MessageBox.Show("An error occurred while resetting settings.", "Reset Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void RefreshCurrentPanel()
            {
                string currentPanelType = GetCurrentPanelType();

                if (SettingsForm._tableLayoutPanel != null)
                {
                    SettingsForm._tableLayoutPanel.Controls.Clear();
                }

                switch (currentPanelType)
                {
                    case "General":
                        var generalPanel = RefreshGeneralSettingsPanel();
                        SettingsForm._tableLayoutPanel.Controls.Add(generalPanel);
                        break;

                    case "Actuators":
                        var actuatorPanel = RefreshExtensionPanel<ActuatorManager, Actuators>(
                            () => ACAT.Lib.Core.PanelManagement.Context.AppActuatorManager.LoadExtensions(ACAT.Lib.Core.PanelManagement.Context.ExtensionDirs, true),
                            () => ACAT.Lib.Core.PanelManagement.Context.ExtensionDirs,
                            new Actuators(),
                            context => null,
                            "Actuator Settings",
                            OnSetupClicked
                        );
                        SettingsForm._tableLayoutPanel.Controls.Add(actuatorPanel);
                        break;

                    case "Word Predictors":
                        var wordPredictorPanel = RefreshExtensionPanel<WordPredictionManager, WordPredictors>(
                            () => ACAT.Lib.Core.PanelManagement.Context.AppWordPredictionManager.LoadExtensions(ACAT.Lib.Core.PanelManagement.Context.ExtensionDirs),
                            () => ACAT.Lib.Core.PanelManagement.Context.ExtensionDirs,
                            new WordPredictors(),
                            context => context.Collection,
                            "Word Predictor Settings",
                            OnSetupClicked
                        );
                        SettingsForm._tableLayoutPanel.Controls.Add(wordPredictorPanel);
                        break;

                    case "Text to Speech":
                        var ttsPanel = RefreshExtensionPanel<TTSManager, TTSEngines>(
                            () => ACAT.Lib.Core.PanelManagement.Context.AppTTSManager.LoadExtensions(ACAT.Lib.Core.PanelManagement.Context.ExtensionDirs),
                            () => ACAT.Lib.Core.PanelManagement.Context.ExtensionDirs,
                            new TTSEngines(),
                            context => context.Collection,
                            "TTS Settings",
                            OnSetupClicked
                        );
                        SettingsForm._tableLayoutPanel.Controls.Add(ttsPanel);
                        break;

                    default:
                        var defaultPanel = RefreshGeneralSettingsPanel();
                        SettingsForm._tableLayoutPanel.Controls.Add(defaultPanel);
                        break;
                }
            }

            private string GetCurrentPanelType()
            {
                return _currentPanelType ?? "General";
            }


            #endregion
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            WpfInitializationHelper.EnsureApplicationResources();

            Application.Run(new SettingsForm());
        }

        public class GeneralSettingsPanel : UserControl
        {
            public GeneralSettingsPanel(Action<UserControl, string> showPanel)
            {
                //  var label = new Label { Text = "General Settings", Dock = DockStyle.Top, Height = 40 };
                Controls.Add(SettingsForm.RefreshGeneralSettingsPanel());
            }

            private class GeneralSettingsDescriptor : IDescriptor
            {
                public string Name => "General Settings";
                public string Description => "Configure general ACAT application settings";
                public string Category => "General";
                public Guid Id => Guid.Empty;
                public bool HasSettings => true;
            }
        }

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

        public class ActuatorSettingsPanel : UserControl
        {
            private readonly Actuators _context = new Actuators();

            public ActuatorSettingsPanel(Action<UserControl, string> showPanel)
            {
                Controls.Add(

                    SettingsForm.RefreshExtensionPanel<ActuatorManager, Actuators>(
                        () => ACAT.Lib.Core.PanelManagement.Context.AppActuatorManager.LoadExtensions(ACAT.Lib.Core.PanelManagement.Context.ExtensionDirs, true),
                        () => ACAT.Lib.Core.PanelManagement.Context.ExtensionDirs,
                        _context as Actuators,
                        context => null,
                        "Actuator Settings",
                        OnSetupClicked
                        ));

                Controls.Add(new Label { Text = "      Actuator Settings", Dock = DockStyle.Top, Height = 40 });
            }
        }

        public class WordPredictorsPanel : UserControl
        {
            private readonly WordPredictors _context = new();

            public WordPredictorsPanel(Action<UserControl, string> showPanel)
            {
                Controls.Add(SettingsForm.RefreshExtensionPanel<WordPredictionManager, WordPredictors>(
                    () => ACAT.Lib.Core.PanelManagement.Context.AppWordPredictionManager.LoadExtensions(ACAT.Lib.Core.PanelManagement.Context.ExtensionDirs),
                    () => ACAT.Lib.Core.PanelManagement.Context.ExtensionDirs,
                    _context as WordPredictors,
                    context => (context as WordPredictors).Collection,
                    "Word Predictor Settings",
                    OnSetupClicked
                    ));

                Controls.Add(new Label { Text = "      Word Predictors - Settings", Dock = DockStyle.Top, Height = 40 });
            }
        }

        public class TTSPanel : UserControl
        {
            private readonly TTSEngines _context = new TTSEngines();

            public TTSPanel(Action<UserControl, string> showPanel)
            {
                Controls.Add(SettingsForm.RefreshExtensionPanel<TTSManager, TTSEngines>(
                  () => ACAT.Lib.Core.PanelManagement.Context.AppWordPredictionManager.LoadExtensions(ACAT.Lib.Core.PanelManagement.Context.ExtensionDirs),
                  () => ACAT.Lib.Core.PanelManagement.Context.ExtensionDirs,
                      _context as TTSEngines,
                   context => (context as TTSEngines).Collection,
                  "Text to Speech Settings", OnSetupClicked
                  ));

                Controls.Add(new Label { Text = "      Text to Speech Settings", Dock = DockStyle.Top, Height = 40 });
            }
        }

        public class DisplaySettingsPanel : UserControl
        {
            public DisplaySettingsPanel()
            {
                Controls.Add(new Label { Text = "      Display Settings Details", Dock = DockStyle.Top, Height = 40 });
            }
        }

        private static void OnSetupClicked(object sender, PreferencesCategory category)
        {
            var SetupButton = sender as Button;
            var extension = category.PreferenceObj as IExtension;

            if (extension != null)
            {
                if (extension != null && category.PreferenceObj is ISupportsPreferences supportsPrefs)
                {
                    SettingsForm._tableLayoutPanel.Controls.Clear();

                    TableLayoutPanel preferencesPanel = CreatePreferencesTableLayoutForExtension(extension, supportsPrefs);

                    if (preferencesPanel != null)
                    {
                        SettingsForm._tableLayoutPanel.Controls.Add(preferencesPanel);
                    }
                }
            }
        }



        /// <summary>
        ///  Handle different extension types separately  
        /// </summary>
        /// <param name="extension"></param>
        /// <param name="supportsPrefs"></param>
        /// <returns></returns>
        private static TableLayoutPanel CreatePreferencesTableLayoutForExtension(IExtension extension, ISupportsPreferences supportsPrefs)
        {
            switch (extension)
            {
                case IActuator actuator:
                    return CreateActuatorPreferencesPanel(actuator, supportsPrefs);

                case ITTSEngine ttsEngine:
                    return CreateTTSEnginePreferencesPanel(ttsEngine, supportsPrefs);

                case IWordPredictor wordPredictor:
                    return CreateWordPredictorPreferencesPanel(wordPredictor, supportsPrefs);

                default:
                    return CreateGenericExtensionPreferencesPanel(extension, supportsPrefs);
            }
        }

        private static TableLayoutPanel CreateActuatorPreferencesPanel(IActuator actuator, ISupportsPreferences supportsPrefs)
        {
            var tableLayout = CustomControls.CreateCategoryTableLayoutPanel();

            var descriptor = actuator.Descriptor;
            tableLayout.Controls.Add(CustomControls.CreateLabel($"Actuator: {descriptor.Name}"));
            tableLayout.Controls.Add(CustomControls.CreateDescriptionLabel(descriptor.Description));

            var preferences = supportsPrefs.GetPreferences();
            if (preferences != null)
            {
                var props = preferences.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var prop in props)
                {
                    var propPanel = CustomControls.CreateLabeledPanel(prop, preferences);
                    var host = CustomControls.ElementHost(propPanel);
                    tableLayout.Controls.Add(host);
                }
            }

            return tableLayout;
        }

        private static TableLayoutPanel CreateTTSEnginePreferencesPanel(ITTSEngine ttsEngine, ISupportsPreferences supportsPrefs)
        {
            var tableLayout = CustomControls.CreateCategoryTableLayoutPanel();

            var descriptor = ttsEngine.Descriptor;
            tableLayout.Controls.Add(CustomControls.CreateLabel($"TTS Engine: {descriptor.Name}"));
            tableLayout.Controls.Add(CustomControls.CreateDescriptionLabel(descriptor.Description));

            var preferences = supportsPrefs.GetPreferences();
            if (preferences != null)
            {
                CreateTTSSpecificControls(tableLayout, preferences);
            }

            return tableLayout;
        }

        private static TableLayoutPanel CreateWordPredictorPreferencesPanel(IWordPredictor wordPredictor, ISupportsPreferences supportsPrefs)
        {
            var tableLayout = CustomControls.CreateCategoryTableLayoutPanel();
            var descriptor = wordPredictor.Descriptor;
            tableLayout.Controls.Add(CustomControls.CreateLabel($"Word Predictor: {descriptor.Name}"));
            tableLayout.Controls.Add(CustomControls.CreateDescriptionLabel(descriptor.Description));

            var preferences = supportsPrefs.GetPreferences();
            if (preferences != null)
            {
                CreateWordPredictorSpecificControls(tableLayout, preferences);
            }

            return tableLayout;
        }

        private static TableLayoutPanel CreateGenericExtensionPreferencesPanel(IExtension extension, ISupportsPreferences supportsPrefs)
        {
            var tableLayout = CustomControls.CreateCategoryTableLayoutPanel();

            var descriptor = extension.Descriptor;
            tableLayout.Controls.Add(CustomControls.CreateLabel($"Extension: {descriptor.Name}"));
            tableLayout.Controls.Add(CustomControls.CreateDescriptionLabel(descriptor.Description));

            var preferences = supportsPrefs.GetPreferences();
            if (preferences != null)
            {
                var props = preferences.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var prop in props)
                {
                    var propPanel = CustomControls.CreateLabeledPanel(prop, preferences);
                    var host = CustomControls.ElementHost(propPanel);
                    tableLayout.Controls.Add(host);
                }
            }

            return tableLayout;
        }

        private static void CreateWordPredictorSpecificControls(TableLayoutPanel tableLayout, IPreferences preferences)
        {
            var props = preferences.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                switch (prop.Name.ToLower())
                {
                    case "predictionwordcount":
                        var wordCountPanel = CustomControls.CreateWordCountControl(prop, preferences);
                        tableLayout.Controls.Add(wordCountPanel);
                        break;

                    case "ngram":
                        var ngramPanel = CustomControls.CreateNGramControl(prop, preferences);
                        tableLayout.Controls.Add(ngramPanel);
                        break;

                    case "filterpunctuationsenable":
                        var punctuationPanel = CustomControls.CreatePunctuationFilterControl(prop, preferences);
                        tableLayout.Controls.Add(punctuationPanel);
                        break;

                    case "supportslearning":
                        var learningPanel = CustomControls.CreateLearningControl(prop, preferences);
                        tableLayout.Controls.Add(learningPanel);
                        break;

                    case "filterchars":
                        var filterCharsPanel = CustomControls.CreateFilterCharsControl(prop, preferences);
                        tableLayout.Controls.Add(filterCharsPanel);
                        break;

                    case "usedefaultencoding":
                        var encodingPanel = CustomControls.CreateEncodingControl(prop, preferences);
                        tableLayout.Controls.Add(encodingPanel);
                        break;

                    case "showdisclaimeronStartup":
                        var disclaimerPanel = CustomControls.CreateDisclaimerControl(prop, preferences);
                        tableLayout.Controls.Add(disclaimerPanel);
                        break;

                    default:
                        var propPanel = CustomControls.CreateLabeledPanel(prop, preferences);
                        var host = CustomControls.ElementHost(propPanel);
                        tableLayout.Controls.Add(host);
                        break;
                }
            }
        }

        private static void CreateTTSSpecificControls(TableLayoutPanel tableLayout, IPreferences preferences)
        {
            var props = preferences.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                switch (prop.Name.ToLower())
                {
                    case "voice":
                        var voicePanel = CustomControls.CreateVoiceSelectionControl(prop, preferences);
                        tableLayout.Controls.Add(voicePanel);
                        break;

                    case "rate":
                        var ratePanel = CustomControls.CreateRateControl(prop, preferences, SettingsForm.TrackPreferenceModification);// TODO repeat on each numericUpDown
                        tableLayout.Controls.Add(ratePanel);
                        break;

                    case "volume":
                        var volumePanel = CustomControls.CreateVolumeControl(prop, preferences);
                        tableLayout.Controls.Add(volumePanel);
                        break;

                    case "pitch":
                        var pitchPanel = CustomControls.CreatePitchControl(prop, preferences);
                        tableLayout.Controls.Add(pitchPanel);
                        break;

                    default:
                        var propPanel = CustomControls.CreateLabeledPanel(prop, preferences);
                        var host = CustomControls.ElementHost(propPanel);
                        tableLayout.Controls.Add(host);
                        break;
                }
            }

        }


    }
}