////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// TTSClientSettings.cs
//
// Settings for the text-to-speech client extension
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.PreferencesManagement;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Xml.Serialization;
using ACATResources;

namespace ACAT.Extensions.TTSEngines.TTSClient
{
    public enum TransportProtocol
    {
        Http
    }

    /// <summary>
    /// Settings for TTSClient
    /// </summary>
    [Serializable]
    public partial class TTSClientSettings : PreferencesBase
    {
        /// <summary>
        /// Path to the preferences file
        /// </summary>
        [NonSerialized, XmlIgnore]
        public static String PreferencesFilePath;

        /// <summary>
        /// Name of the alternate pronunciations file
        /// </summary>
        public String PronunciationsFile = "TTSPronunciations.json";

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public TTSClientSettings()
        {
            volume = 100;
            rate = 0;
            Pitch = 0;
            Protocol = TransportProtocol.Http;
            HttpSettings = new HttpSettings();
        }

        /// <summary>
        /// Gets or sets whether a puncutation should be appended if it
        /// is not already there.
        /// </summary>
        [Display(Name = nameof(StringResources.Autoappendsentenceterminator),
            Description = nameof(StringResources.Getsorsetswhetherapuncutationshouldbeappendedifitisnotalreadythere),
            ResourceType = typeof(StringResources))]
        [UIHint("ToggleSwitch")]
        [DefaultValue(false)]
        [ObservableProperty]
        private bool autoAppendPunctuation = false;

        public HttpSettings HttpSettings { get; set; }

        /// <summary>
        /// Gets or sets the pitch
        /// </summary>
        public int Pitch { get; set; }

        public TransportProtocol Protocol { get; set; }

        /// <summary>
        /// Gets or sets the rate of speech
        /// </summary>
        [Display(Name = nameof(StringResources.Speakiate),
         Description = nameof(StringResources.Getsorsetstherateofspeech),
         ResourceType = typeof(StringResources))]
        [Range(-10, 10)]
        [UIHint("Slider")]
        [ObservableProperty]
        private int rate;

        /// <summary>
        /// Gets or sets whether to use alternate pronunciations
        /// </summary>
        [Display(Name = nameof(StringResources.Usealternatepronunciations),
         ResourceType = typeof(StringResources))]
        [UIHint("ToggleSwitch")]
        [DefaultValue(false)]
        [ObservableProperty]
        private bool useAlternatePronunciations = false;

        /// <summary>
        /// Gets or sets the voice for TTS
        /// </summary>
        public String Voice { get; set; }

        /// <summary>
        /// Gets or sets the volume
        /// </summary>
        [Display(Name = nameof(StringResources.Volumesetting),
         ResourceType = typeof(StringResources))]
        [Range(0, 100)]
        [UIHint("Slider")]
        [ObservableProperty]
        private int volume;

        /// <summary>
        /// Loads settings from file
        /// </summary>
        /// <returns>true on success</returns>
        public static TTSClientSettings Load()
        {
            var retVal = PreferencesBase.Load<TTSClientSettings>(PreferencesFilePath);
            if (!File.Exists(PreferencesFilePath))
            {
                retVal.Save();
            }

            return retVal;
        }

        /// <summary>
        /// Save settings to file
        /// </summary>
        /// <returns>true on success</returns>
        public override bool Save()
        {
            bool retVal = Save(this, PreferencesFilePath);
            if (retVal)
            {
                NotifyPreferencesChanged();
            }

            return retVal;
        }

        public override bool ResetToDefault()
        {
            var tmp = LoadDefaults<TTSClientSettings>();
            var res = Save(tmp, PreferencesFilePath);
            Load();

            return res;
        }
    }
}