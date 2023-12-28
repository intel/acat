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
    /// Event args for the event raised when the TTS voice changes
    /// </summary>
    public class TTSVoiceChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the TTSVoiceChangedEventArgs class
        /// </summary>
        /// <param name="voice"></param>
        public TTSVoiceChangedEventArgs(String voice)
        {
            Voice = voice;
        }

        /// <summary>
        /// Gets the voice name
        /// </summary>
        public string Voice { get; private set; }
    }
}