////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// Settings.cs
//
// Preference settings for the ConvAssist word predictor (English)
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.PreferencesManagement;
using ACAT.Core.Utility;
using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace ACAT.Extensions.WordPredictors.ConvAssist
{
    /// <summary>
    /// Preference settings for the ConvAssist word predictor (English)
    /// </summary>
    [Serializable]
    public class Settings : PreferencesBase
    {
        [Descriptor("A string of characters that should be filtered out from the predicted words, eg, punctuations")]
        [UIHint("TextBox")]
        public String FilterChars = String.Empty;

        /// <summary>
        /// Path to the file where preferences are stored
        /// </summary>
        [NonSerialized, XmlIgnore]
        public static String PreferencesFilePath;

        /// <summary>
        /// Set this to true if the language uses diacritics
        /// </summary>
        [Descriptor("Set this to true if the ConvAssist database for this language requires encoding translation")]
        [UIHint("ToggleSwitch")]
        public bool UseDefaultEncoding = true;

        [Descriptor("Display disclaimer on application startup")]
        [UIHint("ToggleSwitch")]
        public bool ShowDisclaimerOnStartup = true;

        [Descriptor("Wait time (in seconds) for the ConvAssist executable to load")]
        [Range(60, 500)]
        [UIHint("Slider")]
        public int ConvAssistExeLoadWaitTime = 100;

        [Descriptor("Wait time (in secs) for the ConvAssist modules to load")]
        [Range(30, 200)]
        [UIHint("Slider")]
        public int ConvAssistModuleLoadWaitTime = 80;

        [Descriptor("Enable small model sentence prediction ")]
        [UIHint("ToggleSwitch")]
        public bool EnableSmallVocabularySentencePrediction = false;

        [Descriptor("Enable sentence prediction")]
        [UIHint("ToggleSwitch")]
        public bool Test_GeneralSentencePrediction = false;

        /// <summary>
        /// Loads the settings from the settings file
        /// </summary>
        /// <returns>Settings object</returns>
        public static Settings Load()
        {
            Settings retVal = Load<Settings>(PreferencesFilePath);
            Save(retVal, PreferencesFilePath);
            return retVal;
        }

        /// <summary>
        /// Saves the settings to the settings file
        /// </summary>
        /// <returns>true on success</returns>
        public override bool Save()
        {
            return Save(this, PreferencesFilePath);
        }
    }
}