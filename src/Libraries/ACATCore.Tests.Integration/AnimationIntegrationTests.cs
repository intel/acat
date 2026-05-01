////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// AnimationIntegrationTests.cs
//
// Integration tests for the animation engine adapter layer.
// Validates that AnimationPlayerAdapter correctly bridges the new
// IAnimationService / IAnimationSession engine to the existing
// PanelAnimationManager and UserControlAnimationManager callers.
//
// Test scenarios:
//   IT01: AnimationPlayerAdapter.TryCreate returns null when IAnimationService is null
//   IT02: AnimationPlayerAdapter.TryCreate succeeds with valid service and XML
//   IT03: AnimationPlayerAdapter.TryCreate succeeds with null animationsNode (no XML)
//   IT04: AnimationPlayerAdapter.Start transitions session to Running
//   IT05: AnimationPlayerAdapter.Stop transitions session to Stopped
//   IT06: AnimationPlayerAdapter.Pause and Resume preserve state correctly
//   IT07: AnimationPlayerAdapter.Interrupt does not throw
//   IT08: AnimationPlayerAdapter.Transition moves to named sequence
//   IT09: AnimationPlayerAdapter.Dispose releases the session
//   IT10: AnimationPlayerAdapter.TryCreate falls back when session creation throws
//   IT11: XmlAnimationConfigAdapter round-trip with multi-sequence XML
//   IT12: AnimationService.CreateSession with XmlAnimationConfigAdapter config
//   IT13: AnimationPlayerAdapter.Start with named animation
//   IT14: Multiple sessions can coexist (multi-panel scenario)
//   IT15: AnimationSession publishes AnimationStateChangedEvent on Start
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
using System.Xml;

namespace ACATCore.Tests.Integration
{
    [TestClass]
    public class AnimationIntegrationTests
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
                    AutoStart = true,
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

