////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// EventBusTests.cs
//
// Unit tests for EventBus and related event types.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.EventManagement;
using System;
using System.Collections.Generic;
using Xunit;

namespace ACATCore.Tests
{
    /// <summary>
    /// Unit tests for <see cref="EventBus"/>, <see cref="IEventBus"/>,
    /// and the domain event types.
    /// </summary>
    public class EventBusTests
    {
        // ----------------------------------------------------------------
        // Subscribe / Publish
        // ----------------------------------------------------------------

        [Fact]
        public void Publish_WithSingleSubscriber_InvokesHandler()
        {
            var bus = new EventBus();
            PanelShowEvent received = null;

            bus.Subscribe<PanelShowEvent>(e => received = e);
            bus.Publish(new PanelShowEvent("TestPanel"));

            Assert.NotNull(received);
            Assert.Equal("TestPanel", received.PanelClass);
        }

        [Fact]
        public void Publish_WithMultipleSubscribers_InvokesAllHandlers()
        {
            var bus = new EventBus();
            var results = new List<string>();

            bus.Subscribe<PanelShowEvent>(e => results.Add("handler1:" + e.PanelClass));
            bus.Subscribe<PanelShowEvent>(e => results.Add("handler2:" + e.PanelClass));
            bus.Publish(new PanelShowEvent("Main"));

            Assert.Equal(2, results.Count);
            Assert.Contains("handler1:Main", results);
            Assert.Contains("handler2:Main", results);
        }

        [Fact]
        public void Publish_WithNoSubscribers_DoesNotThrow()
        {
            var bus = new EventBus();
            // Should complete without exception
            bus.Publish(new PanelHideEvent("SomePanel"));
        }

        [Fact]
        public void Publish_DifferentEventTypes_OnlyNotifiesCorrectSubscribers()
        {
            var bus = new EventBus();
            bool showReceived = false;
            bool hideReceived = false;

            bus.Subscribe<PanelShowEvent>(_ => showReceived = true);
            bus.Subscribe<PanelHideEvent>(_ => hideReceived = true);

            bus.Publish(new PanelShowEvent("Panel"));

            Assert.True(showReceived);
            Assert.False(hideReceived);
        }

        // ----------------------------------------------------------------
        // Unsubscribe
        // ----------------------------------------------------------------

        [Fact]
        public void Unsubscribe_RemovesHandler_HandlerNotCalledAfterUnsubscribe()
        {
            var bus = new EventBus();
            int callCount = 0;
            Action<PanelShowEvent> handler = _ => callCount++;

            bus.Subscribe(handler);
            bus.Publish(new PanelShowEvent("A"));
            bus.Unsubscribe(handler);
            bus.Publish(new PanelShowEvent("B"));

            Assert.Equal(1, callCount);
        }

        [Fact]
        public void Unsubscribe_NonRegisteredHandler_DoesNotThrow()
        {
            var bus = new EventBus();
            // Should complete without exception
            bus.Unsubscribe<PanelShowEvent>(_ => { });
        }

        // ----------------------------------------------------------------
        // Null argument guards
        // ----------------------------------------------------------------

        [Fact]
        public void Subscribe_NullHandler_ThrowsArgumentNullException()
        {
            var bus = new EventBus();
            Assert.Throws<ArgumentNullException>(() =>
                bus.Subscribe<PanelShowEvent>(null));
        }

        [Fact]
        public void Unsubscribe_NullHandler_ThrowsArgumentNullException()
        {
            var bus = new EventBus();
            Assert.Throws<ArgumentNullException>(() =>
                bus.Unsubscribe<PanelShowEvent>(null));
        }

        [Fact]
        public void Publish_NullEvent_ThrowsArgumentNullException()
        {
            var bus = new EventBus();
            Assert.Throws<ArgumentNullException>(() =>
                bus.Publish<PanelShowEvent>(null));
        }

        // ----------------------------------------------------------------
        // Weak reference support
        // ----------------------------------------------------------------

