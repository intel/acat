////////////////////////////////////////////////////////////////////////////
// <copyright file="IActuator.cs" company="Intel Corporation">
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

using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.PreferencesManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ACAT.Lib.Core.ActuatorManagement
{
    /// <summary>
    /// Delegate for the event raised when a switch is engaged. This is the
    /// start of the switch activation event.  For instance, for a keyboard switch,
    /// this is equivalent to a keydown
    /// </summary>
    /// <param name="sender">sender object</param>
    /// <param name="e">event argument</param>
    public delegate void SwitchActivated(object sender, ActuatorSwitchEventArgs e);

    /// <summary>
    /// Delegate for the event raised when a switch is disengaged.  Ths is the end of
    /// the switch activation event.  For instance, for a keyboard switch, this is
    /// equivalent to a keyup
    /// </summary>
    /// <param name="sender">sender object</param>
    /// <param name="e">event argument</param>
    public delegate void SwitchDeactivated(object sender, ActuatorSwitchEventArgs e);

    /// <summary>
    /// Delegate for the event raised when a switch is triggered - engaged and disengaged.
    /// For instance, for a keyboard switch, this is equivalent to a KeyTriggered
    /// </summary>
    /// <param name="sender">sender object</param>
    /// <param name="e">event arguement</param>
    public delegate void SwitchTriggered(object sender, ActuatorSwitchEventArgs e);

    /// <summary>
    /// Represents the state of the actuator
    /// </summary>
    public enum State
    {
        Paused,
        Running,
        Stopped
    }

    /// <summary>
    /// Actutators must implement this interface.  An actuator contains one or
    /// more switches and raises events when the switches are actuated.
    /// </summary>
    public interface IActuator : ISupportsPreferences, IExtension, IDisposable
    {
        /// <summary>
        /// Raised when one of the switches in this actuator is engaged
        /// </summary>
        event SwitchActivated EvtSwitchActivated;

        /// <summary>
        /// Raised when one of the switches in this actuator is disengaged
        /// </summary>
        event SwitchDeactivated EvtSwitchDeactivated;

        /// <summary>
        /// Raised when one of the switches in this actuator is triggered
        /// </summary>
        event SwitchTriggered EvtSwitchTriggered;

        /// <summary>
        /// Indicates whether the acutator is enabled or not
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Gets the name of the actuator
        /// </summary>
        String Name { get; }

        /// <summary>
        /// Gets a collection of switches that are a part of this actuator
        /// </summary>
        ICollection<IActuatorSwitch> Switches { get; }

        /// <summary>
        /// Creates an actuator switch object
        /// </summary>
        /// <returns>Switch object</returns>
        IActuatorSwitch CreateSwitch();

        /// <summary>
        /// Initializes the actuator
        /// </summary>
        /// <returns>true on success</returns>
        bool Init();

        /// <summary>
        /// Parses the XML node that contains all the info for this actuator
        /// </summary>
        /// <param name="actuatorNode">The xml fragment for the actuator</param>
        /// <returns>true on success</returns>
        bool Load(IEnumerable<SwitchSetting> switchSettings);

        /// <summary>
        /// Invoked when the user presses the button on the
        /// calibration dialog
        /// </summary>
        void OnCalibrationAction();

        /// <summary>
        /// Invoked if the calibration is canceled
        /// </summary>
        void OnCalibrationCanceled();

        /// <summary>
        /// If the calibration is required to complete in a specific time,
        /// this function is invoked when the timer expires
        /// </summary>
        void OnCalibrationPeriodExpired();

        /// <summary>
        /// Invoked when the application quits
        /// </summary>
        void OnQuitApplication();

        /// <summary>
        /// The Actutor should register switches when this function is invoked
        /// </summary>
        void OnRegisterSwitches();

        /// <summary>
        /// Pauses the actuator.  No events will be raised from the acutator
        /// when paused
        /// </summary>
        void Pause();

        /// <summary>
        /// Unloads the specified switch
        /// </summary>
        /// <param name="name">Switch to unload</param>
        /// <returns></returns>
        bool RemoveSwitch(String name);

        /// <summary>
        /// Resumes actuator.  Will resume raising events
        /// </summary>
        void Resume();

        /// <summary>
        /// Starts calibration of the actuator
        /// </summary>
        void StartCalibration();
    }
}