////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// EventBase.cs
//
// Abstract base class for events published through the event bus.
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Core.EventManagement
{
    /// <summary>
    /// Abstract base class for all events published through <see cref="IEventBus"/>.
    /// Captures the UTC timestamp at the moment the event is created.
    /// </summary>
    public abstract class EventBase : IEvent
    {
        /// <summary>
        /// Initializes a new instance of <see cref="EventBase"/> and
        /// records the current UTC time as the event timestamp.
        /// </summary>
        protected EventBase()
        {
            Timestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Gets the UTC date and time at which this event was created.
        /// </summary>
        public DateTime Timestamp { get; }
    }
}
