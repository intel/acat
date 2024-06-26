////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// NewKeywordDialog.cs
//
// Dialog to add a new keyword for CRG
//
////////////////////////////////////////////////////////////////////////////
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ACAT.Applications.ConvAssistTestApp
{
    public partial class NewKeywordDialog : Form
    {
        public String NewKeyword;
        private readonly Color buttonBackColor;
        public NewKeywordDialog()
        {
            InitializeComponent();

            buttonBackColor = buttonAdd.BackColor;

            Load += NewKeywordDialog_Load;
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            NewKeyword = textBox1.Text;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void enableAddButton(bool enabled)
        {
            buttonAdd.Enabled = enabled;
            buttonAdd.BackColor = (enabled) ? buttonBackColor : Color.LightGray;
        }

        private void NewKeywordDialog_Load(object sender, EventArgs e)
        {
            CenterToParent();
            textBox1.KeyPress += TextBox1_KeyPress;
            enableAddButton(false);
            textBox1.TextChanged += TextBox1_TextChanged;
        }

        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check if the Enter key is pressed
            if (e.KeyChar == (char)Keys.Enter)
            {
                // Close the dialog form
                this.DialogResult = DialogResult.OK;
                NewKeyword = textBox1.Text;
                this.Close();
            }
            else if (e.KeyChar == (char)Keys.Escape)
            {
                // Close the dialog form
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            enableAddButton(!String.IsNullOrEmpty(textBox1.Text.Trim()));
        }
    }
}