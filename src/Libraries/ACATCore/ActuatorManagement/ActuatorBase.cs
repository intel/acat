////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ActuatorBase.cs
//
// Base class for all the actuators.  Actuators are input mechanisms
// to the application.  An actuator contains a list
// of switches, each of which act as a trigger to drive the UI. For
// instance, a keyboard actuator will use input from the keyboard as
// triggers.  Soft actuators can also be implemented that use sockets
// to send triggers to the UI.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.Onboarding;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PreferencesManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;

namespace ACAT.Lib.Core.ActuatorManagement
{
    /// <summary>
    /// Base class for all the actuators.  Actuators are input mechanisms
    /// to the application.  An actuator contains a list
    /// of switches, each of which act as a trigger to drive the UI. For
    /// instance, a keyboard actuator will use input from the keyboard as
    /// triggers.  Soft actuators can also be implemented that use sockets
    /// to send triggers to the UI.
    /// </summary>
    public abstract class ActuatorBase : IActuator
    {
        /// <summary>
        /// Used to invoke methods/properties in the actuator
        /// </summary>
        private readonly ExtensionInvoker _invoker;

        /// <summary>
        /// A list of switches defined for this actuator.  Each switch has a
        /// name that is unique to the actuator
        /// </summary>
        private readonly Dictionary<String, IActuatorSwitch> _switches;

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the ActuatorBase class
        /// </summary>
        protected ActuatorBase()
        {
            Enabled = false;
            _switches = new Dictionary<String, IActuatorSwitch>();
            actuatorState = State.Stopped;
            _invoker = new ExtensionInvoker(this);
        }

        /// <summary>
        /// Raised when the actuator wants to send custom data to the application
        /// </summary>
        public event IoctlResponse EvtIoctlResponse;

        /// <summary>
        /// Triggered when one of the switches in this actuator is engaged.
        /// </summary>
        public event SwitchActivated EvtSwitchActivated;

        /// <summary>
        /// Triggered when one of the switches in this actuator is disengaged
        /// </summary>
        public event SwitchDeactivated EvtSwitchDeactivated;

        /// <summary>
        /// Raised when one of the switches in this actuator is triggered (engaged
        /// followed by a disengaged)
        /// </summary>
        public event SwitchTriggered EvtSwitchTriggered;

