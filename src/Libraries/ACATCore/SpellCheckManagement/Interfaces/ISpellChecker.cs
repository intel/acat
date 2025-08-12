////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Utility;
using System;
using System.Globalization;

namespace ACAT.Core.SpellCheckManagement.Interfaces
{
    /// <summary>
    /// Interface to Spellcheckers
    /// </summary>
    public interface ISpellChecker : IDisposable
    {
        /// <summary>
        /// Returns a descriptor which contains a user readable name, a
        /// short textual description and a unique GUID.
        /// </summary>
        ClassDescriptorAttribute Descriptor { get; }

        /// <summary>
        /// Initialize the spell checker
        /// </summary>
        /// <param name="ci">Language for the spellchecker</param>
        /// <returns>true on success, false on failure</returns>
        bool Init(CultureInfo ci);

        /// <summary>
        /// Reset to factory default settings
        /// </summary>
        /// <returns>true on success, false on failure</returns>
        bool LoadDefaultSettings();

        /// <summary>
        /// Load settings from a file maintained by the word predictor.
        /// </summary>
        /// <param name="configFileDirectory">Directory where the settings are stored</param>
        /// <returns>true on success, false on failure</returns>
        bool LoadSettings(string configFileDirectory);

        string Lookup(string word);

        /// <summary>
        /// Save the word predictor settings to a file that is maintained
        /// by the word predictor.
        /// </summary>
        /// <param name="configFileDirectory">Directory where the settings are stored</param>
        /// <returns>true on success, false on failure</returns>
        bool SaveSettings(string configFileDirectory);

        /// <summary>
        /// Uninitializes
        /// </summary>
        void Uninit();
    }
}