////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// IScanTimer.cs
//
// Abstraction over System.Timers.Timer for the animation scan loop.
// The default implementation (SystemScanTimer) wraps System.Timers.Timer.
// The test implementation (TestScanTimer) fires Elapsed synchronously
// via ManualTick() for deterministic unit testing.
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Timers;

namespace ACAT.Core.AnimationManagement.Interfaces
{
    /// <summary>
    /// Abstraction over System.Timers.Timer used to drive the animation scan loop.
    /// Implementations must be thread-safe: Elapsed may fire on a thread-pool thread.
    /// </summary>
    public interface IScanTimer : IDisposable
    {
        /// <summary>Gets or sets whether the timer is enabled (running).</summary>
        bool Enabled { get; set; }

        /// <summary>Gets or sets the interval in milliseconds between timer firings.</summary>
        double Interval { get; set; }

        /// <summary>Gets or sets whether the timer resets automatically after each firing.</summary>
        bool AutoReset { get; set; }

        /// <summary>Raised when the timer interval elapses.</summary>
        event ElapsedEventHandler Elapsed;

        /// <summary>Starts the timer. Sets Enabled = true.</summary>
        void Start();

        /// <summary>Stops the timer. Sets Enabled = false.</summary>
        void Stop();
    }
}
