////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.SpellCheckManagement.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace ACAT.Core.SpellCheckManagement
{
    /// <summary>
    /// Interface for SpellCheckManager to support dependency injection.
    /// Manages SpellChecker engines.
    /// </summary>
    public interface ISpellCheckManager : IDisposable
    {
        /// <summary>
        /// Gets the currently active spell checker
        /// </summary>
        ISpellChecker ActiveSpellChecker { get; }

        /// <summary>
        /// Returns the collection of discovered spell checker extensions
        /// </summary>
        /// <returns>collection of types</returns>
        IEnumerable<Type> GetExtensions();

        /// <summary>
        /// Initialize the SpellCheck manager
        /// </summary>
        /// <param name="extensionDirs">list of directories</param>
        /// <returns>true on success</returns>
        bool Init(IEnumerable<String> extensionDirs);

        /// <summary>
        /// Indicates to the active spell checker that it needs to load its default settings
        /// </summary>
        void LoadDefaultSettings();

        /// <summary>
        /// Initializes the SpellCheck manager by looking for SpellCheck extension dlls
        /// </summary>
        /// <param name="extensionDirs">list of directories</param>
        /// <returns>true on success</returns>
        bool LoadExtensions(IEnumerable<String> extensionDirs);

        /// <summary>
        /// Indicates to the active spell checker that it needs to save its settings
        /// </summary>
        void SaveSettings();

        /// <summary>
        /// Sets the active spellchecker for the specified culture
        /// </summary>
        /// <param name="ci">culture info, null for default</param>
        /// <returns>true on success</returns>
        bool SetActiveSpellChecker(CultureInfo ci = null);

        /// <summary>
        /// Switch language to the specified one
        /// </summary>
        /// <param name="ci">culture to switch to</param>
        /// <returns>true on success</returns>
        bool SwitchLanguage(CultureInfo ci);
    }
}
