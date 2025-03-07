////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BCIDeviceTester.cs
//
// Tests BCI devices - connections to the hardware (cyton board, optical sensor),
// displays errors accordingly, and begins signal quality check
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Extensions.BCI.Actuators.EEG.EEGDataAcquisition;
using ACAT.Extensions.BCI.Actuators.EEG.EEGSettings;
using ACAT.Extensions.BCI.Actuators.EEG.EEGUtils;
using ACAT.Extensions.BCI.Common.BCIControl;
using ACAT.Lib.Core.Audit;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ACAT.Extensions.BCI.Actuators.EEG.EEGDataAcquisition.DAQ_OpenBCI;

namespace ACAT.Extensions.BCI.Actuators.SensorUI
{
    /// <summary>
    /// Tests BCI devices - connections to the hardware (cyton board, optical sensor), displays errors accordingly, and begins signal quality check
    /// </summary>
    public class BCIDeviceTester
    {
        /// <summary>
        /// Variables representing all the different states in testing process (state machine)
        /// </summary>
        public enum DeviceTestingState
        {
            Testing_BCIConnections, // Connecting screen - Show GIF and disable Next button
            ReceivedBCIError_UsbDongle, // Received connection based error - Display info and provide "Retest" functionality
            ReceivedBCIError_CytonBoard, // Received connection based error - Display info and provide "Retest" functionality
            ReceivedBCIError_PortConfig,  // Received port latency incorrect error - Display info and provide option to Exit
            ReceivedBCIError_OpticalSensor, // Received optical sensor error - Plot signal and provide "Retest" functionality
            ReceivedBCIError_LostDataConnection, // Stopped receiving data from optical sensor after initially passing tests - Rerun entire BCI device testing process
            BCISignalCheckStartRequired, // Notifies user that maximum time has elapsed since last signal quality check, new one is needed
            BCISignalCheckStartPrompt, // Prompts user if they need to do signal check based on a couple questions
            PromptFilterSettings, // Prompts user to set BCI filter settings (50Hz / 60Hz)
            BCISignalCheck, // Checking all electodes and their signal quality
            OpticalSensorDetectError,
            ExitBCITesting // Exit BCI testing process completely

            // Signal status / quality based errors ?
        }

        /// <summary>
        /// List of windows / states to visit during debugging process
        /// </summary>
        private DeviceTestingState[] _DebugStates = new DeviceTestingState[12] {
            DeviceTestingState.Testing_BCIConnections,
            DeviceTestingState.ReceivedBCIError_UsbDongle,
            DeviceTestingState.ReceivedBCIError_CytonBoard,
            DeviceTestingState.ReceivedBCIError_PortConfig,
            DeviceTestingState.ReceivedBCIError_OpticalSensor,
            DeviceTestingState.BCISignalCheckStartRequired,
            DeviceTestingState.BCISignalCheckStartPrompt,
            DeviceTestingState.PromptFilterSettings,
            DeviceTestingState.BCISignalCheck,
            DeviceTestingState.BCISignalCheck,
            DeviceTestingState.BCISignalCheck,
            DeviceTestingState.ExitBCITesting
        };

        /// <summary>
        /// Current device testing state
        /// </summary>
        public static DeviceTestingState _deviceTestingState;

        /// <summary>
        /// Read from BCIActuatorSettings (Testing_useSensor). Setting to false enables debugging with dummy sensor
        /// </summary>
        public static bool _Testing_useSensor = true;

        public static int _Testing_useSensor_TestIndex = 0;

        /// <summary>
        /// Whether or not to skip optical sensor checks (sensor not currently attached) during BCI Onboarding
        /// </summary>
        private bool _Testing_BCIOnboardingIgnoreOpticalSensorChecks = false;

        /// <summary>
        /// Main form showing different user controls with information
        /// on connecting status, errors, and bCI data
        /// </summary>
        public SensorForm _mainForm = null;

        /// <summary>
        /// Used to signal BCI form is fully loaded
        /// </summary>
        public bool _FormFullySHown = false;

        /// <summary>
        /// Box in upper left hand corner where optical sensor placed
        /// </summary>
        public ScannerRoundedButtonControl _triggerBox;

        /// <summary>
        /// Time used to switch color for the trigger box
        /// </summary>
        private int _triggerBoxSwitchDelayMsec = 600;

        /// <summary>
        /// Amount of iteration for the test in the trigger box
        /// </summary>
        private int _triggerBoxNumTestIterations = 10;

        /// <summary>
        /// Start trigger box test
        /// </summary>
        private bool _requestTestTriggerBox = false;

        /// <summary>
        /// Minimum amount of time to wait on initial "Connecting..." screen
        /// Just for some consistency in user experience - so they can always see "Connecting..." screen
        /// </summary>
        public double _minTimeShowConnectingScreen = 4; // 1, 3, 5, 7, 10

        /// <summary>
        /// Maximum amount of time after not receiving data (after initially receiving good data) to throw error
        /// </summary>
        public const double THRESHOLD_ERROR_NO_DATA_SEC = 5.0;

        /// <summary>
        /// DateTime last received good BCI / signal status check data
        /// </summary>
        public static DateTime dateTimeLastReceivedOKSignalStatus = DateTime.MinValue;

        /// <summary>
        /// Event sent when it's time to change the screen displaying testing information to the user
        /// </summary>
        /// <param name="status"></param>
        /// <param name="result"></param>
        /// <param name="Testing_useSensor"></param>
        public delegate void ChangeDeviceTestingState(DeviceTestingState state);

