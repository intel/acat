////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// PerformanceTests.cs
//
// T15 — Performance gate tests for the Animation POC:
//   - IEventBus dispatch time < 1ms per event
//   - AnimationSession construction time < 20ms (even for 25-widget BCI config)
//   - Scan interval accuracy: TestScanTimer tick overhead < 1ms
//
// These tests validate the performance targets from Issue #207 §9.4.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Experimental.AnimationPOC.Config;
using ACAT.Experimental.AnimationPOC.Core;
using ACAT.Experimental.AnimationPOC.Events;
using ACAT.Experimental.AnimationPOC.Infrastructure;
using ACAT.Experimental.AnimationPOC.Interfaces;
using ACAT.Experimental.AnimationPOC.Strategies;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xunit;
using Xunit.v3;

namespace ACAT.Experimental.AnimationPOC.Tests
{
    public class PerformanceTests
    {
        private readonly ITestOutputHelper _output;

        public PerformanceTests(ITestOutputHelper output)
        {
            _output = output;
        }

        // ── T15a: IEventBus dispatch time < 1ms per event ────────────

        [Fact]
        public void T15a_EventBus_DispatchTime_LessThan1ms()
        {
            const int iterations = 1000;
            var bus = new SimpleEventBus();
            int received = 0;

            bus.Subscribe<AnimationStateChangedEvent>(_ => received++);

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                bus.Publish(new AnimationStateChangedEvent("P", PlayerState.Running, PlayerState.Paused, "S"));
            }
            sw.Stop();

            double avgMs = sw.Elapsed.TotalMilliseconds / iterations;
            _output.WriteLine($"EventBus dispatch avg: {avgMs:F4}ms over {iterations} publishes ({sw.Elapsed.TotalMilliseconds:F1}ms total)");

            Assert.Equal(iterations, received);
            Assert.True(avgMs < 1.0,
                $"EventBus dispatch avg {avgMs:F4}ms exceeds 1ms target");
        }

        // ── T15b: AnimationSession construction < 20ms (standard config) ─

        [Fact]
        public void T15b_SessionConstruction_StandardConfig_LessThan20ms()
        {
            const int iterations = 100;
            var bus = new SimpleEventBus();
            var renderer = new NullHighlightRenderer();
            var config = BuildConfig(8); // standard panel: 8 widgets

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                var timer = new TestScanTimer();
                using var session = new AnimationSession(config, timer, new AutoScanStrategy(), bus, renderer);
            }
            sw.Stop();

            double avgMs = sw.Elapsed.TotalMilliseconds / iterations;
            _output.WriteLine($"Session construction (8 widgets) avg: {avgMs:F4}ms over {iterations}");

            Assert.True(avgMs < 20.0,
                $"Session construction {avgMs:F4}ms exceeds 20ms target");
        }

        // ── T15c: AnimationSession construction < 20ms (BCI worst-case 25 widgets) ──

        [Fact]
        public void T15c_SessionConstruction_BCIConfig25Widgets_LessThan20ms()
        {
            const int iterations = 100;
            var bus = new SimpleEventBus();
            var renderer = new NullHighlightRenderer();
            var config = BuildConfig(25); // BCI keyboard worst-case: 25 widgets

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                var timer = new TestScanTimer();
                using var session = new AnimationSession(config, timer, new AutoScanStrategy(), bus, renderer);
            }
            sw.Stop();

            double avgMs = sw.Elapsed.TotalMilliseconds / iterations;
            _output.WriteLine($"Session construction (25 widgets BCI) avg: {avgMs:F4}ms over {iterations}");

            Assert.True(avgMs < 20.0,
                $"Session construction {avgMs:F4}ms exceeds 20ms target for BCI 25-widget config");
        }

        // ── T15d: Timer tick overhead < 1ms (TestScanTimer synchronous path) ──

        [Fact]
        public void T15d_TestScanTimer_TickOverhead_LessThan1ms()
        {
            const int ticks = 1000;
            var bus = new SimpleEventBus();
            var renderer = new NullHighlightRenderer();
            var config = BuildConfig(10);
            var timer = new TestScanTimer();

            using var session = new AnimationSession(config, timer, new AutoScanStrategy(), bus, renderer);
            session.Start();

            // Warm up
            for (int i = 0; i < 10; i++) timer.ManualTick();

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < ticks; i++) timer.ManualTick();
            sw.Stop();

            double avgMs = sw.Elapsed.TotalMilliseconds / ticks;
            _output.WriteLine($"TestScanTimer tick overhead avg: {avgMs:F4}ms over {ticks} ticks ({sw.Elapsed.TotalMilliseconds:F1}ms total)");

            Assert.True(avgMs < 1.0,
                $"Timer tick overhead {avgMs:F4}ms exceeds 1ms target");
        }

        // ── T15e: Highlight event publishing < 1ms ─────────────────────

        [Fact]
        public void T15e_HighlightEvent_PublishTime_LessThan1ms()
        {
            const int ticks = 500;
            var bus = new SimpleEventBus();
            var renderer = new NullHighlightRenderer();
            var config = BuildConfig(5);
            var timer = new TestScanTimer();
            var dispatchTimes = new List<double>();

            bus.Subscribe<AnimationHighlightEvent>(_ => { }); // lightweight subscriber

            using var session = new AnimationSession(config, timer, new AutoScanStrategy(), bus, renderer);
            session.Start();

            for (int i = 0; i < ticks; i++)
            {
                var sw = Stopwatch.StartNew();
                timer.ManualTick();
                sw.Stop();
                dispatchTimes.Add(sw.Elapsed.TotalMilliseconds);
            }

            double avg = 0;
            foreach (var t in dispatchTimes) avg += t;
            avg /= dispatchTimes.Count;

            _output.WriteLine($"Highlight publish avg: {avg:F4}ms over {ticks} ticks");

            Assert.True(avg < 1.0,
                $"Highlight event publish avg {avg:F4}ms exceeds 1ms target");
        }

        // ── Helpers ───────────────────────────────────────────────────

        private static AnimationConfig BuildConfig(int widgetCount)
        {
            var widgets = new List<AnimationWidgetConfig>();
            for (int i = 0; i < widgetCount; i++)
                widgets.Add(new AnimationWidgetConfig { Name = "W" + i });

            return new AnimationConfig
            {
                PanelName = "PerfTestPanel",
                Sequences = new List<AnimationSequenceConfig>
                {
                    new AnimationSequenceConfig
                    {
                        Name = "PerfSeq",
                        IsFirst = true,
                        Iterations = "0",   // infinite loop for perf tests
                        ScanTime = "100",
                        Widgets = widgets
                    }
                }
            };
        }

        private class NullHighlightRenderer : IHighlightRenderer
        {
            public void Render(string widgetName, HighlightStyle style) { }
            public void ClearHighlight(string widgetName) { }
            public void ClearAll() { }
        }
    }
}
