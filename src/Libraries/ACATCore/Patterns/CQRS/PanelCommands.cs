////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// PanelCommands.cs
//
// Sample CQRS command and query definitions for panel operations.
//
////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

namespace ACAT.Core.Patterns.CQRS
{
    // -----------------------------------------------------------------------
    // Panel Commands (state-changing operations)
    // -----------------------------------------------------------------------

    /// <summary>Command to show (display) a named panel.</summary>
    public class ShowPanelCommand : ICommand
    {
        /// <summary>Gets the name of the panel to show.</summary>
        public string PanelName { get; }

        /// <summary>Initializes a new <see cref="ShowPanelCommand"/>.</summary>
        public ShowPanelCommand(string panelName)
        {
            PanelName = panelName;
        }
    }

    /// <summary>Command to hide (close) a named panel.</summary>
    public class HidePanelCommand : ICommand
    {
        /// <summary>Gets the name of the panel to hide.</summary>
        public string PanelName { get; }

        /// <summary>Initializes a new <see cref="HidePanelCommand"/>.</summary>
        public HidePanelCommand(string panelName)
        {
            PanelName = panelName;
        }
    }

    // -----------------------------------------------------------------------
    // Panel Queries (read-only operations)
    // -----------------------------------------------------------------------

    /// <summary>Query that returns the name of the currently active panel.</summary>
    public class GetActivePanelQuery : IQuery<string> { }

    /// <summary>Query that returns the names of all registered panels.</summary>
    public class GetAllPanelNamesQuery : IQuery<IReadOnlyList<string>> { }

    // -----------------------------------------------------------------------
    // Configuration Queries
    // -----------------------------------------------------------------------

    /// <summary>Query that returns a configuration value by key.</summary>
    public class GetConfigurationValueQuery : IQuery<string>
    {
        /// <summary>Gets the configuration key to look up.</summary>
        public string Key { get; }

        /// <summary>Gets the default value to return when the key is not found.</summary>
        public string DefaultValue { get; }

        /// <summary>Initializes a new <see cref="GetConfigurationValueQuery"/>.</summary>
        public GetConfigurationValueQuery(string key, string defaultValue = null)
        {
            Key = key;
            DefaultValue = defaultValue;
        }
    }

    // -----------------------------------------------------------------------
    // Actuator Commands
    // -----------------------------------------------------------------------

    /// <summary>Command representing an actuator switch activation.</summary>
    public class HandleActuatorSwitchCommand : ICommand
    {
        /// <summary>Gets the name of the switch that was activated.</summary>
        public string SwitchName { get; }

        /// <summary>Gets optional data associated with the switch activation.</summary>
        public string SwitchData { get; }

        /// <summary>Initializes a new <see cref="HandleActuatorSwitchCommand"/>.</summary>
        public HandleActuatorSwitchCommand(string switchName, string switchData = null)
        {
            SwitchName = switchName;
            SwitchData = switchData;
        }
    }
}
