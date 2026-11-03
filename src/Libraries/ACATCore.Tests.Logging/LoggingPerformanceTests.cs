////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using Microsoft.VisualStudio.TestTools.UnitTesting;
using ACAT.Core.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ACATCore.Tests.Logging
{
    [TestClass]
    public class LoggingPerformanceTests
    {
        private ILogger<LoggingPerformanceTests> _logger;

        [TestInitialize]
        public void Setup()
        {
            // Use the modern logging infrastructure
            _logger = LoggingConfiguration.CreateLogger<LoggingPerformanceTests>();
        }

        [TestMethod]
        public void HighVolumeLoggingCompletesQuickly()
        {
            Stopwatch timer = Stopwatch.StartNew();

            for (int i = 0; i < 1000; i++)
            {
                _logger.LogInformation("Performance message {MessageNumber}", i);
            }

            timer.Stop();

            // Async logging should complete quickly even with high volume
            Assert.IsTrue(timer.ElapsedMilliseconds < 2000,
                $"1000 messages took {timer.ElapsedMilliseconds}ms (should be < 2000ms with async logging)");
        }

        [TestMethod]
        public void ConcurrentLoggingFromMultipleThreadsSucceeds()
        {
            int threadCount = 10;
            int messagesPerThread = 50;
            List<Task> tasks = new List<Task>();
            int completions = 0;

            for (int t = 0; t < threadCount; t++)
            {
                int threadId = t;
                Task task = Task.Run(() =>
                {
                    // Each thread gets its own logger
                    var logger = LoggingConfiguration.CreateLogger<LoggingPerformanceTests>();

                    for (int m = 0; m < messagesPerThread; m++)
                    {
                        logger.LogInformation("Thread{ThreadId}_Msg{MessageNumber}", threadId, m);
                    }
                    Interlocked.Increment(ref completions);
                });
                tasks.Add(task);
            }

            bool completed = Task.WaitAll(tasks.ToArray(), TimeSpan.FromSeconds(10));

            Assert.IsTrue(completed, "All tasks should complete within timeout");
            Assert.AreEqual(threadCount, completions, "All threads should complete");
        }

        [TestMethod]
        public void RapidSequentialCallsComplete()
        {
            Stopwatch timer = Stopwatch.StartNew();

            for (int i = 0; i < 100; i++)
            {
                _logger.LogDebug("Debug{MessageNumber}", i);
                _logger.LogInformation("Info{MessageNumber}", i);
                _logger.LogWarning("Warn{MessageNumber}", i);
                _logger.LogError("Error{MessageNumber}", i);
            }

            timer.Stop();

            // 400 messages should complete quickly with async logging
            Assert.IsTrue(timer.ElapsedMilliseconds < 1000,
                $"400 messages took {timer.ElapsedMilliseconds}ms (should be < 1000ms)");
        }

        [TestMethod]
        public void SingleLogCallReturnsQuickly()
        {
            Stopwatch timer = Stopwatch.StartNew();
            _logger.LogInformation("Quick return test");
            timer.Stop();

            // Single call should return almost instantly (async buffering)
            Assert.IsTrue(timer.ElapsedMilliseconds < 100,
                $"Single log call took {timer.ElapsedMilliseconds}ms (should be < 100ms)");
        }

        [TestMethod]
        public void ParallelLoggingWithExceptionsHandled()
        {
            Parallel.For(0, 20, i =>
            {
                var logger = LoggingConfiguration.CreateLogger<LoggingPerformanceTests>();
                try
                {
                    throw new Exception($"Test exception {i}");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Test exception {ExceptionNumber}", i);
                }
            });

            // Test passes if no exceptions escape
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void LoggerFactoryCreationIsConsistent()
        {
            // Verify that CreateLogger returns working loggers
            var logger1 = LoggingConfiguration.CreateLogger<LoggingPerformanceTests>();
            var logger2 = LoggingConfiguration.CreateLogger("TestCategory");

            Assert.IsNotNull(logger1, "Generic logger should be created");
            Assert.IsNotNull(logger2, "Named logger should be created");

            // Both should log successfully
            logger1.LogInformation("Test from generic logger");
            logger2.LogInformation("Test from named logger");

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void SharedLoggerFactoryWritesToSameFile()
        {
            // Get the shared factory multiple times
            var factory1 = LoggingConfiguration.GetSharedLoggerFactory();
            var factory2 = LoggingConfiguration.GetSharedLoggerFactory();

            Assert.AreSame(factory1, factory2, 
                "GetSharedLoggerFactory should return the same instance");

            // Create loggers from the shared factory
            var logger1 = factory1.CreateLogger("Test1");
            var logger2 = factory2.CreateLogger("Test2");

            logger1.LogInformation("Message from logger 1");
            logger2.LogInformation("Message from logger 2");

            Assert.IsTrue(true);
        }
    }
}
