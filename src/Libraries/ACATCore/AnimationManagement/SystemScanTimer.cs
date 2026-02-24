////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// SystemScanTimer.cs
//
// Default IScanTimer implementation wrapping System.Timers.Timer.
// Behavior is identical to the existing timer in AnimationPlayer.cs.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.AnimationManagement.Interfaces;
using System.Timers;

namespace ACAT.Core.AnimationManagement
{
    /// <summary>
    /// Production IScanTimer implementation.
    /// Wraps System.Timers.Timer with no behavior change from the existing AnimationPlayer timer.
    /// Elapsed fires on a thread-pool thread (same as System.Timers.Timer).
    /// </summary>
    public class SystemScanTimer : IScanTimer
    {
        private readonly Timer _timer;
        private bool _disposed;

        /// <summary>Initializes a new SystemScanTimer with a 600ms default interval.</summary>
        public SystemScanTimer()
        {
            _timer = new Timer(600);
            _timer.AutoReset = true;
            _timer.Elapsed += OnInternalElapsed;
        }

        /// <inheritdoc/>
        public bool Enabled
        {
            get => _timer.Enabled;
            set => _timer.Enabled = value;
        }

        /// <inheritdoc/>
        public double Interval
        {
            get => _timer.Interval;
            set => _timer.Interval = value;
        }

        /// <inheritdoc/>
        public bool AutoReset
        {
            get => _timer.AutoReset;
            set => _timer.AutoReset = value;
        }

        /// <inheritdoc/>
        public event ElapsedEventHandler Elapsed;

        /// <inheritdoc/>
        public void Start() => _timer.Start();

        /// <inheritdoc/>
        public void Stop() => _timer.Stop();

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _timer.Elapsed -= OnInternalElapsed;
            _timer.Dispose();
        }

        private void OnInternalElapsed(object sender, ElapsedEventArgs e)
        {
            Elapsed?.Invoke(this, e);
        }
    }
}
