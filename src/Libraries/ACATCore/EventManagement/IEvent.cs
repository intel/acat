////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// IEvent.cs
//
// Marker interface for all events published through the event bus.
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Core.EventManagement
{
    /// <summary>
    /// Marker interface for all events published through the event bus.
    /// All event payload types must implement this interface.
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// Gets the UTC timestamp at which the event was created.
        /// </summary>
        DateTime Timestamp { get; }
    }

    /// <summary>
    /// Represents a token that identifies a single event subscription.
    /// Dispose the token or pass it to <see cref="IEventBus.Unsubscribe{TEvent}"/>
    /// to cancel the subscription.
    /// </summary>
    public interface ISubscriptionToken : IDisposable
    {
        /// <summary>
        /// Gets a unique identifier for this subscription.
        /// </summary>
        Guid Id { get; }
    }

    /// <summary>
    /// Base class providing default <see cref="IEvent"/> implementation.
    /// Derive from this class for convenience when creating new event types.
    /// </summary>
    public abstract class EventBase : IEvent
    {
        /// <inheritdoc />
        public DateTime Timestamp { get; } = DateTime.UtcNow;
    }

    // -----------------------------------------------------------------------
    // Built-in ACAT event types
    // -----------------------------------------------------------------------

    /// <summary>Event raised when a panel becomes visible.</summary>
    public class PanelShownEvent : EventBase
    {
        /// <summary>Gets the name of the panel that was shown.</summary>
        public string PanelName { get; }

        /// <summary>Initializes a new <see cref="PanelShownEvent"/>.</summary>
        public PanelShownEvent(string panelName)
        {
            PanelName = panelName;
        }
    }

    /// <summary>Event raised when a panel is hidden.</summary>
    public class PanelHiddenEvent : EventBase
    {
        /// <summary>Gets the name of the panel that was hidden.</summary>
        public string PanelName { get; }

        /// <summary>Initializes a new <see cref="PanelHiddenEvent"/>.</summary>
        public PanelHiddenEvent(string panelName)
        {
            PanelName = panelName;
        }
    }

    /// <summary>Event raised when an actuator switch is activated.</summary>
    public class ActuatorSwitchEvent : EventBase
    {
        /// <summary>Gets the name of the switch that was activated.</summary>
        public string SwitchName { get; }

        /// <summary>Gets additional data associated with the switch activation.</summary>
        public string SwitchData { get; }

        /// <summary>Initializes a new <see cref="ActuatorSwitchEvent"/>.</summary>
        public ActuatorSwitchEvent(string switchName, string switchData = null)
        {
            SwitchName = switchName;
            SwitchData = switchData;
        }
    }

    /// <summary>Event raised when a configuration file is reloaded.</summary>
    public class ConfigurationReloadedEvent : EventBase
    {
        /// <summary>Gets the path of the configuration file that was reloaded.</summary>
        public string FilePath { get; }

        /// <summary>Gets a value indicating whether the reload succeeded.</summary>
        public bool Success { get; }

        /// <summary>Initializes a new <see cref="ConfigurationReloadedEvent"/>.</summary>
        public ConfigurationReloadedEvent(string filePath, bool success)
        {
            FilePath = filePath;
            Success = success;
        }
    }

    /// <summary>Event raised when the active agent context changes.</summary>
    public class AgentContextChangedEvent : EventBase
    {
        /// <summary>Gets the name of the new active context.</summary>
        public string ContextName { get; }

        /// <summary>Initializes a new <see cref="AgentContextChangedEvent"/>.</summary>
        public AgentContextChangedEvent(string contextName)
        {
            ContextName = contextName;
        }
    }
}
