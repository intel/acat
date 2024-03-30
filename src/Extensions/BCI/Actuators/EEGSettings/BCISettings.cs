////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BCISettings.cs
//
// Settings for the BCI Actuator
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PreferencesManagement;
using System;
using System.Xml.Serialization;

namespace ACAT.Extensions.BCI.Actuators.EEG.EEGSettings
{
    /// <summary>
    /// Settings for the Sample Actuator
    /// </summary>
    [Serializable]
    public class BCISettings : PreferencesBase
    {
        /// <summary>
        /// Name of the settings file
        /// </summary>
        [NonSerialized, XmlIgnore]
        public static String SettingsFilePath;

        // ************************** Testing (internal use)  *********************************** //

        [BoolDescriptor("For internal use, true to use OpenBCI Cyton board, false to use a dummy sensor")]
        public bool Testing_UseSensor;

        // [BoolDescriptor("For internal use, force recalibration from file in Testing_CalibrationFileId", false)]
        public bool Testing_ForceRecalibrateFromFile;

        //[StringDescriptor("For internal use, ID of the file to recalibrate from")]
        public string Testing_CalibrationFileId; // if empty use session

        //[IntDescriptor("For internal use, testID for data collection V2 (box / small buttons calibration and testing. Use 5 for ACAT-Talk", 1, 5, 1)]
        public int Testing_TestID; // For data collection V2

        //[BoolDescriptor("Disables signal quality checks when selecting Next from BCI Onboarding")]
        public bool Testing_IgnoreSignalTestResultDuringOnboarding;

        //[BoolDescriptor("Skips optical sensor tests in BCI Onboarding")]
        public bool Testing_BCIOnboardingIgnoreOpticalSensorChecks;

        public int Testing_MinimumProbabiltyToDisplayBarOnTyping;

        [BoolDescriptor("For internal use, automatically duplicate the required channels as optional channels (simulate connection of daisy board)", false)]
        public bool Testing_DuplicateRequiredChannelsAsOptionalChannels;

        // ************************** Scanning **************************************//

        [IntDescriptor("Pause time (ins ms)", 100, 5000, 300)]
        public int Scanning_PauseTime { get; set; }

        [IntDescriptor("Pause time (ins ms)", 100, 5000, 300)]
        public int Scanning_ShortPauseTime { get; set; }

        [IntDescriptor("Time (ins ms) when decision is shown", 200, 5000, 2000)]
        public int Scanning_ShowDecisionTime { get; set; }

        [IntDescriptor("Delay (in ms) after a decision is made", 0, 20000, 5000)]
        public int Scanning_DelayAfterDecision { get; set; }

        [IntDescriptor("Delay (in ms) to get ready before typing", 0, 20000, 3000)]
        public int Scanning_DelayToGetReady { get; set; }

        [BoolDescriptor("Is focal circle filled?", false)]
        public bool Scanning_IsFocalCircleFilled;

        [StringDescriptor("Color of the focal circle. Available options: green, yellow.", "green")]
        public String Scanning_FocalCircleColor;

        // ************************** Calibration *********************************** //

        //[IntDescriptor("Offset added to target in calibration", 0, 10000, 1000)]
        public int Calibration_OffsetTarget;

        [IntDescriptor("Maximum elapsed time to force calibrating again", 30, 600, 360)]
        public int Calibration_MaxElapsedTimeToForceRecalibration;

        [StringDescriptor("Path where the trained classifiers are stored", "Actuators\\BCI\\TrainedClassifiers")]
        public string Calibration_TrainedClassifiersFilePath;

        [BoolDescriptor("Display popup window with signals after calibration", false)]
        public bool Calibration_DisplaySignalsAfterCalibrationFlag;

        [BoolDescriptor("Use advance mode for typing-calibration mappins?")]
        public bool Calibration_UseAdvanceModeForTypingMappings;

        // ************************** DAQ / sensor *********************************** //

        /// <summary>
        /// Data parser: column index where EEG data starts. Default: 8
        /// </summary>
        [IntDescriptor("Number of channels of the device. Options: 8, 16", 8, 16, 8)]
        public int DAQ_NumEEGChannels;

        /// <summary>
        /// ComPort where the optical sensor is connected (this will be automatically detected in Onboarding)
        /// </summary>
        //[StringDescriptor("BCI sensor port", "COM4")]
        public string DAQ_ComPort { get; set; }

