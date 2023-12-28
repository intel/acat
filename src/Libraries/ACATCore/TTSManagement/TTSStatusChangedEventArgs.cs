////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Lib.Core.TTSManagement
{
    /// <summary>
    /// Event args when the TTS engine status changes
    /// </summary>
    public class TTSStatusChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the TTSStatusChangedEventArgs class
        /// </summary>
        /// <param name="flags"></param>
        public TTSStatusChangedEventArgs(StatusFlags flags)
        {
            Status = flags;
        }

        /// <summary>
        /// Gets the TTS Engine status
        /// </summary>
        public StatusFlags Status { get; private set; }
    }
}