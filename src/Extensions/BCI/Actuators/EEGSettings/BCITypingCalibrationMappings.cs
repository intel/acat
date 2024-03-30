////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// TypingCalibrationMappings
//
// Typing section - calibration classifier mappings
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
    public class BCITypingCalibrationMappings : PreferencesBase
    {
        /// <summary>
        /// Name of the settings file
        /// </summary>
        [NonSerialized, XmlIgnore]
        public static String SettingsFilePath;

        /// <summary>
        /// Calibration used for Box typing section
        /// </summary>
        public String BoxCalibrationMapping;

        /// <summary>
        /// Calibration used for Word typing section
        /// </summary>
        public String WordCalibrationMapping;

        /// <summary>
        /// Calibration used for Sentence typing section
        /// </summary>
        public String SentenceCalibrationMapping;

        /// <summary>
        /// Calibration used for KeyrboardL typing section
        /// </summary>
        public String KeyboardLCalibrationMapping;

        /// <summary>
        /// Calibration used for KeyboardR typing section
        /// </summary>
        public String KeyboardRCalibrationMapping;

        public BCITypingCalibrationMappings()
        {
            BoxCalibrationMapping = BCIScanSections.Box.ToString();
            WordCalibrationMapping = BCIScanSections.Sentence.ToString();
            SentenceCalibrationMapping = BCIScanSections.Sentence.ToString();
            KeyboardLCalibrationMapping = BCIScanSections.KeyboardL.ToString();
            KeyboardRCalibrationMapping = BCIScanSections.KeyboardL.ToString();
        }

        /// <summary>
        /// Loads the settings from the settings file
        /// </summary>
        /// <returns>true on success</returns>
        public static BCITypingCalibrationMappings Load()
        {
            BCITypingCalibrationMappings retVal = PreferencesBase.Load<BCITypingCalibrationMappings>(SettingsFilePath);
            Save(retVal, SettingsFilePath);
            return retVal;
        }

        /// <summary>
        /// Saves settings
        /// </summary>
        /// <returns>true on success</returns>
        public override bool Save()
        {
            return Save<BCITypingCalibrationMappings>(this, SettingsFilePath);
        }
    }
}