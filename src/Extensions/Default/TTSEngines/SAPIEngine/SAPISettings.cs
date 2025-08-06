///////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// SAPISettings.cs
//
// Microsoft Speech Synth Text to speech settings
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.PreferencesManagement;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Speech.Synthesis;
using System.Xml.Serialization;

namespace ACAT.Extensions.TTSEngines.SAPIEngine
{
    /// <summary>
    /// Microsoft Speech Synth Text to speech settings
    /// </summary>
    [Serializable]
    public partial class SAPISettings : PreferencesBase
    {
        /// <summary>
        /// Path to the preferences file
        /// </summary>
        [NonSerialized, XmlIgnore]
        public static String PreferencesFilePath;

        /// <summary>
        /// Name of the alternate pronunciations file
        /// </summary>
        public String PronunciationsFile = "SAPIPronunciations.xml";

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public SAPISettings()
        {
            volume = 100;
            rate = -2;
            Gender = VoiceGender.Female;
            useAlternatePronunciations = true;
        }

        /// <summary>
        /// Gets or sets whether a puncutation should be appended if it
        /// is not already there.
        /// </summary>
        [Descriptor("Auto append sentence terminator?")]
        [UIHint("ToggleSwitch")]
        [Description("Gets or sets whether a puncutation should be appended if it  is not already there.")]
        [ObservableProperty]

        private bool autoAppendPunctuation = false;

        /// <summary>
        /// Preferred Gender of the voice
        /// </summary>
        public VoiceGender Gender { get; set; }

        /// <summary>
        /// Gets or sets the pitch
        /// </summary>
        public int Pitch { get; set; }

        /// <summary>
        /// Gets or sets the rate of speech
        /// </summary>
        [Descriptor("Speaking rate")]
        [Range(-10, 10)]
        [UIHint("Slider")]
        [ObservableProperty]
        private int rate;

        /// <summary>
        /// Gets or sets whether to use alternate pronunciations
        /// </summary>
        [Descriptor("Use alternate pronunciations?")]
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
        [Descriptor("Volume setting")]
        [Range(0, 100)]
        [UIHint("Slider")]
        [ObservableProperty]
        private int volume;

        /// <summary>
        /// Loads settings from file
        /// </summary>
        /// <returns>true on success</returns>
        public static SAPISettings Load()
        {
            var retVal = PreferencesBase.Load<SAPISettings>(PreferencesFilePath);
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
    }
}