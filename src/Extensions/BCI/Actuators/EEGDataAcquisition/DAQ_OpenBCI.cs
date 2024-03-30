////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// DAQ_OpenBCI.cs
//
// Interfaces with the OpenBCI sensor
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Extensions.BCI.Actuators.EEG.EEGSettings;
using ACAT.Extensions.BCI.Actuators.EEG.EEGUtils;
using ACAT.Extensions.BCI.Common.BCIControl;
using ACAT.Lib.Core.Utility;
using Accord.Math;
using brainflow;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace ACAT.Extensions.BCI.Actuators.EEG.EEGDataAcquisition
{
    public class DAQ_OpenBCI
    {
        /// <summary>
        /// Settings
        /// </summary>
        public static String SettingsFileName = "BCIActuatorSettings.xml";

        // ********** Params set here (not read from settings)
        // private static readonly string[] otherChannelsPinsNameList = { "x", "D11", "D12", "D13", "D17", "D18", "x" };
        // private static readonly int[] otherChannelsPinsIdxList = {12, 13, 14, 15, 16, 17, 18}; this is returnet when DeviceObj.get_other_channels();
        private static readonly string boardLogFileName = "boardLog";

        private static readonly bool boardLoggerEnabled = false;

        // ********* Params read from settings
        private static int boardID; //0=Cyton, -1 = synthetic,  1=ganglion, 2=daysi

        /// <summary>
        /// port where sensor is connected
        /// (this can be input, read from settings or automatically detected from this class)
        /// </summary>
        private static string serialPort;

        /// <summary>
        /// Port (in Cyton board) where teh optical sensor is connected
        /// Options:  1,2,3,4,5 corresponging to [D11, D12, D13, D17, D18]
        /// </summary>
        public static int opticalSensorPinIdx = 3;

        /// <summary>
        /// Sample rate
        /// </summary>
        public static int sampleRate;

        /// <summary>
        /// Bolean, true if data shoudl be saved in file
        /// </summary>
        private static bool saveDataToFile;

        /// <summary>
        /// Index for the notch filter
        /// </summary>
        private static int notchFilterIdx;

        /// <summary>
        /// Index for the frontend filter
        /// </summary>
        private static int frontendFilterIdx;

        /// <summary>
        /// Eys closes detection method (true if fix threshold, false if adaptive threshold)
        /// </summary>
        private static bool eyesClosedDetectionUseFixThreshold;

        /// <summary>
        /// Threshold for eyes closed detection
        /// </summary>
        private static double eyesClosedDetectionThreshold;

        /// <summary>
        /// Window duration used to calculate alpha values in eyes close detection
        /// </summary>
        private static int eyesClosed_WindowDuration;

        /// <summary>
        /// Duration used to calculate VRMS and detect signal status (red/yellow/green)
        /// </summary>
        private static int SignalControl_WindowDurationForVrmsMeaseurment;

        /// <summary>
        /// MInimum duty cycle required to pass trigger test. Set to 0 for no duty cycle requirement
        /// </summary>
        private static float SignalControl_MinDutyCycleToPassTriggerTest;

        // ********** Objects for this class

        /// <summary>
        /// Object to interact with cyton board via Brainflow library
        /// </summary>
        private static BoardShim DeviceObj;

        /// <summary>
        /// Object to handle writting to files
        /// </summary>
        private static FileWriter FileWriterObj;

        /// <summary>
        /// Notch filter
        /// </summary>
        private static Filter NotchFilter;

        /// <summary>
        /// Frontend (bandpass) filter
        /// </summary>
        private static Filter FrontendFilter;

        /// <summary>
        /// Status of the board
        /// </summary>
        private static BoardStatus status;

        /// <summary>
        /// Boolean, true if device initialized
        /// </summary>
        public static bool deviceInitialized = false;

        /// <summary>
        /// Buffer to store data and calculate signal stauts
        /// </summary>
        private static double[,] _bufferSignalStatus;

        /// <summary>
        /// Buffer to store data for eyes closed detection
        /// </summary>
        private static double[,] _bufferEyesClosed;

        /// <summary>
        /// Buffer for triggertest
        /// </summary>
        private static List<double> _bufferTriggerTest;

        /// <summary>
        /// Flag, true when trigger test is in progress
        /// </summary>
        private static bool triggerTestInProgressFlag;

        /// <summary>
        /// Index of the EEG channels in data returned from sensor
        /// This is directly via from brainflow
        /// </summary>
        public static int[] indEegChannels;

        /// <summary>
        /// Index of the optical sensor channel
        /// This is directly read from brainflow
        /// </summary>
        public static int indOpticalSensorChannel;

        public enum DeviceStatus
        {
            DEVICE_STANDBY,
            DEVICE_ERROR,
            DEVICE_ACQUIRINGDATA,
        };

        public static DeviceStatus deviceStatus;

        public enum BoardStatus
        {
            BOARD_STANDBY,
            BOARD_OPEN,
            BOARD_CLOSED,
            BOARD_ACQUIRINGDATA,
        };

        public static ExitCodes sensorStatus;

        public enum DaisyBoardStatus
        {
            UNKNOWN, // Connection test has not been executed
            NOT_CONNECTED, // Connection test has been executed - Daisy board not connected - Default 8 channels
            CONNECTED // Connection test has been executed - Daisy board connected - 16 channels
        };

        /// <summary>
        /// Flag denoting whether or not daisy board connection has been tested
        /// Testing function initializes it's own serial port so cannot run test after BoardShim device has been initialized
        /// </summary>
        public static DaisyBoardStatus _daisyBoardStatus = DaisyBoardStatus.UNKNOWN;

        public enum ExitCodes
        {
            STATUS_OK,
            PORT_ALREADY_OPEN_ERROR,
            UNABLE_TO_OPEN_PORT_ERROR,
            SET_PORT_ERROR,
            BOARD_WRITE_ERROR,
            INCOMMING_MSG_ERROR,
            INITIAL_MSG_ERROR,
            BOARD_NOT_READY_ERROR,
            STREAM_ALREADY_RUN_ERROR,
            INVALID_bufferSignalStatus_SIZE_ERROR,
            STREAM_THREAD_ERROR,
            STREAM_THREAD_IS_NOT_RUNNING,
            EMPTY_bufferSignalStatus_ERROR,
            INVALID_ARGUMENTS_ERROR,
            UNSUPPORTED_BOARD_ERROR,
            BOARD_NOT_CREATED_ERROR,
            ANOTHER_BOARD_IS_CREATED_ERROR,
            GENERAL_ERROR,
            SYNC_TIMEOUT_ERROR,
            JSON_NOT_FOUND_ERROR,
            NO_SUCH_DATA_IN_JSON_ERROR,
            CLASSIFIER_IS_NOT_PREPARED_ERROR,
            ANOTHER_CLASSIFIER_IS_PREPARED_ERROR,
            UNSUPPORTED_CLASSIFIER_AND_METRIC_COMBINATION_ERROR,
            UNABLE_TO_CLOSE,
            IDLE,
            PHOTOSENSOR_NO_PULSE_DETECTED_ERROR,
            PHOTOSENSOR_DIFFERENT_NUM_PULSES_DETECTED_ERROR,
            PHOTOSENSOR_UNKNOWN_ERROR,
            PHOTOSENSOR_DUTYCICLE_HIGH_ERROR,
            PHOTOSENSOR_DUTYCICLE_LOW_ERROR,
            PHOTOSENSOR_STATUS_OK
        };

        /// <summary>
        ///  Default contstructor
        /// </summary>
        public static void LoadSettings()
        {
            SignalControl_WindowDurationForVrmsMeaseurment = BCIActuatorSettings.Settings.SignalControl_WindowDurationForVrmsMeaseurment;
            SignalControl_MinDutyCycleToPassTriggerTest = BCIActuatorSettings.Settings.TriggerTest_MinDutyCycleToPassTriggerTest;
            Log.Debug("DAQ settings loaded. Min duty cycle to pass trigger test" + SignalControl_MinDutyCycleToPassTriggerTest + " Window duration for uVrmsMeasurement: " + SignalControl_WindowDurationForVrmsMeaseurment);

            switch (BCIActuatorSettings.Settings.DAQ_NumEEGChannels)
            {
                case 8:
                    BCISettingsFixed.DAQ_SensorId = 0;
                    BCISettingsFixed.DataParser_IdxTriggerSignal_Hw = 16;
                    BCISettingsFixed.DataParser_IdxTriggerSignal_Sw = 24;
                    BCISettingsFixed.DimReduct_DownsampleRate = 2;
                    break;

                case 16:
                    BCISettingsFixed.DAQ_SensorId = 2;
                    BCISettingsFixed.DataParser_IdxTriggerSignal_Hw = 24;
                    BCISettingsFixed.DataParser_IdxTriggerSignal_Sw = 32;
                    BCISettingsFixed.DimReduct_DownsampleRate = 1;
                    break;

                default:
                    BCIActuatorSettings.Settings.DAQ_NumEEGChannels = 8;
                    BCISettingsFixed.DAQ_SensorId = 0;
                    BCISettingsFixed.DataParser_IdxTriggerSignal_Hw = 16;
                    BCISettingsFixed.DataParser_IdxTriggerSignal_Sw = 24;
                    BCISettingsFixed.DimReduct_DownsampleRate = 2;
                    Log.Debug("Num Channels settings is incorrect. Sensor set to default: 8 channels");
                    break;
            }

            BCIActuatorSettings.Save();
            Log.Debug("Sensor set to " + BCIActuatorSettings.Settings.DAQ_NumEEGChannels + " channels. SensorID: " + BCISettingsFixed.DAQ_SensorId + " , Downsample rate: " + BCISettingsFixed.DimReduct_DownsampleRate +
                      " , Idx hw trigger signal: " + BCISettingsFixed.DataParser_IdxTriggerSignal_Hw + " , Idx sw trigger signal: " + BCISettingsFixed.DataParser_IdxTriggerSignal_Sw);

            boardID = BCISettingsFixed.DAQ_SensorId;
            saveDataToFile = BCIActuatorSettings.Settings.DAQ_SaveToFileFlag;
            frontendFilterIdx = BCIActuatorSettings.Settings.DAQ_FrontendFilterIdx;
            notchFilterIdx = BCIActuatorSettings.Settings.DAQ_NotchFilterIdx;
            Log.Debug(" Frontend filter: " + frontendFilterIdx + " Notch filter: " + notchFilterIdx);

            eyesClosedDetectionUseFixThreshold = BCIActuatorSettings.Settings.EyesClosed_UseFixThreshold;
            if (eyesClosedDetectionUseFixThreshold)
                eyesClosedDetectionThreshold = BCIActuatorSettings.Settings.EyesClosed_FixThreshold_Threshold;
            else
                eyesClosedDetectionThreshold = BCIActuatorSettings.Settings.EyesClosed_AdaptiveThreshold;
            eyesClosed_WindowDuration = BCIActuatorSettings.Settings.EyesClosed_WindowDuration;
            Log.Debug("Eyes closed detection. Use Fix Threshold" + eyesClosedDetectionUseFixThreshold + " Threshold: " + eyesClosedDetectionThreshold + " Window duration: " + eyesClosed_WindowDuration);
        }

        #region Get/set

        /// <summary>
        /// Get session directory
        /// </summary>
        /// <returns></returns>
        public static String GetSessionDirectory()
        {
            if (FileWriterObj != null)
                return FileWriterObj.sessionDirectory;
            else
                return null;
        }

        /// <summary>
        /// Get list of serial ports in the computer
        /// </summary>
        /// <returns></returns>
        public static List<String> GetSerialPorts()
        {
            string[] serialPorts = SerialPort.GetPortNames();
            if (serialPorts == null)
                return new List<string>();
            else
                return serialPorts.ToList();
        }

        /// <summary>
        /// Sets port where device is connected
        /// </summary>
        /// <param name="port"></param>
        public static void SetPort(String port)
        {
            serialPort = port;
        }

        /// <summary>
        /// Sets eyes closed adaptive threshold
        /// </summary>
        /// <param name="threshold"></param>
        public static void SetEyesClosedAdaptiveThreshold(float threshold)
        {
            if (!eyesClosedDetectionUseFixThreshold)
                eyesClosedDetectionThreshold = threshold;
        }

        /// <summary>
        /// Gets eyes closes threshold
        /// </summary>
        /// <returns></returns>
        public static double GetEyesClosedThreshold()
        {
            return eyesClosedDetectionThreshold;
        }

        #endregion Get/set

        /// <summary>
        /// Checks if device is acquiring data
        /// </summary>
        /// <returns></returns>
        public static bool IsAcquiring()
        {
            if (status == BoardStatus.BOARD_ACQUIRINGDATA)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Detects port where sensor is connected
        /// </summary>
        /// <returns></returns>
        public static String DetectPort()
        {
            serialPort = null;

            Log.Debug("Optical sensor port: " + OpticalSensorComm.PortName);

            foreach (String port in SerialPort.GetPortNames())
            {
                Log.Debug("Checking port " + port);
                if (String.Compare(port, OpticalSensorComm.PortName, true) == 0)
                {
                    Log.Debug("Skipping " + OpticalSensorComm.PortName);
                    continue;
                }

                serialPort = port;
                AddWarning(ExitCodes.IDLE, "  Time: " + DateTime.Now.ToString("h:mm:ss tt") + "  TESTING PORT    MESSAGE: Serial port " + serialPort);
                bool portAlreadyInit;
                if (TestPort(port, out portAlreadyInit))
                {
                    Log.Debug("Detected cytonboard port " + port);
                    return serialPort;
                }
            }

            return serialPort;
        }

        /// <summary>
        /// Checks the latency of the port. It returns true if latency==1ms, false otherwise
        /// </summary>
        /// <returns></returns>
        public static bool CheckLatencyPort()
        {
            uint latency = ReadLatencyTimerValue(serialPort);
            return latency == 1;
        }

        // Function to detect specific UNABLE_TO_OPEN_PORT_ERROR error
        // Some redudancy with existing functions but don't want to mess with existing functionality - afraid I might break something
        public static ExitCodes getUsbDongleConnected(String port = null)
        {
            try
            {
                LoadSettings();

                BoardShim.disable_board_logger();

                // Find port
                if (port == null || port == "")
                {
                    // Load port from settings
                    //Settings.SettingsFilePath = UserManager.GetFullPath(SettingsFileName);
                    //var settings = Settings.Load();
                    port = BCIActuatorSettings.Settings.DAQ_ComPort;
                }
                bool portAlreadyInit;
                // Test port
                bool sensorConnected = TestPort(port, out portAlreadyInit);
                if (sensorConnected)
                    return ExitCodes.IDLE;
                if (!sensorConnected)
                {
                    port = DetectPort();
                    sensorConnected = TestPort(port, out portAlreadyInit);
                }
                if (!sensorConnected && !portAlreadyInit)
                    return ExitCodes.UNABLE_TO_OPEN_PORT_ERROR; // Only return UNABLE_TO_OPEN_PORT_ERROR at this particular case in time - when trying to open COM port and that fails
            }
            catch (Exception e)
            {
                sensorStatus = getErrorCode(e.Message, ExitCodes.UNABLE_TO_OPEN_PORT_ERROR);
                AddWarning(sensorStatus, "  Time: " + DateTime.Now.ToString("h:mm:ss tt") + "  WARNING             MESSAGE: Error Code: " + sensorStatus);
                // return ExitCodes.GENERAL_ERROR;
            }

            // Return IDLE if able to connect to sensor or in any other case
            return ExitCodes.IDLE;
        }

        /// <summary>
        /// Initializes sensor
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool InitDevice(string port)
        {
            try
            {
                if (status == BoardStatus.BOARD_OPEN)
                {
                    Log.Debug("Board was open, closing device");
                    CloseDevice();
                }

                if (status == BoardStatus.BOARD_ACQUIRINGDATA)
                {
                    Log.Debug("Board already acquiring data, returning");
                    return true;
                }
                else
                {
                    LoadSettings();

                    // Enable /disable boardlogging
                    if (boardLoggerEnabled)
                    {
                        Log.Debug("BoardLoggerEnabled: " + boardLoggerEnabled + " Enabling brainflow logging");
                        BoardShim.enable_dev_board_logger();
                        BoardShim.set_log_file(boardLogFileName);
                    }
                    else
                    {
                        Log.Debug("BoardLoggerEnabled: " + boardLoggerEnabled + " Disabling brainflow logging");
                        BoardShim.disable_board_logger();
                    }

                    // Find port
                    if (port == null || port == "")
                    {
                        port = BCIActuatorSettings.Settings.DAQ_ComPort;
                    }

                    // Test port
                    Log.Debug("Testing port: " + port);
                    bool sensorConnected = TestPort(port, out _);
                    if (!sensorConnected)
                    {
                        Log.Debug("Sensor not connected to port " + port + ". Starting port detection");
                        port = DetectPort();
                        Log.Debug("Port " + port + " detected. Testing port");
                        sensorConnected = TestPort(port, out _);
                        Log.Debug("Port " + port + " tested. Result: " + sensorConnected);
                    }

                    BrainFlowInputParams input_params = new BrainFlowInputParams();

                    if (sensorConnected)
                    {
                        Log.Debug("Sensor connected to port " + port);

                        // Save port
                        serialPort = port;

                        // Save port to settings
                        BCIActuatorSettings.Settings.DAQ_ComPort = serialPort;
                        BCIActuatorSettings.Save();
                        Log.Debug("Port: " + serialPort + " saved to settings");

                        // Check if Cyton Daisy board attached
                        // Makes separate COM connection (BrainFlow / BoardShim does not allow parsing of responses from lower level commands
                        // Need to do before BoardShim object initialized
                        // Saves result to _daisyBoardStatus and settings (DAQ_NumEEGChannels)
                        if (_daisyBoardStatus == DaisyBoardStatus.UNKNOWN)
                        {
                            Thread daisyCheckThread = new Thread(() => cytonIsDaisyAttached(serialPort));
                            daisyCheckThread.Start();
                            daisyCheckThread.Join();
                        }

                        // DAQ_NumEEGChannels may have changed - run LoadSettings() at this point
                        LoadSettings();

                        input_params.serial_port = serialPort;

                        DeviceObj = new BoardShim(boardID, input_params);
                        DeviceObj.prepare_session();

                        //string stringBoardMode = "/" + boardMode;
                        // Config board to digital mode (mode 3) for photo sensor
                        Log.Debug("DAQ_OpenBCI - InitDevice | Sending board mode commands");
                        DeviceObj.config_board("/3");
                        DeviceObj.config_board("/2");
                        DeviceObj.config_board("/3");

                        indEegChannels = BoardShim.get_eeg_channels(boardID);
                        int[] indOtherChannels = BoardShim.get_other_channels(boardID); //indOtherChannels = 12...18
                        indOpticalSensorChannel = indOtherChannels[opticalSensorPinIdx];// = D13=15=indOtherChannels[3];
                        sampleRate = BoardShim.get_sampling_rate(boardID);
                        BCISettingsFixed.DAQ_SampleRate = sampleRate;

                        BCIActuatorSettings.Save();

                        FrontendFilter = new Filter(frontendFilterIdx, Filter.FilterTypes.Frontend);
                        NotchFilter = new Filter(notchFilterIdx, Filter.FilterTypes.Notch);
                        Log.Debug("Creating Frontend filter: " + frontendFilterIdx + " | Notch filter: " + notchFilterIdx);

                        status = BoardStatus.BOARD_OPEN;
                        deviceInitialized = true;
                        AddWarning(ExitCodes.IDLE, "  Time: " + DateTime.Now.ToString("h:mm:ss tt") + "  STATUS                 MESSAGE: Device initialized at serial port: " + serialPort);
                        Log.Debug("Board initialized. Status: " + status.ToString());
                        return true;
                    }
                    else
                        return false;
                }
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
                sensorStatus = getErrorCode(e.Message, ExitCodes.BOARD_NOT_READY_ERROR);
                AddWarning(sensorStatus, "  Time: " + DateTime.Now.ToString("h:mm:ss tt") + "  WARNING             MESSAGE: Error Code: " + sensorStatus);
                return false;
            }
        }

        /// <summary>
        /// Starts sensor
        /// </summary>
        /// <param name="port"></param>
        /// <param name="saveData"></param>
        /// <param name="sessionID"></param>
        /// <returns></returns>
        public static bool Start(String port = null, bool saveData = false, String sessionID = "")
        {
            bool success = false;
            try
            {
                if (status != BoardStatus.BOARD_ACQUIRINGDATA)
                {
                    // Init device
                    bool initPortSuccess;
                    Log.Debug("Initiating device");
                    if (status != BoardStatus.BOARD_OPEN)
                        initPortSuccess = InitDevice(port);
                    else
                        initPortSuccess = true;

                    Log.Debug("Starting stream");
                    DeviceObj.start_stream();
                    Log.Debug("Stream started");

                    status = BoardStatus.BOARD_ACQUIRINGDATA;
                    triggerTestInProgressFlag = false;

                    if (saveDataToFile)
                    {
                        Log.Debug("Creating files for session " + sessionID);
                        CreateFiles(sessionID);
                    }

                    if (status == BoardStatus.BOARD_ACQUIRINGDATA && initPortSuccess)
                        success = true;
                }
                else if (status == BoardStatus.BOARD_ACQUIRINGDATA)
                    success = true;
            }
            catch (Exception e) //needs to handle error better
            {
                sensorStatus = getErrorCode(e.Message, ExitCodes.BOARD_NOT_CREATED_ERROR);
                AddWarning(sensorStatus, "  Time: " + DateTime.Now.ToString("h:mm:ss tt") + "  WARNING             MESSAGE: Error Code: " + sensorStatus);
                Log.Debug("Exception:" + e.Message + " Error code:" + sensorStatus);
                success = false;
            }
            Log.Debug("Device started: " + success);
            return success;
        }

        /// <summary>
        /// Stops sensor
        /// </summary>
        /// <returns></returns>
        public static bool Stop()
        {
            try
            {
                if (status == BoardStatus.BOARD_ACQUIRINGDATA)
                {
                    Log.Debug("Board acquiring data. Stopping device");
                    GetData();
                    DeviceObj.stop_stream();
                    DeviceObj.release_session();
                    Log.Debug("Device stopped");
                }

                if (saveDataToFile && FileWriterObj != null && FileWriterObj.isFileOpened)
                {
                    Log.Debug("Closing files");
                    FileWriterObj.CloseFiles();
                    FileWriterObj = null;
                    Log.Debug("Files closed");
                }

                status = BoardStatus.BOARD_STANDBY;
                return true;
            }
            catch (Exception e)
            {
                sensorStatus = getErrorCode(e.Message, ExitCodes.SYNC_TIMEOUT_ERROR);
                AddWarning(sensorStatus, "  Time: " + DateTime.Now.ToString("h:mm:ss tt") + "  WARNING               MESSAGE: Error Code: " + sensorStatus);
                Log.Debug("Exception:" + e.Message + " Error code: " + sensorStatus);
                return false;
            }
        }

        /// <summary>
        /// CLoses sensor and files
        /// </summary>
        /// <returns></returns>
        public static bool CloseDevice()
        {
            try
            {
                if (status == BoardStatus.BOARD_CLOSED)
                {
                    Log.Debug("Board already closed");
                    return true;
                }

                if (status == BoardStatus.BOARD_ACQUIRINGDATA)
                    DeviceObj.stop_stream();

                DeviceObj.release_session();
                status = BoardStatus.BOARD_CLOSED;
                Log.Debug("Device closed");
                return true;
            }
            catch (Exception e)
            {
                sensorStatus = getErrorCode(e.Message, ExitCodes.UNABLE_TO_CLOSE);
                AddWarning(sensorStatus, "  Time: " + DateTime.Now.ToString("h:mm:ss tt") + "  WARNING             MESSAGE: Error Code: " + sensorStatus);
                Log.Debug("Exception:" + e.Message + " Error code: " + sensorStatus);
                return false;
            }
        }

        /// TODO - Fix function. Currently modifies rawData returned
        /// <summary>
        /// Get all available data from sensor
        /// </summary>
        /// <returns></returns>
        public static double[,] GetData(bool returnFilteredData = true)
        {
            double[,] rawData = null;
            double[,] filteredData = null;
            try
            {
                if (status == BoardStatus.BOARD_ACQUIRINGDATA)
                {
                    // Get data
                    rawData = DeviceObj.get_board_data();

                    if (rawData != null && rawData.Length > 0)
                    {
                        // Filter data
                        var filteredData_notch = NotchFilter.FilterData(rawData, indEegChannels);
                        filteredData = FrontendFilter.FilterData(filteredData_notch, indEegChannels);

                        // Append data to buffer for status signal and eyes closed detection
                        AppendDataToBuffer(filteredData, _bufferSignalStatus, SignalControl_WindowDurationForVrmsMeaseurment, out _bufferSignalStatus);
                        AppendDataToBuffer(filteredData, _bufferEyesClosed, eyesClosed_WindowDuration, out _bufferEyesClosed);

                        // Append data to buffer for triggerTest
                        if (triggerTestInProgressFlag)
                        {
                            if (_bufferTriggerTest == null)
                                _bufferTriggerTest = new List<double>();
                            _bufferTriggerTest.AddRange(rawData.GetRow(indOpticalSensorChannel));
                        }

                        // Write data to file
                        if (saveDataToFile && FileWriterObj != null && FileWriterObj.isFileOpened)
                        {
                            FileWriterObj.WriteFilteredDataToFile(filteredData);
                            if (BCIActuatorSettings.Settings.DAQ_SaveAditionalFileWithRawData)
                                FileWriterObj.WriteRawDataToFile(rawData);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Debug("Exception: " + e.Message);
            }

            if (returnFilteredData)
                return filteredData;
            else
                return rawData;
        }

        /// <summary>
        /// Get all available data from sensor
        /// </summary>
        /// <returns></returns>
        public static double[,] GetData2()
        {
            if (status == BoardStatus.BOARD_ACQUIRINGDATA)
            {
                // Get data
                double[,] rawData = DeviceObj.get_board_data();
                int numChannels = rawData.GetLength(0);
                int numSamples = rawData.GetLength(1);

                if (numChannels > 0 && numSamples > 0)
                {
                    /*// Filter data
                    var filteredData_notch = NotchFilter.FilterData(rawData, indEegChannels);
                    var filteredData = FrontendFilter.FilterData(filteredData_notch, indEegChannels);

                    // Append data to buffer for status signal and eyes closed detection
                    AppendDataToBuffer(filteredData, _bufferSignalStatus, SignalControl_WindowDurationForVrmsMeaseurment, out _bufferSignalStatus);
                    AppendDataToBuffer(filteredData, _bufferEyesClosed, eyesClosed_WindowDuration, out _bufferEyesClosed);*/

                    // Append data to buffer for triggerTest
                    if (triggerTestInProgressFlag)
                    {
                        if (_bufferTriggerTest == null)
                            _bufferTriggerTest = new List<double>();
                        _bufferTriggerTest.AddRange(rawData.GetRow(indOpticalSensorChannel));
                    }

                    // Write data to file
                    if (saveDataToFile && FileWriterObj != null && FileWriterObj.isFileOpened)
                        FileWriterObj.WriteRawDataToFile(rawData);

                    return rawData;
                }
            }

            return null;
        }

        public static double[,] daq_filter_data(double[,] unfilteredData)
        {
            var filteredData_notch = NotchFilter.FilterData(unfilteredData, indEegChannels);
            double[,] filteredData = FrontendFilter.FilterData(filteredData_notch, indEegChannels);
            return filteredData;
        }

        /// <summary>
        /// Inserts marker
        /// </summary>
        /// <param name="marker"></param>
        public static void InsertMarker(float marker)
        {
            if (status == BoardStatus.BOARD_ACQUIRINGDATA)
                DeviceObj.insert_marker(marker + 1);//1=off, 2=0n
        }

        /// <summary>
        /// Writes markers to file
        /// </summary>
        /// <param name="markerValues"></param>
        public static void WriteMarkerValues2File(List<int> markerValues)
        {
            if (saveDataToFile)
            {
                if (FileWriterObj == null)
                    FileWriterObj = new FileWriter();

                FileWriterObj.WriteMarkerValueToFile(markerValues);
            }
        }

        /// <summary>
        /// Gets marker (from optical sensor)
        /// </summary>
        /// <returns></returns>
        public static int GetMarker()
        {
            int optSensorValue = -1;
            if (status == BoardStatus.BOARD_ACQUIRINGDATA)
            {
                double[,] allData = GetData(); //By doing get data, places last samples in _bufferSignalStatus
                if ((_bufferSignalStatus != null && _bufferSignalStatus.Length > 0))
                {
                    int numSamples = _bufferSignalStatus.GetLength(1);

                    optSensorValue = (int)_bufferSignalStatus[indOpticalSensorChannel, numSamples - 1];
                    optSensorValue = 1 - optSensorValue; //Flip value so black = 0, white = 1
                    deviceStatus = DeviceStatus.DEVICE_ACQUIRINGDATA;
                }
            }
            if (-1 == optSensorValue)
            {
                switch (status)
                {
                    case BoardStatus.BOARD_ACQUIRINGDATA:
                        deviceStatus = DeviceStatus.DEVICE_ERROR;
                        break;

                    case BoardStatus.BOARD_STANDBY:
                        deviceStatus = DeviceStatus.DEVICE_STANDBY;
                        break;
                }
            }
            return optSensorValue;
        }

        /// <summary>
        /// Gets status
        /// </summary>
        /// <returns></returns>v
        public static SignalStatus GetStatus(out SignalStatus[] statusSignals, out SignalStatus statusOpticalSensor)
        {
            SignalStatus statusAllSignals = SignalStatus.SIGNAL_ERROR;
            statusSignals = new SignalStatus[indEegChannels.Length];
            statusOpticalSensor = SignalStatus.SIGNAL_ERROR;
            for (int channelIdx = 0; channelIdx < indEegChannels.Length; channelIdx++)
                statusSignals[channelIdx] = SignalStatus.SIGNAL_ERROR;

            if (status == BoardStatus.BOARD_ACQUIRINGDATA)
            {
                try
                {
                    double[,] allData = GetData();

                    if (_bufferSignalStatus != null && _bufferSignalStatus.Length > 0)
                    {
                        /* TODO: Use railing to calculate channel status
                       // Calculate status for each channel
                       for (int channelIdx = 0; channelIdx < indEegChannels.Length; channelIdx++)
                       {
                           // Get channel data
                           var chData = _bufferSignalStatus.GetRow(indEegChannels[channelIdx]);

                           // Calculate uVrms
                           var chStd = chData.StandardDeviation(); //std = uVRMS

                           // Calculate railing

                           // To calculate overall status
                           if (statusSignals[channelIdx] == SignalStatus.SIGNAL_OK)
                               numChannelsOk++;

                           //String logTxt = "Channel " + channelIdx + " status: " + statusSignals[channelIdx];
                           //Log.Debug(logTxt)
                        }

                        // Calculate overall status
                        statusAllSignals = SignalStatus.SIGNAL_KO;
                        if (numChannelsOk >= 6)
                            statusAllSignals = SignalStatus.SIGNAL_ACCEPTABLE;
                        if (numChannelsOk == 8)
                            statusAllSignals = SignalStatus.SIGNAL_OK;
                        */

                        // Set overall status and status signals as OK (temprarily until railing is implemented)
                        statusAllSignals = SignalStatus.SIGNAL_OK;
                        for (int channelIdx = 0; channelIdx < indEegChannels.Length; channelIdx++)
                            statusSignals[channelIdx] = SignalStatus.SIGNAL_OK;

                        // Sanity check of optical sensor, will return SIGNAL_OK if 0 and 1 are detected in the buffer
                        // (this assumes the optical sensor is placed on top of the triggerbox and the triggerbox has changed from black to white)
                        statusOpticalSensor = SignalStatus.SIGNAL_KO;
                        double[] opticalSensorSignal = _bufferSignalStatus.GetRow(indOpticalSensorChannel);
                        bool containsZeroes = opticalSensorSignal.Any(o => o == 0);
                        bool containsOnes = opticalSensorSignal.Any(o => o == 1);
                        if (containsZeroes && containsOnes)
                            statusOpticalSensor = SignalStatus.SIGNAL_OK;
                    }
                }
                catch (Exception e)
                {
                    Log.Debug("Exception: " + e.Message);
                }
            }
            return statusAllSignals;
        }

        /// <summary>
        /// Maximum amount of time for optical sensor data to remain the same before throwing error
        /// </summary>
        // public const double THRESHOLD_ERROR_NO_OPTICAL_SENSOR_DATA_CHANGE_SEC = 10.0;
        public const double THRESHOLD_ERROR_NO_OPTICAL_SENSOR_DATA_CHANGE_SEC = 5.0;

        /// <summary>
        /// Tracks optical sensor data for comparison across time
        /// </summary>
        // public static string pastStringOpticalSensorData = "NONE"; // Reset to "NONE" everytme BCI device testing process repeated
        public static double[] pastOpticalSensorData = new double[1]; // Reset to = new double[1] everytme BCI device testing process repeated

        /// <summary>
        /// Tracks time optical sensor data last changed
        /// </summary>
        public static DateTime dateTimeLastChangeOpticalSensorData = DateTime.MinValue; // Reset to DateTime.MinValue everytme BCI device testing process repeated

        /// <summary>
        /// Get overall Cyton board / USB dongle status by comparing values in the optical sensor buffer
        /// </summary>
        /// <returns></returns>
        public static SignalStatus GetStatus2_ReceivedData()
        {
            GetMarker(); // Just call GetMarker to set deviceStatus accordingly

            SignalStatus retStatus = SignalStatus.SIGNAL_OK;
            double timeSinceOpticalSensorDataChange = -999;
            // string stringOpticalSensorData = "NONE";

            double[] opticalSensorSignal = _bufferSignalStatus.GetRow(indOpticalSensorChannel);
            bool containsZeroes = opticalSensorSignal.Any(o => o == 0);
            bool containsOnes = opticalSensorSignal.Any(o => o == 1);
            if (containsZeroes && containsOnes)
            {
                // stringOpticalSensorData = string.Join(", ", opticalSensorSignal);
                DateTime dateTimeNow = DateTime.UtcNow;
                // if (pastStringOpticalSensorData != "NONE")
                if (pastOpticalSensorData.Length > 1)
                {
                    bool isEqual = Enumerable.SequenceEqual(pastOpticalSensorData, opticalSensorSignal);

                    // if (stringOpticalSensorData == pastStringOpticalSensorData)
                    if (isEqual)
                    {
                        if (dateTimeLastChangeOpticalSensorData != DateTime.MinValue)
                        {
                            timeSinceOpticalSensorDataChange = (dateTimeNow - dateTimeLastChangeOpticalSensorData).TotalSeconds;
                            if (timeSinceOpticalSensorDataChange != -999 && timeSinceOpticalSensorDataChange > THRESHOLD_ERROR_NO_OPTICAL_SENSOR_DATA_CHANGE_SEC)
                            {
                                retStatus = SignalStatus.SIGNAL_KO;

                                // DAQ_OpenBCI.DeviceStatus.DEVICE_ERROR;
                                if (deviceStatus == DeviceStatus.DEVICE_ERROR)
                                {
                                    DAQ_OpenBCI.Stop(); // close board connection before attempting new one if got error
                                }
                            }
                        }
                    }
                    else
                    {
                        dateTimeLastChangeOpticalSensorData = dateTimeNow;
                    }
                }

                // pastStringOpticalSensorData = stringOpticalSensorData;
                pastOpticalSensorData = opticalSensorSignal;
            }

            /*// Debugging
            string str_opticalSensorSignal = string.Join(", ", opticalSensorSignal);
            string str_pastOpticalSensorSignal = string.Join(", ", pastOpticalSensorData);
            Log.Debug("\n\nGetStatus2_ReceivedData | timeSinceOpticalSensorDataChange: " + timeSinceOpticalSensorDataChange.ToString());
            Log.Debug("status: " + status.ToString() + " | sensorStatus: " + sensorStatus.ToString() + " | sensorStatus: "+ sensorStatus.ToString());
            Log.Debug("\npastOpticalSensorData: " + str_pastOpticalSensorSignal);
            Log.Debug("\nopticalSensorSignal: " + str_pastOpticalSensorSignal);
            Log.Debug("\nretStatus: " + retStatus.ToString());*/

            return retStatus;
        }

        /// <summary>
        /// For internal use, adds filtered data to a buffer to assess signal status
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        private static bool AppendDataToBuffer(double[,] data, double[,] inBuffer, int numSamplesInBuffer, out double[,] outBuffer)
        {
            bool result = false;
            outBuffer = null;
            try
            {
                // Append to buffer
                if (inBuffer != null)
                    inBuffer = Matrix.Concatenate(inBuffer, data);
                else
                    inBuffer = data;

                // Keep only last N samples in buffer (N samples are used to calculate status)
                int numSamplesCurrBuffer = inBuffer.GetLength(1);
                int numSamplesToKeep = (numSamplesInBuffer * sampleRate) / 1000;
                List<int> idxToKeep = new List<int>();
                for (int i = numSamplesCurrBuffer - numSamplesToKeep; i < numSamplesCurrBuffer; i++)
                {
                    if (i >= 0)
                        idxToKeep.Add(i);
                }
                outBuffer = inBuffer.GetColumns(idxToKeep.ToArray());

                result = true;
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
            }
            return result;
        }

        /// <summary>
        /// Starts trigger tests
        /// </summary>
        public static bool TriggerTestStart()
        {
            if (status == BoardStatus.BOARD_ACQUIRINGDATA)
            {
                Log.Debug("Starting trigger test");
                GetData();
                _bufferTriggerTest = new List<double>();
                triggerTestInProgressFlag = true;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Stops trigger test and outputs result and number of detected pulses
        /// </summary>
        /// <param name="numExpectedPulses"></param>
        /// <param name="numDetectedPulses"></param>
        /// <returns></returns>
        public static ExitCodes TriggerTestStop(int numExpectedPulses, out int numDetectedPulses, out List<double> dutyCycleList, out double dutyCycleAvg)
        {
            Log.Debug("Stopping trigger test");
            numDetectedPulses = 0;
            ExitCodes statusCode = ExitCodes.PHOTOSENSOR_UNKNOWN_ERROR;
            dutyCycleList = new List<double>();
            dutyCycleAvg = 0;

            if (status == BoardStatus.BOARD_ACQUIRINGDATA)
            {
                GetData();
                triggerTestInProgressFlag = false;

                if (_bufferTriggerTest != null && _bufferTriggerTest.Count > 0)
                {
                    try
                    {
                        double[] triggerSignal = _bufferTriggerTest.ToArray();
                        int numSamples = triggerSignal.Count();
                        // detect number of pulses
                        bool pulseOnDetected = false;
                        bool pulseOffDetected = false;
                        int numSamplesOn = 0;
                        int numSamplesOff = 0;
                        List<float> dutycycleList = new List<float>();

                        // Flip signal (triggertest signal is reversed)
                        for (int sampleIdx = 0; sampleIdx < numSamples; sampleIdx++)
                        {
                            if (triggerSignal[sampleIdx] == 1)
                                triggerSignal[sampleIdx] = 0;
                            else
                                triggerSignal[sampleIdx] = 1;
                        }
                        for (int sampleIdx = 0; sampleIdx < numSamples - 1; sampleIdx++)
                        {
                            if (triggerSignal[sampleIdx] == 1) // On detected
                            {
                                numSamplesOn++;
                                pulseOnDetected = true;
                            }

                            if (pulseOnDetected && triggerSignal[sampleIdx] == 0) //Off detected
                            {
                                numSamplesOff++;
                                pulseOffDetected = true;
                            }

                            if ((triggerSignal[sampleIdx] == 0 && triggerSignal[sampleIdx + 1] == 1 && pulseOnDetected && pulseOffDetected)
                                || (pulseOnDetected && triggerSignal[sampleIdx] == 0 && sampleIdx == numSamples - 2))
                            {
                                float dutyCicle = (float)numSamplesOn / (float)(numSamplesOff + numSamplesOn);
                                Log.Debug("Duty cycle: " + dutyCicle);
                                dutycycleList.Add(dutyCicle);
                                numSamplesOn = 0;
                                numSamplesOff = 0;
                                pulseOnDetected = false;
                                pulseOffDetected = false;
                            }
                        }

                        numDetectedPulses = dutycycleList.Count;

                        // Verify duty cycle by using average
                        bool dutycycleError = false;
                        if (dutycycleList.Count > 0)
                        {
                            if (dutycycleList.Count > 1)
                                dutyCycleAvg = dutycycleList.Take(dutycycleList.Count - 1).Average(); // take N-1 since last pulse off is not accurate
                            else
                                dutyCycleAvg = dutycycleList[0]; // Only one pulse available, set value s average

                            Log.Debug("Average duty cicle: " + dutyCycleAvg);
                            dutycycleError = dutyCycleAvg < SignalControl_MinDutyCycleToPassTriggerTest;
                        }

                        // Verify duty cycle for each individual pulse
                        // Note: using average instead, uncomment to use individual pulses
                        //bool dutycycleError = false;
                        //for (int pulseIdx = 0; pulseIdx < duticycleList.Count - 1; pulseIdx++)
                        //{
                        //    if (duticycleList[pulseIdx] < SignalControl_MinDutyCycleToPassTriggerTest)
                        //    {
                        //        dutycycleError = true;
                        //        Log.Debug("Duty cicle error. Pulse: " + pulseIdx + " Duty cycle: " + duticycleList[pulseIdx]);
                        //    }
                        //}

                        // Verify number of detected pulses and dutycicle error
                        if (numDetectedPulses == numExpectedPulses && !dutycycleError)// && statusCode == ExitCodes.PHOTOSENSOR_UNKNOWN_ERROR)
                            statusCode = ExitCodes.PHOTOSENSOR_STATUS_OK;
                        else if (numDetectedPulses != numExpectedPulses)
                            statusCode = ExitCodes.PHOTOSENSOR_DIFFERENT_NUM_PULSES_DETECTED_ERROR;
                        else if (numDetectedPulses == 0)
                            statusCode = ExitCodes.PHOTOSENSOR_NO_PULSE_DETECTED_ERROR;
                        else if (dutycycleError)
                            statusCode = ExitCodes.PHOTOSENSOR_DUTYCICLE_LOW_ERROR;

                        Log.Debug("Trigger test result: " + statusCode.ToString() + " Num pulses expected: " + numExpectedPulses + ", Num pulses detected: " + numDetectedPulses + ", Duty cicle average: " + dutyCycleAvg);

                        _bufferTriggerTest = new List<double>();
                    }
                    catch (Exception e)
                    {
                        statusCode = ExitCodes.PHOTOSENSOR_UNKNOWN_ERROR;
                        Log.Debug("Excepcion: " + e.Message + " Sending status:" + statusCode.ToString());
                    }
                }
            }
            return statusCode;
        }

        /// <summary>
        /// Detects eyes closed
        /// </summary>
        /// <param name="alphaValues"></param>
        /// <param name="avgAlpha"></param>
        /// <param name="betaValues"></param>
        /// <param name="avgBeta"></param>
        /// <returns></returns>
        public static bool DetectEyesClosed(out double[] alphaValues, out double avgAlpha, out double[] betaValues, out double avgBeta)
        {
            alphaValues = new double[indEegChannels.Length];
            betaValues = new double[indEegChannels.Length];
            avgAlpha = 0;
            avgBeta = 0;

            bool eyesClosedDetected = false;
            if (status == BoardStatus.BOARD_ACQUIRINGDATA)
            {
                double[,] allData = GetData();
                if (_bufferEyesClosed != null && _bufferEyesClosed.Length > 0)
                {
                    int nfft = DataFilter.get_nearest_power_of_two(sampleRate);

                    for (int channelIdx = 0; channelIdx < indEegChannels.Length; channelIdx++)
                    {
                        try
                        {
                            // Get channel data
                            var chData = _bufferEyesClosed.GetRow(indEegChannels[channelIdx]);
                            double[] detrend = DataFilter.detrend(chData, (int)DetrendOperations.LINEAR);
                            Tuple<double[], double[]> psd = DataFilter.get_psd_welch(detrend, nfft, nfft / 2, sampleRate, (int)WindowOperations.HANNING);
                            alphaValues[channelIdx] = DataFilter.get_band_power(psd, 7.0, 13.0);
                            betaValues[channelIdx] = DataFilter.get_band_power(psd, 14.0, 30.0);

                            avgAlpha += alphaValues[channelIdx];
                            avgBeta += betaValues[channelIdx];
                        }
                        catch (Exception e)
                        {
                            Log.Debug(e.Message);
                        }
                    }
                    avgAlpha = avgAlpha / indEegChannels.Length;
                    avgBeta = avgBeta / indEegChannels.Length;

                    if (avgAlpha > eyesClosedDetectionThreshold)
                    {
                        eyesClosedDetected = true;
                        _bufferEyesClosed.Clear();
                    }
                }
            }

            return eyesClosedDetected;
        }

        #region Utils

        /// <summary>
        /// Tests if sensor is connected to the port
        /// </summary>
        /// <param name="port"></param> port to test
        /// <returns></returns>
        private static bool TestPort(String port, out bool portAlreadyOpen)
        {
            portAlreadyOpen = false;
            try
            {
                Log.Debug("Testing port " + port);
                BrainFlowInputParams input_params = new BrainFlowInputParams();
                input_params.serial_port = port;

                DeviceObj = new BoardShim(boardID, input_params);
                DeviceObj.prepare_session();
                DeviceObj.release_session();
                Log.Debug("Sensor detected to port" + port);
                return true;
            }
            catch (Exception e)
            {
                sensorStatus = getErrorCode(e.Message, ExitCodes.SET_PORT_ERROR);
                if (sensorStatus == ExitCodes.ANOTHER_BOARD_IS_CREATED_ERROR)
                    portAlreadyOpen = true;
                AddWarning(sensorStatus, "  Time: " + DateTime.Now.ToString("h:mm:ss tt") + "  WARNING             MESSAGE: Error Code: " + sensorStatus);
                Log.Debug("Exception: " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Read the latency of the port
        /// </summary>
        /// <param name="comPort"></param>
        /// <returns></returns>
        private static UInt32 ReadLatencyTimerValue(String comPort)
        {
            try
            {
                var rootKey = "System\\CurrentControlSet\\Enum\\FTDIBUS";
                RegistryKey key = Registry.LocalMachine.OpenSubKey(rootKey);
                if (key == null)
                {
                    return 0;
                }

                foreach (var subKeyName in key.GetSubKeyNames())
                {
                    if (subKeyName.StartsWith("VID"))
                    {
                        var currKey = rootKey + "\\" + subKeyName + "\\0000\\Device Parameters";
                        RegistryKey deviceKey = Registry.LocalMachine.OpenSubKey(currKey);

                        if (deviceKey != null)
                        {
                            var obj = deviceKey.GetValue("PortName");

                            if ((obj != null) && String.Compare(Convert.ToString(obj), comPort, true) == 0)
                            {
                                obj = deviceKey.GetValue("LatencyTimer");
                                if (obj != null)
                                {
                                    uint latencyValue = 0;
                                    if (UInt32.TryParse(Convert.ToString(obj), out latencyValue))
                                    {
                                        return latencyValue;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Debug("Exception: " + e.Message);
            }

            return 0;
        }

        /// <summary>
        /// Converts message to exit code
        /// </summary>
        /// <param name="message"></param>
        /// <param name="defaultErrorCode"></param>
        /// <returns></returns>
        private static ExitCodes getErrorCode(string message, ExitCodes defaultErrorCode)
        {
            foreach (ExitCodes code in Enum.GetValues(typeof(ExitCodes)))
            {
                if (message.Contains(code.ToString()))
                    return code;
            }
            return defaultErrorCode;
        }

        /// <summary>
        /// Creates files where data is stored
        /// </summary>
        /// <param name="sessionID"></param>
        private static void CreateFiles(String sessionID)
        {
            if (saveDataToFile)
            {
                if (FileWriterObj == null)
                {
                    Log.Debug("Creating files for session: " + sessionID);

                    if (sessionID == "")
                        FileWriterObj = new FileWriter();
                    else
                        FileWriterObj = new FileWriter(sessionID);
                }

                FileWriterObj.WriteHeaders(BoardShim.get_sampling_rate(boardID), indEegChannels, indOpticalSensorChannel);
            }
        }

        /// <summary>
        /// Creates a new sesion (new files) without having to stop and start the device
        /// </summary>
        /// <param name="sessionID"></param>
        public static bool StartSession(String sessionID, bool forceSavingData)
        {
            bool result = false;
            try
            {
                if (forceSavingData)
                    saveDataToFile = forceSavingData; // THis is useful for calibration where data should always be saved
                else
                    saveDataToFile = BCIActuatorSettings.Settings.DAQ_SaveToFileFlag;

                if (status == BoardStatus.BOARD_ACQUIRINGDATA)
                {
                    // Empty buffer
                    GetData();

                    if (saveDataToFile)
                    {
                        Log.Debug("Creating files for session: " + sessionID);

                        // Creates new file
                        if (sessionID == "")
                            FileWriterObj = new FileWriter();
                        else
                            FileWriterObj = new FileWriter(sessionID);

                        FileWriterObj.WriteHeaders(BoardShim.get_sampling_rate(boardID), indEegChannels, indOpticalSensorChannel);

                        result = true;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Debug("Exception " + e.Message);
            }
            return result;
        }

        /// <summary>
        /// Ends session
        /// </summary>
        /// <returns></returns>
        public static bool EndSession()
        {
            bool result = false;
            try
            {
                if (saveDataToFile && FileWriterObj != null && FileWriterObj.isFileOpened)
                {
                    FileWriterObj.CloseFiles();
                    FileWriterObj = null;
                }
                if (status == BoardStatus.BOARD_ACQUIRINGDATA)
                {
                    triggerTestInProgressFlag = false;
                    GetData(); // Empty buffer
                }
                Log.Debug("Session closed");
                result = true;
            }
            catch (Exception e)
            {
                Log.Debug("Exception " + e.Message);
            }

            return result;
        }

        private static Queue<Dictionary<ExitCodes, string>> warnings = new Queue<Dictionary<ExitCodes, string>>();
        private static readonly int limit = 10;

        /// <summary>
        /// Add a warning to the queue
        /// </summary>
        /// <param name="info">string warnings</param>
        public static void AddWarning(ExitCodes code, String info)
        {
            var data = new Dictionary<ExitCodes, string>
            {
                { code, info }
            };
            if (warnings.Count < limit)
            {
                warnings.Enqueue(data);
            }
            else
            {
                var release = warnings.Dequeue();
                warnings.Enqueue(data);
            }
        }

        /// <summary>
        /// Gets the available warnings in the queue
        /// </summary>
        /// <returns>Warnings</returns>
        public static Dictionary<ExitCodes, string> getWarning()
        {
            Dictionary<ExitCodes, string> info = null;
            try
            {
                if (warnings.Count() > 0)
                    info = warnings.Dequeue();
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
                return info;
            }
            return info;
        }

        /// <summary>
        /// Send lower level config command to BoardShim device
        /// </summary>
        /// <param name="cmd"></param>
        public static void Config_Board(string cmd)
        {
            try
            {
                Log.Debug("Config board. Command" + cmd);
                DeviceObj.config_board(cmd);
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        /// <summary>
        /// Helper function to start Cyton board streaming
        /// </summary>
        public static void Start_Streaming()
        {
            //DeviceObj.start_stream();
            Config_Board("b");
        }

        /// <summary>
        /// Helper function to stop Cyton board streaming
        /// </summary>
        public static void Stop_Streaming()
        {
            //DeviceObj.stop_stream();
            Config_Board("s");
        }

        /// <summary>
        /// Helper function to reset Cyton board to default state
        /// </summary>
        public static void Reset_Board()
        {
            Config_Board("d");
        }

        /// <summary>
        /// Create serial connection to Cyton board, write command to select maximum 16 channels
        /// Based on response, get if 8 channels available or 16 channels available (daisy board attached)
        /// Command "C" = Use 16 channels. returns - If daisy already attached, returns 16$$$.
        /// If the daisy is not currently attached and is not able to be attached, then no daisy to attach!8$$$ is returned.
        /// If the daisy is not currently attached and is able to be attached, then daisy attached16$$$ is returned.
        /// Note: On reset, the OpenBCI Cyton board will 'sniff' for the Daisy Module, and if it is present,
        /// it will default to 16 channel capability
        /// </summary>
        /// <param name="comPort">COM port to connect to and send command to check for daisy connection</param>
        private static bool cytonIsDaisyAttached(String comPort)
        {
            bool receivedResponse = false;
            bool daisyBoardAttached = false;
            SerialPort serialPort = null;
            try
            {
                ////String port = DAQ_OpenBCI.DetectPort();
                String port = comPort;

                serialPort = new SerialPort()
                {
                    PortName = port,
                    BaudRate = 115200,
                    ReadTimeout = 500,
                    WriteTimeout = 500,
                    NewLine = "$$$"
                };

                Log.Debug(String.Format("cytonIsDaisyAttached | Opening serial port with port name: {0}, baud rate: {1}", serialPort.PortName, serialPort.BaudRate));
                serialPort.Open();
                Thread.Sleep(100);

                // If the port is open, do something
                if (serialPort.IsOpen)
                {
                    Log.Debug("cytonIsDaisyAttached | serialPort is open");
                    int max_tries = 3;
                    while (!receivedResponse && max_tries > 0)
                    {

                        ////Log.Debug("Sending command C then waiting a bit until reading response");
                        serialPort.WriteLine("C");
                        Thread.Sleep(1000);

                        String response = serialPort.ReadLine().Trim();
                        Log.Debug(String.Format("cytonIsDaisyAttached | response: {0}", response));

                        if (response == "8" || response == "16")
                        {
                            if (response == "8")
                                daisyBoardAttached = false;
                            else if (response == "16")
                                daisyBoardAttached = true;

                            receivedResponse = true;
                            break;
                        }
                        else if (response == "no daisy to attach!8" || response == "daisy attached16")
                        {
                            if (response == "no daisy to attach!8")
                                daisyBoardAttached = false;
                            else if (response == "daisy attached16")
                                daisyBoardAttached = true;

                            receivedResponse = true;
                            break;
                        }

                        max_tries -= 1;
                        Thread.Sleep(500);
                    }

                    Log.Debug("cytonIsDaisyAttached | End read line / check loop. Sending reset board command");
                    serialPort.WriteLine("d");
                    Thread.Sleep(3500);

                }

                // Save result to config file and _daisyBoardStatus if got response
                if (receivedResponse == true)
                {
                    if (daisyBoardAttached)
                    {
                        BCIActuatorSettings.Settings.DAQ_NumEEGChannels = 16;
                        _daisyBoardStatus = DaisyBoardStatus.CONNECTED;
                    }
                    else
                    {
                        BCIActuatorSettings.Settings.DAQ_NumEEGChannels = 8;
                        _daisyBoardStatus = DaisyBoardStatus.NOT_CONNECTED;
                    }
                    Log.Debug("cytonIsDaisyAttached | Received a valid response from cyton board | DAQ_NumEEGChannels: "+
                        BCIActuatorSettings.Settings.DAQ_NumEEGChannels.ToString());

                    BCIActuatorSettings.Save();
                }
                else
                {
                    Log.Debug("cytonIsDaisyAttached | Did not receive a valid response from cyton board. Setting DAQ_NumEEGChannels to 8");
                    BCIActuatorSettings.Settings.DAQ_NumEEGChannels = 16;
                }
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
            }
            finally
            {
                Log.Debug("cytonIsDaisyAttached | closing serialPort from finally");
                serialPort.Close();
                Log.Debug("cytonIsDaisyAttached | serialPort closed from finally");
            }



            if (serialPort != null && serialPort.IsOpen)
            {
                Log.Debug("cytonIsDaisyAttached | serialPort not yet closed. calling close() again");
                serialPort.Close();
            }

            Log.Debug(String.Format("cytonDaisyAttached() done | " +
                "receivedResponse: {0}, " +
                "daisyBoardAttached: {1}, " +
                "BCIActuatorSettings.Settings.DAQ_NumEEGChannels: {2}",
                receivedResponse.ToString(),
                daisyBoardAttached.ToString(), 
                BCIActuatorSettings.Settings.DAQ_NumEEGChannels.ToString()));
            
            return daisyBoardAttached;
        }
    }

    #endregion Utils
}