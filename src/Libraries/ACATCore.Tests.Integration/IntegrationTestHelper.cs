////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;

namespace ACATCore.Tests.Integration
{
    /// <summary>
    /// Provides utilities for integration testing including test workspace management
    /// </summary>
    public class IntegrationTestHelper
    {
        private static readonly string TempBasePath = Path.Combine(Path.GetTempPath(), "ACATIntegrationTests");

        /// <summary>
        /// Creates a unique test workspace directory
        /// </summary>
        public static string CreateTestWorkspace(string testName)
        {
            string testDir = Path.Combine(TempBasePath, testName, Guid.NewGuid().ToString());
            Directory.CreateDirectory(testDir);
            return testDir;
        }

        /// <summary>
        /// Cleans up a test workspace directory
        /// </summary>
        public static void CleanupTestWorkspace(string testDir)
        {
            if (Directory.Exists(testDir))
            {
                try
                {
                    Directory.Delete(testDir, true);
                }
                catch (UnauthorizedAccessException)
                {
                    // Retry after a brief delay if files are locked
                    System.Threading.Thread.Sleep(100);
                    try
                    {
                        Directory.Delete(testDir, true);
                    }
                    catch
                    {
                        // Best effort cleanup
                    }
                }
            }
        }

        /// <summary>
        /// Creates a sample XML configuration file for testing migration
        /// </summary>
        public static string CreateSampleXmlConfig(string directory, string configType)
        {
            string filePath;
            string content;

            if (configType == "ActuatorSettings")
            {
                filePath = Path.Combine(directory, "ActuatorSettings.xml");
                content = @"<?xml version=""1.0"" encoding=""utf-8""?>
<ACAT>
  <ActuatorSettings>
    <Actuator name=""Keyboard Actuator"" id=""9AF14CB3-0169-47E5-A413-43C5610ECAD4"" enabled=""True"" />
    <Actuator name=""Camera Actuator"" id=""EAF6F2AE-72C4-4334-A2D2-DCE60F9A2A9E"" enabled=""False"" />
  </ActuatorSettings>
</ACAT>";
            }
            else if (configType == "Theme")
            {
                filePath = Path.Combine(directory, "Theme.xml");
                content = @"<?xml version=""1.0"" encoding=""utf-8""?>
<ACAT>
  <Theme>
    <Name>Default</Name>
    <ColorSchemes>
      <ColorScheme name=""Scanner"">
        <ForegroundColor>White</ForegroundColor>
        <BackgroundColor>Black</BackgroundColor>
      </ColorScheme>
    </ColorSchemes>
  </Theme>
</ACAT>";
            }
            else
            {
                throw new ArgumentException($"Unknown config type: {configType}");
            }

            File.WriteAllText(filePath, content);
            return filePath;
        }

        /// <summary>
        /// Creates an invalid JSON configuration file for testing error handling
        /// </summary>
        public static string CreateInvalidJsonConfig(string directory, string fileName)
        {
            string filePath = Path.Combine(directory, fileName);
            string invalidJson = @"{
  ""name"": ""Test Config"",
  ""enabled"": true,
  ""missingClosingBrace"": ""value""
"; // Intentionally invalid - missing closing brace
            File.WriteAllText(filePath, invalidJson);
            return filePath;
        }

        /// <summary>
        /// Verifies that a directory contains expected files
        /// </summary>
        public static bool DirectoryContainsFile(string directory, string fileName)
        {
            return File.Exists(Path.Combine(directory, fileName));
        }

        /// <summary>
        /// Counts files matching a pattern in a directory
        /// </summary>
        public static int CountFilesMatching(string directory, string pattern)
        {
            if (!Directory.Exists(directory))
                return 0;

            return Directory.GetFiles(directory, pattern).Length;
        }
    }
}
