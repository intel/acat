////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ACATCore.Tests.Integration
{
    /// <summary>
    /// Provides utilities for integration testing including test workspace management
    /// NOTE: Consider using TestWorkspace from ACATCore.Tests.Shared for new tests
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
                catch (IOException)
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

        /// <summary>
        /// Creates a valid JSON configuration file for testing
        /// </summary>
        public static string CreateValidJsonConfig(string directory, string fileName, object configObject)
        {
            string filePath = Path.Combine(directory, fileName);
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(configObject, options);
            File.WriteAllText(filePath, json);
            return filePath;
        }

        /// <summary>
        /// Copies a directory recursively for testing
        /// </summary>
        public static void CopyDirectory(string sourceDir, string destDir)
        {
            if (!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            // Copy files
            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(destDir, fileName);
                File.Copy(file, destFile, true);
            }

            // Copy subdirectories
            foreach (string subDir in Directory.GetDirectories(sourceDir))
            {
                string dirName = Path.GetFileName(subDir);
                string destSubDir = Path.Combine(destDir, dirName);
                CopyDirectory(subDir, destSubDir);
            }
        }

        /// <summary>
        /// Verifies that a directory structure matches expected pattern
        /// </summary>
        public static bool VerifyDirectoryStructure(string directory, params string[] expectedPaths)
        {
            foreach (string expectedPath in expectedPaths)
            {
                string fullPath = Path.Combine(directory, expectedPath);
                if (!File.Exists(fullPath) && !Directory.Exists(fullPath))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Gets all files in a directory tree
        /// </summary>
        public static List<string> GetAllFiles(string directory, string searchPattern = "*.*")
        {
            if (!Directory.Exists(directory))
                return new List<string>();

            return Directory.GetFiles(directory, searchPattern, SearchOption.AllDirectories).ToList();
        }

        /// <summary>
        /// Compares two files for equality
        /// </summary>
        public static bool FilesAreEqual(string file1, string file2)
        {
            if (!File.Exists(file1) || !File.Exists(file2))
                return false;

            string content1 = File.ReadAllText(file1);
            string content2 = File.ReadAllText(file2);
            return content1 == content2;
        }

        /// <summary>
        /// Waits for a file to be created with timeout
        /// </summary>
        public static bool WaitForFile(string filePath, TimeSpan timeout)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            while (stopwatch.Elapsed < timeout)
            {
                if (File.Exists(filePath))
                    return true;
                System.Threading.Thread.Sleep(100);
            }
            return false;
        }

        /// <summary>
        /// Creates a backup of a file or directory
        /// </summary>
        public static string CreateBackup(string path)
        {
            if (!File.Exists(path) && !Directory.Exists(path))
                throw new ArgumentException($"Path does not exist: {path}");

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string backupPath = $"{path}.backup_{timestamp}";

            if (File.Exists(path))
            {
                File.Copy(path, backupPath);
            }
            else if (Directory.Exists(path))
            {
                CopyDirectory(path, backupPath);
            }

            return backupPath;
        }

        /// <summary>
        /// Verifies that a JSON file is valid
        /// </summary>
        public static bool IsValidJson(string filePath)
        {
            try
            {
                string content = File.ReadAllText(filePath);
                JsonDocument.Parse(content);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the size of a directory in bytes
        /// </summary>
        public static long GetDirectorySize(string directory)
        {
            if (!Directory.Exists(directory))
                return 0;

            return Directory.GetFiles(directory, "*", SearchOption.AllDirectories)
                .Sum(file => new FileInfo(file).Length);
        }
    }
}
