////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// InputSensorSwitchEventArgs.cs
//
// Event arguments to the event that is invoked when a
// switch activity is to be notified
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Extensions.Default.Actuators.SampleActuator
{
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