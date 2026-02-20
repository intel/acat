////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// PerformanceRegressionDetector.cs
//
// Compares observed metric values against a persisted baseline and reports
// any values that exceed their defined thresholds.
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

namespace ACAT.Core.Utility.Diagnostics
{
    /// <summary>
    /// Describes a single detected performance regression.
    /// </summary>
    public class RegressionResult
    {
        /// <summary>Metric name that regressed.</summary>
        public string MetricName { get; set; }

        /// <summary>Observed value of the metric.</summary>
        public double ObservedValue { get; set; }

        /// <summary>Baseline threshold that was exceeded.</summary>
        public double ThresholdValue { get; set; }

        /// <summary>Percentage by which the threshold was exceeded.</summary>
        public double ExceedancePercent =>
            ThresholdValue > 0 ? ((ObservedValue - ThresholdValue) / ThresholdValue) * 100.0 : 0.0;

        /// <summary>Unit of the metric value.</summary>
        public string Unit { get; set; }

        /// <summary>Human-readable description of the regression.</summary>
        public string Description { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format(
                "REGRESSION [{0}]: observed {1:F1} {2} exceeds threshold {3:F1} {2} (+{4:F1}%)",
                MetricName, ObservedValue, Unit, ThresholdValue, ExceedancePercent);
        }
    }

    /// <summary>
    /// Compares observed metric values against a <see cref="PerformanceBaselineData"/>
    /// and identifies regressions.
    /// </summary>
    public class PerformanceRegressionDetector
    {
        private readonly PerformanceBaselineData _baseline;

        /// <summary>
        /// Initialises the detector with the supplied baseline.
        /// </summary>
        /// <param name="baseline">Baseline thresholds to compare against.
        /// If <c>null</c>, <see cref="PerformanceBaseline.CreateDefault"/> is used.</param>
        public PerformanceRegressionDetector(PerformanceBaselineData baseline = null)
        {
            _baseline = baseline ?? PerformanceBaseline.CreateDefault();
        }

        /// <summary>
        /// Checks a single observed value against its baseline threshold.
        /// </summary>
        /// <param name="metricName">Name of the metric (must match a threshold in the baseline).</param>
        /// <param name="observedValue">Measured value.</param>
        /// <param name="result">Populated when the method returns <c>true</c>.</param>
        /// <returns><c>true</c> if the observed value exceeds the threshold.</returns>
        public bool IsRegression(string metricName, double observedValue,
            out RegressionResult result)
        {
            result = null;

            if (string.IsNullOrEmpty(metricName))
            {
                return false;
            }

            if (!_baseline.Thresholds.TryGetValue(metricName, out PerformanceThreshold threshold))
            {
                return false;
            }

            if (observedValue <= threshold.MaxAcceptableValue)
            {
                return false;
            }

            result = new RegressionResult
            {
                MetricName = metricName,
                ObservedValue = observedValue,
                ThresholdValue = threshold.MaxAcceptableValue,
                Unit = threshold.Unit ?? string.Empty,
                Description = threshold.Description ?? string.Empty
            };

            return true;
        }

        /// <summary>
        /// Checks a dictionary of observed values against all matching thresholds.
        /// </summary>
        /// <param name="observations">Map of metric name → observed value.</param>
        /// <returns>All detected regressions (empty list when none).</returns>
        public IReadOnlyList<RegressionResult> DetectRegressions(
            IDictionary<string, double> observations)
        {
            var regressions = new List<RegressionResult>();

            if (observations == null)
            {
                return regressions;
            }

            foreach (KeyValuePair<string, double> kv in observations)
            {
                if (IsRegression(kv.Key, kv.Value, out RegressionResult r))
                {
                    regressions.Add(r);
                }
            }

            return regressions;
        }

        /// <summary>
        /// Produces a human-readable summary report of all regressions found in
        /// <paramref name="observations"/>.
        /// </summary>
        /// <param name="observations">Map of metric name → observed value.</param>
        /// <returns>Report string; indicates "no regressions" when the list is empty.</returns>
        public string GenerateReport(IDictionary<string, double> observations)
        {
            IReadOnlyList<RegressionResult> regressions = DetectRegressions(observations);

            var sb = new StringBuilder();
            sb.AppendLine("Performance Regression Report");
            sb.AppendLine(new string('=', 60));

            if (regressions.Count == 0)
            {
                sb.AppendLine("  No regressions detected. All metrics within baseline.");
            }
            else
            {
                sb.AppendFormat("  {0} regression(s) detected:{1}", regressions.Count, Environment.NewLine);
                sb.AppendLine();
                foreach (RegressionResult r in regressions)
                {
                    sb.AppendLine("  " + r);
                }
            }

            sb.AppendLine(new string('=', 60));
            return sb.ToString();
        }
    }
}