        /// <summary>
        /// Index where the data from the optical sensor is sent
        /// This can change depending on the port where is connected
        /// </summary>
        [IntDescriptor("Index of the port where the optical sensor is connected. Use 1 for port D11, 2 for port D12, 3 for port D13", 1, 3, 2)]
        public int DAQ_OpticalSensorIdxPort { get; set; }

        /// <summary>
        /// Lux threshold for the optical sensor
        /// </summary>
        [IntDescriptor("Lux threshold value for the optical sensor", 0, 60, 20)]
        public int DAQ_OpticalSensorLuxThreshold { get; set; }

        [BoolDescriptor("Automatically disable bad channels while typing", true)]
        public bool DAQ_DisableChannelsAutomatically;

        [BoolDescriptor("Display filter settings screen before displaying EEG signals screen", true)]
        public bool DAQ_ShowFilterSettings;

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
        //[IntDescriptor("Index of the eeg filter where 0=no filter, 1=bandpass[1-50]Hz, 2=bandpass[7-13]Hz, 3=bandpass[15-50]Hz, 4=bandpass[5-50Hz], 5=highpass 20Hz", 0, 5, 4)]
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
        [BoolDescriptor("Save filtered eeg data from typing to a file? NOTE: Calibration data from current session will always be saved to a file.", true)]
        public bool DAQ_SaveToFileFlag { get; set; }

        /// <summary>
        /// Saves raw data in addition to filtered data
        /// </summary>
        [BoolDescriptor("Save additional file with rawData")]
        public bool DAQ_SaveAditionalFileWithRawData { get; set; }

        [IntDescriptor("Delay after typing repetition to ensure data is received from Cyton board", 0, 3000, 850)]
        public int DAQ_DelayAfterTypingRepetition;

        [IntDescriptor("Delay after calibration repetition to mimic typing", 0, 3000, 0)]
        public int DAQ_DelayAfterCalibrationRepetition;

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
        [IntDescriptor("Duration (in ms) of the window used to calculate the status of each channel", 100, 10000, 1000)]
        public int SignalControl_WindowDurationForVrmsMeaseurment { get; set; }

        /// <summary>
        /// Boolen, true if recheck for signal quality required
        /// </summary>
        public bool SignalControl_RecheckNeeded { get; set; }

        [StringDescriptor("Short name / id (ex: Pz, C3, etc.) of channel #1 in required group", "Cz")]
        public String SignalControl_RequiredChannel_Channel1_Name;

        [StringDescriptor("Short name / id (ex: Pz, C3, etc.) of channel #2 in required group", "C3")]
        public String SignalControl_RequiredChannel_Channel2_Name;

        [StringDescriptor("Short name / id (ex: Pz, C3, etc.) of channel #3 in required group", "C4")]
        public String SignalControl_RequiredChannel_Channel3_Name;

        [StringDescriptor("Short name / id (ex: Pz, C3, etc.) of channel #4 in required group", "Pz")]
        public String SignalControl_RequiredChannel_Channel4_Name;

        [StringDescriptor("Short name / id (ex: Pz, C3, etc.) of channel #5 in required group", "P3")]
        public String SignalControl_RequiredChannel_Channel5_Name;

        [StringDescriptor("Short name / id (ex: Pz, C3, etc.) of channel #6 in required group", "P4")]
        public String SignalControl_RequiredChannel_Channel6_Name;

        [StringDescriptor("Short name / id (ex: Pz, C3, etc.) of channel #7 in required group", "T5")]
        public String SignalControl_RequiredChannel_Channel7_Name;

        [StringDescriptor("Short name / id (ex: Pz, C3, etc.) of channel #8 in required group", "Fz")]
        public String SignalControl_RequiredChannel_Channel8_Name;

        [StringDescriptor("Short name / id (ex: Pz, C3, etc.) of channel #9 in optional group", "T6")]
        public String SignalControl_OptionalChannel_Channel9_Name;

        [StringDescriptor("Short name / id (ex: Pz, C3, etc.) of channel #10 bin optional group", "F3")]
        public String SignalControl_OptionalChannel_Channel10_Name;

        [StringDescriptor("Short name / id (ex: Pz, C3, etc.) of channel #11 in optional group", "F4")]
        public String SignalControl_OptionalChannel_Channel11_Name;