        /// <summary>
        /// Gets the descriptor for the actuator class
        /// </summary>
        public virtual IDescriptor Descriptor
        {
            get { return DescriptorAttribute.GetDescriptor(GetType()); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the actuator is enabled or not.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the name of the actuator
        /// </summary>
        public String Name
        {
            get { return Descriptor.Name; }
        }

        public virtual String OnboardingImageFileName
        {
            get
            {
                return String.Empty;
            }
        }

        public virtual bool ShowTryoutOnStartup
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets whether this supports a custom settings dialog
        /// </summary>
        public virtual bool SupportsPreferencesDialog
        {
            get { return false; }
        }

        public virtual bool SupportsScanTimingsConfigureDialog
        {
            get
            {
                return false;
            }
        }

        public virtual bool SupportsTryout
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the list of switches that are a part of this actuator
        /// </summary>
        public ICollection<IActuatorSwitch> Switches
        {
            get { return _switches.Values; }
        }

        /// <summary>
        /// Gets or sets the current state of the actuator
        /// </summary>
        protected State actuatorState { get; set; }

        /// <summary>
        /// Class factory to create a switch.  Override this in the
        /// derived classes to enable creating switches that are specific to the
        /// actuator
        /// </summary>
        /// <returns>Switch object</returns>
        public abstract IActuatorSwitch CreateSwitch();

        /// <summary>
        /// Creates a switch object using the specified switch object
        /// as the source.  Override this to create your specific switch object
        /// </summary>
        /// <param name="sourceSwitch">source switch object</param>
        /// <returns>created object</returns>
        public virtual IActuatorSwitch CreateSwitch(IActuatorSwitch sourceSwitch)
        {
            return new ActuatorSwitchBase(sourceSwitch);
        }

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
        /// Override this to returns the default preferences for the actuator
        /// </summary>
        /// <returns>default preferences</returns>
        public virtual IPreferences GetDefaultPreferences()
        {
            return null;
        }

        /// <summary>
        /// Returns invoker used to access methods and properties through
        /// reflection
        /// </summary>
        /// <returns></returns>
        public virtual ExtensionInvoker GetInvoker()
        {
            return _invoker;
        }

        public virtual IOnboardingExtension GetOnboardingExtension()
        {
            return null;
        }

        /// <summary>
        /// Returns the preferences object for the actuator
        /// </summary>
        /// <returns>preferences object</returns>
        public virtual IPreferences GetPreferences()
        {
            return null;
        }

        /// <summary>
        /// Returns the current state of the actuator
        /// </summary>
        /// <returns>Current state</returns>
        public State GetState()
        {
            return actuatorState;
        }

        public virtual IEnumerable<String> GetSupportedKeyboardConfigs()
        {
            return null;
        }

        /// <summary>
        /// Allow derived classes to allocate resources
        /// </summary>
        /// <returns>true on success</returns>
        public virtual bool Init()
        {
            ActuatorManager.Instance.OnInitDone(this);

            return true;
        }

        /// <summary>
        /// Invoked by an application to send custom data to the actuator
        /// </summary>
        /// <param name="opcode">operation code</param>
        /// <param name="request">data (typically JSON string)</param>
        /// <returns>true on success</returns>
        public virtual bool IoctlRequest(int opcode, String request)
        {
            return true;
        }

        /// <summary>
        /// Loads switch settings from the specified settings
        /// object. Creates the switches with attributes from the
        /// switchSettings and adds the switches to the _switches
        /// dictionary
        /// </summary>
        /// <param name="switchSettings">Settings for switches</param>
        /// <returns>true on success, false otherwise</returns>
        public bool Load(IEnumerable<SwitchSetting> switchSettings)
        {
            // enumerate the switches in this actuator and create
            // each switch object using the switch ClassFactory

            Log.Debug("Loading switches");
            foreach (var switchSetting in switchSettings)
            {
                var actuatorSwitch = CreateSwitch();
                if (actuatorSwitch != null)
                {
                    Log.Debug("name=" + switchSetting.Name);
                    if (!_switches.ContainsKey(switchSetting.Name))
                    {
                        if (actuatorSwitch.Load(switchSetting) && actuatorSwitch.Init())
                        {
                            Log.Debug("Adding switch " + actuatorSwitch.Name);
                            actuatorSwitch.Actuator = this;
                            _switches.Add(actuatorSwitch.Name, actuatorSwitch);
                        }
                    }
                    else
                    {
                        Log.Error("Warning.  Switch " + actuatorSwitch.Name + " defined more than once");
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Invoked when the user presses the button on the
        /// calibration dialog
        /// </summary>
        public virtual void OnCalibrationAction()
        {
        }

        /// <summary>
        /// Invoked if the calibration is canceled before the time period
        /// expires
        /// </summary>
        public virtual void OnCalibrationCanceled()
        {
        }

        /// <summary>
        /// If the calibration is required to complete in a specific time,
        /// this function is invoked when the timer expires
        /// </summary>
        public virtual void OnCalibrationPeriodExpired()
        {
        }

        /// <summary>
        /// Invoked when the application quits
        /// </summary>
        public virtual void OnQuitApplication()
        {
        }

        /// <summary>
        /// This function is invoked to enable the actuator to
        /// register its switches
        /// </summary>
        public virtual void OnRegisterSwitches()
        {
        }

        /// <summary>
        /// Pauses actuator.  No events will be received from the actuator
        /// when paused
        /// </summary>
        public virtual void Pause()
        {
        }

        public virtual bool PostInit()
        {
            ActuatorManager.Instance.OnPostInitDone(this);

            return true;
        }

        /// <summary>
        /// Removes the switch from the list of switches for this
        /// actuator
        /// </summary>
        /// <param name="switchName">name of switch to remove</param>
        /// <returns>true on success</returns>
        public bool RemoveSwitch(String switchName)
        {
            bool retVal = _switches.ContainsKey(switchName);
            if (retVal)
            {
                _switches.Remove(switchName);
            }

            return retVal;
        }

        /// <summary>
        /// Resumes actuator.  Will start sending events
        /// </summary>
        public virtual void Resume()
        {
        }

        /// <summary>
        /// Shows the preferences dialog
        /// </summary>
        /// <returns>true on success</returns>
        public virtual bool ShowPreferencesDialog()
        {
            return true;
        }

        public virtual bool ShowScanTimingsConfigureDialog()
        {
            return false;
        }

        public virtual bool ShowTryoutDialog()
        {
            return false;
        }

        /// <summary>
        /// Starts calibration of the actuator
        /// </summary>
        public virtual void StartCalibration(RequestCalibrationReason reason)
        {
        }

        /// <summary>
        /// Indicates whether the actuator supports calibration or not
        /// </summary>
        /// <returns>true if it does, false otherwise</returns>
        public virtual bool SupportsCalibration()
        {
            return false;
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
                    // dispose all managed resources.
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        /// Parses the "action" string and returns the corresponding
        /// enum for SwitchAction
        /// </summary>
        /// <param name="action">string to parse</param>
        /// <returns>enum value</returns>
        protected SwitchAction getSwitchAction(String action)
        {
            var retVal = SwitchAction.Unknown;
            try
            {
                retVal = (SwitchAction)Enum.Parse(typeof(SwitchAction), action, true);
            }
            catch (Exception e)
            {
                Log.Warn("VisionAcutator switch, invalid action specified " + action);
                Log.Exception(e);
            }

            return retVal;
        }

        /// <summary>
        /// Looks up the list of switches for a matching gesture and returns
        /// a clone of the matching switch object
        /// </summary>
        /// <param name="gesture">The gesture string</param>
        /// <param name="switches">Collection of switches for the actuator</param>
        /// <returns></returns>
        protected IActuatorSwitch getSwitchForGesture(
                                            String gesture,
                                            IEnumerable<IActuatorSwitch> switches)
        {
            foreach (var switchObj in switches)
            {
                var imageSwitch = switchObj;
                if (String.Compare(imageSwitch.Source, gesture, true) == 0)
                {
                    Log.Debug("Found switch object " + switchObj.Name + " for gesture" + gesture);
                    return CreateSwitch(switchObj);
                }
            }

            return null;
        }

        /// <summary>
        /// The actuator should invoke this function to indicate end of calibration
        /// </summary>
        protected void OnEndCalibration()
        {
            ActuatorManager.Instance.OnEndCalibration(this);
        }

        /// <summary>
        /// Raise event that initialization completed
        /// </summary>
        protected void OnInitDone()
        {
            ActuatorManager.Instance.OnInitDone(this);
        }

        protected void OnPostInitDone(bool success = true)
        {
            ActuatorManager.Instance.OnPostInitDone(this, success);
        }

        /// <summary>
        /// Helper function to trigger an event that a switch was engaged.  This
        /// function is called by the derived classes
        /// </summary>
        /// <param name="switchObj">Switch that caused the event</param>
        protected virtual void OnSwitchActivated(IActuatorSwitch switchObj)
        {
            switchObj.Action = SwitchAction.Down;

            if (EvtSwitchActivated != null)
            {
                EvtSwitchActivated(this, new ActuatorSwitchEventArgs(switchObj));
            }
        }

        /// <summary>
        /// Helper function to trigger an event that a switch was disengaged.  This
        /// function is called by the derived classes
        /// </summary>
        /// <param name="switchObj">Switch that caused the event</param>
        protected virtual void OnSwitchDeactivated(IActuatorSwitch switchObj)
        {
            switchObj.Action = SwitchAction.Up;

            if (EvtSwitchDeactivated != null)
            {
                EvtSwitchDeactivated(this, new ActuatorSwitchEventArgs(switchObj));
            }
        }

        /// <summary>
        /// Helper function to trigger an event that a switch was triggered.  This
        /// function is called by the derived classes
        /// </summary>
        /// <param name="switchObj">Switch that caused the event</param>
        protected virtual void OnSwitchTriggered(IActuatorSwitch switchObj)
        {
            switchObj.Action = SwitchAction.Trigger;

            if (EvtSwitchActivated != null)
            {
                EvtSwitchTriggered(this, new ActuatorSwitchEventArgs(switchObj));
            }
        }

        /// <summary>
        /// Helper function to parse actuator trigger message that can be sent
        /// by some input sensors.  The message is in the form of a string.
        /// After parsing the string, creates an Actuator switch object that
        /// correponds to the gesture info in the string
        /// Format of the string is:
        ///    gesture=gesturetype;action=gestureevent;conf=confidence;time=timestamp;actuate=flag;tag=userdata
        /// where
        ///  gesturetype    is a string representing the gesture. This is used as
        ///                 the 'source' field in the actuator switch object
        ///  gestureevent   should be a valid value from the SwitchAction enum
        ///  confidence     Integer representing the confidence level, for future use
        ///  timestamp      Timestamp of when the switch event triggered (in ticks)
        ///  flag           true/false.  If false, the switch trigger event will be ignored
        ///  userdata       Any user data
        /// Eg
        ///    gesture=G1;action=trigger;conf=75;time=3244394443
        /// </summary>
        /// <param name="strData">input string to parse</param>
        /// <param name="parsedGesture">gesture contained in the string</param>
        /// <returns>switch object for the gesture, null if not found</returns>
        protected IActuatorSwitch parseActuatorMsgAndGetSwitch(String strData, ref String parsedGesture)
        {
            IActuatorSwitch actuatorSwitch = null;
            String gesture = String.Empty;
            var switchAction = SwitchAction.Unknown;
            String tag = String.Empty;
            int confidence = -1;
            long time = -1;
            bool actuate = true;
            parsedGesture = String.Empty;

            Log.Debug(strData);

            var tokens = strData.Split(';');
            foreach (var token in tokens)
            {
                String[] nameValue = token.Split('=');
                if (nameValue.Length == 2)
                {
                    switch (nameValue[0])
                    {
                        case "gesture":  // G1, G2, ...
                            gesture = nameValue[1];
                            parsedGesture = gesture;
                            break;

                        case "action": // Up, Down, Trigger
                            switchAction = getSwitchAction(nameValue[1]);
                            break;

                        case "time":  // in Ticks
                            time = parseLong(nameValue[1]);
                            break;

                        case "confidence":  // integer 0 to 100
                            confidence = (int)parseLong(nameValue[1]);
                            break;

                        case "actuate":
                            actuate = String.Compare(nameValue[1], "true", true) == 0;
                            break;

                        case "tag":
                            tag = nameValue[1];
                            break;
                    }
                }
            }

            if (!String.IsNullOrEmpty(gesture) && switchAction != SwitchAction.Unknown)
            {
                actuatorSwitch = getSwitchForGesture(gesture, Switches);
                if (actuatorSwitch != null)
                {
                    actuatorSwitch.Source = gesture;
                    actuatorSwitch.Action = switchAction;
                    actuatorSwitch.Confidence = confidence;
                    actuatorSwitch.Timestamp = time;
                    actuatorSwitch.Actuate = actuate;
                    actuatorSwitch.Tag = tag;
                }
            }

            return actuatorSwitch;
        }

        /// <summary>
        /// The Actuator should call this function to request that it
        /// wants calibration done.
        /// </summary>
        protected void RequestCalibration(RequestCalibrationReason reason)
        {
            ActuatorManager.Instance.RequestCalibration(this, reason);
        }

        /// <summary>
        /// Sends a response back to the client through an event
        /// </summary>
        /// <param name="opcode">operation code</param>
        /// <param name="response">data to be sent</param>
        protected virtual void SendIoctlResponse(int opcode, String response)
        {
            if (EvtIoctlResponse != null)
            {
                EvtIoctlResponse(opcode, response);
            }
        }

        protected bool ShowDefaultScanTimingsConfigureDialog()
        {
            if (String.IsNullOrEmpty(CoreGlobals.AppPreferences.DefaultScanTimingsConfigurePanelName))
            {
                return false;
            }

            var form = PanelManager.Instance.CreatePanel(CoreGlobals.AppPreferences.DefaultScanTimingsConfigurePanelName, "Adjust Scanning Speed");
            if (form != null)
            {
                Context.AppPanelManager.ShowDialog(form as IPanel);
                return true;
            }

            return false;
        }

        protected bool ShowDefaultTryoutDialog()
        {
            if (String.IsNullOrEmpty(CoreGlobals.AppPreferences.DefaultTryoutPanelName))
            {
                return false;
            }

            var form = PanelManager.Instance.CreatePanel(CoreGlobals.AppPreferences.DefaultTryoutPanelName, "Switch Tryout");
            if (form != null)
            {
                Context.AppPanelManager.ShowDialog(form as IPanel);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Updates the calibration form with the prompt, caption and timeout.  If timeout
        /// is > 0, it is the time period for calibration.  If timeout is -1, it counts the
        /// elapsed time in seconds and if timeout is 0, the timer is not used
        /// </summary>
        /// <param name="caption">caption to display on the calibration form</param>
        /// <param name="prompt">any message to display</param>
        /// <param name="timeout">calibration timeout</param>
        /// <param name="enableConfigure">should the configure button b e enabled</param>
        protected void UpdateCalibrationStatus(String caption, String prompt, int timeout = 0, bool enableConfigure = true, string buttonText = "")
        {
            Log.Debug("Calling ActuatorManager.Instance.UpdateCalibrationStatus");
            ActuatorManager.Instance.UpdateCalibrationStatus(this, caption, prompt, timeout, enableConfigure, buttonText);
        }

        /// <summary>
        /// Parses a long (as string) and returns value
        /// </summary>
        /// <param name="val">string representation</param>
        /// <returns>long value</returns>
        private long parseLong(String val)
        {
            long retVal = -1;
            try
            {
                retVal = Convert.ToInt32(val);
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }

            return retVal;
        }
    }
}