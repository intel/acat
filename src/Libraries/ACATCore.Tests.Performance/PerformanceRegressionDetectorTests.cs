////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// PerformanceRegressionDetectorTests.cs
//
// Unit tests for PerformanceRegressionDetector and PerformanceBaseline.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Utility.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace ACATCore.Tests.Performance
{
    [TestClass]
    public class PerformanceRegressionDetectorTests
    {
        // ----------------------------------------------------------------
        // PerformanceBaseline tests
        // ----------------------------------------------------------------

        [TestMethod]
        public void CreateDefault_ContainsExpectedThresholds()
        {
            PerformanceBaselineData data = PerformanceBaseline.CreateDefault();

            Assert.IsNotNull(data);
            Assert.IsTrue(data.Thresholds.ContainsKey("StartupTime"));
            Assert.IsTrue(data.Thresholds.ContainsKey("UiInputLag"));
            Assert.IsTrue(data.Thresholds.ContainsKey("PredictionLatency"));
            Assert.IsTrue(data.Thresholds.ContainsKey("PeakWorkingSetMB"));
            Assert.IsTrue(data.Thresholds.ContainsKey("ManagedHeapMB"));
        }

        [TestMethod]
        public void SaveAndLoad_RoundTrip_PreservesThresholds()
        {
            string tmpFile = Path.Combine(Path.GetTempPath(), $"perf_baseline_{Guid.NewGuid()}.json");
            try
            {
                PerformanceBaselineData original = PerformanceBaseline.CreateDefault();
                PerformanceBaseline.Save(original, tmpFile);

                PerformanceBaselineData loaded = PerformanceBaseline.Load(tmpFile);

                Assert.IsNotNull(loaded);
                foreach (string key in original.Thresholds.Keys)
                {
                    Assert.IsTrue(loaded.Thresholds.ContainsKey(key),
                        $"Expected threshold '{key}' to be present after round-trip");
                    Assert.AreEqual(
                        original.Thresholds[key].MaxAcceptableValue,
                        loaded.Thresholds[key].MaxAcceptableValue,
                        0.001,
                        $"Threshold value mismatch for '{key}'");
                }
            }
            finally
            {
                File.Delete(tmpFile);
            }
        }

        [TestMethod]
        public void Load_MissingFile_ReturnsDefault()
        {
            string missingPath = Path.Combine(Path.GetTempPath(), "does_not_exist_abc123.json");

            PerformanceBaselineData result = PerformanceBaseline.Load(missingPath);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Thresholds.Count > 0);
        }

        // ----------------------------------------------------------------
        // PerformanceRegressionDetector tests
        // ----------------------------------------------------------------

        [TestMethod]
        public void IsRegression_BelowThreshold_ReturnsFalse()
        {
            var detector = new PerformanceRegressionDetector();

            bool regression = detector.IsRegression("StartupTime", 1500.0, out RegressionResult result);

            Assert.IsFalse(regression);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void IsRegression_AboveThreshold_ReturnsTrue()
        {
            var detector = new PerformanceRegressionDetector();

            bool regression = detector.IsRegression("StartupTime", 5000.0, out RegressionResult result);

            Assert.IsTrue(regression);
            Assert.IsNotNull(result);
            Assert.AreEqual("StartupTime", result.MetricName);
            Assert.AreEqual(5000.0, result.ObservedValue, 0.001);
            Assert.IsTrue(result.ExceedancePercent > 0);
        }

        [TestMethod]
        public void IsRegression_UnknownMetric_ReturnsFalse()
        {
            var detector = new PerformanceRegressionDetector();

            bool regression = detector.IsRegression("UnknownMetric", 99999.0, out RegressionResult result);

            Assert.IsFalse(regression);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void DetectRegressions_MultipleMetrics_FindsOnlyExceeded()
        {
            var detector = new PerformanceRegressionDetector();
            var observations = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                ["StartupTime"]       = 1000.0,   // under 3000ms threshold – OK
                ["UiInputLag"]        = 200.0,    // over 100ms threshold – regression
                ["PeakWorkingSetMB"]  = 600.0     // over 500MB threshold – regression
            };

            IReadOnlyList<RegressionResult> regressions = detector.DetectRegressions(observations);

            Assert.AreEqual(2, regressions.Count);
        }

        [TestMethod]
        public void DetectRegressions_NullInput_ReturnsEmptyList()
        {
            var detector = new PerformanceRegressionDetector();

            IReadOnlyList<RegressionResult> regressions = detector.DetectRegressions(null);

            Assert.IsNotNull(regressions);
            Assert.AreEqual(0, regressions.Count);
        }

        [TestMethod]
        public void GenerateReport_NoRegressions_IndicatesPass()
        {
            var detector = new PerformanceRegressionDetector();
            var observations = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                ["StartupTime"] = 1000.0,
                ["UiInputLag"]  = 50.0
            };

            string report = detector.GenerateReport(observations);

            Assert.IsTrue(report.Contains("No regressions detected"),
                "Report should indicate no regressions when all metrics pass");
        }

        [TestMethod]
        public void GenerateReport_WithRegressions_IncludesMetricName()
        {
            var detector = new PerformanceRegressionDetector();
            var observations = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                ["StartupTime"] = 9999.0   // well above threshold
            };

            string report = detector.GenerateReport(observations);

            Assert.IsTrue(report.Contains("StartupTime"),
                "Report should include regressing metric name");
        }

        [TestMethod]
        public void RegressionResult_ToString_ContainsKeyInfo()
        {
            var result = new RegressionResult
            {
                MetricName = "UiInputLag",
                ObservedValue = 250.0,
                ThresholdValue = 100.0,
                Unit = "ms"
            };

            string text = result.ToString();

            Assert.IsTrue(text.Contains("UiInputLag"));
            Assert.IsTrue(text.Contains("250"));
            Assert.IsTrue(text.Contains("100"));
        }

        [TestMethod]
        public void PerformanceBaseline_Save_CreatesDirectory()
        {
            string tmpDir = Path.Combine(Path.GetTempPath(), $"acat_test_{Guid.NewGuid()}");
            string filePath = Path.Combine(tmpDir, "baseline.json");

            try
            {
                PerformanceBaselineData data = PerformanceBaseline.CreateDefault();
                PerformanceBaseline.Save(data, filePath);

                Assert.IsTrue(File.Exists(filePath), "Baseline file should have been created");
            }
            finally
            {
                if (Directory.Exists(tmpDir))
                {
                    Directory.Delete(tmpDir, recursive: true);
                }
            }
        }

        // ----------------------------------------------------------------
        // Committed baseline file tests
        // ----------------------------------------------------------------

        [TestMethod]
        public void CommittedBaselineFile_Exists()
        {
            string baselinePath = GetCommittedBaselinePath();

            Assert.IsTrue(File.Exists(baselinePath),
                $"Committed baseline file not found at: {baselinePath}");
        }

        [TestMethod]
        public void CommittedBaselineFile_LoadsSuccessfully()
        {
            string baselinePath = GetCommittedBaselinePath();

            PerformanceBaselineData data = PerformanceBaseline.Load(baselinePath);

            Assert.IsNotNull(data, "Baseline data should not be null");
            Assert.IsTrue(data.Thresholds.Count > 0, "Baseline should contain at least one threshold");
        }

        [TestMethod]
        public void CommittedBaselineFile_ContainsAllRequiredThresholds()
        {
            string baselinePath = GetCommittedBaselinePath();
            PerformanceBaselineData data = PerformanceBaseline.Load(baselinePath);

            Assert.IsTrue(data.Thresholds.ContainsKey("StartupTime"),
                "Baseline must define StartupTime threshold (target: < 3 s)");
            Assert.IsTrue(data.Thresholds.ContainsKey("UiInputLag"),
                "Baseline must define UiInputLag threshold (target: < 100 ms)");
            Assert.IsTrue(data.Thresholds.ContainsKey("PredictionLatency"),
                "Baseline must define PredictionLatency threshold (target: < 500 ms)");
            Assert.IsTrue(data.Thresholds.ContainsKey("PeakWorkingSetMB"),
                "Baseline must define PeakWorkingSetMB threshold (target: < 500 MB)");
            Assert.IsTrue(data.Thresholds.ContainsKey("ManagedHeapMB"),
                "Baseline must define ManagedHeapMB threshold (target: < 200 MB)");
        }

        [TestMethod]
        public void CommittedBaselineFile_ThresholdValuesMatchTargets()
        {
            string baselinePath = GetCommittedBaselinePath();
            PerformanceBaselineData data = PerformanceBaseline.Load(baselinePath);

            Assert.AreEqual(3000.0, data.Thresholds["StartupTime"].MaxAcceptableValue, 0.001,
                "StartupTime threshold must be 3000 ms");
            Assert.AreEqual(100.0, data.Thresholds["UiInputLag"].MaxAcceptableValue, 0.001,
                "UiInputLag threshold must be 100 ms");
            Assert.AreEqual(500.0, data.Thresholds["PredictionLatency"].MaxAcceptableValue, 0.001,
                "PredictionLatency threshold must be 500 ms");
            Assert.AreEqual(500.0, data.Thresholds["PeakWorkingSetMB"].MaxAcceptableValue, 0.001,
                "PeakWorkingSetMB threshold must be 500 MB");
            Assert.AreEqual(200.0, data.Thresholds["ManagedHeapMB"].MaxAcceptableValue, 0.001,
                "ManagedHeapMB threshold must be 200 MB");
        }

        [TestMethod]
        public void CommittedBaselineFile_DetectorIdentifiesRegressions()
        {
            string baselinePath = GetCommittedBaselinePath();
            PerformanceBaselineData data = PerformanceBaseline.Load(baselinePath);
            var detector = new PerformanceRegressionDetector(data);

            var observations = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
            {
                ["StartupTime"]      = 5000.0,  // exceeds 3000 ms → regression
                ["UiInputLag"]       = 50.0,    // within 100 ms → OK
                ["PredictionLatency"] = 300.0,  // within 500 ms → OK
                ["PeakWorkingSetMB"] = 600.0,   // exceeds 500 MB → regression
                ["ManagedHeapMB"]    = 150.0    // within 200 MB → OK
            };

            IReadOnlyList<RegressionResult> regressions = detector.DetectRegressions(observations);

            Assert.AreEqual(2, regressions.Count,
                "Should detect exactly 2 regressions (StartupTime and PeakWorkingSetMB)");
        }

        // ----------------------------------------------------------------
        // Private helpers
        // ----------------------------------------------------------------

        private static string GetCommittedBaselinePath()
        {
            // The baseline file is copied to the output directory by the .csproj Content item.
            string assemblyDir = Path.GetDirectoryName(
                typeof(PerformanceRegressionDetectorTests).Assembly.Location)
                ?? AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(assemblyDir, "baselines", "performance-baseline.json");
        }
    }
}
