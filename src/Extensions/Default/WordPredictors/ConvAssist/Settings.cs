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
using ACATResources;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace ACAT.Extensions.WordPredictors.ConvAssist
{
    /// <summary>
    /// Preference settings for the ConvAssist word predictor (English)
    /// </summary>
    [Serializable]
    public partial class Settings : PreferencesBase
    {
       [Display(Name = nameof(StringResources.Astringofcharactersthatshouldbefilteredoutfromthpredicted),
        ResourceType = typeof(StringResources))]
       [UIHint("TextBox")]
       [DefaultValue("")]
       [ObservableProperty]
       private String filterChars  = "";

        /// <summary>
        /// Path to the file where preferences are stored
        /// </summary>
        [NonSerialized, XmlIgnore]
        public static String PreferencesFilePath;

        ///// <summary>
        ///// Set this to true if the language uses diacritics
        ///// </summary>
        //[Display(Name = nameof(StringResources.SetthistotrueiftheConvAssistdatabaseforthislanguagerequiresencodingtranslation),
        // Description = nameof(StringResources.Sethistotrueifthelanguageusesdiacritics),
        //ResourceType = typeof(StringResources))]
        //[UIHint("ToggleSwitch")]
        //[DefaultValue(true)]
        //[ObservableProperty]
        //private bool useDefaultEncoding = true;

        [Display(Name = nameof(StringResources.Displaydisclaimeronapplicationstartup),
        ResourceType = typeof(StringResources))]
        [UIHint("ToggleSwitch")]
        [DefaultValue(true)]
        [ObservableProperty]
        private bool showDisclaimerOnStartup = true;

        [Display(Name = nameof(StringResources.WaittimefortheConvAssistexecutabletoload),
        ResourceType = typeof(StringResources))]
        [Range(60, 500)]
        [UIHint("Slider")]
        [DefaultValue(100)]
        [ObservableProperty]
        private int convAssistExeLoadWaitTime  = 100;

        [Display(Name = nameof(StringResources.WaittimefortheConvAssistmodulestoload),
        ResourceType = typeof(StringResources))]
        [Range(30, 200)]
        [UIHint("Slider")]
        [DefaultValue(80)]
        [ObservableProperty]
        private int convAssistModuleLoadWaitTime  = 80;

        [Display(Name = nameof(StringResources.Enablesmallmodelsentenceprediction),
        ResourceType = typeof(StringResources))]
        [UIHint("ToggleSwitch")]
        [DefaultValue(false)]
        [ObservableProperty]
        private bool enableSmallVocabularySentencePrediction = false;

        [Display(Name = nameof(StringResources.Enablesentenceprediction),
        ResourceType = typeof(StringResources))]
        [UIHint("ToggleSwitch")]
        [DefaultValue(false)]
        [ObservableProperty]
        private bool test_GeneralSentencePrediction = false;

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

        public override bool ResetToDefault()
        {
            var tmp = LoadDefaults<Settings>();
            var res = Save(tmp, PreferencesFilePath);
            Load();

            return res;
        }
    }
}