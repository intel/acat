////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BCITypingCalibrationAllowedMappingsAdvanced.xml
//
// List of all allowed typing section - calibration classifier mappings for Advance mode
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Extensions.BCI.Common.BCIControl;
using ACAT.Lib.Core.PreferencesManagement;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ACAT.Extensions.BCI.Actuators.EEG.EEGSettings
{
    /// <summary>
    /// Settings for the Sample Actuator
    /// </summary>
    [Serializable]
    public class BCITypingCalibrationAllowedMappingsAdvanced : PreferencesBase
    {
        [NonSerialized, XmlIgnore]
        public static String SettingsFilePath;

        /// <summary>
        /// List of allowed calibrations for Box typing section
        /// </summary>
        public List<String> Box;

        /// <summary>
        /// List of allowed calibrations for Word typing section
        /// </summary>
        public List<String> Word;

        /// <summary>
        /// List of allowed calibrations for Sentence typing section
        /// </summary>
        public List<String> Sentence;

        /// <summary>
        /// List of allowed calibrations for KeyboardL typing section
        /// </summary>
        public List<String> KeyboardL;

        /// <summary>
        /// List of allowed calibrations for KeyboardR typing section
        /// </summary>
        public List<String> KeyboardR;

        public BCITypingCalibrationAllowedMappingsAdvanced()
        {
            Box = new List<String>();
            Word = new List<String>();
            Sentence = new List<String>();
            KeyboardL = new List<String>();
            KeyboardR = new List<String>();
        }

        /// <summary>
        /// Loads the settings from the settings file
        /// </summary>
        /// <returns>true on success</returns>
        public static BCITypingCalibrationAllowedMappingsAdvanced Load()
        {
            BCITypingCalibrationAllowedMappingsAdvanced retVal = PreferencesBase.Load<BCITypingCalibrationAllowedMappingsAdvanced>(SettingsFilePath);
            List<String> allClassifiers = new List<String>() { BCIScanSections.Box.ToString(), BCIScanSections.Word.ToString(), BCIScanSections.Sentence.ToString(), BCIScanSections.KeyboardL.ToString(), BCIScanSections.KeyboardR.ToString() };

            if (retVal.Box.Count == 0)
                retVal.Box = allClassifiers;

            if (retVal.Word.Count == 0)
                retVal.Word = allClassifiers;

            if (retVal.Sentence.Count == 0)
                retVal.Sentence = allClassifiers;

            if (retVal.KeyboardL.Count == 0)
                retVal.KeyboardL = allClassifiers;

            if (retVal.KeyboardR.Count == 0)
                retVal.KeyboardR = allClassifiers;

            Save(retVal, SettingsFilePath);
            return retVal;
        }

        /// <summary>
        /// Saves settings
        /// </summary>
        /// <returns>true on success</returns>
        public override bool Save()
        {
            return Save<BCITypingCalibrationAllowedMappingsAdvanced>(this, SettingsFilePath);
        }
    }
}