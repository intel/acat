////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ConfigureSwitchesForm.cs
//
// Displays a list of switches for an acutator to enable configure
// settings for each of the switches.  User can enable/disable
// switches, turn on/off whether the switch should act as a trigger.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PanelManagement;
using System;
using System.Windows.Forms;

namespace ACAT.Lib.Core.ActuatorManagement
{
    /// <summary>
    /// Displays a list of switches for an acutator to enable configure
    /// settings for each of the switches.  User can enable/disable
    /// switches, turn on/off whether the switch should act as a trigger.
    /// </summary>
    public partial class ConfigureSwitchesForm : Form
    {
        private const String _unmappedValue = "--";

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
        /// Wrap text in rows?
        /// </summary>
        private bool _wrapText = true;

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public ConfigureSwitchesForm()
        {
            InitializeComponent();

            CenterToScreen();
            AllowMultiEnable = true;

            SwitchNameColumnHeaderText = "Switch";
            DescriptionColumnHeaderText = "Description";
            EnableColumnHeaderText = "Enable";
            TriggerColumnHeaderText = "Trigger Select";

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
        /// Gets or sets the title of the form
        /// </summary>
        public String Title { get; set; }

        /// <summary>
        /// Gets or sets the column header for the trigger column
        /// </summary>
        public String TriggerColumnHeaderText { get; set; }

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
        /// Cancels out of the dialog
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            /*if (!_isDirty || MessageBox.Show("Changes not saved. Quit anyway?",
                                Text, MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) == DialogResult.Yes)*/

            if (!_isDirty || ConfirmBox.ShowDialog("Changes not saved. Quit anyway?", null, false))
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
                /*if (_isDirty &&
                    MessageBox.Show("Save changes?",
                                        Text, MessageBoxButtons.YesNo,
                                        MessageBoxIcon.Question) == DialogResult.Yes)*/
                if (_isDirty && ConfirmBox.ShowDialog("Save changes?", null, false))
                {
                    updateDataFromUIAndSave();
                    DialogResult = DialogResult.OK;
                }

                Close();
            }
        }

