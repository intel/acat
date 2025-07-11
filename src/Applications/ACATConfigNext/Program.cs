//using ACAT.Core.WidgetManagement;
//using ACAT.Core.Widgets;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Windows.Forms;
using ACAT.Applications;
using ACAT.Lib.Core.ActuatorManagement;
using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.InputActuators;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PreferencesManagement;
using ACAT.Lib.Core.UserManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WordPredictionManagement;
using ACATResources;
using static ACAT.Lib.Core.Interpreter.Interpret;

namespace ACATConfigNext
{
    internal static class Program
    {
        public class SettingsForm : Form
        {
            private readonly Panel navPanel;
            private readonly Panel contentPanel;
            private readonly FlowLayoutPanel breadcrumbPanel;

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

                contentPanel = CustomControls.CreatePanel(DockStyle.Fill,0);
                breadcrumbPanel = CustomControls.CreateFlowPanel(DockStyle.Top,height: 40, text: "Settings", padding: new Padding(10, 5, 0, 0));
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
                    var btn = CustomControls.CreateFlatButton(text: category, tag: category, width: navPanel.Width - 20, top: y, left: 10,height:40);
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

            private void ShowPanel(UserControl panel, string label)
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
                var generalSettings = new GeneralSettingsCategory();

                var label = new Label { Text = "General Settings", Dock = DockStyle.Top, Height = 40 };
              
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
            //public IEnumerable<PreferencesCategory> PreferencesCategories = Context.AppActuatorManager.GetPreferencesSelectionForm;        
            public delegate void NotifySavePreferencesCategories(object sender, IEnumerable<PreferencesCategory> preferencesCategories);
            public delegate void PreferencesCategorySelected(object sender, ISupportsPreferences preferencesCategory);
            private TableLayoutPanel _tableLayoutPanel = CustomControls.CreateCategoryTableLayoutPanel();
            private Actuators _context = new Actuators();

            public IEnumerable<IActuator> GetActuatorList()
            {
                return ACAT.Lib.Core.PanelManagement.Context.AppActuatorManager.Actuators;
            }

            public IEnumerable<IActuator> GetActuatorListFromActuators(Actuators actuators)
            {
                return actuators.ActuatorList;
            }

            public List<IActuator> GetActuatorList(bool enabledOnly = false, Type specificType = null)
            {
                var actuators = ACAT.Lib.Core.PanelManagement.Context.AppActuatorManager.Actuators;
                var result = new List<IActuator>();

                foreach (var actuator in actuators)
                {
                    // Filter by enabled status if requested  
                    if (enabledOnly && !actuator.Enabled)
                        continue;

                    // Filter by specific type if requested  
                    if (specificType != null && actuator.GetType() != specificType)
                        continue;

                    result.Add(actuator);
                }

                return result;
            }

            public bool AllowMultiEnable { get; set; }

            public ActuatorSettingsPanel(Action<UserControl, string> showPanel)
            {
                Controls.Add(new Label { Text = "Actuator Settings", Dock = DockStyle.Top, Height = 40 });
                refreshPanel("ActuatorSettings.xml");
            }

            private void refreshPanel(string xmlpath)
            {
                if (CoreGlobals.AppPreferences == null)
                {
                    if (!AppCommon.LoadUserPreferences())
                    {
                        // Handle error - preferences couldn't be loaded  
                        return;
                    }
                }

                if (CoreGlobals.AppPreferences?.Extensions != null)
                {

                    #region C-MONDOS-TUFF
                    if (!ACAT.Lib.Core.PanelManagement.Context.AppActuatorManager.LoadExtensions(ACAT.Lib.Core.PanelManagement.Context.ExtensionDirs, true))
                    {
                        return;
                    }

                    _context.Load(ACAT.Lib.Core.PanelManagement.Context.ExtensionDirs, UserManager.GetFullPath(xmlpath), true);

                    var list = new List<PreferencesCategory>();

                    foreach (var preference in _context.ActuatorList)
                    {
                        list.Add(new PreferencesCategory(preference, true, preference.Enabled));
                    }

                    IEnumerable<PreferencesCategory> PreferencesCategories = list;

                    foreach (var category in PreferencesCategories)
                    {
                        if (!IsValidExtension(category, out var desc))
                            continue;

                        var categoryItem = CustomControls.CreateCategoryTableLayoutPanel();

                        categoryItem.Controls.Add(CustomControls.CreateLabel(desc.Name), 0, 0);  // title
                        categoryItem.Controls.Add(CustomControls.CreateDescriptionLabel(desc.Description), 0, 2);  // description

                        var checkBox = CustomControls.CreateCheckBox(">");
                        checkBox.Tag = category;
                        categoryItem.Controls.Add(checkBox, 1, 1);
                        categoryItem.SetRowSpan(checkBox, 2);

                        _tableLayoutPanel.Controls.Add(categoryItem);
                        Controls.Add(_tableLayoutPanel);
                    }

                    #endregion
                }
            }

