////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.PreferencesManagement;
using ACAT.Core.Utility;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace ACAT.Extension
{
    /// <summary>
    /// ACAT preferences that can be serialized/deserialized to a file
    /// Also contains methods to save the settings and read them
    /// from a file.
    /// </summary>
    [Serializable]
    [Descriptor("General Settings for ACAT", "General")]
    public class ACATPreferences : Preferences
    {
        [NonSerialized, XmlIgnore]
        public static String DefaultPreferencesFilePath = String.Empty;

        [NonSerialized, XmlIgnore]
        public static String PreferencesFilePath = String.Empty;

       // [BoolDescriptor("Clear talk window when the typing mode is changed")]
        [Descriptor("Clear talk window when the typing mode is changed")]
        [Category("Test")]
        [UIHint("ToggleSwitch")]
        public bool ClearTalkWindowOnTypeModeChange { get; set; } = true;

      //  [IntDescriptor("Pin to unlock the screen. Use digits 1 through 5 only", 111, 55555)]
        [Descriptor("Pin to unlock the screen. Use digits 1 through 5 only")]
        [UIHint("PinEntry")]
        public String ScreenLockPin { get; set; } = "5143";

       // [BoolDescriptor("Convert text to speech on ENTER key press")]
        [Descriptor("Convert text to speech on ENTER key press")]
        [UIHint("ToggleSwitch")]
        public bool SpeakOnEnterKey { get; set; } = true;

     //   [IntDescriptor("Number of times the buttons in the strip scanner are scanned. Strip scanners are typically used for accented letters in non-English languages", 1, 10)]
        [Descriptor("Number of times the buttons in the strip scanner are scanned. Strip scanners are typically used for accented letters in non-English languages")]
        [UIHint("NumericUpDown")]
        public int StripScannerColumnIterations { get; set; } = 2;

      //  [BoolDescriptor("Enable suggestions for sentence completion")]
        [Descriptor("Enable suggestions for sentence completion")]
        [UIHint("ToggleSwitch")]
        public bool UseSentencePrediction { get; set; } = true;

   //     [IntDescriptor("How many words to display in the word prediction list", 3, 10)]
        [Descriptor("How many words to display in the word prediction list")]
        [UIHint("NumericUpDown")]
        [Range(3, 20, ErrorMessage = "Word prediction count must be between 3 and 20.")]
        public int WordPredictionCount { get; set; } = 10;

      //  [BoolDescriptor("Enable learning for word prediction")]
        [Descriptor("Enable learning for word prediction")]
        [UIHint("ToggleSwitch")]
        public bool WordPredictionEnableLearn { get; set; } = true;

     //   [BoolDescriptor("Display words in the prediction list that match the prefix of the word entered so far")]
        [Descriptor("Display words in the prediction list that match the prefix of the word entered so far")]
        [UIHint("ToggleSwitch")]
        public bool WordPredictionFilterMatchPrefix { get; set; } = false;

//        [IntDescriptor("Length of the prefix to match when filtering words (valid only if WordPredictionFilterMatchPrefix is true)", 1, 10)]
        [Descriptor("Length of the prefix to match when filtering words (valid only if WordPredictionFilterMatchPrefix is true)")]
        [UIHint("NumericUpDown")]
        [Range(1, 10, ErrorMessage = "Prefix length must be between 1 and 10.")]
        public int WordPredictionFilterMatchPrefixLengthAdjust { get; set; } = 1;

   //     [BoolDescriptor("Filter punctuations in word prediction results")]
        [Descriptor("Filter punctuations in word prediction results")]
        [UIHint("ToggleSwitch")]
        public bool WordPredictionFilterPunctuations { get; set; } = true;

  //      [IntDescriptor("Extra time to pause on the first word in the word prediction list (in msecs)", 0, 3000)]
        [Descriptor("Extra time to pause on the first word in the word prediction list (in msecs)")]
        [UIHint("NumericUpDown")]
        [Range(0, 3000, ErrorMessage = "Pause time must be between 0 and 3000 milliseconds.")]
        public int WordPredictionFirstPauseTime { get; set; } = 600;

    //    [IntDescriptor("Number of times the words in the word prediction list are scanned", 1, 10)]
        [Descriptor("Number of times the words in the word prediction list are scanned")]
        [UIHint("NumericUpDown")]
        [Range(1, 10, ErrorMessage = "Word prediction scan iterations must be between 1 and 10.")]
        public int WordPredictionScanIterations { get; set; } = 1;

     //   [IntDescriptor("Number of words suggestions to compute probabilities", 5, 20)]

        [Descriptor("Number of words suggestions to compute probabilities")]
        [UIHint("NumericUpDown")]
        [Range(5, 20, ErrorMessage = "Words suggestions must be between 5 and 20.")]
        public int WordsSuggestions { get; set; } = 10;

        /// <summary>
        /// Loads the settings from the preferences path
        /// </summary>
        /// <param name="loadDefaultsOnFail">set to true to load default settings on error</param>
        /// <returns></returns>
        public static ACATPreferences Load(bool loadDefaultsOnFail = true)
        {
            return !String.IsNullOrEmpty(PreferencesFilePath) ?
                    Load<ACATPreferences>(PreferencesFilePath, loadDefaultsOnFail) :
                    LoadDefaultSettings();
        }

        /// <summary>
        /// Loads default factory settings
        /// </summary>
        /// <returns>Factory default settings</returns>
        public static ACATPreferences LoadDefaultSettings()
        {
            return LoadDefaults<ACATPreferences>();
        }

        /// <summary>
        /// Saves the settings to the preferences file
        /// </summary>
        /// <returns>true on success</returns>
        public override bool Save()
        {
            return !String.IsNullOrEmpty(PreferencesFilePath) &&
                Save(this, PreferencesFilePath);
        }

        /// <summary>
        /// Resolves a variable name into a value by looking up preferences
        /// </summary>
        /// <param name="variableName">name of the variable</param>
        /// <param name="defaultValue">default value of the variable</param>
        /// <returns>value of the variable from the settings</returns>
        protected override int resolveVariableInt(String variableName, int defaultValue)
        {
            var retVal = variableName switch
            {
                "@FirstPauseTime" => FirstPauseTime,
                "@ScanTime" => ScanTime,
                "@GridScanIterations" => GridScanIterations,
                "@RowScanIterations" => RowScanIterations,
                "@ColumnScanIterations" => ColumnScanIterations,
                "@WordPredictionScanIterations" => WordPredictionScanIterations,
                "@MenuDialogScanTime" => MenuDialogScanTime,
                "@FirstRepeatTime" => FirstRepeatTime,
                "@WordPredictionFirstPauseTime" => WordPredictionFirstPauseTime,
                "@StripScannerColumnIterations" => StripScannerColumnIterations,
                _ => base.resolveVariableInt(variableName, defaultValue),
            };
            return retVal;
        }
    }
}