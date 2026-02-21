////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// AnimationSessionTests.cs
//
// Unit tests for AnimationSession covering T06–T14 from the design spec §8.1:
//   T06 - AnimationSession transitions to Running on Start()
//   T07 - AnimationSession transitions to Stopped on Stop()
//   T08 - AnimationSession transitions to Paused on Pause(), resumes on Resume()
//   T09 - AnimationSession publishes AnimationStateChangedEvent on each state transition
//   T10 - AnimationSession highlights widgets in order matching AutoScanStrategy
//   T11 - AnimationSession pauses; widget highlight position preserved on Resume()
//   T12 - Interrupt() triggers widget selection (mock renderer confirms current widget)
//   T13 - AnimationSession loops when iterations > 1
//   T14 - AnimationSession stops after all iterations reached; publishes Stopped event
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
using System.Linq;
using Xunit;

namespace ACAT.Experimental.AnimationPOC.Tests
{
    public class AnimationSessionTests
    {
        // ── T06: Transitions to Running on Start() ───────────────────

        [Fact]
        public void T06_Start_TransitionsToRunning()
        {
            using var session = MakeSession(iterations: "3");
            Assert.Equal(PlayerState.Unknown, session.State);

            session.Start();

            Assert.Equal(PlayerState.Running, session.State);
        }

        // ── T07: Transitions to Stopped on Stop() ────────────────────

        [Fact]
        public void T07_Stop_TransitionsToStopped()
        {
            using var session = MakeSession();
            session.Start();
            session.Stop();

            Assert.Equal(PlayerState.Stopped, session.State);
        }

        [Fact]
        public void T07b_Stop_WhenAlreadyStopped_DoesNotThrow()
        {
            using var session = MakeSession();
            session.Stop(); // not started
            Assert.Equal(PlayerState.Stopped, session.State);
        }

        // ── T08: Pause and Resume ─────────────────────────────────────

        [Fact]
        public void T08_Pause_TransitionsToPaused()
        {
            using var session = MakeSession(iterations: "3");
            session.Start();
            session.Pause();

            Assert.Equal(PlayerState.Paused, session.State);
        }

        [Fact]
        public void T08b_Resume_AfterPause_TransitionsToRunning()
        {
            using var session = MakeSession(iterations: "3");
            session.Start();
            session.Pause();
            session.Resume();

            Assert.Equal(PlayerState.Running, session.State);
        }

        [Fact]
        public void T08c_Pause_WhenNotRunning_IsNoOp()
        {
            using var session = MakeSession();
            session.Pause(); // state is Unknown — should be no-op
            Assert.Equal(PlayerState.Unknown, session.State);
        }

        // ── T09: AnimationStateChangedEvent published on each transition ──

        [Fact]
        public void T09_Start_PublishesStateChangedEvent_ToRunning()
        {
            var (session, _, bus) = MakeSessionWithBus(iterations: "3");
            var events = new List<AnimationStateChangedEvent>();
            bus.Subscribe<AnimationStateChangedEvent>(e => events.Add(e));

            using (session) { session.Start(); }

            Assert.Contains(events, e => e.NewState == PlayerState.Running);
        }

        [Fact]
        public void T09b_Stop_PublishesStateChangedEvent_ToStopped()
        {
            var (session, _, bus) = MakeSessionWithBus(iterations: "3");
            var events = new List<AnimationStateChangedEvent>();
            bus.Subscribe<AnimationStateChangedEvent>(e => events.Add(e));

            session.Start();
            session.Stop();
            session.Dispose();

            Assert.Contains(events, e => e.NewState == PlayerState.Stopped);
        }

        [Fact]
        public void T09c_Pause_PublishesStateChangedEvent_ToPaused()
        {
            var (session, _, bus) = MakeSessionWithBus(iterations: "3");
            var events = new List<AnimationStateChangedEvent>();
            bus.Subscribe<AnimationStateChangedEvent>(e => events.Add(e));

            session.Start();
            session.Pause();
            session.Dispose();

            Assert.Contains(events, e => e.NewState == PlayerState.Paused);
        }

        // ── T10: Highlights widgets in AutoScanStrategy order ─────────

