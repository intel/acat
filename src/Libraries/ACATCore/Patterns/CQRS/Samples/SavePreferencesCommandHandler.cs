////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// SavePreferencesCommandHandler.cs
//
// Async CQRS command handler that saves an XML preferences object to disk
// using PreferencesRepository<T>.SaveAsync, avoiding blocking the
// calling thread during file I/O.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.DataAccess;
using ACAT.Core.Utility;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace ACAT.Core.Patterns.CQRS.Samples
{
    /// <summary>
    /// Handles <see cref="SavePreferencesCommand{T}"/> by saving the preferences
    /// file asynchronously via <see cref="PreferencesRepository{T}.SaveAsync"/>.
    /// </summary>
    /// <typeparam name="T">Preferences type (XML-serializable reference type).</typeparam>
    public class SavePreferencesCommandHandler<T>
        : IAsyncCommandHandler<SavePreferencesCommand<T>>
        where T : class, new()
    {
        private readonly PreferencesRepository<T> _repository;

        /// <summary>
        /// Initialises a new <see cref="SavePreferencesCommandHandler{T}"/> using
        /// an optional logger.
        /// </summary>
        /// <param name="logger">Optional logger instance.</param>
        public SavePreferencesCommandHandler(ILogger logger = null)
        {
            _repository = new PreferencesRepository<T>(logger ?? LogManager.GetLogger<SavePreferencesCommandHandler<T>>());
        }

        /// <inheritdoc />
        public async Task HandleAsync(SavePreferencesCommand<T> command, CancellationToken cancellationToken = default)
        {
            await _repository.SaveAsync(command.Preferences, command.FilePath, cancellationToken).ConfigureAwait(false);
        }
    }
}
