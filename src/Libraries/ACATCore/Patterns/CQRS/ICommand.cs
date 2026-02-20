////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ICommand.cs
//
// Marker interface for command objects in the CQRS pattern.
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Core.Patterns.CQRS
{
    /// <summary>
    /// Marker interface for command objects.
    /// A command represents an intent to change state and does not return a value.
    /// </summary>
    public interface ICommand { }

    /// <summary>
    /// Marker interface for commands that return a result.
    /// </summary>
    /// <typeparam name="TResult">The type of result produced by the command.</typeparam>
    public interface ICommand<out TResult> { }
}
