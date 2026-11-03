////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using ACAT.Core.WidgetManagement.Interfaces;
using System.Windows.Forms;

namespace ACAT.Core.Widgets
{
    /// <summary>
    /// Represents a 'row' of widgets.  The Row is typically
    /// a TableLayoutPanel with one row and multiple columns.
    /// Each column will host a Button (or any other .NET control)
    /// </summary>
    public class RowWidget : Widget, IRowWidget
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="control">the inner .NET Control for the widget</param>
        public RowWidget(Control control)
            : base(control, LogManager.GetLogger<Widget>())
        {
        }

        /// <summary>
        /// Turns highlight off
        /// </summary>
        /// <returns>true</returns>
        protected override bool highlightOff()
        {
            base.highlightOff();

            Windows.SetRegion(UIControl, null);
            UIControl.Invalidate();
            return true;
        }

        /// <summary>
        /// Turns highlight on
        /// </summary>
        /// <returns>true</returns>
        protected override bool highlightOn()
        {
            base.highlightOn();

            UIControl.Region = null;
            UIControl.Invalidate();

            return true;
        }
    }
}