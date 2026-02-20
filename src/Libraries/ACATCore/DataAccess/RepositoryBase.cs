////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// RepositoryBase.cs
//
// Abstract base class for file-based repositories.
// Provides common logging and null-guard helpers.
//
////////////////////////////////////////////////////////////////////////////

using Microsoft.Extensions.Logging;

namespace ACAT.Core.DataAccess
{
    /// <summary>
    /// Abstract base class for file-based repository implementations.
    /// Derived classes implement the actual serialization strategy.
    /// </summary>
    /// <typeparam name="T">The type of the data entity.</typeparam>
    public abstract class RepositoryBase<T> : IRepository<T> where T : class
    {
        /// <summary>Logger for this repository instance.</summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// Initializes a new instance of <see cref="RepositoryBase{T}"/>.
        /// </summary>
        /// <param name="logger">Optional logger; when null a NullLogger is used.</param>
        protected RepositoryBase(ILogger logger = null)
        {
            Logger = logger ?? Utility.LogManager.GetLogger<RepositoryBase<T>>();
        }

        /// <inheritdoc/>
        public abstract T Load(string key);

        /// <inheritdoc/>
        public abstract bool Save(T entity, string key);

        /// <inheritdoc/>
        public abstract T GetDefault();
    }
}