        public event ChangeDeviceTestingState EvtChangeDeviceTestingState;

        /// <summary>
        /// Event sent when exiting out of device testing completely
        /// </summary>
        public delegate void BCIDeviceTestingCompleted();

        public event BCIDeviceTestingCompleted EvtBCIDeviceTestingCompleted;

        /// <summary>
        /// Flag to end all device testing async functions / tasks at this level
        /// </summary>
        private static bool _endTasks = false;

        /// <summary>
        /// Flag to end async task toggling the trigger box
        /// </summary>
        private static bool _endTriggerBoxTask = false;

        /// <summary>
        /// Flag to end getting BCI data and pushing data to graphs
        /// </summary>
        public static bool _endSignalCheckTimer = false;

        /// <summary>
        /// Flag set to true when DAQ initialization task has ended
        /// </summary>
        private static bool _initDAQTaskStopped = false;

        /// <summary>
        /// Flag set to true when trigger box task has ended
        /// </summary>
        private static bool _triggerBoxTaskStopped = false;

        /// <summary>
        /// Flag set to true when optical sensor has been disconnected
        /// </summary>
        private volatile bool _opticalSensorDisconnected = false;

        /// <summary>
        /// Whether Exit was selected and then confirmed from any screen - left Onboarding without completion
        /// </summary>
        public static bool ExitOnboardingEarly = false;

        /// <summary>
        /// Tests BCI devices - connections to the hw and data quality
        /// Displays errors accordingly - After an error, starts at the beginning of the process (testing device connections)
        /// </summary>
        public BCIDeviceTester()
        {
            // Do not call init function here

            // Call init function after creating object of this class and linking any necessary event handler (ex: EvtBCIDeviceTestingCompleted)
        }

        /// <summary>
        /// Do all BCIDeviceTester initialization - this is done in a separate function instead of the constructor
        /// so it's possible to easily reset all variables
        /// </summary>
        public void initialize()
        {
            Log.Debug("BCIDeviceTester | initialize");

            // Close main form if for some reason it's opened at this point
            if (_mainForm != null && _mainForm.IsDisposed == false)
            {
                Log.Debug("BCIDeviceTester | _mainForm != null && _mainForm.IsDisposed == false");
                _mainForm.Close();
                _mainForm.Dispose();
            }

            try
            {
                _Testing_useSensor = BCIActuatorSettings.Settings.Testing_UseSensor;
                _Testing_BCIOnboardingIgnoreOpticalSensorChecks = BCIActuatorSettings.Settings.Testing_BCIOnboardingIgnoreOpticalSensorChecks;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }

            initOpticalSensor();

            if (ExitOnboardingEarly)
            {
                EvtBCIDeviceTestingCompleted();
                return;
            }

            // Set initial device testing states
            _deviceTestingState = DeviceTestingState.Testing_BCIConnections;

            // Set default / initial values of static variables (multiple objects of this class initialized during BCI process)
            _Testing_useSensor_TestIndex = 0; // Set initial testing / developer mode index

            ExitOnboardingEarly = false;
            dateTimeLastReceivedOKSignalStatus = DateTime.MinValue;

            DAQ_OpenBCI.pastOpticalSensorData = new double[1];
            DAQ_OpenBCI.dateTimeLastChangeOpticalSensorData = DateTime.MinValue;

            // Unset flags that will end async tasks and timers
            _endSignalCheckTimer = false;
            _endTriggerBoxTask = false;
            _endTasks = false;

            // Create main form
            _mainForm = new SensorForm(_deviceTestingState);

            // Set handlers for main events
            if (_Testing_useSensor)
            {
                _mainForm.EvtButtonNextClicked += _mainForm_EvtButtonNextClicked;  // Next button click
                _mainForm.EvtButtonRetestClicked += _mainForm_EvtButtonRetestClicked; // Retest button click
                _mainForm.EvtButtonCancelClicked += _mainForm_EvtButtonExitClicked; // Cancel button click
            }
            else
            {
                _mainForm.EvtButtonCancelClicked += _mainForm_EvtButtonExitClicked_DEBUG; // Cancel button click for debugging mode
            }

            // Event called when there is a new screen to be shown during connecting process (ex: got error or completed connecting successfully)
            EvtChangeDeviceTestingState += _mainForm.changeDeviceTestingState;

            //// Enable button hidden / shown on last page depending on acceptable channels
            //// EvtSetEnabledNextButton += _mainForm.enableNextButton;

            // Event for displaying error message when no longer receiving data
            // _mainForm._userControlBCISignalCheck.ShowSensorErrorMsg_Event += showSensorErrorMessage;

            // Event for when main form closed
            _mainForm.FormClosed += _mainForm_EvtFormClosed;

            // Wait until control fully drawn to set flag which begins connecting to BCI devices
            _mainForm.Paint += (s, args) => { _FormFullySHown = true; handleMainFormShown(this, null); };

            // Show main form
            _mainForm.ShowDialog();
        }

        /// <summary>
        /// Called when user selects Exit or Next button from signal check
        /// </summary>
        public void Exit(bool lostConnection)
        {
            // Set device testing state accordingly
            if (lostConnection)
            {
                _deviceTestingState = DeviceTestingState.ReceivedBCIError_LostDataConnection;
            }
            else
            {
                _deviceTestingState = DeviceTestingState.ExitBCITesting;
            }

            // Set flags that will end async tasks and timers
            _endSignalCheckTimer = true;
            _endTriggerBoxTask = true;
            _endTasks = true;

            // Release event handlers at this level
            EvtChangeDeviceTestingState = null;

            // Close main form - has it's own form closed handler that releases resources owned by it
            if (_mainForm != null)
            {
                _mainForm.Close();
                _mainForm.Dispose();
            }
        }

