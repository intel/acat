using ACAT.Applications;
using ACAT.Core.Extensions;
using ACAT.Core.PanelManagement;
using ACAT.Core.PreferencesManagement.Interfaces;
using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using ACAT.Extension;
using ACATConfigNext.UserControls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace ACATConfigNext.Forms
{
    public class SettingsForm : Form
    {
        private readonly ILogger<SettingsForm> _logger;
        private readonly IServiceProvider _serviceProvider;
        private TableLayoutPanel basePanel;
        private FlowLayoutPanel leftPanel;
        private TableLayoutPanel navPanel;
        private TableLayoutPanel mainPanel;
        private FlowLayoutPanel breadcrumbPanel;
        private TableLayoutPanel settingsPanel;
        private FlowLayoutPanel buttonPanel;

        private Button saveButton;
        private Button cancelButton;
        private Button exitButton;

        private ScannerRoundedButtonControl selectedCategoryButton;

        private List<(UserControl Panel, string Label)> breadcrumbStack = new();
        private string currentPageLabel;
        private string _currentCategory;

        private UserControl currentSettingsPanel;

        private bool _isDirty = false;

        public SettingsForm(ILogger<SettingsForm> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            WpfInitializationHelper.EnsureApplicationResources();

            InitializeComponent();
        }

        private FlowLayoutPanel CreateLeftPanel() 
        {
            leftPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                //*
                BackColor = Color.Transparent,
                /*/
                BackColor = Color.Green,
                //*/
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false, // stack vertically only
                Margin = new Padding(40, 48, 40, 48)
            };

            leftPanel.Controls.Add(CreateTitleLabel());

            return leftPanel;
        }

        private TableLayoutPanel CreateMainPanel()
        {
            var parent = new TableLayoutPanel
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Margin = new Padding(0, 48, 48, 48)
            };
            parent.RowStyles.Add(new RowStyle(SizeType.AutoSize));   // top row autosizes
            parent.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // middle row fills remaining space
            parent.RowStyles.Add(new RowStyle(SizeType.AutoSize));   // bottom row autosizes

            breadcrumbPanel = CreateBreadcrumbPanel();
            parent.Controls.Add(breadcrumbPanel, 0, 0);

            settingsPanel = CreateSettingsPanel();
            parent.Controls.Add(settingsPanel, 0, 1);

            buttonPanel = CreateBottomPanel();

            parent.Controls.Add(buttonPanel, 0, 2);

            // finally add parent to your form
            return parent;

        }

        private FlowLayoutPanel CreateBottomPanel()
        {
            // bottom row: a flowlayoutpanel for buttons
            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(5),
            };

            exitButton = CreateButton("Exit", true, ButtonClicked_Exit);
            buttonPanel.Controls.Add(exitButton);
            cancelButton = CreateButton("Cancel", false, ButtonClicked_Cancel);
            buttonPanel.Controls.Add(cancelButton);
            saveButton = CreateButton("Save", false, ButtonClicked_Save);
            buttonPanel.Controls.Add(saveButton);
            return buttonPanel;
        }

        private TableLayoutPanel CreateSettingsPanel()
        {
            return new TableLayoutPanel
            {
                AccessibleName = "SettingsPanel",
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
           };
        }

        private FlowLayoutPanel CreateBreadcrumbPanel()
        {
            breadcrumbPanel = new FlowLayoutPanel
            {
                AccessibleName = "BreadcrumbPanel",
                BackColor = Color.Transparent,
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.LeftToRight,
                Margin = new Padding(0, 0, 0, 40)
            };

            return breadcrumbPanel;
        }

        private void ButtonClicked_Save(object sender, EventArgs e)
        {
            try
            {
                var prefsPanel = currentSettingsPanel as SettingsPanel;
                prefsPanel?.Save();
                _isDirty = false;
                saveButton.Enabled = _isDirty;
                cancelButton.Enabled = _isDirty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving settings");
                MessageBox.Show("An error occurred while saving settings.", "Save Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonClicked_Cancel(object sender, EventArgs e)
        {
            try
            {
                if (CoreGlobals.AppPreferences != null)
                {
                    var defaultPrefs = ACATPreferences.LoadDefaultSettings() as IPreferences;
                    if (defaultPrefs != null)
                    {
                        CopyPreferencesValues(defaultPrefs, CoreGlobals.AppPreferences);
                    }
                }

                CancelExtensionChanges(_currentCategory);

                _isDirty = false;
                saveButton.Enabled = _isDirty;
                cancelButton.Enabled = _isDirty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while canceling changes");
                MessageBox.Show("An error occurred while canceling changes.", "Cancel Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonClicked_Exit(object sender, EventArgs e)
        {
            try
            {
                if (_isDirty)
                {
                    if (!ConfirmBoxTwoOption.ShowDialog("You have unsaved changes.",
                        "Save changes before exiting?", "Save", "Don't Save"))
                    {
                        var prefsPanel = currentSettingsPanel as SettingsPanel;
                        prefsPanel?.Save();
                        Close();
                    }
                    else
                    {
                        Close();
                        return;
                    }
                }
                else
                {
                    Close();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while exiting");
                MessageBox.Show("An error occurred while saving settings.", "Save Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private ScannerRoundedButtonControl CreateButton(string text, bool defaultenabled, EventHandler onClick = null)
        {
            var btn = new ScannerRoundedButtonControl()
            {
                Text = text,
                Font = new Font("Montserrat", 18, FontStyle.Regular, GraphicsUnit.Point),
                ForeColor = Color.White,
                BorderRadiusBottomLeft = 4,
                BorderRadiusTopRight = 4,
                BorderRadiusBottomRight = 4,
                BorderRadiusTopLeft = 4,
                Enabled = defaultenabled,
                AutoSize = true,
                Dock = DockStyle.Fill
            };

            if (onClick != null)
            {
                btn.Click += onClick;
            }

            return btn;
        }

        private Panel CreateTitleLabel()
        {
            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.Transparent,
                ColumnCount = 1,
                RowCount = 4,
            };

            // One column, full width
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            // Two auto-sizing rows
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var title = new Label
            {
                Text = "ACAT",
                Font = new Font("Montserrat Thin", 48f, GraphicsUnit.Point), // DPI aware
                ForeColor = Color.White,
                AutoSize = true,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.TopCenter,
                Margin = new Padding(0),
                BackColor = Color.Transparent
            };
           
            var subTitle = new Label
            {
                Text = "Settings",
                Font = new Font("Montserrat", 28f, FontStyle.Bold, GraphicsUnit.Point), // DPI aware
                ForeColor = Color.White,
                AutoSize = true,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(0),
            };

            var separator = new Panel
            {
                Height = 1,
                Dock = DockStyle.Top,
                BackColor = Color.White,
                Margin = new Padding(0, 38, 0, 38) // insets + spacing below
            };
            navPanel = new TableLayoutPanel
            {
                AutoSize = true,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 0, 0)
            };

            panel.Controls.Add(title, 0, 0);
            panel.Controls.Add(subTitle, 0, 1);
            panel.Controls.Add(separator, 0, 2);
            panel.Controls.Add(navPanel, 0, 3);

            return panel;
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            Text = "ACAT Settings";

            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            MinimumSize = new Size(1440, 1024);
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            MinimizeBox = true;
            BackColor = Color.FromArgb(31, 31, 56);
            ForeColor = Color.White;

            basePanel = new TableLayoutPanel
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                GrowStyle = TableLayoutPanelGrowStyle.AddRows,
            };

            basePanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            basePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            leftPanel = CreateLeftPanel();
            mainPanel = CreateMainPanel();

            basePanel.Controls.Add(leftPanel, 0, 0);
            basePanel.Controls.Add(mainPanel, 1, 0);

            Controls.Add(basePanel);
            LoadNavigation();
            ResumeLayout(false);
        }


        private void CancelExtensionChanges(string category)
        {
            var extensions = LoadSettings(category);

            if (extensions != null)
            {
                foreach (var extension in extensions)
                {
                    if (extension is ISupportsPreferences supportsPrefs)
                    {
                        var defaultPrefs = supportsPrefs.GetDefaultPreferences();
                        if (defaultPrefs != null)
                        {
                            var currentPrefs = supportsPrefs.GetPreferences();
                            if (currentPrefs != null)
                            {
                                CopyPreferencesValues(defaultPrefs, currentPrefs);
                               
                                currentPrefs.Save();
                            }
                        }
                    }
                }
            }
        }

        private void CopyPreferencesValues(IPreferences source, IPreferences target)
        {
            if (source == null || target == null) return;

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

                        if (targetProp != null && targetProp.CanWrite && targetProp.PropertyType == prop.PropertyType)
                        {
                            targetProp.SetValue(target, value);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogDebug("Could not copy property {PropertyName}: {Message}", prop.Name, ex.Message);
                    }
                }
            }
        }

        private object CreateExtensionInstance(Type type)
        {
            // Use shared helper method for consistent extension instantiation across all applications
            return ExtensionHelper.CreateExtensionInstance(_serviceProvider, type, _logger);
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
                       return Context.AppActuatorManager.ActuatorsList;
                    }
                    break;

                case "Word Predictors":
                    if (Context.AppWordPredictionManager.LoadExtensions(Context.ExtensionDirs))
                    {
                        var wordPredictorTypes = Context.AppWordPredictionManager.WordPredictorExtensions;
                        var wordPredictorExtensions = wordPredictorTypes
                            .Select(type => CreateExtensionInstance(type) as IExtension)
                            .Where(instance => instance != null);
                        return wordPredictorExtensions;

                    }
                    break;

                case "Text to Speech":
                    if (Context.AppTTSManager.LoadExtensions(Context.ExtensionDirs))
                    {
                        var ttsEngineTypes = Context.AppTTSManager.GetExtensions();
                        var ttsExtensions = ttsEngineTypes
                            .Select(type => CreateExtensionInstance(type) as IExtension)
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
                var btn = CreateButton(category, true, Category_Click);
                btn.Tag = (Category: category, Settings: LoadSettings(category));

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
                settingsPanel.Controls.Add(new Label { Text = "No settings available.", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter });
            }
        }

        private void Category_Click(object sender, EventArgs e)
        {
            if (sender is ScannerRoundedButtonControl clickedButton)
            {
                // Check if the clicked button is already selected  
                if (selectedCategoryButton == clickedButton)
                {
                    return; // Exit early if clicking on the current category  
                }


                if (selectedCategoryButton != null)
                {
                    selectedCategoryButton.BackColor = Color.Transparent; // Reset previous button color
                    selectedCategoryButton.BorderColor = Color.White; // Reset previous button color
                    selectedCategoryButton.ForeColor = Color.White;
                }

                selectedCategoryButton = clickedButton;
                selectedCategoryButton.BackColor = Color.FromArgb(255, 169, 0); // Highlight selected button

                // Make sure the button is flat and has no border
                clickedButton.FlatStyle = FlatStyle.Flat;
                clickedButton.FlatAppearance.BorderSize = 0;
                clickedButton.TabStop = false; // prevents focus rectangle

                // Highlight the button
                clickedButton.BackColor = Color.FromArgb(255, 169, 0);
                clickedButton.BorderColor = Color.FromArgb(255, 169, 0);
                clickedButton.ForeColor = Color.Black;

                // Remove focus immediately so the outline disappears
                this.ActiveControl = null;

                var (Category, Settings) = ((string Category, IEnumerable<IExtension> Settings))clickedButton.Tag;
                string category = Category;

                _currentCategory = category;

                if (Settings == null)
                {
                    Settings = Enumerable.Empty<IExtension>();
                }

                breadcrumbStack.Clear();
                //contentPanel.Controls.Clear();
                currentPageLabel = category;

                UserControl panel = category switch
                {
                    "General" => new SettingsPanel(ShowPanel, CoreGlobals.AppPreferences, SettingsChanged),
                    "Actuators" => new GroupedSettingsPanel(ShowPanel, Settings, SettingsChanged),
                    "Word Predictors" => new GroupedSettingsPanel(ShowPanel, Settings, SettingsChanged),
                    "Text to Speech" => new GroupedSettingsPanel(ShowPanel, Settings, SettingsChanged),
                    _ => throw new ArgumentException("Invalid category"),
                };

                ShowPanel(panel, category);
            }
        }

        private void SettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            _isDirty = true;
            saveButton.Enabled = _isDirty;
            cancelButton.Enabled = _isDirty;
        }


        public void ShowPanel(UserControl panel, string label)
        {

            if (settingsPanel.Controls.Count > 0)
            {
                var lastPanel = (UserControl)settingsPanel.Controls[0];
                var lastLabel = breadcrumbPanel.Controls.OfType<Control>().LastOrDefault(c => c is Label || c is LinkLabel)?.Text;

                breadcrumbStack.Add((lastPanel, lastLabel));
                settingsPanel.Controls.Clear();
            }

            currentPageLabel = label;
            currentSettingsPanel = panel;

            panel.Dock = DockStyle.Fill;
            settingsPanel.Controls.Add(panel);
            settingsPanel.DataBindings.Clear(); // Clear any existing bindings to avoid conflicts
            settingsPanel.DataBindings.Add("Tag", panel, "Tag", true, DataSourceUpdateMode.OnPropertyChanged);

            UpdateBreadcrumbTrail();
        }

        private void UpdateBreadcrumbTrail()
        {
            breadcrumbPanel.Controls.Clear();
            breadcrumbPanel.FlowDirection = FlowDirection.LeftToRight;
            breadcrumbPanel.WrapContents = false; // important: force single line
            breadcrumbPanel.AutoSize = true;

            for (int i = 0; i < breadcrumbStack.Count; i++)
            {
                // Add the breadcrumb link
                var link = new LinkLabel
                {
                    Text = breadcrumbStack[i].Label,
                    Tag = i,
                    Font = new Font("Montserrat", 32f, FontStyle.Bold, GraphicsUnit.Point),
                    LinkColor = Color.White,
                    ActiveLinkColor = Color.White,
                    VisitedLinkColor = Color.White,
                    LinkBehavior = LinkBehavior.NeverUnderline,
                    Cursor = Cursors.Default,
                    AutoSize = true,
                    Margin = new Padding(0)
                };

                link.Click += (s, e) =>
                {
                    int index = (int)((LinkLabel)s).Tag;
                    NavigateToBreadcrumb(index);
                };

                breadcrumbPanel.Controls.Add(link);

                // Add separator if not the last item
                if (i < breadcrumbStack.Count - 1)
                {
                    breadcrumbPanel.Controls.Add(new Label
                    {
                        Text = " > ",
                        AutoSize = true,
                        Font = new Font("Montserrat", 32f, FontStyle.Bold, GraphicsUnit.Point),
                        Margin = new Padding(0)
                    });
                }
            }

            // Add current page as plain text
            breadcrumbPanel.Controls.Add(new Label
            {
                Text = currentPageLabel,
                AutoSize = true,
                Font = new Font("Montserrat", 32f, FontStyle.Bold, GraphicsUnit.Point),
                Margin = new Padding(8, 0, 0, 0) // space after last separator
            });
        }

        private void NavigateToBreadcrumb(int index)
        {
            // Get the selected breadcrumb entry
            var (targetPanel, label) = breadcrumbStack[index];

            // Trim breadcrumbStack to one step before the target
            breadcrumbStack = breadcrumbStack.Take(index).ToList();

            currentPageLabel = label;

            settingsPanel.Controls.Clear();
            targetPanel.Dock = DockStyle.Fill;
            settingsPanel.Controls.Add(targetPanel);

            UpdateBreadcrumbTrail();
        }
    }
}