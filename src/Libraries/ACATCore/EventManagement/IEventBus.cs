////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// IEventBus.cs
//
// Interface for the pub/sub event bus used throughout ACAT.
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Core.EventManagement
{
    /// <summary>
    /// Defines the contract for the pub/sub event bus.
    /// Handlers are held via weak references so that subscribers that are
    /// garbage-collected do not cause memory leaks.
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// Subscribes <paramref name="handler"/> to receive events of type
        /// <typeparamref name="TEvent"/>.
        /// </summary>
        /// <typeparam name="TEvent">The event type to subscribe to.</typeparam>
        /// <param name="handler">The callback to invoke when the event is published.</param>
        void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent;

        /// <summary>
        /// Removes a previously registered <paramref name="handler"/> for
        /// <typeparamref name="TEvent"/>.  A no-op if the handler is not found.
        /// </summary>
        /// <typeparam name="TEvent">The event type to unsubscribe from.</typeparam>
        /// <param name="handler">The callback to remove.</param>
        void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent;

        /// <summary>
        /// Publishes <paramref name="event"/> to all live subscribers registered
        /// for <typeparamref name="TEvent"/>.  Dead (garbage-collected) subscriptions
        /// are pruned during publishing.
        /// </summary>
        /// <typeparam name="TEvent">The event type to publish.</typeparam>
        /// <param name="event">The event instance to dispatch.</param>
        void Publish<TEvent>(TEvent @event) where TEvent : IEvent;
    }
}
