////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// HandleActuatorSwitchCommandHandler.cs
//
// Sample CQRS command handler that delegates actuator switch actions
// (pause / resume) to the IActuatorManager.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.ActuatorManagement;
using System;

namespace ACAT.Core.Patterns.CQRS.Samples
{
    /// <summary>
    /// Handles <see cref="HandleActuatorSwitchCommand"/> by delegating to
    /// <see cref="IActuatorManager"/>.
    /// </summary>
    public class HandleActuatorSwitchCommandHandler : ICommandHandler<HandleActuatorSwitchCommand>
    {
        private readonly IActuatorManager _actuatorManager;

        /// <summary>
        /// Initialises a new <see cref="HandleActuatorSwitchCommandHandler"/>.
        /// </summary>
        /// <param name="actuatorManager">The actuator manager to use.</param>
        public HandleActuatorSwitchCommandHandler(IActuatorManager actuatorManager)
        {
            _actuatorManager = actuatorManager;
        }

        /// <inheritdoc />
        public void Handle(HandleActuatorSwitchCommand command)
        {
            switch (command.Action)
            {
                case ActuatorSwitchAction.Pause:
                    _actuatorManager.Pause();
                    break;

                case ActuatorSwitchAction.Resume:
                    _actuatorManager.Resume();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(command),
                        command.Action,
                        "Unknown ActuatorSwitchAction.");
            }
        }
    }
}
