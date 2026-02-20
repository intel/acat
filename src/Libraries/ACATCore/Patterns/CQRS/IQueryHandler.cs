////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// IQueryHandler.cs
//
// Interface for query handlers in the CQRS pattern.
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Core.Patterns.CQRS
{
    /// <summary>
    /// Handles a query and returns a result.
    /// </summary>
    /// <typeparam name="TQuery">The query type this handler processes.</typeparam>
    /// <typeparam name="TResult">The type of data returned by the query.</typeparam>
    public interface IQueryHandler<in TQuery, out TResult> where TQuery : IQuery<TResult>
    {
        /// <summary>
        /// Executes the given query and returns the result.
        /// </summary>
        /// <param name="query">The query to execute.</param>
        /// <returns>The query result.</returns>
        TResult Handle(TQuery query);
    }
}
