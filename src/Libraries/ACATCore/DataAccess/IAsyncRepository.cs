////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// IAsyncRepository.cs
//
// Async variant of the generic repository interface for I/O-bound data access.
// Complements IRepository<T> with Task-returning overloads so callers can
// avoid blocking the UI thread during file operations.
//
////////////////////////////////////////////////////////////////////////////

using System.Threading;
using System.Threading.Tasks;

namespace ACAT.Core.DataAccess
{
    /// <summary>
    /// Async variant of the generic repository interface for loading and saving
    /// a single data entity identified by a string key (typically a file path).
    /// </summary>
    /// <remarks>
    /// Implementations must use <c>ConfigureAwait(false)</c> on all awaited calls
    /// to avoid <see cref="System.Threading.SynchronizationContext"/> deadlocks in
    /// WinForms / library code running on .NET Framework 4.8.1.
    /// </remarks>
    /// <typeparam name="T">The type of the data entity.</typeparam>
    public interface IAsyncRepository<T> where T : class
    {
        /// <summary>
        /// Asynchronously loads the entity from the specified location.
        /// </summary>
        /// <param name="key">Location identifier (e.g. file path).</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>The loaded entity, or <c>null</c> on failure.</returns>
        Task<T> LoadAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously saves the entity to the specified location.
        /// </summary>
        /// <param name="entity">The entity to save.</param>
        /// <param name="key">Location identifier (e.g. file path).</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns><c>true</c> on success; <c>false</c> otherwise.</returns>
        Task<bool> SaveAsync(T entity, string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously returns the default (factory-reset) entity.
        /// </summary>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>The default entity instance.</returns>
        Task<T> GetDefaultAsync(CancellationToken cancellationToken = default);
    }
}
