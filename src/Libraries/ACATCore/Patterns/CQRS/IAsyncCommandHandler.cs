////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// IAsyncCommandHandler.cs
//
// Async variant of the generic CQRS command handler interface.
// Implement this interface when the command execution involves I/O-bound
// work that should not block the calling thread.
//
////////////////////////////////////////////////////////////////////////////

using System.Threading;
using System.Threading.Tasks;

namespace ACAT.Core.Patterns.CQRS
{
    /// <summary>
    /// Asynchronously handles a CQRS command of type <typeparamref name="TCommand"/>.
    /// </summary>
    /// <remarks>
    /// Implementations must use <c>ConfigureAwait(false)</c> on all awaited calls
    /// to avoid <see cref="System.Threading.SynchronizationContext"/> deadlocks in
    /// WinForms / library code running on .NET Framework 4.8.1.
    /// </remarks>
    /// <typeparam name="TCommand">
    /// The concrete command type, which must implement <see cref="ICommand"/>.
    /// </typeparam>
    public interface IAsyncCommandHandler<TCommand>
        where TCommand : ICommand
    {
        /// <summary>
        /// Asynchronously executes the supplied <paramref name="command"/>,
        /// applying the requested state change.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>A <see cref="Task"/> that completes when the command is handled.</returns>
        Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
    }
}
