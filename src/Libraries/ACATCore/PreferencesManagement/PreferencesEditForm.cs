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

using ACAT.Core.PanelManagement;
using ACAT.Core.Utility;
using ACATResources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Threading;
using ACAT.Lib.Core.PreferencesManagement.UI;

namespace ACAT.Core.PreferencesManagement
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
    public partial class PreferencesEditForm : Form
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
        public bool _isDirty = false;

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
        public bool _wrapText = true;

        /// <summary>
        /// Delegate for the event triggered when the user makes a change to a preference setting
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="arg">event args</param>
        public delegate void NotifyPreferencesChangeMade();

        /// <summary>
        /// Event raised when the user makes a change to a preference setting
        /// </summary>
        public event NotifyPreferencesChangeMade EvtPreferencesChangeMade;

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public PreferencesEditForm()
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
        /// Gets or sets the title / text for header of settings column of the form
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
        /// Check if form filled correctly, if not, return false
        /// If validated, check if changes have been made to form and if so prompt user asking if they want to save
        /// </summary>
        /// <returns></returns>
        public bool validateAndSave()
        {
            // Update preferences based on latest values then save
            updatePreferences();

            // Save preferences
            Preferences.Save();

            return true;
        }

        /// <summary>
        /// User clicked wrap text checkbox
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        public void checkBoxWrapText_CheckedChanged(object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(CheckBox))
            {
                _wrapText = ((CheckBox)sender).Checked;
                wrapText(_wrapText);
            }
        }

        /// <summary>
        /// User clicked Defaults button
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        public void buttonDefaults_Click(object sender, EventArgs e)
        {
            if (ConfirmBoxTwoOption.ShowDialog("Restore default settings?",
                "This cannot be undone.", StringResources.Yes, StringResources.No, this, true))
            {
                _isDirty = true;
                refreshGridView(DefaultPreferences);
                EvtPreferencesChangeMade();
            }
        }

        /// <summary>
        /// Gets a yes/no response
        /// </summary>
        /// <param name="prompt">prompt to display</param>
        /// <returns>Yes or no</returns>
        private bool confirm(String prompt)
        {
            return ConfirmBoxTwoOption.ShowDialog(prompt.ToString(), "",
                StringResources.Yes, StringResources.No, this, true);
        }

        /// <summary>
        /// Occurs when edit mode stopped for the current selected cell
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
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

        /// <summary>
        /// Something changed. Set dirty flag
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void DataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            _isDirty = true;
            EvtPreferencesChangeMade();
        }

        /// <summary>
        /// Something changed. Set dirty flag
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void DataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            _isDirty = true;
            EvtPreferencesChangeMade();
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
            // For WPF Controls
            if (System.Windows.Application.Current == null)
            {
         


                // Initialize WPF Dispatcher safely
                Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

                // Schedule optional startup logic on the WPF Dispatcher
                dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
                {
                    Console.WriteLine("WPF Dispatcher is ready");
                }));

                new System.Windows.Application();
            }

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
                refreshGridView(Preferences);

                if (dataGridView != null)
                {
                    dataGridView.CellValueChanged += DataGridView_CellValueChanged;
                    dataGridView.CurrentCellDirtyStateChanged += DataGridView_CurrentCellDirtyStateChanged;
                }
            };
        }

        Control CreatedLabeledPanel(PropertyInfo prop)
        {
            Control control;
            Font font = new Font("Montserrat", 18);

            var controlFactory = new Dictionary<Type, Func<PropertyInfo, Control>>
                {
                    { typeof(bool), member => new CheckBox() },
                    { typeof(int), member => new TrackBar {Minimum = 0, Maximum = 100 } },
                    { typeof(float), member => new TrackBar {Minimum = 0, Maximum=100 } },
                    { typeof(string), member => new TextBox() }
                };

            var type = prop.PropertyType;
            if (controlFactory.TryGetValue(type, out var controlBuilder))
            {
                control = controlBuilder(prop);
                control.Tag = prop;
                control.ForeColor = Color.White;                
            }
            else
            {
                control = new Label { Text = $"Unsupported Type: {prop.Name} ({prop.PropertyType.Name})" };
            }

            var panel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Fill,
                WrapContents = true,
                BackColor = Color.Transparent,
                Margin = new Padding(15)
            };

            var descriptionAttr = prop.GetCustomAttribute<DescriptorAttribute>();
            var labelText = descriptionAttr?.Description ?? "MISSING DESCRIPTION";

            var label = new Label
            {
                Text = labelText,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 6, 5, 0),
                ForeColor = Color.White,
                Font = font
            };

            control.Margin = new Padding(0, 3, 0, 0);

            panel.Controls.Add(label);
            panel.Controls.Add(control);

            return panel;

        }

        /// <summary>
        /// Populates the grid view with preferences data
        /// </summary>
        /// <param name="prefs">preferences</param>
        private void refreshGridView(IPreferences prefs)
        {
            //// Do Clear of datagrid rows in try/catch block - sometimes throws exception
            //bool clearSuccessful = true;
            //try
            //{
            //    dataGridView.Rows.Clear();
            //}
            //catch
            //{
            //    Log.Debug("PreferencesEditForm | refreshGridView | clearSuccessful == false");
            //    clearSuccessful = false;
            //}
            //if (!clearSuccessful)
            //    return;

            // HACK until we fix the Form...
            var flowPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                BackColor = Color.Transparent,
                AutoSize = false,
                AutoScroll = false,
                Dock = DockStyle.Fill,

            };
            flowPanel.HorizontalScroll.Visible = false;
            flowPanel.HorizontalScroll.Maximum = 0;

            var parent = dataGridView.Parent;                
            parent.Controls.Remove(dataGridView);
            parent.Controls.Add(flowPanel);

            wrapText(_wrapText);

            var descriptor = prefs.GetType().GetCustomAttribute<DescriptorAttribute>();
            Label category = new Label
            {
                AutoSize = true,
                Dock = DockStyle.Fill,
                Text = descriptor?.Category ?? "UNKNOWN CATEGORY",
                Font = new Font("Montserrat", 24, FontStyle.Bold),
                ForeColor = Color.White
            };
            flowPanel.Controls.Add(category);


            Label description = new Label {
                AutoSize = true,
                Dock = DockStyle.Fill,
                Text = descriptor?.Description ?? "UNKNOWN DESCRIPTION",
                Font = new Font("Montserrat", 20, FontStyle.Regular),
                ForeColor = Color.White
            };
            flowPanel.Controls.Add(description);

            var props = prefs.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var panelBuilder = new SettingsPanelBuilder();
            foreach (var prop in props)
            {
                //var proppanel = CreatedLabeledPanel(prop);
                //flowPanel.Controls.Add(proppanel);
                var panel = panelBuilder.CreateLabeledPanel(prop, prefs);

                var host = new ElementHost
                {
                    Child = panel,
                    AutoSize = true,
                    Margin = new Padding(10),
                    Dock = DockStyle.Top
                };   
                flowPanel.Controls.Add(host);
            }
        }

        /// <summary>
        /// Displays a error status mesage
        /// </summary>
        /// <param name="status">text of the status</param>
        private void showErrorStatus(String settingName, String status)
        {
            ConfirmBoxOneOption ConfirmBoxOneOption = new ConfirmBoxOneOption
            {
                Prompt = "Error\n" + settingName + "\n" + status,
                DecisionPrompt = "OK"
            };
            ConfirmBoxOneOption.ShowDialog(this);
            ConfirmBoxOneOption.Dispose();
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