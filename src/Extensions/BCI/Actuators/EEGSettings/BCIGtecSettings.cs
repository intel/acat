////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BCIGtecSettings.cs
//
// Settings for the BCI Actuator
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PreferencesManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace ACAT.Extensions.BCI.Actuators.EEG.EEGSettings
{
    /// <summary>
    /// Settings for the Sample Actuator
    /// </summary>
    [Serializable]
    public class BCIGtecSettings : PreferencesBase
    {
        /// <summary>
        /// Name of the settings file
        /// </summary>
        [NonSerialized, XmlIgnore]
        public static String SettingsFilePath;

        // ************************** Testing (internal use)  *********************************** //

        [Descriptor("For internal use, true to use GTec Unicorn board, false to use a dummy sensor")]
        [UIHint("ToggleSwitch")]
        public bool Testing_UseSensor;

        [Descriptor("For internal use, automatically duplicate the required channels as optional channels (simulate connection of daisy board)")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(false)]
        public bool Testing_DuplicateRequiredChannelsAsOptionalChannels { get; set; }  =  false;

        // [BoolDescriptor("For internal use, force recalibration from file in Testing_CalibrationFileId", false)]
        public bool Testing_ForceRecalibrateFromFile;

        //[StringDescriptor("For internal use, ID of the file to recalibrate from")]
        public string Testing_CalibrationFileId; // if empty use session

        //[IntDescriptor("For internal use, testID for data collection V2 (box / small buttons calibration and testing. Use 5 for ACAT-Talk", 1, 5, 1)]
        public int Testing_TestID; // For data collection V2

        //[BoolDescriptor("Disables signal quality checks when selecting Next from BCI Onboarding")]
        public bool Testing_IgnoreSignalTestResultDuringOnboarding;

        public int Testing_MinimumProbabiltyToDisplayBarOnTyping;

        // ************************** Scanning **************************************//

        [Descriptor("Pause time (ins ms)")]
        [Range(100, 5000)]
        [UIHint("Slider")]
        [DefaultValue(300)]
        public int Scanning_PauseTime { get; set; }  =  300;

        [Descriptor("Pause time (ins ms)")]
        [Range(100, 5000)]
        [UIHint("Slider")]
        [DefaultValue(300)]
        public int Scanning_ShortPauseTime { get; set; }  =  300;

        [Descriptor("Time (ins ms) when decision is shown")]
        [Range(200, 5000)]
        [UIHint("Slider")]
        [DefaultValue(2000)]
        public int Scanning_ShowDecisionTime { get; set; }  =  2000;

        [Descriptor("Delay (in ms) after a decision is made")]
        [Range(0, 20000)]
        [UIHint("Slider")]
        [DefaultValue(5000)]
        public int Scanning_DelayAfterDecision { get; set; }  =  5000;

        [Descriptor("Delay (in ms) to get ready before typing")]
        [Range(0, 20000)]
        [UIHint("Slider")]
        [DefaultValue(3000)]
        public int Scanning_DelayToGetReady { get; set; }  =  3000;

        [Descriptor("Is focal circle filled?")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(false)]
        public bool Scanning_IsFocalCircleFilled { get; set; }  =  false;

        [Descriptor("Color of the focal circle. Available options: green, yellow.")]
        [UIHint("TextBox")]
        [DefaultValue("green")]
        public String Scanning_FocalCircleColor { get; set; }  =  "green";

        // ************************** Calibration *********************************** //

        //[Descriptor("Offset added to target in calibration")]
        //[Range(0, 10000)]
        //[UIHint("Slider")]
        [DefaultValue(1000)]
        // public int Calibration_OffsetTarget { get; set; }  =  1000;
        public int Calibration_OffsetTarget;

        [IntDescriptor("Maximum elapsed time to force calibrating again")]
        [Range(30, 600)]
        [UIHint("Slider")]
        [DefaultValue(360)]
        public int Calibration_MaxElapsedTimeToForceRecalibration { get; set; }  =  360;

        [Descriptor("Path where the trained classifiers are stored")]
        [UIHint("TextBox")]
        [DefaultValue("Actuators\\BCI\\TrainedClassifiers")]
        public string Calibration_TrainedClassifiersFilePath { get; set; }  =  "Actuators\\BCI\\TrainedClassifiers";

        [Descriptor("Display popup window with signals after calibration")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(false)]
        public bool Calibration_DisplaySignalsAfterCalibrationFlag { get; set; }  =  false;

        [Descriptor("Use advance mode for typing-calibration mappins?")]
        [UIHint("ToggleSwitch")]
        public bool Calibration_UseAdvanceModeForTypingMappings;

        // ************************** DAQ / sensor *********************************** //

        /// <summary>
        /// Data parser: column index where EEG data starts. Default: 8
        /// </summary>
        [IntDescriptor("Number of channels of the device. Options: 8")]
        [UIHint("TextBox")]
        public int DAQ_NumEEGChannels;

        /// <summary>
        /// Bluetooth device name
        /// </summary>
        [Descriptor("Name of the GTec blueooth device")]
        [UIHint("TextBox")]
        [DefaultValue("")]
        public string GTecDeviceName { get; set; }  =  "";


        [Descriptor("Automatically disable bad channels while typing")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        public bool DAQ_DisableChannelsAutomatically { get; set; }  =  true;

        [Descriptor("Display filter settings screen before displaying EEG signals screen")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        public bool DAQ_ShowFilterSettings { get; set; }  =  true;

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
        [Descriptor("Index of the eeg filter where 0=no filter, 1=bandpass[1-50]Hz, 2=bandpass[7-13]Hz, 3=bandpass[15-50]Hz, 4=bandpass[5-50Hz], 5=highpass 20Hz")]
        [Range(0, 5)]
        [UIHint("Slider")]
        [DefaultValue(4)]
       public int DAQ_FrontendFilterIdx { get; set; }  =  4;

        /// <summary>
        /// Index of the notch filter:
        /// 1: 50Hz (Europe)
        /// 2: 60Hz (USA)
        /// 0: none
        /// </summary>
        [Descriptor("Index of the notch filter where 0=no filter, 1=50Hz (Europe), 2=60Hz (US)")]
        [Range(0, 2)]
        [UIHint("Slider")]
        [DefaultValue(2)]
        public int DAQ_NotchFilterIdx { get; set; }  =  2;

        /// <summary>
        /// Directory where data will be saved
        /// </summary>
        ///
        [Descriptor("Directory where EEG data will be saved")]
        [UIHint("TextBox")]
        [DefaultValue("EEGData")]
        public String DAQ_OutputDirectory { get; set; }  =  "EEGData";

        /// <summary>
        /// True if data will be saved to a file
        /// </summary>
        [Descriptor("Save filtered eeg data from typing to a file? NOTE: Calibration data from current session will always be saved to a file.")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        public bool DAQ_SaveToFileFlag { get; set; }  =  true;

        /// <summary>
        /// Saves raw data in addition to filtered data
        /// </summary>
        [Descriptor("Save additional file with rawData")]
        [UIHint("ToggleSwitch")]
        public bool DAQ_SaveAditionalFileWithRawData { get; set; }

        [Descriptor("Delay after typing repetition to ensure data is received from Cyton board")]
        [Range(0, 3000)]
        [UIHint("Slider")]
        public int DAQ_DelayAfterTypingRepetition =850;

        [Descriptor("Delay after calibration repetition to mimic typing")]
        [Range(0, 3000)]
        [UIHint("Slider")]
        [DefaultValue(0)]
        public int DAQ_DelayAfterCalibrationRepetition { get; set; }  =  0;

        // ************************** Signal control *********************************** //

        /// <summary>
        /// Scale displayed on signal monitor
        /// Options:
        ///   0: 50uV
        ///   1: 100uV
        ///   2: 200uV
        ///   3: 500uV
        ///   4: 1mV
        /// </summary>
        //[IntDescriptor("In signal monitor UI, idx corresponding to the scale used in the graphs", 0, 4, 0)]
        public int SignalMonitor_ScaleIdx { get; set; }

        /// <summary>
        /// Duration (in ms) to calculate UVrms of the received signal
        /// </summary>
        [Descriptor("Duration (in ms) of the window used to calculate the status of each channel")]
        [Range(100, 10000)]
        [UIHint("Slider")]
        [DefaultValue(1000)]
        public int SignalControl_WindowDurationForVrmsMeaseurment { get; set; }  =  1000;

        /// <summary>
        /// Boolen, true if recheck for signal quality required
        /// </summary>
        public bool SignalControl_RecheckNeeded { get; set; }

        [Descriptor("Short name / id (ex: Pz, C3, etc.) of channel #1 in required group")] // Cz
        [UIHint("TextBox")]
        public String SignalControl_RequiredChannel_Channel1_Name= "Cz";

        [Descriptor("Short name / id (ex: Pz, C3, etc.) of channel #2 in required group")] // C3
        [UIHint("TextBox")]
        public String SignalControl_RequiredChannel_Channel2_Name= "C3";

        [Descriptor("Short name / id (ex: Pz, C3, etc.) of channel #3 in required group")] // C4
        [UIHint("TextBox")]
        public String SignalControl_RequiredChannel_Channel3_Name= "C4";

        [Descriptor("Short name / id (ex: Pz, C3, etc.) of channel #4 in required group")] // Pz
        [UIHint("TextBox")]
        [DefaultValue("Pz")]
        public String SignalControl_RequiredChannel_Channel4_Name { get; set; }  =  "Pz";

        [Descriptor("Short name / id (ex: Pz, C3, etc.) of channel #5 in required group")] // P3
        [UIHint("TextBox")]
        [DefaultValue("P3")]
        public String SignalControl_RequiredChannel_Channel5_Name { get; set; }  =  "P3";

        [Descriptor("Short name / id (ex: Pz, C3, etc.) of channel #6 in required group")] // P4
        [UIHint("TextBox")]
        [DefaultValue("P4")]
        public String SignalControl_RequiredChannel_Channel6_Name { get; set; }  =  "P4";

        [Descriptor("Short name / id (ex: Pz, C3, etc.) of channel #7 in required group")] // T5
        [UIHint("TextBox")]
        [DefaultValue("T5")]
        public String SignalControl_RequiredChannel_Channel7_Name { get; set; }  =  "T5";

        [Descriptor("Short name / id (ex: Pz, C3, etc.) of channel #8 in required group")] // Fz
        [UIHint("TextBox")]
        [DefaultValue("Fz")]
        public String SignalControl_RequiredChannel_Channel8_Name { get; set; }  =  "Fz";

        // ************************** Signal control *********************************** //

        /// <summary>
        /// Scan time of the trigger test
        /// </summary>
        [Descriptor("Scan time of the trigger test")]
        [Range(50, 10000)]
        [UIHint("Slider")]
        public int TriggerTest_ScanTime=200;

        /// <summary>
        /// Number of iterations of the trigger test
        /// </summary>
        [IntDescriptor("Number of repetitons for the trigger test. One repetition corresponds to the trigger box switching black-white-black")]
        [Range(1, 1000)]
        [UIHint("Slider")]
        public int TriggerTest_NumRepetitions=10;

        /// <summary>
        /// Minimum duty cycle required to pass the trigger test. Set as 0 to bypass. Default 70
        /// </summary>
        public float TriggerTest_MinDutyCycleToPassTriggerTest { get; set; }

        // ************************** Signal Quality *********************************** //

        //[BoolDescriptor("Will the user do the signal quality checks. True if user answers yes to adjusting the electrodes since last time or the maximum time has elapsed")]
        // public bool SignalQuality_RecheckNeeded; // Not needed? Just need time at which last test was executed?

        //[LongDescriptorAttribute("Unix timestamp (seconds) of user's last impedance check completed", 0, long.MaxValue, 0)]
        public long SignalQuality_TimeOfLastImpedanceCheck​;

        [Descriptor("Maximum time elapsed (minutes) since user's last impedance check to allow before forcing a recheck")]
        [Range(0, 600)]
        [UIHint("Slider")]
        public int SignalQuality_MaxTimeMinsElapsedSinceLastImpedanceCheck​=360;

        // Most recent railing values computed during the user's last signal quality railing test
        public int[] SignalQuality_LastRailingValues;

        //[BoolDescriptor("If the user passed the last overall signal quality check that was executed (saved on user Exit or continuation to calibration")]
        public bool SignalQuality_PassedLastOverallQualityCheck;

        [Descriptor("Minimum number of electrodes with good status (green) required for overall good sensing quality")]
        [Range(0, 8)]
        [UIHint("Slider")]
        public int SignalQuality_MinOverallGoodChannels=5;

        [Descriptor("Maximum number of electrodes allowed with ok status (yellow) required for overall ok sensing quality")]
        [Range(0, 8)]
        [UIHint("Slider")]
        public int SignalQuality_MaxOverallOKChannels​=3;

        [Descriptor("Maximum number of electrodes with bad status (red) allowed to avoid overall bad sensing quality")]
        [Range(0, 8)]
        [UIHint("Slider")]
        public int SignalQuality_MaxOverallBadChannels=0;

        //// Default ranges for parameters with Cap attached

        [Descriptor("Upper bound (percentage) of the range of railing values considered good (green)")]
        [Range(0, 20)]
        [UIHint("Slider")]
        public int SignalQuality_RailingGoodMaxThreshold;

        [Descriptor("Upper bound (percentage) of the range of railing values considered ok (yellow)")]
        [Range(0, 25)]
        [UIHint("Slider")]
        public int SignalQuality_RailingOkMaxThreshold​=20;


        // ****************************** Feature extraction ********************************//
        //[IntDescriptor("Duration (in ms) of the window to detect ERPs in the eeg signals", 200, 1000, 500)]
        public int FeatureExtraction_WindowDurationInMs;

        /// <summary>
        /// Subset of channels. This will be an array where true=enabled, false=disabled
        /// </summary>
        [Descriptor("Whether channel #1 is enabled / utilized by the algorithm during processing")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        public bool Classifier_EnableChannel1 { get; set; }  =  true;

        [Descriptor("Whether channel #2 is enabled / utilized by the algorithm during processing")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        public bool Classifier_EnableChannel2 { get; set; }  =  true;

        [Descriptor("Whether channel #3 is enabled / utilized by the algorithm during processing")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        public bool Classifier_EnableChannel3 { get; set; }  =  true;

        [Descriptor("Whether channel #4 is enabled / utilized by the algorithm during processing")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        public bool Classifier_EnableChannel4 { get; set; }  =  true;

        [Descriptor("Whether channel #5 is enabled / utilized by the algorithm during processing")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        public bool Classifier_EnableChannel5 { get; set; }  =  true;

        [Descriptor("Whether channel #6 is enabled / utilized by the algorithm during processing")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        public bool Classifier_EnableChannel6 { get; set; }  =  true;

        [Descriptor("Whether channel #7 is enabled / utilized by the algorithm during processing")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        public bool Classifier_EnableChannel7 { get; set; }  =  true;

        [Descriptor("Whether channel #8 is enabled / utilized by the algorithm during processing")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        public bool Classifier_EnableChannel8 { get; set; }  =  true;

        [Descriptor("Whether channel #9 is enabled / utilized by the algorithm during processing")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        public bool Classifier_EnableChannel9 { get; set; }  =  true;

        [Descriptor("Whether channel #10 is enabled / utilized by the algorithm during processing")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        public bool Classifier_EnableChannel10 { get; set; }  =  true;

        [Descriptor("Whether channel #11 is enabled / utilized by the algorithm during processing")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        public bool Classifier_EnableChannel11 { get; set; }  =  true;

        [Descriptor("Whether channel #12 is enabled / utilized by the algorithm during processing")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        public bool Classifier_EnableChannel12 { get; set; }  =  true;

        [Descriptor("Whether channel #13 is enabled / utilized by the algorithm during processing")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        public bool Classifier_EnableChannel13 { get; set; }  =  true;

        [Descriptor("Whether channel #14 is enabled / utilized by the algorithm during processing")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        public bool Classifier_EnableChannel14 { get; set; }  =  true;

        [Descriptor("Whether channel #15 is enabled / utilized by the algorithm during processing")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        public bool Classifier_EnableChannel15 { get; set; }  =  true;

        [Descriptor("Whether channel #16 is enabled / utilized by the algorithm during processing")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        public bool Classifier_EnableChannel16 { get; set; }  =  true;

        // [StringDescriptor("Method to use for signal quality check ('Top8' = Signal quality check only on top 8 channels | 'AllEnabled' = Signal quality check on all channels enabled with Classifier_EnableChannel1-16)" , "AllEnabled")]
        public String SignalQuality_AcceptanceMode;

        /// <summary>
        /// Component to sort eigenvalues:
        /// Options: firstNcomponents (params: MinEigenvalue), threshold (params: VarianceThreshold), minRelativeEigenvalue (params:NumComponents)
        /// </summary>
        public string DimReductPCA_ComponentSortMethod;

        /// <summary>
        /// PCA: minimum number of eigenvalues  (if DimReductPCA_ComponentSortedMethod = MinEigenvalue). See DimReductPCA for details
        /// </summary>
        public double DimReductPCA_MinEigenvalue;

        /// <summary>
        /// PCA: variance threshold (if DimReductPCA_ComponentSortedMethod = threshold). See DimReductPCA for details
        /// </summary>
        public float DimReductPCA_VarianceThreshold;

        /// <summary>
        /// PCA: number of components (if DimReductPCA_ComponentSortedMethod = minRelativeEigenvalue). See DimReductPCA for details
        /// </summary>
        public int DimRecudtPCA_NumComponents;

        /// <summary>
        /// RDA: shrinkare parameter [0, 1, default: 0.9]
        /// </summary>
        public double DimReductRDA_ShrinkParam;

        /// <summary>
        /// RDA: regularization parameter [0, 1, default: 0.1]
        /// </summary>
        public double DimReductRDA_RegParam;

        // ****************************** Classification ********************************//

        /// <summary>
        /// Crossvalidation: number of folds. Default: 10
        /// </summary>
        public int CrossValidation_NumFolds;

        /// <summary>
        /// Crossvalidation: sorting method. Options: "sequential" "random"
        /// </summary>
        public string CrossaValidation_SortMethod;

        [Descriptor("Maximum number of sequences to predict user intended selection")]
        [Range(1, 50)]
        [UIHint("Slider")]
        public int Classifier_MaxDecisionSequences=10;

        [FloatDescriptor("Confidence threshold to make a selection", 0.5f, 1f, 0.95f)]
        public float Classifier_ConfidenceThreshold;

        [Descriptor("Include next character probabilities from a language model for faster character prediction")]
        [UIHint("ToggleSwitch")]
        public bool Classifier_UseNextCharacterProbabilities=true;

        [Descriptor("Include next word probabilities from a language model for faster word prediction")]
        [UIHint("ToggleSwitch")]
        public bool Classifier_UseNextWordProbabilities=true;

        // ***************************** Data parser ********************************** //

        /// <summary>
        /// Data parser: true to use software triggers, false to use hardware trigggers
        /// </summary>
        public bool DataParser_UseSoftwareTrigers;

        // ************************** Eyes closed detection *********************************** //

        [Descriptor("Enable eyes closed detection")]
        [UIHint("ToggleSwitch")]
        public bool EyesClosed_EnableDetection=false;

        [Descriptor("Eyes closed calibration, number of repetitions")]
        [UIHint("Slider")]
        public int EyesClosedCalibration_NumRepetitions;

        [Descriptor("Eyes closed calibration, duration when eyes are open/closed")]
        [UIHint("Slider")]
        public int EyesClosedCalibration_IntervalDuration;

        [Descriptor("Window duration for eyes closed detection")]
        [Range(1200, 10000)]
        [UIHint("Slider")]
        public int EyesClosed_WindowDuration=1500;

        [Descriptor("Delay to start animation after eyes closed detection")]
        [Range(0, 10000)]
        [UIHint("Slider")]
        public int EyesClosed_DelayToStartAnimationAfterDetection=1000;

        // This is dynamically updated
        [FloatDescriptor("Adaptive threshold (automatically calculated after calibration) for eyes closed detection", 0, 20, 5.5f)]
        public float EyesClosed_AdaptiveThreshold;

        [Descriptor("If eyes closed detection enabled, using fix threshold?")]
        [UIHint("ToggleSwitch")]
        public bool EyesClosed_UseFixThreshold=true;

        [Descriptor("Threshold for eyes closed detection")]
        [Range(1, 10)]
        [UIHint("Slider")]
        public float EyesClosed_FixThreshold_Threshold=5.8f;

        [FloatDescriptor("If eyes closed detection enabled and not using fix threshold", 0.1f, 10, 3)]
        public float EyesClosed_AdaptiveThreshold_StandardDeviationMultiplier;

        [Descriptor("Show disclaimer dialog on startup")]
        [UIHint("ToggleSwitch")]
        public bool ShowDisclaimerOnStartup=true;

        public BCIGtecSettings()
        {
            Calibration_DisplaySignalsAfterCalibrationFlag = false;
            Calibration_OffsetTarget = 1000;
            Calibration_MaxElapsedTimeToForceRecalibration = 360;
            Calibration_TrainedClassifiersFilePath = "Actuators\\BCI\\TrainedClassifiers";
            Calibration_UseAdvanceModeForTypingMappings = false;

            Classifier_ConfidenceThreshold = 0.95f;
            Classifier_MaxDecisionSequences = 10;
            Classifier_UseNextCharacterProbabilities = true;
            Classifier_UseNextWordProbabilities = false;

            CrossValidation_NumFolds = 10;
            CrossaValidation_SortMethod = "sequential";

            DataParser_UseSoftwareTrigers = false;

            // Required Channels
            Classifier_EnableChannel1 = true;
            Classifier_EnableChannel2 = true;
            Classifier_EnableChannel3 = true;
            Classifier_EnableChannel4 = true;
            Classifier_EnableChannel5 = true;
            Classifier_EnableChannel6 = true;
            Classifier_EnableChannel7 = true;
            Classifier_EnableChannel8 = true;

            SignalQuality_AcceptanceMode = "AllEnabled";

            DimReductPCA_ComponentSortMethod = "minRelativeEigenvalue";
            DimReductPCA_MinEigenvalue = 0.00001;
            DimReductRDA_ShrinkParam = 0.9;
            DimReductRDA_RegParam = 0.1;

            GTecDeviceName = "";
            DAQ_DisableChannelsAutomatically = false;
            DAQ_FrontendFilterIdx = 4; //Bandpass 5-50Hz
            DAQ_OutputDirectory = "EEGData";
            DAQ_NotchFilterIdx = 2; //60Hz
            DAQ_SaveToFileFlag = true;
            DAQ_SaveAditionalFileWithRawData = true;
            DAQ_ShowFilterSettings = true;
            DAQ_DelayAfterTypingRepetition = 850;
            DAQ_DelayAfterCalibrationRepetition = 0;
            DAQ_NumEEGChannels = 8;

            EyesClosedCalibration_IntervalDuration = 5000;
            EyesClosedCalibration_NumRepetitions = 10;
            EyesClosed_EnableDetection = false;
            EyesClosed_WindowDuration = 2000;
            EyesClosed_UseFixThreshold = false;
            EyesClosed_FixThreshold_Threshold = 5f;
            EyesClosed_AdaptiveThreshold_StandardDeviationMultiplier = 8;
            EyesClosed_DelayToStartAnimationAfterDetection = 1000;

            FeatureExtraction_WindowDurationInMs = 500;

            Scanning_PauseTime = 300;
            Scanning_ShortPauseTime = 300;
            Scanning_ShowDecisionTime = 2000;
            Scanning_DelayAfterDecision = 5000;
            Scanning_DelayToGetReady = 3000;

            Scanning_FocalCircleColor = "green";
            Scanning_IsFocalCircleFilled = false;

            SignalMonitor_ScaleIdx = 3; //
            SignalControl_WindowDurationForVrmsMeaseurment = 1000; //1 second

            SignalControl_RecheckNeeded = true; // by default, force user to do signal quality tests and calibration

            // Default channel names
            SignalControl_RequiredChannel_Channel1_Name = "Cz";
            SignalControl_RequiredChannel_Channel2_Name = "C3";
            SignalControl_RequiredChannel_Channel3_Name = "C4";
            SignalControl_RequiredChannel_Channel4_Name = "Pz";
            SignalControl_RequiredChannel_Channel5_Name = "P3";
            SignalControl_RequiredChannel_Channel6_Name = "P4";
            SignalControl_RequiredChannel_Channel7_Name = "T5";
            SignalControl_RequiredChannel_Channel8_Name = "Fz";

            TriggerTest_ScanTime = 200; // 200ms
            TriggerTest_NumRepetitions = 10; // 10 repetitions
            TriggerTest_MinDutyCycleToPassTriggerTest = 0.5f; // 0.5

            // SignalQuality_RecheckNeeded = true;
            SignalQuality_LastRailingValues = new int[8];
            for (int i = 0; i < 8; i++)
                SignalQuality_LastRailingValues[i] = int.MaxValue;

            SignalQuality_PassedLastOverallQualityCheck = false;
            SignalQuality_MinOverallGoodChannels = 6;
            SignalQuality_MaxOverallOKChannels​ = 2;
            SignalQuality_MaxOverallBadChannels​ = 0;

            //// Default ranges for parameters with Cap attached

            SignalQuality_RailingGoodMaxThreshold​ = 10;
            SignalQuality_RailingOkMaxThreshold​ = 20;

            //// Default ranges for parameters with Cap attached

            Testing_UseSensor = true;
            Testing_IgnoreSignalTestResultDuringOnboarding = false;
            Testing_ForceRecalibrateFromFile = false;
            Testing_CalibrationFileId = "_";
            Testing_TestID = 5;
            Testing_MinimumProbabiltyToDisplayBarOnTyping = 100; // no probabiliteis
            Testing_DuplicateRequiredChannelsAsOptionalChannels = false;

            ShowDisclaimerOnStartup = true;
        }

        /// <summary>
        /// Loads the settings from the settings file
        /// </summary>
        /// <returns>true on success</returns>
        public static BCIGtecSettings Load(bool saveAfterLoad = true)
        {
            BCIGtecSettings retVal = PreferencesBase.Load<BCIGtecSettings>(SettingsFilePath, true, saveAfterLoad);
            //Save(retVal, SettingsFilePath);
            return retVal;
        }

        /// <summary>
        /// Saves settings
        /// </summary>
        /// <returns>true on success</returns>
        public override bool Save()
        {
            return Save(this, SettingsFilePath);
        }

        public bool GetClassifier_EnableChannel(int channelIndx)
        {
            int channelName = channelIndx + 1;
            switch (channelName)
            {
                case 1:
                    return Classifier_EnableChannel1;

                case 2:
                    return Classifier_EnableChannel2;

                case 3:
                    return Classifier_EnableChannel3;

                case 4:
                    return Classifier_EnableChannel4;

                case 5:
                    return Classifier_EnableChannel5;

                case 6:
                    return Classifier_EnableChannel6;

                case 7:
                    return Classifier_EnableChannel7;

                case 8:
                    return Classifier_EnableChannel8;

            }
            return false;
        }

        public bool SetClassifier_EnableChannel(int channelIndx, bool newVal)
        {
            int channelName = channelIndx + 1;
            switch (channelName)
            {
                case 1:
                    Classifier_EnableChannel1 = newVal;
                    break;

                case 2:
                    Classifier_EnableChannel2 = newVal;
                    break;

                case 3:
                    Classifier_EnableChannel3 = newVal;
                    break;

                case 4:
                    Classifier_EnableChannel4 = newVal;
                    break;

                case 5:
                    Classifier_EnableChannel5 = newVal;
                    break;

                case 6:
                    Classifier_EnableChannel6 = newVal;
                    break;

                case 7:
                    Classifier_EnableChannel7 = newVal;
                    break;

                case 8:
                    Classifier_EnableChannel8 = newVal;
                    break;
            }
            return false;
        }
    }
}