////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Represents the Status Bar in the scanner form that
    /// will display the states of keys such as Ctrl, Alt, Shift.
    /// For instance, these could Label objects that displays whether
    /// Ctrl is currently pressed or not. Or they could be a StatusStrip
    /// labels.
    /// </summary>
    public class ScannerStatusBar
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ScannerStatusBar()
        {
            AltStatus = null;
            CtrlStatus = null;
            FuncStatus = null;
            ShiftStatus = null;
        }

        /// <summary>
        /// Gets or sets control that will display the state of
        /// the Alt key
        /// </summary>
        public object AltStatus { get; set; }

        /// <summary>
        /// Gets or sets control that will display the state of
        /// the Ctrl key
        /// </summary>
        public object CtrlStatus { get; set; }

        /// <summary>
        /// Gets or sets control that will display the state of
        /// the the Function keys
        /// </summary>
        public object FuncStatus { get; set; }

        /// <summary>
        /// Gets or sets control that will display the state of
        /// the Shift key
        /// </summary>
        public object ShiftStatus { get; set; }
    }
}