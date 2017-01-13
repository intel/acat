////////////////////////////////////////////////////////////////////////////
// <copyright file="Pronunciation.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2017 Intel Corporation 
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
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