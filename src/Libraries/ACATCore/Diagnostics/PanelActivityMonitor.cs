////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// PanelActivityMonitor.cs
//
// Monitors panel and actuator activity via EventBus subscriptions.
// Provides real-time logging of panel lifecycle events and switch activations.
// This demonstrates the EventBus pattern in action and provides useful diagnostics.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.EventManagement;
using Microsoft.Extensions.Logging;
using System;

namespace ACAT.Core.Diagnostics
{
    /// <summary>
    /// Monitors panel and actuator activity by subscribing to EventBus events.
    /// Provides real-time logging and diagnostics for panel lifecycle and switch activations.
    /// This is an example of the modern EventBus pattern replacing legacy delegate subscriptions.
    /// </summary>
    public class PanelActivityMonitor : IDisposable
    {
        private readonly IEventBus _eventBus;
        private readonly ILogger<PanelActivityMonitor> _logger;
        private bool _disposed;

        /// <summary>
        /// Statistics tracking
        /// </summary>
        private int _panelShowCount;
        private int _panelHideCount;
        private int _switchActivationCount;
        private int _configReloadCount;
        private int _agentChangeCount;
        private DateTime _startTime;

        /// <summary>
        /// Initializes a new instance of the PanelActivityMonitor.
        /// Subscribes to all relevant EventBus events.
        /// </summary>
        /// <param name="eventBus">The event bus to subscribe to</param>
        /// <param name="logger">Logger instance</param>
        public PanelActivityMonitor(IEventBus eventBus, ILogger<PanelActivityMonitor> logger)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _startTime = DateTime.UtcNow;

            // Subscribe to all events (NEW SYSTEM - EventBus pattern!)
            _eventBus.Subscribe<PanelShowEvent>(OnPanelShow);
            _eventBus.Subscribe<PanelHideEvent>(OnPanelHide);
            _eventBus.Subscribe<ActuatorSwitchActivatedEvent>(OnSwitchActivated);
            _eventBus.Subscribe<ConfigurationReloadEvent>(OnConfigReload);
            _eventBus.Subscribe<ConfigurationReloadFailedEvent>(OnConfigReloadFailed);
            _eventBus.Subscribe<AgentContextChangedEvent>(OnAgentChanged);

            _logger.LogInformation("📊 PanelActivityMonitor started - EventBus subscriptions active");
        }

        /// <summary>
        /// Gets the total number of panel show events
        /// </summary>
        public int PanelShowCount => _panelShowCount;

        /// <summary>
        /// Gets the total number of panel hide events
        /// </summary>
        public int PanelHideCount => _panelHideCount;

        /// <summary>
        /// Gets the total number of switch activation events
        /// </summary>
        public int SwitchActivationCount => _switchActivationCount;

        /// <summary>
        /// Gets the total number of configuration reload events
        /// </summary>
        public int ConfigReloadCount => _configReloadCount;

        /// <summary>
        /// Gets the total number of agent change events
        /// </summary>
        public int AgentChangeCount => _agentChangeCount;

        /// <summary>
        /// Gets the uptime of the monitor
        /// </summary>
        public TimeSpan Uptime => DateTime.UtcNow - _startTime;

        /// <summary>
        /// Event handler for panel show events
        /// </summary>
        private void OnPanelShow(PanelShowEvent evt)
        {
            _panelShowCount++;
            _logger.LogInformation("📊 [EventBus] Panel shown: {PanelClass} at {Timestamp}", 
                evt.PanelClass, evt.Timestamp.ToString("HH:mm:ss.fff"));
        }

        /// <summary>
        /// Event handler for panel hide events
        /// </summary>
        private void OnPanelHide(PanelHideEvent evt)
        {
            _panelHideCount++;
            _logger.LogInformation("📊 [EventBus] Panel hidden: {PanelClass} at {Timestamp}", 
                evt.PanelClass, evt.Timestamp.ToString("HH:mm:ss.fff"));
        }

        /// <summary>
        /// Event handler for switch activation events
        /// </summary>
        private void OnSwitchActivated(ActuatorSwitchActivatedEvent evt)
        {
            _switchActivationCount++;
            _logger.LogInformation("📊 [EventBus] Switch activated: {SwitchName} at {Timestamp}", 
                evt.SwitchName, evt.Timestamp.ToString("HH:mm:ss.fff"));
        }

        /// <summary>
        /// Event handler for configuration reload events
        /// </summary>
        private void OnConfigReload(ConfigurationReloadEvent evt)
        {
            _configReloadCount++;
            _logger.LogInformation("📊 [EventBus] Config reloaded: {ConfigPath} at {Timestamp}", 
                evt.ConfigPath, evt.Timestamp.ToString("HH:mm:ss.fff"));
        }

        /// <summary>
        /// Event handler for configuration reload failure events
        /// </summary>
        private void OnConfigReloadFailed(ConfigurationReloadFailedEvent evt)
        {
            _logger.LogWarning("📊 [EventBus] Config reload FAILED: {ConfigPath} - {Error} at {Timestamp}", 
                evt.ConfigPath, evt.ErrorMessage, evt.Timestamp.ToString("HH:mm:ss.fff"));
        }

        /// <summary>
        /// Event handler for agent context change events
        /// </summary>
        private void OnAgentChanged(AgentContextChangedEvent evt)
        {
            _agentChangeCount++;
            _logger.LogInformation("📊 [EventBus] Agent changed: {AgentName} at {Timestamp}", 
                evt.AgentName, evt.Timestamp.ToString("HH:mm:ss.fff"));
        }

        /// <summary>
        /// Logs current statistics
        /// </summary>
        public void LogStatistics()
        {
            _logger.LogInformation(
                "📊 Activity Statistics (Uptime: {Uptime:hh\\:mm\\:ss}):\n" +
                "  - Panels shown: {PanelShowCount}\n" +
                "  - Panels hidden: {PanelHideCount}\n" +
                "  - Switches activated: {SwitchActivationCount}\n" +
                "  - Config reloads: {ConfigReloadCount}\n" +
                "  - Agent changes: {AgentChangeCount}",
                Uptime, _panelShowCount, _panelHideCount, _switchActivationCount, 
                _configReloadCount, _agentChangeCount);
        }

        /// <summary>
        /// Disposes resources and unsubscribes from events
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _logger.LogInformation("📊 PanelActivityMonitor stopping...");
            LogStatistics();

            // Unsubscribe from all events
            _eventBus.Unsubscribe<PanelShowEvent>(OnPanelShow);
            _eventBus.Unsubscribe<PanelHideEvent>(OnPanelHide);
            _eventBus.Unsubscribe<ActuatorSwitchActivatedEvent>(OnSwitchActivated);
            _eventBus.Unsubscribe<ConfigurationReloadEvent>(OnConfigReload);
            _eventBus.Unsubscribe<ConfigurationReloadFailedEvent>(OnConfigReloadFailed);
            _eventBus.Unsubscribe<AgentContextChangedEvent>(OnAgentChanged);

            _logger.LogInformation("📊 PanelActivityMonitor stopped - EventBus subscriptions cleaned up");
            _disposed = true;
        }
    }
}
