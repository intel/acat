////////////////////////////////////////////////////////////////////////////
// <copyright file="WordListItemWidget.cs" company="Intel Corporation">
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

using ACAT.Lib.Core.ThemeManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Windows.Forms;

namespace ACAT.Lib.Core.Widgets
{
    /// <summary>
    /// Represents a single word list item in the word prediction box
    /// typically used in the Alphabet scanners.
    /// </summary>
    public class WordListItemWidget : ScannerButtonBase
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="control">the inner .NET Control for the widget</param>
        public WordListItemWidget(Control control)
            : base(control)
        {
            if (button != null)
            {
                button.AutoEllipsis = true;
            }

            Colors = ThemeManager.Instance.ActiveTheme.Colors.GetColorScheme(ColorSchemes.WordListItemSchemeName);
        }

        /// <summary>
        /// Check if this needs to be added to the animation
        /// sequence.  If there is no text, no need include this
        /// widget in the scanning sequence.
        /// </summary>
        /// <returns>True if it's ok</returns>
        public override bool CanAddForAnimation()
        {
            String text = GetText().Trim();
            AddForAnimation = !String.IsNullOrEmpty(text);
            Log.Debug(" WordListItem " + Name + ", AddForAnimation: " + AddForAnimation + ", Text: [" + text + "]");
            return AddForAnimation;
        }
    }
}