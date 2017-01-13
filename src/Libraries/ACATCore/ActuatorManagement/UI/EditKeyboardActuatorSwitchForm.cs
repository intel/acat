using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACAT.Lib.Core.ActuatorManagement.UI
{
    public partial class EditKeyboardActuatorSwitchForm : Form
    {
        public String MappedCommand { get; set; }

        public bool IsTriggerSelect { get; set; }

        public String Shortcut { get; set; }

        public bool ShortcutChanged { get; private set; }

        private const string mapString = "Select Command";

        /// <summary>
        /// Aspect ratio of form at design time
        /// </summary>
        private float _designTimeAspectRatio = 0.0f;

        /// <summary>
        /// Has first call to OnClientSizeChanged been made?
        /// </summary>
        private bool _firstClientChangedCall = true;

        public EditKeyboardActuatorSwitchForm()
        {
            InitializeComponent();
            MappedCommand = String.Empty;
            Shortcut = String.Empty;

            Load += EditKeyboardActuatorSwitchForm_Load;
            textBoxKeyboardShortcut.KeyDown += textBoxKeyboardShortcut_KeyDown;
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

        void textBoxKeyboardShortcut_KeyDown(object sender, KeyEventArgs e)
        {
            String shortcut = String.Empty;

            if (e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.ShiftKey ||
                e.KeyCode == Keys.Menu || e.KeyCode == Keys.LWin || e.KeyCode == Keys.RWin)
            {
                return;
            }

            if (e.Control)
            {
                shortcut = addKey(shortcut, "Ctrl");
            }

            if (e.Alt)
            {
                shortcut = addKey(shortcut, "Alt");
            }

            if (e.Shift)
            {
                shortcut = addKey(shortcut, "Shift");
            }

            shortcut = addKey(shortcut, e.KeyCode.ToString());

            textBoxKeyboardShortcut.Text = shortcut;

            ShortcutChanged = true;

            Shortcut = shortcut;
        }

        void EditKeyboardActuatorSwitchForm_Load(object sender, EventArgs e)
        {
            float currentAspectRatio = (float)ClientSize.Height / ClientSize.Width;

            if (_designTimeAspectRatio != 0.0f && currentAspectRatio != _designTimeAspectRatio)
            {
                ClientSize = new System.Drawing.Size(ClientSize.Width, (int)(_designTimeAspectRatio * ClientSize.Width));
            }

            CenterToScreen();

            radioButtonTriggerSelect.Checked = IsTriggerSelect;
            radioButtonMapToCommand.Checked = !IsTriggerSelect;

            buttonCommand.Text = String.IsNullOrEmpty(MappedCommand) ? mapString : MappedCommand;

            if (!IsTriggerSelect)
            {
                radioButtonMapToCommand.Checked = true;
            }
            else
            {
                buttonCommand.Enabled = false;
            }

            textBoxKeyboardShortcut.Text = Shortcut;
        }

        private void radioButtonMapToCommand_CheckedChanged(object sender, EventArgs e)
        {
            radioButtonTriggerSelect.Checked = !radioButtonMapToCommand.Checked;
        }

        private void radioButtonTriggerSelect_CheckedChanged(object sender, EventArgs e)
        {
            radioButtonMapToCommand.Checked = !radioButtonTriggerSelect.Checked;
            buttonCommand.Enabled = !radioButtonTriggerSelect.Checked;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            IsTriggerSelect = radioButtonTriggerSelect.Checked;

            if (String.IsNullOrEmpty(Shortcut))
            {
                showError("Must specify a shortcut");
                return;
            }

            if (!IsTriggerSelect && String.IsNullOrEmpty(MappedCommand))
            {
                showError("Click on " + mapString + " to map a command");
                return;
            }

            DialogResult = DialogResult.OK;

            Close();
        }

        void showError(String message)
        {
            MessageBox.Show(message, Text,
                MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private string addKey(String shortcut, String key)
        {
            return string.IsNullOrEmpty(shortcut) ? key : (shortcut + "+" + key);
        }

        private void buttonCommand_Click(object sender, EventArgs e)
        {
            Hide();

            var switchCommandMapForm = new SwitchCommandMapForm { Title = "Map command to keyboard shortcut" };
            switchCommandMapForm.ShowDialog();

            Show();

            if (!String.IsNullOrEmpty(switchCommandMapForm.SelectedCommand))
            {
                MappedCommand = switchCommandMapForm.SelectedCommand;
                buttonCommand.Text = switchCommandMapForm.SelectedCommand;
            }
        }
    }
}
