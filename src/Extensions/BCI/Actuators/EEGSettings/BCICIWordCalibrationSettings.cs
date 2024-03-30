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
    public class BCIWordCalibrationSettings : PreferencesBase
    {
        /// <summary>
        /// Name of the settings file
        /// </summary>
        [NonSerialized, XmlIgnore]
        public static String SettingsFilePath;

        /// <summary>
        /// Calibration section/mode: Word
        /// </summary>
        public String CalibrationMode;

        /// <summary>
        /// Scan time for word scanning
        /// </summary>
        public int ScanTime;

        /// <summary>
        /// Number of targets for Word calibration
        /// </summary>
        public int NumberOfTargets;

        /// <summary>
        /// Number of iterations per target for Word calibration
        /// </summary>
        public int NumberOfIterationsPerTarget;

        /// <summary>
        /// Minimum score required (1-100) to pass Word calibration
        /// </summary>
        public int MinimumScoreRequired;

        public BCIWordCalibrationSettings()
        {
            CalibrationMode = BCIScanSections.Word.ToString();
            ScanTime = 200;
            NumberOfTargets = 10;
            NumberOfIterationsPerTarget = 10;
            MinimumScoreRequired = 70;
        }

        /// <summary>
        /// Loads the settings from the settings file
        /// </summary>
        /// <returns>true on success</returns>
        public static BCIWordCalibrationSettings Load()
        {
            BCIWordCalibrationSettings retVal = PreferencesBase.Load<BCIWordCalibrationSettings>(SettingsFilePath);
            Save(retVal, SettingsFilePath);
            return retVal;
        }

        /// <summary>
        /// Saves settings
        /// </summary>
        /// <returns>true on success</returns>
        public override bool Save()
        {
            return Save<BCIWordCalibrationSettings>(this, SettingsFilePath);
        }
    }
}