////////////////////////////////////////////////////////////////////////////
// <copyright file="SAPISettings.cs" company="Intel Corporation">
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
        public bool UseAlternatePronunciations { get; set; }

        /// <summary>
        /// Gets or sets the volume
        /// </summary>
        [IntDescriptor("Volume setting", 0, 100)]
        public int Volume { get; set; }

        /// <summary>
        /// Gets or sets the voice for TTS
        /// </summary>
        public String Voice { get; set; }

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