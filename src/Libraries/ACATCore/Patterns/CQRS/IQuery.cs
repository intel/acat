////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// IQuery.cs
//
// Marker interface for all CQRS queries.  A query represents a request to
// read system state and returns a typed result without causing any side
// effects.  Use ICommand for operations that change state.
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Core.Patterns.CQRS
{
    /// <summary>
    /// Marker interface for CQRS queries.
    /// </summary>
    /// <typeparam name="TResult">The type of data returned by the query.</typeparam>
    /// <remarks>
    /// Implement this interface on plain data-transfer objects (DTOs) that
    /// describe a read operation.  The corresponding handler is declared as
    /// <see cref="IQueryHandler{TQuery,TResult}"/>.
    /// </remarks>
    public interface IQuery<TResult>
    {
    }
}