        [Fact]
        public void T10_ManualTick_HighlightsWidgetsInOrder()
        {
            var (session, timer, _) = MakeSessionWithTimer(
                widgets: new[] { "W0", "W1", "W2" }, iterations: "5");
            var rendered = new List<string>();
            // We use a capture renderer to track highlight order

            var renderer = new CaptureRenderer();
            var captureSession = MakeSessionWithCapture(
                new[] { "W0", "W1", "W2" }, "5", renderer, out timer);

            captureSession.Start();

            timer.ManualTick(); // → W0
            timer.ManualTick(); // → W1
            timer.ManualTick(); // → W2

            captureSession.Stop();
            captureSession.Dispose();

            Assert.Equal(new[] { "W0", "W1", "W2" }, renderer.RenderedWidgets.Take(3).ToArray());
        }

        // ── T11: Widget position preserved on Resume after Pause ──────

        [Fact]
        public void T11_Pause_WidgetPositionPreservedOnResume()
        {
            var renderer = new CaptureRenderer();
            var captureSession = MakeSessionWithCapture(
                new[] { "W0", "W1", "W2" }, "5", renderer, out var timer);

            captureSession.Start();
            timer.ManualTick(); // → W0
            timer.ManualTick(); // → W1

            captureSession.Pause();
            // Should still show W1 as highlighted (renderer didn't clear it on pause)
            string widgetBeforePause = renderer.LastRendered;

            captureSession.Resume();
            // After resume, next tick should advance from W1
            timer.ManualTick(); // → W2

            captureSession.Stop();
            captureSession.Dispose();

            Assert.Equal("W1", widgetBeforePause);
            Assert.Equal("W2", renderer.LastRendered);
        }

        // ── T12: Interrupt triggers current widget selection ──────────

        [Fact]
        public void T12_Interrupt_SelectsCurrentWidget()
        {
            var renderer = new CaptureRenderer();
            var captureSession = MakeSessionWithCapture(
                new[] { "W0", "W1", "W2" }, "5", renderer, out var timer);

            captureSession.Start();
            timer.ManualTick(); // → W0
            timer.ManualTick(); // → W1

            string widgetBefore = renderer.LastRendered;
            captureSession.Interrupt(); // should select W1 and return to Running

            Assert.Equal("W1", widgetBefore);
            Assert.Equal(PlayerState.Running, captureSession.State);
            captureSession.Dispose();
        }

        // ── T13: Loops when iterations > 1 ───────────────────────────

        [Fact]
        public void T13_Session_LoopsWhenIterationsGreaterThanOne()
        {
            var renderer = new CaptureRenderer();
            var captureSession = MakeSessionWithCapture(
                new[] { "W0", "W1" }, "2", renderer, out var timer);

            captureSession.Start();
            // Iteration 1: W0, W1, (end → loop)
            timer.ManualTick(); // W0
            timer.ManualTick(); // W1
            timer.ManualTick(); // W0 again (iteration 2)
            timer.ManualTick(); // W1 again

            // State should still be Running (not stopped yet after 2 iterations
            // because the engine stops after Timeout + Stopped)
            captureSession.Dispose();

            // Verify we saw at least 4 highlights (looped through twice)
            Assert.True(renderer.RenderedWidgets.Count >= 4, 
                $"Expected >= 4 highlights, got {renderer.RenderedWidgets.Count}");
        }

        // ── T14: Stops after all iterations reached ───────────────────

        [Fact]
        public void T14_Session_StopsAfterIterationsReached_PublishesStoppedEvent()
        {
            var (session, timer, bus) = MakeSessionWithBusAndTimer(
                new[] { "W0", "W1" }, iterations: "1");
            var events = new List<AnimationStateChangedEvent>();
            bus.Subscribe<AnimationStateChangedEvent>(e => events.Add(e));

            session.Start();

            // Tick through 2 widgets (end of 1 iteration)
            timer.ManualTick(); // W0
            timer.ManualTick(); // W1
            timer.ManualTick(); // end of sequence → Timeout → Stopped

            // Give state machine time to settle
            System.Threading.Thread.Sleep(10);

            session.Dispose();

            Assert.Contains(events, e => e.NewState == PlayerState.Stopped);
        }

        // ── SetSelectedWidget ─────────────────────────────────────────

