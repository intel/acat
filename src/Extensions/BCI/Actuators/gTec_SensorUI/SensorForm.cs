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
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ACAT.Extensions.BCI.Actuators.SensorUI.BCIDeviceTester;
using static ACAT.Extensions.BCI.Actuators.SensorUI.UserControlBCISignalCheck;

namespace ACAT.Extensions.BCI.Actuators.SensorUI
{
    /// <summary>
    /// Main form / UI for BCI Onboarding and Signal Check
    /// </summary>
    public partial class SensorForm : Form
    {
        /// <summary>
        /// User control displayed while trying to connect to sensor
        /// </summary>
        public UserControlTestBCIConnections _userControlTestBCIConnections;

        /// <summary>
        /// User control displayed after receiving Cyton board error
        /// </summary>
        public UserControlBCIErrorCytonBoard _userControlBCIErrorCytonBoard;

        /// <summary>
        /// User control displayed after receiving usb dongle error
        /// </summary>
        public UserControlBCIErrorUsbDongle _userControlBCIErrorUsbDongle;

        /// <summary>
        /// User control displayed after receiving port configuration error
        /// </summary>
        public UserControlBCIErrorPortConfig _userControlBCIErrorPortConfig;

        /// <summary>
        /// User control displayed after receieving optical sensor error
        /// </summary>
        public UserControlBCIErrorOpticalSensor _userControlBCIErrorOpticalSensor;

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

        public UserControlBCIErrorOpticalSensorDetect _userControlBCIErrorOpticalSensorDetect;

        /// <summary>
        /// Current devie testing state
        /// </summary>
        //private static volatile BCIDeviceTester.DeviceTestingState _currentDeviceTestingState;
        private BCIDeviceTester.DeviceTestingState _mainFormDeviceTestingState;

        /// <summary>
        /// Delegate for button click events
        /// </summary>
        public delegate void ButtonNextClickedDelegate(DeviceTestingState deviceTestingState);

        public delegate void ButtonClickedDelegate(object sender);

        /// <summary>
        /// Event called when Next button selected
        /// </summary>
        public event ButtonNextClickedDelegate EvtButtonNextClicked;

        /// <summary>
        /// Event called when Retry button selected
        /// </summary>
        public event ButtonClickedDelegate EvtButtonRetestClicked;

        /// <summary>
        /// Event called when Exit button selected
        /// </summary>
        public event ButtonClickedDelegate EvtButtonCancelClicked;
        
        /// <summary>
        /// Flag controlling exectution of timers related to data acquisition / plotting
        /// </summary>
        public static bool _stopTimers = false;

        /// <summary>
        /// Interval in milliseconds at which timer event for plotting optical sensor data fires
        /// </summary>
        private int _timer_plot_data_interval_ms = 40;

        /// <summary>
        /// Interval in milliseconds at which timer event for acquiring and processing data fires
        /// </summary>
        private int _timer_process_data_interval_ms = 10;

        // Form which acts as parent for / base for all possible user controls displayed during testing process
        public SensorForm(BCIDeviceTester.DeviceTestingState initialState)
        {
            InitializeComponent();
            TriggerBox.BackColor = Color.Black;
            this.WindowState = FormWindowState.Maximized;

            // Set initial / default values of static variables
            _stopTimers = false;
            _mainFormDeviceTestingState = initialState;

            // Preinitialize all user control elements that can be possibly shown
            // Intensive resource controls (ex: Optical sensor, EEG display) have separate initialize() functions that are not called until
            // user controls actually added to form
            _userControlTestBCIConnections = new UserControlTestBCIConnections(BCIDeviceTester.DeviceTestingState.Testing_BCIConnections.ToString());
            _userControlTestBCIConnections.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);

            _userControlBCIErrorCytonBoard = new UserControlBCIErrorCytonBoard(BCIDeviceTester.DeviceTestingState.ReceivedBCIError_CytonBoard.ToString());
            _userControlBCIErrorCytonBoard.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            _userControlBCIErrorCytonBoard.buttonRetry.Click += new System.EventHandler(this.buttonRetest_Click);

            _userControlBCIErrorUsbDongle = new UserControlBCIErrorUsbDongle(BCIDeviceTester.DeviceTestingState.ReceivedBCIError_UsbDongle.ToString());
            _userControlBCIErrorUsbDongle.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            _userControlBCIErrorUsbDongle.buttonRetry.Click += new System.EventHandler(this.buttonRetest_Click);

