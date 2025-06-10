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
using ACAT.Extensions.BCI.Common.BCIControl;
using ACAT.Lib.Core.Audit;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using ACATResources;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACAT.Extensions.BCI.Actuators.gTecSensorUI
{
    /// <summary>
    /// Tests BCI device connections to the gTec board, displays errors accordingly, and begins signal quality check
    /// </summary>
    public class GTecDeviceTester
    {
        public DAQ_gTecBCI gTecBCI = null;


        /// <summary>
        /// Enums representing the different onboarding user states (screens shown to the user)
        /// </summary>
        public enum OnboardingUserState
        {
            Testing_BCIConnections, // Screen that displays "connecting..." status
            ErrorBluetoothDisconnected, // Screen shown to the user if bluetooth connection could not be established with device
            
            SignalCheckRequired_MaxTimeElapsed, // Screen telling user that signal check required because maximum time between signal checks has elapsed
            SignalCheckRequired_FailedRecentSignalCheck,  // Screen telling user that signal check required because they failed their most recent one

            PromptUser_DoSignalCheck, // Screen prompting user if they need to do signal check based on a couple questions
            PromptUser_FilterSettings, // Screen prompting user to set BCI filter settings (50Hz / 60Hz)

            BCISignalCheck, // Main signal check screen

            ExitBCITesting, // Exit BCI testing process completely

        }

        /// <summary>
        /// Current onboarding user state (screen shown to the user)
        /// </summary>
        private OnboardingUserState _currentOnboardingUserState;

        /// <summary>
        /// Main form showing different user controls with information
        /// on connecting status, errors, and bCI data
        /// </summary>
        public SensorForm _mainForm = null;

        /// <summary>
        /// Used to signal BCI form is fully loaded
        /// </summary>
        public bool _FormFullyShown = false;

        /// <summary>
        /// Maximum amount of time after not receiving data (after initially receiving good data) to throw error
        /// </summary>
        public const double THRESHOLD_ERROR_NO_DATA_SEC = 5.0;

        /// <summary>
        /// Event sent when it's time to change the screen displaying testing information to the user
        /// </summary>
        public delegate void DelegateUpdateTestingStatus(OnboardingUserState state, Dictionary<String, object> resultParams);
        public event DelegateUpdateTestingStatus EvtUpdateOnboardingStatus;

        /// <summary>
        /// Event sent when exiting out of device testing completely
        /// </summary>
        public delegate void DelegateBCIDeviceTestingCompleted();
        public event DelegateBCIDeviceTestingCompleted EvtBCIDeviceTestingCompleted;

        /// <summary>
        /// Flag to end getting BCI data and pushing data to graphs
        /// </summary>
        public static bool _endSignalCheckTimer = false;

        /// <summary>
        /// Whether Exit was selected and then confirmed from any screen - left Onboarding without completion
        /// </summary>
        public static bool ExitOnboardingEarly = false;

        // Debugging parameters
        /// <summary>
        /// Read from BCIGtecActuatorSettings (Testing_useSensor). Setting to false enables debugging with dummy sensor
        /// </summary>
        public static bool _Testing_useSensor = true;
        public static int _Testing_useSensor_TestIndex = 0;
        private OnboardingUserState[] _DebugStates;

        /// <summary>
        /// Tests BCI devices - connections to the hw and data quality
        /// Displays errors accordingly - After an error, starts at the beginning of the process (testing device connections)
        /// </summary>
        public GTecDeviceTester()
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

            gTecBCI = new DAQ_gTecBCI();
            
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
                _Testing_useSensor = BCIGtecActuatorSettings.Settings.Testing_UseSensor;
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
            _mainForm = new SensorForm(gTecBCI);


            // Set handlers for main events
            if (_Testing_useSensor)
            {
                _mainForm.EvtButtonNextClicked += buttonNextHandler;  // Next button click
                _mainForm.EvtButtonRetestClicked += buttonRetestHandler; // Retest button click
                _mainForm.EvtButtonExitClicked += buttonExitHandler; // Cancel button click

                // Add handlers for bluetooth events
                gTecBCI.EvtBluetoothResult += bluetoothResultHandler;
                gTecBCI.EvtBluetoothResult += _mainForm._userControlErrorBluetoothDisconnected.bluetoothResultHandler;

                _mainForm._userControlErrorBluetoothDisconnected.EvtBluetoothRequest += gTecBCI.bluetoothRequestHandler;
            }
            else
            {
                intializeDebugStates(); // Create list of states to show all the different screens for debugging
                _mainForm.EvtButtonExitClicked += _mainForm_EvtButtonExitClicked_DEBUG; // Handler for button click for debugging mode
            }

            // Event called when there is a new screen to be shown to the user
            EvtUpdateOnboardingStatus += _mainForm.updateOnboardingStatus;

            //// Enable button hidden / shown on last page depending on acceptable channels
            // EvtSetEnabledNextButton += _mainForm.enableNextButton;

            // Event for displaying error message when no longer receiving data
            // _mainForm._userControlBCISignalCheck.ShowSensorErrorMsg_Event += showSensorErrorMessage;

            // Event for when main form closed
            _mainForm.FormClosed += _mainForm_EvtFormClosed;

            // Wait until control fully drawn to set flag which begins connecting to BCI devices
            _mainForm.Paint += (s, args) => { _FormFullyShown = true; handleMainFormShown(this, null); };

            // Show main form
            _mainForm.ShowDialog();
        }


        /// <summary>
        /// Handler for processing the results of bluetooth requests
        /// </summary>
        /// <param name="bluetoothEvent">Type of bluetooth request to handle</param>
        /// <param name="eventParams">Any extra params sent with bluetooth event request</param>
        public void bluetoothResultHandler(DAQ_gTecBCI.BluetoothEvent bluetoothEvent, Dictionary<String, object> eventParams)
        {
            Log.Debug("gTecDeviceTester | bluetoothResultHandler | bluetoothEvent: " + bluetoothEvent.ToString()+ " | _currentOnboardingUserState: "+ _currentOnboardingUserState.ToString());

            switch (bluetoothEvent)
            {
                // Received event which occurs on successful connection to the BCI headset
                case DAQ_gTecBCI.BluetoothEvent.DEVICE_DISCONNECTED:

                    string error = "";
                    try
                    {
                        if (eventParams != null)
                            error = (string) eventParams["error"];
                    }
                    catch (Exception ex)
                    {
                        Log.Debug("gTecDeviceTester | bluetoothResultHandler | Exception: " + ex.Message);
                    }

                    // We were seeing if device could be connected to from the start of the testing process
                    // Go to screen directing user to connect their unicorn device through bluetooth pairing
                    if (_currentOnboardingUserState == OnboardingUserState.Testing_BCIConnections)
                    {
                        _currentOnboardingUserState = OnboardingUserState.ErrorBluetoothDisconnected;
                        updateOnboardingStatus(_currentOnboardingUserState, null);
                    }

                    break;

                // Received event which occurs on successful connection to the BCI headset
                case DAQ_gTecBCI.BluetoothEvent.SUCCESSFUL_CONNECTION:

                    if (_currentOnboardingUserState == OnboardingUserState.Testing_BCIConnections || _currentOnboardingUserState == OnboardingUserState.ErrorBluetoothDisconnected)
                    {
                        var thread = new Thread(delegate ()
                        {
                            gTecBCI.Start(BCIGtecActuatorSettings.Settings.GTecDeviceName);
                        });
                        thread.Start();
                        thread.Join();

                        _mainForm.Invoke(new Action(() =>
                        {
                            // Finds if signal check is required or asks user if they want to do one. Navigates to the corresponding screen
                            runSignalCheckIfRequired();
                        }));
                    }

                    break;

                default:
                    break;

            }

        }


        // <summary>
        // Asynchronous task that initializes BCI device testing
        // </summary>
        // <returns></returns>
        public async Task startBCIDeviceTesting(int initialDelaySec = 0)
        {
            Log.Debug("gTecDeviceTester | startBCIDeviceTesting | initialDelaySec: " + initialDelaySec.ToString());

            // Extra time to wait before actually starting testing process (a lot of testing functions can return immediately, so this is for user experience)
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

            // Test gTec bluetooth device connection based on device name in settings (result of test are handled as events)
            if (_Testing_useSensor == true)
            {
                gTecBCI.connectionTestAsync();
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
                // Go to screen that displays "connecting..." status
                _currentOnboardingUserState = OnboardingUserState.Testing_BCIConnections;
                updateOnboardingStatus(_currentOnboardingUserState, null);

                // Non-zero value is needed for startBCIDeviceTesting so "connecting..." screen has time to show to the user
                Thread t = new Thread(() => startBCIDeviceTesting(3));
                t.Start();

            }
        }

        /// <summary>
        /// Change onboarding user state (screen shown to the user)
        /// </summary>
        /// <param name="state"></param>
        private void updateOnboardingStatus(OnboardingUserState state, Dictionary<String, object> resultParams)
        {
            try
            {
                _mainForm?.Invoke(new Action(() =>
                    {
                        EvtUpdateOnboardingStatus?.Invoke(state, resultParams);
                    }));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }



        /// <summary>
        /// Handler for Next button click
        /// </summary>
        private void buttonNextHandler(String buttonNextName)
        {
            Log.Debug("gTecDeviceTester | buttonNextHandler | buttonNextName: " + buttonNextName);
            switch (buttonNextName)
            {

                // Next button clicked from UserControlBluetoothDisconnected
                case "buttonNext_userControlErrorBluetoothDisconnected":

                    // Run connection test again if user selects Next from the UserControlBluetoothDisconnected screen
                    
                    // Go to screen that displays "connecting..." status
                    _currentOnboardingUserState = OnboardingUserState.Testing_BCIConnections;
                    updateOnboardingStatus(_currentOnboardingUserState, null);

                    // Non-zero value is needed for startBCIDeviceTesting so "connecting..." screen has time to show to the user
                    Thread t = new Thread(() => startBCIDeviceTesting(3));
                    t.Start();

                    break;


                // Next button clicked from UserControlBCISignalCheckStartRequired
                case "buttonNext_userControlBCISignalCheckStartRequired":

                    // Go to screen prompting user for correct filter setting (start of signal check process)
                    _currentOnboardingUserState = OnboardingUserState.PromptUser_FilterSettings;
                    updateOnboardingStatus(_currentOnboardingUserState, null);
                    break;


                // Next button clicked from UserControlBCISignalCheckStartPrompt
                case "buttonNext_userControlBCISignalCheckStartPrompt": 
                    
                    // Get recheck request from button status
                    bool userRequestedRecheck = _mainForm._userControlBCISignalCheckStartPrompt.UserRequestedRecheck;
                    if (!userRequestedRecheck)
                    {
                        // If no recheck needed, transition out of gTecDeviceTester all together (everything successfully completed)
                        // User already did signal check within acceptable time frame and the result was good

                        // Save SignalQuality_RecheckNeeded
                        BCIGtecActuatorSettings.Settings.SignalControl_RecheckNeeded = false;
                        BCIGtecActuatorSettings.Save();

                        // We are exiting - Call Exit function with lost connection flag set to false
                        Exit(false);
                    }
                    else
                    {
                        // Save SignalQuality_RecheckNeeded
                        BCIGtecActuatorSettings.Settings.SignalControl_RecheckNeeded = true;
                        BCIGtecActuatorSettings.Save();

                        // Go to screen prompting user for correct filter setting (start of signal check process)
                        _currentOnboardingUserState = OnboardingUserState.PromptUser_FilterSettings;
                        updateOnboardingStatus(_currentOnboardingUserState, null);
                    }
                    break;


                // Next button clicked from UserControlBCIFilterSettings
                case "buttonNext_userControlPromptBCIFIlterSettings":

                    // Save filter settings from user's input
                    if (_mainForm._userControlPromptBCIFIlterSettings.checkBoxConfirm60HzCountry.Checked)
                    {
                        // DAQ_NotchFilterIdx = 1; //50Hz
                        // DAQ_NotchFilterIdx = 2; //60Hz
                        BCIGtecActuatorSettings.Settings.DAQ_NotchFilterIdx = 2;
                    }
                    else
                    {
                        BCIGtecActuatorSettings.Settings.DAQ_NotchFilterIdx = 1;
                    }

                    if (_mainForm._userControlPromptBCIFIlterSettings.checkBoxDontShowStartup.Checked)
                    {
                        BCIGtecActuatorSettings.Settings.DAQ_ShowFilterSettings = false;
                    }
                    else
                    {
                        BCIGtecActuatorSettings.Settings.DAQ_ShowFilterSettings = true;
                    }

                    BCIGtecActuatorSettings.Save();

                    // Go to signal check screen
                    _currentOnboardingUserState = OnboardingUserState.BCISignalCheck;
                    updateOnboardingStatus(_currentOnboardingUserState, null);

                    break;



                // Next button clicked from UserControlBCISignalCheck
                case "buttonNext_userControlBCISignalCheck":

                    // TODO: Set true for temporary
                    bool exitBCIOnboarding = true;

                    // Get current signal quality check status (user currently passes or fails the checks)
                    // Is updated every INTERVAL_UPDATE_OVERALL_SIGNAL_QUALITY_STATUS_MS while user is in signal check
                    bool userPassedLastSignalQualityCheck = BCIGtecActuatorSettings.Settings.SignalQuality_PassedLastOverallQualityCheck;
                    if (userPassedLastSignalQualityCheck)
                    {
                        Log.Debug("User passed most recent signal quality check");
                        exitBCIOnboarding = true;
                    }

                    // Check if testing parameter set to ignore signal quality check result
                    if (BCIGtecActuatorSettings.Settings.Testing_IgnoreSignalTestResultDuringOnboarding)
                    {
                        Log.Debug("BCIGtecActuatorSettings.Testing_IgnoreSignalTestResultDuringOnboarding = true");
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
                    int chnIdx = 0;
                    while (chnIdx < BCIGtecActuatorSettings.Settings.DAQ_NumEEGChannels)
                    {
                        channelNames[chnIdx] = UserControlBCISignalCheck._eegChannels[chnIdx]._electrodeName;
                        enabledChannels[chnIdx] = BCIGtecActuatorSettings.Settings.GetClassifier_EnableChannel(chnIdx);
                        railingValues[chnIdx] = (int)UserControlBCISignalCheck._eegChannels[chnIdx].lastRailingResult;
                        chnIdx += 1;
                    }

                    // NOTE: Impedance values are not currently being used in the gTecSensorUI
                    int[] impedanceValues = new int[16];
                    var bciLogEntry = new BCILogEntrySignalQuality(channelNames, enabledChannels, railingValues, impedanceValues, exitBCIOnboarding); // 5th param

                    var jsonString = JsonSerializer.Serialize(bciLogEntry);
                    AuditLog.Audit(new AuditEvent("BCISignalQuality", jsonString));

                    // Based on the status of all electrodes
                    // Either successfully close gTecDeviceTester (continue on to calibration) or display msg to user asking to clean up bad channels
                    if (exitBCIOnboarding)
                    {
                        // Do not modify Classifier_EnableChannel1-16, that's up to the user
                        // Just save settings, set appropriate flags, and exit
                        BCIGtecActuatorSettings.Save(); // Save settings

                        ExitOnboardingEarly = false; // Set global flag denoting onboarding was not exited early

                        Exit(false); // Call Exit function with lost connection flag set to false
                    }

                    // Do not exit
                    else
                    {
                        // Display message to user prompting them to improve signal quality before moving on
                        Log.Debug("Not exiting | Did not pass signal quality criteria");
                        bool confirmed = ConfirmBoxOneOption.ShowDialog("Signal Quality Checks Failed or Incomplete" +
                        "\nYou need to complete both “Railing” and\n“Impedance” tests and get good signals to\nproceed" +
                        "\nPlease refer to the user guide for help", "", StringResources.OK, _mainForm, false);
                    }

                    break;

                default:
                    break;
            }
        }



        /// <summary>
        /// Determines if signal check is required or asks user if they want to do one. Navigates to the corresponding screen
        /// </summary>
        private void runSignalCheckIfRequired()
        {
            Log.Debug("gTecDeviceTester | runSignalCheckIfRequired");


            // Always check time last impedance test was run (all electrodes tested) and update UI accordingly
            long timestampPrevImpedanceTest = BCIGtecActuatorSettings.Settings.SignalQuality_TimeOfLastImpedanceCheck;
            long timestampNow = DateTimeOffset.Now.ToUnixTimeSeconds();
            long secDiff = timestampNow - timestampPrevImpedanceTest;
            double minElapsedPrevSignalQualityCheck = ((double)secDiff) / 60;
            double maxTimeMins = (double)BCIGtecActuatorSettings.Settings.SignalQuality_MaxTimeMinsElapsedSinceLastImpedanceCheck​;
            bool maxTimeHasElapsed = false;
            if (minElapsedPrevSignalQualityCheck >= maxTimeMins)
                maxTimeHasElapsed = true;
            Log.Debug(String.Format("runSignalCheckIfRequired | timestampPrevImpedanceTest: {0}, timestampNow: {1}, secDiff: {2}", timestampPrevImpedanceTest.ToString(), timestampNow.ToString(), secDiff.ToString()));
            Log.Debug(String.Format("minElapsedPrevSignalQualityCheck: {0}, maxTimeMins: {1}, maxTimeHasElapsed: {2}", minElapsedPrevSignalQualityCheck.ToString(), maxTimeMins.ToString(), maxTimeHasElapsed.ToString()));

            // Always check if user passed the last overall signal quality check that was executed
            // If max time has not passed, but user did not pass their most recent overall signal quality check,
            // user must do tests and calibration (SignalControl_RecheckNeeded = true)
            bool userPassedLastSignalQualityCheck = BCIGtecActuatorSettings.Settings.SignalQuality_PassedLastOverallQualityCheck;


            // Initialize parameters and set processing variables / UI elements in main signal check screen accordingly
            _mainForm._userControlBCISignalCheck.initializeBCISignalCheck(gTecBCI, maxTimeHasElapsed, maxTimeMins, minElapsedPrevSignalQualityCheck, userPassedLastSignalQualityCheck);


            // Go to screen telling user that signal check required because maximum time between signal checks has elapsed
            if (maxTimeHasElapsed)
            {
                _currentOnboardingUserState = OnboardingUserState.SignalCheckRequired_MaxTimeElapsed;
                Dictionary<string, object> dictionary = new Dictionary<String, object>
                {
                    ["maxTimeMins"] = maxTimeMins
                };
                Dictionary<String, object> resultParms = dictionary;
                updateOnboardingStatus(_currentOnboardingUserState, resultParms);
            }

            // Go to screen telling user that signal check required because they failed their most recent one
            else if (!maxTimeHasElapsed && !userPassedLastSignalQualityCheck)
            {
                _currentOnboardingUserState = OnboardingUserState.SignalCheckRequired_FailedRecentSignalCheck;
                updateOnboardingStatus(_currentOnboardingUserState, null);
            }

            // Go to screen asking user if they want to do a signal quality check
            else if (!maxTimeHasElapsed && userPassedLastSignalQualityCheck)
            {
                _currentOnboardingUserState = OnboardingUserState.PromptUser_DoSignalCheck;
                updateOnboardingStatus(_currentOnboardingUserState, null);
            }
        }


        /// <summary>
        /// Handler for Retest button click
        /// </summary>
        private void buttonRetestHandler(object sender)
        {
            // Retest BCI connections - can just run function testing usb and bluetooth connections
            // Do not need to handle separate states based on where retest request is coming from

            // Go to screen that displays "connecting..." status
            _currentOnboardingUserState = OnboardingUserState.Testing_BCIConnections;
            updateOnboardingStatus(_currentOnboardingUserState, null);

            // Non-zero value is needed for startBCIDeviceTesting so "connecting..." screen has time to show to the user
            Thread t = new Thread(() => startBCIDeviceTesting(3)); 
            t.Start();
        }


        /// <summary>
        /// Handler for Exit button click - dispayed on all device testing screens and does the same thing,
        /// completely exits testing process early without completion
        /// </summary>
        private void buttonExitHandler(String buttonExitName)
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
        /// Called when user selects Exit or Next button from signal check
        /// </summary>
        public void Exit(bool lostConnection)
        {
            // Do something with the lostConnection flag? - Might not be needed

            // Set flags that will end async tasks and timers
            _endSignalCheckTimer = true;

            // Release event handlers at this level
            EvtUpdateOnboardingStatus = null;

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
        private void _mainForm_EvtFormClosed(object sender, FormClosedEventArgs e)
        {
            Log.Debug("gTecDeviceTester | _mainForm_EvtFormClosed");
        }


        /// <summary>
        /// Display confirmation to quit BCI onboarding
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        private bool confirmExit(Form parent)
        {
            return ConfirmBoxTwoOption.ShowDialog("Onboarding incomplete.",
                "Quit anyway?", StringResources.Yes, StringResources.No, parent, true);
        }


        /// <summary>
        /// Initializes debug states to step through all the onboarding screens
        /// </summary>
        private void intializeDebugStates()
        {
            _DebugStates = new OnboardingUserState[8];
            _DebugStates[0] = OnboardingUserState.Testing_BCIConnections;
            //_DebugStates[1] = OnboardingUserState.ErrorUsbDeviceDisconnected;
            _DebugStates[1] = OnboardingUserState.ErrorBluetoothDisconnected;
            _DebugStates[2] = OnboardingUserState.SignalCheckRequired_MaxTimeElapsed;
            _DebugStates[3] = OnboardingUserState.SignalCheckRequired_FailedRecentSignalCheck;
            _DebugStates[4] = OnboardingUserState.PromptUser_DoSignalCheck;
            _DebugStates[5] = OnboardingUserState.PromptUser_FilterSettings;
            _DebugStates[6] = OnboardingUserState.BCISignalCheck;
            _DebugStates[7] = OnboardingUserState.ExitBCITesting;
        }


        /// <summary>
        /// Handler for button click in debug mode - iterate through all available screens / user controls
        /// </summary>
        private void _mainForm_EvtButtonExitClicked_DEBUG(object sender)
        {
            try
            {
                // We are exiting - Call Exit function with lost connection flag set to false
                Exit(false);
            }
            catch (Exception e)
            {
                Log.Debug("_mainForm_EvtButtonExitClicked_DEBUG exception: " + e.ToString());
            }
        }
    }
}