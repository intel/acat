////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace ACAT.Core.ThemeManagement
{
    /// <summary>
    /// Interface for ThemeManager to support dependency injection.
    /// Manages themes and maintains the currently active theme.
    /// </summary>
    public interface IThemeManager : IDisposable
    {
        /// <summary>
        /// Gets the currently active Theme object
        /// </summary>
        Theme ActiveTheme { get; }

        /// <summary>
        /// Gets the name of the currently active theme
        /// </summary>
        String ActiveThemeName { get; }

        /// <summary>
        /// Gets a list of themes discovered
        /// </summary>
        IEnumerable<String> Themes { get; }

        /// <summary>
        /// Gets the directory of the specified theme
        /// </summary>
        /// <param name="theme">theme name</param>
        /// <returns>theme directory, empty string if theme invalid</returns>
        String GetThemeDir(String theme);

        /// <summary>
        /// Initializes the theme manager
        /// </summary>
        /// <returns>true on success</returns>
        bool Init();

        /// <summary>
        /// Sets the active theme by name
        /// </summary>
        /// <param name="name">Name of the Theme</param>
        /// <returns>true on success</returns>
        bool SetActiveTheme(String name);
    }
}
