////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// RuntimeMetricsCollector.cs
//
// Collects runtime performance metrics at configurable intervals.
// Tracks CPU, memory, GC, and I/O counters without blocking the UI thread.
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace ACAT.Core.Utility.Metrics
{
    /// <summary>
    /// Snapshot of runtime performance counters captured at a single point in time.
    /// </summary>
    public class RuntimeMetricSample
    {
        /// <summary>UTC timestamp of the sample.</summary>
        public DateTime Timestamp { get; set; }

        /// <summary>Process working-set size in megabytes.</summary>
        public double WorkingSetMB { get; set; }

        /// <summary>Managed (GC) heap size in megabytes.</summary>
        public double ManagedHeapMB { get; set; }

        /// <summary>Number of GC collections since process start (all generations).</summary>
        public int GcCollectionCount { get; set; }

        /// <summary>Number of active OS thread-pool threads.</summary>
        public int ThreadCount { get; set; }

        /// <summary>Elapsed application uptime in seconds.</summary>
        public double UptimeSeconds { get; set; }
    }

    /// <summary>
    /// Defines metric categories for runtime performance data.
    /// </summary>
    public enum RuntimeMetricCategory
    {
        Memory,
        Cpu,
        Io,
        Prediction,
        Ui,
        General
    }

    /// <summary>
    /// Collects and stores named runtime performance metrics.
    /// All public members are thread-safe.
    /// </summary>
    public class RuntimeMetricsCollector : IDisposable
    {
        private readonly ConcurrentDictionary<string, RuntimeMetricEntry> _entries =
            new ConcurrentDictionary<string, RuntimeMetricEntry>(StringComparer.Ordinal);

        private readonly List<RuntimeMetricSample> _samples = new List<RuntimeMetricSample>();
        private readonly object _samplesLock = new object();
        private readonly Stopwatch _uptime = new Stopwatch();
        private Timer _sampleTimer;
        private volatile bool _disposed;

        /// <summary>
        /// Raised when a periodic sample is captured.
        /// </summary>
        public event EventHandler<RuntimeMetricSample> SampleCaptured;

        /// <summary>
        /// Starts periodic sampling at the specified interval.
        /// </summary>
        /// <param name="intervalMs">Sampling interval in milliseconds (minimum 100 ms).</param>
        public void Start(int intervalMs = 5000)
        {
            if (intervalMs < 100)
            {
                intervalMs = 100;
            }

            _uptime.Restart();
            _sampleTimer = new Timer(OnSampleTimer, null, intervalMs, intervalMs);
        }

        /// <summary>
        /// Stops periodic sampling.
        /// </summary>
        public void Stop()
        {
            _sampleTimer?.Dispose();
            _sampleTimer = null;
            _uptime.Stop();
        }

        /// <summary>
        /// Records a named metric value with an optional category.
        /// Values for the same name are aggregated (count, min, max, average).
        /// </summary>
        public void Record(string name, double value,
            RuntimeMetricCategory category = RuntimeMetricCategory.General,
            string unit = "")
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            _entries.AddOrUpdate(
                name,
                _ => new RuntimeMetricEntry
                {
                    Name = name,
                    Category = category,
                    Unit = unit,
                    Count = 1,
                    Sum = value,
                    Min = value,
                    Max = value,
                    LastValue = value,
                    LastUpdated = DateTime.UtcNow
                },
                (_, existing) =>
                {
                    existing.Count++;
                    existing.Sum += value;
                    existing.Min = Math.Min(existing.Min, value);
                    existing.Max = Math.Max(existing.Max, value);
                    existing.LastValue = value;
                    existing.LastUpdated = DateTime.UtcNow;
                    return existing;
                });
        }

        /// <summary>
        /// Returns a read-only snapshot of all recorded metric entries.
        /// </summary>
        public IReadOnlyDictionary<string, RuntimeMetricEntry> GetEntries()
        {
            return new Dictionary<string, RuntimeMetricEntry>(_entries);
        }

        /// <summary>
        /// Returns the list of periodic samples captured since <see cref="Start"/> was called.
        /// </summary>
        public IReadOnlyList<RuntimeMetricSample> GetSamples()
        {
            lock (_samplesLock)
            {
                return new List<RuntimeMetricSample>(_samples);
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            Stop();
        }

        // ----------------------------------------------------------------
        // Private helpers
        // ----------------------------------------------------------------

        private void OnSampleTimer(object state)
        {
            try
            {
                var sample = CaptureSample();
                lock (_samplesLock)
                {
                    _samples.Add(sample);
                }

                SampleCaptured?.Invoke(this, sample);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[RuntimeMetricsCollector] Sample error: {ex.Message}");
            }
        }

        private RuntimeMetricSample CaptureSample()
        {
            var process = Process.GetCurrentProcess();
            process.Refresh();

            int gcCount = 0;
            for (int gen = 0; gen <= GC.MaxGeneration; gen++)
            {
                gcCount += GC.CollectionCount(gen);
            }

            return new RuntimeMetricSample
            {
                Timestamp = DateTime.UtcNow,
                WorkingSetMB = process.WorkingSet64 / (1024.0 * 1024.0),
                ManagedHeapMB = GC.GetTotalMemory(false) / (1024.0 * 1024.0),
                GcCollectionCount = gcCount,
                ThreadCount = process.Threads.Count,
                UptimeSeconds = _uptime.Elapsed.TotalSeconds
            };
        }
    }

    /// <summary>
    /// Aggregated entry for a named runtime metric.
    /// </summary>
    public class RuntimeMetricEntry
    {
        /// <summary>Metric name.</summary>
        public string Name { get; set; }

        /// <summary>Metric category.</summary>
        public RuntimeMetricCategory Category { get; set; }

        /// <summary>Unit of the metric value (e.g. "ms", "MB").</summary>
        public string Unit { get; set; }

        /// <summary>Number of recorded samples.</summary>
        public int Count { get; set; }

        /// <summary>Sum of all recorded values (used to compute average).</summary>
        public double Sum { get; set; }

        /// <summary>Minimum recorded value.</summary>
        public double Min { get; set; }

        /// <summary>Maximum recorded value.</summary>
        public double Max { get; set; }

        /// <summary>Most recently recorded value.</summary>
        public double LastValue { get; set; }

        /// <summary>Average of all recorded values.</summary>
        public double Average => Count > 0 ? Sum / Count : 0.0;

        /// <summary>UTC timestamp of the last update.</summary>
        public DateTime LastUpdated { get; set; }
    }
}
