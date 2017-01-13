////////////////////////////////////////////////////////////////////////////
// <copyright file="ConfigureLaunchAppSettings.cs" company="Intel Corporation">
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

using LaunchAppAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.FunctionalAgents.LaunchAppAgent
{
    /// <summary>
    /// Displays the list of apps to launch.  User can add/edit/delete
    /// apps
    /// </summary>
    public partial class ConfigureLaunchAppSettings : Form
    {
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
        public ConfigureLaunchAppSettings()
        {
            InitializeComponent();

            CenterToScreen();

            Load += OnLoad;
        }

        /// <summary>
        /// Gets or sets the list of apps
        /// </summary>
        public List<AppInfo> Applications { get; set; }

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
        /// Adds a new row to the datagrid representing a new app
        /// </summary>
        /// <param name="appInfo">application info</param>
        private void addDataGridRow(AppInfo appInfo)
        {
            int rowNum = dataGridView2.Rows.Add(appInfo.Name, appInfo.Path, appInfo.CommandLine);
            dataGridView2.Rows[rowNum].Tag = appInfo;

            (dataGridView2[ChangeColumn.Name, rowNum] as DataGridViewButtonCell).Value = "Change";
            (dataGridView2[DeleteColumn.Name, rowNum] as DataGridViewButtonCell).Value = "Remove";
        }

        /// <summary>
        /// Event handler to add a new app.  Displays a form for the
        /// user to enter app info
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonAddNew_Click(object sender, EventArgs e)
        {
            Hide();

            var form = new EditAppInfoForm();

            var dialogResult = form.ShowDialog();

            Show();

            if (dialogResult == DialogResult.OK)
            {
                _isDirty = true;

                Applications.Add(new AppInfo(form.AppName, form.Path, form.Arguments));

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
        /// User clicked OK.  Get data from the UI, and update the Applications list
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
                    updateDataFromUI();
                    DialogResult = DialogResult.OK;
                }

                Close();
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
            var senderGrid = (DataGridView)sender;

            if (e.RowIndex < 0)
            {
                return;
            }

            if (senderGrid.Columns[e.ColumnIndex] == ChangeColumn)
            {
                handleEditAppInfo(e);
            }
            else if (senderGrid.Columns[e.ColumnIndex] == DeleteColumn)
            {
                handleDeleteAppInfo(e);
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
        /// User clicked on the "Remove" button to remove an app
        /// </summary>
        /// <param name="e">event args</param>
        private void handleDeleteAppInfo(DataGridViewCellEventArgs e)
        {
            var name = dataGridView2.Rows[e.RowIndex].Cells[AppNameColumn.Name].Value as String;

            var result = MessageBox.Show("Delete application " + name, Text, MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                return;
            }

            updateDataFromUI();

            foreach (var appInfo in Applications)
            {
                var tag = dataGridView2.Rows[e.RowIndex].Tag as AppInfo;
                if (tag == appInfo)
                {
                    Applications.Remove(appInfo);
                    break;
                }
            }

            _isDirty = true;

            refreshDataGridView();
        }

        /// <summary>
        /// User wants to edit app info for an application. Display
        /// form to edit the info
        /// </summary>
        /// <param name="e">event args</param>
        private void handleEditAppInfo(DataGridViewCellEventArgs e)
        {
            var row = dataGridView2.Rows[e.RowIndex];

            Hide();

            var form = new EditAppInfoForm
            {
                AppName = row.Cells[AppNameColumn.Name].Value as String,
                Path = row.Cells[PathColumn.Name].Value as String,
                Arguments = row.Cells[ArgumentsColumn.Name].Value as String
            };

            var dialogResult = form.ShowDialog();

            Show();

            if (dialogResult == DialogResult.OK)
            {
                row.Cells[AppNameColumn.Name].Value = form.AppName;
                row.Cells[PathColumn.Name].Value = form.Path;
                row.Cells[ArgumentsColumn.Name].Value = form.Arguments;

                _isDirty = true;
            }
        }

        /// <summary>
        /// Initializes the UI elements in the datagrid view such as widths
        /// of columns, column header text etc
        /// </summary>
        private void initializeUI()
        {
            dataGridView2.AutoResizeRows();

            dataGridView2.ScrollBars = ScrollBars.Vertical;
            dataGridView2.RowHeadersVisible = false;

            setColumnWidths();

            dataGridView2.CurrentCellDirtyStateChanged += dataGridView2_CurrentCellDirtyStateChanged;
            dataGridView2.CellContentClick += dataGridView2_CellContentClick;
        }

        /// <summary>
        /// Checks if all the application names are unique
        /// </summary>
        /// <returns>true if there are no duplicates</returns>
        private bool noDuplicateNames()
        {
            var array = new List<string>();

            for (int ii = 0; ii < dataGridView2.Rows.Count; ii++)
            {
                array.Add(dataGridView2[AppNameColumn.Name, ii].Value as String);
            }

            return array.Distinct().Count() == array.Count;
        }

        /// <summary>
        /// OnLoad handler for the form. Init the UI and populate
        /// the datagridview
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="eventArgs">eventargs</param>
        private void OnLoad(object sender, EventArgs eventArgs)
        {
            float currentAspectRatio = (float)ClientSize.Height / ClientSize.Width;

            if (_designTimeAspectRatio != 0.0f && currentAspectRatio != _designTimeAspectRatio)
            {
                ClientSize = new System.Drawing.Size(ClientSize.Width, (int)(_designTimeAspectRatio * ClientSize.Width));
            }

            TopMost = false;
            TopMost = true;

            initializeUI();

            refreshDataGridView();
        }

        /// <summary>
        /// Refreshes the Gridview with data from the Appplications list
        /// </summary>
        private void refreshDataGridView()
        {
            dataGridView2.Rows.Clear();

            foreach (var appInfo in Applications)
            {
                addDataGridRow(appInfo);
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
            column.Width = (w * percent) / 100;
        }

        /// <summary>
        /// Sets the widths of the columns in the datagrid
        /// </summary>
        private void setColumnWidths()
        {
            int w = dataGridView2.Width - SystemInformation.VerticalScrollBarWidth;

            setColumnWidthPercent(AppNameColumn, 20);
            setColumnWidthPercent(PathColumn, 30);
            setColumnWidthPercent(ArgumentsColumn, 30);
            setColumnWidthPercent(ChangeColumn, 10);
            setColumnWidthPercent(DeleteColumn, 10);

            dataGridView2.AllowUserToResizeColumns = false;
        }

        /// <summary>
        /// Gets data from the datagrid view, updates the Applications list
        /// </summary>
        private void updateDataFromUI()
        {
            List<AppInfo> list = new List<AppInfo>();
            for (int ii = 0; ii < dataGridView2.Rows.Count; ii++)
            {
                var appInfo = dataGridView2.Rows[ii].Tag as AppInfo;
                if (appInfo == null)
                {
                    continue;
                }

                appInfo.Name = dataGridView2[AppNameColumn.Name, ii].Value as String;
                appInfo.Path = dataGridView2[PathColumn.Name, ii].Value as String;
                appInfo.CommandLine = dataGridView2[ArgumentsColumn.Name, ii].Value as String;

                list.Add(appInfo);
            }

            Applications = list;
        }

        /// <summary>
        /// Performs validation to make sure everything is oK.
        /// Displays error if validation failed
        /// </summary>
        /// <returns>true if everything's fine</returns>
        private bool validate()
        {
            if (!noDuplicateNames())
            {
                MessageBox.Show("Error! Duplicate application names found. Names have to be unique", Text);
                return false;
            }

            return true;
        }
    }
}