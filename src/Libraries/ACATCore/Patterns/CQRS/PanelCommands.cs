////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// PanelCommands.cs
//
// CQRS command and query definitions for panel operations.
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
}
