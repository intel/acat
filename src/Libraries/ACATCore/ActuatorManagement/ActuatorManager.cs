////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ActuatorManager.cs
//
// Manages all the actuators.  Responsible for reading the
// ActuatorSettings config file, parsing it, and creating the
// actuators through the Actuators class.  This class
// receives all the switch trigger events, maintains a list
// of switches that are currently held, checks when the
// switches are released, whether the hold time was >
// than the accept time and if so, trigger the switch events.
// This is a singleton class
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Audit;
using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.InputActuators;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PreferencesManagement;
using ACAT.Lib.Core.UserManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ACAT.Lib.Core.ActuatorManagement
{
    /// <summary>
    /// Manages all the actuators.  Responsible for reading the
    /// ActuatorSettings config file, parsing it, and creating the
    /// actuators through the Actuators class.  This class
    /// receives all the switch trigger events, maintains a list
    /// of switches that are currently held, checks when the
    /// switches are released, whether the hold time was >
    /// than the accept time and if so, trigger the switch events.
    /// This is a singleton class
    /// </summary>
    public class ActuatorManager : IDisposable
    {
        /// <summary>
        /// The base directory under which all the actuator Dll's are
        /// located.
        /// </summary>
        public static String ActuatorsRootDir = "Actuators";

        /// <summary>
        /// Input config file that contains a list of all actuators and
        /// the switches for each actuator.
        /// </summary>
        public const String ActuatorSettingsFileName = "ActuatorSettings.xml";

        /// <summary>
        /// Singleton instance
        /// </summary>
        private static ActuatorManager _instance;

        /// <summary>
        /// List of switches that are currentlu active i.e., those for which
        /// a "switch down" event has been raised
        /// </summary>
        private Dictionary<String, IActuatorSwitch> _activeSwitches;

        /// <summary>
        /// List of switches whose 'actuate' attribute is false.  These switches
        /// will not trigger an action.  Their actions will be merely audited in
        /// the audit log
        /// </summary>
        private Dictionary<String, IActuatorSwitch> _nonActuateSwitches;

        /// <summary>
        /// Object to synchronize multithread access
        /// </summary>
        private Object _syncObjectSwitches;

        /// <summary>
        /// Queue to hold calibration requests from actuators
        /// </summary>
        private BlockingQueue<object> calibrationQueue;

        /// <summary>
        /// Maintains a list of actuators
        /// </summary>
        private Actuators _actuators;

        /// <summary>
        /// Actuator being currently calibrated
        /// </summary>
        private ActuatorEx _calibratingActuatorEx;

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Exit the calibration handler thread
        /// </summary>
        private volatile bool _exitThread = false;

        /// <summary>
        /// Is initialization in progress?
        /// </summary>
        private bool _initInProgress;

        /// <summary>
        /// Calibration handler thread
        /// </summary>
        private Thread _thread;

        
        /// <summary>
        /// Prevents a default instance of ActuatorManager class from being created
        /// </summary>
        private ActuatorManager()
        {
            //_activeSwitches = new Dictionary<String, IActuatorSwitch>();
            //_nonActuateSwitches = new Dictionary<String, IActuatorSwitch>();
            //_syncObjectSwitches = new object();
        }

        /// <summary>
        /// Deleagate for notification of start of calibration by an actuator
        /// </summary>
        /// <param name="args">Calibration notifaction object</param>
        public delegate void CalibrationStartNotify(CalibrationNotifyEventArgs args);

        /// <summary>
        /// Event raised to indicate start of calibration
        /// </summary>
        public event CalibrationStartNotify EvtCalibrationStartNotify;

        /// <summary>
        /// Event raised to indicate end of calibration
        /// </summary>
        public event EventHandler EvtCalibrationEndNotify;

        /// <summary>
        /// For events related to actuator switch triggers
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        public delegate void ActuatorSwitchEvent(object sender, ActuatorSwitchEventArgs e);

        /// <summary>
        /// For the hook event to get actuator events
        /// </summary>
        /// <param name="switchObj">actuator switch</param>
        /// <param name="handled">true if handled</param>
        public delegate void SwitchHook(IActuatorSwitch switchObj, ref bool handled);

        /// <summary>
        /// Event is trigged when a switch is accepted
        /// </summary>
        public event ActuatorSwitchEvent EvtSwitchAccepted;

        /// <summary>
        /// Raised when a switch is activated
        /// </summary>
        public event ActuatorSwitchEvent EvtSwitchActivated;

        /// <summary>
        /// Event is trigged when a switch is down
        /// </summary>
        public event ActuatorSwitchEvent EvtSwitchDown;

        /// <summary>
        /// Hook event to allow apps to acces switch events
        /// </summary>
        public event SwitchHook EvtSwitchHook;

        /// <summary>
        /// Event is trigged when a switch is rejected
        /// </summary>
        public event ActuatorSwitchEvent EvtSwitchRejected;

        /// <summary>
        /// Event is trigged when a switch is up
        /// </summary>
        public event ActuatorSwitchEvent EvtSwitchUp;

        /// <summary>
        /// Gets the singleton instance of the Actuator Manager object
        /// </summary>
        public static ActuatorManager Instance
        {
            get { return _instance ?? (_instance = new ActuatorManager()); }
        }

        /// <summary>
        /// Gets the object that contains a list of actuators
        /// </summary>
        public IEnumerable<IActuator> Actuators
        {
            get { return _actuators.ActuatorList; }
        }

        /// <summary>
        /// Gets the switch configuration map for the user
        /// </summary>
        //public SwitchConfig SwitchConfigMap { get; private set; }

        /// <summary>
        /// Dispose resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // Prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Returns the actuator object for the specified type
        /// </summary>
        /// <param name="actuatorType">Type of actuator</param>
        /// <returns>The actuator object</returns>
        public IActuator GetActuator(Type actuatorType)
        {
            return _actuators.Find(actuatorType);
        }

        /// <summary>
        /// Returns the acutator object corresponding to
        /// the specified GUID
        /// </summary>
        /// <param name="id">ID of the acutator</param>
        /// <returns>actuator object, null if not found</returns>
        public IActuator GetActuator(Guid id)
        {
            foreach (var actuator in _actuators.ActuatorList)
            {
                var descAttribute = DescriptorAttribute.GetDescriptor(actuator.GetType());

                if (descAttribute != null)
                {
                    if (id.Equals(descAttribute.Id))
                    {
                        return actuator;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Initializes the manager.
        /// </summary>
        /// <param name="extensionDirs">Directories to search</param>
        /// <returns>true on success</returns>
        public bool Init(IEnumerable<String> extensionDirs)
        {
            _activeSwitches = new Dictionary<String, IActuatorSwitch>();
            _nonActuateSwitches = new Dictionary<String, IActuatorSwitch>();
            _syncObjectSwitches = new object();


            calibrationQueue = new BlockingQueue<object>();

            _exitThread = false;

            _initInProgress = true;

            _thread = new Thread(calibrationHandlerThread) { IsBackground = true };
            _thread.SetApartmentState(ApartmentState.STA);
            _thread.Start();

            bool retVal = init();

            _initInProgress = false;

            Context.AppPanelManager.EvtAppQuit += AppPanelManager_EvtAppQuit;

            return retVal;
        }

        public bool PostInit()
        {
            bool retVal = true;

            foreach (var actuatorEx in _actuators.Collection)
            {
                actuatorEx.PostInit();
                if (actuatorEx.PostInitError)
                {
                    retVal = false;
                }
            }

            foreach (var actuatorEx in _actuators.Collection)
            {
                actuatorEx.WaitForCalibration();
            }

            return retVal;
        }

        /// <summary>
        /// Checks if any switch is currently engaged
        /// </summary>
        /// <returns>true on success</returns>
        public bool IsSwitchActive()
        {
            int count;
            lock (_syncObjectSwitches)
            {
                count = _activeSwitches.Count;
            }

            return count > 0;
        }

        /// <summary>
        /// Loads actuator DLL's.
        /// The extension dirs parameter contains the root directory under
        /// which to search for Actuator DLL files.  The directories
        /// are specified in a comma delimited fashion.
        /// E.g.  Default, SomeDir
        /// These are relative to the application execution directory or
        /// to the directory where the ACAT framework has been installed.
        /// It recusrively walks the directories and looks for Actuator
        /// extension DLL files
        /// </summary>
        /// <param name="extensionDirs">Directories to search</param>
        /// <returns>true on success</returns>
        public bool LoadExtensions(IEnumerable<String> extensionDirs, bool all = false)
        {
            bool retVal = true;

            if (_actuators == null)
            {
                String configFile = UserManager.GetFullPath(ActuatorSettingsFileName);
                _actuators = new Actuators();
                retVal = _actuators.Load(extensionDirs, configFile, all);
            }

            return retVal;
        }

        /// <summary>
        /// Invoked when calibration is canceled for the
        /// specified actuator
        /// </summary>
        /// <param name="source">source actuator</param>
        public void OnCalibrationCanceled(IActuator source)
        {
            source.OnCalibrationCanceled();

            Log.Debug("Entered ActuatorManger.OnCalibrationCanceled");
            if (isCalibratingActuator(source))
            {
                source.OnCalibrationCanceled();

                _calibratingActuatorEx.OnEndCalibration();
                _calibratingActuatorEx = null;
            }
        }

        /// <summary>
        /// Invoked if the calibration has a timeout period and the
        /// period expires.
        /// </summary>
        /// <param name="source">Actuator being calibrated</param>
        public void OnCalibrationPeriodExpired(IActuator source)
        {
            source.OnCalibrationPeriodExpired();
        }

        /// <summary>
        /// The actuator should invoke this function to indicate end
        /// of calibration.
        /// </summary>
        /// <param name="source">actuator being calibrated</param>
        /// <param name="errorMessage">any error during calibration?</param>
        /// <param name="enableConfigure">should the "configure" be enabled</param>
        public void OnEndCalibration(IActuator source, String errorMessage = "", bool enableConfigure = true)
        {
            Log.Debug();
            Log.Debug("Calling isCalibratingActuator");

            if (isCalibratingActuator(source))
            {
                _calibratingActuatorEx.OnEndCalibration(errorMessage, enableConfigure);

                Log.Debug("Setting calibratingActuatorEx to null");
                _calibratingActuatorEx = null;
            }
        }

        /// <summary>
        /// Actuator can invoke this function to indicate any error.  The error
        /// dialog is displayed
        /// </summary>
        /// <param name="source">source acutator that had the error</param>
        /// <param name="message">error message</param>
        /// <param name="enableConfigure">should "configure" button be enabled?</param>
        public void OnError(IActuator source, String message, bool enableConfigure = true)
        {
            var aex = _actuators.find(source);
            if (aex != null)
            {
                aex.OnError(message, enableConfigure);
            }
        }

        /// <summary>
        /// Actuator should invoke this to indicate asyc initialization has
        /// ended
        /// </summary>
        /// <param name="source">The source actuator</param>
        /// <param name="success">was init successful?</param>
        public void OnInitDone(IActuator source, bool success = true)
        {
            var aex = _actuators.find(source);
            if (aex != null)
            {
                aex.OnInitDone(success);
            }
        }

        public void OnPostInitDone(IActuator source, bool success = true)
        {
            var aex = _actuators.find(source);
            if (aex != null)
            {
                aex.OnPostInitDone(success);
            }
        }


        /// <summary>
        /// Pauses all the acutators.  No events will be triggered
        /// </summary>
        public void Pause()
        {
            foreach (var actuator in _actuators.Collection)
            {
                actuator.SourceActuator.Pause();
            }
        }

        /// <summary>
        /// Adds the specified switch to the list of switches supported
        /// by the actuator. switchSetting contains all the attributes of the
        /// switch.  The swtich is added to the actuators settings file and the
        /// file saved.
        /// </summary>
        /// <param name="actuator">Actuator to add to</param>
        /// <param name="switchSetting">Settings for the switch</param>
        /// <returns>true on success, false if actuator not found</returns>
        public bool RegisterSwitch(IActuator actuator, SwitchSetting switchSetting)
        {
            if (_actuators.Find(actuator.GetType()) == null)
            {
                return false;
            }

            return _actuators.AddSwitch(actuator, switchSetting);
        }

        public IActuator GetCalibrationSupportedActuator()
        {
            foreach (var actuator in _actuators.Collection)
            {
                if (actuator.SourceActuator.SupportsCalibration())
                {
                    return actuator.SourceActuator;
                }
            }

            return null;
        }

        /// <summary>
        /// All calibration requests are queued and handled in the order
        /// they were requested.  Actuators should call this function to
        /// indicate that they want to calibrate
        /// </summary>
        /// <param name="source">the requesting actuator</param>
        public void RequestCalibration(IActuator source, RequestCalibrationReason reason)
        {
            Log.Debug("Entered RequestCalibration for " + source.Name);

            if (!source.SupportsCalibration())
            {
                Log.Debug(source.Name + ": Does not support calibration. returning");
                return;
            }

            if (calibrationQueue.Contains(source) || isCalibratingActuator(source))
            {
                Log.Debug("Already queued up or currently processing. Will not enqueue for " + source.Name);
                return;
            }

            var aex = _actuators.find(source);

            if (aex != null)
            {
                aex.RequestCalibration(reason);
                Log.Debug("Enqueing calibration request for " + source.Name);

                calibrationQueue.Enqueue(aex);
            }
        }

        /// <summary>
        /// Resumes all the actuators.
        /// </summary>
        public void Resume()
        {
            foreach (var actuator in _actuators.Collection)
            {
                actuator.SourceActuator.Resume();
            }
        }

        /// <summary>
        /// Get actuator configuration
        /// </summary>
        /// <returns></returns>
        public ActuatorConfig GetActuatorConfig()
        {
            ActuatorConfig.ActuatorSettingsFileName = UserManager.GetFullPath(ActuatorSettingsFileName);
            return ActuatorConfig.Load();
        }

        /// <summary>
        /// Returns form that displays preferences selection form for actuators and allows configuration.
        /// User can enable/disable actuators and also configure settings for each actuator.  
        /// </summary>
        public Form GetPreferencesSelectionForm(IntPtr parentControlHandle)
        {
            if (_actuators == null)
            {
                return null;
            }

            var list = new List<PreferencesCategory>();
            var keyboardActuator = ActuatorManager.Instance.GetActuator(typeof(KeyboardActuator));

            foreach (var actuator in _actuators.ActuatorList)
            {
                list.Add(new PreferencesCategory(actuator, true, actuator.Enabled));
            }

            // Create and return the form for the user to select which actuators are enabled, change settings etc.
            var form = new PreferencesCategorySelectForm
            {
                PreferencesCategories = list,
                CategoryColumnHeaderText = "Actuator",
                Title = "Actuators",
                ParentControlHandle = parentControlHandle,
                DisallowEnable = true
            };

            return form;
        }

        /// <summary>
        /// Saves preferences in actuator settings
        /// </summary>
        public void SavePreferences(object sender, IEnumerable<PreferencesCategory> preferencesCategories)
        {
            ActuatorConfig.ActuatorSettingsFileName = UserManager.GetFullPath(ActuatorSettingsFileName);
            var actuatorSettings = ActuatorConfig.Load();

            foreach (var category in preferencesCategories)
            {
                if (category.PreferenceObj is IExtension)
                {
                    var extension = category.PreferenceObj as IExtension;
                    var actuatorSetting = actuatorSettings.Find(extension.Descriptor.Id);
                    if (actuatorSetting != null)
                    {
                        actuatorSetting.Enabled = category.Enable;

                        foreach (var actuator in _actuators.ActuatorList.Where(actuator => Equals(actuatorSetting.Id, actuator.Descriptor.Id)))
                        {
                            actuator.Enabled = actuatorSetting.Enabled;
                        }
                    }
                }
            }

            actuatorSettings.Save();
        }

        /// <summary>
        /// Removes the switch from the actuator. The swtich is removed from the
        /// actuators settings file and the file saved.
        /// </summary>
        /// <param name="actuator"></param>
        /// <param name="switchName"></param>
        /// <returns></returns>
        public bool UnregisterSwitch(IActuator actuator, String switchName)
        {
            if (_actuators.Find(actuator.GetType()) == null)
            {
                return false;
            }

            return _actuators.RemoveSwitch(actuator, switchName);
        }

        /// <summary>
        /// Updates the calibration status in the calibration form.
        /// </summary>
        /// <param name="source">Source actuator</param>
        /// <param name="caption">caption for the form</param>
        /// <param name="prompt">message to display on the form</param>
        /// <param name="timeout">calibration timeout</param>
        /// <param name="enableConfigure">should "Configure" button be enabled?</param>
        /// <param name="buttonText">text of the calibration button</param>
        public void UpdateCalibrationStatus(IActuator source, String caption, String prompt, int timeout = 0, bool enableConfigure = true, String buttonText = "")
        {
            Log.Debug("Checking if isCalibrationg for " + source.Name);
            if (isCalibratingActuator(source))
            {
                Log.Debug("UpdateCalibStatus:  Yes it is!!. Calling calibratingActuatorEx.UpdateCalibrationStatus for " + source.Name);
                _calibratingActuatorEx.UpdateCalibrationStatus(getCalibrationFormPosition(), caption, prompt, timeout, enableConfigure, buttonText);
            }
            else
            {
                Log.Debug("isCalibrating returned False.  Actuator is NOT calibrating for " + source.Name);
            }
        }

        /// <summary>
        /// Call this to notify that calibration is about to start
        /// </summary>
        /// <param name="args">Argument for the notifcation</param>
        public void NotifyStartCalibration(CalibrationNotifyEventArgs args)
        {
            EvtCalibrationStartNotify?.Invoke(args);
        }

        /// <summary>
        /// Call this to notify that calibartion has ended
        /// </summary>
        public void NotifyEndCalibration()
        {
            EvtCalibrationEndNotify?.Invoke(this, new EventArgs());
        }

        public IActuator GetKeyboardActuator()
        {
            return GetActuator(typeof(KeyboardActuator));
        }

        public IActuator GetSwitchInterfaceActuator()
        {
            return GetActuator(typeof(SwitchInterfaceActuator));
        }


        public bool CheckScanTimingConfigureEnable()
        {
            var keyboardActuator = Context.AppActuatorManager.GetKeyboardActuator();

            foreach (var actuator in Context.AppActuatorManager.Actuators)
            {
                if (keyboardActuator != null && actuator != keyboardActuator)
                {
                    if (actuator.SupportsScanTimingsConfigureDialog)
                    {
                        return true;
                    }
                }
            }

            if (keyboardActuator != null &&
                keyboardActuator.SupportsScanTimingsConfigureDialog)
            {
                return true;
            }

            return false;
        }


        public void ShowTryoutDialog(bool startup = false)
        {
            var keyboardActuator = Context.AppActuatorManager.GetKeyboardActuator();

            bool dialogShown = false;

            foreach (var actuator in Context.AppActuatorManager.Actuators)
            {
                if (keyboardActuator != null && actuator != keyboardActuator)
                {
                    if (actuator.SupportsTryout)
                    {
                        if (startup && !actuator.ShowTryoutOnStartup)
                        {
                            return;
                        }

                        if (actuator.ShowTryoutDialog())
                        {
                            dialogShown = true;
                        }

                        break;
                    }
                }
            }

            if (!dialogShown && Context.AppActuatorManager.Actuators.Count() == 1 && keyboardActuator != null &&
                keyboardActuator.SupportsTryout &&
               (!startup || keyboardActuator.ShowTryoutOnStartup))
            {
                keyboardActuator.ShowTryoutDialog();
            }
        }

        public void ShowScanTimingsConfigureDialog()
        {
            var keyboardActuator = Context.AppActuatorManager.GetKeyboardActuator();

            bool dialogShown = false;

            foreach (var actuator in Context.AppActuatorManager.Actuators)
            {
                if (keyboardActuator != null && actuator != keyboardActuator)
                {
                    if (actuator.SupportsScanTimingsConfigureDialog)
                    {
                        if (actuator.ShowScanTimingsConfigureDialog())
                        {
                            dialogShown = true;
                        }

                        break;
                    }
                }
            }

            if (!dialogShown && Context.AppActuatorManager.Actuators.Count() == 1 && keyboardActuator != null &&
                keyboardActuator.SupportsScanTimingsConfigureDialog)
               
            {
                keyboardActuator.ShowScanTimingsConfigureDialog();
            }
        }


        /// <summary>
        /// Action to execute when the user intitiates it
        /// </summary>
        /// <param name="source">source actuator</param>
        internal void OnCalibrationAction(IActuator source)
        {
            source.OnCalibrationAction();
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
                    _exitThread = true;

                    if (calibrationQueue != null)
                    {
                        calibrationQueue.Clear();
                        calibrationQueue.Enqueue("Stop");
                    }


                    if (_thread != null)
                    {
                        Log.Debug("Calling thread.join");
                        _thread.Join(2000);
                    }

                    Log.Debug("Exited thread");

                    if (_actuators != null)
                    {
                        _actuators.Dispose();
                        _actuators = null;
                    }
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        /// Trigger an event that this switch was triggered
        /// </summary>
        /// <param name="switchObj">Which switch caused the trigger</param>
        private void act(IActuatorSwitch switchObj)
        {
            bool handled = false;


            var actuator = switchObj.Actuator;
            var action = switchObj.GetTriggerScanMode();

            switch (action)
            {
                case TriggerScanModes.TriggerPause:
                    actuator.Pause();
                    handled = true;
                    break;

                case TriggerScanModes.TriggerResume:
                    actuator.Resume();
                    handled = true;
                    break;

                case TriggerScanModes.TriggerPauseToggle:
                    if (actuator.GetState() == State.Running)
                    {
                        actuator.Pause();
                        handled = true;
                    }
                    else
                    {
                        actuator.Resume();
                        handled = true;
                    }
                    break;
            }

            if (handled)
            {
                return;
            }

            if (actuator.GetState() != State.Running)
            {
                return;
            }
            notifySwitchHooks(switchObj, ref handled);

            if (!handled)
            {
                Log.Debug("ACT on Switch " + switchObj.Name);
                notifySwitchActivated(switchObj);
            }
        }

        /// <summary>
        /// Event triggered when a switch is engaged.  Note that multiple switches
        /// can get engaged at the same time.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event args</param>
        private void actuator_EvtSwitchActivated(object sender, ActuatorSwitchEventArgs e)
        {
            Log.Debug("Switch " + e.SwitchObj.Name + " activated");
            disambiguateAndAct(e.SwitchObj.Actuate ? _activeSwitches : _nonActuateSwitches, e.SwitchObj);
        }

        /// <summary>
        /// Event triggered when a switch is disengaged.  Note that multiple switches
        /// can get disengaged at the same time.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event args</param>
        private void actuator_EvtSwitchDeactivated(object sender, ActuatorSwitchEventArgs e)
        {
            Log.Debug("Switch " + e.SwitchObj.Name + " deactivated");
            //disambiguateAndAct(e.SwitchObj);
            disambiguateAndAct(e.SwitchObj.Actuate ? _activeSwitches : _nonActuateSwitches, e.SwitchObj);
        }

        /// <summary>
        /// Event raised when a switch is triggered (down and aup).
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event args</param>
        private void actuator_EvtSwitchTriggered(object sender, ActuatorSwitchEventArgs e)
        {
            Log.Debug("Switch " + e.SwitchObj.Name + " triggered");
            //disambiguateAndAct(e.SwitchObj);
            disambiguateAndAct(e.SwitchObj.Actuate ? _activeSwitches : _nonActuateSwitches, e.SwitchObj);
        }

        /// <summary>
        /// Event handler for when the application exits
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void AppPanelManager_EvtAppQuit(object sender, EventArgs e)
        {
            _actuators.OnAppQuit();
        }

        /// <summary>
        /// Looks at the queue that has calibration requests and handles
        /// them in the order they were received
        /// </summary>
        private void calibrationHandlerThread()
        {
            while (!_exitThread)
            {
                Log.Debug("Waiting for item");
                var obj = calibrationQueue.Dequeue();
                Log.Debug("Got item");

                if (obj is String)
                {
                    return;
                }

                if (obj is ActuatorEx)
                {
                    _calibratingActuatorEx = obj as ActuatorEx;
                    Log.Debug("Before start calib");
                    var actuator = _calibratingActuatorEx;
                    actuator.StartCalibration();
                    Log.Debug("after start calib");

                    Log.Debug("Waiting for calib for " + actuator.SourceActuator.Name);

                    actuator.WaitForCalibration();
                }
            }
            return;
        }

        /// <summary>
        /// Disambigates signals from various switches and acts upon them.  Disambiguation
        /// logic has not been implemented.  this is  TODO.
        /// Maintains a list of switches that are currently held down.  When the swithches
        /// are released, it checks to see how long they were held and then triggers an
        /// event.
        /// </summary>
        /// <param name="switches">switch collection</param>
        /// <param name="switchObj">Which switch to act on</param>
        private void disambiguateAndAct(Dictionary<String, IActuatorSwitch> switches, IActuatorSwitch switchObj)
        {
            IActuatorSwitch switchActivated = null;
            long elapsedTime = 0;

            if (!switchObj.Enabled)
            {
                Log.Debug("Switch " + switchObj.Name + " is not enabled. Will be ignored");
                return;
            }

            switch (switchObj.Action)
            {
                case SwitchAction.Down:
                    lock (switches)
                    {
                        if (!switches.ContainsKey(switchObj.Name))
                        {
                            Log.Debug("SWITCH DOWN switches does not contain " + switchObj.Name + ". adding it");

                            // add it to the current list of active switches
                            switches.Add(switchObj.Name, switchObj);

                            switchObj.AcceptTimer.Restart();

                            notifySwitchDown(switchObj);
                        }
                        else
                        {
                            Log.Debug("SWITCH DOWN switches already contains " + switchObj.Name);
                        }
                    }

                    break;

                case SwitchAction.Up:
                    // remove from the list of currently accepted switches
                    lock (switches)
                    {
                        switchObj.RegisterSwitchUp();
                        if (switches.ContainsKey(switchObj.Name))
                        {
                            Log.Debug("SWITCH UP switches contains " + switchObj.Name);
                            var activeSwitch = switches[switchObj.Name];

                            elapsedTime = (activeSwitch != null && activeSwitch.AcceptTimer.IsRunning) ?
                                                    activeSwitch.AcceptTimer.ElapsedMilliseconds : 0;

                            bool accepted = false;

                            Log.Debug("SWITCH UP Acauate: " + switchObj.Actuate);
                            Log.Debug("SWITCH UP Acticeswitch != null is " + (activeSwitch != null));

                            if (activeSwitch != null)
                            {
                                Log.Debug("SWITCH UP Accepttimer is running " + activeSwitch.AcceptTimer.IsRunning);
                                Log.Debug("SWITCH UP Elapsed milli: " + activeSwitch.AcceptTimer.ElapsedMilliseconds);
                            }

                            if (switchObj.Actuate &&
                                activeSwitch != null &&
                                activeSwitch.AcceptTimer.IsRunning &&
                                activeSwitch.AcceptTimer.ElapsedMilliseconds >= CoreGlobals.AppPreferences.MinActuationHoldTime)
                            {
                                Log.Debug("SWITCH UP Switch accepted! ElapsedMilliseconds: " + activeSwitch.AcceptTimer.ElapsedMilliseconds);
                                accepted = true;
                                switchActivated = switchObj;
                            }
                            else
                            {
                                Log.Debug("SWITCH UP Switch not found or actuate is false or timer not running or elapsedTime < accept time");
                            }

                            switches.Remove(switchObj.Name);

                            notifySwitchUp(switchObj);

                            if (accepted)
                            {
                                notifySwitchAccepted(switchObj);
                            }
                            else
                            {
                                notifySwitchRejected(switchObj);
                            }
                        }
                        else
                        {
                            Log.Debug("SWITCH UP switches does not contain " + switchObj.Name);
                        }
                    }

                    break;

                case SwitchAction.Trigger:
                    lock (switches)
                    {
                        if (switches.ContainsKey(switchObj.Name))
                        {
                            switches.Remove(switchObj.Name);
                        }
                    }

                    if (switchObj.Actuate)
                    {
                        notifySwitchAccepted(switchObj);
                        switchActivated = switchObj;
                    }

                    break;
            }

            AuditLog.Audit(new AuditEventSwitchActuate(switchObj.Name,
                                                        switchObj.Action.ToString(),
                                                        switchObj.Actuator.Name,
                                                        switchObj.Tag,
                                                        elapsedTime));

            if (switchActivated != null)
            {
                act(switchObj);
            }
        }

        /// <summary>
        /// Returns the position where the calibration form should be
        /// displayed.  This is so as to not conflict with the position
        /// of the scanner
        /// </summary>
        /// <returns>Position</returns>
        private Windows.WindowPosition getCalibrationFormPosition()
        {
            if (_initInProgress)
            {
                return Windows.WindowPosition.CenterScreen;
            }

            var position = Windows.WindowPosition.CenterScreen;

            switch (Context.AppWindowPosition)
            {
                case Windows.WindowPosition.MiddleLeft:
                case Windows.WindowPosition.TopRight:
                    position = Windows.WindowPosition.BottomRight;
                    break;

                case Windows.WindowPosition.CenterScreen:
                case Windows.WindowPosition.TopLeft:
                case Windows.WindowPosition.MiddleRight:
                    position = Windows.WindowPosition.BottomLeft;
                    break;

                case Windows.WindowPosition.BottomRight:
                    position = Windows.WindowPosition.TopRight;
                    break;

                case Windows.WindowPosition.BottomLeft:
                    position = Windows.WindowPosition.TopLeft;
                    break;
            }

            return position;
        }

        /// <summary>
        /// Initializes each of the actuators and subscribes to
        /// events that will be triggered by the actuators
        /// </summary>
        /// <param name="actuators">ActuatorSettings object</param>
        /// <returns>true on success</returns>
        private bool init()
        {
            bool retVal = true;

            _disposed = false;

            foreach (var actuatorEx in _actuators.Collection)
            {
                actuatorEx.Init();

                actuatorEx.SourceActuator.EvtSwitchActivated += actuator_EvtSwitchActivated;
                actuatorEx.SourceActuator.EvtSwitchDeactivated += actuator_EvtSwitchDeactivated;
                actuatorEx.SourceActuator.EvtSwitchTriggered += actuator_EvtSwitchTriggered;
            }

            /*
            foreach (var actuatorEx in _actuators.Collection)
            {
                actuatorEx.WaitForCalibration();
            }
            */

            foreach (var actuatorEx in _actuators.Collection)
            {
                if (actuatorEx.InitError)
                {
                    retVal = false;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Returns true if the indicated actuator is the one
        /// being calibrated currently
        /// </summary>
        /// <param name="actuator"></param>
        /// <returns></returns>
        private bool isCalibratingActuator(IActuator actuator)
        {
            Log.Debug("isCalibrating: " + actuator.Name + " calibratingActuatorEx is null:" + (_calibratingActuatorEx == null));
            if (_calibratingActuatorEx != null)
            {
                Log.Debug("calibratingActuatorEx.SourceActuator: " + _calibratingActuatorEx.SourceActuator.Name);
            }

            bool retVal = _calibratingActuatorEx != null && _calibratingActuatorEx.SourceActuator == actuator;

            Log.Debug("isCalibrating: returning " + retVal);

            return retVal;
        }

        /// <summary>
        /// Notifies that a switch was accepted. It was held down for a
        /// period > acceptTime
        /// </summary>
        /// <param name="switchObj">The switch that caused the trigger</param>
        private void notifySwitchAccepted(IActuatorSwitch switchObj)
        {
            if (EvtSwitchAccepted != null)
            {
                EvtSwitchAccepted(this, new ActuatorSwitchEventArgs(switchObj));
            }
        }

        /// <summary>
        /// Notifies that a switch was triggered
        /// </summary>
        /// <param name="switchObj">The switch that caused the trigger</param>
        private void notifySwitchActivated(IActuatorSwitch switchObj)
        {
            AuditLog.Audit(new AuditEventSwitchActuate(switchObj.Name, "Actuate", switchObj.Actuator.Name, switchObj.Tag, 0));

            if (EvtSwitchActivated == null)
            {
                Log.Debug("No subscribers. Returning");
                return;
            }

            var delegates = EvtSwitchActivated.GetInvocationList();
            foreach (var del in delegates)
            {
                var switchActivated = (ActuatorSwitchEvent)del;
                Log.Debug("Calling begininvoke for " + switchObj.Name);
                switchActivated.BeginInvoke(this, new ActuatorSwitchEventArgs(switchObj), null, null);
            }
        }

        /// <summary>
        /// Notifies that a switch-down was detected
        /// </summary>
        /// <param name="switchObj">The switch that caused the trigger</param>
        private void notifySwitchDown(IActuatorSwitch switchObj)
        {
            if (EvtSwitchDown != null)
            {
                EvtSwitchDown(this, new ActuatorSwitchEventArgs(switchObj));
            }
        }

        /// <summary>
        /// Notifies all subscribers that want to be hooked into the switch
        /// events that a switch has been triggered.  If handled is true
        /// the subscriber has handled the event.  The input manager will
        /// not handle the event
        /// </summary>
        /// <param name="switchObj"></param>
        /// <param name="handled"></param>
        private void notifySwitchHooks(IActuatorSwitch switchObj, ref bool handled)
        {
            handled = false;
            bool evtHandled = false;

            if (EvtSwitchHook == null)
            {
                return;
            }

            Log.Debug();

            var delegates = EvtSwitchHook.GetInvocationList();
            foreach (var del in delegates)
            {
                var switchHook = (SwitchHook)del;
                switchHook.Invoke(switchObj, ref evtHandled);
                if (evtHandled)
                {
                    handled = true;
                }
            }
        }

        /// <summary>
        /// Notifies that a switch was rejected.  It was held down for a period
        /// less than the accept time.
        /// </summary>
        /// <param name="switchObj">The switch that caused the trigger</param>
        private void notifySwitchRejected(IActuatorSwitch switchObj)
        {
            if (EvtSwitchRejected != null)
            {
                EvtSwitchRejected(this, new ActuatorSwitchEventArgs(switchObj));
            }
        }

        /// <summary>
        /// Notifies that a switch-up was detected
        /// </summary>
        /// <param name="switchObj">The switch that caused the trigger</param>
        private void notifySwitchUp(IActuatorSwitch switchObj)
        {
            if (EvtSwitchUp != null)
            {
                EvtSwitchUp(this, new ActuatorSwitchEventArgs(switchObj));
            }
        }
    }
}