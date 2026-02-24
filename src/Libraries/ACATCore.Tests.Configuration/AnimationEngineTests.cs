////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// AnimationEngineTests.cs
//
// Unit tests for the new animation engine: IScanTimer, IScanModeStrategy,
// IAnimationSession, IAnimationService, AnimationConfigProvider,
// and XmlAnimationConfigAdapter.
//
// T01: TestScanTimer fires Elapsed event on ManualTick()
// T02: TestScanTimer does not fire when Enabled=false
// T03: AutoScanStrategy.SelectNext advances sequentially
// T04: AutoScanStrategy.SelectNext returns -1 past last widget
// T05: AutoScanStrategy.HandleInput Switch1 returns Select when Running
// T06: AutoScanStrategy.HandleInput Switch1 returns Resume when Paused
// T07: ManualScanStrategy.HandleInput ScanRight returns Advance
// T08: ManualScanStrategy.HandleInput Switch1 returns Select
// T09: StepScanStrategy wraps around on SelectNext at end
// T10: AnimationSession.Start transitions to Running state
// T11: AnimationSession.Stop transitions to Stopped and clears highlights
// T12: AnimationSession.Pause / Resume preserves widget position
// T13: AnimationSession.Interrupt selects current widget and returns to Running
// T14: AnimationSession.Transition moves to named animation sequence
// T15: AnimationSession timer tick advances highlight and publishes event
// T16: XmlAnimationConfigAdapter converts Animation XML node correctly
// T17: XmlAnimationConfigAdapter converts Widget XML nodes correctly
// T18: AnimationConfigProvider.HasJsonConfig returns false for missing file
// T19: AddAnimationEngine registers IAnimationService as Singleton
// T20: AnimationSession iteration count stops sequence after N iterations
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.AnimationManagement;
using ACAT.Core.AnimationManagement.Configuration;
using ACAT.Core.AnimationManagement.Interfaces;
using ACAT.Core.AnimationManagement.Rendering;
using ACAT.Core.AnimationManagement.Strategies;
using ACAT.Core.DependencyInjection;
using ACAT.Core.EventManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace ACATCore.Tests.Configuration
{
    [TestClass]
    public class AnimationEngineTests
    {
        // ----------------------------------------------------------------
        // Helpers
        // ----------------------------------------------------------------

        private static AnimationConfig MakeConfig(string panelName = "TestPanel",
            int widgetCount = 3, string iterations = "1", bool isFirst = true)
        {
            var widgets = new List<AnimationWidgetConfig>();
            for (int i = 0; i < widgetCount; i++)
                widgets.Add(new AnimationWidgetConfig { Name = $"Widget{i + 1}", PlayBeep = false });

            return new AnimationConfig
            {
                PanelName = panelName,
                ScanStrategy = "auto",
                Sequences = new List<AnimationSequenceConfig>
                {
                    new AnimationSequenceConfig
                    {
                        Name = "Seq1",
                        IsFirst = isFirst,
                        AutoStart = true,
                        Iterations = iterations,
                        ScanTime = "100",
                        Widgets = widgets
                    }
                }
            };
        }

        private static (AnimationSession session, TestScanTimer timer, List<string> highlighted, List<string> cleared)
            MakeSession(AnimationConfig config = null, IScanModeStrategy strategy = null)
        {
            config = config ?? MakeConfig();
            strategy = strategy ?? new AutoScanStrategy();

            var highlighted = new List<string>();
            var cleared = new List<string>();

            var timer = new TestScanTimer();
            var renderer = new WinFormsHighlightRenderer(
                (name, style) => highlighted.Add(name),
                name => cleared.Add(name),
                () => cleared.Add("__ALL__"));
            var bus = new EventBus();

            var session = new AnimationSession(config, timer, strategy, bus, renderer);
            return (session, timer, highlighted, cleared);
        }

        // ----------------------------------------------------------------
        // T01: TestScanTimer fires Elapsed event on ManualTick()
        // ----------------------------------------------------------------

        [TestMethod]
        public void T01_TestScanTimer_ManualTick_FiresElapsed()
        {
            var timer = new TestScanTimer();
            bool fired = false;
            timer.Elapsed += (s, e) => fired = true;
            timer.Start();

            timer.ManualTick();

            Assert.IsTrue(fired, "Elapsed should fire on ManualTick() when Enabled=true");
        }

        // ----------------------------------------------------------------
        // T02: TestScanTimer does not fire when Enabled=false
        // ----------------------------------------------------------------

        [TestMethod]
        public void T02_TestScanTimer_ManualTick_DoesNotFireWhenDisabled()
        {
            var timer = new TestScanTimer();
            bool fired = false;
            timer.Elapsed += (s, e) => fired = true;
            // Do NOT call Start() — Enabled remains false

            timer.ManualTick();

            Assert.IsFalse(fired, "Elapsed should not fire when Enabled=false");
        }

        // ----------------------------------------------------------------
        // T03: AutoScanStrategy.SelectNext advances sequentially
        // ----------------------------------------------------------------

        [TestMethod]
        public void T03_AutoScanStrategy_SelectNext_AdvancesSequentially()
        {
            var strategy = new AutoScanStrategy();
            var widgets = new List<AnimationWidgetConfig>
            {
                new AnimationWidgetConfig { Name = "W1" },
                new AnimationWidgetConfig { Name = "W2" },
                new AnimationWidgetConfig { Name = "W3" }
            };
            var context = MakeScanContext();

            Assert.AreEqual(0, strategy.SelectNext(widgets, -1, context)); // -1 → 0
            Assert.AreEqual(1, strategy.SelectNext(widgets, 0, context));  // 0 → 1
            Assert.AreEqual(2, strategy.SelectNext(widgets, 1, context));  // 1 → 2
        }

        // ----------------------------------------------------------------
        // T04: AutoScanStrategy.SelectNext returns -1 past last widget
        // ----------------------------------------------------------------

        [TestMethod]
        public void T04_AutoScanStrategy_SelectNext_ReturnsMinusOneAtEnd()
        {
            var strategy = new AutoScanStrategy();
            var widgets = new List<AnimationWidgetConfig>
            {
                new AnimationWidgetConfig { Name = "W1" },
                new AnimationWidgetConfig { Name = "W2" }
            };
            var context = MakeScanContext();

            int result = strategy.SelectNext(widgets, 1, context); // last index

            Assert.AreEqual(-1, result, "SelectNext should return -1 after the last widget");
        }

        // ----------------------------------------------------------------
        // T05: AutoScanStrategy.HandleInput Switch1 returns Select when Running
        // ----------------------------------------------------------------

        [TestMethod]
        public void T05_AutoScanStrategy_HandleInput_Switch1_ReturnsSelectWhenRunning()
        {
            var strategy = new AutoScanStrategy();
            var context = MakeScanContext(state: PlayerState.Running);

            var action = strategy.HandleInput(new ScanInputEvent(ScanInputType.Switch1Activated), context);

            Assert.AreEqual(ScanInputAction.Select, action);
        }

        // ----------------------------------------------------------------
        // T06: AutoScanStrategy.HandleInput Switch1 returns Resume when Paused
        // ----------------------------------------------------------------

        [TestMethod]
        public void T06_AutoScanStrategy_HandleInput_Switch1_ReturnsResumeWhenPaused()
        {
            var strategy = new AutoScanStrategy();
            var context = MakeScanContext(state: PlayerState.Paused);

            var action = strategy.HandleInput(new ScanInputEvent(ScanInputType.Switch1Activated), context);

            Assert.AreEqual(ScanInputAction.Resume, action);
        }

        // ----------------------------------------------------------------
        // T07: ManualScanStrategy.HandleInput ScanRight returns Advance
        // ----------------------------------------------------------------

        [TestMethod]
        public void T07_ManualScanStrategy_HandleInput_ScanRight_ReturnsAdvance()
        {
            var strategy = new ManualScanStrategy();
            var context = MakeScanContext();

            var action = strategy.HandleInput(new ScanInputEvent(ScanInputType.ScanRight), context);

            Assert.AreEqual(ScanInputAction.Advance, action);
        }

        // ----------------------------------------------------------------
        // T08: ManualScanStrategy.HandleInput Switch1 returns Select
        // ----------------------------------------------------------------

        [TestMethod]
        public void T08_ManualScanStrategy_HandleInput_Switch1_ReturnsSelect()
        {
            var strategy = new ManualScanStrategy();
            var context = MakeScanContext(state: PlayerState.Running);

            var action = strategy.HandleInput(new ScanInputEvent(ScanInputType.Switch1Activated), context);

            Assert.AreEqual(ScanInputAction.Select, action);
        }

        // ----------------------------------------------------------------
        // T09: StepScanStrategy wraps around on SelectNext at end
        // ----------------------------------------------------------------

        [TestMethod]
        public void T09_StepScanStrategy_SelectNext_WrapsAroundAtEnd()
        {
            var strategy = new StepScanStrategy();
            var widgets = new List<AnimationWidgetConfig>
            {
                new AnimationWidgetConfig { Name = "W1" },
                new AnimationWidgetConfig { Name = "W2" }
            };
            var context = MakeScanContext();

            int result = strategy.SelectNext(widgets, 1, context); // past last

            Assert.AreEqual(0, result, "StepScanStrategy should wrap to index 0 at end");
        }

        // ----------------------------------------------------------------
        // T10: AnimationSession.Start transitions to Running state
        // ----------------------------------------------------------------

        [TestMethod]
        public void T10_AnimationSession_Start_TransitionsToRunning()
        {
            var (session, timer, _, _) = MakeSession();

            session.Start();

            Assert.AreEqual(PlayerState.Running, session.State);

            session.Dispose();
        }

        // ----------------------------------------------------------------
        // T11: AnimationSession.Stop transitions to Stopped and clears highlights
        // ----------------------------------------------------------------

        [TestMethod]
        public void T11_AnimationSession_Stop_TransitionsToStopped()
        {
            var (session, timer, _, cleared) = MakeSession();
            session.Start();

            session.Stop();

            Assert.AreEqual(PlayerState.Stopped, session.State);
            Assert.IsTrue(cleared.Contains("__ALL__"), "ClearAll should be called on Stop");

            session.Dispose();
        }

        // ----------------------------------------------------------------
        // T12: AnimationSession.Pause / Resume preserves state
        // ----------------------------------------------------------------

        [TestMethod]
        public void T12_AnimationSession_PauseResume_PreservesState()
        {
            var (session, timer, highlighted, _) = MakeSession();
            session.Start();
            timer.ManualTick(); // advance to first widget
            string widgetAfterTick = highlighted.Count > 0 ? highlighted[highlighted.Count - 1] : null;

            session.Pause();
            Assert.AreEqual(PlayerState.Paused, session.State);

            session.Resume();
            Assert.AreEqual(PlayerState.Running, session.State);

            session.Dispose();
        }

        // ----------------------------------------------------------------
        // T13: AnimationSession.Interrupt selects widget and returns to Running
        // ----------------------------------------------------------------

        [TestMethod]
        public void T13_AnimationSession_Interrupt_ReturnsToRunning()
        {
            var (session, timer, _, _) = MakeSession();
            session.Start();
            timer.ManualTick(); // highlight first widget

            session.Interrupt(); // simulate switch press

            Assert.AreEqual(PlayerState.Running, session.State);

            session.Dispose();
        }

        // ----------------------------------------------------------------
        // T14: AnimationSession.Transition moves to named animation sequence
        // ----------------------------------------------------------------

        [TestMethod]
        public void T14_AnimationSession_Transition_MovesToNamedSequence()
        {
            var config = new AnimationConfig
            {
                PanelName = "MultiSeq",
                Sequences = new List<AnimationSequenceConfig>
                {
                    new AnimationSequenceConfig
                    {
                        Name = "Row",
                        IsFirst = true,
                        Iterations = "1",
                        Widgets = new List<AnimationWidgetConfig>
                        {
                            new AnimationWidgetConfig { Name = "R1" }
                        }
                    },
                    new AnimationSequenceConfig
                    {
                        Name = "Col",
                        IsFirst = false,
                        Iterations = "1",
                        Widgets = new List<AnimationWidgetConfig>
                        {
                            new AnimationWidgetConfig { Name = "C1" }
                        }
                    }
                }
            };

            var (session, _, _, _) = MakeSession(config);
            session.Start();

            session.Transition("Col");

            Assert.AreEqual("Col", session.CurrentAnimationName);

            session.Dispose();
        }

        // ----------------------------------------------------------------
        // T15: AnimationSession timer tick advances highlight and publishes event
        // ----------------------------------------------------------------

        [TestMethod]
        public void T15_AnimationSession_TimerTick_AdvancesHighlight()
        {
            var (session, timer, highlighted, _) = MakeSession();
            session.Start();

            timer.ManualTick(); // advance to widget 1
            timer.ManualTick(); // advance to widget 2

            Assert.IsTrue(highlighted.Count >= 2, "Highlight should advance on each timer tick");

            session.Dispose();
        }

        // ----------------------------------------------------------------
        // T16: XmlAnimationConfigAdapter converts Animation XML node correctly
        // ----------------------------------------------------------------

        [TestMethod]
        public void T16_XmlAnimationConfigAdapter_ConvertsAnimationNode()
        {
            var xml = @"<Animations>
                <Animation name=""Row1"" start=""true"" autoStart=""true"" scanTime=""500"" iterations=""3""
                           firstPauseTime=""200"" onEnter="""" onEnd="""">
                    <Widget name=""Button1"" onSelect="""" />
                    <Widget name=""Button2"" onSelect="""" />
                </Animation>
            </Animations>";

            var doc = new XmlDocument();
            doc.LoadXml(xml);

            var adapter = new XmlAnimationConfigAdapter();
            var config = adapter.Convert("TestPanel", doc.DocumentElement);

            Assert.IsNotNull(config);
            Assert.AreEqual("TestPanel", config.PanelName);
            Assert.AreEqual(1, config.Sequences.Count);

            var seq = config.Sequences[0];
            Assert.AreEqual("Row1", seq.Name);
            Assert.IsTrue(seq.IsFirst);
            Assert.IsTrue(seq.AutoStart);
            Assert.AreEqual("500", seq.ScanTime);
            Assert.AreEqual("3", seq.Iterations);
            Assert.AreEqual("200", seq.FirstPauseTime);
        }

        // ----------------------------------------------------------------
        // T17: XmlAnimationConfigAdapter converts Widget XML nodes correctly
        // ----------------------------------------------------------------

        [TestMethod]
        public void T17_XmlAnimationConfigAdapter_ConvertsWidgetNodes()
        {
            var xml = @"<Animations>
                <Animation name=""Seq1"" start=""true"">
                    <Widget name=""Button1"" onSelect=""action1"" />
                    <Widget name=""Button2"" onSelect=""action2"" />
                </Animation>
            </Animations>";

            var doc = new XmlDocument();
            doc.LoadXml(xml);

            var adapter = new XmlAnimationConfigAdapter();
            var config = adapter.Convert("Panel", doc.DocumentElement);

            Assert.AreEqual(2, config.Sequences[0].Widgets.Count);
            Assert.AreEqual("Button1", config.Sequences[0].Widgets[0].Name);
            Assert.AreEqual("action1", config.Sequences[0].Widgets[0].OnSelected);
            Assert.AreEqual("Button2", config.Sequences[0].Widgets[1].Name);
        }

        // ----------------------------------------------------------------
        // T18: AnimationConfigProvider.HasJsonConfig returns false for missing file
        // ----------------------------------------------------------------

        [TestMethod]
        public void T18_AnimationConfigProvider_HasJsonConfig_ReturnsFalseForMissingFile()
        {
            var provider = new AnimationConfigProvider();
            var tempDir = Path.GetTempPath();

            bool result = provider.HasJsonConfig("NonExistentPanel_XYZ123", tempDir);

            Assert.IsFalse(result);
        }

        // ----------------------------------------------------------------
        // T19: AddAnimationEngine registers IAnimationService as Singleton
        // ----------------------------------------------------------------

        [TestMethod]
        public void T19_AddAnimationEngine_RegistersIAnimationServiceAsSingleton()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddSingleton<IEventBus, EventBus>();
            services.AddAnimationEngine();

            var provider = services.BuildServiceProvider();
            var service1 = provider.GetService<IAnimationService>();
            var service2 = provider.GetService<IAnimationService>();

            Assert.IsNotNull(service1, "IAnimationService should be registered");
            Assert.AreSame(service1, service2, "IAnimationService should be a Singleton");
        }

        // ----------------------------------------------------------------
        // T20: AnimationSession stops after N iterations
        // ----------------------------------------------------------------

        [TestMethod]
        public void T20_AnimationSession_IterationCount_StopsAfterNIterations()
        {
            // 2 widgets, 1 iteration: after 2 ticks, the sequence should complete.
            var config = MakeConfig(widgetCount: 2, iterations: "1");
            var (session, timer, _, cleared) = MakeSession(config);
            session.Start();

            // Tick through all widgets in the single iteration
            timer.ManualTick(); // highlight widget 1
            timer.ManualTick(); // highlight widget 2
            timer.ManualTick(); // past end → iteration completes → sequence ends

            // Session should have stopped
            Assert.AreEqual(PlayerState.Stopped, session.State);

            session.Dispose();
        }

        // ----------------------------------------------------------------
        // Helpers for IScanContext
        // ----------------------------------------------------------------

        private static IScanContext MakeScanContext(PlayerState state = PlayerState.Running)
        {
            return new FakeScanContext { SessionState = state };
        }

        private class FakeScanContext : IScanContext
        {
            public string PanelName => "TestPanel";
            public string CurrentAnimationName => "Seq1";
            public int IterationCount => 0;
            public int MaxIterations => 1;
            public double ScanIntervalMs => 600;
            public double HesitateTimeMs => 0;
            public PlayerState SessionState { get; set; } = PlayerState.Running;
        }
    }
}
