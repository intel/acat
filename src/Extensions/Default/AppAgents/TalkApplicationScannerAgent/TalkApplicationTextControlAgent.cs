///////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// TalkApplicationTextControlAgent.cs
//
// Handles text Insert and Replae operations for the talk window
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.AgentManagement;
using ACAT.Core.AgentManagement.TextInterface;
using ACAT.Core.Utility;
using System;
using System.Text;
using System.Windows.Automation;
using System.Windows.Forms;

namespace ACAT.Extensions.AppAgents.TalkApplicationScannerAgent
{
    internal class TalkApplicationTextControlAgent : EditTextControlAgent
    {
        private readonly Control _textControl;

        public TalkApplicationTextControlAgent(Control textControl, IntPtr handle, AutomationElement editControlElement, ref bool handled) :
            base(handle, editControlElement, ref handled)
        {
            _textControl = textControl;
        }

        public override void Insert(int offset, string word)
        {
            try
            {
                AgentManager.Instance.TextChangedNotifications.Hold();

                String text = GetText();
                if (offset >= text.Length)
                {
                    if (IsTextSelected())
                    {
                        Keyboard.Send(Keys.Delete);
                    }

                    if (KeyStateTracker.IsCapsLockOn())
                    {
                        word = word.ToUpper();
                    }

                    text += word;

                    Windows.SetText(_textControl, text);

                    SetCaretPos(text.Length);

                    AgentManager.Instance.CurrentEditingMode = EditingMode.TextEntry;
                }
                else
                {
                    base.Insert(offset, word);
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            finally
            {
                AgentManager.Instance.TextChangedNotifications.Release();
            }
        }

        public override void Replace(int offset, int count, String word)
        {
            Log.Debug("HARRIS offset = " + offset + " count " + count + " word " + word);

            try
            {
                int caretPos = GetCaretPos();
                int len = GetText().Length;

                if (caretPos < len)
                {
                    base.Replace(offset, count, word);
                    return;
                }

                AgentManager.Instance.TextChangedNotifications.Hold();

                if (IsTextSelected())
                {
                    Keyboard.Send(Keys.Delete);
                }

                String text = GetText();
                var stringBuilder = new StringBuilder(text);

                stringBuilder.Remove(offset, count);

                if (KeyStateTracker.IsCapsLockOn())
                {
                    word = word.ToUpper();
                }

                stringBuilder.Insert(offset, word);

                Windows.SetText(_textControl, stringBuilder.ToString());

                SetCaretPos(offset + word.Length);

                AgentManager.Instance.CurrentEditingMode = EditingMode.TextEntry;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            finally
            {
                AgentManager.Instance.TextChangedNotifications.Release();
            }
        }
    }
}