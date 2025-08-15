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

using ACAT.Core.Utility;
using ACAT.Extensions.BCI.Actuators.EEG.EEGDataAcquisition.FileManagement;
using ACAT.Extensions.BCI.Actuators.EEG.EEGSettings;
using ACAT.Extensions.BCI.Actuators.EEG.EEGUtils;
using ACAT.Extensions.BCI.Common.BCIControl;
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
    public class DAQ_OpenBCI : BaseDAQ
    {
        // ********** Params set here (not read from settings)
        // private readonly string[] otherChannelsPinsNameList = { "x", "D11", "D12", "D13", "D17", "D18", "x" };
        // private readonly int[] otherChannelsPinsIdxList = {12, 13, 14, 15, 16, 17, 18}; this is returnet when DeviceObj.get_other_channels();

        /// <summary>
        /// port where sensor is connected
        /// (this can be input, read from settings or automatically detected from this class)
        /// </summary>
        private string serialPort;

        /// <summary>
        /// Eys closes detection method (true if fix threshold, false if adaptive threshold)
        /// </summary>
        private bool eyesClosedDetectionUseFixThreshold;

        /// <summary>
        /// Threshold for eyes closed detection
        /// </summary>
        private double eyesClosedDetectionThreshold;

        /// <summary>
        /// Window duration used to calculate alpha values in eyes close detection
        /// </summary>
        private int eyesClosed_WindowDuration;

        /// <summary>
        /// Status of the board
        /// </summary>
        private BoardStatus status;

        /// <summary>
        /// Boolean, true if device initialized
        /// </summary>
        public bool deviceInitialized = false;

        /// <summary>
        /// Buffer to store data and calculate signal stauts
        /// </summary>
        private double[,] _bufferSignalStatus;

        /// <summary>
        /// Buffer to store data for eyes closed detection
        /// </summary>
        private double[,] _bufferEyesClosed;

        /// <summary>
        /// Index of the EEG channels in data returned from sensor
        /// This is directly via from brainflow
        /// </summary>
        public int[] indEegChannels;

        public enum DeviceStatus
        {
            DEVICE_STANDBY,
            DEVICE_ERROR,
            DEVICE_ACQUIRINGDATA,
        };

        public DeviceStatus deviceStatus;

        // BoardStatus enum is now inherited from BaseDAQ

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
        public DaisyBoardStatus _daisyBoardStatus = DaisyBoardStatus.UNKNOWN;

        // ExitCodes enum is now inherited from BaseDAQ

        /// <summary>
        /// Loads settings from the configuration file
        /// </summary>
        public override void LoadSettings()
        {
            SignalControl_WindowDurationForVrmsMeaseurment = BCIActuatorSettings.Settings.SignalControl_WindowDurationForVrmsMeaseurment;
            Log.Debug("DAQ settings loaded. Window duration for uVrmsMeasurement: " + SignalControl_WindowDurationForVrmsMeaseurment);

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

        // GetSessionDirectory() is inherited from BaseDAQ

        /// <summary>
        /// Get list of serial ports in the computer
        /// </summary>
        /// <returns></returns>
        public List<String> GetSerialPorts()
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
        public void SetPort(String port)
        {
            serialPort = port;
        }

        /// <summary>
        /// Sets eyes closed adaptive threshold
        /// </summary>
        /// <param name="threshold"></param>
        public void SetEyesClosedAdaptiveThreshold(float threshold)
        {
            if (!eyesClosedDetectionUseFixThreshold)
                eyesClosedDetectionThreshold = threshold;
        }

        /// <summary>
        /// Gets eyes closes threshold
        /// </summary>
        /// <returns></returns>
        public double GetEyesClosedThreshold()
        {
            return eyesClosedDetectionThreshold;
        }

        #endregion Get/set

        /// <summary>
        /// Checks if device is acquiring data
        /// </summary>
        /// <returns></returns>
        // IsAcquiring() is inherited from BaseDAQ

        /// <summary>
        /// Detects port where sensor is connected
        /// </summary>
        /// <returns></returns>
        public String DetectPort()
        {
            serialPort = null;

            foreach (String port in SerialPort.GetPortNames())
            {
                Log.Debug("Checking port " + port);

                serialPort = port;
                AddWarning(ExitCodes.IDLE, "  Time: " + DateTime.Now.ToString("h:mm:ss tt") + "  TESTING PORT    MESSAGE: Serial port " + serialPort);
                if (TestPort(port, out _))
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
        public bool CheckLatencyPort()
        {
            uint latency = ReadLatencyTimerValue(serialPort);
            return latency == 1;
        }

        // Function to detect specific UNABLE_TO_OPEN_PORT_ERROR error
        // Some redudancy with existing functions but don't want to mess with existing functionality - afraid I might break something
        public ExitCodes getUsbDongleConnected(String port = null)
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
                // Test port
                bool sensorConnected = TestPort(port, out bool portAlreadyInit);
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
        public override bool InitDevice(string deviceIdentifier)
        {
            string port = deviceIdentifier;
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

                    BrainFlowInputParams input_params = new();

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
                            Thread daisyCheckThread = new(() => cytonIsDaisyAttached(serialPort));
                            daisyCheckThread.Start();
                            daisyCheckThread.Join();
                        }

                        // DAQ_NumEEGChannels may have changed - run LoadSettings() at this point
                        LoadSettings();

                        input_params.serial_port = serialPort;

                        DeviceObj = new BoardShim(boardID, input_params);
                        DeviceObj.prepare_session();

                        Log.Debug("DAQ_OpenBCI - InitDevice | Board session prepared");

                        indEegChannels = BoardShim.get_eeg_channels(boardID);
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
                Log.Exception(e.Message);
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
        public override bool Start(string deviceIdentifier = "", bool saveData = false, string sessionID = "")
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
                    {
                        if (!String.IsNullOrWhiteSpace(deviceIdentifier))
                            initPortSuccess = InitDevice(deviceIdentifier);
                        else
                            initPortSuccess = InitDevice(serialPort);
                    }
                    else
                        initPortSuccess = true;

                    Log.Debug("Starting stream");
                    DeviceObj.start_stream();
                    Log.Debug("Stream started");

                    status = BoardStatus.BOARD_ACQUIRINGDATA;

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
                Log.Exception("Exception:" + e.Message + " Error code:" + sensorStatus);
                success = false;
            }
            Log.Debug("Device started: " + success);
            return success;
        }

        /// <summary>
        /// Stops sensor
        /// </summary>
        /// <returns></returns>
        public override bool Stop()
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
                Log.Exception("Exception:" + e.Message + " Error code: " + sensorStatus);
                return false;
            }
        }

        /// <summary>
        /// CLoses sensor and files
        /// </summary>
        /// <returns></returns>
        public override bool CloseDevice()
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
                Log.Exception("Exception:" + e.Message + " Error code: " + sensorStatus);
                return false;
            }
        }

        /// TODO - Fix function. Currently modifies rawData returned
        /// <summary>
        /// Get all available data from sensor
        /// </summary>
        /// <returns></returns>
        public override double[,] GetData(bool returnFilteredData = true)
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

                        // Trigger test disabled - optical sensor removed

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
                Log.Exception("Exception: " + e.Message);
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
        public double[,] GetData2()
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

                    // Trigger test disabled - optical sensor removed

                    // Write data to file
                    if (saveDataToFile && FileWriterObj != null && FileWriterObj.isFileOpened)
                        FileWriterObj.WriteRawDataToFile(rawData);

                    return rawData;
                }
            }

            return null;
        }

        // daq_filter_data() is inherited from BaseDAQ

        /// <summary>
        /// Inserts marker
        /// </summary>
        /// <param name="marker"></param>
        public override void InsertMarker(float marker)
        {
            if (status == BoardStatus.BOARD_ACQUIRINGDATA)
                DeviceObj.insert_marker(marker + 1);//1=off, 2=0n
        }

        /// <summary>
        /// Writes markers to file
        /// </summary>
        /// <param name="markerValues"></param>
        public override void WriteMarkerValues2File(List<int> markerValues)
        {
            if (saveDataToFile)
            {
                FileWriterObj ??= new FileWriter();

                FileWriterObj.WriteMarkerValueToFile(markerValues);
            }
        }

        /// <summary>
        /// Gets marker (removed with optical sensor)
        /// </summary>
        /// <returns>Always returns -1 since optical sensor is removed</returns>
        public int GetMarker()
        {
            // Optical sensor is removed, always return -1
            if (status == BoardStatus.BOARD_ACQUIRINGDATA)
            {
                _ = GetData(); //By doing get data, places last samples in _bufferSignalStatus
                deviceStatus = DeviceStatus.DEVICE_ACQUIRINGDATA;
            }
            else
            {
                deviceStatus = (status == BoardStatus.BOARD_STANDBY) ? DeviceStatus.DEVICE_STANDBY : DeviceStatus.DEVICE_ERROR;
            }
            return -1;
        }

        /// <summary>
        /// Gets status
        /// </summary>
        /// <returns></returns>
        public override SignalStatus GetStatus(out SignalStatus[] statusSignals)
        {
            SignalStatus statusAllSignals = SignalStatus.SIGNAL_ERROR;
            statusSignals = new SignalStatus[indEegChannels.Length];
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
                    }
                }
                catch (Exception e)
                {
                    Log.Exception("Exception: " + e.Message);
                }
            }
            return statusAllSignals;
        }

        /// <summary>
        /// Get overall Cyton board / USB dongle status
        /// </summary>
        /// <returns></returns>
        public SignalStatus GetStatus2_ReceivedData()
        {
            // Call GetMarker to set deviceStatus correctly
            GetMarker();
            
            return SignalStatus.SIGNAL_OK;
        }

        /// <summary>
        /// For internal use, adds filtered data to a buffer to assess signal status
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        private bool AppendDataToBuffer(double[,] data, double[,] inBuffer, int numSamplesInBuffer, out double[,] outBuffer)
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
                List<int> idxToKeep = new();
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
                Log.Exception(e.Message);
            }
            return result;
        }

        /// <summary>
        /// Starts trigger tests - Removed with optical sensor, always returns true if board is acquiring data
        /// </summary>
        public bool TriggerTestStart()
        {
            return status == BoardStatus.BOARD_ACQUIRINGDATA;
        }

        /// <summary>
        /// Stops trigger test - Removed with optical sensor
        /// </summary>
        /// <param name="numExpectedPulses"></param>
        /// <param name="numDetectedPulses"></param>
        /// <returns></returns>
        public ExitCodes TriggerTestStop(int numExpectedPulses, out int numDetectedPulses, out List<double> dutyCycleList, out double dutyCycleAvg)
        {
            numDetectedPulses = numExpectedPulses;
            dutyCycleList = new List<double> { 1.0 };
            dutyCycleAvg = 1.0;
            return ExitCodes.STATUS_OK;
        }

        /// <summary>
        /// Detects eyes closed
        /// </summary>
        /// <param name="alphaValues"></param>
        /// <param name="avgAlpha"></param>
        /// <param name="betaValues"></param>
        /// <param name="avgBeta"></param>
        /// <returns></returns>
        public bool DetectEyesClosed(out double[] alphaValues, out double avgAlpha, out double[] betaValues, out double avgBeta)
        {
            alphaValues = new double[indEegChannels.Length];
            betaValues = new double[indEegChannels.Length];
            avgAlpha = 0;
            avgBeta = 0;

            bool eyesClosedDetected = false;
            if (status == BoardStatus.BOARD_ACQUIRINGDATA)
            {
                _ = GetData();
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
                            Log.Exception(e.Message);
                        }
                    }
                    avgAlpha /= indEegChannels.Length;
                    avgBeta /= indEegChannels.Length;

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
        private bool TestPort(String port, out bool portAlreadyOpen)
        {
            portAlreadyOpen = false;
            try
            {
                Log.Debug("Testing port " + port);
                BrainFlowInputParams input_params = new()
                {
                    serial_port = port
                };

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
                Log.Exception("Exception: " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Read the latency of the port
        /// </summary>
        /// <param name="comPort"></param>
        /// <returns></returns>
        private UInt32 ReadLatencyTimerValue(String comPort)
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
                                    if (UInt32.TryParse(Convert.ToString(obj), out uint latencyValue))
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
                Log.Exception("Exception: " + e.Message);
            }

            return 0;
        }

        /// <summary>
        /// Converts message to exit code
        /// </summary>
        /// <param name="message"></param>
        /// <param name="defaultErrorCode"></param>
        /// <returns></returns>
        private ExitCodes getErrorCode(string message, ExitCodes defaultErrorCode)
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
        private void CreateFiles(String sessionID)
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

                // Pass -1 as index for optical sensor channel since it's been removed
                FileWriterObj.WriteHeaders(BoardShim.get_sampling_rate(boardID), indEegChannels, -1);
            }
        }

        /// <summary>
        /// Creates a new sesion (new files) without having to stop and start the device
        /// </summary>
        /// <param name="sessionID"></param>
        public override bool StartSession(string sessionID, bool forceSavingData)
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

                        // Pass -1 as index for optical sensor channel since it's been removed
                        FileWriterObj.WriteHeaders(BoardShim.get_sampling_rate(boardID), indEegChannels, -1);

                        result = true;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Exception("Exception " + e.Message);
            }
            return result;
        }

        /// <summary>
        /// Ends session
        /// </summary>
        /// <returns></returns>
        public override bool EndSession()
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
                    GetData(); // Empty buffer
                }
                Log.Debug("Session closed");
                result = true;
            }
            catch (Exception e)
            {
                Log.Exception("Exception " + e.Message);
            }

            return result;
        }

        private readonly Queue<Dictionary<ExitCodes, string>> warnings = new();
        private readonly int limit = 10;

        /// <summary>
        /// Add a warning to the queue
        /// </summary>
        /// <param name="info">string warnings</param>
        public void AddWarning(ExitCodes code, String info)
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
                _ = warnings.Dequeue();
                warnings.Enqueue(data);
            }
        }

        /// <summary>
        /// Gets the available warnings in the queue
        /// </summary>
        /// <returns>Warnings</returns>
        public Dictionary<ExitCodes, string> getWarning()
        {
            Dictionary<ExitCodes, string> info = null;
            try
            {
                if (warnings.Count() > 0)
                    info = warnings.Dequeue();
            }
            catch (Exception e)
            {
                Log.Exception(e.Message);
                return info;
            }
            return info;
        }

        /// <summary>
        /// Send lower level config command to BoardShim device
        /// </summary>
        /// <param name="cmd"></param>
        public void Config_Board(string cmd)
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
        public void Start_Streaming()
        {
            //DeviceObj.start_stream();
            Config_Board("b");
        }

        /// <summary>
        /// Helper function to stop Cyton board streaming
        /// </summary>
        public void Stop_Streaming()
        {
            //DeviceObj.stop_stream();
            Config_Board("s");
        }

        /// <summary>
        /// Helper function to reset Cyton board to default state
        /// </summary>
        public void Reset_Board()
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
        private bool cytonIsDaisyAttached(String comPort)
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
                    Log.Debug("cytonIsDaisyAttached | Received a valid response from cyton board | DAQ_NumEEGChannels: " +
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
                Log.Exception(e.Message);
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