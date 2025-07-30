using ACAT.Applications;
using ACAT.Core.PanelManagement;
using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using ACAT.Core.Utility.WpfUserControlUtilities;
using ACATConfigNext.UserControls;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using MahAppsControls = MahApps.Metro.Controls;
using Windows = System.Windows;
using WPFControls = System.Windows.Controls;
using ACAT.Core.Extensions;
using ACAT.Core.PreferencesManagement;

namespace ACATConfigNext
{
    public class SettingsForm : Form
    {
        private TableLayoutPanel basePanel;
        private FlowLayoutPanel leftPanel;
        private TableLayoutPanel navPanel;
        private TableLayoutPanel mainPanel;
        private FlowLayoutPanel breadcrumbPanel;
        private TableLayoutPanel contentPanel;
        private FlowLayoutPanel bottomPanel;

        private Button selectedCategoryButton;
        private Button saveButton;
        private Button resetButton;
      
        private List<(UserControl Panel, string Label)> breadcrumbStack = new();
        private string currentPageLabel;

        private bool _isDirty = false;

        //Delegate for the event triggered when the user makes a change to a preference setting 
        public delegate void NotifyPreferencesChangeMade();

        //Event raised when the user makes a change to a preference setting 
        public event NotifyPreferencesChangeMade EvtPreferencesChangeMade;

        public SettingsForm()
        {
            WpfInitializationHelper.EnsureApplicationResources();

            InitializeComponent();

            EvtPreferencesChangeMade += SettingsChanged;
        }

        private void InitializeComponent()
        { 
            Text = "ACAT Settings";
            MaximumSize = new Size(2000, 1400);
            MinimumSize = new Size(2000, 1200);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.FromArgb(31, 31, 56);
            ForeColor = Color.White;

            basePanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                //*
                BackColor = Color.Transparent,
                /*/
                BackColor = Color.Red,
                //*/
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 2,
                RowCount = 1,
                GrowStyle = TableLayoutPanelGrowStyle.AddRows,
                ColumnStyles = { new ColumnStyle(SizeType.AutoSize), new ColumnStyle(SizeType.Percent, 100F) }
            };

            leftPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Left,
                //*
                BackColor = Color.Transparent,
                /*/
                BackColor = Color.Green,
                //*/
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(10),
                Margin = new Padding(0),
            };
                
            var acatlabel = new Label
            {
                Text = "ACAT",
                Font = new Font("Montserrat.Thin", 64),
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = true
            };

            var settingslabel = new Label
            {
                Text = "Settings",
                Font = new Font("Montserrat", 32, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = true
            };
            leftPanel.Controls.Add(acatlabel);
            leftPanel.Controls.Add(settingslabel);

            navPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Left,
                //*
                BackColor = Color.Transparent,
                /*/
                BackColor = Color.Blue,
                //*/
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 1,
                RowCount = 4,
                GrowStyle = TableLayoutPanelGrowStyle.AddRows
            };

               
            leftPanel.Controls.Add(navPanel);

            mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                //*
                BackColor = Color.Transparent,
                /*/
                BackColor = Color.Purple,
                //*/
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 1,
                RowCount = 3,
                GrowStyle = TableLayoutPanelGrowStyle.AddRows,
                RowStyles = { new RowStyle(SizeType.AutoSize), new RowStyle(SizeType.Percent, 100F), new RowStyle(SizeType.AutoSize) }
            };

            breadcrumbPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Name = "Settings",
                Padding = new Padding(5),
                AutoScroll = false,
                //*/
                BackColor = Color.Transparent,
                /*/
                BackColor = Color.Orange,
                //*/
                Font = new Font("Montserrat", 14, FontStyle.Italic),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            contentPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                //*/
                BackColor = Color.Transparent,
                /*
                BackColor = Color.DarkBlue,
                //*/
                Padding = new Padding(10),
                Margin = new Padding(10),
                RowCount = 1,
                ColumnCount = 1,
                GrowStyle = TableLayoutPanelGrowStyle.AddRows,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
            };

            bottomPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                //*/
                BackColor = Color.Transparent,
                /*/
                BackColor = Color.DarkBlue,
                //*/
                Padding = new Padding(10),
                Margin = new Padding(10),
                //RowCount = 1,
                //ColumnCount = 1,
                //GrowStyle = TableLayoutPanelGrowStyle.AddRows,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
            };

            saveButton = new ScannerRoundedButtonControl()
            {
                Text = "Save",
                Font = new Font("Montserrat", 18, FontStyle.Italic),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Enabled = false
            };
            saveButton.Click += (s, e) =>
            {
                try
                {
                    bool success = AppCommon.SaveAllPreferences();
                    if (success)
                    {
                        if (CoreGlobals.AppPreferences != null)
                        {
                            success &= CoreGlobals.AppPreferences.Save();
                        }
                        MessageBox.Show("Settings saved successfully.", "Save Complete",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        _isDirty = false;
                        saveButton.Enabled = _isDirty;
                        saveButton.Enabled = _isDirty;
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
            };

            resetButton = new ScannerRoundedButtonControl()
            {
                Text = "Reset to Defaults",
                Font = new Font("Montserrat", 18, FontStyle.Italic),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Enabled = false
            };
            resetButton.Click += (s, e) =>
            {
                try
                {
                    var list = new List<PreferencesCategory>();
                    bool success = AppCommon.ResetAllPreferences(list);
                    if (success)
                    {
                        _isDirty = false;
                        saveButton.Enabled = _isDirty;
                        resetButton.Enabled = _isDirty;
                    }
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                    MessageBox.Show("An error occurred while saving settings.", "Save Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            bottomPanel.Controls.Add(saveButton);
            bottomPanel.Controls.Add(resetButton); 

            mainPanel.Controls.Add(breadcrumbPanel, 0, 0);
            mainPanel.Controls.Add(contentPanel, 0, 1);
            mainPanel.Controls.Add(bottomPanel, 0, 2);

            basePanel.Controls.Add(leftPanel, 0, 0);
            basePanel.Controls.Add(mainPanel, 1, 0);
            Controls.Add(basePanel);

            LoadNavigation();
        }


        private IEnumerable<IExtension> LoadSettings(string category)
        {
            switch (category)
            {
                case "General":
                    AppCommon.LoadUserPreferences();
                    break;

                case "Actuators":
                    if (Context.AppActuatorManager.LoadExtensions(Context.ExtensionDirs, true))
                    {
                        //MessageBox.Show("Missing Actuator");
                       return Context.AppActuatorManager.ActuatorsList;
                      //  return Context.AppActuatorManager.ActuatorsList.Cast<IExtension>();

                    }
                    break;

                case "Word Predictors":
                    if (Context.AppWordPredictionManager.LoadExtensions(Context.ExtensionDirs))
                    {
                        //  return Context.AppWordPredictionManager.WordPredictorsList;
                        var wordPredictorTypes = Context.AppWordPredictionManager.WordPredictorExtensions;
                        var wordPredictorExtensions = wordPredictorTypes
                            .Select(type => Activator.CreateInstance(type) as IExtension)
                            .Where(instance => instance != null);
                        return wordPredictorExtensions;

                    }
                    break;

                case "Text to Speech":
                    if (Context.AppTTSManager.LoadExtensions(Context.ExtensionDirs))
                    {
                       // return Context.AppTTSManager.TTSEnginesList;

                        var ttsEngineTypes = Context.AppTTSManager.GetExtensions();
                        var ttsExtensions = ttsEngineTypes
                            .Select(type => Activator.CreateInstance(type) as IExtension)
                            .Where(instance => instance != null);
                        return ttsExtensions;
                    }
                    break;
            }

            return null;
        }

        private void LoadNavigation()
        {
            string[] categories = { "General", "Actuators", "Word Predictors", "Text to Speech" };

            foreach (var category in categories)
            {
                var btn = new ScannerRoundedButtonControl()
                {
                    Text = category,
                    Font = new Font("Montserrat", 18, FontStyle.Italic),
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    ForeColor = Color.White,
                    Dock = DockStyle.Top,
                    Tag = (Category: category, Settings: LoadSettings(category))
                };

                btn.Click += Category_Click;
                navPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                navPanel.Controls.Add(btn);
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

                var (Category, Settings) = ((string Category, IEnumerable<IExtension> Settings))clickedButton.Tag;
                string category = Category;

                if (Settings == null)
                {
                    Settings = Enumerable.Empty<IExtension>();
                }

                breadcrumbStack.Clear();
                contentPanel.Controls.Clear();
                currentPageLabel = category;

           


                UserControl panel = category switch
                {
                    "General" => new SettingsPanel(ShowPanel, CoreGlobals.AppPreferences),
                    "Actuators" => new GroupedSettingsPanel(ShowPanel, Settings),
                    "Word Predictors" => new GroupedSettingsPanel(ShowPanel, Settings),
                    "Text to Speech" => new GroupedSettingsPanel(ShowPanel, Settings),
                    _ => throw new ArgumentException("Invalid category"),
                };

                AttachInputEvents(panel);

                ShowPanel(panel, category);
            }
        }
        private void AttachInputEvents(Control container)
        {
            // Attach event handlers to input controls within the container
            foreach (object child in container.Controls)
            {
                if (child is Panel childControl)
                {
                    AttachInputEvents(childControl); // Recursively attach events to child controls
                }
                else if (child is ElementHost host)
                {
                    var panel = host.Child as Windows.UIElement;

                    var controls = panel.FindChildren<Windows.Controls.Control>().ToList();
                    foreach (var control in controls)
                    {
                        if (control is WPFControls.Slider cb)
                        {
                            cb.ValueChanged += OnValueChanged;
                        }
                        else if (control is MahAppsControls.ToggleSwitch ts)
                        {
                            ts.Toggled += OnValueChanged;
                        }
                        else if (control is WPFControls.TextBox tb)
                        {
                            tb.TextChanged += OnValueChanged;
                        }
                    }
                }
            }
        }

        private void OnValueChanged(object sender, EventArgs e)
        {
            _isDirty = true;
            saveButton.Enabled = _isDirty;  
            resetButton.Enabled = _isDirty;
            EvtPreferencesChangeMade();
        }

        public void SettingsChanged()
        {
            // Enable save button if any settings have changed
            saveButton.Enabled = true;
            resetButton.Enabled = true;
            // Optionally, you can also update the UI or perform other actions here
            // For example, you might want to highlight the changed panel or show a message
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
                var link = new LinkLabel
                {
                    Text = breadcrumbStack[i].Label,
                    AutoSize = true,
                    Tag = i,
                    LinkColor = Color.White,
                    ActiveLinkColor = Color.White,
                    VisitedLinkColor = Color.White,
                    LinkBehavior = LinkBehavior.NeverUnderline,
                    Cursor = Cursors.Default
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
                //Font = new Font(DefaultFont, FontStyle.Bold)
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

        //public static TableLayoutPanel RefreshGeneralSettingsPanel(Action<Control> onControlCreated = null)
        //{
        //    var tableLayoutPanel = CustomControls.CreateCategoryTableLayoutPanel();

        //    if (CoreGlobals.AppPreferences == null)
        //    {
        //        if (!AppCommon.LoadUserPreferences())
        //        {
        //            return tableLayoutPanel; // Return empty panel on error  
        //        }
        //    }

        //    if (CoreGlobals.AppPreferences != null)
        //    {
        //        var descriptor = CoreGlobals.AppPreferences.GetType().GetCustomAttribute<DescriptorAttribute>();

        //        tableLayoutPanel.Controls.Add(CustomControls.CreateLabel(descriptor?.Category ?? "UNKNOWN CATEGORY"));
        //        tableLayoutPanel.Controls.Add(CustomControls.CreateLabel(descriptor?.Description ?? "UNKNOWN DESCRIPTION"));

        //        var props = CoreGlobals.AppPreferences.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        //        foreach (var prop in props)
        //        {
        //            var propPanel = CustomControls.CreateLabeledPanel(prop, CoreGlobals.AppPreferences);
        //            var host = CustomControls.ElementHost(propPanel);
        //            tableLayoutPanel.Controls.Add(host);

        //            onControlCreated?.Invoke(host);
        //        }
        //    }

        //    return tableLayoutPanel;
        //}

        //public static TableLayoutPanel RefreshExtensionPanel<TManager, TCollection>(Func<bool> loadManagerExtensions, Func<IEnumerable<string>> getExtensionDirs, TCollection context, Func<TCollection, IEnumerable<Type>> getTypeCollection, string panelTitle, EventHandler<PreferencesCategory> onClick = null) where TCollection : class
        //{
        //    if (CoreGlobals.AppPreferences == null)
        //    {
        //        if (!AppCommon.LoadUserPreferences())
        //        {
        //            return new TableLayoutPanel();
        //        }
        //    }

        //    if (CoreGlobals.AppPreferences?.Extensions != null)
        //    {
        //        _tableLayoutPanel.Controls.Clear();

        //        if (!loadManagerExtensions())
        //        {
        //            return new TableLayoutPanel();
        //        }

        //        var extensionDirs = getExtensionDirs();

        //        if (context is WordPredictors wp)
        //        {
        //            wp.Load(extensionDirs);
        //        }
        //        else if (context is TTSEngines tts)
        //        {
        //            tts.Load(extensionDirs);
        //        }
        //        else if (context is Actuators actuators)
        //        {
        //            actuators.Load(extensionDirs, UserManager.GetFullPath("ActuatorSettings.xml"), true);
        //        }

        //        var list = new List<PreferencesCategory>();

        //        if (context is Actuators actuatorContext)
        //        {
        //            foreach (var actuator in actuatorContext.ActuatorList)
        //            {
        //                list.Add(new PreferencesCategory(actuator, true, actuator.Enabled));
        //            }
        //            _currentActuatorCategories = list;
        //        }
        //        else
        //        {
        //            foreach (var type in getTypeCollection(context))
        //            {
        //                var instance = Activator.CreateInstance(type);
        //                list.Add(new PreferencesCategory(instance, true, true));
        //            }

        //            if (context is WordPredictors)
        //            {
        //                _currentWordPredictorCategories = list;
        //            }
        //            else if (context is TTSEngines)
        //            {
        //                _currentTTSCategories = list;
        //            }
        //        }

        //        IEnumerable<PreferencesCategory> PreferencesCategories = list;

        //        foreach (var category in PreferencesCategories)
        //        {
        //            if (!IsValidExtension(category, out var desc))
        //                continue;

        //            var categoryItem = CustomControls.CreateCategoryTableLayoutPanel();

        //            categoryItem.Controls.Add(CustomControls.CreateLabel(desc.Name), 0, 0);
        //            categoryItem.Controls.Add(CustomControls.CreateDescriptionLabel(desc.Description), 0, 2);


        //            var checkBox = CustomControls.CreateCheckBox("Enabled");
        //            checkBox.Tag = category;
        //            categoryItem.Controls.Add(checkBox, 1, 1);
        //            categoryItem.SetRowSpan(checkBox, 2);

        //            var setupButton = CustomControls.CreateSetupButton(">", onClick: (sender, e) => OnSetupClicked(sender, category), tag: category);
        //            categoryItem.Controls.Add(setupButton, 2, 0);
        //            categoryItem.SetRowSpan(setupButton, 3);


        //            _tableLayoutPanel.Controls.Add(categoryItem);
        //        }

        //        return _tableLayoutPanel;
        //    }

        //    return new TableLayoutPanel();
        //}

        //public static bool IsValidExtension(PreferencesCategory category, out IDescriptor descriptor)
        //{
        //    descriptor = null;


        //    var extension = category.PreferenceObj as IExtension;
        //    if (extension == null)
        //        return false;

        //    descriptor = extension.Descriptor;
        //    return descriptor != null && descriptor.HasSettings;
        //}

        //private void SaveButton_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        bool success = SaveAllPreferences();

        //        if (success)
        //        {
        //            MessageBox.Show("Settings saved successfully.", "Save Complete",
        //                MessageBoxButtons.OK, MessageBoxIcon.Information);
        //            _modifiedPreferences.Clear();
        //        }
        //        else
        //        {
        //            MessageBox.Show("Some settings could not be saved. Please check the logs.",
        //                "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Exception(ex);
        //        MessageBox.Show("An error occurred while saving settings.", "Save Error",
        //            MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}
        //public static bool SaveAllPreferences()
        //{
        //    bool success = true;

        //    try
        //    {
        //        if (CoreGlobals.AppPreferences != null)
        //        {
        //            success &= CoreGlobals.AppPreferences.Save();
        //        }

        //        success &= SaveActuatorPreferences();

        //        success &= SaveWordPredictorPreferences();

        //        success &= SaveTTSPreferences();

        //        return success;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Exception(ex);
        //        return false;
        //    }
        //}

        //private static bool SaveActuatorPreferences()
        //{
        //    try
        //    {
        //        var actuatorConfig = ActuatorManager.Instance.GetActuatorConfig();
        //        return actuatorConfig.Save();
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Exception(ex);
        //        return false;
        //    }
        //}

        //private static bool SaveWordPredictorPreferences()
        //{
        //    try
        //    {
        //        foreach (var category in _currentWordPredictorCategories)
        //        {
        //            if (category.PreferenceObj is ISupportsPreferences supportsPrefs)
        //            {
        //                var preferences = supportsPrefs.GetPreferences();
        //                if (preferences != null)
        //                {
        //                    preferences.Save();
        //                }
        //            }
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Exception(ex);
        //        return false;
        //    }
        //}

        //private static bool SaveTTSPreferences()
        //{
        //    try
        //    {
        //        foreach (var category in _currentTTSCategories)
        //        {
        //            if (category.PreferenceObj is ISupportsPreferences supportsPrefs)
        //            {
        //                var preferences = supportsPrefs.GetPreferences();
        //                if (preferences != null)
        //                {
        //                    preferences.Save();
        //                }
        //            }

        //            if (category.PreferenceObj is ITTSEngine ttsEngine)
        //            {
        //                ttsEngine.Save();
        //            }
        //        }

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Exception(ex);
        //        return false;
        //    }
        //}

    }
}