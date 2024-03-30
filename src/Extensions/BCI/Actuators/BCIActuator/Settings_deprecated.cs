////////////////////////////////////////////////////////////////////////////
// <copyright file="Settings.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2017 Intel Corporation 
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PreferencesManagement;
using System;
using System.Xml.Serialization;

namespace ACAT.Extensions.Default.Actuators.BCIActuator
{
    /// <summary>
    /// Settings for the Sample Actuator
    /// </summary>
    [Serializable]
    public class Settings : PreferencesBase
    {
        /// <summary>
        /// Name of the settings file
        /// </summary>
        [NonSerialized, XmlIgnore]
        public static String SettingsFilePath;

        [BoolDescriptor("For internal use, false to use a dummy sensor")]
        public bool useSensor;

        [IntDescriptor("For internal data collection only, number of targets to type", 1, 100, 20)]
        public int NumCharactersToTypeInDataCollection;

        [BoolDescriptor("Calibrate on start")]
        public bool calibrateOnStart;

        [IntDescriptor("Number of targets in calibration", 1, 500, 60)]
        public int CalibrationTargetCount;

        [IntDescriptor("Number of iterations per target in calibration", 1, 500, 1)]
        public int CalibrationIterationsPerTarget;


        /// <summary>
        /// ComPort where the optical sensor is connected
        /// </summary>
        [StringDescriptor("BCI sensor port", "COM4")]
        public string DAQ_ComPort { get; set; }

        /// <summary>
        /// Index where the data from the optical sensor is sent
        /// This can change depending on the port where is connected
        /// </summary>
        [IntDescriptor("Index of the port where the optical sensor is connected. Use 1 for D11, 2 for D12, 3 for D13", 1, 3, 2)]
        public int DAQ_OpticalSensorIdxPort { get; set; }


        /// <summary>
        /// Index of the frontend filter: 
        /// 1: bandpass 1-50Hz
        /// 2: bandpass 7-13Hz
        /// 3: bandpass 15-50Hz
        /// 4: bandpass 5-50Hz
        /// 5: highpass 20Hz
        /// 0: no filter
        /// Default: 4 (bandpass 5-50Hz)
        /// </summary>
        /// 
        [IntDescriptor("Index of the eeg filter where 0=no filter, 1=bandpass[1-50]Hz, 2=bandpass[7-13]Hz, 3=bandpass[15-50]Hz, 4=bandpass[5-50Hz], 5=highpass 20Hz", 0, 5, 4)]
        public int DAQ_FrontendFilterIdx { get; set; }


        /// <summary>
        /// Index of the notch filter: 
        /// 1: 50Hz (Europe)
        /// 2: 60Hz (USA)
        /// 0: none
        /// </summary>
        [IntDescriptor("Index of the notch filter where 0=no filter, 1=50Hz (Europe), 2=60Hz (US)", 0, 2, 2)]
        public int DAQ_NotchFilterIdx { get; set; }

        /// <summary>
        /// Directory where data will be saved
        /// </summary>
        /// 
       [StringDescriptor("Directory where EEG data will be saved", "EEGData")]
        public String DAQ_OutputDirectory { get; set; }

        /// <summary>
        /// True if data will be saved to a file
        /// </summary>
        [BoolDescriptor("Save eeg data to file?", true)]
        public bool DAQ_SaveToFileFlag { get; set; }

        /// <summary>
        /// Scale displayed on signal monitor
        /// Options: 
        ///   0: 50uV
        ///   1: 100uV 
        ///   2: 200uV 
        ///   3: 500uV 
        ///   4: 1mV
        /// </summary>
        [IntDescriptor("In signal monitor UI, idx corresponding to the scale used in the graphs", 0, 4, 0)]
        public int SignalMonitor_ScaleIdx { get; set; } // settings.SignalMonitor_scaleIdx;

        /// <summary>
        /// Duration (in ms) to calculate UVrms of the received signal
        /// </summary>
        [IntDescriptor("Duration (in ms) of the window used to calculate the status of each channel")]
        public int SignalControl_WindowDurationForVrmsMeaseurment { get; set; }

        /// <summary>
        /// If signal is under the threshold, the signal is considered railed
        /// </summary>
        public double SignalControl_ThresholdSignalRailed { get; set; }

        /// <summary>
        /// Threshold to consider the signal out of range
        /// </summary>
        public double SignalControl_ThresholdSignalOutOfRange { get; set; }




        public Settings()
        {
            useSensor = true;
            calibrateOnStart = true;
            CalibrationTargetCount = 60;
            CalibrationIterationsPerTarget = 1;
            NumCharactersToTypeInDataCollection = 100;
            DAQ_ComPort = "COM4";
            DAQ_OpticalSensorIdxPort = 3; // Connected to D13
            DAQ_NotchFilterIdx = 2; //60Hz
            DAQ_FrontendFilterIdx = 4; //Bandpass 5-50Hz
            DAQ_OutputDirectory = "EEGData";
            SignalMonitor_ScaleIdx = 0; //50Hz
            SignalControl_WindowDurationForVrmsMeaseurment = 1000; //1 second
            SignalControl_ThresholdSignalRailed = 4;
            SignalControl_ThresholdSignalOutOfRange = 12;
        }


        /// <summary>
        /// Loads the settings from the settings file
        /// </summary>
        /// <returns>true on success</returns>
        public static Settings Load()
        {
            Settings retVal = PreferencesBase.Load<Settings>(SettingsFilePath);
            Save(retVal, SettingsFilePath);
            return retVal;
        }

        /// <summary>
        /// Saves settings
        /// </summary>
        /// <returns>true on success</returns>
        public override bool Save()
        {
            return Save<Settings>(this, SettingsFilePath);
        }
    }
}