        /// <summary>
        /// Handler for BCI device testing form closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _mainForm_EvtFormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            Log.Debug("BCIDeviceTester | _mainForm_EvtFormClosed | _deviceTestingState: " + _deviceTestingState.ToString());

            if (_deviceTestingState == DeviceTestingState.ReceivedBCIError_LostDataConnection)
            {
                // Start BCI device retesting process from the beginning
                // Resources already released - last thing that happens is main form closed handler

                // Start in separate thread so you can do any waits without freezing main thread
                Thread t = new Thread(() =>
                {
                    // Wait a bit until starting initialization again
                    Thread.Sleep(250);

                    // Wait until all these flags are correct (all async tasks stopped)
                    while (!_initDAQTaskStopped || !_triggerBoxTaskStopped)
                    {
                        Thread.Sleep(250);
                    }

                    // Wait a bit until starting initialization again
                    Thread.Sleep(250);
                });
                t.Start();
                t.Join();

                // Run initialize on main thread
                initialize();
            }
            else if (_deviceTestingState == DeviceTestingState.ExitBCITesting)
            {
                // Exit BCI device testing process completely

                // Send event notification that all BCI device testing completed
                if (EvtBCIDeviceTestingCompleted != null)
                    EvtBCIDeviceTestingCompleted();
            }
        }


        /// <summary>
        /// Handler to complete all necessary actions before transition to a new signal quality testing state
        /// </summary>
        /// <param name="newDeviceTestingState"></param>
        private void startSignalQualityTestingState(DeviceTestingState newDeviceTestingState)
        {
            // Always update _deviceTestingState which is checked / used in changeDeviceTestingState (main form)
            _deviceTestingState = newDeviceTestingState;

            // Start at BCISignalCheckStartRequired (first signal quality checking state)
            if (newDeviceTestingState == DeviceTestingState.BCISignalCheckStartRequired)
            {
                // Always check time last impedance test was run (all electrodes tested) and update UI accordingly
                long timestampPrevImpedanceTest = BCIActuatorSettings.Settings.SignalQuality_TimeOfLastImpedanceCheck;
                long timestampNow = DateTimeOffset.Now.ToUnixTimeSeconds();
                long secDiff = timestampNow - timestampPrevImpedanceTest;
                double minElapsedPrevSignalQualityCheck = ((double)secDiff) / 60;
                double maxTimeMins = (double)BCIActuatorSettings.Settings.SignalQuality_MaxTimeMinsElapsedSinceLastImpedanceCheck​;
                bool maxTimeHasElapsed = false;
                if (minElapsedPrevSignalQualityCheck >= maxTimeMins)
                    maxTimeHasElapsed = true;
                Log.Debug(String.Format("startSignalQualityTestingState | newDeviceTestingState == DeviceTestingState.BCISignalCheckStartRequired" +
                    "\ntimestampPrevImpedanceTest: {0}, timestampNow: {1}, secDiff: {2}", timestampPrevImpedanceTest.ToString(), timestampNow.ToString(), secDiff.ToString()));
                Log.Debug(String.Format("minElapsedPrevSignalQualityCheck: {0}, maxTimeMins: {1}, maxTimeHasElapsed: {2}", minElapsedPrevSignalQualityCheck.ToString(), maxTimeMins.ToString(), maxTimeHasElapsed.ToString()));

                // Always check if user passed the last overall signal quality check that was executed
                // If max time has not passed, but user did not pass their most recent overall signal quality check,
                // user must do tests and calibration (SignalControl_RecheckNeeded = true)
                bool userPassedLastSignalQualityCheck = BCIActuatorSettings.Settings.SignalQuality_PassedLastOverallQualityCheck;

                // Initialize parameters and set processing variables / UI elements in main signal check screen accordingly
                _mainForm._userControlBCISignalCheck.initializeBCISignalCheck(maxTimeHasElapsed, maxTimeMins, minElapsedPrevSignalQualityCheck,
                    userPassedLastSignalQualityCheck);

                // Update user controls that are not BCI signal check with result from previous tests then go to correct state

                // Update screen that tells user recheck is required
                if (maxTimeHasElapsed)
                {
                    // Update with maximum time that has already passed since previous test
                    _mainForm._userControlBCISignalCheckStartRequired.labelMinsElapsedSignalCheckStartRequired.Text = String.Format("{0:0} minutes", maxTimeMins);

                    // Go to BCI signal check start required screen
                    changeDeviceTestingState(DeviceTestingState.BCISignalCheckStartRequired);
                }
                else if (!maxTimeHasElapsed && !userPassedLastSignalQualityCheck)
                {
                    // Update with text telling user they failed previous signal quality check
                    _mainForm._userControlBCISignalCheckStartRequired.labelInfo1SignalCheckStartRequired.Text = "You did not pass your most recent signal quality check";
                    _mainForm._userControlBCISignalCheckStartRequired.labelMinsElapsedSignalCheckStartRequired.Text = "";
                    _mainForm._userControlBCISignalCheckStartRequired.labelInfo2SignalCheckStartRequired.Text = "";

                    // Go to BCI signal check start required screen
                    changeDeviceTestingState(DeviceTestingState.BCISignalCheckStartRequired);
                }
                else if (!maxTimeHasElapsed && userPassedLastSignalQualityCheck)
                {
                    // Go to BCI signal check start prompt screen
                    startSignalQualityTestingState(DeviceTestingState.BCISignalCheckStartPrompt);
                }
            }
            else if (newDeviceTestingState == DeviceTestingState.BCISignalCheckStartPrompt)
            {
                // Go to BCI signal check start prompt screen
                changeDeviceTestingState(DeviceTestingState.BCISignalCheckStartPrompt);
            }
            else if (newDeviceTestingState == DeviceTestingState.PromptFilterSettings)
            {
                // If got to this point, it's been determined YES will go through signal check flow
                // Decide whether or not to show the settings screen before going to main signal check screen

                // Reset all saved values / flags used to get user to this point
                _mainForm._userControlBCISignalCheck.resetSavedSignalQualityValues();

                // Go to either PromptFilterSettings or BCISignalCheck
                bool showFilterSettings = BCIActuatorSettings.Settings.DAQ_ShowFilterSettings;
                if (showFilterSettings)
                {
                    // Display filter settings screen
                    changeDeviceTestingState(DeviceTestingState.PromptFilterSettings);
                }
                else
                {
                    // Go straight to main signal quality testing state
                    startSignalQualityTestingState(DeviceTestingState.BCISignalCheck);
                }
            }
            else if (newDeviceTestingState == DeviceTestingState.BCISignalCheck)
            {
                // Go to main signal quality testing state
                changeDeviceTestingState(DeviceTestingState.BCISignalCheck);
            }
        }

