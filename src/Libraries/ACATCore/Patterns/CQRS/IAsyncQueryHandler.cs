////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// IAsyncQueryHandler.cs
//
// Async variant of the generic CQRS query handler interface.
// Implement this interface when query evaluation involves I/O-bound work
// that should not block the calling thread.
//
////////////////////////////////////////////////////////////////////////////

using System.Threading;
using System.Threading.Tasks;

namespace ACAT.Core.Patterns.CQRS
{
    /// <summary>
    /// Asynchronously handles a CQRS query of type <typeparamref name="TQuery"/>
    /// and returns a result of type <typeparamref name="TResult"/>.
    /// </summary>
    /// <remarks>
    /// Implementations must use <c>ConfigureAwait(false)</c> on all awaited calls
    /// to avoid <see cref="System.Threading.SynchronizationContext"/> deadlocks in
    /// WinForms / library code running on .NET Framework 4.8.1.
    /// </remarks>
    /// <typeparam name="TQuery">
    /// The concrete query type, which must implement
    /// <see cref="IQuery{TResult}"/>.
    /// </typeparam>
    /// <typeparam name="TResult">The type of data returned by the query.</typeparam>
    public interface IAsyncQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        /// <summary>
        /// Asynchronously executes the supplied <paramref name="query"/> and
        /// returns the requested data without causing any side effects.
        /// </summary>
        /// <param name="query">The query to execute.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> that resolves to the result produced
        /// by evaluating the query.
        /// </returns>
        Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
    }
}
