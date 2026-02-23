////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.ActuatorManagement.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace ACAT.Core.ActuatorManagement
{
    /// <summary>
    /// Factory interface for creating actuator (<see cref="IActuator"/>) instances.
    /// Supports both type-based and name-based instantiation with
    /// dependency injection for services such as loggers.
    /// </summary>
    public interface IActuatorFactory
    {
        /// <summary>
        /// Creates an actuator instance of the specified concrete type.
        /// </summary>
        /// <param name="actuatorType">
        /// The concrete type to instantiate. Must implement <see cref="IActuator"/>.
        /// </param>
        /// <returns>A new actuator instance.</returns>
        IActuator Create(Type actuatorType);

        /// <summary>
        /// Creates an actuator instance identified by its simple class name.
        /// The name is matched against types that implement <see cref="IActuator"/>
        /// in the currently loaded assemblies.
        /// </summary>
        /// <param name="actuatorName">
        /// The simple class name of the actuator (e.g. "KeyboardActuator").
        /// </param>
        /// <returns>A new actuator instance.</returns>
        IActuator Create(string actuatorName);
    }

    /// <summary>
    /// Default implementation of <see cref="IActuatorFactory"/>.
    /// Uses <see cref="ActivatorUtilities"/> so that constructor-injected
    /// services (e.g. <see cref="ILogger{T}"/>) are resolved from the DI container.
    /// </summary>
    public class ActuatorFactory : IActuatorFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ActuatorFactory> _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="ActuatorFactory"/>.
        /// </summary>
        /// <param name="serviceProvider">The DI service provider.</param>
        /// <param name="logger">Optional logger for diagnostics.</param>
        public ActuatorFactory(IServiceProvider serviceProvider, ILogger<ActuatorFactory> logger = null)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger;
        }

        /// <inheritdoc/>
        public IActuator Create(Type actuatorType)
        {
            if (actuatorType == null)
                throw new ArgumentNullException(nameof(actuatorType));

            if (!typeof(IActuator).IsAssignableFrom(actuatorType))
                throw new ArgumentException(
                    $"Type '{actuatorType.FullName}' does not implement IActuator.",
                    nameof(actuatorType));

            try
            {
                _logger?.LogDebug("Creating actuator of type {TypeName}", actuatorType.FullName);
                var instance = ActivatorUtilities.CreateInstance(_serviceProvider, actuatorType);
                return (IActuator)instance;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to create actuator of type {TypeName}", actuatorType.FullName);
                throw;
            }
        }

        /// <inheritdoc/>
        public IActuator Create(string actuatorName)
        {
            if (string.IsNullOrWhiteSpace(actuatorName))
                throw new ArgumentNullException(nameof(actuatorName));

            Type actuatorType = FindActuatorType(actuatorName);
            if (actuatorType == null)
                throw new InvalidOperationException(
                    $"No actuator type found for name '{actuatorName}'.");

            return Create(actuatorType);
        }

        /// <summary>
        /// Searches the currently loaded assemblies for a non-abstract type
        /// that implements <see cref="IActuator"/> and whose simple class
        /// name matches <paramref name="actuatorName"/> (case-insensitive).
        /// </summary>
        private Type FindActuatorType(string actuatorName)
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
                        typeof(IActuator).IsAssignableFrom(type) &&
                        string.Equals(type.Name, actuatorName, StringComparison.OrdinalIgnoreCase))
                    {
                        return type;
                    }
                }
            }

            return null;
        }
    }
}
