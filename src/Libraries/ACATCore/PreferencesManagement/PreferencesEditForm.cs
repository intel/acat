////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// PreferencesEditForm.cs
//
// A generic preferences editor for a class that
// has fields and properties which are intergers,
// strings, bool or floats. Picks those fields and
// properties which are qualified by custom attributes
// (BoolDescritpor, IntDescriptor etc). Displays the
// settings as a gridview. Does validation of data
// to make sure it is within range etc.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PreferencesManagement.UI;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using ACATResources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Threading;

namespace ACAT.Lib.Core.PreferencesManagement
{
    public partial class PreferencesEditForm : Form
    {
        #region Properties

        public IPreferences DefaultPreferences;                                 //Default values for the preferences

        public IPreferences Preferences;                                        //The preferences object
        public ISupportsPreferences SupportsPreferencesObj { get; set; }        //Gets or sets the preferences object

        public interface IPreferenceEditor
        {
            string PropertyName { get; }
            object GetValue();
        }

        public bool _isDirty = false;                                           //Did the user change anything in the form

        private float _designTimeAspectRatio = 0.0f;                            //Aspect ratio of form at design time
                                  
        private bool _firstClientChangedCall = true;                            //Has first call to OnClientSizeChanged been made? 

        public bool _wrapText = true;                                           // Whether the text should be wrapped or not

        public String Title { get; set; }                                       //Gets or sets the title / text for header of settings column of the form

        #endregion

        //Delegate for the event triggered when the user makes a change to a preference setting 
        public delegate void NotifyPreferencesChangeMade();

        //Event raised when the user makes a change to a preference setting 
        public event NotifyPreferencesChangeMade EvtPreferencesChangeMade;

        //Initializes an instance of the class
        public PreferencesEditForm()
        {
            InitializeComponent();

            // For WPF Controls
            if (System.Windows.Application.Current == null)
            {
                new System.Windows.Application();
            }

            Text = "Settings";
            Load += PreferencesEditForm_Load;
        }

        #region Controls

        private FlowLayoutPanel CreateFlowPanel()
        {
            var panel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                BackColor = Color.Transparent,
                AutoSize = false,
                AutoScroll = false,
                Dock = DockStyle.Fill
            };

            panel.HorizontalScroll.Visible = false;
            panel.HorizontalScroll.Maximum = 0;

            return panel;
        }

        private FlowLayoutPanel _flowPanel;

        private Label CreateLabel(string text, int fontSize, FontStyle fontStyle)
        {
            return new Label
            {
                AutoSize = true,
                Dock = DockStyle.Fill,
                Text = text,
                Font = new Font("Montserrat", fontSize, fontStyle),
                ForeColor = Color.White
            };
        }

        #endregion

        //Check if form filled correctly, if not, return false If validated, check if changes have been made to form and if so prompt user asking if they want to save
        public bool validateAndSave()
        {
            updatePreferences();            //Update preferences based on latest values then save
            Preferences.Save();             //Save preferences

            return true;
        }

