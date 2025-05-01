////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// SensorForm.cs
//
// Main form / UI for BCI Onboarding and Signal Check
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Extensions.BCI.Actuators.EEG.EEGDataAcquisition;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ACAT.Extensions.BCI.Actuators.gTecSensorUI.GTecDeviceTester;
using static ACAT.Extensions.BCI.Actuators.gTecSensorUI.UserControlBCISignalCheck;

namespace ACAT.Extensions.BCI.Actuators.gTecSensorUI
{
    /// <summary>
    /// Main form / UI for BCI Onboarding and Signal Check
    /// </summary>
    public partial class SensorForm : Form
    {
        /// <summary>
        /// Variable storing connection manager for gTec BCI device 
        /// </summary>
        private DAQ_gTecBCI _gTecBCI = null;
        
        /// <summary>
        /// User control displayed while trying to connect to sensor
        /// </summary>
        public UserControlTestBCIConnections _userControlTestBCIConnections;

        /// <summary>
        /// User control displayed after receiving gTec board error
        /// </summary>
        public UserControlBCIErrorGTecBoard _userControlBCIErrorgTecBoard;

        /// <summary>
        /// User control displayed after receiving usb dongle error
        /// </summary>
        public UserControlErrorBluetoothDisconnected _userControlErrorBluetoothDisconnected;

        /// <summary>
        /// User control displayed for starting signal check process - when maximum time has elapsed
        /// since last test
        /// </summary>
        public UserControlBCISignalCheckStartRequired _userControlBCISignalCheckStartRequired;

        /// <summary>
        //// User control displayed for starting signal check process - prompts user for signal check
        /// based on a couple questions
        /// </summary>
        public UserControlBCISignalCheckStartPrompt _userControlBCISignalCheckStartPrompt;

        /// <summary>
        /// User control displayed for prompting user about filter settings
        /// Display EEG signals screen
        /// </summary>
        public UserControlBCIFilterSettings _userControlPromptBCIFIlterSettings;

        /// <summary>
        /// User control displayed if didn't receieve any errors while trying to connect to BCI sensor
        /// Display EEG signals view
        /// </summary>
        public UserControlBCISignalCheck _userControlBCISignalCheck;

        /// <summary>
        /// Variable tracking the previous OnboardingUserState
        /// </summary>
        private OnboardingUserState prevOnboardingState;

        /// <summary>
        /// Delegate for button click events
        /// </summary>
        public delegate void ButtonClickedDelegate(String buttonNextName);

        /// <summary>
        /// Event called when Next button selected
        /// </summary>
        public event ButtonClickedDelegate EvtButtonNextClicked;

        /// <summary>
        /// Event called when Retry button selected
        /// </summary>
        public event ButtonClickedDelegate EvtButtonRetestClicked;

        /// <summary>
        /// Event called when Exit button selected
        /// </summary>
        public event ButtonClickedDelegate EvtButtonExitClicked;
        
        /// <summary>
        /// Flag controlling exectution of timers related to data acquisition / plotting
        /// </summary>
        public static bool _stopTimers = false;

        /// <summary>
        /// Interval in milliseconds at which timer event for acquiring and processing data is called
        /// </summary>
        private int _timer_process_data_interval_ms = 10;

        // Variable storing current user control shown to the user
        public UserControl _currentUserControlShown;

