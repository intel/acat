////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// MemoryProfiler.cs
//
// Captures memory snapshots and detects potential memory leaks by comparing
// snapshots across key application lifecycle points.
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ACAT.Core.Utility.Diagnostics
{
    /// <summary>
    /// A point-in-time snapshot of the process memory state.
    /// </summary>
    public class MemorySnapshot
    {
        /// <summary>UTC timestamp of the snapshot.</summary>
        public DateTime Timestamp { get; set; }

        /// <summary>Human-readable label for this snapshot (e.g. "AfterStartup").</summary>
        public string Label { get; set; }

        /// <summary>Process working-set size in megabytes.</summary>
        public double WorkingSetMB { get; set; }

        /// <summary>Private (committed) memory in megabytes.</summary>
        public double PrivateMemoryMB { get; set; }

        /// <summary>Total managed (GC) heap size in megabytes.</summary>
        public double ManagedHeapMB { get; set; }

        /// <summary>GC collection counts per generation (index 0 = Gen0).</summary>
        public int[] GcCollections { get; set; }

        /// <summary>Number of OS threads owned by the process.</summary>
        public int ThreadCount { get; set; }

        /// <summary>Number of open file handles.</summary>
        public int HandleCount { get; set; }
    }

    /// <summary>
    /// Captures memory snapshots at named points in the application lifecycle
    /// and provides basic leak-detection by comparing two snapshots.
    /// </summary>
    public class MemoryProfiler
    {
        private readonly List<MemorySnapshot> _snapshots = new List<MemorySnapshot>();
        private readonly object _lock = new object();

        /// <summary>
        /// Capture a snapshot of the current process memory state.
        /// </summary>
        /// <param name="label">Optional descriptive label for this snapshot.</param>
        /// <returns>The captured snapshot.</returns>
        public MemorySnapshot CaptureSnapshot(string label = "")
        {
            var process = Process.GetCurrentProcess();
            process.Refresh();

            int maxGen = GC.MaxGeneration;
            var gcCounts = new int[maxGen + 1];
            for (int gen = 0; gen <= maxGen; gen++)
            {
                gcCounts[gen] = GC.CollectionCount(gen);
            }

            var snapshot = new MemorySnapshot
            {
                Timestamp = DateTime.UtcNow,
                Label = label ?? string.Empty,
                WorkingSetMB = process.WorkingSet64 / (1024.0 * 1024.0),
                PrivateMemoryMB = process.PrivateMemorySize64 / (1024.0 * 1024.0),
                ManagedHeapMB = GC.GetTotalMemory(false) / (1024.0 * 1024.0),
                GcCollections = gcCounts,
                ThreadCount = process.Threads.Count,
                HandleCount = process.HandleCount
            };

            lock (_lock)
            {
                _snapshots.Add(snapshot);
            }

            return snapshot;
        }

        /// <summary>
        /// Returns all captured snapshots in capture order.
        /// </summary>
        public IReadOnlyList<MemorySnapshot> GetSnapshots()
        {
            lock (_lock)
            {
                return new List<MemorySnapshot>(_snapshots);
            }
        }

        /// <summary>
        /// Clears all stored snapshots.
        /// </summary>
        public void ClearSnapshots()
        {
            lock (_lock)
            {
                _snapshots.Clear();
            }
        }

        /// <summary>
        /// Compares two snapshots and returns a human-readable delta report.
        /// </summary>
        /// <param name="before">Earlier snapshot.</param>
        /// <param name="after">Later snapshot.</param>
        /// <returns>Report describing the memory delta between the two snapshots.</returns>
        public static string CompareSnapshots(MemorySnapshot before, MemorySnapshot after)
        {
            if (before == null) throw new ArgumentNullException("before");
            if (after == null) throw new ArgumentNullException("after");

            var sb = new StringBuilder();
            sb.AppendLine("Memory Delta Report");
            sb.AppendLine(new string('-', 50));
            sb.AppendFormat("  From : {0} ({1}){2}", before.Timestamp.ToLocalTime(), before.Label, Environment.NewLine);
            sb.AppendFormat("  To   : {0} ({1}){2}", after.Timestamp.ToLocalTime(), after.Label, Environment.NewLine);
            sb.AppendLine();

            double wsDelta = after.WorkingSetMB - before.WorkingSetMB;
            double pmDelta = after.PrivateMemoryMB - before.PrivateMemoryMB;
            double mhDelta = after.ManagedHeapMB - before.ManagedHeapMB;

            sb.AppendFormat("  Working Set    : {0:F1} MB → {1:F1} MB  (Δ {2:+0.0;-0.0} MB){3}",
                before.WorkingSetMB, after.WorkingSetMB, wsDelta, Environment.NewLine);
            sb.AppendFormat("  Private Memory : {0:F1} MB → {1:F1} MB  (Δ {2:+0.0;-0.0} MB){3}",
                before.PrivateMemoryMB, after.PrivateMemoryMB, pmDelta, Environment.NewLine);
            sb.AppendFormat("  Managed Heap   : {0:F1} MB → {1:F1} MB  (Δ {2:+0.0;-0.0} MB){3}",
                before.ManagedHeapMB, after.ManagedHeapMB, mhDelta, Environment.NewLine);
            sb.AppendFormat("  Threads        : {0} → {1}{2}",
                before.ThreadCount, after.ThreadCount, Environment.NewLine);
            sb.AppendFormat("  Handles        : {0} → {1}{2}",
                before.HandleCount, after.HandleCount, Environment.NewLine);

            if (before.GcCollections != null && after.GcCollections != null)
            {
                sb.AppendLine();
                sb.AppendLine("  GC Collections:");
                int gens = Math.Min(before.GcCollections.Length, after.GcCollections.Length);
                for (int gen = 0; gen < gens; gen++)
                {
                    int delta = after.GcCollections[gen] - before.GcCollections[gen];
                    sb.AppendFormat("    Gen{0}: {1:+0;-0;0} collection(s){2}", gen, delta, Environment.NewLine);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns <c>true</c> if the working-set growth between two snapshots
        /// exceeds <paramref name="thresholdMB"/> megabytes, indicating a potential leak.
        /// </summary>
        public static bool IsPotentialLeak(MemorySnapshot before, MemorySnapshot after, double thresholdMB = 50.0)
        {
            if (before == null || after == null)
            {
                return false;
            }

            return (after.WorkingSetMB - before.WorkingSetMB) > thresholdMB;
        }
    }
}
