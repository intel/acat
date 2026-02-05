////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using Microsoft.VisualStudio.TestTools.UnitTesting;
using ACAT.Core.Utility;
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
        [TestMethod]
        public void HighVolumeLoggingCompletesQuickly()
        {
            Log.TraceLevelSwitch = new TraceSwitch("PerfTest", "", "Info");
            
            Stopwatch timer = Stopwatch.StartNew();
            
            for (int i = 0; i < 1000; i++)
            {
                Log.Info($"Performance message {i}");
            }
            
            timer.Stop();
            
            Assert.IsTrue(timer.ElapsedMilliseconds < 2000,
                $"1000 messages took {timer.ElapsedMilliseconds}ms");
        }

        [TestMethod]
        public void ConcurrentLoggingFromMultipleThreadsSucceeds()
        {
            Log.TraceLevelSwitch = new TraceSwitch("ConcurrentTest", "", "Info");
            
            int threadCount = 10;
            int messagesPerThread = 50;
            List<Task> tasks = new List<Task>();
            int completions = 0;
            
            for (int t = 0; t < threadCount; t++)
            {
                int threadId = t;
                Task task = Task.Run(() =>
                {
                    for (int m = 0; m < messagesPerThread; m++)
                    {
                        Log.Info($"Thread{threadId}_Msg{m}");
                    }
                    Interlocked.Increment(ref completions);
                });
                tasks.Add(task);
            }
            
            bool completed = Task.WaitAll(tasks.ToArray(), TimeSpan.FromSeconds(10));
            
            Assert.IsTrue(completed);
            Assert.AreEqual(threadCount, completions);
        }

        [TestMethod]
        public void RapidSequentialCallsComplete()
        {
            Log.TraceLevelSwitch = new TraceSwitch("RapidTest", "", "Verbose");
            
            Stopwatch timer = Stopwatch.StartNew();
            
            for (int i = 0; i < 100; i++)
            {
                Log.Debug($"Debug{i}");
                Log.Info($"Info{i}");
                Log.Warn($"Warn{i}");
                Log.Error($"Error{i}");
            }
            
            timer.Stop();
            
            Assert.IsTrue(timer.ElapsedMilliseconds < 1000);
        }

        [TestMethod]
        public void SingleLogCallReturnsQuickly()
        {
            Log.TraceLevelSwitch = new TraceSwitch("QuickTest", "", "Info");
            
            Stopwatch timer = Stopwatch.StartNew();
            Log.Info("Quick return test");
            timer.Stop();
            
            Assert.IsTrue(timer.ElapsedMilliseconds < 100);
        }

        [TestMethod]
        public void ParallelLoggingWithExceptionsHandled()
        {
            Log.TraceLevelSwitch = new TraceSwitch("ParallelExcTest", "", "Error");
            
            Parallel.For(0, 20, i =>
            {
                try
                {
                    throw new Exception($"Test exception {i}");
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                }
            });
            
            Assert.IsTrue(true);
        }
    }
}
