////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.ActuatorManagement.Interfaces;
using ACAT.Core.ActuatorManagement.Settings;
using ACAT.Core.PreferencesManagement;
using System;
using System.Collections.Generic;

namespace ACAT.Core.ActuatorManagement
{
    /// <summary>
    /// Interface for ActuatorManager to support dependency injection.
    /// Manages all the actuators and their switches, handling switch trigger events
    /// and calibration operations.
    /// </summary>
    public interface IActuatorManager : IDisposable
    {
        /// <summary>
        /// Event raised to indicate start of calibration
        /// </summary>
        event ActuatorManager.CalibrationStartNotify EvtCalibrationStartNotify;

        /// <summary>
        /// Event raised to indicate end of calibration
        /// </summary>
        event EventHandler EvtCalibrationEndNotify;

        /// <summary>
        /// Event is trigged when a switch is accepted
        /// </summary>
        event ActuatorManager.ActuatorSwitchEvent EvtSwitchAccepted;

        /// <summary>
        /// Raised when a switch is activated
        /// </summary>
        event ActuatorManager.ActuatorSwitchEvent EvtSwitchActivated;

        /// <summary>
        /// Event is trigged when a switch is down
        /// </summary>
        event ActuatorManager.ActuatorSwitchEvent EvtSwitchDown;

        /// <summary>
        /// Hook event to allow apps to access switch events
        /// </summary>
        event ActuatorManager.SwitchHook EvtSwitchHook;

        /// <summary>
        /// Event is trigged when a switch is rejected
        /// </summary>
        event ActuatorManager.ActuatorSwitchEvent EvtSwitchRejected;

        /// <summary>
        /// Event is trigged when a switch is up
        /// </summary>
        event ActuatorManager.ActuatorSwitchEvent EvtSwitchUp;

        /// <summary>
        /// Gets the list of all actuators
        /// </summary>
        IEnumerable<IActuator> ActuatorsList { get; }

        /// <summary>
        /// Gets the actuator by the specified type
        /// </summary>
        /// <param name="actuatorType">actuator type</param>
        /// <returns>actuator object, null if not found</returns>
        IActuator GetActuator(Type actuatorType);

        /// <summary>
        /// Gets the actuator by the specified GUID
        /// </summary>
        /// <param name="id">GUID of the actuator</param>
        /// <returns>actuator object, null if not found</returns>
        IActuator GetActuator(Guid id);

        /// <summary>
        /// Performs initialization
        /// </summary>
        /// <param name="extensionDirs">Directories to search</param>
        /// <returns>true on success</returns>
        bool Init(IEnumerable<String> extensionDirs);

        /// <summary>
        /// Performs second phase initialization
        /// </summary>
        /// <returns>true on success</returns>
        bool PostInit();

        /// <summary>
        /// Checks if any switch is currently active
        /// </summary>
        /// <returns>true if so</returns>
        bool IsSwitchActive();

        /// <summary>
        /// Loads actuator extensions from the specified directories
        /// </summary>
        /// <param name="extensionDirs">Directories to search</param>
        /// <param name="all">Load all actuators?</param>
        /// <returns>true on success</returns>
        bool LoadExtensions(IEnumerable<String> extensionDirs, bool all = false);

        /// <summary>
        /// Called when calibration is canceled
        /// </summary>
        /// <param name="source">actuator that raised the event</param>
        void OnCalibrationCanceled(IActuator source);

        /// <summary>
        /// Called when calibration period expires
        /// </summary>
        /// <param name="source">actuator that raised the event</param>
        void OnCalibrationPeriodExpired(IActuator source);

        /// <summary>
        /// Called when calibration ends
        /// </summary>
        /// <param name="source">actuator that raised the event</param>
        /// <param name="errorMessage">error message if any</param>
        /// <param name="enableConfigure">enable configure button?</param>
        void OnEndCalibration(IActuator source, String errorMessage = "", bool enableConfigure = true);

