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

using ACAT.Lib.Core.PreferencesManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace ACAT.Extensions.Default.Actuators.CameraActuator
{
    [Serializable]
    public class Settings : PreferencesBase
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

        [Descriptor("Value is in milliseconds. If you observe two movement detections, once when you move your cheek up and the second when you return to normal position, adjust the value to the approximate time you hold the cheek in the up position​​")]
        [Range(0, 1000)]
        [UIHint("Slider")]
        [DefaultValue(0)]
        public int CheekTwitchHoldTime { get; set; }  =  0;

        [Descriptor("Try higher values if the system is triggering with involuntary cheek movement. Try lower values  if you want the system to trigger with less cheek movement​​")]
        [Range(5, 50)]
        [UIHint("Slider")]
        [DefaultValue(20)]
        public int CheekTwitchSensitivity { get; set; }  =  20;

        [Descriptor("Value is in milliseconds. If you observe two movement detections, once when you raise your eyebrows and the second when you return to normal position, adjust the value to the approximate time you hold the eyebrow in the raised position")]
        [Range(0, 2000)]
        [UIHint("Slider")]
        [DefaultValue(0)]
        public int EyebrowRaiseHoldTime { get; set; }  =  0;

        [Descriptor("Try higher values if the system is triggering with involuntary eyebrow movement. Try lower values  if you want to the system to trigger with less eyebrow movement")]
        [Range(5, 50)]
        [UIHint("Slider")]
        [DefaultValue(10)]
        public int EyebrowRaiseSensitivity { get; set; }  =  10;

        [Descriptor("Try higher values if the system is trying to recalibrate too often with involuntary head movements​. Try lower values if the system is too slow in adjusting the facial regions with head repositioning​")]
        [Range(20, 100)]
        [UIHint("Slider")]
        [DefaultValue(40)]
        public int HeadMovementSensitivity { get; set; }  =  40;

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
            Save(retVal, SettingsFilePath);
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