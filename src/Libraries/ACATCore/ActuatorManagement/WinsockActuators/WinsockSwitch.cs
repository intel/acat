////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// WinsockSwitch.cs
//
// Represents a switch triggered from data sent over a tcp/ip connection
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.ActuatorManagement;

namespace ACAT.Lib.Core.InputActuators
{
    /// <summary>
    /// Represents a switch triggered from data sent over a tcp/ip connection
    /// </summary>
    internal class WinsockSwitch : ActuatorSwitchBase
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="switchObj">Source switch from which to create a clone</param>
        public WinsockSwitch(IActuatorSwitch switchObj)
            : base(switchObj)
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public WinsockSwitch()
        {
        }
    }
}