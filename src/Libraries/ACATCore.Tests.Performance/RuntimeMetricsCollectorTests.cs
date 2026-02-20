////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// RuntimeMetricsCollectorTests.cs
//
// Unit tests for RuntimeMetricsCollector.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Utility.Metrics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ACATCore.Tests.Performance
{
    [TestClass]
    public class RuntimeMetricsCollectorTests
    {
        [TestMethod]
        public void Record_SingleValue_StoresEntry()
        {
            var collector = new RuntimeMetricsCollector();

            collector.Record("TestMetric", 42.0, RuntimeMetricCategory.General, "ms");

            IReadOnlyDictionary<string, RuntimeMetricEntry> entries = collector.GetEntries();
            Assert.IsTrue(entries.ContainsKey("TestMetric"));
            Assert.AreEqual(42.0, entries["TestMetric"].LastValue, 0.001);
            Assert.AreEqual(1, entries["TestMetric"].Count);
        }

        [TestMethod]
        public void Record_MultipleValues_AggregatesCorrectly()
        {
            var collector = new RuntimeMetricsCollector();

            collector.Record("Latency", 10.0, RuntimeMetricCategory.Ui, "ms");
            collector.Record("Latency", 20.0, RuntimeMetricCategory.Ui, "ms");
            collector.Record("Latency", 30.0, RuntimeMetricCategory.Ui, "ms");

            IReadOnlyDictionary<string, RuntimeMetricEntry> entries = collector.GetEntries();
            RuntimeMetricEntry entry = entries["Latency"];

            Assert.AreEqual(3, entry.Count);
            Assert.AreEqual(10.0, entry.Min, 0.001);
            Assert.AreEqual(30.0, entry.Max, 0.001);
            Assert.AreEqual(20.0, entry.Average, 0.001);
        }

        [TestMethod]
        public void Record_NullOrEmptyName_DoesNotThrow()
        {
            var collector = new RuntimeMetricsCollector();

            collector.Record(null, 1.0);
            collector.Record(string.Empty, 1.0);

            Assert.AreEqual(0, collector.GetEntries().Count);
        }

        [TestMethod]
        public void Start_CollectsSamplesOverTime()
        {
            var collector = new RuntimeMetricsCollector();
            var samplesReceived = new List<RuntimeMetricSample>();
            collector.SampleCaptured += (s, e) => samplesReceived.Add(e);

            collector.Start(intervalMs: 200);
            Thread.Sleep(700);
            collector.Stop();

            // Expect at least 2 samples in ~700 ms with a 200 ms interval
            Assert.IsTrue(samplesReceived.Count >= 2,
                $"Expected >= 2 samples, got {samplesReceived.Count}");
        }

        [TestMethod]
        public void Start_SamplesHavePositiveWorkingSet()
        {
            var collector = new RuntimeMetricsCollector();
            collector.Start(intervalMs: 100);
            Thread.Sleep(250);
            collector.Stop();

            IReadOnlyList<RuntimeMetricSample> samples = collector.GetSamples();
            Assert.IsTrue(samples.Count > 0, "Should have captured at least one sample");

            foreach (RuntimeMetricSample sample in samples)
            {
                Assert.IsTrue(sample.WorkingSetMB > 0, "Working set must be positive");
                Assert.IsTrue(sample.ManagedHeapMB >= 0, "Managed heap must be non-negative");
                Assert.IsTrue(sample.ThreadCount > 0, "Thread count must be positive");
            }
        }

        [TestMethod]
        public void Dispose_StopsSampling()
        {
            var collector = new RuntimeMetricsCollector();
            collector.Start(intervalMs: 100);
            Thread.Sleep(150);
            collector.Dispose();

            int countAfterDispose = collector.GetSamples().Count;
            Thread.Sleep(300);
            int countAfterWait = collector.GetSamples().Count;

            Assert.AreEqual(countAfterDispose, countAfterWait,
                "No new samples should be added after Dispose");
        }

        [TestMethod]
        public void Record_CategoryAndUnitPreserved()
        {
            var collector = new RuntimeMetricsCollector();

            collector.Record("PredLatency", 123.4, RuntimeMetricCategory.Prediction, "ms");

            RuntimeMetricEntry entry = collector.GetEntries()["PredLatency"];
            Assert.AreEqual(RuntimeMetricCategory.Prediction, entry.Category);
            Assert.AreEqual("ms", entry.Unit);
        }
    }
}
