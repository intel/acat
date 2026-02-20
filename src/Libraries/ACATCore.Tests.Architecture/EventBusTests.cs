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
    /// Unit tests for <see cref="EventBus"/>.
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

            bus.Subscribe<PanelShownEvent>(e => received = e.PanelName);
            bus.Publish(new PanelShownEvent("MainPanel"));

            Assert.AreEqual("MainPanel", received);
        }

        [TestMethod]
        public void Publish_WithNoSubscribers_DoesNotThrow()
        {
            var bus = new EventBus();
            bus.Publish(new PanelShownEvent("X"));
            // No assertion needed – test passes if no exception is thrown
        }

        [TestMethod]
        public void Publish_WithMultipleSubscribers_AllReceiveEvent()
        {
            var bus = new EventBus();
            var names = new List<string>();

            bus.Subscribe<PanelHiddenEvent>(e => names.Add("A:" + e.PanelName));
            bus.Subscribe<PanelHiddenEvent>(e => names.Add("B:" + e.PanelName));
            bus.Publish(new PanelHiddenEvent("ContextPanel"));

            CollectionAssert.Contains(names, "A:ContextPanel");
            CollectionAssert.Contains(names, "B:ContextPanel");
        }

        [TestMethod]
        public void Subscribe_DifferentEventTypes_OnlyCorrectHandlerCalled()
        {
            var bus = new EventBus();
            bool panelReceived = false;
            bool actuatorReceived = false;

            bus.Subscribe<PanelShownEvent>(_ => panelReceived = true);
            bus.Subscribe<ActuatorSwitchEvent>(_ => actuatorReceived = true);

            bus.Publish(new PanelShownEvent("P"));

            Assert.IsTrue(panelReceived);
            Assert.IsFalse(actuatorReceived);
        }

        // -----------------------------------------------------------------------
        // Unsubscribe / Token
        // -----------------------------------------------------------------------

        [TestMethod]
        public void Unsubscribe_ViaToken_HandlerNoLongerCalled()
        {
            var bus = new EventBus();
            int count = 0;

            var token = bus.Subscribe<PanelShownEvent>(_ => count++);
            bus.Publish(new PanelShownEvent("A"));
            bus.Unsubscribe<PanelShownEvent>(token);
            bus.Publish(new PanelShownEvent("B"));

            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void SubscriptionToken_Dispose_RemovesSubscription()
        {
            var bus = new EventBus();
            int count = 0;

            using (bus.Subscribe<ActuatorSwitchEvent>(_ => count++))
            {
                bus.Publish(new ActuatorSwitchEvent("Switch1"));
            }
            bus.Publish(new ActuatorSwitchEvent("Switch2"));

            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void SubscriptionToken_HasUniqueId()
        {
            var bus = new EventBus();
            var t1 = bus.Subscribe<PanelShownEvent>(_ => { });
            var t2 = bus.Subscribe<PanelShownEvent>(_ => { });

            Assert.AreNotEqual(t1.Id, t2.Id);

            t1.Dispose();
            t2.Dispose();
        }

        // -----------------------------------------------------------------------
        // EventBase timestamp
        // -----------------------------------------------------------------------

        [TestMethod]
        public void EventBase_Timestamp_IsUtcAndRecent()
        {
            var before = DateTime.UtcNow.AddSeconds(-1);
            var evt = new PanelShownEvent("X");
            var after = DateTime.UtcNow.AddSeconds(1);

            Assert.IsTrue(evt.Timestamp >= before);
            Assert.IsTrue(evt.Timestamp <= after);
        }

        // -----------------------------------------------------------------------
        // Built-in event types
        // -----------------------------------------------------------------------

        [TestMethod]
        public void ConfigurationReloadedEvent_Properties_SetCorrectly()
        {
            var evt = new ConfigurationReloadedEvent("/path/to/config.xml", true);
            Assert.AreEqual("/path/to/config.xml", evt.FilePath);
            Assert.IsTrue(evt.Success);
        }

        [TestMethod]
        public void AgentContextChangedEvent_ContextName_SetCorrectly()
        {
            var evt = new AgentContextChangedEvent("TextEditorContext");
            Assert.AreEqual("TextEditorContext", evt.ContextName);
        }

        [TestMethod]
        public void ActuatorSwitchEvent_Properties_SetCorrectly()
        {
            var evt = new ActuatorSwitchEvent("LeftSwitch", "extra-data");
            Assert.AreEqual("LeftSwitch", evt.SwitchName);
            Assert.AreEqual("extra-data", evt.SwitchData);
        }

        // -----------------------------------------------------------------------
        // Null guards
        // -----------------------------------------------------------------------

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Publish_NullEventData_ThrowsArgumentNullException()
        {
            var bus = new EventBus();
            bus.Publish<PanelShownEvent>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Subscribe_NullHandler_ThrowsArgumentNullException()
        {
            var bus = new EventBus();
            bus.Subscribe<PanelShownEvent>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Unsubscribe_NullToken_ThrowsArgumentNullException()
        {
            var bus = new EventBus();
            bus.Unsubscribe<PanelShownEvent>(null);
        }

        // -----------------------------------------------------------------------
        // Dispose
        // -----------------------------------------------------------------------

        [TestMethod]
        public void Dispose_ClearsAllSubscriptions_NoHandlersCalled()
        {
            var bus = new EventBus();
            int count = 0;
            bus.Subscribe<PanelShownEvent>(_ => count++);
            bus.Dispose();
            bus.Publish(new PanelShownEvent("X"));
            Assert.AreEqual(0, count);
        }
    }
}
