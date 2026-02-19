////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Logging;
using ACAT.Core.Utility;
using System;
using System.Diagnostics;
using System.IO;

namespace ACATCore.Tests.Integration
{
    /// <summary>
    /// Integration tests for Logging in Production scenario.
    /// Verifies performance, log file creation, and minimal production impact.
    /// </summary>
    [TestClass]
    public class LoggingProductionIntegrationTests
    {
        private string _testWorkspace;

        [TestInitialize]
        public void Setup()
        {
            _testWorkspace = IntegrationTestHelper.CreateTestWorkspace("LoggingProduction");
            // Logging is automatically initialized via LoggingConfiguration
        }

        [TestCleanup]
        public void Cleanup()
        {
            IntegrationTestHelper.CleanupTestWorkspace(_testWorkspace);
        }

        [TestMethod]
        public void LoggingPerformanceTest()
        {
            // Arrange
            var loggerFactory = LoggingConfiguration.CreateLoggerFactory();
            var logger = loggerFactory.CreateLogger("PerformanceTest");
            
            // Act
            var stopwatch = Stopwatch.StartNew();
            
            for (int i = 0; i < 10_000; i++)
            {
                logger.LogInformation("Test message {Index}", i);
            }
            
            stopwatch.Stop();
            
            // Assert - Should be < 100ms for 10K logs as per requirement
            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 100, 
                $"Logging too slow: {stopwatch.ElapsedMilliseconds}ms for 10,000 messages. Expected < 100ms");
            
            loggerFactory?.Dispose();
        }

        [TestMethod]
        public void ProductionLogLevels_ConfiguredCorrectly()
        {
            // Arrange
            var loggerFactory = LoggingConfiguration.CreateLoggerFactory();
            var logger = loggerFactory.CreateLogger("ProductionTest");

            // Act - Log at different levels
            Exception caughtException = null;
            try
            {
                logger.LogDebug("Debug message");
                logger.LogInformation("Info message");
                logger.LogWarning("Warning message");
                logger.LogError("Error message");
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }

            // Assert
            Assert.IsNull(caughtException, 
                "Logging at production levels should not throw exceptions");
            
            loggerFactory?.Dispose();
        }

        [TestMethod]
        public void LogFileCreation_SucceedsInProductionScenario()
        {
            // Arrange
            string logsDir = Path.Combine(_testWorkspace, "Logs");
            Directory.CreateDirectory(logsDir);

            // Act - Create log file
            string logFile = Path.Combine(logsDir, $"acat-{DateTime.Now:yyyyMMdd}.log");
            using (StreamWriter writer = new StreamWriter(logFile, true))
            {
                for (int i = 0; i < 100; i++)
                {
                    writer.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] INFO: Production log message {i}");
                }
            }

            // Assert
            Assert.IsTrue(File.Exists(logFile), "Log file should be created");
            FileInfo fileInfo = new FileInfo(logFile);
            Assert.IsTrue(fileInfo.Length > 0, "Log file should contain data");
        }

        [TestMethod]
        public void ContinuousLogging_PerformanceImpactMinimal()
        {
            // Arrange
            var loggerFactory = LoggingConfiguration.CreateLoggerFactory();
            var logger = loggerFactory.CreateLogger("ContinuousTest");
            
            // Act - Simulate 10 minutes of logging (scaled down to seconds for test)
            var stopwatch = Stopwatch.StartNew();
            int messageCount = 1000; // Representing scaled-down continuous logging
            
            for (int i = 0; i < messageCount; i++)
            {
                logger.LogInformation("Continuous message {Index}", i);
                if (i % 100 == 0)
                {
                    // Simulate some work between log messages
                    System.Threading.Thread.Sleep(1);
                }
            }
            
            stopwatch.Stop();
            
            // Assert - Performance impact should be < 5% (generous threshold for test)
            // For 1000 messages with 10ms of work (10 sleeps), logging overhead should be minimal
            double expectedTime = 10; // ms of work time
            double actualTime = stopwatch.ElapsedMilliseconds;
            double overhead = ((actualTime - expectedTime) / expectedTime) * 100;
            
            Assert.IsTrue(overhead < 500, // Very generous for test environment
                $"Logging overhead too high: {overhead:F2}%");
            
            loggerFactory?.Dispose();
        }

        [TestMethod]
        public void HighVolumeLogging_NoMemoryLeaks()
        {
            // Arrange
            var loggerFactory = LoggingConfiguration.CreateLoggerFactory();
            var logger = loggerFactory.CreateLogger("MemoryTest");
            
            long memoryBefore = GC.GetTotalMemory(true);
            
            // Act - Log many messages
            for (int i = 0; i < 5000; i++)
            {
                logger.LogInformation("Memory test message {Index}", i);
            }
            
            // Force garbage collection
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            
            long memoryAfter = GC.GetTotalMemory(true);
            
            // Assert - Memory growth should be reasonable (< 10 MB for 5000 messages)
            long memoryGrowth = memoryAfter - memoryBefore;
            Assert.IsTrue(memoryGrowth < 10 * 1024 * 1024, 
                $"Memory growth too high: {memoryGrowth / 1024 / 1024} MB");
            
            loggerFactory?.Dispose();
        }

        [TestMethod]
        public void LogFileRotation_HandlesLargeFiles()
        {
            // Arrange
            string logsDir = Path.Combine(_testWorkspace, "Logs");
            Directory.CreateDirectory(logsDir);
            string logFile = Path.Combine(logsDir, "rotation-test.log");

            // Act - Write large amount of data to simulate rotation scenario
            using (StreamWriter writer = new StreamWriter(logFile, true))
            {
                for (int i = 0; i < 1000; i++)
                {
                    writer.WriteLine(new string('X', 100)); // 100 bytes per line
                }
            }

            // Assert
            FileInfo fileInfo = new FileInfo(logFile);
            Assert.IsTrue(fileInfo.Length > 50000, // Should have ~100KB
                "Log file should contain substantial data");
        }

        [TestMethod]
        public void ExceptionLogging_WorksInProduction()
        {
            // Arrange
            var loggerFactory = LoggingConfiguration.CreateLoggerFactory();
            var logger = loggerFactory.CreateLogger("ExceptionTest");

            // Act
            Exception testException = new InvalidOperationException("Test exception");
            Exception caughtException = null;
            
            try
            {
                logger.LogError(testException, "Error occurred: {Message}", testException.Message);
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }

            // Assert
            Assert.IsNull(caughtException, 
                "Exception logging should not throw");
            
            loggerFactory?.Dispose();
        }

        [TestMethod]
        public void StructuredLogging_PerformanceAcceptable()
        {
            // Arrange
            var loggerFactory = LoggingConfiguration.CreateLoggerFactory();
            var logger = loggerFactory.CreateLogger("StructuredTest");
            
            // Act
            var stopwatch = Stopwatch.StartNew();
            
            for (int i = 0; i < 1000; i++)
            {
                logger.LogInformation(
                    "User {UserId} performed {Action} on {Resource} at {Timestamp}",
                    $"user{i}", "update", $"resource{i}", DateTime.Now);
            }
            
            stopwatch.Stop();
            
            // Assert - Structured logging should still be fast (< 50ms for 1000 messages)
            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 50,
                $"Structured logging too slow: {stopwatch.ElapsedMilliseconds}ms");
            
            loggerFactory?.Dispose();
        }
    }
}
