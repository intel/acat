////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// SwitchCommandMapForm.cs
//
// Configure switch command mapping
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.CommandManagement;
using ACAT.Lib.Core.PanelManagement;
using System;
using System.Windows.Forms;

namespace ACAT.Lib.Core.ActuatorManagement
{
    public partial class SwitchCommandMapForm : Form
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
        /// Wrap text in rows?
        /// </summary>
        private bool _wrapText = true;

        public SwitchCommandMapForm()
        {
            InitializeComponent();

            SelectedCommand = String.Empty;

            Load += SwitchCommandMapForm_Load;
        }

        public String SelectedCommand { get; private set; }

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

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                var row = dataGridView2.SelectedRows[0];

                SelectedCommand = row.Cells[0].Value as String;

                // MessageBox.Show("Selected Command: " + SelectedCommand, Text);
                bool result = ConfirmBox.ShowDialog("Selected Command: " + SelectedCommand, null, false);
            }
            Close();
        }

        private void checkBoxWrapText_CheckedChanged(object sender, EventArgs e)
        {
            _wrapText = checkBoxWrapText.Checked;
            wrapText(_wrapText);
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedCells.Count > 0)
            {
                int selectedrowindex = dataGridView2.SelectedCells[0].RowIndex;

                DataGridViewRow selectedRow = dataGridView2.Rows[selectedrowindex];
                dataGridView2.Rows[selectedrowindex].Selected = true;
            }
        }

        /// <summary>
        /// Initializes the UI elements in the datagrid view such as widths
        /// of columns, column header text etc
        /// </summary>
        private void initializeUI()
        {
            dataGridView2.AutoResizeRows();

            //SwitchNameColumn.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            dataGridView2.ScrollBars = ScrollBars.Vertical;
            dataGridView2.RowHeadersVisible = false;
            dataGridView2.SelectionChanged += dataGridView1_SelectionChanged;

            setColumnWidths();
        }

        /// <summary>
        /// Refreshes the Gridview with data from the switches
        /// </summary>
        private void refreshDataGridView()
        {
            foreach (var cmdDescriptor in CommandManager.Instance.AppCommandTable.CmdDescriptors)
            {
                if (cmdDescriptor.EnableSwitchMap)
                {
                    dataGridView2.Rows.Add(cmdDescriptor.Command, cmdDescriptor.Description);
                }
            }

            dataGridView2.AutoResizeRows();

            wrapText(true);

            if (dataGridView2.Rows.Count > 0)
            {
                dataGridView2.Rows[0].Selected = true;
            }
        }

        /// <summary>
        /// Sets the widths of the columns in the datagrid
        /// </summary>
        private void setColumnWidths()
        {
            int w = dataGridView2.Width - SystemInformation.VerticalScrollBarWidth;

            CommandColumn.Width = w / 3;
            DescriptionColumn.Width = 2 * w / 3;
        }

        private void SwitchCommandMapForm_Load(object sender, EventArgs e)
        {
            float currentAspectRatio = (float)ClientSize.Height / ClientSize.Width;

            if (_designTimeAspectRatio != 0.0f && currentAspectRatio != _designTimeAspectRatio)
            {
                ClientSize = new System.Drawing.Size(ClientSize.Width, (int)(_designTimeAspectRatio * ClientSize.Width));
            }

            CenterToScreen();

            TopMost = false;
            TopMost = true;

            if (!String.IsNullOrEmpty(Title))
            {
                Text = Title;
            }

            checkBoxWrapText.Checked = _wrapText;

            initializeUI();

            refreshDataGridView();
        }

        /// <summary>
        /// Wraps text in rows
        /// </summary>
        /// <param name="onOff">whether to wrap or not</param>
        private void wrapText(bool onOff)
        {
            DataGridViewTextBoxColumn tbc = dataGridView2.Columns[1] as DataGridViewTextBoxColumn;
            tbc.DefaultCellStyle.WrapMode = (onOff) ? DataGridViewTriState.True : DataGridViewTriState.False;
            dataGridView2.AutoResizeRows();
        }
    }
}