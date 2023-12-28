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

using ACAT.Lib.Core.PreferencesManagement;
using System;
using System.IO;
using System.Xml.Serialization;

namespace ACAT.Extensions.Default.TTSEngines.TTSClient
{
    public enum TransportProtocol
    {
        Http
    }

    /// <summary>
    /// Settings for TTSClient
    /// </summary>
    [Serializable]
    public class TTSClientSettings : PreferencesBase
    {
        /// <summary>
        /// Path to the preferences file
        /// </summary>
        [NonSerialized, XmlIgnore]
        public static String PreferencesFilePath;

        /// <summary>
        /// Name of the alternate pronunciations file
        /// </summary>
        public String PronunciationsFile = "TTSPronunciations.xml";

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public TTSClientSettings()
        {
            Volume = 100;
            Rate = 0;
            Pitch = 0;
            Protocol = TransportProtocol.Http;
            HttpSettings = new HttpSettings();
        }

        /// <summary>
        /// Gets or sets whether a puncutation should be appended if it
        /// is not already there.
        /// </summary>
        [BoolDescriptor("Auto append sentence terminator?", false)]
        public bool AutoAppendPunctuation { get; set; }

        public HttpSettings HttpSettings { get; set; }

        /// <summary>
        /// Gets or sets the pitch
        /// </summary>
        public int Pitch { get; set; }

        public TransportProtocol Protocol { get; set; }

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
            bool retVal = Save<TTSClientSettings>(this, PreferencesFilePath);
            if (retVal)
            {
                NotifyPreferencesChanged();
            }

            return retVal;
        }
    }
}