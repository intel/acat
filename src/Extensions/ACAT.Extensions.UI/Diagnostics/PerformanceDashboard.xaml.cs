////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// PerformanceDashboard.xaml.cs
//
// Code-behind for the WPF performance monitoring dashboard.
// Refreshes live metrics every 2 seconds, renders a working-set trend
// sparkline, and supports CSV/JSON export.
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
using System.Windows.Input;
using System.Windows.Media;

namespace ACAT.Extensions.UI.Diagnostics
{
    /// <summary>
    /// Interaction logic for PerformanceDashboard.xaml — live performance monitoring window.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The dashboard auto-refreshes every 2 seconds and displays four panels:
    /// <list type="bullet">
    ///   <item><description><b>Memory</b> – working set, managed heap, and total GC collection count.</description></item>
    ///   <item><description><b>Runtime</b> – process uptime, thread count, OS handle count.</description></item>
    ///   <item><description><b>Category Status</b> – one row per <see cref="RuntimeMetricCategory"/> with a toggle
    ///     checkbox. Green ✓ = within baseline; orange ⚠ = threshold exceeded; grey = no data.</description></item>
    ///   <item><description><b>Sample History</b> – peak working-set and timestamp of the last refresh.</description></item>
    /// </list>
    /// </para>
    /// <para><b>Minimal usage</b> (self-contained, creates its own collectors):</para>
    /// <code>
    /// var dashboard = new PerformanceDashboard();
    /// dashboard.Show();
    /// </code>
    /// <para><b>Shared collectors</b> (display data already gathered by the application):</para>
    /// <code>
    /// var collector = new RuntimeMetricsCollector();
    /// var profiler  = new MemoryProfiler();
    /// collector.Start(intervalMs: 5000);
    ///
    /// var dashboard = new PerformanceDashboard(collector, profiler);
    /// dashboard.Show();
    /// </code>
    /// <para><b>Custom baseline</b> (change regression thresholds):</para>
    /// <code>
    /// PerformanceBaselineData baseline = PerformanceBaseline.Load(baselinePath);
    /// var dashboard = new PerformanceDashboard(collector, profiler, baseline);
    /// dashboard.Show();
    /// </code>
    /// <para>
    /// Toolbar buttons let the user export all captured data to <b>CSV</b> or <b>JSON</b>
    /// via a standard SaveFileDialog, or clear the accumulated snapshot history.
    /// </para>
    /// </remarks>
    public partial class PerformanceDashboard : Window
    {
        private readonly RuntimeMetricsCollector _collector;
        private readonly MemoryProfiler _profiler;
        private readonly PerformanceRegressionDetector _detector;
        private readonly PerformanceDashboardViewModel _viewModel;
        private Timer _refreshTimer;

        /// <summary>
        /// Initializes the dashboard with optional pre-existing components.
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
            _viewModel = new PerformanceDashboardViewModel();
            DataContext = _viewModel;
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

        /// <summary>
        /// Handles keyboard shortcuts: F5 = Refresh, Ctrl+E = Export CSV, Ctrl+J = Export JSON.
        /// </summary>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                RefreshMetrics();
                e.Handled = true;
            }
            else if (e.Key == Key.E && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                OnExportCsvClick(sender, e);
                e.Handled = true;
            }
            else if (e.Key == Key.J && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                OnExportJsonClick(sender, e);
                e.Handled = true;
            }
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
            _viewModel.ClearHistory();
            UpdateSparkline();
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
                IReadOnlyDictionary<string, RuntimeMetricEntry> entries = _collector.GetEntries();

