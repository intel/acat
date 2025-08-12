////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ActuatorConfig.cs
//
// Holds the settings for all the actuators discovered. Settings
// include attributes like name, guid, hold time, all the switches
// associated with each actuator, switch settings etc.  This class
// is persisted to a file
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.PreferencesManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace ACAT.Core.ActuatorManagement.Settings
{
    /// <summary>
    /// Holds the settings for all the actuators discovered. Settings
    /// include attributes like name, guid, hold time, all the switches
    /// associated with each actuator, switch settings etc.  This class
    /// is persisted to a file
    /// </summary>
    [Serializable]
    public class ActuatorConfig : PreferencesBase
    {
        /// <summary>
        /// Name of the settings file
        /// </summary>
        [NonSerialized, XmlIgnore]
        public static string ActuatorSettingsFileName;

        /// <summary>
        /// Arrary of actuator settings, one for each actuator
        /// </summary>
        public List<ActuatorSetting> ActuatorSettings = new();

        /// <summary>
        /// Loads settings from file
        /// </summary>
        /// <returns>Actuator settings object </returns>
        public static ActuatorConfig Load()
        {
            return Load<ActuatorConfig>(ActuatorSettingsFileName);
        }

        /// <summary>
        /// Finds the actuator setting for the specified actuator name
        /// </summary>
        /// <param name="name">name of the actuator</param>
        /// <returns>object if found, null otherwise</returns>
        public ActuatorSetting Find(string name)
        {
            return ActuatorSettings.FirstOrDefault(actuatorSetting => string.Compare(name, actuatorSetting.Name, true) == 0);
        }

        /// <summary>
        /// Finds the actuator setting for the specified actuator id
        /// </summary>
        /// <param name="id">id of the actuator</param>
        /// <returns>object if found, null otherwise</returns>
        public ActuatorSetting Find(Guid id)
        {
            return ActuatorSettings.FirstOrDefault(actuator => Equals(id, actuator.Id));
        }

        public override bool ResetToDefault()
        {
            var tmp = LoadDefaults<ActuatorConfig>();
            var res = Save(tmp, ActuatorSettingsFileName);
            Load();

            return res;
        }

        /// <summary>
        /// Saves the settings to a file indicated by
        /// ActuatorSettingsFileName
        /// </summary>
        /// <returns></returns>
        public override bool Save()
        {
            return !string.IsNullOrEmpty(ActuatorSettingsFileName) && Save(this, ActuatorSettingsFileName);
        }
    }
}