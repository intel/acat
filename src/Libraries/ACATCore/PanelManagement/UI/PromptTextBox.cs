////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;
using System;
using System.ComponentModel;
using System.Windows.Forms;

/// <summary>
/// A text box with a help prompt that is displayed when it is empty
/// </summary>
public class PromptTextBox : TextBox
{
    private const uint EM_SETCUEBANNER = 0x1501;
    private string _prompt;

    [Localizable(true)]
    public string Prompt
    {
        get
        {
            return _prompt;
        }
        set
        {
            _prompt = value;
            updatePrompt();
        }
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        updatePrompt();
    }

    private void updatePrompt()
    {
        if (this.IsHandleCreated && _prompt != null)
        {
            User32Interop.SendMessageText(this.Handle, EM_SETCUEBANNER, (IntPtr)1, _prompt);
        }
    }
}