                // Memory section — prefer periodic collector entries, fall back to snapshot
                WorkingSetValue.Text = entries.TryGetValue("WorkingSetMB", out RuntimeMetricEntry wsmEntry)
                    ? $"{wsmEntry.LastValue:F1} MB"
                    : $"{snap.WorkingSetMB:F1} MB";
                ManagedHeapValue.Text = entries.TryGetValue("ManagedHeapMB", out RuntimeMetricEntry mhmEntry)
                    ? $"{mhmEntry.LastValue:F1} MB"
                    : $"{snap.ManagedHeapMB:F1} MB";
                GcCollectionsValue.Text = entries.TryGetValue("GcCollectionCount", out RuntimeMetricEntry gcEntry)
                    ? ((int)gcEntry.LastValue).ToString()
                    : (snap.GcCollections?.Sum() ?? 0).ToString();

                // Runtime section — use live entry data if available, fall back to snapshot
                UptimeValue.Text = FormatUptime(snap.Timestamp);
                ThreadCountValue.Text = entries.TryGetValue("ThreadCount", out RuntimeMetricEntry tcEntry)
                    ? ((int)tcEntry.LastValue).ToString()
                    : snap.ThreadCount.ToString();
                HandleCountValue.Text = snap.HandleCount.ToString();

                // Sample history counts and peak — used by ViewModel and regression detection
                IReadOnlyList<MemorySnapshot> allSnaps = _profiler.GetSnapshots();
                IReadOnlyList<RuntimeMetricSample> runtimeSamples = _collector.GetSamples();
                double peak = allSnaps.Count > 0 ? allSnaps.Max(s => s.WorkingSetMB) : 0;

                // Update view model with latest snapshot data (updates SampleCount, PeakWorkingSet,
                // LastSampleTime text via data binding and keeps WorkingSetHistory for sparkline)
                _viewModel.UpdateFromSnapshot(snap, entries,
                    allSnaps.Count, runtimeSamples.Count, peak);
                UpdateSparkline();

                // Build observations for regression detection
                var observations = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
                {
                    ["PeakWorkingSetMB"] = peak,
                    ["ManagedHeapMB"] = snap.ManagedHeapMB
                };
                foreach (KeyValuePair<string, RuntimeMetricEntry> kv in entries)
                {
                    if (!observations.ContainsKey(kv.Key))
                    {
                        observations[kv.Key] = kv.Value.Average;
                    }
                }

                IReadOnlyList<RegressionResult> regressions = _detector.DetectRegressions(observations);

                // Per-category status rows
                UpdateCategoryStatus(CatUiToggle,         CatUiStatus,         RuntimeMetricCategory.Ui,         entries, regressions);
                UpdateCategoryStatus(CatPredictionToggle, CatPredictionStatus, RuntimeMetricCategory.Prediction, entries, regressions);
                UpdateCategoryStatus(CatIoToggle,         CatIoStatus,         RuntimeMetricCategory.Io,         entries, regressions);
                UpdateCategoryStatus(CatCpuToggle,        CatCpuStatus,        RuntimeMetricCategory.Cpu,        entries, regressions);
                UpdateCategoryStatus(CatGeneralToggle,    CatGeneralStatus,    RuntimeMetricCategory.General,    entries, regressions);

                // Memory category uses snapshot values (PeakWorkingSetMB / ManagedHeapMB)
                UpdateMemoryCategoryStatus(snap, peak, regressions);

