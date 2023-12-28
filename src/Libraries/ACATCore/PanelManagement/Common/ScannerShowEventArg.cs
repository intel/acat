////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Delegate for the event raised when the
    /// scanner is displayed
    /// </summary>
    /// <param name="sender">event sender</param>
    /// <param name="arg">event args</param>
    public delegate void ScannerShow(object sender, ScannerShowEventArg arg);

    /// <summary>
    /// Argument for the event raised when the scanner is shown
    /// </summary>
    public class ScannerShowEventArg : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="panel">scanner being shown</param>
        public ScannerShowEventArg(IScannerPanel panel)
        {
            Scanner = panel;
        }

        /// <summary>
        /// Gets the scanner object
        /// </summary>
        public IScannerPanel Scanner { get; internal set; }
    }
}