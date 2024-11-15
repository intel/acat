////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.AbbreviationsManagement;
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections;
using System.Linq;
using System.Windows.Forms;

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
        public enum LastAction
        {
            Unknown,
            AlphaNumeric,
            Punctuation,
            AutoCompleteWord
        }

        /// <summary>
        /// Gets the last editing change
        /// </summary>
        public static LastAction LastEditChange
        {
            get { return _lastAction; }
        }

        /// <summary>
        /// Checks if the specified key (such as alt, ctrl, shift)
        /// is down
        /// </summary>
        /// <param name="vKey">key to check</param>
        /// <returns>true if it is</returns>
        public static bool IsKeyDown(Keys vKey)
        {
            return 0 != (User32Interop.GetAsyncKeyState((int)vKey) & 0x8000);
        }

        /// <summary>
        /// Auto-completes a partially entered with with the 'wordselected'. The
        /// 'wordSelected' is typically one that the word prediction engine suggests
        /// and the one that the user selects
        /// </summary>
        /// <param name="wordSelected">the autocomplete word.</param>
        public void AutoCompleteWord(String wordSelected)
        {
            if (Context.AppAgentMgr.TextChangedNotifications.OnHold())
            {
                return;
            }
            bool isCapitalizedWordToReplace = false;

            Log.Debug("Entered AutoCompleteWord");
            try
            {
                using (AgentContext context = Context.AppAgentMgr.ActiveContext())
                {
                    Context.AppAgentMgr.TextChangedNotifications.Hold();

                    CoreGlobals.Stopwatch4.Reset();
                    CoreGlobals.Stopwatch4.Start();

                    int caretPos = context.TextAgent().GetCaretPos();

                    _beforeAutoCompleteCaretPos = caretPos;
                    _autocompleteStartOffset = -1;

                    context.TextAgent().GetPrevWordOffsetAutoComplete(out int offset, out int count);
                    Log.Debug("PrevWord offset: " + offset + ", count: " + count);

                    CoreGlobals.Stopwatch4.Stop();
                    Log.Debug("AutoComplete TimeElapsed 1: " + CoreGlobals.Stopwatch4.ElapsedMilliseconds);

                    CoreGlobals.Stopwatch4.Reset();
                    CoreGlobals.Stopwatch4.Start();

                    // check if we are just completing the current word or inserting a new word
                    bool checkInsert = context.TextAgent().CheckInsertOrReplaceWord(out int insertOrReplaceOffset, out string wordToReplace);
                    Log.Debug("checkInsert: " + checkInsert + ", insertorreplaceoffset: " + insertOrReplaceOffset +
                              ", caret: " + caretPos + ", caretPos-delprev: " + (caretPos - count));
                    Log.Debug("wordtoReplace: " + wordToReplace);

                    _autoCompletePartialWord = wordToReplace;

                    int wordToReplaceLength = wordToReplace.Length;
                    if (wordToReplaceLength > 0)
                    {
                        isCapitalizedWordToReplace = Char.IsUpper(wordToReplace[0]);
                    }

                    CoreGlobals.Stopwatch4.Stop();
                    Log.Debug("AutoComplete TimeElapsed 2: " + CoreGlobals.Stopwatch4.ElapsedMilliseconds);
                    CoreGlobals.Stopwatch4.Reset();
                    CoreGlobals.Stopwatch4.Start();

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

                        CoreGlobals.Stopwatch5.Reset();
                        CoreGlobals.Stopwatch5.Start();

                        context.TextAgent().Insert(insertOrReplaceOffset, wordSelected);

                        CoreGlobals.Stopwatch5.Stop();
                        Log.Debug("AutoComplete Insert operation TimeElapsed: " + CoreGlobals.Stopwatch5.ElapsedMilliseconds);
                    }
                    else
                    {
                        /*
                        if (count > 0)
                        {
                            wordToReplaceLength = count;
                        }
                        */

                        if (wordToReplaceLength > 0 && isCapitalizedWordToReplace &&
                            Char.ToUpper(wordToReplace[0]) == Char.ToUpper(wordSelected[0]))
                        {
                            wordSelected = capitalizeWord(wordSelected);
                        }

                        Log.Debug("Replacing word at offset " + insertOrReplaceOffset + ". Length: " +
                                  wordToReplaceLength + ". with [" + wordSelected + "]");

                        context.TextAgent().Replace(insertOrReplaceOffset, wordToReplaceLength, wordSelected);
                    }

                    CoreGlobals.Stopwatch4.Stop();
                    Log.Debug("AutoComplete TimeElapsed 3: " + CoreGlobals.Stopwatch4.ElapsedMilliseconds);
                    CoreGlobals.Stopwatch4.Reset();
                    CoreGlobals.Stopwatch4.Start();

                    _autocompleteStartOffset = insertOrReplaceOffset;

                    _lastAction = LastAction.AutoCompleteWord;

                    postAutoCompleteWord();

                    _autoCompleteCaretPos = context.TextAgent().GetCaretPos();
                    Log.Debug("_autocompleteCursorPos is " + _autoCompleteCaretPos);

                    CoreGlobals.Stopwatch4.Stop();
                    Log.Debug("AutoComplete TimeElapsed 4: " + CoreGlobals.Stopwatch4.ElapsedMilliseconds);
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
                uint threadId = Kernel32Interop.GetCurrentThreadId();
                Log.Debug("Calling TextChangedNotifications.Release AFTER Autocompeting word");

                Context.AppAgentMgr.TextChangedNotifications.Release();

                Log.Debug("Returned from TextChangedNotifications.Release AFTER Autocompeting word " + threadId);
            }

            Log.Debug("Exited AutoCompleteWord");
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
                    if (!context.TextAgent().GetCharLeftOfCaret(out char charAtCaret))
                    {
                        return null;
                    }

                    if (CoreGlobals.AppPreferences.ExpandAbbreviationsOnSeparator &&
                        !TextUtils.IsTerminatorOrWhiteSpace(charAtCaret))
                    {
                        Log.Debug("no sentence terminator or white space here.  returning");
                        return null;
                    }

                    int startPos = context.TextAgent().GetPreviousWordAtCaret(out string word);
                    Log.Debug("Prev word: " + word);
                    if (String.IsNullOrEmpty(word))
                    {
                        return null;
                    }

                    // if there is a preceeding sentence terminator, we have to capitalize the word
                    bool isFirstWord = context.TextAgent().IsPreviousWordAtCaretTheFirstWord();

                    abbr = Context.AppAbbreviationsManager.Abbreviations.Lookup(word);

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
                Context.AppAgentMgr.TextChangedNotifications.Hold();

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
                        context.TextAgent().DelPrevWord();
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
            finally
            {
                Context.AppAgentMgr.TextChangedNotifications.Release();
            }
        }

        /// <summary>
        /// Handles actuation of a button representing an alphanumeric character
        /// </summary>
        /// <param name="modifiers">Modifier keys -shift, ctrl, alt</param>
        /// <param name="inputChar">char to send</param>
        public void HandleAlphaNumericChar(ArrayList modifiers, char inputChar)
        {
            Context.AppAgentMgr.Keyboard.Send((modifiers != null) ? modifiers.Cast<Keys>().ToList() : KeyStateTracker.GetExtendedKeys(), inputChar);

            KeyStateTracker.KeyTriggered(inputChar);

            _lastAction = LastAction.AlphaNumeric;
        }

        /// <summary>
        /// Handles actuation of a button representing an alphanumeric character
        /// </summary>
        /// <param name="inputChar">char to send</param>
        public void HandleAlphaNumericChar(char inputChar)
        {
            Context.AppAgentMgr.Keyboard.Send(inputChar);

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
                Context.AppAgentMgr.TextChangedNotifications.Hold();

                if (!ResourceUtils.LanguageSettings().IsInsertSpaceAfterChar(punctuation) &&
                    !ResourceUtils.LanguageSettings().IsDeletePrecedingSpacesChar(punctuation))
                {
                    return false;
                }

                // turn off extended keys such as alt, ctrl.  This causes problems when space
                // is inserted after puncuation and alt+space causes the system menu to show up.

                KeyStateTracker.ClearAlt();
                KeyStateTracker.ClearCtrl();

                using (AgentContext context = Context.AppAgentMgr.ActiveContext())
                {
                    if (!context.TextAgent().EnableSmartPunctuations())
                    {
                        Context.AppAgentMgr.Keyboard.Send((modifiers != null) ?
                                                            modifiers.Cast<Keys>().ToList() :
                                                            KeyStateTracker.GetExtendedKeys(),
                            punctuation);
                        _lastAction = LastAction.AlphaNumeric;
                        return true;
                    }

                    if (ResourceUtils.LanguageSettings().IsDeletePrecedingSpacesChar(punctuation))
                    {
                        // delete any spaces before the punctuation
                        context.TextAgent().GetPrecedingWhiteSpaces(out int offset, out int count);
                        Log.Debug("Preceding whitespace count: " + count);
                        if (count > 0)
                        {
                            Log.Debug("Deleting whitespaces from offset " + offset);
                            context.TextAgent().Delete(offset, count);
                        }
                    }

                    Log.Debug("Sending punctuation");
                    Context.AppAgentMgr.Keyboard.Send((modifiers != null) ?
                                                        modifiers.Cast<Keys>().ToList() :
                                                        KeyStateTracker.GetExtendedKeys(), punctuation);

                    if (ResourceUtils.LanguageSettings().IsInsertSpaceAfterChar(punctuation))
                    {
                        Context.AppAgentMgr.Keyboard.Send(KeyStateTracker.GetExtendedKeys(), ' ');
                    }

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
            finally
            {
                Context.AppAgentMgr.TextChangedNotifications.Release();
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
                    Context.AppAgentMgr.Keyboard.Send((modifiers != null) ?
                                                        modifiers.Cast<Keys>().ToList() :
                                                        KeyStateTracker.GetExtendedKeys(), virtualKey);
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
        /// Deletes the previous word.  Depends on the last editing action.
        /// If a punctuation was the last thing that was entered, it deletes
        /// the punctuations.  If word auto-completion was the last action,
        /// it undoes the auto-completion and restores the previously entered
        /// partial word.  Otherwise, it just deletes the previous word.
        /// </summary>
        public void SmartDeletePrevWord()
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

        /// <summary>
        /// Auto-corrects the previous word using the active spell
        /// checker. Also handles capitalization of the first word.
        /// Call this function after a word is autocompleted or if a
        /// space or a punctuation is inserted into the text.
        /// </summary>
        public void SpellCheck()
        {
            if (!AgentManager.Instance.TextControlAgent.GetCharLeftOfCaret(out char charAtCaret))
            {
                return;
            }

            Log.Debug("char left of  caret: [" + charAtCaret + "]");
            if (!TextUtils.IsTerminatorOrWhiteSpace(charAtCaret))
            {
                Log.Debug("no sentence terminator or white space here.  returning");
                return;
            }

            int startPos = AgentManager.Instance.TextControlAgent.GetPreviousWordAtCaret(out string word);
            Log.Debug("Prev word: [" + word + "]");
            if (String.IsNullOrEmpty(word))
            {
                return;
            }

            bool isFirstWord = AgentManager.Instance.TextControlAgent.IsPreviousWordAtCaretTheFirstWord();

            Log.Debug("Looking up " + word);
            String replacement = Context.AppSpellCheckManager.ActiveSpellChecker.Lookup(word);
            Log.Debug("Replacement is [" + replacement + "]");
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
        /// Undoes the last editing change - autocomplete word,
        /// insert punctuation or insert a character
        /// </summary>
        public void UndoLastEditChange()
        {
            try
            {
                Log.Debug("LastAction: " + _lastAction + ", currentEditingMode: " +
                          Context.AppAgentMgr.CurrentEditingMode);

                if (Context.AppAgentMgr.CurrentEditingMode != EditingMode.TextEntry)
                {
                    return;
                }

                switch (_lastAction)
                {
                    case LastAction.Punctuation:
                        deletePreviousPunctuation();
                        break;

                    case LastAction.AutoCompleteWord:

                        int caretPos;
                        String text;
                        bool isChar = false;
                        using (AgentContext context = Context.AppAgentMgr.ActiveContext())
                        {
                            caretPos = context.TextAgent().GetCaretPos();
                            text = context.TextAgent().GetText();
                            if (caretPos <= text.Length - 1)
                            {
                                char ch = text[caretPos];
                                isChar = !Char.IsWhiteSpace(ch) && !Char.IsPunctuation(ch);
                            }
                        }

                        DeletePreviousWord();

                        if (isChar)
                        {
                            AgentManager.Instance.Keyboard.Send(' ');
                        }
                        break;

                    case LastAction.AlphaNumeric:
                        AgentManager.Instance.Keyboard.Send(Keys.Back);
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }

            _lastAction = LastAction.Unknown;
        }

        public void ClearText()
        {
            using (AgentContext context = Context.AppAgentMgr.ActiveContext())
            {
                Context.AppAgentMgr.TextChangedNotifications.Hold();

                context.TextAgent().ClearText();

                _lastAction = LastAction.Unknown;    

                Context.AppAgentMgr.TextChangedNotifications.Release();
            }
        }

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
        /// Deletes a punctuation if it was last text entry.  If there is a space
        /// after the punctuation, deletes the space, the punctuation itself and then
        /// inserts a space
        /// </summary>
        private void deletePreviousPunctuation()
        {
            try
            {
                using (AgentContext context = Context.AppAgentMgr.ActiveContext())
                {
                    if (Context.AppAgentMgr.CurrentEditingMode == EditingMode.TextEntry)
                    {
                        int numChars = 2;  // punctuation + space after punctuation
                        context.TextAgent().GetPrecedingCharacters(numChars, out string precedingChars);
                        Log.Debug("prev " + numChars + " chars are : [" + precedingChars + "]");
                        if (precedingChars.Length == numChars && ResourceUtils.LanguageSettings().IsInsertSpaceAfterChar(precedingChars[0]))
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
                        bool retVal = context.TextAgent().GetCharAtCaret(out char charAtCaret);

                        Log.Debug("charAtCaret is " + Convert.ToInt32(charAtCaret));
                        if (!Char.IsPunctuation(charAtCaret) && (!retVal || charAtCaret == 0x0D || !Char.IsWhiteSpace(charAtCaret)))
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