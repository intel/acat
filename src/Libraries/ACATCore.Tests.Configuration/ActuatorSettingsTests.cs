////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ActuatorSettingsTests.cs
//
// Unit tests for ActuatorSettings JSON configuration
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Configuration;
using ACAT.Core.Validation;
using FluentValidation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text.Json;

namespace ACATCore.Tests.Configuration
{
    [TestClass]
    public class ActuatorSettingsJsonTests
    {
        [TestMethod]
        public void CanCreateDefaultSettings()
        {
            // Act
            var settings = ActuatorSettingsJson.CreateDefault();

            // Assert
            Assert.IsNotNull(settings);
            Assert.IsNotNull(settings.ActuatorSettings);
            Assert.AreEqual(1, settings.ActuatorSettings.Count);
            Assert.AreEqual("Keyboard", settings.ActuatorSettings[0].Name);
        }

        [TestMethod]
        public void CanSerializeToJson()
        {
            // Arrange
            var settings = ActuatorSettingsJson.CreateDefault();

            // Act
            var json = JsonSerializer.Serialize(settings);

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(json));
            Assert.IsTrue(json.Contains("actuatorSettings"));
            Assert.IsTrue(json.Contains("Keyboard"));
        }

        [TestMethod]
        public void CanDeserializeFromJson()
        {
            // Arrange
            var json = @"{
                ""actuatorSettings"": [
                    {
                        ""name"": ""TestActuator"",
                        ""id"": ""d91a1877-c92b-4d7e-9ab6-f01f30b12df9"",
                        ""description"": ""Test description"",
                        ""enabled"": true,
                        ""imageFileName"": ""test.jpg"",
                        ""switchSettings"": []
                    }
                ]
            }";

            // Act
            ActuatorSettingsJson settings = JsonSerializer.Deserialize<ActuatorSettingsJson>(json);

