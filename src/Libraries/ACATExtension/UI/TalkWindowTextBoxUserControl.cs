////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;
using System;
using System.Windows.Forms;

namespace ACAT.Extensions
{
    /// <summary>
    /// User control that is used by the Talk application. This is where the
    /// text typed by the user is displayed
    /// </summary>
    public partial class TalkWindowTextBoxUserControl : UserControl, ITalkWindowTextBox
    {
        private int _caretPos;

        public TalkWindowTextBoxUserControl(Form parent, Control container)
        {
            InitializeComponent();
            _caretPos = 0;
            TextBoxTalkWindow.TextChanged += TextBoxTalkWindow_TextChanged;
            parent.Activated += Parent_Activated;
            container.ControlAdded += Container_ControlAdded;
        }

        public TextBox TextBoxControl
        {
            get
            {
                return TextBoxTalkWindow;
            }
        }

        public void OnPause()
        {
            _caretPos = Windows.GetCaretPosition(TextBoxTalkWindow);
        }

        public void OnResume()
        {
            Windows.SetCaretPosition(TextBoxTalkWindow, _caretPos);
        }

        private void Container_ControlAdded(object sender, ControlEventArgs e)
        {
            if (e.Control == this)
            {
                TextBoxTalkWindow.Select();
                deselectText();
            }
        }

        private void deselectText()
        {
            var index = Windows.GetCaretPosition(TextBoxTalkWindow);
            TextBoxTalkWindow.DeselectAll();
            Windows.SetCaretPosition(TextBoxTalkWindow, index);
        }

        private void Parent_Activated(object sender, EventArgs e)
        {
            deselectText();
        }

        private void TextBoxTalkWindow_TextChanged(object sender, EventArgs e)
        {
            if (TextBoxTalkWindow.Text.Length == 0)
            {
                _caretPos = 0;
            }
        }
    }
}