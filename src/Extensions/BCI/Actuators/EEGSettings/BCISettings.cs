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

using ACATResources;
using ACAT.Core.PreferencesManagement;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ACAT.Extensions.BCI.Actuators.EEG.EEGSettings
{
    /// <summary>
    /// Settings for the Sample Actuator
    /// </summary>
    [Serializable]
    public partial class BCISettings : PreferencesBase
    {
        /// <summary>
        /// Name of the settings file
        /// </summary>
        [NonSerialized, XmlIgnore]
        public static String SettingsFilePath;

        // ************************** Testing (internal use)  *********************************** //

#if DEBUG
        [Display(
            Name = "Enable Debug Mode",
            Description = "Disables directly using BCI sensor.  UI will let you skip through steps.",
            GroupName = "Developer Settings")]
        [ObservableProperty]
        private bool testing_UseSensor = true;

        [Display(
            Name = "Force Recalibration From File",
            Description = "For internal use, force recalibration from file in Testing_CalibrationFileId",
            GroupName = "Developer Settings")]
        [ObservableProperty]
        private bool testing_ForceRecalibrateFromFile = false;

        [Display(
            Name = "Calibration File Name",
            Description = "ID of the file to recalibrate from.",
            GroupName = "Developer Settings")]
        [ObservableProperty]
        private string testing_CalibrationFileId;

        [Display(
            Name = "Test ID Number",
            Description = "testID for data collection V2 (box / small buttons calibration and testing. Use 5 for ACAT-Talk",
            GroupName = "Developer Settings")]
        [ObservableProperty]
        private int testing_TestID; // For data collection V2

        [Display(
            Name = "Disable Signal Qualty Check",
            Description = "Disables signal quality checks when selecting Next from BCI Onboarding",
            GroupName = "Developer Settings")]
        [ObservableProperty]
        private bool testing_IgnoreSignalTestResultDuringOnboarding = false;

        [Display(
            Name = "Disable Optical Sensor Tests",
            Description = "Skips optical sensor tests in BCI Onboarding",
            GroupName = "Developer Settings")]
        [ObservableProperty]
        private bool testing_BCIOnboardingIgnoreOpticalSensorChecks = false;

        [Display(
            Name = "Min Probability to Display on Typing",
            Description = "Minimum probablility score to show on the display while typing.",
            GroupName = "Developer Settings")]
        [UIHint("TextBox")]
        [ObservableProperty]
        private int testing_MinimumProbabiltyToDisplayBarOnTyping;

        [Display(
            Name = "Duplicate Required Channels as Optional",
            Description = "Automatically duplicate the required channels as optional channels (simulate connection of daisy board)",
            GroupName = "Developer Settings")]
        [ObservableProperty]
        private bool testing_DuplicateRequiredChannelsAsOptionalChannels = false;

#else // DON'T DISPLAY THESE SETTINGS IN RELEASE BUILD, ONLY FOR TESTING PURPOSES
        public bool Testing_UseSensor = true;

        public bool Testing_ForceRecalibrateFromFile = false;

        public string Testing_CalibrationFileId;

        public int Testing_TestID = 5;

        public bool Testing_IgnoreSignalTestResultDuringOnboarding = false;

        public bool Testing_BCIOnboardingIgnoreOpticalSensorChecks = false;

        public int Testing_MinimumProbabiltyToDisplayBarOnTyping = 0;

        public bool Testing_DuplicateRequiredChannelsAsOptionalChannels = false;
#endif
        // ************************** Scanning **************************************//

        [Display(
            Name = "Scanning pause time (ms)",
            Description = "Pause time in milliseconds between scanning iterations. Default: 300ms",
            GroupName = "Scanning Settings")]
        [Range(100, 5000)]
        [UIHint("Slider")]
        [DefaultValue(300)]
        [ObservableProperty]
        private int scanning_PauseTime = 300;

        [Display(
            Name = "Scanning short pause time (ms)",
            Description = "Pause time in milliseconds between scanning iterations when the user is typing. Default: 300ms",
            GroupName = "Scanning Settings")]
        [Range(100, 5000)]
        [UIHint("Slider")]
        [DefaultValue(300)]
        [ObservableProperty]
        private int scanning_ShortPauseTime = 300;

        [Display(
            Name = "Scanning decision display time (ms)",
            Description = "Time in milliseconds to display the decision made by the user. Default: 2000ms",
            GroupName = "Scanning Settings")]
        [Range(0, 5000)]
        [UIHint("Slider")]
        [DefaultValue(2000)]
        [ObservableProperty]
        private int scanning_ShowDecisionTime = 2000;

        [Display(
            Name = "Scanning delay after decision (ms)",
            Description = "Delay in milliseconds after a decision is made before the next scanning iteration starts. Default: 5000ms",
            GroupName = "Scanning Settings")]
        [Range(0, 20000)]
        [UIHint("Slider")]
        [DefaultValue(5000)]
        [ObservableProperty]
        private int scanning_DelayAfterDecision  = 5000;

        [Display(
            Name = "Scanning delay to get ready (ms)",
            Description = "Delay in milliseconds to get ready before typing. Default: 3000ms",
            GroupName = "Scanning Settings")]
        [Range(0, 20000)]
        [UIHint("Slider")]
        [DefaultValue(3000)]
        [ObservableProperty]
        private int scanning_DelayToGetReady = 3000;

        [Display(
            Name="Fill Focal Circle",
            Description = "If true, the focal circle will be filled with the color specified in Scanning_FocalCircleColor. Default: false")]
        [UIHint("ToggleSwitch")]
        [ObservableProperty]
        private bool scanning_IsFocalCircleFilled = false;

        [Display(
            Name = "Focal Circle Color",
            Description = "Color of the focal circle. Available options: green, yellow. Default: green",
            GroupName = "Scanning Settings")]
        [UIHint("TextBox")]
        [ObservableProperty]
        private String scanning_FocalCircleColor = "green";

        // ************************** Calibration *********************************** //

        //[IntDescriptor("Offset added to target in calibration", 0, 10000, 1000)]
        public int Calibration_OffsetTarget;

        [Display(
            Name = "Maximum elapsed time to force calibrating again")]
        [Range(30, 600)]
        [UIHint("Slider")]
        [ObservableProperty]
        private int calibration_MaxElapsedTimeToForceRecalibration = 360;

        public string Calibration_TrainedClassifiersFilePath  = "Actuators\\BCI\\TrainedClassifiers";

        [Display(
            Name = "Display popup window with signals after calibration?")]
        [UIHint("ToggleSwitch")]
        [ObservableProperty]
        private bool calibration_DisplaySignalsAfterCalibrationFlag = false;

        [Display(
            Name = "Use advance mode for typing-calibration mappings?")]
        [BoolDescriptor("Use advance mode for typing-calibration mappins?")]
        [UIHint("ToggleSwitch")]
        [ObservableProperty]
        private bool calibration_UseAdvanceModeForTypingMappings;

        // ************************** DAQ / sensor *********************************** //

        /// <summary>
        /// Data parser: column index where EEG data starts. Default: 8
        /// </summary>
        [Descriptor("Number of channels of the device. Options: 8, 16")]
        [Range(8, 16)]
        [UIHint("Slider")]
        [ObservableProperty]
        private int dAQ_NumEEGChannels = 8;

        /// <summary>
        /// ComPort where the optical sensor is connected (this will be automatically detected in Onboarding)
        /// </summary>
        //[StringDescriptor("BCI sensor port", "COM4")]
        public string DAQ_ComPort { get; set; }

        [Descriptor("Name of the GTec blueooth device")]
        [UIHint("TextBox")]
        [DefaultValue("")]
        [ObservableProperty]
        private string gTecDeviceName  = "";

        /// <summary>
        /// Index where the data from the optical sensor is sent
        /// This can change depending on the port where is connected
        /// </summary>
        [Descriptor("Index of the port where the optical sensor is connected. Use 1 for port D11, 2 for port D12, 3 for port D13")]
        [Range(1, 3)]
        [UIHint("Slider")]
        [DefaultValue(2)]
        [ObservableProperty]
        private int dAQ_OpticalSensorIdxPort  = 2;

        /// <summary>
        /// Lux threshold for the optical sensor
        /// </summary>
        [Descriptor("Lux threshold value for the optical sensor")]
        [Range(0, 60)]
        [UIHint("Slider")]
        [DefaultValue(20)]
        [ObservableProperty]
        private int dAQ_OpticalSensorLuxThreshold  = 20;

        [Descriptor("Automatically disable bad channels while typing")]
        [UIHint("ToggleSwitch")]
        [ObservableProperty]
        private bool dAQ_DisableChannelsAutomatically = true;

        [Descriptor("Display filter settings screen before displaying EEG signals screen")]
        [UIHint("ToggleSwitch")]
        [ObservableProperty]
        private bool dAQ_ShowFilterSettings = true;

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
        [Descriptor("Index of the notch filter where 0=no filter, 1=50Hz (Europe), 2=60Hz (US)")]
        [Range(0, 2)]
        [UIHint("Slider")]
        [DefaultValue(2)]
        [ObservableProperty]
        private int dAQ_NotchFilterIdx  = 2;

        /// <summary>
        /// Directory where data will be saved
        /// </summary>
        ///
        [Descriptor("Directory where EEG data will be saved")]
        [UIHint("TextBox")]
        [ObservableProperty]
        private String dAQ_OutputDirectory = "EEGData";

        /// <summary>
        /// True if data will be saved to a file
        /// </summary>
        [Descriptor("Save filtered eeg data from typing to a file? NOTE: Calibration data from current session will always be saved to a file.")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        [ObservableProperty]
        private bool dAQ_SaveToFileFlag  = true;

        /// <summary>
        /// Saves raw data in addition to filtered data
        /// </summary>
        [Descriptor("Save additional file with rawData")]
        [UIHint("ToggleSwitch")]
        [ObservableProperty]
        private bool dAQ_SaveAditionalFileWithRawData;

        [Descriptor("Delay after typing repetition to ensure data is received from Cyton board")]
        [Range(0, 3000)]
        [UIHint("Slider")]
        [ObservableProperty]
        private int dAQ_DelayAfterTypingRepetition = 850;

        [Descriptor("Delay after calibration repetition to mimic typing")]
        [Range(0, 3000)]
        [UIHint("Slider")]
        [ObservableProperty]
        private int dAQ_DelayAfterCalibrationRepetition = 0;

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
        [Range(0, 10000)]
        [UIHint("Slider")]
        [DefaultValue(1000)]
        [ObservableProperty]
        private int signalControl_WindowDurationForVrmsMeaseurment  = 1000;

        /// <summary>
        /// Boolen, true if recheck for signal quality required
        /// </summary>
        public bool SignalControl_RecheckNeeded { get; set; }

        public string SignalControl_RequiredChannel_Channel1_Name = "Cz";
        public string SignalControl_RequiredChannel_Channel2_Name = "C3";
        public string SignalControl_RequiredChannel_Channel3_Name = "C4";
        public string SignalControl_RequiredChannel_Channel4_Name = "Pz";
        public string SignalControl_RequiredChannel_Channel5_Name = "P3";
        public string SignalControl_RequiredChannel_Channel6_Name = "P4";
        public string SignalControl_RequiredChannel_Channel7_Name = "T5";
        public string SignalControl_RequiredChannel_Channel8_Name = "Fz";
        public string SignalControl_OptionalChannel_Channel9_Name = "T6";
        public string SignalControl_OptionalChannel_Channel10_Name  = "F3";
        public string SignalControl_OptionalChannel_Channel11_Name  = "F4";
        public string SignalControl_OptionalChannel_Channel12_Name = "F7";
        public string SignalControl_OptionalChannel_Channel13_Name = "O1";
        public string SignalControl_OptionalChannel_Channel14_Name = "O2";
        public string SignalControl_OptionalChannel_Channel15_Name = "Fp1";
        public string SignalControl_OptionalChannel_Channel16_Name = "Fp2";

        // ************************** Signal control *********************************** //

        /// <summary>
        /// Scan time of the trigger test
        /// </summary>
        [Descriptor("Scan time of the trigger test")]
        [Range(50, 10000)]
        [UIHint("Slider")]
        [ObservableProperty]
        private int triggerTest_ScanTime = 200;

        /// <summary>
        /// Number of iterations of the trigger test
        /// </summary>
        [Descriptor("Number of repetitons for the trigger test. One repetition corresponds to the trigger box switching black-white-black")]
        [Range(1, 1000)]
        [UIHint("Slider")]
        [ObservableProperty]
        private int triggerTest_NumRepetitions = 10;

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

        [Descriptor("Maximum time elapsed (minutes) since user's last impedance check to allow before forcing a recheck")]
        [Range(0, 600)]
        [UIHint("Slider")]
        [ObservableProperty]
        private int signalQuality_MaxTimeMinsElapsedSinceLastImpedanceCheck​;

        //[BoolDescriptor("If the user passed the last overall signal quality check that was executed (saved on user Exit or continuation to calibration")]
        public bool SignalQuality_PassedLastOverallQualityCheck;

        [Descriptor("Minimum number of electrodes with good status (green) required for overall good sensing quality")]
        [Range(0, 8)]
        [UIHint("Slider")]
        [ObservableProperty]
        private int signalQuality_MinOverallGoodChannels = 5;

        [Descriptor("Maximum number of electrodes allowed with ok status (yellow) required for overall ok sensing quality")]
        [Range(0, 8)]
        [UIHint("Slider")]
        [ObservableProperty]
        private int signalQuality_MaxOverallOKChannels​;

        [Descriptor("Maximum number of electrodes with bad status (red) allowed to avoid overall bad sensing quality")]
        [Range(0, 8)]
        [UIHint("Slider")]
        [ObservableProperty]
        private int signalQuality_MaxOverallBadChannels​ = 0;

        [Descriptor("Whether to stop impedance testing after it completes one full cycle through all electrodes")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        [ObservableProperty]
        private bool signalQuality_StopImpedanceTestAfterOneCycle = true;

        //// Default ranges for parameters with Cap attached

        [Descriptor("Upper bound (percentage) of the range of railing values considered good (green)")]
        [Range(0, 20)]
        [UIHint("Slider")]
        [ObservableProperty]
        private int signalQuality_RailingGoodMaxThreshold​;

        [Descriptor("Upper bound (percentage) of the range of railing values considered ok (yellow)")]
        [Range(0, 25)]
        [UIHint("Slider")]
        [ObservableProperty]
        private int signalQuality_RailingOkMaxThreshold​ = 20;

        [Descriptor("Upper bound (kilo Ohms) of the range of impedance values considered good (green)")]
        [Range(0, 1000)]
        [UIHint("Slider")]
        [ObservableProperty]
        private int signalQuality_ImpedanceGoodMaxThreshold​;

        [Descriptor("Upper bound (kilo Ohms) of the range of impedance values considered ok (yellow)")]
        [Range(0, 1000)]
        [UIHint("Slider")]
        [ObservableProperty]
        private int signalQuality_ImpedanceOkMaxThreshold​ = 200;

        //// Default ranges for parameters with Cap attached

        // ****************************** Feature extraction ********************************//

        //[IntDescriptor("Duration (in ms) of the window to detect ERPs in the eeg signals", 200, 1000, 500)]
        public int FeatureExtraction_WindowDurationInMs;

        /// <summary>
        /// Subset of channels. This will be an array where true=enabled, false=disabled
        /// </summary>
        [Descriptor("Whether channel #1 is enabled / utilized by the algorithm during processing")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        [ObservableProperty]
        private bool classifier_EnableChannel1 = true;

        [Descriptor("Whether channel #2 is enabled / utilized by the algorithm during processing")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        [ObservableProperty]
        private bool classifier_EnableChannel2 = true;

        [Descriptor("Whether channel #3 is enabled / utilized by the algorithm during processing")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        [ObservableProperty]
        private bool classifier_EnableChannel3  = true;

        [Descriptor("Whether channel #4 is enabled / utilized by the algorithm during processing")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        [ObservableProperty]
        private bool classifier_EnableChannel4  = true;

        [Descriptor("Whether channel #5 is enabled / utilized by the algorithm during processing")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        [ObservableProperty]
        private bool classifier_EnableChannel5  = true;

        [Descriptor("Whether channel #6 is enabled / utilized by the algorithm during processing")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        [ObservableProperty]
        private bool classifier_EnableChannel6  = true;

        [Descriptor("Whether channel #7 is enabled / utilized by the algorithm during processing")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        [ObservableProperty]
        private bool classifier_EnableChannel7  = true;

        [Descriptor("Whether channel #8 is enabled / utilized by the algorithm during processing")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        [ObservableProperty]
        private bool classifier_EnableChannel8  = true;

        [Descriptor("Whether channel #9 is enabled / utilized by the algorithm during processing")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        [ObservableProperty]
        private bool classifier_EnableChannel9  = true;

        [Descriptor("Whether channel #10 is enabled / utilized by the algorithm during processing")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        [ObservableProperty]
        private bool classifier_EnableChannel10  = true;

        [Descriptor("Whether channel #11 is enabled / utilized by the algorithm during processing")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        [ObservableProperty]
        private bool classifier_EnableChannel11  = true;

        [Descriptor("Whether channel #12 is enabled / utilized by the algorithm during processing")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        [ObservableProperty]
        private bool classifier_EnableChannel12 = true;

        [Descriptor("Whether channel #13 is enabled / utilized by the algorithm during processing")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        [ObservableProperty]
        private bool classifier_EnableChannel13 = true;

        [Descriptor("Whether channel #14 is enabled / utilized by the algorithm during processing")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        [ObservableProperty]
        private bool classifier_EnableChannel14  = true;

        [Descriptor("Whether channel #15 is enabled / utilized by the algorithm during processing")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        [ObservableProperty]
        private bool classifier_EnableChannel15 = true;

        [Descriptor("Whether channel #16 is enabled / utilized by the algorithm during processing")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        [ObservableProperty]
        private bool classifier_EnableChannel16  = true;

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
        [ObservableProperty]
        private int classifier_MaxDecisionSequences = 10;

        [Descriptor("Confidence threshold to make a selection")]
        [Range(0.5f, 1f)]
        [UIHint("Slider")]
        [DefaultValue(0.95f)]
        [ObservableProperty]
        private float classifier_ConfidenceThreshold = 0.95f;

        [Descriptor("Include next character probabilities from a language model for faster character prediction")]
        [UIHint("ToggleSwitch")]
        [ObservableProperty]
        private bool classifier_UseNextCharacterProbabilities = true;

        [Descriptor("Include next word probabilities from a language model for faster word prediction")]
        [UIHint("ToggleSwitch")]
        [ObservableProperty]
        private bool classifier_UseNextWordProbabilities = true;

        // ***************************** Data parser ********************************** //

        /// <summary>
        /// Data parser: true to use software triggers, false to use hardware trigggers
        /// </summary>
        public bool DataParser_UseSoftwareTrigers;

        // ************************** Eyes closed detection *********************************** //

        [Descriptor("Enable eyes closed detection")]
        [UIHint("ToggleSwitch")]
        [ObservableProperty]
        private bool eyesClosed_EnableDetection = false;

        [Descriptor("Eyes closed calibration, number of repetitions")]
        [UIHint("Slider")]
        [ObservableProperty]
        private int eyesClosedCalibration_NumRepetitions;

        [Descriptor("Eyes closed calibration, duration when eyes are open/closed")]
        [UIHint("Slider")]
        [ObservableProperty]
        private int eyesClosedCalibration_IntervalDuration;

        [IntDescriptor("Window duration for eyes closed detection")]
        [Range(1200, 10000)]
        [UIHint("Slider")]
        [ObservableProperty]
        private int eyesClosed_WindowDuration = 1500;

        [Descriptor("Delay to start animation after eyes closed detection")]
        [Range(0, 10000)]
        [UIHint("Slider")]
        [ObservableProperty]
        private int eyesClosed_DelayToStartAnimationAfterDetection = 1000;

        // This is dynamically updated
        [Descriptor("Adaptive threshold (automatically calculated after calibration) for eyes closed detection")]
        [Range(0, 20)]
        [UIHint("Slider")]
        [DefaultValue(5.5f)]
        [ObservableProperty]
        private float eyesClosed_AdaptiveThreshold  = 5.5f;

        [Descriptor("If eyes closed detection enabled, using fix threshold?")]
        [UIHint("ToggleSwitch")]
        [ObservableProperty]
        private bool eyesClosed_UseFixThreshold = true;

        [Descriptor("Threshold for eyes closed detection")]
        [Range(1, 10)]
        [UIHint("Slider")]
        [DefaultValue(5.8f)]
        [ObservableProperty]
        private float eyesClosed_FixThreshold_Threshold = 5.8f;

        [Descriptor("If eyes closed detection enabled and not using fix threshold")]
        [Range(0.1f, 10)]
        [UIHint("Slider")]
        [DefaultValue(3)]
        [ObservableProperty]
        private float eyesClosed_AdaptiveThreshold_StandardDeviationMultiplier = 3;

        [Descriptor("Show disclaimer dialog on startup")]
        [UIHint("ToggleSwitch")]
        [ObservableProperty]
        private bool showDisclaimerOnStartup = true;

        public BCISettings()
        {
            calibration_DisplaySignalsAfterCalibrationFlag = false;
            Calibration_OffsetTarget = 1000;
            calibration_MaxElapsedTimeToForceRecalibration = 360;
            Calibration_TrainedClassifiersFilePath = "Actuators\\BCI\\TrainedClassifiers";
            calibration_UseAdvanceModeForTypingMappings = false;

            classifier_ConfidenceThreshold = 0.95f;
            classifier_MaxDecisionSequences = 10;
            classifier_UseNextCharacterProbabilities = true;
            classifier_UseNextWordProbabilities = false;

            CrossValidation_NumFolds = 10;
            CrossaValidation_SortMethod = "sequential";

            DataParser_UseSoftwareTrigers = true;

            // Required Channels
            classifier_EnableChannel1 = true;
            classifier_EnableChannel2 = true;
            classifier_EnableChannel3 = true;
            classifier_EnableChannel4 = true;
            classifier_EnableChannel5 = true;
            classifier_EnableChannel6 = true;
            classifier_EnableChannel7 = true;
            classifier_EnableChannel8 = true;

            // Optional Channels
            classifier_EnableChannel9 = true;
            classifier_EnableChannel10 = true;
            classifier_EnableChannel11 = true;
            classifier_EnableChannel12 = true;
            classifier_EnableChannel13 = true;
            classifier_EnableChannel14 = true;
            classifier_EnableChannel15 = true;
            classifier_EnableChannel16 = true;

            SignalQuality_AcceptanceMode = "AllEnabled";

            DimReductPCA_ComponentSortMethod = "minRelativeEigenvalue";
            DimReductPCA_MinEigenvalue = 0.00001;
            DimReductRDA_ShrinkParam = 0.9;
            DimReductRDA_RegParam = 0.1;

            DAQ_ComPort = "COM4";
            dAQ_DisableChannelsAutomatically = false;
            DAQ_FrontendFilterIdx = 4; //Bandpass 5-50Hz
            dAQ_OpticalSensorIdxPort = 3; // Connected to D13
            dAQ_OutputDirectory = "EEGData";
            dAQ_NotchFilterIdx = 2; //60Hz
            dAQ_SaveToFileFlag = true;
            dAQ_SaveAditionalFileWithRawData = true;
            dAQ_ShowFilterSettings = true;
            dAQ_DelayAfterTypingRepetition = 850;
            dAQ_DelayAfterCalibrationRepetition = 0;
            dAQ_NumEEGChannels = 8;
            dAQ_OpticalSensorLuxThreshold = 20;

            eyesClosedCalibration_IntervalDuration = 5000;
            eyesClosedCalibration_NumRepetitions = 10;
            eyesClosed_EnableDetection = false;
            eyesClosed_WindowDuration = 2000;
            eyesClosed_UseFixThreshold = false;
            eyesClosed_FixThreshold_Threshold = 5f;
            eyesClosed_AdaptiveThreshold_StandardDeviationMultiplier = 8;
            eyesClosed_DelayToStartAnimationAfterDetection = 1000;

            FeatureExtraction_WindowDurationInMs = 500;

            scanning_PauseTime = 300;
            scanning_ShortPauseTime = 300;
            scanning_ShowDecisionTime = 2000;
            scanning_DelayAfterDecision = 5000;
            scanning_DelayToGetReady = 3000;

            scanning_FocalCircleColor = "green";
            scanning_IsFocalCircleFilled = false;

            SignalMonitor_ScaleIdx = 2; //
            signalControl_WindowDurationForVrmsMeaseurment = 1000; //1 second

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

            triggerTest_ScanTime = 200; // 200ms
            triggerTest_NumRepetitions = 10; // 10 repetitions
            TriggerTest_MinDutyCycleToPassTriggerTest = 0.5f; // 0.5

            // SignalQuality_RecheckNeeded = true;
            SignalQuality_LastRailingValues = new int[16];
            for (int i = 0; i < 16; i++)
                SignalQuality_LastRailingValues[i] = int.MaxValue;
            SignalQuality_LastImpedanceValues = new int[16];
            for (int i = 0; i < 16; i++)
                SignalQuality_LastImpedanceValues[i] = int.MaxValue;
            signalQuality_StopImpedanceTestAfterOneCycle = true;
            SignalQuality_TimeOfLastImpedanceCheck = 0;
            signalQuality_MaxTimeMinsElapsedSinceLastImpedanceCheck​ = 360;
            SignalQuality_PassedLastOverallQualityCheck = false;
            signalQuality_MinOverallGoodChannels = 6;
            signalQuality_MaxOverallOKChannels​ = 2;
            signalQuality_MaxOverallBadChannels​ = 0;

            //// Ranges for parameters with NO Cap attached
            /*
            SignalQuality_RailingGoodMaxThreshold​ = 10;
            SignalQuality_RailingOkMaxThreshold​ = 25;
            SignalQuality_ImpedanceGoodMaxThreshold​ = 5500;
            SignalQuality_ImpedanceOkMaxThreshold​ = 6500;
            */
            //// Ranges for parameters with NO Cap attached

            //// Default ranges for parameters with Cap attached

            signalQuality_RailingGoodMaxThreshold​ = 7;
            signalQuality_RailingOkMaxThreshold​ = 10;
            signalQuality_ImpedanceGoodMaxThreshold​ = 100;
            signalQuality_ImpedanceOkMaxThreshold​ = 200;

            //// Default ranges for parameters with Cap attached

            Testing_UseSensor = true;
            Testing_IgnoreSignalTestResultDuringOnboarding = false;
            Testing_ForceRecalibrateFromFile = false;
            Testing_CalibrationFileId = "_";
            Testing_TestID = 5;
            Testing_MinimumProbabiltyToDisplayBarOnTyping = 100; // no probabiliteis
            Testing_BCIOnboardingIgnoreOpticalSensorChecks = false;
            Testing_DuplicateRequiredChannelsAsOptionalChannels = false;

            showDisclaimerOnStartup = true;

            gTecDeviceName = "";
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
            return Save(this, SettingsFilePath);
        }

        public bool GetClassifier_EnableChannel(int channelIndx)
        {
            int channelName = channelIndx + 1;
            return channelName switch
            {
                1 => Classifier_EnableChannel1,
                2 => Classifier_EnableChannel2,
                3 => Classifier_EnableChannel3,
                4 => Classifier_EnableChannel4,
                5 => Classifier_EnableChannel5,
                6 => Classifier_EnableChannel6,
                7 => Classifier_EnableChannel7,
                8 => Classifier_EnableChannel8,
                9 => Classifier_EnableChannel9,
                10 => Classifier_EnableChannel10,
                11 => Classifier_EnableChannel11,
                12 => Classifier_EnableChannel12,
                13 => Classifier_EnableChannel13,
                14 => Classifier_EnableChannel14,
                15 => Classifier_EnableChannel15,
                16 => Classifier_EnableChannel16,
                _ => false,
            };
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