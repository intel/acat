////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ActuatorSwitchBase.cs
//
// Represents the attributes of a switch object. Note that
// the acutal action of the switch is handled in the Actuators.
// The Switch objects just enacapusate the attributes.
// This is the base class for all the actuator switches.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;
using System;
using System.Diagnostics;
using System.IO;
using System.Media;

namespace ACAT.Lib.Core.ActuatorManagement
{
    /// <summary>
    /// Represents the attributes of a switch object. Note that
    /// the acutal action of the switch is handled in the Actuators.
    /// The Switch objects just enacapusate the attributes.
    /// This is the base class for all the actuator switches.
    /// </summary>
    public class ActuatorSwitchBase : IActuatorSwitch
    {
        /// <summary>
        /// The command verb to be used for a switch that
        /// is used for selection
        /// </summary>
        public const String TriggerCommand = "@Trigger";

        /// <summary>
        /// Used for synchronization
        /// </summary>
        private readonly Object _lockObj = new object();

        /// <summary>
        /// Timer to track the accept time.  If the switch
        /// stays engaged for less then MinActuationHoldTime, the
        /// switch is ignored.
        /// </summary>
        private Stopwatch _acceptTimer;

        /// <summary>
        /// Play beep when switch is triggered
        /// </summary>
        private SoundPlayer _audio;

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Is this enabled or not
        /// </summary>
        private Boolean _isActive;

        /// <summary>
        /// Timer to track switch trigger timings
        /// </summary>
        private TimerQueue _timer;

        /// <summary>
        /// Initializes the Switch object
        /// </summary>
        public ActuatorSwitchBase()
        {
            _isActive = false;
            Name = String.Empty;
            Source = String.Empty;
            _acceptTimer = new Stopwatch();
            Actuate = true;
        }

        /// <summary>
        /// Initializes the switch object from the passed switch object
        /// </summary>
        /// <param name="switchObj">Source switch object</param>
        public ActuatorSwitchBase(IActuatorSwitch switchObj)
        {
            Name = switchObj.Name;
            Description = switchObj.Description;
            Source = switchObj.Source;
            _isActive = switchObj.IsActive;
            AcceptTime = switchObj.AcceptTime;
            Confidence = switchObj.Confidence;
            Timestamp = switchObj.Timestamp;
            Action = switchObj.Action;
            BeepFile = switchObj.BeepFile;
            Audio = switchObj.Audio;
            Actuator = switchObj.Actuator;
            _acceptTimer = switchObj.AcceptTimer;
            Actuate = switchObj.Actuate;
            Tag = switchObj.Tag;
            Command = switchObj.Command;
            Enabled = switchObj.Enabled;
        }

        /// <summary>
        /// Gets or sets the length of time (msecs) the switch to stay
        /// engaged for it to be recognized as a trigger.
        /// </summary>
        public int AcceptTime { get; set; }

        public Stopwatch AcceptTimer
        {
            get
            {
                return _acceptTimer;
            }
        }

        /// <summary>
        /// Gets or sets the Switch action
        /// </summary>
        public SwitchAction Action { get; set; }

        /// <summary>
        /// Gets or sets whether to actuate the switch or not
        /// </summary>
        public bool Actuate { get; set; }

        /// <summary>
        /// Gets or sets the parent Actuator of this switch
        /// </summary>
        public IActuator Actuator { get; set; }

        /// <summary>
        /// Gets the audio player to play the beep
        /// </summary>
        public SoundPlayer Audio
        {
            get
            {
                return _audio;
            }

            private set
            {
                _audio = value;
            }
        }

        /// <summary>
        /// Gets or sets name of the beep file associated wit this switch
        /// </summary>
        public String BeepFile { get; set; }

        /// <summary>
        /// Gets the command associated with this switch
        /// </summary>
        public String Command { get; private set; }

        /// <summary>
        /// Gets or sets the confidence level (unused now)
        /// </summary>
        public int Confidence { get; set; }

