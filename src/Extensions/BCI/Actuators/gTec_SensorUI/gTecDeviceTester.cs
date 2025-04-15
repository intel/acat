////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// gTecDeviceTester.cs
//
// Tests BCI device - connections to the gTec board, displays errors accordingly, and begins signal quality check
// 
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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACAT.Extensions.BCI.Actuators.gTecSensorUI
{
    /// <summary>
    /// Tests BCI device - connections to the gTec board, displays errors accordingly, and begins signal quality check
    /// </summary>
    public class gTecDeviceTester
    {

        DAQ_gTecBCI gTecBCI = null;

        /// <summary>
        /// Enums representing the different states in the testing process
        /// </summary>
        public enum DeviceTestingState
        {
            TestSignalCheckRequired, // Tests if signal check is required (max time has passed or user failed previous signal check)

            TestingBluetoothConnected,
            TestingSignalQuality,
            PerformingCalibration,

            ExitBCITesting, // Exit BCI testing process completely
        }

        private DeviceTestingState _currentTestingState;

        /// <summary>
        /// Enums representing the different results from the testing process
        /// </summary>
        public enum TestResultState
        {
            SignalCheckRequired_MaxTimeElapsed, // Go to screen telling user that signal check required because maximum time between signal checks has elapsed
            SignalCheckRequired_FailedRecentSignalCheck,  // Go to screen telling user that signal check required because they failed their most recent one

            PromptUser_DoSignalCheck, // Prompts user if they need to do signal check based on a couple questions
            PromptUser_FilterSettings, // Prompts user to set BCI filter settings (50Hz / 60Hz)

            ErrorBluetoothDisconnected,
            LostConnectionError,

            SignalQualityError,
            CalibrationError,
            
        }

        private TestResultState _currentResultState;


        /// <summary>
        /// Current device testing state
        /// </summary>
        // public static DeviceTestingState _deviceTestingState;

        /// <summary>
        /// Read from BCIActuatorSettings (Testing_useSensor). Setting to false enables debugging with dummy sensor
        /// </summary>
        public static bool _Testing_useSensor = true;
        public static int _Testing_useSensor_TestIndex = 0;

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
        /// Maximum amount of time after not receiving data (after initially receiving good data) to throw error
        /// </summary>
        public const double THRESHOLD_ERROR_NO_DATA_SEC = 5.0;

        /// <summary>
        /// Event sent when it's time to change the screen displaying testing information to the user
        /// </summary>
        public delegate void DelegateUpdateTestingStatus(TestResultState state, Dictionary<String, object> resultParams);
        public event DelegateUpdateTestingStatus EvtUpdateTestingStatus;

        /// <summary>
        /// Event sent when exiting out of device testing completely
        /// </summary>
        public delegate void BCIDeviceTestingCompleted();
        public event BCIDeviceTestingCompleted EvtBCIDeviceTestingCompleted;

        /// <summary>
        /// Flag to end getting BCI data and pushing data to graphs
        /// </summary>
        public static bool _endSignalCheckTimer = false;

        /// <summary>
        /// Whether Exit was selected and then confirmed from any screen - left Onboarding without completion
        /// </summary>
        public static bool ExitOnboardingEarly = false;

        /// <summary>
        /// Tests BCI devices - connections to the hw and data quality
        /// Displays errors accordingly - After an error, starts at the beginning of the process (testing device connections)
        /// </summary>
        public gTecDeviceTester()
        {
            // Do not call init function here

            // Call init function after creating object of this class and linking any necessary event handler (ex: EvtBCIDeviceTestingCompleted)
        }

        /// <summary>
        /// Do all gTecDeviceTester initialization - this is done in a separate function instead of the constructor
        /// so it's possible to easily reset all variables
        /// </summary>
        public void initialize()
        {
            Log.Debug("gTecDeviceTester | initialize");

            // Close main form if for some reason it's opened at this point
            if (_mainForm != null && _mainForm.IsDisposed == false)
            {
                Log.Debug("gTecDeviceTester | _mainForm != null && _mainForm.IsDisposed == false");
                _mainForm.Close();
                _mainForm.Dispose();
            }

            // Get test flag saying whether we are actually using the sensor or not
            try
            {
                _Testing_useSensor = BCIActuatorSettings.Settings.Testing_UseSensor;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }

            // If flag set to exit onboarding early, then send event EvtBCIDeviceTestingCompleted and do not continue this function
            if (ExitOnboardingEarly)
            {
                EvtBCIDeviceTestingCompleted();
                return;
            }
            ExitOnboardingEarly = false;

            // Unset flags that will end async tasks and timers
            _endSignalCheckTimer = false;


            // Create main form
            //_mainForm = new SensorForm(_deviceTestingState);

            _mainForm = new SensorForm(_currentTestingState);

            // Set initial device testing states
            _currentTestingState = DeviceTestingState.TestingBluetoothConnected;


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
            EvtUpdateTestingStatus += _mainForm.updateTestingStatus;

            //// Enable button hidden / shown on last page depending on acceptable channels
            // EvtSetEnabledNextButton += _mainForm.enableNextButton;

            // Event for displaying error message when no longer receiving data
            // _mainForm._userControlBCISignalCheck.ShowSensorErrorMsg_Event += showSensorErrorMessage;

            // Event for when main form closed
            _mainForm.FormClosed += _mainForm_EvtFormClosed;

            // Wait until control fully drawn to set flag which begins connecting to BCI devices
            _mainForm.Paint += (s, args) => { _FormFullySHown = true; handleMainFormShown(this, null); };

            // Show main form
            _mainForm.ShowDialog();
        }

        public void executeDeviceTest()
        {
            switch (_currentTestingState)
            {
                case DeviceTestingState.TestSignalCheckRequired: // Notifies user that maximum time has elapsed since last signal quality check, new one is needed
                     
                    // Always check time last impedance test was run (all electrodes tested) and update UI accordingly
                    long timestampPrevImpedanceTest = BCIActuatorSettings.Settings.SignalQuality_TimeOfLastImpedanceCheck;
                    long timestampNow = DateTimeOffset.Now.ToUnixTimeSeconds();
                    long secDiff = timestampNow - timestampPrevImpedanceTest;
                    double minElapsedPrevSignalQualityCheck = ((double)secDiff) / 60;
                    double maxTimeMins = (double)BCIActuatorSettings.Settings.SignalQuality_MaxTimeMinsElapsedSinceLastImpedanceCheck​;
                    bool maxTimeHasElapsed = false;
                    if (minElapsedPrevSignalQualityCheck >= maxTimeMins)
                        maxTimeHasElapsed = true;
                    Log.Debug(String.Format("changeDeviceTestingState | _currentTestingState == DeviceTestingState.TestSignalCheckRequired" +
                        "\ntimestampPrevImpedanceTest: {0}, timestampNow: {1}, secDiff: {2}", timestampPrevImpedanceTest.ToString(), timestampNow.ToString(), secDiff.ToString()));
                    Log.Debug(String.Format("minElapsedPrevSignalQualityCheck: {0}, maxTimeMins: {1}, maxTimeHasElapsed: {2}", minElapsedPrevSignalQualityCheck.ToString(), maxTimeMins.ToString(), maxTimeHasElapsed.ToString()));

                    // Always check if user passed the last overall signal quality check that was executed
                    // If max time has not passed, but user did not pass their most recent overall signal quality check,
                    // user must do tests and calibration (SignalControl_RecheckNeeded = true)
                    bool userPassedLastSignalQualityCheck = BCIActuatorSettings.Settings.SignalQuality_PassedLastOverallQualityCheck;

                    // Initialize parameters and set processing variables / UI elements in main signal check screen accordingly
                    _mainForm._userControlBCISignalCheck.initializeBCISignalCheck(maxTimeHasElapsed, maxTimeMins, minElapsedPrevSignalQualityCheck, userPassedLastSignalQualityCheck);


                    // Go to screen telling user that signal check required because maximum time between signal checks has elapsed
                    if (maxTimeHasElapsed)
                    {
                        _currentResultState = TestResultState.SignalCheckRequired_MaxTimeElapsed;

                        // Update label with maximum time that has already passed since previous test
                        //_mainForm._userControlBCISignalCheckStartRequired.labelMinsElapsedSignalCheckStartRequired.Text = String.Format("{0:0} minutes", maxTimeMins);
                        Dictionary<String, object> resultParms = new Dictionary<String, object>();
                        resultParms["maxTimeMins"] = maxTimeMins;
                        updateTestingStatus(_currentResultState, resultParms);
                    }
                    
                    // Go to screen telling user that signal check required because they failed their most recent one
                    else if (!maxTimeHasElapsed && !userPassedLastSignalQualityCheck)
                    {
                        _currentResultState = TestResultState.SignalCheckRequired_FailedRecentSignalCheck;
                       
                        // Update label telling user they failed previous signal quality check
                        //_mainForm._userControlBCISignalCheckStartRequired.labelInfo1SignalCheckStartRequired.Text = "You did not pass your most recent signal quality check";
                        //_mainForm._userControlBCISignalCheckStartRequired.labelMinsElapsedSignalCheckStartRequired.Text = "";
                        //_mainForm._userControlBCISignalCheckStartRequired.labelInfo2SignalCheckStartRequired.Text = "";
                        updateTestingStatus(_currentResultState, null);
                    }

                    // Go to screen asking user if they want to do a signal quality check
                    else if (!maxTimeHasElapsed && userPassedLastSignalQualityCheck)
                    {
                        _currentResultState = TestResultState.PromptUser_DoSignalCheck;
                        updateTestingStatus(_currentResultState, null);
                    }

                    break;


                case DeviceTestingState.TestingBluetoothConnected:

                    //gTecBCI = new DAQ_gTecBCI();

                    //gTecBCI.InitDevice("UN-2023.05.61");

                    testBluetoothStatus();
                    break;

                case DeviceTestingState.TestingSignalQuality:

                    break;
                case DeviceTestingState.PerformingCalibration:

                    break;
                case DeviceTestingState.ExitBCITesting: // Exit BCI testing process completely

                    break;
                default:
                    break;
            }

        }


        /// <summary>
        /// Placeholder - test bluetooth paired
        /// </summary>
        public void testBluetoothStatus()
        {
            bool devicePairedConnected = false;

            if (!devicePairedConnected)
            {
                _currentResultState = TestResultState.ErrorBluetoothDisconnected;
                updateTestingStatus(_currentResultState, null);

            }


        }



        /// <summary>
        /// Called when user selects Exit or Next button from signal check
        /// </summary>
        public void Exit(bool lostConnection)
        {
           /* // Set device testing state accordingly
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

            */
            // Release event handlers at this level
            EvtUpdateTestingStatus = null;

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
            Log.Debug("gTecDeviceTester | _mainForm_EvtFormClosed | _currentTestingState: " + _currentTestingState.ToString());

        }


        /// <summary>
        /// Handler to complete all necessary actions before transition to a new signal quality testing state
        /// </summary>
        /// <param name="newDeviceTestingState"></param>
        private void startSignalQualityTestingState(DeviceTestingState newDeviceTestingState)
        {
/*            // Always update _currentTestingState which is checked / used in updateTestingStatus (main form)
            _currentTestingState = newDeviceTestingState;

            // Start at TestSignalCheckRequired (first signal quality checking state)
            if (newDeviceTestingState == DeviceTestingState.TestSignalCheckRequired)
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
                Log.Debug(String.Format("startSignalQualityTestingState | newDeviceTestingState == DeviceTestingState.TestSignalCheckRequired" +
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
                    updateTestingStatus(DeviceTestingState.TestSignalCheckRequired);
                }
                else if (!maxTimeHasElapsed && !userPassedLastSignalQualityCheck)
                {
                    // Update with text telling user they failed previous signal quality check
                    _mainForm._userControlBCISignalCheckStartRequired.labelInfo1SignalCheckStartRequired.Text = "You did not pass your most recent signal quality check";
                    _mainForm._userControlBCISignalCheckStartRequired.labelMinsElapsedSignalCheckStartRequired.Text = "";
                    _mainForm._userControlBCISignalCheckStartRequired.labelInfo2SignalCheckStartRequired.Text = "";

                    // Go to BCI signal check start required screen
                    updateTestingStatus(DeviceTestingState.TestSignalCheckRequired);
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
                updateTestingStatus(DeviceTestingState.BCISignalCheckStartPrompt);
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
                    updateTestingStatus(DeviceTestingState.PromptFilterSettings);
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
                updateTestingStatus(DeviceTestingState.BCISignalCheck);
            }*/
        }

        //// 

        /// <summary>
        /// Handler to complete all necessary actions during transition out of / completion of signal quality testing state
        /// Ex: selecting "Next" button
        /// </summary>
        /// <param name="currentDeviceTestingState"></param>
        private void finishSignalQualityTestingState(DeviceTestingState currentDeviceTestingState)
        {
            /*
            Log.Debug("gTecDeviceTester | finishSignalQualityTestingState | currentDeviceTestingState: " + currentDeviceTestingState.ToString());

            // Next button selected from BCI signal check start required screen
            if (currentDeviceTestingState == DeviceTestingState.TestSignalCheckRequired)
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
                    // If no recheck needed, transition out of gTecDeviceTester all together (everything successfully completed)
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
                var jsonString = JsonConvert.SerializeObject(bciLogEntry);
                AuditLog.Audit(new AuditEvent("BCISignalQuality", jsonString));

                // Based on the status of all electrodes
                // Either successfully close gTecDeviceTester (continue on to calibration) or display msg to user asking to clean up bad channels
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
            }*/

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
        /// Runs appropriate test / action based on _currentTestingState
        /// </summary>
        private void retestBCIConnections()
        {
            Log.Debug("retestBCIConnections(). _currentTestingState: " + _currentTestingState);

            /*// If already on Optical sensor error screen -> retest button does not check all BCI connections from the beginning, tests optical sensor right away
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
                updateTestingStatus(DeviceTestingState.Testing_BCIConnections); // display "connecting" screen

                // Start startBCIDeviceTesting() function from separate non-UI thread
                Thread t = new Thread(() => startBCIDeviceTesting(0));
                t.Start();

                return; // Do not run anything after - device retesting process already started
            }

            }*/
        }

        /// <summary>
        /// Handler for Exit button click in debug mode - iterate through all available screens / user controls
        /// </summary>
        private void _mainForm_EvtButtonExitClicked_DEBUG(object sender)
        {
            /*try
            {
                _Testing_useSensor_TestIndex += 1;
                Log.Debug("gTecDeviceTester | _mainForm_EvtButtonExitClicked_DEBUG | _Testing_useSensor_TestIndex: " + _Testing_useSensor_TestIndex.ToString());
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

                    EvtUpdateTestingStatus?.Invoke(newState);
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
            }*/
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

            Log.Debug("startBCIDeviceTesting | Calling ex()");

            // Call async function which connects to BCI sensor + starts task that controls TriggerBox flashing and tests optical sensor by request
            if (_Testing_useSensor == true)
            {
                // InitDAQ();

                executeDeviceTest();
            }
        }

        /// <summary>
        /// Handler for when form first shwon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void handleMainFormShown(object sender, EventArgs e)
        {
            // Automatically start device testing when main form is shown

            _mainForm.BringToFront();

            if (_Testing_useSensor)
            {
                // Set initial device testing states
                _currentTestingState = DeviceTestingState.TestingBluetoothConnected;

                // Start startBCIDeviceTesting() function from separate non-UI thread
                // Thread t = new Thread(() => startBCIDeviceTesting(0));

                Thread t = new Thread(() => executeDeviceTest());
                t.Start();
            }
        }


        /// <summary>
        /// Change device testing state
        /// </summary>
        /// <param name="state"></param>
        private void updateTestingStatus(TestResultState state, Dictionary<String, object> resultParams)
        {
            try
            {
                if (_mainForm != null)
                {
                    _mainForm.Invoke(new Action(() =>
                    {
                        EvtUpdateTestingStatus?.Invoke(state, resultParams);
                    }));
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
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
    }
}