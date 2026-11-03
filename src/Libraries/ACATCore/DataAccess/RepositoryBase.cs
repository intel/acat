////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// RepositoryBase.cs
//
// Abstract base class for file-based repositories.
// Provides common logging and null-guard helpers.
//
////////////////////////////////////////////////////////////////////////////

using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace ACAT.Core.DataAccess
{
    /// <summary>
    /// Abstract base class for file-based repository implementations.
    /// Derived classes implement the actual serialization strategy.
    /// </summary>
    /// <typeparam name="T">The type of the data entity.</typeparam>
    public abstract class RepositoryBase<T> : IRepository<T>, IAsyncRepository<T> where T : class
    {
        /// <summary>Logger for this repository instance.</summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// Initializes a new instance of <see cref="RepositoryBase{T}"/>.
        /// </summary>
        /// <param name="logger">Optional logger; when null a NullLogger is used.</param>
        protected RepositoryBase(ILogger logger = null)
        {
            Logger = logger ?? Utility.LogManager.GetLogger<RepositoryBase<T>>();
        }

        /// <inheritdoc/>
        public abstract T Load(string key);

        /// <inheritdoc/>
        public abstract bool Save(T entity, string key);

        /// <inheritdoc/>
        public abstract T GetDefault();

        /// <inheritdoc/>
        /// <remarks>
        /// Default implementation offloads the synchronous <see cref="Load"/> call to a
        /// thread-pool thread via <see cref="Task.Run(System.Func{T})"/>.
        /// Override in derived classes when a fully-async implementation is available.
        /// Uses <c>ConfigureAwait(false)</c> to avoid SynchronizationContext deadlocks.
        /// </remarks>
        public virtual Task<T> LoadAsync(string key, CancellationToken cancellationToken = default)
        {
            return Task.Run(() => Load(key), cancellationToken);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Default implementation offloads the synchronous <see cref="Save"/> call to a
        /// thread-pool thread via <see cref="Task.Run(System.Func{bool})"/>.
        /// Override in derived classes when a fully-async implementation is available.
        /// Uses <c>ConfigureAwait(false)</c> to avoid SynchronizationContext deadlocks.
        /// </remarks>
        public virtual Task<bool> SaveAsync(T entity, string key, CancellationToken cancellationToken = default)
        {
            return Task.Run(() => Save(entity, key), cancellationToken);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Default implementation wraps the synchronous <see cref="GetDefault"/> in a
        /// completed task. Override if the default value requires async initialization.
        /// </remarks>
        public virtual Task<T> GetDefaultAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(GetDefault());
        }
    }
}
