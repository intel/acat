////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.WidgetManagement;
using System;
using System.Windows.Forms;

namespace ACAT.Lib.Core.Widgets
{
    /// <summary>
    /// Represents the predicted letter list widget that contains
    /// a list of LetterListItem widgets (eg the Alphabet scanner).
    /// Assumes that it has an array of WordListItem entries as
    /// children
    /// </summary>
    public class LetterListWidget : Widget
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="control">the inner .NET Control for the widget</param>
        public LetterListWidget(Control control)
            : base(control)
        {
            AddForAnimation = false;
        }

        ///<summary>
        /// Clears the text in all child widgets
        /// </summary>
        public void ClearEntries()
        {
            ClearEntries(0);
        }

        /// <summary>
        /// Clears the text the child widgets starting at the index. Use
        /// this if the letter list has fewer letters than the max. The
        /// remaining entires in the word list should be cleared.
        /// </summary>
        /// <param name="start">starting index</param>
        public void ClearEntries(int start)
        {
            for (int jj = start; jj < _children.Count; jj++)
            {
                if (_children[jj] is WordListItemWidget)
                {
                    _children[jj].SetText(String.Empty);
                    _children[jj].Value = String.Empty;
                }
            }
        }
    }
}