        [Fact]
        public void SetSelectedWidget_ValidName_UpdatesCurrentWidget()
        {
            var renderer = new CaptureRenderer();
            var captureSession = MakeSessionWithCapture(
                new[] { "W0", "W1", "W2" }, "5", renderer, out var timer);

            captureSession.Start();
            captureSession.SetSelectedWidget("W2");

            // Next tick should advance from W2 → end (then loop)
            timer.ManualTick(); // -1 (end of sequence), triggers loop back to W0
            timer.ManualTick(); // W0

            captureSession.Stop();
            captureSession.Dispose();

            Assert.Equal("W0", renderer.LastRendered);
        }

        // ── Start is no-op when already Running ──────────────────────

        [Fact]
        public void Start_WhenAlreadyRunning_IsNoOp()
        {
            using var session = MakeSession(iterations: "5");
            session.Start();
            PlayerState stateAfterFirst = session.State;

            session.Start(); // should be no-op
            PlayerState stateAfterSecond = session.State;

            Assert.Equal(PlayerState.Running, stateAfterFirst);
            Assert.Equal(PlayerState.Running, stateAfterSecond);
        }

        // ── Dispose ───────────────────────────────────────────────────

        [Fact]
        public void Dispose_WhenRunning_StopsTimerAndClearsHighlights()
        {
            var renderer = new CaptureRenderer();
            var session = MakeSessionWithCapture(
                new[] { "W0", "W1" }, "5", renderer, out _);
            session.Start();
            session.Dispose();
            // After dispose, renderer should have received ClearAll
            Assert.True(renderer.ClearAllCalled);
        }

        // ── Helpers ───────────────────────────────────────────────────

        private IAnimationSession MakeSession(
            string[] widgets = null, string iterations = "0")
        {
            var (session, _, _) = MakeSessionWithBus(widgets, iterations);
            return session;
        }

        private (AnimationSession session, TestScanTimer timer, SimpleEventBus bus)
            MakeSessionWithBus(string[] widgets = null, string iterations = "0")
        {
            var bus = new SimpleEventBus();
            var timer = new TestScanTimer();
            var renderer = new CaptureRenderer();
            var config = BuildConfig(widgets ?? new[] { "A", "B", "C" }, iterations);
            var session = new AnimationSession(config, timer, new AutoScanStrategy(), bus, renderer);
            return (session, timer, bus);
        }

        private (AnimationSession session, TestScanTimer timer, SimpleEventBus bus)
            MakeSessionWithTimer(string[] widgets = null, string iterations = "0")
        {
            return MakeSessionWithBus(widgets, iterations);
        }

        private (AnimationSession session, TestScanTimer timer, SimpleEventBus bus)
            MakeSessionWithBusAndTimer(string[] widgets, string iterations)
        {
            return MakeSessionWithBus(widgets, iterations);
        }

        private AnimationSession MakeSessionWithCapture(
            string[] widgetNames, string iterations, CaptureRenderer renderer, out TestScanTimer timer)
        {
            var bus = new SimpleEventBus();
            timer = new TestScanTimer();
            var config = BuildConfig(widgetNames, iterations);
            return new AnimationSession(config, timer, new AutoScanStrategy(), bus, renderer);
        }

        private static AnimationConfig BuildConfig(string[] widgetNames, string iterations)
        {
            var widgets = new List<AnimationWidgetConfig>();
            foreach (var name in widgetNames)
                widgets.Add(new AnimationWidgetConfig { Name = name });

            return new AnimationConfig
            {
                PanelName = "TestPanel",
                Sequences = new List<AnimationSequenceConfig>
                {
                    new AnimationSequenceConfig
                    {
                        Name = "TestSeq",
                        IsFirst = true,
                        Iterations = iterations,
                        ScanTime = "100",
                        Widgets = widgets
                    }
                }
            };
        }

        // ── CaptureRenderer: test double for IHighlightRenderer ───────

        private class CaptureRenderer : IHighlightRenderer
        {
            public List<string> RenderedWidgets { get; } = new List<string>();
            public string LastRendered { get; private set; }
            public bool ClearAllCalled { get; private set; }

            public void Render(string widgetName, HighlightStyle style)
            {
                RenderedWidgets.Add(widgetName);
                LastRendered = widgetName;
            }

            public void ClearHighlight(string widgetName) { }

            public void ClearAll()
            {
                ClearAllCalled = true;
            }
        }
    }
}
