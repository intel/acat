////////////////////////////////////////////////////////////////////////////
// <copyright file="ConfigureKeyboardActuatorForm.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2017 Intel Corporation 
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.ActuatorManagement.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ACAT.Lib.Core.ActuatorManagement
{
    /// <summary>
    /// Displays a list of switches for an acutator to enable configure
    /// settings for each of the switches.  User can enable/disable
    /// switches, turn on/off whether the switch should act as a trigger.
    /// </summary>
    public partial class ConfigureKeyboardActuatorForm : Form
    {
        private const String _unmappedValue = "--";

        private ActuatorConfig _actuatorConfig;

        private ActuatorSetting _actuatorSetting;

        /// <summary>
        /// Aspect ratio of form at design time
        /// </summary>
        private float _designTimeAspectRatio = 0.0f;

        /// <summary>
        /// Has first call to OnClientSizeChanged been made?
        /// </summary>
        private bool _firstClientChangedCall = true;

        /// <summary>
        /// Has anything changed?
        /// </summary>
        private bool _isDirty = false;

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public ConfigureKeyboardActuatorForm()
        {
            InitializeComponent();

            CenterToScreen();
            AllowMultiEnable = true;

            Load += OnLoad;
        }

        /// <summary>
        /// Gets or sets the acutator for which the switches
        /// need to be configured
        /// </summary>
        public IActuator Actuator { get; set; }

        /// <summary>
        /// Gets or sets the property on whether to allow enabling
        /// or disabling multiple categories or just one at a time
        /// (like a radio button)
        /// </summary>
        public bool AllowMultiEnable { get; set; }

        /// <summary>
        /// Gets or sets the column header text of the description column
        /// </summary>
        public String DescriptionColumnHeaderText { get; set; }

        /// <summary>
        /// Gets or sets the column header text for enabling/disabling a switch
        /// </summary>
        public String EnableColumnHeaderText { get; set; }

        /// <summary>
        /// Gets or sets the column header text for the switch name column
        /// </summary>
        public String SwitchNameColumnHeaderText { get; set; }

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
        /// Gets or sets the title of the form
        /// </summary>
        public String Title { get; set; }

        /// <summary>
        /// Gets or sets the column header for the trigger column
        /// </summary>
        public String TriggerColumnHeaderText { get; set; }

        /// <summary>
        /// Adds a new row to the datagrid representing a new
        /// switch
        /// </summary>
        /// <param name="switchSetting">parameters for the new switch</param>
        private void addDataGridRow(SwitchSetting switchSetting)
        {
            int rowNum = dataGridView2.Rows.Add(switchSetting.Source, switchSetting.Command);
            dataGridView2.Rows[rowNum].Tag = switchSetting;

            (dataGridView2[EnableColumn.Name, rowNum] as DataGridViewCheckBoxCell).Value = switchSetting.Enabled;

            (dataGridView2[TriggerColumn.Name, rowNum] as DataGridViewCheckBoxCell).Value =
                switchSetting.IsTriggerSwitch();

            dataGridView2[CommandColumn.Name, rowNum].Value = (switchSetting.IsTriggerSwitch() ||
                                                               String.IsNullOrEmpty(switchSetting.Command))
                ? _unmappedValue
                : formatCommandForDisplay(switchSetting.Command);

            dataGridView2[CommandColumn.Name, rowNum].Tag = dataGridView2[CommandColumn.Name, rowNum].Value;

            (dataGridView2[EditSwitchColumn.Name, rowNum] as DataGridViewButtonCell).Value = "Change";
            (dataGridView2[DeleteSwitchColumn.Name, rowNum] as DataGridViewButtonCell).Value = "Remove";
        }

        private void buttonAddNew_Click(object sender, EventArgs e)
        {
            Hide();

            var editKeyboardActuatorForm = new EditKeyboardActuatorSwitchForm();

            var dialogResult = editKeyboardActuatorForm.ShowDialog();

            Show();

            if (dialogResult == DialogResult.OK)
            {
                var source = editKeyboardActuatorForm.ShortcutChanged ? editKeyboardActuatorForm.Shortcut : String.Empty;

                if (String.IsNullOrEmpty(source))
                {
                    return;
                }

                var switchName = generateSwitchName();

                var command = editKeyboardActuatorForm.MappedCommand;

                var switchSetting = new SwitchSetting(switchName, "Keyboard shortcut", command) {Source = source};

                if (editKeyboardActuatorForm.IsTriggerSelect)
                {
                    switchSetting.ConfigureAsTriggerSwitch(true);
                    switchSetting.BeepFile = "beep.wav";
                }

                _actuatorSetting.SwitchSettings.Add(switchSetting);

                _isDirty = true;

                refreshDataGridView();
            }
        }

        /// <summary>
        /// Cancels out of the dialog
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (!_isDirty || MessageBox.Show("Changes not saved. Quit anyway?",
                Text, MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        /// <summary>
        /// User clicked OK.  Get data from the UI, save changes and quit
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (validate())
            {
                if (_isDirty &&
                    MessageBox.Show("Save changes?",
                        Text, MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    updateDataFromUIAndSave();
                    DialogResult = DialogResult.OK;
                }

                Close();
            }
        }

        /// <summary>
        /// Generate a switch name
        /// </summary>
        /// <returns>generated name</returns>
        String generateSwitchName()
        {
            var switchBaseName = "Shortcut";

            if (Actuator.Switches.Count == 0)
            {
                return switchBaseName + "1";
            }

            int index = 1;

            while (true)
            {
                var name = switchBaseName + index;
                if (!switchNameExists(name))
                {
                    return name;
                }

                index++;
            }
        }

        /// <summary>
        /// User clicked in a cell in the datagrid view. Handle it 
        /// depending on which cell the click was detected
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView) sender;

            if (e.RowIndex < 0)
            {
                return;
            }

            if (senderGrid.Columns[e.ColumnIndex] == TriggerColumn)
            {
                handleTriggerSelect(e);
            }
            else if (senderGrid.Columns[e.ColumnIndex] == EditSwitchColumn)
            {
                handleEditSwitch(e);
            }
            else if (senderGrid.Columns[e.ColumnIndex] == DeleteSwitchColumn)
            {
                handleRemoveSwitch(e);
            }
        }

        /// <summary>
        /// If the user clicked on the Enable column, and if AllowMultiEnable
        /// is false, then make sure only one cell is checked in the column
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
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

                var checkCell = (DataGridViewCheckBoxCell) row.Cells[e.ColumnIndex];

                bool isChecked = (Boolean) checkCell.Value;
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

        /// <summary>
        /// Dirty state changed
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void dataGridView2_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView2.IsCurrentCellDirty)
            {
                dataGridView2.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }

            _isDirty = true;
        }

        /// <summary>
        /// Formats the command field for display in the datagridview
        /// </summary>
        /// <param name="command">command</param>
        /// <returns>formatted command</returns>
        private String formatCommandForDisplay(String command)
        {
            if (command.StartsWith("@") && command.Length > 1)
            {
                command = command.Substring(1);
            }

            return command;
        }

        /// <summary>
        /// User wants to edit a switch. Display the switch edit form 
        /// and update the datagridview with the edited data
        /// </summary>
        /// <param name="e">event args</param>
        private void handleEditSwitch(DataGridViewCellEventArgs e)
        {
            var row = dataGridView2.Rows[e.RowIndex];

            Hide();

            var editKeyboardActuatorForm = new EditKeyboardActuatorSwitchForm();

            var checkBoxCell = (dataGridView2.Rows[e.RowIndex].Cells[TriggerColumn.Name]) as DataGridViewCheckBoxCell;
            if (checkBoxCell.Value != null)
            {
                editKeyboardActuatorForm.IsTriggerSelect = (bool) checkBoxCell.Value;
            }

            if (!editKeyboardActuatorForm.IsTriggerSelect)
            {
                editKeyboardActuatorForm.MappedCommand =
                    dataGridView2.Rows[e.RowIndex].Cells[CommandColumn.Name].Value as String;
            }

            editKeyboardActuatorForm.Shortcut =
                dataGridView2.Rows[e.RowIndex].Cells[ShortcutColumn.Name].Value as String;

            var dialogResult = editKeyboardActuatorForm.ShowDialog();

            Show();

            if (dialogResult == DialogResult.OK)
            {
                dataGridView2.Rows[e.RowIndex].Cells[TriggerColumn.Name].Value =
                    editKeyboardActuatorForm.IsTriggerSelect;

                if (editKeyboardActuatorForm.IsTriggerSelect)
                {
                    row.Cells[CommandColumn.Name].Value = _unmappedValue;
                }
                else
                {
                    dataGridView2.Rows[e.RowIndex].Cells[CommandColumn.Name].Value =
                        formatCommandForDisplay(editKeyboardActuatorForm.MappedCommand);
                    dataGridView2.Rows[e.RowIndex].Cells[CommandColumn.Name].Tag =
                        dataGridView2.Rows[e.RowIndex].Cells[CommandColumn.Name].Value;
                }

                if (editKeyboardActuatorForm.ShortcutChanged)
                {
                    dataGridView2.Rows[e.RowIndex].Cells[ShortcutColumn.Name].Value = editKeyboardActuatorForm.Shortcut;
                }

                _isDirty = true;
            }
        }

        /// <summary>
        /// User clicked on the "Remove" button to remove a switch.
        /// Handles it.
        /// </summary>
        /// <param name="e">event args</param>
        private void handleRemoveSwitch(DataGridViewCellEventArgs e)
        {
            var shortcut = dataGridView2.Rows[e.RowIndex].Cells[ShortcutColumn.Name].Value as String;

            var result = MessageBox.Show("Delete shortcut " + shortcut, Text, MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                return;
            }

            var switchSetting = dataGridView2.Rows[e.RowIndex].Tag as SwitchSetting;

            _actuatorSetting.SwitchSettings.Remove(switchSetting);

            _isDirty = true;

            refreshDataGridView();
        }

        /// <summary>
        /// User clicked the trigger select checkbox.
        /// </summary>
        /// <param name="e">event args</param>
        private void handleTriggerSelect(DataGridViewCellEventArgs e)
        {
            var row = dataGridView2.Rows[e.RowIndex];

            var checkCell = (DataGridViewCheckBoxCell) row.Cells[e.ColumnIndex];

            bool isChecked = (Boolean) checkCell.Value;
            if (isChecked)
            {
                row.Cells[CommandColumn.Name].Value = _unmappedValue;
            }
            else if (row.Cells[CommandColumn.Name].Tag is String)
            {
                row.Cells[CommandColumn.Name].Value = row.Cells[CommandColumn.Name].Tag;
            }
        }

        /// <summary>
        /// Initializes the UI elements in the datagrid view such as widths
        /// of columns, column header text etc
        /// </summary>
        private void initializeUI()
        {
            //dataGridView2.Width = Width - SystemInformation.VerticalScrollBarWidth;

            dataGridView2.AutoResizeRows();

            dataGridView2.ScrollBars = ScrollBars.Vertical;
            dataGridView2.RowHeadersVisible = false;

            setColumnWidths();

            dataGridView2.CellValueChanged += dataGridView2_CellValueChanged;
            dataGridView2.CurrentCellDirtyStateChanged += dataGridView2_CurrentCellDirtyStateChanged;
            dataGridView2.CellContentClick += dataGridView2_CellContentClick;
        }

        /// <summary>
        /// OnLoad handler for the form. Init the UI and populate
        /// the datagridview
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="eventArgs">eventargs</param>
        private void OnLoad(object sender, EventArgs eventArgs)
        {
            if (Actuator == null)
            {
                MessageBox.Show("Error.  Actuator to configure is null");
                Close();
            }

            float currentAspectRatio = (float)ClientSize.Height / ClientSize.Width;

            if (_designTimeAspectRatio != 0.0f && currentAspectRatio != _designTimeAspectRatio)
            {
                ClientSize = new System.Drawing.Size(ClientSize.Width, (int)(_designTimeAspectRatio * ClientSize.Width));
            }

            TopMost = false;
            TopMost = true;

            if (!String.IsNullOrEmpty(Title))
            {
                Text = Title;
            }
            else
            {
                Text = Text + " - " + Actuator.Name;
            }

            initializeUI();

            _actuatorConfig = ActuatorConfig.Load();

            _actuatorSetting = _actuatorConfig.Find(Actuator.Descriptor.Id);

            refreshDataGridView();
        }

        /// <summary>
        /// Refreshes the Gridview with data from the switches
        /// </summary>
        private void refreshDataGridView()
        {
            dataGridView2.Rows.Clear();

            foreach (var switchSetting in _actuatorSetting.SwitchSettings)
            {
                addDataGridRow(switchSetting);
            }

            dataGridView2.AutoResizeRows();
        }

        /// <summary>
        /// Sets the widths of the columns as a percentage of the
        /// grid width
        /// </summary>
        /// <param name="column">which column</param>
        /// <param name="percent">width percentage</param>
        private void setColumnWidthPercent(DataGridViewColumn column, int percent)
        {
            int w = dataGridView2.Width - SystemInformation.VerticalScrollBarWidth;
            column.Width = (w*percent)/100;
        }

        /// <summary>
        /// Sets the widths of the columns in the datagrid
        /// </summary>
        private void setColumnWidths()
        {
            int w = dataGridView2.Width - SystemInformation.VerticalScrollBarWidth;

            setColumnWidthPercent(ShortcutColumn, 30);
            setColumnWidthPercent(CommandColumn, 25);
            setColumnWidthPercent(EnableColumn, 11);
            setColumnWidthPercent(TriggerColumn, 11);
            setColumnWidthPercent(EditSwitchColumn, 11);
            setColumnWidthPercent(DeleteSwitchColumn, 11);

            dataGridView2.AllowUserToResizeColumns = false;
        }

        /// <summary>
        /// Checks if the specified switch name already exists
        /// </summary>
        /// <param name="name">name to check</param>
        /// <returns>true if it does</returns>
        bool switchNameExists(String name)
        {
            foreach (var actuatorSwitch in Actuator.Switches)
            {
                if (String.Compare(name, actuatorSwitch.Name, true) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets data from the datagrid view, updates the switch settings
        /// and saves the settings to file
        /// </summary>
        private void updateDataFromUIAndSave()
        {
            var actuatorConfig = ActuatorConfig.Load();

            for (int ii = 0; ii < dataGridView2.Rows.Count; ii++)
            {
                var switchSetting = dataGridView2.Rows[ii].Tag as SwitchSetting;
                if (switchSetting == null)
                {
                    continue;
                }

                switchSetting.Enabled = (Boolean) dataGridView2[EnableColumn.Name, ii].Value;

                bool isTrigger = (Boolean) dataGridView2[TriggerColumn.Name, ii].Value;

                if (isTrigger)
                {
                    switchSetting.ConfigureAsTriggerSwitch(isTrigger);
                }
                else
                {
                    var command = dataGridView2[CommandColumn.Name, ii].Value as String;
                    if (command == _unmappedValue)
                    {
                        switchSetting.Command = String.Empty;
                    }
                    else
                    {
                        if (!command.StartsWith("@"))
                        {
                            command = "@" + command;
                        }

                        switchSetting.Command = command;
                    }
                }

                switchSetting.Source = dataGridView2[ShortcutColumn.Name, ii].Value as String;
            }

            var actuatorSetting = actuatorConfig.Find(Actuator.Name);

            if (actuatorSetting != null)
            {
                actuatorSetting.SwitchSettings = _actuatorSetting.SwitchSettings;

                actuatorConfig.Save();
            }
        }

        /// <summary>
        /// Performs validation to make sure everything is oK.
        /// Displays error if validation failed
        /// </summary>
        /// <returns>true if everything's fine</returns>
        private bool validate()
        {
            bool ok = false;

            if (!noDuplicateShortcuts())
            {
                MessageBox.Show("Error! Duplicate shortcuts found. Shorcuts have to be unique", Actuator.Name);
                return false;
            }

            for (int ii = 0; ii < dataGridView2.Rows.Count; ii++)
            {
                if ((Boolean) dataGridView2[TriggerColumn.Name, ii].Value)
                {
                    ok = true;
                    break;
                }
            }

            if (!ok)
            {
                MessageBox.Show("Warning! You have not set any of the switches to select on trigger", Actuator.Name);
            }

            ok = false;

            for (int ii = 0; ii < dataGridView2.Rows.Count; ii++)
            {
                if ((Boolean) dataGridView2[EnableColumn.Name, ii].Value)
                {
                    ok = true;
                }
            }

            if (!ok)
            {
                MessageBox.Show("Warning! You have disabled all switches", Actuator.Name);
            }

            return true;
        }

        /// <summary>
        /// Checks if all the shortcuts are unique
        /// </summary>
        /// <returns>true if there are no duplicates</returns>
        private bool noDuplicateShortcuts()
        {
            List<String> array = new List<string>();
            for (int ii = 0; ii < dataGridView2.Rows.Count; ii++)
            {
                array.Add(dataGridView2[ShortcutColumn.Name, ii].Value as String);
            }

            return (array.Distinct().Count() == array.Count());
        }
    }
}