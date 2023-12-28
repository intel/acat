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
    public delegate void PanelPreShow(object sender, PanelPreShowEventArg arg);

    /// <summary>
    /// Argument for the event raised just before when the panel is shown
    /// </summary>
    public class PanelPreShowEventArg : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="panel">scanner being shown</param>
        public PanelPreShowEventArg(IPanel panel, DisplayModeTypes dislayMode)
        {
            Panel = panel;
            DisplayMode = dislayMode;
        }

        /// <summary>
        /// Gets DisplayMode for the panel
        /// </summary>
        public DisplayModeTypes DisplayMode { get; private set; }

        /// <summary>
        /// Gets the scanner object
        /// </summary>
        public IPanel Panel { get; internal set; }
    }
}