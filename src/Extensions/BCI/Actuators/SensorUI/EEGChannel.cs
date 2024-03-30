using ACAT.Extensions.BCI.Common.BCIControl;
using ACAT.Lib.Core.WidgetManagement;
using System;
using System.Windows.Forms.DataVisualization.Charting;

////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// EEGChannel.cs
//
// Holds information for each channel / electrode including name, enabled status,
// most recent railing and impedance values
//
////////////////////////////////////////////////////////////////////////////

namespace SensorUI
{
    /// <summary>
    /// Holds information for each channel / electrode including name, enabled status,
    /// most recent railing and impedance values
    /// </summary>
    public class EEGChannel
    {
        /// <summary>
        /// Short name / id (ex: Pz, C3, etc.) of channel
        /// </summary>
        public String _electrodeName;

        /// <summary>
        /// Index of this channel out of the possible 16
        /// </summary>
        public int _channelIndex;

        /// <summary>
        /// Where in data returned from cyton read, is this channel's data
        /// </summary>
        public int _channelRawDataIndex;

        /// <summary>
        /// Whether channel is enabled / disabled with config parameter Classifier_EnableChannelX 
        /// </summary>
        public bool _channelEnabled;

        /// <summary>
        /// Most recent railing test result
        /// </summary>
        public double lastRailingResult;

        /// <summary>
        /// Most recent impedance test result
        /// </summary>
        public double lastImpedanceResult;

        /// <summary>
        /// Signal quality status
        /// </summary>
        public SignalStatus signalStatus;

        /// <summary>
        /// Flag denoting whether electrode is one of the (required) base 8 channels or part of the extended 8 channels (daisy board)
        /// </summary>
        public bool isRequiredElectrode = false;

        /// <summary>
        /// Have we successfully computed new signal quality result for this electrode within this session?
        /// </summary>
        public int signalQualityUpdatedCurrentSession = 0;

        /// <summary>
        /// Commands to enable impedance testing for this specific electrode 
        /// </summary>
        public String ImpedanceTestingEnableCmd;

        /// <summary>
        /// Command to disable impedance testing for this specific electrode 
        /// </summary>
        public String ImpedanceTestingDisableCmd;

        /// <summary>
        /// Time in seconds status was obtained for this channel
        /// </summary>
        public long timeLastUpdatedSec = 0;

        /// <summary>
        /// Time in minutes status was obtained for this channel
        /// </summary>
        public int timeLastUpdatedMin = 0;

        // UI elements associated with each channel
        public ScannerRoundedButtonControl electrodeCap;
        public ScannerRoundedButtonControl electrodeRailingTest;
        public Chart chartSignalDataRailingTest;
        public Title textRailingResultRailingTest;
        public ScannerRoundedButtonControl electrodeImpedanceTest;
        public ScannerRoundedButtonControl impedanceResultImpedanceTest;
        public ScannerRoundedButtonControl electrodeQualityResults;
        public ScannerRoundedButtonControl railingResultQualityResults;
        public ScannerRoundedButtonControl impedanceResultQualityResults;

        public EEGChannel(String electrodeName, int channelIndex, bool enableChannel)
        {
            _electrodeName = electrodeName;
            _channelIndex = channelIndex;
            _channelEnabled = enableChannel;
            lastRailingResult = int.MaxValue;
            lastImpedanceResult = int.MaxValue;
            signalStatus = SignalStatus.SIGNAL_ERROR;
            signalQualityUpdatedCurrentSession = 0;
        }
    }
}