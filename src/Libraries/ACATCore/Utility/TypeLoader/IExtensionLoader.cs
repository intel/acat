////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// IExtensionLoader.cs
//
// Interface for loading and instantiating plugin extensions using dependency
// injection.  Combines assembly scanning (via TypeLoader) with DI-aware
// object creation and optional service-container registration.
//
////////////////////////////////////////////////////////////////////////////

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace ACAT.Core.Utility.TypeLoader
{
    /// <summary>
    /// Defines the contract for loading plugin extensions with dependency-injection support.
    /// </summary>
    /// <typeparam name="TExtension">
    /// The plugin-extension interface type.  Must be a reference type that implements
    /// <see cref="IPluginExtension"/>.
    /// </typeparam>
    public interface IExtensionLoader<TExtension>
        where TExtension : class, IPluginExtension
    {
        /// <summary>
        /// Gets a read-only view of all extension types that have been discovered so far,
        /// keyed by their plugin GUID.
        /// </summary>
        IReadOnlyDictionary<Guid, Type> LoadedTypes { get; }

        /// <summary>
        /// Scans a single assembly for types that implement <typeparamref name="TExtension"/>
        /// and adds them to <see cref="LoadedTypes"/>.
        /// </summary>
        /// <param name="assemblyPath">Full path to the assembly file to scan.</param>
        /// <param name="firstOrDefault">
        /// When <see langword="true"/> (the default) only the first matching type is added.
        /// When <see langword="false"/> every matching type is added.
        /// </param>
        void LoadFromAssembly(string assemblyPath, bool firstOrDefault = true);

        /// <summary>
        /// Scans multiple assemblies for types that implement <typeparamref name="TExtension"/>.
        /// </summary>
        /// <param name="assemblyPaths">Paths to the assemblies to scan.</param>
        void LoadFromAssemblies(IEnumerable<string> assemblyPaths);

        /// <summary>
        /// Creates a DI-resolved instance of the extension identified by <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The GUID of the extension to instantiate.</param>
        /// <returns>
        /// The newly created extension, or <see langword="null"/> if the GUID is not found
        /// in <see cref="LoadedTypes"/> or instantiation fails.
        /// </returns>
        TExtension CreateInstance(Guid id);

        /// <summary>
        /// Creates DI-resolved instances for every type in <see cref="LoadedTypes"/>.
        /// Types that fail to instantiate are skipped (errors are logged).
        /// </summary>
        /// <returns>An enumerable of successfully created extension instances.</returns>
        IEnumerable<TExtension> CreateAllInstances();

        /// <summary>
        /// Registers all types currently in <see cref="LoadedTypes"/> with the supplied
        /// service collection so that they can be resolved through the DI container.
        /// </summary>
        /// <param name="services">The service collection to populate.</param>
        /// <param name="lifetime">
        /// The service lifetime to use for the registered types.
        /// Defaults to <see cref="ServiceLifetime.Transient"/>.
        /// </param>
        void RegisterExtensions(IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient);
    }
}
