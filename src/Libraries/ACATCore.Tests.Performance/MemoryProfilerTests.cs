////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// MemoryProfilerTests.cs
//
// Unit tests for MemoryProfiler.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Utility.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace ACATCore.Tests.Performance
{
    [TestClass]
    public class MemoryProfilerTests
    {
        [TestMethod]
        public void CaptureSnapshot_ReturnsValidSnapshot()
        {
            var profiler = new MemoryProfiler();

            MemorySnapshot snap = profiler.CaptureSnapshot("Test");

            Assert.IsNotNull(snap);
            Assert.AreEqual("Test", snap.Label);
            Assert.IsTrue(snap.WorkingSetMB > 0, "Working set must be positive");
            Assert.IsTrue(snap.ManagedHeapMB >= 0, "Managed heap must be non-negative");
            Assert.IsTrue(snap.ThreadCount > 0, "Thread count must be positive");
        }

        [TestMethod]
        public void CaptureSnapshot_StoredInList()
        {
            var profiler = new MemoryProfiler();

            profiler.CaptureSnapshot("First");
            profiler.CaptureSnapshot("Second");

            IReadOnlyList<MemorySnapshot> snapshots = profiler.GetSnapshots();
            Assert.AreEqual(2, snapshots.Count);
            Assert.AreEqual("First", snapshots[0].Label);
            Assert.AreEqual("Second", snapshots[1].Label);
        }

        [TestMethod]
        public void ClearSnapshots_RemovesAll()
        {
            var profiler = new MemoryProfiler();
            profiler.CaptureSnapshot("A");
            profiler.CaptureSnapshot("B");

            profiler.ClearSnapshots();

            Assert.AreEqual(0, profiler.GetSnapshots().Count);
        }

        [TestMethod]
        public void CompareSnapshots_ReturnsReport()
        {
            var profiler = new MemoryProfiler();

            MemorySnapshot before = profiler.CaptureSnapshot("Before");
            // Allocate a small amount so the snapshot differs
            byte[] dummy = new byte[1024];
            MemorySnapshot after = profiler.CaptureSnapshot("After");

            string report = MemoryProfiler.CompareSnapshots(before, after);

            Assert.IsFalse(string.IsNullOrEmpty(report));
            Assert.IsTrue(report.Contains("Memory Delta Report"));
            Assert.IsTrue(report.Contains("Working Set"));
        }

        [TestMethod]
        public void IsPotentialLeak_BelowThreshold_ReturnsFalse()
        {
            var before = new MemorySnapshot { WorkingSetMB = 100.0 };
            var after  = new MemorySnapshot { WorkingSetMB = 140.0 };

            bool result = MemoryProfiler.IsPotentialLeak(before, after, thresholdMB: 50.0);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsPotentialLeak_AboveThreshold_ReturnsTrue()
        {
            var before = new MemorySnapshot { WorkingSetMB = 100.0 };
            var after  = new MemorySnapshot { WorkingSetMB = 200.0 };

            bool result = MemoryProfiler.IsPotentialLeak(before, after, thresholdMB: 50.0);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsPotentialLeak_NullArguments_ReturnsFalse()
        {
            Assert.IsFalse(MemoryProfiler.IsPotentialLeak(null, new MemorySnapshot()));
            Assert.IsFalse(MemoryProfiler.IsPotentialLeak(new MemorySnapshot(), null));
        }
    }
}