        //// 

        /// <summary>
        /// Handler to complete all necessary actions during transition out of / completion of signal quality testing state
        /// Ex: selecting "Next" button
        /// </summary>
        /// <param name="currentDeviceTestingState"></param>
        private void finishSignalQualityTestingState(DeviceTestingState currentDeviceTestingState)
        {
            Log.Debug("BCIDeviceTester | finishSignalQualityTestingState | currentDeviceTestingState: " + currentDeviceTestingState.ToString());

            // Next button selected from BCI signal check start required screen
            if (currentDeviceTestingState == DeviceTestingState.BCISignalCheckStartRequired)
            {
                startSignalQualityTestingState(DeviceTestingState.PromptFilterSettings);
            }

            // Next button selected from BCI signal check start prompt screen
            else if (currentDeviceTestingState == DeviceTestingState.BCISignalCheckStartPrompt)
            {
                // Get recheck request from button status
                bool userRequestedRecheck = _mainForm._userControlBCISignalCheckStartPrompt.UserRequestedRecheck;
                if (!userRequestedRecheck)
                {
                    // If no recheck needed, transition out of BCIDeviceTester all together (everything successfully completed)
                    // User already did impedance test within acceptable time frame
                    // User's last overall signal quality results was good

                    // Save SignalQuality_RecheckNeeded
                    BCIActuatorSettings.Settings.SignalControl_RecheckNeeded = false;
                    BCIActuatorSettings.Save();

                    // We are exiting - Call Exit function with lost connection flag set to false
                    Exit(false);
                }
                else
                {
                    // Save SignalQuality_RecheckNeeded
                    BCIActuatorSettings.Settings.SignalControl_RecheckNeeded = true;
                    BCIActuatorSettings.Save();

                    startSignalQualityTestingState(DeviceTestingState.PromptFilterSettings);
                }
            }

            // Next button selected from Filter Settings screen
            else if (currentDeviceTestingState == DeviceTestingState.PromptFilterSettings)
            {
                // Save filter settings from user's input
                if (_mainForm._userControlPromptBCIFIlterSettings.checkBoxConfirm60HzCountry.Checked)
                {
                    // DAQ_NotchFilterIdx = 1; //50Hz
                    // DAQ_NotchFilterIdx = 2; //60Hz
                    BCIActuatorSettings.Settings.DAQ_NotchFilterIdx = 2;
                }
                else
                {
                    BCIActuatorSettings.Settings.DAQ_NotchFilterIdx = 1;
                }

                if (_mainForm._userControlPromptBCIFIlterSettings.checkBoxDontShowStartup.Checked)
                {
                    BCIActuatorSettings.Settings.DAQ_ShowFilterSettings = false;
                }
                else
                {
                    BCIActuatorSettings.Settings.DAQ_ShowFilterSettings = true;
                }

                BCIActuatorSettings.Save();

                // Transition to main signal checking screen
                startSignalQualityTestingState(DeviceTestingState.BCISignalCheck);
            }

            // Next button selected from BCI signal check screen
            else if (currentDeviceTestingState == DeviceTestingState.BCISignalCheck)
            {
                bool exitBCIOnboarding = false;

                // Get current signal quality check status (user currently passes or fails the checks)
                // Is updated every INTERVAL_UPDATE_OVERALL_SIGNAL_QUALITY_STATUS_MS while user is in signal check
                bool userPassedLastSignalQualityCheck = BCIActuatorSettings.Settings.SignalQuality_PassedLastOverallQualityCheck;
                if (userPassedLastSignalQualityCheck)
                {
                    Log.Debug("User passed most recent signal quality check");
                    exitBCIOnboarding = true;
                }

                // Check if testing parameter set to ignore signal quality check result
                if (BCIActuatorSettings.Settings.Testing_IgnoreSignalTestResultDuringOnboarding)
                {
                    Log.Debug("BCIActuatorSettings.Testing_IgnoreSignalTestResultDuringOnboarding = true");
                    exitBCIOnboarding = true;

                    if (!userPassedLastSignalQualityCheck)
                    {
                        // Exit anyways regardless of signal quality result
                        Log.Debug("User did not pass signal quality check but set testing parameter to ignore result. Exiting as if user did pass the check");
                    }
                }

                // Log current channel names, enabled status, railing, impedance values
                // Also log whether or not signal check is exiting at this time
                String[] channelNames = new string[16];
                bool[] enabledChannels = new bool[16];
                int[] railingValues = new int[16];
                int[] impedanceValues = new int[16];
                int chnIdx = 0;
                while (chnIdx < BCIActuatorSettings.Settings.DAQ_NumEEGChannels)
                {
                    channelNames[chnIdx] = UserControlBCISignalCheck._eegChannels[chnIdx]._electrodeName;
                    enabledChannels[chnIdx] = BCIActuatorSettings.Settings.GetClassifier_EnableChannel(chnIdx);
                    railingValues[chnIdx] = (int)UserControlBCISignalCheck._eegChannels[chnIdx].lastRailingResult;
                    impedanceValues[chnIdx] = (int)UserControlBCISignalCheck._eegChannels[chnIdx].lastImpedanceResult;
                    chnIdx += 1;
                }
                var bciLogEntry = new BCILogEntrySignalQuality(channelNames, enabledChannels, railingValues, impedanceValues, exitBCIOnboarding); // 5th param
                var jsonString = JsonSerializer.Serialize(bciLogEntry);
                AuditLog.Audit(new AuditEvent("BCISignalQuality", jsonString));

                // Based on the status of all electrodes
                // Either successfully close BCIDeviceTester (continue on to calibration) or display msg to user asking to clean up bad channels
                if (exitBCIOnboarding)
                {
                    // Do not modify Classifier_EnableChannel1-16, that's up to the user
                    // Just save settings, set appropriate flags, and exit

                    BCIActuatorSettings.Save(); // Save settings

                    ExitOnboardingEarly = false; // Set global flag denoting onboarding was not exited early

                    Exit(false); // Call Exit function with lost connection flag set to false
                }

                // Do not exit
                else
                {
                    // Display message to user prompting them to improve signal quality before moving on
                    Log.Debug("Not exiting | Did not pass signal quality criteria");
                    bool confirmed = ConfirmBoxSingleOption.ShowDialog("Signal Quality Checks Failed or Incomplete" +
                        "\nYou need to complete both “Railing” and\n“Impedance” tests and get good signals to\nproceed" +
                        "\nPlease refer to the user guide for help", "Ok", _mainForm, false);

                    return; // return to form
                }
            }
        }

