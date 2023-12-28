////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Lib.Core.Utility
{
    /// <summary>
    /// This class is used by scanners when they are closing.
    /// The Status is set to Closing or Closed in the event
    /// handlers for OnFormCLosing or FormClosed. This is used
    /// by timers or threads owned by the form to know that they
    /// have to quit. This is cleaner than having to use a
    /// volatile bool for instance.
    /// Can also be used to see if a window has closed.
    /// </summary>
    public class SyncLock
    {
        /// <summary>
        /// Initialzies an instance of the class
        /// </summary>
        public SyncLock()
        {
            Status = StatusValues.None;
        }

        /// <summary>
        /// Indicates the scanner status
        /// </summary>
        public enum StatusValues
        {
            None = 0,
            Closing = 1,
            Closed = 2
        }

        /// <summary>
        /// Gets or sets the status value
        /// </summary>
        public StatusValues Status { get; set; }

        /// <summary>
        /// Returns whether the window is in the process of closing
        /// </summary>
        /// <returns>true if it is</returns>
        public bool IsClosing()
        {
            return (Status == StatusValues.Closed) || (Status == StatusValues.Closing);
        }
    }
}