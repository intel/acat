////////////////////////////////////////////////////////////////////////////
// <copyright file="InputSensorSwitchEventArgs.cs" company="Intel Corporation">
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

namespace ACAT.Extensions.Default.Actuators.SampleActuator
{
    /// <summary>
    /// Event arguments to the event that is invoked when a
    /// switch activity is to be notified
    /// </summary>
    internal class InputSensorSwitchEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        /// <param name="gesture"></param>
        public InputSensorSwitchEventArgs(String gesture)
        {
            Gesture = gesture;
        }

        /// <summary>
        /// Gets or sets the gesture
        /// </summary>
        public String Gesture { get; set; }
    }
}