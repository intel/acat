////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// DAQ_gTecBCI.cs
//
// Interfaces with the GTEC Unicorn sensor
//
////////////////////////////////////////////////////////////////////////////
using ACAT.Core.Utility;
using ACAT.Extensions.BCI.Actuators.EEG.EEGDataAcquisition.FileManagement;
using ACAT.Extensions.BCI.Actuators.EEG.EEGSettings;
using ACAT.Extensions.BCI.Common.BCIControl;
using Accord.Math;
using brainflow;
#if !SIMULATIONBOARD
using Gtec.Unicorn;
#endif

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ACAT.Extensions.BCI.Actuators.EEG.EEGDataAcquisition
{
    /// <summary>
    /// GTec implementation of DAQ for interfacing with the GTEC Unicorn sensor
    /// </summary>
    public class DAQ_gTecBCI : BaseDAQ
    {
        private static readonly ILogger<DAQ_gTecBCI> _logger = LoggingConfiguration.CreateLogger<DAQ_gTecBCI>();

        /// <summary>
        /// MInimum duty cycle required to pass trigger test. Set to 0 for no duty cycle requirement
        /// </summary>
        private float SignalControl_MinDutyCycleToPassTriggerTest;

        public enum BluetoothEvent
        {
            SUCCESSFUL_CONNECTION,
            DEVICE_DISCONNECTED,
            SCAN_DEVICES_REQUEST,
            SCAN_DEVICES_RESULT,
        };

        public delegate void DelegateBluetoothUpdate(BluetoothEvent bluetoothEvent, Dictionary<String, object> eventParams);

        public event DelegateBluetoothUpdate EvtBluetoothResult;

        /// <summary>
        /// Load settings from configuration
        /// </summary>
        public override void LoadSettings()
        {
            SignalControl_WindowDurationForVrmsMeaseurment = BCIActuatorSettings.Settings.SignalControl_WindowDurationForVrmsMeaseurment;
            SignalControl_MinDutyCycleToPassTriggerTest = BCIActuatorSettings.Settings.TriggerTest_MinDutyCycleToPassTriggerTest;
            _logger.LogDebug("DAQ settings loaded. Min duty cycle to pass trigger test" + SignalControl_MinDutyCycleToPassTriggerTest + " Window duration for uVrmsMeasurement: " + SignalControl_WindowDurationForVrmsMeaseurment);

            // Gtec Does not have hardware trigger so we will keep both same for ML that support openbci
            BCIActuatorSettings.Settings.DAQ_NumEEGChannels = 8;
            BCIActuatorSettings.Settings.DataParser_UseSoftwareTrigers = true;

#if SIMULATIONBOARD
            BCISettingsFixed.DataParser_IdxTriggerSignal_Sw = 32;
#else
            BCISettingsFixed.DataParser_IdxTriggerSignal_Sw = 19;
#endif
            BCISettingsFixed.DimReduct_DownsampleRate = 2;

            // Gtec does not use 9 to 16 it is only 8 channels
            BCIActuatorSettings.Settings.Classifier_EnableChannel9 = false;
            BCIActuatorSettings.Settings.Classifier_EnableChannel10 = false;
            BCIActuatorSettings.Settings.Classifier_EnableChannel11 = false;
            BCIActuatorSettings.Settings.Classifier_EnableChannel12 = false;
            BCIActuatorSettings.Settings.Classifier_EnableChannel13 = false;
            BCIActuatorSettings.Settings.Classifier_EnableChannel14 = false;
            BCIActuatorSettings.Settings.Classifier_EnableChannel15 = false;
            BCIActuatorSettings.Settings.Classifier_EnableChannel16 = false;

            BCIActuatorSettings.Save();

            _logger.LogDebug("Sensor set to " + BCIActuatorSettings.Settings.DAQ_NumEEGChannels + " channels. SensorID: " + BCISettingsFixed.DAQ_SensorId + " , Downsample rate: " + BCISettingsFixed.DimReduct_DownsampleRate +
                      " , Idx hw trigger signal: " + BCISettingsFixed.DataParser_IdxTriggerSignal_Hw + " , Idx sw trigger signal: " + BCISettingsFixed.DataParser_IdxTriggerSignal_Sw);

            saveDataToFile = BCIActuatorSettings.Settings.DAQ_SaveToFileFlag;
            frontendFilterIdx = BCIActuatorSettings.Settings.DAQ_FrontendFilterIdx;
            notchFilterIdx = BCIActuatorSettings.Settings.DAQ_NotchFilterIdx;

            _logger.LogDebug(" Frontend filter: " + frontendFilterIdx + " Notch filter: " + notchFilterIdx);
        }

        public static bool IsDeviceAvailable()
        {
            bool success = false;
            try
            {
#if SIMULATIONBOARD
                success = true;
#else
                // check if drivers are installed
                success = Unicorn.IsDeviceLibraryLoadable();

                // and if the device is available
                IList<string> devices = Unicorn.GetAvailableDevices(true);

                success = success && devices.Count > 0;
                //*/
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return success;
        }

        /// <summary>
        /// Initialize the device
        /// </summary>
        /// <param name="serial_number">Serial number of device to connect</param>
        /// <returns>True if successful, false otherwise</returns>
        public override bool InitDevice(string serial_number = "")
        {
#if SIMULATIONBOARD
            // Hardcode serial_number to indicate it's a simulator
            serial_number = "SYNTHETIC_BOARD";
#endif

            try
            {
                // Initialize boardID from base class
#if SIMULATIONBOARD
                boardID = (int)BoardIds.SYNTHETIC_BOARD;
#else
                boardID = (int)BoardIds.UNICORN_BOARD;
#endif

                if (status == BoardStatus.BOARD_OPEN)
                {
                    _logger.LogDebug("Board was open, closing device");
                    CloseDevice();
                }

                if (status == BoardStatus.BOARD_ACQUIRINGDATA)
                {
                    _logger.LogDebug("Board already acquiring data, returning");
                    return true;
                }
                else
                {
                    LoadSettings();

                    // Enable /disable boardlogging
                    if (boardLoggerEnabled)
                    {
                        _logger.LogDebug("BoardLoggerEnabled: " + boardLoggerEnabled + " Enabling brainflow logging");
                        BoardShim.enable_dev_board_logger();
                        BoardShim.set_log_file(boardLogFileName);
                    }
                    else
                    {
                        _logger.LogDebug("BoardLoggerEnabled: " + boardLoggerEnabled + " Disabling brainflow logging");
                        BoardShim.disable_board_logger();
                    }

                    // Test port
                    _logger.LogDebug("Testing port: " + serial_number);
                    bool sensorConnected = TestPort(serial_number, out _);

                    BrainFlowInputParams input_params = new();

                    if (sensorConnected)
                    {
                        _logger.LogDebug("Sensor connected to port " + serial_number);

                        // Save port to settings
                        BCIActuatorSettings.Settings.GTecDeviceName = serial_number;
                        BCIActuatorSettings.Save();
                        _logger.LogDebug("Port: " + serial_number + " saved to settings");

                        // DAQ_NumEEGChannels may have changed - run LoadSettings() at this point
                        LoadSettings();

                        input_params.serial_number = serial_number;

                        DeviceObj = new BoardShim(boardID, input_params);
                        DeviceObj.prepare_session();

                        indEegChannels = BoardShim.get_eeg_channels(boardID);
                        int battery = BoardShim.get_battery_channel(boardID);
#if SIMULATIONBOARD
                        int[] indOtherChannels = new int[7] { 12, 13, 14, 15, 16, 17, 18 };
#else
                        int[] indOtherChannels = BoardShim.get_other_channels(boardID); //indOtherChannels = 12...18
#endif
                        sampleRate = BoardShim.get_sampling_rate(boardID);
                        BCISettingsFixed.DAQ_SampleRate = sampleRate;

                        BCIActuatorSettings.Save();

                        FrontendFilter = new Filter(frontendFilterIdx, Filter.FilterTypes.Frontend);
                        NotchFilter = new Filter(notchFilterIdx, Filter.FilterTypes.Notch);
                        _logger.LogDebug("Creating Frontend filter: " + frontendFilterIdx + " | Notch filter: " + notchFilterIdx);

                        status = BoardStatus.BOARD_OPEN;

                        deviceInitialized = true;

                        AddWarning(ExitCodes.IDLE, "  Time: " + DateTime.Now.ToString("h:mm:ss tt") + "  STATUS                 MESSAGE: Device initialized at serial port: " + serial_number);
                        _logger.LogDebug("Board initialized. Status: " + status.ToString());

                        return true;
                    }
                    else
                        return false;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                sensorStatus = getErrorCode(e.Message, ExitCodes.BOARD_NOT_READY_ERROR);
                AddWarning(sensorStatus, "  Time: " + DateTime.Now.ToString("h:mm:ss tt") + "  WARNING             MESSAGE: Error Code: " + sensorStatus);
                return false;
            }
        }

        /// <summary>
        /// Start data acquisition
        /// </summary>
        /// <param name="serial_number">Serial number of device to connect</param>
        /// <param name="saveData">Whether to save data to file</param>
        /// <param name="sessionID">Session identifier</param>
        /// <returns>True if successful, false otherwise</returns>
        public override bool Start(string serial_number = "", bool saveData = false, string sessionID = "")
        {
            bool success = false;
            try
            {
                if (status != BoardStatus.BOARD_ACQUIRINGDATA)
                {
                    // Init device
                    bool initPortSuccess;
                    _logger.LogDebug("Initiating device");
                    if (status != BoardStatus.BOARD_OPEN)
                        initPortSuccess = InitDevice(serial_number);
                    else
                        initPortSuccess = true;

                    _logger.LogDebug("Starting stream");
                    Start_Streaming();
                    _logger.LogDebug("Stream started");

                    status = BoardStatus.BOARD_ACQUIRINGDATA;
                    //triggerTestInProgressFlag = false;

                    if (saveDataToFile)
                    {
                        _logger.LogDebug("Creating files for session " + sessionID);
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
                _logger.LogError("Exception:" + e.Message + " Error code:" + sensorStatus);
                success = false;
            }
            _logger.LogDebug("Device started: " + success);
            return success;
        }

        /// <summary>
        /// Stop data acquisition
        /// </summary>
        /// <returns>True if successful, false otherwise</returns>
        public override bool Stop()
        {
            try
            {
                if (status == BoardStatus.BOARD_ACQUIRINGDATA)
                {
                    _logger.LogDebug("Board acquiring data. Stopping device");
                    GetData();
                    Stop_Streaming();
                    _logger.LogDebug("Device stopped");
                }

                if (saveDataToFile && FileWriterObj != null && FileWriterObj.isFileOpened)
                {
                    _logger.LogDebug("Closing files");
                    FileWriterObj.CloseFiles();
                    FileWriterObj = null;
                    _logger.LogDebug("Files closed");
                }

                status = BoardStatus.BOARD_STANDBY;
                return true;
            }
            catch (Exception e)
            {
                sensorStatus = getErrorCode(e.Message, ExitCodes.SYNC_TIMEOUT_ERROR);
                AddWarning(sensorStatus, "  Time: " + DateTime.Now.ToString("h:mm:ss tt") + "  WARNING               MESSAGE: Error Code: " + sensorStatus);
                _logger.LogError("Exception:" + e.Message + " Error code: " + sensorStatus);
                return false;
            }
        }

        /// <summary>
        /// Close device and release resources
        /// </summary>
        /// <returns>True if successful, false otherwise</returns>
        public override bool CloseDevice()
        {
            try
            {
                if (status == BoardStatus.BOARD_CLOSED)
                {
                    _logger.LogDebug("Board already closed");
                    return true;
                }

                if (status == BoardStatus.BOARD_ACQUIRINGDATA)
                {
                    Stop_Streaming();
                }

                status = BoardStatus.BOARD_CLOSED;
                _logger.LogDebug("Device closed");
                return true;
            }
            catch (Exception e)
            {
                sensorStatus = getErrorCode(e.Message, ExitCodes.UNABLE_TO_CLOSE);
                AddWarning(sensorStatus, "  Time: " + DateTime.Now.ToString("h:mm:ss tt") + "  WARNING             MESSAGE: Error Code: " + sensorStatus);
                _logger.LogError("Exception:" + e.Message + " Error code: " + sensorStatus);
                return false;
            }
        }

        /// <summary>
        /// Get all available data from sensor
        /// </summary>
        /// <param name="returnFilteredData">Whether to return filtered data</param>
        /// <returns>Array of data samples</returns>
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

                        // Append data to buffer for status signal
                        AppendDataToBuffer(filteredData, _bufferSignalStatus, SignalControl_WindowDurationForVrmsMeaseurment, out _bufferSignalStatus);

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
                _logger.LogError("Exception: " + e.Message);
            }

            if (returnFilteredData)
                return filteredData;
            else
                return rawData;
        }

        /// <summary>
        /// Insert marker
        /// </summary>
        /// <param name="marker">Marker value</param>
        public override void InsertMarker(float marker)
        {
            if (status == BoardStatus.BOARD_ACQUIRINGDATA)
                DeviceObj.insert_marker(marker + 1); //1=off, 2=0n
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
                _logger.LogDebug("Testing port " + serial_number);
                BrainFlowInputParams input_params = new();
                //input_params.serial_number = serial_number;

                DeviceObj = new BoardShim(boardID, input_params);
                DeviceObj.prepare_session();
                DeviceObj.release_session();
                _logger.LogDebug("Sensor detected to port" + serial_number);
                return true;
            }
            catch (Exception e)
            {
                sensorStatus = getErrorCode(e.Message, ExitCodes.SET_PORT_ERROR);
                if (sensorStatus == ExitCodes.ANOTHER_BOARD_IS_CREATED_ERROR)
                    portAlreadyOpen = true;
                AddWarning(sensorStatus, "  Time: " + DateTime.Now.ToString("h:mm:ss tt") + "  WARNING             MESSAGE: Error Code: " + sensorStatus);
                _logger.LogError("Exception: " + e.Message);
                return false;
            }
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

        private readonly Queue<Dictionary<ExitCodes, string>> warnings = new();

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
            if (warnings.Count < warningLimit)
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
                _logger.LogError(e.Message);
                return info;
            }
            return info;
        }

        /// <summary>
        /// Helper function to start Cyton board streaming
        /// </summary>
        public void Start_Streaming()
        {
            DeviceObj.start_stream();
        }

        /// <summary>
        /// Helper function to stop Cyton board streaming
        /// </summary>
        public void Stop_Streaming()
        {
            DeviceObj.stop_stream();
            DeviceObj.release_session();
        }

        #endregion Utils
        
        /// <summary>
        /// Get Unicorn paired / unpaired devices using Unicorn API
        /// Pass on lists of devices in EvtBluetoothResult
        /// </summary>
        /// <param name="paired">Whether to get paired Unicorn devices (false = get unpaired devices)</param>
        /// <returns></returns>
        public async Task<IList<string>> ScanDevicesAsync(bool paired = true)
        {
            return await Task.Run(() =>
            {
                IList<string> devices = null;

                try
                {
#if SIMULATIONBOARD
                    devices = new List<string>()
                    {
                        "SYNTHETIC_BOARD"
                    };
#else
                    devices = Unicorn.GetAvailableDevices(paired);
#endif
                    Dictionary<String, object> eventParams = new()
                    {
                        ["paired"] = paired,
                        ["devices"] = devices
                    };
                    EvtBluetoothResult(BluetoothEvent.SCAN_DEVICES_RESULT, eventParams);
                }
                catch (Gtec.Unicorn.DeviceException ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
                return devices;
            });
        }

        /// <summary>
        /// Asynchronous task to test bluetooth connection status of device name stored in settings
        /// Send results of connection test in EvtBluetoothResult
        /// </summary>
        /// <returns></returns>
        public async Task<bool> connectionTestAsync()
        {
            // Check if there is a device name saved in settings
            if (string.IsNullOrEmpty(BCIActuatorSettings.Settings.GTecDeviceName))
            {
                Dictionary<String, object> eventParams = new()
                {
                    ["error"] = "String GTecDeviceName is null or empty"
                };
                EvtBluetoothResult(BluetoothEvent.DEVICE_DISCONNECTED, eventParams);
                return false;
            }
            else
            {
                // Use Unicorn API to try to initialize device (throws exception if device can't be initialized
                return await Task.Run(() =>
                {   
                    try
                    {

#if !SIMULATIONBOARD
                        _logger.LogDebug($"Selected device: {BCIActuatorSettings.Settings.GTecDeviceName}, trying to connect...");
                        using Unicorn device = new Unicorn(BCIActuatorSettings.Settings.GTecDeviceName);
                        _logger.LogDebug($"Device: {device} is connected...");
                        device.Dispose();
#endif
                        EvtBluetoothResult(BluetoothEvent.SUCCESSFUL_CONNECTION, null);
                        return true;
                    }
                    catch (Gtec.Unicorn.DeviceException ex)
                    {
                        _logger.LogError(ex, ex.Message);
                        Dictionary<String, object> eventParams = new()
                        {
                            ["error"] = ex.Message
                        };
                        EvtBluetoothResult(BluetoothEvent.DEVICE_DISCONNECTED, eventParams);
                        return false;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                        Dictionary<String, object> eventParams = new()
                        {
                            ["error"] = ex.Message
                        };
                        EvtBluetoothResult(BluetoothEvent.DEVICE_DISCONNECTED, eventParams);
                        return false;
                    }
                });
            }
        }

        /// <summary>
        /// Handler for dealing with BluetoothEvent requests (ex: scan for devices request)
        /// </summary>
        /// <param name="bluetoothEvent">Type of bluetooth request to handle</param>
        /// <param name="eventParams">Any extra params sent with bluetooth event request</param>
        public void bluetoothRequestHandler(BluetoothEvent bluetoothEvent, Dictionary<String, object> eventParams)
        {
            _logger.LogDebug("DAQ_gTecBCI | bluetoothRequestHandler | bluetoothEvent: " + bluetoothEvent.ToString());

            switch (bluetoothEvent)
            {
                case DAQ_gTecBCI.BluetoothEvent.SCAN_DEVICES_REQUEST:
                    _ = ScanDevicesAsync((bool)eventParams["paired"]);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Gets the status of the signals
        /// </summary>
        /// <param name="statusSignals">Array of signal status values for each channel</param>
        /// <returns>Overall signal status</returns>
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
                        // Set overall status and status signals as OK (temprarily until railing is implemented)
                        statusAllSignals = SignalStatus.SIGNAL_OK;
                        for (int channelIdx = 0; channelIdx < indEegChannels.Length; channelIdx++)
                            statusSignals[channelIdx] = SignalStatus.SIGNAL_OK;
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError("Exception: " + e.Message);
                }
            }
            return statusAllSignals;
        }
    }
}