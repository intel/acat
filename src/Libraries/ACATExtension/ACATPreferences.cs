////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Utility;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using ACATResources;
using ACAT.Core.PreferencesManagement.Attributes;

namespace ACAT.Extension
{
    /// <summary>
    /// ACAT preferences that can be serialized/deserialized to a file
    /// Also contains methods to save the settings and read them
    /// from a file.
    /// </summary>
    [Serializable]
    [Descriptor("General Settings for ACAT", "General")]
    public partial class ACATPreferences : SystemPreferences
    {
        [NonSerialized, XmlIgnore]
        public static String DefaultPreferencesFilePath = String.Empty;

        [NonSerialized, XmlIgnore]
        public static String PreferencesFilePath = String.Empty;

        [Display(Name = nameof(StringResources.Cleartalkwindowwhenthetypingmodeischanged), 
           // Description = nameof(StringResources.Cleartalkwindowwhenthetypingmodeischanged), 
            ResourceType = typeof(StringResources))]
        [Category("Test")]
        [UIHint("ToggleSwitch")]
        [ObservableProperty]
        private bool clearTalkWindowOnTypeModeChange = true;

        [Display(Name = nameof(StringResources.Pintounlockthescreen),
            Description = nameof(StringResources.Usedigits1throug5only),
            ResourceType = typeof(StringResources))]
        [Description("Use digits 1 through 5 only")]
        [UIHint("PinEntry")]
        [ObservableProperty]
        private String screenLockPin = "5143";

        [Display(Name = nameof(StringResources.ConverttexttospeechonENTERkeypress),
         // Description = nameof(StringResources.ConverttexttospeechonENTERkeypress),
          ResourceType = typeof(StringResources))]
        [UIHint("ToggleSwitch")]
        [ObservableProperty]
        private bool speakOnEnterKey = true;

        [Display(Name = nameof(StringResources.Numberoftimesthebuttonsinthestripscannerarecanned),
          Description = nameof(StringResources.ScannerColumnIterations),
          ResourceType = typeof(StringResources))]
        [UIHint("NumericUpDown")]
        [ObservableProperty]
        private int stripScannerColumnIterations = 2;

        [Display(Name = nameof(StringResources.Enablesuggestionsforsentencecompletion),
      //   Description = nameof(StringResources.Enablesuggestionsforsentencecompletion),
         ResourceType = typeof(StringResources))]
        [UIHint("ToggleSwitch")]
        [ObservableProperty]
        private bool useSentencePrediction = true;

        [Display(Name = nameof(StringResources.Howmanywordstodisplayinthewordpredictionlist),
        // Description = nameof(StringResources.Howmanywordstodisplayinthewordpredictionlist),
         ResourceType = typeof(StringResources))]
        [UIHint("NumericUpDown")]
        [Range(3, 20, ErrorMessage = "Word prediction count must be between 3 and 20.")]
        [ObservableProperty]
        private int wordPredictionCount = 10;

        [Display(Name = nameof(StringResources.Enablelearningforwordprediction),
       //  Description = nameof(StringResources.Enablelearningforwordprediction),
         ResourceType = typeof(StringResources))]
        [Descriptor("Enable learning for word prediction")]
        [UIHint("ToggleSwitch")]
        [ObservableProperty]
        private bool wordPredictionEnableLearn = true;

        [Display(Name = nameof(StringResources.Displaywordsinthepredictionlistthatmatchtheprefixofthewordenteredsofar),
       // Description = nameof(StringResources.Displaywordsinthepredictionlistthatmatchtheprefixofthewordenteredsofar),
        ResourceType = typeof(StringResources))]
        [UIHint("ToggleSwitch")]
        [ObservableProperty]
        private bool wordPredictionFilterMatchPrefix = false;

        [Display(Name = nameof(StringResources.Lengthoftheprefixtomatchwhenfilteringwords_),
       //  Description = nameof(StringResources.Lengthoftheprefixtomatchwhenfilteringwords_),
         ResourceType = typeof(StringResources))]
        [UIHint("NumericUpDown")]
        [Range(1, 10, ErrorMessage = "Prefix length must be between 1 and 10.")]
        [ObservableProperty]
        private int wordPredictionFilterMatchPrefixLengthAdjust = 1;

        [Display(Name = nameof(StringResources.Filterpunctuationsinwordpredictionresults),
        // Description = nameof(StringResources.Filterpunctuationsinwordpredictionresults),
         ResourceType = typeof(StringResources))]
        [UIHint("ToggleSwitch")]
        [ObservableProperty]
        private bool wordPredictionFilterPunctuations = true;

        [Display(Name = nameof(StringResources.Extratimetopauseonthefirstwordinthewordpredictionlist),
        // Description = nameof(StringResources.Extratimetopauseonthefirstwordinthewordpredictionlist),
         ResourceType = typeof(StringResources))]
        [UIHint("NumericUpDown")]
        [Range(0, 3000, ErrorMessage = "Pause time must be between 0 and 3000 milliseconds.")]
        [ObservableProperty]
        private int wordPredictionFirstPauseTime = 600;

        [Display(Name = nameof(StringResources.Numberoftimesthewordsinthewordpredictionlistarescanned),
      //   Description = nameof(StringResources.Numberoftimesthewordsinthewordpredictionlistarescanned),
         ResourceType = typeof(StringResources))]
        [UIHint("NumericUpDown")]
        [Range(1, 10, ErrorMessage = "Word prediction scan iterations must be between 1 and 10.")]
        [ObservableProperty]
        private int wordPredictionScanIterations = 1;

        [Display(Name = nameof(StringResources.Numberofwordssuggestionstoomputeprobabilities),
        // Description = nameof(StringResources.Numberofwordssuggestionstoomputeprobabilities),
         ResourceType = typeof(StringResources))]
        [UIHint("NumericUpDown")]
        [Range(5, 20, ErrorMessage = "Words suggestions must be between 5 and 20.")]
        [ObservableProperty]
        private int wordsSuggestions = 10;

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
        public override int resolveVariableInt(String variableName, int defaultValue)
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