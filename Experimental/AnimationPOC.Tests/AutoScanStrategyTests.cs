////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// AutoScanStrategyTests.cs
//
// Unit tests for AutoScanStrategy covering:
//   T01 - SelectNext returns 0 when currentIndex is -1
//   T02 - SelectNext advances index by 1
//   T03 - SelectNext returns -1 after last widget
//   Extra: SelectPrevious, HandleInput, empty widget list
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Experimental.AnimationPOC.Config;
using ACAT.Experimental.AnimationPOC.Interfaces;
using ACAT.Experimental.AnimationPOC.Strategies;
using System.Collections.Generic;
using Xunit;

namespace ACAT.Experimental.AnimationPOC.Tests
{
    public class AutoScanStrategyTests
    {
        private readonly AutoScanStrategy _strategy = new AutoScanStrategy();
        private readonly IReadOnlyList<AnimationWidgetConfig> _widgets = BuildWidgets(3);
        private readonly IScanContext _context = new MockScanContext();

        // ── T01: SelectNext with no current widget ───────────────────

        [Fact]
        public void T01_SelectNext_WhenCurrentIndexIsMinusOne_ReturnsZero()
        {
            int next = _strategy.SelectNext(_widgets, -1, _context);
            Assert.Equal(0, next);
        }

        // ── T02: SelectNext advances index by 1 ─────────────────────

        [Fact]
        public void T02_SelectNext_WhenCurrentIndex0_Returns1()
        {
            int next = _strategy.SelectNext(_widgets, 0, _context);
            Assert.Equal(1, next);
        }

        [Fact]
        public void T02b_SelectNext_WhenCurrentIndex1_Returns2()
        {
            int next = _strategy.SelectNext(_widgets, 1, _context);
            Assert.Equal(2, next);
        }

        // ── T03: SelectNext returns -1 after last widget ─────────────

        [Fact]
        public void T03_SelectNext_WhenCurrentIndexIsLast_ReturnsMinusOne()
        {
            int next = _strategy.SelectNext(_widgets, 2, _context); // 3 widgets, index 2 is last
            Assert.Equal(-1, next);
        }

        // ── Extra: empty widget list ─────────────────────────────────

        [Fact]
        public void SelectNext_EmptyWidgetList_ReturnsMinusOne()
        {
            var empty = new List<AnimationWidgetConfig>();
            int next = _strategy.SelectNext(empty, -1, _context);
            Assert.Equal(-1, next);
        }

        [Fact]
        public void SelectNext_NullWidgetList_ReturnsMinusOne()
        {
            int next = _strategy.SelectNext(null, -1, _context);
            Assert.Equal(-1, next);
        }

        // ── SelectPrevious ───────────────────────────────────────────

        [Fact]
        public void SelectPrevious_WhenCurrentIndex2_Returns1()
        {
            int prev = _strategy.SelectPrevious(_widgets, 2, _context);
            Assert.Equal(1, prev);
        }

        [Fact]
        public void SelectPrevious_WhenCurrentIndex0_Returns0()
        {
            int prev = _strategy.SelectPrevious(_widgets, 0, _context);
            Assert.Equal(0, prev);
        }

        // ── HandleInput ──────────────────────────────────────────────

        [Fact]
        public void HandleInput_Switch1Activated_WhenRunning_ReturnsSelect()
        {
            var ctx = new MockScanContext { SessionState = PlayerState.Running };
            var action = _strategy.HandleInput(new ScanInputEvent(ScanInputType.Switch1Activated), ctx);
            Assert.Equal(ScanInputAction.Select, action);
        }

        [Fact]
        public void HandleInput_Switch1Activated_WhenPaused_ReturnsResume()
        {
            var ctx = new MockScanContext { SessionState = PlayerState.Paused };
            var action = _strategy.HandleInput(new ScanInputEvent(ScanInputType.Switch1Activated), ctx);
            Assert.Equal(ScanInputAction.Resume, action);
        }

        [Fact]
        public void HandleInput_OtherInput_ReturnsNone()
        {
            var action = _strategy.HandleInput(new ScanInputEvent(ScanInputType.ScanLeft), _context);
            Assert.Equal(ScanInputAction.None, action);
        }

        [Fact]
        public void HandleInput_NullEvent_ReturnsNone()
        {
            var action = _strategy.HandleInput(null, _context);
            Assert.Equal(ScanInputAction.None, action);
        }

        // ── Name property ────────────────────────────────────────────

        [Fact]
        public void Name_ReturnsAuto()
        {
            Assert.Equal("auto", _strategy.Name);
        }

        // ── Helpers ──────────────────────────────────────────────────

        private static IReadOnlyList<AnimationWidgetConfig> BuildWidgets(int count)
        {
            var list = new List<AnimationWidgetConfig>();
            for (int i = 0; i < count; i++)
                list.Add(new AnimationWidgetConfig { Name = "W" + i });
            return list;
        }

        private class MockScanContext : IScanContext
        {
            public string PanelName { get; set; } = "TestPanel";
            public string CurrentAnimationName { get; set; } = "TestSeq";
            public int IterationCount { get; set; } = 0;
            public int MaxIterations { get; set; } = 1;
            public double ScanIntervalMs { get; set; } = 600;
            public double HesitateTimeMs { get; set; } = 0;
            public PlayerState SessionState { get; set; } = PlayerState.Running;
        }
    }
}
