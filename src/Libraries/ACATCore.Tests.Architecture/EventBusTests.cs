////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// EventBusTests.cs
//
// Unit tests for the EventBus publish/subscribe system.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.EventManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace ACATCore.Tests.Architecture
{
    /// <summary>
    /// Unit tests for <see cref="EventBus"/> using the handler-reference subscribe/unsubscribe API.
    /// </summary>
    [TestClass]
    public class EventBusTests
    {
        // -----------------------------------------------------------------------
        // Subscribe / Publish
        // -----------------------------------------------------------------------

        [TestMethod]
        public void Publish_WithSubscriber_InvokesHandler()
        {
            var bus = new EventBus();
            string received = null;

            bus.Subscribe<PanelShowEvent>(e => received = e.PanelClass);
            bus.Publish(new PanelShowEvent("MainPanel"));

            Assert.AreEqual("MainPanel", received);
        }

        [TestMethod]
        public void Publish_WithNoSubscribers_DoesNotThrow()
        {
            var bus = new EventBus();
            bus.Publish(new PanelShowEvent("X"));
            // passes if no exception is thrown
        }

        [TestMethod]
        public void Publish_WithMultipleSubscribers_AllReceiveEvent()
        {
            var bus = new EventBus();
            var names = new List<string>();

            bus.Subscribe<PanelHideEvent>(e => names.Add("A:" + e.PanelClass));
            bus.Subscribe<PanelHideEvent>(e => names.Add("B:" + e.PanelClass));
            bus.Publish(new PanelHideEvent("ContextPanel"));

            CollectionAssert.Contains(names, "A:ContextPanel");
            CollectionAssert.Contains(names, "B:ContextPanel");
        }

        [TestMethod]
        public void Subscribe_DifferentEventTypes_OnlyCorrectHandlerCalled()
        {
            var bus = new EventBus();
            bool panelReceived = false;
            bool actuatorReceived = false;

            bus.Subscribe<PanelShowEvent>(_ => panelReceived = true);
            bus.Subscribe<ActuatorSwitchActivatedEvent>(_ => actuatorReceived = true);

            bus.Publish(new PanelShowEvent("P"));

            Assert.IsTrue(panelReceived);
            Assert.IsFalse(actuatorReceived);
        }

        // -----------------------------------------------------------------------
        // Unsubscribe
        // -----------------------------------------------------------------------

        [TestMethod]
        public void Unsubscribe_ByHandler_HandlerNoLongerCalled()
        {
            var bus = new EventBus();
            int count = 0;
            Action<PanelShowEvent> handler = _ => count++;

            bus.Subscribe<PanelShowEvent>(handler);
            bus.Publish(new PanelShowEvent("A"));
            bus.Unsubscribe<PanelShowEvent>(handler);
            bus.Publish(new PanelShowEvent("B"));

            Assert.AreEqual(1, count);
        }

        // -----------------------------------------------------------------------
        // Built-in event types
        // -----------------------------------------------------------------------

        [TestMethod]
        public void PanelShowEvent_PanelClass_SetCorrectly()
        {
            var evt = new PanelShowEvent("AlphabetScanner");
            Assert.AreEqual("AlphabetScanner", evt.PanelClass);
        }

        [TestMethod]
        public void PanelHideEvent_PanelClass_SetCorrectly()
        {
            var evt = new PanelHideEvent("ContextPanel");
            Assert.AreEqual("ContextPanel", evt.PanelClass);
        }

        [TestMethod]
        public void ActuatorSwitchActivatedEvent_SwitchName_SetCorrectly()
        {
            var evt = new ActuatorSwitchActivatedEvent("LeftSwitch");
            Assert.AreEqual("LeftSwitch", evt.SwitchName);
        }

        [TestMethod]
        public void ConfigurationReloadEvent_ConfigPath_SetCorrectly()
        {
            var evt = new ConfigurationReloadEvent("/path/to/config.xml");
            Assert.AreEqual("/path/to/config.xml", evt.ConfigPath);
        }

        [TestMethod]
        public void AgentContextChangedEvent_Properties_SetCorrectly()
        {
            var ctx = new object();
            var evt = new AgentContextChangedEvent("NotepadAgent", ctx);
            Assert.AreEqual("NotepadAgent", evt.AgentName);
            Assert.AreSame(ctx, evt.Context);
        }

        // -----------------------------------------------------------------------
        // Null guards
        // -----------------------------------------------------------------------

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Publish_NullEventData_ThrowsArgumentNullException()
        {
            var bus = new EventBus();
            bus.Publish<PanelShowEvent>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Subscribe_NullHandler_ThrowsArgumentNullException()
        {
            var bus = new EventBus();
            bus.Subscribe<PanelShowEvent>(null);
        }
    }
}