        // User clicked wrap text checkbox
        public void checkBoxWrapText_CheckedChanged(object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(CheckBox))
            {
                _wrapText = ((CheckBox)sender).Checked;
                wrapText(_wrapText);
            }

        }

        // User clicked Defaults button
        public void buttonDefaults_Click(object sender, EventArgs e)
        {
            if (ConfirmBoxTwoOption.ShowDialog("Restore default settings?", 
                "This cannot be undone.", StringResources.Yes, StringResources.No, this, true))
            {
                _isDirty = true;
                refreshPanel(DefaultPreferences);
                EvtPreferencesChangeMade();
            }
        }

        //Gets a yes/no response
        private bool confirm(String prompt)
        {
            return ConfirmBoxTwoOption.ShowDialog(prompt.ToString(), "",
                StringResources.Yes, StringResources.No, this, true);
        }

        //Here's where checking is done on the validity of the data If it is an integer for eg, make sure that all the text in the cell are digits and that the integer is within range.
        private void dataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            var senderGrid = (DataGridView)sender;
            e.Cancel = false;
            String name = null;
            String newVal = null;
            var cell = senderGrid[e.ColumnIndex, e.RowIndex];
            if (e.ColumnIndex == 2 && e.RowIndex >= 0 && cell is DataGridViewTextBoxCell)
            {
                var textBox = cell as DataGridViewTextBoxCell;
                String value = textBox.EditedFormattedValue as string;

                string prevVal;
                string defaultVal;
                // Cell is PropertyInfo
                if (senderGrid.Rows[e.RowIndex].Tag is PropertyInfo)
                {
                    PropertyInfo property = senderGrid.Rows[e.RowIndex].Tag as PropertyInfo;
                    name = property.Name;
                    prevVal = property.GetValue(Preferences, null).ToString();
                    defaultVal = getDefaultValue(name);

                    // Property is integer type
                    if (isInt(property))
                    {

                        // Based on new value set by user, show error status if needed and set value which cell will be automatically set to
                        if (Int32.TryParse(value, out int intValue))
                        {
                            var intDescriptor = getIntAttribute(property);
                            if (intDescriptor != null)
                            {
                                if (intValue < intDescriptor.MinValue || intValue > intDescriptor.MaxValue)
                                {
                                    e.Cancel = true;
                                    showErrorStatus(name, "Out of range");
                                    newVal = prevVal;
                                }
                            }
                        }
                        else
                        {
                            e.Cancel = true;
                            showErrorStatus(name, "Must be numeric");
                            newVal = prevVal;
                        }

                        // If previous cell value not valid, set to default value
                        if (String.IsNullOrEmpty(newVal))
                        {
                            newVal = defaultVal;
                        }
                        else if (!String.IsNullOrEmpty(newVal))
                        {
                            if (Int32.TryParse(newVal, out int intValue2))
                            {
                                var intDescriptor = getIntAttribute(property);
                                if (intDescriptor != null)
                                {
                                    if (intValue2 < intDescriptor.MinValue || intValue2 > intDescriptor.MaxValue)
                                    {
                                        newVal = defaultVal;
                                    }
                                }
                            }
                            else
                            {
                                newVal = defaultVal;
                            }
                        }
                    }


                    // Property is float type
                    else if (isFloat(property))
                    {

                        // Based on new value set by user, show error status if needed and set value which cell will be automatically set to
                        try
                        {
                            var floatDescriptor = getFloatAttribute(property);
                            var floatValue = float.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
                            if (floatValue < floatDescriptor.MinValue || floatValue > floatDescriptor.MaxValue)
                            {
                                e.Cancel = true;
                                showErrorStatus(name, "Out of range");
                                newVal = prevVal;
                            }
                        }
                        catch
                        {
                            e.Cancel = true;
                            showErrorStatus(name, "Must be numeric");
                            newVal = prevVal;
                        }

                        // If previous cell value not valid, set to default value
                        if (String.IsNullOrEmpty(newVal))
                        {
                            newVal = defaultVal;
                        }
                        else if (!String.IsNullOrEmpty(newVal))
                        {
                            try
                            {
                                var floatDescriptor = getFloatAttribute(property);
                                var floatValue = float.Parse(newVal, CultureInfo.InvariantCulture.NumberFormat);
                                if (floatValue < floatDescriptor.MinValue || floatValue > floatDescriptor.MaxValue)
                                {
                                    newVal = defaultVal;
                                }
                            }
                            catch
                            {
                                newVal = defaultVal;
                            }
                        }

                    }
                }

                // Cell is FieldInfo
                else if (senderGrid.Rows[e.RowIndex].Tag is FieldInfo)
                {
                    FieldInfo fieldInfo = senderGrid.Rows[e.RowIndex].Tag as FieldInfo;
                    name = fieldInfo.Name;
                    prevVal = fieldInfo.GetValue(Preferences).ToString();
                    defaultVal = getDefaultValue(name);

                    // Field is integer type
                    if (isInt(fieldInfo))
                    {

                        // Based on new value set by user, show error status if needed and set value which cell will be automatically set to
                        if (Int32.TryParse(value, out int intValue))
                        {
                            var intDescriptor = getIntAttribute(fieldInfo);
                            if (intDescriptor != null)
                            {
                                if (intValue < intDescriptor.MinValue || intValue > intDescriptor.MaxValue)
                                {
                                    e.Cancel = true;
                                    showErrorStatus(name, "Out of range");
                                    newVal = prevVal;
                                }
                            }
                        }
                        else
                        {
                            e.Cancel = true;
                            showErrorStatus(name, "Must be numeric");
                            newVal = prevVal;
                        }

                        // If previous cell value not valid, set to default value
                        if (String.IsNullOrEmpty(newVal))
                        {
                            newVal = defaultVal;
                        }
                        else if (!String.IsNullOrEmpty(newVal))
                        {
                            if (Int32.TryParse(newVal, out int intValue2))
                            {
                                var intDescriptor = getIntAttribute(fieldInfo);
                                if (intDescriptor != null)
                                {
                                    if (intValue2 < intDescriptor.MinValue || intValue2 > intDescriptor.MaxValue)
                                    {
                                        newVal = defaultVal;
                                    }
                                }
                            }
                            else
                            {
                                newVal = defaultVal;
                            }
                        }
                    }

                    // Field is float type
                    else if (isFloat(fieldInfo))
                    {

                        // Based on new value set by user, show error status if needed and set value which cell will be automatically set to
                        try
                        {
                            var floatDescriptor = getFloatAttribute(fieldInfo);
                            var floatValue = float.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
                            if (floatValue < floatDescriptor.MinValue || floatValue > floatDescriptor.MaxValue)
                            {
                                e.Cancel = true;
                                showErrorStatus(name, "Out of range");
                                newVal = prevVal;
                            }
                        }
                        catch
                        {
                            e.Cancel = true;
                            showErrorStatus(name, "Must be numeric");
                            newVal = prevVal;
                        }

                        // If previous cell value not valid, set to default value
                        if (String.IsNullOrEmpty(newVal))
                        {
                            newVal = defaultVal;
                        }
                        else if (!String.IsNullOrEmpty(newVal))
                        {
                            try
                            {
                                var floatDescriptor = getFloatAttribute(fieldInfo);
                                var floatValue = float.Parse(newVal, CultureInfo.InvariantCulture.NumberFormat);
                                if (floatValue < floatDescriptor.MinValue || floatValue > floatDescriptor.MaxValue)
                                {
                                    newVal = defaultVal;
                                }
                            }
                            catch
                            {
                                newVal = defaultVal;
                            }
                        }
                    }
                }
            }


            if (e.Cancel)
            {
                if (!String.IsNullOrEmpty(name) && !String.IsNullOrEmpty(newVal))
                {
                    ((DataGridViewTextBoxCell)cell).Value = newVal;
                    ((DataGridView)sender).RefreshEdit();
                }

            }

        }

        //Something changed. Set dirty flag
        private void DataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            _isDirty = true;
            EvtPreferencesChangeMade();
        }

        //Something changed. Set dirty flag
        private void DataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            _isDirty = true;
            EvtPreferencesChangeMade();
        }

        //Returns the default value for the specified field
        private String getDefaultValue(String fieldName)
        {
            var members = DefaultPreferences.GetType().GetMembers();
            foreach (var memberInfo in members)
            {
                var name = memberInfo.Name;
                if (String.Compare(name, fieldName) != 0)
                {
                    continue;
                }

                MemberInfo[] member = DefaultPreferences.GetType().GetMember(name);
                if (member.Length == 0)
                {
                    continue;
                }

                switch (member[0].MemberType)
                {
                    case MemberTypes.Field:
                        FieldInfo fieldInfo = DefaultPreferences.GetType().GetField(name);
                        return fieldInfo.GetValue(DefaultPreferences).ToString();

                    case MemberTypes.Property:
                        var property = DefaultPreferences.GetType().GetProperty(name);
                        return property.GetValue(DefaultPreferences, null).ToString();
                }
            }

            return String.Empty;
        }

        //Returns the field info for the specified field name
        private FieldInfo getField(object obj, String name)
        {
            return obj.GetType().GetFields().FirstOrDefault(field => String.Compare(field.Name, name) == 0);
        }

        //Returns the custom attribute for a float field
        private FloatDescriptorAttribute getFloatAttribute(FieldInfo field)
        {
            var attributes = field.GetCustomAttributes(false);
            foreach (var attribute in attributes)
            {
                if (attribute.GetType() == typeof(FloatDescriptorAttribute))
                {
                    return (FloatDescriptorAttribute)attribute;
                }
            }

            return null;
        }

        //Returns the custom attribute for a float property
        private FloatDescriptorAttribute getFloatAttribute(PropertyInfo property)
        {
            var attributes = property.GetCustomAttributes(false);

            foreach (var attribute in attributes)
            {
                if (attribute.GetType() == typeof(FloatDescriptorAttribute))
                {
                    return (FloatDescriptorAttribute)attribute;
                }
            }

            return null;
        }

        /// Returns the custom attribute for an integer field
        private IntDescriptorAttribute getIntAttribute(FieldInfo field)
        {
            var attributes = field.GetCustomAttributes(false);
            foreach (var attribute in attributes)
            {
                if (attribute.GetType() == typeof(IntDescriptorAttribute))
                {
                    return (IntDescriptorAttribute)attribute;
                }
            }

            return null;
        }

        //Returns the custom attribute for a integer property
        private IntDescriptorAttribute getIntAttribute(PropertyInfo property)
        {
            var attributes = property.GetCustomAttributes(false);

            foreach (var attribute in attributes)
            {
                if (attribute.GetType() == typeof(IntDescriptorAttribute))
                {
                    return (IntDescriptorAttribute)attribute;
                }
            }

            return null;
        }

        //Returns the property info for the specified property
        private PropertyInfo getProperty(object obj, String name)
        {
            return obj.GetType().GetProperties().FirstOrDefault(property => String.Compare(property.Name, name) == 0);
        }

        //Formats the datagridview>
        private void initializeGridView()
        {
            dataGridView.RowHeadersVisible = false;
            dataGridView.ScrollBars = ScrollBars.Vertical;

            SettingColumn.Width = (dataGridView.Width) / 5;
            DescriptionColumn.Width = dataGridView.Width / 5;
            ValueColumn.Width = dataGridView.Width / 5;
            DefaultColumn.Width = dataGridView.Width / 5;
            RangeColumn.Width = dataGridView.Width / 5;


            dataGridView.Sort(SettingColumn, ListSortDirection.Ascending);
            SettingColumn.HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Ascending;

            DescriptionColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            ValueColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            DefaultColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            RangeColumn.SortMode = DataGridViewColumnSortMode.NotSortable;

            dataGridView.CellValidating += dataGridView_CellValidating;
        }

        //Form loader.  Initialize the grid and populate it
        private void PreferencesEditForm_Load(object sender, EventArgs e)
        {
            float currentAspectRatio = (float)ClientSize.Height / ClientSize.Width;

            if (_designTimeAspectRatio != 0.0f && currentAspectRatio != _designTimeAspectRatio)
            {
                ClientSize = new System.Drawing.Size(ClientSize.Width, (int)(_designTimeAspectRatio * ClientSize.Width));
            }

            Activate();

            CenterToScreen();

           initializeGridView();

            if (!String.IsNullOrEmpty(Title))
            {
                Text = Title;
                SettingColumn.HeaderText = Title;
            }

            // Get Preferences and DefaultPreferences
            Preferences = SupportsPreferencesObj.GetPreferences();
            DefaultPreferences = SupportsPreferencesObj.GetDefaultPreferences();

            _isDirty = false;

            // Refresh grid view and set handlers which change _dirty flag after form has been fully painted / shown
            Paint += (s, args) =>
            {
                refreshPanel(Preferences);

                if (dataGridView != null)
                {
                    dataGridView.CellValueChanged += DataGridView_CellValueChanged;
                    dataGridView.CurrentCellDirtyStateChanged += DataGridView_CurrentCellDirtyStateChanged;
                }
            };
        }

        private void ReplaceDataGridWith(Control newControl)
        {
            var parent = dataGridView.Parent;
            parent.Controls.Remove(dataGridView);
            parent.Controls.Add(newControl);
        }

        private void AddPreferencePropertiesToPanel(FlowLayoutPanel panel, IPreferences prefs)
        {
            var props = prefs.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var builder = new SettingsPanelBuilder();

            foreach (var prop in props)
            {
                var propPanel = builder.CreateLabeledPanel(prop, prefs);

                var host = new ElementHost
                {
                    Child = propPanel,
                    AutoSize = true,
                    Margin = new Padding(10),
                    Dock = DockStyle.Top
                };

                panel.Controls.Add(host);
            }
        }

        //Populates the grid view with preferences data
        private void refreshPanel(IPreferences prefs)
        {
            if (_flowPanel == null)
            {
                _flowPanel = CreateFlowPanel();
            }

            ReplaceDataGridWith(_flowPanel);
            wrapText(_wrapText);

            var descriptor = prefs.GetType().GetCustomAttribute<DescriptorAttribute>();

            _flowPanel.Controls.Add(CreateLabel(descriptor?.Category ?? "UNKNOWN CATEGORY", 24, FontStyle.Bold));
            _flowPanel.Controls.Add(CreateLabel(descriptor?.Description ?? "UNKNOWN DESCRIPTION", 20, FontStyle.Regular));

            AddPreferencePropertiesToPanel(_flowPanel, prefs);
        }

        //Displays a error status mesage
        private void showErrorStatus(String settingName, String status)
        {
            using (var confirmBox = new ConfirmBoxOneOption
            {
                Prompt = $"Error\n{settingName}\n{status}",DecisionPrompt = "OK"
            })
            {
                confirmBox.ShowDialog(this);
            }
        }

        private void SavePreferencesFromPanel(FlowLayoutPanel panel, IPreferences prefs)
        {
            var props = prefs.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                foreach (Control control in panel.Controls)
                {
                    var host = control as ElementHost;
                    if (host?.Child == null)
                        continue;

                    var editor = host.Child as IPreferenceEditor;
                    if (editor == null)
                        continue;

                    if (editor.PropertyName == prop.Name)
                    {
                        object value = editor.GetValue();

                        try
                        {
                            prop.SetValue(prefs, value);
                        }
                        catch
                        {
                        }

                        break;
                    }
                }
            }
        }

        //Updates preferneces using the data in the grid view
        private void updatePreferences()
        {
            // Iterate over each row in the DataGridView
            foreach (DataGridViewRow row in dataGridView.Rows)
            {

                string name = row.Cells[SettingColumn.Name].Value as string;                // Get the setting name from the "Setting" column
                var valueCell = row.Cells[ValueColumn.Name];                                // Get the value cell from the "Value" column
                object editedValue = valueCell.EditedFormattedValue;                       // Get the edited value entered by the user
                FieldInfo field = getField(Preferences, name);                            // Try to get a field from the Preferences object that matches the setting name
                PropertyInfo property = null;

                // If no field is found, try getting a property instead
                if (field == null)
                {
                    property = getProperty(Preferences, name);
                }

                // If neither a field nor a property is found, skip this row
                if (field == null && property == null)
                {
                    continue;
                }
                Type memberType = null;                                               // Determine the type of the target member (field or property)
                if (field != null)
                {
                    memberType = field.FieldType;
                }
                else
                {
                    memberType = property.PropertyType;
                }

                object parsedValue = null;                                        // Will store the parsed and converted value (if successful)

                try
                {
                    // Handle integer values from text box cells
                    if (memberType == typeof(int) && valueCell is DataGridViewTextBoxCell)
                    {
                        int intValue;
                        if (int.TryParse(editedValue as string, out intValue))
                        {
                            parsedValue = intValue;
                        }
                    }
                    // Handle boolean values from checkbox cells
                    else if (memberType == typeof(bool) && valueCell is DataGridViewCheckBoxCell)
                    {
                        parsedValue = editedValue;
                    }
                    // Handle string values from text box cells
                    else if (memberType == typeof(string) && valueCell is DataGridViewTextBoxCell)
                    {
                        parsedValue = editedValue as string;
                    }
                    // Handle float values from text box cells
                    else if (memberType == typeof(float) && valueCell is DataGridViewTextBoxCell)
                    {
                        float floatValue;
                        if (float.TryParse(editedValue as string, NumberStyles.Float, CultureInfo.InvariantCulture, out floatValue))
                        {
                            parsedValue = floatValue;
                        }
                    }

                    // If a valid value was parsed, apply it to the Preferences object
                    if (parsedValue != null)
                    {
                        if (field != null)
                        {
                            field.SetValue(Preferences, parsedValue);
                        }
                        else
                        {
                            property.SetValue(Preferences, parsedValue);
                        }
                    }
                }
                catch
                {
                    // Silently ignore parsing errors; optionally log if needed
                }
            }


            /*
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                String name = row.Cells[SettingColumn.Name].Value as String;
                var valueCell = row.Cells[ValueColumn.Name];

                FieldInfo field = getField(Preferences, name);
                if (field != null)
                {
                    if (isInt(field) && valueCell is DataGridViewTextBoxCell)
                    {
                        if (Int32.TryParse(valueCell.EditedFormattedValue as String, out int intValue))
                        {
                            field.SetValue(Preferences, intValue);
                        }
                    }
                    else if (isBool(field) && valueCell is DataGridViewCheckBoxCell)
                    {
                        field.SetValue(Preferences, (valueCell as DataGridViewCheckBoxCell).EditedFormattedValue);
                    }
                    else if (isString(field) && valueCell is DataGridViewTextBoxCell)
                    {
                        field.SetValue(Preferences, valueCell.EditedFormattedValue);
                    }
                    else if (isFloat(field) && valueCell is DataGridViewTextBoxCell)
                    {
                        try
                        {
                            var floatValue = float.Parse(valueCell.EditedFormattedValue as String, CultureInfo.InvariantCulture.NumberFormat);
                            field.SetValue(Preferences, floatValue);
                        }
                        catch
                        {
                        }
                    }
                }
                else
                {
                    PropertyInfo property = getProperty(Preferences, name);
                    if (property != null)
                    {
                        if (isInt(property) && valueCell is DataGridViewTextBoxCell)
                        {
                            if (Int32.TryParse(valueCell.EditedFormattedValue as String, out int intValue))
                            {
                                property.SetValue(Preferences, intValue);
                            }
                        }
                        else if (isBool(property) && valueCell is DataGridViewCheckBoxCell)
                        {
                            property.SetValue(Preferences, (valueCell as DataGridViewCheckBoxCell).EditedFormattedValue);
                        }
                        else if (isString(property) && valueCell is DataGridViewTextBoxCell)
                        {
                            property.SetValue(Preferences, valueCell.EditedFormattedValue);
                        }
                        else if (isFloat(property) && valueCell is DataGridViewTextBoxCell)
                        {
                            try
                            {
                                var floatValue = float.Parse(valueCell.EditedFormattedValue as String, CultureInfo.InvariantCulture.NumberFormat);
                                property.SetValue(Preferences, floatValue);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }

            */
        }

        //Wraps/unwraps text
        private void wrapText(bool onOff)
        {
            // Loop through each row (TableLayoutPanel) in the FlowPanel
            foreach (Control control in _flowPanel.Controls)
            {
                if (control is TableLayoutPanel rowPanel)
                {
                    foreach (Control inner in rowPanel.Controls)
                    {
                        if (inner is Label label)
                        {
                            label.AutoSize = false;
                            label.MaximumSize = onOff ? new Size(rowPanel.Width - 10, 0) : Size.Empty;
                        }
                    }
                }
            }

            _flowPanel.PerformLayout();
            /*
            DescriptionColumn.DefaultCellStyle.WrapMode = (onOff) ? DataGridViewTriState.True : DataGridViewTriState.False;
            dataGridView.AutoResizeRows();
            */
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

        /// <summary>
        /// Adds a row for a boolean property
        /// </summary>
        /// <param name="obj">preferences object</param>
        /// <param name="property">the boolean property</param>
        /// <param name="attr">descriptor for the property</param>
        private void addCheckBoxRow(object obj, PropertyInfo property, BoolDescriptorAttribute attr)
        {
            var str = getDefaultValue(property.Name);

            var ii = dataGridView.Rows.Add(property.Name,
                                            attr.Description,
                                            property.GetValue(obj, null).ToString(),
                                            getDefaultValue(property.Name),
                                            "N/A");

            dataGridView[ValueColumn.Name, ii] = new DataGridViewCheckBoxCell
            {
                Value = property.GetValue(obj, null)
            };

            bool defaultValue = (String.Compare(str, "True", true) == 0);
            dataGridView[DefaultColumn.Name, ii] = new DataGridViewCheckBoxCell
            {
                Value = defaultValue
            };

            dataGridView.Rows[ii].Tag = property;
        }

        /// <summary>
        /// Adds a row for a boolean field
        /// </summary>
        /// <param name="obj">preferences object</param>
        /// <param name="fieldInfo">the boolean field</param>
        /// <param name="attr">descriptor for the field</param>
        private void addCheckBoxRow(object obj, FieldInfo fieldInfo, BoolDescriptorAttribute attr)
        {
            var ii = dataGridView.Rows.Add(fieldInfo.Name,
                                            attr.Description,
                                            fieldInfo.GetValue(obj).ToString(),
                                            getDefaultValue(fieldInfo.Name),
                                            "N/A");

            var checkBoxCell = new DataGridViewCheckBoxCell
            {
                Value = fieldInfo.GetValue(obj)
            };

            dataGridView[ValueColumn.Name, ii] = checkBoxCell;

            checkBoxCell = new DataGridViewCheckBoxCell { Value = false };

            dataGridView[DefaultColumn.Name, ii] = checkBoxCell;

            dataGridView.Rows[ii].Tag = fieldInfo;
        }

        /// <summary>
        /// Adds a row for a float field
        /// </summary>
        /// <param name="obj">preferences object</param>
        /// <param name="fieldInfo">the float field</param>
        /// <param name="attr">descriptor for the field</param>
        private void addFloatRow(object obj, FieldInfo fieldInfo, FloatDescriptorAttribute attr)
        {
            String range = attr.MinValue + " to " + attr.MaxValue;

            int rowNum = dataGridView.Rows.Add(fieldInfo.Name,
                                                attr.Description,
                                                fieldInfo.GetValue(obj).ToString(),
                                                getDefaultValue(fieldInfo.Name),
                                                range);

            dataGridView.Rows[rowNum].Tag = fieldInfo;
        }

        /// <summary>
        /// Adds a row for a float property
        /// </summary>
        /// <param name="obj">preferences object</param>
        /// <param name="property">the float property</param>
        /// <param name="attr">descriptor for the property</param>
        private void addFloatRow(object obj, PropertyInfo property, FloatDescriptorAttribute attr)
        {
            String range = attr.MinValue + " to " + attr.MaxValue;

            int rowNum = dataGridView.Rows.Add(property.Name,
                                            attr.Description,
                                            property.GetValue(obj, null).ToString(),
                                            getDefaultValue(property.Name),
                                            range);

            dataGridView.Rows[rowNum].Tag = property;
        }

        /// <summary>
        /// Adds a row for a integer field
        /// </summary>
        /// <param name="obj">preferences object</param>
        /// <param name="fieldInfo">the integer field</param>
        /// <param name="attr">descriptor for the field</param>
        private void addIntegerRow(object obj, FieldInfo fieldInfo, IntDescriptorAttribute attr)
        {
            String range = attr.MinValue + " to " + attr.MaxValue;

            int rowNum = dataGridView.Rows.Add(fieldInfo.Name,
                                                attr.Description,
                                                fieldInfo.GetValue(obj).ToString(),
                                                getDefaultValue(fieldInfo.Name),
                                                range);

            dataGridView.Rows[rowNum].Tag = fieldInfo;
        }

        /// <summary>
        /// Adds a row for a integer property
        /// </summary>
        /// <param name="obj">preferences object</param>
        /// <param name="property">the integer property</param>
        /// <param name="attr">descriptor for the property</param>
        private void addIntegerRow(object obj, PropertyInfo property, IntDescriptorAttribute attr)
        {
            String range = attr.MinValue + " to " + attr.MaxValue;

            int rowNum = dataGridView.Rows.Add(property.Name,
                                                attr.Description,
                                                property.GetValue(obj, null).ToString(),
                                                getDefaultValue(property.Name),
                                                range);

            dataGridView.Rows[rowNum].Tag = property;
        }

        /// <summary>
        /// Adds a row for a String field
        /// </summary>
        /// <param name="obj">preferences object</param>
        /// <param name="fieldInfo">the string field</param>
        /// <param name="attr">descriptor for the field</param>
        private void addStringRow(object obj, FieldInfo fieldInfo, StringDescriptorAttribute attr)
        {
            String range = "N/A";

            int rowNum = dataGridView.Rows.Add(fieldInfo.Name,
                                                attr.Description,
                                                fieldInfo.GetValue(obj).ToString(),
                                                getDefaultValue(fieldInfo.Name),
                                                range);

            dataGridView.Rows[rowNum].Tag = fieldInfo;
        }

        /// <summary>
        /// Adds a row for a string property
        /// </summary>
        /// <param name="obj">preferences object</param>
        /// <param name="property">the string property</param>
        /// <param name="attr">descriptor for the property</param>
        private void addStringRow(object obj, PropertyInfo property, StringDescriptorAttribute attr)
        {
            String range = "N/A";

            int rowNum = dataGridView.Rows.Add(property.Name,
                                                attr.Description,
                                                property.GetValue(obj, null).ToString(),
                                                getDefaultValue(property.Name),
                                                range);

            dataGridView.Rows[rowNum].Tag = property;
        }


        /// <summary>
        /// Returns the custom attribute for a boolean field
        /// </summary>
        /// <param name="field">the field</param>
        /// <returns>attribute, null if not found</returns>
        private BoolDescriptorAttribute getBoolAttribute(FieldInfo field)
        {
            var attributes = field.GetCustomAttributes(false);
            foreach (var attribute in attributes)
            {
                if (attribute.GetType() == typeof(BoolDescriptorAttribute))
                {
                    return (BoolDescriptorAttribute)attribute;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the custom attribute for a boolean property
        /// </summary>
        /// <param name="property">the property</param>
        /// <returns>attribute, null if not found</returns>
        private BoolDescriptorAttribute getBoolAttribute(PropertyInfo property)
        {
            var attributes = property.GetCustomAttributes(false);
            foreach (var attribute in attributes)
            {
                if (attribute.GetType() == typeof(BoolDescriptorAttribute))
                {
                    return (BoolDescriptorAttribute)attribute;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the custom attribute for a string field
        /// </summary>
        /// <param name="field">the field</param>
        /// <returns>attribute, null if not found</returns>
        private StringDescriptorAttribute getStringAttribute(FieldInfo field)
        {
            var attributes = field.GetCustomAttributes(false);
            foreach (var attribute in attributes)
            {
                if (attribute.GetType() == typeof(StringDescriptorAttribute))
                {
                    return (StringDescriptorAttribute)attribute;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the custom attribute for a String property
        /// </summary>
        /// <param name="property">the property</param>
        /// <returns>attribute, null if not found</returns>
        private StringDescriptorAttribute getStringAttribute(PropertyInfo property)
        {
            var attributes = property.GetCustomAttributes(false);

            foreach (var attribute in attributes)
            {
                if (attribute.GetType() == typeof(StringDescriptorAttribute))
                {
                    return (StringDescriptorAttribute)attribute;
                }
            }

            return null;
        }

        /// Returns true if the property is a float
        private bool isFloat(PropertyInfo property)
        {
            return property.PropertyType == typeof(float);
        }

        /// Returns true if the field is a float
        private bool isFloat(FieldInfo field)
        {
            return field.FieldType == typeof(float);
        }

        /// Returns true if the property is a integer
        private bool isInt(PropertyInfo property)
        {
            return property.PropertyType == typeof(int) ||
                    property.PropertyType == typeof(Int32);
        }

        /// Returns true if the field is an integer
        private bool isInt(FieldInfo field)
        {
            return field.FieldType == typeof(int) ||
                    field.FieldType == typeof(Int32);
        }

        /// Returns true if the field is a string
        private bool isString(FieldInfo field)
        {
            return field.FieldType == typeof(String) ||
                    field.FieldType == typeof(string);
        }

        /// Returns true if the property is a string
        private bool isString(PropertyInfo property)
        {
            return property.PropertyType == typeof(String) ||
                    property.PropertyType == typeof(string);
        }

        /// <summary>
        /// Returns true if the property is a bool
        /// </summary>
        /// <param name="property">property</param>
        /// <returns>true if it is</returns>
        private bool isBool(PropertyInfo property)
        {
            return property.PropertyType == typeof(Boolean) ||
                    property.PropertyType == typeof(bool);
        }

        /// Returns true if the field is a bool
        private bool isBool(FieldInfo field)
        {
            return field.FieldType == typeof(Boolean) ||
                    field.FieldType == typeof(bool);
        }


        #endregion

    }
}