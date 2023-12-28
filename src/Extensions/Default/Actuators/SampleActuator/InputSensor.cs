////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// InputSensor.cs
//
// This is a sample sensor class.  This is the one that interacts
// with the sensor hardware, detects switch actuations and triggers.
// It then raises events indicating the type of switch activity.
// An example would be a camera sensor that detects facial gestures.
// It could detect multiple gestures such as a mouth twitch,
// cheek twitch, eyebrow raise, smile, frown etc and raise events
// indicating which type of gesture was detected.
// Gestures can be mapped to actions.  The default gesture is a
// switch activation which selects the currently highlighted
// element in a swtich scanning interface.
// Each Gesture is treated as an actuator switch.  This is
// analogous to assigning keyboard hot keys to actions.  Each
// hotkey can be treated as a switch, which when activated,
// executes the assigned action.
// Mapping of switches to actions is done through two
// actuator configuration files - Actuators.xml and SwitchConfigMap.xml.
// Refer to the ACAT Developer's Guide for details on these two files.
//
// A Note about switch activity events:
//
// Some sensors have the ability to detect a swtich-down and
// switch-up events, and some have the ability to just detect
// when a swtich was triggered. If your sensor is able to
// detect all three types of events, you must choose which ones
// you want to use.  You can't use all three.  You can eiter
// choose to notify ACAT about:
// a) A swtich-down event followed by a swtich-up event
//       OR
// b) A switch trigger event.
//
// Do NOT do both a) and b).
//
// So in this class, you must either call:
// a) notifySensorActivate() followed by notifySensorDectivate
//        OR
// b) notifySensorTrigger()
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Extensions.Default.Actuators.SampleActuator
{
    internal delegate void ActuatorSwitchEvent(object sender, InputSensorSwitchEventArgs e);

    internal class InputSensor
    {
        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public InputSensor()
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
            if (EvtSwitchActivate != null)
            {
                EvtSwitchActivate(this, new InputSensorSwitchEventArgs(gesture));
            }
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
            if (EvtSwitchDeactivate != null)
            {
                EvtSwitchDeactivate(this, new InputSensorSwitchEventArgs(gesture));
            }
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
            if (EvtSwitchTrigger != null)
            {
                EvtSwitchTrigger(this, new InputSensorSwitchEventArgs(gesture));
            }
        }
    }
}