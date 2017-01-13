////////////////////////////////////////////////////////////////////////////
// <copyright file="ITextController.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2017 Intel Corporation 
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
    }
}