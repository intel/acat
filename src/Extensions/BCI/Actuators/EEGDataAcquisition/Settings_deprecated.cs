using ACAT.Lib.Core;
using ACAT.Lib.Core.PreferencesManagement;
using System;
using System.Xml.Serialization;



namespace ACAT.Extensions.Default.Actuators.EEG.EEGDataAcquisition
{
    /// <summary>
    /// Settings for the EEG acquisition part of the EEG actuator
    /// </summary>
    [Serializable]
    public class Settings : PreferencesBase
    {

        /// <summary>
        /// Name of the settings file
        /// </summary>
        [NonSerialized, XmlIgnore]
        public static String SettingsFilePath;


        /// <summary>
        /// 
        /// </summary>
        public string DAQ_ComPort { get; set; }

        /// <summary>
        /// Index where the data from the optical sensor is sent
        /// This can change depending on the port where is connected
        /// </summary>
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
        public int DAQ_FrontendFilterIdx { get; set; }


        /// <summary>
        /// Index of the notch filter: 
        /// 1: 50Hz (Europe)
        /// 2: 60Hz (USA)
        /// 0: none
        /// </summary>
        public int DAQ_NotchFilterIdx { get; set; }

        /// <summary>
        /// Directory where data will be saved
        /// </summary>
        public String DAQ_OutputDirectory { get; set; }

        /// <summary>
        /// True if data will be saved to a file
        /// </summary>
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
        public int SignalMonitor_ScaleIdx { get; set; } // settings.SignalMonitor_scaleIdx;

        /// <summary>
        /// Duration (in ms) to calculate UVrms of the received signal
        /// </summary>
        public int SignalControl_WindowDurationForVrmsMeaseurment { get; set; }

        /// <summary>
        /// Threshold (low) to consider a signal "Good", which corresponds to green status
        /// </summary>
        public float SignalControl_GoodSignal_Threshold_Low { get; set; }

        /// <summary>
        /// Threshold (high) to consider a signal "Good", which corresponds to green status
        /// </summary>
        public float SignalControl_GoodSignal_Threshold_High { get; set; }

        /// <summary>
        /// Threshold (low) to consider a signal "Acceptable", which corresponds to yellow status
        /// </summary>
        public float SignalControl_AcceptableSignal_Threshold_Low { get; set; }

        /// <summary>
        /// Threshold (high) to consider a signal "Acceptable", which corresponds to yellow status
        /// </summary>
        public float SignalControl_AcceptableSignal_Threshold_High { get; set; }



        /// <summary>
        /// Default values
        /// </summary>
        public Settings()
        {
            DAQ_ComPort = "COM4";
            DAQ_OpticalSensorIdxPort = 3; // Connected to D13
            DAQ_NotchFilterIdx = 2; //60Hz
            DAQ_FrontendFilterIdx = 4; //Bandpass 5-50Hz
            DAQ_OutputDirectory = "EEGData";
            DAQ_SaveToFileFlag = true;
            SignalMonitor_ScaleIdx = 1;
            SignalControl_WindowDurationForVrmsMeaseurment = 500;

            SignalControl_AcceptableSignal_Threshold_Low = 3;
            SignalControl_AcceptableSignal_Threshold_High = 12;
            SignalControl_GoodSignal_Threshold_High = 10;
            SignalControl_GoodSignal_Threshold_Low = 5;

        }


        /// <summary>
        /// Loads settings from the file
        /// </summary>
        /// <returns>Settings object</returns>
        public static Settings Load()
        {
            Settings retVal = PreferencesBase.Load<Settings>(SettingsFilePath);
            Save(retVal, SettingsFilePath);
            return retVal;
        }

        /// <summary>
        /// Saves the current settings to the preferences file
        /// </summary>
        /// <returns>true on success</returns>
        public override bool Save()
        {
            return Save<Settings>(this, SettingsFilePath);
        }

        /// <summary>
        /// Loads settings from the file
        /// </summary>
        /// <returns>Settings object</returns>
        public static Settings LoadDefaults()
        {
            return PreferencesBase.LoadDefaults<Settings>();
        }

    }
}
