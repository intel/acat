////////////////////////////////////////////////////////////////////////////
// <copyright file="ActuatorSwitchEventArgs.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2017 Intel Corporation 
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
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