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
using SensorUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ACAT.Extensions.BCI.Actuators.SensorUI
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

        /// <summary>
        /// Unique ID for this step
        /// </summary>
        private String _stepId;

        /// <summary>
        /// Debugging flag - ignore optical sensor error checks
        /// </summary>
        private bool _Testing_BCIOnboardingIgnoreOpticalSensorChecks = false;

        // Lower level Cyton board commands to enable and disable impedance testing for each channel

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
        private List<String> _channelNamesOptional;
        private List<ScannerRoundedButtonControl> _requiredListElectrodesRailingTest;
        private List<ScannerRoundedButtonControl> _optionalListElectrodesRailingTest;
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

        /// <summary>
        /// Flag that controls start / stop of impedance testing
        /// </summary>
        public static bool _runImpedanceTestingCycle;

        /// <summary>
        /// Flag which provides status on whether impedance testing is running or not
        /// </summary>
        public static bool _impedanceTestingRunning = false;

        /// <summary>
        /// The index of the current electrode being tested for impedance
        /// </summary>
        public static int _currentImpedenceTestElectrodeIndex;
        
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
        public UserControlBCISignalCheck(String stepId)
        {
            InitializeComponent();

            _stepId = stepId;
            buttonNext.Enabled = true;
            buttonNext.Visible = true;

            // Create electrode name / id -> cap electrode mapping
            _electrodeCapMap = new Dictionary<string, ScannerRoundedButtonControl>();
            _electrodeCapMap["Cz"] = btnElectrodeCapCz;
            _electrodeCapMap["C3"] = btnElectrodeCapC3;
            _electrodeCapMap["C4"] = btnElectrodeCapC4;
            _electrodeCapMap["Pz"] = btnElectrodeCapPz;
            _electrodeCapMap["P3"] = btnElectrodeCapP3;
            _electrodeCapMap["P4"] = btnElectrodeCapP4;
            _electrodeCapMap["Fz"] = btnElectrodeCapFz;
            _electrodeCapMap["T5"] = btnElectrodeCapT5;
            _electrodeCapMap["T6"] = btnElectrodeCapT6;
            _electrodeCapMap["O1"] = btnElectrodeCapO1;
            _electrodeCapMap["O2"] = btnElectrodeCapO2;
            _electrodeCapMap["Fp1"] = btnElectrodeCapFp1;
            _electrodeCapMap["Fp2"] = btnElectrodeCapFp2;
            _electrodeCapMap["F3"] = btnElectrodeCapF3;
            _electrodeCapMap["F4"] = btnElectrodeCapF4;
            _electrodeCapMap["F7"] = btnElectrodeCapF7;
            _electrodeCapMap["F8"] = btnElectrodeCapF8;
            _electrodeCapMap["T3"] = btnElectrodeCapT3;

            // Get UI elements to modify based on signal data, and railing / impedance tests

            _requiredListElectrodesRailingTest = new List<ScannerRoundedButtonControl> { btnElectrodeRailingTestR1, btnElectrodeRailingTestR2, btnElectrodeRailingTestR3, btnElectrodeRailingTestR4,
                btnElectrodeRailingTestR5, btnElectrodeRailingTestR6, btnElectrodeRailingTestR7, btnElectrodeRailingTestR8 };
            _optionalListElectrodesRailingTest = new List<ScannerRoundedButtonControl> { btnElectrodeRailingTestOp1, btnElectrodeRailingTestOp2, btnElectrodeRailingTestOp3, btnElectrodeRailingTestOp4,
                btnElectrodeRailingTestOp5, btnElectrodeRailingTestOp6, btnElectrodeRailingTestOp7, btnElectrodeRailingTestOp8 };
            _requiredListChartsSignalDataRailingTest = new List<Chart> { chartRailingTestR1, chartRailingTestR2, chartRailingTestR3, chartRailingTestR4,
                chartRailingTestR5, chartRailingTestR6, chartRailingTestR7, chartRailingTestR8 };
            _optionalListChartsSignalDataRailingTest = new List<Chart> { chartRailingTestOp1, chartRailingTestOp2, chartRailingTestOp3, chartRailingTestOp4,
                chartRailingTestOp5, chartRailingTestOp6, chartRailingTestOp7, chartRailingTestOp8 };
            _requiredListTextsRailingResultsRailingTest = new List<Title>();
            _optionalListTextsRailingResultsRailingTest = new List<Title>();

            int chnIdx = 0;
            while (chnIdx < 8)
            {
                _requiredListTextsRailingResultsRailingTest.Add(_requiredListChartsSignalDataRailingTest[chnIdx].Titles[0]);
                _optionalListTextsRailingResultsRailingTest.Add(_optionalListChartsSignalDataRailingTest[chnIdx].Titles[0]);
                chnIdx += 1;
            }

            // UI Elements for Railing Test Page
            _requiredListElectrodesImpedanceTest = new List<ScannerRoundedButtonControl> { btnElectrodeImpedanceTestR1, btnElectrodeImpedanceTestR2, btnElectrodeImpedanceTestR3, btnElectrodeImpedanceTestR4,
                btnElectrodeImpedanceTestR5, btnElectrodeImpedanceTestR6, btnElectrodeImpedanceTestR7, btnElectrodeImpedanceTestR8 };
            _optionalListElectrodesImpedanceTest = new List<ScannerRoundedButtonControl> { btnElectrodeImpedanceTestOp1, btnElectrodeImpedanceTestOp2, btnElectrodeImpedanceTestOp3, btnElectrodeImpedanceTestOp4,
                btnElectrodeImpedanceTestOp5, btnElectrodeImpedanceTestOp6, btnElectrodeImpedanceTestOp7, btnElectrodeImpedanceTestOp8 };
            _requiredListImpedanceResultsImpedanceTest = new List<ScannerRoundedButtonControl> { btnImpedanceResImpedanceTestR1, btnImpedanceResImpedanceTestR2, btnImpedanceResImpedanceTestR3, btnImpedanceResImpedanceTestR4,
                btnImpedanceResImpedanceTestR5, btnImpedanceResImpedanceTestR6, btnImpedanceResImpedanceTestR7, btnImpedanceResImpedanceTestR8 };
            _optionalListImpedanceResultsImpedanceTest = new List<ScannerRoundedButtonControl> { btnImpedanceResImpedanceTestOp1, btnImpedanceResImpedanceTestOp2, btnImpedanceResImpedanceTestOp3, btnImpedanceResImpedanceTestOp4,
                btnImpedanceResImpedanceTestOp5, btnImpedanceResImpedanceTestOp6, btnImpedanceResImpedanceTestOp7, btnImpedanceResImpedanceTestOp8 };

            // UI Elements for Impedance Test Page
            _requiredListElectrodesQualityResults = new List<ScannerRoundedButtonControl> { btnElectrodeQualityResultsR1, btnElectrodeQualityResultsR2, btnElectrodeQualityResultsR3, btnElectrodeQualityResultsR4,
                btnElectrodeQualityResultsR5, btnElectrodeQualityResultsR6, btnElectrodeQualityResultsR7, btnElectrodeQualityResultsR8 };
            _optionalListElectrodesQualityResults = new List<ScannerRoundedButtonControl> { btnElectrodeQualityResultsOp1, btnElectrodeQualityResultsOp2, btnElectrodeQualityResultsOp3, btnElectrodeQualityResultsOp4,
                btnElectrodeQualityResultsOp5, btnElectrodeQualityResultsOp6, btnElectrodeQualityResultsOp7, btnElectrodeQualityResultsOp8 };
            _requiredListRailingResultsQualityResults = new List<ScannerRoundedButtonControl> { btnRailingResQualityResultsR1, btnRailingResQualityResultsR2, btnRailingResQualityResultsR3, btnRailingResQualityResultsR4,
                btnRailingResQualityResultsR5, btnRailingResQualityResultsR6, btnRailingResQualityResultsR7, btnRailingResQualityResultsR8 };
            _optionalListRailingResultsQualityResults = new List<ScannerRoundedButtonControl> { btnRailingResQualityResultsOp1, btnRailingResQualityResultsOp2, btnRailingResQualityResultsOp3, btnRailingResQualityResultsOp4,
                btnRailingResQualityResultsOp5, btnRailingResQualityResultsOp6, btnRailingResQualityResultsOp7, btnRailingResQualityResultsOp8 };
            _requiredListImpedanceResultsQualityResults = new List<ScannerRoundedButtonControl> { btnImpedanceResQualityResultsR1, btnImpedanceResQualityResultsR2, btnImpedanceResQualityResultsR3, btnImpedanceResQualityResultsR4,
                btnImpedanceResQualityResultsR5, btnImpedanceResQualityResultsR6, btnImpedanceResQualityResultsR7, btnImpedanceResQualityResultsR8 };
            _optionalListImpedanceResultsQualityResults = new List<ScannerRoundedButtonControl> { btnImpedanceResQualityResultsOp1, btnImpedanceResQualityResultsOp2, btnImpedanceResQualityResultsOp3, btnImpedanceResQualityResultsOp4,
                btnImpedanceResQualityResultsOp5, btnImpedanceResQualityResultsOp6, btnImpedanceResQualityResultsOp7, btnImpedanceResQualityResultsOp8 };

            // Load images for signal quality gradient / heatmap
            _signalQualityGradientImages = new Image[9];
            _signalQualityGradientImages[0] = global::SensorUI.Properties.Resources.signalQualityGradient_1AcceptableChannel; // for heatmap - 0 is the same as 1 accepted channel
            _signalQualityGradientImages[1] = global::SensorUI.Properties.Resources.signalQualityGradient_1AcceptableChannel;
            _signalQualityGradientImages[2] = global::SensorUI.Properties.Resources.signalQualityGradient_2AcceptableChannels;
            _signalQualityGradientImages[3] = global::SensorUI.Properties.Resources.signalQualityGradient_3AcceptableChannels;
            _signalQualityGradientImages[4] = global::SensorUI.Properties.Resources.signalQualityGradient_4AcceptableChannels;
            _signalQualityGradientImages[5] = global::SensorUI.Properties.Resources.signalQualityGradient_5AcceptableChannels;
            _signalQualityGradientImages[6] = global::SensorUI.Properties.Resources.signalQualityGradient_6AcceptableChannels;
            _signalQualityGradientImages[7] = global::SensorUI.Properties.Resources.signalQualityGradient_7AcceptableChannels;
            _signalQualityGradientImages[8] = global::SensorUI.Properties.Resources.signalQualityGradient_8AcceptableChannels;

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
            // Write max value to saved impedance values (sets as incomplete / not yet tested, no impedance value shown in UI)
            for (int chIdx = 0; chIdx < _numChannels; chIdx++)
            {
                updateImpedanceResult(chIdx, int.MaxValue, false, false);
            }
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
                // If currently running impedance testing and this channel is not the one being tested
                if ((_runImpedanceTestingCycle || _impedanceTestingRunning) && chIdx != _currentImpedenceTestElectrodeIndex)
                    continue;

                //////// GetData() function already filters data ////////
                double[] unfilteredChannelData = _unfilteredChannelData.GetRow(chIdx);

                //////// BCI Default Filter - Notch and FrontEnd ////////
                double[] filteredChanelData = _filteredChannelData.GetRow(chIdx);

                // If we're currently on impedance testing page and we're running impedance testing
                // and this channel is the one being tested, compute and update impedance
                if ((_runImpedanceTestingCycle || _impedanceTestingRunning) && _currentImpedenceTestElectrodeIndex == chIdx)
                {
                    // Compute impedance
                    int stdCalcStartPos = filteredChanelData.Length - 1 - _samplingRate;
                    int stdCalcEndPos = filteredChanelData.Length - 1;
                    if (stdCalcStartPos < 0)
                    {
                        stdCalcStartPos = 0;
                    }

                    double data_std_uV_channel = DataFilter.calc_stddev(filteredChanelData, stdCalcStartPos, stdCalcEndPos);
                    double impedence = (Math.Sqrt(2.0) * data_std_uV_channel * 1.0e-6) / LEADOFFDRIVE_AMPS;
                    impedence -= OPENBCI_SERIES_RESISTOR_OHMS;
                    impedence = impedence / 1000;

                    // Update latest impedance result - save value and update UI
                    updateImpedanceResult(chIdx, (int)impedence, update_UI, false);
                }

                // If we're currently on the railing testing page and we're not running the impedance testing
                if ((!_runImpedanceTestingCycle && !_impedanceTestingRunning) && _currentBCISignalCheckMode == BCISignalCheckMode.TEST_RAILING)
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
                else if (mode == BCISignalCheckMode.TEST_IMPEDANCE)
                {
                    tabControlSignalQuality.SelectedTab = tabPageImpedance;
                }
                else if (mode == BCISignalCheckMode.TEST_QUALITY)
                {
                    tabControlSignalQuality.SelectedTab = tabPageQuality;
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
        public void initializeBCISignalCheck(bool maxTimeHasElapsed, double maxTimeMins, double minElapsedPrevSignalQualityCheck,
            bool userPassedLastSignalQualityCheck)
        {
            Log.Debug(String.Format("initializeBCISignalCheck | maxTimeHasElapsed: {0}, " +
                "minElapsedPrevSignalQualityCheck: {1}, userPassedLastSignalQualityCheck: {2}",
                maxTimeHasElapsed.ToString(), minElapsedPrevSignalQualityCheck.ToString(), userPassedLastSignalQualityCheck.ToString()));

            _Testing_BCIOnboardingIgnoreOpticalSensorChecks = BCIActuatorSettings.Settings.Testing_BCIOnboardingIgnoreOpticalSensorChecks;

            // Get / inititalize variables related to board config used in data processing
            _indEegChannels = DAQ_OpenBCI.indEegChannels;
            _numChannels = BCIActuatorSettings.Settings.DAQ_NumEEGChannels;
            _samplingRate = DAQ_OpenBCI.sampleRate;
            _scaleIdx = BCIActuatorSettings.Settings.SignalMonitor_ScaleIdx;

            // If Daisy board connection found earlier or testing parameter set to duplicate required channels
            // Update UI with expectation of 16 channels
            if (BCIActuatorSettings.Settings.Testing_DuplicateRequiredChannelsAsOptionalChannels)
                _numChannels = 16;

            if (_numChannels == 16)
            {
                foreach (Label labelEquals in new List<Label> { labelEqualsIRailingTest1, labelEqualsIRailingTest2, labelEqualsIRailingTest3, labelEqualsIRailingTest4,
                    labelEqualsIRailingTest5, labelEqualsIRailingTest6, labelEqualsIRailingTest7, labelEqualsIRailingTest8})
                    labelEquals.ForeColor = Color.White;
                foreach (Label labelEquals in new List<Label> { labelEqualsImpedanceTest1, labelEqualsImpedanceTest2, labelEqualsImpedanceTest3, labelEqualsImpedanceTest4,
                    labelEqualsImpedanceTestOp5, labelEqualsImpedanceTestOp6, labelEqualsImpedanceTestOp7, labelEqualsImpedanceTestOp8})
                    labelEquals.ForeColor = Color.White;

                // TODO - Plus and equal signs quality results page
                labelOptionalRailingTest.ForeColor = Color.White;
                labelOptionalElectrodesImpedanceTest.ForeColor = Color.White;
                labelOptionalElectrodesQualityResults.ForeColor = Color.White;
            }

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

            _channelNamesOptional =
            new List<String> { BCIActuatorSettings.Settings.SignalControl_OptionalChannel_Channel9_Name, BCIActuatorSettings.Settings.SignalControl_OptionalChannel_Channel10_Name,
                BCIActuatorSettings.Settings.SignalControl_OptionalChannel_Channel11_Name, BCIActuatorSettings.Settings.SignalControl_OptionalChannel_Channel12_Name,
                BCIActuatorSettings.Settings.SignalControl_OptionalChannel_Channel13_Name, BCIActuatorSettings.Settings.SignalControl_OptionalChannel_Channel14_Name,
                BCIActuatorSettings.Settings.SignalControl_OptionalChannel_Channel15_Name, BCIActuatorSettings.Settings.SignalControl_OptionalChannel_Channel16_Name};

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
                    _eegChannels[chIdx].electrodeImpedanceTest = _requiredListElectrodesImpedanceTest[chIdx];
                    _eegChannels[chIdx].electrodeImpedanceTest.Text = electrodeName;
                    _eegChannels[chIdx].impedanceResultImpedanceTest = _requiredListImpedanceResultsImpedanceTest[chIdx];
                    _eegChannels[chIdx].electrodeQualityResults = _requiredListElectrodesQualityResults[chIdx];
                    _eegChannels[chIdx].electrodeQualityResults.Text = electrodeName;
                    _eegChannels[chIdx].railingResultQualityResults = _requiredListRailingResultsQualityResults[chIdx];
                    _eegChannels[chIdx].impedanceResultQualityResults = _requiredListImpedanceResultsQualityResults[chIdx];

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
                else if (chIdx >= 8 && chIdx < 16)
                {
                    // Optional channel
                    int intOptionalChnOffset = chIdx - 8;

                    // Real optional channel connected via Cyton Daisy board
                    String electrodeName = _channelNamesOptional[intOptionalChnOffset];
                    _eegChannels[chIdx] = new EEGChannel(electrodeName, chIdx, channelEnabled);
                    _eegChannels[chIdx].isRequiredElectrode = false;
                    _eegChannels[chIdx]._channelRawDataIndex = _indEegChannels[chIdx];
                    _eegChannels[chIdx].ImpedanceTestingEnableCmd = BCI_CMDS_IMPEDANCE_ENABLE_OPTIONAL8CHANNELS[intOptionalChnOffset];
                    _eegChannels[chIdx].ImpedanceTestingDisableCmd = BCI_CMDS_IMPEDANCE_DISABLE_OPTIONAL8CHANNELS[intOptionalChnOffset];
                    _eegChannels[chIdx].lastRailingResult = int.MaxValue;
                    _eegChannels[chIdx].lastImpedanceResult = int.MaxValue;
                    _eegChannels[chIdx].signalStatus = SignalStatus.SIGNAL_ERROR;
                    _eegChannels[chIdx].signalQualityUpdatedCurrentSession = 0;

                    // Get corresponding railing and impedance UI elements and initialize with default values
                    _eegChannels[chIdx].electrodeCap = _electrodeCapMap[electrodeName];
                    _eegChannels[chIdx].electrodeCap.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
                    _eegChannels[chIdx].electrodeCap.BorderColor = Color.White;
                    _eegChannels[chIdx].electrodeRailingTest = _optionalListElectrodesRailingTest[intOptionalChnOffset];
                    _eegChannels[chIdx].electrodeRailingTest.Text = electrodeName;
                    _eegChannels[chIdx].chartSignalDataRailingTest = _optionalListChartsSignalDataRailingTest[intOptionalChnOffset];
                    _eegChannels[chIdx].textRailingResultRailingTest = _optionalListTextsRailingResultsRailingTest[intOptionalChnOffset];
                    _eegChannels[chIdx].electrodeImpedanceTest = _optionalListElectrodesImpedanceTest[intOptionalChnOffset];
                    _eegChannels[chIdx].electrodeImpedanceTest.Text = electrodeName;
                    _eegChannels[chIdx].impedanceResultImpedanceTest = _optionalListImpedanceResultsImpedanceTest[intOptionalChnOffset];
                    _eegChannels[chIdx].electrodeQualityResults = _optionalListElectrodesQualityResults[intOptionalChnOffset];
                    _eegChannels[chIdx].electrodeQualityResults.Text = electrodeName;
                    _eegChannels[chIdx].railingResultQualityResults = _optionalListRailingResultsQualityResults[intOptionalChnOffset];
                    _eegChannels[chIdx].impedanceResultQualityResults = _optionalListImpedanceResultsQualityResults[intOptionalChnOffset];

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

            // Recheck is required for signal quality tests and calibration
            if (maxTimeHasElapsed || !userPassedLastSignalQualityCheck)
            {
                // Update initial text shown in impedance testing tab depending on result from previous tests
                if (maxTimeHasElapsed)
                {
                    // Do not show anything to user in impedance test screen
                    updateImpedanceTestingStateLabels(ImpedanceTestingState.NOT_RUNNING_MAX_TIME_ELAPSED,
                        (double)maxTimeMins);
                }
                else if (!maxTimeHasElapsed && !userPassedLastSignalQualityCheck)
                {
                    // Update with text telling user they failed previous signal quality check
                    updateImpedanceTestingStateLabels(ImpedanceTestingState.NOT_RUNNING_FAILED_LAST_SIGNAL_QUALITY_CHECK,
                        (double)maxTimeMins);
                }
            }
            else if (!maxTimeHasElapsed && userPassedLastSignalQualityCheck)
            {
                // Do not update UI with saved impedance values

                // Update UI with time since most recent test
                updateImpedanceTestingStateLabels(ImpedanceTestingState.NOT_RUNNING_MAX_TIME_NOT_ELAPSED, (double)minElapsedPrevSignalQualityCheck);
            }
        }

        /// <summary>
        /// Send lower level commands to Cyton board to enable / disable impedance testing
        /// </summary>
        //private async Task StartImpedanceTesting()
        private void StartImpedanceTesting()
        {
            Log.Debug("StartImpedanceTesting");

            if (!_runImpedanceTestingCycle && DAQ_OpenBCI.deviceInitialized)
            {
                _runImpedanceTestingCycle = true;
                _impedanceTestingRunning = true;
                _currentImpedenceTestElectrodeIndex = 0;

                ////// Before running impedence tests ///////
                //// Stop streaming on board, does not consistently register commands while streaming
                Log.Debug("Stop streaming");
                DAQ_OpenBCI.Stop_Streaming();
                Thread.Sleep(50);

                while (_runImpedanceTestingCycle)
                {
                    _impedanceTestingRunning = true;

                    EEGChannel currentEegChannel = _eegChannels[_currentImpedenceTestElectrodeIndex];
                    String electrodeName = currentEegChannel._electrodeName;

                    Log.Debug("StartImpedanceTesting loop | _currentImpedenceTestElectrodeIndex: " + _currentImpedenceTestElectrodeIndex.ToString() +
                        " | electrodeName: " + electrodeName.ToString());

                    String cmdStartElectrodeImpedenceTest = currentEegChannel.ImpedanceTestingEnableCmd;
                    String cmdEndElectrodeImpedenceTest = currentEegChannel.ImpedanceTestingDisableCmd;

                    /////// ******* NOTE - Send impedence enable command multiple times back to back to increase duration of tests (1 command too short) ******* ///////
                    //String cmdEndElectrodeImpedenceTest = cmdStartElectrodeImpedenceTest + cmdStartElectrodeImpedenceTest;

                    // Set back color of impedance result button in Impedance testing page to orange
                    Invoke(new Action(() =>
                    {
                        currentEegChannel.impedanceResultImpedanceTest.BackColor = Color.DarkOrange;
                    }));

                    DAQ_OpenBCI.GetData(); // Clear buffer

                    //// Send enable electrode impedence testing commands
                    Log.Debug(String.Format("Sending enable electrode {0} impedence testing command: {1}", electrodeName, cmdStartElectrodeImpedenceTest));
                    DAQ_OpenBCI.Config_Board(cmdStartElectrodeImpedenceTest);
                    Thread.Sleep(750);

                    // Make sure DAQ_OpenBCI.deviceInitialized set to true when streaming enabled
                    DAQ_OpenBCI.deviceInitialized = true;

                    //// Send start streaming
                    Log.Debug("Start streaming");
                    DAQ_OpenBCI.Start_Streaming();
                    Thread.Sleep(50);

                    // Wait for a bit then send stop streaming and disable impedence testing commands
                    long currentTimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    long endImpudenceTestTimestamp = currentTimestamp + 2000;
                    while (currentTimestamp < endImpudenceTestTimestamp)
                    {
                        Thread.Sleep(25);
                        currentTimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    }

                    // Set DAQ_OpenBCI.deviceInitialized to false to stop processing data when streaming stopped
                    // Will be set to true again during DAQ_OpenBCI.Start()
                    DAQ_OpenBCI.deviceInitialized = false;

                    //// Stop streaming
                    Log.Debug("Stop streaming");
                    DAQ_OpenBCI.Stop_Streaming();
                    Thread.Sleep(50);

                    // Send command to disable impedance testing for specific electrode
                    Log.Debug(String.Format("Sending disable electrode {0} impedence testing command: {1}", electrodeName, cmdEndElectrodeImpedenceTest));
                    DAQ_OpenBCI.Config_Board(cmdEndElectrodeImpedenceTest);
                    Thread.Sleep(750);
                    Log.Debug("Completed impedence testing electrode: " + _currentImpedenceTestElectrodeIndex.ToString());

                    // Reset back color of impedance result button in Impedance testing page to transparent
                    Invoke(new Action(() =>
                    {
                        currentEegChannel.impedanceResultImpedanceTest.BackColor = Color.Transparent;
                    }));

                    // Increment electode index for impedence testing
                    _currentImpedenceTestElectrodeIndex += 1;
                    if (_currentImpedenceTestElectrodeIndex >= _numChannels)
                    {
                        _currentImpedenceTestElectrodeIndex = 0;
                        if (BCIActuatorSettings.Settings.SignalQuality_StopImpedanceTestAfterOneCycle)
                        {
                            Log.Debug("SignalQuality_StopImpedanceTestAfterOneCycle = true | Stopping impedance testing");
                            try
                            {
                                Invoke(new Action(() =>
                                {
                                    buttonTestImpedance.Enabled = false;
                                    buttonTestImpedance.BackColor = Color.Gray;
                                    updateImpedanceTestingStateLabels(ImpedanceTestingState.STOP_IN_PROGRESS);
                                }));

                                // Set impedance testing flag to false - exits this loop
                                _runImpedanceTestingCycle = false;
                            }
                            catch (Exception ex)
                            {
                                Log.Debug(ex.Message);
                            }
                        }
                    }
                }

                //// Stopping impedence testing process -  _runImpedanceTestingCycle set to false
                //// Do opposite of what was done at the beginning on this function to bring board back to default state

                // Reset board to default parameters
                Log.Debug("Send command to reset board");
                DAQ_OpenBCI.Reset_Board(); // Run multiple times? DAQ_OpenBCI.Reset_Board();
                Thread.Sleep(750); // Tested 750 - is ok
                Log.Debug("Send command to reset board");
                DAQ_OpenBCI.Reset_Board();
                Thread.Sleep(4500);

                Log.Debug("Calling DAQ_OpenBCI.Stop()");
                DAQ_OpenBCI.Stop();
                Thread.Sleep(250); // Tested 250 - is ok

                Log.Debug("Calling DAQ_OpenBCI.Start()");
                DAQ_OpenBCI.Start(); // Also starts streaming

                // Stopped Impedance testing cycle, update UI accordingly
                Invoke(new Action(() =>
                {
                    buttonTestImpedance.Text = "Start";
                    buttonTestImpedance.Enabled = true;
                    buttonTestImpedance.BackColor = COLOR_ACAT_DEFAULT_ORANGE;
                    buttonNext.Enabled = true;
                    buttonNext.BackColor = COLOR_ACAT_DEFAULT_ORANGE;
                    buttonExit.Enabled = true;
                    buttonExit.BackColor = Color.Transparent;
                    updateImpedanceTestingStateLabels(ImpedanceTestingState.NOT_RUNNING);
                }));

                // Done automatically everytime impedance testing completed
                // If all electrodes are updated within this session
                // save this time as time of last signal quality check completed
                if (_AllElectrodesOverallSignalQualityResult.allElectrodesUpdatedWithinSession == true)
                {
                    Log.Debug("Saving current time as SignalQuality_TimeOfLastImpedanceCheck");
                    BCIActuatorSettings.Settings.SignalQuality_TimeOfLastImpedanceCheck = DateTimeOffset.Now.ToUnixTimeSeconds();
                    BCIActuatorSettings.Save(); // Save settings
                }

                // Set flag providing current impedance testing state
                _impedanceTestingRunning = false;
            }
        }

        /// <summary>
        /// Function called for timer that processes data for BCI signal check
        /// </summary>
        /// <param name="data"></param>
        public void ProcessDataSignalCheck(double[,] data, double[,] filteredData)
        {
            if (BCIDeviceTester._endSignalCheckTimer)
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

                        String railingResPercentage_format = railingResultPercentage.ToString() + "%";
                        _eegChannels[chIdx].textRailingResultRailingTest.Text = railingResPercentage_format;
                        _eegChannels[chIdx].textRailingResultRailingTest.ForeColor = railingResultColor;

                        _eegChannels[chIdx].railingResultQualityResults.Text = railingResPercentage_format;
                        _eegChannels[chIdx].railingResultQualityResults.ForeColor = railingResultColor;
                        _eegChannels[chIdx].railingResultQualityResults.BorderColor = railingResultColor;
                    }));
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        /// <summary>
        /// Update impedance value and update UI elements
        /// </summary>
        private void updateImpedanceResult(int chIdx, int impedanceResult, bool update_ui, bool loadedSavedValue)
        {
            try
            {
                BCIActuatorSettings.Settings.SignalQuality_LastImpedanceValues[chIdx] = impedanceResult;

                if (update_ui && impedanceResult != int.MaxValue)
                {
                    String impedanceResultText = impedanceResult.ToString();

                    // Update from initializeBCISignalCheck (main UI thread) - no invoke required
                    if (loadedSavedValue)
                    {
                        // Set text + colors for impedance result buttons on impedance testing and quality resulst pages
                        // White for text + border, transparent background
                        _eegChannels[chIdx].impedanceResultImpedanceTest.Text = impedanceResultText;
                        _eegChannels[chIdx].impedanceResultImpedanceTest.ForeColor = Color.White;
                        _eegChannels[chIdx].impedanceResultImpedanceTest.BorderColor = Color.White;
                        _eegChannels[chIdx].impedanceResultImpedanceTest.BackColor = Color.Transparent;

                        _eegChannels[chIdx].impedanceResultQualityResults.Text = impedanceResultText;
                        _eegChannels[chIdx].impedanceResultQualityResults.ForeColor = Color.White;
                        _eegChannels[chIdx].impedanceResultQualityResults.BorderColor = Color.White;
                        _eegChannels[chIdx].impedanceResultQualityResults.BackColor = Color.Transparent;
                    }
                    else
                    {
                        // Update from separate async task - invoke required
                        Invoke(new Action(() =>
                        {
                            Color impedanceResultColor = SelectColorFromStatus(SignalStatus.SIGNAL_ERROR);
                            if (impedanceResult <= BCIActuatorSettings.Settings.SignalQuality_ImpedanceGoodMaxThreshold)
                                impedanceResultColor = SelectColorFromStatus(SignalStatus.SIGNAL_OK);
                            else if (impedanceResult > BCIActuatorSettings.Settings.SignalQuality_ImpedanceGoodMaxThreshold &&
                                impedanceResult <= BCIActuatorSettings.Settings.SignalQuality_ImpedanceOkMaxThreshold)
                                impedanceResultColor = SelectColorFromStatus(SignalStatus.SIGNAL_ACCEPTABLE);
                            else if (impedanceResult > BCIActuatorSettings.Settings.SignalQuality_ImpedanceOkMaxThreshold)
                                impedanceResultColor = SelectColorFromStatus(SignalStatus.SIGNAL_KO);

                            // Set text and colors for impedance result buttons on impedance testing and quality results pages (orange back color set earlier)
                            _eegChannels[chIdx].impedanceResultImpedanceTest.Text = impedanceResultText;
                            _eegChannels[chIdx].impedanceResultImpedanceTest.ForeColor = impedanceResultColor;
                            _eegChannels[chIdx].impedanceResultImpedanceTest.BorderColor = impedanceResultColor;
                            _eegChannels[chIdx].electrodeImpedanceTest.BackColor = impedanceResultColor;

                            _eegChannels[chIdx].impedanceResultQualityResults.Text = impedanceResultText;
                            _eegChannels[chIdx].impedanceResultQualityResults.ForeColor = impedanceResultColor;
                            _eegChannels[chIdx].impedanceResultQualityResults.BorderColor = impedanceResultColor;
                        }));
                    }
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
            // If tab is changed while doing impedance testing - go back to Impedance Testing tab
            if (_impedanceTestingRunning)
            {
                _currentBCISignalCheckMode = BCISignalCheckMode.TEST_IMPEDANCE;
                tabControlSignalQuality.SelectedIndex = 1;
                highlightSelectedTab(1);

                if (_runImpedanceTestingCycle)
                    updateImpedanceTestingStateLabels(ImpedanceTestingState.RUNNING_TAB_SWITCH_ATTEMPTED);
                else
                    updateImpedanceTestingStateLabels(ImpedanceTestingState.STOP_IN_PROGRESS_TAB_SWITCH_ATTEMPTED);
            }
            else
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
            }

            Log.Debug("tabControlElectrodeQuality_SelectedIndexChanged" +
                " | _impedanceTestingRunning: " + _impedanceTestingRunning.ToString() +
                " | _currentBCISignalCheckMode: " + _currentBCISignalCheckMode.ToString());
        }

        /// <summary>
        /// Helper function to highlight the current tab selected
        /// </summary>
        /// <param name="tabControlIndex"></param>
        private void highlightSelectedTab(int tabControlIndex)
        {
            try
            {
                switch (tabControlIndex)
                {
                    case 0:
                        tabPageRailing.Text = "- 1. Railing -";
                        tabPageImpedance.Text = "2. Impedance";
                        tabPageQuality.Text = "Quality";
                        break;

                    case 1:
                        tabPageRailing.Text = "1. Railing";
                        tabPageImpedance.Text = "- 2. Impedance -";
                        tabPageQuality.Text = "Quality";
                        break;

                    case 2:
                        tabPageRailing.Text = "1. Railing";
                        tabPageImpedance.Text = "2. Impedance";
                        tabPageQuality.Text = "- Quality -";
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
            }
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
        /// Handler for when Impedance Test button pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonTestImpedance_Click(object sender, EventArgs e)
        {
            Log.Debug("buttonTestImpedance_Click | _currentImpedenceTestElectrodeIndex: " + _currentImpedenceTestElectrodeIndex.ToString());

            if (_runImpedanceTestingCycle)
            {
                try
                {
                    Log.Debug("Impedence cyclical testing running. Stopping process...");
                    buttonTestImpedance.Enabled = false;
                    buttonTestImpedance.BackColor = Color.Gray;
                    updateImpedanceTestingStateLabels(ImpedanceTestingState.STOP_IN_PROGRESS);

                    // Set impedance testing flag to false - exits thread doing impedance testing
                    _runImpedanceTestingCycle = false;
                }
                catch (Exception ex)
                {
                    Log.Debug(ex.Message);
                }
            }
            else if (!_runImpedanceTestingCycle)
            {
                // Start impedance testing
                try
                {
                    Log.Debug("Impedence testing not running. Starting process...");
                    buttonTestImpedance.Text = "Stop";
                    buttonNext.Enabled = false;
                    buttonNext.BackColor = Color.Gray;
                    buttonExit.Enabled = false;
                    buttonExit.BackColor = Color.Gray;
                    updateImpedanceTestingStateLabels(ImpedanceTestingState.RUNNING);
                }
                catch (Exception ex)
                {
                    Log.Debug(ex.Message);
                }

                // Start thread doing impedance testing
                Thread t = new Thread(() => StartImpedanceTesting());
                t.Start();
            }
        }

        /// <summary>
        /// Update texts related to impedance testing
        /// </summary>
        /// <param name="impedanceTestingState"></param>
        /// <param name="valueMinutes"></param>
        private void updateImpedanceTestingStateLabels(ImpedanceTestingState impedanceTestingState, double valueMinutes = -1)
        {
            try
            {
                switch (impedanceTestingState)
                {
                    case ImpedanceTestingState.NOT_RUNNING:
                        labelImpedanceTestingState1.Text = "";
                        labelImpedanceTestingState2.Text = "";
                        labelImpedanceTestingState3.Text = "";
                        labelImpedanceTestingState4.Text = "Press Start to begin";
                        labelImpedanceTestingState5.Text = "impedance testing";
                        break;

                    case ImpedanceTestingState.NOT_RUNNING_MAX_TIME_ELAPSED:
                        labelImpedanceTestingState1.Text = "";
                        labelImpedanceTestingState2.Text = "";
                        labelImpedanceTestingState3.Text = "";
                        labelImpedanceTestingState4.Text = "Press Start to begin";
                        labelImpedanceTestingState5.Text = "impedance testing";
                        break;

                    case ImpedanceTestingState.NOT_RUNNING_FAILED_LAST_SIGNAL_QUALITY_CHECK:
                        labelImpedanceTestingState1.Text = "You did not pass";
                        labelImpedanceTestingState2.Text = "your most recent";
                        labelImpedanceTestingState3.Text = "signal quality check";
                        labelImpedanceTestingState4.Text = "Press Start to begin";
                        labelImpedanceTestingState5.Text = "impedance testing";
                        break;

                    case ImpedanceTestingState.NOT_RUNNING_MAX_TIME_NOT_ELAPSED:
                        labelImpedanceTestingState1.Text = "It has been";
                        if (valueMinutes > 0)
                            labelImpedanceTestingState2.Text = String.Format("{0:00} minutes", valueMinutes);
                        else
                            labelImpedanceTestingState2.Text = "a long time";
                        labelImpedanceTestingState3.Text = "since your last test";
                        labelImpedanceTestingState4.Text = "Press Start to begin";
                        labelImpedanceTestingState5.Text = "impedance testing";
                        break;

                    case ImpedanceTestingState.RUNNING:
                        labelImpedanceTestingState1.Text = "";
                        labelImpedanceTestingState2.Text = "";
                        labelImpedanceTestingState3.Text = "";
                        labelImpedanceTestingState4.Text = "Impedance tests running";
                        labelImpedanceTestingState5.Text = "Press Stop to end";
                        break;

                    case ImpedanceTestingState.RUNNING_TAB_SWITCH_ATTEMPTED:
                        labelImpedanceTestingState1.Text = "Cannot continue or exit";
                        labelImpedanceTestingState2.Text = "while testing in progress";
                        labelImpedanceTestingState3.Text = "";
                        labelImpedanceTestingState4.Text = "Impedance tests running";
                        labelImpedanceTestingState5.Text = "Press Stop to end";
                        break;

                    case ImpedanceTestingState.STOP_IN_PROGRESS:
                        labelImpedanceTestingState1.Text = "";
                        labelImpedanceTestingState2.Text = "";
                        labelImpedanceTestingState3.Text = "";
                        labelImpedanceTestingState4.Text = "Stopping impedance tests...";
                        labelImpedanceTestingState5.Text = "Please Wait";
                        break;

                    case ImpedanceTestingState.STOP_IN_PROGRESS_TAB_SWITCH_ATTEMPTED:
                        labelImpedanceTestingState1.Text = "Cannot continue or exit";
                        labelImpedanceTestingState2.Text = "while testing in progress";
                        labelImpedanceTestingState3.Text = "";
                        labelImpedanceTestingState4.Text = "Stopping impedance tests...";
                        labelImpedanceTestingState5.Text = "Please Wait";
                        break;

                    default:
                        labelImpedanceTestingState1.Text = "";
                        labelImpedanceTestingState2.Text = "";
                        labelImpedanceTestingState3.Text = "";
                        labelImpedanceTestingState4.Text = "";
                        labelImpedanceTestingState5.Text = "";
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.Message);
            }
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
                foreach (Label labelResize in new List<Label> { labelBCISignalCheck, labelRailingTest, labelImpedanceTest, labelQualityResults })
                {
                    labelResize.Font = new System.Drawing.Font("Montserrat", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                }

                // Bold smaller texts
                foreach (Label labelResize in new List<Label> { labelRailingTestInfo2, labelImpedanceTestInfo2, labelQualityResultsInfo2 })
                {
                    labelResize.Font = new System.Drawing.Font("Montserrat SemiBold", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                }

                // Normal smaller texts
                foreach (Label labelResize in new List<Label> { labelBCISignalCheckDescription, labelRailingTestInfo1, labelRailingTestInfo3, labelImpedanceTestInfo1, labelQualityResultsInfo1 })
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