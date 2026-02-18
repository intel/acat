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

using ACAT.Core.Configuration;
using ACAT.Core.PreferencesManagement;
using ACAT.Core.Utility;
using ACAT.Core.Validation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
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
        private static readonly ILogger<ActuatorConfig> _logger = LoggingConfiguration.CreateLogger<ActuatorConfig>();

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
        /// Loads settings from file (JSON format)
        /// </summary>
        /// <returns>Actuator settings object </returns>
        public static ActuatorConfig Load()
        {
            return LoadFromJson(ActuatorSettingsFileName);
        }

        /// <summary>
        /// Loads settings from JSON file with validation
        /// </summary>
        /// <param name="filePath">Path to JSON configuration file</param>
        /// <returns>Actuator settings object</returns>
        private static ActuatorConfig LoadFromJson(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                _logger.LogError("ActuatorSettingsFileName is null or empty");
                return LoadDefaults<ActuatorConfig>();
            }

            try
            {
                // Use JsonConfigurationLoader with validation
                var validator = new ActuatorSettingsValidator();
                var loader = new JsonConfigurationLoader<ActuatorSettingsJson>(validator, _logger);

                ActuatorSettingsJson jsonSettings = loader.Load(filePath, createDefaultOnError: true);
                
                if (jsonSettings == null)
                {
                    _logger.LogWarning("Failed to load JSON settings, using defaults");
                    return LoadDefaults<ActuatorConfig>();
                }

                // Convert JSON model to legacy model
                var config = new ActuatorConfig();
                config.ActuatorSettings = ActuatorSettingsConverter.FromJson(jsonSettings);
                
                _logger.LogInformation("Successfully loaded {Count} actuator(s) from JSON", config.ActuatorSettings.Count);
                
                return config;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading actuator settings from JSON: {FilePath}", filePath);
                return LoadDefaults<ActuatorConfig>();
            }
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
            ActuatorConfig tmp = LoadDefaults<ActuatorConfig>();
            var res = SaveToJson(tmp, ActuatorSettingsFileName);
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
            return !string.IsNullOrEmpty(ActuatorSettingsFileName) && SaveToJson(this, ActuatorSettingsFileName);
        }

        /// <summary>
        /// Saves settings to JSON file with validation
        /// </summary>
        /// <param name="config">Configuration to save</param>
        /// <param name="filePath">Path to JSON file</param>
        /// <returns>True if successful</returns>
        private static bool SaveToJson(ActuatorConfig config, string filePath)
        {
            if (config == null)
            {
                _logger.LogError("Cannot save null ActuatorConfig");
                return false;
            }

            if (string.IsNullOrEmpty(filePath))
            {
                _logger.LogError("ActuatorSettingsFileName is null or empty");
                return false;
            }

            try
            {
                // Convert legacy model to JSON model
                ActuatorSettingsJson jsonSettings = ActuatorSettingsConverter.ToJson(config.ActuatorSettings);
                
                // Use JsonConfigurationLoader with validation
                var validator = new ActuatorSettingsValidator();
                var loader = new JsonConfigurationLoader<ActuatorSettingsJson>(validator, _logger);
                
                bool success = loader.Save(jsonSettings, filePath);
                
                if (success)
                {
                    _logger.LogInformation("Successfully saved actuator settings to JSON: {FilePath}", filePath);
                }
                else
                {
                    _logger.LogError("Failed to save actuator settings to JSON: {FilePath}", filePath);
                }
                
                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving actuator settings to JSON: {FilePath}", filePath);
                return false;
            }
        }
    }
}