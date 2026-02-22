////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.AgentManagement.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace ACAT.Core.AgentManagement
{
    /// <summary>
    /// Factory interface for creating application-agent (<see cref="IApplicationAgent"/>) instances.
    /// Supports both type-based and name-based instantiation with
    /// dependency injection for services such as loggers.
    /// </summary>
    public interface IAgentFactory
    {
        /// <summary>
        /// Creates an agent instance of the specified concrete type.
        /// </summary>
        /// <param name="agentType">
        /// The concrete type to instantiate. Must implement <see cref="IApplicationAgent"/>.
        /// </param>
        /// <returns>A new agent instance.</returns>
        IApplicationAgent Create(Type agentType);

        /// <summary>
        /// Creates an agent instance identified by its simple class name.
        /// The name is matched against types that implement <see cref="IApplicationAgent"/>
        /// in the currently loaded assemblies.
        /// </summary>
        /// <param name="agentName">
        /// The simple class name of the agent (e.g. "NotepadAgent").
        /// </param>
        /// <returns>A new agent instance.</returns>
        IApplicationAgent Create(string agentName);
    }

    /// <summary>
    /// Default implementation of <see cref="IAgentFactory"/>.
    /// Uses <see cref="ActivatorUtilities"/> so that constructor-injected
    /// services (e.g. <see cref="ILogger{T}"/>) are resolved from the DI container.
    /// </summary>
    public class AgentFactory : IAgentFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AgentFactory> _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="AgentFactory"/>.
        /// </summary>
        /// <param name="serviceProvider">The DI service provider.</param>
        /// <param name="logger">Optional logger for diagnostics.</param>
        public AgentFactory(IServiceProvider serviceProvider, ILogger<AgentFactory> logger = null)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger;
        }

        /// <inheritdoc/>
        public IApplicationAgent Create(Type agentType)
        {
            if (agentType == null)
                throw new ArgumentNullException(nameof(agentType));

            if (!typeof(IApplicationAgent).IsAssignableFrom(agentType))
                throw new ArgumentException(
                    $"Type '{agentType.FullName}' does not implement IApplicationAgent.",
                    nameof(agentType));

            try
            {
                _logger?.LogDebug("Creating agent of type {TypeName}", agentType.FullName);
                var instance = ActivatorUtilities.CreateInstance(_serviceProvider, agentType);
                return (IApplicationAgent)instance;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to create agent of type {TypeName}", agentType.FullName);
                throw;
            }
        }

        /// <inheritdoc/>
        public IApplicationAgent Create(string agentName)
        {
            if (string.IsNullOrWhiteSpace(agentName))
                throw new ArgumentNullException(nameof(agentName));

            Type agentType = FindAgentType(agentName);
            if (agentType == null)
                throw new InvalidOperationException(
                    $"No agent type found for name '{agentName}'.");

            return Create(agentType);
        }

        /// <summary>
        /// Searches the currently loaded assemblies for a non-abstract type
        /// that implements <see cref="IApplicationAgent"/> and whose simple class
        /// name matches <paramref name="agentName"/> (case-insensitive).
        /// </summary>
        private Type FindAgentType(string agentName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types;
                try { types = assembly.GetTypes(); }
                catch (Exception ex)
                {
                    _logger?.LogDebug(ex, "Could not load types from assembly {AssemblyName}", assembly.FullName);
                    continue;
                }

                foreach (var type in types)
                {
                    if (!type.IsAbstract && !type.IsInterface &&
                        typeof(IApplicationAgent).IsAssignableFrom(type) &&
                        string.Equals(type.Name, agentName, StringComparison.OrdinalIgnoreCase))
                    {
                        return type;
                    }
                }
            }

            return null;
        }
    }
}
