////////////////////////////////////////////////////////////////////////////
// <copyright file="ITextControlAgent.cs" company="Intel Corporation">
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
    /// Used for text navigation
    /// </summary>
    public enum GoToItem
    {
        TopOfDocument,
        EndOfDocument,
        PreviousParagaph,
        NextParagraph,
        PreviousSentence,
        NextSentence,
        PreviousWord,
        NextWord,
        PreviousLine,
        NextLine,
        PreviousCharacter,
        NextCharacter,
        Home,
        End,
        PreviousPage,
        NextPage
    }

    /// <summary>
    /// Interface that handles all interactions with a text control in the
    /// active window. For eg, the notepad window, or an edit box in a dialog.
    /// As application agents are intimately familiar with the application they
    /// are written for, the text control agent will know how to manipualte text
    /// in the application's window.  For instance, a MS Word agent will use
    /// Office interop to manipuate text in the ms word window.
    /// </summary>
    public interface ITextControlAgent : IDisposable
    {
        /// <summary>
        /// Returns a keyboard interface to send keystrokes
        /// to the text window
        /// </summary>
        IKeyboard Keyboard { get; }

        bool CheckInsertOrReplaceWord(out int insertOrReplaceOffset, out String wordToReplace);

        /// <summary>
        /// Invoked to contextually check if a button on the scanner
        /// needs to be enabled or not
        /// </summary>
        /// <param name="arg">contextual info about the button</param>
        void CheckWidgetEnabled(CheckEnabledArgs arg);

        /// <summary>
        /// Clears text in window
        /// </summary>
        /// <returns></returns>
        void ClearText();

        /// <summary>
        /// Copy to clipboard
        /// </summary>
        /// <returns>true on success</returns>
        bool Copy();

        /// <summary>
        /// Cut to clipboard
        /// </summary>
        /// <returns>true on success</returns>
        bool Cut();

        /// <summary>
        /// Deletes 'count' characters starting from 'offset'
        /// </summary>
        /// <param name="offset">Where to start</param>
        /// <param name="count">How many chars to delete</param>
        void Delete(int offset, int count);

        /// <summary>
        /// Indicates whether abbreviations need to be expanded or not.  For
        /// instance, if the text control is for accepting a filename, we
        ///  would not want abbreviations to be expanded
        /// </summary>
        /// <returns>true if so</returns>
        bool ExpandAbbreviations();

        /// <summary>
        /// Gets caret position.
        /// </summary>
        /// <returns>caret position, -1 on error</returns>
        int GetCaretPos();

        /// <summary>
        /// Returns the character at the caret position
        /// </summary>
        /// <param name="value">return the character</param>
        /// <returns>true on success</returns>
        bool GetCharAtCaret(out char value);

        /// <summary>
        /// Returns the charater to the left of the caret
        /// </summary>
        /// <param name="value">returns the character</param>
        /// <returns>true on success</returns>
        bool GetCharLeftOfCaret(out char value);

        /// <summary>
        /// Returns the paragraph that is after the one that the caret
        /// is at.
        /// </summary>
        /// <param name="paragraph">Returns the paragraph</param>
        void GetParagraphAfterCaret(out String paragraph);

        /// <summary>
        /// Returns the current paragraph starting from the beginning
        /// of the para upto where the caret is positioned
        /// </summary>
        /// <param name="paragraph">the para text</param>
        /// <returns>offset of the paragraph</returns>
        int GetParagraphAtCaret(out String paragraph);

        /// <summary>
        /// Returns the paragraph that is before the one that the caret
        /// is at.
        /// </summary>
        /// <param name="paragraph">Returns the paragraph</param>
        void GetParagraphBeforeCaret(out String paragraph);

        /// <summary>
        /// Returns 'count' character preceding the caret position
        /// </summary>
        /// <param name="count">How many characters to fetch</param>
        /// <param name="word">returns the string of characters</param>
        /// <returns>number of characters </returns>
        int GetPrecedingCharacters(int count, out String word);

        /// <summary>
        /// Works backward from the caret position looking for whitespaces
        /// and stops when it encounters the first non-whitespace character.
        /// Returns the offset of the first whitespace character behind the
        /// cursor and also the number of whitespace characters
        /// </summary>
        /// <param name="offset">offset of the first white space</param>
        /// <param name="count">number of whitespace characters</param>
        void GetPrecedingWhiteSpaces(out int offset, out int count);

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
        void GetPrefixAndWordAtCaret(out String prefix, out String word);

        /// <summary>
        /// Returns the word previous to the one the caret is at.
        /// </summary>
        /// <param name="word">Word</param>
        /// <returns>Offset of the previous word</returns>
        int GetPreviousWordAtCaret(out String word);

        /// <summary>
        /// Returns the offset to the word previous to the word
        /// at the caret
        /// </summary>
        /// <param name="offset">return the offset</param>
        /// <param name="count">length of the previous word</param>
        /// <returns>true on success</returns>
        bool GetPrevWordOffset(out int offset, out int count);

        /// <summary>
        /// Returns highlighted text (if any)
        /// </summary>
        /// <returns>highlighted text</returns>
        String GetSelectedText();

        /// <summary>
        /// Returns the select mode
        /// </summary>
        /// <returns>true if on</returns>
        bool GetSelectMode();

        /// <summary>
        /// Returns the senctence after the one the caret is at.
        /// </summary>
        /// <param name="sentence">Returns the sentence</param>
        void GetSentenceAfterCaret(out String sentence);

        /// <summary>
        /// Returns the current sentence at the caret - beginning
        /// from the beginning of the sentence up to the caret position
        /// </summary>
        /// <param name="sentence">Returns the sentence</param>
        void GetSentenceAtCaret(out String sentence);

        /// <summary>
        /// Returns the sentence before the one that the caret is at
        /// </summary>
        /// <param name="sentence">return the sentence</param>
        void GetSentenceBeforeCaret(out String sentence);

        /// <summary>
        /// Returns the text starting from 'startPos' upto
        /// the caret
        /// </summary>
        /// <param name="startPos">Where to start from</param>
        /// <returns>The text</returns>
        String GetStringToCaret(int startPos);

        /// <summary>
        /// Gets the string of text from the text window
        /// </summary>
        /// <returns>text</returns>
        String GetText();

        /// <summary>
        /// Returns the word at the caret, starting from the beginning
        /// of the word upto the caret
        /// </summary>
        /// <param name="word">return the word</param>
        void GetWordAtCaret(out String word);

        /// <summary>
        /// Navigate text indicated by GotoItem
        /// </summary>
        /// <param name="gotoItem">How to navigate</param>
        /// <returns>true on success</returns>
        bool Goto(GoToItem gotoItem);

        /// <summary>
        /// Insert the 'word' at the 'offset'
        /// </summary>
        /// <param name="offset">where to insert the word</param>
        /// <param name="word">word to insert</param>
        void Insert(int offset, String word);

        /// <summary>
        /// Has the object been disposed off?
        /// </summary>
        /// <returns></returns>
        bool IsDisposed();

        /// <summary>
        /// Indicates whether the previous word marks the beginning
        /// of the sentence
        /// </summary>
        /// <returns>true if so</returns>
        bool IsPreviousWordAtCaretTheFirstWord();

        /// <summary>
        /// Indicates whether any text is currently
        /// highlighted
        /// </summary>
        /// <returns>true if so</returns>
        bool IsTextSelected();

        /// <summary>
        /// Invoked on a key down event
        /// </summary>
        /// <param name="keyEventArgs">event arg</param>
        void OnKeyDown(KeyEventArgs keyEventArgs);

        /// <summary>
        /// Invoked on a key up
        /// </summary>
        /// <param name="keyEventArgs">event arg</param>
        void OnKeyUp(KeyEventArgs keyEventArgs);

        /// <summary>
        /// Invoked on a mouse down event
        /// </summary>
        /// <param name="mouseEventArgs">event args</param>
        void OnMouseDown(MouseEventArgs mouseEventArgs);

        /// <summary>
        /// Paste from clipboard
        /// </summary>
        /// <returns></returns>
        bool Paste();

        /// <summary>
        /// Pause.  Don't make any text changes.
        /// </summary>
        void Pause();

        /// <summary>
        /// Redo an undo (equivalent to Ctrl-Y)
        /// </summary>
        /// <returns>true on success</returns>
        bool Redo();

        /// <summary>
        /// Replaces 'count' letters at 'offset' with the word
        /// </summary>
        /// <param name="offset">Where to start</param>
        /// <param name="count">How many chars to replace</param>
        /// <param name="word">replace with what</param>
        void Replace(int offset, int count, String word);

        /// <summary>
        /// Resume making text changes
        /// </summary>
        void Resume();

        /// <summary>
        /// Highlight all the text (equivalent to pressing
        /// Ctrl-A)
        /// </summary>
        /// <returns>true on success</returns>
        bool SelectAll();

        /// <summary>
        /// Highlight the paragraph after the one the
        /// caret's at.
        /// </summary>
        /// <returns>true on success</returns>
        bool SelectNextParagraph();

        /// <summary>
        /// Highlight the sentence after the one the
        /// caret's at.
        /// </summary>
        /// <returns>true on success</returns>
        bool SelectNextSentence();

        /// <summary>
        /// Highlight the paragraph at the caret
        /// </summary>
        /// <returns>true on success</returns>
        bool SelectParagraphAtCaret();

        /// <summary>
        /// Highlight the paragraph before the one the
        /// caret's at.
        /// </summary>
        /// <returns>true on success</returns>
        bool SelectPreviousParagraph();

        /// <summary>
        /// Highlight the sentence before the one the
        /// caret's at.
        /// </summary>
        /// <returns>true on success</returns>
        bool SelectPreviousSentence();

        /// <summary>
        /// Highlight the sentence at the caret
        /// </summary>
        /// <returns>true on success</returns>
        bool SelectSentenceAtCaret();

        /// <summary>
        /// Sets the caret position in the text window
        /// </summary>
        /// <param name="pos">caret position</param>
        /// <returns>true on success</returns>
        bool SetCaretPos(int pos);

        /// <summary>
        /// Sets focus to the text window
        /// </summary>
        /// <returns>true on success</returns>
        bool SetFocus();

        /// <summary>
        /// If selectMode is true, Select is turned on.
        /// If selectMode is on, when the cursor moves,
        /// text is highlighted.  This is similar to holding
        /// the shift key down and using one of the arrow keys
        /// or page navigation.  Text continues to highlight as
        /// caret moves
        /// </summary>
        /// <param name="selectMode">true to turn on</param>
        void SetSelectMode(bool selectMode);

        /// <summary>
        /// Indicates whether the text control supports spell checking.  FOr
        /// instance, MS Word does.  If it doesn't, ACAT will use it's own
        /// native spell checker
        /// </summary>
        /// <returns>true if so</returns>
        bool SupportsSpellCheck();

        /// <summary>
        /// Undo changes (Equivalent to Ctrl-Z)
        /// </summary>
        /// <returns>true on success</returns>
        bool Undo();

        /// <summary>
        /// Unselects any selection
        /// </summary>
        void UnselectText();
    }
}