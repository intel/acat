////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BaseDAQ.cs
//
// Abstract base class for EEG data acquisition implementations
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Utility;
using ACAT.Extensions.BCI.Actuators.EEG.EEGDataAcquisition.FileManagement;
using ACAT.Extensions.BCI.Actuators.EEG.EEGSettings;
using ACAT.Extensions.BCI.Common.BCIControl;
using Accord.Math;
using brainflow;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ACAT.Extensions.BCI.Actuators.EEG.EEGDataAcquisition
{
    /// <summary>
    /// Base abstract class for DAQ (Data Acquisition) implementations
    /// </summary>
    public abstract class BaseDAQ
    {
        /// <summary>
        /// Settings file name
        /// </summary>
        protected const string SettingsFileName = "BCIActuatorSettings.xml";

        /// <summary>
        /// Board log file name
        /// </summary>
        protected readonly string boardLogFileName = "boardLog";

        /// <summary>
        /// Whether board logging is enabled
        /// </summary>
        protected readonly bool boardLoggerEnabled = false;

        /// <summary>
        /// Board ID 
        /// </summary>
        protected int boardID;

        /// <summary>
        /// Sample rate
        /// </summary>
        public int sampleRate;

        /// <summary>
        /// Boolean, true if data should be saved to file
        /// </summary>
        protected bool saveDataToFile;

        /// <summary>
        /// Index for the notch filter
        /// </summary>
        protected int notchFilterIdx;

        /// <summary>
        /// Index for the frontend filter
        /// </summary>
        protected int frontendFilterIdx;

        /// <summary>
        /// Duration used to calculate VRMS and detect signal status (red/yellow/green)
        /// </summary>
        protected int SignalControl_WindowDurationForVrmsMeaseurment;

        /// <summary>
        /// Object to interact with board via Brainflow library
        /// </summary>
        protected BoardShim DeviceObj;

        /// <summary>
        /// Object to handle writing to files
        /// </summary>
        protected FileWriter FileWriterObj;

        /// <summary>
        /// Notch filter
        /// </summary>
        protected Filter NotchFilter;

        /// <summary>
        /// Frontend (bandpass) filter
        /// </summary>
        protected Filter FrontendFilter;

        /// <summary>
        /// Status of the board
        /// </summary>
        protected BoardStatus status;

        /// <summary>
        /// Boolean, true if device initialized
        /// </summary>
        public bool deviceInitialized = false;

        /// <summary>
        /// Buffer to store data and calculate signal status
        /// </summary>
        protected double[,] _bufferSignalStatus;

        /// <summary>
        /// Index of the EEG channels in data returned from sensor
        /// This is directly via from brainflow
        /// </summary>
        public int[] indEegChannels;

        /// <summary>
        /// Limit for the warning queue
        /// </summary>
        protected readonly int warningLimit = 10;

        /// <summary>
        /// Device status enum
        /// </summary>
        public enum DeviceStatus
        {
            DEVICE_STANDBY,
            DEVICE_ERROR,
            DEVICE_ACQUIRINGDATA,
        };

        /// <summary>
        /// Current device status
        /// </summary>
        public DeviceStatus deviceStatus;

        /// <summary>
        /// Board status enum
        /// </summary>
        public enum BoardStatus
        {
            BOARD_STANDBY,
            BOARD_OPEN,
            BOARD_CLOSED,
            BOARD_ACQUIRINGDATA,
        };

        /// <summary>
        /// Exit codes enum for error handling
        /// </summary>
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
            IDLE
        };

        /// <summary>
        /// Current sensor status
        /// </summary>
        public ExitCodes sensorStatus;

        /// <summary>
        /// Load settings from configuration file
        /// </summary>
        public abstract void LoadSettings();

        /// <summary>
        /// Get session directory
        /// </summary>
        /// <returns>Session directory path or null if FileWriter is not initialized</returns>
        public virtual string GetSessionDirectory()
        {
            if (FileWriterObj != null)
                return FileWriterObj.sessionDirectory;
            else
                return null;
        }

        /// <summary>
        /// Checks if device is acquiring data
        /// </summary>
        /// <returns>True if device is acquiring data, false otherwise</returns>
        public virtual bool IsAcquiring()
        {
            if (status == BoardStatus.BOARD_ACQUIRINGDATA)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Initialize the device 
        /// </summary>
        /// <param name="deviceIdentifier">Serial port or other device identifier</param>
        /// <returns>True if successful, false otherwise</returns>
        public abstract bool InitDevice(string deviceIdentifier);

        /// <summary>
        /// Start data acquisition
        /// </summary>
        /// <param name="deviceIdentifier">Serial port or other device identifier</param>
        /// <param name="saveData">Whether to save data to file</param>
        /// <param name="sessionID">Session identifier</param>
        /// <returns>True if successful, false otherwise</returns>
        public abstract bool Start(string deviceIdentifier = "", bool saveData = false, string sessionID = "");

        /// <summary>
        /// Stop data acquisition
        /// </summary>
        /// <returns>True if successful, false otherwise</returns>
        public abstract bool Stop();

        /// <summary>
        /// Close device and release resources
        /// </summary>
        /// <returns>True if successful, false otherwise</returns>
        public abstract bool CloseDevice();

        /// <summary>
        /// Get data from the device
        /// </summary>
        /// <param name="returnFilteredData">Whether to return filtered data</param>
        /// <returns>Array of data samples</returns>
        public abstract double[,] GetData(bool returnFilteredData = true);

        /// <summary>
        /// Filter raw data
        /// </summary>
        /// <param name="unfilteredData">Raw data to be filtered</param>
        /// <returns>Filtered data</returns>
        public virtual double[,] daq_filter_data(double[,] unfilteredData)
        {
            var filteredData_notch = NotchFilter.FilterData(unfilteredData, indEegChannels);
            double[,] filteredData = FrontendFilter.FilterData(filteredData_notch, indEegChannels);
            return filteredData;
        }

        /// <summary>
        /// Insert marker
        /// </summary>
        /// <param name="marker">Marker value</param>
        public virtual void InsertMarker(float marker)
        {
            if (status == BoardStatus.BOARD_ACQUIRINGDATA)
                DeviceObj.insert_marker(marker);
        }

        /// <summary>
        /// Write marker values to file
        /// </summary>
        /// <param name="markerValues">List of marker values</param>
        public virtual void WriteMarkerValues2File(List<int> markerValues)
        {
            if (saveDataToFile)
            {
                if (FileWriterObj == null)
                    FileWriterObj = new FileWriter();

                FileWriterObj.WriteMarkerValueToFile(markerValues);
            }
        }

        /// <summary>
        /// Get status of the signals
        /// </summary>
        /// <param name="statusSignals">Array to store status of individual signals</param>
        /// <returns>Overall signal status</returns>
        public abstract SignalStatus GetStatus(out SignalStatus[] statusSignals);

        /// <summary>
        /// Creates a new session (new files) without having to stop and start the device
        /// </summary>
        /// <param name="sessionID">Session identifier</param>
        /// <param name="forceSavingData">Force saving data even if disabled in settings</param>
        /// <returns>True if successful, false otherwise</returns>
        public virtual bool StartSession(string sessionID, bool forceSavingData)
        {
            bool result = false;
            try
            {
                if (forceSavingData)
                    saveDataToFile = forceSavingData; // This is useful for calibration where data should always be saved
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
                Log.Exception("Exception " + e.Message);
            }
            return result;
        }

        /// <summary>
        /// End current session
        /// </summary>
        /// <returns>True if successful, false otherwise</returns>
        public virtual bool EndSession()
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

        /// <summary>
        /// For internal use, adds filtered data to a buffer to assess signal status
        /// </summary>
        /// <param name="data">Data to append</param>
        /// <param name="inBuffer">Input buffer</param>
        /// <param name="numSamplesInBuffer">Number of samples to maintain in buffer</param>
        /// <param name="outBuffer">Output buffer with appended data</param>
        /// <returns>True if successful, false otherwise</returns>
        protected virtual bool AppendDataToBuffer(double[,] data, double[,] inBuffer, int numSamplesInBuffer, out double[,] outBuffer)
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
        /// Create files where data is stored
        /// </summary>
        /// <param name="sessionID">Session identifier</param>
        protected virtual void CreateFiles(string sessionID)
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
        /// Send lower level config command to BoardShim device
        /// </summary>
        /// <param name="cmd">Command to send</param>
        public virtual void Config_Board(string cmd)
        {
            try
            {
                Log.Debug("Config board. Command: " + cmd);
                DeviceObj.config_board(cmd);
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
    }
}
