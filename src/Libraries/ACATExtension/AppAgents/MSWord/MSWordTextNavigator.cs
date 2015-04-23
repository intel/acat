////////////////////////////////////////////////////////////////////////////
// <copyright file="MSWordTextNavigator.cs" company="Intel Corporation">
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
    /// Handles all the navigation functions in the
    /// active word document - next/prev sentence, para,
    /// selecting text etc.
    /// Uses Word Interop to do the navigation.
    /// </summary>
    internal class MSWordTextNavigator
    {
        /// Initializes an instance of the object
        /// </summary>
        public MSWordTextNavigator()
        {
            SelectMode = false;
            MoveMode = SelectionMoveMode.None;
        }

        /// <summary>
        /// If select mode is on, are we moving the beginning
        /// of the selection or the end of the selection
        /// </summary>
        internal enum SelectionMoveMode
        {
            None,
            Beginning,
            End
        }

        /// <summary>
        /// Gets or sets the selection move mode
        /// </summary>
        public SelectionMoveMode MoveMode { get; set; }

        /// <summary>
        /// Gets or sets the current select mode. If true, as
        /// the caret moves, selects text in the window
        ///
        /// </summary>
        public bool SelectMode { get; set; }

        /// <summary>
        /// Gets or sets the MS Word interop application object
        /// </summary>
        public Word.Application WordApp { get; set; }

        /// <summary>
        /// Gets the paragraph after the one at the caret
        /// </summary>
        /// <param name="paragraph">the paragraph</param>
        public void GetParagraphAfterCaret(out String paragraph)
        {
            String retVal = String.Empty;
            Word.Range paraRange = WordApp.Selection.Range.Paragraphs[1].Range;
            if (paraRange != null)
            {
                Word.Range next = paraRange.Next(Word.WdUnits.wdParagraph);
                while (next != null)
                {
                    if (!String.IsNullOrEmpty(next.Text.Trim()))
                    {
                        retVal = next.Text;
                        break;
                    }

                    next = next.Next(Word.WdUnits.wdParagraph);
                }

                if (next == null)
                {
                    retVal = String.Empty;
                }
            }

            paragraph = retVal;
        }

        /// <summary>
        /// Returns the paragraph before the one at the
        /// caret position
        /// </summary>
        /// <param name="paragraph">the paragraph</param>
        public void GetParagraphBeforeCaret(out String paragraph)
        {
            String retVal = String.Empty;
            Word.Range paraRange = WordApp.Selection.Range.Paragraphs[1].Range;
            if (paraRange != null)
            {
                Word.Range prev = paraRange.Previous(Word.WdUnits.wdParagraph);
                while (prev != null)
                {
                    if (!String.IsNullOrEmpty(prev.Text.Trim()))
                    {
                        retVal = prev.Text;
                        break;
                    }

                    prev = prev.Next(Word.WdUnits.wdParagraph);
                }

                if (prev == null)
                {
                    retVal = String.Empty;
                }
            }

            paragraph = retVal;
        }

        /// <summary>
        /// Reterns selected text (if any)
        /// </summary>
        /// <returns>selected text</returns>
        public String GetSelectedText()
        {
            String retVal = String.Empty;
            if (WordApp != null)
            {
                int start = WordApp.Selection.Start;
                int end = WordApp.Selection.End;
                retVal = start == end ? String.Empty : WordApp.Selection.Text;
            }

            Log.Debug("returning" + retVal);
            return retVal;
        }

        /// <summary>
        /// Returns the sentence after the one at the caret position
        /// </summary>
        /// <param name="sentence">senctence</param>
        public void GetSentenceAfterCaret(out String sentence)
        {
            sentence = String.Empty;
            Word.Range sentenceRange = WordApp.Selection.Range.Sentences[1];
            if (sentenceRange != null)
            {
                Word.Range next = sentenceRange.Next(Word.WdUnits.wdSentence);
                if (next != null)
                {
                    sentence = next.Text;
                }
            }
        }

        /// <summary>
        /// Gets the sentence before the one at the caret
        /// </summary>
        /// <param name="sentence">sentence</param>
        public void GetSentenceBeforeCaret(out String sentence)
        {
            sentence = String.Empty;
            try
            {
                Word.Range sentenceRange = WordApp.Selection.Range.Sentences[1];
                if (sentenceRange != null)
                {
                    Word.Range prev = sentenceRange.Previous(Word.WdUnits.wdSentence);
                    if (prev != null)
                    {
                        sentence = prev.Text;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }
        }

        /// <summary>
        /// Naviagates to the paragraph after the one at
        /// the caret position.  If select mode is true,
        /// extends the selection to the next para
        /// </summary>
        /// <returns>true on success</returns>
        public bool GotoNextParagraph()
        {
            if (WordApp == null)
            {
                return false;
            }

            bool retVal = true;

            try
            {
                int selectionStart = WordApp.Selection.Range.Start;
                int selectionEnd = WordApp.Selection.Range.End;
                int nextParaStart;

                if (selectionStart != selectionEnd && SelectMode)
                {
                    if (MoveMode == SelectionMoveMode.Beginning)
                    {
                        setCaretPos(selectionStart);
                        nextParaStart = getStartOfNextPara();
                        if (nextParaStart >= 0)
                        {
                            if (nextParaStart <= selectionEnd)
                            {
                                selectText(nextParaStart, selectionEnd);
                            }
                            else
                            {
                                selectText(selectionEnd, nextParaStart);
                                MoveMode = SelectionMoveMode.End;
                            }
                        }
                    }
                    else
                    {
                        setCaretPos(selectionEnd);
                        nextParaStart = getStartOfNextPara();
                        if (nextParaStart >= 0)
                        {
                            setCaretPos(nextParaStart);
                            selectText(selectionStart, nextParaStart);
                        }

                        MoveMode = SelectionMoveMode.End;
                    }
                }
                else
                {
                    var caretPos = getCaretPos();
                    nextParaStart = getStartOfNextPara();

                    if (nextParaStart >= 0)
                    {
                        if (SelectMode || KeyStateTracker.IsShiftOn())
                        {
                            selectText(caretPos, nextParaStart);
                            MoveMode = SelectionMoveMode.End;
                            if (KeyStateTracker.IsShiftOn())
                            {
                                KeyStateTracker.KeyTriggered(Keys.End);
                            }
                        }
                        else
                        {
                            setCaretPos(nextParaStart);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Naviagates to the sentence after the one at
        /// the caret position.  If select mode is true,
        /// extends the selection to the next sentence
        /// </summary>
        /// <returns>true on success</returns>
        public bool GotoNextSentence()
        {
            if (WordApp == null)
            {
                return false;
            }

            bool retVal = true;

            try
            {
                int selectionStart = WordApp.Selection.Range.Start;
                int selectionEnd = WordApp.Selection.Range.End;

                Log.Debug("Start: " + selectionStart + ", end: " + selectionEnd);

                int nextSentenceStart;
                if (selectionStart != selectionEnd && SelectMode)
                {
                    if (MoveMode == SelectionMoveMode.Beginning)
                    {
                        setCaretPos(selectionStart);
                        nextSentenceStart = getStartOfNextSentence();
                        if (nextSentenceStart >= 0)
                        {
                            if (nextSentenceStart <= selectionEnd)
                            {
                                selectText(nextSentenceStart, selectionEnd);
                            }
                            else
                            {
                                selectText(selectionEnd, nextSentenceStart);
                                MoveMode = SelectionMoveMode.End;
                            }
                        }
                    }
                    else
                    {
                        setCaretPos(selectionEnd);
                        nextSentenceStart = getStartOfNextSentence();
                        if (nextSentenceStart >= 0)
                        {
                            setCaretPos(nextSentenceStart);
                            selectText(selectionStart, nextSentenceStart);
                        }
                        else
                        {
                            int lastPos = WordApp.ActiveDocument.Characters.Last.Start;
                            selectText(selectionStart, lastPos);
                        }

                        MoveMode = SelectionMoveMode.End;
                    }
                }
                else
                {
                    var caretPos = getCaretPos();
                    nextSentenceStart = getStartOfNextSentence();

                    if (nextSentenceStart >= 0)
                    {
                        if (SelectMode || KeyStateTracker.IsShiftOn())
                        {
                            MoveMode = SelectionMoveMode.End;
                            selectText(caretPos, nextSentenceStart);
                            if (KeyStateTracker.IsShiftOn())
                            {
                                KeyStateTracker.KeyTriggered(Keys.End);
                            }
                        }
                        else
                        {
                            setCaretPos(nextSentenceStart);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Naviagates to the paragraph before the one at
        /// the caret position.  If select mode is true,
        /// extends the selection to the previous para
        /// </summary>
        /// <returns>true on success</returns>
        public bool GotoPreviousParagraph()
        {
            if (WordApp == null)
            {
                return false;
            }

            bool retVal = true;

            try
            {
                int selectionStart = WordApp.Selection.Range.Start;
                int selectionEnd = WordApp.Selection.Range.End;

                int prevParaStart;

                if (selectionStart != selectionEnd && SelectMode)
                {
                    if (MoveMode == SelectionMoveMode.End)
                    {
                        setCaretPos(selectionEnd);
                        prevParaStart = getStartOfPrevPara();
                        if (prevParaStart >= 0)
                        {
                            if (prevParaStart <= selectionStart)
                            {
                                selectText(prevParaStart, selectionStart);
                                MoveMode = SelectionMoveMode.Beginning;
                            }
                            else
                            {
                                selectText(selectionStart, prevParaStart);
                            }
                        }
                    }
                    else
                    {
                        setCaretPos(selectionStart);
                        prevParaStart = getStartOfPrevPara();
                        if (prevParaStart >= 0)
                        {
                            setCaretPos(prevParaStart);
                            selectText(prevParaStart, selectionEnd);
                        }
                    }
                }
                else
                {
                    var caretPos = getCaretPos();
                    prevParaStart = getStartOfPrevPara();
                    Log.Debug("prevPara start is " + prevParaStart);
                    if (prevParaStart >= 0)
                    {
                        if (SelectMode || KeyStateTracker.IsShiftOn())
                        {
                            MoveMode = SelectionMoveMode.Beginning;
                        }

                        Log.Debug("Setting cursor to  " + prevParaStart);
                        selectText(prevParaStart, (SelectMode || KeyStateTracker.IsShiftOn()) ? caretPos : prevParaStart);
                        if (KeyStateTracker.IsShiftOn())
                        {
                            KeyStateTracker.KeyTriggered(Keys.Home);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Naviagates to the sentence before the one at
        /// the caret position.  If select mode is true,
        /// extends the selection to the previous sentence
        /// </summary>
        /// <returns>true on success</returns>
        public bool GotoPreviousSentence()
        {
            if (WordApp == null)
            {
                return false;
            }

            bool retVal = true;

            try
            {
                int selectionStart = WordApp.Selection.Range.Start;
                int selectionEnd = WordApp.Selection.Range.End;

                Log.Debug("Start: " + selectionStart + ", end: " + selectionEnd);

                if (selectionStart == 0)
                {
                    return false;
                }

                int prevSentenceStart;
                int caretPos;
                bool skipSentence = false;

                if (selectionStart != selectionEnd && SelectMode)
                {
                    caretPos = getCaretPos();
                    if (MoveMode == SelectionMoveMode.End)
                    {
                        setCaretPos(selectionEnd);
                        prevSentenceStart = getStartOfPrevSentence();

                        caretPos = getCaretPos();
                        if (caretPos - prevSentenceStart == 1)
                        {
                            skipSentence = true;
                        }

                        if (prevSentenceStart >= 0)
                        {
                            if (skipSentence)
                            {
                                prevSentenceStart = skipSentenceAt(prevSentenceStart);
                            }

                            if (prevSentenceStart <= selectionStart)
                            {
                                selectText(prevSentenceStart, selectionStart);
                                MoveMode = SelectionMoveMode.Beginning;
                            }
                            else
                            {
                                selectText(selectionStart, prevSentenceStart);
                            }
                        }
                    }
                    else
                    {
                        setCaretPos(selectionStart);
                        prevSentenceStart = getStartOfPrevSentence();

                        if (caretPos - prevSentenceStart == 1)
                        {
                            skipSentence = true;
                        }

                        if (prevSentenceStart >= 0)
                        {
                            if (skipSentence)
                            {
                                prevSentenceStart = skipSentenceAt(prevSentenceStart);
                            }

                            setCaretPos(prevSentenceStart);
                            selectText(prevSentenceStart, selectionEnd);
                        }
                    }
                }
                else
                {
                    caretPos = getCaretPos();
                    prevSentenceStart = getStartOfPrevSentence();
                    Log.Debug("prevSentence start is " + prevSentenceStart);
                    if (caretPos - prevSentenceStart == 1)
                    {
                        Log.Debug("Setting foo to true");
                        skipSentence = true;
                    }

                    if (prevSentenceStart >= 0)
                    {
                        if (skipSentence)
                        {
                            prevSentenceStart = skipSentenceAt(prevSentenceStart);
                        }

                        if (SelectMode || KeyStateTracker.IsShiftOn())
                        {
                            MoveMode = SelectionMoveMode.Beginning;
                            selectText(prevSentenceStart, caretPos);
                            if (KeyStateTracker.IsShiftOn())
                            {
                                KeyStateTracker.KeyTriggered(Keys.Home);
                            }
                        }
                        else
                        {
                            Log.Debug("Setting cursor to  " + prevSentenceStart);
                            setCaretPos(prevSentenceStart);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Returns true if any text is currently selected
        /// in the document
        /// </summary>
        /// <returns>true if selected</returns>
        public bool IsTextSelected()
        {
            if (WordApp != null)
            {
                int selectionStart = WordApp.Selection.Range.Start;
                int selectionEnd = WordApp.Selection.Range.End;

                return selectionStart != selectionEnd;
            }

            return false;
        }

        /// <summary>
        /// Selects the text of the paragraph next to the
        /// one at the caret position
        /// </summary>
        /// <returns>true on success</returns>
        public bool SelectNextParagraph()
        {
            bool retVal;

            if (WordApp != null)
            {
                unselectAll();
                retVal = GotoNextParagraph();
                if (retVal)
                {
                    retVal = SelectParagraphAtCaret();
                }

                scrollParagraphAtCaretIntoView();
            }
            else
            {
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Selects the text of the sentence next to the
        /// one at the caret position
        /// </summary>
        /// <returns>true on success</returns>
        public bool SelectNextSentence()
        {
            bool retVal = true;

            if (WordApp != null)
            {
                unselectAll();
                retVal = GotoNextSentence();
                if (retVal)
                {
                    retVal = SelectSentenceAtCaret();
                }

                scrollParagraphAtCaretIntoView();
            }

            return retVal;
        }

        /// <summary>
        /// Selects the text of the paragraph at the  caret position
        /// </summary>
        /// <returns>true on success</returns>
        public bool SelectParagraphAtCaret()
        {
            bool retVal = true;

            if (WordApp != null)
            {
                int start = WordApp.Selection.Range.Paragraphs[1].Range.Start;
                int end = WordApp.Selection.Range.Paragraphs[1].Range.End;
                selectText(start, end);
            }
            else
            {
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Selects the text of the paragraph previous to the
        /// one at the caret position
        /// </summary>
        /// <returns>true on success</returns>
        public bool SelectPreviousParagraph()
        {
            bool retVal;

            if (WordApp != null)
            {
                unselectAll();
                retVal = GotoPreviousParagraph();
                if (retVal)
                {
                    retVal = SelectParagraphAtCaret();
                }

                scrollParagraphAtCaretIntoView();
            }
            else
            {
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Selects the text of the sentence before the
        /// one at the caret position
        /// </summary>
        /// <returns>true on success</returns>
        public bool SelectPreviousSentence()
        {
            bool retVal = true;

            if (WordApp != null)
            {
                unselectAll();
                retVal = GotoPreviousSentence();
                if (retVal)
                {
                    retVal = SelectSentenceAtCaret();
                }

                scrollParagraphAtCaretIntoView();
            }

            return retVal;
        }

        /// <summary>
        /// Selects the text of the paragraph next to the
        /// one at the caret position
        /// </summary>
        /// <returns>true on success</returns>
        public bool SelectSentenceAtCaret()
        {
            bool retVal = true;

            if (WordApp != null)
            {
                unselectAll();
                int start = WordApp.Selection.Range.Sentences[1].Start;
                int end = WordApp.Selection.Range.Sentences[1].End;
                selectText(start, end);
            }
            else
            {
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Returns the current caret position
        /// </summary>
        /// <returns>caret position, -1 on error</returns>
        private int getCaretPos()
        {
            int retVal = -1;
            if (WordApp != null)
            {
                try
                {
                    retVal = WordApp.Selection.Range.Start;
                }
                catch (Exception ex)
                {
                    Log.Debug(ex.ToString());
                }
            }

            return retVal;
        }

        /// <summary>
        /// Returns the starting index of the next paragraph
        /// </summary>
        /// <returns>starting index, -1 on error</returns>
        private int getStartOfNextPara()
        {
            if (WordApp == null)
            {
                return -1;
            }

            Word.Paragraph para = WordApp.Selection.Range.Paragraphs[1];
            int start = para.Range.Start;
            int prevStart = start;
            bool flag = false;
            int iteration = 0;
            bool breakOnFirst = false;

            while (true)
            {
                Word.Paragraph next = para.Next();
                if (next == null)
                {
                    break;
                }

                start = next.Range.Start;

                Log.Debug("prevStart: " + prevStart + ", start: " + start);

                if (start - prevStart == 1)
                {
                    if (iteration == 0)
                    {
                        breakOnFirst = true;
                    }
                }
                else if (start - prevStart > 1)
                {
                    if (breakOnFirst || flag)
                    {
                        return prevStart;
                    }

                    flag = true;
                }

                para = next;
                prevStart = start;
                iteration++;
            }

            return -1;
        }

        /// <summary>
        /// Returns the offset of the start of the next sentence
        /// </summary>
        /// <returns>offset, -1 on error</returns>
        private int getStartOfNextSentence()
        {
            if (WordApp != null)
            {
                Word.Range sentenceRange = WordApp.Selection.Range.Sentences[1];
                Word.Range nextSentence = sentenceRange.Next();
                if (nextSentence != null)
                {
                    int start = nextSentence.Start;
                    return start;
                }
            }

            return -1;
        }

        /// <summary>
        /// Returns the offset of the starting of the
        /// previous paragraph
        /// </summary>
        /// <returns>the offset, -1 on error</returns>
        private int getStartOfPrevPara()
        {
            if (WordApp == null)
            {
                return -1;
            }

            int caretPos = getCaretPos();
            int paraStart = WordApp.Selection.Range.Paragraphs[1].Range.Start;

            Log.Debug("caretPos: " + caretPos + ", paraStart = " + paraStart);

            if (caretPos != paraStart)
            {
                return paraStart;
            }

            Word.Paragraph para = WordApp.Selection.Range.Paragraphs[1];
            int start = para.Range.Start;
            int prevStart = start;
            bool flag = false;
            int iteration = 0;
            bool breakOnFirst = false;

            while (true)
            {
                Word.Paragraph prev = para.Previous();
                if (prev == null)
                {
                    break;
                }

                start = prev.Range.Start;

                Log.Debug("start: " + start + ", prevStart: " + prevStart);

                if (prevStart - start == 1)
                {
                    if (iteration == 0)
                    {
                        breakOnFirst = true;
                    }
                }
                else if (prevStart - start > 1)
                {
                    if (breakOnFirst || flag)
                    {
                        return start;
                    }

                    flag = true;
                }

                para = prev;
                prevStart = start;
                iteration++;
            }

            return -1;
        }

        /// <summary>
        /// Returns the offset of the start of the previous sentence
        /// </summary>
        /// <returns>offset, -1 on error</returns>
        private int getStartOfPrevSentence()
        {
            int caretPos = WordApp.Selection.Range.Start;
            int sentenceStart = WordApp.Selection.Range.Sentences[1].Start;
            Log.Debug("caretPos: " + caretPos + ", current sentenceStart = " + sentenceStart);
            if (caretPos != sentenceStart)
            {
                return sentenceStart;
            }

            Word.Range sentenceRange = WordApp.Selection.Range.Sentences[1];
            Word.Range prevSentence = sentenceRange.Previous();
            if (prevSentence != null)
            {
                return prevSentence.Start;
            }

            return -1;
        }

        /// <summary>
        /// If the paragraph at caret is out of view, scroll
        /// it into view
        /// </summary>
        private void scrollParagraphAtCaretIntoView()
        {
            if (WordApp != null)
            {
                WordApp.ActiveDocument.Windows[1].ScrollIntoView(WordApp.Selection.Range.Paragraphs[1].Range, true);
            }
        }

        /// <summary>
        /// Selects text between 'start' and 'end' offsets
        /// </summary>
        /// <param name="start">starting offset</param>
        /// <param name="end">ending offset</param>
        private void selectText(int start, int end)
        {
            if (WordApp != null && start <= end)
            {
                Log.Debug("select text [" + start + ", " + end + "]");
                WordApp.ActiveDocument.Range(start, end).Select();
            }
        }

        /// <summary>
        /// Sets the caret position to the indicated
        /// position
        /// </summary>
        /// <param name="pos">where to set it to?</param>
        /// <returns>true on success</returns>
        private bool setCaretPos(int pos)
        {
            bool retVal = true;

            Log.Debug();

            if (WordApp != null)
            {
                try
                {
                    WordApp.ActiveDocument.Range(pos, pos).Select();
                }
                catch (Exception ex)
                {
                    Log.Debug(ex.ToString());
                    retVal = false;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Returns the offset of the sentence after the
        /// one at the 'offset' position.
        /// </summary>
        /// <param name="offset">offset</param>
        /// <returns>starting offset of the sentence</returns>
        private int skipSentenceAt(int offset)
        {
            setCaretPos(offset);
            return getStartOfPrevSentence();
        }

        /// <summary>
        /// Unselects any text that is currently selected
        /// </summary>
        private void unselectAll()
        {
            int caretPos = getCaretPos();
            if (caretPos >= 0)
            {
                setCaretPos(caretPos);
            }
        }
    }
}