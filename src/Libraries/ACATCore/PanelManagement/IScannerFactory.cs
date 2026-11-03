////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.PanelManagement.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace ACAT.Core.PanelManagement
{
    /// <summary>
    /// Factory interface for creating scanner (IScannerPanel) instances.
    /// Supports both type-based and name-based instantiation with
    /// dependency injection for services such as loggers.
    /// </summary>
    public interface IScannerFactory
    {
        /// <summary>
        /// Creates a scanner instance of the specified concrete type.
        /// </summary>
        /// <param name="scannerType">
        /// The concrete type to instantiate. Must implement <see cref="IScannerPanel"/>.
        /// </param>
        /// <returns>A new scanner instance.</returns>
        IScannerPanel Create(Type scannerType);

        /// <summary>
        /// Creates a scanner instance identified by its panel-class name.
        /// The name is matched against the simple class name of types that
        /// implement <see cref="IScannerPanel"/> in the currently loaded assemblies.
        /// </summary>
        /// <param name="panelClass">
        /// The class name of the scanner (e.g. "AlphabetScanner").
        /// </param>
        /// <returns>A new scanner instance.</returns>
        IScannerPanel Create(string panelClass);
    }

    /// <summary>
    /// Default implementation of <see cref="IScannerFactory"/>.
    /// Uses <see cref="ActivatorUtilities"/> so that constructor-injected
    /// services (e.g. <see cref="ILogger{T}"/>) are resolved from the DI container.
    /// </summary>
    public class ScannerFactory : IScannerFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ScannerFactory> _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="ScannerFactory"/>.
        /// </summary>
        /// <param name="serviceProvider">The DI service provider.</param>
        /// <param name="logger">Optional logger for diagnostics.</param>
        public ScannerFactory(IServiceProvider serviceProvider, ILogger<ScannerFactory> logger = null)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger;
        }

        /// <inheritdoc/>
        public IScannerPanel Create(Type scannerType)
        {
            if (scannerType == null)
                throw new ArgumentNullException(nameof(scannerType));

            if (!typeof(IScannerPanel).IsAssignableFrom(scannerType))
                throw new ArgumentException(
                    $"Type '{scannerType.FullName}' does not implement IScannerPanel.",
                    nameof(scannerType));

            try
            {
                _logger?.LogDebug("Creating scanner of type {TypeName}", scannerType.FullName);
                var instance = ActivatorUtilities.CreateInstance(_serviceProvider, scannerType);
                return (IScannerPanel)instance;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to create scanner of type {TypeName}", scannerType.FullName);
                throw;
            }
        }

        /// <inheritdoc/>
        public IScannerPanel Create(string panelClass)
        {
            if (string.IsNullOrWhiteSpace(panelClass))
                throw new ArgumentNullException(nameof(panelClass));

            Type scannerType = FindScannerType(panelClass);
            if (scannerType == null)
                throw new InvalidOperationException(
                    $"No scanner type found for panel class '{panelClass}'.");

            return Create(scannerType);
        }

        /// <summary>
        /// Searches the currently loaded assemblies for a non-abstract type
        /// that implements <see cref="IScannerPanel"/> and whose simple class
        /// name matches <paramref name="panelClass"/> (case-insensitive).
        /// </summary>
        private Type FindScannerType(string panelClass)
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
                        typeof(IScannerPanel).IsAssignableFrom(type) &&
                        string.Equals(type.Name, panelClass, StringComparison.OrdinalIgnoreCase))
                    {
                        return type;
                    }
                }
            }

            return null;
        }
    }
}