        private static XmlNode MakeAnimationsXml(int sequences = 1, int widgetsPerSeq = 2)
        {
            var xmlBuilder = new System.Text.StringBuilder();
            xmlBuilder.AppendLine("<Animations>");
            for (int s = 0; s < sequences; s++)
            {
                string start = s == 0 ? "true" : "false";
                xmlBuilder.AppendLine($@"  <Animation name=""Seq{s + 1}"" start=""{start}"" autoStart=""true"" scanTime=""300"" iterations=""1"">");
                for (int w = 0; w < widgetsPerSeq; w++)
                    xmlBuilder.AppendLine($@"    <Widget name=""Button{w + 1}"" onSelect="""" />");
                xmlBuilder.AppendLine("  </Animation>");
            }
            xmlBuilder.AppendLine("</Animations>");

            var doc = new XmlDocument();
            doc.LoadXml(xmlBuilder.ToString());
            return doc.DocumentElement;
        }

        // ----------------------------------------------------------------
        // IT01: TryCreate returns null when IAnimationService is null
        // ----------------------------------------------------------------

        [TestMethod]
        public void IT01_TryCreate_ReturnsNull_WhenServiceIsNull()
        {
            var adapter = AnimationPlayerAdapter.TryCreate(
                panelName: "TestPanel",
                animationsNode: MakeAnimationsXml(),
                animationService: null,
                eventBus: null,
                rootWidget: null);

            Assert.IsNull(adapter, "TryCreate should return null when service is null");
        }

        // ----------------------------------------------------------------
        // IT02: TryCreate succeeds with valid service and XML
        // ----------------------------------------------------------------

        [TestMethod]
        public void IT02_TryCreate_Succeeds_WithValidServiceAndXml()
        {
            var service = MakeService();
            var xmlNode = MakeAnimationsXml();

            using var adapter = AnimationPlayerAdapter.TryCreate(
                panelName: "TestPanel",
                animationsNode: xmlNode,
                animationService: service,
                eventBus: null,
                rootWidget: null);

            Assert.IsNotNull(adapter, "TryCreate should succeed with valid service and XML");
            Assert.AreEqual("TestPanel", adapter.PanelName);
        }

        // ----------------------------------------------------------------
        // IT03: TryCreate succeeds with null animationsNode (empty config)
        // ----------------------------------------------------------------

        [TestMethod]
        public void IT03_TryCreate_Succeeds_WithNullXmlNode()
        {
            var service = MakeService();

            using var adapter = AnimationPlayerAdapter.TryCreate(
                panelName: "EmptyPanel",
                animationsNode: null,
                animationService: service,
                eventBus: null,
                rootWidget: null);

            Assert.IsNotNull(adapter, "TryCreate should succeed even with null animationsNode");
            Assert.AreEqual("EmptyPanel", adapter.PanelName);
        }

        // ----------------------------------------------------------------
        // IT04: Start transitions session to Running
        // ----------------------------------------------------------------

        [TestMethod]
        public void IT04_Start_TransitionsSessionToRunning()
        {
            var service = MakeService();
            using var adapter = AnimationPlayerAdapter.TryCreate(
                "ScanPanel", MakeAnimationsXml(), service, null, null);

            Assert.IsNotNull(adapter);
            adapter.Start();

            Assert.AreEqual(PlayerState.Running, adapter.State);

            adapter.Stop();
        }

        // ----------------------------------------------------------------
        // IT05: Stop transitions session to Stopped
        // ----------------------------------------------------------------

        [TestMethod]
        public void IT05_Stop_TransitionsSessionToStopped()
        {
            var service = MakeService();
            using var adapter = AnimationPlayerAdapter.TryCreate(
                "ScanPanel", MakeAnimationsXml(), service, null, null);

            Assert.IsNotNull(adapter);
            adapter.Start();
            adapter.Stop();

            Assert.AreEqual(PlayerState.Stopped, adapter.State);
        }

        // ----------------------------------------------------------------
        // IT06: Pause and Resume preserve state
        // ----------------------------------------------------------------

        [TestMethod]
        public void IT06_PauseResume_PreservesState()
        {
            var service = MakeService();
            using var adapter = AnimationPlayerAdapter.TryCreate(
                "ScanPanel", MakeAnimationsXml(), service, null, null);

            Assert.IsNotNull(adapter);
            adapter.Start();

            adapter.Pause();
            Assert.AreEqual(PlayerState.Paused, adapter.State);

            adapter.Resume();
            Assert.AreEqual(PlayerState.Running, adapter.State);

            adapter.Stop();
        }

        // ----------------------------------------------------------------
        // IT07: Interrupt does not throw
        // ----------------------------------------------------------------

        [TestMethod]
        public void IT07_Interrupt_DoesNotThrow()
        {
            var service = MakeService();
            using var adapter = AnimationPlayerAdapter.TryCreate(
                "ScanPanel", MakeAnimationsXml(), service, null, null);

            Assert.IsNotNull(adapter);
            adapter.Start();

            // Should not throw
            adapter.Interrupt();

            adapter.Stop();
        }

        // ----------------------------------------------------------------
        // IT08: Transition moves to named sequence
        // ----------------------------------------------------------------

        [TestMethod]
        public void IT08_Transition_MovesToNamedSequence()
        {
            var service = MakeService();
            var xmlNode = MakeAnimationsXml(sequences: 2);

            using var adapter = AnimationPlayerAdapter.TryCreate(
                "MultiSeqPanel", xmlNode, service, null, null);

            Assert.IsNotNull(adapter);
            adapter.Start();

            adapter.Transition("Seq2");

            Assert.AreEqual("Seq2", adapter.CurrentAnimationName);

            adapter.Stop();
        }

        // ----------------------------------------------------------------
        // IT09: Dispose releases the session
        // ----------------------------------------------------------------

        [TestMethod]
        public void IT09_Dispose_ReleasesSession()
        {
            var service = MakeService();
            var adapter = AnimationPlayerAdapter.TryCreate(
                "ScanPanel", MakeAnimationsXml(), service, null, null);

            Assert.IsNotNull(adapter);
            adapter.Start();

            adapter.Dispose();

            // Operations after Dispose should throw ObjectDisposedException
            Assert.ThrowsException<ObjectDisposedException>(() => adapter.Start());
        }

        // ----------------------------------------------------------------
        // IT10: TryCreate falls back gracefully when session creation throws
        // ----------------------------------------------------------------

        [TestMethod]
        public void IT10_TryCreate_ReturnsNull_WhenSessionCreationFails()
        {
            // Use a service with no renderer — CreateSession throws InvalidOperationException
            var bus = new EventBus();
            var serviceWithoutRenderer = new AnimationService(bus, renderer: null, new DefaultScanStrategyFactory());

            var adapter = AnimationPlayerAdapter.TryCreate(
                panelName: "TestPanel",
                animationsNode: MakeAnimationsXml(),
                animationService: serviceWithoutRenderer,
                eventBus: null,
                rootWidget: null);

            // TryCreate should catch the exception and return null
            Assert.IsNull(adapter, "TryCreate should return null when session creation fails");
        }

        // ----------------------------------------------------------------
        // IT11: XmlAnimationConfigAdapter round-trip with multi-sequence XML
        // ----------------------------------------------------------------

        [TestMethod]
        public void IT11_XmlAnimationConfigAdapter_RoundTrip_MultiSequence()
        {
            var xmlNode = MakeAnimationsXml(sequences: 3, widgetsPerSeq: 4);
            var adapter = new XmlAnimationConfigAdapter();

            var config = adapter.Convert("RoundTripPanel", xmlNode);

            Assert.IsNotNull(config);
            Assert.AreEqual("RoundTripPanel", config.PanelName);
            Assert.AreEqual(3, config.Sequences.Count);

            // First sequence should be marked as IsFirst
            Assert.IsTrue(config.Sequences[0].IsFirst);
            Assert.IsFalse(config.Sequences[1].IsFirst);
            Assert.IsFalse(config.Sequences[2].IsFirst);

            // Each sequence should have 4 widgets
            foreach (var seq in config.Sequences)
                Assert.AreEqual(4, seq.Widgets.Count);
        }

        // ----------------------------------------------------------------
        // IT12: AnimationService.CreateSession with XmlAnimationConfigAdapter config
        // ----------------------------------------------------------------

        [TestMethod]
        public void IT12_CreateSession_WithXmlAdaptedConfig_Succeeds()
        {
            var service = MakeService();
            var xmlNode = MakeAnimationsXml(sequences: 2, widgetsPerSeq: 3);
            var xmlAdapter = new XmlAnimationConfigAdapter();
            var config = xmlAdapter.Convert("XmlAdaptedPanel", xmlNode);

            using var session = service.CreateSession(rootWidget: null, config: config, strategyName: "auto");

            Assert.IsNotNull(session);
            Assert.AreEqual("XmlAdaptedPanel", session.PanelName);

            session.Start();
            Assert.AreEqual(PlayerState.Running, session.State);
            session.Stop();
        }

        // ----------------------------------------------------------------
        // IT13: AnimationPlayerAdapter.Start with named animation
        // ----------------------------------------------------------------

        [TestMethod]
        public void IT13_Start_WithNamedAnimation_StartsCorrectSequence()
        {
            var service = MakeService();
            var xmlNode = MakeAnimationsXml(sequences: 2);

            using var adapter = AnimationPlayerAdapter.TryCreate(
                "NamedStartPanel", xmlNode, service, null, null);

            Assert.IsNotNull(adapter);
            adapter.Start("Seq2");

            Assert.AreEqual("Seq2", adapter.CurrentAnimationName);

            adapter.Stop();
        }

        // ----------------------------------------------------------------
        // IT14: Multiple sessions can coexist (multi-panel scenario)
        // ----------------------------------------------------------------

        [TestMethod]
        public void IT14_MultipleSessions_Coexist()
        {
            var service = MakeService();

            using var adapter1 = AnimationPlayerAdapter.TryCreate(
                "Panel1", MakeAnimationsXml(), service, null, null);
            using var adapter2 = AnimationPlayerAdapter.TryCreate(
                "Panel2", MakeAnimationsXml(), service, null, null);

            Assert.IsNotNull(adapter1);
            Assert.IsNotNull(adapter2);

            adapter1.Start();
            adapter2.Start();

            Assert.AreEqual(PlayerState.Running, adapter1.State);
            Assert.AreEqual(PlayerState.Running, adapter2.State);
            Assert.AreEqual("Panel1", adapter1.PanelName);
            Assert.AreEqual("Panel2", adapter2.PanelName);

            adapter1.Stop();
            adapter2.Stop();
        }

        // ----------------------------------------------------------------
        // IT15: AnimationSession publishes AnimationStateChangedEvent on Start
        // ----------------------------------------------------------------

        [TestMethod]
        public void IT15_AnimationSession_PublishesStateChangedEvent_OnStart()
        {
            var bus = new EventBus();
            var renderer = new WinFormsHighlightRenderer(
                (name, style) => { },
                name => { },
                () => { });
            var service = new AnimationService(bus, renderer, new DefaultScanStrategyFactory());

            var receivedEvents = new List<AnimationStateChangedEvent>();
            bus.Subscribe<AnimationStateChangedEvent>(e => receivedEvents.Add(e));

            var xmlAdapter = new XmlAnimationConfigAdapter();
            var config = xmlAdapter.Convert("EventPanel", MakeAnimationsXml());
            using var session = service.CreateSession(null, config, "auto");

            session.Start();

            // Allow event to propagate
            System.Threading.Thread.Sleep(50);

            Assert.IsTrue(receivedEvents.Count > 0,
                "AnimationStateChangedEvent should be published when session starts");
            Assert.AreEqual("EventPanel", receivedEvents[0].PanelName);
            Assert.AreEqual(PlayerState.Running, receivedEvents[0].NewState);

            session.Stop();
            session.Dispose();
            service.Shutdown();
        }
    }
}
