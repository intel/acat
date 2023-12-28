////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ActuatorSwitchEventArgs.cs
//
// Event argument for switch trigger events
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Lib.Core.ActuatorManagement
{
    /// <summary>
    /// Event argument for switch trigger events
    /// </summary>
    public class ActuatorSwitchEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the ActuatorSwitchEventArgs class
        /// </summary>
        /// <param name="actuatorSwitch">The switch object</param>
        public ActuatorSwitchEventArgs(IActuatorSwitch actuatorSwitch)
        {
            SwitchObj = actuatorSwitch;
        }

        /// <summary>
        /// Gets the actuator switch object
        /// </summary>
        public IActuatorSwitch SwitchObj { get; private set; }
    }
}