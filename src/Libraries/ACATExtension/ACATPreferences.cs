////////////////////////////////////////////////////////////////////////////
// <copyright file="ACATPreferences.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2017 Intel Corporation 
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
////////////////////////////////////////////////////////////////////////////

using System;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.PreferencesManagement;
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
        public static String PreferencesFilePath = String.Empty;

        [NonSerialized, XmlIgnore]
        public static String DefaultPreferencesFilePath = String.Empty;

        [IntDescriptor("Number of times the grid (top level) is scanned", 1, 10)]
        public int GridScanIterations = 4;

        [IntDescriptor("Number of times the rows in a grid are scanned", 1, 10)]
        public int RowScanIterations = 1;

        [IntDescriptor("Number of times the buttons in a row are scanned", 1, 10)]
        public int ColumnScanIterations = 1;

        [IntDescriptor("Number of times the buttons in the strip scanner are scanned. Strip scanners are typically used for accented letters in non-English languages", 1, 10)]
        public int StripScannerColumnIterations = 2;

        [IntDescriptor("Number of times the words in the word prediction list are scanned", 1, 10)]
        public int WordPredictionScanIterations = 1;

        [IntDescriptor("Extra time to pause on the first word in the word prediction list (in msecs)", 0, 3000)]
        public int WordPredictionFirstPauseTime = 600;

        [IntDescriptor("Timeout for timed dialogs.  The dialog is dismissed automatically after this timeout expires (in msecs)", 3000, 15000)]
        public int TimedDialogTimeout = 4000;

        [BoolDescriptor("Prefix words in the word prediction list with the index number of the word")]
        public bool PrefixNumbersInWordPredictionList = false;

        [BoolDescriptor("Automatically display contextual menu if a AppMenu is activated in applications")]
        public bool EnableContextualMenusForMenus = true;

        [BoolDescriptor("Automatically display contextual menu if a Dialog is activated in applications")]
        public bool EnableContextualMenusForDialogs = true;

        [IntDescriptor("How many words to display in the word prediction list", 3, 10)]
        public int WordPredictionCount = 10;

        [BoolDescriptor("Enable learning for word prediction")]
        public bool EnableWordPredictionDynamicModel = true;

        [IntDescriptor("The NGram value for word prediciton.  How many preceding words to use to predict the next word", 1, 50)]
        public int WordPredictionNGram = 4;

        [BoolDescriptor("Filter punctuations in word prediction results")]
        public bool WordPredictionFilterPunctuations = true;

        [BoolDescriptor("Display words in the prediction list that match the prefix of the word entered so far")]
        public bool WordPredictionFilterMatchPrefix = false;

        [IntDescriptor("Length of the prefix to match when filtering words (valid only if WordPredictionFilterMatchPrefix is true)", 1, 10)]
        public int WordPredictionFilterMatchPrefixLengthAdjust = 1;

        [StringDescriptor("Preferred browser to use for Google searches, Wiki searches etc. Set this to the name of the EXE of the browser. " + 
                            "IExplore.exe for Internet Explorer, Chrome.exe for Chrome, Firefox.exe for Firefox, ApplicationFrameHost.exe for Microsoft Edge.  Leave this setting empty to use the default browser")]
        public String PreferredBrowser = "IExplore.exe";

        // Talk window settings

        [BoolDescriptor("Show the Talk window when ACAT starts")]
        public bool ShowTalkWindowOnStartup = true;

        [BoolDescriptor("Display date/time on the Talk window")]
        public bool TalkWindowDisplayDateTimeEnable = true;
        public String TalkWindowDisplayDateFormat = "ddd, MMM d, yyyy";
        public String TalkWindowDisplayTimeFormat = "h:mm tt";

        //Mouse grid settings

        [IntDescriptor("Speed of the rectangle in Mouse scanning", 1, 500)]
        public int MouseGridRectangleSpeed = 40;

        [IntDescriptor("Number of Mouse rectangle scans", 1, 5)]
        public int MouseGridRectangleCycles = 2;

        [IntDescriptor("Speed of the line in Mouse scanning", 1, 500)]
        public int MouseGridLineSpeed = 20;

        [IntDescriptor("Number of Mouse line scans", 1, 5)]
        public int MouseGridLineCycles = 1;

        [IntDescriptor("Thickness of the Mouse line", 1, 5)]
        public int MouseGridLineThickness = 2;

        [IntDescriptor("Height of the rectangle in Mouse scanning", 50, 500)]
        public int MouseGridRectangleHeight = 120;

        [BoolDescriptor("Enable vertical rectangle scan in Mouse scanning")]
        public bool MouseGridEnableVerticalRectangleScan = true;

        // Screen Lock Settings
        [IntDescriptor("Number of scan iterations in the Screen Lock scanner", 1, 20)]
        public int ScreenLockScanIterations = -1;

        public String ScreenLockPin = "2589";

        [IntDescriptor("Max digit value to use for the Screen Lock PIN", 2, 9)]
        public int ScreenLockPinMaxDigitValue = 9;
        public String MuteScreenDisplayDateFormat = "dddd, MMMM d, yyyy";
        public String MuteScreenDisplayTimeFormat = "h:mm:ss tt";

        [BoolDescriptor("Use bookmarks in Text-to-speech. Valid only if the speech engine supports the bookmarks feature which notifies ACAT that it has finished speaking the text that was sent")]
        public bool TTSUseBookmarks = true;

        // File Browser settings
        [StringDescriptor("Full path to the folder in which new text files will be created (This must be a valid folder)")]
        public String NewTextFileCreateFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        [StringDescriptor("Full path to the folder in which new Word files will be created (This must be a valid folder)")]
        public String NewWordDocCreateFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        [BoolDescriptor("On the start of a new sentence, ignore context from previous sentence")]
        public bool SeedWordPredictionOnNewSentence = true;

        [IntDescriptor("Horizontal size of a Snapped window as a percentage of the width of the display", 40, 100)]
        public int WindowSnapSizePercent = 66;

        public bool TransferredPreferencesFromV098 = false;

        public bool TransferredSettingsFromV099 = false;

        public bool TransferredSettingsFromV0991 = false;

        public bool ShowThemeSelectDialogOnStartup = true;

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
            int retVal = defaultValue;

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

                case "@ScreenLockScanIterations":
                    retVal = ScreenLockScanIterations;
                    break;

                case "@MenuDialogScanTime":
                    retVal = MenuDialogScanTime;
                    break;

                case "@FirstRepeatTime":
                    retVal = FirstRepeatTime;
                    break;

                case "@TimedDialogTimeout":
                    retVal = TimedDialogTimeout;
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
