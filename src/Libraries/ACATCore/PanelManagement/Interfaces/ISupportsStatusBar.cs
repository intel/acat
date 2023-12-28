////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Interface to indicate whether the scanner
    /// supports a status bar.  The status bar
    /// has controls to indicate the current key
    /// pressed status of Shift, Alt, Ctrl, etc.
    /// The ScannerStatusBar class has a collection of
    /// UI controls to display the status of
    /// these keys.
    /// </summary>
    public interface ISupportsStatusBar
    {
        /// <summary>
        /// Gets the status bar
        /// </summary>
        ScannerStatusBar ScannerStatusBar { get; }
    }
}