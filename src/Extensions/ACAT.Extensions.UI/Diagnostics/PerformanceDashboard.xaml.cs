////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// PerformanceDashboard.xaml.cs
//
// Code-behind for the WPF performance monitoring dashboard.
// Refreshes live metrics every 2 seconds and supports CSV/JSON export.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Utility.Diagnostics;
using ACAT.Core.Utility.Metrics;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Windows;

namespace ACAT.Extensions.UI.Diagnostics
{
    /// <summary>
    /// Interaction logic for PerformanceDashboard.xaml.
    /// Displays live runtime metrics, memory snapshots, and baseline
    /// regression status in an accessible WPF window.
    /// </summary>
    public partial class PerformanceDashboard : Window
    {
        private readonly RuntimeMetricsCollector _collector;
        private readonly MemoryProfiler _profiler;
        private readonly PerformanceRegressionDetector _detector;
        private Timer _refreshTimer;

        /// <summary>
        /// Initialises the dashboard with optional pre-existing components.
        /// When parameters are omitted, new instances are created.
        /// </summary>
        /// <param name="collector">Shared runtime-metrics collector (optional).</param>
        /// <param name="profiler">Shared memory profiler (optional).</param>
        /// <param name="baseline">Baseline thresholds for regression detection (optional).</param>
        public PerformanceDashboard(
            RuntimeMetricsCollector collector = null,
            MemoryProfiler profiler = null,
            PerformanceBaselineData baseline = null)
        {
            InitializeComponent();

            _collector = collector ?? new RuntimeMetricsCollector();
            _profiler = profiler ?? new MemoryProfiler();
            _detector = new PerformanceRegressionDetector(baseline);
        }

        // ----------------------------------------------------------------
        // Window events
        // ----------------------------------------------------------------

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _refreshTimer = new Timer(OnRefreshTimer, null,
                TimeSpan.Zero, TimeSpan.FromSeconds(2));
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _refreshTimer?.Dispose();
        }

        // ----------------------------------------------------------------
        // Button handlers
        // ----------------------------------------------------------------

        private void OnRefreshClick(object sender, RoutedEventArgs e)
        {
            RefreshMetrics();
        }

        private void OnClearHistoryClick(object sender, RoutedEventArgs e)
        {
            _profiler.ClearSnapshots();
            StatusBar.Text = "Sample history cleared.";
        }

        private void OnExportCsvClick(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog
            {
                Title = "Export Performance Data",
                Filter = "CSV files (*.csv)|*.csv",
                FileName = $"ACAT_Performance_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
            };

            if (dlg.ShowDialog() == true)
            {
                ExportCsv(dlg.FileName);
            }
        }

        private void OnExportJsonClick(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog
            {
                Title = "Export Performance Data",
                Filter = "JSON files (*.json)|*.json",
                FileName = $"ACAT_Performance_{DateTime.Now:yyyyMMdd_HHmmss}.json"
            };

            if (dlg.ShowDialog() == true)
            {
                ExportJson(dlg.FileName);
            }
        }

        // ----------------------------------------------------------------
        // Refresh logic
        // ----------------------------------------------------------------

        private void OnRefreshTimer(object state)
        {
            Dispatcher.InvokeAsync(RefreshMetrics);
        }

