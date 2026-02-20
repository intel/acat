////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ActuatorEvents.cs
//
// Event types for actuator notifications (switch activation).
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Core.EventManagement
{
    /// <summary>
    /// Published when an actuator switch is activated.
    /// </summary>
    public class ActuatorSwitchActivatedEvent : EventBase
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ActuatorSwitchActivatedEvent"/>.
        /// </summary>
        /// <param name="switchName">The name of the switch that was activated.</param>
        public ActuatorSwitchActivatedEvent(string switchName)
        {
            SwitchName = switchName;
        }

        /// <summary>
        /// Gets the name of the actuator switch that was activated.
        /// </summary>
        public string SwitchName { get; }
    }
}
