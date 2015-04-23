////////////////////////////////////////////////////////////////////////////
// <copyright file="MSWordTextControlAgent.cs" company="Intel Corporation">
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
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.AgentManagement.TextInterface;
using ACAT.Lib.Core.Utility;
using Word = Microsoft.Office.Interop.Word;

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

namespace ACAT.Lib.Extension.AppAgents.MSWord
{
    /// <summary>
    /// Text control agent class for MS word. Handles all editing
    /// functions and navigating through the document.  Uses the
    /// Office Interop API to interface with Word.
    /// </summary>
    public class MSWordTextControlAgent : TextControlAgentBase
    {
        /// <summary>
        /// Don't go below this zoom level
        /// </summary>
        private const int MinZoomLevel = 10;

        /// <summary>
        /// Incremental zoom amout for zoomin or zoomout
        /// </summary>
        private const int ZoomDelta = 10;

        /// <summary>
        /// Which editing features do we support?
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
            "PreviousPara",
            "NextPara",
            "End",
            "SelectAll",
            "SelectMode",
            "DeletePreviousChar",
            "DeleteNextChar",
            "EnterKey"
        };

        /// <summary>
        /// The navigator object that lets us navigate around
        /// the document.
        /// </summary>
        private readonly MSWordTextNavigator _wordNavigator;

        /// <summary>
        /// Used for cross-thread synchronization
        /// </summary>
        private volatile bool _sync;

        /// <summary>
        /// The word application object
        /// </summary>
        private Word.Application _wordApp;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public MSWordTextControlAgent()
        {
            Log.Debug();

            _wordNavigator = new MSWordTextNavigator();
            EvtLearn += MSWordTextControlAgent_EvtLearn;
            createWordApplication();
        }

        /// <summary>
        /// For the event raised to add the current sentence
        /// to the word prediction user model
        /// </summary>
        private delegate void LearnDelegate();

        /// <summary>
        /// Raised to add the current sentence
        /// to the word prediction user model
        /// </summary>
        private event LearnDelegate EvtLearn;

        /// <summary>
        /// Determines whether to replace the word or insert
        /// a new word for auto-completion
        /// </summary>
        /// <param name="insertOrReplaceOffset">offset to replace from</param>
        /// <param name="wordToReplace">the word that should be replaced</param>
        /// <returns>true if the word should be inserted</returns>
        public override bool CheckInsertOrReplaceWord(out int insertOrReplaceOffset, out String wordToReplace)
        {
            insertOrReplaceOffset = 0;
            wordToReplace = String.Empty;
            bool retVal;

            Log.Debug();

            if (!isWordRunning())
            {
                return false;
            }

            try
            {
                Word.Range r = _wordApp.Selection.Range;
                r = r.Previous(Word.WdUnits.wdCharacter);
                int caretPos = _wordApp.Selection.Range.Start;
                if (r != null)
                {
                    if (String.IsNullOrEmpty(r.Text))
                    {
                        Log.Debug("r.Text is empty. returning true.  offset:" + insertOrReplaceOffset);
                        insertOrReplaceOffset = caretPos;
                        return true;
                    }

                    char ch = r.Text[0];

                    Word.Range next = r.Next(Word.WdUnits.wdCharacter);
                    int nextPos;
                    if (next != null)
                    {
                        nextPos = next.Start;
                        Log.Debug("nextPos is " + nextPos);
                    }
                    else
                    {
                        nextPos = r.Start;
                        Log.Debug("next is null. nextPos is " + nextPos);
                    }

                    if (Char.IsWhiteSpace(ch))
                    {
                        insertOrReplaceOffset = nextPos;
                        Log.Debug("ch is whitespace.  returning true " + insertOrReplaceOffset);
                        return true;
                    }

                    if (TextUtils.IsSentenceTerminator(ch))
                    {
                        insertOrReplaceOffset = nextPos;
                        Log.Debug("ch is sentenceterminator.  returning true " + insertOrReplaceOffset);
                        return true;
                    }

                    if (TextUtils.IsWordElement(ch))
                    {
                        Log.Debug("ch is word element. calling getprevwordatcaret");
                        insertOrReplaceOffset = GetPreviousWordAtCaret(out wordToReplace);
                        Log.Debug("insertorrep offset: " + insertOrReplaceOffset + ", wordTOReplace: " + wordToReplace);
                        return false;
                    }
                    Log.Debug("ch is NOT word element. returning true with offset " + insertOrReplaceOffset);
                    insertOrReplaceOffset = nextPos;
                    return true;
                }

                retVal = true;
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
                disposeWordApplication();
                retVal = false;
            }

            Log.Debug("returning insertOrReplaceOffset: " + insertOrReplaceOffset + ", wordToReplace: " + wordToReplace);
            return retVal;
        }

        /// <summary>
        /// Invoked to set the 'enabled' state of a widget.  This
        /// will depend on the current context.
        /// </summary>
        /// <param name="arg">contains info about the widget</param>
        public override void CheckWidgetEnabled(CheckEnabledArgs arg)
        {
            if (String.Compare(arg.Widget.SubClass, "ClearTalkWindowText", true) == 0)
            {
                arg.Handled = true;
                arg.Enabled = false;
            }
            else
            {
                if (_supportedFeatures.Contains(arg.Widget.SubClass))
                {
                    arg.Enabled = true;
                    arg.Handled = true;
                }
            }
        }

