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
using ACATResources;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace ACAT.Lib.Core.PreferencesManagement
{
    public partial class PreferencesEditForm2 : Form
    {
        #region Properties
        public IPreferences DefaultPreferences;             //Default values for the preferences
                                                            //Gets or sets the preferences object
        public ISupportsPreferences SupportsPreferencesObj { get; set; }

        public IPreferences Preferences;                    // The preferences object

        private bool _isDirty = false;                      //Did the user change anything in the form

        private float _designTimeAspectRatio = 0.0f;        //Aspect ratio of form at design time

        private bool _firstClientChangedCall = true;        //Has first call to OnClientSizeChanged been made?

        private bool _wrapText = true;                     //Whether the text should be wrapped or not
        public String Title { get; set; }                   //Gets or sets the title of the form

        #endregion

        //Initializes an instance of the class
        public PreferencesEditForm2()
        {
            InitializeComponent();
            Text = "Settings";
            Load += PreferencesEditForm_Load;
        }

        //Form loader.  Initialize the grid and populate it
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

        //Populates the grid view with preferences data
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

        //Updates preferneces using the data in the grid view
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
                    PropertyInfo property = Preferences.GetType().GetProperties().FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.Ordinal));
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

        //Wraps/unwraps text
        private void wrapText(bool onOff)
        {
            DescriptionColumn.DefaultCellStyle.WrapMode = (onOff) ? DataGridViewTriState.True : DataGridViewTriState.False;
            dataGridView.AutoResizeRows();
        }

        //User canceled. Confirm and close
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            // Ask for confirmation only if there are unsaved changes
            if (!_isDirty ||
                ConfirmBoxTwoOption.ShowDialog(
                    "Changes not saved. Quit anyway?", 
                    string.Empty,                     
                    StringResources.Yes,               
                    StringResources.No,               
                    this,                            
                    true))                             
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        //Restore default values for all the settings
        private void buttonDefaults_Click(object sender, EventArgs e)
        {
            if (ConfirmBoxTwoOption.ShowDialog(
              "Restore default settings?",
              string.Empty,
              StringResources.Yes,
              StringResources.No,
              this,
              true))
            {
                _isDirty = true;
                refreshGridView(DefaultPreferences);
            }
        }

        //User clicked OK. Confirm, save preferences and close
        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (!_isDirty || ConfirmBoxTwoOption.ShowDialog(
             "Save changes?",
             string.Empty,
             StringResources.Yes,
             StringResources.No,
             this,
             true))
            {
                if (_isDirty)
                {
                    updatePreferences();
                    Preferences.Save();
                }

                DialogResult = DialogResult.OK;
                Close();
            }
        }

        //Wrap / unwrap text
        private void checkBoxWrapText_CheckedChanged(object sender, EventArgs e)
        {
            _wrapText = checkBoxWrapText.Checked;
            wrapText(_wrapText);
        }

        #region Datagrid

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
            dataGridView.CellValueChanged += (s, e) => { if (!_isDirty) _isDirty = true; };
            dataGridView.CurrentCellDirtyStateChanged += (s, e) => { if (!_isDirty) _isDirty = true; };
        }

        // Here's where checking is done on the validity of the data If it is an integer for eg, 
        //make sure that all the text in the cell are digits and that the integer is within range.
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
                                    ConfirmBoxOneOption.ShowDialog($"Error\n{name}\nOut of range", "", StringResources.OK, this);
                                    newVal = prevVal;
                                }
                            }
                        }
                        else
                        {
                            e.Cancel = true;
                            ConfirmBoxOneOption.ShowDialog($"Error\n{name}\nMust be numeric", "", StringResources.OK, this);
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
                                ConfirmBoxOneOption.ShowDialog($"Error\n{name}\nOut of range", "", StringResources.OK, this);
                                newVal = prevVal;
                            }
                        }
                        catch
                        {
                            e.Cancel = true;
                            ConfirmBoxOneOption.ShowDialog($"Error\n{name}\nMust be numeric", "", StringResources.OK, this);
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
                                    ConfirmBoxOneOption.ShowDialog($"Error\n{name}\nOut of range", "", StringResources.OK, this);
                                    newVal = prevVal;
                                }
                            }
                        }
                        else
                        {
                            e.Cancel = true;
                            ConfirmBoxOneOption.ShowDialog($"Error\n{name}\nMust be numeric", "", StringResources.OK, this);
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
                                ConfirmBoxOneOption.ShowDialog($"Error\n{name}\nOut of range", "", StringResources.OK, this);

                                newVal = prevVal;
                            }
                        }
                        catch
                        {
                            e.Cancel = true;
                            ConfirmBoxOneOption.ShowDialog($"Error\n{name}\nMust be numeric", "", StringResources.OK, this);
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

        //Adds a row for a boolean property
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

        //Adds a row for a boolean field
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

        //Adds a row for a float field
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

        //Adds a row for a float property
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

        //Adds a row for a integer field
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

        //Adds a row for a integer property
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

        //Adds a row for a String field
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

        //Adds a row for a string property
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

        #endregion

        #region Check Attributes

        //Returns the custom attribute for a boolean field
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

        //Returns the custom attribute for a boolean property
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

        //Returns the default value for the specified field
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

        //Returns the custom attribute for an integer field
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

        //Returns the custom attribute for a string field
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

        //Returns the custom attribute for a String property
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

        #endregion

        #region CheckProperties

        //Returns true if the property is a bool
        private bool isBool(PropertyInfo property)
        {
            return property.PropertyType == typeof(Boolean) ||
                    property.PropertyType == typeof(bool);
        }

        //Returns true if the field is a bool
        private bool isBool(FieldInfo field)
        {
            return field.FieldType == typeof(Boolean) ||
                    field.FieldType == typeof(bool);
        }

        //Returns true if the property is a float
        private bool isFloat(PropertyInfo property)
        {
            return property.PropertyType == typeof(float);
        }

        //Returns true if the field is a float
        private bool isFloat(FieldInfo field)
        {
            return field.FieldType == typeof(float);
        }

        //Returns true if the property is a integer
        private bool isInt(PropertyInfo property)
        {
            return property.PropertyType == typeof(int) ||
                    property.PropertyType == typeof(Int32);
        }

        //Returns true if the field is an integer
        private bool isInt(FieldInfo field)
        {
            return field.FieldType == typeof(int) ||
                    field.FieldType == typeof(Int32);
        }

        //Returns true if the field is a string
        private bool isString(FieldInfo field)
        {
            return field.FieldType == typeof(String) ||
                    field.FieldType == typeof(string);
        }

        //Returns true if the property is a string
        private bool isString(PropertyInfo property)
        {
            return property.PropertyType == typeof(String) ||
                    property.PropertyType == typeof(string);
        }
        
        #endregion

    }
}