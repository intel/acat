////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// PreferencesEditForm2.cs
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

using ACAT.Lib.Core.PanelManagement;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace ACAT.Lib.Core.PreferencesManagement
{
    /// <summary>
    /// A generic preferences editor for a class that
    /// has fields and properties which are intergers,
    /// strings, bool or floats. Picks those fields and
    /// properties which are qualified by custom attributes
    /// (BoolDescritpor, IntDescriptor etc). Displays the
    /// settings as a gridview. Does validation of data
    /// to make sure it is within range etc.
    /// </summary>
    public partial class PreferencesEditForm2 : Form
    {
        /// <summary>
        /// Default values for the preferences
        /// </summary>
        public IPreferences DefaultPreferences;

        /// <summary>
        /// The preferences object
        /// </summary>
        public IPreferences Preferences;

        /// <summary>
        /// Did the user change anything in the form
        /// </summary>
        private bool _isDirty = false;

        /// <summary>
        /// Aspect ratio of form at design time
        /// </summary>
        private float _designTimeAspectRatio = 0.0f;

        /// <summary>
        /// Has first call to OnClientSizeChanged been made?
        /// </summary>
        private bool _firstClientChangedCall = true;

        /// <summary>
        /// Whether the text should be wrapped or not
        /// </summary>
        private bool _wrapText = true;

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public PreferencesEditForm2()
        {
            InitializeComponent();
            Text = "Settings";
            Load += PreferencesEditForm_Load;
        }

        /// <summary>
        /// Gets or sets the preferences object
        /// </summary>
        public ISupportsPreferences SupportsPreferencesObj { get; set; }

        /// <summary>
        /// Gets or sets the title of the form
        /// </summary>
        public String Title { get; set; }

        /// <summary>
        /// Client size changed
        /// </summary>
        /// <param name="e">event args</param>
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
        /// User canceled. Confirm and close
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (!_isDirty || confirm("Changes not saved. Quit anyway?"))
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        /// <summary>
        /// Restore default values for all the settings
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonDefaults_Click(object sender, EventArgs e)
        {
            if (confirm("Restore default settings ?"))
            {
                _isDirty = true;
                refreshGridView(DefaultPreferences);
            }
        }

        /// <summary>
        /// User clicked OK. Confirm, save preferences and close
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (_isDirty && confirm("Save changes?"))
            {
                updatePreferences();

                Preferences.Save();
            }

            DialogResult = DialogResult.OK;

            Close();
        }

        /// <summary>
        /// Wrap / unwrap text
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void checkBoxWrapText_CheckedChanged(object sender, EventArgs e)
        {
            _wrapText = checkBoxWrapText.Checked;
            wrapText(_wrapText);
        }


        /// <summary>
        /// Gets a yes/no response
        /// </summary>
        /// <param name="prompt">prompt to display</param>
        /// <returns>Yes or no</returns>
        private bool confirm(String prompt)
        {
            // return ConfirmBox.ShowDialog(prompt.ToString(), null, false);
            return ConfirmBox.ShowDialog(prompt.ToString(), this, true);
        }

        /// <summary>
        /// clear status
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // clearStatus();
        }


        /// <summary>
        /// Displays a error status mesage
        /// </summary>
        /// <param name="status">text of the status</param>
        private void showErrorStatus(String settingName, String status)
        {
            // ConfirmBoxSingleOption confirmBoxSingleOption = new ConfirmBoxSingleOption();
            // confirmBoxSingleOption.Prompt = "Error\n" + settingName + "\n" + status;
            // confirmBoxSingleOption.DecisionPrompt = "OK";
            // confirmBoxSingleOption.ShowDialog(this);

            ConfirmBoxSingleOption.ShowDialog("Error\n" + settingName + "\n" + status, "OK", this);

        }

        /// <summary>
        /// Here's where checking is done on the validity of the data
        /// If it is an integer for eg, make sure that all the text
        /// in the cell are digits and that the integer is within range.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void dataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            var senderGrid = (DataGridView)sender;
            e.Cancel = false;
            String name = null;
            String newVal = null;
            String prevVal = null;
            String defaultVal = null;

            var cell = senderGrid[e.ColumnIndex, e.RowIndex];
            if (e.ColumnIndex == 2 && e.RowIndex >= 0 && cell is DataGridViewTextBoxCell)
            {
                var textBox = cell as DataGridViewTextBoxCell;
                String value = textBox.EditedFormattedValue as string;

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
                        int intValue;
                        if (Int32.TryParse(value, out intValue))
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
                            int intValue2;
                            if (Int32.TryParse(newVal, out intValue2))
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
                        int intValue;
                        if (Int32.TryParse(value, out intValue))
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
                            int intValue2;
                            if (Int32.TryParse(newVal, out intValue2))
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


            if (!String.IsNullOrEmpty(name))
            {
                // Log.Debug("\ndataGridView_CellValidating | Cell Property / Field Name: " + name + " | e.Cancel: " + e.Cancel.ToString() + " | prevVal: " + prevVal + " | defaultVal: " + defaultVal+ " | newVal: "+ newVal);

                // Valid new value set for cell - send event notifying that cell change has occurred
                if (!e.Cancel)
                {
                    _isDirty = true;
                }

                // Invalid new value set for cell - automatically setting cell to previous or default value
                else
                {
                    if (!String.IsNullOrEmpty(newVal))
                    {
                        ((DataGridViewTextBoxCell)cell).Value = newVal;
                        ((DataGridView)sender).RefreshEdit();
                    }

                }
            }

        }


        /// <summary>
        /// Something changed. Set dirty flag
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void DataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // _isDirty = true;
        }

        /// <summary>
        /// Something changed. Set dirty flag
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void DataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            // _isDirty = true;
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
        /// Returns the default value for the specified field
        /// </summary>
        /// <param name="fieldName">name of the field</param>
        /// <returns>the default value</returns>
        private String getDefaultValue(String fieldName)
        {
            if (DefaultPreferences == null)
            {
                return String.Empty;
            }

            var members = DefaultPreferences.GetType().GetMembers();
            foreach (var memberInfo in members)
            {
                if (DefaultPreferences == null)
                {
                    continue;
                }

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

        /// <summary>
        /// Returns the field info for the specified field name
        /// </summary>
        /// <param name="obj">The object</param>
        /// <param name="name">anme of hte field</param>
        /// <returns>FieldInfo</returns>
        private FieldInfo getField(object obj, String name)
        {
            return obj.GetType().GetFields().FirstOrDefault(field => String.Compare(field.Name, name) == 0);
        }

        /// <summary>
        /// Returns the custom attribute for a float field
        /// </summary>
        /// <param name="field">the field</param>
        /// <returns>attribute, null if not found</returns>
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

        /// <summary>
        /// Returns the custom attribute for a float property
        /// </summary>
        /// <param name="property">the property</param>
        /// <returns>attribute, null if not found</returns>
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

        /// <summary>
        /// Returns the custom attribute for an integer field
        /// </summary>
        /// <param name="field">the field</param>
        /// <returns>attribute, null if not found</returns>
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

        /// <summary>
        /// Returns the custom attribute for a integer property
        /// </summary>
        /// <param name="property">the property</param>
        /// <returns>attribute, null if not found</returns>
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

        /// <summary>
        /// Returns the property info for the specified property
        /// </summary>
        /// <param name="obj"the object></param>
        /// <param name="name">name of the property</param>
        /// <returns>Property info</returns>
        private PropertyInfo getProperty(object obj, String name)
        {
            return obj.GetType().GetProperties().FirstOrDefault(property => String.Compare(property.Name, name) == 0);
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

        /// <summary>
        /// Formats the datagridview
        /// </summary>
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
            dataGridView.CellEndEdit += dataGridView_CellEndEdit;
            dataGridView.CellValueChanged += DataGridView_CellValueChanged;
            dataGridView.CurrentCellDirtyStateChanged += DataGridView_CurrentCellDirtyStateChanged;
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

        /// <summary>
        /// Returns true if the field is a bool
        /// </summary>
        /// <param name="field">field</param>
        /// <returns>true if it is</returns>
        private bool isBool(FieldInfo field)
        {
            return field.FieldType == typeof(Boolean) ||
                    field.FieldType == typeof(bool);
        }

        /// <summary>
        /// Returns true if the property is a float
        /// </summary>
        /// <param name="property">property</param>
        /// <returns>true if it is</returns>
        private bool isFloat(PropertyInfo property)
        {
            return property.PropertyType == typeof(float);
        }

        /// <summary>
        /// Returns true if the field is a float
        /// </summary>
        /// <param name="field">field</param>
        /// <returns>true if it is</returns>
        private bool isFloat(FieldInfo field)
        {
            return field.FieldType == typeof(float);
        }

        /// <summary>
        /// Returns true if the property is a integer
        /// </summary>
        /// <param name="property">property</param>
        /// <returns>true if it is</returns>
        private bool isInt(PropertyInfo property)
        {
            return property.PropertyType == typeof(int) ||
                    property.PropertyType == typeof(Int32);
        }

        /// <summary>
        /// Returns true if the field is an integer
        /// </summary>
        /// <param name="field">field</param>
        /// <returns>true if it is</returns>
        private bool isInt(FieldInfo field)
        {
            return field.FieldType == typeof(int) ||
                    field.FieldType == typeof(Int32);
        }

        /// <summary>
        /// Returns true if the field is a string
        /// </summary>
        /// <param name="field">field</param>
        /// <returns>true if it is</returns>
        private bool isString(FieldInfo field)
        {
            return field.FieldType == typeof(String) ||
                    field.FieldType == typeof(string);
        }

        /// <summary>
        /// Returns true if the property is a string
        /// </summary>
        /// <param name="property">property</param>
        /// <returns>true if it is</returns>
        private bool isString(PropertyInfo property)
        {
            return property.PropertyType == typeof(String) ||
                    property.PropertyType == typeof(string);
        }

        /// <summary>
        /// Form loader.  Initialize the grid and populate it
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">eent args</param>
        private void PreferencesEditForm_Load(object sender, EventArgs e)
        {
            float currentAspectRatio = (float)ClientSize.Height / ClientSize.Width;

            if (_designTimeAspectRatio != 0.0f && currentAspectRatio != _designTimeAspectRatio)
            {
                ClientSize = new System.Drawing.Size(ClientSize.Width, (int)(_designTimeAspectRatio * ClientSize.Width));
            }

            TopMost = false;
            TopMost = true;

            Activate();

            CenterToScreen();

            initializeGridView();

            if (!String.IsNullOrEmpty(Title))
            {
                Text = Title;
            }

            checkBoxWrapText.Checked = _wrapText;

            Preferences = SupportsPreferencesObj.GetPreferences();
            DefaultPreferences = SupportsPreferencesObj.GetDefaultPreferences();
            if (DefaultPreferences == null)
            {
                buttonDefaults.Enabled = false;
            }

            refreshGridView(Preferences);

            _isDirty = false;

            dataGridView.Sort(SettingColumn, ListSortDirection.Ascending);
            SettingColumn.HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Ascending;
        }

        /// <summary>
        /// Populates the grid view with preferences data
        /// </summary>
        /// <param name="prefs">preferences</param>
        private void refreshGridView(IPreferences prefs)
        {
            if (prefs == null)
            {
                return;
            }

            dataGridView.Rows.Clear();

            wrapText(_wrapText);

            var members = prefs.GetType().GetMembers();
            foreach (var memberInfo in members)
            {
                var name = memberInfo.Name;

                MemberInfo[] member = prefs.GetType().GetMember(name);
                if (member.Length == 0)
                {
                    continue;
                }

                switch (member[0].MemberType)
                {
                    case MemberTypes.Field:
                        FieldInfo fieldInfo = prefs.GetType().GetField(name);
                        if (isInt(fieldInfo))
                        {
                            var intDescriptor = getIntAttribute(fieldInfo);
                            if (intDescriptor != null)
                            {
                                addIntegerRow(prefs, fieldInfo, intDescriptor);
                            }
                        }
                        else if (isBool(fieldInfo))
                        {
                            var boolDescriptor = getBoolAttribute(fieldInfo);
                            if (boolDescriptor != null)
                            {
                                addCheckBoxRow(prefs, fieldInfo, boolDescriptor);
                            }
                        }
                        else if (isString(fieldInfo))
                        {
                            var stringDescriptor = getStringAttribute(fieldInfo);
                            if (stringDescriptor != null)
                            {
                                addStringRow(prefs, fieldInfo, stringDescriptor);
                            }
                        }
                        else if (isFloat(fieldInfo))
                        {
                            var floatDescriptor = getFloatAttribute(fieldInfo);
                            if (floatDescriptor != null)
                            {
                                addFloatRow(prefs, fieldInfo, floatDescriptor);
                            }
                        }

                        break;

                    case MemberTypes.Property:
                        var property = prefs.GetType().GetProperty(name);
                        if (isInt(property))
                        {
                            var intDescriptor = getIntAttribute(property);
                            if (intDescriptor != null)
                            {
                                addIntegerRow(prefs, property, intDescriptor);
                            }
                        }
                        else if (isBool(property))
                        {
                            var boolDescriptor = getBoolAttribute(property);
                            if (boolDescriptor != null)
                            {
                                addCheckBoxRow(prefs, property, boolDescriptor);
                            }
                        }
                        else if (isString(property))
                        {
                            var stringDescriptor = getStringAttribute(property);
                            if (stringDescriptor != null)
                            {
                                addStringRow(prefs, property, stringDescriptor);
                            }
                        }
                        else if (isFloat(property))
                        {
                            var floatDescriptor = getFloatAttribute(property);
                            if (floatDescriptor != null)
                            {
                                addFloatRow(prefs, property, floatDescriptor);
                            }
                        }

                        break;
                }
            }

            if (dataGridView.Rows.Count == 0)
            {
                MessageBox.Show("No configurable settings found", Text);
                Close();
            }

            dataGridView.AutoResizeRows();

            if (dataGridView.Rows.Count > 0)
            {
                dataGridView.CurrentCell = dataGridView.Rows[0].Cells[0];
                dataGridView.Rows[0].Selected = true;
            }
        }

        /// <summary>
        /// Displays a status mesage
        /// </summary>
        /// <param name="status">text of the status</param>
        private void showStatus(String status)
        {
            labelStatus.Text = status;
        }

        /// <summary>
        /// Updates preferneces using the data in the grid view
        /// </summary>
        private void updatePreferences()
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                String name = row.Cells[SettingColumn.Name].Value as String;
                var valueCell = row.Cells[ValueColumn.Name];

                FieldInfo field = getField(Preferences, name);
                if (field != null)
                {
                    if (isInt(field) && valueCell is DataGridViewTextBoxCell)
                    {
                        int intValue;
                        if (Int32.TryParse(valueCell.EditedFormattedValue as String, out intValue))
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
                            int intValue;
                            if (Int32.TryParse(valueCell.EditedFormattedValue as String, out intValue))
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
        }

        /// <summary>
        /// Wraps/unwraps text
        /// </summary>
        /// <param name="onOff">to do or not to do</param>
        private void wrapText(bool onOff)
        {
            DescriptionColumn.DefaultCellStyle.WrapMode = (onOff) ? DataGridViewTriState.True : DataGridViewTriState.False;
            dataGridView.AutoResizeRows();
        }
    }
}