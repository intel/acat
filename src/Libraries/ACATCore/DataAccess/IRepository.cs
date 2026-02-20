////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// IRepository.cs
//
// Generic repository interface for data access.
// Defines load and save operations for a single entity type.
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Core.DataAccess
{
    /// <summary>
    /// Generic repository interface for loading and saving a single data entity
    /// identified by a string key (typically a file path).
    /// </summary>
    /// <typeparam name="T">The type of the data entity.</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Loads the entity from the specified location.
        /// </summary>
        /// <param name="key">Location identifier (e.g. file path).</param>
        /// <returns>The loaded entity, or null on failure.</returns>
        T Load(string key);

        /// <summary>
        /// Saves the entity to the specified location.
        /// </summary>
        /// <param name="entity">The entity to save.</param>
        /// <param name="key">Location identifier (e.g. file path).</param>
        /// <returns>true on success; false otherwise.</returns>
        bool Save(T entity, string key);

        /// <summary>
        /// Returns the default (factory-reset) entity.
        /// </summary>
        T GetDefault();
    }
}
