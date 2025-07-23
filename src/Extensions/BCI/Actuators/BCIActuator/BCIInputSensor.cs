////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// InputSensor.cs
//
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Extensions.BCI.Actuators.BCIActuator
{
    internal delegate void ActuatorSwitchEvent(object sender, InputSensorSwitchEventArgs e);

    /// <summary>
    /// </summary>
    internal class BCIInputSensor
    {
        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public BCIInputSensor()
        {
        }

        /// <summary>
        /// Event raised when the switch is activated.  This is
        /// analogous to a key-down event on a keyboard, or a
        /// mouse buttown-down event.
        /// </summary>
        public event ActuatorSwitchEvent EvtSwitchActivate;

        /// <summary>
        /// Event raised when the switch is deactivated. Tkis is
        /// analogous to a key-up event on a keyboard, or a
        /// mouse buttown-up event.
        /// </summary>
        public event ActuatorSwitchEvent EvtSwitchDeactivate;

        /// <summary>
        /// Event raised when the switch is triggered. Tkis is
        /// analogous to a key-press event on a keyboard, or a
        /// mouse click event.
        /// </summary>
        public event ActuatorSwitchEvent EvtSwitchTrigger;

        /// <summary>
        /// Call this function to notify ACAT that a switch was
        /// activated.
        /// The 'gesture' argument is the type of
        /// gesture that was detected (e.g., eyebrow raise,
        /// cheeck twitch etc).  This string value should be
        /// the same as the "source" attribute of the gesture
        /// configured in Actuators.xml.  Here is an example of
        /// the xml fragment in Actuators.xml
        /*
           <Switch name="G1" source="Gesture1" enabled="true" minHoldTime="@MinActuationHoldTime" actuate="true"/>
           <Switch name="G1" source="Gesture1" enabled="true" minHoldTime="@MinActuationHoldTime" actuate="true"/>
           <Switch name="G3" source="Gesture3" enabled="true" minHoldTime="@MinActuationHoldTime" actuate="true"/>
         */

        ///  The 'gesture' argument should be one of "Gesture1", "Gesture2" or "Gesture3"
        ///
        /// Also check the class documentation at the top of this file
        /// on which events to trigger
        /// </summary>
        /// <param name="gesture">type of gesture detected</param>
        private void notifySensorActivate(string gesture)
        {
            EvtSwitchActivate?.Invoke(this, new InputSensorSwitchEventArgs(gesture));
        }

        /// <summary>
        /// Call this function to notify ACAT that a switch was
        /// deactivated.
        /// See comments for the notifySensorActivate() function
        /// for details.
        /// </summary>
        /// <param name="gesture">type of gesture detected</param>
        private void notifySensorDeactivate(string gesture)
        {
            EvtSwitchDeactivate?.Invoke(this, new InputSensorSwitchEventArgs(gesture));
        }

        /// <summary>
        /// Call this function to notify ACAT that a switch was
        /// triggered.
        /// See comments for the notifySensorActivate() function
        /// for details.
        /// </summary>
        /// <param name="gesture">type of gesture detected</param>
        private void notifySensorTrigger(string gesture)
        {
            EvtSwitchTrigger?.Invoke(this, new InputSensorSwitchEventArgs(gesture));
        }
    }
}