            // Assert
            Assert.IsNotNull(settings);
            Assert.AreEqual(1, settings.ActuatorSettings.Count);
            Assert.AreEqual("TestActuator", settings.ActuatorSettings[0].Name);
            Assert.AreEqual("d91a1877-c92b-4d7e-9ab6-f01f30b12df9", settings.ActuatorSettings[0].Id);
            Assert.IsTrue(settings.ActuatorSettings[0].Enabled);
        }

        [TestMethod]
        public void RoundTripSerializationPreservesData()
        {
            // Arrange
            var original = ActuatorSettingsJson.CreateDefault();

            // Act
            var json = JsonSerializer.Serialize(original);
            ActuatorSettingsJson deserialized = JsonSerializer.Deserialize<ActuatorSettingsJson>(json);

            // Assert
            Assert.IsNotNull(deserialized);
            Assert.AreEqual(original.ActuatorSettings.Count, deserialized.ActuatorSettings.Count);
            Assert.AreEqual(original.ActuatorSettings[0].Name, deserialized.ActuatorSettings[0].Name);
            Assert.AreEqual(original.ActuatorSettings[0].Id, deserialized.ActuatorSettings[0].Id);
            Assert.AreEqual(original.ActuatorSettings[0].Enabled, deserialized.ActuatorSettings[0].Enabled);
        }

        [TestMethod]
        public void FactoryMethodCreatesKeyboardActuator()
        {
            // Act
            var actuator = ActuatorSettingJson.CreateKeyboardActuator();

            // Assert
            Assert.IsNotNull(actuator);
            Assert.AreEqual("Keyboard", actuator.Name);
            Assert.AreEqual("d91a1877-c92b-4d7e-9ab6-f01f30b12df9", actuator.Id);
            Assert.IsTrue(actuator.Enabled);
            Assert.AreEqual("KeyboardSwitch.jpg", actuator.ImageFileName);
            Assert.AreEqual(1, actuator.SwitchSettings.Count);
        }

        [TestMethod]
        public void FactoryMethodCreatesCameraActuator()
        {
            // Act
            var actuator = ActuatorSettingJson.CreateCameraActuator();

            // Assert
            Assert.IsNotNull(actuator);
            Assert.AreEqual("Camera", actuator.Name);
            Assert.IsFalse(actuator.Enabled);
            Assert.AreEqual("WebcamSwitch.jpg", actuator.ImageFileName);
        }

        [TestMethod]
        public void FactoryMethodCreatesBCIActuator()
        {
            // Act
            var actuator = ActuatorSettingJson.CreateBCIActuator();

            // Assert
            Assert.IsNotNull(actuator);
            Assert.AreEqual("BCI", actuator.Name);
            Assert.IsFalse(actuator.Enabled);
            Assert.AreEqual("BCISwitch.png", actuator.ImageFileName);
        }

        [TestMethod]
        public void SwitchFactoryCreatesTriggerSwitch()
        {
            // Act
            var sw = SwitchSettingJson.CreateTriggerSwitch();

            // Assert
            Assert.IsNotNull(sw);
            Assert.AreEqual("Trigger", sw.Name);
            Assert.AreEqual("F12", sw.Source);
            Assert.IsTrue(sw.Enabled);
            Assert.IsTrue(sw.Actuate);
            Assert.AreEqual("@Trigger", sw.Command);
            Assert.AreEqual("beep.wav", sw.BeepFile);
        }
    }

    [TestClass]
    public class ActuatorSettingsValidatorTests
    {
        private ActuatorSettingsValidator _validator;

        [TestInitialize]
        public void Setup()
        {
            _validator = new ActuatorSettingsValidator();
        }

        [TestMethod]
        public void ValidSettingsPass()
        {
            // Arrange
            var settings = ActuatorSettingsJson.CreateDefault();

            // Act
            ValidationResult result = _validator.Validate(settings);

            // Assert
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(0, result.Errors.Count);
        }

        [TestMethod]
        public void NullActuatorSettingsListFails()
        {
            // Arrange
            var settings = new ActuatorSettingsJson { ActuatorSettings = null };

            // Act
            ValidationResult result = _validator.Validate(settings);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Exists(e => e.ErrorMessage.Contains("cannot be null")));
        }

        [TestMethod]
        public void EmptyActuatorSettingsListFails()
        {
            // Arrange
            var settings = new ActuatorSettingsJson();

            // Act
            ValidationResult result = _validator.Validate(settings);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Exists(e => e.ErrorMessage.Contains("At least one actuator")));
        }

        [TestMethod]
        public void NoEnabledActuatorsFails()
        {
            // Arrange
            var settings = new ActuatorSettingsJson
            {
                ActuatorSettings = new System.Collections.Generic.List<ActuatorSettingJson>
                {
                    new ActuatorSettingJson
                    {
                        Name = "Test",
                        Id = Guid.NewGuid().ToString(),
                        Enabled = false
                    }
                }
            };

            // Act
            ValidationResult result = _validator.Validate(settings);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Exists(e => e.ErrorMessage.Contains("At least one actuator must be enabled")));
        }

        [TestMethod]
        public void DuplicateActuatorIdsFails()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var settings = new ActuatorSettingsJson
            {
                ActuatorSettings = new System.Collections.Generic.List<ActuatorSettingJson>
                {
                    new ActuatorSettingJson { Name = "Test1", Id = id, Enabled = true },
                    new ActuatorSettingJson { Name = "Test2", Id = id, Enabled = false }
                }
            };

            // Act
            ValidationResult result = _validator.Validate(settings);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Exists(e => e.ErrorMessage.Contains("Actuator IDs must be unique")));
        }

        [TestMethod]
        public void InvalidGuidFails()
        {
            // Arrange
            var settings = new ActuatorSettingsJson
            {
                ActuatorSettings = new System.Collections.Generic.List<ActuatorSettingJson>
                {
                    new ActuatorSettingJson
                    {
                        Name = "Test",
                        Id = "not-a-guid",
                        Enabled = true
                    }
                }
            };

            // Act
            ValidationResult result = _validator.Validate(settings);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Exists(e => e.ErrorMessage.Contains("valid GUID")));
        }

        [TestMethod]
        public void EmptyActuatorNameFails()
        {
            // Arrange
            var settings = new ActuatorSettingsJson
            {
                ActuatorSettings = new System.Collections.Generic.List<ActuatorSettingJson>
                {
                    new ActuatorSettingJson
                    {
                        Name = "",
                        Id = Guid.NewGuid().ToString(),
                        Enabled = true
                    }
                }
            };

            // Act
            ValidationResult result = _validator.Validate(settings);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Exists(e => e.ErrorMessage.Contains("Actuator name is required")));
        }

        [TestMethod]
        public void EnabledActuatorWithoutEnabledSwitchFails()
        {
            // Arrange
            var settings = new ActuatorSettingsJson
            {
                ActuatorSettings = new System.Collections.Generic.List<ActuatorSettingJson>
                {
                    new ActuatorSettingJson
                    {
                        Name = "Test",
                        Id = Guid.NewGuid().ToString(),
                        Enabled = true,
                        SwitchSettings = new System.Collections.Generic.List<SwitchSettingJson>
                        {
                            new SwitchSettingJson { Name = "S1", Enabled = false }
                        }
                    }
                }
            };

            // Act
            ValidationResult result = _validator.Validate(settings);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Exists(e => e.ErrorMessage.Contains("at least one enabled switch")));
        }

        [TestMethod]
        public void DuplicateSwitchNamesWithinActuatorFails()
        {
            // Arrange
            var settings = new ActuatorSettingsJson
            {
                ActuatorSettings = new System.Collections.Generic.List<ActuatorSettingJson>
                {
                    new ActuatorSettingJson
                    {
                        Name = "Test",
                        Id = Guid.NewGuid().ToString(),
                        Enabled = true,
                        SwitchSettings = new System.Collections.Generic.List<SwitchSettingJson>
                        {
                            new SwitchSettingJson { Name = "Trigger", Enabled = true, Actuate = true, Command = "@Trigger" },
                            new SwitchSettingJson { Name = "Trigger", Enabled = false }
                        }
                    }
                }
            };

            // Act
            ValidationResult result = _validator.Validate(settings);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Exists(e => e.ErrorMessage.Contains("Switch names must be unique")));
        }

        [TestMethod]
        public void EnabledActuatingSwitchWithoutCommandFails()
        {
            // Arrange
            var settings = new ActuatorSettingsJson
            {
                ActuatorSettings = new System.Collections.Generic.List<ActuatorSettingJson>
                {
                    new ActuatorSettingJson
                    {
                        Name = "Test",
                        Id = Guid.NewGuid().ToString(),
                        Enabled = true,
                        SwitchSettings = new System.Collections.Generic.List<SwitchSettingJson>
                        {
                            new SwitchSettingJson { Name = "S1", Enabled = true, Actuate = true, Command = "" }
                        }
                    }
                }
            };

            // Act
            ValidationResult result = _validator.Validate(settings);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Exists(e => e.ErrorMessage.Contains("must have a command")));
        }
    }
}
