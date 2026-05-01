////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// AnimationPerformanceBenchmarks.cs
//
// Performance benchmarks for the new animation engine.
// Validates that the engine meets the targets specified in the design spec §14:
//   - Config load time  ≤20ms for complex panels (25 animations)
//   - Session Start()   ≤5ms
//   - Scan interval deviation  ≤5% at 200ms (validated structurally, not by wall-clock)
//   - AnimationSession.Stop()  clears highlights within 50ms
//
// BP01: AnimationConfig conversion of 5-animation XML completes in ≤20ms
// BP02: AnimationConfig conversion of 25-animation XML completes in ≤20ms
// BP03: AnimationService.CreateSession completes in ≤5ms
// BP04: AnimationSession.Start completes in ≤5ms
// BP05: AnimationSession.Stop completes in ≤50ms
// BP06: AnimationService.Shutdown disposes all sessions within 100ms
// BP07: 100 consecutive TryCreate / Stop / Dispose cycles complete in ≤2s
// BP08: XmlAnimationConfigAdapter parses 25-sequence XML under 20ms
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.AnimationManagement;
using ACAT.Core.AnimationManagement.Configuration;
using ACAT.Core.AnimationManagement.Interfaces;
using ACAT.Core.AnimationManagement.Rendering;
using ACAT.Core.AnimationManagement.Strategies;
using ACAT.Core.EventManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

namespace ACATCore.Tests.Performance
{
    [TestClass]
    public class AnimationPerformanceBenchmarks
    {
        // ----------------------------------------------------------------
        // Helpers
        // ----------------------------------------------------------------

        private static AnimationService MakeService()
        {
            var bus = new EventBus();
            var renderer = new WinFormsHighlightRenderer(
                (name, style) => { },
                name => { },
                () => { });
            return new AnimationService(bus, renderer, new DefaultScanStrategyFactory());
        }