        /// <summary>
        /// Handler for Next button click
        /// </summary>
        private void _mainForm_EvtButtonNextClicked(DeviceTestingState deviceTestingState)
        {
            finishSignalQualityTestingState(deviceTestingState);
        }

        /// <summary>
        /// Handler for Retest button click
        /// </summary>
        private void _mainForm_EvtButtonRetestClicked(object sender)
        {
            // Retest BCI connections
            retestBCIConnections();
        }

        /// <summary>
        /// Retest BCI connections
        /// Runs appropriate test / action based on _deviceTestingState
        /// </summary>
        private void retestBCIConnections()
        {
            Log.Debug("retestBCIConnections(). deviceTestingState: " + _deviceTestingState);

            // If already on Optical sensor error screen -> retest button does not check all BCI connections from the beginning, tests optical sensor right away
            // _requestTestTriggerBox goes to correct user control when test completed
            if (_deviceTestingState == DeviceTestingState.ReceivedBCIError_OpticalSensor)
            {
                _requestTestTriggerBox = true;
                return;
            }
            else if (_deviceTestingState == DeviceTestingState.ReceivedBCIError_UsbDongle ||
                _deviceTestingState == DeviceTestingState.ReceivedBCIError_CytonBoard)
            {
                // If trying Retest from connection error screen or lost connection after initially established
                // or cannot connect to optical sensor COM port - start retesting process again from the beginning
                changeDeviceTestingState(DeviceTestingState.Testing_BCIConnections); // display "connecting" screen

                // Start startBCIDeviceTesting() function from separate non-UI thread
                Thread t = new Thread(() => startBCIDeviceTesting(0));
                t.Start();

                return; // Do not run anything after - device retesting process already started
            }

            // Don't need below anymore?
            // TaskStartStopDataProcessing needed to be called in these cases because plotting
            // timer needs to be stopped (optical sensor or BCI signal check)
            switch (DAQ_OpenBCI.deviceStatus)
            {
                case DAQ_OpenBCI.DeviceStatus.DEVICE_STANDBY:
                    InitDAQ();
                    _mainForm.TaskStartStopDataProcessing(DeviceTestingState.Testing_BCIConnections);
                    break;

                case DAQ_OpenBCI.DeviceStatus.DEVICE_ERROR:
                    DAQ_OpenBCI.Stop(); // close board connection before attempting new one if got error
                    InitDAQ();
                    _mainForm.TaskStartStopDataProcessing(DeviceTestingState.Testing_BCIConnections);
                    break;
            }
        }

