////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// HandleActuatorSwitchCommand.cs
//
// Sample CQRS command that encapsulates a request to handle an actuator
// switch event (e.g. pause or resume scanning based on switch state).
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Core.Patterns.CQRS.Samples
{
    /// <summary>
    /// Indicates the requested actuator switch action.
    /// </summary>
    public enum ActuatorSwitchAction
    {
        /// <summary>Pause scanning / switch handling.</summary>
        Pause,
        /// <summary>Resume scanning / switch handling.</summary>
        Resume
    }

    /// <summary>
    /// Command that requests a pause or resume of actuator switch handling.
    /// </summary>
    public class HandleActuatorSwitchCommand : ICommand
    {
        /// <summary>
        /// Gets the action to apply to the actuator manager.
        /// </summary>
        public ActuatorSwitchAction Action { get; }

        /// <summary>
        /// Initialises a new <see cref="HandleActuatorSwitchCommand"/>.
        /// </summary>
        /// <param name="action">The switch action to perform.</param>
        public HandleActuatorSwitchCommand(ActuatorSwitchAction action)
        {
            Action = action;
        }
    }
}
