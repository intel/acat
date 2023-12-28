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
    /// Represents the predicted word list widget that contains
    /// a list of WordListItem widgets (eg the Alphabet scanner).
    /// Assumes that it has an array of WordListItem entries as
    /// children
    /// </summary>
    public class SentenceListWidget : Widget
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="control">the inner .NET Control for the widget</param>
        public SentenceListWidget(Control control)
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
        /// this if the word list has fewer words than the max. The
        /// remaining entires in the word list should be cleared.
        /// </summary>
        /// <param name="start">starting index</param>
        public void ClearEntries(int start, bool ellipses = false)
        {
            for (int jj = start; jj < _children.Count; jj++)
            {
                if (_children[jj] is SentenceListItemWidget)
                {
                    _children[jj].SetText(ellipses ? ". . ." : String.Empty);
                    _children[jj].Value = String.Empty;
                }
            }
        }

        public void ClearEntriesWithEllipses()
        {
            ClearEntries(0, true);
        }
    }
}