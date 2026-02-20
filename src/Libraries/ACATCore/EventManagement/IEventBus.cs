////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// IEventBus.cs
//
// Interface for the event bus that provides publish/subscribe messaging.
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Core.EventManagement
{
    /// <summary>
    /// Interface for the event bus that enables decoupled publish/subscribe messaging.
    /// Subscribers register handlers for specific event types; publishers raise events
    /// without needing a direct reference to their consumers.
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// Publishes an event to all registered subscribers.
        /// </summary>
        /// <typeparam name="TEvent">The type of event to publish.</typeparam>
        /// <param name="eventData">The event data to send to subscribers.</param>
        void Publish<TEvent>(TEvent eventData) where TEvent : IEvent;

        /// <summary>
        /// Subscribes a handler to receive events of the specified type.
        /// The subscription is held with a weak reference to prevent memory leaks.
        /// </summary>
        /// <typeparam name="TEvent">The type of event to subscribe to.</typeparam>
        /// <param name="handler">The handler delegate invoked when the event is published.</param>
        /// <returns>A subscription token that can be passed to <see cref="Unsubscribe{TEvent}"/> to remove the subscription.</returns>
        ISubscriptionToken Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent;

        /// <summary>
        /// Removes a subscription so the handler no longer receives events.
        /// </summary>
        /// <typeparam name="TEvent">The type of event to unsubscribe from.</typeparam>
        /// <param name="token">The token returned by <see cref="Subscribe{TEvent}"/>.</param>
        void Unsubscribe<TEvent>(ISubscriptionToken token) where TEvent : IEvent;
    }
}
