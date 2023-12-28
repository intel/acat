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
    /// The text box control that is displayed in the talk app when the
    /// PHRASE MODE is selected for word prediction
    /// </summary>
    public partial class TalkWindowTextBoxPhraseModeUserControl : UserControl, ITalkWindowTextBox
    {
        private int _caretPos;

        public TalkWindowTextBoxPhraseModeUserControl(Form parent, Control container)
        {
            InitializeComponent();
            Load += TalkWindowTextBoxPhraseModeUserControl_Load;
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

        private void TalkWindowTextBoxPhraseModeUserControl_Load(object sender, EventArgs e)
        {
            TextBoxTalkWindow.Prompt = "Enter keywords to search";
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