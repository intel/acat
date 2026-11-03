////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// CreatePanelCommandHandler.cs
//
// Sample CQRS command handler that creates a panel using IPanelManager.
// Register this handler in your composition root and inject IPanelManager
// so that panel creation is decoupled from the calling code.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.PanelManagement;
using ACAT.Core.PanelManagement.Interfaces;
using System.Windows.Forms;

namespace ACAT.Core.Patterns.CQRS.Samples
{
    /// <summary>
    /// Handles <see cref="CreatePanelCommand"/> by delegating to
    /// <see cref="IPanelManager"/>.
    /// </summary>
    public class CreatePanelCommandHandler : ICommandHandler<CreatePanelCommand>
    {
        private readonly IPanelManager _panelManager;

        /// <summary>
        /// Initialises a new <see cref="CreatePanelCommandHandler"/>.
        /// </summary>
        /// <param name="panelManager">The panel manager to use.</param>
        public CreatePanelCommandHandler(IPanelManager panelManager)
        {
            _panelManager = panelManager;
        }

        /// <inheritdoc />
        public void Handle(CreatePanelCommand command)
        {
            Form panel;

            if (command.StartupArg != null)
            {
                panel = _panelManager.CreatePanel(
                    command.PanelClass,
                    command.Title ?? string.Empty,
                    command.StartupArg);
            }
            else if (!string.IsNullOrEmpty(command.Title))
            {
                panel = _panelManager.CreatePanel(command.PanelClass, command.Title);
            }
            else
            {
                panel = _panelManager.CreatePanel(command.PanelClass);
            }

            // Set the created panel on the command for callers to use
            command.CreatedPanel = panel as IPanel;
        }
    }
}