        /// <summary>
        /// Creates an XML <c>&lt;Animations&gt;</c> element with the given number of
        /// <c>&lt;Animation&gt;</c> children, each with <paramref name="widgetsPerSeq"/> widgets.
        /// </summary>
        private static XmlNode MakeAnimationsXml(int sequences, int widgetsPerSeq = 3)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("<Animations>");
            for (int s = 0; s < sequences; s++)
            {
                string start = s == 0 ? "true" : "false";
                sb.AppendLine($@"  <Animation name=""Seq{s + 1}"" start=""{start}"" autoStart=""true"" scanTime=""300"" iterations=""1"">");
                for (int w = 0; w < widgetsPerSeq; w++)
                    sb.AppendLine($@"    <Widget name=""Widget{w + 1}"" onSelect="""" />");
                sb.AppendLine("  </Animation>");
            }
            sb.AppendLine("</Animations>");

            var doc = new XmlDocument();
            doc.LoadXml(sb.ToString());
            return doc.DocumentElement;
        }

        private static AnimationConfig MakeConfig(string panelName, int sequences = 1, int widgetsPerSeq = 3)
        {
            var config = new AnimationConfig
            {
                PanelName = panelName,
                ScanStrategy = "auto",
                Sequences = new List<AnimationSequenceConfig>()
            };
            for (int s = 0; s < sequences; s++)
            {
                var seq = new AnimationSequenceConfig
                {
                    Name = $"Seq{s + 1}",
                    IsFirst = s == 0,
                    Iterations = "1",
                    ScanTime = "200",
                    Widgets = new List<AnimationWidgetConfig>()
                };
                for (int w = 0; w < widgetsPerSeq; w++)
                    seq.Widgets.Add(new AnimationWidgetConfig { Name = $"Widget{w + 1}" });
                config.Sequences.Add(seq);
            }
            return config;
        }

        // ----------------------------------------------------------------
        // BP01: 5-animation XML conversion ≤20ms
        // ----------------------------------------------------------------

        [TestMethod]
        public void BP01_XmlConversion_5Animations_Under20ms()
        {
            var xmlNode = MakeAnimationsXml(sequences: 5, widgetsPerSeq: 3);
            var adapter = new XmlAnimationConfigAdapter();

            // Warm-up
            adapter.Convert("WarmUp", xmlNode);

            var sw = Stopwatch.StartNew();
            var config = adapter.Convert("Panel5Anims", xmlNode);
            sw.Stop();

            Assert.IsNotNull(config);
            Assert.AreEqual(5, config.Sequences.Count);
            Assert.IsTrue(sw.ElapsedMilliseconds <= 20,
                $"5-animation XML conversion took {sw.ElapsedMilliseconds}ms; expected ≤20ms");
        }

        // ----------------------------------------------------------------
        // BP02: 25-animation XML conversion ≤20ms (BCI worst-case)
        // ----------------------------------------------------------------

        [TestMethod]
        public void BP02_XmlConversion_25Animations_Under20ms()
        {
            var xmlNode = MakeAnimationsXml(sequences: 25, widgetsPerSeq: 5);
            var adapter = new XmlAnimationConfigAdapter();

            // Warm-up
            adapter.Convert("WarmUp", xmlNode);

            var sw = Stopwatch.StartNew();
            var config = adapter.Convert("PanelBCIWorstCase", xmlNode);
            sw.Stop();

            Assert.IsNotNull(config);
            Assert.AreEqual(25, config.Sequences.Count);
            Assert.IsTrue(sw.ElapsedMilliseconds <= 20,
                $"25-animation XML conversion took {sw.ElapsedMilliseconds}ms; expected ≤20ms (BCI worst-case target from design spec §14)");
        }

        // ----------------------------------------------------------------
        // BP03: AnimationService.CreateSession ≤5ms
        // ----------------------------------------------------------------

        [TestMethod]
        public void BP03_CreateSession_Under5ms()
        {
            var service = MakeService();
            var config = MakeConfig("BenchPanel");

            // Warm-up
            service.CreateSession(null, config, "auto").Dispose();

            var sw = Stopwatch.StartNew();
            var session = service.CreateSession(null, config, "auto");
            sw.Stop();

            Assert.IsNotNull(session);
            Assert.IsTrue(sw.ElapsedMilliseconds <= 5,
                $"CreateSession took {sw.ElapsedMilliseconds}ms; expected ≤5ms");

            session.Dispose();
            service.Shutdown();
        }

        // ----------------------------------------------------------------
        // BP04: AnimationSession.Start ≤5ms
        // ----------------------------------------------------------------

        [TestMethod]
        public void BP04_SessionStart_Under5ms()
        {
            var service = MakeService();
            var config = MakeConfig("BenchPanel");
            var session = service.CreateSession(null, config, "auto");

            var sw = Stopwatch.StartNew();
            session.Start();
            sw.Stop();

            Assert.AreEqual(PlayerState.Running, session.State);
            Assert.IsTrue(sw.ElapsedMilliseconds <= 5,
                $"Session.Start() took {sw.ElapsedMilliseconds}ms; expected ≤5ms");

            session.Stop();
            session.Dispose();
            service.Shutdown();
        }

        // ----------------------------------------------------------------
        // BP05: AnimationSession.Stop ≤50ms
        // ----------------------------------------------------------------

        [TestMethod]
        public void BP05_SessionStop_Under50ms()
        {
            var service = MakeService();
            var config = MakeConfig("BenchPanel");
            var session = service.CreateSession(null, config, "auto");
            session.Start();

            var sw = Stopwatch.StartNew();
            session.Stop();
            sw.Stop();

            Assert.AreEqual(PlayerState.Stopped, session.State);
            Assert.IsTrue(sw.ElapsedMilliseconds <= 50,
                $"Session.Stop() took {sw.ElapsedMilliseconds}ms; expected ≤50ms");

            session.Dispose();
            service.Shutdown();
        }

        // ----------------------------------------------------------------
        // BP06: AnimationService.Shutdown disposes all sessions within 100ms
        // ----------------------------------------------------------------

        [TestMethod]
        public void BP06_ServiceShutdown_Under100ms()
        {
            var service = MakeService();
            var sessions = new List<IAnimationSession>();

            // Create 10 sessions
            for (int i = 0; i < 10; i++)
            {
                var config = MakeConfig($"Panel{i}");
                var s = service.CreateSession(null, config, "auto");
                s.Start();
                sessions.Add(s);
            }

            var sw = Stopwatch.StartNew();
            service.Shutdown();
            sw.Stop();

            Assert.IsTrue(sw.ElapsedMilliseconds <= 100,
                $"Service.Shutdown() with 10 sessions took {sw.ElapsedMilliseconds}ms; expected ≤100ms");
        }

        // ----------------------------------------------------------------
        // BP07: 100 TryCreate / Stop / Dispose cycles complete in ≤2s
        // ----------------------------------------------------------------

        [TestMethod]
        public void BP07_AdapterLifecycle_100Cycles_Under2s()
        {
            var service = MakeService();
            var xmlNode = MakeAnimationsXml(sequences: 3, widgetsPerSeq: 3);

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 100; i++)
            {
                var adapter = AnimationPlayerAdapter.TryCreate(
                    $"Panel_{i}", xmlNode, service, null, null);

                Assert.IsNotNull(adapter, $"TryCreate failed on iteration {i}");

                adapter.Start();
                adapter.Stop();
                adapter.Dispose();
            }
            sw.Stop();

            Assert.IsTrue(sw.ElapsedMilliseconds <= 2000,
                $"100 adapter cycles took {sw.ElapsedMilliseconds}ms; expected ≤2000ms");

            service.Shutdown();
        }

        // ----------------------------------------------------------------
        // BP08: XmlAnimationConfigAdapter parses 25-sequence XML under 20ms
        // ----------------------------------------------------------------

        [TestMethod]
        public void BP08_XmlAdapter_25Sequences_RepeatedParsing_Under20msEach()
        {
            var xmlNode = MakeAnimationsXml(sequences: 25, widgetsPerSeq: 5);
            var adapter = new XmlAnimationConfigAdapter();

            const int iterations = 10;
            long maxMs = 0;
            long totalMs = 0;

            for (int i = 0; i < iterations; i++)
            {
                var sw = Stopwatch.StartNew();
                var config = adapter.Convert($"Panel_{i}", xmlNode);
                sw.Stop();

                Assert.IsNotNull(config);
                Assert.AreEqual(25, config.Sequences.Count);

                maxMs = Math.Max(maxMs, sw.ElapsedMilliseconds);
                totalMs += sw.ElapsedMilliseconds;
            }

            long avgMs = totalMs / iterations;
            Assert.IsTrue(maxMs <= 20,
                $"Max parse time over {iterations} iterations was {maxMs}ms; expected ≤20ms");
        }
    }
}
