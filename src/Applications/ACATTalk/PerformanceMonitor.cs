////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// PerformanceMonitor.cs
//
// Performance monitoring and baseline metrics collection for ACATTalk
//
////////////////////////////////////////////////////////////////////////////
#define PERFORMANCE

#if PERFORMANCE

using ACAT.Core.Utility.Diagnostics;
using ACAT.Core.Utility.Metrics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace ACATTalk
{
    /// <summary>
    /// Provides performance monitoring and baseline metrics collection
    /// for ACATTalk application. Only compiled when PERFORMANCE symbol is defined.
    /// Integrates <see cref="RuntimeMetricsCollector"/>, <see cref="MemoryProfiler"/>,
    /// and <see cref="PerformanceRegressionDetector"/> from the ACATCore library.
    /// </summary>
    public static class PerformanceMonitor
    {
        private static readonly ConcurrentDictionary<string, PerformanceMetric> _metrics = new ConcurrentDictionary<string, PerformanceMetric>();
        private static readonly ConcurrentDictionary<string, Stopwatch> _activeTimers = new ConcurrentDictionary<string, Stopwatch>();
        private static readonly Stopwatch _applicationLifetime = new Stopwatch();
        private static long _peakWorkingSet = 0;
        private static long _startWorkingSet = 0;
        private static Timer _memoryMonitor;
        private static readonly object _reportLock = new object();

        // ---- ACATCore performance infrastructure ----
        private static readonly RuntimeMetricsCollector _runtimeCollector = new RuntimeMetricsCollector();
        private static readonly MemoryProfiler _memoryProfiler = new MemoryProfiler();
        private static PerformanceRegressionDetector _regressionDetector;

        // Track which RuntimeMetricCategories are enabled for recording (all on by default)
        private static readonly ConcurrentDictionary<RuntimeMetricCategory, bool> _enabledCategories =
            new ConcurrentDictionary<RuntimeMetricCategory, bool>(
                Enum.GetValues(typeof(RuntimeMetricCategory))
                    .Cast<RuntimeMetricCategory>()
                    .Select(c => new KeyValuePair<RuntimeMetricCategory, bool>(c, true)));

        // Shared metric name constants to avoid silent drift between recording sites
        private const string MetricNameWorkingSetMB = "WorkingSetMB";

        /// <summary>
        /// Metric categories for organizing performance data
        /// </summary>
        public enum MetricCategory
        {
            Startup,
            Initialization,
            UI,
            Interaction,
            TextPrediction,
            TTS,
            Memory,
            Shutdown
        }

        /// <summary>
        /// Returns <c>true</c> if recording for the specified category is currently enabled.
        /// </summary>
        public static bool IsCategoryEnabled(RuntimeMetricCategory category)
            => _enabledCategories.GetOrAdd(category, true);

        /// <summary>
        /// Enables metric recording for the specified category.
        /// </summary>
        public static void EnableCategory(RuntimeMetricCategory category)
            => _enabledCategories[category] = true;

        /// <summary>
        /// Disables metric recording for the specified category.
        /// Calls to category-specific record methods become no-ops until re-enabled.
        /// </summary>
        public static void DisableCategory(RuntimeMetricCategory category)
            => _enabledCategories[category] = false;

        /// <summary>
        /// Initialize the performance monitor
        /// </summary>
        public static void Initialize()
        {
            _applicationLifetime.Start();
            _startWorkingSet = Process.GetCurrentProcess().WorkingSet64;
            _peakWorkingSet = _startWorkingSet;

            // Monitor memory every 5 seconds
            _memoryMonitor = new Timer(MonitorMemory, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));

            // Start the ACATCore runtime metrics collector (5-second interval)
            _runtimeCollector.Start(5000);

            // Capture startup memory snapshot
            _memoryProfiler.CaptureSnapshot("Startup");

            // Load baseline (if present) or use defaults
            string baselinePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "ACAT", "performance_baseline.json");
            PerformanceBaselineData baseline = PerformanceBaseline.Load(baselinePath);
            _regressionDetector = new PerformanceRegressionDetector(baseline);

            LogEvent("PerformanceMonitor", "Performance monitoring initialized");
        }

        /// <summary>
        /// Start timing an operation
        /// </summary>
        public static void StartTimer(string operationName)
        {
            var sw = Stopwatch.StartNew();
            _activeTimers.AddOrUpdate(operationName, sw, (key, existing) =>
            {
                existing.Restart();
                return existing;
            });
        }

        /// <summary>
        /// Stop timing an operation and record the metric
        /// </summary>
        public static void StopTimer(string operationName, MetricCategory category = MetricCategory.Startup)
        {
            if (_activeTimers.TryRemove(operationName, out Stopwatch sw))
            {
                sw.Stop();
                RecordMetric(operationName, sw.Elapsed.TotalMilliseconds, "ms", category);
            }
        }

        /// <summary>
        /// Record a performance metric
        /// </summary>
        public static void RecordMetric(string name, double value, string unit, MetricCategory category)
        {
            _metrics.AddOrUpdate(name, 
                new PerformanceMetric 
                { 
                    Name = name, 
                    Value = value, 
                    Unit = unit, 
                    Category = category,
                    Timestamp = DateTime.Now,
                    Count = 1,
                    Min = value,
                    Max = value,
                    Sum = value
                },
                (key, existing) =>
                {
                    existing.Count++;
                    existing.Sum += value;
                    existing.Value = existing.Sum / existing.Count; // Average
                    existing.Min = Math.Min(existing.Min, value);
                    existing.Max = Math.Max(existing.Max, value);
                    existing.Timestamp = DateTime.Now;
                    return existing;
                });
        }

        /// <summary>
        /// Record a UI input lag measurement and forward to the runtime collector.
        /// </summary>
        /// <param name="milliseconds">Measured input lag in milliseconds.</param>
        public static void RecordUiInputLag(double milliseconds)
        {
            if (!IsCategoryEnabled(RuntimeMetricCategory.Ui)) return;
            RecordMetric("UiInputLag", milliseconds, "ms", MetricCategory.UI);
            _runtimeCollector.Record("UiInputLag", milliseconds, RuntimeMetricCategory.Ui, "ms");
        }

        /// <summary>
        /// Record a word-prediction latency measurement and forward to the runtime collector.
        /// </summary>
        /// <param name="milliseconds">Measured latency in milliseconds.</param>
        public static void RecordPredictionLatency(double milliseconds)
        {
            if (!IsCategoryEnabled(RuntimeMetricCategory.Prediction)) return;
            RecordMetric("PredictionLatency", milliseconds, "ms", MetricCategory.TextPrediction);
            _runtimeCollector.Record("PredictionLatency", milliseconds, RuntimeMetricCategory.Prediction, "ms");
        }

        /// <summary>
        /// Record an autocomplete operation latency and forward to the runtime collector.
        /// Covers word, letter, and sentence autocomplete insertions.
        /// </summary>
        /// <param name="milliseconds">Duration of the autocomplete operation in milliseconds.</param>
        public static void RecordAutoCompleteLatency(double milliseconds)
        {
            if (!IsCategoryEnabled(RuntimeMetricCategory.Prediction)) return;
            RecordMetric("AutoCompleteInsert", milliseconds, "ms", MetricCategory.TextPrediction);
            _runtimeCollector.Record("AutoCompleteInsert", milliseconds, RuntimeMetricCategory.Prediction, "ms");
        }

        /// <summary>
        /// Record a word-prediction refresh latency and forward to the runtime collector.
        /// </summary>
        /// <param name="milliseconds">Duration of the prediction refresh in milliseconds.</param>
        public static void RecordPredictionRefresh(double milliseconds)
        {
            if (!IsCategoryEnabled(RuntimeMetricCategory.Prediction)) return;
            RecordMetric("PredictionRefresh", milliseconds, "ms", MetricCategory.TextPrediction);
            _runtimeCollector.Record("PredictionRefresh", milliseconds, RuntimeMetricCategory.Prediction, "ms");
        }

        /// <summary>
        /// Record a key actuation latency and forward to the runtime collector.
        /// </summary>
        /// <param name="keyType">"SingleKey" or "MultiChar".</param>
        /// <param name="milliseconds">Elapsed time of the actuation in milliseconds.</param>
        public static void RecordKeyActuationLatency(string keyType, double milliseconds)
        {
            if (!IsCategoryEnabled(RuntimeMetricCategory.Ui)) return;
            RecordMetric($"KeyActuation_{keyType}", milliseconds, "ms", MetricCategory.Interaction);
            _runtimeCollector.Record($"KeyActuation_{keyType}", milliseconds, RuntimeMetricCategory.Ui, "ms");
        }

        /// <summary>
        /// Record a per-phase autocomplete latency and forward to the runtime collector.
        /// Phase names: "GetPrevWord", "CheckInsertReplace", "Insert", "Replace", "PostCompletion".
        /// </summary>
        /// <param name="phase">Phase name.</param>
        /// <param name="milliseconds">Elapsed time of the phase in milliseconds.</param>
        public static void RecordAutoCompletePhaseLatency(string phase, double milliseconds)
        {
            if (!IsCategoryEnabled(RuntimeMetricCategory.Prediction)) return;
            RecordMetric($"AutoComplete_{phase}", milliseconds, "ms", MetricCategory.TextPrediction);
            _runtimeCollector.Record($"AutoComplete_{phase}", milliseconds, RuntimeMetricCategory.Prediction, "ms");
        }

        /// <summary>
        /// Record a text-change event latency and forward to the runtime collector.
        /// </summary>
        /// <param name="milliseconds">Elapsed time of the text-change event handler in milliseconds.</param>
        public static void RecordTextChangeEventLatency(double milliseconds)
        {
            if (!IsCategoryEnabled(RuntimeMetricCategory.Ui)) return;
            RecordMetric("TextChangeEvent", milliseconds, "ms", MetricCategory.Interaction);
            _runtimeCollector.Record("TextChangeEvent", milliseconds, RuntimeMetricCategory.Ui, "ms");
        }

        /// <summary>
        /// Record an I/O operation duration and forward to the runtime collector.
        /// </summary>
        /// <param name="operationName">Name of the I/O operation (e.g. "FileRead", "NetworkRequest").</param>
        /// <param name="milliseconds">Duration of the operation in milliseconds.</param>
        public static void RecordIoOperation(string operationName, double milliseconds)
        {
            if (!IsCategoryEnabled(RuntimeMetricCategory.Io)) return;
            RecordMetric(operationName, milliseconds, "ms", MetricCategory.Interaction);
            _runtimeCollector.Record(operationName, milliseconds, RuntimeMetricCategory.Io, "ms");
        }

        /// <summary>
        /// Record a memory working-set measurement and forward to the runtime collector.
        /// </summary>
        /// <param name="workingSetMB">Current process working-set size in megabytes.</param>
        public static void RecordMemoryUsage(double workingSetMB)
        {
            if (!IsCategoryEnabled(RuntimeMetricCategory.Memory)) return;
            RecordMetric(MetricNameWorkingSetMB, workingSetMB, "MB", MetricCategory.Memory);
            _runtimeCollector.Record(MetricNameWorkingSetMB, workingSetMB, RuntimeMetricCategory.Memory, "MB");
        }

        /// <summary>
        /// Log a performance event
        /// </summary>
        public static void LogEvent(string operation, string details = "")
        {
            var elapsed = _applicationLifetime.Elapsed.TotalSeconds;
            var message = $"[{elapsed:F3}s] {operation}";
            if (!string.IsNullOrEmpty(details))
            {
                message += $": {details}";
            }
            Debug.WriteLine(message);
        }

        /// <summary>
        /// Monitor memory usage
        /// </summary>
        private static void MonitorMemory(object state)
        {
            try
            {
                var process = Process.GetCurrentProcess();
                long currentWorkingSet = process.WorkingSet64;
                
                if (currentWorkingSet > _peakWorkingSet)
                {
                    _peakWorkingSet = currentWorkingSet;
                }

                double workingSetMB = currentWorkingSet / (1024.0 * 1024.0);
                RecordMetric("CurrentMemoryUsage", workingSetMB, "MB", MetricCategory.Memory);

                if (IsCategoryEnabled(RuntimeMetricCategory.Memory))
                {
                    _runtimeCollector.Record(MetricNameWorkingSetMB, workingSetMB, RuntimeMetricCategory.Memory, "MB");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Memory monitoring error: {ex.Message}");
            }
        }

        /// <summary>
        /// Shutdown the performance monitor and generate report
        /// </summary>
        public static void Shutdown()
        {
            _applicationLifetime.Stop();
            _memoryMonitor?.Dispose();
            _runtimeCollector.Stop();

            var process = Process.GetCurrentProcess();
            long endWorkingSet = process.WorkingSet64;

            RecordMetric("TotalApplicationLifetime", _applicationLifetime.Elapsed.TotalSeconds, "s", MetricCategory.Shutdown);
            if (IsCategoryEnabled(RuntimeMetricCategory.General))
            {
                _runtimeCollector.Record("StartupTime",
                    _applicationLifetime.Elapsed.TotalMilliseconds, RuntimeMetricCategory.General, "ms");
            }

            RecordMetric("StartMemoryUsage", _startWorkingSet / (1024.0 * 1024.0), "MB", MetricCategory.Memory);
            RecordMetric("EndMemoryUsage", endWorkingSet / (1024.0 * 1024.0), "MB", MetricCategory.Memory);
            RecordMetric("PeakMemoryUsage", _peakWorkingSet / (1024.0 * 1024.0), "MB", MetricCategory.Memory);
            RecordMetric("MemoryGrowth", (endWorkingSet - _startWorkingSet) / (1024.0 * 1024.0), "MB", MetricCategory.Memory);

            // Capture shutdown memory snapshot and check for regressions
            MemorySnapshot shutdownSnap = _memoryProfiler.CaptureSnapshot("Shutdown");
            if (_regressionDetector != null)
            {
                var observations = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
                {
                    ["TotalApplicationLifetime"] = _applicationLifetime.Elapsed.TotalSeconds * 1000,
                    ["PeakWorkingSetMB"] = _peakWorkingSet / (1024.0 * 1024.0),
                    ["ManagedHeapMB"] = shutdownSnap.ManagedHeapMB
                };

                // Include runtime-collector aggregates for regression checking
                IReadOnlyDictionary<string, RuntimeMetricEntry> runtimeEntries = _runtimeCollector.GetEntries();
                foreach (KeyValuePair<string, RuntimeMetricEntry> kv in runtimeEntries)
                {
                    if (!observations.ContainsKey(kv.Key))
                    {
                        observations[kv.Key] = kv.Value.Average;
                    }
                }

                IReadOnlyList<RegressionResult> regressions = _regressionDetector.DetectRegressions(observations);
                foreach (RegressionResult r in regressions)
                {
                    Debug.WriteLine($"[PerformanceMonitor] {r}");
                }
            }

            GenerateReport();
        }

        /// <summary>
        /// Generate performance report
        /// </summary>
        private static void GenerateReport()
        {
            lock (_reportLock)
            {
                try
                {
                    string userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                    string reportsDir = Path.Combine(userProfilePath, "ACATTalk_PerformanceReports");
                    Directory.CreateDirectory(reportsDir);

                    string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    string reportPath = Path.Combine(reportsDir, $"ACATTalk_Performance_{timestamp}.txt");
                    string csvPath = Path.Combine(reportsDir, $"ACATTalk_Performance_{timestamp}.csv");

                    GenerateTextReport(reportPath);
                    GenerateCsvReport(csvPath);

                    Debug.WriteLine($"Performance reports generated:");
                    Debug.WriteLine($"  Text: {reportPath}");
                    Debug.WriteLine($"  CSV:  {csvPath}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error generating performance report: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Generate human-readable text report
        /// </summary>
        private static void GenerateTextReport(string path)
        {
            var sb = new StringBuilder();
            sb.AppendLine("=".PadRight(80, '='));
            sb.AppendLine("ACATTalk Performance Baseline Report");
            sb.AppendLine($"Generated: {DateTime.Now}");
            sb.AppendLine("=".PadRight(80, '='));
            sb.AppendLine();

            IOrderedEnumerable<IGrouping<MetricCategory, PerformanceMetric>> categorized = _metrics.Values
                .GroupBy(m => m.Category)
                .OrderBy(g => g.Key);

            foreach (IGrouping<MetricCategory, PerformanceMetric> category in categorized)
            {
                sb.AppendLine($"[{category.Key}]");
                sb.AppendLine("-".PadRight(80, '-'));

                IOrderedEnumerable<PerformanceMetric> sorted = category.OrderBy(m => m.Name);
                foreach (PerformanceMetric metric in sorted)
                {
                    sb.AppendLine($"  {metric.Name,-40} {metric.Value,10:F2} {metric.Unit}");
                    if (metric.Count > 1)
                    {
                        sb.AppendLine($"    {"Count:",-38} {metric.Count,10}");
                        sb.AppendLine($"    {"Min:",-38} {metric.Min,10:F2} {metric.Unit}");
                        sb.AppendLine($"    {"Max:",-38} {metric.Max,10:F2} {metric.Unit}");
                        sb.AppendLine($"    {"Avg:",-38} {metric.Value,10:F2} {metric.Unit}");
                    }
                }
                sb.AppendLine();
            }

            // Include runtime-collector entries (UI, Prediction, I/O, etc.)
            IReadOnlyDictionary<string, RuntimeMetricEntry> runtimeEntries = _runtimeCollector.GetEntries();
            if (runtimeEntries.Count > 0)
            {
                sb.AppendLine("[Runtime Metrics]");
                sb.AppendLine("-".PadRight(80, '-'));

                IOrderedEnumerable<RuntimeMetricEntry> runtimeSorted = runtimeEntries.Values
                    .OrderBy(e => e.Category)
                    .ThenBy(e => e.Name);
                foreach (RuntimeMetricEntry entry in runtimeSorted)
                {
                    sb.AppendLine($"  {entry.Name,-40} {entry.LastValue,10:F2} {entry.Unit} [{entry.Category}]");
                    if (entry.Count > 1)
                    {
                        sb.AppendLine($"    {"Count:",-38} {entry.Count,10}");
                        sb.AppendLine($"    {"Min:",-38} {entry.Min,10:F2} {entry.Unit}");
                        sb.AppendLine($"    {"Max:",-38} {entry.Max,10:F2} {entry.Unit}");
                        sb.AppendLine($"    {"Avg:",-38} {entry.Average,10:F2} {entry.Unit}");
                    }
                }
                sb.AppendLine();
            }

            sb.AppendLine("=".PadRight(80, '='));
            sb.AppendLine("End of Report");
            sb.AppendLine("=".PadRight(80, '='));

            File.WriteAllText(path, sb.ToString());
        }

        /// <summary>
        /// Generate CSV report for data analysis
        /// </summary>
        private static void GenerateCsvReport(string path)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Category,Metric,Value,Unit,Count,Min,Max,Timestamp");

            IOrderedEnumerable<PerformanceMetric> sorted = _metrics.Values.OrderBy(m => m.Category).ThenBy(m => m.Name);
            foreach (PerformanceMetric metric in sorted)
            {
                sb.AppendLine($"{metric.Category},{metric.Name},{metric.Value:F2},{metric.Unit},{metric.Count},{metric.Min:F2},{metric.Max:F2},{metric.Timestamp:o}");
            }

            // Append runtime-collector entries
            IReadOnlyDictionary<string, RuntimeMetricEntry> runtimeEntries = _runtimeCollector.GetEntries();
            foreach (RuntimeMetricEntry entry in runtimeEntries.Values.OrderBy(e => e.Category).ThenBy(e => e.Name))
            {
                sb.AppendLine($"Runtime.{entry.Category},{entry.Name},{entry.Average:F2},{entry.Unit},{entry.Count},{entry.Min:F2},{entry.Max:F2},{entry.LastUpdated:o}");
            }

            File.WriteAllText(path, sb.ToString());
        }

        /// <summary>
        /// Get current metrics snapshot (for debugging)
        /// </summary>
        public static Dictionary<string, PerformanceMetric> GetMetrics()
        {
            return new Dictionary<string, PerformanceMetric>(_metrics);
        }

        /// <summary>
        /// Get current runtime-collector entries snapshot (for debugging).
        /// </summary>
        public static IReadOnlyDictionary<string, RuntimeMetricEntry> GetRuntimeMetrics()
        {
            return _runtimeCollector.GetEntries();
        }

        /// <summary>
        /// Returns the shared <see cref="RuntimeMetricsCollector"/> instance so that
        /// external components (e.g. the debug dashboard) can observe the same data
        /// that <see cref="PerformanceMonitor"/> records to.
        /// </summary>
        public static RuntimeMetricsCollector GetRuntimeCollector()
        {
            return _runtimeCollector;
        }

        /// <summary>
        /// Returns the shared <see cref="MemoryProfiler"/> instance so that
        /// external components (e.g. the debug dashboard) can observe the same
        /// snapshot history that <see cref="PerformanceMonitor"/> captures.
        /// </summary>
        public static MemoryProfiler GetMemoryProfiler()
        {
            return _memoryProfiler;
        }

        /// <summary>
        /// Performance metric data structure
        /// </summary>
        public class PerformanceMetric
        {
            public string Name { get; set; }
            public double Value { get; set; }
            public string Unit { get; set; }
            public MetricCategory Category { get; set; }
            public DateTime Timestamp { get; set; }
            public int Count { get; set; }
            public double Min { get; set; }
            public double Max { get; set; }
            public double Sum { get; set; }
        }
    }
}

#endif
