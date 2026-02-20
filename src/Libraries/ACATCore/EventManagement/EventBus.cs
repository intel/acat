////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// EventBus.cs
//
// Thread-safe pub/sub event bus with weak-reference subscriber support.
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Reflection;

namespace ACAT.Core.EventManagement
{
    /// <summary>
    /// Thread-safe implementation of <see cref="IEventBus"/>.
    /// Each subscriber is stored as a weak reference to its target object so
    /// that subscribers which are garbage-collected do not prevent the event
    /// bus from being collected and do not receive further notifications.
    /// Dead subscriptions are pruned lazily during <see cref="Publish{TEvent}"/>.
    /// </summary>
    public class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<WeakHandlerBase>> _subscriptions =
            new Dictionary<Type, List<WeakHandlerBase>>();

        private readonly object _lock = new object();

        /// <inheritdoc/>
        public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            lock (_lock)
            {
                Type eventType = typeof(TEvent);
                if (!_subscriptions.TryGetValue(eventType, out List<WeakHandlerBase> handlers))
                {
                    handlers = new List<WeakHandlerBase>();
                    _subscriptions[eventType] = handlers;
                }
                handlers.Add(new WeakHandler<TEvent>(handler));
            }
        }

        /// <inheritdoc/>
        public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            lock (_lock)
            {
                Type eventType = typeof(TEvent);
                if (!_subscriptions.TryGetValue(eventType, out List<WeakHandlerBase> handlers))
                    return;

                handlers.RemoveAll(h =>
                    h is WeakHandler<TEvent> wh && wh.Matches(handler));
            }
        }

        /// <inheritdoc/>
        public void Publish<TEvent>(TEvent @event) where TEvent : IEvent
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            List<WeakHandlerBase> snapshot;
            lock (_lock)
            {
                Type eventType = typeof(TEvent);
                if (!_subscriptions.TryGetValue(eventType, out List<WeakHandlerBase> handlers))
                    return;

                snapshot = new List<WeakHandlerBase>(handlers);
            }

            var dead = new List<WeakHandlerBase>();
            foreach (WeakHandlerBase handler in snapshot)
            {
                if (!handler.TryInvoke(@event))
                    dead.Add(handler);
            }

            if (dead.Count > 0)
            {
                lock (_lock)
                {
                    Type eventType = typeof(TEvent);
                    if (_subscriptions.TryGetValue(eventType, out List<WeakHandlerBase> handlers))
                    {
                        foreach (WeakHandlerBase d in dead)
                            handlers.Remove(d);
                    }
                }
            }
        }

        // ----------------------------------------------------------------
        // Private helpers
        // ----------------------------------------------------------------

        /// <summary>
        /// Non-generic base so heterogeneous handler lists can be stored.
        /// </summary>
        private abstract class WeakHandlerBase
        {
            /// <summary>
            /// Attempts to invoke the handler with the given event.
            /// Returns <c>false</c> when the target has been garbage-collected.
            /// </summary>
            public abstract bool TryInvoke(object @event);
        }

        /// <summary>
        /// Holds a weak reference to the delegate's target object so that the
        /// subscriber is not kept alive solely by the event bus registration.
        /// For static handlers (no target) the subscription is always live.
        /// </summary>
        private class WeakHandler<TEvent> : WeakHandlerBase where TEvent : IEvent
        {
            private readonly WeakReference _targetRef;
            private readonly MethodInfo _method;

            public WeakHandler(Action<TEvent> handler)
            {
                _targetRef = handler.Target != null
                    ? new WeakReference(handler.Target)
                    : null;
                _method = handler.Method;
            }

            public override bool TryInvoke(object @event)
            {
                if (_targetRef == null)
                {
                    // Static method — always alive.
                    _method.Invoke(null, new object[] { @event });
                    return true;
                }

                object target = _targetRef.Target;
                if (target == null)
                    return false;

                _method.Invoke(target, new object[] { @event });
                return true;
            }

            /// <summary>
            /// Returns <c>true</c> when this handler wraps the same
            /// target/method pair as <paramref name="handler"/>.
            /// </summary>
            public bool Matches(Action<TEvent> handler)
            {
                if (_targetRef == null)
                    return handler.Target == null && handler.Method == _method;

                object target = _targetRef.Target;
                return target != null
                    && ReferenceEquals(target, handler.Target)
                    && handler.Method == _method;
            }
        }
    }
}
