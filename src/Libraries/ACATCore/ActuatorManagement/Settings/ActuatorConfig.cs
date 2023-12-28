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

using ACAT.Lib.Core.PreferencesManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace ACAT.Lib.Core.ActuatorManagement
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
        public static String ActuatorSettingsFileName;

        /// <summary>
        /// Arrary of actuator settings, one for each actuator
        /// </summary>
        public List<ActuatorSetting> ActuatorSettings = new List<ActuatorSetting>();

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
        public ActuatorSetting Find(String name)
        {
            return ActuatorSettings.FirstOrDefault(actuatorSetting => String.Compare(name, actuatorSetting.Name, true) == 0);
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

        /// <summary>
        /// Saves the settings to a file indicated by
        /// ActuatorSettingsFileName
        /// </summary>
        /// <returns></returns>
        public override bool Save()
        {
            return !String.IsNullOrEmpty(ActuatorSettingsFileName) && Save(this, ActuatorSettingsFileName);
        }
    }
}