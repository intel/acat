////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace ACAT.Core.Tests
{
    /// <summary>
    /// Simple test program to verify Microsoft.Extensions.Logging configuration
    /// </summary>
    public class LoggingConfigurationTest
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=== ACAT Logging Configuration Test ===\n");

            ILoggerFactory loggerFactory = null;

            try
            {
                // Create a SINGLE shared logger factory (as should be done in real application)
                Console.WriteLine("Creating shared logger factory...");
                loggerFactory = LoggingConfiguration.CreateLoggerFactory();
                Console.WriteLine("✓ Logger factory created successfully\n");

                // Test 1: Create logger using factory
                Console.WriteLine("Test 1: Creating logger using factory.CreateLogger<T>()...");
                ILogger<LoggingConfigurationTest> logger1 = loggerFactory.CreateLogger<LoggingConfigurationTest>();
                Console.WriteLine("✓ Logger created successfully\n");

                // Test 2: Create logger with category name
                Console.WriteLine("Test 2: Creating logger with category name...");
                ILogger logger2 = loggerFactory.CreateLogger("TestCategory");
                Console.WriteLine("✓ Logger created successfully\n");

                // Test 3: Log at different levels
                Console.WriteLine("Test 3: Logging at different levels...");
                logger1.LogDebug("This is a DEBUG message");
                logger1.LogInformation("This is an INFORMATION message");
                logger1.LogWarning("This is a WARNING message");
                logger1.LogError("This is an ERROR message");
                Console.WriteLine("✓ Messages logged successfully\n");

                // Test 4: Structured logging with parameters
                Console.WriteLine("Test 4: Testing structured logging...");
                var userId = "TestUser123";
                DateTime timestamp = DateTime.Now;
                logger1.LogInformation("User {UserId} performed test at {Timestamp}", userId, timestamp);
                Console.WriteLine("✓ Structured logging successful\n");

                // Test 5: Exception logging
                Console.WriteLine("Test 5: Testing exception logging...");
                try
                {
                    throw new InvalidOperationException("Test exception for logging");
                }
                catch (Exception ex)
                {
                    logger1.LogError(ex, "Test exception caught and logged");
                }
                Console.WriteLine("✓ Exception logging successful\n");

                // Test 6: Verify log file location
                Console.WriteLine("Test 6: Verifying log file location...");
                var logsDir = FileUtils.GetLogsDir();
                Console.WriteLine($"Log directory: {logsDir}");
                
                if (Directory.Exists(logsDir))
                {
                    var logFiles = Directory.GetFiles(logsDir, "acat-*.txt");
                    if (logFiles.Length > 0)
                    {
                        Console.WriteLine($"✓ Found {logFiles.Length} log file(s):");
                        foreach (var file in logFiles)
                        {
                            Console.WriteLine($"  - {Path.GetFileName(file)} ({new FileInfo(file).Length} bytes)");
                        }
                    }
                    else
                    {
                        Console.WriteLine("⚠ Warning: No log files found. They may be created on application exit.");
                    }
                }
                else
                {
                    Console.WriteLine($"⚠ Warning: Log directory does not exist: {logsDir}");
                }
                Console.WriteLine();

                Console.WriteLine("=== All Tests Completed Successfully ===");
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n✗ ERROR: {ex.Message}");
                Console.WriteLine($"Stack trace:\n{ex.StackTrace}");
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
                Environment.Exit(1);
            }
            finally
            {
                // Dispose the logger factory to ensure logs are flushed
                loggerFactory?.Dispose();
                Console.WriteLine("\nLogger factory disposed - logs flushed to disk.");
            }
        }
    }
}