        /// <summary>
        /// Gets or sets the description for the switch
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// Gets or sets whether the switch is enabled or not
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the active state of the switch.
        /// </summary>
        public Boolean IsActive
        {
            get
            {
                lock (_lockObj)
                {
                    return _isActive;
                }
            }

            private set
            {
                lock (_lockObj)
                {
                    _isActive = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the switch
        /// </summary>
        public String Name
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the source of the switch tht caused the trigger.
        /// Depends on the type of the switch.  For instance, for
        /// a keyboard switch, source would be "F5" for the F5 function key.
        /// </summary>
        public String Source { get; set; }

        /// <summary>
        /// Gets or sets opaque tag
        /// </summary>
        public String Tag { get; set; }

        /// <summary>
        /// Gets or sets the time when the switch was triggered.
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// Disposes resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // Prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Returns the trigger scanmode for this switch if is configured as
        /// a scan trigger.  TriggerScanModes.None  if it is not configured
        /// as a scan trigger.
        /// </summary>
        /// <returns>The scan trigger mode</returns>
        public TriggerScanModes GetTriggerScanMode()
        {
            if (String.IsNullOrEmpty(Command))
            {
                return TriggerScanModes.None;
            }

            var cmd = (Command[0] == '@') ? Command.Substring(1) : Command;

            if (string.Compare(cmd, "CmdScanMoveLeft", true) == 0)
            {
                return TriggerScanModes.TriggerMoveLeft;
            }

            if (string.Compare(cmd, "CmdScanMoveRight", true) == 0)
            {
                return TriggerScanModes.TriggerMoveRight;
            }

            if (string.Compare(cmd, "CmdScanMoveUp", true) == 0)
            {
                return TriggerScanModes.TriggerMoveUp;
            }

            if (string.Compare(cmd, "CmdScanMoveDown", true) == 0)
            {
                return TriggerScanModes.TriggerMoveDown;
            }

            if (string.Compare(cmd, "CmdScanBeginLeft", true) == 0)
            {
                return TriggerScanModes.TriggerScanLeft;
            }

            if (string.Compare(cmd, "CmdScanBeginRight", true) == 0)
            {
                return TriggerScanModes.TriggerScanRight;
            }

            if (string.Compare(cmd, "CmdScanBeginUp", true) == 0)
            {
                return TriggerScanModes.TriggerScanUp;
            }

            if (string.Compare(cmd, "CmdScanBeginDown", true) == 0)
            {
                return TriggerScanModes.TriggerScanDown;
            }

            if (string.Compare(cmd, "CmdScanStop", true) == 0)
            {
                return TriggerScanModes.TriggerStop;
            }

            if (string.Compare(cmd, "CmdScanPause", true) == 0)
            {
                return TriggerScanModes.TriggerPause;
            }

            if (string.Compare(cmd, "CmdScanResume", true) == 0)
            {
                return TriggerScanModes.TriggerResume;
            }

            if (string.Compare(cmd, "CmdScanPauseToggle", true) == 0)
            {
                return TriggerScanModes.TriggerPauseToggle;
            }

            return TriggerScanModes.None;
        }

        /// <summary>
        /// Override this to do init for derived classes
        /// </summary>
        /// <returns>true on success</returns>
        public virtual bool Init()
        {
            return true;
        }

        /// <summary>
        /// Returns true if the switch is configured as a
        /// trigger to select a highlighed widget
        /// </summary>
        /// <returns>true if it is</returns>
        public bool IsSelectTriggerSwitch()
        {
            return string.Compare(Command, TriggerCommand, true) == 0;
        }

        /// <summary>
        /// Reads switch attributes from the XML file.
        /// </summary>
        /// <param name="xmlNode">The XML node that contains the Switch attributes</param>
        /// <returns>True on successful parse, false otherwise</returns>
        public virtual bool Load(SwitchSetting switchSetting)
        {
            Name = switchSetting.Name;
            Source = switchSetting.Source;
            Description = switchSetting.Description;
            Actuate = switchSetting.Actuate;
            Enabled = switchSetting.Enabled;
            AcceptTime = CoreGlobals.AppPreferences.ResolveVariableInt(switchSetting.MinHoldTime, CoreGlobals.AppPreferences.MinActuationHoldTime, 0);

            if (!String.IsNullOrEmpty(switchSetting.BeepFile))
            {
                var beepFile = FileUtils.GetSoundPath(switchSetting.BeepFile);
                if (File.Exists(beepFile))
                {
                    try
                    {
                        _audio = new SoundPlayer(beepFile);
                        BeepFile = beepFile;
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            Command = switchSetting.Command;

            return true;
        }

        /// <summary>
        /// Records the fact that a switch down was detected.  Starts
        /// a timer to track the MinActuationHoldTime
        /// </summary>
        public void RegisterSwitchDown()
        {
            IsActive = true;
        }

        /// <summary>
        /// Records the fact the swtich up was dected. Stos the
        /// accept timer
        /// </summary>
        public void RegisterSwitchUp()
        {
            IsActive = false;
        }

        /// <summary>
        /// Disposer. Release resources and cleanup.
        /// </summary>
        /// <param name="disposing">true to dispose managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!_disposed)
            {
                Log.Debug();

                if (disposing)
                {
                    if (_timer != null)
                    {
                        _timer.Dispose();
                        _timer = null;
                    }

                    if (_acceptTimer != null)
                    {
                        _acceptTimer.Stop();
                        _acceptTimer = null;
                    }

                    if (_audio != null)
                    {
                        _audio.Dispose();
                        _audio = null;
                    }
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }
    }
}