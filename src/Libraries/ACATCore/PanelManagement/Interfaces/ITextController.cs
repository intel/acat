////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.AbbreviationsManagement;
using System;

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Interface used for text manipulation of text in
    /// the application window (notepad for eg), or a
    /// text box on a Windows form.
    /// </summary>
    public interface ITextController
    {
        /// <summary>
        /// Autocompletes a partially entered a word.
        /// This is typically done by word prediction
        /// </summary>
        /// <param name="partialWord"></param>
        void AutoCompleteWord(String partialWord);

        /// <summary>
        /// Check if an abbreviation was entered and if so,
        /// replace it
        /// </summary>
        /// <param name="handled">true if handled</param>
        /// <returns>the abbreviation object</returns>
        Abbreviation CheckAndReplaceAbbreviation(ref bool handled);

        /// <summary>
        /// Deletes the previous character
        /// </summary>
        void DeletePreviousChar();

        /// <summary>
        /// Deletes the previous word
        /// </summary>
        void DeletePreviousWord();

        /// <summary>
        /// Smart deletes the previous word.  For eg, if the
        /// last action was autocomplete word, should restore
        /// the previously entered partial word
        /// </summary>
        void SmartDeletePrevWord();

        /// <summary>
        /// Perform spell check on the last word entered.
        /// Invoked when an word is auto-completed or a whitespace
        /// character is inserted indicated the completion of
        /// a word
        /// </summary>
        void SpellCheck();

        /// <summary>
        /// Undoes the last edit change - auto-complete word,
        /// insert punctuation, type a letter etc.
        /// </summary>
        void UndoLastEditChange();

        /// <summary>
        /// Clear text in the text box
        /// </summary>
        void ClearText();
    }
}