        [StringDescriptor("Short name / id (ex: Pz, C3, etc.) of channel #12 in optional group", "F7")]
        public String SignalControl_OptionalChannel_Channel12_Name;

        [StringDescriptor("Short name / id (ex: Pz, C3, etc.) of channel #13 in optional group", "O1")]
        public String SignalControl_OptionalChannel_Channel13_Name;

        [StringDescriptor("Short name / id (ex: Pz, C3, etc.) of channel #14 in optional group", "O2")]
        public String SignalControl_OptionalChannel_Channel14_Name;

        [StringDescriptor("Short name / id (ex: Pz, C3, etc.) of channel #15 in optional group", "Fp1")]
        public String SignalControl_OptionalChannel_Channel15_Name;

        [StringDescriptor("Short name / id (ex: Pz, C3, etc.) of channel #16 in optional group", "Fp2")]
        public String SignalControl_OptionalChannel_Channel16_Name;

        // ************************** Signal control *********************************** //

        /// <summary>
        /// Scan time of the trigger test
        /// </summary>
        [IntDescriptor("Scan time of the trigger test", 50, 10000, 200)]
        public int TriggerTest_ScanTime;

        /// <summary>
        /// Number of iterations of the trigger test
        /// </summary>
        [IntDescriptor("Number of repetitons for the trigger test. One repetition corresponds to the trigger box switching black-white-black", 1, 1000, 10)]
        public int TriggerTest_NumRepetitions;

        /// <summary>
        /// Minimum duty cycle required to pass the trigger test. Set as 0 to bypass. Default 70
        /// </summary>
        public float TriggerTest_MinDutyCycleToPassTriggerTest { get; set; }

        // ************************** Signal Quality *********************************** //

        //[BoolDescriptor("Will the user do the signal quality checks. True if user answers yes to adjusting the electrodes since last time or the maximum time has elapsed")]
        // public bool SignalQuality_RecheckNeeded; // Not needed? Just need time at which last test was executed?

        // Most recent railing values computed during the user's last signal quality railing test
        public int[] SignalQuality_LastRailingValues;

        // Most recent impedance values computed during the user's last signal quality impedance test
        public int[] SignalQuality_LastImpedanceValues;

        //[LongDescriptorAttribute("Unix timestamp (seconds) of user's last impedance check completed", 0, long.MaxValue, 0)]
        public long SignalQuality_TimeOfLastImpedanceCheck​;

        [IntDescriptorAttribute("Maximum time elapsed (minutes) since user's last impedance check to allow before forcing a recheck", 0, 600, 360)]
        public int SignalQuality_MaxTimeMinsElapsedSinceLastImpedanceCheck​;

        //[BoolDescriptor("If the user passed the last overall signal quality check that was executed (saved on user Exit or continuation to calibration")]
        public bool SignalQuality_PassedLastOverallQualityCheck;

        [IntDescriptorAttribute("Minimum number of electrodes with good status (green) required for overall good sensing quality", 0, 8, 5)]
        public int SignalQuality_MinOverallGoodChannels;

        [IntDescriptorAttribute("Maximum number of electrodes allowed with ok status (yellow) required for overall ok sensing quality", 0, 8, 3)]
        public int SignalQuality_MaxOverallOKChannels​;

        [IntDescriptorAttribute("Maximum number of electrodes with bad status (red) allowed to avoid overall bad sensing quality", 0, 8, 0)]
        public int SignalQuality_MaxOverallBadChannels​;

        [BoolDescriptor("Whether to stop impedance testing after it completes one full cycle through all electrodes", true)]
        public bool SignalQuality_StopImpedanceTestAfterOneCycle = true;

        //// Ranges for parameters with NO Cap attached
        /*
        [IntDescriptorAttribute("Upper bound (percentage) of the range of railing values considered good (green)", 0, 20, 10)]
        public int SignalQuality_RailingGoodMaxThreshold​;

        [IntDescriptorAttribute("Upper bound (percentage) of the range of railing values considered ok (yellow)", 0, 25, 25)]
        public int SignalQuality_RailingOkMaxThreshold​;

        [IntDescriptorAttribute("Upper bound (kilo Ohms) of the range of impedance values considered good (green)", 0, 7000, 5500)]
        public int SignalQuality_ImpedanceGoodMaxThreshold​;

        [IntDescriptorAttribute("Upper bound (kilo Ohms) of the range of impedance values considered ok (yellow)", 0, 7000, 6500)]
        public int SignalQuality_ImpedanceOkMaxThreshold​;
        */
        //// Ranges for parameters with NO Cap attached

