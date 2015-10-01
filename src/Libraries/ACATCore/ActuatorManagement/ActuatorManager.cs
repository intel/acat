////////////////////////////////////////////////////////////////////////////
// <copyright file="ActuatorManager.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2015 Intel Corporation 
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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using ACAT.Lib.Core.Audit;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.UserManagement;
using ACAT.Lib.Core.Utility;

#region SupressStyleCopWarnings

[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1126:PrefixCallsCorrectly",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1101:PrefixLocalCallsWithThis",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1121:UseBuiltInTypeAlias",
        Scope = "namespace",
        Justification = "Since they are just aliases, it doesn't really matter")]
[module: SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1200:UsingDirectivesMustBePlacedWithinNamespace",
        Scope = "namespace",
        Justification = "ACAT guidelines")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1309:FieldNamesMustNotBeginWithUnderscore",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private fields begin with an underscore")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1300:ElementMustBeginWithUpperCaseLetter",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private/Protected methods begin with lowercase")]

#endregion

namespace ACAT.Lib.Core.ActuatorManagement
{
    /// <summary>
    /// Manages all the actuators.  Responsible for reading the
    /// Actuators config file, parsing it, and creating the
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
        private const String ActuatorConfigFileName = "Actuators.xml";

        /// <summary>
        /// Input config file that maps each of the switches to commands
        /// that will be evented to the application.
        /// </summary>
        private const String SwitchConfigMapFilename = "SwitchConfigMap.xml";

        /// <summary>
        /// Singleton instance
        /// </summary>
        private static ActuatorManager _instance;

        /// <summary>
        /// List of switches that are currentlu active i.e., those for which
        /// a "switch down" event has been raised
        /// </summary>
        private readonly Dictionary<String, IActuatorSwitch> _activeSwitches;

        /// <summary>
        /// List of switches whose 'actuate' attribute is false.  These switches
        /// will not trigger an action.  Their actions will be merely audited in
        /// the audit log
        /// </summary>
        private readonly Dictionary<String, IActuatorSwitch> _nonActuateSwitches;

        /// <summary>
        /// Object to synchronize multithread access
        /// </summary>
        private readonly Object _syncObjectSwitches;

        /// <summary>
        /// Maintains a list of actuators
        /// </summary>
        private Actuators _actuators;

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
        /// Actuator being currently calibrated
        /// </summary>
        private ActuatorEx calibratingActuatorEx;

        /// <summary>
        /// Queue to hold calibration requests from actuators
        /// </summary>
        private BlockingQueue<object> calibrationQueue = new BlockingQueue<object>();

        /// <summary>
        /// Prevents a default instance of ActuatorManager class from being created
        /// </summary>
        private ActuatorManager()
        {
            SwitchConfigMap = new SwitchConfig();

            _activeSwitches = new Dictionary<String, IActuatorSwitch>();
            _nonActuateSwitches = new Dictionary<String, IActuatorSwitch>();

            _syncObjectSwitches = new object();
        }

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
        /// <param name="switchObj"></param>
        public event ActuatorSwitchEvent EvtSwitchAccepted;

        /// <summary>
        /// Raised when a switch is activated
        /// </summary>
        public event ActuatorSwitchEvent EvtSwitchActivated;

        /// <summary>
        /// Event is trigged when a switch is down
        /// </summary>
        /// <param name="switchObj"></param>
        public event ActuatorSwitchEvent EvtSwitchDown;

        /// <summary>
        /// Hook event to allow apps to acces switch events
        /// </summary>
        public event SwitchHook EvtSwitchHook;

        /// <summary>
        /// Event is trigged when a switch is rejected
        /// </summary>
        /// <param name="switchObj"></param>
        public event ActuatorSwitchEvent EvtSwitchRejected;

