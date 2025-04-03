using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BCIActuator
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