        //// Default ranges for parameters with Cap attached

        [IntDescriptorAttribute("Upper bound (percentage) of the range of railing values considered good (green)", 0, 20, 10)]
        public int SignalQuality_RailingGoodMaxThreshold​;

        [IntDescriptorAttribute("Upper bound (percentage) of the range of railing values considered ok (yellow)", 0, 25, 20)]
        public int SignalQuality_RailingOkMaxThreshold​;

        [IntDescriptorAttribute("Upper bound (kilo Ohms) of the range of impedance values considered good (green)", 0, 1000, 100)]
        public int SignalQuality_ImpedanceGoodMaxThreshold​;

        [IntDescriptorAttribute("Upper bound (kilo Ohms) of the range of impedance values considered ok (yellow)", 0, 1000, 200)]
        public int SignalQuality_ImpedanceOkMaxThreshold​;

        //// Default ranges for parameters with Cap attached

        // ****************************** Feature extraction ********************************//

        //[IntDescriptor("Duration (in ms) of the window to detect ERPs in the eeg signals", 200, 1000, 500)]
        public int FeatureExtraction_WindowDurationInMs;

        /// <summary>
        /// Subset of channels. This will be an array where true=enabled, false=disabled
        /// </summary>
        [BoolDescriptor("Whether channel #1 is enabled / utilized by the algorithm during processing", true)]
        public bool Classifier_EnableChannel1 = true;

        [BoolDescriptor("Whether channel #2 is enabled / utilized by the algorithm during processing", true)]
        public bool Classifier_EnableChannel2 = true;

        [BoolDescriptor("Whether channel #3 is enabled / utilized by the algorithm during processing", true)]
        public bool Classifier_EnableChannel3 = true;

        [BoolDescriptor("Whether channel #4 is enabled / utilized by the algorithm during processing", true)]
        public bool Classifier_EnableChannel4 = true;

        [BoolDescriptor("Whether channel #5 is enabled / utilized by the algorithm during processing", true)]
        public bool Classifier_EnableChannel5 = true;

        [BoolDescriptor("Whether channel #6 is enabled / utilized by the algorithm during processing", true)]
        public bool Classifier_EnableChannel6 = true;

        [BoolDescriptor("Whether channel #7 is enabled / utilized by the algorithm during processing", true)]
        public bool Classifier_EnableChannel7 = true;

        [BoolDescriptor("Whether channel #8 is enabled / utilized by the algorithm during processing", true)]
        public bool Classifier_EnableChannel8 = true;

        [BoolDescriptor("Whether channel #9 is enabled / utilized by the algorithm during processing", true)]
        public bool Classifier_EnableChannel9 = true;

        [BoolDescriptor("Whether channel #10 is enabled / utilized by the algorithm during processing", true)]
        public bool Classifier_EnableChannel10 = true;

        [BoolDescriptor("Whether channel #11 is enabled / utilized by the algorithm during processing", true)]
        public bool Classifier_EnableChannel11 = true;

        [BoolDescriptor("Whether channel #12 is enabled / utilized by the algorithm during processing", true)]
        public bool Classifier_EnableChannel12 = true;

        [BoolDescriptor("Whether channel #13 is enabled / utilized by the algorithm during processing", true)]
        public bool Classifier_EnableChannel13 = true;

        [BoolDescriptor("Whether channel #14 is enabled / utilized by the algorithm during processing", true)]
        public bool Classifier_EnableChannel14 = true;

        [BoolDescriptor("Whether channel #15 is enabled / utilized by the algorithm during processing", true)]
        public bool Classifier_EnableChannel15 = true;

        [BoolDescriptor("Whether channel #16 is enabled / utilized by the algorithm during processing", true)]
        public bool Classifier_EnableChannel16 = true;

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

        [IntDescriptor("Maximum number of sequences to predict user intended selection", 1, 50, 10)]
        public int Classifier_MaxDecisionSequences;

