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

using ACAT.Lib.Core.PreferencesManagement;
using ACAT.Lib.Core.Utility;
using System.ComponentModel.DataAnnotations;
using System;
using System.Xml.Serialization;

namespace ACAT.Extensions.Default.WordPredictors.ConvAssist
{
    /// <summary>
    /// Preference settings for the ConvAssist word predictor (English)
    /// </summary>
    [Serializable]
    public class Settings : PreferencesBase
    {
        [Descriptor("A string of characters that should be filtered out from the predicted words, eg, punctuations")]
        [UIHint("TextBox")]
        [DefaultValue(String.Empty)]
        public String FilterChars { get; set; }  =  String.Empty;

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
        [DefaultValue(true)]
        public bool UseDefaultEncoding { get; set; }  =  true;

        [Descriptor("Display disclaimer on application startup")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        public bool ShowDisclaimerOnStartup { get; set; }  =  true;

        [Descriptor("Wait time (in seconds) for the ConvAssist executable to load")]
        [Range(60, 500)]
        [UIHint("Slider")]
        [DefaultValue(100)]
        public int ConvAssistExeLoadWaitTime { get; set; }  =  100;

        [Descriptor("Wait time (in secs) for the ConvAssist modules to load")]
        [Range(30, 200)]
        [UIHint("Slider")]
        [DefaultValue(80)]
        public int ConvAssistModuleLoadWaitTime { get; set; }  =  80;

        [Descriptor("Enable small model sentence prediction ")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(false)]
        public bool EnableSmallVocabularySentencePrediction { get; set; }  =  false;

        [Descriptor("Enable sentence prediction")]
        [UIHint("ToggleSwitch")]
        [DefaultValue(false)]
        public bool Test_GeneralSentencePrediction { get; set; }  =  false;

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