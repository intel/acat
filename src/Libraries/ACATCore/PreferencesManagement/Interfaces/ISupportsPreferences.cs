////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ISupportsPreferences.cs
//
// Implement this interface if the extension supports settings
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Lib.Core.PreferencesManagement
{
    /// <summary>
    /// Implement this interface if the extension supports settings
    /// </summary>
    public interface ISupportsPreferences
    {
        /// <summary>
        /// Gets whether this supports a custom settings dialog
        /// </summary>
        bool SupportsPreferencesDialog { get; }

        /// <summary>
        /// Returns the default preferences (factory settings)
        /// </summary>
        /// <returns>Default preferences object</returns>
        IPreferences GetDefaultPreferences();

        /// <summary>
        /// Returns the preferences object
        /// </summary>
        /// <returns>The preferences object</returns>
        IPreferences GetPreferences();

        /// <summary>
        /// Shows the preferences dialog
        /// </summary>
        /// <returns>true on success</returns>
        bool ShowPreferencesDialog();
    }
}