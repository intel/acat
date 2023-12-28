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
        [StringDescriptor("A string of characters that should be filtered out from the predicted words, eg, punctuations")]
        public String FilterChars = String.Empty;

        /// <summary>
        /// Path to the file where preferences are stored
        /// </summary>
        [NonSerialized, XmlIgnore]
        public static String PreferencesFilePath;

        /// <summary>
        /// Set this to true if the language uses diacritics
        /// </summary>
        [BoolDescriptor("Set this to true if the ConvAssist database for this language requires encoding translation", true)]
        public bool UseDefaultEncoding = true;

        [BoolDescriptor("Display disclaimer on application startup", true)]
        public bool ShowDisclaimerOnStartup = true;

        [IntDescriptor("Wait time (in seconds) for the ConvAssist executable to load", 60, 500)]
        public int ConvAssistExeLoadWaitTime = 100;

        [IntDescriptor("Wait time (in secs) for the ConvAssist modules to load", 30, 200)]
        public int ConvAssistModuleLoadWaitTime = 80;

        [BoolDescriptor("Enable small model sentence prediction ")]
        public bool EnableSmallVocabularySentencePrediction = false;


        [BoolDescriptor("Enable sentence prediction")]
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