        /// <summary>
        /// Called when there is an error
        /// </summary>
        /// <param name="source">actuator that raised the event</param>
        /// <param name="message">error message</param>
        /// <param name="enableConfigure">enable configure button?</param>
        void OnError(IActuator source, String message, bool enableConfigure = true);

        /// <summary>
        /// Called when initialization is done
        /// </summary>
        /// <param name="source">actuator that raised the event</param>
        /// <param name="success">was init successful?</param>
        void OnInitDone(IActuator source, bool success = true);

        /// <summary>
        /// Called when post-initialization is done
        /// </summary>
        /// <param name="source">actuator that raised the event</param>
        /// <param name="success">was post-init successful?</param>
        void OnPostInitDone(IActuator source, bool success = true);

        /// <summary>
        /// Pauses all actuators
        /// </summary>
        void Pause();

        /// <summary>
        /// Registers a switch with the actuator manager
        /// </summary>
        /// <param name="actuator">parent actuator</param>
        /// <param name="switchSetting">switch settings</param>
        /// <returns>true on success</returns>
        bool RegisterSwitch(IActuator actuator, SwitchSetting switchSetting);

        /// <summary>
        /// Gets an actuator that supports calibration
        /// </summary>
        /// <returns>actuator object, null if not found</returns>
        IActuator GetCalibrationSupportedActuator();

        /// <summary>
        /// Requests calibration for the specified actuator
        /// </summary>
        /// <param name="source">actuator requesting calibration</param>
        /// <param name="reason">reason for calibration</param>
        void RequestCalibration(IActuator source, RequestCalibrationReason reason);

        /// <summary>
        /// Resumes all actuators
        /// </summary>
        void Resume();

        /// <summary>
        /// Gets the actuator configuration
        /// </summary>
        /// <returns>configuration object</returns>
        ActuatorConfig GetActuatorConfig();

        /// <summary>
        /// Saves preferences
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="preferencesCategories">preferences to save</param>
        void SavePreferences(object sender, IEnumerable<PreferencesCategory> preferencesCategories);

        /// <summary>
        /// Unregisters a switch
        /// </summary>
        /// <param name="actuator">parent actuator</param>
        /// <param name="switchName">name of the switch</param>
        /// <returns>true on success</returns>
        bool UnregisterSwitch(IActuator actuator, String switchName);

        /// <summary>
        /// Updates the calibration status display
        /// </summary>
        /// <param name="source">actuator that raised the event</param>
        /// <param name="caption">caption to display</param>
        /// <param name="prompt">prompt to display</param>
        /// <param name="timeout">timeout in milliseconds</param>
        /// <param name="enableConfigure">enable configure button?</param>
        /// <param name="buttonText">button text</param>
        void UpdateCalibrationStatus(IActuator source, String caption, String prompt, int timeout = 0, bool enableConfigure = true, String buttonText = "");

        /// <summary>
        /// Notifies subscribers that calibration has started
        /// </summary>
        /// <param name="args">event args</param>
        void NotifyStartCalibration(CalibrationNotifyEventArgs args);

        /// <summary>
        /// Notifies subscribers that calibration has ended
        /// </summary>
        void NotifyEndCalibration();

        /// <summary>
        /// Gets the keyboard actuator
        /// </summary>
        /// <returns>keyboard actuator, null if not found</returns>
        IActuator GetKeyboardActuator();

        /// <summary>
        /// Gets the switch interface actuator
        /// </summary>
        /// <returns>switch interface actuator, null if not found</returns>
        IActuator GetSwitchInterfaceActuator();

        /// <summary>
        /// Checks if scan timing configuration should be enabled
        /// </summary>
        /// <returns>true if enabled</returns>
        bool CheckScanTimingConfigureEnable();

        /// <summary>
        /// Shows the actuator tryout dialog
        /// </summary>
        /// <param name="startup">is this during startup?</param>
        void ShowTryoutDialog(bool startup = false);

        /// <summary>
        /// Shows the scan timings configuration dialog
        /// </summary>
        void ShowScanTimingsConfigureDialog();
    }
}