        /// <summary>
        /// Not used
        /// </summary>
        public override void ClearText()
        {
        }

        /// <summary>
        /// Copies selected text to the clipboard
        /// </summary>
        /// <returns>true on success</returns>
        public override bool Copy()
        {
            if (!isWordRunning())
            {
                return false;
            }

            base.Copy();
            return true;
        }

        /// <summary>
        /// Cuts selected text to clipboard
        /// </summary>
        /// <returns>true on success</returns>
        public override bool Cut()
        {
            if (!isWordRunning())
            {
                return false;
            }

            base.Cut();
            return true;
        }

        /// <summary>
        /// Deletes 'count' characters at the specified offset
        /// </summary>
        /// <param name="offset">Where to delete</param>
        /// <param name="count">how many characters to delete</param>
        public override void Delete(int offset, int count)
        {
            try
            {
                AgentManager.Instance.TextChangedNotifications.Hold();

                Log.Debug("offset: " + offset + ", count: " + count);

                SetCaretPos(offset);
                int caretPos = moveCaret(count);
                Log.Debug("new caretpos:  " + caretPos);

                if (caretPos >= 0 && offset <= caretPos)
                {
                    Log.Debug("actual chars to delete :  " + (caretPos - offset));
                    Word.Range r = _wordApp.ActiveDocument.Range(offset, caretPos);
                    if (r != null)
                    {
                        r.Delete();
                    }
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
        /// Returns the current caret position
        /// in the active Word document.
        /// </summary>
        /// <returns></returns>
        public override int GetCaretPos()
        {
            return getCaretPos();
        }

        /// <summary>
        /// Returns the charcter at the caret position
        /// </summary>
        /// <param name="value">the character</param>
        /// <returns>true on success</returns>
        public override bool GetCharAtCaret(out char value)
        {
            bool retVal = true;
            value = '\0';

            Log.Debug();

            if (!isWordRunning())
            {
                return false;
            }

            try
            {
                value = _wordApp.Selection.Characters[1].Text[0];
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
                disposeWordApplication();
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Returns the characters that is to the immediate left
        /// of the caret
        /// </summary>
        /// <param name="value">returns the character</param>
        /// <returns>true on success</returns>
        public override bool GetCharLeftOfCaret(out char value)
        {
            value = '\0';
            bool retVal = true;

            Log.Debug();

            if (!isWordRunning())
            {
                return false;
            }

            try
            {
                Word.Range r = _wordApp.Selection.Range;
                Word.Range prev = r.Previous(Word.WdUnits.wdCharacter);
                if (prev != null)
                {
                    String text = prev.Text;
                    if (!String.IsNullOrEmpty(text))
                    {
                        value = text[0];
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
                disposeWordApplication();
                retVal = false;
            }

            Log.Debug("returning");
            return retVal;
        }

        /// <summary>
        /// Gets the filename of the active word document
        /// </summary>
        /// <returns>the filename</returns>
        public String GetFileName()
        {
            return (_wordApp != null) ? getWordApplication().ActiveDocument.Name : String.Empty;
        }

        /// <summary>
        /// Gets the paragraph after the one at the caret
        /// </summary>
        /// <param name="paragraph">the paragraph</param>
        public override void GetParagraphAfterCaret(out String paragraph)
        {
            paragraph = String.Empty;
            if (isWordRunning())
            {
                lock (_wordNavigator)
                {
                    _wordNavigator.GetParagraphAfterCaret(out paragraph);
                }
            }
        }

        /// <summary>
        /// Gets the paragraph where the caret position
        /// </summary>
        /// <param name="paragraph">Returns the paragraph text</param>
        /// <returns>starting position of the paragraph</returns>
        public override int GetParagraphAtCaret(out String paragraph)
        {
            paragraph = String.Empty;
            int retVal = -1;

            Log.Debug();

            if (!isWordRunning())
            {
                return retVal;
            }

            try
            {
                Word.Range paraRange = _wordApp.Selection.Range.Paragraphs[1].Range;
                paragraph = paraRange.Text;
                retVal = _wordApp.Selection.Range.Paragraphs[1].Range.Start;
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
                disposeWordApplication();
                retVal = -1;
            }

            Log.Debug("returning");

            return retVal;
        }

        /// <summary>
        /// Returns the paragraph before the one at the
        /// caret position
        /// </summary>
        /// <param name="paragraph">the paragraph</param>
        public override void GetParagraphBeforeCaret(out String paragraph)
        {
            paragraph = String.Empty;
            if (isWordRunning())
            {
                lock (_wordNavigator)
                {
                    _wordNavigator.GetParagraphBeforeCaret(out paragraph);
                }
            }
        }

        /// <summary>
        /// Gets the preceding 'count' characters
        /// </summary>
        /// <param name="count">how many characters?</param>
        /// <param name="word">the preceding characters</param>
        /// <returns>index</returns>
        public override int GetPrecedingCharacters(int count, out String word)
        {
            word = String.Empty;
            if (!isWordRunning())
            {
                return 0;
            }

            try
            {
                Word.Range r = _wordApp.Selection.Range;
                int ii = 0;
                for (ii = 0; ii < count && r != null; ii++)
                {
                    r = r.Previous(Word.WdUnits.wdCharacter);
                }

                if (r != null && r.Start <= _wordApp.Selection.Range.Start)
                {
                    word = _wordApp.ActiveDocument.Range(r.Start, _wordApp.Selection.Range.Start).Text;
                }

                return ii;
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }

            return 0;
        }

        /// <summary>
        /// Starting at offset, gets the preceding whie space count
        /// </summary>
        /// <param name="offset">where to start from</param>
        /// <param name="count">returns number of preceding white spaces</param>
        public override void GetPrecedingWhiteSpaces(out int offset, out int count)
        {
            offset = 0;
            count = 0;

            if (!isWordRunning())
            {
                return;
            }

            try
            {
                String sentence;
                int sentenceOffset;
                getSentenceAtCaret(out sentence, out sentenceOffset);
                if (!String.IsNullOrEmpty(sentence))
                {
                    Log.Debug("sentenceOffset: " + sentenceOffset);
                    Word.Range r = _wordApp.Selection.Range;
                    Log.Debug("caretPos : " + r.Start);
                    Word.Range prev = r.Previous(Word.WdUnits.wdCharacter);
                    while (prev != null && prev.Start > sentenceOffset)
                    {
                        String text = prev.Text;
                        if (!String.IsNullOrEmpty(text))
                        {
                            char value = text[0];
                            Log.Debug("prevChar: " + value);
                            if (value == 0x0D || value == 0x0A || !Char.IsWhiteSpace(value))
                            {
                                Log.Debug("breaking...");
                                break;
                            }
                        }
                        else
                        {
                            Log.Debug("text is null. break");
                            break;
                        }

                        prev = prev.Previous(Word.WdUnits.wdCharacter);
                    }

                    if (prev != null)
                    {
                        prev = prev.Next(Word.WdUnits.wdCharacter);
                        offset = prev.Start;
                        count = _wordApp.Selection.Range.Start - prev.Start;
                        Log.Debug("return offset: " + offset + ", count: " + count);
                    }
                    else
                    {
                        Log.Debug("prev is null!!!!");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }
        }

        /// <summary>
        /// Used for word prediction.  Returns the previous
        /// words in the current sentence as well as the word
        /// at the caret position
        /// </summary>
        /// <param name="prefix">previous words</param>
        /// <param name="word">the word at the caret position</param>
        public override void GetPrefixAndWordAtCaret(out String prefix, out String word)
        {
            prefix = String.Empty;
            word = String.Empty;

            Log.Debug();

            if (!isWordRunning())
            {
                return;
            }

            try
            {
                prefix = String.Empty;
                word = String.Empty;

                char charAtCaret;
                char leftOfCaret;
                GetCharAtCaret(out charAtCaret);

                Log.Debug("charAtCaret: " + charAtCaret);

                int wordStart = -1;
                GetCharLeftOfCaret(out leftOfCaret);
                Log.Debug("leftofcaret: " + leftOfCaret);

                if (TextUtils.IsSentenceTerminator(leftOfCaret) || leftOfCaret == '\r')
                {
                    Log.Debug("Left of caret is termiantor. returning");
                    return;
                }

                if (Char.IsPunctuation(charAtCaret))
                {
                    Log.Debug("Yes.  This s a punctuation");
                    if (!Char.IsPunctuation(leftOfCaret) && 
                        !Char.IsWhiteSpace(leftOfCaret) && 
                        leftOfCaret != '’')
                    {
                        wordStart = GetPreviousWordAtCaret(out word);
                        Log.Debug("Prev word is " + word + ", at offset " + wordStart);
                    }
                }

                if (wordStart < 0)
                {
                    if (leftOfCaret != '’' && 
                        (Char.IsPunctuation(leftOfCaret) || 
                        Char.IsWhiteSpace(leftOfCaret)))
                    {
                        wordStart = _wordApp.Selection.Start;
                        word = String.Empty;
                    }
                    else
                    {
                        wordStart = GetPreviousWordAtCaret(out word);
                    }
                }

                word = word ?? String.Empty;
                if (wordStart >= 0)
                {
                    int sentenceStart = _wordApp.Selection.Sentences[1].Start;
                    Log.Debug("wordStart: " + wordStart + ", sentenceStart: " + sentenceStart);
                    if (sentenceStart <= wordStart)
                    {
                        Word.Range prefixRange = _wordApp.ActiveDocument.Range(sentenceStart, wordStart);
                        if (prefixRange != null)
                        {
                            prefix = prefixRange.Text ?? string.Empty;
                        }
                    }
                }

                Log.Debug("prefix: [" + prefix + "]. word: [" + word + "]");
            }
            catch (Exception ex)
            {
                word = String.Empty;
                prefix = String.Empty;
                Log.Debug(ex.ToString());
                disposeWordApplication();
            }

            Log.Debug("returning");
        }

        /// <summary>
        /// Gets the word before the caret
        /// </summary>
        /// <param name="word">the word</param>
        /// <returns>offset of the previous word</returns>
        public override int GetPreviousWordAtCaret(out String word)
        {
            int retVal = -1;

            Log.Debug();

            word = String.Empty;

            if (!isWordRunning())
            {
                return -1;
            }

            try
            {
                word = String.Empty;

                Word.Range wr = _wordApp.Selection.Range.Words[1];
                if (wr != null)
                {
                    String text = wr.Text;
                    Log.Debug("word at caret: [" + text + "]");
                }

                Word.Range endCharRange = _wordApp.Selection.Range.Characters[1];
                while (true)
                {
                    endCharRange = endCharRange.Previous(Word.WdUnits.wdCharacter);
                    if (endCharRange == null)
                    {
                        Log.Debug("char range is null");
                        break;
                    }

                    char ch = endCharRange.Text[0];
                    Log.Debug("Char ch: [" + ch + "]");

                    // find the first previous printable character
                    if (ch == '’' || (!TextUtils.IsPunctuationOrWhiteSpace(ch) &&
                        !TextUtils.IsTerminatorOrWhiteSpace(ch)))
                    {
                        break;
                    }
                }

                if (endCharRange != null)
                {
                    Word.Range beginCharRange = endCharRange;
                    while (true)
                    {
                        char ch = beginCharRange.Text[0];
                        if (TextUtils.IsTerminatorOrWhiteSpace(ch))
                        {
                            Log.Debug("It is a terminator or whitespace. breaking");
                            beginCharRange = beginCharRange.Next(Word.WdUnits.wdCharacter);
                            break;
                        }

                        beginCharRange = beginCharRange.Previous(Word.WdUnits.wdCharacter);
                        if (beginCharRange == null)
                        {
                            beginCharRange = _wordApp.ActiveDocument.Range(0, 0);
                            break;
                        }
                    }

                    if (beginCharRange != null)
                    {
                        endCharRange = endCharRange.Next(Word.WdUnits.wdCharacter);
                        Word.Range rr = _wordApp.ActiveDocument.Range(beginCharRange.Start, endCharRange.Start);
                        word = rr.Text;
                        Log.Debug("returning- WORD: [" + word + "]");
                        return beginCharRange.Start;
                    }
                    else
                    {
                        Log.Debug("begincharrange is null");
                    }
                }
                else
                {
                    Log.Debug("charrange is null");
                }
            }
            catch (Exception ex)
            {
                retVal = -1;
                Log.Debug(ex.ToString());
                disposeWordApplication();
            }

            Log.Debug("returning");
            return retVal;
        }

        /// <summary>
        /// Gets the offset of the previous word and also the count
        /// of characters
        /// </summary>
        /// <param name="wordOffset">offset of the previous word</param>
        /// <param name="count">number of characters in the word</param>
        /// <returns></returns>
        public override bool GetPrevWordOffset(out int wordOffset, out int count)
        {
            bool retVal = true;

            wordOffset = 0;
            count = 0;

            Log.Debug();

            if (!isWordRunning())
            {
                return false;
            }

            try
            {
                String paragraph;
                GetParagraphAtCaret(out paragraph);
                int caretPos = _wordApp.Selection.Range.Start;

                count = 0;
                wordOffset = 0;

                // reach beginning of para
                if (!getPrevWordOffset(caretPos, out wordOffset, out count))
                {
                    Log.Debug("Reached the beginning of para.  Going to previous para");
                    Word.Paragraph para = _wordApp.Selection.Range.Paragraphs[1];
                    para = para.Previous();

                    if (para != null)
                    {
                        Word.Range range = para.Range;
                        getPrevWordOffset(range.End, out wordOffset, out count);
                    }
                    else
                    {
                        Log.Debug("prev para is null");
                        retVal = false;
                    }
                }

                Log.Debug("Prev word offset: " + wordOffset + ". count: " + count);
            }
            catch (Exception ex)
            {
                retVal = false;
                Log.Debug(ex.ToString());
            }

            Log.Debug("returning");

            return retVal;
        }

        /// <summary>
        /// Returns text that is selected.  Empty string
        /// if nothing is currently selected
        /// </summary>
        /// <returns>selected text</returns>
        public override String GetSelectedText()
        {
            if (!isWordRunning())
            {
                return String.Empty;
            }

            lock (_wordNavigator)
            {
                return _wordNavigator.GetSelectedText();
            }
        }

        /// <summary>
        /// Returns the sentence after the one at the caret position
        /// </summary>
        /// <param name="sentence">senctence</param>
        public override void GetSentenceAfterCaret(out String sentence)
        {
            sentence = String.Empty;
            if (isWordRunning())
            {
                lock (_wordNavigator)
                {
                    _wordNavigator.GetSentenceAfterCaret(out sentence);
                }
            }
        }

        /// <summary>
        /// Returns the sentence at the caret position
        /// </summary>
        /// <param name="sentence">the sentence</param>
        public override void GetSentenceAtCaret(out String sentence)
        {
            int offset;
            getSentenceAtCaret(out sentence, out offset);
        }

        /// <summary>
        /// Gets the sentence before the one at the caret
        /// </summary>
        /// <param name="sentence">sentence</param>
        public override void GetSentenceBeforeCaret(out String sentence)
        {
            sentence = String.Empty;
            if (isWordRunning())
            {
                lock (_wordNavigator)
                {
                    _wordNavigator.GetSentenceBeforeCaret(out sentence);
                }
            }
        }

        /// <summary>
        /// Gets the text starting from startPos to the
        /// caret position
        /// </summary>
        /// <param name="startPos">where to start from</param>
        /// <returns>the text</returns>
        public override String GetStringToCaret(int startPos)
        {
            var retVal = String.Empty;

            Log.Debug("startPos:  " + startPos);

            if (!isWordRunning())
            {
                return retVal;
            }

            try
            {
                int caret = _wordApp.Selection.Range.Start;
                if (startPos <= caret)
                {
                    Word.Range r = _wordApp.ActiveDocument.Range(startPos, caret);
                    if (r != null)
                    {
                        retVal = r.Text;
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = String.Empty;
                Log.Debug(ex.ToString());
                disposeWordApplication();
            }

            Log.Debug("returning [" + retVal + "]");
            return retVal;
        }

        /// <summary>
        /// Gets the string of text from the target app's window
        /// </summary>
        /// <returns>text</returns>
        public override String GetText()
        {
            if (_wordApp == null)
            {
                return String.Empty;
            }

            String retVal = String.Empty;
            try
            {
                Log.Debug("Retrieving text...");
                var sb = new StringBuilder(_wordApp.ActiveDocument.Content.Text);
                sb.Remove(sb.Length - 1, 1);
                retVal = sb.ToString();
                Log.Debug("AutoCompleteWord.  text: [" + retVal + "]");
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }

            return retVal;
        }

        /// <summary>
        /// Returns the text from the word document.  If
        /// track changes are turned on, temporarily toggles it
        /// off and retrieves the final text
        /// </summary>
        /// <param name="fixCarriageReturns">fix cr-lf's</param>
        /// <returns></returns>
        public string GetTextFromDocument(bool fixCarriageReturns = true)
        {
            if (_wordApp != null)
            {
                Word.Revisions revs = getWordApplication().ActiveDocument.Revisions;
                String text = String.Empty;
                if (revs.Count == 0)
                {
                    text = getWordApplication().ActiveDocument.Content.Text;
                }
                else
                {
                    // toggle revisions off
                    Word.WdRevisionsView v = getWordApplication().ActiveDocument.ActiveWindow.View.RevisionsView;
                    getWordApplication().ActiveDocument.ActiveWindow.View.RevisionsView = Word.WdRevisionsView.wdRevisionsViewFinal;
                    bool showRevisions = getWordApplication().ActiveDocument.ActiveWindow.View.ShowRevisionsAndComments;
                    getWordApplication().ActiveDocument.ActiveWindow.View.ShowRevisionsAndComments = false;

                    // get the text
                    text = getWordApplication().ActiveDocument.Content.Text;

                    // restore revisions to the previous state
                    getWordApplication().ActiveDocument.ActiveWindow.View.RevisionsView = v;
                    getWordApplication().ActiveDocument.ActiveWindow.View.ShowRevisionsAndComments = showRevisions;
                }

                return fixCarriageReturns ? Regex.Replace(text, "\r(?<!\n)", "\r\n") : text;
            }

            return String.Empty;
        }

        /// <summary>
        /// Returns the word at the caret position
        /// </summary>
        /// <param name="word">the word</param>
        public override void GetWordAtCaret(out String word)
        {
            word = String.Empty;

            Log.Debug();

            if (!isWordRunning())
            {
                return;
            }

            try
            {
                word = _wordApp.Selection.Range.Words[1].Text;
                Log.Debug("word: " + word);
            }
            catch (Exception ex)
            {
                word = String.Empty;
                Log.Debug(ex.ToString());
                disposeWordApplication();
            }

            Log.Debug("returning");
        }

        /// <summary>
        /// Sets the cursor to the next/previous sentence/para
        /// </summary>
        /// <param name="gotoItem">navigation mode</param>
        /// <returns>true on success</returns>
        public override bool Goto(GoToItem gotoItem)
        {
            bool retVal = true;

            if (!isWordRunning())
            {
                return false;
            }

            _sync = true;
            lock (_wordNavigator)
            {
                switch (gotoItem)
                {
                    case GoToItem.NextParagraph:
                        retVal = _wordNavigator.GotoNextParagraph();
                        break;

                    case GoToItem.PreviousParagaph:
                        retVal = _wordNavigator.GotoPreviousParagraph();
                        break;

                    case GoToItem.NextSentence:
                        retVal = _wordNavigator.GotoNextSentence();
                        break;

                    case GoToItem.PreviousSentence:
                        retVal = _wordNavigator.GotoPreviousSentence();
                        break;

                    default:
                        retVal = base.Goto(gotoItem);
                        break;
                }
            }

            _sync = false;
            triggerTextChanged(this);
            return retVal;
        }

        /// <summary>
        /// Checks if the previous word is the first word in the
        /// sentence
        /// </summary>
        /// <returns>true if it is</returns>
        public override bool IsPreviousWordAtCaretTheFirstWord()
        {
            Log.Debug();

            if (!isWordRunning())
            {
                return false;
            }

            try
            {
                String word;
                int offset = GetPreviousWordAtCaret(out word);
                int sentenceOffset;
                String sentence;
                getSentenceAtCaret(out sentence, out sentenceOffset);

                return offset == sentenceOffset;
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
                disposeWordApplication();
            }

            Log.Debug("returning");
            return false;
        }

        /// <summary>
        /// Returns if there is any text selected in
        /// the active word document
        /// </summary>
        /// <returns>true if it is</returns>
        public override bool IsTextSelected()
        {
            if (!isWordRunning())
            {
                return false;
            }

            lock (_wordNavigator)
            {
                return _wordNavigator.IsTextSelected();
            }
        }

        /// <summary>
        /// On key up handler.  Since a key was
        /// pressed, text changed in the active window.
        /// Raise the event
        /// </summary>
        /// <param name="e">event args</param>
        public override void OnKeyUp(KeyEventArgs e)
        {
            Thread.Sleep(10);
            triggerTextChangedAsync(this);
        }

        /// <summary>
        /// A sentence terminator was detected. Trigger event
        /// 'learn' the current sentence
        /// </summary>
        public override void OnSentenceTerminator()
        {
            EvtLearn.BeginInvoke(null, null);
        }

        /// <summary>
        /// Pastes clipboard content into the document
        /// </summary>
        /// <returns>true on success</returns>
        public override bool Paste()
        {
            if (!isWordRunning())
            {
                return false;
            }

            base.Paste();
            return true;
        }

        /// <summary>
        /// Redoes the previous undo
        /// </summary>
        /// <returns>true on success</returns>
        public override bool Redo()
        {
            if (!isWordRunning())
            {
                return false;
            }

            base.Redo();
            return true;
        }

        /// <summary>
        /// Replaces count characters at the specified offset
        /// with the specified word
        /// </summary>
        /// <param name="offset">0 based starting offset</param>
        /// <param name="count">number of chars to replace</param>
        /// <param name="word">Word to insert at the 'offset'</param>
        public override void Replace(int offset, int count, String word)
        {
            Log.Debug("offset = " + offset + " count " + count + " word " + word);

            try
            {
                AgentManager.Instance.TextChangedNotifications.Hold();

                if (!IsTextSelected())
                {
                    SetCaretPos(offset);
                    Log.Debug("actual chars to delete :  " + count);
                    SendKeys.SendWait("{DELETE " + count + "}");
                }

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
        /// Selects all the text in the document
        /// </summary>
        /// <returns>true on success</returns>
        public override bool SelectAll()
        {
            if (!isWordRunning())
            {
                return false;
            }

            base.SelectAll();
            return true;
        }

        /// <summary>
        /// Selects the paragraph following the one at the caret
        /// </summary>
        /// <returns>true on success</returns>
        public override bool SelectNextParagraph()
        {
            if (!isWordRunning())
            {
                return false;
            }

            lock (_wordNavigator)
            {
                return _wordNavigator.SelectNextParagraph();
            }
        }

        /// <summary>
        /// Selects the sentence following the one at the caret
        /// </summary>
        /// <returns>true on success</returns>
        public override bool SelectNextSentence()
        {
            if (!isWordRunning())
            {
                return false;
            }

            lock (_wordNavigator)
            {
                return _wordNavigator.SelectNextSentence();
            }
        }

        /// <summary>
        /// Selects the paragraph as the caret position
        /// </summary>
        /// <returns>true on success</returns>
        public override bool SelectParagraphAtCaret()
        {
            if (!isWordRunning())
            {
                return false;
            }

            lock (_wordNavigator)
            {
                return _wordNavigator.SelectParagraphAtCaret();
            }
        }

        /// <summary>
        /// Selects the paragraph before the one at the caret
        /// </summary>
        /// <returns>true on success</returns>
        public override bool SelectPreviousParagraph()
        {
            if (!isWordRunning())
            {
                return false;
            }

            lock (_wordNavigator)
            {
                return _wordNavigator.SelectPreviousParagraph();
            }
        }

        /// <summary>
        /// Selects the sentence before the one at the caret
        /// </summary>
        /// <returns>true on success</returns>
        public override bool SelectPreviousSentence()
        {
            if (!isWordRunning())
            {
                return false;
            }

            lock (_wordNavigator)
            {
                return _wordNavigator.SelectPreviousSentence();
            }
        }

        /// <summary>
        /// Selects the senetence at the caret position
        /// </summary>
        /// <returns>true on success</returns>
        public override bool SelectSentenceAtCaret()
        {
            if (!isWordRunning())
            {
                return false;
            }

            lock (_wordNavigator)
            {
                return _wordNavigator.SelectSentenceAtCaret();
            }
        }

        /// <summary>
        /// Sets the caret position to the indicated
        /// value in the active Word document.
        /// </summary>
        /// <param name="pos">caret position</param>
        /// <returns>true on success</returns>
        public override bool SetCaretPos(int pos)
        {
            bool retVal = true;

            Log.Debug();

            if (!isWordRunning())
            {
                return false;
            }

            Log.Debug("caret pos: " + pos);
            try
            {
                _sync = true;
                _wordApp.ActiveDocument.Range(pos, pos).Select();
                _sync = false;
                triggerTextChanged(this);
            }
            catch (Exception ex)
            {
                _sync = false;
                Log.Debug(ex.ToString());

                disposeWordApplication();
                retVal = false;
            }

            Log.Debug("returning");
            return retVal;
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <returns></returns>
        public override bool SetFocus()
        {
            return false;
        }

        /// <summary>
        /// Word supports spell check.  No need
        /// to use ACAT spell checker
        /// </summary>
        /// <returns>true</returns>
        public override bool SupportsSpellCheck()
        {
            return true;
        }

        /// <summary>
        /// Undoes the previous operation
        /// </summary>
        /// <returns>true on success</returns>
        public override bool Undo()
        {
            if (!isWordRunning())
            {
                return false;
            }

            base.Undo();
            return true;
        }

        /// <summary>
        /// Unselects any text that is selected
        /// </summary>
        public override void UnselectText()
        {
            int caretPos = getCaretPos();
            if (caretPos >= 0)
            {
                SetCaretPos(caretPos);
            }
        }

        /// <summary>
        /// Fits the zoom level of the document in the
        /// window
        /// </summary>
        public void ZoomFit()
        {
            if (_wordApp != null)
            {
                try
                {
                    _wordApp.ActiveWindow.View.Zoom.PageFit = Word.WdPageFit.wdPageFitBestFit;
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                }
            }
        }

        /// <summary>
        /// Zooms in the document incrementally
        /// </summary>
        public void ZoomIn()
        {
            if (_wordApp != null)
            {
                try
                {
                    int zoomLevel = _wordApp.ActiveWindow.View.Zoom.Percentage;
                    zoomLevel += ZoomDelta;
                    _wordApp.ActiveWindow.View.Zoom.Percentage = zoomLevel;
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                }
            }
        }

        /// <summary>
        /// Zooms out the document incrementally
        /// </summary>
        public void ZoomOut()
        {
            if (_wordApp != null)
            {
                try
                {
                    int zoomLevel = _wordApp.ActiveWindow.View.Zoom.Percentage;
                    zoomLevel -= ZoomDelta;
                    if (zoomLevel > MinZoomLevel)
                    {
                        _wordApp.ActiveWindow.View.Zoom.Percentage = zoomLevel;
                    }
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                }
            }
        }

        /// <summary>
        /// Disposes current instance of the word application
        /// object and creates a new one
        /// </summary>
        protected void disposeAndCreateWordApplication()
        {
            disposeWordApplication();
            createWordApplication();
        }

        /// <summary>
        /// Gets an instance of the Interop word application object
        /// </summary>
        /// <returns>the word application object</returns>
        protected Word.Application getWordApplication()
        {
            Word.Application retVal = null;
            try
            {
                createWordApplication();
                if (_wordApp == null)
                {
                    disposeAndCreateWordApplication();
                }

                if (_wordApp != null)
                {
                    try
                    {
                        retVal = _wordApp.Application;
                    }
                    catch (Exception e)
                    {
                        Log.Debug(e.ToString());
                        disposeAndCreateWordApplication();
                        if (_wordApp != null)
                        {
                            retVal = _wordApp.Application;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }

            return retVal;
        }

        /// <summary>
        /// Dispose resources
        /// </summary>
        protected override void OnDispose()
        {
            disposeWordApplication();
        }

        /// <summary>
        /// Invoked when the select mode changes. If it is ON,
        /// when the caret is moved, it selects the text as it moves.
        /// </summary>
        /// <param name="selectMode">new selection mode</param>
        protected override void OnSelectModeChanged(bool selectMode)
        {
            if (!isWordRunning())
            {
                return;
            }

            lock (_wordNavigator)
            {
                _wordNavigator.SelectMode = selectMode;
            }
        }

        /// <summary>
        /// Event handler for when selection changes in the active
        /// word document
        /// </summary>
        /// <param name="selection">the selected text</param>
        private void Application_WindowSelectionChange(Word.Selection selection)
        {
            if (_sync)
            {
                Log.Debug("sync is true returning");
                return;
            }

            Log.Debug("MsWord selection changed");

            triggerTextChanged(this);
        }

        /// <summary>
        /// Creates an instance of the Word application object
        /// </summary>
        private void createWordApplication()
        {
            if (_wordApp != null)
            {
                return;
            }

            Log.Debug();
            try
            {
                _wordApp = (Word.Application)Marshal.GetActiveObject("Word.Application");
                _wordApp.Application.WindowSelectionChange += Application_WindowSelectionChange;
                _wordNavigator.WordApp = _wordApp;
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
                _wordApp = null;
            }
        }

        /// <summary>
        /// Disposes the current instance of the Word applicaton object
        /// </summary>
        private void disposeWordApplication()
        {
            Log.Debug();
            try
            {
                if (_wordApp != null)
                {
                    _wordApp.Application.WindowSelectionChange -= Application_WindowSelectionChange;
                    _wordNavigator.WordApp = null;
                    Marshal.ReleaseComObject(_wordApp);
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }

            _wordApp = null;
        }

        /// <summary>
        /// Gets the current caret position. -1 on error
        /// </summary>
        /// <returns>caret position</returns>
        private int getCaretPos()
        {
            bool done = false;
            int caretPos = -1;
            int count = 0;

            while (!done)
            {
                createWordApplication();
                if (_wordApp == null)
                {
                    done = true;
                    continue;
                }

                try
                {
                    caretPos = _wordApp.Selection.Range.Start;
                    done = true;
                }
                catch (Exception ex)
                {
                    Log.Debug(ex.ToString());
                    caretPos = -1;
                    disposeWordApplication();
                    count++;
                    if (count > 3)
                    {
                        done = true;
                    }
                }
            }

            return caretPos;
        }

        /// <summary>
        /// Works backwards from the caret position and finds the index of the previous word,
        /// and also returns the size of the word
        /// </summary>
        /// <param name="startFrom">cursor position</param>
        /// <param name="offset">Offset of the previous word (0 if beginning reached)</param>
        /// <param name="count">size of the previous word</param>
        /// <returns></returns>
        private bool getPrevWordOffset(int startFrom, out int offset, out int count)
        {
            try
            {
                count = 0;
                offset = startFrom;

                Word.Range range = _wordApp.ActiveDocument.Range(startFrom - 1, startFrom);
                while (range != null)
                {
                    String text = range.Text;
                    if (String.IsNullOrEmpty(text))
                    {
                        break;
                    }

                    if (!Char.IsWhiteSpace(text[0]))
                    {
                        break;
                    }

                    count++;
                    range = range.Previous(Word.WdUnits.wdCharacter);
                }

                while (range != null)
                {
                    String text = range.Text;
                    if (String.IsNullOrEmpty(text))
                    {
                        break;
                    }

                    if (!TextUtils.IsTerminator(text[0]))
                    {
                        break;
                    }

                    count++;
                    range = range.Previous(Word.WdUnits.wdCharacter);
                }

                if (range != null)
                {
                    while (range != null)
                    {
                        String text = range.Text;
                        if (String.IsNullOrEmpty(text))
                        {
                            break;
                        }

                        if (Char.IsWhiteSpace(text[0]) || TextUtils.IsSentenceTerminator(text[0]))
                        {
                            break;
                        }

                        count++;
                        range = range.Previous(Word.WdUnits.wdCharacter);
                    }

                    if (range != null)
                    {
                        range = range.Next(Word.WdUnits.wdCharacter);
                    }
                }

                offset = range != null ? range.Start : 0;

                return true;
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
                offset = 0;
                count = 0;
                return false;
            }
        }

        /// <summary>
        /// Gets the sentence at the caret position and returns the
        /// starting offset of the sentence.
        /// </summary>
        /// <param name="sentence">the sentence text</param>
        /// <param name="sentenceOffset">offset</param>
        private void getSentenceAtCaret(out String sentence, out int sentenceOffset)
        {
            sentenceOffset = -1;
            sentence = String.Empty;

            Log.Debug();

            if (!isWordRunning())
            {
                return;
            }

            try
            {
                Word.Range sentenceRange = _wordApp.Selection.Range.Sentences[1];
                sentenceOffset = sentenceRange.Start;
                int selectionEnd = _wordApp.Selection.Range.End;
                if (sentenceOffset <= selectionEnd)
                {
                    String t = _wordApp.ActiveDocument.Range(sentenceOffset, selectionEnd).Text;
                    sentence = t;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
                disposeWordApplication();
            }

            Log.Debug("returning");
        }

        /// <summary>
        /// Checks if MS word is currently running on the computer
        /// </summary>
        /// <returns>true if it is</returns>
        private bool isWordRunning()
        {
            return getCaretPos() != -1;
        }

        /// <summary>
        /// Moves the caret by "howMuch" characters.  This is
        /// a tricky one. We can't just move the caret position as
        /// this causes problems if markups are enabled. This
        /// function does it the right way so it works even if
        /// markups are enabled.  howMuch is positive to move
        /// forward and negative to move backwords
        /// </summary>
        /// <param name="howMuch">how much to move</param>
        /// <returns>new caret position</returns>
        private int moveCaret(int howMuch)
        {
            Word.Range range = _wordApp.Selection.Range;

            if (howMuch > 0)
            {
                for (int ii = 0; ii < howMuch; ii++)
                {
                    Word.Range r = range.Next(Word.WdUnits.wdCharacter);
                    if (r != null)
                    {
                        range = r;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                for (int ii = 0; ii < howMuch; ii++)
                {
                    Word.Range r = range.Previous(Word.WdUnits.wdCharacter);
                    if (r != null)
                    {
                        range = r;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (range != null)
            {
                return range.Start;
            }

            return -1;
        }

        /// <summary>
        /// Raised to indicate that the current sentence can be
        /// added to the word prediction learning model
        /// </summary>
        private void MSWordTextControlAgent_EvtLearn()
        {
            learn();
        }
    }
}