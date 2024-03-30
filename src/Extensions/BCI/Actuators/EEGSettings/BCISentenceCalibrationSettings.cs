////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BoxCalibrationSettings.cs
//
// Settings for the BCI Actuator
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Extensions.BCI.Common.BCIControl;
using ACAT.Lib.Core.PreferencesManagement;
using System;
using System.Xml.Serialization;

namespace ACAT.Extensions.BCI.Actuators.EEG.EEGSettings
{
    /// <summary>
    /// Settings for the Sample Actuator
    /// </summary>
    [Serializable]
    public class BCISentenceCalibrationSettings : PreferencesBase
    {
        /// <summary>
        /// Name of the settings file
        /// </summary>
        [NonSerialized, XmlIgnore]
        public static String SettingsFilePath;

        /// <summary>
        /// Calibration section/mode: Box
        /// </summary>
        public String CalibrationMode;

        /// <summary>
        /// Scan time for the Box scanning
        /// </summary>
        public int ScanTime;

        /// <summary>
        /// Number of targets for Box calibration
        /// </summary>
        public int NumberOfTargets;

        /// <summary>
        /// Number of iterations per target for Box calibration
        /// </summary>
        public int NumberOfIterationsPerTarget;

        /// <summary>
        /// Minimum score required (1-100) to pass Box calibration
        /// </summary>
        public int MinimumScoreRequired;

        public BCISentenceCalibrationSettings()
        {
            CalibrationMode = BCIScanSections.Sentence.ToString();
            ScanTime = 200;
            NumberOfTargets = 10;
            NumberOfIterationsPerTarget = 10;
            MinimumScoreRequired = 70;
        }

        /// <summary>
        /// Loads the settings from the settings file
        /// </summary>
        /// <returns>true on success</returns>
        public static BCISentenceCalibrationSettings Load()
        {
            BCISentenceCalibrationSettings retVal = PreferencesBase.Load<BCISentenceCalibrationSettings>(SettingsFilePath);
            Save(retVal, SettingsFilePath);
            return retVal;
        }

        /// <summary>
        /// Saves settings
        /// </summary>
        /// <returns>true on success</returns>
        public override bool Save()
        {
            return Save<BCISentenceCalibrationSettings>(this, SettingsFilePath);
        }
    }
}