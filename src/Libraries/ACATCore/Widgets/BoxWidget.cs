////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.WidgetManagement;
using ACAT.Core.WidgetManagement.Interfaces;
using System.Windows.Forms;

namespace ACAT.Core.Widgets
{
    /// <summary>
    /// Encapsulates behavior of the Box UI element.  Examples
    /// </summary>
    public class BoxWidget : Widget, IBoxWidget
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="uiControl">the inner .NET Control for the widget</param>
        public BoxWidget(Control uiControl)
            : base(uiControl)
        {
        }
    }
}