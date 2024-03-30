////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BCISettingsFixed.cs
//
// Settings that can't be changed for the user
// This settings are updated automatically based on other settings set by the user
// or sensor configurations
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Extensions.BCI.Common.BCIControl;
using System.Collections.Generic;

namespace ACAT.Extensions.BCI.Actuators.EEG.EEGSettings
{
    /// <summary>
    /// Hardcoded settings
    /// </summary>
    public static class BCISettingsFixed
    {
        /// <summary>
        /// Id of the sensor. Use 0 for cyton, 2 for cyton + daisy (16ch)
        /// This settings is updated automatically based on DAQ_NumEEGChannels
        /// </summary>
        public static int DAQ_SensorId = 0;

        /// <summary>
        /// Sample rate of the device
        /// This settings is updated automatically based on DAQ_NumEEGChannels
        /// </summary>
        public static int DAQ_SampleRate = 250; //This is set to 250Hz

        /// <summary>
        /// Downsample rate
        /// This settings is updated automatically based on DAQ_NumEEGChannels
        /// </summary>
        public static int DimReduct_DownsampleRate = 2;

        /// <summary>
        /// Data parser: number of lines in header. Default: 7
        /// </summary>
        public static int DataParser_HeaderLinesToSkip = 7;

        /// <summary>
        /// Data parser: column index where sampleIdx is stored. Default: 0
        ///  This settings is updated automatically based on DAQ_NumEEGChannels
        /// </summary>
        public static int DataParser_IdxSampleIdx = 0;

        /// <summary>
        /// Data parser: column index where EEG data starts. Default: 2
        ///  This settings is updated automatically based on DAQ_NumEEGChannels
        /// </summary>
        public static int DataParser_IdxStartEEGData = 2;

        /// <summary>
        /// Data parser: column index where EEG data starts. Default: 16
        ///  This settings is updated automatically based on DAQ_NumEEGChannels
        /// </summary>
        public static int DataParser_IdxTriggerSignal_Hw = 16;

        /// <summary>
        /// Data parser: column index where EEG data starts. Default: 24
        ///  This settings is updated automatically based on DAQ_NumEEGChannels
        /// </summary>
        public static int DataParser_IdxTriggerSignal_Sw = 24;

        /// <summary>
        /// Recheck needed when user has added new gel, repositioned the cap, etc
        /// </summary>
        public static bool SignalControl_RecheckNeeded = false;

        /// <summary>
        /// List of default classifiers
        /// </summary>
        public static List<BCIScanSections> Classifier_DefaultClassifiers = new List<BCIScanSections>() { BCIScanSections.Box, BCIScanSections.Sentence, BCIScanSections.KeyboardL };
    }
}