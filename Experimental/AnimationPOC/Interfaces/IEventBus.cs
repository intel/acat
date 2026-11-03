////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// IEventBus.cs
//
// Minimal pub/sub event bus interface for the Animation POC.
// Mirrors the IEventBus contract from ACATCore.EventManagement without
// depending on that assembly.
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Experimental.AnimationPOC.Interfaces
{
    /// <summary>
    /// Marker interface that all events published through <see cref="IEventBus"/> must implement.
    /// </summary>
    public interface IEvent { }

    /// <summary>
    /// Minimal pub/sub event bus for the Animation POC.
    /// </summary>
    public interface IEventBus
    {
        /// <summary>Subscribes <paramref name="handler"/> to receive events of type <typeparamref name="TEvent"/>.</summary>
        void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent;

        /// <summary>Removes a previously registered handler. No-op if not found.</summary>
        void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent;

        /// <summary>Publishes <paramref name="event"/> to all live subscribers.</summary>
        void Publish<TEvent>(TEvent @event) where TEvent : IEvent;
    }
}
