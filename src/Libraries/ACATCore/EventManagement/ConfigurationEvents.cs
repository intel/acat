////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ConfigurationEvents.cs
//
// Event types for configuration notifications (reload, change).
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Core.EventManagement
{
    /// <summary>
    /// Published when the application configuration is reloaded from disk.
    /// </summary>
    public class ConfigurationReloadEvent : EventBase
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ConfigurationReloadEvent"/>.
        /// </summary>
        /// <param name="configPath">
        /// The file-system path of the configuration file that was reloaded.
        /// </param>
        public ConfigurationReloadEvent(string configPath)
        {
            ConfigPath = configPath;
        }

        /// <summary>
        /// Gets the file-system path of the configuration file that was reloaded.
        /// </summary>
        public string ConfigPath { get; }
    }

    /// <summary>
    /// Published when an individual configuration value changes.
    /// </summary>
    public class ConfigurationChangedEvent : EventBase
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ConfigurationChangedEvent"/>.
        /// </summary>
        /// <param name="key">The configuration key that changed.</param>
        /// <param name="newValue">
        /// The new value associated with <paramref name="key"/>
        /// (may be <c>null</c>).
        /// </param>
        public ConfigurationChangedEvent(string key, object newValue)
        {
            Key = key;
            NewValue = newValue;
        }

        /// <summary>
        /// Gets the configuration key whose value changed.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets the new value for the configuration key.
        /// </summary>
        public object NewValue { get; }
    }
}
