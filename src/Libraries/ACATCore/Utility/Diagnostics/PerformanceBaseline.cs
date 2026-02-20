////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// PerformanceBaseline.cs
//
// Stores and persists named performance baseline thresholds for regression
// detection. Baselines are serialised as JSON so they can be committed to
// source control or updated by CI pipelines.
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace ACAT.Core.Utility.Diagnostics
{
    /// <summary>
    /// A single named performance threshold used for regression detection.
    /// </summary>
    public class PerformanceThreshold
    {
        /// <summary>Human-readable name of the metric (e.g. "StartupTime").</summary>
        public string Name { get; set; }

        /// <summary>Maximum acceptable value. Values above this are regressions.</summary>
        public double MaxAcceptableValue { get; set; }

        /// <summary>Unit of the metric value (e.g. "ms", "MB").</summary>
        public string Unit { get; set; }

        /// <summary>Optional description of why this threshold was chosen.</summary>
        public string Description { get; set; }
    }

    /// <summary>
    /// Container for a complete set of baseline thresholds.
    /// </summary>
    public class PerformanceBaselineData
    {
        /// <summary>Version label for this baseline snapshot.</summary>
        public string Version { get; set; } = "1.0";

        /// <summary>UTC date/time when this baseline was recorded.</summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>All defined thresholds, keyed by metric name.</summary>
        public Dictionary<string, PerformanceThreshold> Thresholds { get; set; }
            = new Dictionary<string, PerformanceThreshold>(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Loads and saves performance baselines from/to a JSON file.
    /// </summary>
    public static class PerformanceBaseline
    {
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        /// <summary>
        /// Returns a default baseline with well-known ACAT performance targets.
        /// </summary>
        public static PerformanceBaselineData CreateDefault()
        {
            var data = new PerformanceBaselineData();

            Add(data, "StartupTime",          3000, "ms",  "Application must start within 3 seconds");
            Add(data, "UiInputLag",           100,  "ms",  "UI input response must be < 100 ms");
            Add(data, "PredictionLatency",    500,  "ms",  "Word-prediction latency must be < 500 ms");
            Add(data, "PeakWorkingSetMB",     500,  "MB",  "Peak working set must be < 500 MB");
            Add(data, "ManagedHeapMB",        200,  "MB",  "Managed heap must be < 200 MB");

            return data;
        }

        /// <summary>
        /// Saves a baseline to a JSON file.
        /// </summary>
        /// <param name="data">The baseline data to save.</param>
        /// <param name="filePath">Destination file path.</param>
        public static void Save(PerformanceBaselineData data, string filePath)
        {
            if (data == null) throw new ArgumentNullException("data");
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException("filePath");

            string directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string json = JsonSerializer.Serialize(data, _jsonOptions);
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Loads a baseline from a JSON file.
        /// Returns <see cref="CreateDefault"/> when the file does not exist.
        /// </summary>
        /// <param name="filePath">Source file path.</param>
        /// <returns>Loaded or default baseline data.</returns>
        public static PerformanceBaselineData Load(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return CreateDefault();
            }

            try
            {
                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<PerformanceBaselineData>(json, _jsonOptions)
                    ?? CreateDefault();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PerformanceBaseline] Failed to load '{filePath}': {ex.Message}");
                return CreateDefault();
            }
        }

        // ----------------------------------------------------------------
        // Private helpers
        // ----------------------------------------------------------------

        private static void Add(PerformanceBaselineData data, string name,
            double max, string unit, string description)
        {
            data.Thresholds[name] = new PerformanceThreshold
            {
                Name = name,
                MaxAcceptableValue = max,
                Unit = unit,
                Description = description
            };
        }
    }
}
