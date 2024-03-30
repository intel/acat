////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System.Windows.Forms;

namespace ACAT.Extensions
{
    public partial class TalkWindowTextBoxPromptUserControlLabel : UserControl
    {
        public TalkWindowTextBoxPromptUserControlLabel(Form parent, Control container)
        {
            InitializeComponent();
        }

        public void SetText(string text)
        {
            if (label1.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    label1.Text = text;
                }));
            }
            else
            {
                label1.Text = text;
            }
        }

    }
}