                StatusBar.Text = $"Updated {DateTime.Now:HH:mm:ss}";
            }
            catch (Exception ex)
            {
                StatusBar.Text = $"Refresh error: {ex.Message}";
            }
        }

        /// <summary>
        /// Renders the working-set trend sparkline on <see cref="SparklineCanvas"/>.
        /// Points are scaled to fill the canvas dimensions.
        /// </summary>
        private void UpdateSparkline()
        {
            System.Collections.ObjectModel.ObservableCollection<double> history =
                _viewModel.WorkingSetHistory;

            SparklinePolyline.Points.Clear();

            if (history.Count < 2)
            {
                SparklineMaxLabel.Text = string.Empty;
                SparklineMinLabel.Text = string.Empty;
                return;
            }

            double canvasWidth = SparklineCanvas.ActualWidth;
            double canvasHeight = SparklineCanvas.ActualHeight;

            // Guard against layout not yet performed (ActualWidth/Height = 0)
            if (canvasWidth < 1 || canvasHeight < 1)
            {
                return;
            }

            double max = _viewModel.SparklineMax;
            double min = _viewModel.SparklineMin;
            double range = Math.Max(1.0, max - min);

            int count = history.Count;
            for (int i = 0; i < count; i++)
            {
                double x = (i / (double)(count - 1)) * canvasWidth;
                double y = canvasHeight - ((history[i] - min) / range) * (canvasHeight - 4) - 2;
                SparklinePolyline.Points.Add(new System.Windows.Point(x, y));
            }

            // Update axis labels
            SparklineMaxLabel.Text = $"{max:F0} MB";
            Canvas.SetTop(SparklineMinLabel, canvasHeight - 12);
            SparklineMinLabel.Text = $"{min:F0} MB";
        }

        /// <summary>
        /// Updates a single per-category status row based on the latest runtime entries
        /// and any detected regressions. Greyed out when the toggle is unchecked.
        /// </summary>
        private void UpdateCategoryStatus(
            System.Windows.Controls.CheckBox toggle,
            System.Windows.Controls.TextBlock statusText,
            RuntimeMetricCategory category,
            IReadOnlyDictionary<string, RuntimeMetricEntry> entries,
            IReadOnlyList<RegressionResult> regressions)
        {
            if (toggle.IsChecked != true)
            {
                statusText.Text = "--";
                statusText.Foreground = System.Windows.Media.Brushes.Gray;
                return;
            }

            var categoryEntries = entries.Values
                .Where(e => e.Category == category)
                .ToList();

            if (categoryEntries.Count == 0)
            {
                statusText.Text = "No data";
                statusText.Foreground = System.Windows.Media.Brushes.Gray;
                return;
            }

            // Use a HashSet for O(1) regression lookup across category entry names
            var entryNames = new HashSet<string>(
                categoryEntries.Select(e => e.Name),
                StringComparer.OrdinalIgnoreCase);

            RegressionResult worst = regressions
                .Where(r => entryNames.Contains(r.MetricName))
                .OrderByDescending(r => r.ExceedancePercent)
                .FirstOrDefault();

            if (worst != null)
            {
                statusText.Text = $"⚠ {worst.ObservedValue:F1} {worst.Unit}";
                statusText.Foreground = System.Windows.Media.Brushes.OrangeRed;
            }
            else
            {
                RuntimeMetricEntry primary = categoryEntries
                    .OrderByDescending(e => e.LastUpdated)
                    .First();
                statusText.Text = $"✓ {primary.LastValue:F1} {primary.Unit}";
                statusText.Foreground = System.Windows.Media.Brushes.LightGreen;
            }
        }

        /// <summary>
        /// Updates the Memory category row using snapshot values rather than named entries.
        /// </summary>
        private void UpdateMemoryCategoryStatus(
            MemorySnapshot snap,
            double peakWorkingSetMB,
            IReadOnlyList<RegressionResult> regressions)
        {
            if (CatMemoryToggle.IsChecked != true)
            {
                CatMemoryStatus.Text = "--";
                CatMemoryStatus.Foreground = System.Windows.Media.Brushes.Gray;
                return;
            }

            RegressionResult worst = regressions
                .Where(r => string.Equals(r.MetricName, "PeakWorkingSetMB", StringComparison.OrdinalIgnoreCase)
                         || string.Equals(r.MetricName, "ManagedHeapMB", StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(r => r.ExceedancePercent)
                .FirstOrDefault();

            if (worst != null)
            {
                CatMemoryStatus.Text = $"⚠ {worst.ObservedValue:F1} {worst.Unit}";
                CatMemoryStatus.Foreground = System.Windows.Media.Brushes.OrangeRed;
            }
            else
            {
                CatMemoryStatus.Text = $"✓ {snap.WorkingSetMB:F1} MB";
                CatMemoryStatus.Foreground = System.Windows.Media.Brushes.LightGreen;
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
