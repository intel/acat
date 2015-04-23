////////////////////////////////////////////////////////////////////////////
// <copyright file="KeyLogTextControlAgent.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2015 Intel Corporation 
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using ACAT.Lib.Core.Utility;

#region SupressStyleCopWarnings

[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1126:PrefixCallsCorrectly",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1101:PrefixLocalCallsWithThis",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1121:UseBuiltInTypeAlias",
        Scope = "namespace",
        Justification = "Since they are just aliases, it doesn't really matter")]
[module: SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1200:UsingDirectivesMustBePlacedWithinNamespace",
        Scope = "namespace",
        Justification = "ACAT guidelines")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1309:FieldNamesMustNotBeginWithUnderscore",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private fields begin with an underscore")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1300:ElementMustBeginWithUpperCaseLetter",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private/Protected methods begin with lowercase")]

#endregion SupressStyleCopWarnings

namespace ACAT.Lib.Core.AgentManagement.TextInterface
{
    /// <summary>
    /// Uses key logging to manipulate text in the target text control. Use
    /// this class if the text control cannot be manipulated with Windows
    /// automation.  Key logging is where a hidden window is created. This
    /// acts as a shadow window into which all the editing changes are made
    /// to shadow the changes made in the actual text window.
    /// </summary>
    public class KeyLogTextControlAgent : TextControlAgentBase
    {
        /// <summary>
        /// Windows constant
        /// </summary>
        private const int WM_KEYDOWN = 0x100;

        /// <summary>
        /// Windows constant
        /// </summary>
        private const int WM_KEYUP = 0x101;

        /// <summary>
        /// Indicates if this is in a paused state
        /// </summary>
        private static volatile bool _paused;

        /// <summary>
        /// Used to generate the name of the shadow window
        /// </summary>
        private static int _textBoxNameCounter = 1;

        /// <summary>
        /// Shadow text box control
        /// </summary>
        private TextBox _textBox;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public KeyLogTextControlAgent()
        {
            _paused = false;
            createTextBox();
        }

        /// <summary>
        /// Returns the native windows handle to the shadow
        /// text box
        /// </summary>
        private IntPtr textBoxHandle
        {
            get
            {
                return (_textBox != null) ? _textBox.Handle : IntPtr.Zero;
            }
        }

        /// <summary>
        /// Clears text in the text box
        /// </summary>
        public override void ClearText()
        {
            if (isValid(_textBox))
            {
                Windows.SetText(_textBox, String.Empty);
            }
        }

        /// <summary>
        /// Returns caret position
        /// </summary>
        /// <returns>caret position</returns>
        public override int GetCaretPos()
        {
            Log.Debug();
            if (isValid(_textBox))
            {
                return Windows.GetCaretPosition(_textBox);
            }

            return -1;
        }

        /// <summary>
        /// Returns highlighted text if any
        /// </summary>
        /// <returns>highlighted text</returns>
        public override String GetSelectedText()
        {
            return isValid(_textBox) ? Windows.GetSelectedText(_textBox) : String.Empty;
        }

        /// <summary>
        /// Gets the string of text from the shadow text control
        /// </summary>
        /// <returns>text</returns>
        public override String GetText()
        {
            return isValid(_textBox) ? Windows.GetText(_textBox) : String.Empty;
        }

        /// <summary>
        /// Indicates whether the previous word marks the beginning
        /// of the sentence
        /// </summary>
        /// <returns>true if so</returns>
        public override bool IsPreviousWordAtCaretTheFirstWord()
        {
            String word;

            int startPos = GetPreviousWordAtCaret(out word);
            var text = GetText();
            bool isFirstWord = true;
            for (int ii = 0; ii < startPos; ii++)
            {
                if (!Char.IsWhiteSpace(text[ii]))
                {
                    isFirstWord = false;
                    break;
                }
            }

            return !isFirstWord && TextUtils.IsPrevSentenceTerminator(text, startPos);
        }

        /// <summary>
        /// Invoked on a key down event.  Clear the text in the
        /// shadow box on certain keys
        /// </summary>
        /// <param name="keyEventArgs">event arg</param>
        public override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (_paused)
            {
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.LineFeed:
                case Keys.Enter:
                case Keys.Home:
                case Keys.End:
                case Keys.Up:
                case Keys.Down:
                case Keys.PageDown:
                case Keys.PageUp:
                    resetTextBox();
                    break;

                case Keys.Left:
                    if (GetCaretPos() == 0)
                    {
                        resetTextBox();
                    }

                    Windows.PostMessage(textBoxHandle, WM_KEYDOWN, (int)e.KeyCode, 0);
                    break;

                case Keys.Delete:
                    Windows.PostMessage(textBoxHandle, WM_KEYDOWN, (int)e.KeyCode, 0);
                    onTextChanged();
                    break;

                default:
                    Windows.PostMessage(textBoxHandle, WM_KEYDOWN, (int)e.KeyCode, 0);
                    break;
            }

