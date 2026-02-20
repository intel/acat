////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// IQuery.cs
//
// Marker interface for query objects in the CQRS pattern.
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Core.Patterns.CQRS
{
    /// <summary>
    /// Marker interface for query objects.
    /// A query reads state and returns a result without producing side effects.
    /// </summary>
    /// <typeparam name="TResult">The type of data returned by the query.</typeparam>
    public interface IQuery<out TResult> { }
}
