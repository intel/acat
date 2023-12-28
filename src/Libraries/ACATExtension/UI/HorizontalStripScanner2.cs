////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using System;
using System.Diagnostics.CodeAnalysis;

namespace ACAT.Lib.Extension
{
    /// <summary>
    /// This is a scanner with a single row of buttons.Closes the
    /// scanner on button actuation
    /// </summary>
    [Descriptor("83FACD42-3678-4D14-9350-22B0E69D0D01",
                    "HorizontalStripScanner2",
                    "Horizontal strip of buttons, closes scanner when button is actuated")]
    public partial class HorizontalStripScanner2 : HorizontalStripScanner
    {
        /// <summary>
        /// Initalizes a new instance of the class
        /// </summary>
        public HorizontalStripScanner2()
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="panelClass">The panel class of the conextual menu</param>
        /// <param name="panelTitle">title of the contextual</param>
        public HorizontalStripScanner2(String panelClass, String panelTitle)
            : base(panelClass, panelTitle)
        {
        }

        /// <summary>
        /// Invoked when the user actuates a button in
        /// the scanner form
        /// </summary>
        /// <param name="widget">widget actuated</param>
        /// <param name="handled">was this handled here?</param>
        public override void OnWidgetActuated(WidgetActuatedEventArgs e, ref bool handled)
        {
            base.OnWidgetActuated(e, ref handled);
            Windows.CloseAsync(this);
        }
    }
}