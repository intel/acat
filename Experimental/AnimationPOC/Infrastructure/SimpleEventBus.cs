////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// SimpleEventBus.cs
//
// Lightweight IEventBus implementation for the Animation POC.
// Uses strong references (suitable for controlled-lifetime POC scenarios).
// The production IEventBus in ACATCore uses weak references.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Experimental.AnimationPOC.Interfaces;
using System;
using System.Collections.Generic;

namespace ACAT.Experimental.AnimationPOC.Infrastructure
{
    /// <summary>
    /// Lightweight pub/sub event bus for the Animation POC.
    /// Uses strong references to handlers (not weak — the POC manages subscriptions explicitly).
    /// Thread-safe: subscribe/unsubscribe/publish are all synchronized.
    /// </summary>
    public class SimpleEventBus : IEventBus
    {
        private readonly object _lock = new object();
        private readonly Dictionary<Type, List<Delegate>> _handlers =
            new Dictionary<Type, List<Delegate>>();

        /// <inheritdoc/>
        public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            lock (_lock)
            {
                var key = typeof(TEvent);
                if (!_handlers.TryGetValue(key, out var list))
                {
                    list = new List<Delegate>();
                    _handlers[key] = list;
                }
                list.Add(handler);
            }
        }

        /// <inheritdoc/>
        public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            lock (_lock)
            {
                var key = typeof(TEvent);
                if (_handlers.TryGetValue(key, out var list))
                {
                    list.Remove(handler);
                }
            }
        }

        /// <inheritdoc/>
        public void Publish<TEvent>(TEvent @event) where TEvent : IEvent
        {
            if (@event == null) throw new ArgumentNullException(nameof(@event));

            List<Delegate> snapshot;
            lock (_lock)
            {
                var key = typeof(TEvent);
                if (!_handlers.TryGetValue(key, out var list) || list.Count == 0)
                    return;
                snapshot = new List<Delegate>(list);
            }

            foreach (var handler in snapshot)
            {
                ((Action<TEvent>)handler)(@event);
            }
        }
    }
}
