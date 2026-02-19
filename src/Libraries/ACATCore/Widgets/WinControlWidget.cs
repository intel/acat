////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using System;
using System.Windows.Forms;

namespace ACAT.Core.Widgets
{
    /// <summary>
    /// A generic wrapper widget class for any windows control that
    /// doesn't require font scaling as the widget is scaled up or down
    /// </summary>
    public class WinControlWidget : Widget
    {
        public WinControlWidget(Control uiControl)
            : base(uiControl, LogManager.GetLogger<Widget>())
        {
        }

        public WinControlWidget(String widgetName)
            : base(widgetName)
        {
        }
    }
}