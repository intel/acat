////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// PreferencesCategorySelectForm.cs
//
// Displays a list of categories allowing the user to enable/disable
// a category, change settings for a category etc. The category could
// be a word predictor, a spellchecker, actuator etc.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using ACATResources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ACAT.Lib.Core.PreferencesManagement
{
    public partial class PreferencesCategorySelectForm : Form
    {
        #region Properties
        public IEnumerable<PreferencesCategory> PreferencesCategories;         //List of preference categories to display
    
        public bool _isDirty = false;                                          //Did the user change anything in the form
        public bool AllowMultiEnable { get; set; }                           //Gets or sets the property on whether to allow enabling or disabling multiple categories or just one at a time (like a radio button)
        public bool ShowEnable { get; set; }                                 //Gets or sets whether to show the enable column
        public bool DisallowEnable { get; set; }                             //Gets or sets whether the Enable column should be readonly
        
        private bool _firstClientChangedCall = true;                         //Has first call to OnClientSizeChanged been made?

        private float _designTimeAspectRatio = 0.0f;                         //Aspect ratio of form at design time
        public String CategoryColumnHeaderText { get; set; }                 //Gets or sets the column header text for the category column
        public String ConfigureColumnHeaderText { get; set; }                //Gets or sets the column header text for the configure column
        public String DescriptionColumnHeaderText { get; set; }              //Gets or sets the column header text of the description column
        public String EnableColumnHeaderText { get; set; }                   //Gets or sets the column header text for enabling/disabling a category
        public String Title { get; set; }                                    //Gets or sets the title of the form
        public IntPtr ParentControlHandle { get; set; }                      //Gets or sets the handle of the parent control for the form

        #endregion

        #region events

        //Delegate for the event triggered when the user saves new preferences
        public delegate void NotifySavePreferencesCategories(object sender, IEnumerable<PreferencesCategory> preferencesCategories);
        //Event raised when preferences cateogry selected - show custom Preferences dialog or default Preferences edit form
        public delegate void PreferencesCategorySelected(object sender, ISupportsPreferences preferencesCategory);
        //Delegate for the event triggered when the user makes a change to a preference setting 
        public delegate void NotifyPreferencesChangeMade();
        //Event raised when the user selects Done and then elects to save changes
        public event NotifySavePreferencesCategories EvtSavePreferences;
        //Event raised when the user makes a change to a preference setting 
        public event NotifyPreferencesChangeMade EvtPreferencesChangeMade;

        public event PreferencesCategorySelected EvtPreferencesCategorySelected;

        #endregion

        #region controls

        private Button CreateSetupButton(PreferencesCategory category)
        {
            bool enabled = category.PreferenceObj is ISupportsPreferences prefs &&
                           (prefs.SupportsPreferencesDialog || prefs.GetPreferences() != null);

            var button = new Button
            {
                Text = ">",
                Font = new Font("Montserrat", 24, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                BackColor = Color.FromArgb(48, 49, 64),
                Enabled = enabled,
                FlatStyle = FlatStyle.Flat
            };

            button.FlatAppearance.BorderSize = 0;

            return button;
        }

        private CheckBox CreateCheckBox(PreferencesCategory category)
        {
            var checkBox = new CheckBox
            {
                Text = "Enable",
                Checked = category.Enable,
                Enabled = category.AllowEnable,
                Anchor = AnchorStyles.None,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 5)
            };

            checkBox.CheckedChanged += CheckBox_CheckedChanged;

            return checkBox;
        }

        private Label CreateDescriptionLabel(string description)
        {
            return new Label
            {
                Text = description,
                AutoSize = true,
                Font = new Font("Segoe UI", 20, FontStyle.Italic),
                ForeColor = Color.White,
                Margin = new Padding(0, 0, 0, 5)
            };
        }

        private Label CreateLabel(string text)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Font = new Font("Montserrat", 24, FontStyle.Bold),
                ForeColor = Color.White,
                Margin = new Padding(0, 0, 0, 5)
            };
        }

        private TableLayoutPanel CreateCategoryPanel()
        {
            var panel = new TableLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowOnly,
                Margin = new Padding(10),
                Padding = new Padding(10),
                BackColor = Color.FromArgb(48, 49, 64),
                Dock = DockStyle.Fill,
            };

            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F)); // Label + description
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));     // Checkbox
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));     // Setup button

            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));    // Row 0: Title
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Spacer row for centering
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));    // Row 2: Description

            return panel;
        }

        private TableLayoutPanel CreateFlowPanel()
        {
            return new TableLayoutPanel
            {
                BackColor = Color.Transparent,
                AutoSize = false,
                AutoScroll = false,
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 0,
                GrowStyle = TableLayoutPanelGrowStyle.AddRows
            };
        }

        private TableLayoutPanel _flowPanel;

        private Label CreateCategoryHeaderLabel(string title)
        {
            return new Label
            {
                Font = new Font("Montserrat", 28, FontStyle.Bold),
                ForeColor = Color.White,
                Text = title,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 0, 0, 10),
                Padding = new Padding(0, 0, 0, 10),
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                AutoSize = true
            };
        }

        #endregion

        //Initializes an instance of the class
        public PreferencesCategorySelectForm()
        {
            InitializeComponent();
            CenterToScreen();
            AllowMultiEnable = true;
            DisallowEnable = false;
            ShowEnable = true;
            Load += PreferencesSelectForm_Load;
        }

        //PreferencesSelectForm_Load handler for the form. Init the UI and populate the datagridview
        private void PreferencesSelectForm_Load(object sender, EventArgs eventArgs)
        {
            float currentAspectRatio = (float)ClientSize.Height / ClientSize.Width;

            if (_designTimeAspectRatio != 0.0f && currentAspectRatio != _designTimeAspectRatio)
            {
                ClientSize = new System.Drawing.Size(ClientSize.Width, (int)(_designTimeAspectRatio * ClientSize.Width));
            }

            Activate();

            CenterToScreen();

            if (!String.IsNullOrEmpty(Title))
            {
                Text = Title;
            }

            _isDirty = false;

            refreshPanel();

        }

        // Update the preferencesCategories list with the current state of the controls in the form
        private void updateDataFromUI()
        {
            if (_flowPanel == null) return;

            foreach (Control categoryPanel in _flowPanel.Controls)
            {
                if (categoryPanel is TableLayoutPanel tablePanel)
                {
                    foreach (Control ctrl in tablePanel.Controls)
                    {
                        if (ctrl is CheckBox cb && cb.Tag is PreferencesCategory category)
                        {
                            category.Enable = cb.Checked;
                            break;
                        }
                    }
                }
            }
            /*
            for (int ii = 0; ii < dataGridView2.Rows.Count; ii++)
            {
                var category = dataGridView2.Rows[ii].Tag as PreferencesCategory;
                if (category != null)
                {
                    category.Enable = (Boolean)dataGridView2[EnableColumn.Name, ii].Value;
                }
            }

            */
        }

        //Refreshes the Panel with data from the Categories
        private void refreshPanel()
        {

            if (_flowPanel == null)
            {
                _flowPanel = CreateFlowPanel();
                var parent = dataGridView2.Parent;
                parent.Controls.Remove(dataGridView2);
                parent.Controls.Add(_flowPanel);
            }

            _flowPanel.Controls.Clear(); // clear old category rows

            var headerLabel = CreateCategoryHeaderLabel(this.AccessibilityObject.Name);
            _flowPanel.Controls.Add(headerLabel);

            foreach (var category in PreferencesCategories)
            {
                if (!IsValidExtensionCategory(category, out var desc))
                    continue;

                var categoryItem = CreateCategoryPanel();
                categoryItem.Controls.Add(CreateLabel(desc.Name), 0, 0);  // title
                categoryItem.Controls.Add(CreateDescriptionLabel(desc.Description), 0, 2);  // description

                var checkBox = CreateCheckBox(category);
                checkBox.Tag = category;
                checkBox.CheckedChanged += CheckBox_CheckedChanged;
                categoryItem.Controls.Add(checkBox, 1, 1);
                categoryItem.SetRowSpan(checkBox, 2);

                var setupButton = CreateSetupButton(category);
                categoryItem.Controls.Add(setupButton, 2, 0);
                categoryItem.SetRowSpan(setupButton, 3);

                _flowPanel.Controls.Add(categoryItem);
            }

            //// Sort first column ascending everytime grid is refreshed
            //dataGridView2.Sort(CategoryNameColumn, ListSortDirection.Ascending);
            //dataGridView2.AutoResizeRows();

            //// Wrap text everytime grid is refreshed
            //wrapText(true);

            //if (dataGridView2.Rows.Count > 0)
            //{
            //    dataGridView2.CurrentCell = dataGridView2.Rows[0].Cells[0];
            //    dataGridView2.Rows[0].Selected = true;
            //}
        }

        private bool IsValidExtensionCategory(PreferencesCategory category, out IDescriptor descriptor)
        {
            descriptor = null;

            var extension = category.PreferenceObj as IExtension;
            if (extension == null)
                return false;

            descriptor = extension.Descriptor;
            return descriptor != null && descriptor.HasSettings;
        }

        //Check if form filled correctly, if not, return false If form validated, send event notifying that preferences are to be saved
        public bool validateAndSave()
        {
            // Form not validated / filled correctly - immediately return false
            if (!validate())
            {
                return false; // return false - keep form open
            }

            // Form validated / filled correctly
            updateDataFromUI();
            DialogResult = DialogResult.OK;

            // Send event notification that preferences are to be saved
            EvtSavePreferences?.Invoke(this, this.PreferencesCategories);

            return true;
        }

        //Perform validation to make sure everything is oK Display error if validation failed
        private bool validate()
        {
            if (AllowMultiEnable)
            {
                return true;
            }

            foreach (Control control in _flowPanel.Controls)
            {
                if (control is TableLayoutPanel categoryPanel)
                {
                    foreach (Control innerControl in categoryPanel.Controls)
                    {
                        if (innerControl is CheckBox checkBox &&
                            checkBox.Tag is PreferencesCategory category &&
                            checkBox.Checked)
                        {
                            return true; // At least one is enabled
                        }
                    }
                }
            }

            /*
            for (int ii = 0; ii < dataGridView2.Rows.Count; ii++)
            {
                if ((Boolean)dataGridView2[EnableColumn.Name, ii].Value)
                {
                    return true;
                }
            }

            */
            ConfirmBoxOneOption.ShowDialog("You must enable at least one as default.", "", StringResources.OK, this, true);

            return false;
        }
        //Turns wrapping on /off in the rows
        public void wrapText(bool onOff)
        {
            foreach (Control control in _flowPanel.Controls)
            {
                if (control is TableLayoutPanel categoryPanel)
                {
                    foreach (Control inner in categoryPanel.Controls)
                    {
                        if (inner is Label label)
                        {
                            label.AutoSize = false;
                            label.MaximumSize = onOff ? new Size(categoryPanel.Width - 10, 0) : Size.Empty;
                            label.AutoEllipsis = !onOff;
                        }
                    }
                }
            }

            _flowPanel.PerformLayout();
            /*
            DataGridViewTextBoxColumn tbc = dataGridView2.Columns[1] as DataGridViewTextBoxColumn;
            tbc.DefaultCellStyle.WrapMode = (onOff) ? DataGridViewTriState.True : DataGridViewTriState.False;
            dataGridView2.AutoResizeRows();
            */
        }

        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox cb && cb.Tag is PreferencesCategory category)
            {
                // Update the data model
                category.Enable = cb.Checked;

                // Mark as dirty and trigger change event
                _isDirty = true;
                EvtPreferencesChangeMade?.Invoke();

                // If multi-enable is not allowed, uncheck all others
                if (!AllowMultiEnable && cb.Checked)
                {
                    foreach (Control control in _flowPanel.Controls)
                    {
                        if (control is TableLayoutPanel panel)
                        {
                            foreach (Control subControl in panel.Controls)
                            {
                                if (subControl is CheckBox otherCb &&
                                    otherCb != cb &&
                                    otherCb.Checked)
                                {
                                    otherCb.CheckedChanged -= CheckBox_CheckedChanged; // Prevent recursion
                                    otherCb.Checked = false;
                                    otherCb.CheckedChanged += CheckBox_CheckedChanged;
                                }
                            }
                        }
                    }
                }
            }
        }
        // User clicked wrap text checkbox
        public void checkBoxWrapText_CheckedChanged(object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(CheckBox))
            {
                bool doWrapText = ((CheckBox)sender).Checked;
                wrapText(doWrapText);
            }
        }


        #region CuelloButNoYet
        //Client size changed
        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);
            if (_firstClientChangedCall)
            {
                _designTimeAspectRatio = (float)ClientSize.Height / ClientSize.Width;
                _firstClientChangedCall = false;
            }
        }

        // If the user clicked in a cell.  If its is the Configure column, bring up the preferences form for the category so the user can set the preferences for that category
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (e.RowIndex >= 0 && senderGrid.Columns[e.ColumnIndex] == ConfigureColumn)
            {
                var tag = senderGrid.Rows[e.RowIndex].Tag;
                if (!(tag is PreferencesCategory))
                {
                    return;
                }

                var category = tag as PreferencesCategory;
                if (category.PreferenceObj is ISupportsPreferences)
                {

                    // Call event notifying that new preferences cateogry selected - handler set in ACATConfigMainForm.cs
                    EvtPreferencesCategorySelected(this, (ISupportsPreferences)category.PreferenceObj);
                    return;

                }
            }
        }

        //If the user clicked on the Enable column, and if AllowMultiEnable is false, then make sure only one cell is checked in the column
        private void dataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = dataGridView2;

            if (e.RowIndex < 0)
            {
                return;
            }

            if (senderGrid.Columns[e.ColumnIndex] == EnableColumn && !AllowMultiEnable)
            {
                var row = dataGridView2.Rows[e.RowIndex];

                var checkCell = (DataGridViewCheckBoxCell)row.Cells[e.ColumnIndex];

                bool isChecked = (Boolean)checkCell.Value;
                if (isChecked)
                {
                    for (int ii = 0; ii < senderGrid.Rows.Count; ii++)
                    {
                        if (ii != e.RowIndex)
                        {
                            dataGridView2[e.ColumnIndex, ii].Value = false;
                        }
                    }
                }

                dataGridView2.Invalidate();
            }

        }

        // Dirty state changed
        private void dataGridView2_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView2.IsCurrentCellDirty)
            {
                dataGridView2.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }

            _isDirty = true;
            EvtPreferencesChangeMade();
        }


        //Initializes the UI controls
        private void initializeUI()
        {
            dataGridView2.AutoResizeRows();

            CategoryNameColumn.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            dataGridView2.ScrollBars = ScrollBars.Vertical;
            dataGridView2.RowHeadersVisible = false;

            dataGridView2.CellContentClick += dataGridView2_CellContentClick;
            dataGridView2.CellValueChanged += dataGridView2_CellValueChanged;

            Paint += (s, args) =>
            {
                if (dataGridView2 != null)
                {
                    dataGridView2.CurrentCellDirtyStateChanged += dataGridView2_CurrentCellDirtyStateChanged;
                }
            };
        }
        #endregion
    }
}