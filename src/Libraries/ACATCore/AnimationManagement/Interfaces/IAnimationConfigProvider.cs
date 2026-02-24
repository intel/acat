////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// IAnimationConfigProvider.cs
//
// Abstraction for loading animation configuration for a panel.
// Supports both JSON (preferred) and XML (legacy) sources.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.AnimationManagement.Configuration;

namespace ACAT.Core.AnimationManagement.Interfaces
{
    /// <summary>
    /// Loads and provides <see cref="AnimationConfig"/> for a named panel.
    /// The default implementation (AnimationConfigProvider) loads JSON files.
    /// The XmlAnimationConfigAdapter converts legacy XML Animation elements.
    /// </summary>
    public interface IAnimationConfigProvider
    {
        /// <summary>
        /// Loads the animation configuration for the specified panel.
        /// </summary>
        /// <param name="panelName">The registered panel name.</param>
        /// <param name="configPath">Path to the directory containing the config file.</param>
        /// <returns>The loaded <see cref="AnimationConfig"/>, or null if not found.</returns>
        AnimationConfig LoadForPanel(string panelName, string configPath);

        /// <summary>
        /// Returns true if a JSON configuration file exists for the given panel name.
        /// </summary>
        bool HasJsonConfig(string panelName, string configPath);
    }
}
