////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// XmlDeserializersTests.cs
//
// Unit tests for XML deserializers
//
////////////////////////////////////////////////////////////////////////////

using ACAT.ConfigMigrationTool;
using ACAT.ConfigMigrationTool.Configuration;

namespace ACAT.ConfigMigrationTool.Tests
{
    [TestClass]
    public class XmlDeserializersTests
    {
        private string _testDataDir = "";

        [TestInitialize]
        public void Setup()
        {
            _testDataDir = Path.Combine(Path.GetTempPath(), "acat-test-" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDataDir);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(_testDataDir))
            {
                Directory.Delete(_testDataDir, true);
            }
        }

        [TestMethod]
        public void DeserializeActuatorSettings_ValidXml_Success()
        {
            // Arrange
            var xmlContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<ActuatorConfig>
  <ActuatorSettings>
    <ActuatorSetting>
      <Name>Keyboard</Name>
      <Id>d91a1877-c92b-4d7e-9ab6-f01f30b12df9</Id>
      <Description>Keyboard actuator</Description>
      <Enabled>true</Enabled>
      <ImageFileName>keyboard.jpg</ImageFileName>
      <SwitchSettings>
        <SwitchSetting>
          <Name>Trigger</Name>
          <Source>F12</Source>
          <Description>Trigger switch</Description>
          <Enabled>true</Enabled>
          <Actuate>true</Actuate>
          <Command>@Trigger</Command>
          <MinHoldTime>@MinActuationHoldTime</MinHoldTime>
          <BeepFile>beep.wav</BeepFile>
        </SwitchSetting>
      </SwitchSettings>
    </ActuatorSetting>
  </ActuatorSettings>
</ActuatorConfig>";

            var xmlPath = Path.Combine(_testDataDir, "test.xml");
            File.WriteAllText(xmlPath, xmlContent);

            // Act
            var result = XmlDeserializers.DeserializeActuatorSettings(xmlPath);

            // Assert
            Assert.IsNotNull(result);
            Assert.HasCount(1, result.ActuatorSettings);
            Assert.AreEqual("Keyboard", result.ActuatorSettings[0].Name);
            Assert.AreEqual("d91a1877-c92b-4d7e-9ab6-f01f30b12df9", result.ActuatorSettings[0].Id);
            Assert.IsTrue(result.ActuatorSettings[0].Enabled);
            Assert.HasCount(1, result.ActuatorSettings[0].SwitchSettings);
            Assert.AreEqual("Trigger", result.ActuatorSettings[0].SwitchSettings[0].Name);
        }

        [TestMethod]
        public void DeserializeTheme_ValidXml_Success()
        {
            // Arrange
            var xmlContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<ACAT>
  <Theme description=""Test Theme"">
    <ColorSchemes>
      <ColorScheme name=""Scanner""
                   background=""#232433""
                   foreground=""White""
                   highlightBackground=""#ffaa00""
                   highlightForeground=""#232433"" />
    </ColorSchemes>
  </Theme>
</ACAT>";

            var xmlPath = Path.Combine(_testDataDir, "test.xml");
            File.WriteAllText(xmlPath, xmlContent);

            // Act
            var result = XmlDeserializers.DeserializeTheme(xmlPath);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Test Theme", result.Description);
            Assert.HasCount(1, result.ColorSchemes);
            Assert.AreEqual("Scanner", result.ColorSchemes[0].Name);
            Assert.AreEqual("#232433", result.ColorSchemes[0].Background);
            Assert.AreEqual("White", result.ColorSchemes[0].Foreground);
        }

        [TestMethod]
        public void DeserializePanelConfig_ValidXml_Success()
        {
            // Arrange
            var xmlContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<ACAT>
  <WidgetAttributes>
    <WidgetAttribute name=""Button1"" label=""OK"" fontsize=""18"" fontname=""Arial"" />
  </WidgetAttributes>
  <Layout colorScheme=""Dialog"">
    <Widget class=""RowWidget"" name=""Row1"">
      <Widget class=""ScannerButton"" name=""Button1"" />
    </Widget>
  </Layout>
</ACAT>";

            var xmlPath = Path.Combine(_testDataDir, "test.xml");
            File.WriteAllText(xmlPath, xmlContent);

            // Act
            var result = XmlDeserializers.DeserializePanelConfig(xmlPath);

            // Assert
            Assert.IsNotNull(result);
            Assert.HasCount(1, result.WidgetAttributes);
            Assert.AreEqual("Button1", result.WidgetAttributes[0].Name);
            Assert.AreEqual("OK", result.WidgetAttributes[0].Label);
            Assert.IsNotNull(result.Layout);
            Assert.AreEqual("Dialog", result.Layout.ColorScheme);
            Assert.HasCount(1, result.Layout.Widgets);
        }

        [TestMethod]
        public void DeserializeActuatorSettings_InvalidFormat_ThrowsException()
        {
            // Arrange
            var xmlContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<WrongRoot>
  <Data>Invalid</Data>
</WrongRoot>";

            var xmlPath = Path.Combine(_testDataDir, "test.xml");
            File.WriteAllText(xmlPath, xmlContent);

            // Act & Assert
            try
            {
                XmlDeserializers.DeserializeActuatorSettings(xmlPath);
                Assert.Fail("Expected InvalidOperationException to be thrown");
            }
            catch (InvalidOperationException)
            {
                // Expected exception
            }
        }
    }
}