        // Form which acts as parent for / base for all possible user controls displayed during testing process
        public SensorForm(DAQ_gTecBCI gTecBCI)
        {
            InitializeComponent();

            this.WindowState = FormWindowState.Maximized;

            // Set initial / default values of static variables
            _stopTimers = false;

            // Preinitialize all user control elements that can be possibly shown
            // Intensive resource controls (ex: Optical sensor, EEG display) have separate initialize() functions that are not called until
            // user controls actually added to form
            _userControlTestBCIConnections = new UserControlTestBCIConnections();
            _userControlTestBCIConnections.buttonExit_userControlTestBCIConnections.Click += new System.EventHandler(this.buttonExit_Click);

            _userControlBCIErrorgTecBoard = new UserControlBCIErrorGTecBoard();
            _userControlBCIErrorgTecBoard.buttonExit_userControlBCIErrorgTecBoard.Click += new System.EventHandler(this.buttonExit_Click);
            _userControlBCIErrorgTecBoard.buttonRetry_userControlBCIErrorgTecBoard.Click += new System.EventHandler(this.buttonRetest_Click);

            _userControlErrorBluetoothDisconnected = new UserControlErrorBluetoothDisconnected();
            _userControlErrorBluetoothDisconnected.buttonExit_userControlErrorBluetoothDisconnected.Click += new System.EventHandler(this.buttonExit_Click);
            _userControlErrorBluetoothDisconnected.buttonNext_userControlErrorBluetoothDisconnected.Click += new System.EventHandler(this.buttonNext_Click);

            _userControlBCISignalCheckStartRequired = new UserControlBCISignalCheckStartRequired();
            _userControlBCISignalCheckStartRequired.buttonExit_userControlBCISignalCheckStartRequired.Click += new System.EventHandler(this.buttonExit_Click);
            _userControlBCISignalCheckStartRequired.buttonNext_userControlBCISignalCheckStartRequired.Click += new System.EventHandler(this.buttonNext_Click);

            _userControlBCISignalCheckStartPrompt = new UserControlBCISignalCheckStartPrompt();
            _userControlBCISignalCheckStartPrompt.buttonExit_userControlBCISignalCheckStartPrompt.Click += new System.EventHandler(this.buttonExit_Click);
            _userControlBCISignalCheckStartPrompt.buttonNext_userControlBCISignalCheckStartPrompt.Click += new System.EventHandler(this.buttonNext_Click);

            _userControlPromptBCIFIlterSettings = new UserControlBCIFilterSettings();
            _userControlPromptBCIFIlterSettings.buttonExit_userControlPromptBCIFIlterSettings.Click += new System.EventHandler(this.buttonExit_Click);
            _userControlPromptBCIFIlterSettings.buttonNext_userControlPromptBCIFIlterSettings.Click += new System.EventHandler(this.buttonNext_Click);

            _userControlBCISignalCheck = new UserControlBCISignalCheck();
            _userControlBCISignalCheck.buttonExit_userControlBCISignalCheck.Click += new System.EventHandler(this.buttonExit_Click);
            _userControlBCISignalCheck.buttonNext_userControlBCISignalCheck.Click += new System.EventHandler(this.buttonNext_Click);

            _gTecBCI = gTecBCI;

            // Set current signal check view mode for last screens
            // Default = Railing Test screen
            //UserControlBCISignalCheck._currentBCISignalCheckMode = BCISignalCheckMode.TEST_RAILING;

            if (!GTecDeviceTester._Testing_useSensor)
            {
                // Set Exit button on each user control screen to [Developer Mode] which iterates through all available screens on button press
                modifyUserControlsForDebugMode();
            }

            Paint += (s, args) => { };

            FormClosing += Handle_FormCLosing;

        }

