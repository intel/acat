////////////////////////////////////////////////////////////////////////////
// <copyright file="TextControlAgentBase.cs" company="Intel Corporation">
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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WordPredictionManagement;

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
    /// Base class for all text control agents.  Text control
    /// agents handle all text manipulations and cursor navigation
    /// in the target text control window.
    /// </summary>
    public class TextControlAgentBase : ITextControlAgent
    {
        /// <summary>
        /// Keyboard interface to send keystrokes to the target
        /// text window
        /// </summary>
        private static readonly Keyboard _keyboard = new Keyboard();

        /// <summary>
        /// Text manipulation features supported by the base class
        /// </summary>
        private readonly String[] _supportedFeatures =
        {
            "Cut",
            "Copy",
            "Paste",
            "Undo",
            "Redo",
            "PreviousChar",
            "NextChar",
            "PreviousLine",
            "NextLine",
            "PreviousWord",
            "NextWord",
            "PreviousPage",
            "NextPage",
            "TopOfDoc",
            "EndOfDoc",
            "Home",
            "End",
            "SelectAll",
            "SelectMode",
            "DeletePreviousChar",
            "DeleteNextChar",
            "EnterKey"
        };

        /// <summary>
        /// Disposed?
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Current select mode.  If true, text is highlighted
        /// </summary>
        private bool _selectMode;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        public TextControlAgentBase()
        {
            _selectMode = false;
        }

        /// <summary>
        /// Raised when text changes are detected in the target
        /// text control
        /// </summary>
        public event TextChangedDelegate EvtTextChanged;

        /// <summary>
        /// Gets the keyboard interface
        /// </summary>
        public IKeyboard Keyboard
        {
            get { return _keyboard; }
        }

        /// <summary>
        /// Gets the current select mode
        /// </summary>
        protected bool selectMode
        {
            get { return _selectMode; }
        }

        public virtual bool CheckInsertOrReplaceWord(out int insertOrReplaceOffset, out String wordToReplace)
        {
            return TextUtils.CheckInsertOrReplaceWord(
                                    GetText(), 
                                    GetCaretPos(), 
                                    out insertOrReplaceOffset, 
                                    out wordToReplace);
        }

        /// <summary>
        /// Checks if the widget (scanner button) should to be enabled or not. Check
        /// our supported features list.
        /// </summary>
        /// <param name="arg">widget info</param>
        public virtual void CheckWidgetEnabled(CheckEnabledArgs arg)
        {
            if (_supportedFeatures.Contains(arg.Widget.SubClass))
            {
                arg.Handled = true;
                arg.Enabled = true;
            }
        }

        /// <summary>
        /// Override this to clear text
        /// </summary>
        public virtual void ClearText()
        {
        }

        /// <summary>
        /// Copy to clipboard
        /// </summary>
        /// <returns>true on success</returns>
        public virtual bool Copy()
        {
            AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.C);
            return true;
        }

        /// <summary>
        /// Cut to clipboard
        /// </summary>
        /// <returns>true on success</returns>
        public virtual bool Cut()
        {
            AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.X);
            return true;
        }

        /// <summary>
        /// Deletes 'count' characters at the specified offset
        /// </summary>
        /// <param name="offset">Where to delete</param>
        /// <param name="count">how many characters to delete</param>
        public virtual void Delete(int offset, int count)
        {
            try
            {
                AgentManager.Instance.TextChangedNotifications.Hold();

                Log.Debug("offset: " + offset + ", count: " + count);
                SetCaretPos(offset + count);

                int ii = offset + count;
                while (ii > offset)
                {
                    SendKeys.SendWait("{BACKSPACE}");
                    ii = GetCaretPos();
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

        /// <summary>
        /// Disposes resources
        /// </summary>
        public void Dispose()
        {
            dispose(true);

            // Prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Indicates whether abbreviations need to be expanded or not.  For
        /// instance, if the text control is for accepting a filename, we
        ///  would not want abbreviations to be expanded
        /// </summary>
        /// <returns>true if so</returns>
        public virtual bool ExpandAbbreviations()
        {
            return true;
        }

        /// <summary>
        /// Override this to get the caret position
        /// </summary>
        /// <returns>caret position, -1 on error</returns>
        public virtual int GetCaretPos()
        {
            return -1;
        }

        /// <summary>
        /// Returns the character at the caret position
        /// </summary>
        /// <param name="value">return the character</param>
        /// <returns>true on success</returns>
        public virtual bool GetCharAtCaret(out char value)
        {
            bool retVal = false;
            int caretPos = GetCaretPos();
            value = '\0';
            var str = GetText();

            if (!String.IsNullOrEmpty(str))
            {
                if (caretPos < str.Length && caretPos >= 0)
                {
                    value = str[caretPos];
                    retVal = true;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Returns the charater to the left of the caret
        /// </summary>
        /// <param name="value">returns the character</param>
        /// <returns>true on success</returns>
        public virtual bool GetCharLeftOfCaret(out char value)
        {
            try
            {
                value = '\0';

                int caretPos = GetCaretPos();

                if (caretPos > 0)
                {
                    String str = GetText();
                    ////Log.Debug("CaretPos: " + caretPos + "str: [" + str + "]");
                    if (!String.IsNullOrEmpty(str))
                    {
                        caretPos--;
                        ////Log.Debug("str: [" + str + "], After subtracting CaretPos: " + caretPos);
                        value = str[caretPos];
                        ////Log.Debug("Returned from value assignment");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
                value = '\0';
            }

            return false;
        }

        public virtual void GetParagraphAfterCaret(out String sentence)
        {
            sentence = String.Empty;
        }

        /// <summary>
        /// Returns the current paragraph starting from the beginning
        /// of the para upto where the caret is positioned
        /// </summary>
        /// <param name="paragraph">the para text</param>
        /// <returns>offset of the paragraph</returns>
        public virtual int GetParagraphAtCaret(out String paragraph)
        {
            return TextUtils.GetParagraphAtCaret(GetText(), GetCaretPos(), out paragraph);
        }

        /// <summary>
        /// Override this to get para before the caret
        /// </summary>
        /// <param name="paragraph">Returns the paragraph</param>
        public virtual void GetParagraphBeforeCaret(out String sentence)
        {
            sentence = String.Empty;
        }

        /// <summary>
        /// Returns 'count' character preceding the caret position
        /// </summary>
        /// <param name="count">How many characters to fetch</param>
        /// <param name="word">returns the string of characters</param>
        /// <returns>number of characters </returns>
        public virtual int GetPrecedingCharacters(int count, out String word)
        {
            return TextUtils.GetPrecedingCharacters(GetText(), GetCaretPos(), count, out word);
        }

        /// <summary>
        /// Works backward from the caret position looking for whitespaces
        /// and stops when it encounters the first non-whitespace character.
        /// Returns the offset of the first whitespace character behind the
        /// cursor and also the number of whitespace characters
        /// </summary>
        /// <param name="offset">offset of the first white space</param>
        /// <param name="count">number of whitespace characters</param>
        public virtual void GetPrecedingWhiteSpaces(out int offset, out int count)
        {
            TextUtils.GetPrecedingWhiteSpaces(GetText(), GetCaretPos(), out offset, out count);
        }

        /// <summary>
        /// Returns the previous words in the current sentence and the
        /// current word where the caret is positioned.  This function is
        /// used for word prediction.  For instance, if the sentence is
        /// "Hello how are you doing" and the caret is after the letter 'o'
        /// in "doing", the 'prefix' would be "Hello how are you" and 'word'
        /// would be "do".  Note that prefix and word should be empty if in
        /// the beginning of a sentence.
        /// </summary>
        /// <param name="prefix">Return the previous words</param>
        /// <param name="word">Return the current word</param>
        public virtual void GetPrefixAndWordAtCaret(out String prefix, out String word)
        {
            String text = GetText();
            int caretPos = GetCaretPos();
            ////Log.Debug("**** text: [" + text + "], caretPos: " + caretPos);
            Log.Debug("caretPos: " + caretPos);
            TextUtils.GetPrefixAndWordAtCaret(text, caretPos, out prefix, out word);
        }

        /// <summary>
        /// Returns the word previous to the one the caret is at.
        /// </summary>
        /// <param name="word">Word</param>
        /// <returns>Offset of the previous word</returns>
        public virtual int GetPreviousWordAtCaret(out String word)
        {
            return TextUtils.GetPreviousWord(GetText(), GetCaretPos(), out word);
        }

        /// <summary>
        /// Returns the offset to the word previous to the word
        /// at the caret
        /// </summary>
        /// <param name="offset">return the offset</param>
        /// <param name="count">length of the previous word</param>
        /// <returns>true on success</returns>
        public virtual bool GetPrevWordOffset(out int offset, out int count)
        {
            return TextUtils.GetPrevWordOffset(GetText(), GetCaretPos(), out offset, out count);
        }

        /// <summary>
        /// Override this to returns highlighted text in the text control
        /// </summary>
        /// <returns>text</returns>
        public virtual String GetSelectedText()
        {
            return String.Empty;
        }

        /// <summary>
        /// Returns the select mode
        /// </summary>
        /// <returns>true if on</returns>
        public bool GetSelectMode()
        {
            return _selectMode;
        }

        /// <summary>
        /// Override to return the sentence after the one the caret is at.
        /// </summary>
        /// <param name="sentence">Returns the sentence</param>
        public virtual void GetSentenceAfterCaret(out String sentence)
        {
            sentence = String.Empty;
        }

        /// <summary>
        /// Returns the current sentence at the caret - beginning
        /// from the beginning of the sentence up to the caret position
        /// </summary>
        /// <param name="sentence">Returns the sentence</param>
        public virtual void GetSentenceAtCaret(out String sentence)
        {
            TextUtils.GetSentenceAtCaret(GetText(), GetCaretPos(), out sentence);
        }

        /// <summary>
        /// Override to return the sentence before the one that the caret is at
        /// </summary>
        /// <param name="sentence">return the sentence</param>
        public virtual void GetSentenceBeforeCaret(out String sentence)
        {
            sentence = String.Empty;
        }

        /// <summary>
        /// Returns the text starting from 'startPos' upto
        /// the caret
        /// </summary>
        /// <param name="startPos">Where to start from</param>
        /// <returns>The text</returns>
        public virtual String GetStringToCaret(int startPos)
        {
            var text = GetText();
            String strToCaret = String.Empty;
            if (!String.IsNullOrEmpty(text))
            {
                int caretPos = GetCaretPos();

                if (caretPos > text.Length)
                {
                    caretPos = text.Length - 1;
                }

                if (startPos < caretPos)
                {
                    strToCaret = text.Substring(startPos, caretPos - startPos);
                }
            }

            return strToCaret;
        }

        /// <summary>
        /// Override this to return text from the text control window
        /// </summary>
        /// <returns>text</returns>
        public virtual String GetText()
        {
            return String.Empty;
        }

        /// <summary>
        /// Override this to get current word at caret.
        /// </summary>
        /// <param name="word">return the word</param>
        public virtual void GetWordAtCaret(out String word)
        {
            TextUtils.GetWordAtCaret(GetText(), GetCaretPos(), out word);
        }

        /// <summary>
        /// Navigate text indicated by GotoItem
        /// </summary>
        /// <param name="gotoItem">How to navigate</param>
        /// <returns>true on success</returns>
        public virtual bool Goto(GoToItem gotoItem)
        {
            switch (gotoItem)
            {
                case GoToItem.TopOfDocument:
                    handleGoToTop();
                    break;

                case GoToItem.EndOfDocument:
                    handleGoToBottom();
                    break;

                case GoToItem.NextLine:
                    handleGoToNextLine();
                    break;

                case GoToItem.PreviousLine:
                    handleGoToPrevLine();
                    break;

                case GoToItem.NextCharacter:
                    handleGoToNextChar();
                    break;

                case GoToItem.PreviousCharacter:
                    handleGoToPrevChar();
                    break;

                case GoToItem.Home:
                    handleGoToHome();
                    break;

                case GoToItem.End:
                    handleGoToEnd();
                    break;

                case GoToItem.PreviousPage:
                    handleGoToPrevPage();
                    break;

                case GoToItem.NextPage:
                    handleGoToNextPage();
                    break;

                case GoToItem.PreviousWord:
                    handleGoToPrevWord();
                    break;

                case GoToItem.NextWord:
                    handleGoToNextWord();
                    break;
            }

            return true;
        }

        /// <summary>
        /// Inserts the word at the specified offset
        /// </summary>
        /// <param name="offset">Where to insert the word</param>
        /// <param name="word">The word to insert</param>
        public virtual void Insert(int offset, String word)
        {
            try
            {
                AgentManager.Instance.TextChangedNotifications.Hold();

                int caretPos = GetCaretPos();
                if (caretPos != offset)
                {
                    SetCaretPos(offset);
                }
                else if (IsTextSelected())
                {
                    Keyboard.Send(Keys.Delete);
                }

                if (KeyStateTracker.IsCapsLockOn())
                {
                    word = word.ToLower();
                }

                sendWait(word);
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

        /// <summary>
        /// Has this object been disposed?
        /// </summary>
        /// <returns>true if disposed</returns>
        public bool IsDisposed()
        {
            return _disposed;
        }

        /// <summary>
        /// Indicates whether the previous word marks the beginning
        /// of the sentence
        /// </summary>
        /// <returns>true if so</returns>
        public virtual bool IsPreviousWordAtCaretTheFirstWord()
        {
            String word;
            int startPos = GetPreviousWordAtCaret(out word);

            return TextUtils.IsPrevSentenceTerminator(GetText(), startPos);
        }

        /// <summary>
        /// Override this to indicates whether any text is currently
        /// highlighted
        /// </summary>
        /// <returns>true if so</returns>
        public virtual bool IsTextSelected()
        {
            return false;
        }

        /// <summary>
        /// Invoked on a key down
        /// </summary>
        /// <param name="keyEventArgs">event args</param>
        public virtual void OnKeyDown(KeyEventArgs keyEventArgs)
        {
            if (AgentManager.Instance.CurrentEditingMode == EditingMode.TextEntry)
            {
                char ch = KeyToChar.GetChar(keyEventArgs.KeyCode);
                if (TextUtils.IsSentenceTerminator(ch))
                {
                    Log.Debug("NARAM");
                    OnSentenceTerminator();
                }
            }
        }

        /// <summary>
        /// Invoked on a key up
        /// </summary>
        /// <param name="keyEventArgs">event arg</param>
        public virtual void OnKeyUp(KeyEventArgs keyEventArgs)
        {
        }

        /// <summary>
        /// Invoked on a mouse down event
        /// </summary>
        /// <param name="mouseEventArgs">event args</param>
        public virtual void OnMouseDown(MouseEventArgs mouseEventArgs)
        {
        }

        /// <summary>
        /// Invoked when a sentence terminator is detected (.,?)
        /// </summary>
        public virtual void OnSentenceTerminator()
        {
        }

        /// <summary>
        /// Paste from clipboard
        /// </summary>
        /// <returns>true on success</returns>
        public virtual bool Paste()
        {
            AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.V);
            return true;
        }

        /// <summary>
        /// Override this to pause
        /// </summary>
        public virtual void Pause()
        {
        }

        /// <summary>
        /// Redo an undo (equivalent to Ctrl-Y)
        /// </summary>
        /// <returns>true on success</returns>
        public virtual bool Redo()
        {
            AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.Y);
            return true;
        }

        /// <summary>
        /// Replaces 'count' letters at 'offset' with the word
        /// </summary>
        /// <param name="offset">0 based starting offset</param>
        /// <param name="count">number of chars to replace</param>
        /// <param name="word">Word to insert at the 'offset'</param>
        public virtual void Replace(int offset, int count, String word)
        {
            Log.Debug("offset = " + offset + " count " + count + " word " + word);

            try
            {
                AgentManager.Instance.TextChangedNotifications.Hold();

                int caretPos = GetCaretPos();
                if (caretPos != offset + count)
                {
                    SetCaretPos(offset + count);
                }
                else if (IsTextSelected())
                {
                    Keyboard.Send(Keys.Delete);
                }

                Log.Debug("Sending back");

                SendKeys.SendWait("{BACKSPACE " + count + "}");

                if (KeyStateTracker.IsCapsLockOn())
                {
                    word = word.ToLower();
                }

                Log.Debug("Sending word " + word);

                sendWait(word);
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

        /// <summary>
        /// Override this to resume
        /// </summary>
        public virtual void Resume()
        {
        }

        /// <summary>
        /// Highlight all the text (equivalent to pressing
        /// Ctrl-A)
        /// </summary>
        /// <returns>true on success</returns>
        public virtual bool SelectAll()
        {
            AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.A);
            return true;
        }

        /// <summary>
        /// Override this to highlight the paragraph after the one the
        /// caret's at.
        /// </summary>
        /// <returns>true on success</returns>
        public virtual bool SelectNextParagraph()
        {
            return false;
        }

        /// <summary>
        /// Override this to highlight the sentence after the one the
        /// caret's at.
        /// </summary>
        /// <returns>true on success</returns>
        public virtual bool SelectNextSentence()
        {
            return false;
        }

        /// <summary>
        /// Override this to highlight the paragraph at the caret
        /// </summary>
        /// <returns>true on success</returns>
        public virtual bool SelectParagraphAtCaret()
        {
            return false;
        }

        /// <summary>
        /// Override this to highlight the paragraph before the one the
        /// caret's at.
        /// </summary>
        /// <returns>true on success</returns>
        public virtual bool SelectPreviousParagraph()
        {
            return false;
        }

        /// <summary>
        /// Override this to highlight the sentence before the one the
        /// caret's at.
        /// </summary>
        /// <returns>true on success</returns>
        public virtual bool SelectPreviousSentence()
        {
            return false;
        }

        /// <summary>
        /// Override this to highlight the sentence at the caret
        /// </summary>
        /// <returns>true on success</returns>
        public virtual bool SelectSentenceAtCaret()
        {
            return false;
        }

        /// <summary>
        /// Override this to set the caret position
        /// </summary>
        /// <param name="pos">caret position</param>
        /// <returns>true on success</returns>
        public virtual bool SetCaretPos(int pos)
        {
            return true;
        }

        /// <summary>
        /// Override this to set focus to the target text control
        /// </summary>
        /// <returns>true on success</returns>
        public virtual bool SetFocus()
        {
            return false;
        }

        /// <summary>
        /// If selectMode is true, Select is turned on.
        /// If selectMode is on, when the cursor moves,
        /// text is highlighted.  This is similar to holding
        /// the shift key down and using one of the arrow keys
        /// or page navigation.  Text continues to highlight as
        /// caret moves
        /// </summary>
        /// <param name="selectMode">true to turn on</param>
        public void SetSelectMode(bool mode)
        {
            _selectMode = mode;
            OnSelectModeChanged(mode);
        }

        /// <summary>
        /// Does it support a spell checker
        /// </summary>
        /// <returns>true if so</returns>
        public virtual bool SupportsSpellCheck()
        {
            return false;
        }

        /// <summary>
        /// Undo changes (Equivalent to Ctrl-Z)
        /// </summary>
        /// <returns>true on success</returns>
        public virtual bool Undo()
        {
            AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.Z);
            return true;
        }

        /// <summary>
        /// Override this to Unhighlight text in the text control
        /// </summary>
        public virtual void UnselectText()
        {
        }

        /// <summary>
        /// Check to see if the widget should be enabled or not based
        /// on contextual information
        /// </summary>
        /// <param name="supportedFeatures"></param>
        /// <param name="arg"></param>
        protected void checkWidgetEnabled(String[] supportedFeatures, CheckEnabledArgs arg)
        {
            if (supportedFeatures.Contains(arg.Widget.SubClass))
            {
                arg.Handled = true;
                arg.Enabled = true;
            }
        }

        /// <summary>
        /// Clears text in window
        /// </summary>
        /// <returns></returns>
        protected void ClearText(IntPtr handleText)
        {
            if (handleText != IntPtr.Zero)
            {
                const uint WM_SETTEXT = 0x000C;
                User32Interop.SendMessageText(handleText, WM_SETTEXT, IntPtr.Zero, String.Empty);
            }
        }

        /// <summary>
        /// Gets caret position.
        /// </summary>
        /// <returns>caret position, -1 on error</returns>
        protected int GetCaretPos(IntPtr handleText)
        {
            int caretPos = -1;

            if (handleText == IntPtr.Zero)
            {
                return caretPos;
            }

            try
            {
                int dummy = -1;
                User32Interop.SendMessageRefRef(handleText, User32Interop.EM_GETSEL, ref caretPos, ref dummy);
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }

            return caretPos;
        }

        /// <summary>
        /// Normalizes the string to make it compatible with the SendKeys.SendWait
        /// function. special characters must be enclosed in braces
        /// </summary>
        /// <param name="word">input string</param>
        /// <returns>normalized string</returns>
        protected String getNormalizedStringForSendWait(String word)
        {
            var sb = new StringBuilder();
            const string stopChars = "+^%~()[]{}";
            foreach (char ch in word)
            {
                if (stopChars.Contains(ch))
                {
                    sb.Append("{" + ch + "}");
                }
                else
                {
                    sb.Append(ch);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns highlighted text (if any)
        /// </summary>
        /// <returns>highlighted text</returns>
        protected String GetSelectedText(IntPtr handleText)
        {
            var selectedText = String.Empty;

            if (handleText == IntPtr.Zero)
            {
                return selectedText;
            }

            try
            {
                Int32 start = -1;
                Int32 end = -1;

                User32Interop.SendMessageRefRef(handleText, User32Interop.EM_GETSEL, ref start, ref end);
                var text = GetText();
                if (end > text.Length)
                {
                    end = text.Length - 1;
                }

                if (end > start)
                {
                    selectedText = text.Substring(start, end - start);
                }
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }

            return selectedText;
        }

        /// <summary>
        /// Gets the string of text from the text window
        /// </summary>
        /// <returns>text</returns>
        protected String GetText(IntPtr handleText)
        {
            String str = String.Empty;

            Log.Debug();

            if (handleText == IntPtr.Zero)
            {
                return str;
            }

            try
            {
                str = Windows.GetText(handleText);
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }

            return str;
        }

        /// <summary>
        /// Add the sentence at the caret to the word prediction.  This is
        /// typically used by word predictors that support learning to make
        /// word prediction better by learing the users style of writing
        /// </summary>
        protected void learn()
        {
            Log.Debug();

            if (!WordPredictionManager.Instance.ActiveWordPredictor.SupportsLearning)
            {
                return;
            }

            String str;
            GetSentenceAtCaret(out str);
            var trimmed = str.Trim();
            if (String.IsNullOrEmpty(trimmed))
            {
                Log.Debug("Nothing to learn. sentence is empty");
                return;
            }

            Log.Debug("Learn : [" + trimmed + "]");
            WordPredictionManager.Instance.ActiveWordPredictor.Learn(trimmed);
        }

        /// <summary>
        /// Override to handle dispose
        /// </summary>
        protected virtual void OnDispose()
        {
        }

        /// <summary>
        /// Invoked when the select mode is changed
        /// </summary>
        /// <param name="mode">select mode</param>
        protected virtual void OnSelectModeChanged(bool mode)
        {
        }

        /// <summary>
        /// Synchronously sends the string to the target text window
        /// </summary>
        /// <param name="word">string to send</param>
        protected void sendWait(String word)
        {
            SendKeys.SendWait(getNormalizedStringForSendWait(word));
        }

        protected bool SetCaretPos(IntPtr handleText, int pos)
        {
            if (handleText == IntPtr.Zero)
            {
                return false;
            }

            try
            {
                User32Interop.SendMessageIntInt(handleText, User32Interop.EM_SETSEL, pos, pos);
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }

            return true;
        }

        /// <summary>
        /// Sets focus to the text window
        /// </summary>
        /// <returns>true on success</returns>
        protected bool SetFocus(IntPtr handleText)
        {
            Int32 ret = 0;

            Log.Debug();

            if (handleText == IntPtr.Zero)
            {
                return false;
            }

            try
            {
                ret = User32Interop.SetFocus(handleText).ToInt32();
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }

            return ret != 0;
        }

        /// <summary>
        /// Call this function to raise events to indicated that something changed
        /// in the text window either due to editing or due to cursor movement. Event
        /// raised is synchronous
        /// </summary>
        /// <param name="textInterface">text agent</param>
        protected void triggerTextChanged(ITextControlAgent textInterface)
        {
            if (EvtTextChanged != null)
            {
                EvtTextChanged(this, new TextChangedEventArgs(textInterface));
            }
        }

        /// <summary>
        /// Call this function to raise events to indicated that something changed
        /// in the text window either due to editing or due to cursor movement. Event
        /// raised is asynchronous
        /// </summary>
        /// <param name="textInterface">text agent</param>
        protected void triggerTextChangedAsync(ITextControlAgent textInterface)
        {
            if (EvtTextChanged != null)
            {
                EvtTextChanged.BeginInvoke(textInterface, new TextChangedEventArgs(textInterface), null, null);
            }
        }

        /// <summary>
        /// Unselects any selection
        /// </summary>
        protected void UnselectText(IntPtr handleText)
        {
            if (handleText != IntPtr.Zero)
            {
                User32Interop.SendMessageIntInt(handleText, User32Interop.EM_SETSEL, -1, -1);
            }
        }

        /// <summary>
        /// Dispose resources
        /// </summary>
        /// <param name="disposing">is it disposing?</param>
        private void dispose(bool disposing)
        {
            if (!_disposed)
            {
                Log.Debug();

                if (disposing)
                {
                    OnDispose();
                }
                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        /// Navigates to the bottom of the text control
        /// </summary>
        private void handleGoToBottom()
        {
            if (_selectMode)
            {
                AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.LShiftKey, Keys.End);
            }
            else
            {
                if (KeyStateTracker.IsShiftOn())
                {
                    AgentManager.Instance.Keyboard.Send(Keys.LShiftKey, Keys.LControlKey, Keys.End);
                }
                else
                {
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.End);
                }

                KeyStateTracker.KeyTriggered(Keys.End);
            }
        }

        /// <summary>
        /// Navigates to the end of the current line in the
        /// text control
        /// </summary>
        private void handleGoToEnd()
        {
            if (_selectMode)
            {
                if (KeyStateTracker.IsCtrlOn())
                {
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.LShiftKey, Keys.End);
                }
                else
                {
                    AgentManager.Instance.Keyboard.Send(Keys.LShiftKey, Keys.End);
                }
            }
            else
            {
                AgentManager.Instance.Keyboard.Send(KeyStateTracker.GetExtendedKeys(), Keys.End);
                KeyStateTracker.KeyTriggered(Keys.End);
            }
        }

        /// <summary>
        /// Navigates to the beginning of the current line in the
        /// text control
        /// </summary>
        private void handleGoToHome()
        {
            if (_selectMode)
            {
                if (KeyStateTracker.IsCtrlOn())
                {
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.LShiftKey, Keys.Home);
                }
                else
                {
                    AgentManager.Instance.Keyboard.Send(Keys.LShiftKey, Keys.Home);
                }
            }
            else
            {
                AgentManager.Instance.Keyboard.Send(KeyStateTracker.GetExtendedKeys(), Keys.Home);
                KeyStateTracker.KeyTriggered(Keys.Home);
            }
        }

        /// <summary>
        /// Navigates to the next char in the text control
        /// </summary>
        private void handleGoToNextChar()
        {
            if (_selectMode)
            {
                AgentManager.Instance.Keyboard.Send(Keys.LShiftKey, Keys.Right);
            }
            else
            {
                AgentManager.Instance.Keyboard.Send(KeyStateTracker.GetExtendedKeys(), Keys.Right);
                KeyStateTracker.KeyTriggered(Keys.Right);
            }
        }

        /// <summary>
        /// Navigates to the next line in the text control
        /// </summary>
        private void handleGoToNextLine()
        {
            if (_selectMode)
            {
                AgentManager.Instance.Keyboard.Send(Keys.LShiftKey, Keys.Down);
            }
            else
            {
                AgentManager.Instance.Keyboard.Send(KeyStateTracker.GetExtendedKeys(), Keys.Down);
                KeyStateTracker.KeyTriggered(Keys.Down);
            }
        }

        /// <summary>
        /// Navigates to the next page in the text control
        /// </summary>
        private void handleGoToNextPage()
        {
            if (_selectMode)
            {
                if (KeyStateTracker.IsCtrlOn())
                {
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.LShiftKey, Keys.PageDown);
                }
                else
                {
                    AgentManager.Instance.Keyboard.Send(Keys.LShiftKey, Keys.PageDown);
                }
            }
            else
            {
                AgentManager.Instance.Keyboard.Send(KeyStateTracker.GetExtendedKeys(), Keys.PageDown);
                KeyStateTracker.KeyTriggered(Keys.PageDown);
            }
        }

        /// <summary>
        /// Navigates to the next word in the text control
        /// </summary>
        private void handleGoToNextWord()
        {
            if (_selectMode)
            {
                AgentManager.Instance.Keyboard.Send(Keys.LShiftKey, Keys.LControlKey, Keys.Right);
            }
            else
            {
                if (KeyStateTracker.IsShiftOn())
                {
                    AgentManager.Instance.Keyboard.Send(Keys.LShiftKey, Keys.LControlKey, Keys.Right);
                }
                else
                {
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.Right);
                }

                KeyStateTracker.KeyTriggered(Keys.Right);
            }
        }

        /// <summary>
        /// Navigates to the previous char in the text control
        /// </summary>
        private void handleGoToPrevChar()
        {
            if (_selectMode)
            {
                AgentManager.Instance.Keyboard.Send(Keys.LShiftKey, Keys.Left);
            }
            else
            {
                AgentManager.Instance.Keyboard.Send(KeyStateTracker.GetExtendedKeys(), Keys.Left);
                KeyStateTracker.KeyTriggered(Keys.Left);
            }
        }

        /// <summary>
        /// Navigates to the previous line in the text control
        /// </summary>
        private void handleGoToPrevLine()
        {
            if (_selectMode)
            {
                AgentManager.Instance.Keyboard.Send(Keys.LShiftKey, Keys.Up);
            }
            else
            {
                AgentManager.Instance.Keyboard.Send(KeyStateTracker.GetExtendedKeys(), Keys.Up);
                KeyStateTracker.KeyTriggered(Keys.Up);
            }
        }

        /// <summary>
        /// Navigates to the previous page in the text control
        /// </summary>
        private void handleGoToPrevPage()
        {
            if (_selectMode)
            {
                if (KeyStateTracker.IsCtrlOn())
                {
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.LShiftKey, Keys.PageUp);
                }
                else
                {
                    AgentManager.Instance.Keyboard.Send(Keys.LShiftKey, Keys.PageUp);
                }
            }
            else
            {
                AgentManager.Instance.Keyboard.Send(KeyStateTracker.GetExtendedKeys(), Keys.PageUp);
                KeyStateTracker.KeyTriggered(Keys.PageUp);
            }
        }

        /// <summary>
        /// Navigates to the previous word in the text control
        /// </summary>
        private void handleGoToPrevWord()
        {
            if (_selectMode)
            {
                AgentManager.Instance.Keyboard.Send(Keys.LShiftKey, Keys.LControlKey, Keys.Left);
            }
            else
            {
                if (KeyStateTracker.IsShiftOn())
                {
                    AgentManager.Instance.Keyboard.Send(Keys.LShiftKey, Keys.LControlKey, Keys.Left);
                }
                else
                {
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.Left);
                }

                KeyStateTracker.KeyTriggered(Keys.Left);
            }
        }

        /// <summary>
        /// Navigates to the top of the text control
        /// </summary>
        private void handleGoToTop()
        {
            if (_selectMode)
            {
                AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.LShiftKey, Keys.Home);
            }
            else
            {
                if (KeyStateTracker.IsShiftOn())
                {
                    AgentManager.Instance.Keyboard.Send(Keys.LShiftKey, Keys.LControlKey, Keys.Home);
                }
                else
                {
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.Home);
                }

                KeyStateTracker.KeyTriggered(Keys.Home);
            }
        }
    }
}