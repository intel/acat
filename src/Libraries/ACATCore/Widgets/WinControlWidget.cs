////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.WidgetManagement;
using System;
using System.Windows.Forms;

namespace ACAT.Lib.Core.Widgets
{
    /// <summary>
    /// A generic wrapper widget class for any windows control that
    /// doesn't require font scaling as the widget is scaled up or down
    /// </summary>
    public class WinControlWidget : Widget
    {
        public WinControlWidget(Control uiControl)
            : base(uiControl)
        {
        }

        public WinControlWidget(String widgetName)
            : base(widgetName)
        {
        }
    }
}