        /// <summary>
        /// Replaces user control displayed in tableLayoutPanelContainer depending on OnboardingUserState
        /// </summary>
        /// <param name="state"></param>
        public void updateOnboardingStatus(GTecDeviceTester.OnboardingUserState state, Dictionary<String, object> resultParams)
        {
            Log.Debug("SensorForm | updateOnboardingStatus | state: " + state.ToString());
            UserControl newUserControl = null;

            switch (state)
            {
                // Go to screen that displays "connecting..." status
                case OnboardingUserState.Testing_BCIConnections:
                    newUserControl = _userControlTestBCIConnections;
                    break;

                /*
                // Go to screen telling user usb device could not be detected
                case OnboardingUserState.ErrorUsbDeviceDisconnected:
                    newUserControl = _userControlBCIErrorgTecBoard;
                    break;
                */

                // Go to screen directing user to connect their unicorn device through bluetooth pairing
                case OnboardingUserState.ErrorBluetoothDisconnected:
                    if(prevOnboardingState != OnboardingUserState.ErrorBluetoothDisconnected)
                    {
                        newUserControl = _userControlErrorBluetoothDisconnected;
                    }
                    break;

                // Go to screen telling user that signal check required because maximum time between signal checks has elapsed
                case OnboardingUserState.SignalCheckRequired_MaxTimeElapsed:
                    newUserControl = _userControlBCISignalCheckStartRequired;

                    // Update label with maximum time that has already passed since previous test
                    _userControlBCISignalCheckStartRequired.labelMinsElapsedSignalCheckStartRequired.Text = String.Format("{0:0} minutes", (double) resultParams["maxTimeMins"]);
                    break;

                // Go to screen telling user that signal check required because they failed their most recent one
                case OnboardingUserState.SignalCheckRequired_FailedRecentSignalCheck:
                    newUserControl = _userControlBCISignalCheckStartRequired;

                    // Update label telling user they failed previous signal quality check
                    _userControlBCISignalCheckStartRequired.labelInfo1SignalCheckStartRequired.Text = "You did not pass your most recent signal quality check";
                    _userControlBCISignalCheckStartRequired.labelMinsElapsedSignalCheckStartRequired.Text = "";
                    _userControlBCISignalCheckStartRequired.labelInfo2SignalCheckStartRequired.Text = "";
                    break;

                // Go to screen asking user if they want to do a signal quality check
                case OnboardingUserState.PromptUser_DoSignalCheck:
                    // Always reset checkbox (set to false) asking if user wants to do signal recheck when accessing user control
                    _userControlBCISignalCheckStartPrompt.resetCheckbox();
                    newUserControl = _userControlBCISignalCheckStartPrompt;
                    break;

                // Go to screen asking user if they want to do a signal quality check
                case OnboardingUserState.PromptUser_FilterSettings:
                    newUserControl = _userControlPromptBCIFIlterSettings;
                    break;

                // Go to signal check screen
                case OnboardingUserState.BCISignalCheck:
                    newUserControl = _userControlBCISignalCheck;                    
                    break;

                default:
                    break;
            }


            if (newUserControl != null)
            {
                // Every screen except last one - display in normal 1024 x 768 dimensions
                if (state != OnboardingUserState.BCISignalCheck)
                {
                    tableLayoutPanelContainer.Controls.Clear();
                    tableLayoutPanelContainer.Controls.Add(newUserControl, 0, 0);
                    tableLayoutPanelContainer.Refresh();
                }

                // Last screen - BCI signal check screen created with special layout
                // Basically remove everything in existing table layout panel and re-add to work with bigger format
                else if (state == OnboardingUserState.BCISignalCheck)
                {
                    bool displayReminderGelElectrodes = false;

                    //Previous selection was not a BCISignalCheck screen
                    if (prevOnboardingState != OnboardingUserState.BCISignalCheck)
                    {
                        // Remove all controls tableLayoutPanelContainer and then tableLayoutPanelContainer itself
                        tableLayoutPanelContainer.Controls.Clear();
                        tableLayoutPanelMain.Controls.Clear();

                        // Clear row and column styles
                        tableLayoutPanelMain.RowStyles.Clear();
                        tableLayoutPanelMain.ColumnStyles.Clear();

                        // Set 1 column and row style such that user control will appear in the top right of primary screen (TriggerBox placed correctly)
                        tableLayoutPanelMain.ColumnCount = 1;
                        tableLayoutPanelMain.RowCount = 1;
                        tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 1920));
                        tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 1080));
                        tableLayoutPanelMain.Controls.Add(newUserControl, 0, 0);

                        newUserControl.Dock = DockStyle.Fill;
                        tableLayoutPanelMain.Refresh();

                        // Always start at railing test tab
                        UserControlBCISignalCheck._currentBCISignalCheckMode = UserControlBCISignalCheck.BCISignalCheckMode.TEST_RAILING;

                        // If accessing signal check for first time - display reminder to gel important electrodes
                        displayReminderGelElectrodes = true;
                    }

