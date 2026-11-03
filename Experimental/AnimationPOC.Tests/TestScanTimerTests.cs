////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// TestScanTimerTests.cs
//
// Unit tests for TestScanTimer covering:
//   T04 - ManualTick fires Elapsed synchronously
//   T05 - ManualTick does nothing when Enabled=false
//   Extra: AutoReset=false disables after firing; Start/Stop behavior
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Experimental.AnimationPOC.Infrastructure;
using System;
using System.Timers;
using Xunit;

namespace ACAT.Experimental.AnimationPOC.Tests
{
    public class TestScanTimerTests
    {
        // ── T04: ManualTick fires Elapsed synchronously ──────────────

        [Fact]
        public void T04_ManualTick_WhenEnabled_FiresElapsedSynchronously()
        {
            var timer = new TestScanTimer { Enabled = true };
            bool fired = false;
            timer.Elapsed += (s, e) => fired = true;

            timer.ManualTick();

            Assert.True(fired);
        }

        [Fact]
        public void T04b_ManualTick_FiresElapsedOnCallingThread()
        {
            var timer = new TestScanTimer { Enabled = true };
            int callCount = 0;

            timer.Elapsed += (s, e) => callCount++;
            timer.ManualTick();
            timer.ManualTick();

            Assert.Equal(2, callCount);
        }

        // ── T05: ManualTick does nothing when Enabled=false ──────────

        [Fact]
        public void T05_ManualTick_WhenDisabled_DoesNotFireElapsed()
        {
            var timer = new TestScanTimer { Enabled = false };
            bool fired = false;
            timer.Elapsed += (s, e) => fired = true;

            timer.ManualTick();

            Assert.False(fired);
        }

        // ── AutoReset behavior ───────────────────────────────────────

        [Fact]
        public void ManualTick_WhenAutoResetFalse_DisablesAfterFiring()
        {
            var timer = new TestScanTimer { Enabled = true, AutoReset = false };
            int count = 0;
            timer.Elapsed += (s, e) => count++;

            timer.ManualTick();  // fires and disables
            timer.ManualTick();  // should not fire (disabled)

            Assert.Equal(1, count);
            Assert.False(timer.Enabled);
        }

        [Fact]
        public void ManualTick_WhenAutoResetTrue_RemainsEnabledAfterFiring()
        {
            var timer = new TestScanTimer { Enabled = true, AutoReset = true };
            int count = 0;
            timer.Elapsed += (s, e) => count++;

            timer.ManualTick();
            timer.ManualTick();

            Assert.Equal(2, count);
            Assert.True(timer.Enabled);
        }

        // ── Start/Stop ───────────────────────────────────────────────

        [Fact]
        public void Start_SetsEnabledTrue()
        {
            var timer = new TestScanTimer();
            timer.Start();
            Assert.True(timer.Enabled);
        }

        [Fact]
        public void Stop_SetsEnabledFalse()
        {
            var timer = new TestScanTimer { Enabled = true };
            timer.Stop();
            Assert.False(timer.Enabled);
        }

        // ── Interval / AutoReset defaults ────────────────────────────

        [Fact]
        public void DefaultInterval_Is600Ms()
        {
            var timer = new TestScanTimer();
            Assert.Equal(600, timer.Interval);
        }

        [Fact]
        public void DefaultAutoReset_IsTrue()
        {
            var timer = new TestScanTimer();
            Assert.True(timer.AutoReset);
        }

        // ── Dispose ──────────────────────────────────────────────────

        [Fact]
        public void Dispose_DoesNotThrow()
        {
            var timer = new TestScanTimer { Enabled = true };
            timer.Dispose(); // should not throw
        }
    }
}
