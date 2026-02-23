////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ExtensionLoader.cs
//
// Loads plugin extensions from assemblies and creates instances through the
// DI container.  Wraps TypeLoader<TExtension> for type discovery and uses
// ActivatorUtilities so that extension constructors receive any registered
// services automatically.
//
////////////////////////////////////////////////////////////////////////////

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace ACAT.Core.Utility.TypeLoader
{
    /// <summary>
    /// Loads plugin extensions from assemblies and instantiates them via
    /// dependency injection.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="ExtensionLoader{TExtension}"/> ties together the two existing
    /// primitives:
    /// <list type="bullet">
    ///   <item><see cref="TypeLoader{TInterface}"/> – scans assemblies and builds a
    ///   GUID → <see cref="Type"/> cache.</item>
    ///   <item><see cref="ActivatorUtilities"/> – constructs objects whose constructors
    ///   may declare services registered in the DI container.</item>
    /// </list>
    /// </para>
    /// <para>
    /// Typical usage:
    /// <code>
    /// var loader = new ExtensionLoader&lt;IMyExtension&gt;(serviceProvider);
    /// loader.LoadFromAssembly(path);
    /// var instances = loader.CreateAllInstances();
    /// </code>
    /// </para>
    /// <para>
    /// To support a scenario where extensions themselves need to be resolved through
    /// DI later (e.g., injected into other services), call
    /// <see cref="RegisterExtensions"/> with the application's
    /// <see cref="IServiceCollection"/> before building the container.
    /// </para>
    /// </remarks>
    /// <typeparam name="TExtension">
    /// The plugin-extension interface type.  Must be a reference type that implements
    /// <see cref="IPluginExtension"/>.
    /// </typeparam>
    public class ExtensionLoader<TExtension> : IExtensionLoader<TExtension>
        where TExtension : class, IPluginExtension
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TypeLoader<TExtension> _typeLoader;
        private readonly ILogger<ExtensionLoader<TExtension>> _logger;

        /// <summary>
        /// Initialises a new <see cref="ExtensionLoader{TExtension}"/> that uses
        /// <paramref name="serviceProvider"/> to resolve extension dependencies.
        /// </summary>
        /// <param name="serviceProvider">
        /// The DI service provider used when creating extension instances.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="serviceProvider"/> is <see langword="null"/>.
        /// </exception>
        public ExtensionLoader(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _typeLoader = new TypeLoader<TExtension>();
            _logger = serviceProvider.GetService<ILogger<ExtensionLoader<TExtension>>>();
        }

        /// <inheritdoc/>
        public IReadOnlyDictionary<Guid, Type> LoadedTypes => _typeLoader.LoadedTypes;

        /// <inheritdoc/>
        public void LoadFromAssembly(string assemblyPath, bool firstOrDefault = true)
        {
            _typeLoader.LoadFromAssembly(assemblyPath, firstOrDefault);
        }

        /// <inheritdoc/>
        public void LoadFromAssemblies(IEnumerable<string> assemblyPaths)
        {
            _typeLoader.LoadFromAssemblies(assemblyPaths);
        }

        /// <inheritdoc/>
        public TExtension CreateInstance(Guid id)
        {
            if (!_typeLoader.LoadedTypes.TryGetValue(id, out var type))
            {
                _logger?.LogWarning("No extension type found for ID {ExtensionId}", id);
                return null;
            }

            try
            {
                _logger?.LogDebug("Creating extension instance for {TypeName}", type.FullName);
                var instance = ActivatorUtilities.CreateInstance(_serviceProvider, type);

                if (instance is TExtension extension)
                {
                    _logger?.LogInformation("Successfully created extension: {ExtensionName}", type.Name);
                    return extension;
                }

                _logger?.LogWarning("Type {TypeName} does not implement {Interface}",
                    type.FullName, typeof(TExtension).FullName);
                return null;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to create extension instance for {TypeName}", type.FullName);
                return null;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<TExtension> CreateAllInstances()
        {
            var instances = new List<TExtension>();

            foreach (var kvp in _typeLoader.LoadedTypes)
            {
                var instance = CreateInstance(kvp.Key);
                if (instance != null)
                {
                    instances.Add(instance);
                }
            }

            return instances;
        }

        /// <inheritdoc/>
        public void RegisterExtensions(IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            foreach (var kvp in _typeLoader.LoadedTypes)
            {
                var extensionType = kvp.Value;

                // Register the concrete type under itself
                services.Add(new ServiceDescriptor(extensionType, extensionType, lifetime));

                // Also register under the TExtension interface so callers can resolve
                // IEnumerable<TExtension> or request TExtension directly
                services.Add(new ServiceDescriptor(typeof(TExtension), extensionType, lifetime));

                _logger?.LogDebug(
                    "Registered extension {TypeName} with lifetime {Lifetime}",
                    extensionType.FullName, lifetime);
            }
        }
    }
}