        /// <summary>
        /// Event is trigged when a switch is up
        /// </summary>
        /// <param name="switchObj"></param>
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
        public SwitchConfig SwitchConfigMap { get; private set; }

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
        /// Initializes the manager.  Reads and parses the actuators
        /// XML file and creates a list of actuators from it.  The
        /// extension dirs parameter contains the root directory under
        /// which to search for Actuator DLL files.  The directories
        /// are specified in a comma delimited fashion.
        /// E.g.  Base, Hawking
        /// These are relative to the application execution directory or
        /// to the directory where the ACAT framework has been installed.
        /// It recusrively walks the directories and looks for Actuator
        /// extension DLL files
        /// </summary>
        /// <param name="extensionDirs">Directories to search</param>
        /// <returns>true on success</returns>
        public bool Init(IEnumerable<String> extensionDirs)
        {
            bool retVal = true;

            _initInProgress = true;

            _thread = new Thread(calibrationHandlerThread) { IsBackground = true };
            _thread.Start();

            // load all the acutators
            if (_actuators == null)
            {
                String configFile = UserManager.GetFullPath(ActuatorConfigFileName);
                _actuators = new Actuators();
                retVal = _actuators.Load(extensionDirs, configFile);
                if (retVal)
                {
                    retVal = init();
                }
            }

            if (retVal)
            {
                // load the switchmap config file for the user which contains
                // switch configuration mappings
                String switchConfigFile = UserManager.GetFullPath(SwitchConfigMapFilename);
                retVal = SwitchConfigMap.Load(switchConfigFile);
            }

            _initInProgress = false;

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

        public void OnCalibrationCanceled(IActuator source)
        {
            source.OnCalibrationCanceled();

            Log.Debug("Entered ActuatorManger.OnCalibrationCanceled");
            if (isCalibratingActuator(source))
            {
                source.OnCalibrationCanceled();

                calibratingActuatorEx.OnEndCalibration();
                calibratingActuatorEx = null;

                //Debug.WriteLine("CANCELED!! SETTING CALIBRATINGACUTAOTRX to NULL");
                //calibratingActuatorEx = null;
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
            Log.Debug("Entered ActuatorManger.EndCalibration");
            if (isCalibratingActuator(source))
            {
                calibratingActuatorEx.OnEndCalibration(errorMessage, enableConfigure);
                Log.Debug("SETTING CALIBRATINGACUTAOTRX to NULL");
                calibratingActuatorEx = null;
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
        /// All calibration requests are queued and handled in the order
        /// they were requested.  Actuators should call this function to
        /// indicate that they want to calibrate
        /// </summary>
        /// <param name="source">the requesting actuator</param>
        public void RequestCalibration(IActuator source)
        {
            if (calibrationQueue.Contains(source) || isCalibratingActuator(source))
            {
                Log.Debug("Already queued up or currently processing");
                return;
            }

            var aex = _actuators.find(source);

            if (aex != null)
            {
                aex.RequestCalibration();
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
        /// Updates the calibration status in the calibration form.
        /// </summary>
        /// <param name="source">Source actuator</param>
        /// <param name="caption">caption for the form</param>
        /// <param name="prompt">message to display on the form</param>
        /// <param name="timeout">calibration timeout</param>
        /// <param name="enableConfigure">should "Configure" button be enabled?</param>
        public void UpdateCalibrationStatus(IActuator source, String caption, String prompt, int timeout = 0, bool enableConfigure = true)
        {
            if (isCalibratingActuator(source))
            {
                calibratingActuatorEx.UpdateCalibrationStatus(getCalibrationFormPosition(), caption, prompt, timeout, enableConfigure);
            }
        }

        /// <summary>
        /// Invoke this to indicate that the settings dialog should be displayed
        /// </summary>
        /// <param name="source"></param>
        internal void OnDialogRequest(IActuator source)
        {
            var aex = _actuators.find(source);
            if (aex != null)
            {
                aex.OnDialogRequest();
            }
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
                    calibrationQueue.Clear();
                    calibrationQueue.Enqueue("Stop");
                    Log.Debug("Calling thread.join");

                    _thread.Join(2000);
                    Log.Debug("Exited thread");

                    if (_actuators != null)
                    {
                        _actuators.Dispose();
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

            switch (switchObj.Action)
            {
                case SwitchAction.Down:
                    lock (switches)
                    {
                        if (!switches.ContainsKey(switchObj.Name))
                        {
                            Log.Debug("switches does not contain " + switchObj.Name + ". adding it");

                            // add it to the current list of active switches
                            switches.Add(switchObj.Name, switchObj);

                            switchObj.AcceptTimer.Restart();

                            notifySwitchDown(switchObj);
                        }
                        else
                        {
                            Log.Debug("switches already contains " + switchObj.Name);
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
                            Log.Debug("switches contains " + switchObj.Name);
                            var activeSwitch = switches[switchObj.Name];

                            elapsedTime = (activeSwitch != null && activeSwitch.AcceptTimer.IsRunning) ?
                                                    activeSwitch.AcceptTimer.ElapsedMilliseconds : 0;

                            bool accepted = false;

                            if (switchObj.Actuate &&
                                activeSwitch != null &&
                                activeSwitch.AcceptTimer.IsRunning &&
                                activeSwitch.AcceptTimer.ElapsedMilliseconds >= CoreGlobals.AppPreferences.AcceptTime)
                            {
                                Log.Debug("Switch accepted!");
                                accepted = true;
                                switchActivated = switchObj;
                            }
                            else
                            {
                                Log.Debug("Switch not found or actuate is false or timer not running or elapsedTime < accept time");
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
                            Log.Debug("switches does not contain " + switchObj.Name);
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
                case Windows.WindowPosition.TopRight:
                    position = Windows.WindowPosition.BottomRight;
                    break;

                case Windows.WindowPosition.TopLeft:
                    position = Windows.WindowPosition.BottomLeft;
                    break;

                case Windows.WindowPosition.BottomLeft:
                    position = Windows.WindowPosition.TopLeft;
                    break;

                case Windows.WindowPosition.BottomRight:
                    position = Windows.WindowPosition.TopRight;
                    break;
            }

            return position;
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
                    calibratingActuatorEx = obj as ActuatorEx;
                    Log.Debug("Before start calib");
                    var actuator = calibratingActuatorEx;
                    calibratingActuatorEx.StartCalibration();
                    Log.Debug("after start calib");

                    Log.Debug("Waiting for calib for " + actuator.SourceActuator.Name);

                    actuator.WaitForCalibration();
                }
            }
        }

        /// <summary>
        /// Initializes each of the actuators and subscribes to
        /// events that will be triggered by the actuators
        /// </summary>
        /// <param name="actuators">Actuators object</param>
        /// <returns>true on success</returns>
        private bool init()
        {
            bool retVal = true;

            foreach (var actuatorEx in _actuators.Collection)
            {
                actuatorEx.Init();

                actuatorEx.SourceActuator.EvtSwitchActivated += actuator_EvtSwitchActivated;
                actuatorEx.SourceActuator.EvtSwitchDeactivated += actuator_EvtSwitchDeactivated;
                actuatorEx.SourceActuator.EvtSwitchTriggered += actuator_EvtSwitchTriggered;
            }

            foreach (var actuatorEx in _actuators.Collection)
            {
                actuatorEx.WaitForCalibration();
            }

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
            return calibratingActuatorEx != null && calibratingActuatorEx.SourceActuator == actuator;
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