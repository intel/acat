////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PreferencesManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Xml.Serialization;

namespace ACAT.Lib.Extension
{
    /// <summary>
    /// ACAT preferences that can be serialized/deserialized to a file
    /// Also contains methods to save the settings and read them
    /// from a file.
    /// </summary>
    [Serializable]
    public class ACATPreferences : Preferences
    {
        [NonSerialized, XmlIgnore]
        public static String DefaultPreferencesFilePath = String.Empty;

        [NonSerialized, XmlIgnore]
        public static String PreferencesFilePath = String.Empty;

        [BoolDescriptor("Clear talk window when the typing mode is changed")]
        public bool ClearTalkWindowOnTypeModeChange = true;

        [IntDescriptor("Pin to unlock the screen. Use digits 1 through 5 only", 111, 55555)]
        public String ScreenLockPin = "5143";

        [BoolDescriptor("Convert text to speech on ENTER key press")]
        public bool SpeakOnEnterKey = true;

        [IntDescriptor("Number of times the buttons in the strip scanner are scanned. Strip scanners are typically used for accented letters in non-English languages", 1, 10)]
        public int StripScannerColumnIterations = 2;
        
        [BoolDescriptor("Enable suggestions for sentence completion")]
        public bool UseSentencePrediction = true;

        [IntDescriptor("How many words to display in the word prediction list", 3, 10)]
        public int WordPredictionCount = 10;

        [BoolDescriptor("Enable learning for word prediction")]
        public bool WordPredictionEnableLearn = true;

        [BoolDescriptor("Display words in the prediction list that match the prefix of the word entered so far")]
        public bool WordPredictionFilterMatchPrefix = false;

        [IntDescriptor("Length of the prefix to match when filtering words (valid only if WordPredictionFilterMatchPrefix is true)", 1, 10)]
        public int WordPredictionFilterMatchPrefixLengthAdjust = 1;

        [BoolDescriptor("Filter punctuations in word prediction results")]
        public bool WordPredictionFilterPunctuations = true;

        [IntDescriptor("Extra time to pause on the first word in the word prediction list (in msecs)", 0, 3000)]
        public int WordPredictionFirstPauseTime = 600;

        [IntDescriptor("Number of times the words in the word prediction list are scanned", 1, 10)]
        public int WordPredictionScanIterations = 1;

        [IntDescriptor("Number of words suggestions to compute probabilities", 15, 200)]
        public int WordsSuggestions = 100;

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
                Save<ACATPreferences>(this, PreferencesFilePath);
        }

        /// <summary>
        /// Resolves a variable name into a value by looking up preferences
        /// </summary>
        /// <param name="variableName">name of the variable</param>
        /// <param name="defaultValue">default value of the variable</param>
        /// <returns>value of the variable from the settings</returns>
        protected override int resolveVariableInt(String variableName, int defaultValue)
        {
            int retVal;
            switch (variableName)
            {
                case "@FirstPauseTime":
                    retVal = FirstPauseTime;
                    break;

                case "@ScanTime":
                    retVal = ScanTime;
                    break;

                case "@GridScanIterations":
                    retVal = GridScanIterations;
                    break;

                case "@RowScanIterations":
                    retVal = RowScanIterations;
                    break;

                case "@ColumnScanIterations":
                    retVal = ColumnScanIterations;
                    break;

                case "@WordPredictionScanIterations":
                    retVal = WordPredictionScanIterations;
                    break;

                case "@MenuDialogScanTime":
                    retVal = MenuDialogScanTime;
                    break;

                case "@FirstRepeatTime":
                    retVal = FirstRepeatTime;
                    break;

                case "@WordPredictionFirstPauseTime":
                    retVal = WordPredictionFirstPauseTime;
                    break;

                case "@StripScannerColumnIterations":
                    retVal = StripScannerColumnIterations;
                    break;

                default:
                    retVal = base.resolveVariableInt(variableName, defaultValue);
                    break;
            }

            return retVal;
        }
    }
}