        /// <summary>
        /// Handler for Exit button click in debug mode - iterate through all available screens / user controls
        /// </summary>
        private void _mainForm_EvtButtonExitClicked_DEBUG(object sender)
        {
            try
            {
                _Testing_useSensor_TestIndex += 1;
                Log.Debug("BCIDeviceTester | _mainForm_EvtButtonExitClicked_DEBUG | _Testing_useSensor_TestIndex: " + _Testing_useSensor_TestIndex.ToString());
                if (_Testing_useSensor_TestIndex < _DebugStates.Length)
                {
                    DeviceTestingState newState = _DebugStates[_Testing_useSensor_TestIndex];

                    // Check if previous device testing state was also BCISignalCheck
                    // If yes, change _currentBCISignalCheckMode so other BCI signal check user control can be shown
                    if (newState == DeviceTestingState.BCISignalCheck)
                    {
                        if (_DebugStates[_Testing_useSensor_TestIndex - 1] == DeviceTestingState.BCISignalCheck)
                        {
                            if (UserControlBCISignalCheck._currentBCISignalCheckMode == UserControlBCISignalCheck.BCISignalCheckMode.TEST_RAILING)
                            {
                                UserControlBCISignalCheck._currentBCISignalCheckMode = UserControlBCISignalCheck.BCISignalCheckMode.TEST_IMPEDANCE;
                            }
                            else if (UserControlBCISignalCheck._currentBCISignalCheckMode == UserControlBCISignalCheck.BCISignalCheckMode.TEST_IMPEDANCE)
                            {
                                UserControlBCISignalCheck._currentBCISignalCheckMode = UserControlBCISignalCheck.BCISignalCheckMode.TEST_QUALITY;
                            }
                        }
                    }

                    EvtChangeDeviceTestingState?.Invoke(newState);
                }
                else
                {
                    // We are exiting - Call Exit function with lost connection flag set to false
                    Exit(false);
                }
            }
            catch (Exception e)
            {
                Log.Debug("_mainForm_EvtButtonExitClicked_DEBUG exception: " + e.ToString());
            }
        }

        /// <summary>
        /// Handler for Exit button click - dispayed on all device testing screens and does the same thing,
        /// completely exits testing process early without completion
        /// </summary>
        private void _mainForm_EvtButtonExitClicked(object sender)
        {
            if (!confirmExit(_mainForm))
            {
                return;
            }
            else
            {
                ExitOnboardingEarly = true; // Set flag corresponding to early exit

                // We are exiting - Call Exit function with lost connection flag set to false
                Exit(false);
            }
        }

        /// <summary>
        /// Asynchronous task that initializes BCI device testing
        /// </summary>
        /// <returns></returns>
        public async Task startBCIDeviceTesting(int initialDelaySec = 0)
        {
            // Wait until main form fully loaded before starting
            while (!_FormFullySHown)
            {
                await Task.Delay(500); // 2000, 500, 50, 10
            }

            // Extra time to wait before starting testing process
            if (initialDelaySec > 0)
            {
                DateTime startDatetime = DateTime.UtcNow;
                double timeElapsed = 0;
                while (timeElapsed <= initialDelaySec)
                {
                    await Task.Delay(50); // 2000, 500, 50, 10
                    timeElapsed = ((TimeSpan)(DateTime.UtcNow - startDatetime)).TotalSeconds;
                }
            }

            Log.Debug("startBCIDeviceTesting | Calling InitDAQ()");

            // Call async function which connects to BCI sensor + starts task that controls TriggerBox flashing and tests optical sensor by request
            if (_Testing_useSensor == true)
            {
                InitDAQ();
            }
        }

        /// <summary>
        /// Handler for when form first shwon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void handleMainFormShown(object sender, EventArgs e)
        {
            _mainForm.BringToFront();

            if (_Testing_useSensor)
            {
                // Automatically start device testing when main form is shown
                // Start startBCIDeviceTesting() function from separate non-UI thread
                Thread t = new Thread(() => startBCIDeviceTesting(0));
                t.Start();
            }
        }

