////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// Settings.cs
//
// Settings for the Vision Actuator
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.PreferencesManagement;
using ACATResources;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace ACAT.Extensions.Actuators.CameraActuator
{
    [Serializable]
    public partial class Settings : PreferencesBase
    {
        /// <summary>
        /// Name of the settings file
        /// </summary>
        [NonSerialized, XmlIgnore]
        public static String SettingsFilePath;

        /// <summary>
        /// List of installed cameras
        /// </summary>
        public String[] CameraList;

        [Display(Name = nameof(StringResources.CheekTwitchHoldTime),
         Description = nameof(StringResources.Valueisinmilliseconds),
            ResourceType = typeof(StringResources))]
        [Range(0, 1000)]
        [DefaultValue(0)]
        [ObservableProperty]
        private int cheekTwitchHoldTime = 0;

        [Display(Name = nameof(StringResources.CheekTwitchSensitivity),
         Description = nameof(StringResources.TryHigherValuesIfTheSystemIsTriggeringWithInvoluntaryCheekMovemen),
            ResourceType = typeof(StringResources))]
        [Range(5, 50)]
        [DefaultValue(20)]
        [ObservableProperty]
        private int cheekTwitchSensitivity = 20;

        [Display(Name = nameof(StringResources.EyebrowRaiseHoldTime),
           Description = nameof(StringResources.valueisinmillisecondsIfyouobservetwomovementdetections),ResourceType = typeof(StringResources))]
        [Range(0, 2000)]
        [DefaultValue(0)]
        [ObservableProperty]
        private int eyebrowRaiseHoldTime = 0;

        [Display(Name = nameof(StringResources.EyebrowRaiseSensitivity),
         Description = nameof(StringResources.TryHigherValuesIfTheSystemIsTriggeringWithInvoluntaryCheekMovemen), ResourceType = typeof(StringResources))]
        [Range(5, 50)]
        [DefaultValue(10)]
        [ObservableProperty]
        private int eyebrowRaiseSensitivity = 10;

        [Display(Name = nameof(StringResources.HeadMovementSensitivity),
            Description = nameof(StringResources.Tryhighervalueshead), ResourceType = typeof(StringResources))]
        [Range(20, 100)]
        [DefaultValue(40)]
        [ObservableProperty]
        private int headMovementSensitivity = 40;

        /// <summary>
        /// Preferred camera to use
        /// </summary>
        public String PreferredCamera;

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public Settings()
        {
            CameraList = new string[] { };
            PreferredCamera = String.Empty;
        }

        /// <summary>
        /// Loads the settings from the settings file
        /// </summary>
        /// <returns>true on success</returns>
        public static Settings Load()
        {
            Settings retVal = PreferencesBase.Load<Settings>(SettingsFilePath);
            //Save(retVal, SettingsFilePath);
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