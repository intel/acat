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

using ACAT.Lib.Core.PreferencesManagement;
using System;
using System.IO;
using System.Speech.Synthesis;
using System.Xml.Serialization;

namespace ACAT.Extensions.Default.TTSEngines.SAPIEngine
{
    /// <summary>
    /// Microsoft Speech Synth Text to speech settings
    /// </summary>
    [Serializable]
    public class SAPISettings : PreferencesBase
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
            Volume = 100;
            Rate = -2;
            Gender = VoiceGender.Female;
            UseAlternatePronunciations = true;
        }

        /// <summary>
        /// Gets or sets whether a puncutation should be appended if it
        /// is not already there.
        /// </summary>
        [BoolDescriptor("Auto append sentence terminator?", false)]
        public bool AutoAppendPunctuation { get; set; }

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
        [IntDescriptor("Speaking rate", -10, 10)]
        public int Rate { get; set; }

        /// <summary>
        /// Gets or sets whether to use alternate pronunciations
        /// </summary>
        [BoolDescriptor("Use alternate pronunciations?", false)]
        public bool UseAlternatePronunciations { get; set; }

        /// <summary>
        /// Gets or sets the voice for TTS
        /// </summary>
        public String Voice { get; set; }

        /// <summary>
        /// Gets or sets the volume
        /// </summary>
        [IntDescriptor("Volume setting", 0, 100)]
        public int Volume { get; set; }

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
            bool retVal = Save<SAPISettings>(this, PreferencesFilePath);
            if (retVal)
            {
                NotifyPreferencesChanged();
            }

            return retVal;
        }
    }
}