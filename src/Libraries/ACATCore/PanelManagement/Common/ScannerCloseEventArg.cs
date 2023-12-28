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
    /// Delegate for the event raised when the scanner is closed.
    /// </summary>
    /// <param name="sender">event sender</param>
    /// <param name="arg">event args</param>
    public delegate void ScannerClose(object sender, ScannerCloseEventArg arg);

    /// <summary>
    /// Argument for the event raised when the scanner is closed
    /// </summary>
    public class ScannerCloseEventArg : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="panel">scanner being shown</param>
        public ScannerCloseEventArg(IPanel panel)
        {
            Scanner = panel;
        }

        /// <summary>
        /// Gets the scanner object
        /// </summary>
        public IPanel Scanner { get; internal set; }
    }
}