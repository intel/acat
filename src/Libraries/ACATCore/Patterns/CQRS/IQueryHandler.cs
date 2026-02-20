////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// IQueryHandler.cs
//
// Generic handler interface for CQRS queries.  Implement this interface to
// provide the logic that satisfies a specific query by reading (but never
// mutating) system state.
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Core.Patterns.CQRS
{
    /// <summary>
    /// Handles a CQRS query of type <typeparamref name="TQuery"/> and returns
    /// a result of type <typeparamref name="TResult"/>.
    /// </summary>
    /// <typeparam name="TQuery">
    /// The concrete query type, which must implement
    /// <see cref="IQuery{TResult}"/>.
    /// </typeparam>
    /// <typeparam name="TResult">The type of data returned by the query.</typeparam>
    public interface IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        /// <summary>
        /// Executes the supplied <paramref name="query"/> and returns the
        /// requested data without causing any side effects.
        /// </summary>
        /// <param name="query">The query to execute.</param>
        /// <returns>The result produced by evaluating the query.</returns>
        TResult Handle(TQuery query);
    }
}
