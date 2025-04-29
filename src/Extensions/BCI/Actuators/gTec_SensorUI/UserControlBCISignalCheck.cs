////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// UserControlBCISignalCheck.cs
//
// Makes sure the BCI signals are good before continuing onto calibration.
// Displays signals from electrodes and does railing and impedance tests
//
// The original insipiration from this class is the OpenBCI GUI application:
// https://github.com/OpenBCI/OpenBCI_GUI
// It is licensed under the MIT License
// Copyright (c) 2018 OpenBCI
// https://github.com/OpenBCI/OpenBCI_GUI/blob/master/LICENSE
////////////////////////////////////////////////////////////////////////////

using ACAT.Extensions.BCI.Actuators.EEG.EEGDataAcquisition;
using ACAT.Extensions.BCI.Actuators.EEG.EEGSettings;
using ACAT.Extensions.BCI.Common.BCIControl;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using Accord.Math;
using brainflow;
using gTecSensorUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ACAT.Extensions.BCI.Actuators.gTecSensorUI
{
    /// <summary>
    /// Makes sure the BCI signals are good before continuing onto calibration. 
    /// Displays signals from electrodes and does railing and impedance tests
    /// </summary>
    public partial class UserControlBCISignalCheck : UserControl
    {
        private String _htmlText = "<!DOCTYPE html>\r\n<html>\r\n  <head>\r\n  <style>\r\n    a:link{color: rgb(255, 170, 0);}\r\n  " +
                            "</style>\r\n  </head>\r\n  <body style=\"background-color:#232433;\">\r\n    " +
                            "<p style=\"font-family:'Montserrat Medium'; font-size:20px; color:white; text-align: center;\">\r\n" +
                            "For additional help on getting good signal quality, watch this <a href=\"$ASSETS_VIDEOS_DIR#ACATOverviewBCI.mp4\">video</a> " +
                            "or review the <a href=$ACAT_USER_GUIDE#SignalCheck>setup guide</a>\r\n    </p>\r\n  </body>\r\n</html>";

        /// <summary>
        /// Describes the different states for which to update the texts related to impedance testing
        /// </summary>
        public enum ImpedanceTestingState
        {
            RUNNING,
            RUNNING_TAB_SWITCH_ATTEMPTED,
            NOT_RUNNING,
            NOT_RUNNING_MAX_TIME_ELAPSED,
            NOT_RUNNING_FAILED_LAST_SIGNAL_QUALITY_CHECK,
            NOT_RUNNING_MAX_TIME_NOT_ELAPSED,
            STOP_IN_PROGRESS,
            STOP_IN_PROGRESS_TAB_SWITCH_ATTEMPTED
        }

        /// <summary>
        /// Holds information related to overall signal quality tests
        /// </summary>
        public struct OverallSignalQualityResult
        {
            public int numOverallGoodElectrodes;
            public int numOverallOkElectrodes;
            public int numOverallBadElectrodes;
            public bool allElectrodesUpdatedWithinSession;
            public int numOverallSignalQualityCheckChannelsConsidered;
        }

        public static OverallSignalQualityResult _AllElectrodesOverallSignalQualityResult;

        // Impedance testing enable / disable commands for deafult 8 channels
        private const string BCI_CMD_IMPEDENCE_CH1_ENABLE = "x1000100Xz101Z";
        private const string BCI_CMD_IMPEDENCE_CH1_DISABLE = "x1060110Xz100Z";
        private const string BCI_CMD_IMPEDENCE_CH2_ENABLE = "x2000100Xz201Z";
        private const string BCI_CMD_IMPEDENCE_CH2_DISABLE = "x2060110Xz200Z";
        private const string BCI_CMD_IMPEDENCE_CH3_ENABLE = "x3000100Xz301Z";
        private const string BCI_CMD_IMPEDENCE_CH3_DISABLE = "x3060110Xz300Z";
        private const string BCI_CMD_IMPEDENCE_CH4_ENABLE = "x4000100Xz401Z";
        private const string BCI_CMD_IMPEDENCE_CH4_DISABLE = "x4060110Xz400Z";
        private const string BCI_CMD_IMPEDENCE_CH5_ENABLE = "x5000100Xz501Z";
        private const string BCI_CMD_IMPEDENCE_CH5_DISABLE = "x5060110Xz500Z";
        private const string BCI_CMD_IMPEDENCE_CH6_ENABLE = "x6000100Xz601Z";
        private const string BCI_CMD_IMPEDENCE_CH6_DISABLE = "x6060110Xz600Z";
        private const string BCI_CMD_IMPEDENCE_CH7_ENABLE = "x7000100Xz701Z";
        private const string BCI_CMD_IMPEDENCE_CH7_DISABLE = "x7060110Xz700Z";
        private const string BCI_CMD_IMPEDENCE_CH8_ENABLE = "x8000100Xz801Z";
        private const string BCI_CMD_IMPEDENCE_CH8_DISABLE = "x8060110Xz800Z";

        // Impedance testing enable / disable commands for extended daisy board channels (total 16)
        private const string BCI_CMD_IMPEDENCE_CH9_ENABLE = "xQ000100XzQ01Z";
        private const string BCI_CMD_IMPEDENCE_CH9_DISABLE = "xQ060110XzQ00Z";
        private const string BCI_CMD_IMPEDENCE_CH10_ENABLE = "xW000100XzW01Z";
        private const string BCI_CMD_IMPEDENCE_CH10_DISABLE = "xW060110XzW00Z";
        private const string BCI_CMD_IMPEDENCE_CH11_ENABLE = "xE000100XzE01Z";
        private const string BCI_CMD_IMPEDENCE_CH11_DISABLE = "xE060110XzE00Z";
        private const string BCI_CMD_IMPEDENCE_CH12_ENABLE = "xR000100Xz401Z";
        private const string BCI_CMD_IMPEDENCE_CH12_DISABLE = "xR060110Xz400Z";
        private const string BCI_CMD_IMPEDENCE_CH13_ENABLE = "xT000100Xz501Z";
        private const string BCI_CMD_IMPEDENCE_CH13_DISABLE = "xT060110Xz500Z";
        private const string BCI_CMD_IMPEDENCE_CH14_ENABLE = "xY000100Xz601Z";
        private const string BCI_CMD_IMPEDENCE_CH14_DISABLE = "xY060110Xz600Z";
        private const string BCI_CMD_IMPEDENCE_CH15_ENABLE = "xU000100Xz701Z";
        private const string BCI_CMD_IMPEDENCE_CH15_DISABLE = "xU060110Xz700Z";
        private const string BCI_CMD_IMPEDENCE_CH16_ENABLE = "xI000100Xz801Z";
        private const string BCI_CMD_IMPEDENCE_CH16_DISABLE = "xI060110Xz800Z";

        private List<String> BCI_CMDS_IMPEDANCE_ENABLE_REQUIRED8CHANNELS =
            new List<String> { BCI_CMD_IMPEDENCE_CH1_ENABLE, BCI_CMD_IMPEDENCE_CH2_ENABLE, BCI_CMD_IMPEDENCE_CH3_ENABLE, BCI_CMD_IMPEDENCE_CH4_ENABLE,
                BCI_CMD_IMPEDENCE_CH5_ENABLE, BCI_CMD_IMPEDENCE_CH6_ENABLE, BCI_CMD_IMPEDENCE_CH7_ENABLE, BCI_CMD_IMPEDENCE_CH8_ENABLE };
        private List<String> BCI_CMDS_IMPEDANCE_DISABLE_REQUIRED8CHANNELS =
            new List<String> { BCI_CMD_IMPEDENCE_CH1_DISABLE, BCI_CMD_IMPEDENCE_CH2_DISABLE, BCI_CMD_IMPEDENCE_CH3_DISABLE, BCI_CMD_IMPEDENCE_CH4_DISABLE,
                BCI_CMD_IMPEDENCE_CH5_DISABLE, BCI_CMD_IMPEDENCE_CH6_DISABLE, BCI_CMD_IMPEDENCE_CH7_DISABLE, BCI_CMD_IMPEDENCE_CH8_DISABLE };
        private List<String> BCI_CMDS_IMPEDANCE_ENABLE_OPTIONAL8CHANNELS =
            new List<String> { BCI_CMD_IMPEDENCE_CH9_ENABLE, BCI_CMD_IMPEDENCE_CH10_ENABLE, BCI_CMD_IMPEDENCE_CH11_ENABLE, BCI_CMD_IMPEDENCE_CH12_ENABLE,
                BCI_CMD_IMPEDENCE_CH13_ENABLE, BCI_CMD_IMPEDENCE_CH14_ENABLE, BCI_CMD_IMPEDENCE_CH15_ENABLE, BCI_CMD_IMPEDENCE_CH16_ENABLE };
        private List<String> BCI_CMDS_IMPEDANCE_DISABLE_OPTIONAL8CHANNELS =
            new List<String> { BCI_CMD_IMPEDENCE_CH9_DISABLE, BCI_CMD_IMPEDENCE_CH10_DISABLE, BCI_CMD_IMPEDENCE_CH11_DISABLE, BCI_CMD_IMPEDENCE_CH12_DISABLE,
                BCI_CMD_IMPEDENCE_CH13_DISABLE, BCI_CMD_IMPEDENCE_CH14_DISABLE, BCI_CMD_IMPEDENCE_CH15_DISABLE, BCI_CMD_IMPEDENCE_CH16_DISABLE };

        /// <summary>
        /// Interval im ms at which to update UI elements
        /// </summary>
        private const int INTERVAL_UPDATE_UI_MS = 50;

        /// <summary>
        /// Last timestamp UI was updated
        /// </summary>
        private long last_timestamp_update_ui = 0;

        /// <summary>
        /// Interval im ms at which to update overall signal quality used to determine whether user can move onto calibration
        /// </summary>
        private const int INTERVAL_UPDATE_OVERALL_SIGNAL_QUALITY_STATUS_MS = 250;

        /// <summary>
        /// Last timestamp when overall signal quality was updated
        /// </summary>
        private long last_timestamp_update_overall_signal_quallity_status = 0;

        /// <summary>
        /// Buffer length in seconds for EEG data to calculate railing
        /// </summary>
        private const int PROCESSING_BUFFER_SIZE_SEC = 5;

        // Constants found in OpenBCI_GUI application (OpenBCI_GUI.pde, InterfaceSerial.pde, etc.) to calculate impedance
        private const int GAIN = 24;
        private const double LEADOFFDRIVE_AMPS = 6.0e-9;  //6 nA, set by its Arduino code
        private const double OPENBCI_SERIES_RESISTOR_OHMS = 2200;

        // Lists holding UI elements for all channels
        private List<String> _channelNamesRequired;
        private List<ScannerRoundedButtonControl> _requiredListElectrodesRailingTest;
        private List<Chart> _requiredListChartsSignalDataRailingTest;
        private List<Chart> _optionalListChartsSignalDataRailingTest;
        private List<Title> _requiredListTextsRailingResultsRailingTest;
        private List<Title> _optionalListTextsRailingResultsRailingTest;
        private List<ScannerRoundedButtonControl> _requiredListElectrodesImpedanceTest;
        private List<ScannerRoundedButtonControl> _optionalListElectrodesImpedanceTest;
        private List<ScannerRoundedButtonControl> _requiredListImpedanceResultsImpedanceTest;
        private List<ScannerRoundedButtonControl> _optionalListImpedanceResultsImpedanceTest;
        private List<ScannerRoundedButtonControl> _requiredListElectrodesQualityResults;
        private List<ScannerRoundedButtonControl> _optionalListElectrodesQualityResults;
        private List<ScannerRoundedButtonControl> _requiredListRailingResultsQualityResults;
        private List<ScannerRoundedButtonControl> _optionalListRailingResultsQualityResults;
        private List<ScannerRoundedButtonControl> _requiredListImpedanceResultsQualityResults;
        private List<ScannerRoundedButtonControl> _optionalListImpedanceResultsQualityResults;
        private Dictionary<String, ScannerRoundedButtonControl> _electrodeCapMap;

        // Variables related to BCI board configuration
        private int _scaleIdx;
        private int _Ymin = -200;
        private int _Ymax = 200;
        private int _bufSize;
        private int[] _indEegChannels;
        private int _samplingRate;
        private int _numChannels;

        public DAQ_gTecBCI gTecBCI = null;
        
        /// <summary>
        /// Holds data / information for each channel 
        /// </summary>
        public static EEGChannel[] _eegChannels;

        /// <summary>
        /// Buffer for unfiltered EEG data for each electrode
        /// </summary>
        private double[,] _unfilteredChannelData;

        /// <summary>
        /// Buffer for filtered EEG data for each electode
        /// </summary>
        private double[,] _filteredChannelData;

        /// <summary>
        /// Array holding images to display for signal quality heat map
        /// </summary>
        private Image[] _signalQualityGradientImages;

        // Colors for different UI elements which are changed based on signal quality or impedance testing status
        private Color COLOR_ACAT_DEFAULT_ORANGE = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
        private Color COLOR_STATUS_OK; // Green
        private Color COLOR_STATUS_ACCEPTABLE; // Yellow
        private Color COLOR_STATUS_KO; // Red

        /// <summary>
        /// Variables representing the different BCI signal check modes
        /// </summary>
        public enum BCISignalCheckMode
        {
            TEST_RAILING,
            TEST_IMPEDANCE,
            TEST_QUALITY
        }

        /// <summary>
        /// Current signal check view mode
        /// </summary>
        public static BCISignalCheckMode _currentBCISignalCheckMode;

        /// <summary>
        /// Makes sure the BCI signals are good before continuing onto calibration. 
        /// Displays signals from electrodes and does railing and impedance tests
        /// </summary>
        /// <param name="stepId"></param>
        /// 
        public UserControlBCISignalCheck()
        {
            InitializeComponent();

            buttonNext_userControlBCISignalCheck.Enabled = true;
            buttonNext_userControlBCISignalCheck.Visible = true;

            // Create electrode name / id -> cap electrode mapping
            _electrodeCapMap = new Dictionary<string, ScannerRoundedButtonControl>();
            _electrodeCapMap["Cz"] = btnElectrodeCapCz;
            _electrodeCapMap["C3"] = btnElectrodeCapC3;
            _electrodeCapMap["C4"] = btnElectrodeCapC4;
            _electrodeCapMap["Pz"] = btnElectrodeCapPz;
            _electrodeCapMap["P3"] = btnElectrodeCapP3;
            _electrodeCapMap["P4"] = btnElectrodeCapP4;
            _electrodeCapMap["T5"] = btnElectrodeCapFz;
            _electrodeCapMap["Fz"] = btnElectrodeCapT5;
            

            // Get UI elements to modify based on signal data, and railing / impedance tests

            _requiredListElectrodesRailingTest = new List<ScannerRoundedButtonControl> { btnElectrodeRailingTestR1, btnElectrodeRailingTestR2, btnElectrodeRailingTestR3, btnElectrodeRailingTestR4,
                btnElectrodeRailingTestR5, btnElectrodeRailingTestR6, btnElectrodeRailingTestR7, btnElectrodeRailingTestR8 };
            _requiredListChartsSignalDataRailingTest = new List<Chart> { chartRailingTestR1, chartRailingTestR2, chartRailingTestR3, chartRailingTestR4,
                chartRailingTestR5, chartRailingTestR6, chartRailingTestR7, chartRailingTestR8 };
            _requiredListTextsRailingResultsRailingTest = new List<Title>();
            _optionalListTextsRailingResultsRailingTest = new List<Title>();

            int chnIdx = 0;
            while (chnIdx < 8)
            {
                _requiredListTextsRailingResultsRailingTest.Add(_requiredListChartsSignalDataRailingTest[chnIdx].Titles[0]);
                //_optionalListTextsRailingResultsRailingTest.Add(_optionalListChartsSignalDataRailingTest[chnIdx].Titles[0]);
                chnIdx += 1;
            }

          
            // Load images for signal quality gradient / heatmap
            _signalQualityGradientImages = new Image[9];
            _signalQualityGradientImages[0] = global::gTecSensorUI.Properties.Resources.signalQualityGradient_1AcceptableChannel; // for heatmap - 0 is the same as 1 accepted channel
            _signalQualityGradientImages[1] = global::gTecSensorUI.Properties.Resources.signalQualityGradient_1AcceptableChannel;
            _signalQualityGradientImages[2] = global::gTecSensorUI.Properties.Resources.signalQualityGradient_2AcceptableChannels;
            _signalQualityGradientImages[3] = global::gTecSensorUI.Properties.Resources.signalQualityGradient_3AcceptableChannels;
            _signalQualityGradientImages[4] = global::gTecSensorUI.Properties.Resources.signalQualityGradient_4AcceptableChannels;
            _signalQualityGradientImages[5] = global::gTecSensorUI.Properties.Resources.signalQualityGradient_5AcceptableChannels;
            _signalQualityGradientImages[6] = global::gTecSensorUI.Properties.Resources.signalQualityGradient_6AcceptableChannels;
            _signalQualityGradientImages[7] = global::gTecSensorUI.Properties.Resources.signalQualityGradient_7AcceptableChannels;
            _signalQualityGradientImages[8] = global::gTecSensorUI.Properties.Resources.signalQualityGradient_8AcceptableChannels;

            // Initialize colors so don't have to constantly create them
            COLOR_ACAT_DEFAULT_ORANGE = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            ColorConverter colorConverter = new ColorConverter();
            COLOR_STATUS_OK = (Color)colorConverter.ConvertFromString("#00FF00"); // Green
            COLOR_STATUS_ACCEPTABLE = (Color)colorConverter.ConvertFromString("#FFFF00"); // Yellow
            COLOR_STATUS_KO = (Color)colorConverter.ConvertFromString("#FF4040"); // Red

            webBrowser.DocumentCompleted += WebBrowser_DocumentCompleted;
            var html = _htmlText.Replace(CoreGlobals.MacroACATUserGuide, HtmlUtils.EncodeString(CoreGlobals.ACATUserGuideFileName));
            webBrowser.DocumentText = html;
        }

        /// <summary>
        /// Reset saved impedance values and flags used to check whether user has passed signal quality checks
        /// Called everytime signal check is accessed
        /// </summary>
        public void resetSavedSignalQualityValues()
        {
            BCIActuatorSettings.Settings.SignalControl_RecheckNeeded = true;
            BCIActuatorSettings.Settings.SignalQuality_PassedLastOverallQualityCheck = false;
            BCIActuatorSettings.Save();
        }

        /// <summary>
        /// Process data (buffer + filter) then update UI elements
        /// </summary>
        /// <param name="latestUnfilteredData"></param>
        /// <param name="runningImpedenceTestingCycle"></param>
        public void updateSignalStatus(double[,] latestUnfilteredData, double[,] latestFilteredData)
        {
            // Concatonate recent data to larger buffer
            AppendDataToBuffer2(latestUnfilteredData, _unfilteredChannelData, _bufSize, out _unfilteredChannelData);
            AppendDataToBuffer2(latestFilteredData, _filteredChannelData, _bufSize, out _filteredChannelData);

            // At interval INTERVAL_UPDATE_UI_MS, use railing and impedance to update UI
            long currentTimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            bool update_UI = false;
            if (currentTimestamp - last_timestamp_update_ui > INTERVAL_UPDATE_UI_MS)
            {
                update_UI = true;
                last_timestamp_update_ui = currentTimestamp;
            }
            bool scale_plots = false;
            // At interval INTERVAL_UPDATE_OVERALL_SIGNAL_QUALITY_STATUS_MS, update overall signal quality
            bool update_overall_signal_quality_status = false;
            if (currentTimestamp - last_timestamp_update_overall_signal_quallity_status > INTERVAL_UPDATE_OVERALL_SIGNAL_QUALITY_STATUS_MS)
            {
                update_overall_signal_quality_status = true;
                last_timestamp_update_overall_signal_quallity_status = currentTimestamp;
            }

            // Iterate through each channel's data and calculate std dev, railing, and update plots
            for (int chIdx = 0; chIdx < _numChannels; chIdx++)
            {
                //////// GetData() function already filters data ////////
                double[] unfilteredChannelData = _unfilteredChannelData.GetRow(chIdx);

                //////// BCI Default Filter - Notch and FrontEnd ////////
                double[] filteredChanelData = _filteredChannelData.GetRow(chIdx);

                // If we're currently on the railing testing page and we're not running the impedance testing
                if (_currentBCISignalCheckMode == BCISignalCheckMode.TEST_RAILING)
                {
                    // Compute railing on latest buffer of UNFILTERED data
                    double railingResPercentage = DataFilter.get_railed_percentage(unfilteredChannelData, GAIN);

                    // Update latest railing result - save value and update UI
                    updateRailingTestResult(chIdx, (int)railingResPercentage, update_UI);

                    // Update signal data chart in railing testing page
                    updateSignalChart(chIdx, latestFilteredData.GetRow(chIdx), scale_plots); // Plot data filtered using Bruna's method
                }

                // Update overall signal quality for electrode
                if (update_overall_signal_quality_status)
                {
                    updateSignalQualityResult(chIdx);
                }
            }

            // Get overall signal quality for all electrodes
            if (update_overall_signal_quality_status)
            {
                // Get latest overall signal quality across all channels
                _AllElectrodesOverallSignalQualityResult = getElectrodeSignalQualityResults();

                // All electrodes have valid values (none have status Error) and are updated within this session
                if (_AllElectrodesOverallSignalQualityResult.allElectrodesUpdatedWithinSession)
                {
                    // Update overall signal quality slider
                    updateSignalQualityGradient(_AllElectrodesOverallSignalQualityResult.numOverallSignalQualityCheckChannelsConsidered,
                        _AllElectrodesOverallSignalQualityResult.numOverallGoodElectrodes,
                        _AllElectrodesOverallSignalQualityResult.numOverallOkElectrodes,
                        _AllElectrodesOverallSignalQualityResult.numOverallBadElectrodes);
                }
            }
        }


        /// <summary>
        /// Based on current signal quality results - return struct that has aggregate info
        /// Ex: # of valid electrodes (has signal status), # of good status electrodes, etc.
        /// </summary>
        /// <returns></returns>
        public OverallSignalQualityResult getElectrodeSignalQualityResults()
        {
            OverallSignalQualityResult newResult = new OverallSignalQualityResult();

            int numOverallSignalQualityCheckChannelsConsidered = 0;
            int numOverallSignalQualityCheckChannelsUpdated = 0;
            for (int chIdx = 0; chIdx < _numChannels; chIdx++)
            {
                EEGChannel eegChannel = _eegChannels[chIdx];
                bool includeOverallSignalQualityCheck = false;

                // "Top8" - Only consider top 8 channels
                if (BCIActuatorSettings.Settings.SignalQuality_AcceptanceMode == "Top8" && eegChannel.isRequiredElectrode)
                {
                    includeOverallSignalQualityCheck = true;
                }

                // "AllEnabled" or not "Top8" (Default)
                // Only consider channels enabled with Classifier_EnableChannelX
                else if (BCIActuatorSettings.Settings.SignalQuality_AcceptanceMode == "AllEnabled" || BCIActuatorSettings.Settings.SignalQuality_AcceptanceMode != "Top8")
                {
                    if (eegChannel._channelEnabled)
                        includeOverallSignalQualityCheck = true;

                    // If SignalQuality_AcceptanceMode == "AllEnabled" don't require Daisy board channel in signal quality check
                    // if only have 8 channels
                    // Even if enabled true with Classifier_EnableChannel9-16
                    if (chIdx >= 8 && BCIActuatorSettings.Settings.DAQ_NumEEGChannels != 16)
                        includeOverallSignalQualityCheck = false;
                }

                // Include channel for overall signal quality check if it meets BCIActuatorSettings.Settings.SignalQuality_AcceptanceMode criteria
                if (includeOverallSignalQualityCheck)

                {
                    numOverallSignalQualityCheckChannelsConsidered += 1;

                    // Count channel if it was updated this session and has valid signal status
                    if (eegChannel.signalQualityUpdatedCurrentSession == 1 && eegChannel.signalStatus != SignalStatus.SIGNAL_ERROR)
                    {
                        numOverallSignalQualityCheckChannelsUpdated += 1;

                        if (eegChannel.signalStatus == SignalStatus.SIGNAL_OK)
                        {
                            newResult.numOverallGoodElectrodes += 1;
                        }
                        else if (eegChannel.signalStatus == SignalStatus.SIGNAL_ACCEPTABLE)
                        {
                            newResult.numOverallOkElectrodes += 1;
                        }
                        else if (eegChannel.signalStatus == SignalStatus.SIGNAL_KO)
                        {
                            newResult.numOverallBadElectrodes += 1;
                        }
                    }
                }
            }

            newResult.numOverallSignalQualityCheckChannelsConsidered = numOverallSignalQualityCheckChannelsConsidered;

            if (numOverallSignalQualityCheckChannelsUpdated < numOverallSignalQualityCheckChannelsConsidered)
            {
                newResult.allElectrodesUpdatedWithinSession = false;
            }
            else
            {
                newResult.allElectrodesUpdatedWithinSession = true;
                if (newResult.numOverallGoodElectrodes >= BCIActuatorSettings.Settings.SignalQuality_MinOverallGoodChannels &&
                    newResult.numOverallOkElectrodes <= BCIActuatorSettings.Settings.SignalQuality_MaxOverallOKChannels​ &&
                    newResult.numOverallBadElectrodes <= BCIActuatorSettings.Settings.SignalQuality_MaxOverallBadChannels​)
                {
                    // All electrodes have valid values (none have status Error) and overall signal quality criteria met
                    BCIActuatorSettings.Settings.SignalQuality_PassedLastOverallQualityCheck = true;
                }
                else
                {
                    BCIActuatorSettings.Settings.SignalQuality_PassedLastOverallQualityCheck = false;
                }
            }

            /*Log.Debug(String.Format("getElectrodeSignalQualityResults() | numOverallValidElectrodes: {0}, " +
                "numOverallGoodElectrodes: {1}, " +
                "numOverallOkElectrodes: {2}, " +
                "numOverallBadElectrodes: {3}, " +
                "allElectrodesUpdatedWithinSession: {4}",
                results[0].ToString(), results[1].ToString(), results[2].ToString(), results[3].ToString(), results[4].ToString()));*/

            return newResult;
        }

        /// <summary>
        /// Handler for when BCISignalCheckMode changed programatically - switch to the correspond tab
        /// </summary>
        /// <param name="mode"></param>
        public void changeSignalCheckMode(BCISignalCheckMode mode)
        {
            try
            {
                if (mode == BCISignalCheckMode.TEST_RAILING)
                {
                    tabControlSignalQuality.SelectedTab = tabPageRailing;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        /// <summary>
        /// Load relevant settings and set processing variables / UI elements accordingly
        /// Update UI based results from previous tests
        /// Update time of last signal quality check (all electrodes tested)
        /// Update status of last user signal quality check
        /// </summary>
        public void initializeBCISignalCheck(DAQ_gTecBCI gtecbci, bool maxTimeHasElapsed, double maxTimeMins, double minElapsedPrevSignalQualityCheck, bool userPassedLastSignalQualityCheck)
        {
            gTecBCI = gtecbci;
            Log.Debug(String.Format("initializeBCISignalCheck | maxTimeHasElapsed: {0}, " +
                "minElapsedPrevSignalQualityCheck: {1}, userPassedLastSignalQualityCheck: {2}",
                maxTimeHasElapsed.ToString(), minElapsedPrevSignalQualityCheck.ToString(), userPassedLastSignalQualityCheck.ToString()));

            // Get / inititalize variables related to board config used in data processing
            _indEegChannels = gTecBCI.indEegChannels;
            _numChannels = BCIActuatorSettings.Settings.DAQ_NumEEGChannels;
            _samplingRate = gTecBCI.sampleRate;
            _scaleIdx = BCIActuatorSettings.Settings.SignalMonitor_ScaleIdx;

            _bufSize = _samplingRate * PROCESSING_BUFFER_SIZE_SEC;
            _unfilteredChannelData = new double[_numChannels, _bufSize];

            // Get initial chartRailingTest Y axis min/max values based on scale
            GetGraphYLims(_scaleIdx, out _Ymin, out _Ymax);

            String _indEegChannels_str = String.Format("_indEegChannels | length: {0}", _indEegChannels.Length.ToString() + " | ");
            for (int i = 0; i < _indEegChannels.Length; i++)
                _indEegChannels_str += (_indEegChannels[i].ToString() + ", ");

            Log.Debug(String.Format("initializeBCISignalCheck | _numChannels: {0}, " + "_samplingRate: {1}, " +
                "_scaleIdx: {2}, _bufSize: {3}\n" +
                "_indEegChannels_str: {4}",
                _numChannels.ToString(), _samplingRate.ToString(), _scaleIdx.ToString(), _bufSize.ToString(), _indEegChannels_str));

            // Set some text fields to smaller font size for 125 scaling (100 scaling is default)
            var tuple = DualMonitor.GetDisplayWidthAndScaling();
            if (tuple.Item1 > 0 && tuple.Item2 == 125)
            {
                handle125Scaling();
            }

            // Initialize object in memory tracking railing, impedance, and overall signal quality statuses across all electrodes
            _eegChannels = new EEGChannel[_numChannels];
            _AllElectrodesOverallSignalQualityResult = new OverallSignalQualityResult();

            // Get required and optional electrode names from settings
            _channelNamesRequired =
            new List<String> { BCIActuatorSettings.Settings.SignalControl_RequiredChannel_Channel1_Name,BCIActuatorSettings.Settings.SignalControl_RequiredChannel_Channel2_Name,
                BCIActuatorSettings.Settings.SignalControl_RequiredChannel_Channel3_Name,BCIActuatorSettings.Settings.SignalControl_RequiredChannel_Channel4_Name,
                BCIActuatorSettings.Settings.SignalControl_RequiredChannel_Channel5_Name,BCIActuatorSettings.Settings.SignalControl_RequiredChannel_Channel6_Name,
                BCIActuatorSettings.Settings.SignalControl_RequiredChannel_Channel7_Name,BCIActuatorSettings.Settings.SignalControl_RequiredChannel_Channel8_Name};

            // For each electrode - initialize object which tracks each channel's data, signal quality, and manages corresponding plots and UI elements
            for (int chIdx = 0; chIdx < _numChannels; chIdx++)
            {
                // Initialize object storing channel properties / corresponding data
                // Get whether channel is enabled / disabled with Classifier_EnableChannelX
                // Set flag whether or not electrode is part of the required base 8
                // Set index of channel's data in raw data
                // Get corresponding UI elements for this channel

                bool channelEnabled = BCIActuatorSettings.Settings.GetClassifier_EnableChannel(chIdx);

                if (chIdx < 8)
                {
                    // Required channel
                    String electrodeName = _channelNamesRequired[chIdx];
                    _eegChannels[chIdx] = new EEGChannel(electrodeName, chIdx, channelEnabled);
                    _eegChannels[chIdx].isRequiredElectrode = true;
                    _eegChannels[chIdx]._channelRawDataIndex = _indEegChannels[chIdx];
                    _eegChannels[chIdx].ImpedanceTestingEnableCmd = BCI_CMDS_IMPEDANCE_ENABLE_REQUIRED8CHANNELS[chIdx];
                    _eegChannels[chIdx].ImpedanceTestingDisableCmd = BCI_CMDS_IMPEDANCE_DISABLE_REQUIRED8CHANNELS[chIdx];
                    _eegChannels[chIdx].lastRailingResult = int.MaxValue;
                    _eegChannels[chIdx].lastImpedanceResult = int.MaxValue;
                    _eegChannels[chIdx].signalStatus = SignalStatus.SIGNAL_ERROR;
                    _eegChannels[chIdx].signalQualityUpdatedCurrentSession = 0;

                    // Get corresponding railing and impedance UI elements and initialize with default values
                    _eegChannels[chIdx].electrodeCap = _electrodeCapMap[electrodeName];
                    _eegChannels[chIdx].electrodeCap.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
                    _eegChannels[chIdx].electrodeCap.BorderColor = Color.White;
                    _eegChannels[chIdx].electrodeRailingTest = _requiredListElectrodesRailingTest[chIdx];
                    _eegChannels[chIdx].electrodeRailingTest.Text = electrodeName;
                    _eegChannels[chIdx].chartSignalDataRailingTest = _requiredListChartsSignalDataRailingTest[chIdx];
                    _eegChannels[chIdx].textRailingResultRailingTest = _requiredListTextsRailingResultsRailingTest[chIdx];

                    // Set X axis min/max
                    _eegChannels[chIdx].chartSignalDataRailingTest.ChartAreas[0].AxisX.Minimum = 0;
                    _eegChannels[chIdx].chartSignalDataRailingTest.ChartAreas[0].AxisX.Maximum = _bufSize;

                    // Set Y axis min/max based on scale
                    _eegChannels[chIdx].chartSignalDataRailingTest.ChartAreas[0].AxisY.IsStartedFromZero = false;
                    _eegChannels[chIdx].chartSignalDataRailingTest.ChartAreas[0].AxisY.Minimum = _Ymin;
                    _eegChannels[chIdx].chartSignalDataRailingTest.ChartAreas[0].AxisY.Maximum = _Ymax;

                    // Set other plot parameters
                    _eegChannels[chIdx].chartSignalDataRailingTest.Series[0].ChartType = SeriesChartType.FastLine;
                    _eegChannels[chIdx].chartSignalDataRailingTest.Series[0].BorderColor = Color.White;
                    _eegChannels[chIdx].chartSignalDataRailingTest.Series[0].BorderWidth = 1;
                }
              
                // TODO - testing parameter not enabled currently
                // For testing - duplicate required channel at the corresponding offset position
                // if(BCIActuatorSettings.Settings.Testing_DuplicateRequiredChannelsAsOptionalChannels) { }
            }

            // Should have checked already if Cyton Daisy board attached
            // Correct number of channels saved in DAQ_NumEEGChannels
        }

        
        /// <summary>
        /// Function called for timer that processes data for BCI signal check
        /// </summary>
        /// <param name="data"></param>
        public void ProcessDataSignalCheck(double[,] data, double[,] filteredData)
        {
            if (GTecDeviceTester._endSignalCheckTimer)
                return;

            try
            {
                int numSamples = data.GetLength(1);
                double[,] latestUnfilteredData = new double[_numChannels, numSamples];
                double[,] latestFilteredData = new double[_numChannels, numSamples];

                // Remove _indEegChannels indexing from raw unfiltered data before processing
                for (int chIdx = 0; chIdx < _numChannels; chIdx++)
                {
                    latestUnfilteredData.SetRow(chIdx, data.GetRow(_indEegChannels[chIdx]));
                    latestFilteredData.SetRow(chIdx, filteredData.GetRow(_indEegChannels[chIdx]));
                }

                // Pass data to function that processes data for signal quality checking and updates UI
                if (_currentBCISignalCheckMode == BCISignalCheckMode.TEST_RAILING ||
                    _currentBCISignalCheckMode == BCISignalCheckMode.TEST_IMPEDANCE)
                    updateSignalStatus(latestUnfilteredData, latestFilteredData);
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }

            // Removed automatic optical sensor checks below for now - it's likely the optical sensor is still
            // working at this point + the calibration will catch any potential issues with the optical sensor

            // Next button always enabled / visible
        }

        /// <summary>
        /// Update overall signal quality heat map / slider based on aggregated signal quality status
        /// </summary>
        private void updateSignalQualityGradient(int numChannelsConsidered,
            int numOverallGoodElectrodes, int numOverallOkElectrodes, int numOverallBadElectrodes)
        {
            int indxGradientImage = 0;
            double totalPoints = 2 * numChannelsConsidered;
            double points = (2 * numOverallGoodElectrodes) + (1 * numOverallOkElectrodes);

            indxGradientImage = ((int)Math.Round(((points / totalPoints) * _signalQualityGradientImages.Length))) - 1;
            if (indxGradientImage < 0)
                indxGradientImage = 0;
            else if (indxGradientImage > _signalQualityGradientImages.Length - 1)
                indxGradientImage = _signalQualityGradientImages.Length - 1;

            Invoke(new Action(() =>
            {
                Image newGradientImage = newGradientImage = _signalQualityGradientImages[indxGradientImage];
                panelSignalQualitySlider.BackgroundImage = newGradientImage;
            }));
        }

        /// <summary>
        /// Update railing value and update UI elements
        /// </summary>
        private void updateRailingTestResult(int chIdx, int railingResultPercentage, bool update_UI)
        {
            try
            {
                BCIActuatorSettings.Settings.SignalQuality_LastRailingValues[chIdx] = railingResultPercentage;

                if (update_UI && railingResultPercentage != int.MaxValue)
                {
                    Invoke(new Action(() =>
                    {
                        Color railingResultColor = SelectColorFromStatus(SignalStatus.SIGNAL_ERROR);
                        if (railingResultPercentage <= BCIActuatorSettings.Settings.SignalQuality_RailingGoodMaxThreshold)
                            railingResultColor = SelectColorFromStatus(SignalStatus.SIGNAL_OK);
                        else if (railingResultPercentage > BCIActuatorSettings.Settings.SignalQuality_RailingGoodMaxThreshold &&
                            railingResultPercentage <= BCIActuatorSettings.Settings.SignalQuality_RailingOkMaxThreshold​)
                            railingResultColor = SelectColorFromStatus(SignalStatus.SIGNAL_ACCEPTABLE);
                        else if (railingResultPercentage == 100 ||
                            railingResultPercentage > BCIActuatorSettings.Settings.SignalQuality_RailingOkMaxThreshold​)
                            railingResultColor = SelectColorFromStatus(SignalStatus.SIGNAL_KO);

                        _eegChannels[chIdx].electrodeRailingTest.BackColor = railingResultColor;
                        _eegChannels[chIdx].chartSignalDataRailingTest.Series[0].BorderColor = railingResultColor;
                        _eegChannels[chIdx].chartSignalDataRailingTest.Series[0].Color = railingResultColor;

                        _eegChannels[chIdx].electrodeCap.BackColor = railingResultColor;
                        _eegChannels[chIdx].electrodeCap.BorderColor = railingResultColor;
                        _eegChannels[chIdx].electrodeCap.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));

                        String railingResPercentage_format = railingResultPercentage.ToString() + "%";
                        _eegChannels[chIdx].textRailingResultRailingTest.Text = railingResPercentage_format;
                        _eegChannels[chIdx].textRailingResultRailingTest.ForeColor = railingResultColor;

                        //_eegChannels[chIdx].railingResultQualityResults.Text = railingResPercentage_format;
                        //_eegChannels[chIdx].railingResultQualityResults.ForeColor = railingResultColor;
                        //_eegChannels[chIdx].railingResultQualityResults.BorderColor = railingResultColor;
                    }));
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }


        /// <summary>
        /// From latest impedance and railing values - get overall electrode signal quality status
        /// </summary>
        /// <param name="chIdx"></param>
        /// <returns></returns>
        public bool updateSignalQualityResult(int chIdx)
        {
            bool ret = false;

            try
            {
                int currentRailingPercentage = BCIActuatorSettings.Settings.SignalQuality_LastRailingValues[chIdx];
                int currentImpedanceResult = BCIActuatorSettings.Settings.SignalQuality_LastImpedanceValues[chIdx];
                SignalStatus overallSignalQualityStatus = SignalStatus.SIGNAL_ERROR;

                if (currentRailingPercentage != int.MaxValue && currentImpedanceResult != int.MaxValue)
                {
                    _eegChannels[chIdx].lastRailingResult = currentRailingPercentage;
                    _eegChannels[chIdx].lastImpedanceResult = currentImpedanceResult;

                    if (currentRailingPercentage <= BCIActuatorSettings.Settings.SignalQuality_RailingGoodMaxThreshold &&
                        currentImpedanceResult <= BCIActuatorSettings.Settings.SignalQuality_ImpedanceGoodMaxThreshold)
                        overallSignalQualityStatus = SignalStatus.SIGNAL_OK;
                    else if (currentRailingPercentage <= BCIActuatorSettings.Settings.SignalQuality_RailingOkMaxThreshold &&
                        currentImpedanceResult <= BCIActuatorSettings.Settings.SignalQuality_ImpedanceOkMaxThreshold)
                        overallSignalQualityStatus = SignalStatus.SIGNAL_ACCEPTABLE;
                    else if (currentRailingPercentage > BCIActuatorSettings.Settings.SignalQuality_RailingOkMaxThreshold ||
                        currentImpedanceResult > BCIActuatorSettings.Settings.SignalQuality_ImpedanceOkMaxThreshold)
                        overallSignalQualityStatus = SignalStatus.SIGNAL_KO;

                    _eegChannels[chIdx].timeLastUpdatedSec = DateTimeOffset.Now.ToUnixTimeSeconds();
                    _eegChannels[chIdx].timeLastUpdatedMin = (int)(_eegChannels[chIdx].timeLastUpdatedSec / 60);
                    _eegChannels[chIdx].signalQualityUpdatedCurrentSession = 1;
                    _eegChannels[chIdx].signalStatus = overallSignalQualityStatus;

                    Invoke(new Action(() =>
                    {
                        Color overallSignalQualityColor = SelectColorFromStatus(overallSignalQualityStatus);

                        _eegChannels[chIdx].electrodeCap.BackColor = overallSignalQualityColor;
                        _eegChannels[chIdx].electrodeCap.BorderColor = overallSignalQualityColor;
                        _eegChannels[chIdx].electrodeCap.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
                        _eegChannels[chIdx].electrodeQualityResults.BackColor = overallSignalQualityColor;
                    }));
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }

            return ret;
        }

        /// <summary>
        /// Update railing chart displaying signal data
        /// </summary>
        /// <param name="inputSample"></param>
        /// <param name="isGestureDetected"></param>
        private void updateSignalChart(int channelIndex, double[] samples, bool scale_plots)
        {
            try
            {
                Invoke(new Action(() =>
                {
                    EEGChannel currentEegChannel = _eegChannels[channelIndex];

                    int sampleIdx = 0;
                    while (sampleIdx < samples.Length)
                    {
                        // Add signal
                        _eegChannels[channelIndex].chartSignalDataRailingTest.Series[0].Points.Add(samples[sampleIdx]);

                        // Remove points at the end of the graph (for timeseries data)
                        if (_eegChannels[channelIndex].chartSignalDataRailingTest.Series[0].Points.Count >= _bufSize)
                            _eegChannels[channelIndex].chartSignalDataRailingTest.Series[0].Points.RemoveAt(0);

                        sampleIdx += 1;
                    }

                    if (scale_plots)
                    {
                        //Log.Debug("updateSignalChart | scale_plots == true | channelIndex: " + channelIndex.ToString());

                        // Autoscale
                        //_eegChannels[channelIndex].chartSignalDataRailingTest.ChartAreas[0].RecalculateAxesScale();

                        // Use _Ymin, _Ymax
                        _eegChannels[channelIndex].chartSignalDataRailingTest.ChartAreas[0].AxisY.Minimum = _Ymin;
                        _eegChannels[channelIndex].chartSignalDataRailingTest.ChartAreas[0].AxisY.Maximum = _Ymax;
                    }
                }));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        /// <summary>
        /// For internal use, adds filtered data to a buffer to assess signal status
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        private static bool AppendDataToBuffer2(double[,] data, double[,] inBuffer, int numSamplesToKeep, out double[,] outBuffer)
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

        /// <summary>
        /// Called when user selects one of the tabs (Railing, Impedance, Quality)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControlElectrodeQuality_SelectedIndexChanged(object sender, EventArgs e)
        {
        
            if (tabControlSignalQuality.SelectedIndex == 0)
            {
                _currentBCISignalCheckMode = BCISignalCheckMode.TEST_RAILING;
                highlightSelectedTab(0);
            }
            else if (tabControlSignalQuality.SelectedIndex == 1)
            {
                _currentBCISignalCheckMode = BCISignalCheckMode.TEST_IMPEDANCE;
                highlightSelectedTab(1);
            }
            else if (tabControlSignalQuality.SelectedIndex == 2)
            {
                _currentBCISignalCheckMode = BCISignalCheckMode.TEST_QUALITY;
                highlightSelectedTab(2);
            }
            
            Log.Debug("tabControlElectrodeQuality_SelectedIndexChanged" + " | _currentBCISignalCheckMode: " + _currentBCISignalCheckMode.ToString());
        }

        /// <summary>
        /// Helper function to highlight the current tab selected
        /// </summary>
        /// <param name="tabControlIndex"></param>
        private void highlightSelectedTab(int tabControlIndex)
        {
            tabPageRailing.Text = "- 1. Railing -";
        }

        /// <summary>
        /// Get the minimum and maximum values of the chartRailingTest using _scaleIdx
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        private void GetGraphYLims(int scaleIdx, out int yLimMin, out int yLimMax)
        {
            int scale = 100;

            try
            {
                switch (scaleIdx)
                {
                    case 0:
                        //20uV
                        scale = 20;
                        break;

                    case 1:
                        // 50uV
                        scale = 50;
                        break;

                    case 2:
                        // 100uV
                        scale = 100;
                        break;

                    case 3:
                        // 200uV
                        scale = 200;
                        break;

                    case 4:
                        // 500uV
                        scale = 500;
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
            }

            yLimMax = scale;
            yLimMin = -1 * scale;
        }

        /// <summary>
        /// Gets the color according to the status of the signal
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        private Color SelectColorFromStatus(SignalStatus status)
        {
            Color colorStatus = COLOR_STATUS_KO;
            switch (status)
            {
                case SignalStatus.SIGNAL_OK:
                    colorStatus = COLOR_STATUS_OK;
                    break;

                case SignalStatus.SIGNAL_ACCEPTABLE:
                    colorStatus = COLOR_STATUS_ACCEPTABLE;
                    break;

                case SignalStatus.SIGNAL_KO:
                    colorStatus = COLOR_STATUS_KO;
                    break;

                case SignalStatus.SIGNAL_ERROR:
                    colorStatus = Color.Gray;
                    break;
            }
            return colorStatus;
        }

        private void WebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowser.Navigating -= WebBrowserDesc_Navigating;
            webBrowser.Navigating += WebBrowserDesc_Navigating;
        }

        private void WebBrowserDesc_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            Utils.HandleHelpNavigaion(e);
        }

        /// <summary>
        /// Handle 125 scaling for the display - Make most problematic texts smaller
        /// </summary>
        public void handle125Scaling()
        {
            try
            {
                // Make most problematic text fields in this user control smaller to work better with 125 scaling

                // Large header texts
                foreach (Label labelResize in new List<Label> { labelBCISignalCheck, labelRailingTest })
                {
                    labelResize.Font = new System.Drawing.Font("Montserrat", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                }

                // Bold smaller texts
                foreach (Label labelResize in new List<Label> { labelRailingTestInfo2 })
                {
                    labelResize.Font = new System.Drawing.Font("Montserrat SemiBold", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                }

                // Normal smaller texts
                foreach (Label labelResize in new List<Label> { labelBCISignalCheckDescription, labelRailingTestInfo1, labelRailingTestInfo3 })
                {
                    labelResize.Font = new System.Drawing.Font("Montserrat", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                }

                tabControlSignalQuality.Font = new System.Drawing.Font("Montserrat Medium", 11.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            }
            catch (Exception ex)
            {
                Log.Debug(ex.Message);
            }
        }
    }
}