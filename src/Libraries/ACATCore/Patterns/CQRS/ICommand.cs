////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ICommand.cs
//
// Marker interface for all CQRS commands.  A command represents an intent
// to change system state and carries all data required to perform that
// change.  Commands do not return a result value; use IQuery<TResult> for
// operations that need to return data.
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Core.Patterns.CQRS
{
    /// <summary>
    /// Marker interface for CQRS commands.
    /// </summary>
    /// <remarks>
    /// Implement this interface on plain data-transfer objects (DTOs) that
    /// describe a state-changing operation.  The corresponding handler is
    /// declared as <see cref="ICommandHandler{TCommand}"/>.
    /// </remarks>
    public interface ICommand
    {
    }
}
