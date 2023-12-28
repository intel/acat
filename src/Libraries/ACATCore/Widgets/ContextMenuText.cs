////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.ThemeManagement;
using System.Windows.Forms;

namespace ACAT.Lib.Core.Widgets
{
    /// <summary>
    /// Represents the text in a contextual menu
    /// </summary>
    public class ContextMenuText : ScannerButtonBase
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="uiControl">the inner .NET Control for the widget</param>
        public ContextMenuText(Control uiControl)
            : base(uiControl)
        {
            Colors = ThemeManager.Instance.ActiveTheme.Colors.GetColorScheme(ColorSchemes.ContextMenuTextSchemeName);
            DisabledButtonColors = ThemeManager.Instance.ActiveTheme.Colors.GetColorScheme(ColorSchemes.DisabledContextMenuTextSchemeName);
        }
    }
}