////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace ACAT.Lib.Core.TTSManagement
{
    /// <summary>
    /// Indicates a bookmark has been reached in the text-to-speech
    /// conversion process.  Bookmarks indicates how far the speech
    /// engine has progressed in the conversion.
    /// </summary>
    /// <param name="sender">event sender</param>
    /// <param name="e">event args</param>
    public delegate void TTSBookmarkReached(object sender, TTSBookmarkReachedEventArgs e);

    /// <summary>
    /// Indicates the TTS engine status has changed
    /// </summary>
    /// <param name="sender">event sender</param>
    /// <param name="e">event args</param>
    public delegate void TTSStatusChanged(object sender, TTSStatusChangedEventArgs e);

    /// <summary>
    /// Indicates the speech voice has changed
    /// </summary>
    /// <param name="sender">event sender</param>
    /// <param name="e">event args</param>
    public delegate void TTSVoiceChanged(object sender, TTSVoiceChangedEventArgs e);

    [Flags]
    public enum SpeakFlags
    {
        /// <summary>
        /// Default is Synchronous speech, PurgeAllBeforeSpeaking
        /// </summary>
        Default = 0,

        /// <summary>
        /// Speaks asynchronously.  Function returns immediately
        /// </summary>
        Asynchronous = 1,

        /// <summary>
        /// Speak the string NOW.  If there are multiple conversations enqueued, this
        /// one precedes all of them.  Useful for alerts, error messages, etc.
        /// </summary>
        Priority = 2,

        /// <summary>
        /// Purge queue of all items before speaking.
        /// </summary>
        PurgeAllBeforeSpeaking = 4
    }

    /// <summary>
    /// Status of the speech engine
    /// </summary>
    [Flags]
    public enum StatusFlags
    {
        None = 0,
        Speaking = 1,
        Paused = 2,
        Stopped = 4
    }

    /// <summary>
    /// Interface for the TTS Engine.  All TTS Engines must implement
    /// this interface.
    /// </summary>
    public interface ITTSEngine : IDisposable, IExtension
    {
        /// <summary>
        /// Raised when bookmark reached after async text to speech
        /// </summary>
        event TTSBookmarkReached EvtBookmarkReached;

        /// <summary>
        /// Raised when a property changes
        /// </summary>
        event EventHandler EvtPropertyChanged;

        /// <summary>
        /// Raised when the status of the speech engine changes
        /// </summary>
        event TTSStatusChanged EvtStatusChanged;

        /// <summary>
        /// Raised when voice is changed
        /// </summary>
        event TTSVoiceChanged EvtVoiceChanged;

        /// <summary>
        /// Gets or sets the culture info for the voice to use
        /// </summary>
        CultureInfo Culture { get; set; }

        /// <summary>
        /// Gets the current status of the speech engine
        /// </summary>
        StatusFlags Status { get; }

        /// <summary>
        /// Get/sets whether alternate pronunciations should be used or not
        /// before TTS
        /// </summary>
        bool UseAlternatePronunciations { get; set; }

        /// <summary>
        /// Gets or sets the voice to use
        /// </summary>
        String Voice { get; set; }

        /// <summary>
        /// Pitch of speech. Value range depends on the
        /// speech engine
        /// </summary>
        TTSValue GetPitch();

        /// <summary>
        /// Rate of speech.  Value is engine-dependent
        /// </summary>
        TTSValue GetRate();

        /// <summary>
        /// Returns a list of voices supported by the speech engine
        /// </summary>
        /// <returns>List of names of vlices</returns>
        List<String> GetVoices();

        /// <summary>
        /// Volume of speech. Value is engine-dependent
        /// </summary>
        TTSValue GetVolume();

        /// <summary>
        /// Initializes the speech engine
        /// </summary>
        /// <returns></returns>
        bool Init(CultureInfo ci);

        /// <summary>
        /// Indicates whether the engine is muted
        /// </summary>
        /// <returns>true if it is</returns>
        bool IsMuted();

        /// <summary>
        /// Mutes the engine.
        /// </summary>
        void Mute();

        /// <summary>
        /// Pauses the speech
        /// </summary>
        /// <returns>true on success, false on failure</returns>
        bool Pause();

        /// <summary>
        /// Restores default settings
        /// </summary>
        void RestoreDefaults();

        /// <summary>
        /// Resumes paused speech
        /// </summary>
        /// <returns>true on success, false on failure</returns>
        bool Resume();

        /// <summary>
        /// Saves the current settings
        /// </summary>
        /// <returns>true on success</returns>
        bool Save();

        /// <summary>
        /// Sets the pitch. Value range depends on the
        /// speech engine
        /// </summary>
        /// <param name="pitch">the pitch value</param>
        /// <returns>true on success</returns>
        bool SetPitch(int pitch);

        /// <summary>
        /// Sets the rate of speech.  Value range depends on the
        /// speech engine
        /// </summary>
        /// <param name="rate">the rate</param>
        /// <returns>true on success</returns>
        bool SetRate(int rate);

        /// <summary>
        /// Sets the volume.  Value range depends on the
        /// speech engine
        /// </summary>
        /// <param name="volume">the volume level</param>
        /// <returns>true on success</returns>
        bool SetVolume(int volume);

        /// <summary>
        /// Converts text to speech
        /// </summary>
        /// <param name="text">Text to convert</param>
        /// <returns>true on success, false on error</returns>
        bool Speak(String text);

        /// <summary>
        /// Converts TTS asynchronously.  Bookmark is sent back as an
        /// event arg when speech completes
        /// </summary>
        /// <param name="text">Text to convert</param>
        /// <param name="bookmark">Bookmark</param>
        /// <returns></returns>
        bool SpeakAsync(String text, out int bookmark);

        /// <summary>
        /// Converts text to speech with text in the SSML format
        /// </summary>
        /// <param name="ssml">SSML formatted text</param>
        /// <returns>true on success, false on error</returns>
        bool SpeakSsml(String ssml, String text, String ttsPlaceHolder);

        /// <summary>
        /// Converts TTS asynchronously.  Bookmark is sent back as an
        /// event arg when speech completes. Text to speak is in the SSML format
        /// </summary>
        /// <param name="ssml">Text to convert (SSML format)</param>
        /// <param name="bookmark">Bookmark</param>
        /// <returns></returns>
        bool SpeakSsmlAsync(String ssml, String text, String ttsPlaceHolder, out int bookmark);

        /// <summary>
        /// Stops the speech. All items are
        /// </summary>
        /// <returns>true on success, false on failure</returns>
        bool Stop();

        /// <summary>
        /// Unmutes the engine
        /// </summary>
        void UnMute();

        /// <summary>
        /// Waits until speech finishes or timeout expires
        /// </summary>
        /// <param name="timeout">Timeout to wait in milliseconds</param>
        /// <returns>true on success, false on failure</returns>
        bool WaitUntilDone(int timeout);
    }
}