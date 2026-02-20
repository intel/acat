////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ICommandHandler.cs
//
// Generic handler interface for CQRS commands.  Implement this interface to
// provide the logic that executes a specific command and applies the
// corresponding state change.
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Core.Patterns.CQRS
{
    /// <summary>
    /// Handles a CQRS command of type <typeparamref name="TCommand"/>.
    /// </summary>
    /// <typeparam name="TCommand">
    /// The concrete command type, which must implement <see cref="ICommand"/>.
    /// </typeparam>
    public interface ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        /// <summary>
        /// Executes the supplied <paramref name="command"/>, applying the
        /// requested state change.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        void Handle(TCommand command);
    }
}