        /// <summary>
        /// Wrap or unwrap text
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void checkBoxWrapText_CheckedChanged(object sender, EventArgs e)
        {
            _wrapText = checkBoxWrapText.Checked;
            wrapText(_wrapText);
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] == TriggerColumn)
            {
                var row = dataGridView2.Rows[e.RowIndex];
                var checkCell = (DataGridViewCheckBoxCell)row.Cells[e.ColumnIndex];

                bool isChecked = (Boolean)checkCell.Value;
                if (isChecked)
                {
                    row.Cells[CommandColumn.Name].Value = _unmappedValue;
                }
                else if (row.Cells[CommandColumn.Name].Tag is String)
                {
                    row.Cells[CommandColumn.Name].Value = row.Cells[CommandColumn.Name].Tag;
                }
            }
            else if (e.RowIndex >= 0 && senderGrid.Columns[e.ColumnIndex] == MapColumn)
            {
                var switchName = dataGridView2.Rows[e.RowIndex].Cells[SwitchNameColumn.Name].Value;

                Hide();

                // var switchCommandMapForm = new SwitchCommandMapForm {Title = "Map Command to Switch " + switchName};
                var switchCommandMapForm = new SwitchCommandMapForm { Title = "Map Command to Switch " + switchName };
                switchCommandMapForm.ShowDialog();

                Show();

                if (!String.IsNullOrEmpty(switchCommandMapForm.SelectedCommand))
                {
                    dataGridView2.Rows[e.RowIndex].Cells[CommandColumn.Name].Value = formatCommandForDisplay(switchCommandMapForm.SelectedCommand);

                    dataGridView2.Rows[e.RowIndex].Cells[CommandColumn.Name].Tag = dataGridView2.Rows[e.RowIndex].Cells[CommandColumn.Name].Value;

                    dataGridView2.Rows[e.RowIndex].Cells[TriggerColumn.Name].Value = false;

                    _isDirty = true;
                }

                switchCommandMapForm.Dispose();
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

        private String formatCommandForDisplay(String command)
        {
            if (command.StartsWith("@") && command.Length > 1)
            {
                command = command.Substring(1);
            }

            return command;
        }

        /// <summary>
        /// Initializes the UI elements in the datagrid view such as widths
        /// of columns, column header text etc
        /// </summary>
        private void initializeUI()
        {
            dataGridView2.AutoResizeRows();

            SwitchNameColumn.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            dataGridView2.ScrollBars = ScrollBars.Vertical;
            dataGridView2.RowHeadersVisible = false;

            setColumnWidths();

            setColumnHeaderText();

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
                // MessageBox.Show("Error.  Actuator to configure is null");
                bool result = ConfirmBox.ShowDialog("Error. Actuator to configure is null", null, false);
                Close();
            }

            float currentAspectRatio = (float)ClientSize.Height / ClientSize.Width;

            if (_designTimeAspectRatio != 0.0f && currentAspectRatio != _designTimeAspectRatio)
            {
                ClientSize = new System.Drawing.Size(ClientSize.Width, (int)(_designTimeAspectRatio * ClientSize.Width));
            }

            TopMost = false;
            TopMost = true;

            checkBoxWrapText.Checked = _wrapText;

            if (!String.IsNullOrEmpty(Title))
            {
                Text = Title;
            }
            else
            {
                Text = Text + " - " + Actuator.Name;
            }

            initializeUI();

            refreshDataGridView();
        }

        /// <summary>
        /// Refreshes the Gridview with data from the switches
        /// </summary>
        private void refreshDataGridView()
        {
            var actuatorConfig = ActuatorConfig.Load();

            var actuatorSetting = actuatorConfig.Find(Actuator.Descriptor.Id);

            foreach (var actuatorSwitch in Actuator.Switches)
            {
                var switchSetting = actuatorSetting.Find(actuatorSwitch.Name);
                if (switchSetting == null)
                {
                    continue;
                }

                int rowNum = dataGridView2.Rows.Add(actuatorSwitch.Name, actuatorSwitch.Description);
                dataGridView2.Rows[rowNum].Tag = actuatorSwitch;

                (dataGridView2[EnableColumn.Name, rowNum] as DataGridViewCheckBoxCell).Value = switchSetting.Enabled;

                (dataGridView2[TriggerColumn.Name, rowNum] as DataGridViewCheckBoxCell).Value = switchSetting.IsTriggerSwitch();

                dataGridView2[CommandColumn.Name, rowNum].Value = (switchSetting.IsTriggerSwitch() || String.IsNullOrEmpty(switchSetting.Command)) ?
                                                                    _unmappedValue :
                                                                    formatCommandForDisplay(switchSetting.Command);

                dataGridView2[CommandColumn.Name, rowNum].Tag = dataGridView2[CommandColumn.Name, rowNum].Value;

                (dataGridView2[MapColumn.Name, rowNum] as DataGridViewButtonCell).Value = "Map";
            }

            dataGridView2.AutoResizeRows();

            wrapText(true);
        }

        /// <summary>
        /// Sets the text for the column headers
        /// </summary>
        private void setColumnHeaderText()
        {
            if (!String.IsNullOrEmpty(SwitchNameColumnHeaderText))
            {
                SwitchNameColumn.HeaderText = SwitchNameColumnHeaderText;
            }

            if (!String.IsNullOrEmpty(DescriptionColumnHeaderText))
            {
                DescriptionColumn.HeaderText = DescriptionColumnHeaderText;
            }

            if (!String.IsNullOrEmpty(EnableColumnHeaderText))
            {
                EnableColumn.HeaderText = EnableColumnHeaderText;
            }

            if (!String.IsNullOrEmpty(TriggerColumnHeaderText))
            {
                TriggerColumn.HeaderText = TriggerColumnHeaderText;
            }
        }

        private void setColumnWidthPercent(DataGridViewColumn column, int percent)
        {
            int w = dataGridView2.Width - SystemInformation.VerticalScrollBarWidth;
            column.Width = (w * percent) / 100;
        }

        /// <summary>
        /// Sets the widths of the columns in the datagrid
        /// </summary>
        private void setColumnWidths()
        {
            int w = dataGridView2.Width - SystemInformation.VerticalScrollBarWidth;

            setColumnWidthPercent(SwitchNameColumn, 15);
            setColumnWidthPercent(DescriptionColumn, 30);
            setColumnWidthPercent(EnableColumn, 10);
            setColumnWidthPercent(TriggerColumn, 10);
            setColumnWidthPercent(CommandColumn, 25);
            setColumnWidthPercent(MapColumn, 10);

            dataGridView2.AllowUserToResizeColumns = false;
        }

        /// <summary>
        /// Gets data from the datagrid view, updates the switch settings
        /// and saves the settings to file
        /// </summary>
        private void updateDataFromUIAndSave()
        {
            var actuatorConfig = ActuatorConfig.Load();
            var actuatorSetting = actuatorConfig.Find(Actuator.Name);

            for (int ii = 0; ii < dataGridView2.Rows.Count; ii++)
            {
                var actuatorSwitch = dataGridView2.Rows[ii].Tag as IActuatorSwitch;
                var switchSetting = actuatorSetting.Find(actuatorSwitch.Name);
                if (switchSetting == null)
                {
                    continue;
                }

                switchSetting.Enabled = (Boolean)dataGridView2[EnableColumn.Name, ii].Value;

                bool isTrigger = (Boolean)dataGridView2[TriggerColumn.Name, ii].Value;

                if (isTrigger)
                {
                    switchSetting.ConfigureAsTriggerSwitch(isTrigger);
                }
                else
                {
                    var command = dataGridView2[CommandColumn.Name, ii].Value as String;
                    if (!command.StartsWith("@"))
                    {
                        command = "@" + command;
                    }

                    switchSetting.Command = (command != _unmappedValue) ? command : String.Empty;
                }
            }

            actuatorConfig.Save();
        }

        /// <summary>
        /// Performs validation to make sure everything is oK.
        /// Displays error if validation failed
        /// </summary>
        /// <returns>true if everything's fine</returns>
        private bool validate()
        {
            bool ok = false;

            for (int ii = 0; ii < dataGridView2.Rows.Count; ii++)
            {
                if ((Boolean)dataGridView2[TriggerColumn.Name, ii].Value)
                {
                    ok = true;
                    break;
                }
            }

            if (!ok)
            {
                // MessageBox.Show("Warning! You have not set any of the switches to select on trigger", Actuator.Name);
                bool result = ConfirmBox.ShowDialog("Warning! You have not set any of the switches to select on trigger. Actuator: " + Actuator.Name.ToString(), null, false);
            }

            ok = false;

            for (int ii = 0; ii < dataGridView2.Rows.Count; ii++)
            {
                if ((Boolean)dataGridView2[EnableColumn.Name, ii].Value)
                {
                    ok = true;
                }
            }

            if (!ok)
            {
                // MessageBox.Show("Warning! You have disabled all switches", Actuator.Name);
                bool result = ConfirmBox.ShowDialog("Warning! You have disabled all switches. Actuator: " + Actuator.Name.ToString(), null, false);
            }

            return true;
        }

        /// <summary>
        /// Wraps text in rows
        /// </summary>
        /// <param name="onOff">whether to wrap or not</param>
        private void wrapText(bool onOff)
        {
            (dataGridView2.Columns[DescriptionColumn.Name] as DataGridViewTextBoxColumn).DefaultCellStyle.WrapMode = (onOff) ? DataGridViewTriState.True : DataGridViewTriState.False;

            dataGridView2.AutoResizeRows();
        }
    }
}