        [Fact]
        public void WeakReference_DeadSubscriber_IsNotInvokedAndIsPruned()
        {
            var bus = new EventBus();
            int callCount = 0;

            // Subscribe from a short-lived object that will be collected.
            SubscribeFromShortLivedObject(bus, () => callCount++);

            // Force garbage collection so the subscriber is collected.
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            // Publishing after the subscriber is gone should not call the handler.
            bus.Publish(new PanelShowEvent("AfterGC"));

            Assert.Equal(0, callCount);
        }

        // ----------------------------------------------------------------
        // Static handler support
        // ----------------------------------------------------------------

        [Fact]
        public void Subscribe_StaticHandler_IsInvokedCorrectly()
        {
            var bus = new EventBus();
            _staticCallCount = 0;

            bus.Subscribe<PanelShowEvent>(StaticHandler);
            bus.Publish(new PanelShowEvent("Static"));

            Assert.Equal(1, _staticCallCount);
        }

        private static int _staticCallCount;
        private static void StaticHandler(PanelShowEvent e) => _staticCallCount++;

        // ----------------------------------------------------------------
        // EventBase timestamp
        // ----------------------------------------------------------------

        [Fact]
        public void EventBase_Timestamp_IsSetOnCreation()
        {
            DateTime before = DateTime.UtcNow;
            var ev = new PanelShowEvent("T");
            DateTime after = DateTime.UtcNow;

            Assert.InRange(ev.Timestamp, before, after);
        }

        // ----------------------------------------------------------------
        // Domain event property tests
        // ----------------------------------------------------------------

        [Fact]
        public void PanelShowEvent_PanelClass_IsPreserved()
        {
            var e = new PanelShowEvent("Alpha");
            Assert.Equal("Alpha", e.PanelClass);
        }

        [Fact]
        public void PanelHideEvent_PanelClass_IsPreserved()
        {
            var e = new PanelHideEvent("Beta");
            Assert.Equal("Beta", e.PanelClass);
        }

        [Fact]
        public void PanelActivateEvent_PanelClass_IsPreserved()
        {
            var e = new PanelActivateEvent("Gamma");
            Assert.Equal("Gamma", e.PanelClass);
        }

        [Fact]
        public void AgentContextChangedEvent_Properties_ArePreserved()
        {
            var ctx = new object();
            var e = new AgentContextChangedEvent("MyAgent", ctx);

            Assert.Equal("MyAgent", e.AgentName);
            Assert.Same(ctx, e.Context);
        }

        [Fact]
        public void ActuatorSwitchActivatedEvent_SwitchName_IsPreserved()
        {
            var e = new ActuatorSwitchActivatedEvent("Switch1");
            Assert.Equal("Switch1", e.SwitchName);
        }

        [Fact]
        public void ConfigurationReloadEvent_ConfigPath_IsPreserved()
        {
            var e = new ConfigurationReloadEvent(@"C:\config\acat.json");
            Assert.Equal(@"C:\config\acat.json", e.ConfigPath);
        }

        [Fact]
        public void ConfigurationChangedEvent_Properties_ArePreserved()
        {
            var e = new ConfigurationChangedEvent("theme", "Dark");
            Assert.Equal("theme", e.Key);
            Assert.Equal("Dark", e.NewValue);
        }

        // ----------------------------------------------------------------
        // Helpers
        // ----------------------------------------------------------------

        /// <summary>
        /// Creates a short-lived subscriber object in its own stack frame so
        /// the GC can collect it once this method returns.
        /// </summary>
        private static void SubscribeFromShortLivedObject(IEventBus bus, Action onEvent)
        {
            var subscriber = new ShortLivedSubscriber(onEvent);
            bus.Subscribe<PanelShowEvent>(subscriber.Handle);
            // subscriber goes out of scope when this method returns.
        }

        private class ShortLivedSubscriber
        {
            private readonly Action _onEvent;
            public ShortLivedSubscriber(Action onEvent) => _onEvent = onEvent;
            public void Handle(PanelShowEvent e) => _onEvent();
        }
    }
}
