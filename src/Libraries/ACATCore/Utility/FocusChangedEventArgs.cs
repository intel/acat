////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Lib.Core.Utility
{
    /// <summary>
    /// Event args for when the focus changes in Windows.r Focus
    /// can change because active window changed, or within a window,
    /// focus changed to a different control
    /// </summary>
    public class FocusChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initialzies an instance of the class
        /// </summary>
        /// <param name="monitorInfo">Focused element info</param>
        public FocusChangedEventArgs(WindowActivityMonitorInfo monitorInfo)
        {
            WindowActivityInfo = monitorInfo;
        }

        /// <summary>
        /// Gets the focused element information
        /// </summary>
        public WindowActivityMonitorInfo WindowActivityInfo { get; private set; }
    }
}