            private bool IsValidExtension(PreferencesCategory category, out IDescriptor descriptor)
            {
                descriptor = null;


                var extension = category.PreferenceObj as IExtension;
                if (extension == null)
                    return false;

                descriptor = extension.Descriptor;
                return descriptor != null && descriptor.HasSettings;
            }
        }

        public class WordPredictorsPanel : UserControl
        {
            private TableLayoutPanel _tableLayoutPanel;
            private WordPredictors _context = new WordPredictors();


            public WordPredictorsPanel(Action<UserControl, string> showPanel)
            {
                Controls.Add(new Label { Text = "Word Predictors - Settings", Dock = DockStyle.Top, Height = 40 });
                refreshPanel();
            }

            private void refreshPanel()
            {
                if (CoreGlobals.AppPreferences == null)
                {
                    if (!AppCommon.LoadUserPreferences())
                    {
                        // Handle error - preferences couldn't be loaded  
                        return;
                    }
                }

                if (CoreGlobals.AppPreferences?.Extensions != null)
                {

                    #region C-MONDOS-TUFF
                    if (!ACAT.Lib.Core.PanelManagement.Context.AppWordPredictionManager.LoadExtensions(ACAT.Lib.Core.PanelManagement.Context.ExtensionDirs))
                    {
                        return;
                    }

                    _context.Load(ACAT.Lib.Core.PanelManagement.Context.ExtensionDirs, true);

                    var list = new List<PreferencesCategory>();

                    foreach (var preference in _context.Collection)
                    {
                        list.Add(new PreferencesCategory(preference, true,true));
                    }

                    IEnumerable<PreferencesCategory> PreferencesCategories = list;

                    foreach (var category in PreferencesCategories)
                    {
                        if (!IsValidExtension(category, out var desc))
                            continue;

                        var categoryItem = CustomControls.CreateCategoryTableLayoutPanel();

                        categoryItem.Controls.Add(CustomControls.CreateLabel(desc.Name), 0, 0);  // title
                        categoryItem.Controls.Add(CustomControls.CreateDescriptionLabel(desc.Description), 0, 2);  // description

                        var checkBox = CustomControls.CreateCheckBox(">");
                        checkBox.Tag = category;
                        categoryItem.Controls.Add(checkBox, 1, 1);
                        categoryItem.SetRowSpan(checkBox, 2);

                        _tableLayoutPanel.Controls.Add(categoryItem);
                        Controls.Add(_tableLayoutPanel);
                    }

                    #endregion
                }


            }

            private bool IsValidExtension(PreferencesCategory category, out IDescriptor descriptor)
            {
                descriptor = null;


                var extension = category.PreferenceObj as IExtension;
                if (extension == null)
                    return false;

                descriptor = extension.Descriptor;
                return descriptor != null && descriptor.HasSettings;
            }

        }

        public class TTSPanel : UserControl
        {
            public TTSPanel(Action<UserControl, string> showPanel)
            {
                Controls.Add(new Label { Text = "Privacy Settings", Dock = DockStyle.Top, Height = 40 });
            }
        }

        public class DisplaySettingsPanel : UserControl
        {
            public DisplaySettingsPanel()
            {
                Controls.Add(new Label { Text = "Display Settings Details", Dock = DockStyle.Top, Height = 40 });
            }
        }


        
    }
}
