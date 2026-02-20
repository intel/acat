////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// CreatePanelCommand.cs
//
// Sample CQRS command that encapsulates a request to create (show) a panel.
// Pass this to an ICommandHandler<CreatePanelCommand> implementation to
// display the panel via the PanelManager.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.PanelManagement.Common;

namespace ACAT.Core.Patterns.CQRS.Samples
{
    /// <summary>
    /// Command that requests creation of an ACAT panel.
    /// </summary>
    public class CreatePanelCommand : ICommand
    {
        /// <summary>
        /// Gets the class name of the panel to create.
        /// </summary>
        public string PanelClass { get; }

        /// <summary>
        /// Gets the display title for the panel (optional).
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets optional startup arguments for the panel (may be null).
        /// </summary>
        public StartupArg StartupArg { get; }

        /// <summary>
        /// Initialises a new <see cref="CreatePanelCommand"/>.
        /// </summary>
        /// <param name="panelClass">Class name of the panel to create.</param>
        /// <param name="title">Optional display title.</param>
        /// <param name="startupArg">Optional startup arguments.</param>
        public CreatePanelCommand(string panelClass, string title = null, StartupArg startupArg = null)
        {
            PanelClass = panelClass;
            Title = title;
            StartupArg = startupArg;
        }
    }
}
