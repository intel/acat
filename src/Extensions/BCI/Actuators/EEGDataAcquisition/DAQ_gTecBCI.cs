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
using System.Configuration;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gtec.Unicorn;

namespace ACAT.Extensions.BCI.Actuators.EEG.EEGDataAcquisition
{
    public class DAQ_gTecBCI
    {
        /// <summary>
        /// Settings
        /// </summary>
        public String SettingsFileName = "BCIActuatorSettings.xml";

        // ********** Params set here (not read from settings)
        // private readonly string[] otherChannelsPinsNameList = { "x", "D11", "D12", "D13", "D17", "D18", "x" };
        // private readonly int[] otherChannelsPinsIdxList = {12, 13, 14, 15, 16, 17, 18}; this is returnet when DeviceObj.get_other_channels();
        private readonly string boardLogFileName = "boardLog";

        private readonly bool boardLoggerEnabled = false;

        // ********* Params read from settings
        private int boardID = (int)BoardIds.UNICORN_BOARD;

        /// <summary>
        /// Sample rate
        /// </summary>
        public int sampleRate;

        /// <summary>
        /// Bolean, true if data shoudl be saved in file
        /// </summary>
        private bool saveDataToFile;

        /// <summary>
        /// Index for the notch filter
        /// </summary>
        private int notchFilterIdx;

        /// <summary>
        /// Index for the frontend filter
        /// </summary>
        private int frontendFilterIdx;

        
        /// <summary>
        /// Duration used to calculate VRMS and detect signal status (red/yellow/green)
        /// </summary>
        private int SignalControl_WindowDurationForVrmsMeaseurment;

        /// <summary>
        /// MInimum duty cycle required to pass trigger test. Set to 0 for no duty cycle requirement
        /// </summary>
        private float SignalControl_MinDutyCycleToPassTriggerTest;

        // ********** Objects for this class

        /// <summary>
        /// Object to interact with cyton board via Brainflow library
        /// </summary>
        private BoardShim DeviceObj;

        /// <summary>
        /// Object to handle writting to files
        /// </summary>
        private FileWriter FileWriterObj;

        /// <summary>
        /// Notch filter
        /// </summary>
        private Filter NotchFilter;

        /// <summary>
        /// Frontend (bandpass) filter
        /// </summary>
        private Filter FrontendFilter;

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
        /// Flag, true when trigger test is in progress
        /// </summary>
        private bool triggerTestInProgressFlag;

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

        public enum BoardStatus
        {
            BOARD_STANDBY,
            BOARD_OPEN,
            BOARD_CLOSED,
            BOARD_ACQUIRINGDATA,
        };

        public ExitCodes sensorStatus;

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
        };

        /// <summary>
        ///  Default contstructor
        /// </summary>
        public void LoadSettings()
        {
            SignalControl_WindowDurationForVrmsMeaseurment = BCIActuatorSettings.Settings.SignalControl_WindowDurationForVrmsMeaseurment;
            SignalControl_MinDutyCycleToPassTriggerTest = BCIActuatorSettings.Settings.TriggerTest_MinDutyCycleToPassTriggerTest;
            Log.Debug("DAQ settings loaded. Min duty cycle to pass trigger test" + SignalControl_MinDutyCycleToPassTriggerTest + " Window duration for uVrmsMeasurement: " + SignalControl_WindowDurationForVrmsMeaseurment);

            switch (BCIActuatorSettings.Settings.DAQ_NumEEGChannels)
            {
                case 8:
                    BCISettingsFixed.DataParser_IdxTriggerSignal_Hw = 16;
                    BCISettingsFixed.DataParser_IdxTriggerSignal_Sw = 24;
                    BCISettingsFixed.DimReduct_DownsampleRate = 2;
                    break;

                case 16:
                    BCISettingsFixed.DataParser_IdxTriggerSignal_Hw = 24;
                    BCISettingsFixed.DataParser_IdxTriggerSignal_Sw = 32;
                    BCISettingsFixed.DimReduct_DownsampleRate = 1;
                    break;

                default:
                    BCIActuatorSettings.Settings.DAQ_NumEEGChannels = 8;
                    BCISettingsFixed.DataParser_IdxTriggerSignal_Hw = 16;
                    BCISettingsFixed.DataParser_IdxTriggerSignal_Sw = 24;
                    BCISettingsFixed.DimReduct_DownsampleRate = 2;
                    Log.Debug("Num Channels settings is incorrect. Sensor set to default: 8 channels");
                    break;
            }

            BCIActuatorSettings.Save();
            Log.Debug("Sensor set to " + BCIActuatorSettings.Settings.DAQ_NumEEGChannels + " channels. SensorID: " + BCISettingsFixed.DAQ_SensorId + " , Downsample rate: " + BCISettingsFixed.DimReduct_DownsampleRate +
                      " , Idx hw trigger signal: " + BCISettingsFixed.DataParser_IdxTriggerSignal_Hw + " , Idx sw trigger signal: " + BCISettingsFixed.DataParser_IdxTriggerSignal_Sw);

            saveDataToFile = BCIActuatorSettings.Settings.DAQ_SaveToFileFlag;
            frontendFilterIdx = BCIActuatorSettings.Settings.DAQ_FrontendFilterIdx;
            notchFilterIdx = BCIActuatorSettings.Settings.DAQ_NotchFilterIdx;
            Log.Debug(" Frontend filter: " + frontendFilterIdx + " Notch filter: " + notchFilterIdx);
        }

