////////////////////////////////////////////////////////////////////////////
// <copyright file="ContextMenuText.cs" company="Intel Corporation">
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