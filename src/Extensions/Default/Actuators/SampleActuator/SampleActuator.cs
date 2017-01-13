////////////////////////////////////////////////////////////////////////////
// <copyright file="SampleActuator.cs" company="Intel Corporation">
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

using ACAT.Lib.Core.ActuatorManagement;
using ACAT.Lib.Core.UserManagement;
using ACAT.Lib.Core.Utility;
using System;

namespace ACAT.Extensions.Default.Actuators.SampleActuator
{
    /// <summary>
    /// This is a sample actuator class. It subscribes to events
    /// from the sampe input sensor class and notifies ACAT
    /// about these events.
    /// </summary>
    [DescriptorAttribute("FE3DE70B-8084-46C1-BAB1-905B215C7738",
                            "Sample Actuator",
                            "Skeleton sample code for a sample actuator")]
    internal class SampleActuator : ActuatorBase
    {
        /// <summary>
        /// The settings object for this actuator
        /// </summary>
        public static Settings SampleActuatorSettings = null;

        /// <summary>
        /// Name of the file that stores the settings for
        /// this actuator
        /// </summary>
        private const String SettingsFileName = "SampleActuatorSettings.xml";

        /// <summary>
        /// Has this object been disposed?
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// The input sensor (hardware?) that detects
        /// input swtich activity.
        /// </summary>
        private InputSensor _sensor;

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public SampleActuator()
        {
        }

        /// <summary>
        /// Class factory to create the switch object
        /// </summary>
        /// <returns>the switch object</returns>
        public override IActuatorSwitch CreateSwitch()
        {
            return new SampleActuatorSwitch();
        }

        /// <summary>
        /// Initialize resources
        /// </summary>
        /// <returns>true on success, false otherwise</returns>
        public override bool Init()
        {
            Settings.SettingsFilePath = UserManager.GetFullPath(SettingsFileName);
            SampleActuatorSettings = Settings.Load();

            // perform initialization here.

            _sensor = new InputSensor();

            _sensor.EvtSwitchActivate += sensor_EvtSwitchActivate;
            _sensor.EvtSwitchDeactivate += sensor_EvtSwitchDeactivate;
            _sensor.EvtSwitchTrigger += sensor_EvtSwitchTrigger;

            actuatorState = State.Running;

            OnInitDone();

            return true;
        }

        /// <summary>
        /// Pause the actuator
        /// </summary>
        public override void Pause()
        {
            actuatorState = State.Paused;
        }

        /// <summary>
        /// Resume the actuator
        /// </summary>
        public override void Resume()
        {
            actuatorState = State.Running;
        }

        /// <summary>
        /// Dispose resources
        /// </summary>
        /// <param name="disposing">true to dispose managed resources</param>
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                try
                {
                    Log.Debug();

                    if (disposing)
                    {
                        // release managed resources
                        unInit();
                    }

                    // Release the native unmanaged resources

                    _disposed = true;
                }
                finally
                {
                    // Call Dispose on your base class.
                    base.Dispose(disposing);
                }
            }
        }

        /// <summary>
        /// Find the switch that deals with the input detected
        /// </summary>
        /// <param name="switchSource">The source name of the switch</param>
        /// <returns>Switch object, null if not found</returns>
        private IActuatorSwitch find(String switchSource)
        {
            foreach (IActuatorSwitch switchObj in Switches)
            {
                if (switchObj is SampleActuatorSwitch)
                {
                    var actuatorSwitch = (SampleActuatorSwitch)switchObj;
                    if (actuatorSwitch.Source == switchSource)
                    {
                        return new SampleActuatorSwitch(switchObj);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Event handler for when a swtich activate event is detected.
        /// Notify ACAT
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void sensor_EvtSwitchActivate(object sender, InputSensorSwitchEventArgs e)
        {
            if (actuatorState == State.Running)
            {
                IActuatorSwitch actuatorSwitch = find(e.Gesture);
                if (actuatorSwitch != null)
                {
                    OnSwitchActivated(actuatorSwitch);
                }
            }
        }

        /// <summary>
        /// Event handler for when a swtich deactivate event is detected.
        /// Notify ACAT
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void sensor_EvtSwitchDeactivate(object sender, InputSensorSwitchEventArgs e)
        {
            if (actuatorState == State.Running)
            {
                IActuatorSwitch actuatorSwitch = find(e.Gesture);
                if (actuatorSwitch != null)
                {
                    OnSwitchDeactivated(actuatorSwitch);
                }
            }
        }

        /// <summary>
        /// Event handler for when a swtich trigger event is detected.
        /// Notify ACAT
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void sensor_EvtSwitchTrigger(object sender, InputSensorSwitchEventArgs e)
        {
            if (actuatorState == State.Running)
            {
                IActuatorSwitch actuatorSwitch = find(e.Gesture);
                if (actuatorSwitch != null)
                {
                    OnSwitchTriggered(actuatorSwitch);
                }
            }
        }

        /// <summary>
        /// Release resources
        /// </summary>
        /// <returns></returns>
        private void unInit()
        {
            actuatorState = State.Stopped;

            // perform unitialization here
        }
    }
}