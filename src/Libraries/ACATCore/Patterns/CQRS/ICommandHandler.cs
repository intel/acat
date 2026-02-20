////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ICommandHandler.cs
//
// Interface for command handlers in the CQRS pattern.
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Core.Patterns.CQRS
{
    /// <summary>
    /// Handles a command that does not return a result.
    /// </summary>
    /// <typeparam name="TCommand">The command type this handler processes.</typeparam>
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        /// <summary>
        /// Executes the given command.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        void Handle(TCommand command);
    }

    /// <summary>
    /// Handles a command that returns a result.
    /// </summary>
    /// <typeparam name="TCommand">The command type this handler processes.</typeparam>
    /// <typeparam name="TResult">The type of result produced.</typeparam>
    public interface ICommandHandler<in TCommand, out TResult> where TCommand : ICommand<TResult>
    {
        /// <summary>
        /// Executes the given command and returns a result.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <returns>The result of the command execution.</returns>
        TResult Handle(TCommand command);
    }
}