            _userControlBCIErrorPortConfig = new UserControlBCIErrorPortConfig(BCIDeviceTester.DeviceTestingState.ReceivedBCIError_PortConfig.ToString());
            _userControlBCIErrorPortConfig.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);

            _userControlBCIErrorOpticalSensor = new UserControlBCIErrorOpticalSensor(BCIDeviceTester.DeviceTestingState.ReceivedBCIError_OpticalSensor.ToString());
            _userControlBCIErrorOpticalSensor.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            _userControlBCIErrorOpticalSensor.buttonRetry.Click += new System.EventHandler(this.buttonRetest_Click);

            _userControlBCISignalCheckStartRequired = new UserControlBCISignalCheckStartRequired(BCIDeviceTester.DeviceTestingState.BCISignalCheckStartRequired.ToString() + "_Required");
            _userControlBCISignalCheckStartRequired.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            _userControlBCISignalCheckStartRequired.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);

            _userControlBCISignalCheckStartPrompt = new UserControlBCISignalCheckStartPrompt(BCIDeviceTester.DeviceTestingState.BCISignalCheckStartPrompt.ToString() + "_Prompt");
            _userControlBCISignalCheckStartPrompt.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            _userControlBCISignalCheckStartPrompt.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);

            _userControlPromptBCIFIlterSettings = new UserControlBCIFilterSettings(BCIDeviceTester.DeviceTestingState.PromptFilterSettings.ToString());
            _userControlPromptBCIFIlterSettings.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            _userControlPromptBCIFIlterSettings.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);

            _userControlBCISignalCheck = new UserControlBCISignalCheck(BCIDeviceTester.DeviceTestingState.BCISignalCheck.ToString());
            _userControlBCISignalCheck.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            _userControlBCISignalCheck.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);

            _userControlBCIErrorOpticalSensorDetect = new UserControlBCIErrorOpticalSensorDetect(BCIDeviceTester.DeviceTestingState.OpticalSensorDetectError.ToString());
            _userControlBCIErrorOpticalSensorDetect.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            _userControlBCIErrorOpticalSensorDetect.buttonRetry.Click += new System.EventHandler(this.buttonRetest_Click);

            // Set current signal check view mode for last screens
            // Default = Railing Test screen
            UserControlBCISignalCheck._currentBCISignalCheckMode = BCISignalCheckMode.TEST_RAILING;

            if (!BCIDeviceTester._Testing_useSensor)
            {
                // Set Exit button on each user control screen to [Developer Mode] which iterates through all available screens on button press
                modifyUserControlsForDebugMode();
            }

            Paint += (s, args) => { };

            FormClosing += Handle_FormCLosing;

            changeDeviceTestingState(initialState);
        }

        /// <summary>
        /// Replaces user control displayed in tableLayoutPanelContainer
        /// </summary>
        /// <param name="state"></param>
        public void changeDeviceTestingState(BCIDeviceTester.DeviceTestingState state)
        {
            Log.Debug("SensorForm | changeDeviceTestingState | state: " + state.ToString());

            DeviceTestingState prevDeviceTestingState = _mainFormDeviceTestingState;
            UserControl newUserControl = null;
            _mainFormDeviceTestingState = state;

            // Get correct user control to display based on error received (or none) during testing process
            if (state == DeviceTestingState.Testing_BCIConnections)
            {
                newUserControl = _userControlTestBCIConnections;
                ((UserControlTestBCIConnections)newUserControl).pictureBoxTestBCIConnections.Refresh();
            }
            else if (state == DeviceTestingState.ReceivedBCIError_UsbDongle)
            {
                newUserControl = _userControlBCIErrorUsbDongle;
            }
            else if (state == DeviceTestingState.ReceivedBCIError_CytonBoard)
            {
                newUserControl = _userControlBCIErrorCytonBoard;
            }
            else if (state == DeviceTestingState.ReceivedBCIError_PortConfig)
            {
                newUserControl = _userControlBCIErrorPortConfig;
            }
            else if (state == DeviceTestingState.ReceivedBCIError_OpticalSensor)
            {
                newUserControl = _userControlBCIErrorOpticalSensor;
            }
            else if (state == DeviceTestingState.BCISignalCheckStartRequired)
            {
                newUserControl = _userControlBCISignalCheckStartRequired;
            }
            else if (state == DeviceTestingState.BCISignalCheckStartPrompt)
            {
                // Always reset checkbox (set to false) asking if user wants to do signal recheck when accessing user control
                _userControlBCISignalCheckStartPrompt.resetCheckbox();
                newUserControl = _userControlBCISignalCheckStartPrompt;
            }
            else if (state == DeviceTestingState.PromptFilterSettings)
            {
                newUserControl = _userControlPromptBCIFIlterSettings;
            }
            else if (state == DeviceTestingState.BCISignalCheck)
            {
                newUserControl = _userControlBCISignalCheck;
                this.TriggerBox = _userControlBCISignalCheck.TriggerBox;
            }
            else if (state == DeviceTestingState.OpticalSensorDetectError)
            {
                newUserControl = _userControlBCIErrorOpticalSensorDetect;
            }

            if (newUserControl != null)
            {
                // Every screen except last one - display in normal 1024 x 768 dimensions
                if (state != DeviceTestingState.BCISignalCheck)
                {
                    tableLayoutPanelContainer.Controls.Clear();
                    tableLayoutPanelContainer.Controls.Add(newUserControl, 0, 0);
                    tableLayoutPanelContainer.Refresh();
                }

                // Last screen - BCI signal check screen created with special layout
                // Basically remove everything in existing table layout panel and re-add to work with bigger format
                else if (state == DeviceTestingState.BCISignalCheck)
                {
                    bool displayReminderGelElectrodes = false;

                    //Previous selection was not a BCISignalCheck screen
                    if (prevDeviceTestingState != DeviceTestingState.BCISignalCheck)
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
                    else if (UserControlBCISignalCheck._currentBCISignalCheckMode == UserControlBCISignalCheck.BCISignalCheckMode.TEST_IMPEDANCE)
                    {
                        _userControlBCISignalCheck.changeSignalCheckMode(BCISignalCheckMode.TEST_IMPEDANCE);
                    }
                    else if (UserControlBCISignalCheck._currentBCISignalCheckMode == UserControlBCISignalCheck.BCISignalCheckMode.TEST_QUALITY)
                    {
                        _userControlBCISignalCheck.changeSignalCheckMode(BCISignalCheckMode.TEST_QUALITY);
                    }
                }

                // Start task that will launch data processing / plotting (optical sensor or signal check screens)
                if (BCIDeviceTester._Testing_useSensor)
                    TaskStartStopDataProcessing(state);
            }
        }

        /// <summary>
        /// Start / stop timer which plots data
        /// </summary>
        /// <param name="state"></param>
        private void startStopPlotDataTimer(bool startPlotDataTimer, DeviceTestingState state)
        {
            Log.Debug("startStopPlotDataTimer | startProcessDataTimer: " + startPlotDataTimer.ToString() +
                " | state: " + state.ToString());

            if (startPlotDataTimer)
            {
                startStopPlotDataTimer(false, state);
                try
                {
                    timerPlotData = new System.Windows.Forms.Timer(this.components);
                    timerPlotData.Enabled = true;
                    timerPlotData.Interval = _timer_plot_data_interval_ms; //// 50, 100, 200
                    timerPlotData.Stop();

                    if (state == DeviceTestingState.ReceivedBCIError_OpticalSensor)
                    {
                        timerPlotData.Tick += new System.EventHandler(this.PlotOpticalSensorData_Tick);
                    }

                    timerPlotData.Start();
                    Log.Debug("startStopPlotDataTimer | Started timerPlotData");
                }
                catch (Exception e)
                {
                    Log.Debug("startStopPlotDataTimer | Exception: " + e.ToString());
                }
            }
            else
            {
                try
                {
                    if (timerPlotData != null && timerPlotData.Enabled)
                    {
                        timerPlotData.Stop();
                        timerPlotData.Enabled = false;
                        timerPlotData.Dispose();
                        timerPlotData = null;
                    }
                }
                catch (Exception e)
                {
                    Log.Debug("startStopPlotDataTimer | Exception: " + e.ToString());
                }
            }
        }

        /// <summary>
        /// Start / stop timer which processes signal status
        /// </summary>
        /// <param name="state"></param>
        private void startStopProcessDataTimer(bool startProcessDataTimer, DeviceTestingState state)
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

                    if (state == DeviceTestingState.BCISignalCheck)
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
        /// Task in charge to start or stop data acquisition
        /// </summary>
        /// <returns></returns>
        /// Run only once per new set of state changes receieved
        public async Task TaskStartStopDataProcessing(DeviceTestingState state)
        {
            await Task.Delay(100);

            // Start data processing timer for Optical sensor error screen
            if (state == DeviceTestingState.ReceivedBCIError_OpticalSensor)
            {
                Invoke(new Action(() =>
                {
                    startStopPlotDataTimer(true, state);
                }));
            }

            // Stop data processing timer for Optical sensor error screen
            else
            {
                Invoke(new Action(() =>
                {
                    startStopPlotDataTimer(false, state);
                }));
            }

            // Start task which processes data for signal status checks
            if (state == DeviceTestingState.BCISignalCheck)
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
        /// Obtain, process, and plot BCI EEG data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessDataSignalCheck_Tick(object sender, EventArgs e)
        {
            // Check flag to stop all timers (checked during possible timer tick)
            // Check flag to stop this particular timer
            if (_stopTimers || BCIDeviceTester._endSignalCheckTimer)
            {
                Log.Debug("ProcessDataSignalCheck_Tick | _stopTimers | BCIDeviceTester._endSignalCheckTimer");
                startStopProcessDataTimer(false, DeviceTestingState.ExitBCITesting);
                return;
            }

            if (DAQ_OpenBCI.deviceInitialized)
            {
                //double[,] data = DAQ_OpenBCI.GetData();

                double[,] data = DAQ_OpenBCI.GetData2();

                if (data != null && data.Length > 0 && data.GetLength(1) > 0)
                {
                    double[,] dataCopy = (double[,])data.Clone();
                    double[,] DAQ_filteredData = DAQ_OpenBCI.daq_filter_data(dataCopy);

                    if (_userControlBCISignalCheck != null)
                        _userControlBCISignalCheck.ProcessDataSignalCheck(data, DAQ_filteredData);
                }
            }
        }

        /// <summary>
        /// Update optical sensor data plot
        /// Plot samples at every tick (this simulates data is received continuously)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlotOpticalSensorData_Tick(object sender, EventArgs e)
        {
            // Check flag to stop all timers (checked during possible timer tick)
            if (_stopTimers)
            {
                startStopPlotDataTimer(false, DeviceTestingState.ExitBCITesting);
                return;
            }

            if (DAQ_OpenBCI.deviceInitialized)
            {
                // REMOVE ONLY FOR TESTING
                DAQ_OpenBCI.InsertMarker(5.89f);

                double[,] data = DAQ_OpenBCI.GetData();

                if (data != null && data.Length > 0)
                {
                    int numSamples = data.GetLength(1);
                    double[] opticalSensorData = new double[numSamples];
                    for (int sampleIdx = 0; sampleIdx < numSamples; sampleIdx++)
                    {
                        opticalSensorData[sampleIdx] = 1 - data[DAQ_OpenBCI.indOpticalSensorChannel, sampleIdx]; // 1- data[DAQ_OpenBCI.indOpticalSensorChannel, sampleIdx]
                    }

                    Invoke(new Action(() =>
                    {
                        _userControlBCIErrorOpticalSensor.updateOpticalSensorDataPlot(opticalSensorData);
                    }));
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
                startStopPlotDataTimer(false, DeviceTestingState.ExitBCITesting);

                // Stop process data timer
                startStopProcessDataTimer(false, DeviceTestingState.ExitBCITesting);

                // Release resources
                if (EvtButtonNextClicked != null)
                    EvtButtonNextClicked = null;
                if (EvtButtonRetestClicked != null)
                    EvtButtonRetestClicked = null;
                if (EvtButtonCancelClicked != null)
                    EvtButtonCancelClicked = null;

                if (_userControlTestBCIConnections != null)
                    _userControlTestBCIConnections.Dispose();
                if (_userControlBCIErrorCytonBoard != null)
                    _userControlBCIErrorCytonBoard.Dispose();
                if (_userControlBCIErrorUsbDongle != null)
                    _userControlBCIErrorUsbDongle.Dispose();
                if (_userControlBCIErrorPortConfig != null)
                    _userControlBCIErrorPortConfig.Dispose();
                if (_userControlBCIErrorOpticalSensor != null)
                    _userControlBCIErrorOpticalSensor.Dispose();
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
                EvtButtonNextClicked(_mainFormDeviceTestingState);
            }
        }

        /// <summary>
        /// Handler for Cancel button click - call appropriate event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonExit_Click(object sender, EventArgs e)
        {
            if (EvtButtonCancelClicked != null)
            {
                EvtButtonCancelClicked(sender);
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
                EvtButtonRetestClicked(sender);
            }
        }

        /// <summary>
        /// Debugging / Developer Mode.
        /// Set Cancel button in each user control to red text and clicking it increments debug index which moves to the next user control
        /// </summary>
        private void modifyUserControlsForDebugMode()
        {
            Log.Debug("SensorForm | modifyUserControlsForDebugMode");

            _userControlTestBCIConnections.buttonExit.AutoSize = true;
            _userControlTestBCIConnections.buttonExit.Font = new System.Drawing.Font("Montserrat Medium", 13F);
            _userControlTestBCIConnections.buttonExit.ForeColor = Color.Red;
            _userControlTestBCIConnections.buttonExit.Text = "[Developer Mode]";

            _userControlBCIErrorCytonBoard.buttonExit.AutoSize = true;
            _userControlBCIErrorCytonBoard.buttonExit.Font = new System.Drawing.Font("Montserrat Medium", 13F);
            _userControlBCIErrorCytonBoard.buttonExit.ForeColor = Color.Red;
            _userControlBCIErrorCytonBoard.buttonExit.Text = "[Developer Mode]";

            _userControlBCIErrorUsbDongle.buttonExit.AutoSize = true;
            _userControlBCIErrorUsbDongle.buttonExit.Font = new System.Drawing.Font("Montserrat Medium", 13F);
            _userControlBCIErrorUsbDongle.buttonExit.ForeColor = Color.Red;
            _userControlBCIErrorUsbDongle.buttonExit.Text = "[Developer Mode]";

            _userControlBCIErrorPortConfig.buttonExit.AutoSize = true;
            _userControlBCIErrorPortConfig.buttonExit.Font = new System.Drawing.Font("Montserrat Medium", 13F);
            _userControlBCIErrorPortConfig.buttonExit.ForeColor = Color.Red;
            _userControlBCIErrorPortConfig.buttonExit.Text = "[Developer Mode]";

            _userControlBCIErrorOpticalSensor.buttonExit.AutoSize = true;
            _userControlBCIErrorOpticalSensor.buttonExit.Font = new System.Drawing.Font("Montserrat Medium", 13F);
            _userControlBCIErrorOpticalSensor.buttonExit.ForeColor = Color.Red;
            _userControlBCIErrorOpticalSensor.buttonExit.Text = "[Developer Mode]";

            _userControlBCISignalCheckStartRequired.buttonExit.AutoSize = true;
            _userControlBCISignalCheckStartRequired.buttonExit.Font = new System.Drawing.Font("Montserrat Medium", 11F);
            _userControlBCISignalCheckStartRequired.buttonExit.ForeColor = Color.Red;
            _userControlBCISignalCheckStartRequired.buttonExit.Text = "[Developer Mode]";

            _userControlBCISignalCheckStartPrompt.buttonExit.AutoSize = true;
            _userControlBCISignalCheckStartPrompt.buttonExit.Font = new System.Drawing.Font("Montserrat Medium", 11F);
            _userControlBCISignalCheckStartPrompt.buttonExit.ForeColor = Color.Red;
            _userControlBCISignalCheckStartPrompt.buttonExit.Text = "[Developer Mode]";

            _userControlPromptBCIFIlterSettings.buttonExit.AutoSize = true;
            _userControlPromptBCIFIlterSettings.buttonExit.Font = new System.Drawing.Font("Montserrat Medium", 11F);
            _userControlPromptBCIFIlterSettings.buttonExit.ForeColor = Color.Red;
            _userControlPromptBCIFIlterSettings.buttonExit.Text = "[Developer Mode]";

            _userControlBCISignalCheck.buttonExit.AutoSize = true;
            _userControlBCISignalCheck.buttonExit.Font = new System.Drawing.Font("Montserrat Medium", 11F);
            _userControlBCISignalCheck.buttonExit.ForeColor = Color.Red;
            _userControlBCISignalCheck.buttonExit.Text = "[Developer Mode]";
        }
    }

}