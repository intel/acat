////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;
using System;

namespace ACAT.Lib.Core.TTSManagement
{
    /// <summary>
    /// Represents a single pronunciation.  A pronunciation
    /// contains the original word and a phonetic spelling that
    /// tells the speech engine how to pronounce it. This is useful
    /// where the TTS engine may not pronounce words correctly (eg
    /// proper nouns). This object maps the actual spelling with
    /// the phonetic spelling. The phonetically spelt word is the
    /// one sent to the TTS engine to convert to speech.
    /// </summary>
    public class Pronunciation : IDisposable
    {
        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Initializes an instance of this class
        /// </summary>
        /// <param name="word">original word</param>
        /// <param name="altPronunciation">phonetic spelling</param>
        public Pronunciation(String word, String altPronunciation)
        {
            Log.Debug("Entering...word=" + word + " altPronunciation=" + altPronunciation);
            Word = word;
            AltPronunciation = altPronunciation;
        }

        /// <summary>
        /// Gets or sets the alternate pronunciation
        /// </summary>
        public String AltPronunciation { get; set; }

        /// <summary>
        /// Gets or sets the original word
        /// </summary>
        public String Word { get; set; }

        /// <summary>
        /// Disposes the object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // Prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposer. Release resources and cleanup.
        /// </summary>
        /// <param name="disposing">true to dispose managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!_disposed)
            {
                if (disposing)
                {
                    // dispose all managed resources.
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }
    }
}