        [FloatDescriptor("Confidence threshold to make a selection", 0.5f, 1f, 0.95f)]
        public float Classifier_ConfidenceThreshold;

        [BoolDescriptor("Include next character probabilities from a language model for faster character prediction", true)]
        public bool Classifier_UseNextCharacterProbabilities;

        [BoolDescriptor("Include next word probabilities from a language model for faster word prediction", true)]
        public bool Classifier_UseNextWordProbabilities;

        // ***************************** Data parser ********************************** //

        /// <summary>
        /// Data parser: true to use software triggers, false to use hardware trigggers
        /// </summary>
        public bool DataParser_UseSoftwareTrigers;

        // ************************** Eyes closed detection *********************************** //

        [BoolDescriptor("Enable eyes closed detection", false)]
        public bool EyesClosed_EnableDetection;

        [IntDescriptor("Eyes closed calibration, number of repetitions")]
        public int EyesClosedCalibration_NumRepetitions;

        [IntDescriptor("Eyes closed calibration, duration when eyes are open/closed")]
        public int EyesClosedCalibration_IntervalDuration;

        [IntDescriptor("Window duration for eyes closed detection", 1200, 10000, 1500)]
        public int EyesClosed_WindowDuration;

        [IntDescriptor("Delay to start animation after eyes closed detection", 0, 10000, 1000)]
        public int EyesClosed_DelayToStartAnimationAfterDetection;

        // This is dynamically updated
        [FloatDescriptor("Adaptive threshold (automatically calculated after calibration) for eyes closed detection", 0, 20, 5.5f)]
        public float EyesClosed_AdaptiveThreshold;

        [BoolDescriptor("If eyes closed detection enabled, using fix threshold?", true)]
        public bool EyesClosed_UseFixThreshold;

        [FloatDescriptor("Threshold for eyes closed detection", 1, 10, 5.8f)]
        public float EyesClosed_FixThreshold_Threshold;

        [FloatDescriptor("If eyes closed detection enabled and not using fix threshold", 0.1f, 10, 3)]
        public float EyesClosed_AdaptiveThreshold_StandardDeviationMultiplier;

        [BoolDescriptor("Show disclaimer dialog on startup", true)]
        public bool ShowDisclaimerOnStartup;

        public BCISettings()
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

            // Optional Channels
            Classifier_EnableChannel9 = true;
            Classifier_EnableChannel10 = true;
            Classifier_EnableChannel11 = true;
            Classifier_EnableChannel12 = true;
            Classifier_EnableChannel13 = true;
            Classifier_EnableChannel14 = true;
            Classifier_EnableChannel15 = true;
            Classifier_EnableChannel16 = true;

            SignalQuality_AcceptanceMode = "AllEnabled";

            DimReductPCA_ComponentSortMethod = "minRelativeEigenvalue";
            DimReductPCA_MinEigenvalue = 0.00001;
            DimReductRDA_ShrinkParam = 0.9;
            DimReductRDA_RegParam = 0.1;

            DAQ_ComPort = "COM4";
            DAQ_DisableChannelsAutomatically = false;
            DAQ_FrontendFilterIdx = 4; //Bandpass 5-50Hz
            DAQ_OpticalSensorIdxPort = 3; // Connected to D13
            DAQ_OutputDirectory = "EEGData";
            DAQ_NotchFilterIdx = 2; //60Hz
            DAQ_SaveToFileFlag = true;
            DAQ_SaveAditionalFileWithRawData = true;
            DAQ_ShowFilterSettings = true;
            DAQ_DelayAfterTypingRepetition = 850;
            DAQ_DelayAfterCalibrationRepetition = 0;
            DAQ_NumEEGChannels = 8;
            DAQ_OpticalSensorLuxThreshold = 20;

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

            SignalMonitor_ScaleIdx = 2; //
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

            SignalControl_OptionalChannel_Channel9_Name = "T6";
            SignalControl_OptionalChannel_Channel10_Name = "F3";
            SignalControl_OptionalChannel_Channel11_Name = "F4";
            SignalControl_OptionalChannel_Channel12_Name = "F7";
            SignalControl_OptionalChannel_Channel13_Name = "O1";
            SignalControl_OptionalChannel_Channel14_Name = "O2";
            SignalControl_OptionalChannel_Channel15_Name = "Fp1";
            SignalControl_OptionalChannel_Channel16_Name = "Fp2";

