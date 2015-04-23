////////////////////////////////////////////////////////////////////////////
// <copyright file="TextController.cs" company="Intel Corporation">
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
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ACAT.Lib.Core.AbbreviationsManagement;
using ACAT.Lib.Core.AgentManagement;
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

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Implements the ITextController interface.  Contains helper
    /// functions that is used by ACAT for text manipulation in the
    /// application window
    /// </summary>
    public class TextController : ITextController
    {
        /// <summary>
        /// The caret position after a partially entered word was
        /// autocompleted
        /// </summary>
        private static int _autoCompleteCaretPos = -1;

        /// <summary>
        /// Text of the partial word that was autocompleted
        /// </summary>
        private static String _autoCompletePartialWord = String.Empty;

        /// <summary>
        /// Text offset of the partial word that is to be autocompleted
        /// </summary>
        private static int _autocompleteStartOffset = -1;

        /// <summary>
        /// Caret position before a partially entered word was
        /// autocompleted
        /// </summary>
        private static int _beforeAutoCompleteCaretPos = -1;

        /// <summary>
        /// Last text editing action
        /// </summary>
        private static LastAction _lastAction = LastAction.Unknown;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        internal TextController()
        {
        }

        /// <summary>
        /// Tracks the current state of text entry
        /// </summary>
        private enum LastAction
        {
            Unknown,
            AlphaNumeric,
            Punctuation,
            AutoCompleteWord
        }

        /// <summary>
        /// Auto-completes a partially entered with with the 'wordselected'. The
        /// 'wordSelected' is typically one that the word prediction engine suggests
        /// and the one that the user selects
        /// </summary>
        /// <param name="wordSelected">the autocomplete word.</param>
        public void AutoCompleteWord(String wordSelected)
        {
            bool isCapitalizedWordToReplace = false;

            try
            {
                using (AgentContext context = Context.AppAgentMgr.ActiveContext())
                {
                    Context.AppAgentMgr.TextChangedNotifications.Hold();

                    int caretPos = context.TextAgent().GetCaretPos();

                    _beforeAutoCompleteCaretPos = caretPos;
                    _autocompleteStartOffset = -1;

                    int offset;
                    int count;
                    context.TextAgent().GetPrevWordOffset(out offset, out count);
                    Log.Debug("PrevWord offset: " + offset + ", count: " + count);

                    // check if we are just completing the current word or inserting a new word
                    int insertOrReplaceOffset;
                    String wordToReplace;
                    bool checkInsert = context.TextAgent().CheckInsertOrReplaceWord(out insertOrReplaceOffset, out wordToReplace);
                    Log.Debug("checkInsert: " + checkInsert + ", insertorreplaceoffset: " + insertOrReplaceOffset +
                              ", caret: " + caretPos + ", caretPos-delprev: " + (caretPos - count));

                    _autoCompletePartialWord = wordToReplace;

                    int wordToReplaceLength = wordToReplace.Length;
                    if (wordToReplaceLength > 0)
                    {
                        isCapitalizedWordToReplace = Char.IsUpper(wordToReplace[0]);
                    }

                    Log.Debug("checkInsert: " + checkInsert + ". inserRepOff: " + insertOrReplaceOffset +
                              ". wordTORep: " + wordToReplace);

                    if (KeyStateTracker.IsStickyShiftOn())
                    {
                        wordSelected = wordSelected.ToUpper();
                    }
                    else if (KeyStateTracker.IsShiftOn())
                    {
                        wordSelected = capitalizeWord(wordSelected);
                        KeyStateTracker.KeyTriggered(wordSelected[0]);
                    }

                    if (checkInsert)
                    {
                        Log.Debug("Inserting [" + wordSelected + "] at offset " + insertOrReplaceOffset);
                        context.TextAgent().Insert(insertOrReplaceOffset, wordSelected);
                    }
                    else
                    {
                        if (count > 0)
                        {
                            wordToReplaceLength = count;
                        }

                        if (wordToReplaceLength > 0 && isCapitalizedWordToReplace &&
                            Char.ToUpper(wordToReplace[0]) == Char.ToUpper(wordSelected[0]))
                        {
                            wordSelected = capitalizeWord(wordSelected);
                        }

                        Log.Debug("Replacing word at offset " + insertOrReplaceOffset + ". Length: " +
                                  wordToReplaceLength + ". with [" + wordSelected + "]");

                        context.TextAgent().Replace(insertOrReplaceOffset, wordToReplaceLength, wordSelected);
                    }

                    _autocompleteStartOffset = insertOrReplaceOffset;

                    _lastAction = LastAction.AutoCompleteWord;

                    postAutoCompleteWord();

                    _autoCompleteCaretPos = context.TextAgent().GetCaretPos();
                    Log.Debug("_autocompleteCursorPos is " + _autoCompleteCaretPos);
                }
            }
            catch (InvalidAgentContextException iace)
            {
                Log.Debug("Agent Context is invalid " + iace);
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }
            finally
            {
                uint threadId = GetCurrentThreadId();
                Log.Debug("Calling TextChangedNotifications.Release AFTER Autocompeting word " + threadId + ", caretPos: " + Context.AppAgentMgr.ActiveContext().TextAgent().GetCaretPos());
                Context.AppAgentMgr.TextChangedNotifications.Release();
                Log.Debug("Returned from TextChangedNotifications.Release AFTER Autocompeting word " + threadId);
            }
        }

        /// <summary>
        /// Call this function to check if an abbreviation has been detected
        /// in the input text stream.  It checks the word at the caret position and
        /// looks up the mapping table to detect an abbreviation.  If detected, it
        /// raises an event to replace the abbreviation and the app can do it.
        /// </summary>
        /// <param name="handled">was the abbr handled</param>
        /// <returns>The abbreviation object</returns>
        public Abbreviation CheckAndReplaceAbbreviation(ref bool handled)
        {
            Abbreviation abbr = null;
            handled = false;

            try
            {
                using (AgentContext context = Context.AppAgentMgr.ActiveContext())
                {
                    char charAtCaret;
                    if (!context.TextAgent().GetCharLeftOfCaret(out charAtCaret))
                    {
                        return null;
                    }

                    if (CoreGlobals.AppPreferences.ExpandAbbreviationsOnSeparator &&
                        !TextUtils.IsTerminatorOrWhiteSpace(charAtCaret))
                    {
                        Log.Debug("no sentence terminator or white space here.  returning");
                        return null;
                    }

                    String word;
                    int startPos = context.TextAgent().GetPreviousWordAtCaret(out word);
                    Log.Debug("Prev word: " + word);
                    if (String.IsNullOrEmpty(word))
                    {
                        return null;
                    }

                    // if there is a preceeding sentence terminator, we have to capitalize the word
                    bool isFirstWord = context.TextAgent().IsPreviousWordAtCaretTheFirstWord();

                    abbr = Context.AppAbbreviations.Lookup(word);

                    // do we detect something?
                    if (abbr != null)
                    {
                        String replacement = abbr.Expansion;

                        String stringToCaret = context.TextAgent().GetStringToCaret(startPos);

                        Log.Debug("String to caret from startPos " + startPos + ": [" + stringToCaret + "]");
                        replacement = stringToCaret.Replace(word, replacement);
                        Log.Debug("After replacement, replacement : [" + replacement + "]");

                        if (isFirstWord)
                        {
                            String capitalized = TextUtils.Capitalize(replacement);
                            if (capitalized != null)
                            {
                                replacement = capitalized;
                            }
                        }

                        int wordLength = word.Length +
                                         (CoreGlobals.AppPreferences.ExpandAbbreviationsOnSeparator ? 1 : 0);
                        String replaceWith = (abbr.Mode == Abbreviation.AbbreviationMode.Write)
                            ? replacement
                            : String.Empty;

                        context.TextAgent().Replace(startPos, wordLength, replaceWith);

                        if (abbr.Mode == Abbreviation.AbbreviationMode.Write)
                        {
                            handled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }

            return abbr;
        }

        /// <summary>
        /// Deletes the previous char in the application text window
        /// </summary>
        public void DeletePreviousChar()
        {
            Context.AppAgentMgr.Keyboard.Send(KeyStateTracker.GetExtendedKeys(), '\b');
        }

        /// <summary>
        /// Deletes the previous word in the application text window. If the
        /// last editing action was autocompleting a word, this function restores
        /// the previously partially entered word. In a sense, this is undoing the
        /// auto-completion.  If the previous action was not auto-completion then
        /// it simply deletes the previous word
        /// </summary>
        public void DeletePreviousWord()
        {
            try
            {
                using (AgentContext context = Context.AppAgentMgr.ActiveContext())
                {
                    int caretPos = context.TextAgent().GetCaretPos();

                    if (_lastAction == LastAction.AutoCompleteWord &&
                        _autoCompleteCaretPos >= 0 &&
                        caretPos == _autoCompleteCaretPos &&
                        _beforeAutoCompleteCaretPos >= 0)
                    {
                        Log.Debug("Delete: _autoCompleteCaretPos: " + _autoCompleteCaretPos +
                                  ",  _beforeAutoCOmpleteCaretPos: " + _beforeAutoCompleteCaretPos + ", count: " +
                                  (_autoCompleteCaretPos - _beforeAutoCompleteCaretPos));

                        int prefixLen = _autoCompletePartialWord.Length;
                        Log.Debug("prefixLen: " + prefixLen);
                        if (prefixLen > 0)
                        {
                            int start = _autocompleteStartOffset;
                            Log.Debug("start: " + start);
                            start = Math.Max(0, start);

                            Log.Debug("Deleting from " + start + ", numchars: " + (_autoCompleteCaretPos - start));
                            context.TextAgent().Delete(start, _autoCompleteCaretPos - start);
                            Log.Debug("Inserting at " + start + ", string: " + _autoCompletePartialWord);
                            context.TextAgent().Insert(start, _autoCompletePartialWord);
                        }
                        else
                        {
                            Log.Debug("Delete from " + _beforeAutoCompleteCaretPos);
                            context.TextAgent().Delete(_beforeAutoCompleteCaretPos, _autoCompleteCaretPos - _beforeAutoCompleteCaretPos);
                        }
                    }
                    else
                    {
                        int offset;
                        int count;
                        context.TextAgent().GetPrevWordOffset(out offset, out count);
                        context.TextAgent().Delete(offset, count);
                    }

                    _autoCompleteCaretPos = -1;
                    _beforeAutoCompleteCaretPos = -1;
                    _autocompleteStartOffset = -1;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        /// <summary>
        /// Handles actuation of a button representing an alphanumeric character
        /// </summary>
        public void HandleAlphaNumericChar(ArrayList modifiers, char inputChar)
        {
            Context.AppAgentMgr.Keyboard.Send((modifiers != null) ? modifiers.Cast<Keys>().ToList() : KeyStateTracker.GetExtendedKeys(), inputChar);

            KeyStateTracker.KeyTriggered(inputChar);

            _lastAction = LastAction.AlphaNumeric;
        }

        /// <summary>
        /// Handles actuation of a button representation a punctuation
        /// </summary>
        /// <param name="modifiers">Any modfiers (shift, alt, ctrl)</param>
        /// <param name="punctuation">the punctuation</param>
        /// <returns>true on success</returns>
        public bool HandlePunctuation(ArrayList modifiers, char punctuation)
        {
            Log.Debug();

            try
            {
                if (!isPunctuation(punctuation))
                {
                    return false;
                }

                // turn off extended keys such as alt, ctrl.  This causes problems when space
                // is inserted after puncuation and alt+space causes the system menu to show up.

                KeyStateTracker.ClearAlt();
                KeyStateTracker.ClearCtrl();
                KeyStateTracker.ClearFunc();

                using (AgentContext context = Context.AppAgentMgr.ActiveContext())
                {
                    int offset;
                    int count;

                    // delete any spaces before the punctuation
                    context.TextAgent().GetPrecedingWhiteSpaces(out offset, out count);
                    Log.Debug("Preceding whitespace count: " + count);
                    if (count > 0)
                    {
                        Log.Debug("Deleting whitespaces from offset " + offset);
                        context.TextAgent().Delete(offset, count);
                    }

                    Log.Debug("Sending punctuation");
                    Context.AppAgentMgr.Keyboard.Send((modifiers != null) ? modifiers.Cast<Keys>().ToList() : KeyStateTracker.GetExtendedKeys(), punctuation);

                    // if this is a sentence terminator, add spaces
                    // after the punctuation.

                    if ("})]".LastIndexOf(punctuation) >= 0)
                    {
                        Context.AppAgentMgr.Keyboard.Send(KeyStateTracker.GetExtendedKeys(), ' ');
                    }
                    else if (TextUtils.IsTerminator(punctuation))
                    {
                        for (int ii = 0; ii < CoreGlobals.AppPreferences.SpacesAfterPunctuation; ii++)
                        {
                            Context.AppAgentMgr.Keyboard.Send(KeyStateTracker.GetExtendedKeys(), ' ');
                        }
                    }

                    // this is important.  The ACAT talk window caretpos doesn't update until this we exit
                    // this function.  DoEvents give a chance to the talk window to update its caret position.
                    //Application.DoEvents();

                    _autoCompleteCaretPos = context.TextAgent().GetCaretPos();
                    Log.Debug("after actuating, caretpos is " + _autoCompleteCaretPos);

                    KeyStateTracker.KeyTriggered(punctuation);

                    _lastAction = LastAction.Punctuation;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }

            return true;
        }

        /// <summary>
        /// Handles acutation of a key such as a Function key.
        /// The 'value' parameter must be one of the Keys enum
        /// values.
        /// </summary>
        /// <param name="modifiers"></param>
        /// <param name="value">the key</param>
        public void HandleVirtualKey(ArrayList modifiers, String value)
        {
            _lastAction = LastAction.Unknown;

            try
            {
                Keys virtualKey = MapVirtualKey(value);
                if (virtualKey != Keys.None)
                {
                    Context.AppAgentMgr.Keyboard.Send((modifiers != null) ? modifiers.Cast<Keys>().ToList() : KeyStateTracker.GetExtendedKeys(), virtualKey);
                    //Context.AppAgentMgr.Keyboard.Send(KeyStateTracker.GetExtendedKeys(), virtualKey);
                    KeyStateTracker.KeyTriggered(virtualKey);
                }
            }
            catch
            {
                Log.Error("Invalid virtual key " + value.Substring(1));
            }
        }

        /// <summary>
        /// Translates a string representation of a key into Keys enum
        /// </summary>
        /// <param name="value">String representation</param>
        /// <returns>Keys enum version</returns>
        public Keys MapVirtualKey(String value)
        {
            Keys retVal;
            try
            {
                retVal = (Keys)Enum.Parse(typeof(Keys), value.Substring(1), true);
            }
            catch
            {
                Log.Error("Invalid virtual key " + value.Substring(1));
                return Keys.None;
            }

            return retVal;
        }

        /// <summary>
        /// Call this in OnClosing override of the scanner form
        /// </summary>
        public void OnClosing()
        {
            unsubscribeEvents();
        }

        /// <summary>
        /// Call this in the load handler of the scanner form
        /// </summary>
        public void OnLoad()
        {
            subscribeToEvents();
        }

        /// <summary>
        /// Invoked when the text in the application window is cleared.
        /// For instance, a talk window is cleared.
        /// </summary>
        public void OnTextCleared()
        {
            _lastAction = LastAction.Unknown;
        }

        /// <summary>
        /// Auto-corrects the previous word using the active spell
        /// checker. Also handles capitalization of the first word.
        /// Call this function after a word is autocompleted or if a
        /// space or a punctuation is inserted into the text.
        /// </summary>
        public void SpellCheck()
        {
            String word;

            char charAtCaret;

            if (!AgentManager.Instance.TextControlAgent.GetCharLeftOfCaret(out charAtCaret))
            {
                return;
            }

            Log.Debug("char left of  caret: [" + charAtCaret + "]");
            if (!TextUtils.IsTerminatorOrWhiteSpace(charAtCaret))
            {
                Log.Debug("no sentence terminator or white space here.  returning");
                return;
            }

            int startPos = AgentManager.Instance.TextControlAgent.GetPreviousWordAtCaret(out word);
            Log.Debug("Prev word: [" + word + "]");
            if (String.IsNullOrEmpty(word))
            {
                return;
            }

            bool isFirstWord = AgentManager.Instance.TextControlAgent.IsPreviousWordAtCaretTheFirstWord();

            String replacement = Context.AppSpellCheckManager.ActiveSpellChecker.Lookup(word);
            if (String.IsNullOrEmpty(replacement) && isFirstWord)
            {
                replacement = word;
            }

            if (isFirstWord)
            {
                replacement = capitalizeWord(replacement);
            }

            if (!String.IsNullOrEmpty(replacement) && String.Compare(word, replacement) != 0)
            {
                replaceMisspeltWord(word, replacement, startPos, isFirstWord);
            }
        }

        /// <summary>
        /// Deletes the previous word.  Depends on the last editing action.
        /// If a punctuation was the last thing that was entered, it deletes
        /// the punctuations.  If word auto-completion was the last action,
        /// it undoes the auto-completion and restores the previously entered
        /// partial word.  Otherwise, it just deletes the previous word.
        /// </summary>
        public void UndoLastAction()
        {
            try
            {
                Log.Debug("LastAction: " + _lastAction + ", currentEditingMode: " +
                          Context.AppAgentMgr.CurrentEditingMode);

                using (var context = Context.AppAgentMgr.ActiveContext())
                {
                    if (context.TextAgent().IsTextSelected())
                    {
                        context.TextAgent().Keyboard.Send(Keys.Back);
                        _lastAction = LastAction.Unknown;
                        return;
                    }
                }

                switch (_lastAction)
                {
                    case LastAction.Punctuation:
                        deletePreviousPunctuation();
                        break;

                    case LastAction.AutoCompleteWord:
                        DeletePreviousWord();
                        break;

                    default:
                        DeletePreviousWord();
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }

            _lastAction = LastAction.Unknown;
        }

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

        /// <summary>
        /// Handler for the event that is raised when the focus in the
        /// application window changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppAgentMgr_EvtFocusChanged(object sender, FocusChangedEventArgs e)
        {
            _lastAction = LastAction.Unknown;
            _autoCompleteCaretPos = -1;
            _beforeAutoCompleteCaretPos = -1;
            _autocompleteStartOffset = -1;
            _autoCompletePartialWord = String.Empty;
        }

        /// <summary>
        /// Capitalizes the specified word
        /// </summary>
        /// <param name="word">word to capitalize</param>
        /// <returns>Captilized word</returns>
        private String capitalizeWord(String word)
        {
            return Char.ToUpper(word[0]) + ((word.Length > 1) ? word.Substring(1) : String.Empty);
        }

        /// <summary>
        /// Deletes a punctuation if it is the last previous non-whitespace character
        /// </summary>
        private void deletePreviousPunctuation()
        {
            try
            {
                using (AgentContext context = Context.AppAgentMgr.ActiveContext())
                {
                    if (Context.AppAgentMgr.CurrentEditingMode == EditingMode.TextEntry)
                    {
                        String word;
                        int numChars = CoreGlobals.AppPreferences.SpacesAfterPunctuation + 1;
                        context.TextAgent().GetPrecedingCharacters(numChars, out word);
                        Log.Debug("prev " + numChars + " chars are : [" + word + "]");
                        if (word.Length == numChars && isPunctuation(word[0]))
                        {
                            Context.AppAgentMgr.Keyboard.Send(Keys.Back, numChars);
                            Context.AppAgentMgr.Keyboard.Send(Keys.Space);
                        }
                        else
                        {
                            DeletePreviousWord();
                        }
                    }
                    else
                    {
                        DeletePreviousWord();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        /// <summary>
        /// Check if the character is a punctuation
        /// </summary>
        /// <param name="c">character</param>
        /// <returns>true if it is</returns>
        private bool isPunctuation(char c)
        {
            const string list = ".?!,:;@})]";
            return list.LastIndexOf(c) >= 0;
        }

        /// <summary>
        /// Handles operations after a word has been autocompleted
        /// </summary>
        private void postAutoCompleteWord()
        {
            try
            {
                using (AgentContext context = Context.AppAgentMgr.ActiveContext())
                {
                    int caretPos = context.TextAgent().GetCaretPos();
                    if (caretPos > 0)
                    {
                        char charAtCaret;
                        bool retVal = context.TextAgent().GetCharAtCaret(out charAtCaret);

                        Log.Debug("charAtCaret is " + Convert.ToInt32(charAtCaret));
                        if (!retVal || charAtCaret == 0x0D || !Char.IsWhiteSpace(charAtCaret))
                        {
                            Log.Debug("Sending space suffix... caretpos is " + context.TextAgent().GetCaretPos());
                            Context.AppAgentMgr.Keyboard.Send(KeyStateTracker.GetExtendedKeys(), ' ');
                            Log.Debug("Done sending space suffix caretPos is " + context.TextAgent().GetCaretPos());
                            KeyStateTracker.KeyTriggered(' ');
                        }
                        else if (Char.IsWhiteSpace(charAtCaret))
                        {
                            SendKeys.SendWait("{DELETE}");
                            SendKeys.SendWait(charAtCaret.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        /// <summary>
        /// Replaces a misspelt word starting at startPos with the correct
        /// spelling and notifies the app of what needs to be replaced and
        /// where.
        /// </summary>
        /// <param name="word">word to replace</param>
        /// <param name="replacement">the replacemenet word</param>
        /// <param name="startPos">starting position of the word</param>
        /// <param name="isFirstWord">is this the first word of the sentence</param>
        private void replaceMisspeltWord(String word, String replacement, int startPos, bool isFirstWord)
        {
            try
            {
                using (AgentContext context = Context.AppAgentMgr.ActiveContext())
                {
                    String textToCaret = context.TextAgent().GetStringToCaret(startPos);
                    Log.Debug("textToCaret : [" + textToCaret + "]");
                    replacement = textToCaret.Replace(word, replacement);
                    Log.Debug("After replacement, replacement : [" + replacement + "]");
                    if (isFirstWord)
                    {
                        String cap = TextUtils.Capitalize(replacement);
                        if (cap != null)
                        {
                            replacement = cap;
                        }
                    }

                    Log.Debug("Replace word at " + startPos + ". Length: " + replacement.Length + ". replacement: " +
                              replacement);

                    context.TextAgent().Replace(startPos, word.Length + 1, replacement);
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        /// <summary>
        /// Subscribes to events
        /// </summary>
        private void subscribeToEvents()
        {
            Context.AppAgentMgr.EvtFocusChanged += AppAgentMgr_EvtFocusChanged;
        }

        /// <summary>
        /// Unsubscribes from events
        /// </summary>
        private void unsubscribeEvents()
        {
            Context.AppAgentMgr.EvtFocusChanged -= AppAgentMgr_EvtFocusChanged;
        }
    }
}