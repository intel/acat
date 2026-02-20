////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// IRepository.cs
//
// Generic repository interface for data access abstraction.
//
////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

namespace ACAT.Core.DataAccess
{
    /// <summary>
    /// Generic repository interface that abstracts data access for a given entity type.
    /// Implementations provide the concrete storage strategy (XML file, JSON file,
    /// database, in-memory, etc.) while callers depend only on this interface.
    /// </summary>
    /// <typeparam name="TEntity">The entity type managed by this repository.</typeparam>
    /// <typeparam name="TKey">The type of the entity's unique identifier.</typeparam>
    public interface IRepository<TEntity, TKey> where TEntity : class
    {
        /// <summary>
        /// Returns the entity with the specified identifier, or <c>null</c> if not found.
        /// </summary>
        TEntity GetById(TKey id);

        /// <summary>
        /// Returns all entities managed by this repository.
        /// </summary>
        IReadOnlyList<TEntity> GetAll();

        /// <summary>
        /// Adds a new entity to the repository.
        /// </summary>
        void Add(TEntity entity);

        /// <summary>
        /// Updates an existing entity in the repository.
        /// </summary>
        void Update(TEntity entity);

        /// <summary>
        /// Removes the entity with the specified identifier from the repository.
        /// </summary>
        void Remove(TKey id);

        /// <summary>
        /// Persists all pending changes to the underlying storage.
        /// </summary>
        /// <returns><c>true</c> if the save succeeded; otherwise <c>false</c>.</returns>
        bool Save();
    }
}
