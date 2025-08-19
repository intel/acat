using ACAT.Applications;
using ACAT.Core.PanelManagement;
using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using ACATConfigNext.UserControls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ACAT.Core.Extensions;
using ACAT.Core.PreferencesManagement;
using System.ComponentModel;
using ACATResources;
using System.Reflection;
using ACAT.Extension;
using ACAT.Core.PreferencesManagement.Interfaces;

namespace ACATConfigNext.Forms
{
    public class SettingsForm : Form
    {
        private TableLayoutPanel basePanel;
        private FlowLayoutPanel leftPanel;
        private TableLayoutPanel navPanel;
        private TableLayoutPanel mainPanel;
        private FlowLayoutPanel breadcrumbPanel;
        private TableLayoutPanel contentPanel;
        private TableLayoutPanel bottomPanel;

        private ScannerRoundedButtonControl selectedCategoryButton;
        private Button saveButton;
        private Button cancelButton;
        private Button exitButton;

        private List<(UserControl Panel, string Label)> breadcrumbStack = new();
        private string currentPageLabel;
        private string _currentCategory;

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
            //MaximumSize = new Size(2000, 1400);
            Size = new Size(2000, 1400);
            MinimumSize = new Size(1000, 600);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            MinimizeBox = true;
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
                MaximumSize = new Size(2000, 1440),
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
                WrapContents = false, // stack vertically only
               // Padding = new Padding(10),
                 Padding = new Padding(left: 40, top: 48, right: 20, bottom: 48), // inner padding
                Margin = new Padding(0),

            };

            navPanel = new TableLayoutPanel
            {
               // Dock = DockStyle.Left,
                //*
                BackColor = Color.Transparent,
                /*/
                BackColor = Color.Blue,
                //*/
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 1,
                RowCount = 4,
                GrowStyle = TableLayoutPanelGrowStyle.AddRows,
                Margin = new Padding(0, 40, 0, 0)
            };

            // container that aligns acatlabel + navPanel in 1 column
            var centerPanel = new TableLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 1,
                RowCount = 2,
                Dock = DockStyle.Top,
                BackColor = Color.Transparent
            };
            centerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            var acatlabel = new Label
            {
                Text = "ACAT",
                Font = new Font("Montserrat Thin", 64),
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
                Margin = new Padding(0, 0, 0, 0) ,
                Height = 250,
                Width = 300
            };

            acatlabel.Paint += (s, e) =>
            {
                var settingsText = "Settings";
                using (var font = new Font("Montserrat", 38, FontStyle.Bold))
                using (var brush = new SolidBrush(Color.White))
                {
                    var textSize = e.Graphics.MeasureString(settingsText, font);
                    float x = (acatlabel.Width - textSize.Width) / 2;
                    float y = acatlabel.Font.Height * 1.53f; //overlap
                    e.Graphics.DrawString(settingsText, font, brush, x, y);
                }
            };


            centerPanel.Controls.Add(acatlabel, 0, 0);
            centerPanel.Controls.Add(navPanel, 0, 2);


            // centerPanel.SetColumnSpan(acatlabel, 1);
            // centerPanel.SetColumnSpan(navPanel, 1);

            acatlabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
          //  settingslabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            navPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            leftPanel.Controls.Add(centerPanel);

           // leftPanel.Controls.Add(acatlabel);
        //    leftPanel.Controls.Add(settingslabel);
          //  leftPanel.Controls.Add(navPanel);

         //   navPanel.Width = settingslabel.PreferredWidth;
          //  settingslabel.SizeChanged += (s, e) => navPanel.Width = settingslabel.Width;
         //  settingslabel.Width = acatlabel.Width;

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
                RowStyles = { new RowStyle(SizeType.AutoSize), new RowStyle(SizeType.Percent, 100F), new RowStyle(SizeType.AutoSize) },
                Padding = new Padding(left: 28, top: 48, right: 48, bottom: 48)
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
                /*/
                BackColor = Color.DeepPink,
                //*/
                Padding = new Padding(10),
                Margin = new Padding(10),
                RowCount = 1,
                ColumnCount = 1,
                GrowStyle = TableLayoutPanelGrowStyle.AddRows,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
            };

            bottomPanel = new TableLayoutPanel
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
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                    MessageBox.Show("An error occurred while saving settings.", "Save Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            cancelButton = new ScannerRoundedButtonControl()
            {
                Text = "Cancel",
                Font = new Font("Montserrat", 18, FontStyle.Italic),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Enabled = false
            };
            cancelButton.Click += (s, e) =>
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
                    Log.Exception(ex);
                    MessageBox.Show("An error occurred while canceling changes.", "Cancel Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            exitButton = new ScannerRoundedButtonControl()
            {
                Text = "Exit",
                Font = new Font("Montserrat", 18, FontStyle.Italic),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ForeColor = Color.White,
                Enabled = true
            };

            exitButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            exitButton.Click += (s, e) =>
            {
                try
                {
                    if (_isDirty)
                    {
                        if (!ConfirmBoxTwoOption.ShowDialog("You have unsaved changes.",
                            "Save changes before exiting?", "Don't Save", "Save"))
                        {
                            Close();
                            return;
                        }
                        else
                        {
                            var prefsPanel = currentSettingsPanel as SettingsPanel;
                            prefsPanel?.Save();
                            Close();
                        }
                    }
                    else
                    {
                        Close();
                    }
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                    MessageBox.Show("An error occurred while saving settings.", "Save Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };


            bottomPanel.ColumnCount = 4;
            bottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); 
            bottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); 
            bottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); 
            bottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); 


            bottomPanel.Controls.Add(saveButton, 0, 0);
            bottomPanel.Controls.Add(cancelButton, 1, 0);
            bottomPanel.Controls.Add(new Panel(), 2, 0);
            bottomPanel.Controls.Add(exitButton, 3, 0);

            bottomPanel.Controls.Add(saveButton);
            bottomPanel.Controls.Add(cancelButton);
            bottomPanel.Controls.Add(exitButton);


            mainPanel.Controls.Add(breadcrumbPanel, 0, 0);
            mainPanel.Controls.Add(contentPanel, 0, 1);
            mainPanel.Controls.Add(bottomPanel, 0, 2);

            basePanel.Controls.Add(leftPanel, 0, 0);
            basePanel.Controls.Add(mainPanel, 1, 0);
            Controls.Add(basePanel);

            LoadNavigation();
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
                        Log.Debug($"Could not copy property {prop.Name}: {ex.Message}");
                    }
                }
            }
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
                            .Select(type => Activator.CreateInstance(type) as IExtension)
                            .Where(instance => instance != null);
                        return wordPredictorExtensions;

                    }
                    break;

                case "Text to Speech":
                    if (Context.AppTTSManager.LoadExtensions(Context.ExtensionDirs))
                    {
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
                    Font = new Font("Montserrat", 18, FontStyle.Regular),
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    ForeColor = Color.White,
                    Dock = DockStyle.Top,
                    Tag = (Category: category, Settings: LoadSettings(category)),
                    FlatStyle = FlatStyle.Flat,
                 //   BorderRadiusBottomLeft = 0,
                 //   BorderRadiusBottomRight = 0,
                //    BorderRadiusTopLeft = 0,
                 //   BorderRadiusTopRight = 0,
                   // BorderWidth = 0F,
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
                //selectedCategoryButton.BackColor = Color.FromArgb(255, 169, 0); // Highlight selected button

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

                ShowPanel(panel, category);
            }
        }

        private void SettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            _isDirty = true;
            saveButton.Enabled = _isDirty;
            cancelButton.Enabled = _isDirty;
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