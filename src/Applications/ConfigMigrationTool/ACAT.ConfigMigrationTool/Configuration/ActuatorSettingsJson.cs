////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ActuatorSettingsJson.cs
//
// JSON-serializable POCO classes for actuator configuration with
// System.Text.Json attributes for modern JSON serialization
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ACAT.ConfigMigrationTool.Configuration
{
    /// <summary>
    /// Root configuration for all actuators in ACAT
    /// Supports JSON serialization with System.Text.Json
    /// </summary>
    public class ActuatorSettingsJson
    {
        /// <summary>
        /// List of actuator configurations
        /// </summary>
        [JsonPropertyName("actuatorSettings")]
        [Required]
        public List<ActuatorSettingJson> ActuatorSettings { get; set; } = new();

        /// <summary>
        /// Factory method to create default keyboard actuator configuration
        /// </summary>
        public static ActuatorSettingsJson CreateDefault()
        {
            return new ActuatorSettingsJson
            {
                ActuatorSettings = new List<ActuatorSettingJson>
                {
                    ActuatorSettingJson.CreateKeyboardActuator()
                }
            };
        }
    }

    /// <summary>
    /// Configuration for a single actuator (input device)
    /// </summary>
    public class ActuatorSettingJson
    {
        /// <summary>
        /// Name of the actuator (e.g., "Keyboard", "Camera", "BCI")
        /// </summary>
        [JsonPropertyName("name")]
        [Required(ErrorMessage = "Actuator name is required")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Unique identifier (GUID) for the actuator
        /// </summary>
        [JsonPropertyName("id")]
        [Required(ErrorMessage = "Actuator ID is required")]
        public string Id { get; set; } = Guid.Empty.ToString();

        /// <summary>
        /// User-friendly description of the actuator
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Whether the actuator is enabled
        /// </summary>
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; } = false;

        /// <summary>
        /// Image file name for the actuator icon
        /// </summary>
        [JsonPropertyName("imageFileName")]
        public string ImageFileName { get; set; } = string.Empty;

        /// <summary>
        /// List of switch configurations for this actuator
        /// </summary>
        [JsonPropertyName("switchSettings")]
        public List<SwitchSettingJson> SwitchSettings { get; set; } = new();

        /// <summary>
        /// Factory method to create a keyboard actuator with default settings
        /// </summary>
        public static ActuatorSettingJson CreateKeyboardActuator()
        {
            return new ActuatorSettingJson
            {
                Name = "Keyboard",
                Id = "d91a1877-c92b-4d7e-9ab6-f01f30b12df9",
                Description = "Use the computer keyboard as a switch to control ACAT.",
                Enabled = true,
                ImageFileName = "KeyboardSwitch.jpg",
                SwitchSettings = new List<SwitchSettingJson>
                {
                    SwitchSettingJson.CreateTriggerSwitch("F12")
                }
            };
        }

        /// <summary>
        /// Factory method to create a camera actuator with default settings
        /// </summary>
        public static ActuatorSettingJson CreateCameraActuator()
        {
            return new ActuatorSettingJson
            {
                Name = "Camera",
                Id = "7da7f870-80dc-47b4-994c-5f46a4dfe538",
                Description = "Uses your webcam as a switch to control ACAT.",
                Enabled = false,
                ImageFileName = "WebcamSwitch.jpg",
                SwitchSettings = new List<SwitchSettingJson>
                {
                    new SwitchSettingJson
                    {
                        Name = "Cheek Twitch",
                        Source = "CT",
                        Description = "Cheek twitch gesture",
                        Enabled = true,
                        Actuate = true,
                        Command = "@Trigger",
                        MinHoldTime = "@MinActuationHoldTime"
                    }
                }
            };
        }

        /// <summary>
        /// Factory method to create a BCI actuator with default settings
        /// </summary>
        public static ActuatorSettingJson CreateBCIActuator()
        {
            return new ActuatorSettingJson
            {
                Name = "BCI",
                Id = "77809d19-f450-4d36-a633-d818400b3d9a",
                Description = "Brain Computer Interface (BCI) is technology that reads brain waves to help you interact with your computer.",
                Enabled = false,
                ImageFileName = "BCISwitch.png",
                SwitchSettings = new List<SwitchSettingJson>
                {
                    new SwitchSettingJson
                    {
                        Name = "R1",
                        Source = "R1",
                        Enabled = true,
                        Actuate = true,
                        Command = "@Trigger",
                        MinHoldTime = "@MinActuationHoldTime"
                    }
                }
            };
        }
    }

    /// <summary>
    /// Configuration for a single switch belonging to an actuator
    /// </summary>
    public class SwitchSettingJson
    {
        /// <summary>
        /// Name of the switch
        /// </summary>
        [JsonPropertyName("name")]
        [Required(ErrorMessage = "Switch name is required")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Switch source identifier (e.g., key code, gesture)
        /// </summary>
        [JsonPropertyName("source")]
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// User-friendly description of the switch
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Whether the switch is enabled
        /// </summary>
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; } = false;

        /// <summary>
        /// Whether the switch should trigger actions
        /// </summary>
        [JsonPropertyName("actuate")]
        public bool Actuate { get; set; } = false;

        /// <summary>
        /// Command verb associated with the switch
        /// </summary>
        [JsonPropertyName("command")]
        public string Command { get; set; } = string.Empty;

        /// <summary>
        /// Minimum hold time for the switch
        /// </summary>
        [JsonPropertyName("minHoldTime")]
        public string MinHoldTime { get; set; } = string.Empty;

        /// <summary>
        /// Audio file to play when switch is activated
        /// </summary>
        [JsonPropertyName("beepFile")]
        public string BeepFile { get; set; } = string.Empty;

        /// <summary>
        /// Factory method to create a trigger switch
        /// </summary>
        public static SwitchSettingJson CreateTriggerSwitch(string source = "F12", string beepFile = "beep.wav")
        {
            return new SwitchSettingJson
            {
                Name = "Trigger",
                Source = source,
                Enabled = true,
                Actuate = true,
                Command = "@Trigger",
                MinHoldTime = "@MinActuationHoldTime",
                BeepFile = beepFile
            };
        }
    }
}
