////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ContextThreadSafetyTests.cs
//
// Unit tests for Context class thread safety
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.ActuatorManagement;
using ACAT.Core.PanelManagement;
using ACAT.Core.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ACATCore.Tests.Configuration
{
    [TestClass]
    public class ContextThreadSafetyTests
    {
        [TestCleanup]
        public void Cleanup()
        {
            Context.ServiceProvider = null;
        }

        [TestMethod]
        public void GetManager_ConcurrentAccess_AllThreadsGetSameInstance()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();
            Context.ServiceProvider = services.BuildServiceProvider();

            var managers = new System.Collections.Concurrent.ConcurrentBag<IActuatorManager>();
            const int threadCount = 10;
            const int iterationsPerThread = 100;

            // Act - Multiple threads trying to get manager simultaneously
            Parallel.For(0, threadCount, _ =>
            {
                for (int i = 0; i < iterationsPerThread; i++)
                {
                    var manager = Context.GetManager<IActuatorManager>();
                    managers.Add(manager);
                }
            });

            // Assert - All threads should get the same singleton instance
            var distinctManagers = managers.Distinct().ToList();
            Assert.AreEqual(1, distinctManagers.Count, 
                "All threads should receive the same singleton instance");
        }

        [TestMethod]
        public void ServiceProvider_ConcurrentSet_NoExceptions()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();
            var provider1 = services.BuildServiceProvider();
            var provider2 = services.BuildServiceProvider();

            var exceptions = new System.Collections.Concurrent.ConcurrentBag<Exception>();

            // Act - Multiple threads trying to set ServiceProvider
            Parallel.For(0, 10, i =>
            {
                try
                {
                    Context.ServiceProvider = (i % 2 == 0) ? provider1 : provider2;
                    Thread.Sleep(1); // Small delay to increase race condition likelihood
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            });

            // Assert - No exceptions should occur (thread-safe property)
            Assert.AreEqual(0, exceptions.Count, 
                $"ServiceProvider setter should be thread-safe. Exceptions: {string.Join(", ", exceptions.Select(e => e.Message))}");
        }

        [TestMethod]
        public void GetManager_ConcurrentWithServiceProviderChange_NoExceptions()
        {
            // Arrange
            var services1 = new ServiceCollection();
            services1.AddLogging();
            services1.AddACATServices();
            var provider1 = services1.BuildServiceProvider();

            var services2 = new ServiceCollection();
            services2.AddLogging();
            services2.AddACATServices();
            var provider2 = services2.BuildServiceProvider();

            Context.ServiceProvider = provider1;

            var exceptions = new System.Collections.Concurrent.ConcurrentBag<Exception>();
            var cancellation = new CancellationTokenSource();

            // Act - One thread reading managers, another changing ServiceProvider
            var readTask = Task.Run(() =>
            {
                while (!cancellation.Token.IsCancellationRequested)
                {
                    try
                    {
                        var manager = Context.GetManager<IActuatorManager>();
                        if (manager == null)
                        {
                            exceptions.Add(new Exception("GetManager returned null"));
                        }
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }
            });

            var writeTask = Task.Run(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    try
                    {
                        Context.ServiceProvider = (i % 2 == 0) ? provider1 : provider2;
                        Thread.Sleep(1);
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }
            });

            // Wait for write task to complete, then stop read task
            writeTask.Wait();
            cancellation.Cancel();
            readTask.Wait(TimeSpan.FromSeconds(5));

            // Assert - Should handle concurrent read/write without exceptions
            Assert.AreEqual(0, exceptions.Count,
                $"Concurrent access should be safe. Exceptions: {string.Join(", ", exceptions.Select(e => e.Message))}");
        }

        [TestMethod]
        public void MultipleManagers_ConcurrentAccess_AllReturnCorrectTypes()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();
            Context.ServiceProvider = services.BuildServiceProvider();

            var results = new System.Collections.Concurrent.ConcurrentBag<bool>();
            const int threadCount = 20;

            // Act - Multiple threads requesting different managers
            Parallel.For(0, threadCount, i =>
            {
                try
                {
                    IActuatorManager actuator = null;
                    ACAT.Core.AgentManagement.IAgentManager agent = null;
                    ACAT.Core.PanelManagement.IPanelManager panel = null;

                    switch (i % 3)
                    {
                        case 0:
                            actuator = Context.GetManager<IActuatorManager>();
                            results.Add(actuator != null);
                            break;
                        case 1:
                            agent = Context.GetManager<ACAT.Core.AgentManagement.IAgentManager>();
                            results.Add(agent != null);
                            break;
                        case 2:
                            panel = Context.GetManager<ACAT.Core.PanelManagement.IPanelManager>();
                            results.Add(panel != null);
                            break;
                    }
                }
                catch
                {
                    results.Add(false);
                }
            });

            // Assert - All threads should successfully get their managers
            Assert.AreEqual(threadCount, results.Count);
            Assert.IsTrue(results.All(r => r), "All threads should successfully resolve managers");
        }

        [TestMethod]
        public void GetLogger_ConcurrentAccess_AllThreadsGetValidLogger()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();
            Context.ServiceProvider = services.BuildServiceProvider();

            var loggers = new System.Collections.Concurrent.ConcurrentBag<Microsoft.Extensions.Logging.ILogger>();
            const int threadCount = 10;

            // Act - Multiple threads trying to get loggers simultaneously
            Parallel.For(0, threadCount, _ =>
            {
                for (int i = 0; i < 50; i++)
                {
                    var logger = LogManager.GetLogger(typeof(ContextThreadSafetyTests));
                    loggers.Add(logger);
                }
            });

            // Assert - All threads should get valid loggers
            Assert.AreEqual(threadCount * 50, loggers.Count);
            Assert.IsTrue(loggers.All(l => l != null), "All loggers should be non-null");
        }

        [TestMethod]
        [Timeout(5000)] // 5 second timeout to detect deadlocks
        public void ServiceProvider_RapidSetAndGet_NoDeadlock()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddACATServices();

            // Act & Assert - Rapid alternating set/get should not deadlock
            for (int i = 0; i < 1000; i++)
            {
                var provider = services.BuildServiceProvider();
                Context.ServiceProvider = provider;
                var manager = Context.GetManager<IActuatorManager>();
                Assert.IsNotNull(manager);
            }
        }
    }
}
