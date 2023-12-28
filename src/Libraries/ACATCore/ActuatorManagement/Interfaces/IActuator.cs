////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// IActuator.cs
//
// Actutators must implement this interface.  An actuator contains one or
// more switches and raises events when the switches are actuated.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.Onboarding;
using ACAT.Lib.Core.PreferencesManagement;
using System;
using System.Collections.Generic;

namespace ACAT.Lib.Core.ActuatorManagement
{
    /// <summary>
    /// Delegate for the event raised when an actuator wants to send custom data
    /// to the application
    /// </summary>
    /// <param name="opcode">operation code</param>
    /// <param name="response">response to be sent (typically JSON)</param>
    public delegate void IoctlResponse(int opcode, String response);

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

    public enum RequestCalibrationReason
    {
        None,
        SensorInitiated,
        AppRequested
    }

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
        /// Raised when the actuator wants to send custom data to the application
        /// </summary>
        event IoctlResponse EvtIoctlResponse;

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

        String OnboardingImageFileName { get; }

        bool ShowTryoutOnStartup { get; }

        bool SupportsScanTimingsConfigureDialog { get; }

        bool SupportsTryout { get; }

        /// <summary>
        /// Gets a collection of switches that are a part of this actuator
        /// </summary>
        ICollection<IActuatorSwitch> Switches { get; }

        /// <summary>
        /// Creates an actuator switch object
        /// </summary>
        /// <returns>Switch object</returns>
        IActuatorSwitch CreateSwitch();

        IOnboardingExtension GetOnboardingExtension();

        /// <summary>
        /// Returns the current state of the actuator
        /// </summary>
        /// <returns></returns>
        State GetState();

        IEnumerable<String> GetSupportedKeyboardConfigs();

        /// <summary>
        /// Initializes the actuator
        /// </summary>
        /// <returns>true on success</returns>
        bool Init();

        /// <summary>
        /// IO Control - Actuator specific data sent by an application
        /// </summary>
        /// <param name="opcode">operation code</param>
        /// <param name="request">Data (typically a JSON script fragment)</param>
        /// <returns></returns>
        bool IoctlRequest(int opcode, String request);

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

        bool PostInit();

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

        bool ShowScanTimingsConfigureDialog();

        bool ShowTryoutDialog();

        /// <summary>
        /// Starts calibration of the actuator
        /// </summary>
        void StartCalibration(RequestCalibrationReason reason);

        /// <summary>
        /// Indicates whether the actuator supports calibration or not
        /// </summary>
        /// <returns>true if it does, false otherwise</returns>
        bool SupportsCalibration();
    }
}