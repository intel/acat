////////////////////////////////////////////////////////////////////////////
// <copyright file="WordListWidget.cs" company="Intel Corporation">
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
    public class WordListWidget : Widget
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="control">the inner .NET Control for the widget</param>
        public WordListWidget(Control control)
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
        public void ClearEntries(int start)
        {
            for (int jj = start; jj < _children.Count; jj++)
            {
                _children[jj].SetText(String.Empty);
                _children[jj].Value = String.Empty;
            }
        }
    }
}