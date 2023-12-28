////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Globalization;

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Delegate for the event raised when the culture has changed
    /// </summary>
    /// <param name="sender">event sender</param>
    /// <param name="arg">event args</param>
    public delegate void CultureChanged(object sender, CultureChangedEventArg arg);

    /// <summary>
    /// Argument for the event raised when the culture (language) changes
    /// </summary>
    public class CultureChangedEventArg : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="panel">scanner being shown</param>
        public CultureChangedEventArg(CultureInfo cultureInfo)
        {
            Culture = cultureInfo;
        }

        /// <summary>
        /// Gets the culture
        /// </summary>
        public CultureInfo Culture { get; private set; }
    }
}