        private void RefreshMetrics()
        {
            try
            {
                MemorySnapshot snap = _profiler.CaptureSnapshot("Dashboard");
                IReadOnlyList<RuntimeMetricSample> samples = _collector.GetSamples();

                // Memory section
                WorkingSetValue.Text = $"{snap.WorkingSetMB:F1} MB";
                ManagedHeapValue.Text = $"{snap.ManagedHeapMB:F1} MB";
                int totalGc = snap.GcCollections?.Sum() ?? 0;
                GcCollectionsValue.Text = totalGc.ToString();

                // Runtime section
                UptimeValue.Text = FormatUptime(snap.Timestamp);
                ThreadCountValue.Text = snap.ThreadCount.ToString();
                HandleCountValue.Text = snap.HandleCount.ToString();

                // Sample history
                IReadOnlyList<MemorySnapshot> allSnaps = _profiler.GetSnapshots();
                SampleCount.Text = $"{allSnaps.Count} sample(s)";
                double peak = allSnaps.Count > 0 ? allSnaps.Max(s => s.WorkingSetMB) : 0;
                PeakWorkingSet.Text = $"Peak WS: {peak:F1} MB";
                LastSampleTime.Text = $"Last: {snap.Timestamp.ToLocalTime():HH:mm:ss}";

                // Regression check
                var observations = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
                {
                    ["PeakWorkingSetMB"] = peak,
                    ["ManagedHeapMB"] = snap.ManagedHeapMB
                };

                IReadOnlyList<RegressionResult> regressions = _detector.DetectRegressions(observations);
                if (regressions.Count == 0)
                {
                    RegressionStatus.Text = "✓ All metrics within baseline";
                    RegressionStatus.Foreground = System.Windows.Media.Brushes.LightGreen;
                }
                else
                {
                    RegressionStatus.Text = string.Join(Environment.NewLine,
                        regressions.Select(r => $"⚠ {r.MetricName}: {r.ObservedValue:F1} > {r.ThresholdValue:F1} {r.Unit}"));
                    RegressionStatus.Foreground = System.Windows.Media.Brushes.OrangeRed;
                }

                StatusBar.Text = $"Updated {DateTime.Now:HH:mm:ss}";
            }
            catch (Exception ex)
            {
                StatusBar.Text = $"Refresh error: {ex.Message}";
            }
        }

        // ----------------------------------------------------------------
        // Export helpers
        // ----------------------------------------------------------------

        private void ExportCsv(string filePath)
        {
            try
            {
                IReadOnlyList<MemorySnapshot> snapshots = _profiler.GetSnapshots();
                var sb = new StringBuilder();
                sb.AppendLine("Timestamp,Label,WorkingSetMB,PrivateMemoryMB,ManagedHeapMB,ThreadCount,HandleCount");

                foreach (MemorySnapshot s in snapshots)
                {
                    sb.AppendLine(string.Format("{0:o},{1},{2:F2},{3:F2},{4:F2},{5},{6}",
                        s.Timestamp, s.Label,
                        s.WorkingSetMB, s.PrivateMemoryMB, s.ManagedHeapMB,
                        s.ThreadCount, s.HandleCount));
                }

                File.WriteAllText(filePath, sb.ToString());
                StatusBar.Text = $"Exported to {filePath}";
            }
            catch (Exception ex)
            {
                StatusBar.Text = $"Export error: {ex.Message}";
            }
        }

        private void ExportJson(string filePath)
        {
            try
            {
                IReadOnlyList<MemorySnapshot> snapshots = _profiler.GetSnapshots();
                IReadOnlyDictionary<string, RuntimeMetricEntry> entries = _collector.GetEntries();

                var export = new
                {
                    ExportedAt = DateTime.UtcNow,
                    MemorySnapshots = snapshots,
                    RuntimeMetrics = entries
                };

                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(export, options);
                File.WriteAllText(filePath, json);
                StatusBar.Text = $"Exported to {filePath}";
            }
            catch (Exception ex)
            {
                StatusBar.Text = $"Export error: {ex.Message}";
            }
        }

        // ----------------------------------------------------------------
        // Private helpers
        // ----------------------------------------------------------------

        private static string FormatUptime(DateTime snapshotTime)
        {
            TimeSpan uptime = snapshotTime - System.Diagnostics.Process.GetCurrentProcess().StartTime.ToUniversalTime();
            if (uptime.TotalSeconds < 0)
            {
                return "0 s";
            }

            return uptime.TotalHours >= 1
                ? $"{uptime.Hours}h {uptime.Minutes}m"
                : $"{(int)uptime.TotalSeconds} s";
        }
    }
}
