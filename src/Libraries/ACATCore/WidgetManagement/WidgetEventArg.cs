////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Lib.Core.WidgetManagement
{
    /// <summary>
    /// Event argument for widget events
    /// </summary>
    public class WidgetEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes an instance of the WidgetEventArgs class
        /// </summary>
        /// <param name="widget"></param>
        public WidgetEventArgs(Widget widget)
        {
            SourceWidget = widget;
        }

        /// <summary>
        /// Returns the widget that triggered the event
        /// </summary>
        public Widget SourceWidget { get; private set; }
    }
}