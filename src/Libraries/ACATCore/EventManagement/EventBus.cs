////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// EventBus.cs
//
// Thread-safe publish/subscribe event bus with weak-reference support.
//
////////////////////////////////////////////////////////////////////////////

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace ACAT.Core.EventManagement
{
    /// <summary>
    /// Thread-safe publish/subscribe event bus.
    /// Subscriptions are stored as weak references so that subscribers that are
    /// garbage-collected are automatically removed, preventing memory leaks.
    /// </summary>
    public class EventBus : IEventBus, IDisposable
    {
        private readonly ILogger<EventBus> _logger;
        private readonly Dictionary<Type, List<WeakSubscription>> _subscriptions
            = new Dictionary<Type, List<WeakSubscription>>();
        private readonly object _lock = new object();
        private bool _disposed;

        /// <summary>
        /// Initializes a new <see cref="EventBus"/> instance.
        /// </summary>
        /// <param name="logger">Logger for diagnostic output.</param>
        public EventBus(ILogger<EventBus> logger = null)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public void Publish<TEvent>(TEvent eventData) where TEvent : IEvent
        {
            if (eventData == null) throw new ArgumentNullException(nameof(eventData));

            List<WeakSubscription> snapshot;

            lock (_lock)
            {
                if (!_subscriptions.TryGetValue(typeof(TEvent), out var subs))
                    return;

                snapshot = new List<WeakSubscription>(subs);
            }

            var dead = new List<WeakSubscription>();
            int invoked = 0;

            foreach (var sub in snapshot)
            {
                try
                {
                    if (sub.TryInvoke(eventData))
                        invoked++;
                    else
                        dead.Add(sub);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Unhandled exception in event handler for {EventType}", typeof(TEvent).Name);
                }
            }

            if (dead.Count > 0)
            {
                lock (_lock)
                {
                    if (_subscriptions.TryGetValue(typeof(TEvent), out var subs))
                        foreach (var d in dead)
                            subs.Remove(d);
                }
            }

            _logger?.LogDebug("Published {EventType} to {Count} subscriber(s)", typeof(TEvent).Name, invoked);
        }

        /// <inheritdoc />
        public ISubscriptionToken Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            var token = new SubscriptionToken<TEvent>(this);
            var sub = new WeakSubscription<TEvent>(token.Id, handler);

            lock (_lock)
            {
                if (!_subscriptions.TryGetValue(typeof(TEvent), out var subs))
                {
                    subs = new List<WeakSubscription>();
                    _subscriptions[typeof(TEvent)] = subs;
                }
                subs.Add(sub);
            }

            _logger?.LogDebug("Subscribed to {EventType} (token {TokenId})", typeof(TEvent).Name, token.Id);
            return token;
        }

        /// <inheritdoc />
        public void Unsubscribe<TEvent>(ISubscriptionToken token) where TEvent : IEvent
        {
            if (token == null) throw new ArgumentNullException(nameof(token));

            lock (_lock)
            {
                if (!_subscriptions.TryGetValue(typeof(TEvent), out var subs))
                    return;

                subs.RemoveAll(s => s.TokenId == token.Id);
            }

            _logger?.LogDebug("Unsubscribed from {EventType} (token {TokenId})", typeof(TEvent).Name, token.Id);
        }

        /// <summary>
        /// Disposes the event bus and removes all subscriptions.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            lock (_lock)
            {
                _subscriptions.Clear();
            }
        }

        // -----------------------------------------------------------------------
        // Private helpers
        // -----------------------------------------------------------------------

        private abstract class WeakSubscription
        {
            public Guid TokenId { get; protected set; }

            /// <summary>
            /// Attempts to invoke the stored handler with the given event data.
            /// Returns <c>false</c> if the target has been garbage-collected.
            /// </summary>
            public abstract bool TryInvoke(IEvent eventData);
        }

        private sealed class WeakSubscription<TEvent> : WeakSubscription where TEvent : IEvent
        {
            private readonly WeakReference<Action<TEvent>> _weakHandler;

            public WeakSubscription(Guid tokenId, Action<TEvent> handler)
            {
                TokenId = tokenId;
                _weakHandler = new WeakReference<Action<TEvent>>(handler);
            }

            public override bool TryInvoke(IEvent eventData)
            {
                if (!_weakHandler.TryGetTarget(out var handler))
                    return false;
                handler((TEvent)eventData);
                return true;
            }
        }

        private sealed class SubscriptionToken<TEvent> : ISubscriptionToken where TEvent : IEvent
        {
            private readonly EventBus _bus;
            private bool _disposed;

            public Guid Id { get; } = Guid.NewGuid();

            public SubscriptionToken(EventBus bus)
            {
                _bus = bus;
            }

            public void Dispose()
            {
                if (_disposed) return;
                _disposed = true;
                _bus.Unsubscribe<TEvent>(this);
            }
        }
    }
}
