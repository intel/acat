////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// PerformanceDashboardViewModel.cs
//
// MVVM view model for the PerformanceDashboard WPF window.
// Exposes observable metric properties and sparkline history data
// that XAML binds to for display.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Utility.Diagnostics;
using ACAT.Core.Utility.Metrics;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ACAT.Extensions.UI.Diagnostics
{
    /// <summary>
    /// MVVM view model for <see cref="PerformanceDashboard"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The view model owns:
    /// <list type="bullet">
    ///   <item><description>Observable metric text properties bound to the XAML panels.</description></item>
    ///   <item><description><see cref="WorkingSetHistory"/> — recent working-set samples
    ///     used to render the trend sparkline.</description></item>
    ///   <item><description><see cref="SparklineMax"/> and <see cref="SparklineMin"/> —
    ///     range values the code-behind uses to scale the sparkline canvas.</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// The code-behind calls <see cref="UpdateFromSnapshot"/> and
    /// <see cref="UpdateFromEntries"/> after each metric refresh to keep
    /// the view model in sync.
    /// </para>
    /// </remarks>
    public class PerformanceDashboardViewModel : INotifyPropertyChanged
    {
        // Maximum number of working-set samples retained in the sparkline history.
        private const int MaxSparklinePoints = 60;

        private string _workingSetText = "-- MB";
        private string _managedHeapText = "-- MB";
        private string _gcCollectionsText = "--";
        private string _uptimeText = "-- s";
        private string _threadCountText = "--";
        private string _handleCountText = "--";
        private string _sampleCountText = "0 samples";
        private string _peakWorkingSetText = "Peak WS: -- MB";
        private string _lastSampleTimeText = "Last: --";
        private string _statusText = "Ready";
        private double _sparklineMax = 1.0;
        private double _sparklineMin = 0.0;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        // ----------------------------------------------------------------
        // Displayed metric text properties
        // ----------------------------------------------------------------

        /// <summary>Display text for the process working-set size.</summary>
        public string WorkingSetText
        {
            get => _workingSetText;
            set { _workingSetText = value; OnPropertyChanged(nameof(WorkingSetText)); }
        }

        /// <summary>Display text for the managed (GC) heap size.</summary>
        public string ManagedHeapText
        {
            get => _managedHeapText;
            set { _managedHeapText = value; OnPropertyChanged(nameof(ManagedHeapText)); }
        }

        /// <summary>Display text for the total GC collection count.</summary>
        public string GcCollectionsText
        {
            get => _gcCollectionsText;
            set { _gcCollectionsText = value; OnPropertyChanged(nameof(GcCollectionsText)); }
        }

        /// <summary>Display text for the process uptime.</summary>
        public string UptimeText
        {
            get => _uptimeText;
            set { _uptimeText = value; OnPropertyChanged(nameof(UptimeText)); }
        }

        /// <summary>Display text for the active thread count.</summary>
        public string ThreadCountText
        {
            get => _threadCountText;
            set { _threadCountText = value; OnPropertyChanged(nameof(ThreadCountText)); }
        }

        /// <summary>Display text for the OS handle count.</summary>
        public string HandleCountText
        {
            get => _handleCountText;
            set { _handleCountText = value; OnPropertyChanged(nameof(HandleCountText)); }
        }

        /// <summary>Display text for the captured sample count.</summary>
        public string SampleCountText
        {
            get => _sampleCountText;
            set { _sampleCountText = value; OnPropertyChanged(nameof(SampleCountText)); }
        }

        /// <summary>Display text for the peak working-set across all samples.</summary>
        public string PeakWorkingSetText
        {
            get => _peakWorkingSetText;
            set { _peakWorkingSetText = value; OnPropertyChanged(nameof(PeakWorkingSetText)); }
        }

        /// <summary>Display text for the timestamp of the most recent sample.</summary>
        public string LastSampleTimeText
        {
            get => _lastSampleTimeText;
            set { _lastSampleTimeText = value; OnPropertyChanged(nameof(LastSampleTimeText)); }
        }

        /// <summary>Text displayed in the status bar at the bottom of the window.</summary>
        public string StatusText
        {
            get => _statusText;
            set { _statusText = value; OnPropertyChanged(nameof(StatusText)); }
        }

        // ----------------------------------------------------------------
        // Sparkline / trend data
        // ----------------------------------------------------------------

        /// <summary>
        /// Recent working-set values (up to <c>60</c> points) used to render
        /// the trend sparkline. Updated on every metrics refresh.
        /// </summary>
        public ObservableCollection<double> WorkingSetHistory { get; }
            = new ObservableCollection<double>();

        /// <summary>Maximum working-set value in <see cref="WorkingSetHistory"/>.</summary>
        public double SparklineMax
        {
            get => _sparklineMax;
            private set { _sparklineMax = value; OnPropertyChanged(nameof(SparklineMax)); }
        }

        /// <summary>Minimum working-set value in <see cref="WorkingSetHistory"/>.</summary>
        public double SparklineMin
        {
            get => _sparklineMin;
            private set { _sparklineMin = value; OnPropertyChanged(nameof(SparklineMin)); }
        }

        // ----------------------------------------------------------------
        // Update methods (called from code-behind after each refresh)
        // ----------------------------------------------------------------

        /// <summary>
        /// Updates the memory and runtime display text from a freshly captured
        /// <see cref="MemorySnapshot"/> and the latest collector entries.
        /// Also appends the working-set value to <see cref="WorkingSetHistory"/>.
        /// </summary>
        /// <param name="snapshot">Current memory snapshot.</param>
        /// <param name="entries">Current runtime metric entries from the collector.</param>
        /// <param name="totalMemorySamples">Total memory snapshots count.</param>
        /// <param name="totalRuntimeSamples">Total runtime samples count.</param>
        /// <param name="peakWorkingSetMB">Peak working-set across all snapshots.</param>
        public void UpdateFromSnapshot(
            MemorySnapshot snapshot,
            System.Collections.Generic.IReadOnlyDictionary<string, RuntimeMetricEntry> entries,
            int totalMemorySamples,
            int totalRuntimeSamples,
            double peakWorkingSetMB)
        {
            if (snapshot == null)
            {
                return;
            }

            // Memory values
            WorkingSetText = entries != null && entries.TryGetValue("WorkingSetMB", out RuntimeMetricEntry wsmEntry)
                ? $"{wsmEntry.LastValue:F1} MB"
                : $"{snapshot.WorkingSetMB:F1} MB";

            ManagedHeapText = entries != null && entries.TryGetValue("ManagedHeapMB", out RuntimeMetricEntry mhmEntry)
                ? $"{mhmEntry.LastValue:F1} MB"
                : $"{snapshot.ManagedHeapMB:F1} MB";

            GcCollectionsText = entries != null && entries.TryGetValue("GcCollectionCount", out RuntimeMetricEntry gcEntry)
                ? ((int)gcEntry.LastValue).ToString()
                : (snapshot.GcCollections != null
                    ? snapshot.GcCollections[0].ToString()
                    : "0");

            // Runtime values
            UptimeText = FormatUptime(snapshot.Timestamp);
            ThreadCountText = entries != null && entries.TryGetValue("ThreadCount", out RuntimeMetricEntry tcEntry)
                ? ((int)tcEntry.LastValue).ToString()
                : snapshot.ThreadCount.ToString();
            HandleCountText = snapshot.HandleCount.ToString();

            // Sample history
            SampleCountText = $"{totalMemorySamples} memory, {totalRuntimeSamples} runtime sample(s)";
            PeakWorkingSetText = $"Peak WS: {peakWorkingSetMB:F1} MB";
            LastSampleTimeText = $"Last: {snapshot.Timestamp.ToLocalTime():HH:mm:ss}";

            // Append working-set to sparkline history
            if (WorkingSetHistory.Count >= MaxSparklinePoints)
            {
                WorkingSetHistory.RemoveAt(0);
            }

            WorkingSetHistory.Add(snapshot.WorkingSetMB);

            // Recompute range for canvas scaling
            double max = 1.0;
            double min = double.MaxValue;
            foreach (double v in WorkingSetHistory)
            {
                if (v > max) max = v;
                if (v < min) min = v;
            }

            // Ensure min is never greater than max (e.g. when history has one entry)
            if (min > max) min = 0.0;

            SparklineMax = max;
            SparklineMin = min;
        }

        /// <summary>
        /// Clears the sparkline history (called when the user clicks "Clear History").
        /// </summary>
        public void ClearHistory()
        {
            WorkingSetHistory.Clear();
            SparklineMax = 1.0;
            SparklineMin = 0.0;
            SampleCountText = "0 samples";
            PeakWorkingSetText = "Peak WS: -- MB";
            LastSampleTimeText = "Last: --";
        }

        // ----------------------------------------------------------------
        // Private helpers
        // ----------------------------------------------------------------

        private static string FormatUptime(DateTime snapshotTime)
        {
            TimeSpan uptime = snapshotTime
                - System.Diagnostics.Process.GetCurrentProcess().StartTime.ToUniversalTime();

            if (uptime.TotalSeconds < 0)
            {
                return "0 s";
            }

            return uptime.TotalHours >= 1
                ? $"{uptime.Hours}h {uptime.Minutes}m"
                : $"{(int)uptime.TotalSeconds} s";
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
