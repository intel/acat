////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BCIActuator.cs
//
// Handles all the interactions between the BCI actuator and ACAT: controls the sensor,
// sends data to the processing algorithm and sends results to ACAT
//
////////////////////////////////////////////////////////////////////////////
#define OPTICAL_SENSOR

using ACAT.ACATResources;
using ACAT.Extensions.BCI.Actuators.EEG.EEGDataAcquisition;
using ACAT.Extensions.BCI.Actuators.EEG.EEGProcessing;
using ACAT.Extensions.BCI.Actuators.EEG.EEGSettings;
using ACAT.Extensions.BCI.Actuators.EEG.EEGUtils;
using ACAT.Extensions.BCI.Actuators.SensorUI;
using ACAT.Extensions.BCI.Common.BCIControl;
using ACAT.Lib.Core.ActuatorManagement;
using ACAT.Lib.Core.Audit;
using ACAT.Lib.Core.PreferencesManagement;
using ACAT.Lib.Core.UserManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Extension;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace ACAT.Extensions.BCI.Actuators.BCIActuator
{
    /// <summary>
    /// BCI Actuator ID
    /// </summary>
    [DescriptorAttribute("77809D19-F450-4D36-A633-D818400B3D9A",
                            "BCI EEG Actuator",
                            "BCI Actuator")]
    internal class BCIActuator : ActuatorBase, ISupportsPreferences
    {
        /// <summary>
        /// The setting object for the box calibration
        /// </summary>
        public static BCIBoxCalibrationSettings BoxCalibrationSettings = null;

        /// <summary>
        ///  The settings object for the word calibration
        /// </summary>
        public static BCIWordCalibrationSettings WordCalibrationSettings = null;

        /// <summary>
        /// The settings object for the sentence calibration
        /// </summary>
        public static BCISentenceCalibrationSettings SentenceCalibrationSettings = null;

        /// <summary>
        /// The settings object for the keyboard left calibration
        /// </summary>
        public static BCIKeyboardLeftCalibrationSettings KeyboardLeftCalibrationSettings = null;

        /// <summary>
        /// The settings object for the keyboard right calibration
        /// </summary>
        public static BCIKeyboardRightCalibrationSettings KeyboardRightCalibrationSettings = null;

        /// <summary>
        /// The settings file where the mappings typing-calibration are stored
        /// </summary>
        public static BCITypingCalibrationMappings TypingCalibrationMappings = null;

        /// <summary>
        /// BCI device tester object
        /// </summary>
        public BCIDeviceTester _bciDeviceTester = null;

        /// <summary>
        /// Name of the file that stores the settings for
        /// the box calibration
        /// </summary>
        private const String BoxCalibrationSettingsFileName = "BCIBoxCalibrationSettings.xml";

        /// <summary>
        /// Name of the file that stores the settings for
        /// word calibration
        /// </summary>
        private const String WordCalibratioSettingsFileName = "BCIWordCalibrationSettings.xml";

        /// <summary>
        /// Name of the file that stores the settings for
        /// sentence calibration
        /// </summary>
        private const String SentenceCalibratioSettingsFileName = "BCISentenceCalibrationSettings.xml";

        /// <summary>
        /// Name of the file that stores the settings for
        /// left keyboard calibration
        /// </summary>
        private const String KeyboardLeftCalibratioSettingsFileName = "BCIKeyboardLeftCalibrationSettings.xml";

        /// <summary>
        /// Name of the file that stores the settings for
        /// right keyboard calibration
        /// </summary>
        private const String KeyboardRightCalibratioSettingsFileName = "BCIKeyboardRightCalibrationSettings.xml";

        /// <summary>
        /// Name of the file that stores the mappings for typing-calibration
        /// </summary>
        private const String MappingFileName = "BCITypingCalibrationMappings.xml";

        /// <summary>
        /// Name of the file that stores the allowed mappings for typing-calibration
        /// </summary>
        private const String AllowedMappingsAdvanceFilename = "BCITypingCalibrationAllowedMappingsAdvanced.xml";

        /// <summary>
        /// Name of the file that stores the allowed mappings for typing-calibration
        /// </summary>
        private const String AllowedMappingsRestrictedFilename = "BCITypingCalibrationAllowedMappingsRestricted.xml";

        /// <summary>
        /// List of files to copy in the session folder
        /// </summary>
        private List<String> FilesToCopy;

        /// <summary>
        /// Calibration parameters
        /// </summary>
        public Dictionary<BCIScanSections, CalibrationParametersForSection> DictCalibrationParameters;

        /// <summary>
        /// Typing - Calibration mappings
        /// </summary>
        public Dictionary<BCIScanSections, BCIScanSections> DictTypingCalibrationMappings;

        /// <summary>
        /// Data parser object to be used in calibration end repetition
        /// </summary>
        public DataParser DataParserObj;

        /// <summary>
        /// Current calibration mode
        /// </summary>
        public BCIScanSections _currentCalibrationMode;

        /// <summary>
        /// Boolean, true if typing is in progress
        /// </summary>
        public bool _isSessionInProgress;

        /// <summary>
        /// Has this object been disposed?
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// The input sensor that detects
        /// input swtich activity.
        /// </summary>
        private InputSensor _sensor;

        /// <summary>
        /// List to store average alpha values
        /// </summary>
        private List<double> avgAlphaValues;

        /// <summary>
        /// epoch Idx, used for dummy sensor
        /// </summary>
        private int epochIdx;

        /// <summary>
        /// Flag to resume actuator
        /// </summary>
        private bool onResumeFlagActivated;

        /// <summary>
        /// sequence Idx, used for
        /// </summary>
        private int seqIdx;

        /// <summary>
        /// ID for the session.
        /// Every new calibration or typing is considered a session.
        /// </summary>
        private string sessionID;

        /// <summary>
        /// Use hw sensor (true) or dummy sensor (false)
        /// This parameter is read from BCISettings
        /// </summary>
        private bool useSensor;

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public BCIActuator()
        {
            try
            {
                // Load settings
                BCIActuatorSettings.Load();

                DictCalibrationParameters = new Dictionary<BCIScanSections, CalibrationParametersForSection>();

                BCIBoxCalibrationSettings.SettingsFilePath = UserManager.GetFullPath(BoxCalibrationSettingsFileName);
                BoxCalibrationSettings = BCIBoxCalibrationSettings.Load();
                DictCalibrationParameters.Add(BCIScanSections.Box, new CalibrationParametersForSection(BCIScanSections.Box, BoxCalibrationSettings.ScanTime, BoxCalibrationSettings.NumberOfTargets, BoxCalibrationSettings.NumberOfIterationsPerTarget, BoxCalibrationSettings.MinimumScoreRequired));

                BCIWordCalibrationSettings.SettingsFilePath = UserManager.GetFullPath(WordCalibratioSettingsFileName);
                WordCalibrationSettings = BCIWordCalibrationSettings.Load();
                DictCalibrationParameters.Add(BCIScanSections.Word, new CalibrationParametersForSection(BCIScanSections.Word, WordCalibrationSettings.ScanTime, WordCalibrationSettings.NumberOfTargets, WordCalibrationSettings.NumberOfIterationsPerTarget, WordCalibrationSettings.MinimumScoreRequired));

                BCISentenceCalibrationSettings.SettingsFilePath = UserManager.GetFullPath(SentenceCalibratioSettingsFileName);
                SentenceCalibrationSettings = BCISentenceCalibrationSettings.Load();
                DictCalibrationParameters.Add(BCIScanSections.Sentence, new CalibrationParametersForSection(BCIScanSections.Sentence, SentenceCalibrationSettings.ScanTime, SentenceCalibrationSettings.NumberOfTargets, SentenceCalibrationSettings.NumberOfIterationsPerTarget, SentenceCalibrationSettings.MinimumScoreRequired));

                BCIKeyboardLeftCalibrationSettings.SettingsFilePath = UserManager.GetFullPath(KeyboardLeftCalibratioSettingsFileName);
                KeyboardLeftCalibrationSettings = BCIKeyboardLeftCalibrationSettings.Load();
                DictCalibrationParameters.Add(BCIScanSections.KeyboardL, new CalibrationParametersForSection(BCIScanSections.KeyboardL, KeyboardLeftCalibrationSettings.ScanTime, KeyboardLeftCalibrationSettings.NumberOfTargets, KeyboardLeftCalibrationSettings.NumberOfIterationsPerTarget, KeyboardLeftCalibrationSettings.MinimumScoreRequired));

                BCIKeyboardRightCalibrationSettings.SettingsFilePath = UserManager.GetFullPath(KeyboardRightCalibratioSettingsFileName);
                KeyboardRightCalibrationSettings = BCIKeyboardRightCalibrationSettings.Load();
                DictCalibrationParameters.Add(BCIScanSections.KeyboardR, new CalibrationParametersForSection(BCIScanSections.KeyboardR, KeyboardRightCalibrationSettings.ScanTime, KeyboardRightCalibrationSettings.NumberOfTargets, KeyboardRightCalibrationSettings.NumberOfIterationsPerTarget, KeyboardRightCalibrationSettings.MinimumScoreRequired, KeyboardRightCalibrationSettings.UseRandomTargetsFlag, KeyboardRightCalibrationSettings.Sequence));

                BCITypingCalibrationMappings.SettingsFilePath = UserManager.GetFullPath(MappingFileName);
                TypingCalibrationMappings = BCITypingCalibrationMappings.Load();

                // Create list of files to copy to the session folder
                FilesToCopy = new List<String> { BCIActuatorSettings.SettingsFileName, BoxCalibrationSettingsFileName, WordCalibratioSettingsFileName, SentenceCalibratioSettingsFileName, KeyboardLeftCalibratioSettingsFileName, KeyboardRightCalibratioSettingsFileName, MappingFileName };
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
            }
        }

        /// <summary>
        /// Compare current available channels with channels from a classifier
        /// </summary>
        /// <returns></returns>
        private bool AreChannelsEqual(bool[] currentChannels, int[] classifierChannels)
        {
            bool equalChannels = false;

            if (currentChannels == null || classifierChannels == null)
                return false;
            try
            {
                bool[] classifierChannelsBool = new bool[currentChannels.Length];
                for (int i = 0; i < classifierChannels.Length; i++)
                    classifierChannelsBool[classifierChannels[i] - 1] = true; //channels indexed from 0-numChannels

                equalChannels = classifierChannelsBool.SequenceEqual(currentChannels);
            }
            catch (Exception e)
            {
                Log.Debug("Exception: " + e.Message);
            }
            return equalChannels;
        }

        /// <summary>
        /// Get a dictionary with all the available classifiers
        /// </summary>
        /// <returns></returns>
        private Dictionary<BCIScanSections, BCIClassifierInfo> GetAvailableClassifiers()
        {
            Dictionary<BCIScanSections, BCIClassifierInfo> availableClassifiers = new Dictionary<BCIScanSections, BCIClassifierInfo>();
            bool[] currentChannels = new bool[] { BCIActuatorSettings.Settings.Classifier_EnableChannel1, BCIActuatorSettings.Settings.Classifier_EnableChannel2, BCIActuatorSettings.Settings.Classifier_EnableChannel3, BCIActuatorSettings.Settings.Classifier_EnableChannel4, BCIActuatorSettings.Settings.Classifier_EnableChannel5, BCIActuatorSettings.Settings.Classifier_EnableChannel6, BCIActuatorSettings.Settings.Classifier_EnableChannel7, BCIActuatorSettings.Settings.Classifier_EnableChannel8, BCIActuatorSettings.Settings.Classifier_EnableChannel9, BCIActuatorSettings.Settings.Classifier_EnableChannel10, BCIActuatorSettings.Settings.Classifier_EnableChannel11, BCIActuatorSettings.Settings.Classifier_EnableChannel12, BCIActuatorSettings.Settings.Classifier_EnableChannel13, BCIActuatorSettings.Settings.Classifier_EnableChannel14, BCIActuatorSettings.Settings.Classifier_EnableChannel15, BCIActuatorSettings.Settings.Classifier_EnableChannel16 };

            //If recheck needed, all classifiers need to be recalibrated
            if (BCISettingsFixed.SignalControl_RecheckNeeded)
            {
                Log.Debug("Signal recheck needed.No available classifiers");
                return availableClassifiers;
            }

            try
            {
                // Load mappings if haven't laoded before
                if (DictTypingCalibrationMappings == null)
                    LoadTypingMappings();

                foreach (BCIScanSections scanSection in Enum.GetValues(typeof(BCIScanSections)))
                {
                    String classifierFileName = BCIActuatorSettings.Settings.Calibration_TrainedClassifiersFilePath + "_" + scanSection;
                    String classifierFilePath = Path.Combine(UserManager.CurrentUserDir, classifierFileName);
                    BCIClassifierStatus classifierStatus = BCIClassifierStatus.NotFound;

                    if (File.Exists(classifierFilePath))
                    {
                        DecisionMaker tmpDecisionMaker = new DecisionMaker(classifierFilePath);

                        // Find if classifier is available and add to dictionary
                        if (tmpDecisionMaker != null && tmpDecisionMaker.TrainedClassifiersObj != null)
                        {
                            // Check if classifier is required
                            bool isRequired = true;
                            if(DictTypingCalibrationMappings!=null)
                                isRequired = DictTypingCalibrationMappings.ContainsValue(scanSection);
                            float auc = -1;

                            // Check if current available channels are different that channels used in calibration (in the classifier)
                            // NOTE: Status set to Expired, in a future release a new status should be created for this condition
                            if (!AreChannelsEqual(currentChannels, tmpDecisionMaker.TrainedClassifiersObj.channelSubset))
                                classifierStatus = BCIClassifierStatus.Expired;
                            // Check if classifier expired
                            else if (DateTime.Now.Subtract(tmpDecisionMaker.TrainedClassifiersObj.calibrationTime).TotalMinutes > BCIActuatorSettings.Settings.Calibration_MaxElapsedTimeToForceRecalibration)
                                classifierStatus = BCIClassifierStatus.Expired;
                            else
                            {
                                classifierStatus = BCIClassifierStatus.Ok;
                                auc = tmpDecisionMaker.TrainedClassifiersObj.meanAUC;
                            }

                            // Add classifier to dictionary
                            BCIClassifierInfo classifierInfo = new BCIClassifierInfo(isRequired, scanSection, classifierStatus, auc);
                            availableClassifiers.Add(scanSection, classifierInfo);
                            Log.Debug("Classifier: " + scanSection + " found | AUC:" + availableClassifiers[scanSection].Auc + " isRequired:" + isRequired + " Status:" + classifierStatus);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
            }
            return availableClassifiers;
        }

        /// <summary>
        /// Load typing-mappings from xml file
        /// </summary>
        private void LoadTypingMappings()
        {
            try
            {
                BCITypingCalibrationMappings.SettingsFilePath = UserManager.GetFullPath(MappingFileName);
                TypingCalibrationMappings = BCITypingCalibrationMappings.Load();

                DictTypingCalibrationMappings = new Dictionary<BCIScanSections, BCIScanSections>();

                DictTypingCalibrationMappings.Add(BCIScanSections.Box, (BCIScanSections)Enum.Parse(typeof(BCIScanSections), TypingCalibrationMappings.BoxCalibrationMapping));
                DictTypingCalibrationMappings.Add(BCIScanSections.Word, (BCIScanSections)Enum.Parse(typeof(BCIScanSections), TypingCalibrationMappings.WordCalibrationMapping));
                DictTypingCalibrationMappings.Add(BCIScanSections.Sentence, (BCIScanSections)Enum.Parse(typeof(BCIScanSections), TypingCalibrationMappings.SentenceCalibrationMapping));
                DictTypingCalibrationMappings.Add(BCIScanSections.KeyboardL, (BCIScanSections)Enum.Parse(typeof(BCIScanSections), TypingCalibrationMappings.KeyboardLCalibrationMapping));
                DictTypingCalibrationMappings.Add(BCIScanSections.KeyboardR, (BCIScanSections)Enum.Parse(typeof(BCIScanSections), TypingCalibrationMappings.KeyboardRCalibrationMapping));

                foreach (KeyValuePair<BCIScanSections, BCIScanSections> mapping in DictTypingCalibrationMappings)
                    Log.Debug("Typing section:" + mapping.Key + " Using classifier:" + mapping.Value);
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
            }
        }

        /// <summary>
        /// Load classifiers
        /// </summary>
        /// <param name="typingMappings"></param>
        /// <returns></returns>
        private bool LoadClassifiers()
        {
            bool missingClassifier = false;

            try
            {
                LoadTypingMappings();
                EEGProcessingGlobals.DecisionMakerDict = new Dictionary<BCIScanSections, DecisionMaker>();
                bool[] currentChannels = new bool[] { BCIActuatorSettings.Settings.Classifier_EnableChannel1, BCIActuatorSettings.Settings.Classifier_EnableChannel2, BCIActuatorSettings.Settings.Classifier_EnableChannel3, BCIActuatorSettings.Settings.Classifier_EnableChannel4, BCIActuatorSettings.Settings.Classifier_EnableChannel5, BCIActuatorSettings.Settings.Classifier_EnableChannel6, BCIActuatorSettings.Settings.Classifier_EnableChannel7, BCIActuatorSettings.Settings.Classifier_EnableChannel8, BCIActuatorSettings.Settings.Classifier_EnableChannel9, BCIActuatorSettings.Settings.Classifier_EnableChannel10, BCIActuatorSettings.Settings.Classifier_EnableChannel11, BCIActuatorSettings.Settings.Classifier_EnableChannel12, BCIActuatorSettings.Settings.Classifier_EnableChannel13, BCIActuatorSettings.Settings.Classifier_EnableChannel14, BCIActuatorSettings.Settings.Classifier_EnableChannel15, BCIActuatorSettings.Settings.Classifier_EnableChannel16 };

                foreach (var typingMapping in DictTypingCalibrationMappings)
                {
                    String classifierFileName = BCIActuatorSettings.Settings.Calibration_TrainedClassifiersFilePath + "_" + typingMapping.Value;
                    String classifierFilePath = Path.Combine(UserManager.CurrentUserDir, classifierFileName);

                    if (File.Exists(classifierFilePath))
                    {
                        Log.Debug("Section: " + typingMapping.Key + " | Loading classifier:" + typingMapping.Value + " from:" + classifierFilePath);
                        DecisionMaker tmpDecisionMaker = new DecisionMaker(classifierFilePath);

                        if (tmpDecisionMaker != null && tmpDecisionMaker.TrainedClassifiersObj != null)
                        {
                            // Set scan time for the section as the one used for calibration
                            DictCalibrationParameters[typingMapping.Key].ScanTime = tmpDecisionMaker.TrainedClassifiersObj.calibrationParameters.ScanTime;

                            // Add classifier to dictionary (if not expired and current channels and calibration channels are the same)
                            if (DateTime.Now.Subtract(tmpDecisionMaker.TrainedClassifiersObj.calibrationTime).TotalMinutes < BCIActuatorSettings.Settings.Calibration_MaxElapsedTimeToForceRecalibration
                             && AreChannelsEqual(currentChannels, tmpDecisionMaker.TrainedClassifiersObj.channelSubset))
                            {
                                EEGProcessingGlobals.DecisionMakerDict.Add(typingMapping.Key, tmpDecisionMaker);

                                Log.Debug(typingMapping.Key.ToString() + " typing section -  Using " + typingMapping.Value.ToString() + " classifier. Status: loaded");

                                var bciLogEntry = new BCILogEntryClassifierLoaded(typingMapping.Key.ToString(), typingMapping.Value.ToString(), tmpDecisionMaker.TrainedClassifiersObj.trainedClassifiersSessionID, tmpDecisionMaker.TrainedClassifiersObj.meanAUC,
                                  DictCalibrationParameters[typingMapping.Key].ScanTime, DictCalibrationParameters[typingMapping.Key].TargetCount, DictCalibrationParameters[typingMapping.Key].IterationsPerTarget, DictCalibrationParameters[typingMapping.Key].MinimumScoreRequired, !missingClassifier);
                                var jsonString = JsonSerializer.Serialize(bciLogEntry);
                                AuditLog.Audit(new AuditEvent("BCIClassifierLoaded", jsonString));
                            }
                        }
                        else
                        {
                            missingClassifier = true;
                            Log.Debug(typingMapping.Key.ToString() + " typing section -  Using " + typingMapping.Value.ToString() + " classifier. Status: missing!!");
                        }
                    }
                    else
                    {
                        missingClassifier = true;
                        Log.Debug(typingMapping.Key.ToString() + " typing section. Classifier file not found. -  Using " + typingMapping.Value.ToString() + " classifier. Status: missing!!");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
                return false;
            }
            return !missingClassifier;
        }

        public override String OnboardingImageFileName
        {
            get
            {
                return FileUtils.GetImagePath("BCISwitch.png");
            }
        }

        /// <summary>
        /// Gets whether this supports a custom settings dialog
        /// </summary>
        public override bool SupportsPreferencesDialog
        {
            get { return false; }
        }

        /// <summary>
        /// Class factory to create the switch object
        /// </summary>
        /// <returns>the switch object</returns>
        public override IActuatorSwitch CreateSwitch()
        {
            return new SampleActuatorSwitch();
        }

        /// <summary>
        /// Returns the default preferences object
        /// </summary>
        /// <returns>default preferences object</returns>
        public override IPreferences GetDefaultPreferences()
        {
            return PreferencesBase.LoadDefaults<BCISettings>();
        }

        /// <summary>
        /// Returns the preferences object
        /// </summary>
        /// <returns>preferences object</returns>
        public override IPreferences GetPreferences()
        {
            return BCIActuatorSettings.Settings;
        }

        public override IEnumerable<String> GetSupportedKeyboardConfigs()
        {
            return new List<String>() {"TalkApplicationBCIScannerABC"};
        }

        /// <summary>
        /// Initialize resources
        /// </summary>
        /// <returns>true on success, false otherwise</returns>
        public override bool Init()
        {
            Disclaimers.Add("BCI", R.GetString("DisclaimerBCI"));

            CoreGlobals.ACATUserGuideFileName = "ACAT BCI User Guide.pdf";

            useSensor = BCIActuatorSettings.Settings.Testing_UseSensor;

            // perform initialization here.
            _sensor = new InputSensor();

            _sensor.EvtSwitchActivate += sensor_EvtSwitchActivate;
            _sensor.EvtSwitchDeactivate += sensor_EvtSwitchDeactivate;
            _sensor.EvtSwitchTrigger += sensor_EvtSwitchTrigger;

            actuatorState = State.Running;
            OnInitDone();

            return true;
        }

        /// <summary>
        /// Display the BCI onboarding dialogs
        /// </summary>
        /// <returns>true</returns>
        public override bool PostInit()
        {
            WindowActivityMonitor.Pause();

            showDisclaimer();

            TestBCIDevices();
            WindowActivityMonitor.Resume();

            OnPostInitDone();
            if (actuatorState == State.Stopped)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Handles IoctRequest to interact with ACAT
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public override bool IoctlRequest(int opcode, string request)
        {
            bool retVal = false;

            Log.Debug("IoctRequest received: " + (OpCodes)opcode + " | Request: " + request);

            switch (opcode)
            {
                case (int)OpCodes.Init:

                    if (!useSensor)
                    {
                        //For dummy sensor
                        sessionID = "EEG_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    }
                    retVal = true;
                    actuatorState = State.Running;
                    break;

                case (int)OpCodes.CalibrationEndRepetition:
                    OnCalibrationEndRepetition(request);
                    break;

                case (int)OpCodes.CalibrationEnd:
                    OnCalibrationEnd(request);
                    break;

                case (int)OpCodes.TypingEndRepetition:
                    OnTypingRepetitionEnd(request);
                    break;

                case (int)OpCodes.LanguageModelProbabilities:
                    OnLanguageModelProbabilities(request);
                    break;

                case (int)OpCodes.ToggleCalibrationWindow:
                    OnToggleCalibrationWindow();
                    break;

                case (int)OpCodes.CalibrationWindowShow:
                    OnShowCalibrationWindow();
                    break;

                case (int)OpCodes.Pause:
                    OnPause(request);
                    break;

                case (int)OpCodes.HighlightOnOff:
                    OnHighlightOnOff(request);
                    break;

                case (int)OpCodes.RequestParameters:
                    OnRequestParameters(request);
                    break;

                case (int)OpCodes.StartSession:
                    OnSessionStart(request);
                    break;

                case (int)OpCodes.CalibrationEyesClosedRequestParameters:
                    OnCalibrationEyesClosedRequestParameters(request);
                    break;

                case (int)OpCodes.CalibrationEyesClosedSaveParameters:
                    OnCalibrationEyesClosedSaveParameters(request);
                    break;

                case (int)OpCodes.CalibrationEyesClosedIterationEnd:
                    OnCalibrationEyesClosedIterationEnd(request);
                    break;

                case (int)OpCodes.CalibrationEyesClosedEnd:
                    OnCalibrationEyesClosedEnd(request);
                    break;

                case (int)OpCodes.TriggerTestRequestParameters:
                    OnTriggerTestRequestParameters(request);
                    break;

                case (int)OpCodes.TriggerTestSaveParameters:
                    OnTriggerTestSaveParameters(request);
                    break;

                case (int)OpCodes.TriggerTestStart:
                    OnTriggerTestStart(request);
                    break;

                case (int)OpCodes.TriggerTestStop:
                    OnTriggerTestStop(request);
                    break;

                case (int)OpCodes.RequestCalibrationStatus:
                    OnRequestCalibrationStatus(request);
                    break;

                case (int)OpCodes.RequestMapOptions:
                    OnRequestMapOptions(request);
                    break;

                case (int)OpCodes.SendUpdatedMappings:
                    OnSendUpdatedMappings(request);
                    break;

                default:
                    break;
            }

            return retVal;
        }

        /// <summary>
        /// Pause the actuator
        /// </summary>
        public override void Pause()
        {
            actuatorState = State.Paused;
        }

        /// <summary>
        /// Resume the actuator
        /// </summary>
        public override void Resume()
        {
            actuatorState = State.Running;
        }

        /// <summary>
        /// Dispose resources
        /// </summary>
        /// <param name="disposing">true to dispose managed resources</param>
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                try
                {
                    Log.Debug();

                    if (disposing)
                    {
                        // release managed resources
                        unInit();
                    }

                    // Release the native unmanaged resources
                    _disposed = true;
                }
                finally
                {
                    // Call Dispose on your base class.
                    base.Dispose(disposing);
                }
            }
        }

        /// <summary>
        /// Handler for BCI device testing form closed
        /// Same functionality as SignalForm_FormClosed() function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        // private void bciDeviceTestingCompleted(object sender, System.Windows.Forms.FormClosedEventArgs e)
        private void bciDeviceTestingCompleted()
        {
            // Set actuatorState based on BCIDeviceTester._ExitOnboardingEarly flag
            actuatorState = (BCIDeviceTester.ExitOnboardingEarly) ? State.Stopped : State.Running;
            Log.Debug("\nbciDeviceTestingCompleted | actuatorState: " + actuatorState.ToString());

            SendIoctlResponse((int)OpCodes.CalibrationWindowClose, String.Empty);
            Log.Debug("IoctRequest " + OpCodes.CalibrationWindowClose + " sent. Message: empty");
        }

        /// <summary>
        /// Find the switch that deals with the input detected
        /// </summary>
        /// <param name="switchSource">The source name of the switch</param>
        /// <returns>Switch object, null if not found</returns>
        private IActuatorSwitch find(String switchSource)
        {
            foreach (IActuatorSwitch switchObj in Switches)
            {
                if (switchObj is SampleActuatorSwitch)
                {
                    var actuatorSwitch = (SampleActuatorSwitch)switchObj;
                    if (actuatorSwitch.Source == switchSource)
                    {
                        return new SampleActuatorSwitch(switchObj);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Request parameters for trigger test
        /// </summary>
        /// <param name="request"></param>
        private void OnTriggerTestRequestParameters(String request)
        {
            Log.Debug("TriggerTest parameters requested");

            // Send parameters to ACAT
            var bciTriggerTestParameters = new BCITriggerTestParameters(BCIActuatorSettings.Settings.TriggerTest_NumRepetitions, BCIActuatorSettings.Settings.TriggerTest_ScanTime);
            var str = JsonSerializer.Serialize(bciTriggerTestParameters);
            Log.Debug(" Sending parameters. Scan time: " + bciTriggerTestParameters.ScanTime + " | Num repetitions: " + bciTriggerTestParameters.NumRepetitions);
            SendIoctlResponse((int)OpCodes.TriggerTestSendParameters, str);
            Log.Debug("IoctRequest " + OpCodes.TriggerTestSendParameters + " sent. Message: empty");
        }

        /// <summary>
        /// Request Calibration Status
        /// </summary>
        /// <param name="request"></param>
        private void OnRequestCalibrationStatus(String request)
        {
            Log.Debug("Request Calibration Status received");

            BCIError error;
            BCIClassifierStatus overallStatus = BCIClassifierStatus.NotFound;
            Dictionary<BCIScanSections, BCIClassifierInfo> DictClassifierInfo = new Dictionary<BCIScanSections, BCIClassifierInfo>();
            bool areMoreClassifiersThanMapping = false;
            bool showOnlyDefaults = true;

            try
            {
                // Load typing-mappihngs (will reload in case those changed)
                LoadTypingMappings();

                // Load classifiers
                LoadClassifiers();

                // Get available classifiers
                Dictionary<BCIScanSections, BCIClassifierInfo> availableClassifiers = GetAvailableClassifiers();

                // Start status as Ok, if classifier isRequired and status=NotFound/Required, overall status will be changed to that
                overallStatus = BCIClassifierStatus.Ok;

                foreach (BCIScanSections calibrationSection in DictTypingCalibrationMappings.Keys)
                {
                    BCIClassifierInfo classifierInfo;

                    // Check if required
                    bool isRequired = DictTypingCalibrationMappings.ContainsValue(calibrationSection);

                    // Check if classifier is available
                    if (availableClassifiers.Keys.Contains(calibrationSection))
                        classifierInfo = availableClassifiers[calibrationSection];
                    else
                        classifierInfo = new BCIClassifierInfo(isRequired, calibrationSection, BCIClassifierStatus.NotFound, -1);

                    // Add classifier to dictionary
                    DictClassifierInfo.Add(calibrationSection, classifierInfo);
                    Log.Debug("Calibration:" + calibrationSection + " isRequired:" + classifierInfo.IsRequired + " Status:" + classifierInfo.ClassifierStatus.ToString() + " Auc:" + classifierInfo.Auc);

                    // Set overall status
                    if (classifierInfo.IsRequired && classifierInfo.ClassifierStatus != BCIClassifierStatus.Ok)
                        overallStatus = classifierInfo.ClassifierStatus;

                    // Set are more classifiers than mapping
                    if (classifierInfo.ClassifierStatus == BCIClassifierStatus.Ok && !classifierInfo.IsRequired)
                        areMoreClassifiersThanMapping = true;

                    if (!BCISettingsFixed.Classifier_DefaultClassifiers.Contains(calibrationSection))
                    {
                        if (showOnlyDefaults) // if one of the optional classifiers set showOnlyDefaults to false, keep it false
                        {
                            // showOnlyDefaults = true if NotRequired (not in typing-mapping file ) AND one of the additional calibrations is done
                            if (!isRequired && availableClassifiers.ContainsKey(calibrationSection) && availableClassifiers[calibrationSection].ClassifierStatus != BCIClassifierStatus.Ok)
                                showOnlyDefaults = true;

                            //showOnlyDefaults = false if Required (in typing-mapping file) OR calibration present
                            if (isRequired || availableClassifiers.ContainsKey(calibrationSection) && availableClassifiers[calibrationSection].ClassifierStatus == BCIClassifierStatus.Ok)
                                showOnlyDefaults = false;
                        }

                        Log.Debug("Additional classifier:" + calibrationSection + "Set showOnlyDefaults to:" + showOnlyDefaults);
                    }
                }
                error = new BCIError(BCIErrorCodes.Status_Ok, BCIMessages.Status_Ok);
            }
            catch (Exception e)
            {
                error = new BCIError(BCIErrorCodes.CalibrationError_LoadingClassifiers, BCIMessages.ClassifiersNotLoadedError);
                Log.Debug("Error " + BCIErrorCodes.CalibrationError_LoadingClassifiers.ToString() + " " + "Excepction: " + e.Message);
            }

            // Set oKToGoToTyping status
            bool okToGoToTyping = overallStatus == BCIClassifierStatus.Ok;

            // Send parameters to ACAT
            var bciCalibrationStatus = new BCICalibrationStatus(showOnlyDefaults, areMoreClassifiersThanMapping, okToGoToTyping, overallStatus, DictClassifierInfo, error);
            var str = JsonSerializer.Serialize(bciCalibrationStatus);
            Log.Debug("Sending Calibration Status. ShowOnlyDefaults:" + showOnlyDefaults + " | areMoreClassifiersThanMappings:" + areMoreClassifiersThanMapping + " | okToGoToTyping:" + okToGoToTyping + " | OverallStatus:" + overallStatus.ToString());
            SendIoctlResponse((int)OpCodes.SendCalibrationStatus, str);
            Log.Debug("IoctRequest: " + OpCodes.SendCalibrationStatus + " sent. Message: " + str);
        }

        /// <summary>
        /// Request Map Options
        /// </summary>
        /// <param name="request"></param>
        private void OnRequestMapOptions(String request)
        {
            Log.Debug("Request Map options received");

            BCIError error;

            Dictionary<BCIScanSections, List<BCIClassifierInfo>> DictClassifierInfoForAvailableMappings = new Dictionary<BCIScanSections, List<BCIClassifierInfo>>();

            try
            {
                Dictionary<BCIScanSections, List<BCIScanSections>> DictAllowedMappings = new Dictionary<BCIScanSections, List<BCIScanSections>>();

                // Load classifiers
                LoadClassifiers();

                // Get available classifiers
                Dictionary<BCIScanSections, BCIClassifierInfo> availableClassifiers = GetAvailableClassifiers();

                // Read allowed mappings
                if (BCIActuatorSettings.Settings.Calibration_UseAdvanceModeForTypingMappings)
                {
                    // Read settings file
                    BCITypingCalibrationAllowedMappingsAdvanced.SettingsFilePath = UserManager.GetFullPath(AllowedMappingsAdvanceFilename);
                    var allowedTypingMappings = BCITypingCalibrationAllowedMappingsAdvanced.Load();

                    ///File up dictionary with info from settings
                    DictAllowedMappings.Add(BCIScanSections.Box, allowedTypingMappings.Box.Select(x => (BCIScanSections)Enum.Parse(typeof(BCIScanSections), x)).ToList());
                    DictAllowedMappings.Add(BCIScanSections.Word, allowedTypingMappings.Word.Select(x => (BCIScanSections)Enum.Parse(typeof(BCIScanSections), x)).ToList());
                    DictAllowedMappings.Add(BCIScanSections.Sentence, allowedTypingMappings.Sentence.Select(x => (BCIScanSections)Enum.Parse(typeof(BCIScanSections), x)).ToList());
                    DictAllowedMappings.Add(BCIScanSections.KeyboardL, allowedTypingMappings.KeyboardL.Select(x => (BCIScanSections)Enum.Parse(typeof(BCIScanSections), x)).ToList());
                    DictAllowedMappings.Add(BCIScanSections.KeyboardR, allowedTypingMappings.KeyboardR.Select(x => (BCIScanSections)Enum.Parse(typeof(BCIScanSections), x)).ToList());
                }
                else
                {
                    // Read settings file
                    BCITypingCalibrationAllowedMappingsRestricted.SettingsFilePath = UserManager.GetFullPath(AllowedMappingsRestrictedFilename);
                    var allowedTypingMappings = BCITypingCalibrationAllowedMappingsRestricted.Load();

                    // File up dictionary with info from settings
                    DictAllowedMappings.Add(BCIScanSections.Box, allowedTypingMappings.Box.Select(x => (BCIScanSections)Enum.Parse(typeof(BCIScanSections), x)).ToList());
                    DictAllowedMappings.Add(BCIScanSections.Word, allowedTypingMappings.Word.Select(x => (BCIScanSections)Enum.Parse(typeof(BCIScanSections), x)).ToList());
                    DictAllowedMappings.Add(BCIScanSections.Sentence, allowedTypingMappings.Sentence.Select(x => (BCIScanSections)Enum.Parse(typeof(BCIScanSections), x)).ToList());
                    DictAllowedMappings.Add(BCIScanSections.KeyboardL, allowedTypingMappings.KeyboardL.Select(x => (BCIScanSections)Enum.Parse(typeof(BCIScanSections), x)).ToList());
                    DictAllowedMappings.Add(BCIScanSections.KeyboardR, allowedTypingMappings.KeyboardR.Select(x => (BCIScanSections)Enum.Parse(typeof(BCIScanSections), x)).ToList());
                }

                // For every section, find list of available classifiers
                foreach (BCIScanSections typingSection in DictAllowedMappings.Keys)
                {
                    List<BCIClassifierInfo> availableClassifiersForSection = new List<BCIClassifierInfo>();

                    // Find classifier for section
                    foreach (BCIScanSections availableClassifierForSection in DictAllowedMappings[typingSection])
                    {
                        // Add classifier if exists and not expired
                        if (availableClassifiers.ContainsKey(availableClassifierForSection) && availableClassifiers[availableClassifierForSection].ClassifierStatus == BCIClassifierStatus.Ok)
                        {
                            availableClassifiersForSection.Add(availableClassifiers[availableClassifierForSection]);
                            Log.Debug("Section:" + typingSection + " Available classifier:" + availableClassifierForSection + " Auc:" + availableClassifiers[availableClassifierForSection].Auc);
                        }
                        else
                            Log.Debug("Section:" + typingSection + " Classsifier:" + DictAllowedMappings[typingSection] + " is not available");
                    }
                    // Add classifier found in dictionary
                    DictClassifierInfoForAvailableMappings.Add(typingSection, availableClassifiersForSection);
                }
                error = new BCIError(BCIErrorCodes.Status_Ok, BCIMessages.Status_Ok);
            }
            catch (Exception e)
            {
                error = new BCIError(BCIErrorCodes.CalibrationError_LoadingClassifiers, BCIMessages.ClassifiersNotLoadedError);
                Log.Debug("Error " + BCIErrorCodes.CalibrationError_LoadingClassifiers.ToString() + " " + "Excepction: " + e.Message);
            }

            // Send response to ACAT
            var bciMapOptions = new BCIMapOptions(BCIActuatorSettings.Settings.Calibration_UseAdvanceModeForTypingMappings, DictClassifierInfoForAvailableMappings, DictTypingCalibrationMappings, error);
            var str = JsonSerializer.Serialize(bciMapOptions);
            Log.Debug("Sending map options. Is advanced: " + bciMapOptions.IsAdvanced + " | error: " + (BCIErrorCodes)error.ErrorCode);
            SendIoctlResponse((int)OpCodes.SendMapOptions, str);
            Log.Debug("IoctRequest " + OpCodes.SendMapOptions + " sent. Message: " + str);
        }

        /// <summary>
        /// Send updated mapppings
        /// </summary>
        /// <param name="request"></param>
        private void OnSendUpdatedMappings(String request)
        {
            Log.Debug("Send Updated mappings called");
            try
            {
                var bciUpdatedMappings = JsonSerializer.Deserialize<BCICalibrationUpdatedMappings>(request);
                var dictUpdatedMappings = bciUpdatedMappings.DictUpdatedMappings;

                if (dictUpdatedMappings != null && dictUpdatedMappings.Count > 0)
                {
                    foreach (KeyValuePair<BCIScanSections, BCIScanSections> mappingForTypingSection in dictUpdatedMappings)
                    {
                        // Update mappings accordingly
                        switch (mappingForTypingSection.Key)
                        {
                            case BCIScanSections.Box:
                                TypingCalibrationMappings.BoxCalibrationMapping = mappingForTypingSection.Value.ToString();
                                break;

                            case BCIScanSections.Word:
                                TypingCalibrationMappings.WordCalibrationMapping = mappingForTypingSection.Value.ToString();
                                break;

                            case BCIScanSections.Sentence:
                                TypingCalibrationMappings.SentenceCalibrationMapping = mappingForTypingSection.Value.ToString();
                                break;

                            case BCIScanSections.KeyboardL:
                                TypingCalibrationMappings.KeyboardLCalibrationMapping = mappingForTypingSection.Value.ToString();
                                break;

                            case BCIScanSections.KeyboardR:
                                TypingCalibrationMappings.KeyboardRCalibrationMapping = mappingForTypingSection.Value.ToString();
                                break;
                        }
                        Log.Debug("Typing section:" + mappingForTypingSection.Key.ToString() + " Classifier used:" + mappingForTypingSection.Value.ToString());
                    }

                    // Save settings
                    TypingCalibrationMappings.Save();
                    Log.Debug("Settings updated and saved");

                    // Reload classifiers
                    LoadClassifiers();
                    Log.Debug("Classifiers re-loaded");
                }
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
            }
        }

        /// <summary>
        /// Save parameters for trigger test
        /// </summary>
        /// <param name="request"></param>
        private void OnTriggerTestSaveParameters(String request)
        {
            Log.Debug("Received OnTriggerTestSaveParameters");

            // Receive parameters from ACAT
            var bciTriggerTestParameters = JsonSerializer.Deserialize<BCITriggerTestParameters>(request);

            // Save parameters to settings
            BCIActuatorSettings.Settings.TriggerTest_NumRepetitions = bciTriggerTestParameters.NumRepetitions;
            BCIActuatorSettings.Settings.TriggerTest_ScanTime = bciTriggerTestParameters.ScanTime;
            BCIActuatorSettings.Settings.Save();

            Log.Debug("Eyes closed parameters (Scan time: " + BCIActuatorSettings.Settings.TriggerTest_ScanTime + " , num repetitions: " + BCIActuatorSettings.Settings.TriggerTest_NumRepetitions + ") received from ACAT and saved to BCISettings file");
        }

        /// <summary>
        /// Start trigger test
        /// </summary>
        /// <param name="request"></param>
        private void OnTriggerTestStart(String request)
        {
            Log.Debug("Trigger test start");
            bool result = DAQ_OpenBCI.TriggerTestStart();
            Log.Debug("Trigger test started");
            SendIoctlResponse((int)OpCodes.TriggerTestStartReady, null);
            Log.Debug("IoctRequest " + OpCodes.TriggerTestStartReady + " sent. Message:null ");
        }

        /// <summary>
        /// Stop triggerTest
        /// </summary>
        /// <param name="request"></param>
        private void OnTriggerTestStop(String request)
        {
            Log.Debug("Trigger test stop");

            DAQ_OpenBCI.ExitCodes exitCode = DAQ_OpenBCI.TriggerTestStop(BCIActuatorSettings.Settings.TriggerTest_NumRepetitions, out int numDetectedPulse, out List<double> dutyCycleList, out double dutyCycleAvg);

            bool triggerTestSuccesful = false;
            if (exitCode == DAQ_OpenBCI.ExitCodes.PHOTOSENSOR_STATUS_OK)
                triggerTestSuccesful = true;

            // Send parameters to ACAT
            var bciTriggerTest = new BCITriggerTestResult(triggerTestSuccesful, dutyCycleList, dutyCycleAvg);
            var str = JsonSerializer.Serialize(bciTriggerTest);
            Log.Debug("Sending trigger test results. TriggerTestSuccesful: " + triggerTestSuccesful + " | dutyCycleAvg: " + dutyCycleAvg + " | dutyCycle for individual pulses: " + dutyCycleList.ToArray().ToString());
            SendIoctlResponse((int)OpCodes.TriggerTestResult, str);
            Log.Debug("IoctRequest " + OpCodes.TriggerTestResult + " sent. Message: " + str);
        }

        /// <summary>
        /// Request parameters for calibration of eyes closed
        /// </summary>
        /// <param name="request"></param>
        private void OnCalibrationEyesClosedRequestParameters(String request)
        {
            Log.Debug("Requesting calibration for eyes close parameters");
            // Send parameters to ACAT
            var bciCalibrationEyesClosedParameters = new BCICalibrationEyesClosedParameters(BCIActuatorSettings.Settings.EyesClosedCalibration_NumRepetitions, BCIActuatorSettings.Settings.EyesClosedCalibration_IntervalDuration);
            var str = JsonSerializer.Serialize(bciCalibrationEyesClosedParameters);
            Log.Debug("Sending eyes close parameters. Num repetitions: " + bciCalibrationEyesClosedParameters.NumRepetitions + " | Interval duration: " + bciCalibrationEyesClosedParameters.IntervalDuration);
            SendIoctlResponse((int)OpCodes.CalibrationEyesClosedSendParameters, str);
            Log.Debug("IoctRequest " + OpCodes.CalibrationEyesClosedSendParameters + " sent. Message: " + str);
        }

        /// <summary>
        /// Start calibration of eyes open / close
        /// </summary>
        /// <param name="request"></param>
        private void OnCalibrationEyesClosedSaveParameters(String request)
        {
            Log.Debug("Calibration eyes closed save parameters called");

            // Receive parameters from ACAT
            var bciCalibrationEyesClosedParameters = JsonSerializer.Deserialize<BCICalibrationEyesClosedParameters>(request);

            // Save parameters to settings
            BCIActuatorSettings.Settings.EyesClosedCalibration_IntervalDuration = bciCalibrationEyesClosedParameters.IntervalDuration;
            BCIActuatorSettings.Settings.EyesClosedCalibration_NumRepetitions = bciCalibrationEyesClosedParameters.NumRepetitions;
            BCIActuatorSettings.Settings.Save();

            Log.Debug("Eyes closed parameters (Interval duration: " + BCIActuatorSettings.Settings.EyesClosedCalibration_IntervalDuration + " , num repetitions: " + BCIActuatorSettings.Settings.EyesClosedCalibration_NumRepetitions + ") received from ACAT and saved to BCISettings file");
        }

        /// <summary>
        /// Request parameters for calibration of eyes closed
        /// </summary>
        /// <param name="request"></param>
        private void OnCalibrationEyesClosedIterationEnd(String request)
        {
            if (useSensor)
            {
                try
                {
                    var bciEyesClosedIterationEnd = JsonSerializer.Deserialize<BCICalibrationEyesClosedIterationEnd>(request);

                    // Get data from sensor (it automatically saves it into a file)
                    DAQ_OpenBCI.GetData();
                    bool eyesClosedDetected = DAQ_OpenBCI.DetectEyesClosed(out double[] alphaValues, out double avgAlpha, out double[] betaValues, out double avgBeta);

                    // Log results
                    var bciLogEntry = new BCILogEntryEyesClosed(BCIActuatorSettings.Settings.EyesClosed_EnableDetection, 0, eyesClosedDetected, alphaValues, betaValues, avgAlpha, avgBeta, bciEyesClosedIterationEnd.BciEyesClosedMode.ToString());
                    var jsonString = JsonSerializer.Serialize(bciLogEntry);
                    AuditLog.Audit(new AuditEvent("BCIEyesClosedCalibration", jsonString));
                    Log.Debug("Saved to audit file:" + jsonString);
                }
                catch (Exception e)
                {
                    Log.Debug(e.Message);
                }
            }
        }

        /// <summary>
        /// End calibration eyes closed
        /// </summary>
        /// <param name="request"></param>
        private void OnCalibrationEyesClosedEnd(String request)
        {
            Log.Debug("Calibration eyes closed ended");

            if (useSensor)
            {
                try
                {
                    // Delay to ensure enough data is collected
                    Thread.Sleep(2000);

                    // Get all the data (it automacially saves it in the file)
                    DAQ_OpenBCI.GetData();

                    // End the esssion
                    DAQ_OpenBCI.EndSession();

                    Log.Debug("Session ended");
                }
                catch (Exception e)
                {
                    Log.Debug(e.Message);
                }
            }
        }

        /// <summary>
        /// Handler for when a calibration  ends
        /// It analyzies the calibration data, trains the classifiers
        /// and return the results (auc) to ACAT (via IoctCode)
        /// </summary>
        /// <param name="request"></param>
        private void OnCalibrationEnd(String request)
        {
            Log.Debug("Calibartion End Received");

            var bciCalibrationEnd = JsonSerializer.Deserialize<BCICalibrationEnd>(request);

            float auc = 0f;
            BCIError error = new BCIError(BCIErrorCodes.Status_Ok, BCIMessages.Status_Ok);
            bool calibrationSuccesful = false;

            if (useSensor)
            {
                try
                {
                    // Get all the data and stop sensor
                    Thread.Sleep(2000); // Delay to ensure enough data is collected
                    DAQ_OpenBCI.GetData();
                    DAQ_OpenBCI.EndSession();

                    // Train classifiers and estimate threshold for eyes closed detection
                    var FeatureExtractionObj = new FeatureExtraction(bciCalibrationEnd.FlashingSequence, DictCalibrationParameters[_currentCalibrationMode]);

                    // Eyes closed estimation
                    if (!BCIActuatorSettings.Settings.EyesClosed_UseFixThreshold)
                    {
                        BCIActuatorSettings.Settings.EyesClosed_AdaptiveThreshold = FeatureExtractionObj.LearnEyesClosedAdaptiveThreshold(avgAlphaValues);
                        BCIActuatorSettings.Save();
                        DAQ_OpenBCI.SetEyesClosedAdaptiveThreshold(BCIActuatorSettings.Settings.EyesClosed_AdaptiveThreshold);
                    }
                    auc = FeatureExtractionObj.Learn(sessionID); // will return -1 if error when training classifiers

                    if (auc * 100 >= DictCalibrationParameters[_currentCalibrationMode].MinimumScoreRequired)
                        calibrationSuccesful = true;

                    Log.Debug("Session: " + sessionID + " Calibrated succesfully: " + calibrationSuccesful + " - AUC: " + auc);

                    if (auc == -1)// Error when training classifiers
                    {
                        Log.Debug("Error when training classifiers");
                        error = new BCIError(BCIErrorCodes.CalibrationError_OnAnalyzingData_TrainingClassifiersError, BCIMessages.CalibrationError_CalibrationFailed);
                    }
                }
                catch (Exception e)
                {
                    auc = -1;
                    Log.Debug(e.Message);
                    error = new BCIError(BCIErrorCodes.CalibrationError_OnAnalyzingData_UnknownException, BCIMessages.CalibrationError_CalibrationFailed);
                }
            }
            else
            {
                // For Debugging / Testing: calibrate from old file when selected from settings
                if (BCIActuatorSettings.Settings.Testing_ForceRecalibrateFromFile && BCIActuatorSettings.Settings.Testing_CalibrationFileId != null)
                {
                    auc = RecalibrateFromFile();
                    Log.Debug("Recalibrated from file. AUC: " + auc);
                }
                calibrationSuccesful = true;
            }

            _isSessionInProgress = false;

            // Send auc to ACAT
            var bciCalibrationResult = new BCICalibrationResult(auc, calibrationSuccesful, error);
            var str = JsonSerializer.Serialize(bciCalibrationResult);
            Log.Debug("Sending response. Calibration succesful: " + calibrationSuccesful + " | AUC: " + auc);
            SendIoctlResponse((int)OpCodes.CalibrationResult, str);
            Log.Debug("IoctRequest " + OpCodes.CalibrationResult + " sent. Message: " + str);
        }

        /// <summary>
        /// Handler after a calibration repetition (highlights of all rows and columns),
        /// processes the data and detects sensor errors
        /// </summary                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            >
        /// <param name="request"></param>
        private void OnCalibrationEndRepetition(String request)
        {
            Log.Debug("Calibration end repetition received");

            BCIError sensorError = new BCIError(BCIErrorCodes.Status_Ok, BCIMessages.Status_Ok);
            SignalStatus statusSignal = SignalStatus.SIGNAL_KO;
            int numTriggerPulsesExpected = 0;
            int numTriggerPulsesDetected = 0;
            _isSessionInProgress = true;

            if (useSensor)
            {
                var bciCalibrationInput = JsonSerializer.Deserialize<BCICalibrationInput>(request);
                if (bciCalibrationInput != null)
                {
                    try
                    {
                        if (BCIActuatorSettings.Settings.DAQ_DelayAfterCalibrationRepetition > 0)
                            Thread.Sleep(BCIActuatorSettings.Settings.DAQ_DelayAfterCalibrationRepetition); // Delay added to have same behavior as typing

                        double[,] allData = DAQ_OpenBCI.GetData();

                        if (allData == null || allData.Length == 0)
                            sensorError = new BCIError(BCIErrorCodes.SensorError_DataNotReceived, BCIMessages.SensorError);
                        else
                        {
                            // Write marker values to file
                            DAQ_OpenBCI.WriteMarkerValues2File(bciCalibrationInput.RowColumnIDs);

                            // Get signal status
                            var overallStatus = DAQ_OpenBCI.GetStatus(out SignalStatus[] signalStatus, out SignalStatus opticalSensorStatus);
                            if (overallStatus == SignalStatus.SIGNAL_OK)
                                statusSignal = SignalStatus.SIGNAL_OK;

                            if (DataParserObj == null)
                                DataParserObj = new DataParser(BCISettingsFixed.DAQ_SampleRate, BCIActuatorSettings.Settings.FeatureExtraction_WindowDurationInMs, BCIActuatorSettings.Settings.Calibration_OffsetTarget, null);

                            DataParserObj.ParseDataFromBrainflow(allData, out _, out _, out numTriggerPulsesDetected);

                            numTriggerPulsesExpected = bciCalibrationInput.RowColumnIDs.Count;
                            if (numTriggerPulsesDetected == 0)
                            {
                                Log.Debug("Optical sensor error. No pulses were detected");
                                sensorError = new BCIError(BCIErrorCodes.OpticalSensorError_NoPulsesDetected, BCIMessages.OpticalSensorError);
                            }
                            // Note: Removed code since we can't guarantee the number of pulses received given bluetooth delays
                            //else if (numTriggerPulsesDetected > numTriggerPulsesExpected)
                            //    sensorError = new BCIError(BCIErrorCodes.OpticalSensorError_TooManyPulsesDetected, BCIMessages.OpticalSensorError);
                            //else if (numTriggerPulsesDetected < numTriggerPulsesExpected)
                            //    sensorError = new BCIError(BCIErrorCodes.OpticalSensorError_NotEnoughPulsesDetected, BCIMessages.OpticalSensorError);

                            // Save AlphaValues
                            bool eyesClosedDetected = DAQ_OpenBCI.DetectEyesClosed(out double[] alphaValues, out double avgAlpha, out double[] betaValues, out double avgBeta);
                            avgAlphaValues.Add(avgAlpha);

                            // Log results
                            var bciLogEntry = new BCILogEntryEyesClosed(BCIActuatorSettings.Settings.EyesClosed_EnableDetection, 0, eyesClosedDetected, alphaValues, betaValues, avgAlpha, avgBeta);
                            var jsonString = JsonSerializer.Serialize(bciLogEntry);
                            AuditLog.Audit(new AuditEvent("BCIEyesClosed", jsonString));
                            Log.Debug("Line added to audit file: " + jsonString);
                        }
                    }
                    catch (Exception e)
                    {
                        sensorError = new BCIError(BCIErrorCodes.CalibrationError_UnknwonException, BCIMessages.CalibrationError_CalibrationFailed);
                        Log.Debug(e.Message);
                    }
                }
            }

            if (sensorError.ErrorCode != BCIErrorCodes.Status_Ok)
            {
                String txt = "Error on calibration repetition end (Error: " + sensorError.ErrorCode + " Message: " + sensorError.ErrorMessage + ") Expected pulses: " + numTriggerPulsesExpected + " - Detected pulses: " + numTriggerPulsesDetected;
                Log.Debug(txt);
            }

            // Send result
            var bciCalibrationResult = new BCISensorStatus(sensorError, statusSignal);
            var str = JsonSerializer.Serialize(bciCalibrationResult);
            Log.Debug("Sending response. SensorError:" + (BCIErrorCodes)bciCalibrationResult.Error.ErrorCode);
            SendIoctlResponse((int)OpCodes.CalibrationEndRepetitionResult, str);
            Log.Debug("IoctRequest " + OpCodes.CalibrationEndRepetitionResult + " sent. Message: " + str);
        }

        /// <summary>
        /// Handler for when a highlight is on or off Saves marker (ID) to file
        /// </summary>
        /// <param name="request"></param>
        private void OnHighlightOnOff(String request)
        {
            float marker = float.Parse(request);
            DAQ_OpenBCI.InsertMarker(marker);
        }

        /// <summary>
        /// Called from ACAT (via IoctCode), receives the language model probabilities
        /// and inputs them in the DecisionMaker algorithm to later fuse them with the EEG probabilities
        /// </summary>
        /// <param name="request"></param>
        private void OnLanguageModelProbabilities(String request)
        {
            Log.Debug("Language model probabilities received");

            var bciLanguageModelProbabilities = JsonSerializer.Deserialize<BCILanguageModelProbabilities>(request);
            bool enableLanguageModelprobabilities = false;
            BCIScanSections currentSection = BCIScanSections.None;
            String languageModelProbabilityType = "";
            epochIdx++;

            // Restart classifiers (nextcharacterprobs = clear talk window, resume after pause, etc)
            Log.Debug("Restarting all probabilities");
            EEGProcessingGlobals.RestartAllDecisionMakerProbabilities();

            // Add next character probabilities (flag will be turn on/off if they shouldn't be
            // applied). By doing it this way, next character probabilities will be saved in logs

            switch (bciLanguageModelProbabilities.LanguageModelProbabilityType)
            {
                case ProbabilityType.NextCharacterProbabilities:
                    languageModelProbabilityType = "characters";
                    enableLanguageModelprobabilities = BCIActuatorSettings.Settings.Classifier_UseNextCharacterProbabilities;
                    currentSection = BCIScanSections.KeyboardR;
                    break;

                case ProbabilityType.NextWordProbabilities:
                    enableLanguageModelprobabilities = BCIActuatorSettings.Settings.Classifier_UseNextWordProbabilities;
                    languageModelProbabilityType = "words";
                    currentSection = BCIScanSections.Word;
                    break;

                case ProbabilityType.NextSentenceProbabilities:
                    languageModelProbabilityType = "sentences";
                    break;
            }

            if (bciLanguageModelProbabilities.LanguageModelProbabilities != null)
            {
                if (EEGProcessingGlobals.DecisionMakerDict.ContainsKey(currentSection))
                {
                    EEGProcessingGlobals.DecisionMakerDict[currentSection].AddNextCharacterProbabilities(bciLanguageModelProbabilities.LanguageModelProbabilities);
                    EEGProcessingGlobals.DecisionMakerDict[currentSection].enableLanguageModelProbabilities = enableLanguageModelprobabilities;
                    Log.Debug("Probabilities for " + currentSection + " added to. Use LM for section: " + enableLanguageModelprobabilities);
                }
                else
                {
                    Log.Debug("Probabilities not added");
                }
            }

            // Add LMprobabilities to log file
            var bciLogEntry = new BCILogEntryLanguageModelProbabilitiesReceived(languageModelProbabilityType, bciLanguageModelProbabilities.LanguageModelProbabilities, enableLanguageModelprobabilities);
            var jsonString = JsonSerializer.Serialize(bciLogEntry);
            AuditLog.Audit(new AuditEvent("BCILanguageModelProbabilitiesReceived", jsonString));
            Log.Debug("Line added to audit file: " + jsonString);
        }

        /// <summary>
        /// Actuator paused handler
        /// </summary>
        /// <param name="request"></param>
        private void OnPause(String request)
        {
            if (String.Compare(request, "true", true) == 0)
            {
                // pause the acutator
                onResumeFlagActivated = false;
            }
            else
            {
                // resume the actuator
                //Clear data in actuator
                if (useSensor)
                {
                    DAQ_OpenBCI.GetData();
                    EEGProcessingGlobals.RestartAllDecisionMakerProbabilities();
                    onResumeFlagActivated = true;
                }
            }
        }

        /// <summary>
        /// Handler to request parameters.
        /// It reads the parameters from the settings file
        /// and sends them to ACAT (via IoctCode)
        /// </summary>
        /// <param name="request"></param>
        private void OnRequestParameters(String request)
        {
            bool recalibrationRequired = true;
            float lastCalibrationAUC = 0.99f;
            BCIError error = new BCIError(BCIErrorCodes.Status_Ok, BCIMessages.Status_Ok);

            var bciUserInputParameters = JsonSerializer.Deserialize<BCIUserInputParameters>(request);
            Log.Debug("Request parameters for mode " + bciUserInputParameters.BciMode);

            switch (bciUserInputParameters.BciMode)
            {
                case BCIModes.TYPING:
                    // If typing mode, load classifiers and change scan time to the scan time used in calibration
                    if (!_isSessionInProgress)
                        LoadClassifiers();
                    break;

                case BCIModes.CALIBRATION:
                    Log.Debug("Calibration mode: " + bciUserInputParameters.BciCalibrationMode + " Scan time: " + bciUserInputParameters.ScanTime + " | Number of targets: " + bciUserInputParameters.NumTargets + " | Number of iterations per target: " + bciUserInputParameters.NumIterationsPerTarget + " | Minimum score required: " + bciUserInputParameters.MinScoreRequired);

                    // Update settings for the corresponding section from parameters sent from ACAT
                    switch (bciUserInputParameters.BciCalibrationMode)
                    {
                        case BCIScanSections.Box:
                            BoxCalibrationSettings.ScanTime = bciUserInputParameters.ScanTime;
                            BoxCalibrationSettings.NumberOfTargets = bciUserInputParameters.NumTargets;
                            BoxCalibrationSettings.NumberOfIterationsPerTarget = bciUserInputParameters.NumIterationsPerTarget;
                            BoxCalibrationSettings.MinimumScoreRequired = bciUserInputParameters.MinScoreRequired;
                            BoxCalibrationSettings.Save();
                            Log.Debug("Parameters saved for box calibration");
                            break;

                        case BCIScanSections.Word:
                            WordCalibrationSettings.ScanTime = bciUserInputParameters.ScanTime;
                            WordCalibrationSettings.NumberOfTargets = bciUserInputParameters.NumTargets;
                            WordCalibrationSettings.NumberOfIterationsPerTarget = bciUserInputParameters.NumIterationsPerTarget;
                            WordCalibrationSettings.MinimumScoreRequired = bciUserInputParameters.MinScoreRequired;
                            WordCalibrationSettings.Save();
                            Log.Debug("Parameters saved for calibration");
                            break;

                        case BCIScanSections.Sentence:
                            SentenceCalibrationSettings.ScanTime = bciUserInputParameters.ScanTime;
                            SentenceCalibrationSettings.NumberOfTargets = bciUserInputParameters.NumTargets;
                            SentenceCalibrationSettings.NumberOfIterationsPerTarget = bciUserInputParameters.NumIterationsPerTarget;
                            SentenceCalibrationSettings.MinimumScoreRequired = bciUserInputParameters.MinScoreRequired;
                            SentenceCalibrationSettings.Save();
                            Log.Debug("Parameters saved for sentence calibration");
                            break;

                        case BCIScanSections.KeyboardL:
                            KeyboardLeftCalibrationSettings.ScanTime = bciUserInputParameters.ScanTime;
                            KeyboardLeftCalibrationSettings.NumberOfTargets = bciUserInputParameters.NumTargets;
                            KeyboardLeftCalibrationSettings.NumberOfIterationsPerTarget = bciUserInputParameters.NumIterationsPerTarget;
                            KeyboardLeftCalibrationSettings.MinimumScoreRequired = bciUserInputParameters.MinScoreRequired;
                            KeyboardLeftCalibrationSettings.Save();
                            Log.Debug("Parameters saved for keyboard Left calibration");
                            break;

                        case BCIScanSections.KeyboardR:
                            KeyboardRightCalibrationSettings.ScanTime = bciUserInputParameters.ScanTime;
                            KeyboardRightCalibrationSettings.NumberOfTargets = bciUserInputParameters.NumTargets;
                            KeyboardRightCalibrationSettings.NumberOfIterationsPerTarget = bciUserInputParameters.NumIterationsPerTarget;
                            KeyboardRightCalibrationSettings.MinimumScoreRequired = bciUserInputParameters.MinScoreRequired;
                            KeyboardRightCalibrationSettings.Save();
                            Log.Debug("Paramters saved for keyboard right calibration");
                            break;
                    }

                    // Update parameter
                    DictCalibrationParameters[bciUserInputParameters.BciCalibrationMode].ScanTime = bciUserInputParameters.ScanTime;
                    DictCalibrationParameters[bciUserInputParameters.BciCalibrationMode].TargetCount = bciUserInputParameters.NumTargets;
                    DictCalibrationParameters[bciUserInputParameters.BciCalibrationMode].IterationsPerTarget = bciUserInputParameters.NumIterationsPerTarget;
                    DictCalibrationParameters[bciUserInputParameters.BciCalibrationMode].MinimumScoreRequired = bciUserInputParameters.MinScoreRequired;
                    Log.Debug("Parameters updated in the " + bciUserInputParameters.BciCalibrationMode + " dictionary");
                    break;
            }

            // Send parameters to ACAT
            var bciParameters = new BCIParameters(DictCalibrationParameters, recalibrationRequired, lastCalibrationAUC, BCIActuatorSettings.Settings.Scanning_PauseTime, BCIActuatorSettings.Settings.Scanning_ShortPauseTime, BCIActuatorSettings.Settings.Scanning_ShowDecisionTime, BCIActuatorSettings.Settings.Scanning_DelayAfterDecision, BCIActuatorSettings.Settings.Scanning_DelayToGetReady, BCIActuatorSettings.Settings.Testing_MinimumProbabiltyToDisplayBarOnTyping, BCIActuatorSettings.Settings.Scanning_FocalCircleColor, BCIActuatorSettings.Settings.Scanning_IsFocalCircleFilled, error);
            var str = JsonSerializer.Serialize(bciParameters);
            Log.Debug("Sending parameters. Error:" + (BCIErrorCodes)bciParameters.Error.ErrorCode + " RecalibrationRequired:" + bciParameters.CalibrationRequiredFlag + " | Pause time:" + bciParameters.Scanning_PauseTime + ", Short pause time:" + bciParameters.Scanning_ShortPauseTime + ", Show decision time: " + bciParameters.Scanning_ShowDecisionTime + ", Delay after decision:" + bciParameters.Scanning_DelayAfterDecision + ", Minimum probability to display bars on typing:" + bciParameters.MinProbablityToDisplayBarOnTyping);
            SendIoctlResponse((int)OpCodes.SendParameters, str);
            Log.Debug("IoctRequest " + OpCodes.SendParameters + " sent. Message: " + str);
        }

        /// <summary>
        /// Handler to start a session
        /// </summary>
        /// <param name="request"></param>
        private void OnSessionStart(String request)
        {
            BCIError error = new BCIError(BCIErrorCodes.Status_Ok, BCIMessages.Status_Ok);
            bool sensorReady = false;
            String sessionDirectory = null;
            avgAlphaValues = new List<double>();

            try
            {
                var bciModeObj = JsonSerializer.Deserialize<BCIMode>(request);

                if (bciModeObj != null)
                {
                    Log.Debug("Start session: " + bciModeObj.BciMode);

                    if (useSensor)
                    {
                        // Empty sensor buffer. If device not started, start the device
                        bool deviceStarted = false;
                        _currentCalibrationMode = bciModeObj.BciCalibrationMode;

                        if (DAQ_OpenBCI.IsAcquiring())
                            DAQ_OpenBCI.GetData(); // empty buffer
                        else
                            deviceStarted = DAQ_OpenBCI.Start();

                        if (DAQ_OpenBCI.IsAcquiring() || deviceStarted)
                        {
                            switch (bciModeObj.BciMode)
                            {
                                case BCIModes.CALIBRATION_EYESOPENCLOSE:
                                    sessionID = "EEG_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_CalibrationEyesOpenClose"; // Create new files
                                    break;

                                case BCIModes.CALIBRATION:
                                    sessionID = "EEG_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_Calibration_" + _currentCalibrationMode.ToString(); // Create new files
                                    break;

                                case BCIModes.TYPING:

                                    // For Debugging / Testing: calibrate from old file when selected from settings
                                    if (BCIActuatorSettings.Settings.Testing_ForceRecalibrateFromFile && BCIActuatorSettings.Settings.Testing_CalibrationFileId != null)
                                    {
                                        Log.Debug("Recalibrating from file");
                                        float auc = RecalibrateFromFile();
                                        Log.Debug("Recalibrated from file. AUC: " + auc);
                                    }

                                    if (!LoadClassifiers())
                                        error = new BCIError(BCIErrorCodes.TypingError_ClassifiersNotLoaded, BCIMessages.ClassifiersNotLoadedError);

                                    // Load trained classifier
                                    sessionID = "EEG_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_Typing"; // Create new files
                                    epochIdx = 0;
                                    seqIdx = 0;
                                    break;

                                case BCIModes.TRIGGERTEST:
                                    sessionID = "EEG_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_TriggerTest"; // Create new files
                                    break;
                            }

                            // Start session
                            Log.Debug("Starting session: " + sessionID);
                            sensorReady = DAQ_OpenBCI.StartSession(sessionID, true);
                            sessionDirectory = DAQ_OpenBCI.GetSessionDirectory();
                            _isSessionInProgress = false; // set to true on calibrationEndRepetition and TypingEndRepetition

                            // Copy files to folder
                            string filePathCopyTo, filePathCopyFrom;
                            foreach (String fileName in FilesToCopy)
                            {
                                filePathCopyTo = Path.Combine(sessionDirectory, fileName);
                                filePathCopyFrom = UserManager.GetFullPath(fileName);
                                Log.Debug("Copying files from: " + filePathCopyFrom + " to " + filePathCopyTo);
                                if (!String.IsNullOrEmpty(filePathCopyTo) && !String.IsNullOrEmpty(filePathCopyFrom) && File.Exists(filePathCopyFrom) && Directory.Exists(sessionDirectory))
                                {
                                    File.Copy(filePathCopyFrom, filePathCopyTo);
                                    Log.Debug("FIles copied from: " + filePathCopyFrom + " to " + filePathCopyTo);
                                }
                            }
                        }
                        if (!sensorReady)
                            error = new BCIError(BCIErrorCodes.SensorError_StartSessionFailed, BCIMessages.SensorError);

                        Thread.Sleep(1000);// To make sure all markers are sent correctly
                    }
                    else // dummy sensor
                    {
                        sensorReady = true;
                    }
                }

                // Send start session results to ACAT
                var bciStartSessionResults = new BCIStartSessionResult(sensorReady, sessionDirectory, sessionID, error);
                var str = JsonSerializer.Serialize(bciStartSessionResults);
                Log.Debug("Sending response. Error: " + (BCIErrorCodes)error.ErrorCode + " Sensor ready: " + bciStartSessionResults.SensorReady + " | session ID: " + sessionID + " | directory: " + sessionDirectory);
                SendIoctlResponse((int)OpCodes.StartSessionResult, str);
                Log.Debug("IoctRequest " + OpCodes.StartSessionResult + " sent. Message: " + str);
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
            }
        }

        /// <summary>
        /// Recalibrate from an old file. This is s for testing purposes only.
        /// Better implementation might be required (Eg: save flashingSequence & read it from file, read BCISettings & calibrationParameters from the particular session instead of generic) if used in release.
        /// </summary>
        /// <returns></returns>
        private float RecalibrateFromFile()
        {
            float auc = 0;
            sessionID = BCIActuatorSettings.Settings.Testing_CalibrationFileId;
            FeatureExtraction FeatureExtractionObj;

            try
            {
                String strToFind = "_Calibration_";
                int idxCalibrationType = sessionID.LastIndexOf(strToFind);
                if (idxCalibrationType != -1)
                {
                    string calibrationType = sessionID.Substring(idxCalibrationType + strToFind.Length);
                    Log.Debug("Recalibrating from file " + sessionID + " | Test ID: " + BCIActuatorSettings.Settings.Testing_TestID + " | Calibration type:" + calibrationType);

                    switch (calibrationType.ToLower())
                    {
                        case "box":
                            FeatureExtractionObj = new FeatureExtraction(null, DictCalibrationParameters[BCIScanSections.Box], 0, 4, false);
                            break;

                        case "word":
                            FeatureExtractionObj = new FeatureExtraction(null, DictCalibrationParameters[BCIScanSections.Word], 0, 10, false);
                            break;

                        case "sentence":
                            FeatureExtractionObj = new FeatureExtraction(null, DictCalibrationParameters[BCIScanSections.Sentence], 0, 5, false);
                            break;

                        case "keyboardl":
                            FeatureExtractionObj = new FeatureExtraction(null, DictCalibrationParameters[BCIScanSections.KeyboardL], 4, 3, true);
                            break;

                        case "keyboardr":
                            FeatureExtractionObj = new FeatureExtraction(null, DictCalibrationParameters[BCIScanSections.KeyboardR], 4, 7, true);
                            break;

                        default:
                            FeatureExtractionObj = new FeatureExtraction(null, DictCalibrationParameters[BCIScanSections.KeyboardR], 4, 7, true);
                            break;
                    }
                }
                else
                {
                    switch (BCIActuatorSettings.Settings.Testing_TestID)
                    {
                        case 1:
                            FeatureExtractionObj = new FeatureExtraction(null, null, 6, 6, true);
                            break;

                        case 2:
                            FeatureExtractionObj = new FeatureExtraction(null, null, 0, 5, false);
                            break;

                        case 3:
                            FeatureExtractionObj = new FeatureExtraction(null, null, 0, 5, false);
                            break;

                        case 4:
                            FeatureExtractionObj = new FeatureExtraction(null, null, 6, 6, true);
                            break;

                        case 5:
                            FeatureExtractionObj = new FeatureExtraction(null, null, 4, 7, true);
                            break;

                        default:
                            FeatureExtractionObj = new FeatureExtraction(null, null, 4, 7, true);
                            break;
                    }
                }

                Log.Debug(" Recalibrating file " + sessionID);
                // Train classifiers
                auc = FeatureExtractionObj.Learn(sessionID); // will return -1 if error when training classifiers
                Log.Debug("Recalibrated with AUC: " + auc);
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
            }
            return auc;
        }

        /// <summary>
        /// Handler for when calibration window shown
        /// </summary>
        private void OnShowCalibrationWindow()
        {
            // Explicitly call unit() first - release managed resources
            unInit();

            // Run BCI device testing
            // Pete's redesign 5/23 of device testing and signal display windows
            TestBCIDevices();
        }

        /// <summary>
        /// Handler for when calibration window toggled
        /// </summary>
        private void OnToggleCalibrationWindow()
        {
            /*if (!IsSignalFormVisible())
            {*/
            SendIoctlResponse((int)OpCodes.CalibrationWindowPreShow, String.Empty);
            Log.Debug("IoctRequest " + OpCodes.CalibrationWindowPreShow + " sent. Message: empty");

            //}
        }

        /// <summary>
        ///Handler for when typing repetition (highlight of all rows and columns) ends.
        /// It reads the data from the sensor, sends it to the algorithm and return the results
        /// (probabilities and decision) to ACAT (via IoctCode)
        /// </summary>
        /// <param name="request"></param>
        private void OnTypingRepetitionEnd(String request)
        {
            bool decidedFlag = false;
            int decidedButtonID = 3;
            string decidedButtonLabel = "";
            _isSessionInProgress = true;
            BCIError error = new BCIError(BCIErrorCodes.Status_Ok, BCIMessages.Status_Ok);
            int repetition = 0;
            bool returnToBoxScanningFlag = false;
            bool eyesClosedDetected = false;
            SignalStatus statusSignal = SignalStatus.SIGNAL_OK;

            SortedDictionary<int, double> posteriorProbs = null;
            Dictionary<int, double> eegProbs = null;
            Dictionary<int, double> nextCharProbs = null;
            Dictionary<int, double> posteriorProbsDict = null;

            var bciTypingRepetitionEnd = JsonSerializer.Deserialize<BCITypingRepetitionEnd>(request);
            BCIScanSections currentScanSection = bciTypingRepetitionEnd.ScanningSection;
            Log.Debug("On typing Repetition end called for section: " + bciTypingRepetitionEnd.ScanningSection);

            if (useSensor)
            {
                try
                {
                    Thread.Sleep(BCIActuatorSettings.Settings.DAQ_DelayAfterTypingRepetition);
                    if (DAQ_OpenBCI.IsAcquiring())
                    {
                        double[,] allSamples = DAQ_OpenBCI.GetData();

                        if (allSamples != null && allSamples.Length > 0)
                        {
                            // Write marker values to file
                            DAQ_OpenBCI.WriteMarkerValues2File(bciTypingRepetitionEnd.RowColumnIDs);

                            // Check signal status
                            var overallStatus = DAQ_OpenBCI.GetStatus(out SignalStatus[] signalStatus, out SignalStatus opticalSensorStatus);
                            bool[] availableChannels = new bool[signalStatus.Length];
                            int numKOChannels = 0;
                            for (int channelIdx = 0; channelIdx < signalStatus.Length; channelIdx++)
                            {
                                switch (signalStatus[channelIdx])
                                {
                                    case SignalStatus.SIGNAL_KO:
                                    case SignalStatus.SIGNAL_ERROR:
                                        numKOChannels++;
                                        availableChannels[channelIdx] = false;
                                        break;

                                    case SignalStatus.SIGNAL_OK:
                                    case SignalStatus.SIGNAL_ACCEPTABLE:
                                        availableChannels[channelIdx] = true;
                                        break;
                                }
                            }

                            if (numKOChannels >= 3)
                            {
                                error = new BCIError(BCIErrorCodes.SensorError_BadSignalQuality, BCIMessages.SensorError);
                                statusSignal = SignalStatus.SIGNAL_KO;
                            }

                            if (!BCIActuatorSettings.Settings.DAQ_DisableChannelsAutomatically)
                            {
                                error = new BCIError(BCIErrorCodes.Status_Ok, BCIMessages.Status_Ok);

                                availableChannels = new bool[] { BCIActuatorSettings.Settings.Classifier_EnableChannel1, BCIActuatorSettings.Settings.Classifier_EnableChannel2, BCIActuatorSettings.Settings.Classifier_EnableChannel3, BCIActuatorSettings.Settings.Classifier_EnableChannel4, BCIActuatorSettings.Settings.Classifier_EnableChannel5, BCIActuatorSettings.Settings.Classifier_EnableChannel6, BCIActuatorSettings.Settings.Classifier_EnableChannel7, BCIActuatorSettings.Settings.Classifier_EnableChannel8, BCIActuatorSettings.Settings.Classifier_EnableChannel9, BCIActuatorSettings.Settings.Classifier_EnableChannel10, BCIActuatorSettings.Settings.Classifier_EnableChannel11, BCIActuatorSettings.Settings.Classifier_EnableChannel12, BCIActuatorSettings.Settings.Classifier_EnableChannel13, BCIActuatorSettings.Settings.Classifier_EnableChannel14, BCIActuatorSettings.Settings.Classifier_EnableChannel15, BCIActuatorSettings.Settings.Classifier_EnableChannel16 };

                                //for (int channelIdx = 0; channelIdx < availableChannels.Length; channelIdx++)
                                //    availableChannels[channelIdx] = true;
                            }

                            // Check if eyes open / closed
                            eyesClosedDetected = DAQ_OpenBCI.DetectEyesClosed(out double[] alphaValues, out double avgAlpha, out double[] betaValues, out double avgBeta);

                            // Log values for eyes open /closed
                            var bciLogEntry2 = new BCILogEntryEyesClosed(BCIActuatorSettings.Settings.EyesClosed_EnableDetection, DAQ_OpenBCI.GetEyesClosedThreshold(), eyesClosedDetected, alphaValues, betaValues, avgAlpha, avgBeta);
                            var jsonString2 = JsonSerializer.Serialize(bciLogEntry2);
                            AuditLog.Audit(new AuditEvent("BCIEyesClosed", jsonString2));

                            if (BCIActuatorSettings.Settings.EyesClosed_EnableDetection)
                            {
                                Log.Debug("Eyes closed detected");
                                returnToBoxScanningFlag = eyesClosedDetected;
                                if (eyesClosedDetected)
                                {
                                    EEGProcessingGlobals.RestartAllDecisionMakerProbabilities();
                                    SoundManager.playSound(SoundManager.SoundType.OpenEyes);
                                    Thread.Sleep(BCIActuatorSettings.Settings.EyesClosed_DelayToStartAnimationAfterDetection);
                                }
                            }

                            if (!returnToBoxScanningFlag)
                            {
                                // Reduce and compute posterior probabilities
                                decidedButtonID = 0;
                                decidedFlag = false;
                                Log.Debug("Calculating posterior probabilities");
                                EEGProcessingGlobals.DecisionMakerDict[currentScanSection].ComputePosteriorProbs(allSamples,
                                                                     bciTypingRepetitionEnd.RowColumnIDs,
                                                                     bciTypingRepetitionEnd.FlashingSequence,
                                                                     availableChannels,
                                                                     out decidedFlag,
                                                                     out decidedButtonID,
                                                                     out repetition,
                                                                     out posteriorProbs,
                                                                     out eegProbs,
                                                                     out nextCharProbs);

                                // Save data for log
                                if (posteriorProbs != null)
                                    posteriorProbsDict = posteriorProbs.ToDictionary(x => x.Key, x => x.Value);
                                if (bciTypingRepetitionEnd.ButtonTextValues.ContainsKey(decidedButtonID))
                                    decidedButtonLabel = bciTypingRepetitionEnd.ButtonTextValues[decidedButtonID];

                                // Calculate number of buttons highlighted. Those highlighted are shown in the flashingSequence
                                // NOTE: Don't use buttontextvalues as some buttons might be displayed but not highlighted.
                                List<int> buttonIds = new List<int>();
                                foreach (int[] rowcolumn in bciTypingRepetitionEnd.FlashingSequence.Values)
                                    foreach (int buttonId in rowcolumn)
                                        if (!buttonIds.Contains(buttonId))
                                            buttonIds.Add(buttonId);

                                if (posteriorProbs == null || posteriorProbs.Count == 0)
                                {
                                    //error = new BCIError(BCIErrorCodes.TypingError_OnRepetitionEnd_NoProbabilitiesCalculated, BCIMessages.TypingError); // no probabilities returned. optical sensor dead?
                                    Log.Debug("Probabilities not returned from DeicisionMaker");
                                }
                                else if (posteriorProbs.Count != buttonIds.Count)
                                {
                                    //  error = new BCIError(BCIErrorCodes.TypingError_OnRepetitionEnd_ProabilitiesMarkersMissmatch, BCIMessages.TypingError);
                                    Log.Debug("Number of probabilities different than number of highlights. Num buttons: " + buttonIds.Count + " Num Probabilities: " + posteriorProbs.Count);
                                }

                                Log.Debug("Posterior probabilities calculated. Repetition: " + repetition + " Decided:" + decidedFlag + " DecisionLabel:" + decidedButtonLabel + " DecisionID: " + decidedButtonID + " Probabilities" + eegProbs.ToString());
                            }
                        }
                        else
                        {
                            error = new BCIError(BCIErrorCodes.TypingError_OnRepetitionEnd_DataNotReceived, BCIMessages.TypingError); // no data received, send error
                        }
                    }
                    else
                    {
                        error = new BCIError(BCIErrorCodes.TypingError_OnRepetitionEnd_SensorDisconected, BCIMessages.TypingError); // device not acquiring data. Start?
                    }
                }
                catch (Exception e)
                {
                    error = new BCIError(BCIErrorCodes.TypingError_OnRepetitionEnd_UnknownException, BCIMessages.TypingError);// Error when processing data
                    Log.Debug("Error: " + error.ErrorCode + " Message" + error.ErrorMessage + " Excepcion: " + e.Message);
                }

                // Display error on logs
                if (error.ErrorCode != BCIErrorCodes.Status_Ok)
                    Log.Debug("Error: " + error.ErrorCode + " Message" + error.ErrorMessage);
            }
            else
            {
                // Dummy sensor
                Thread.Sleep(BCIActuatorSettings.Settings.DAQ_DelayAfterTypingRepetition); // To simulate the sensor behavior

                /* Uncomment to test return to box scanning
                Random rndReturnToBoxScanning = new Random();
                if (rndReturnToBoxScanning.Next(1, 2) == 1) //1 of every 3 iterations (on average), will send decidedLabel = true;
                    returnToBoxScanningFlag = false;
                */

                seqIdx++;
                decidedFlag = false;
                decidedButtonID = 1;
                decidedButtonLabel = "a";
                float prob = 0;
                int decidedIdx;
                Random rndprob = new Random();
                posteriorProbs = new SortedDictionary<int, double>();
                Random rndDecision = new Random();
                Random rndDecisionID = new Random();
                float totalProbs = 0;
                foreach (int buttonID in bciTypingRepetitionEnd.FlashingSequence.Keys)
                {
                    if (!posteriorProbs.ContainsKey(buttonID))
                    {
                        prob = rndprob.Next(1, 10) * 0.1f;
                        posteriorProbs.Add(buttonID, prob);
                        totalProbs += prob;
                    }
                }

                // Normalize probabilities
                foreach (int buttonID in bciTypingRepetitionEnd.FlashingSequence.Keys)
                    posteriorProbs[buttonID] = posteriorProbs[buttonID] / totalProbs;

                if (BCIActuatorSettings.Settings.Classifier_MaxDecisionSequences >= 1) //Hack to make decision every maxDecisionSequences
                {
                    if (seqIdx == BCIActuatorSettings.Settings.Classifier_MaxDecisionSequences)
                        decidedFlag = true;
                }
                else //make decision randomly
                {
                    if (rndDecision.Next(1, 4) == 1) //1 of every 4 iterations (on average), will send decidedLabel = true;
                        decidedFlag = true;
                }

                if (decidedFlag)
                {
                    decidedIdx = rndDecisionID.Next(posteriorProbs.Keys.Count);
                    decidedButtonID = posteriorProbs.Keys.ElementAt(decidedIdx);
                    posteriorProbs[decidedButtonID] = rndprob.Next(1, 10) * 0.01f + 0.9;
                    decidedButtonLabel = bciTypingRepetitionEnd.ButtonTextValues[decidedButtonID];
                    seqIdx = 0;
                }

                error = new BCIError(BCIErrorCodes.Status_Ok, BCIMessages.Status_Ok);
            }

            if (onResumeFlagActivated)
                error = new BCIError(BCIErrorCodes.Status_Ok, BCIMessages.Status_Ok);

            if (repetition == 1)
            {
                // Add log entry (only for the first repetition) for new scanning section
                var bciScanningSectionStarted = new BCILogEntrNewScanningSectionStarted(bciTypingRepetitionEnd.ScanningSection.ToString(), bciTypingRepetitionEnd.ButtonTextValues, (EEGProcessingGlobals.DecisionMakerDict[currentScanSection].enableLanguageModelProbabilities && nextCharProbs != null));
                var jsonString2 = JsonSerializer.Serialize(bciScanningSectionStarted);
                AuditLog.Audit(new AuditEvent("BCIScanningSectionStarted", jsonString2));
            }

            // Log results
            var bciLogEntry = new BCILogEntryTypingEnd(decidedButtonLabel, decidedButtonID, decidedFlag, repetition, bciTypingRepetitionEnd.ScanningSection.ToString(),
                bciTypingRepetitionEnd.RowColumnIDs, bciTypingRepetitionEnd.FlashingSequence, nextCharProbs, eegProbs, posteriorProbsDict);
            var jsonString = JsonSerializer.Serialize(bciLogEntry);
            AuditLog.Audit(new AuditEvent("BCIRepetitionEnd", jsonString));
            Log.Debug("Results: " + jsonString);

            // Send results to application
            var bciTypingRepetitionResult = new BCITypingRepetitionResult
            {
                ReturnToBoxScanningFlag = returnToBoxScanningFlag,
                DecidedFlag = decidedFlag,
                DecidedId = decidedButtonID,
                Error = error,
                StatusSignal = statusSignal,
            };

            if (posteriorProbs != null)
                bciTypingRepetitionResult.PosteriorProbs = posteriorProbs;

            var str = JsonSerializer.Serialize(bciTypingRepetitionResult);
            Log.Debug("Sending response. Error: " + (BCIErrorCodes)error.ErrorCode + " | Decided:" + decidedFlag + "DecidedID:" + decidedButtonID + " ReturnToBoxScanning:" + returnToBoxScanningFlag + " StatusSignal: " + statusSignal);
            SendIoctlResponse((int)OpCodes.TypingEndRepetitionResult, str);
            Log.Debug("IoctRequest " + OpCodes.TypingEndRepetitionResult + " sent. Message: " + str);
        }

        /// <summary>
        /// Event handler for when a swtich activate event is detected.
        /// Notify ACAT
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void sensor_EvtSwitchActivate(object sender, InputSensorSwitchEventArgs e)
        {
            if (actuatorState == State.Running)
            {
                IActuatorSwitch actuatorSwitch = find(e.Gesture);
                if (actuatorSwitch != null)
                {
                    OnSwitchActivated(actuatorSwitch);
                }
            }
        }

        /// <summary>
        /// Event handler for when a swtich deactivate event is detected.
        /// Notify ACAT
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void sensor_EvtSwitchDeactivate(object sender, InputSensorSwitchEventArgs e)
        {
            if (actuatorState == State.Running)
            {
                IActuatorSwitch actuatorSwitch = find(e.Gesture);
                if (actuatorSwitch != null)
                {
                    OnSwitchDeactivated(actuatorSwitch);
                }
            }
        }

        /// <summary>
        /// Event handler for when a swtich trigger event is detected.
        /// Notify ACAT
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void sensor_EvtSwitchTrigger(object sender, InputSensorSwitchEventArgs e)
        {
            if (actuatorState == State.Running)
            {
                IActuatorSwitch actuatorSwitch = find(e.Gesture);
                if (actuatorSwitch != null)
                {
                    OnSwitchTriggered(actuatorSwitch);
                }
            }
        }

        /// <summary>
        /// Run BCI device testing. When complete call bciDeviceTestingCompleted()
        /// </summary>
        private void TestBCIDevices()
        {
            _bciDeviceTester = new BCIDeviceTester();
            _bciDeviceTester.EvtBCIDeviceTestingCompleted += bciDeviceTestingCompleted;
            _bciDeviceTester.initialize();
        }

        /// <summary>
        /// Release resources
        /// </summary>
        /// <returns></returns>
        private void unInit()
        {
            actuatorState = State.Stopped;

            DAQ_OpenBCI.Stop();

            try
            {
                //#if OPTICAL_SENSOR
                if (OpticalSensorComm.IsConnected())
                {
                    OpticalSensorComm.StopStreaming();
                    OpticalSensorComm.Close();
                }
                //#endif
            }
            catch
            {
            }
        }

        /// <summary>
        /// Show disclamer
        /// </summary>
        private void showDisclaimer()
        {
            if (!BCIActuatorSettings.Settings.ShowDisclaimerOnStartup)
            {
                return;
            }

            if (DisclaimerDialog.ShowDialog(R.GetString("DisclaimerBCI"), null, true))
            {
                BCIActuatorSettings.Settings.ShowDisclaimerOnStartup = false;
                BCIActuatorSettings.Settings.Save();
            }
        }
    }
}