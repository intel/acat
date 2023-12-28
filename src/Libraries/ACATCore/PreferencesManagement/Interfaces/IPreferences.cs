////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// IPreferences.cs
//
// Implement this interface if the class supports Preferences.
// The generic preference settings forms use this interface
// to save the settings
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Lib.Core.PreferencesManagement
{
    /// <summary>
    /// Implement this interface if the class supports Preferences.
    /// The generic preference settings forms use this interface
    /// to save the settings
    /// </summary>
    public interface IPreferences
    {
        /// <summary>
        /// Save the preferences to a file
        /// </summary>
        /// <returns>true on sucess</returns>
        bool Save();
    }
}