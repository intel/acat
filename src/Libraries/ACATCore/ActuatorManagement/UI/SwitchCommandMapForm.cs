using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ACAT.Lib.Core.CommandManagement;

namespace ACAT.Lib.Core.ActuatorManagement
{
    public partial class SwitchCommandMapForm : Form
    {
        /// <summary>
        /// Wrap text in rows?
        /// </summary>
        private bool _wrapText = true;

        /// <summary>
        /// Aspect ratio of form at design time
        /// </summary>
        private float _designTimeAspectRatio = 0.0f;

        /// <summary>
        /// Has first call to OnClientSizeChanged been made?
        /// </summary>
        private bool _firstClientChangedCall = true;

        /// <summary>
        /// Gets or sets the title of the form
        /// </summary>
        public String Title { get; set; }

        public String SelectedCommand { get; private set; }

        public SwitchCommandMapForm()
        {
            InitializeComponent();

            SelectedCommand = String.Empty;

            Load += SwitchCommandMapForm_Load;
        }

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

        void SwitchCommandMapForm_Load(object sender, EventArgs e)
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

        private void checkBoxWrapText_CheckedChanged(object sender, EventArgs e)
        {
            _wrapText = checkBoxWrapText.Checked;
            wrapText(_wrapText);
        }

        /// <summary>
        /// Initializes the UI elements in the datagrid view such as widths
        /// of columns, column header text etc
        /// </summary>
        private void initializeUI()
        {
            dataGridView1.AutoResizeRows();

            //SwitchNameColumn.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            dataGridView1.ScrollBars = ScrollBars.Vertical;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;

            setColumnWidths();
        }

        void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
            {
                int selectedrowindex = dataGridView1.SelectedCells[0].RowIndex;

                DataGridViewRow selectedRow = dataGridView1.Rows[selectedrowindex];
                dataGridView1.Rows[selectedrowindex].Selected = true;
            }
        }


        /// <summary>
        /// Sets the widths of the columns in the datagrid
        /// </summary>
        private void setColumnWidths()
        {
            int w = dataGridView1.Width - SystemInformation.VerticalScrollBarWidth;

            CommandColumn.Width = w / 3;
            DescriptionColumn.Width = 2* w / 3;
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
                    dataGridView1.Rows.Add(cmdDescriptor.Command, cmdDescriptor.Description);
                }
            }

            dataGridView1.AutoResizeRows();

            wrapText(true);

            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.Rows[0].Selected = true;
            }
        }

        /// <summary>
        /// Wraps text in rows
        /// </summary>
        /// <param name="onOff">whether to wrap or not</param>
        private void wrapText(bool onOff)
        {
            DataGridViewTextBoxColumn tbc = dataGridView1.Columns[1] as DataGridViewTextBoxColumn;
            tbc.DefaultCellStyle.WrapMode = (onOff) ? DataGridViewTriState.True : DataGridViewTriState.False;
            dataGridView1.AutoResizeRows();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var row = dataGridView1.SelectedRows[0];

                SelectedCommand = row.Cells[0].Value as String;

                MessageBox.Show("Selected Command: " + SelectedCommand, Text);
            }
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