                    if (UserControlBCISignalCheck._currentBCISignalCheckMode == UserControlBCISignalCheck.BCISignalCheckMode.TEST_RAILING)
                    {
                        _userControlBCISignalCheck.changeSignalCheckMode(BCISignalCheckMode.TEST_RAILING);

                        if (displayReminderGelElectrodes)
                        {
                            bool confirmed = ConfirmBoxSingleOption.ShowDialog("Please remember to add gel to GND and T4 electrodes, if you have not already", "OK", this, false);
                        }
                    }

                    // Start task that will launch data processing / plotting for signal check screen
                    if (GTecDeviceTester._Testing_useSensor)
                        TaskStartStopDataProcessing(OnboardingUserState.BCISignalCheck);
                }


                if (newUserControl == _userControlErrorBluetoothDisconnected)
                {
                    _userControlErrorBluetoothDisconnected.startStopUpdateBluetoothListTimer(true);
                }
                else
                {
                    _userControlErrorBluetoothDisconnected.startStopUpdateBluetoothListTimer(false);
                }

                //// Start task that will launch data processing / plotting for signal check screen
                //if (GTecDeviceTester._Testing_useSensor)
                    //TaskStartStopDataProcessing(OnboardingUserState.BCISignalCheck);

                _currentUserControlShown = newUserControl;
            }

            prevOnboardingState = state;
           
        }


        /// <summary>
        /// Task in charge to start of starting / stopping data processing timer
        /// </summary>
        /// <returns></returns>
        /// Run only once per new set of state changes receieved
        public async Task TaskStartStopDataProcessing(OnboardingUserState state)
        {
            // Start task which processes data for signal status checks
            if (state == OnboardingUserState.BCISignalCheck)
            {
                Invoke(new Action(() =>
                {
                    startStopProcessDataTimer(true, state);
                }));
            }
            else
            {
                Invoke(new Action(() =>
                {
                    startStopProcessDataTimer(false, state);
                }));
            }
        }

        /// <summary>
        /// Start / stop timer which processes signal status
        /// </summary>
        /// <param name="state"></param>
        private void startStopProcessDataTimer(bool startProcessDataTimer, OnboardingUserState state)
        {
            Log.Debug("startStopProcessDataTimer | startProcessDataTimer: " + startProcessDataTimer.ToString() +
                " | state: " + state.ToString());

            if (startProcessDataTimer)
            {
                try
                {
                    if (timerProcessData != null && timerProcessData.Enabled)
                    {
                        timerProcessData.Stop();
                        timerProcessData.Enabled = false;
                        timerProcessData.Dispose();
                        timerProcessData = null;
                    }

                    timerProcessData = new System.Windows.Forms.Timer(this.components);
                    timerProcessData.Enabled = true;
                    timerProcessData.Interval = _timer_process_data_interval_ms;
                    timerProcessData.Stop();

                    if (state == OnboardingUserState.BCISignalCheck)
                    {
                        timerProcessData.Tick += new System.EventHandler(ProcessDataSignalCheck_Tick);
                    }

                    timerProcessData.Start();
                    Log.Debug("startStopProcessDataTimer | Started timerProcessData");
                }
                catch (Exception e)
                {
                    Log.Debug("startStopProcessDataTimer | Exception: " + e.ToString());
                }
            }
            else
            {
                try
                {
                    if (timerProcessData != null && timerProcessData.Enabled)
                    {
                        timerProcessData.Stop();
                        timerProcessData.Enabled = false;
                        timerProcessData.Dispose();
                        timerProcessData = null;
                    }
                }
                catch (Exception e)
                {
                    Log.Debug("startStopProcessDataTimer | Exception: " + e.ToString());
                }
            }

        }


        /// <summary>
        /// Obtain, process, and plot BCI EEG data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessDataSignalCheck_Tick(object sender, EventArgs e)
        {
            // Check flag to stop all timers (checked during possible timer tick)
            // Check flag to stop this particular timer
            if (_stopTimers || GTecDeviceTester._endSignalCheckTimer)
            {
                Log.Debug("ProcessDataSignalCheck_Tick | _stopTimers | GTecDeviceTester._endSignalCheckTimer");
                startStopProcessDataTimer(false, OnboardingUserState.ExitBCITesting);
                return;
            }

            if (_gTecBCI.deviceInitialized)
            {
                // Obtain sensor data stored in buffers and pass to signal check user control
                double[,] data = _gTecBCI.GetData();

                if (data != null && data.Length > 0 && data.GetLength(1) > 0)
                {
                    double[,] dataCopy = (double[,])data.Clone();
                    double[,] DAQ_filteredData = _gTecBCI.daq_filter_data(dataCopy);

                    if (_userControlBCISignalCheck != null)
                        _userControlBCISignalCheck.ProcessDataSignalCheck(data, DAQ_filteredData);
                }
            }
        }


        /// <summary>
        /// Dispose all objects and task used by Signal monitor
        /// </summary>
        private void Handle_FormCLosing(object sender, FormClosingEventArgs e)
        {
            bool closeReasonIsUserClosing = false;

            /*// Skip this functionality for now - bug with skipping signal check to go straight to calibration
            switch (e.CloseReason)
            {
                // Do not close form if user does Alt + F4 (CloseReason = UserClosing)
                case CloseReason.UserClosing:

                    // Only exit if ExitOnboardingEarly flag has been set (user selected Exit button)
                    if (!ExitOnboardingEarly)
                    {
                        Log.Debug("User has requested to close form (Alt + F4) - ignore");
                        e.Cancel = true;
                        closeReasonIsUserClosing = true;
                    }
                    break;
            }*/

            // We are closing
            if (!closeReasonIsUserClosing)
            {
                // Set flag to stop any timer tick in the middle of execution
                _stopTimers = true;

                // Stop plot data timer
                // startStopPlotDataTimer(false, OnboardingUserState.ExitBCITesting);

                // Stop process data timer
                startStopProcessDataTimer(false, OnboardingUserState.ExitBCITesting);

                // Release resources (remove events and handlers and dispose of user controls)
                if (EvtButtonNextClicked != null)
                    EvtButtonNextClicked = null;
                if (EvtButtonRetestClicked != null)
                    EvtButtonRetestClicked = null;
                if (EvtButtonExitClicked != null)
                    EvtButtonExitClicked = null;

                if (_userControlTestBCIConnections != null)
                    _userControlTestBCIConnections.Dispose();
                if (_userControlBCIErrorgTecBoard != null)
                    _userControlBCIErrorgTecBoard.Dispose();
                if (_userControlErrorBluetoothDisconnected != null)
                    _userControlErrorBluetoothDisconnected.Dispose();
                if (_userControlBCISignalCheckStartRequired != null)
                    _userControlBCISignalCheckStartRequired.Dispose();
                if (_userControlBCISignalCheckStartPrompt != null)
                    _userControlBCISignalCheckStartPrompt.Dispose();
                if (_userControlPromptBCIFIlterSettings != null)
                    _userControlPromptBCIFIlterSettings.Dispose();
                if (_userControlBCISignalCheck != null)
                    _userControlBCISignalCheck.Dispose();
            }
        }

        /// <summary>
        /// Handler for Next button click - send current device testing state with button press event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (EvtButtonNextClicked != null)
            {
                EvtButtonNextClicked(((Button)sender).Name);
            }
        }

        /// <summary>
        /// Handler for Cancel button click - call appropriate event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonExit_Click(object sender, EventArgs e)
        {
            if (EvtButtonExitClicked != null)
            {
                EvtButtonExitClicked(((Button)sender).Name);
            }
        }

        /// <summary>
        ///  Handler for Retest button click - call appropriate event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRetest_Click(object sender, EventArgs e)
        {
            if (EvtButtonRetestClicked != null)
            {
                EvtButtonRetestClicked(((Button)sender).Name);
            }
        }

        /// <summary>
        /// Debugging / Developer Mode.
        /// Set Cancel button in each user control to red text and clicking it increments debug index which moves to the next user control
        /// </summary>
        private void modifyUserControlsForDebugMode()
        {
            Log.Debug("SensorForm | modifyUserControlsForDebugMode");

            _userControlTestBCIConnections.buttonExit_userControlTestBCIConnections.AutoSize = true;
            _userControlTestBCIConnections.buttonExit_userControlTestBCIConnections.Font = new System.Drawing.Font("Montserrat Medium", 13F);
            _userControlTestBCIConnections.buttonExit_userControlTestBCIConnections.ForeColor = Color.Red;
            _userControlTestBCIConnections.buttonExit_userControlTestBCIConnections.Text = "[Developer Mode]";

            _userControlBCIErrorgTecBoard.buttonExit_userControlBCIErrorgTecBoard.AutoSize = true;
            _userControlBCIErrorgTecBoard.buttonExit_userControlBCIErrorgTecBoard.Font = new System.Drawing.Font("Montserrat Medium", 13F);
            _userControlBCIErrorgTecBoard.buttonExit_userControlBCIErrorgTecBoard.ForeColor = Color.Red;
            _userControlBCIErrorgTecBoard.buttonExit_userControlBCIErrorgTecBoard.Text = "[Developer Mode]";

            _userControlErrorBluetoothDisconnected.buttonExit_userControlErrorBluetoothDisconnected.AutoSize = true;
            _userControlErrorBluetoothDisconnected.buttonExit_userControlErrorBluetoothDisconnected.Font = new System.Drawing.Font("Montserrat Medium", 13F);
            _userControlErrorBluetoothDisconnected.buttonExit_userControlErrorBluetoothDisconnected.ForeColor = Color.Red;
            _userControlErrorBluetoothDisconnected.buttonExit_userControlErrorBluetoothDisconnected.Text = "[Developer Mode]";

            _userControlBCISignalCheckStartRequired.buttonExit_userControlBCISignalCheckStartRequired.AutoSize = true;
            _userControlBCISignalCheckStartRequired.buttonExit_userControlBCISignalCheckStartRequired.Font = new System.Drawing.Font("Montserrat Medium", 11F);
            _userControlBCISignalCheckStartRequired.buttonExit_userControlBCISignalCheckStartRequired.ForeColor = Color.Red;
            _userControlBCISignalCheckStartRequired.buttonExit_userControlBCISignalCheckStartRequired.Text = "[Developer Mode]";

            _userControlBCISignalCheckStartPrompt.buttonExit_userControlBCISignalCheckStartPrompt.AutoSize = true;
            _userControlBCISignalCheckStartPrompt.buttonExit_userControlBCISignalCheckStartPrompt.Font = new System.Drawing.Font("Montserrat Medium", 11F);
            _userControlBCISignalCheckStartPrompt.buttonExit_userControlBCISignalCheckStartPrompt.ForeColor = Color.Red;
            _userControlBCISignalCheckStartPrompt.buttonExit_userControlBCISignalCheckStartPrompt.Text = "[Developer Mode]";

            _userControlPromptBCIFIlterSettings.buttonExit_userControlPromptBCIFIlterSettings.AutoSize = true;
            _userControlPromptBCIFIlterSettings.buttonExit_userControlPromptBCIFIlterSettings.Font = new System.Drawing.Font("Montserrat Medium", 11F);
            _userControlPromptBCIFIlterSettings.buttonExit_userControlPromptBCIFIlterSettings.ForeColor = Color.Red;
            _userControlPromptBCIFIlterSettings.buttonExit_userControlPromptBCIFIlterSettings.Text = "[Developer Mode]";

            _userControlBCISignalCheck.buttonExit_userControlBCISignalCheck.AutoSize = true;
            _userControlBCISignalCheck.buttonExit_userControlBCISignalCheck.Font = new System.Drawing.Font("Montserrat Medium", 11F);
            _userControlBCISignalCheck.buttonExit_userControlBCISignalCheck.ForeColor = Color.Red;
            _userControlBCISignalCheck.buttonExit_userControlBCISignalCheck.Text = "[Developer Mode]";
        }
    }

}