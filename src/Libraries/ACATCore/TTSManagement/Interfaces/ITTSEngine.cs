////////////////////////////////////////////////////////////////////////////
// <copyright file="ITTSEngine.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2015 Intel Corporation 
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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.Utility;

#region SupressStyleCopWarnings

[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1126:PrefixCallsCorrectly",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1101:PrefixLocalCallsWithThis",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1121:UseBuiltInTypeAlias",
        Scope = "namespace",
        Justification = "Since they are just aliases, it doesn't really matter")]
[module: SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1200:UsingDirectivesMustBePlacedWithinNamespace",
        Scope = "namespace",
        Justification = "ACAT guidelines")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1309:FieldNamesMustNotBeginWithUnderscore",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private fields begin with an underscore")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1300:ElementMustBeginWithUpperCaseLetter",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private/Protected methods begin with lowercase")]

#endregion SupressStyleCopWarnings

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
    /// Interface for the TTS Engine.
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
        /// Gets the descriptor for the TTS engine
        /// </summary>
        IDescriptor Descriptor { get; }

        /// <summary>
        /// Gets the current status of the speech engine
        /// </summary>
        StatusFlags Status { get; }

        /// <summary>
        /// Gets whether TTS engine has a dialog to allow the
        /// user to change settings
        /// </summary>
        bool SupportsSettingsDialog { get; }

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
        /// Gets the settings dialog
        /// </summary>
        /// <returns></returns>
        ITTSSettingsDialog GetSettingsDialog();

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
        bool Init();

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