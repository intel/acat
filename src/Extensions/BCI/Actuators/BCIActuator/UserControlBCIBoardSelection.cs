using System;
using System.Windows.Forms;

namespace ACAT.Extensions.BCI.Actuators.BCIActuator
{
    public partial class UserControlBCIDeviceSelection : Form
    {
        /// <summary>
        /// Event sent when exiting out of device testing completely
        /// </summary>
        public delegate void BCIDeviceSelected();

        public event BCIDeviceSelected EvtgtecUnicornSelected;

        public event BCIDeviceSelected EvtOpenBCISelected;

        public UserControlBCIDeviceSelection()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void buttonGtecUnicorn_Click(object sender, EventArgs e)
        {
            EvtgtecUnicornSelected();
        }

        private void buttonOpenBCI_Click(object sender, EventArgs e)
        {
            EvtOpenBCISelected();
        }
    }
}