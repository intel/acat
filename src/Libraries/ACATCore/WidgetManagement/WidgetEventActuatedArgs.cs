////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Lib.Core.WidgetManagement
{
    /// <summary>
    /// Triggered when a widget is actuated
    /// </summary>
    public class WidgetActuatedEventArgs : WidgetEventArgs
    {
        /// <summary>
        /// Initializes an instance of the WidgetEventActuatedArgs class
        /// </summary>
        /// <param name="widget">the source widget</param>
        public WidgetActuatedEventArgs(Widget widget, bool repeatActuate = false) : base(widget)
        {
            RepeatActuate = repeatActuate;
        }

        public bool RepeatActuate { get; set; }
    }
}