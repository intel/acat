////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.PreferencesManagement;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ACAT.Extensions.Onboarding.UI
{
    /// <summary>
    /// Class that holds a list of GUIDS of all the onboarding
    /// extensions. This is read from a file
    /// </summary>
    [Serializable]
    public class OnboardingSequence : PreferencesBase
    {
        [NonSerialized, XmlIgnore]
        public static string SettingsFilePath;

        public List<OnboardingSequenceItem> OnboardingSequenceItems = new();

        /// <summary>
        /// Loads the settings from the settings file
        /// </summary>
        /// <returns>true on success</returns>
        public static OnboardingSequence Load()
        {
            OnboardingSequence retVal = Load<OnboardingSequence>(SettingsFilePath, false, false);
            return retVal;
        }

        /// <summary>
        /// Saves settings
        /// </summary>
        /// <returns>true on success</returns>
        public override bool Save()
        {
            return Save(this, SettingsFilePath);
        }
    }
}