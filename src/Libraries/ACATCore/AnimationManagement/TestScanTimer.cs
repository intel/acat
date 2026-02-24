////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// TestScanTimer.cs
//
// Test-only IScanTimer implementation.
// ManualTick() fires Elapsed synchronously on the calling thread,
// making timer-driven tests deterministic.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.AnimationManagement.Interfaces;
using System;
using System.Reflection;
using System.Timers;

namespace ACAT.Core.AnimationManagement
{
    /// <summary>
    /// Test-only implementation of IScanTimer.
    ///
    /// ManualTick() fires Elapsed synchronously on the calling thread when Enabled=true.
    /// This makes timer-driven AnimationSession tests fully deterministic.
    ///
    /// Interval and Enabled properties are respected by ManualTick():
    ///  - ManualTick() does nothing when Enabled=false.
    ///  - If AutoReset=false, ManualTick() sets Enabled=false after firing.
    /// </summary>
    public class TestScanTimer : IScanTimer
    {
        /// <inheritdoc/>
        public bool Enabled { get; set; }

        /// <inheritdoc/>
        public double Interval { get; set; } = 600;

        /// <inheritdoc/>
        public bool AutoReset { get; set; } = true;

        /// <inheritdoc/>
        public event ElapsedEventHandler Elapsed;

        /// <inheritdoc/>
        public void Start() => Enabled = true;

        /// <inheritdoc/>
        public void Stop() => Enabled = false;

        /// <inheritdoc/>
        public void Dispose() { }

        /// <summary>
        /// Fires Elapsed synchronously on the current thread if Enabled is true.
        /// Simulates one timer tick. Respects AutoReset: if false, sets Enabled=false after firing.
        /// This is the primary API for deterministic testing of timer-driven scan logic.
        /// </summary>
        public void ManualTick()
        {
            if (!Enabled) return;
            // ElapsedEventArgs has no public constructor on .NET 4.8.1;
            // create via reflection using the internal DateTime constructor.
            var args = (ElapsedEventArgs)Activator.CreateInstance(
                typeof(ElapsedEventArgs),
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                null,
                new object[] { DateTime.UtcNow },
                null);
            Elapsed?.Invoke(this, args);
            if (!AutoReset) Enabled = false;
        }
    }
}
