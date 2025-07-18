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
using ACAT.Lib.Core.WordPredictionManagement;

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

            private Button selectedCategoryButton;

            private List<(UserControl Panel, string Label)> breadcrumbStack = new();
            private string currentPageLabel;



            public SettingsForm()
            {
                Text = "ACAT Settings";
                Size = new System.Drawing.Size(1000, 700);
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
                    }
                    else
                    {
                        foreach (var type in getTypeCollection(context))
                        {
                            var instance = Activator.CreateInstance(type);
                            list.Add(new PreferencesCategory(instance, true, true));
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

                        //   var setupButton = CustomControls.CreateSetupButton(">", onClick: onClick != null ? (sender, e) => onClick(sender, category) : null, tag: category);
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
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
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
                // TODO CreateWordPredictorSpecificControls(tableLayout, preferences);
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
                        var ratePanel = CustomControls.CreateRateControl(prop, preferences);
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