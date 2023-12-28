////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
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
            AddForAnimation = !String.IsNullOrEmpty(text) && text != ". . .";
            Log.Debug("WordListItemWidget" + Name + ", AddForAnimation: " + AddForAnimation + ", Text: [" + text + "]");
            return AddForAnimation;
        }
    }
}