        /// <summary>
        /// Change background color of trigger box
        /// </summary>
        /// <param name="color"></param>
        private void changeTriggerBoxColor(Color color)
        {
            try
            {
                if (_mainForm != null && _mainForm.TriggerBox != null && !_mainForm.TriggerBox.IsDisposed)
                {
                    _mainForm.TriggerBox.Invoke(new Action(() =>
                    {
                        _mainForm.TriggerBox.BackColor = color;
                    }));
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        /// <summary>
        /// Change device testing state
        /// </summary>
        /// <param name="state"></param>
        private void changeDeviceTestingState(DeviceTestingState state)
        {
            try
            {
                if (_mainForm != null)
                {
                    _mainForm.Invoke(new Action(() =>
                    {
                        EvtChangeDeviceTestingState?.Invoke(state);
                    }));
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        /// <summary>
        /// Continuous loop which processes the optical sensor data
        /// Updates the box in the upper left and can run sensor quality / validation test when flag "_requestTestTriggerBox" set
        /// </summary>
        /// <returns></returns>
        public async Task TriggerBoxHighlighting() // Original function
        {
            _triggerBoxTaskStopped = false;
            int counter = 0;
            bool startTest = false;

            Log.Debug("OPTSEN: Enter TriggerBoxHighlighting()");

            while (!_endTasks && !_endTriggerBoxTask)
            {
                if (_requestTestTriggerBox && !startTest)
                {
                    if ((!OpticalSensorComm.IsConnected() || _opticalSensorDisconnected))
                    {
                        if (!openOpticalSensor())
                        {
                            Log.Debug("OPTSEN: BCIDeviceTester | Failed to open optical sensor COM port. ");
                            _deviceTestingState = DeviceTestingState.ReceivedBCIError_OpticalSensor;
                            changeDeviceTestingState(DeviceTestingState.ReceivedBCIError_OpticalSensor);
                        }
                    }

                    Log.Debug("OPTSEN: Stop streaming");
                    OpticalSensorComm.StopStreaming();
                    await Task.Delay(1000);

                    Log.Debug("OPTSEN: Setting lux threshold to " + BCIActuatorSettings.Settings.DAQ_OpticalSensorLuxThreshold);
                    OpticalSensorComm.SetLuxThreshold(BCIActuatorSettings.Settings.DAQ_OpticalSensorLuxThreshold);

                    Log.Debug("OPTSEN: Start streaming");
                    OpticalSensorComm.StartStreaming();
                    await Task.Delay(1000);

                    // Start TriggerBox test
                    Log.Debug("TriggerBoxHighlighting | _requestTestTriggerBox | !startTest");

                    DAQ_OpenBCI.TriggerTestStart();
                    startTest = true;
                    changeTriggerBoxColor(Color.Black);
                    _triggerBoxSwitchDelayMsec = 100;
                    await Task.Delay(200);
                    counter = 0;
                }

                changeTriggerBoxColor(Color.White);
                await Task.Delay(_triggerBoxSwitchDelayMsec);

                changeTriggerBoxColor(Color.Black);
                await Task.Delay(_triggerBoxSwitchDelayMsec);

                if (_requestTestTriggerBox)
                {
                    counter += 1;

                    if (counter >= _triggerBoxNumTestIterations)
                    {
                        int result;
                        await Task.Delay(200);

                        var status = DAQ_OpenBCI.TriggerTestStop(10, out result, out List<double> dutyCycleList, out double dutyCycleAvg);

                        if (status == DAQ_OpenBCI.ExitCodes.PHOTOSENSOR_STATUS_OK)
                        {
                            BCIActuatorSettings.Save();

                            // Go to first stage of signal quality checking process
                            startSignalQualityTestingState(DeviceTestingState.BCISignalCheckStartRequired);
                        }
                        else
                        {
                            Log.Debug("BCIDeviceTester | _deviceTestingState = DeviceTestingState.ReceivedBCIError_OpticalSensor: " + status);
                            _deviceTestingState = DeviceTestingState.ReceivedBCIError_OpticalSensor;
                            changeDeviceTestingState(DeviceTestingState.ReceivedBCIError_OpticalSensor);
                        }

                        DAQ_OpenBCI.AddWarning(DAQ_OpenBCI.ExitCodes.STATUS_OK, "  Time: " + DateTime.Now.ToString("h:mm:ss tt") + " STATUS " + " MESSAGE: Trigger box Test result");
                        DAQ_OpenBCI.AddWarning(DAQ_OpenBCI.ExitCodes.STATUS_OK, "  Time: " + DateTime.Now.ToString("h:mm:ss tt") + " STATUS " + " MESSAGE: Repetitions: 10 - Result: " + result);
                        DAQ_OpenBCI.AddWarning(status, "  Time: " + DateTime.Now.ToString("h:mm:ss tt") + " STATUS " + " MESSAGE: " + status);
                        _triggerBoxSwitchDelayMsec = 600;
                        counter = 0;
                        _requestTestTriggerBox = false;
                        startTest = false;
                    }
                }
            }

            _triggerBoxTaskStopped = true;

            Log.Debug("OPTSEN: Exiting TriggerBoxHighlighting()");

            Debug.WriteLine("TriggerBoxHighlighting | hole | _triggerBoxTask_TaskStopped = true");
        }

        /// <summary>
        /// Handler for error related to receving data from the optical sensor
        /// </summary>
        /// <param name="msg"></param>
        private void OpticalSensorComm_EvtOpticalSensorDataReceiveError(string msg)
        {
            Debug.WriteLine("OPTSEN: Data receive error. Please (re)plug in the optical sensor");
            _opticalSensorDisconnected = true;
        }

        /// <summary>
        /// Task to Initialize the DAQ sensor if is already then process skip the init method
        /// </summary>
        /// <returns></returns>
        public async Task InitDAQ() // Original function
        {
            Log.Debug("InitDAQ()");
            _initDAQTaskStopped = false;

            while (!_endTasks)
            {
                await Task.Delay(100); // Original delay
                if (_FormFullySHown)
                {
                    await Task.Delay(1500); // Original delay

                    // Log.Debug("BCIDeviceTester | InitDAQ() | Calling DAQ_OpenBCI.getUsbDongleConnected()");
                    ExitCodes exitCode = DAQ_OpenBCI.getUsbDongleConnected();
                    if (exitCode == ExitCodes.UNABLE_TO_OPEN_PORT_ERROR)
                    {
                        Log.Debug("BCIDeviceTester | _deviceTestingState = DeviceTestingState.ReceivedBCIError_UsbDongle");
                        _deviceTestingState = DeviceTestingState.ReceivedBCIError_UsbDongle;
                        changeDeviceTestingState(DeviceTestingState.ReceivedBCIError_UsbDongle);
                    }
                    else
                    {
                        Log.Debug("InitDAQ | exitCode: " + exitCode.ToString());
                        bool success = DAQ_OpenBCI.Start("");
                        if (success)
                        {
                            Log.Debug("DAQ_OpenBCI.Start() | true");

                            // Check latency setting of port is set correctly
                            bool latencyPortOk = DAQ_OpenBCI.CheckLatencyPort();
                            if (latencyPortOk)
                            {
                                Log.Debug("DAQ_OpenBCI.CheckLatencyPort() | true");

                                if (DAQ_OpenBCI.deviceInitialized)
                                {
                                    Log.Debug("DAQ_OpenBCI.deviceInitialized | true");

                                    if (!_Testing_BCIOnboardingIgnoreOpticalSensorChecks)
                                    {
                                        // BCI impedence / railing integration - only go to signal check screens if optical sensor / trigger box tests successful
                                        Log.Debug("_Testing_BCIOnboardingIgnoreOpticalSensorChecks == false");

                                        DAQ_OpenBCI.TriggerTestStart();
                                        DAQ_OpenBCI.AddWarning(DAQ_OpenBCI.ExitCodes.STATUS_OK, "  Time: " + DateTime.Now.ToString("h:mm:ss tt") + " STATUS " + " MESSAGE: Starting Trigger box Test");
                                        _requestTestTriggerBox = true;
                                        _triggerBoxSwitchDelayMsec = 100;
                                        TriggerBoxHighlighting();
                                    }
                                    else
                                    {
                                        // BCI impedence / railing integration - debugging option, go straight to signal check screens without checking optical sensor
                                        Log.Debug("_Testing_BCIOnboardingIgnoreOpticalSensorChecks == true");

                                        // Go to first stage of signal quality checking process
                                        startSignalQualityTestingState(DeviceTestingState.BCISignalCheckStartRequired);
                                    }
                                }
                                else
                                {
                                    Log.Debug("DAQ_OpenBCI.deviceInitialized | false");
                                }
                            }
                            else
                            {
                                Log.Debug("DAQ_OpenBCI.CheckLatencyPort() | false | _deviceTestingState = DeviceTestingState.ReceivedBCIError_PortConfig");
                                DAQ_OpenBCI.Stop();
                                _deviceTestingState = DeviceTestingState.ReceivedBCIError_PortConfig;
                                changeDeviceTestingState(DeviceTestingState.ReceivedBCIError_PortConfig);
                            }
                        }
                        else
                        {
                            Log.Debug("BCIDeviceTester | _deviceTestingState = DeviceTestingState.ReceivedBCIError_CytonBoard");
                            _deviceTestingState = DeviceTestingState.ReceivedBCIError_CytonBoard;
                            changeDeviceTestingState(DeviceTestingState.ReceivedBCIError_CytonBoard);
                        }
                    }

                    break;
                }
            }

            _initDAQTaskStopped = true;
            Log.Debug("InitDAQ | hole | _initDAQ_TaskStopped = true");
            // return;
        }

        /// <summary>
        /// Custom form for when there is an error connecting to the optical sensor
        /// </summary>
        private SensorForm _opticalSensorErrorForm;

        /// <summary>
        /// Initialize optical sensor, show it, and connect custom handlers for the Exit and Retest buttons
        /// </summary>
        private void initOpticalSensor()
        {
            if (!_Testing_useSensor && !_Testing_BCIOnboardingIgnoreOpticalSensorChecks)
            {
                return;
            }

            if (!openOpticalSensor())
            {
                _opticalSensorErrorForm = new SensorForm(DeviceTestingState.OpticalSensorDetectError);
                _opticalSensorErrorForm.EvtButtonCancelClicked += SensorForm_EvtButtonCancelClicked;
                _opticalSensorErrorForm.EvtButtonRetestClicked += SensorForm_EvtButtonRetestClicked;

                _opticalSensorErrorForm.ShowDialog();

                _opticalSensorErrorForm.EvtButtonCancelClicked -= SensorForm_EvtButtonCancelClicked;
                _opticalSensorErrorForm.EvtButtonRetestClicked -= SensorForm_EvtButtonRetestClicked;
            }
        }

        /// <summary>
        /// Handler for when Retry button selected
        /// </summary>
        /// <param name="sender"></param>
        private void SensorForm_EvtButtonRetestClicked(object sender)
        {
            if (openOpticalSensor())
            {
                _opticalSensorErrorForm.Close();
            }
        }

        /// <summary>
        /// Handler for when Exit button selected
        /// </summary>
        /// <param name="sender"></param>
        private void SensorForm_EvtButtonCancelClicked(object sender)
        {
            if (!confirmExit(_opticalSensorErrorForm))
            {
                return;
            }
            else
            {
                ExitOnboardingEarly = true; // Set flag corresponding to early exit
                _opticalSensorErrorForm.Close();
            }
        }

        /// <summary>
        /// Display confirmation to quit BCI onboarding
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        private bool confirmExit(Form parent)
        {
            return ConfirmBox.ShowDialog("Onboarding incomplete. Quit anyway?", parent, true);
        }

        /// <summary>
        /// Connect to opical sensor COM port and attach callbacks related to receiving data
        /// </summary>
        /// <returns></returns>
        private bool openOpticalSensor()
        {
            if (!_Testing_useSensor || _Testing_BCIOnboardingIgnoreOpticalSensorChecks)
            {
                return true;
            }

            try
            {
                _opticalSensorDisconnected = false;

                Log.Debug("OPTSEN: Opening optical sensor port...");
                OpticalSensorComm.Open();
                Log.Debug("OPTSEN: Optical sensor port opened successfully");

                OpticalSensorComm.EvtOpticalSensorDataReceiveError -= OpticalSensorComm_EvtOpticalSensorDataReceiveError;
                OpticalSensorComm.EvtOpticalSensorDataReceiveError += OpticalSensorComm_EvtOpticalSensorDataReceiveError;

                Log.Debug("OPTSEN: Sending stop streaming command");
                OpticalSensorComm.StopStreaming();

                return true;
            }
            catch (Exception ex)
            {
                Log.Debug("OPTSEN: BCIDeviceTester | Failed to open optical sensor COM port. " + ex.Message);
                return false;
            }
        }

    }
}