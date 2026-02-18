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
        /// Initialize the performance monitor
        /// </summary>
        public static void Initialize()
        {
            _applicationLifetime.Start();
            _startWorkingSet = Process.GetCurrentProcess().WorkingSet64;
            _peakWorkingSet = _startWorkingSet;

            // Monitor memory every 5 seconds
            _memoryMonitor = new Timer(MonitorMemory, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));

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

                RecordMetric("CurrentMemoryUsage", currentWorkingSet / (1024.0 * 1024.0), "MB", MetricCategory.Memory);
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

            var process = Process.GetCurrentProcess();
            long endWorkingSet = process.WorkingSet64;

            RecordMetric("TotalApplicationLifetime", _applicationLifetime.Elapsed.TotalSeconds, "s", MetricCategory.Shutdown);
            RecordMetric("StartMemoryUsage", _startWorkingSet / (1024.0 * 1024.0), "MB", MetricCategory.Memory);
            RecordMetric("EndMemoryUsage", endWorkingSet / (1024.0 * 1024.0), "MB", MetricCategory.Memory);
            RecordMetric("PeakMemoryUsage", _peakWorkingSet / (1024.0 * 1024.0), "MB", MetricCategory.Memory);
            RecordMetric("MemoryGrowth", (endWorkingSet - _startWorkingSet) / (1024.0 * 1024.0), "MB", MetricCategory.Memory);

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