            Log.Debug("Keycode: " + e.KeyCode +
                        ", cursor: " + ((_textBox != null) ? _textBox.SelectionStart.ToString() : "-1"));
        }

        /// <summary>
        /// Invoked on a key up
        /// </summary>
        /// <param name="keyEventArgs">event arg</param>
        public override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (_paused)
            {
                return;
            }

            if (_textBox != null)
            {
                Log.Debug("keyup: " + e.KeyCode);

                switch (e.KeyCode)
                {
                    case Keys.LineFeed:
                    case Keys.Enter:
                    case Keys.Home:
                    case Keys.End:
                    case Keys.Up:
                    case Keys.Down:
                    case Keys.PageDown:
                    case Keys.PageUp:
                        break;

                    case Keys.Delete:
                        Windows.PostMessage(textBoxHandle, WM_KEYUP, (int)e.KeyCode, 0xC0000001);
                        break;

                    case Keys.Right:
                    case Keys.Left:
                        onTextChanged();
                        break;

                    default:
                        Windows.PostMessage(textBoxHandle, WM_KEYUP, (int)e.KeyCode, 0xC0000001);
                        break;
                }
            }
        }

        /// <summary>
        /// Invoked on a mouse down event
        /// </summary>
        /// <param name="mouseEventArgs">event args</param>
        public override void OnMouseDown(MouseEventArgs mouseEventArgs)
        {
            resetTextBox();
        }

        /// <summary>
        /// Pause.  Don't make any text changes.
        /// </summary>
        public override void Pause()
        {
            _paused = true;
        }

        /// <summary>
        /// Resume making text changes
        /// </summary>
        public override void Resume()
        {
            _paused = false;
        }

        /// <summary>
        /// Sets the caret position in the target text control by
        /// simulating a right cursor or left cursor movement. This is
        /// because we have no access to the target text control through
        /// Windows SDK.
        /// </summary>
        /// <param name="setPos">where to set the caret</param>
        /// <returns>true on success</returns>
        public override bool SetCaretPos(int setPos)
        {
            if (isValid(_textBox))
            {
                return false;
            }

            int currentPos = Windows.GetCaretPosition(_textBox);

            if (setPos > currentPos)
            {
                for (int ii = 0; ii < setPos - currentPos; ii++)
                {
                    Keyboard.ExtendedKeyDown(Keys.Right);
                    Keyboard.ExtendedKeyUp(Keys.Right);
                }
            }
            else if (setPos < currentPos)
            {
                for (int ii = 0; ii < currentPos - setPos; ii++)
                {
                    Keyboard.ExtendedKeyDown(Keys.Left);
                    Keyboard.ExtendedKeyUp(Keys.Left);
                }
            }

            return true;
        }

        /// <summary>
        /// Sets focus to the text box
        /// </summary>
        /// <returns>true on success</returns>
        public override bool SetFocus()
        {
            return isValid(_textBox) && Windows.SetFocus(_textBox);
        }

        /// <summary>
        /// Invoked on disposal
        /// </summary>
        protected override void OnDispose()
        {
            disposeTextInterface();
        }

        /// <summary>
        /// Event handler invoked when the text changes in the textbox control
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">arg</param>
        private void _textBox_TextChanged(object sender, EventArgs e)
        {
            Log.Debug();
            onTextChanged();
        }

        /// <summary>
        /// Creates the shadow text box
        /// </summary>
        private void createTextBox()
        {
            if (_textBox == null)
            {
                var name = String.Format("NullAgent_TB_" + _textBoxNameCounter++);
                Log.Debug("Creating textbox window " + name);
                _textBox = new TextBox { Name = name, Multiline = true };
                _textBox.TextChanged += _textBox_TextChanged;
            }
            else
            {
                resetTextBox();
            }
        }

        /// <summary>
        /// Dispose resources
        /// </summary>
        private void disposeTextInterface()
        {
            Log.Debug();
            if (isValid(_textBox))
            {
                _textBox.TextChanged -= _textBox_TextChanged;
                _textBox.Dispose();
                _textBox = null;
            }
        }

        /// <summary>
        /// Is the specified text box still valid? Has it
        /// been disposed off?
        /// </summary>
        /// <param name="textBox">Textbox to verify</param>
        /// <returns>true if valid</returns>
        private bool isValid(TextBox textBox)
        {
            return textBox != null && !textBox.IsDisposed;
        }

        /// <summary>
        /// Raises event that the text has changed
        /// </summary>
        private void onTextChanged()
        {
            Log.Debug("textbox changed. Name: " + _textBox.Name +
                        ", caretPos = " + _textBox.SelectionStart +
                        ", _textBox: " + ((_textBox == null) ? "null" : _textBox.Text));
            triggerTextChanged(this);
        }

        /// <summary>
        /// Clears the text in the shadow text control
        /// </summary>
        private void resetTextBox()
        {
            if (isValid(_textBox))
            {
                Windows.SetText(_textBox, String.Empty);
            }

            onTextChanged();
        }
    }
}