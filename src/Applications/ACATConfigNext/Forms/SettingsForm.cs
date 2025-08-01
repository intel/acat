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
using ACAT.Extension;
using System.ComponentModel;

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

        private UserControl currentSettingsPanel;

        private bool _isDirty = false;

        //Delegate for the event triggered when the user makes a change to a preference setting 
        public delegate void NotifyPreferencesChangeMade();

        //Event raised when the user makes a change to a preference setting 
        public event NotifyPreferencesChangeMade EvtPreferencesChangeMade;

        public SettingsForm()
        {
            WpfInitializationHelper.EnsureApplicationResources();

            InitializeComponent();

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
                    var prefsPanel = currentSettingsPanel as SettingsPanel;
                    prefsPanel?.Save();
                    _isDirty = false;
                    saveButton.Enabled = _isDirty;

                    //bool success = AppCommon.SaveAllPreferences();
                    //if (success)
                    //{
                    //    if (CoreGlobals.AppPreferences != null)
                    //    {
                    //        success &= CoreGlobals.AppPreferences.Save();
                    //    }
                    //    MessageBox.Show("Settings saved successfully.", "Save Complete",
                    //        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //}
                    //else
                    //{
                    //    MessageBox.Show("Some settings could not be saved. Please check the logs.",
                    //        "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //}
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
                       return Context.AppActuatorManager.ActuatorsList;
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
                    "General" => new SettingsPanel(ShowPanel, CoreGlobals.AppPreferences, SettingsChanged),
                    "Actuators" => new GroupedSettingsPanel(ShowPanel, Settings, SettingsChanged),
                    "Word Predictors" => new GroupedSettingsPanel(ShowPanel, Settings, SettingsChanged),
                    "Text to Speech" => new GroupedSettingsPanel(ShowPanel, Settings, SettingsChanged),
                    _ => throw new ArgumentException("Invalid category"),
                };

                //AttachInputEvents(panel);

                ShowPanel(panel, category);
            }
        }

        private void SettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            _isDirty = true;
            saveButton.Enabled = _isDirty;
            resetButton.Enabled = _isDirty;
            //EvtPreferencesChangeMade();
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
            currentSettingsPanel = panel;

            panel.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(panel);
            contentPanel.DataBindings.Clear(); // Clear any existing bindings to avoid conflicts
            contentPanel.DataBindings.Add("Tag", panel, "Tag", true, DataSourceUpdateMode.OnPropertyChanged);

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
    }
}