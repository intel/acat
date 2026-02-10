////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ActuatorSettingsConverter.cs
//
// Converts between legacy ActuatorSetting (XML) model and 
// ActuatorSettingsJson (JSON) model for backward compatibility
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ACAT.Core.ActuatorManagement.Settings
{
    /// <summary>
    /// Converts between legacy XML and new JSON configuration models
    /// </summary>
    public static class ActuatorSettingsConverter
    {
        /// <summary>
        /// Convert JSON model to legacy XML model
        /// </summary>
        public static List<ActuatorSetting> FromJson(ActuatorSettingsJson jsonSettings)
        {
            if (jsonSettings == null || jsonSettings.ActuatorSettings == null)
            {
                return new List<ActuatorSetting>();
            }

            return jsonSettings.ActuatorSettings.Select(FromJson).ToList();
        }

        /// <summary>
        /// Convert single JSON actuator to legacy XML model
        /// </summary>
        public static ActuatorSetting FromJson(ActuatorSettingJson jsonActuator)
        {
            if (jsonActuator == null)
            {
                return new ActuatorSetting();
            }

            var actuatorSetting = new ActuatorSetting
            {
                Name = jsonActuator.Name ?? string.Empty,
                Id = Guid.TryParse(jsonActuator.Id, out var id) ? id : Guid.Empty,
                Description = jsonActuator.Description ?? string.Empty,
                Enabled = jsonActuator.Enabled,
                ImageFileName = jsonActuator.ImageFileName ?? string.Empty,
                SwitchSettings = new List<SwitchSetting>()
            };

            if (jsonActuator.SwitchSettings != null)
            {
                actuatorSetting.SwitchSettings = jsonActuator.SwitchSettings
                    .Select(FromJson)
                    .ToList();
            }

            return actuatorSetting;
        }

        /// <summary>
        /// Convert JSON switch to legacy XML model
        /// </summary>
        public static SwitchSetting FromJson(SwitchSettingJson jsonSwitch)
        {
            if (jsonSwitch == null)
            {
                return new SwitchSetting();
            }

            return new SwitchSetting
            {
                Name = jsonSwitch.Name ?? string.Empty,
                Source = jsonSwitch.Source ?? string.Empty,
                Description = jsonSwitch.Description ?? string.Empty,
                Enabled = jsonSwitch.Enabled,
                Actuate = jsonSwitch.Actuate,
                Command = jsonSwitch.Command ?? string.Empty,
                MinHoldTime = jsonSwitch.MinHoldTime ?? string.Empty,
                BeepFile = jsonSwitch.BeepFile ?? string.Empty
            };
        }

        /// <summary>
        /// Convert legacy XML model to JSON model
        /// </summary>
        public static ActuatorSettingsJson ToJson(List<ActuatorSetting> legacySettings)
        {
            if (legacySettings == null)
            {
                return new ActuatorSettingsJson();
            }

            return new ActuatorSettingsJson
            {
                ActuatorSettings = legacySettings.Select(ToJson).ToList()
            };
        }

        /// <summary>
        /// Convert single legacy XML actuator to JSON model
        /// </summary>
        public static ActuatorSettingJson ToJson(ActuatorSetting legacyActuator)
        {
            if (legacyActuator == null)
            {
                return new ActuatorSettingJson();
            }

            var jsonActuator = new ActuatorSettingJson
            {
                Name = legacyActuator.Name ?? string.Empty,
                Id = legacyActuator.Id.ToString(),
                Description = legacyActuator.Description ?? string.Empty,
                Enabled = legacyActuator.Enabled,
                ImageFileName = legacyActuator.ImageFileName ?? string.Empty,
                SwitchSettings = new List<SwitchSettingJson>()
            };

            if (legacyActuator.SwitchSettings != null)
            {
                jsonActuator.SwitchSettings = legacyActuator.SwitchSettings
                    .Select(ToJson)
                    .ToList();
            }

            return jsonActuator;
        }

        /// <summary>
        /// Convert legacy XML switch to JSON model
        /// </summary>
        public static SwitchSettingJson ToJson(SwitchSetting legacySwitch)
        {
            if (legacySwitch == null)
            {
                return new SwitchSettingJson();
            }

            return new SwitchSettingJson
            {
                Name = legacySwitch.Name ?? string.Empty,
                Source = legacySwitch.Source ?? string.Empty,
                Description = legacySwitch.Description ?? string.Empty,
                Enabled = legacySwitch.Enabled,
                Actuate = legacySwitch.Actuate,
                Command = legacySwitch.Command ?? string.Empty,
                MinHoldTime = legacySwitch.MinHoldTime ?? string.Empty,
                BeepFile = legacySwitch.BeepFile ?? string.Empty
            };
        }
    }
}
