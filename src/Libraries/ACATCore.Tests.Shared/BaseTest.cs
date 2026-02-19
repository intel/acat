////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BaseTest.cs
//
// Base class for all ACAT unit tests providing common setup and utilities
//
////////////////////////////////////////////////////////////////////////////

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;

namespace ACATCore.Tests.Shared
{
    /// <summary>
    /// Base class for all ACAT unit tests
    /// Provides common setup, teardown, and utility methods
    /// </summary>
    [TestClass]
    public abstract class BaseTest
    {
        /// <summary>
        /// Test context for accessing test properties and results
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Temporary directory for test artifacts, unique per test
        /// </summary>
        protected string TestDirectory { get; private set; }

        /// <summary>
        /// Stopwatch for performance measurements
        /// </summary>
        protected Stopwatch Stopwatch { get; private set; }

        /// <summary>
        /// Initialize before each test
        /// </summary>
        [TestInitialize]
        public virtual void TestInitialize()
        {
            // Create unique test directory
            TestDirectory = Path.Combine(
                Path.GetTempPath(),
                "ACATTests",
                TestContext.TestName,
                Guid.NewGuid().ToString());
            Directory.CreateDirectory(TestDirectory);

            // Start stopwatch for performance tracking
            Stopwatch = Stopwatch.StartNew();

            WriteTestInfo($"Starting test: {TestContext.TestName}");
        }

        /// <summary>
        /// Cleanup after each test
        /// </summary>
        [TestCleanup]
        public virtual void TestCleanup()
        {
            // Stop stopwatch and log duration
            Stopwatch.Stop();
            WriteTestInfo($"Test completed in {Stopwatch.ElapsedMilliseconds}ms");

            // Cleanup test directory
            CleanupTestDirectory();
        }

        /// <summary>
        /// Writes informational message to test output
        /// </summary>
        protected void WriteTestInfo(string message)
        {
            TestContext?.WriteLine($"[INFO] {DateTime.Now:HH:mm:ss.fff} - {message}");
        }

        /// <summary>
        /// Writes debug message to test output
        /// </summary>
        protected void WriteTestDebug(string message)
        {
            TestContext?.WriteLine($"[DEBUG] {DateTime.Now:HH:mm:ss.fff} - {message}");
        }

        /// <summary>
        /// Writes warning message to test output
        /// </summary>
        protected void WriteTestWarning(string message)
        {
            TestContext?.WriteLine($"[WARN] {DateTime.Now:HH:mm:ss.fff} - {message}");
        }

        /// <summary>
        /// Creates a temporary file in the test directory
        /// </summary>
        protected string CreateTempFile(string fileName, string content = "")
        {
            string filePath = Path.Combine(TestDirectory, fileName);
            File.WriteAllText(filePath, content);
            return filePath;
        }

        /// <summary>
        /// Creates a temporary subdirectory in the test directory
        /// </summary>
        protected string CreateTempDirectory(string directoryName)
        {
            string dirPath = Path.Combine(TestDirectory, directoryName);
            Directory.CreateDirectory(dirPath);
            return dirPath;
        }

        /// <summary>
        /// Cleans up the test directory
        /// </summary>
        protected void CleanupTestDirectory()
        {
            if (!string.IsNullOrEmpty(TestDirectory) && Directory.Exists(TestDirectory))
            {
                try
                {
                    Directory.Delete(TestDirectory, true);
                }
                catch (UnauthorizedAccessException)
                {
                    // Retry after a brief delay if files are locked
                    System.Threading.Thread.Sleep(100);
                    try
                    {
                        Directory.Delete(TestDirectory, true);
                    }
                    catch (Exception ex)
                    {
                        WriteTestWarning($"Failed to cleanup test directory: {ex.Message}");
                    }
                }
                catch (Exception ex)
                {
                    WriteTestWarning($"Failed to cleanup test directory: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Asserts that an action throws an exception of the specified type
        /// </summary>
        protected void AssertThrows<TException>(Action action) where TException : Exception
        {
            try
            {
                action();
                Assert.Fail($"Expected exception of type {typeof(TException).Name} but no exception was thrown");
            }
            catch (TException)
            {
                // Expected exception caught
            }
            catch (Exception ex)
            {
                Assert.Fail($"Expected exception of type {typeof(TException).Name} but got {ex.GetType().Name}");
            }
        }

        /// <summary>
        /// Asserts that an action does not throw any exception
        /// </summary>
        protected void AssertNoThrow(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Assert.Fail($"Expected no exception but got {ex.GetType().Name}: {ex.Message}");
            }
        }

        /// <summary>
        /// Measures execution time of an action
        /// </summary>
        protected TimeSpan MeasureTime(Action action)
        {
            var sw = Stopwatch.StartNew();
            action();
            sw.Stop();
            return sw.Elapsed;
        }

        /// <summary>
        /// Asserts that an action completes within specified time
        /// </summary>
        protected void AssertCompletesWithin(Action action, TimeSpan timeout, string message = "")
        {
            var elapsed = MeasureTime(action);
            if (elapsed > timeout)
            {
                Assert.Fail($"Action took {elapsed.TotalMilliseconds}ms but should complete within {timeout.TotalMilliseconds}ms. {message}");
            }
        }
    }
}