        #region Get/set

        /// <summary>
        /// Get session directory
        /// </summary>
        /// <returns></returns>
        public String GetSessionDirectory()
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
        public List<String> GetSerialPorts()
        {
            string[] serialPorts = SerialPort.GetPortNames();
            if (serialPorts == null)
                return new List<string>();
            else
                return serialPorts.ToList();
        }

       
        #endregion Get/set

        /// <summary>
        /// Checks if device is acquiring data
        /// </summary>
        /// <returns></returns>
        public bool IsAcquiring()
        {
            if (status == BoardStatus.BOARD_ACQUIRINGDATA)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Initializes sensor
        /// </summary>
        /// <param name="serial_number"></param>
        /// <returns></returns>
        public bool InitDevice(string serial_number = "")
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

                    // Test port
                    Log.Debug("Testing port: " + serial_number);
                    bool sensorConnected = TestPort(serial_number, out _);

                    // TODO: Celal check if we can do anything with GTEC
                    //if (!sensorConnected)
                    //{
                    //    Log.Debug("Sensor not connected to port " + serial_number + ". Starting port detection");
                    //    serial_number = DetectPort();
                    //    Log.Debug("Port " + serial_number + " detected. Testing port");
                    //    sensorConnected = TestPort(serial_number, out _);
                    //    Log.Debug("Port " + serial_number + " tested. Result: " + sensorConnected);
                    //}

                    BrainFlowInputParams input_params = new BrainFlowInputParams();

                    if (sensorConnected)
                    {
                        Log.Debug("Sensor connected to port " + serial_number);

                        // Save port to settings
                        BCIActuatorSettings.Settings.DAQ_ComPort = serial_number;
                        BCIActuatorSettings.Save();
                        Log.Debug("Port: " + serial_number + " saved to settings");

                        
                        // DAQ_NumEEGChannels may have changed - run LoadSettings() at this point
                        LoadSettings();

                        input_params.serial_number = serial_number;

                        DeviceObj = new BoardShim(boardID, input_params);
                        DeviceObj.prepare_session();

                        // TODO: Celal check if we can do anything with GTEC
                        //string stringBoardMode = "/" + boardMode;
                        // Config board to digital mode (mode 3) for photo sensor
                        //Log.Debug("DAQ_OpenBCI - InitDevice | Sending board mode commands");
                        //DeviceObj.config_board("/3");
                        //DeviceObj.config_board("/2");
                        //DeviceObj.config_board("/3");

                        indEegChannels = BoardShim.get_eeg_channels(boardID);
                        int[] indOtherChannels = BoardShim.get_other_channels(boardID); //indOtherChannels = 12...18
                        sampleRate = BoardShim.get_sampling_rate(boardID);
                        BCISettingsFixed.DAQ_SampleRate = sampleRate;

                        BCIActuatorSettings.Save();

                        FrontendFilter = new Filter(frontendFilterIdx, Filter.FilterTypes.Frontend);
                        NotchFilter = new Filter(notchFilterIdx, Filter.FilterTypes.Notch);
                        Log.Debug("Creating Frontend filter: " + frontendFilterIdx + " | Notch filter: " + notchFilterIdx);

                        status = BoardStatus.BOARD_OPEN;

                        deviceInitialized = true;
                        
                        AddWarning(ExitCodes.IDLE, "  Time: " + DateTime.Now.ToString("h:mm:ss tt") + "  STATUS                 MESSAGE: Device initialized at serial port: " + serial_number);
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
        /// <param name="serial_number"></param>
        /// <param name="saveData"></param>
        /// <param name="sessionID"></param>
        /// <returns></returns>
        public bool Start(String serial_number = "", bool saveData = false, String sessionID = "")
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
                        initPortSuccess = InitDevice(serial_number);
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
        public bool Stop()
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
        public bool CloseDevice()
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
        public double[,] GetData(bool returnFilteredData = true)
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

                        // Append data to buffer for status signal
                        AppendDataToBuffer(filteredData, _bufferSignalStatus, SignalControl_WindowDurationForVrmsMeaseurment, out _bufferSignalStatus);

                        // TODO : Celal check how ofthen writing data to file!
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

        public double[,] daq_filter_data(double[,] unfilteredData)
        {
            var filteredData_notch = NotchFilter.FilterData(unfilteredData, indEegChannels);
            double[,] filteredData = FrontendFilter.FilterData(filteredData_notch, indEegChannels);
            return filteredData;
        }

        /// <summary>
        /// Inserts marker
        /// </summary>
        /// <param name="marker"></param>
        public void InsertMarker(float marker)
        {
            if (status == BoardStatus.BOARD_ACQUIRINGDATA)
                DeviceObj.insert_marker(marker + 1);//1=off, 2=0n
        }

        /// <summary>
        /// Writes markers to file
        /// </summary>
        /// <param name="markerValues"></param>
        public void WriteMarkerValues2File(List<int> markerValues)
        {
            if (saveDataToFile)
            {
                if (FileWriterObj == null)
                    FileWriterObj = new FileWriter();

                FileWriterObj.WriteMarkerValueToFile(markerValues);
            }
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

        #region Utils

        /// <summary>
        /// Tests if sensor is connected to the port
        /// </summary>
        /// <param name="serial_number"></param> port to test
        /// <returns></returns>
        private bool TestPort(String serial_number, out bool portAlreadyOpen)
        {
            portAlreadyOpen = false;
            try
            {
                Log.Debug("Testing port " + serial_number);
                BrainFlowInputParams input_params = new BrainFlowInputParams();
                //input_params.serial_number = serial_number;

                DeviceObj = new BoardShim(boardID, input_params);
                DeviceObj.prepare_session();
                DeviceObj.release_session();
                Log.Debug("Sensor detected to port" + serial_number);
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

                FileWriterObj.WriteHeaders(BoardShim.get_sampling_rate(boardID), indEegChannels, -1);
            }
        }

        /// <summary>
        /// Creates a new sesion (new files) without having to stop and start the device
        /// </summary>
        /// <param name="sessionID"></param>
        public bool StartSession(String sessionID, bool forceSavingData)
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

                        FileWriterObj.WriteHeaders(BoardShim.get_sampling_rate(boardID), indEegChannels, -1);

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
        public bool EndSession()
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

        private Queue<Dictionary<ExitCodes, string>> warnings = new Queue<Dictionary<ExitCodes, string>>();
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
                var release = warnings.Dequeue();
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
                Log.Debug(e.Message);
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
            DeviceObj.start_stream();
            //Config_Board("b");
        }

        /// <summary>
        /// Helper function to stop Cyton board streaming
        /// </summary>
        public void Stop_Streaming()
        {
            DeviceObj.stop_stream();
            //Config_Board("s");
        }

        /// <summary>
        /// Helper function to reset Cyton board to default state
        /// </summary>
        public void Reset_Board()
        {
            Config_Board("d");
        }


        public async Task<IList<string>> scanDevicesAsync(bool paired = true)
        {
            return await Task.Run(() =>
            {
                IList<string> devices = new List<string>();
                try
                {
                    devices = Unicorn.GetAvailableDevices(paired);
                }
                catch (Gtec.Unicorn.DeviceException ex)
                {
                    Log.Debug($"Error: {ex.Message}");
                }
                return devices;
            });
        }

        public async Task<bool> connectionTestAsync(string deviceName)
        {
            return await Task.Run(() =>
            {
                try
                {
                    Log.Debug($"Selected device: {deviceName}, trying to connect...");
                    using (Unicorn device = new Unicorn(deviceName))
                    {
                        Log.Debug($"Device: {device} is connected...");
                    }
                    Log.Debug($"Device: {deviceName} is disconnected...");
                    return true;
                }
                catch (Gtec.Unicorn.DeviceException ex)
                {
                    Log.Debug($"Error: {ex.Message}");
                    return false;
                }
                catch (Exception ex)
                {
                    Log.Debug($"Unexpected error: {ex.Message}");
                    return false;
                }
            });
        }
    }
    #endregion Utils
}