////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Extensions;
using ACAT.Core.PreferencesManagement;
using ACAT.Core.TTSManagement.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace ACAT.Core.TTSManagement
{
    /// <summary>
    /// Interface for TTSManager to support dependency injection.
    /// Manages text to speech engines.
    /// </summary>
    public interface ITTSManager : IDisposable
    {
        /// <summary>
        /// Raised when the engine changes
        /// </summary>
        event TTSManager.EngineChanged EvtEngineChanged;

        /// <summary>
        /// Gets the currently active TTS engine
        /// </summary>
        ITTSEngine ActiveEngine { get; }

        /// <summary>
        /// Gets the list of TTS engines
        /// </summary>
        IEnumerable<IExtension> TTSEnginesList { get; }

        /// <summary>
        /// Returns the collection of discovered TTS Engine Types
        /// </summary>
        /// <returns>collection of types</returns>
        ICollection<Type> GetExtensions();

        /// <summary>
        /// Returns the current normalized volume
        /// </summary>
        /// <returns>volume level</returns>
        TTSValue GetNormalizedVolume();

        /// <summary>
        /// Initializes the TTS manager
        /// </summary>
        /// <param name="extensionDirs">Directories to search</param>
        /// <returns>true on success</returns>
        bool Init(IEnumerable<String> extensionDirs);

        /// <summary>
        /// Loads TTS Engine extensions from the specified directories
        /// </summary>
        /// <param name="extensionDirs">Directories to search</param>
        /// <returns>true on success</returns>
        bool LoadExtensions(IEnumerable<String> extensionDirs);

        /// <summary>
        /// Saves preferences in text-to-speech settings
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="preferencesCategories">preferences to save</param>
        void SavePreferences(object sender, IEnumerable<PreferencesCategory> preferencesCategories);

        /// <summary>
        /// Sets the active TTS engine for the specified culture
        /// </summary>
        /// <param name="ci">culture info, null for default</param>
        /// <returns>true on success</returns>
        bool SetActiveEngine(CultureInfo ci = null);

        /// <summary>
        /// Sets the normalized volume
        /// </summary>
        /// <param name="normalizedVolume">normalized volume level (0-9)</param>
        void SetNormalizedVolume(int normalizedVolume);

        /// <summary>
        /// Switch language to the specified one
        /// </summary>
        /// <param name="ci">culture to switch to</param>
        /// <returns>true on success</returns>
        bool SwitchLanguage(CultureInfo ci);
    }
}
