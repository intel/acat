////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ActuatorSettingsConverterTests.cs
//
// Unit tests for ActuatorSettingsConverter
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.ActuatorManagement.Settings;
using ACAT.Core.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace ACATCore.Tests.Configuration
{
    [TestClass]
    public class ActuatorSettingsConverterTests
    {
        [TestMethod]
        public void ConvertJsonToLegacy_BasicProperties()
        {
            // Arrange
            var jsonActuator = new ActuatorSettingJson
            {
                Name = "TestActuator",
                Id = "d91a1877-c92b-4d7e-9ab6-f01f30b12df9",
                Description = "Test description",
                Enabled = true,
                ImageFileName = "test.jpg",
                SwitchSettings = new List<SwitchSettingJson>()
            };

            // Act
            var legacyActuator = ActuatorSettingsConverter.FromJson(jsonActuator);

            // Assert
            Assert.IsNotNull(legacyActuator);
            Assert.AreEqual("TestActuator", legacyActuator.Name);
            Assert.AreEqual(new Guid("d91a1877-c92b-4d7e-9ab6-f01f30b12df9"), legacyActuator.Id);
            Assert.AreEqual("Test description", legacyActuator.Description);
            Assert.IsTrue(legacyActuator.Enabled);
            Assert.AreEqual("test.jpg", legacyActuator.ImageFileName);
        }

        [TestMethod]
        public void ConvertJsonToLegacy_SwitchSettings()
        {
            // Arrange
            var jsonActuator = new ActuatorSettingJson
            {
                Name = "TestActuator",
                Id = Guid.NewGuid().ToString(),
                Enabled = true,
                SwitchSettings = new List<SwitchSettingJson>
                {
                    new SwitchSettingJson
                    {
                        Name = "Trigger",
                        Source = "F12",
                        Description = "Trigger switch",
                        Enabled = true,
                        Actuate = true,
                        Command = "@Trigger",
                        MinHoldTime = "@MinActuationHoldTime",
                        BeepFile = "beep.wav"
                    }
                }
            };

            // Act
            var legacyActuator = ActuatorSettingsConverter.FromJson(jsonActuator);

            // Assert
            Assert.IsNotNull(legacyActuator.SwitchSettings);
            Assert.AreEqual(1, legacyActuator.SwitchSettings.Count);
            
            var legacySwitch = legacyActuator.SwitchSettings[0];
            Assert.AreEqual("Trigger", legacySwitch.Name);
            Assert.AreEqual("F12", legacySwitch.Source);
            Assert.AreEqual("Trigger switch", legacySwitch.Description);
            Assert.IsTrue(legacySwitch.Enabled);
            Assert.IsTrue(legacySwitch.Actuate);
            Assert.AreEqual("@Trigger", legacySwitch.Command);
            Assert.AreEqual("@MinActuationHoldTime", legacySwitch.MinHoldTime);
            Assert.AreEqual("beep.wav", legacySwitch.BeepFile);
        }

        [TestMethod]
        public void ConvertLegacyToJson_BasicProperties()
        {
            // Arrange
            var legacyActuator = new ActuatorSetting
            {
                Name = "TestActuator",
                Id = new Guid("d91a1877-c92b-4d7e-9ab6-f01f30b12df9"),
                Description = "Test description",
                Enabled = true,
                ImageFileName = "test.jpg",
                SwitchSettings = new List<SwitchSetting>()
            };

            // Act
            var jsonActuator = ActuatorSettingsConverter.ToJson(legacyActuator);

            // Assert
            Assert.IsNotNull(jsonActuator);
            Assert.AreEqual("TestActuator", jsonActuator.Name);
            Assert.AreEqual("d91a1877-c92b-4d7e-9ab6-f01f30b12df9", jsonActuator.Id);
            Assert.AreEqual("Test description", jsonActuator.Description);
            Assert.IsTrue(jsonActuator.Enabled);
            Assert.AreEqual("test.jpg", jsonActuator.ImageFileName);
        }

        [TestMethod]
        public void ConvertLegacyToJson_SwitchSettings()
        {
            // Arrange
            var legacyActuator = new ActuatorSetting
            {
                Name = "TestActuator",
                Id = Guid.NewGuid(),
                Enabled = true,
                SwitchSettings = new List<SwitchSetting>
                {
                    new SwitchSetting
                    {
                        Name = "Trigger",
                        Source = "F12",
                        Description = "Trigger switch",
                        Enabled = true,
                        Actuate = true,
                        Command = "@Trigger",
                        MinHoldTime = "@MinActuationHoldTime",
                        BeepFile = "beep.wav"
                    }
                }
            };

            // Act
            var jsonActuator = ActuatorSettingsConverter.ToJson(legacyActuator);

            // Assert
            Assert.IsNotNull(jsonActuator.SwitchSettings);
            Assert.AreEqual(1, jsonActuator.SwitchSettings.Count);
            
            var jsonSwitch = jsonActuator.SwitchSettings[0];
            Assert.AreEqual("Trigger", jsonSwitch.Name);
            Assert.AreEqual("F12", jsonSwitch.Source);
            Assert.AreEqual("Trigger switch", jsonSwitch.Description);
            Assert.IsTrue(jsonSwitch.Enabled);
            Assert.IsTrue(jsonSwitch.Actuate);
            Assert.AreEqual("@Trigger", jsonSwitch.Command);
            Assert.AreEqual("@MinActuationHoldTime", jsonSwitch.MinHoldTime);
            Assert.AreEqual("beep.wav", jsonSwitch.BeepFile);
        }

        [TestMethod]
        public void RoundTripConversion_PreservesData()
        {
            // Arrange
            var originalLegacy = new ActuatorSetting
            {
                Name = "Keyboard",
                Id = new Guid("d91a1877-c92b-4d7e-9ab6-f01f30b12df9"),
                Description = "Keyboard actuator",
                Enabled = true,
                ImageFileName = "keyboard.jpg",
                SwitchSettings = new List<SwitchSetting>
                {
                    new SwitchSetting
                    {
                        Name = "Trigger",
                        Source = "F12",
                        Enabled = true,
                        Actuate = true,
                        Command = "@Trigger"
                    }
                }
            };

            // Act - Legacy -> JSON -> Legacy
            var json = ActuatorSettingsConverter.ToJson(originalLegacy);
            var convertedLegacy = ActuatorSettingsConverter.FromJson(json);

            // Assert
            Assert.AreEqual(originalLegacy.Name, convertedLegacy.Name);
            Assert.AreEqual(originalLegacy.Id, convertedLegacy.Id);
            Assert.AreEqual(originalLegacy.Description, convertedLegacy.Description);
            Assert.AreEqual(originalLegacy.Enabled, convertedLegacy.Enabled);
            Assert.AreEqual(originalLegacy.ImageFileName, convertedLegacy.ImageFileName);
            Assert.AreEqual(originalLegacy.SwitchSettings.Count, convertedLegacy.SwitchSettings.Count);
            Assert.AreEqual(originalLegacy.SwitchSettings[0].Name, convertedLegacy.SwitchSettings[0].Name);
        }

        [TestMethod]
        public void ConvertJsonListToLegacyList()
        {
            // Arrange
            var jsonSettings = new ActuatorSettingsJson
            {
                ActuatorSettings = new List<ActuatorSettingJson>
                {
                    ActuatorSettingJson.CreateKeyboardActuator(),
                    ActuatorSettingJson.CreateCameraActuator()
                }
            };

            // Act
            var legacyList = ActuatorSettingsConverter.FromJson(jsonSettings);

            // Assert
            Assert.IsNotNull(legacyList);
            Assert.AreEqual(2, legacyList.Count);
            Assert.AreEqual("Keyboard", legacyList[0].Name);
            Assert.AreEqual("Camera", legacyList[1].Name);
        }

        [TestMethod]
        public void ConvertLegacyListToJson()
        {
            // Arrange
            var legacyList = new List<ActuatorSetting>
            {
                new ActuatorSetting("Keyboard", Guid.NewGuid(), enabled: true),
                new ActuatorSetting("Camera", Guid.NewGuid(), enabled: false)
            };

            // Act
            var jsonSettings = ActuatorSettingsConverter.ToJson(legacyList);

            // Assert
            Assert.IsNotNull(jsonSettings);
            Assert.IsNotNull(jsonSettings.ActuatorSettings);
            Assert.AreEqual(2, jsonSettings.ActuatorSettings.Count);
            Assert.AreEqual("Keyboard", jsonSettings.ActuatorSettings[0].Name);
            Assert.AreEqual("Camera", jsonSettings.ActuatorSettings[1].Name);
        }

        [TestMethod]
        public void ConvertNullJsonReturnsEmpty()
        {
            // Act
            var legacyList = ActuatorSettingsConverter.FromJson((ActuatorSettingsJson)null);

            // Assert
            Assert.IsNotNull(legacyList);
            Assert.AreEqual(0, legacyList.Count);
        }

        [TestMethod]
        public void ConvertNullLegacyListReturnsEmptyJson()
        {
            // Act
            var jsonSettings = ActuatorSettingsConverter.ToJson((List<ActuatorSetting>)null);

            // Assert
            Assert.IsNotNull(jsonSettings);
            Assert.IsNotNull(jsonSettings.ActuatorSettings);
            Assert.AreEqual(0, jsonSettings.ActuatorSettings.Count);
        }

        [TestMethod]
        public void ConvertInvalidGuidHandlesGracefully()
        {
            // Arrange
            var jsonActuator = new ActuatorSettingJson
            {
                Name = "Test",
                Id = "not-a-guid",
                Enabled = true
            };

            // Act
            var legacyActuator = ActuatorSettingsConverter.FromJson(jsonActuator);

            // Assert
            Assert.IsNotNull(legacyActuator);
            Assert.AreEqual(Guid.Empty, legacyActuator.Id);
        }

        [TestMethod]
        public void ConvertNullStringsHandlesGracefully()
        {
            // Arrange
            var jsonActuator = new ActuatorSettingJson
            {
                Name = null,
                Id = Guid.NewGuid().ToString(),
                Description = null,
                ImageFileName = null
            };

            // Act
            var legacyActuator = ActuatorSettingsConverter.FromJson(jsonActuator);

            // Assert
            Assert.IsNotNull(legacyActuator);
            Assert.AreEqual(string.Empty, legacyActuator.Name);
            Assert.AreEqual(string.Empty, legacyActuator.Description);
            Assert.AreEqual(string.Empty, legacyActuator.ImageFileName);
        }
    }
}
