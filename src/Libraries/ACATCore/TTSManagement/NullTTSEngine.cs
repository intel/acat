////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace ACAT.Lib.Core.TTSManagement
{
    /// <summary>
    /// Represents a 'no-op' text-to-speech engine.  Has no TTS
    /// functionality. Useful if there is no active TTS engine.
    /// </summary>
    [DescriptorAttribute("A98DA439-A6A9-48EF-AC8D-3D3588363341",
                        "Null Text-to-speech Engine",
                        "Text-to-speech disabled")]
    public class NullTTSEngine : ExtensionInvoker, ITTSEngine
    {
        private static int _nextBookmark = 1;

        private readonly StatusFlags _currentStatus;

        /// <summary>
        /// Pitch of the voice
        /// </summary>
        private readonly TTSValue _speechPitch = new TTSValue(0, 0, 0);

        /// <summary>
        /// Speech rate
        /// </summary>
        private readonly TTSValue _speechRate = new TTSValue(0, 0, 0);

        /// <summary>
        /// Speech volume
        /// </summary>
        private readonly TTSValue _speechVolume = new TTSValue(0, 0, 0);

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        private bool _muted;

        /// <summary>
        /// Constructor
        /// </summary>
        public NullTTSEngine()
        {
            _currentStatus = StatusFlags.None;
            EvtVoiceChanged = null;
            EvtStatusChanged = null;
        }

#pragma warning disable

        /// <summary>
        /// Triggered when bookmark reached after async text to speech
        /// </summary>
        public event TTSBookmarkReached EvtBookmarkReached;

        /// <summary>
        /// Raised whenever one of the speech properties changes
        /// </summary>
        public event EventHandler EvtPropertyChanged;

        /// <summary>
        /// Triggered when the status of the speech engine changes
        /// </summary>
        public event TTSStatusChanged EvtStatusChanged;

        /// <summary>
        /// Triggered when voice is changed
        /// </summary>
        public event TTSVoiceChanged EvtVoiceChanged;

#pragma warning enable

        /// <summary>
        /// Gets or sets the culture info for the voice to use
        /// </summary>
        public CultureInfo Culture { get; set; }

        /// <summary>
        /// Gets the ACAT descriptor for this engine
        /// </summary>
        public IDescriptor Descriptor
        {
            get { return DescriptorAttribute.GetDescriptor(GetType()); }
        }

        /// <summary>
        /// Returns the current status of the speech engine
        /// </summary>
        public StatusFlags Status
        {
            get { return _currentStatus; }
        }

        /// <summary>
        /// Get/sets whether alternate pronunciations should be used or not
        /// before TTS
        /// </summary>
        public bool UseAlternatePronunciations { get; set; }

        /// <summary>
        /// Gets or sets the voice to use
        /// </summary>
        public String Voice
        {
            get { return String.Empty; }

            set { }
        }

        /// <summary>
        /// Disposes resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // Prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Pitch of speech.  Value is engine-dependent
        /// </summary>
        public TTSValue GetPitch()
        {
            return _speechPitch;
        }

        /// <summary>
        /// Rate of speech.  Value is engine-dependent
        /// </summary>
        public TTSValue GetRate()
        {
            return _speechRate;
        }

        /// <summary>
        /// Returns a list of voices supported by the speech engine
        /// </summary>
        /// <returns>List of names of vlices</returns>
        public List<String> GetVoices()
        {
            return new List<String>();
        }

        /// <summary>
        /// Volume of speech. Value is engine-dependent
        /// </summary>
        public TTSValue GetVolume()
        {
            return _speechVolume;
        }

        /// <summary>
        /// Call this first.  Initializes the speech engine
        /// </summary>
        /// <returns></returns>
        public bool Init(CultureInfo ci)
        {
            return true;
        }

        /// <summary>
        /// Is engine muted
        /// </summary>
        /// <returns>mute state</returns>
        public bool IsMuted()
        {
            return _muted;
        }

        /// <summary>
        /// Mute the engine
        /// </summary>
        public void Mute()
        {
            _muted = true;
        }

        /// <summary>
        /// Pauses the speech
        /// </summary>
        /// <returns>true on success, false on failure</returns>
        public bool Pause()
        {
            return true;
        }

        /// <summary>
        /// Restores default settings
        /// </summary>
        public void RestoreDefaults()
        {
            SetVolume(0);
            SetPitch(0);
            SetRate(0);
        }

        /// <summary>
        /// Resumes paused speech
        /// </summary>
        /// <returns>true on success, false on failure</returns>
        public bool Resume()
        {
            return true;
        }

        /// <summary>
        /// Save settings
        /// </summary>
        /// <returns>true</returns>
        public bool Save()
        {
            return true;
        }

        /// <summary>
        /// Sets pitch value
        /// </summary>
        /// <param name="pitch">pitch value</param>
        /// <returns>true</returns>
        public bool SetPitch(int pitch)
        {
            _speechPitch.Value = pitch;

            notifyPropertyChanged();

            return true;
        }

        /// <summary>
        /// Sets rate of speech
        /// </summary>
        /// <param name="rate">value</param>
        /// <returns>true</returns>
        public bool SetRate(int rate)
        {
            _speechRate.Value = rate;

            notifyPropertyChanged();

            return true;
        }

        /// <summary>
        /// Sets volume level
        /// </summary>
        /// <param name="volume">level</param>
        /// <returns>true</returns>
        public bool SetVolume(int volume)
        {
            _speechVolume.Value = volume;

            notifyPropertyChanged();

            return true;
        }

        /// <summary>
        /// Convert the string to speech
        /// </summary>
        /// <param name="text">Text to convert.  Can have multiple sentences</param>
        /// <returns>true</returns>
        public bool Speak(String text)
        {
            return true;
        }

        /// <summary>
        /// Perform TTS asynchronously
        /// </summary>
        /// <param name="text">text</param>
        /// <param name="bookmark">bookmark</param>
        /// <returns>true</returns>
        public bool SpeakAsync(String text, out int bookmark)
        {
            bookmark = _nextBookmark++;
            return true;
        }

        /// <summary>
        /// Convert the SSML string to speech
        /// </summary>
        /// <param name="ssml">Text to convert (SSML format)</param>
        /// <returns>true</returns>
        public bool SpeakSsml(String ssml, String text, String ttsPlaceHolder)
        {
            return true;
        }

        /// <summary>
        /// Convert the SSML String to speech asynchronously
        /// </summary>
        /// <param name="ssml">String to convert (SSML format)</param>
        /// <param name="bookmark">returns bookmark</param>
        /// <returns>true</returns>
        public bool SpeakSsmlAsync(String ssml, String text, String ttsPlaceHolder, out int bookmark)
        {
            bookmark = _nextBookmark++;
            return true;
        }

        /// <summary>
        /// Stops the speech. All items are
        /// </summary>
        /// <returns>true on success, false on failure</returns>
        public bool Stop()
        {
            return true;
        }

        /// <summary>
        /// Unmute the engine
        /// </summary>
        public void UnMute()
        {
            _muted = false;
        }

        /// <summary>
        /// Waits until speech finishes or timeout expires
        /// </summary>
        /// <param name="timeout">Timeout to wait in milliseconds</param>
        /// <returns>true on success, false on failure</returns>
        public bool WaitUntilDone(int timeout)
        {
            return true;
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
                Log.Debug();

                if (disposing)
                {
                    // dispose all managed resources.
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        /// Notifies subscribers that some property changed
        /// in the engine
        /// </summary>
        private void notifyPropertyChanged()
        {
            if (EvtPropertyChanged != null)
            {
                EvtPropertyChanged(this, new EventArgs());
            }
        }
    }
}