using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConvAssistTest
{
    public partial class ShowDialogHistoryForm : Form
    {
        public String History;

        public ShowDialogHistoryForm()
        {
            InitializeComponent();

            Load += ShowDialogHistoryForm_Load;
        }

        private void ShowDialogHistoryForm_Load(object sender, EventArgs e)
        {
            CenterToScreen();


            textBox1.Text = History;

            Shown += ShowDialogHistoryForm_Shown;
        }

        private void ShowDialogHistoryForm_Shown(object sender, EventArgs e)
        {
            buttonClose.Focus();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
