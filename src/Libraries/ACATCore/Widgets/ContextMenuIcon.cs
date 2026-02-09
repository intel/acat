////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.ThemeManagement;
using ACAT.Core.Utility;
using System.Windows.Forms;

namespace ACAT.Core.Widgets
{
    /// <summary>
    /// The icon widget that is used for menu items in the contextual menu
    /// </summary>
    public class ContextMenuIcon : ScannerButtonBase
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="uiControl">the inner .NET Control for the widget</param>
        public ContextMenuIcon(Control uiControl)
            : base(uiControl, LoggingConfiguration.CreateLogger<ScannerButtonBase>())
        {
            Colors = ThemeManager.Instance.ActiveTheme.Colors.GetColorScheme(ColorSchemes.ContextMenuIconSchemeName);
            DisabledButtonColors = ThemeManager.Instance.ActiveTheme.Colors.GetColorScheme(ColorSchemes.DisabledContextMenuIconSchemeName);
        }
    }
}