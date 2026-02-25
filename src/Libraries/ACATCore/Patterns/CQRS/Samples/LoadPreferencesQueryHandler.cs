////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// LoadPreferencesQueryHandler.cs
//
// Async CQRS query handler that loads an XML preferences object from disk
// using PreferencesRepository<T>.LoadAsync, avoiding blocking the
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
    /// Handles <see cref="LoadPreferencesQuery{T}"/> by loading the preferences
    /// file asynchronously via <see cref="PreferencesRepository{T}.LoadAsync"/>.
    /// </summary>
    /// <typeparam name="T">Preferences type (XML-serializable reference type).</typeparam>
    public class LoadPreferencesQueryHandler<T>
        : IAsyncQueryHandler<LoadPreferencesQuery<T>, T>
        where T : class, new()
    {
        private readonly PreferencesRepository<T> _repository;

        /// <summary>
        /// Initialises a new <see cref="LoadPreferencesQueryHandler{T}"/> using
        /// an optional logger.
        /// </summary>
        /// <param name="logger">Optional logger instance.</param>
        public LoadPreferencesQueryHandler(ILogger logger = null)
        {
            _repository = new PreferencesRepository<T>(logger ?? LogManager.GetLogger<LoadPreferencesQueryHandler<T>>());
        }

        /// <inheritdoc />
        public Task<T> HandleAsync(LoadPreferencesQuery<T> query, CancellationToken cancellationToken = default)
        {
            return _repository.LoadAsync(query.FilePath, cancellationToken);
        }
    }
}
