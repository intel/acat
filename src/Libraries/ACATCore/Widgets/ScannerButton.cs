////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.ThemeManagement;
using ACAT.Lib.Core.Utility;
using System.Drawing;
using System.Windows.Forms;

namespace ACAT.Lib.Core.Widgets
{
    /// <summary>
    /// A widget that uses a ScannerButton as the UI control.
    /// </summary>
    public class ScannerButton : ScannerButtonBase
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="uiControl">the inner .NET Control for the widget</param>
        public ScannerButton(Control uiControl)
            : base(uiControl)
        {
            if (ThemeManager.Instance.ActiveTheme.Colors.Exists("ScannerButton"))
            {
                Colors = ThemeManager.Instance.ActiveTheme.Colors.GetColorScheme("ScannerButton");
            }

            //scaleBitmap(uiControl);
        }

        public override bool Enabled
        {
            get
            {
                return base.Enabled;
            }

            set
            {
                base.Enabled = value;
                if (UIControl != null)
                {
                    Windows.SetEnabled(UIControl, value);
                    //UIControl.Enabled = value;
                }
            }
        }
    }
}