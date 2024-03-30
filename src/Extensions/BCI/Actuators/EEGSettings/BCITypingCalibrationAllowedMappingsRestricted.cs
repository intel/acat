////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BCITypingCalibrationAllowedMappingsRestricted.xml
//
// List of all allowed typing section - calibration classifier mappings for Restricted Mode
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Extensions.BCI.Common.BCIControl;
using ACAT.Lib.Core.PreferencesManagement;
using System;
using System.Collections.Generic;

namespace ACAT.Extensions.BCI.Actuators.EEG.EEGSettings
{
    /// <summary>
    /// Settings for the Sample Actuator
    /// </summary>
    [Serializable]
    public class BCITypingCalibrationAllowedMappingsRestricted : PreferencesBase
    {
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

        public BCITypingCalibrationAllowedMappingsRestricted()
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
        public static BCITypingCalibrationAllowedMappingsRestricted Load()
        {
            BCITypingCalibrationAllowedMappingsRestricted retVal = PreferencesBase.Load<BCITypingCalibrationAllowedMappingsRestricted>(SettingsFilePath);

            if (retVal.Box.Count == 0)
                retVal.Box = new List<String>() { BCIScanSections.Box.ToString() };

            if (retVal.Word.Count == 0)
                retVal.Word = new List<String>() { BCIScanSections.Word.ToString(), BCIScanSections.Sentence.ToString() };

            if (retVal.Sentence.Count == 0)
                retVal.Sentence = new List<String>() { BCIScanSections.Word.ToString(), BCIScanSections.Sentence.ToString(), };

            if (retVal.KeyboardL.Count == 0)
                retVal.KeyboardL = new List<String>() { BCIScanSections.KeyboardL.ToString(), BCIScanSections.KeyboardR.ToString() };

            if (retVal.KeyboardR.Count == 0)
                retVal.KeyboardR = new List<String>() { BCIScanSections.KeyboardL.ToString(), BCIScanSections.KeyboardR.ToString() };

            Save(retVal, SettingsFilePath);
            return retVal;
        }

        /// <summary>
        /// Saves settings
        /// </summary>
        /// <returns>true on success</returns>
        public override bool Save()
        {
            return Save<BCITypingCalibrationAllowedMappingsRestricted>(this, SettingsFilePath);
        }
    }
}