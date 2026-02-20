////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// RuntimeMetricCategoryTests.cs
//
// Tests that verify sample metrics for every RuntimeMetricCategory are
// recorded and aggregated correctly by RuntimeMetricsCollector.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Utility.Metrics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace ACATCore.Tests.Performance
{
    [TestClass]
    public class RuntimeMetricCategoryTests
    {
        // ----------------------------------------------------------------
        // UI Responsiveness
        // ----------------------------------------------------------------

        [TestMethod]
        public void Record_UiInputLag_CategoryAndUnitPreserved()
        {
            var collector = new RuntimeMetricsCollector();

            collector.Record("UiInputLag", 45.0, RuntimeMetricCategory.Ui, "ms");

            RuntimeMetricEntry entry = collector.GetEntries()["UiInputLag"];
            Assert.AreEqual(RuntimeMetricCategory.Ui, entry.Category);
            Assert.AreEqual("ms", entry.Unit);
            Assert.AreEqual(45.0, entry.LastValue, 0.001);
        }

        [TestMethod]
        public void Record_UiInputLag_MultipleValues_AggregatesCorrectly()
        {
            var collector = new RuntimeMetricsCollector();

            collector.Record("UiInputLag", 20.0, RuntimeMetricCategory.Ui, "ms");
            collector.Record("UiInputLag", 40.0, RuntimeMetricCategory.Ui, "ms");
            collector.Record("UiInputLag", 60.0, RuntimeMetricCategory.Ui, "ms");

            RuntimeMetricEntry entry = collector.GetEntries()["UiInputLag"];
            Assert.AreEqual(3, entry.Count);
            Assert.AreEqual(20.0, entry.Min, 0.001);
            Assert.AreEqual(60.0, entry.Max, 0.001);
            Assert.AreEqual(40.0, entry.Average, 0.001);
        }

        // ----------------------------------------------------------------
        // Prediction Performance
        // ----------------------------------------------------------------

        [TestMethod]
        public void Record_PredictionLatency_CategoryPreserved()
        {
            var collector = new RuntimeMetricsCollector();

            collector.Record("PredictionLatency", 120.0, RuntimeMetricCategory.Prediction, "ms");

            RuntimeMetricEntry entry = collector.GetEntries()["PredictionLatency"];
            Assert.AreEqual(RuntimeMetricCategory.Prediction, entry.Category);
            Assert.AreEqual(120.0, entry.LastValue, 0.001);
        }

        [TestMethod]
        public void Record_PredictionLatency_MultipleValues_AggregatesCorrectly()
        {
            var collector = new RuntimeMetricsCollector();

            collector.Record("PredictionLatency", 100.0, RuntimeMetricCategory.Prediction, "ms");
            collector.Record("PredictionLatency", 200.0, RuntimeMetricCategory.Prediction, "ms");

            RuntimeMetricEntry entry = collector.GetEntries()["PredictionLatency"];
            Assert.AreEqual(2, entry.Count);
            Assert.AreEqual(150.0, entry.Average, 0.001);
        }

        // ----------------------------------------------------------------
        // I/O Operations
        // ----------------------------------------------------------------

        [TestMethod]
        public void Record_IoOperation_CategoryPreserved()
        {
            var collector = new RuntimeMetricsCollector();

            collector.Record("FileRead", 5.0, RuntimeMetricCategory.Io, "ms");

            RuntimeMetricEntry entry = collector.GetEntries()["FileRead"];
            Assert.AreEqual(RuntimeMetricCategory.Io, entry.Category);
            Assert.AreEqual("ms", entry.Unit);
        }

        [TestMethod]
        public void Record_MultipleIoOperations_StoredSeparately()
        {
            var collector = new RuntimeMetricsCollector();

            collector.Record("FileRead", 5.0, RuntimeMetricCategory.Io, "ms");
            collector.Record("NetworkRequest", 150.0, RuntimeMetricCategory.Io, "ms");

            IReadOnlyDictionary<string, RuntimeMetricEntry> entries = collector.GetEntries();
            Assert.IsTrue(entries.ContainsKey("FileRead"));
            Assert.IsTrue(entries.ContainsKey("NetworkRequest"));
            Assert.AreEqual(RuntimeMetricCategory.Io, entries["FileRead"].Category);
            Assert.AreEqual(RuntimeMetricCategory.Io, entries["NetworkRequest"].Category);
        }

        // ----------------------------------------------------------------
        // Memory Usage
        // ----------------------------------------------------------------

        [TestMethod]
        public void Record_MemoryMetric_CategoryPreserved()
        {
            var collector = new RuntimeMetricsCollector();

            collector.Record("WorkingSetMB", 256.0, RuntimeMetricCategory.Memory, "MB");

            RuntimeMetricEntry entry = collector.GetEntries()["WorkingSetMB"];
            Assert.AreEqual(RuntimeMetricCategory.Memory, entry.Category);
            Assert.AreEqual("MB", entry.Unit);
            Assert.AreEqual(256.0, entry.LastValue, 0.001);
        }

        // ----------------------------------------------------------------
        // CPU
        // ----------------------------------------------------------------

        [TestMethod]
        public void Record_CpuMetric_CategoryPreserved()
        {
            var collector = new RuntimeMetricsCollector();

            collector.Record("CpuUsagePercent", 12.5, RuntimeMetricCategory.Cpu, "%");

            RuntimeMetricEntry entry = collector.GetEntries()["CpuUsagePercent"];
            Assert.AreEqual(RuntimeMetricCategory.Cpu, entry.Category);
            Assert.AreEqual("%", entry.Unit);
        }

        // ----------------------------------------------------------------
        // Mixed categories
        // ----------------------------------------------------------------

        [TestMethod]
        public void Record_AllCategories_AllStoredIndependently()
        {
            var collector = new RuntimeMetricsCollector();

            collector.Record("UiInputLag",        45.0,  RuntimeMetricCategory.Ui,         "ms");
            collector.Record("PredictionLatency", 120.0, RuntimeMetricCategory.Prediction,  "ms");
            collector.Record("FileRead",           5.0,  RuntimeMetricCategory.Io,          "ms");
            collector.Record("WorkingSetMB",     256.0,  RuntimeMetricCategory.Memory,      "MB");
            collector.Record("CpuUsagePercent",   12.5,  RuntimeMetricCategory.Cpu,         "%");
            collector.Record("AppInit",          200.0,  RuntimeMetricCategory.General,     "ms");

            IReadOnlyDictionary<string, RuntimeMetricEntry> entries = collector.GetEntries();
            Assert.AreEqual(6, entries.Count);
            Assert.AreEqual(RuntimeMetricCategory.Ui,         entries["UiInputLag"].Category);
            Assert.AreEqual(RuntimeMetricCategory.Prediction, entries["PredictionLatency"].Category);
            Assert.AreEqual(RuntimeMetricCategory.Io,         entries["FileRead"].Category);
            Assert.AreEqual(RuntimeMetricCategory.Memory,     entries["WorkingSetMB"].Category);
            Assert.AreEqual(RuntimeMetricCategory.Cpu,        entries["CpuUsagePercent"].Category);
            Assert.AreEqual(RuntimeMetricCategory.General,    entries["AppInit"].Category);
        }
    }
}
