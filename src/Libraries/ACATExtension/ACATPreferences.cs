////////////////////////////////////////////////////////////////////////////
// <copyright file="ACATPreferences.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2015 Intel Corporation 
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
using System.Diagnostics.CodeAnalysis;
using ACAT.Lib.Core.Utility;

#region SupressStyleCopWarnings
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1126:PrefixCallsCorrectly",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1101:PrefixLocalCallsWithThis",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1121:UseBuiltInTypeAlias",
        Scope = "namespace",
        Justification = "Since they are just aliases, it doesn't really matter")]
[module: SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1200:UsingDirectivesMustBePlacedWithinNamespace",
        Scope = "namespace",
        Justification = "ACAT guidelines")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1309:FieldNamesMustNotBeginWithUnderscore",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private fields begin with an underscore")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1300:ElementMustBeginWithUpperCaseLetter",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private/Protected methods begin with lowercase")]
[module: SuppressMessage(
        "StyleCop.CSharp.MaintainabilityRules",
        "SA1401:FieldsMustBePrivate",
        Scope = "namespace",
        Justification = "This class is serialized")]
#endregion

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
        [NonSerialized]
        public static String PreferencesFilePath = String.Empty;

        [NonSerialized]
        public static String DefaultPreferencesFilePath = String.Empty;

        // Scanner Settings
        public int HalfScanIterations = 4;
        public int RowScanIterations = 1;
        public int ColumnScanIterations = 1;
        public int WordPredictionScanIterations = 1;

        public int WordPredictionHesitateTime = 600;
        public bool HideLaunchpadOnIdle = false;
        public int TimedDialogTimeout = 4000;

        public bool PrefixNumbersInWordPredictionList = true;

        //Contextual Menu Settings
        public bool EnableContextualMenusForMenus = true;
        public bool EnableContextualMenusForDialogs = true;

        // Word prediction settings
        public int WordPredictionCount = 10;
        public bool EnableWordPredictionCorpusModel = false;
        public bool EnableWordPredictionDynamicModel = true;
        public int WordPredictionNGram = 4;
        public bool WordPredictionFilterPunctuations = true;
        public bool WordPredictionFilterMatchPrefix = false;
        public int WordPredictionFilterMatchPrefixLengthAdjust = 1;

        // Talk window settings
        public bool ShowTalkWindowOnStartup = true;
        public bool TalkWindowDisplayDateTimeEnable = true;
        public String TalkWindowDisplayDateFormat = "ddd, MMM d, yyyy";
        public String TalkWindowDisplayTimeFormat = "h:mm tt";
        public bool TalkWindowShowBorder = true;
        public bool TalkWindowShowTitleBar = true;

        // Mouse Radar settings
        public int MouseRadarRotatingSpeed = 6;
        public int MouseRadarRotatingSweeps = 1;
        public int MouseRadarRadialSpeed = 6;
        public int MouseRadarRadialSweeps = 1;
        public int MouseRadarLineWidth = 3;
        public bool MouseRadarStartFromLastCursorPos = true;
        public bool MouseRadarSoundEffectsOn = false;
        public int MouseRadarRotatingSpeedMultiplier = 4;
        public int MouseRadarRadialSpeedMultipler = 12;

        //Mouse grid settings
        public int MouseGridVerticalSpeed = 4;
        public int MouseGridVerticalSweeps = 1;
        public int MouseGridHorizontalSpeed = 11;
        public int MouseGridHorizontalSweeps = 1;
        public int MouseGridLineWidth = 3;
        public bool MouseGridStartFromLastCursorPos = false;
        public int MouseGridMouseMoveSpeedMultiplier = 6;
        public int MouseGridScanSpeedMultiplier = 14;

        // Mute screen settings
        public int MuteScanIterations = -1;
        public String MutePin = "2589";
        public int MutePinDigitMax = 9;
        public String MuteScreenDisplayDateFormat = "dddd, MMMM d, yyyy";
        public String MuteScreenDisplayTimeFormat = "h:mm:ss tt";

        // Text to speech settings
        
        public String UserVoiceTestString = "The boundary condition of the universe is that it has no boundary.  ";
        public bool TTSUseBookmarks = true;

        // File Browser settings
        public String FileBrowserDateFormat = "MM/dd/yyyy";
        public String FavoriteFolders = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public String FileBrowserExcludeFileExtensions = String.Empty;
        public bool FileBrowserShowFileOperationsMenu = true;
        public String NewTextFileCreateFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public String NewWordDocCreateFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        // Other settings
        public bool SeedWordPredictionOnNewSentence = true;
        public int WindowMaximizeSizePercent = 66;

        public int LectureManagerSpeakAllParagraphPause = 4000;

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
        /// Returns list of favorite folders.  Parses the
        /// "FavoriteFolders" setting, normalizes it and 
        /// returns the list of folders
        /// </summary>
        /// <returns></returns>
        public String[] GetFavoriteFolders()
        {
            if (String.IsNullOrEmpty(FavoriteFolders))
            {
                return new[] { Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) };
            }

            return SmartPath.ACATParseAndNormalizePaths(FavoriteFolders);
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
                case "@HesitateTime":
                    retVal = HesitateTime;
                    break;

                case "@SteppingTime":
                    retVal = SteppingTime;
                    break;

                case "@HalfScanIterations":
                    retVal = HalfScanIterations;
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

                case "@MuteScanIterations":
                    retVal = MuteScanIterations;
                    break;

                case "@TabScanTime":
                    retVal = TabScanTime;
                    break;

                case "@FirstRepeatTime":
                    retVal = FirstRepeatTime;
                    break;

                case "@TimedDialogTimeout":
                    retVal = TimedDialogTimeout;
                    break;

                case "@WordPredictionHesitateTime":
                    retVal = WordPredictionHesitateTime;
                    break;

                default:
                    retVal = base.resolveVariableInt(variableName, defaultValue);
                    break;
            }

            return retVal;
        }
    }
}
