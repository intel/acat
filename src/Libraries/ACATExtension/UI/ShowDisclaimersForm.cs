////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.ACATResources;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ACAT.Lib.Extension
{
    /// <summary>
    /// A form to display disclaimers. Disclaimers are added to
    /// the Disclaimers class by the various extensions.
    /// </summary>
    public partial class ShowDisclaimersForm : Form
    {
        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public ShowDisclaimersForm()
        {
            InitializeComponent();
            ShowInTaskbar = false;
            Load += ShowDisclaimersForm_Load;
        }

        /// <summary>
        /// Close button handler.  Close the form
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Form loader.  Get disclaimers from the disclaimers class
        /// and display them here.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void ShowDisclaimersForm_Load(object sender, EventArgs e)
        {
            TopMost = true;

            CenterToScreen();

            buttonOK.Text = R.GetString(buttonOK.Text);

            try
            {
                var sb = new StringBuilder();
                foreach (var str in Disclaimers.GetAll())
                {
                    sb.Append(str);
                    sb.AppendLine();
                    sb.AppendLine();
                }

                var text = sb.ToString().Trim();
                if (String.IsNullOrEmpty(text))
                {
                    ConfirmBoxSingleOption.ShowDialog("No disclaimers found", "OK");
                    Close();
                }
                else
                {
                    textBoxDisclaimers.Text += text;
                }
                
            }
            catch
            {
                Close();
            }
        }
    }
}