            TriggerTest_ScanTime = 200; // 200ms
            TriggerTest_NumRepetitions = 10; // 10 repetitions
            TriggerTest_MinDutyCycleToPassTriggerTest = 0.5f; // 0.5

            // SignalQuality_RecheckNeeded = true;
            SignalQuality_LastRailingValues = new int[16];
            for (int i = 0; i < 16; i++)
                SignalQuality_LastRailingValues[i] = int.MaxValue;
            SignalQuality_LastImpedanceValues = new int[16];
            for (int i = 0; i < 16; i++)
                SignalQuality_LastImpedanceValues[i] = int.MaxValue;
            SignalQuality_StopImpedanceTestAfterOneCycle = true;
            SignalQuality_TimeOfLastImpedanceCheck = 0;
            SignalQuality_MaxTimeMinsElapsedSinceLastImpedanceCheck​ = 360;
            SignalQuality_PassedLastOverallQualityCheck = false;
            SignalQuality_MinOverallGoodChannels = 6;
            SignalQuality_MaxOverallOKChannels​ = 2;
            SignalQuality_MaxOverallBadChannels​ = 0;

            //// Ranges for parameters with NO Cap attached
            /*
            SignalQuality_RailingGoodMaxThreshold​ = 10;
            SignalQuality_RailingOkMaxThreshold​ = 25;
            SignalQuality_ImpedanceGoodMaxThreshold​ = 5500;
            SignalQuality_ImpedanceOkMaxThreshold​ = 6500;
            */
            //// Ranges for parameters with NO Cap attached

            //// Default ranges for parameters with Cap attached

            SignalQuality_RailingGoodMaxThreshold​ = 10;
            SignalQuality_RailingOkMaxThreshold​ = 20;
            SignalQuality_ImpedanceGoodMaxThreshold​ = 100;
            SignalQuality_ImpedanceOkMaxThreshold​ = 200;

            //// Default ranges for parameters with Cap attached

            Testing_UseSensor = true;
            Testing_IgnoreSignalTestResultDuringOnboarding = false;
            Testing_ForceRecalibrateFromFile = false;
            Testing_CalibrationFileId = "_";
            Testing_TestID = 5;
            Testing_MinimumProbabiltyToDisplayBarOnTyping = 100; // no probabiliteis
            Testing_BCIOnboardingIgnoreOpticalSensorChecks = false;
            Testing_DuplicateRequiredChannelsAsOptionalChannels = false;

            ShowDisclaimerOnStartup = true;
        }

        /// <summary>
        /// Loads the settings from the settings file
        /// </summary>
        /// <returns>true on success</returns>
        public static BCISettings Load(bool saveAfterLoad = true)
        {
            BCISettings retVal = PreferencesBase.Load<BCISettings>(SettingsFilePath, true, saveAfterLoad);
            //Save(retVal, SettingsFilePath);
            return retVal;
        }

        /// <summary>
        /// Saves settings
        /// </summary>
        /// <returns>true on success</returns>
        public override bool Save()
        {
            return Save<BCISettings>(this, SettingsFilePath);
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

                case 9:
                    return Classifier_EnableChannel9;

                case 10:
                    return Classifier_EnableChannel10;

                case 11:
                    return Classifier_EnableChannel11;

                case 12:
                    return Classifier_EnableChannel12;

                case 13:
                    return Classifier_EnableChannel13;

                case 14:
                    return Classifier_EnableChannel14;

                case 15:
                    return Classifier_EnableChannel15;

                case 16:
                    return Classifier_EnableChannel16;
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

                case 9:
                    Classifier_EnableChannel9 = newVal;
                    break;

                case 10:
                    Classifier_EnableChannel10 = newVal;
                    break;

                case 11:
                    Classifier_EnableChannel11 = newVal;
                    break;

                case 12:
                    Classifier_EnableChannel12 = newVal;
                    break;

                case 13:
                    Classifier_EnableChannel13 = newVal;
                    break;

                case 14:
                    Classifier_EnableChannel14 = newVal;
                    break;

                case 15:
                    Classifier_EnableChannel15 = newVal;
                    break;

                case 16:
                    Classifier_EnableChannel16 = newVal;
                    break;
            }
            return false;
        }
    }
}