////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// IActuator.cs
//
// Interface for an actuator switch.  All switches must implement this
// interface.
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.Media;

namespace ACAT.Lib.Core.ActuatorManagement
{
    /// <summary>
    /// Delegate for the event raised when the accept time for the switch elapses
    /// </summary>
    /// <param name="sender">sender of the event</param>
    /// <param name="e">event args</param>
    public delegate void AcceptTimeExpired(object sender, ActuatorSwitchEventArgs e);

    /// <summary>
    /// Action of the switch
    /// </summary>
    public enum SwitchAction
    {
        Unknown,
        Down,
        Up,
        Trigger
    }

    /// <summary>
    /// Enmeration of the different switch triggers for scanning for manual scan.
    /// Actuator switches can be configured as a trigger for one of these modes
    /// the mode
    /// </summary>
    public enum TriggerScanModes
    {
        /// <summary>
        /// Undefined
        /// </summary>
        None,

        /// <summary>
        /// Scan horizontal in the left direction
        /// </summary>
        TriggerScanLeft,

        /// <summary>
        /// Scan horizontal in the right direction
        /// </summary>
        TriggerScanRight,

        /// <summary>
        /// Scan vertical in the upward direction
        /// </summary>
        TriggerScanUp,

        /// <summary>
        /// Scan vertical in the downward direction
        /// </summary>
        TriggerScanDown,

        /// <summary>
        /// Move scan one widget to the left
        /// </summary>
        TriggerMoveLeft,

        /// <summary>
        /// Move scan one widget to the right
        /// </summary>
        TriggerMoveRight,

        /// <summary>
        /// Move scan one widget above
        /// </summary>
        TriggerMoveUp,

        /// <summary>
        /// Move scan one widget down
        /// </summary>
        TriggerMoveDown,

        /// <summary>
        /// Stop scanning
        /// </summary>
        TriggerStop,

        /// <summary>
        /// Pause scanning
        /// </summary>
        TriggerPause,

        /// <summary>
        /// Resume scanning
        /// </summary>
        TriggerResume,

        /// <summary>
        /// Pause/Resume toggle
        /// </summary>
        TriggerPauseToggle
    }

    /// <summary>
    /// Interface for an actuator switch.  All switches must derive from this interface.
    /// </summary>
    public interface IActuatorSwitch : IDisposable
    {
        /// <summary>
        /// Gets or sets the length of time the switch to stay engaged for it to be
        /// recognized as a valid trigger.  If the switch stays engaged for less than
        /// the MinActuationHoldTime, it is ignored.
        /// </summary>
        int AcceptTime { get; set; }

        /// <summary>
        /// Gets the timer that tracks the accept time for the switch
        /// </summary>
        Stopwatch AcceptTimer { get; }

        /// <summary>
        /// Gets or sets the switch action
        /// </summary>
        SwitchAction Action { get; set; }

        /// <summary>
        /// Gets or sets property that controls whether the switch should be
        /// actuated or not.
        /// </summary>
        bool Actuate { get; set; }

        /// <summary>
        /// Gets or sets the parent actuator that contains the switch object
        /// </summary>
        IActuator Actuator { get; set; }

        /// <summary>
        /// Gets or sets the audio player to play the beep
        /// </summary>
        SoundPlayer Audio { get; }

        /// <summary>
        /// Gets or sets the name of the WAV file containing the beep associated
        /// with the switch. Beep is sounded when the switch is trigerred
        /// </summary>
        String BeepFile { get; set; }

        /// <summary>
        /// Command that is mapped to this switch
        /// </summary>
        String Command { get; }

        /// <summary>
        /// Gets or sets the confidence level of activation.  For future use.
        /// </summary>
        int Confidence { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        String Description { get; set; }

        /// <summary>
        /// Gets or sets whether this switch is enabled or not
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Gets the active state of the switch.  True if active.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Name of the switch
        /// </summary>
        String Name { get; }

        /// <summary>
        /// Gets or sets the source of the switch activation.Depends on the type of
        /// the switch.  For instance, for a keyboard switch, source
        /// would be "F5" for the F5 function key.
        /// </summary>
        String Source { get; set; }

        /// <summary>
        /// Gets or sets auxilliary data.  Opaque, can be set by the application.
        /// </summary>
        String Tag { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of switch event
        /// </summary>
        long Timestamp { get; set; }

        /// <summary>
        /// If this switch is configured as a trigger for manual scanning, returns
        /// the scan mode
        /// </summary>
        /// <returns></returns>
        TriggerScanModes GetTriggerScanMode();

        /// <summary>
        /// Initialize the actuator switch
        /// </summary>
        /// <returns>true on success</returns>
        bool Init();

        /// <summary>
        /// Is this switch configured as a select trigger?
        /// </summary>
        /// <returns></returns>
        bool IsSelectTriggerSwitch();

        /// <summary>
        /// XML node that contains attributes for the switch
        /// </summary>
        /// <param name="xmlNode">The XML node that contains the Switch attributes</param>
        /// <returns>True on successful parse, false otherwise</returns>
        bool Load(SwitchSetting switchSetting);

        /// <summary>
        /// Records the fact that a switch down was detected.  Call this when the
        /// switch is engaged
        /// </summary>
        void RegisterSwitchDown();

        /// <summary>
        /// Records the fact the switch up was detected. Call this when the switch
        ///  is disengaged
        /// </summary>
        void RegisterSwitchUp();
    }
}