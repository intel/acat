////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.PreferencesManagement;
using ACAT.Core.WordPredictorManagement.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace ACAT.Core.WordPredictorManagement
{
    /// <summary>
    /// Interface for WordPredictionManager to support dependency injection.
    /// Manages word prediction engines.
    /// </summary>
    public interface IWordPredictionManager : IDisposable
    {
        /// <summary>
        /// Gets the list of word predictors
        /// </summary>
        IEnumerable<IWordPredictor> WordPredictorsList { get; }

        /// <summary>
        /// Gets the currently active word predictor
        /// </summary>
        IWordPredictor ActiveWordPredictor { get; }

        /// <summary>
        /// Gets the collection of discovered word predictors
        /// </summary>
        ICollection<Type> WordPredictorExtensions { get; }

        /// <summary>
        /// Gets the word predictor root directory relative to the user's current profile
        /// </summary>
        string WordPredictorRootDirRelativeToProfile { get; }

        /// <summary>
        /// Gets the word predictor root directory relative to the user home directory
        /// </summary>
        string WordPredictorRootDirRelativeToUser { get; }

        /// <summary>
        /// Initialize the Word Predictor manager
        /// </summary>
        /// <param name="extensionDirs">directories to search</param>
        /// <returns>true on success</returns>
        bool Init(IEnumerable<string> extensionDirs);

        /// <summary>
        /// Performs post-initialization
        /// </summary>
        /// <returns>true on success</returns>
        bool PostInit();

        /// <summary>
        /// Indicates to the active word predictor that it needs to load its default settings
        /// </summary>
        void LoadDefaultSettings();

        /// <summary>
        /// Loads all the word prediction extensions
        /// </summary>
        /// <param name="extensionDirs">root directory</param>
        /// <returns>true on success</returns>
        bool LoadExtensions(IEnumerable<string> extensionDirs);

        /// <summary>
        /// Saves preferences in word predictor settings
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="preferencesCategories">preferences to save</param>
        void SavePreferences(object sender, IEnumerable<PreferencesCategory> preferencesCategories);

        /// <summary>
        /// Indicates to the active word predictor that it needs to save its settings
        /// </summary>
        void SaveSettings();

        /// <summary>
        /// Sets the active word predictor for the specified culture
        /// </summary>
        /// <param name="ci">culture info, null for default</param>
        /// <returns>true on success</returns>
        bool SetActiveWordPredictor(CultureInfo ci = null);

        /// <summary>
        /// Switch language to the specified one
        /// </summary>
        /// <param name="ci">culture to switch to</param>
        /// <returns>true on success</returns>
        bool SwitchLanguage(CultureInfo ci);
    }
}
